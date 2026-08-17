# SPEC — Sistema de Emissão de Notas Fiscais

## 1. Visão geral

Este projeto deve implementar um sistema de emissão de notas fiscais com arquitetura baseada em Angular no front-end e .NET no back-end, utilizando como base a estrutura já criada em:

- Front-end: `korp_teste_marcosmaciel.client`
- Back-end: `Korp_Teste_MarcosMaciel.Server`

O objetivo é entregar uma aplicação funcional, com cadastros persistidos em banco de dados, regras de negócio para estoque e faturamento, suporte a falhas e possibilidade de expansão para concorrência e idempotência.

## 2. Objetivo do sistema

Desenvolver um sistema que permita:

- cadastrar produtos com saldo em estoque;
- cadastrar notas fiscais com itens e quantidades;
- imprimir notas fiscais seguindo regras de negócio;
- atualizar automaticamente o saldo do produto quando a nota for impressa;
- manter o sistema resiliente quando um microsserviço falhar;
- oferecer feedback claro ao usuário em caso de erro.

## 3. Tecnologias esperadas

### Front-end
- Angular 22
- TypeScript
- RxJS
- HttpClient
- Angular Material (recomendado para formulários, tabelas, dialogs e feedback visual)

### Back-end
- ASP.NET Core 8
- C#
- EF Core
- LINQ para consultas e regras de negócio
- SQL Server ou PostgreSQL como banco de dados
- Swagger/OpenAPI para documentação da API

### Arquitetura
- Arquitetura de microsserviços com no mínimo 2 serviços:
  1. Serviço de Estoque
  2. Serviço de Faturamento
- A solução pode manter `Korp_Teste_MarcosMaciel.Server` como projeto base de API/coordenação ou como um dos microsserviços, sendo necessário expandir a solução para incluir os serviços específicos.

## 4. Estrutura sugerida da solução

```text
Korp_Teste_MarcosMaciel/
├── korp_teste_marcosmaciel.client/      # Angular app
├── Korp_Teste_MarcosMaciel.Server/      # Projeto base do backend
├── Korp_Teste_MarcosMaciel.InventoryService/
├── Korp_Teste_MarcosMaciel.BillingService/
├── Korp_Teste_MarcosMaciel.Shared/
├── Korp_Teste_MarcosMaciel.slnx
├── requisitos.txt
├── SPEC.md
└── README.md
```

## 5. Requisitos funcionais

### 5.1 Cadastro de produtos

O sistema deve permitir registrar produtos com os seguintes campos obrigatórios:

- Código
- Descrição
- Saldo (quantidade disponível em estoque)

Regra de negócio:

- código deve ser único;
- descrição deve ser obrigatória;
- saldo deve ser maior ou igual a zero;
- o produto deve ser persistido no banco de dados;
- produto cadastrado deve poder ser reutilizado em notas fiscais.

Critérios de aceite:

- ao salvar um produto válido, o registro é persistido;
- ao salvar um produto com campo obrigatório ausente, a operação falha com feedback adequado;
- ao tentar inserir código duplicado, o sistema rejeita a operação.

### 5.2 Cadastro de notas fiscais

O sistema deve permitir a criação de notas fiscais com os seguintes dados obrigatórios:

- Numeração sequencial
- Status: `Aberta` ou `Fechada`
- Inclusão de múltiplos produtos com quantidades respectivas

Regra de negócio:

- a numeração deve ser sequencial e única;
- a nota deve iniciar com status `Aberta`;
- cada item da nota deve conter produto e quantidade;
- uma mesma nota pode conter vários produtos;
- a nota deve ser persistida fisicamente em banco de dados.

Critérios de aceite:

- a nota é criada com status `Aberta`;
- não é permitido criar nota sem produtos;
- não é permitido criar nota com quantidade zero ou negativa;
- a API deve validar se o produto informado existe.

### 5.3 Impressão de notas fiscais

A tela da nota fiscal deve possuir um botão de impressão visível e intuitivo.

Ao clicar no botão:

- exibir indicador de processamento (spinner, loading state ou mensagem de processamento);
- após a conclusão, atualizar o status da nota para `Fechada`;
- não permitir impressão de notas com status diferente de `Aberta`;
- atualizar o saldo dos produtos conforme a quantidade usada na nota.

