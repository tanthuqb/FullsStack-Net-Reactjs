# fullstack-aspnet-spa

> [!NOTE]  
> This project uses ASP.NET Core Web API as backend, a modern SPA frontend (React/Vite), and PostgreSQL for data. Docker support is included.

---

## 🛠 Installation (Local Dev)

> Requires:
> - .NET SDK 7+
> - Node.js
> - PostgreSQL (or use Docker)
> - Docker Desktop (optional)

### Backend

```bash
cd backend
dotnet restore
dotnet run
```

### Frontend

```bash
cd frontend
npm install
npm run dev
```

---

## ⚙️ Database

EF Core + PostgreSQL

### Connection string (`backend/appsettings.Development.json`)

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=mydb;Username=postgres;Password=yourpassword"
}
```

### EF CLI commands

```bash
dotnet ef migrations add Init --project backend --startup-project backend
dotnet ef database update --project backend --startup-project backend
```

---

## 🐳 Docker Support – ✅ Ready

### Features

- Dockerfile for both backend + frontend
- docker-compose for API + Client + PostgreSQL
- .env config support

### ✅ Build & Run Fullstack App

```bash
cp .env.example .env
docker-compose up --build
```

### Services

| Service     | URL                          |
|-------------|------------------------------|
| Frontend    | http://localhost:3000        |
| Backend API | http://localhost:5000/swagger|
| PostgreSQL  | localhost:5432 (`taskmanager` DB)

---

## 📁 Project Structure

```
/
├── backend/
│   ├── Controllers/
│   ├── AppDbContext.cs
│   └── Dockerfile
├── frontend/
│   ├── src/
│   ├── vite.config.ts
│   └── Dockerfile
├── .env
├── docker-compose.yml
└── README.md
```

---

## 🐳 Dockerfile (backend)

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TaskManager.Server.csproj", "."]
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TaskManager.Server.dll"]
```

---

## 🐳 Dockerfile (frontend - Vite React)

```dockerfile
FROM node:18-alpine as build
WORKDIR /app
COPY . .
RUN npm install && npm run build

FROM nginx:stable-alpine
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```

---

## 🐳 docker-compose.yml

```yaml
version: '3.8'

services:
  client:
    build:
      context: ./frontend
      dockerfile: Dockerfile
    ports:
      - "3000:80"
    depends_on:
      - api

  api:
    build:
      context: ./backend
      dockerfile: Dockerfile
    ports:
      - "5000:80"
    environment:
      - ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=taskmanager;Username=postgres;Password=${POSTGRES_PASSWORD}
    depends_on:
      - postgres

  postgres:
    image: postgres:15
    environment:
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: ${POSTGRES_PASSWORD}
      POSTGRES_DB: taskmanager
    ports:
      - "5432:5432"
    volumes:
      - pgdata:/var/lib/postgresql/data

volumes:
  pgdata:
```

---

## 🔐 .env.example

```env
POSTGRES_PASSWORD=123456
```

---

## ✅ Notes

- Uses top-level statements (`Program.cs`)
- Vite auto-reloads on `npm run dev`
- NGINX serves static `dist` in production
- PostgreSQL data is stored in Docker volume
- Compatible with Visual Studio or CLI

---

## 🛠 CI/CD Friendly

Example GitHub Actions or GitLab CI:

```yaml
steps:
  - name: Build
    run: docker-compose -f docker-compose.yml up --build -d

  - name: Run Tests
    run: docker exec backend dotnet test

  - name: Push Image
    run: |
      docker tag backend yourrepo/backend:latest
      docker push yourrepo/backend:latest
```

---

## 📦 Changelog

### [v0.2.0] – Full Docker Support

- ✅ Dockerfile backend + frontend
- ✅ docker-compose with PostgreSQL
- ✅ .env config support

### [v0.1.0] – Initial Setup

- ASP.NET Core Web API
- Vite + React frontend
- EF Core + PostgreSQL integration

---

**Maintainer:** [hoangconglock15@gmail.com](mailto:hoangconglock15@gmail.com)
