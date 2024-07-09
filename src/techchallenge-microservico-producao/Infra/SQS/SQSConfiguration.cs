﻿using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Amazon.SQS;
using Amazon.SQS.ExtendedClient;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using techchallenge_microservico_pedido.Models;
using techchallenge_microservico_producao.Models;

namespace Infra.SQS
{
    public class SQSConfiguration : ISQSConfiguration
    {
        private readonly ILogger<SQSConfiguration> _logger;

        public SQSConfiguration(ILogger<SQSConfiguration> log)
        {
            _logger = log;
        }
        public async Task<AmazonSQSClient> ConfigurarSQS()
        {
            using (var secretsManagerClient = new AmazonSecretsManagerClient())
            {
                var secretName = Environment.GetEnvironmentVariable("MY_SECRET");
                var getSecretValueRequest = new GetSecretValueRequest
                {
                    SecretId = secretName
                };

                var getSecretValueResponse = await secretsManagerClient.GetSecretValueAsync(getSecretValueRequest);
                var secretString = getSecretValueResponse.SecretString;

                var sqsConnectionDetails = ParseSecretString(secretString);

                var sqsConfig = new AmazonSQSConfig
                {
                    RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(sqsConnectionDetails.Region)
                };

                var sqsClient = new AmazonSQSClient(sqsConnectionDetails.AccessKeyId, sqsConnectionDetails.SecretAccessKey, sqsConfig);
                
                return sqsClient;
            }
        }

        private SqsConnectionDetails ParseSecretString(string secretString)
        {
            return JsonConvert.DeserializeObject<SqsConnectionDetails>(secretString);
        }

        public async Task EnviarParaSQS(string jsonMessage, AmazonSQSClient sqsclient)
        {
            try
            {
                var listQueueResponse = await sqsclient.ListQueuesAsync(new ListQueuesRequest());
                foreach (var queueUrl in listQueueResponse.QueueUrls)
                {
                    Console.WriteLine($"SQS Queue URL: {queueUrl}");
                    await sqsclient.SendMessageAsync(queueUrl, jsonMessage);
                }
            }
            catch
            {
                //logar
            }
          
        }

        public Pedido? TratarMessage(string body)
        {
            var obj = new Pedido();
            try
            {
                obj = Newtonsoft.Json.JsonConvert.DeserializeObject<Pedido>(body);

                if (string.IsNullOrEmpty(obj.Id))
                {
                    _logger.LogWarning($"TratarMessage: objeto não tinha id. Body: {body}");
                    return null;
                }

            }
            catch
            {
                _logger.LogError($"TratarMessage: erro ao deserializar json. Body: {body}");
                return null;
            }

            return obj;
        }

        public async Task CreateMessageInQueueWithStatusASyncLocalStack(AmazonSQSExtendedClient sqs, string queueName)
        {
            var responseQueue = await sqs.CreateQueueAsync(new CreateQueueRequest(queueName));

            if (responseQueue.HttpStatusCode != HttpStatusCode.OK)
            {
                var erro = $"Failed to CreateQueue for queue {queueName}. Response: {responseQueue.HttpStatusCode}";
                //log.LogError(erro);
                throw new AmazonSQSException(erro);
            }
        }

        public async Task SendTestMessageAsyncLocalStack(string queue, AmazonSQSExtendedClient sqs)
        {
            var messageBody = GerarMessageBody();
            var jsonObj = Newtonsoft.Json.JsonConvert.SerializeObject(messageBody);
            await sqs.SendMessageAsync(queue, jsonObj);
        }


        public MessageBody GerarMessageBody()
        {
            var messageBody = new MessageBody();
            messageBody.Id = "668765228949bcd28073e197";
            messageBody.Numero = 0;

            var produto = new Produto()
            {
                Id = "65a315a4db1f522d916d935a",
                Nome = "Hamburguer especial da casa",
                Descricao = "Hamburguer artesanal da casa com maionese caseira e molho secreto",
                Preco = 35.99m,
                CategoriaId = "65a315a4db1f522d916d9357"
            };

            messageBody.Produtos = new List<Produto>() { produto };

            messageBody.Usuario = new Usuario()
            {
                Id = "65a315a4db1f522d916d9355",
                Nome = "Marcos",
                Email = "marcao@gmail.com",
                CPF = "65139370000",
                Tipo = null,
                Senha = null
            };

            messageBody.Total = 35.99;
            messageBody.Status = "1";
            messageBody.DataTransacao = DateTime.Now;
            messageBody.IdCarrinho = "";
            messageBody.Pagamento = new Pagamento()
            {
                Id = Guid.NewGuid().ToString(),
                Tipo = 0,
                QRCodeUrl = "www.qrcodeurlteste.com.br",
                Bandeira = null,
                OrdemDePagamento = Guid.NewGuid().ToString()
            };

            messageBody.IdPedidoOrigem = "668763894d7e2544b98492cb";

            return messageBody;
        }

        class SqsConnectionDetails
        {
            public string AccessKeyId { get; set; }
            public string SecretAccessKey { get; set; }
            public string Region { get; set; }
        }
    }
}