Exemplo de regra:

- saldo anterior: 10
- nota usa 2 unidades
- novo saldo: 8

Critérios de aceite:

- nota em `Aberta` pode ser impressa;
- nota em `Fechada` não pode ser impressa;
- ao imprimir com sucesso, o status passa para `Fechada`;
- o estoque de cada produto utilizado é reduzido no valor correspondente;
- em caso de erro no processamento, o usuário recebe mensagem clara do problema.

## 6. Requisitos obrigatórios

### 6.1 Arquitetura de microsserviços

O sistema deve seguir uma arquitetura com no mínimo dois microsserviços:

1. Serviço de Estoque
   - responsável por manter produtos e controle de saldos;
   - expõe endpoints de criação, consulta e atualização de produto;
   - deve controlar a disponibilidade do saldo.

2. Serviço de Faturamento
   - responsável por criar, consultar e emitir notas fiscais;
   - deve validar status, itens e regras de negócio da nota;
   - deve orquestrar a emissão com o serviço de estoque.

Observação importante:

- A estrutura atual do backend pode servir como base para o projeto maior.
- O objetivo é manter os microsserviços dentro da mesma solução, com comunicação entre serviços via HTTP/REST, fila ou outro padrão, conforme necessidade do projeto.

### 6.2 Tratamento de falhas

O sistema deve implementar um cenário em que um dos microsserviços falha.

Exemplo:

- o serviço de estoque fica indisponível;
- a operação de emissão da nota deve capturar a falha;
- o sistema deve retornar mensagem amigável ao usuário, por exemplo:
  - "Não foi possível concluir a emissão da nota no momento. Tente novamente mais tarde.";
  - "Serviço de estoque indisponível.";

Exigências:

- usar tratamento de exceções no backend;
- mapear erros para respostas HTTP apropriadas (`400`, `404`, `409`, `500`, `503`);
- evitar quebra de fluxo para o usuário final;
- registrar falhas em logs.

### 6.3 Conexão real com banco de dados

Os cadastros devem ser persistidos fisicamente em um banco de dados real.

Opções aceitas:

- SQL Server
- PostgreSQL
- SQLite apenas para testes locais, mas o ideal é SQL Server/PostgreSQL para ambiente real

Requisitos:

- utilizar migrations ou criação inicial de schema;
- persistir produtos e notas fiscais em tabelas reais;
- não aceitar persistência apenas em memória.

## 7. Requisitos opcionais

Os itens abaixo são opcionais, mas podem ser implementados para elevar a qualidade do projeto:

### 7.1 Tratamento de concorrência

Cenário:

- produto com saldo 1;
- duas notas tentam consumir o mesmo saldo ao mesmo tempo;

Esse cenário deve ser tratado corretamente para evitar inconsistência de estoque.

Possíveis abordagens:

- lock pessimista em nível de linha;
- transação com controle de concorrência;
- `rowversion`/`Version` em entidades;
- validação de saldo antes da confirmação final.

### 7.2 Inteligência artificial

Pode-se implementar uma funcionalidade que gere algum valor agregado, por exemplo:

- recomendação de produtos para nota fiscal;
- sugestão de código ou descrição com base em histórico;
- resumo de nota fiscal;
- análise de comportamento de vendas.

### 7.3 Idempotência

Garantir que operações repetidas não gerem efeitos colaterais indesejados.

Exemplo:

- se a mesma ação de impressão for enviada duas vezes, o sistema não deve duplicar a redução de estoque nem alterar a nota de forma indevida.

## 8. Regras de negócio detalhadas

### 8.1 Produto

Entidade mínima:

```ts
interface Produto {
  id: number;
  codigo: string;
  descricao: string;
  saldo: number;
  criadoEm: string;
  atualizadoEm: string;
}
```

### 8.2 Nota fiscal

Entidade mínima:

```ts
interface NotaFiscal {
  id: number;
  numero: number;
  status: 'Aberta' | 'Fechada';
  itens: NotaFiscalItem[];
  criadoEm: string;
  atualizadoEm: string;
}
```

```ts
interface NotaFiscalItem {
  id: number;
  produtoId: number;
  quantidade: number;
}
```

### 8.3 Regras de validação

