# Expense Splitter API

Backend API for splitting expenses between users and groups.  
Built with **.NET** and **PostgreSQL**.

## Tech Stack
- .NET (ASP.NET Core Web API)
-   PostgreSQL    
-   Docker
-   Entity Framework Core

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
3. **Run the API**
```
dotnet run
```
API will start on:
`http://localhost:PORT`

4. **Open Swagger**
After starting the application, open:
`http://localhost:PORT/swagger`
Swagger provides interactive documentation and allows testing API endpoints directly from the browser.

## Project status
The project is currently in the early development stage. Below is a breakdown of implemented components and the planned API endpoints and business logic.
- [x] Project setup
- [x] Docker + PostgreSQL 
- [x]  Entity Models
- [x]  Database migrations
- [ ]  Basic CRUD endpoints
- [ ]  Expense splitting logic
- [ ]  Authentication

## API endpoints
The list of available API endpoints will be added as they are implemented.

## Screenshots
Screenshots showing the API usage and Swagger documentation will be added in future updates.
