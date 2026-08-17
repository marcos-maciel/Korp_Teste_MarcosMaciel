# SPEC de Implementação para IA

## 1. Contexto

Este repositório contém uma solução composta por:

- Backend em ASP.NET Core: `Korp_Teste_MarcosMaciel.Server`
- Frontend em Angular: `korp_teste_marcosmaciel.client`
- Arquivo de solução: `Korp_Teste_MarcosMaciel.slnx`

A estrutura atual já é um template inicial padrão do Visual Studio com um projeto server e um cliente Angular, porém ainda não foi adaptada para uma aplicação funcional de negócio.

## 2. Objetivo da Implementação

Implementar uma aplicação full-stack com arquitetura moderna, onde:

- o backend expõe APIs REST em ASP.NET Core;
- o frontend Angular consome essas APIs;
- a comunicação entre cliente e servidor é tratada de forma segura e organizada;
- a solução pode ser executada localmente em ambiente de desenvolvimento com comandos simples;
- o código segue padrões limpos e permite manutenção futura.

## 3. Stack Tecnológica

### Backend
- .NET 8
- ASP.NET Core Web API
- Swagger/OpenAPI
- C# com controllers e DTOs

### Frontend
- Angular 22
- TypeScript
- HttpClient para consumo de APIs
- Estrutura modular do Angular

## 4. Requisitos Funcionais

### RF-01 — Backend funcional
- O projeto `Korp_Teste_MarcosMaciel.Server` deve iniciar sem erros.
- Deve haver uma API REST com endpoints organizados por domínio.
- Deve existir suporte a Swagger/OpenAPI em ambiente de desenvolvimento.
- Deve haver resposta JSON estruturada e padronizada.

### RF-02 — Frontend integrado
- O Angular deve consumir a API do backend.
- O cliente deve exibir dados vindos da API em tela.
- Deve haver tratamento de erro para falhas de requisição.
- A comunicação deve ocorrer via proxy ou configuração adequada para desenvolvimento.

### RF-03 — Arquitetura limpa
- A estrutura deve separar responsabilidades em camadas, idealmente:
  - Controllers
  - Services
  - Models / DTOs / ViewModels
  - Data / Repository (quando aplicável)
- Não deve haver lógica de negócio espalhada em controllers.

### RF-04 — Qualidade e manutenção
- Código com nomes claros e coerentes.
- Uso de boas práticas de C# e Angular.
- Validação de entrada em endpoints.
- Tratamento de erros e mensagens amigáveis.

## 5. Requisitos Não Funcionais

- O projeto deve rodar em ambiente Windows e com base em dotnet + npm.
- O backend e o frontend devem ser facilmente iniciados em desenvolvimento.
- A solução deve ser intuitiva para manutenção e extensão.
- A aplicação deve seguir convenções de nomenclatura em português ou em inglês, mas de forma consistente.

## 6. Estrutura Esperada da Solução

### Backend

```text
Korp_Teste_MarcosMaciel.Server/
  Controllers/
  Models/
  DTOs/
  Services/
  Data/
  Program.cs
  appsettings.json
  appsettings.Development.json
```

### Frontend

```text
korp_teste_marcosmaciel.client/
  src/
    app/
      components/
      services/
      models/
      pages/
      app-routing.module.ts
      app.module.ts
      app.component.*
  package.json
  angular.json
```

## 7. Especificação de Implementação para a IA

### Tarefa 1 — Validar o estado inicial da solução
- Verificar se a solução compila.
- Confirmar a existência dos projetos no arquivo `Korp_Teste_MarcosMaciel.slnx`.
- Rodar restore/build do backend.
- Rodar install/build do frontend.

### Tarefa 2 — Preparar o backend
- Ajustar `Program.cs` para configurar corretamente:
  - controllers;
  - Swagger em desenvolvimento;
  - CORS, se necessário;
  - fallback para o frontend em produção;
  - mapeamento de endpoints.
- Remover ou adaptar código padrão gerado que não faz parte do objetivo final.
- Criar endpoints REST claros para o domínio da aplicação.

### Tarefa 3 — Criar modelos e DTOs
- Definir entidades ou contratos usados pela aplicação.
- Criar classes de resposta com estrutura consistente.
- Garantir uso de nomes claros e propriedades tipadas.

### Tarefa 4 — Implementar serviços no backend
- Extrair a lógica da aplicação para services.
- Simplificar controllers para apenas receber e responder requisições.
- Implementar tratamento de dados e validações.

### Tarefa 5 — Integrar o Angular ao backend
- Ajustar `HttpClient` no frontend.
- Criar serviços para comunicação com a API.
- Exibir dados em componente(s) Angular.
- Tratar falhas com mensagens amigáveis.

### Tarefa 6 — Ajustar proxy/configuração de ambiente
- Garantir que o desenvolvimento local funcione sem erros de CORS.
- Configurar o Angular para apontar para o backend em desenvolvimento.
- Validar que os endpoints funcionam em `http://localhost` com base no setup do projeto.

### Tarefa 7 — Validar e estabilizar
- Rodar build do backend: `dotnet build`.
- Rodar build do frontend: `npm run build`.
- Iniciar ambos os projetos e verificar funcionamento real.
- Confirmar que não há erros de runtime nem de compilação.

## 8. Critérios de Aceitação

A implementação será considerada concluída quando:

1. O backend inicia sem erros e expõe endpoints válidos.
2. O frontend consegue consumir a API sem erros de CORS ou de rede.
3. A app exibe dados reais da API em tela.
4. O código está organizado por responsabilidades.
5. Há tratamento de erro e validação básica.
6. A solução compila e roda em desenvolvimento.

## 9. Checklist Final para a IA

- [ ] Verificar projeto atual
- [ ] Configurar backend ASP.NET Core
- [ ] Definir modelos/DTOs
- [ ] Implementar serviços
- [ ] Expor endpoints REST
- [ ] Configurar Angular para consumir API
- [ ] Criar componentes/telas necessárias
- [ ] Garantir UI funcional
- [ ] Validar build do backend
- [ ] Validar build do frontend
- [ ] Verificar execução local

## 10. Prompt Pronto para IA

> Implemente uma aplicação full-stack usando a solução atual do repositório, com backend ASP.NET Core e frontend Angular. A solução já possui a estrutura base, então você deve adaptar o projeto para uma aplicação funcional, organizada por camadas, com APIs REST no backend e consumo desses endpoints no Angular. Faça a integração correta em desenvolvimento, configure CORS/proxy se necessário, trate erros, mantenha o código limpo e valide que o projeto compila e roda localmente.

## 11. Observações

Como o projeto atual é um template inicial, a IA deve priorizar:

- funcionalidade real;
- clareza arquitetural;
- integração backend/frontend;
- execução local com verificação via build e execução.

A implementação deve respeitar esse escopo e evitar criação de recursos desnecessários sem vínculo com objetivos do sistema.