- código do produto obrigatório e único;
- descrição obrigatória;
- saldo obrigatório e não negativo;
- quantidade da nota deve ser maior que zero;
- cada item da nota precisa referenciar um produto existente;
- a nota só pode ser impressa quando estiver em status `Aberta`;
- ao imprimir, o sistema deve reduzir o estoque do(s) produto(s) referenciado(s);
- após impressão, a nota deve ficar `Fechada`.

## 9. Requisitos de UI/UX

### Tela de produtos

- formulário com campos: código, descrição, saldo;
- botão de salvar;
- listagem de produtos com saldo;
- mensagens de sucesso e erro;
- estado de carregamento enquanto salva/consulta.

### Tela de notas fiscais

- formulário para cadastrar nota;
- seleção de produtos e quantidade por item;
- botão para adicionar/remover itens;
- listagem das notas;
- botão de impressão por nota;
- exibição de status (`Aberta` ou `Fechada`);
- feedback visual para ações e falhas.

### Feedback visual

- loaders durante processamento;
- snackbars/toasts para sucesso, falha e validação;
- páginas com estados vazios e erros de carregamento.

## 10. Requisitos de API

A API deve expor endpoints mínimos como:

### Produtos
- `GET /api/produtos`
- `POST /api/produtos`
- `GET /api/produtos/{id}`

### Notas fiscais
- `GET /api/notas-fiscais`
- `POST /api/notas-fiscais`
- `GET /api/notas-fiscais/{id}`
- `POST /api/notas-fiscais/{id}/imprimir`

### Tratamento de erro

A API deve responder com:

- `400 Bad Request` para dados inválidos;
- `404 Not Found` para recursos inexistentes;
- `409 Conflict` para conflitos de regra ou duplicidade;
- `500 Internal Server Error` em falhas inesperadas;
- `503 Service Unavailable` quando um microsserviço estiver indisponível.

## 11. Tratamento de erros no backend

O backend deve seguir boas práticas de tratamento de exceções:

- usar `try/catch` em operações críticas;
- criar exceções de domínio específicas quando necessário;
- mapear erros para respostas HTTP coerentes;
- registrar logs com contexto relevante;
- padronizar respostas de erro no formato JSON;
- preservar o fluxo de negócio sem expor detalhes internos ao cliente.

No C#:

- usar `Exception` e classes de erro customizadas;
- usar `IResult` ou `Results` em Minimal API, ou `ControllerBase` em APIs MVC;
- utilizar `LINQ` para consultas, filtros e agregações;
- utilizar EF Core para ações de banco de dados e controle transacional.

## 12. Implementação recomendada pelo AI

### Prioridade de desenvolvimento

1. Estruturar a solução e mapas de entidades;
2. Implementar persistência e banco de dados;
3. Criar APIs de produtos;
4. Criar APIs de notas fiscais;
5. Implementar regra de impressão e atualização de estoque;
6. Criar camada de tratamento de falhas;
7. Implementar frontend Angular com formularios e listagens;
8. Adicionar feedback visual e validações;
9. Validar cenários de falha e concorrência;
10. Preparar documentação e apresentação final.

### Regras para a IA implementadora

- não criar features fora do escopo;
- priorizar o funcionamento correto do fluxo principal;
- manter a arquitetura consistente com Angular + .NET;
- não deixar o sistema em memória apenas;
- garantir que toda ação relevante seja persistida em banco;
- manter mensagens de erro úteis e amigáveis ao usuário;
- validar que a nota só é fechada após processamento bem-sucedido;
- validar que o saldo do estoque só é alterado quando a impressão é considerada válida.

## 13. Critérios de conclusão

O projeto estará concluído quando:

- a aplicação permitir cadastrar produtos e notas fiscais;
- a nota fiscal puder ser impressa com o fluxo de processamento e atualização de estoque;
- o sistema impedir impressão indevida de notas fechadas;
- o backend persistir dados em banco real;
- a arquitetura possuir no mínimo dois microsserviços;
- falhas de serviços forem tratadas e exibidas ao usuário;
- a interface Angular estiver funcional e com feedback visual consistente;
- o projeto estiver em repositório público com documentação e vídeo de apresentação.

## 14. Entregáveis esperados

