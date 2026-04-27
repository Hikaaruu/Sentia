# Sentia 💬
A real-time chat application featuring live AI sentiment analysis. Sentia demonstrates modern full-stack development utilizing Clean Architecture, CQRS, and Azure cloud services.

### ⚙️ Backend
[![.NET 10](https://img.shields.io/badge/.NET_10.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-239120?logo=c-sharp&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-512BD4?logo=dotnet&logoColor=white)](https://docs.microsoft.com/en-us/aspnet/core/)
[![Entity Framework Core](https://img.shields.io/badge/EF_Core-512BD4?logo=dotnet&logoColor=white)](https://docs.microsoft.com/en-us/ef/core/)
[![Dapper](https://img.shields.io/badge/Dapper-e34f26?logo=database&logoColor=white)](https://github.com/DapperLib/Dapper)
[![MediatR](https://img.shields.io/badge/CQRS-MediatR-4CAF50?logo=nuget&logoColor=white)](https://github.com/jbogard/MediatR)
[![SignalR](https://img.shields.io/badge/SignalR-0078D4?logo=microsoft&logoColor=white)](https://dotnet.microsoft.com/apps/aspnet/signalr)
[![xUnit](https://img.shields.io/badge/xUnit-512BD4?logo=dotnet&logoColor=white)](https://xunit.net/)

### 🎨 Frontend
[![React 19](https://img.shields.io/badge/React_19-20232A?logo=react&logoColor=61DAFB)](https://react.dev/)
[![TypeScript](https://img.shields.io/badge/TypeScript-3178C6?logo=typescript&logoColor=white)](https://www.typescriptlang.org/)
[![Vite](https://img.shields.io/badge/Vite-646CFF?logo=vite&logoColor=white)](https://vitejs.dev/)
[![Tailwind CSS](https://img.shields.io/badge/Tailwind_CSS-38B2AC?logo=tailwind-css&logoColor=white)](https://tailwindcss.com/)
[![Zustand](https://img.shields.io/badge/Zustand-443E38?logo=react&logoColor=white)](https://github.com/pmndrs/zustand)
[![React Query](https://img.shields.io/badge/React_Query-FF4154?logo=reactquery&logoColor=white)](https://tanstack.com/query/v3/)
[![shadcn/ui](https://img.shields.io/badge/shadcn%2Fui-000000?logo=shadcnui&logoColor=white)](https://ui.shadcn.com/)

### ☁️ Cloud & DevOps
[![Azure App Service](https://img.shields.io/badge/Azure_App_Service-008AD7?logo=microsoftazure&logoColor=white)](https://azure.microsoft.com/en-us/products/app-service/)
[![Azure Static Web Apps](https://img.shields.io/badge/Azure_Static_Web_Apps-008AD7?logo=microsoftazure&logoColor=white)](https://azure.microsoft.com/en-us/products/app-service/static)
[![Azure SQL](https://img.shields.io/badge/Azure_SQL-008AD7?logo=microsoftazure&logoColor=white)](https://azure.microsoft.com/en-us/products/azure-sql/database/)
[![Azure AI](https://img.shields.io/badge/Azure_AI_Language-008AD7?logo=microsoftazure&logoColor=white)](https://azure.microsoft.com/en-us/products/ai-services/ai-language/)
[![GitHub Actions](https://img.shields.io/badge/GitHub_Actions-2088FF?logo=githubactions&logoColor=white)](https://github.com/features/actions)


---

## 🚀 Live Demo
**[🚀 View the Live Application](https://salmon-flower-083c06f03.7.azurestaticapps.net/)** 
Experience the real-time messaging and live AI sentiment analysis firsthand by exploring the deployed application. Create an account to start a conversation and watch the AI evaluate message sentiment on the fly.

---

## ✨ Key Capabilities

💬 **Real-Time Messaging:** Lightning-fast, bi-directional communication powered by Azure SignalR.  
🧠 AI Sentiment Analysis: Real-time processing of message tone (Positive 🟢, Neutral ⚪, Negative 🔴) using Azure Cognitive Services. This is handled via a non-blocking background System.Threading.Channel to keep the chat instantly responsive.
🟢 **Presence Tracking:** Live online status indicators for users.  
✍️ **Typing Indicators:** Real-time animated "user is typing..." feedback.  
🛡️ **Security & Rate Limiting:** JWT-based authentication, Azure Key Vault secret management, and endpoint rate-limiting.  
🚀 **Optimized Data Fetching:** Write operations are handled by Entity Framework Core, while complex read queries are optimized using **Dapper**.  

---

## 📸 Screenshots

![Register Page](docs/register.png)

![Login Page](docs/login.png)

![Create New Chat](docs/create-new-chat.png)

![Chat Window](docs/chat.png)

![Typing Indicator](docs/typing.png)

---

## 🏗️ Architecture & Patterns

The project follows a strict layered architecture to ensure separation of concerns, testability, and scalability:

- **`Sentia.Domain`** – The enterprise core. Contains fundamental business entities (`Chat`, `Message`, `ChatParticipant`), enums, and domain events. It has absolutely zero external project dependencies.
- **`Sentia.Application`** – The use-case orchestrator. Houses all business logic using CQRS via MediatR (e.g., `SendMessageCommand`, `GetUserChatsQuery`). It includes DTOs, FluentValidation rules, and custom MediatR pipeline behaviors for Cross-Cutting Concerns (Validation and Authorization).
- **`Sentia.Infrastructure.Persistence`** – The data layer. Manages SQL Server access containing the EF Core `ApplicationDbContext`, Fluent API entity configurations, migrations, and highly optimized read-only data access using Dapper (`ChatQueryService`).
- **`Sentia.Infrastructure.Cognitive`** – The AI layer. Isolated integration with Azure Cognitive Services for Text Analytics. It implements a thread-safe `System.Threading.Channels` queue to offload sentiment processing from the main web threads.
- **`Sentia.Infrastructure.RealTime`** – The WebSocket layer. Encapsulates the Azure SignalR `ChatHub`, mapping user connections to SignalR groups, and manages live user states via the `PresenceTracker`.
- **`Sentia.API`** – The presentation/entry point. Exposes RESTful Controllers, configures Dependency Injection, handles JWT Bearer Authentication, enforces API Rate Limiting, maps global exceptions to `ProblemDetails`, and hosts the `SentimentBackgroundWorker`.


### Core Patterns
- **CQRS Pattern:** Implemented via **MediatR**. Commands and Queries are strictly segregated.
- **Pipeline Behaviors:** MediatR pipeline behaviors automatically handle cross-cutting concerns like **FluentValidation** and custom Chat Participant Authorization.
- **Background Processing:** AI Sentiment Analysis is offloaded to a background worker using `System.Threading.Channels` to ensure the main chat thread remains instantly responsive.
- **Global Error Handling:** Centralized exception handling returning standard RFC 7807 `ProblemDetails` (Validation, NotFound, Forbidden).

---

## 🛠️ Tech Stack

### Backend
- **Framework:** .NET 10, ASP.NET Core Web API
- **Real-Time:** Azure SignalR Service
- **Database Access:** Entity Framework Core, Dapper 
- **Architecture Tools:** MediatR, AutoMapper, FluentValidation
- **Authentication:** ASP.NET Core Identity, JWT Bearer
- **API Documentation:** Scalar

### Frontend
- **Framework:** React 19, Vite, TypeScript
- **Styling:** Tailwind CSS v4, shadcn/ui components, Lucide Icons
- **State Management:** Zustand (Global State & Presence), TanStack React Query (Data Fetching, Caching, Optimistic UI updates)
- **Forms & Validation:** React Hook Form + Zod
- **Real-Time:** `@microsoft/signalr`

### Infrastructure & Cloud
- Azure App Service (.NET Backend)
- Azure Static Web Apps (React Frontend)
- Azure SQL Database
- Azure AI Language Service (Text Analytics)
- Azure Key Vault (Secrets Management)
- GitHub Actions (CI/CD)

### Testing 🧪
- **Frameworks:** xUnit, Moq, FluentAssertions
- **Approach:** Extensive unit test coverage for MediatR Handlers, Validation Rules, Pipeline Behaviors, and Event Handlers.

---

## 🗄️ Database Schema

![Database Entity-Relationship Diagram](docs/db-diagram.svg)