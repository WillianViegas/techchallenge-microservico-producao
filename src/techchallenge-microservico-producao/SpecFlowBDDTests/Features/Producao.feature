Feature: Producao

@mytag
Scenario: GetAllPedidos
	Given Que devo exibir todos os pedidos
	When For solicitado buscar todos os pedidos
	Then Retorno uma lista com os pedidos


@mytag
Scenario: UpdateStatusPedido
	Given Que preciso atualizar o status de um pedido
	When Receber um novo status e o pedido para ser atualizado
	Then Atualizo o status do pedido