- Repositório público do GitHub com nome `Korp_Teste_SeuNome`;
- Aplicação funcionando com front-end e back-end integrados;
- Código documentado de forma compreensível;
- Vídeo demonstrando telas e funcionamento;
- Descrição técnica detalhada com:
  - ciclos de vida do Angular utilizados;
  - uso de RxJS;
  - bibliotecas utilizadas;
  - bibliotecas visuais;
  - gerenciamento de dependências do Go/C# se aplicável;
  - frameworks usados no Go/C#;
  - tratamento de erros e exceções no backend;
  - uso de LINQ no C#.

## 15. Plano de execução por tarefas e sub-tarefas

A implementação deve ser dividida em etapas pequenas e testáveis, para facilitar o acompanhamento e a validação incremental por parte do responsável pelo projeto. Cada tarefa abaixo deve ser executada separadamente pelo Copilot, com validação do resultado antes de prosseguir para a próxima.

### Fase 1 — Base do backend e persistência

#### Tarefa 1 — Estruturar a solução .NET base
- Objetivo: preparar a estrutura inicial da solução para o backend.
- Subtarefas:
  - criar a organização de pastas para `Models`, `Data`, `Services`, `Controllers`, `DTOs`, `Exceptions` e `Interfaces`;
  - manter a solução principal e os projetos separados para estoque e faturamento;
  - configurar referências entre projetos;
  - preparar a base para EF Core e para API REST.
- Critério de aceite:
  - solução compilando sem erros de estrutura;
  - projetos organizados e prontos para receber entidades, serviços e controllers.
- Prompt sugerido para Copilot:
  - "No projeto `Korp_Teste_MarcosMaciel.Server`, organize a estrutura base da solução .NET para um sistema de notas fiscais. Crie a organização inicial para microsserviços de estoque e faturamento, mantendo projetos e dependências bem separadas. Preparar a base para EF Core, DTOs, serviços, controllers e tratamento de erro. Não implementar regras de negócio ainda."

#### Tarefa 2 — Configurar EF Core e banco de dados
- Objetivo: configurar conexão com banco real e o contexto base.
- Subtarefas:
  - escolher SQL Server ou PostgreSQL;
  - instalar os pacotes do EF Core e provider do banco;
  - configurar `appsettings.json` e `Program.cs`;
  - criar `DbContext` base;
  - validar a conexão com o banco de dados.
- Critério de aceite:
  - conexão funcionando com o banco real;
  - `DbContext` configurado corretamente;
  - o sistema preparado para persistir entidades.
- Prompt sugerido para Copilot:
  - "Configure o Entity Framework Core no backend .NET com conexão real ao banco de dados. Escolha SQL Server ou PostgreSQL e configure `DbContext`, `appsettings`, injeção de dependência e provider do banco. Crie a estrutura mínima para persistência dos domínios de Produto e NotaFiscal."

#### Tarefa 3 — Criar entidade Produto
- Objetivo: modelar o domínio do produto.
- Subtarefas:
  - definir campos: `Id`, `Codigo`, `Descricao`, `Saldo`, `CriadoEm`, `AtualizadoEm`;
  - garantir que `Codigo` e `Descricao` sejam obrigatórios;
  - garantir que o saldo seja maior ou igual a zero;
  - mapear entidade no EF Core;
  - gerar migration inicial.
- Critério de aceite:
  - entidade persistida no banco;
  - campos obrigatórios validados;
  - saldo não negativo.
- Prompt sugerido para Copilot:
  - "Crie a entidade `Produto` com os campos `Id`, `Codigo`, `Descricao`, `Saldo`, `CriadoEm` e `AtualizadoEm`. Configure validação de campos obrigatórios e saldo não negativo. Mapeie a entidade no `DbContext` e gere a migration inicial."

#### Tarefa 4 — Implementar repositório e serviço de produto
- Objetivo: separar armazenamento e regra de negócio de produto.
- Subtarefas:
  - criar interface do repositório;
  - implementar métodos de listagem, busca por id e criação;
  - validar entidade duplicada pelo código;
  - centralizar lógica de negócio em serviço.
- Critério de aceite:
  - cadastro de produtos funcionando;
  - código único;
  - regras de domínio aplicadas antes da persistência.
