using Amazon.SQS.Model;
using Amazon.SQS;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using techchallenge_microservico_producao.Repositories.Interfaces;
using techchallenge_microservico_producao.Models;
using Amazon.SQS.ExtendedClient;
using System.Net;
using techchallenge_microservico_pedido.Models;
using Microsoft.EntityFrameworkCore;
using Infra.DatabaseConfig;
using Microsoft.Data.SqlClient;

namespace Infra.SQS
{
    public class SqsListenerService : BackgroundService, IHostedService
    {
        private readonly ILogger<SqsListenerService> _logger;
        private readonly bool _useLocalStack;
        private AmazonSQSClient _sqsClient;
        private readonly string _queueUrlRead;
        private readonly string _queueName;
        private readonly string _queueListenerName;
        private readonly bool _criarFila;
        private readonly bool _enviarMensagem;
        private readonly string _bucketName;
        private IAmazonSQS _amazonSQS;
        private IAmazonS3 _amazonS3;
        private readonly ISQSConfiguration _sqsConfiguration;
        private readonly IDbContextFactory<EFDbconfig> _myDbContextFactory;
        private readonly string _sqlConnectionString;

        public SqsListenerService(ILogger<SqsListenerService> logger, IConfiguration config, IAmazonS3 s3, IAmazonSQS sqs, ISQSConfiguration sqsConfiguration, IDbContextFactory<EFDbconfig> myDbContextFactory)
        {
            var useLocalStack = config.GetSection("SQSConfig").GetSection("useLocalStack").Value;
            var criarFila = config.GetSection("SQSConfig").GetSection("CreateTestQueue").Value;
            var enviarMensagem = config.GetSection("SQSConfig").GetSection("SendTestMessage").Value;
            _myDbContextFactory = myDbContextFactory;

            _logger = logger;
            _useLocalStack = Convert.ToBoolean(useLocalStack);
            _criarFila = Convert.ToBoolean(criarFila);
            _enviarMensagem = Convert.ToBoolean(enviarMensagem);
            _bucketName = config.GetSection("SQSExtendedClient").GetSection("S3Bucket").Value;
            _amazonS3 = s3;
            _queueUrlRead = config.GetSection("QueueUrl").Value;
            _queueName = config.GetSection("SQSConfig").GetSection("TestQueueName").Value;
            _amazonSQS = sqs;
            _sqsConfiguration = sqsConfiguration;
            _sqlConnectionString = config.GetSection("ConnectionStrings").GetSection("MyAppCs").Value;
            _sqsClient = new AmazonSQSClient(Amazon.RegionEndpoint.GetBySystemName(
                    string.IsNullOrEmpty(Environment.GetEnvironmentVariable("MY_SECRET")) || Environment.GetEnvironmentVariable("MY_SECRET").Equals("{MY_SECRET}")
                    ? "us-east-1"
                    : Environment.GetEnvironmentVariable("MY_SECRET")));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                if (_useLocalStack)
                {
                    var configSQS = new AmazonSQSExtendedClient(_amazonSQS, new ExtendedClientConfiguration().WithLargePayloadSupportEnabled(_amazonS3, _bucketName));

                    if (_criarFila)
                        await _sqsConfiguration.CreateMessageInQueueWithStatusASyncLocalStack(configSQS, _queueName);

                    if (_enviarMensagem)
                        await _sqsConfiguration.SendTestMessageAsyncLocalStack(_queueUrlRead, configSQS);
                }

                while (!stoppingToken.IsCancellationRequested)
                {
                    var receiveMessageRequest = new ReceiveMessageRequest
                    {
                        QueueUrl = _queueUrlRead,
                        WaitTimeSeconds = 10,
                        AttributeNames = new List<string> { "ApproximateReceiveCount" },
                        MessageAttributeNames = new List<string> { "All" },
                        MaxNumberOfMessages = 10
                    };

                    var receiveMessageResponse = await _amazonSQS.ReceiveMessageAsync(receiveMessageRequest, stoppingToken);

                    foreach (var message in receiveMessageResponse.Messages)
                    {
                        using (EFDbconfig dbContext = _myDbContextFactory.CreateDbContext())
                        {
                            using (var transaction = dbContext.Database.BeginTransaction())
                            {
                                try
                                {
                                    _logger.LogInformation($"Received message: {message.Body}");

                                    var obj = _sqsConfiguration.TratarMessage(message.Body);

                                    if (obj == null)
                                        continue;

                                    var pedido = await CreatePedido(obj, dbContext);
                                    Console.WriteLine(message.Body);
                                    _logger.LogInformation(message.Body);

                                    var deleteMessageRequest = new DeleteMessageRequest
                                    {
                                        QueueUrl = _queueUrlRead,
                                        ReceiptHandle = message.ReceiptHandle
                                    };


                                    await _amazonSQS.DeleteMessageAsync(deleteMessageRequest, stoppingToken);
                                    await transaction.CommitAsync();
                                }
                                catch
                                {
                                    try
                                    {
                                        await transaction.RollbackAsync();
                                    }
                                    catch (Exception rollbackEx)
                                    {
                                        _logger.LogError("Erro ao desfazer a transação: " + rollbackEx.Message);
                                    }
                                }
                            }
                        }

                    }
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }
            catch
            {
                _logger.LogError("Erro ao desfazer a transação");
            }
        }

        public async Task<Pedido> CreatePedido(Pedido pedido, EFDbconfig dbContext)
        {
            var numeroPedido = dbContext.Pedidos
             .Include(pedido => pedido.Produtos)
             .Include(pedido => pedido.Usuario)
             .Include(pedido => pedido.Pagamento)
             .ToList();

            if (pedido.Pagamento != null)
            {
                pedido.Pagamento.Id = Guid.NewGuid().ToString();
            }

            var novoPedido = new Pedido
            {
                Produtos = pedido.Produtos,
                Total = pedido.Produtos.Sum(x => x.Preco),
                Status = 0,
                DataCriacao = DateTime.Now,
                Numero = numeroPedido.Count + 1,
                Usuario = pedido.Usuario,
                IdCarrinho = pedido.IdCarrinho,
                IdPedidoOrigem = pedido.Id,
                Pagamento = pedido.Pagamento
            };


            dbContext.Pedidos.Add(pedido);
            await dbContext.SaveChangesAsync();

            return novoPedido;
        }

    }
}
