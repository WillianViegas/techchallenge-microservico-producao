# techchallenge-microservico-producao

Repositório relacionado a Fase 4 e 5 do techChallenge FIAP. Refatoração do projeto de totem em três microsserviços (Pedido, Pagamento e Produção) + Utilização do Padrão SAGA;

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




Fluxograma (Contendo arquitetura e Justificativa da utilização do padrão SAGA, está na Fase 5):
https://www.figma.com/board/foY2Q9t6aj6Gzv9WK8actk/Documenta%C3%A7%C3%A3o-Sistema-DDD?node-id=0%3A1&t=oY6vBdqPodcM5LMR-1

Video explicando a estruturas
- Fase 4 (Fase Passada): https://youtu.be/-OZgHsUoLkM
- Fase 5 (Fase Atual): https://youtu.be/_8Dvd5Me59w

Link para os relatórios OWASP ZAP:
- Vulnerabilidades: https://fiap-docs.s3.amazonaws.com/OWASP+ZAP+Relatorios/Vulnerabilidades/MS-Producao.html
- Correções: https://fiap-docs.s3.amazonaws.com/OWASP+ZAP+Relatorios/Correcoes/MS-Producao.html

Links para repositórios relacionados: 

- techchallenge-microservico-pedido (Microsserviço de Pedido):
  - https://github.com/WillianViegas/techchallenge-microservico-pedido
- techchallenge-microservico-pagamento (Microsserviço de Pagamento)
  - https://github.com/WillianViegas/techchallenge-microservico-pagamento
- TechChallenge-LanchoneteTotem (Repositório com o projeto que originou os microsserviços e histórico das fases):
  - https://github.com/WillianViegas/TechChallenge-LanchoneteTotem
- MS-Cancelamento-Dados (Microsserviço de solicitação de exclusão de dados):
  - https://github.com/WillianViegas/techchallenge-microservico-cancelamento-dados
 

## Rodando ambiente com Docker

### Pré-Requisitos
* Possuir o docker instalado:
    https://www.docker.com/products/docker-desktop/

Acesse o diretório em que o repositório foi clonado através do terminal e
execute os comandos:
 - `docker-compose build` para compilar imagens, criar containers etc.
 - `docker-compose up` para criar os containers do banco de dados e do projeto

### Iniciando e finalizando containers
Para inicializar execute o comando `docker-compose start` e
para finalizar `docker-compose stop`

Lembrando que se você for rodar pelo visual studio fica bem mais simplificado, basta estar com o docker desktop aberto na máquina e escolher a opção abaixo:

![image](https://github.com/user-attachments/assets/5c29e590-a38d-4090-8d2a-6e745e583f46)