- Prompt sugerido para Copilot:
  - "Implemente a camada de acesso a dados para `Produto` com listagem, busca por id e criação. Crie um serviço dedicado para validar duplicidade de código e garantir regras de negócio antes de persistir. Mantenha persistência e validação separadas da camada de API."

#### Tarefa 5 — Criar endpoints de produtos
- Objetivo: expor a API de produtos.
- Subtarefas:
  - `GET /api/produtos`;
  - `GET /api/produtos/{id}`;
  - `POST /api/produtos`;
  - retornar modelos de resposta e erros HTTP adequados.
- Critério de aceite:
  - listagem funcionando;
  - cadastro persistido;
  - 400, 404, 409 e 500 corretamente mapeados.
- Prompt sugerido para Copilot:
  - "Crie os endpoints da API para produtos: `GET /api/produtos`, `GET /api/produtos/{id}` e `POST /api/produtos`. Valide dados obrigatórios e retorne status HTTP apropriados para dados inválidos, não encontrado, duplicidade e falhas internas."

### Fase 2 — Backend: notas fiscais e impressão

#### Tarefa 6 — Criar entidades de NotaFiscal e ItemNotaFiscal
- Objetivo: modelar o domínio da nota fiscal.
- Subtarefas:
  - definir `NotaFiscal` com `Id`, `Numero`, `Status`, `CriadoEm`, `AtualizadoEm`;
  - definir `NotaFiscalItem` com `Id`, `NotaFiscalId`, `ProdutoId`, `Quantidade`;
  - mapear relacionamento entre nota e itens;
  - configurar status inicial como `Aberta`;
  - gerar migration.
- Critério de aceite:
  - tabela de notas e itens criada no banco;
  - relacionamento persistido corretamente;
  - status validado.
- Prompt sugerido para Copilot:
  - "Crie as entidades `NotaFiscal` e `NotaFiscalItem` com os campos mínimos do sistema. Mapeie o relacionamento entre nota e itens no `DbContext`, defina status inicial `Aberta` e gere a migration correspondente."

#### Tarefa 7 — Implementar serviço para criação de notas fiscais
- Objetivo: criar a regra de negócio para notas fiscais.
- Subtarefas:
  - validar número sequencial e único;
  - validar itens da nota;
  - permitir múltiplos produtos por nota;
  - verificar que cada produto existe;
  - impedir nota sem itens ou quantidade inválida.
- Critério de aceite:
  - notas válidas criadas com status `Aberta`;
  - regra de quantidade e itens validados.
- Prompt sugerido para Copilot:
  - "Implemente o serviço de faturamento para criação de notas fiscais. A nota deve iniciar em status `Aberta`, aceitar múltiplos itens, validar quantidade maior que zero, verificar produto existente e impedir criação sem itens válidos."

#### Tarefa 8 — Criar endpoints de notas fiscais
- Objetivo: expor a API de faturamento.
- Subtarefas:
  - `GET /api/notas-fiscais`;
  - `GET /api/notas-fiscais/{id}`;
  - `POST /api/notas-fiscais`;
  - retornar erros bem definidos para validações e conflitos.
- Critério de aceite:
  - criação e consulta funcionando;
  - resposta padronizada;
  - falhas de regra corretamente reportadas.
- Prompt sugerido para Copilot:
  - "Crie os endpoints de notas fiscais: `GET /api/notas-fiscais`, `GET /api/notas-fiscais/{id}` e `POST /api/notas-fiscais`. Implemente validações e respostas HTTP padronizadas. O número da nota deve ser sequencial e único."

#### Tarefa 9 — Implementar regra de impressão da nota
- Objetivo: criar o fluxo de emissão de nota.
- Subtarefas:
  - verificar se a nota está em `Aberta`;
  - validar itens e saldo disponível;
  - permitir impressão somente quando a nota puder ser concluída;
  - atualizar status para `Fechada` após sucesso.
- Critério de aceite:
  - nota fechada somente após processamento bem-sucedido;
  - nota em `Fechada` bloqueada para impressão;
  - fluxo de negócio separado do endpoint.
- Prompt sugerido para Copilot:
  - "Implemente a lógica de impressão da nota fiscal. A nota somente pode ser impressa quando estiver em status `Aberta`. Depois do processamento, deve ser atualizada para `Fechada` e o sistema deve validar disponibilidade do estoque antes de concluir a operação."

