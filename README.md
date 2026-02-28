# 🪪 AgroSolutions.Identity
> O serviço de identidade do Hackathon da AgroSolutions mantém o contexto de autenticação dos produtores rurais, podendo gerenciar totalmente seu perfil.

## 🚜 Funcionalidades
  - Cadastro de Produtor Rural;
  - Autenticação de Produtor Rural (login com emissão de JWT);
  - Atualização cadastral do Produtor Rural;
  - Consulta de dados do Produtor Rural;
  - Validação de existência e integridade do Produtor via _Introspecção de token_;
  - Exclusão de Produtor Rural com propagação assíncrona através de Arquitetura Orientada a Eventos (EDA).

## ⚙️ Requisitos não funcionais
  - O sistema garante a segurança via autenticação e autorização JWT.
  - O sistema garante a integridade dos dados com validações internos à base de dados.
  - O sistema suporta escalabilidade horizontal conforme aumento de carga com HPA.
  - O sistema garante confiabilidade e consistência eventual na comunicação orientada a eventos.
  - O sistema garante manutenabilidade dado os microsserviços desacoplados.
  - O sistema prove observabilidade, com métricas, logs e logs distribuídos rastreáveis.
  - O sistema garante atualizações contínuas do artefeto de produção com fluxos de integração e entrega contínua.

## 🏗️ Desenho da Arquitetura
<img width="4123" height="4559" alt="Diagrama" src="https://github.com/user-attachments/assets/71eff8d1-67e7-42bf-9005-11694eaa4f83" />

## 🛠️ Detalhes Técnicos
### ⭐ Arquitetura e Padrões
 - Arquitetura orientada a eventos (Event-Driven Architecture – EDA);
 - Notification Pattern (Exceptionless);
 - Padrão CQRS (Command Query Responsibility Segregation);
 - Mediator Pattern com MediatR;
 - Clean Architecture;
 - Unit of Work;
 - Arquitetura baseada em APIs REST;
 - Uso de Middlewares e Action Filters para cross-cutting concerns;
 - Microsserviços containerizados.

### ⚙️ Backend & Framework
 - .NET 10 com C# 14;
 - ASP.NET Core;
 - Entity Framework Core;
 - FluentValidation para validações robustas;
 - Autenticação e autorização via JWT;
 - MemoryCache;
 - Padrão Cache Aside;
 - Documentação de APIs com Swagger / OpenAPI.

### 🗄️ Banco de Dados & Mensageria
 - SQL Server;
 - RabbitMQ para mensageria assíncrona;
 - Comunicação orientada a eventos;
 - Logs distribuídos com CorrelationId para rastreabilidade.

### 📊 Observabilidade & Monitoramento
 - Prometheus para coleta de métricas;
 - Grafana Loki para centralização de logs;
 - Estratégia de logging estruturado e distribuído.

### 🧪 Testes
 - Testes unitários com xUnit;
 - FluentAssertions para assertions mais expressivas;
 - Moq para criação de mocks e isolamento de dependências.

### 🚀 DevOps & Infraestrutura
 - CI/CD self-hosted;
 - Docker para containerização;
 - Kubernetes (Deployments, Services, HPA, ConfigMaps e Secrets);
 - Kong API Gateway para gerenciamento e roteamento de APIs.

## 🧪 Testes
  - Navegue até o diretório dos testes:
  ```
  cd ./AgroSolutions.Identity.Tests/
  ```
  - E insira o comando de execução de testes:
  ```
  dotnet test
  ```

## ▶️ Execução
  - Via HTTP.sys:
    - Navegue até o diretório da camada API da aplicação:
    ```
    cd ./AgroSolutions.Identity.API/
    ```
    - Insira o comando de execução do projeto:
    ```
    dotnet run --launch-profile https
    ```
    - Acesse [https://localhost:7132/swagger/index.html](https://localhost:7132/swagger/index.html)

  - Via Kubernertes local (minikube/kind):
    - Execute o comando para aplicar todos os arquivos yamls presentes no diretório:
    ```
    kubectl apply -f .\k8s\
    ```
    - Em seguida faça o PortForward:
    ```
    kubectl port-forward svc/agrosolutions-identity-api 8080:80
    ```
    - Acesse [http://localhost:8080/swagger/index.html](http://localhost:8080/swagger/index.html)

## 🚀 Requisições para Kong API Gateway
```javascript
let token;
let name = "Initial Name";
let email = "initialEmail@gmail.com";
const password = "password1234$$";
const headers = {
  "Content-Type": "application/json"
};

// Create User
let response = await fetch("http://localhost:8000/identity/api/v1/auth/register", {
  method: "POST",
  body: JSON.stringify({ name, email, password }),
  headers
}).then(r => r.json());
token = response.data.token;

// Update Name and E-mail
name = "New Name";
email = "newEmail@gmail.com";
response = await fetch("http://localhost:8000/identity/api/v1/auth/me", {
  method: "PATCH",
  body: JSON.stringify({ name, email }),
  headers: {
    ...headers,
    Authorization: `Bearer ${token}`
  }
}).then(r => r.json());
token = response.data.token;

// Get Current User
await fetch("http://localhost:8000/identity/api/v1/auth/me", {
  method: "GET",
  headers: {
    ...headers,
    Authorization: `Bearer ${token}`
  }
})

// Login
response = await fetch("http://localhost:8000/identity/api/v1/auth/login", {
  method: "POST",
  body: JSON.stringify({ email, password }),
  headers
}).then(r => r.json());
token = response.data.token;

// Delete User
await fetch("http://localhost:8000/identity/api/v1/auth/me", {
  method: "DELETE",
  headers: {
    ...headers,
    Authorization: `Bearer ${token}`
  }
})
```
