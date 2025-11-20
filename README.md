# 🎮 FIAP Cloud Games (FCG)
O FIAP Cloud Games (FCG) é um projeto acadêmico que reúne conhecimentos adquiridos nas disciplinas onde o desafio envolve o desenvolvimento de uma plataforma que permitirá a venda de jogos digitais e a gestão de servidores para partidas online.
Esta estapa do projeto tem como foco a criação de uma API REST em .NET 8 para gerenciar usuários e suas bibliotecas de jogos adquiridos, garantindo persistência de dados, qualidade do software e boas práticas de desenvolvimento.

## _Microserviço Usuarios_

Usuários.Api é um dos principais microserviços que compõem a arquitetura do projeto FIAP Cloud Games, responsável por centralizar e gerenciar todas as operações relacionadas aos usuários da plataforma.
Este microserviço oferece funcionalidades completas para:
- Cadastro de novos usuários
- Atualização de informações cadastrais
- Gerenciamento de autenticação e autorização
- Recuperação e alteração de senhas
- Consultas detalhadas de perfis e permissões
A API foi desenvolvida com foco em segurança, escalabilidade e integração, utilizando o Entity Framework para persistência relacional e implementando autenticação baseada em JWT, garantindo proteção dos dados e uma experiência confiável tanto para usuários quanto para administradores da plataforma.


## 📋 Pré-requisitos

Antes de iniciar o projeto, é necessário atender aos seguintes pré-requisitos para garantir um ambiente de desenvolvimento adequado:

### 🛠 Tecnologias Necessárias
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) – Plataforma de desenvolvimento para criar a API REST
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) – Banco de dados para persistência dos dados
- [Visual Studio 2022](https://visualstudio.microsoft.com/pt-br/) ou [VS Code](https://code.visualstudio.com/) – IDE recomendada para desenvolvimento

### 📦 Pacotes e Dependências

O projeto depende dos seguintes pacotes:

#### Projeto Usuarios.Api
- Autenticação via JWT: Microsoft.AspNetCore.Authentication.JwtBearer
- Observabilidade e Telemetria: Microsoft.ApplicationInsights.AspNetCore, Microsoft.Extensions.Logging.ApplicationInsights, prometheus-net.AspNetCore
- ORM e Banco de Dados: Microsoft.EntityFrameworkCore, Microsoft.EntityFrameworkCore.SqlServer, Microsoft.EntityFrameworkCore.Design, Microsoft.EntityFrameworkCore.Tools
- Documentação da API: Swashbuckle.AspNetCore, Swashbuckle.AspNetCore.Annotations
- Hash de Senhas Seguro: Isopoh.Cryptography.Argon2
- Componentes e utilitários internos: Fcg.Common
- Suporte a containers (Docker): Microsoft.VisualStudio.Azure.Containers.Tools.Targets

```
Install-Package Fcg.Common -Version 1.0.0
Install-Package Microsoft.ApplicationInsights.AspNetCore -Version 2.23.0
Install-Package Isopoh.Cryptography.Argon2 -Version 2.0.0
Install-Package Microsoft.AspNetCore.Authentication.JwtBearer -Version 8.0.15
Install-Package Microsoft.EntityFrameworkCore -Version 8.0.19
Install-Package Microsoft.EntityFrameworkCore.Design -Version 8.0.19
Install-Package Microsoft.EntityFrameworkCore.SqlServer -Version 8.0.19
Install-Package Microsoft.EntityFrameworkCore.Tools -Version 8.0.19
Install-Package Microsoft.Extensions.Logging.ApplicationInsights -Version 2.23.0
Install-Package Microsoft.VisualStudio.Azure.Containers.Tools.Targets -Version 1.22.1
Install-Package prometheus-net.AspNetCore -Version 8.2.1
Install-Package Swashbuckle.AspNetCore -Version 7.3.2
Install-Package Swashbuckle.AspNetCore.Annotations -Version 7.3.2
```

#### Projeto Usuarios.Api.Tests
- Framework de Testes Unitários: xunit, xunit.runner.visualstudio
- Mock e Simulação de Dependências: Moq
- Infraestrutura de Execução de Testes: Microsoft.NET.Test.Sdk
```
Install-Package xunit -Version 2.9.3
Install-Package xunit.runner.visualstudio -Version 3.1.5
Install-Package Moq -Version 4.20.72
Install-Package Microsoft.NET.Test.Sdk -Version 17.14.1
```

## 🗂️ Estrutura
O projeto Usuarios.Api está organizado em camadas, seguindo boas práticas de separação de responsabilidades e facilitando a manutenção, testes e evolução do sistema.
```
Usuarios.Api/
│──📂 Applitation/
│   ├──📂 Constants/
│   ├──📂 DTOs/
│   ├──📂 Mappers/
│   ├──📂 Services/
│──📂 Configurations/
│──📂 Controllers/
│──📂 Domain/
│   ├──📂 Entities/
│   ├──📂 Interfaces/
│──📂 Infraestructure/
│   ├──📂 Data/
│   ├──📂 Mappings
│   ├──📂 Search/
Usuarios.Api.Tests/
│──📂 ServicesTests/
```
#### 1. Application
Agrupa a lógica de aplicação, servindo de ponte entre a API e o domínio.
- DTOs: Objetos de transferência de dados, usados para entrada e saída de informações na API.
- Mappers: Classes estáticas ou utilitários para conversão entre entidades do domínio e DTOs.
- Services: Serviços de aplicação que orquestram regras de negócio, validações e interações entre as camadas.

#### 2. Configurations
Contém classes responsáveis pelas configurações globais da aplicação, como injeção de dependências, configuração do Swagger, Application Insights, Prometheus, autenticação, entre outros. Centraliza tudo que é necessário para inicializar e configurar o ambiente da API.

#### 3. Controllers
Reúne os controladores da API, que são responsáveis por expor os endpoints HTTP. Cada controller lida com as requisições, validações iniciais e retorna as respostas apropriadas, delegando a lógica de negócio para os serviços da camada de aplicação.

#### 4. Domain
Representa o núcleo do sistema, onde ficam as regras de negócio e abstrações principais. Suas subpastas normalmente incluem:
- Entities: Entidades de domínio, que representam os objetos principais do negócio (ex: Usuario).
- Interfaces: Contratos para repositórios e serviços, promovendo o desacoplamento entre domínio e infraestrutura.

#### 5. Infraestructure
Responsável pela implementação de detalhes técnicos e integrações externas, como acesso a banco de dados, mecanismos de busca, etc. Suas subpastas podem incluir:
- Data: Implementações de repositórios, contexto do Entity Framework (DbContext) e migrações.

#### 6. ServicesTests
Contém os testes automatizados do sistema, organizados por tipo:
- Unitários: Testam funcionalidades isoladas.
- Integração: Validam a integração entre componentes e camadas.

## 🏛️ Entidades do Domínio
Integrada aos demais serviços do ecossistema FIAP Cloud Games, a Usuarios.Api atua como o núcleo de dados dos usuarios, permitindo que suas informações possam ser consumidas de forma segura e eficiente.

## ⚙️ Funcionalidades da Api
A API expõe os seguintes endpoints:

| **Método** | **Endpoint** | **Descrição** | 
| ------ | ------ | ------ |
| 🟩 POST | `/Usuarios/Login` | Efetua a autenticação do usuário retornando um token JWT | 
| 🟩 POST | `/Usuarios/LoginAtivacao` | Realiza login para ativação de conta utilizando o código enviado por e-mail | 
| 🟩 POST | `/Usuarios/LoginNovaSenha` | Realiza login para redefinição de senha utilizando o código enviado por e-mail | 
| 🔵 GET | `/Usuarios/SolicitarNovaSenha` | Envia um código de validação para redefinição de senha | 
| 🔵 GET | `/Usuarios/SolicitarReativacao` | Envia um código de reativação de conta para o e-mail do usuário | 
| 🔵 GET | `/Usuarios/ReenviarCodigoAtivacao` | Reenvia o código de ativação da conta para o e-mail do usuário | 
| 🔵 GET | `/Usuarios/ReenviarCodigoValidacao` | Reenvia o código de validação para recuperação de senha | 
| 🔵 GET | `/Usuarios/ObterUsuario` | Obtém os dados do usuário autenticado | 
| 🔵 GET | `/Usuarios/ObterUsuarioPorApelido` | Obtém um usuário cadastrado pelo apelido | 
| 🔵 GET | `/Usuarios/ObterUsuarioPorEmail` | Obtém um usuário cadastrado pelo e-mail | 
| 🔵 GET | `/Usuarios/ObterUsuarios` | Obtém todos os usuários cadastrados (ativos e inativos) | 
| 🔵 GET | `/Usuarios/ObterUsuariosAtivos` | Obtém todos os usuários ativos cadastrados | 
| 🟩 POST | `/Usuarios/AdicionarUsuario` | Cria um novo usuário e envia um código de ativação por e-mail | 
| 🟧 PUT | `/Usuarios/AlterarUsuario` | Altera os dados do usuário autenticado ou permite alteração por administradores | 
| 🟧 PUT | `/Usuarios/AlterarSenha` | Permite que um usuário altere sua senha | 
| 🟧 PUT | `/Usuarios/AtivarUsuario` | Permite que um administrador ative uma conta | 
| 🟧 PUT | `/Usuarios/DesativarUsuario` | Permite que um administrador desative uma conta | 
| 🟧 PUT | `/Usuarios/TornarUsuario` | Permite que um administrador altere o perfil do usuário para "Usuário" | 
| 🟧 PUT | `/Usuarios/TornarAdministrador` | Permite que um administrador altere o perfil do usuário para "Administrador" | 


## 🚀 Executando os testes

Para garantir a qualidade e a estabilidade do projeto, é essencial executar os testes automatizados. O projeto utiliza xUnit para testes e Moq para simulação de dependências.

### Estrutura dos testes
Os testes estão organizados conforme a estrutura do projeto:

```
Usuarios.Api.Tests
│── 📂 ServicesTests
│    │── 📄 UsuarioServiceTests

```
Para rodar os testes, siga os passos:

#### ✅ Executar todos os testes
```
dotnet test
```

#### ✅ Executar um teste espesífico

```
dotnet test --filter FullyQualifiedName=Namespace.Classe.Teste
```

Exemplo:
```
dotnet test --filter FullyQualifiedName=FCG.Tests.IntegrationTests.ServicesTests.CadastrarUsuario_ComDadosValidos_DeveSalvarNoBanco
```

#### ✅ Executar apenas testes unitários
```
dotnet test --filter Category=Unit
```

#### ✅ Executar apenas testes de integração
```
dotnet test --filter Category=Integration
```

#### ✅ Executar apenas testes de BDD
```
dotnet test --filter Category=BDD
```

## ✒️ Autor
*Márcio Henrique Vieira dos Santos - ✉️ marciohenriquev@gmail.com*# FCG