#### Tarefa 10 — Criar endpoint de impressão da nota
- Objetivo: expor o endpoint de processamento.
- Subtarefas:
  - `POST /api/notas-fiscais/{id}/imprimir`;
  - indicar processamento em andamento;
  - tratar falha sem fechar a nota;
  - retornar status adequado em caso de erro.
- Critério de aceite:
  - botão de impressão no frontend consegue chamar esse endpoint;
  - operação bem-sucedida fecha a nota e baixa o estoque;
  - falhas não fecham a nota.
- Prompt sugerido para Copilot:
  - "Crie o endpoint `POST /api/notas-fiscais/{id}/imprimir` para realizar a emissão da nota. O endpoint deve validar o status da nota, processar a operação, atualizar a nota para `Fechada` somente após sucesso e tratar falhas de forma segura."

#### Tarefa 11 — Atualizar saldo do estoque ao imprimir
- Objetivo: reduzir o saldo dos produtos usados na nota.
- Subtarefas:
  - para cada item da nota, obter o produto associado;
  - validar se o saldo cobre a quantidade;
  - subtrair a quantidade consumida;
  - persistir a atualização em banco.
- Critério de aceite:
  - saldo reduzido conforme a quantidade utilizada;
  - sem redução em caso de falha;
  - estoque consistente com a nota emitida.
- Prompt sugerido para Copilot:
  - "Ao imprimir uma nota fiscal com sucesso, reduza o saldo dos produtos utilizados conforme a quantidade informada. Exemplo: saldo anterior 10 e quantidade 2 => saldo 8. A atualização deve ser persistida e não pode ocorrer se a operação falhar."

#### Tarefa 12 — Implementar tratamento de falhas e exceções
- Objetivo: lidar com erros do backend.
- Subtarefas:
  - criar middleware de exceções;
  - mapear 400, 404, 409, 500, 503;
  - registrar logs;
  - responder com JSON padronizado.
- Critério de aceite:
  - falhas do backend tratadas de forma amigável;
  - logs registrados;
  - API consistente para frontend.
- Prompt sugerido para Copilot:
  - "Implemente tratamento centralizado de exceções no backend .NET. Crie middleware ou filtros para mapear erros para `400`, `404`, `409`, `500` e `503`, registre logs e retorne JSON amigável ao cliente sem expor detalhes internos."

#### Tarefa 13 — Implementar comunicação entre microsserviços
- Objetivo: conectar estoque e faturamento.
- Subtarefas:
  - criar cliente HTTP para comunicação entre serviços;
  - consumir API de estoque ao validar saldo e atualização do estoque;
  - tratar indisponibilidade de um serviço;
  - retornar mensagem clara para o usuário.
- Critério de aceite:
  - falha de um microsserviço capturada e tratada;
  - resposta amigável no frontend e no backend.
- Prompt sugerido para Copilot:
  - "Configure a comunicação entre os microsserviços de estoque e faturamento usando HTTP/REST. O serviço de faturamento deve consultar e atualizar o estoque e tratar falhas do serviço de estoque com respostas HTTP/JSON apropriadas."

#### Tarefa 14 — Validar concorrência e idempotência
- Objetivo: garantir consistência em cenários críticos.
- Subtarefas:
  - testar caso de dois usuários tentando consumir o mesmo saldo ao mesmo tempo;
  - aplicar lock ou controle de concorrência em nível de linha ou transação;
  - opcionalmente implementar idempotência na impressão.
- Critério de aceite:
  - estoque não fica inconsistente;
  - impressão repetida não gera efeito colateral indevido.
- Prompt sugerido para Copilot:
  - "Implemente proteção contra concorrência para evitar que duas notas consumam o mesmo saldo simultaneamente. Use transação, lock ou outras técnicas simples para preservar consistência. Se houver tempo, aplique idempotência na impressão para evitar efeitos colaterais repetidos."

### Fase 3 — Frontend Angular

#### Tarefa 15 — Configurar shell da aplicação Angular
- Objetivo: preparar a base do front-end.
- Subtarefas:
  - configurar rotas e navegação; 
  - criar estrutura para módulos/páginas de produto e nota fiscal;
  - preparar layout base do sistema.
