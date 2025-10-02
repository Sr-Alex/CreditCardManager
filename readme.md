
# Credit Card Manager

Web API do projeto Credit Card Manager.
Uma solução simplificada para gerenciamento de faturas de cartões de crédito pessoais ou compartilhados.

Crie seus cartões, adicione os usuários que o utilizam e tenha os registros das compras efetuadas mantidas e exibidas de forma organizada.



## Funcionalidades

- Gerencie os usuários que podem visualizar seu cartão e registrar as compras efetuadas por eles.

- Gerencie seus cartões, visualize suas faturas e datas de expiração.


## Documentação da API

#### Gerenciamento de usuários:

```http
  /User/
```

| Função   | Método       | Descrição                           |
| :---------- | :--------- | :---------------------------------- |
| `/User/{id}` | `GET` | Acessar informações de usuário. | 
| `/User/` | `POST` | Criar novo usuário. | 
| `/User/Login` | `POST` | Efetuar login de usuário. | 

#### Gerenciamento de cartões de crédito:

```http
  /CreditCard/
```

| Função   | Método       | Descrição                                   |
| :---------- | :--------- | :------------------------------------------ |
| `/CreditCard/{userId}`      | `GET` | Obter cartões de crédito de um usuário |
| `/CreditCard/{id}/users`      | `GET` | Obter usuários de um cartão de crédito. |
| `/CreditCard/`      | `POST` | Criar novo cartão de crédito |
| `/CreditCard/`      | `DELETE` | Excluir cartão de crédito |


