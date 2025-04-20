TaskManager Fullstack ASP.NET + React

[!NOTE]Dự án sử dụng ASP.NET Core Web API làm backend, SPA frontend (React + Vite) và PostgreSQL. Có sẵn cấu hình Docker.

📂 Cấu trúc thư mục

/
├── taskmanager.client/ # React + Vite frontend
├── taskmanager.Server/ # ASP.NET Core Web API
├── taskmanager.tests/ # Unit & integration tests
└── TaskManager.sln # Visual Studio solution

🛠 Cài đặt & chạy (Local Dev)

Yêu cầu

.NET SDK 7+

Node.js

PostgreSQL (hoặc dùng Docker)

Docker Desktop (tuỳ chọn)

1. Backend

cd taskmanager.Server
dotnet restore
dotnet run

Mặc định API chạy ở http://localhost:5000

2. Frontend

cd taskmanager.client
npm install
npm run dev

Mặc định SPA chạy ở http://localhost:3000

3. Tests

cd taskmanager.tests
dotnet restore
dotnet test

⚙️ Database

EF Core + PostgreSQL

Connection string (ở taskmanager.Server/appsettings.Development.json):

"ConnectionStrings": {
"DefaultConnection": "Host=localhost;Port=5432;Database=taskmanager;Username=postgres;Password=yourpassword"
}

EF CLI:

dotnet ef migrations add Init --project taskmanager.Server --startup-project taskmanager.Server
dotnet ef database update --project taskmanager.Server --startup-project taskmanager.Server

🐳 Docker (toàn bộ stack)

1. Copy env

cp .env.example .env

2. Build & chạy

docker-compose up --build

Services

Service

URL

Frontend

http://localhost:3000

Backend API

http://localhost:5000/swagger

PostgreSQL

localhost:5432 (DB=taskmanager)

🔧 Dockerfile & docker-compose

Backend (taskmanager.Server/Dockerfile)

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["taskmanager.Server/taskmanager.Server.csproj", "."]
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "taskmanager.Server.dll"]

Frontend (taskmanager.client/Dockerfile)

FROM node:18-alpine AS build
WORKDIR /app
COPY . .
RUN npm install && npm run build

FROM nginx:stable-alpine
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]

docker-compose.yml (ở root)

version: '3.8'

services:
client:
build:
context: ./taskmanager.client
dockerfile: Dockerfile
ports: - "3000:80"
depends_on: - api

api:
build:
context: ./taskmanager.Server
dockerfile: Dockerfile
ports: - "5000:80"
environment: - ConnectionStrings\_\_DefaultConnection=Host=postgres;Port=5432;Database=taskmanager;Username=postgres;Password=${POSTGRES_PASSWORD}
depends_on: - postgres

postgres:
image: postgres:15
environment:
POSTGRES_USER: postgres
POSTGRES_PASSWORD: ${POSTGRES_PASSWORD}
POSTGRES_DB: taskmanager
ports: - "5432:5432"
volumes: - pgdata:/var/lib/postgresql/data

volumes:
pgdata:

.env.example

POSTGRES_PASSWORD=123456

✅ Lưu ý

Thực thi migrations chỉ khi không ở môi trường Testing.

Cuối Program.cs trong taskmanager.Server cần:

public partial class Program { }

CI/CD có thể dùng Docker Compose và dotnet test trong container.

📦 Changelog

[v0.2.0] – Docker hoàn chỉnh

✅ Dockerfile cho backend + frontend

✅ docker‑compose với PostgreSQL

✅ .env hỗ trợ

[v0.1.0] – Cài đặt ban đầu

ASP.NET Core Web API

React + Vite frontend

EF Core + PostgreSQL

Maintainer: hoangconglock15@gmail.com