- Critério de aceite:
  - aplicativo Angular carregando com navegação inicial;
  - telas preparadas para integração com API.
- Prompt sugerido para Copilot:
  - "Configure a estrutura base do Angular para o sistema de notas fiscais. Crie a navegação inicial com páginas para Produtos e Notas Fiscais, e organize os módulos/serviços da aplicação."

#### Tarefa 16 — Criar serviço e modelos de produto no Angular
- Objetivo: consumir a API de produtos.
- Subtarefas:
  - criar interfaces `Produto`;
  - criar `ProdutoService` com `HttpClient`;
  - implementar listagem e criação.
- Critério de aceite:
  - front-end consegue listar e cadastrar produtos na API.
- Prompt sugerido para Copilot:
  - "Crie o service Angular para produtos, com `HttpClient`, interfaces TypeScript e métodos para listar e criar produtos. Trate erros de requisição de forma elegante."

#### Tarefa 17 — Implementar tela de cadastro de produtos
- Objetivo: permitir cadastro de produtos.
- Subtarefas:
  - formulário com código, descrição e saldo;
  - validações de campos obrigatórios;
  - botão de salvar;
  - loading e mensagens de sucesso/erro.
- Critério de aceite:
  - produto salvo com sucesso;
  - erros de validação exibidos ao usuário.
- Prompt sugerido para Copilot:
  - "Crie a tela de cadastro de produtos no Angular com campos Código, Descrição e Saldo. Inclua validações, botão de salvar, loading e mensagens de sucesso e erro."

#### Tarefa 18 — Implementar listagem de produtos
- Objetivo: visualizar o estoque.
- Subtarefas:
  - tabela ou cards com código, descrição e saldo;
  - carregamento e estado vazio;
  - feedback para erro de consulta.
- Critério de aceite:
  - listagem funcionando após integração com API;
  - visual claro do saldo em estoque.
- Prompt sugerido para Copilot:
  - "Crie a listagem de produtos no Angular com tabela ou cards exibindo código, descrição e saldo. Inclua estado de carregamento, estado vazio e erro de consulta."

#### Tarefa 19 — Criar serviço e modelos de nota fiscal no Angular
- Objetivo: consumir a API de notas fiscais.
- Subtarefas:
  - criar interfaces `NotaFiscal` e `NotaFiscalItem`;
  - criar `NotaFiscalService` com funções de listagem, criação e impressão;
  - tratar erros de comunicação.
- Critério de aceite:
  - frontend consegue criar e consultar notas;
  - endpoint de impressão integrado.
- Prompt sugerido para Copilot:
  - "Crie o service Angular para notas fiscais com interfaces, métodos para listar, criar e imprimir notas e tratamento de erro de comunicação com a API."

#### Tarefa 20 — Implementar tela de cadastro de nota fiscal
- Objetivo: criar notas com itens.
- Subtarefas:
  - selecionar produtos;
  - adicionar/remover itens;
  - informar quantidade por item;
  - status inicial `Aberta`;
  - salvar nota via API.
- Critério de aceite:
  - nota criada com múltiplos itens;
  - quantidade e produto validados.
- Prompt sugerido para Copilot:
  - "Crie a tela de cadastro de notas fiscais no Angular. Permita selecionar produtos, informar quantidade, adicionar e remover itens, salvar a nota e manter status inicial `Aberta`."

#### Tarefa 21 — Implementar listagem e impressão de notas
- Objetivo: visualizar notas e aplicar regra de impressão.
- Subtarefas:
  - listar notas com número, status e itens;
  - botão de impressão visível para notas `Aberta`;
  - bloquear ação para notas `Fechada`;
  - feedback visual de processamento.
- Critério de aceite:
  - fluxo de impressão visível e funcional;
  - botão desabilitado ou bloqueado quando necessário.
- Prompt sugerido para Copilot:
  - "Crie a listagem de notas fiscais no Angular com número, status, itens e botão de impressão. O botão deve estar habilitado somente para notas `Aberta` e mostrar loading durante o processamento."

#### Tarefa 22 — Implementar feedback visual e UX
- Objetivo: melhorar usabilidade e percepção do processo.
- Subtarefas:
  - loaders;
  - snackbars/toasts;
  - estados vazios;
  - mensagens de sucesso/erro;
  - destaque visual para notas abertas e fechadas.
