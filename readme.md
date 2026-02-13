# Credit Card Manager

Web API para gerenciamento de faturas de cartões de crédito pessoais ou compartilhados.

Crie seus cartões, adicione os usuários que o utilizam, controle as dívidas/despesas efetuadas e tenha os registros mantidos e exibidos de forma organizada.

## Funcionalidades

- **Gerenciamento de Usuários**: Cadastro, autenticação e controle de acesso com tokens JWT;
- **Gerenciamento de Cartões**: Crie e gerencie seus cartões de crédito, visualize faturas e limites;
- **Compartilhamento de Cartões**: Adicione múltiplos usuários a um cartão de crédito;
- **Controle de Dívidas**: Registre despesas nos cartões, acompanhe o status de pagamento e marque como pagas;
- **Autorização em Cascata**: Apenas proprietários de cartões podem gerenciar usuários e dívidas.

## Documentação da API

### Autenticação

A maioria dos endpoints requer autenticação via JWT. O token deve ser enviado no header `Authorization`.

---

### Gerenciamento de Usuários

#### `/user`

| Método   | Endpoint      | Descrição                                  | Autenticação |
| :------- | :------------ | :----------------------------------------- | :----------- |
| `GET`    | `/user`       | Obter lista de todos os usuários           | ✓ Requerida  |
| `GET`    | `/user/{id}`  | Obter informações de um usuário específico | -            |
| `POST`   | `/user`       | Criar novo usuário                         | -            |
| `POST`   | `/user/Login` | Executar login e obter token               | -            |
| `DELETE` | `/user`       | Deletar conta do usuário autenticado       | ✓ Requerida  |

---

### Gerenciamento de Cartões de Crédito

#### `/creditcard`

| Método   | Endpoint                             | Descrição                                       | Autenticação |
| :------- | :----------------------------------- | :---------------------------------------------- | :----------- |
| `GET`    | `/creditcard`                        | Obter cartões de crédito do usuário autenticado | ✓ Requerida  |
| `GET`    | `/creditcard/details/{cardId}`       | Obter detalhes de um cartão específico          | -            |
| `POST`   | `/creditcard`                        | Criar novo cartão de crédito                    | ✓ Requerida  |
| `DELETE` | `/creditcard/{id}`                   | Deletar cartão de crédito                       | ✓ Requerida  |
| `GET`    | `/creditcard/details/{cardId}/users` | Obter usuários vinculados ao cartão             | ✓ Requerida  |
| `POST`   | `/creditcard/details/{cardId}/users` | Adicionar usuário ao cartão                     | ✓ Requerida  |
| `DELETE` | `/creditcard/details/{cardId}/users` | Remover usuário do cartão                       | ✓ Requerida  |

---

### Gerenciamento de Dívidas/Despesas

#### `/debt`

| Método   | Endpoint                | Descrição                                  | Autenticação |
| :------- | :---------------------- | :----------------------------------------- | :----------- |
| `GET`    | `/debt/{id}`            | Obter informações de uma dívida específica | -            |
| `GET`    | `/debt?cardId={cardId}` | Obter todas as dívidas de um cartão        | ✓ Requerida  |
| `POST`   | `/debt`                 | Criar nova dívida/despesa                  | ✓ Requerida  |
| `PUT`    | `/debt/{debtId}`        | Atualizar informações de uma dívida        | ✓ Requerida  |
| `DELETE` | `/debt/{id}`            | Deletar uma dívida                         | ✓ Requerida  |
| `POST`   | `/debt/{debtId}/pay`    | Marcar uma dívida como paga                | ✓ Requerida  |

---

## Notas Importantes

- **Proprietário do Cartão**: Apenas o criador do cartão pode adicionar/remover usuários e gerenciar dívidas;
- **Acesso a Dívidas**: Usuários vinculados ao cartão podem visualizar e gerenciar dívidas;
- **Validações**: As datas de dívida devem ser no passado; datas de expiração do cartão devem ser no futuro;
- **Tokens JWT**: Obtenha um token ao fazer login e o utilize para acessar endpoints protegidos.
