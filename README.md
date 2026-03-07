# Expense Splitter API

Backend API for splitting expenses between users and groups.  
Built with **.NET** and **PostgreSQL**.

## Tech Stack
- <img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/dotnetcore/dotnetcore-original.svg" height="20"/> ASP.NET Core Web API  
- <img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/csharp/csharp-original.svg" height="20"/> C#  
- <img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/postgresql/postgresql-original.svg" height="20"/> PostgreSQL  
- <img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/docker/docker-original.svg" height="20"/> Docker  
- <img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/dotnetcore/dotnetcore-original.svg" height="20"/> Entity Framework Core
- <img src="https://cdn.simpleicons.org/jsonwebtokens/white" height="20"/> JSON Web Token (JWT)

## Features (planned)
-   User management
-   Group creation
-   Adding expenses
-   Splitting expenses between participants
-   Expense history

## Running locally
### Prerequisites
Make sure you have the following tools installed:
-   .NET SDK
-   EF Core CLI
-   Docker
-   Docker Compose
-   Git
1. **Clone repository**
```
git clone https://github.com/marybrown123/expense-splitter-backend.git
```
```
cd expense-splitter-backend
```
2. **Start database**
The project uses **PostgreSQL** running in **Docker**.
```
docker compose up -d
```
3. **Apply database migrations**
```
dotnet ef database update --project ExpenseSplitter.Infrastructure --startup-project ExpenseSplitter.Api
```
4. **Run the API**
```
dotnet run --project ExpenseSplitter.Api
```
API will start on:
`http://localhost:PORT`

5. **Open Swagger**
After starting the application, open:
`http://localhost:PORT/swagger`
Swagger provides interactive documentation and allows testing API endpoints directly from the browser.

## Project status
The project is currently in the early development stage. Below is a breakdown of implemented components and the planned API endpoints and business logic.
- [x] Project setup
- [x] Docker + PostgreSQL 
- [x]  Entity Models
- [x]  Database migrations
- [x]  Basic CRUD endpoints
- [x]  Expense splitting logic
- [x]  Authentication

## API endpoints
The list of available API endpoints will be added as they are implemented.

## Screenshots
Screenshots showing the API usage and Swagger documentation will be added in future updates.
