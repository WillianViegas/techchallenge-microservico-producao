using Amazon.S3;
using Amazon.SQS;
using Amazon.SQS.ExtendedClient;
using Amazon.SQS.Model;
using Azure;
using Domain.Enum;
using Infra.DatabaseConfig;
using Infra.SQS;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using techchallenge_microservico_producao.Models;
using techchallenge_microservico_producao.Repositories.Interfaces;
using techchallenge_microservico_producao.Services.Interfaces;

namespace techchallenge_microservico_producao.Services
{
    public class ProducaoService : IProducaoService
    {
        private readonly IProducaoRepository _pedidoRepository;
        private readonly ILogger _log;
        private readonly bool _useLocalStack;
        private AmazonSQSClient _sqs;
        private readonly string _queueUrl;
        private readonly bool _criarFila;
        private readonly bool _enviarMensagem;
        private readonly string _queueName;
        private readonly string _bucketName;
        private IAmazonSQS _amazonSQS;
        private IAmazonS3 _amazonS3;
        private readonly ISQSConfiguration _sQSConfiguration;
        private readonly string _sqlConnectionString;
        private readonly IDbContextFactory<EFDbconfig> _myDbContextFactory;

        public ProducaoService(IProducaoRepository producaoRepository, ILogger<Pedido> log, IConfiguration config, IAmazonS3 s3, IAmazonSQS sqs, ISQSConfiguration sqsConfiguration, IDbContextFactory<EFDbconfig> myDbContextFactory)
        {
            var criarFila = config.GetSection("SQSConfig").GetSection("CreateTestQueue").Value;
            var enviarMensagem = config.GetSection("SQSConfig").GetSection("SendTestMessage").Value;
            var useLocalStack = config.GetSection("SQSConfig").GetSection("useLocalStack").Value;

            _myDbContextFactory = myDbContextFactory;
            _sQSConfiguration = sqsConfiguration;
            _log = log;
            _pedidoRepository = producaoRepository;
            _useLocalStack = Convert.ToBoolean(useLocalStack);
            _criarFila = Convert.ToBoolean(criarFila);
            _queueUrl = config.GetSection("QueueUrl").Value;
            _enviarMensagem = Convert.ToBoolean(enviarMensagem);
            _queueName = config.GetSection("SQSConfig").GetSection("TestQueueName").Value;
            _bucketName = config.GetSection("SQSExtendedClient").GetSection("S3Bucket").Value;
            _amazonS3 = s3;
            _amazonSQS = sqs;
            _sqlConnectionString = config.GetSection("ConnectionStrings").GetSection("MyAppCs").Value;
            _sqs = new AmazonSQSClient(Amazon.RegionEndpoint.GetBySystemName(
                    string.IsNullOrEmpty(Environment.GetEnvironmentVariable("MY_SECRET")) || Environment.GetEnvironmentVariable("MY_SECRET").Equals("{MY_SECRET}")
                    ? "us-east-1"
                    : Environment.GetEnvironmentVariable("MY_SECRET")));
        }

        public async Task<IList<Pedido>> GetAllPedidos()
        {
            try
            {
                var listaPedidos = await _pedidoRepository.GetAllPedidos();
                var listaPedidosFiltrados = listaPedidos.Where(x => x.Status == EPedidoStatus.EmPreparo || x.Status == EPedidoStatus.Pronto || x.Status == EPedidoStatus.Recebido)
                    .OrderBy(n => n.Status == EPedidoStatus.Recebido)
                    .ThenBy(n => n.Status == EPedidoStatus.EmPreparo)
                    .ThenBy(n => n.Status == EPedidoStatus.Pronto).ToList();

                return listaPedidosFiltrados;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<Pedido> GetPedidoByIdOrigem(string id)
        {
            try
            {
                return await _pedidoRepository.GetPedidoByIdOrigem(id);
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<Pedido> CreatePedido(Pedido pedido)
        {
            try
            {
                var numeroPedido = await _pedidoRepository.GetAllPedidos();

                var novoPedido = new Pedido
                {
                    Id = Guid.NewGuid().ToString(),
                    Produtos = pedido.Produtos,
                    Total = pedido.Produtos.Sum(x => x.Preco),
                    Status = 0,
                    DataCriacao = DateTime.Now,
                    Numero = pedido.Numero,
                    Usuario = pedido.Usuario,
                    IdCarrinho = pedido.IdCarrinho,
                    IdPedidoOrigem = pedido.IdPedidoOrigem,
                    Pagamento = pedido.Pagamento
                };

                await _pedidoRepository.CreatePedido(novoPedido);

                return novoPedido;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw new Exception(ex.Message);
            }

        }

        public async Task UpdateStatusPedido(string id, int status, Pedido pedido)
        {
            try
            {
                pedido.Status = (EPedidoStatus)status;
                await _pedidoRepository.UpdatePedido(id, pedido);
                _log.LogInformation($"Pedido atualizado id: {id}, status: {pedido.Status.ToString()}");
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        async Task EnviarMessageSQS(string messageJson)
        {
            if (!_useLocalStack)
            {
                _sqs = await _sQSConfiguration.ConfigurarSQS();

                await _sQSConfiguration.EnviarParaSQS(messageJson, _sqs);
            }
            else
            {
                var configSQS = new AmazonSQSExtendedClient(_amazonSQS, new ExtendedClientConfiguration().WithLargePayloadSupportEnabled(_amazonS3, _bucketName));

                if (_criarFila)
                    await _sQSConfiguration.CreateMessageInQueueWithStatusASyncLocalStack(configSQS, _queueName);

                if (_enviarMensagem)
                    await _sQSConfiguration.SendTestMessageAsyncLocalStack(_queueUrl, configSQS);

                await configSQS.SendMessageAsync(_queueUrl, messageJson);
            }
        }

        public async Task RegistrarPedidos()
        {
            try
            {
                var configSQS = new AmazonSQSExtendedClient(_amazonSQS, new ExtendedClientConfiguration().WithLargePayloadSupportEnabled(_amazonS3, _bucketName));
                await _sQSConfiguration.SendTestMessageAsyncLocalStack(_queueUrl, configSQS);
                _log.LogDebug("Reading queue...");

                var response = await _amazonSQS.ReceiveMessageAsync(new ReceiveMessageRequest
                {
                    QueueUrl = _queueUrl,
                    WaitTimeSeconds = 10,
                    AttributeNames = new List<string> { "ApproximateReceiveCount" },
                    MessageAttributeNames = new List<string> { "All" },
                    MaxNumberOfMessages = 10
                });

                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    _log.LogError($"Error creating the queue: {_queueUrl}!");
                    throw new AmazonSQSException($"Failed to GetMessages for queue {_queueUrl}. Response: {response.HttpStatusCode}");
                }

                foreach (var message in response.Messages)
                {
                    try
                    {
                        var obj = _sQSConfiguration.TratarMessage(message.Body);

                        if (obj == null)
                            continue;

                        //chamar confirmar pedido
                        var pedido = await CreatePedido(obj);
                        Console.WriteLine(message.Body);
                        _log.LogInformation(message.Body);

                        await _amazonSQS.DeleteMessageAsync(_queueUrl, message.ReceiptHandle);
                        _log.LogInformation($"Message deleted");
                    }
                    catch (Exception ex)
                    {
                        _log.LogError(ex, $"Error processing messages for queue {_queueUrl}! Message: {message.Body}");
                    }
                }
            }
            catch
            {
                _log.LogError($"Erro ao consultar fila: {_queueUrl}");
            }
        }
    }
}
