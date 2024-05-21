# techchallenge-microservico-producao

Repositório relacionado a Fase 4 do techChallenge FIAP. Refatoração do projeto de totem em três microsserviços (Pedido, Pagamento e Produção);

Repositório bloqueado para push na main, é necessário abrir um PullRequest;

Este microsserviço tem como objetivo receber informações de pedidos em formato de mensagens vindas de uma fila do serviço de mensageria SQS da AWS, convertendo as mensagens em objetos do tipo pedido e realizando o cadastro na base de dados. Possibilitando assim a visualização dos pedidos gerados.

Estrutura
 - Banco de dados = SQL Server
 - Simulação de ambiente AWS = Localstack
 - Implementação de fila na AWS = SQS
 - Containers = Docker + Docker-Compose
 - Orquestração de containers = Kubernetes
 - Testes unitários e com BDD utilizando a extensão SpecFlow
 - Cobertura de código = SonarCloud
 - Pipeline = Github Actions
 - Deploy = Terraform 




![image](https://github.com/WillianViegas/techchallenge-microservico-producao/assets/58482678/de313559-ff08-4474-8f8d-2d1f98a3b1df)




Fluxograma:
https://www.figma.com/board/foY2Q9t6aj6Gzv9WK8actk/Documenta%C3%A7%C3%A3o-Sistema-DDD?node-id=0%3A1&t=oY6vBdqPodcM5LMR-1

Links para repositórios relacioados: 

- techchallenge-microservico-pedido (Microsserviço de Pedido):
  - https://github.com/WillianViegas/techchallenge-microservico-pedido
- techchallenge-microservico-pagamento (Microsserviço de Pagamento)
  - https://github.com/WillianViegas/techchallenge-microservico-patamento
- TechChallenge-LanchoneteTotem (Repositório com o projeto que originou os microsserviços e histórico das fases):
  - https://github.com/WillianViegas/TechChallenge-LanchoneteTotem