- Critério de aceite:
  - o usuário entende claramente o estado da operação;
  - feedback visual presente em todos os fluxos principais.
- Prompt sugerido para Copilot:
  - "Melhore a experiência do usuário no Angular com loaders, snackbars, estados vazios, mensagens de erro e indicadores visuais para notas abertas e fechadas."

### Fase 4 — Validação, testes e documentação

#### Tarefa 23 — Validar build do backend
- Objetivo: garantir que o backend compila e funciona.
- Subtarefas:
  - rodar build do projeto .NET;
  - corrigir erros de compilação;
  - testar endpoints principais.
- Critério de aceite:
  - backend compilando e com fluxos principais validados.
- Prompt sugerido para Copilot:
  - "Valide o build do backend .NET, corrija erros de compilação e confirme que os endpoints principais de produto e nota fiscal funcionam corretamente."

#### Tarefa 24 — Validar build do frontend
- Objetivo: garantir que o Angular compila e integra corretamente.
- Subtarefas:
  - rodar build do Angular;
  - validar navegação;
  - verificar integração com API.
- Critério de aceite:
  - frontend compilando e navegando corretamente.
- Prompt sugerido para Copilot:
  - "Valide o build do frontend Angular, corrija erros de compilação e confirme que as telas e serviços conectam corretamente com a API do backend."

#### Tarefa 25 — Validar fluxo completo do sistema
- Objetivo: testar funcionalidade real do projeto.
- Subtarefas:
  - cadastrar produto;
  - criar nota fiscal;
  - imprimir nota;
  - validar fechamento e redução do estoque;
  - testar falha de microsserviço.
- Critério de aceite:
  - fluxo end-to-end funcionando;
  - mensagens de erro e sucesso consistentes.
- Prompt sugerido para Copilot:
  - "Teste o fluxo completo da aplicação: cadastro de produto, cadastro de nota, impressão e atualização de saldo; também validem o cenário de falha do microsserviço e a resposta amigável ao usuário."

#### Tarefa 26 — Preparar documentação e entrega final
- Objetivo: deixar o projeto pronto para apresentação.
- Subtarefas:
  - organizar README;
  - documentar arquitetura e regras de negócio;
  - preparar checklist de entrega;
  - escrever detalhes técnicos para apresentação final.
- Critério de aceite:
  - documentação suficiente para execução, apresentação e explicação técnica.
- Prompt sugerido para Copilot:
  - "Prepare a documentação final do projeto para entrega, com instruções de execução, arquitetura, microsserviços, regras de negócio, observações de erro, e detalhes técnicos que serão apresentados ao cliente."

### Ordem recomendada de execução

1. Tarefa 1
2. Tarefa 2
3. Tarefa 3
4. Tarefa 4
5. Tarefa 5
6. Tarefa 6
7. Tarefa 7
8. Tarefa 8
9. Tarefa 9
10. Tarefa 10
11. Tarefa 11
12. Tarefa 12
13. Tarefa 13
14. Tarefa 14
15. Tarefa 15
16. Tarefa 16
17. Tarefa 17
18. Tarefa 18
19. Tarefa 19
20. Tarefa 20
21. Tarefa 21
22. Tarefa 22
23. Tarefa 23
24. Tarefa 24
25. Tarefa 25
26. Tarefa 26

### Regras para execução incremental pelo Copilot

- cada tarefa deve ser entregue e validada separadamente;
- a próxima tarefa só deve iniciar quando a anterior estiver funcionando;
- não avançar para frontend antes da base do backend estar estável;
- sempre validar compilação, execução e regras de negócio antes de seguir;
- manter mensagens e estruturas consistentes entre backend, banco e frontend.

## 16. Observações finais

Este documento deve servir como referência principal para a implementação via IA. Ele concentra os requisitos mínimos do desafio, os critérios de aceite esperados e a sequência de desenvolvimento em tarefas pequenas e executáveis. Qualquer decisão arquitetural adicional deve respeitar a regra de negócio principal: um sistema de emissão de notas fiscais com controle real de estoque, persistência em banco, microsserviços e tratamento robusto de falhas.
