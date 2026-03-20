# Docker Setup Guide for FlowingBot

This guide walks you through setting up and running your FlowingBot application in Docker containers.

## Overview

The Docker setup includes:
- **Backend**: .NET 8.0 ASP.NET Core API with SQLite database
- **Frontend**: React/Vite application served through Nginx
- **Networking**: Docker network for inter-service communication
- **Data Persistence**: Named volume for SQLite database

## Prerequisites

- Docker Engine (version 20.10+)
- Docker Compose (version 2.0+)
- No conflicts on ports 5000 (API) and 80 (Frontend)

## Setup Steps

### Step 1: Configure Environment Variables

Copy the example environment file and configure all variables:

```bash
cp .env.example .env
```

Edit `.env` and configure:
- `API_PORT`: Backend API port (default: 5000)
- `FRONTEND_PORT`: Frontend port (default: 80)
- `ASPNETCORE_ENVIRONMENT`: ASP.NET Core environment (Production/Development)
- `OLLAMA_MODEL_NAME`: Ollama model to use (e.g., llama3.2:latest)
- `OPENAI_API_KEY`: OpenAI API key (optional)
- `OPENAI_MODEL_NAME`: OpenAI model name (optional)
- `VITE_API_BASE_URL`: Frontend API endpoint URL

**Important**: The `.env` file is **REQUIRED** and must be configured before running containers.

### Step 2: Build and Run Containers

```bash
docker-compose up --build
```

This will:
1. Build the backend Docker image
2. Build the frontend Docker image
3. Create the named volume for persistent data
4. Start both services and establish networking

### Step 3: Verify Services

**Backend API**: http://localhost:5000
- Health endpoint: http://localhost:5000/health
- Swagger UI: http://localhost:5000/swagger (in Development mode)

**Frontend**: http://localhost:80
- React application with API proxy to backend

## Docker Configuration Details

### Backend (Dockerfile in `./Backend`)

**Multi-stage build process**:
1. **Stage 1 (Build)**: SDK image for restore and build
2. **Stage 2 (Publish)**: Publish stage with optimizations
3. **Stage 3 (Runtime)**: Minimal runtime image

**Features**:
- Solution-wide restore/build for proper dependency handling
- Health check endpoint: `/health`
- SQLite data persisted in named volume `/app/data`
- Configurable port via `API_PORT` ARG from `.env`

### Frontend (Dockerfile in `./Frontend`)

**Multi-stage build process**:
1. **Stage 1 (Build)**: Node 20 Alpine for npm install and build
2. **Stage 2 (Runtime)**: Nginx Alpine for serving static files

**Features**:
- Environment variables passed to Vite build
- Nginx configured for SPA routing (react-router-dom support)
- API proxy configuration routes `/api/*` requests to backend
- Static asset caching headers

### Nginx Configuration (`./Frontend/nginx.conf`)

- SPA routing: All unknown routes serve `/index.html`
- API proxy: Requests to `/api/*` proxied to backend service
- Gzip compression for text assets
- Cache headers for immutable assets (1 year)
- Security headers (X-Frame-Options, X-Content-Type-Options, X-XSS-Protection)

### Docker Compose (`docker-compose.yml`)

**Services**:
- **backend**: .NET API service
  - Port: `${API_PORT}:${API_PORT}`
  - Volumes: Named volume `backend-data:/app/data` for SQLite persistence
  - Health check: Tests `/health` endpoint
  - Restart policy: `unless-stopped`

- **frontend**: React/Nginx service
  - Port: `${FRONTEND_PORT}:80`
  - Depends on: `backend` (with health check condition)
  - Restart policy: `unless-stopped`

**Network**: `flowingbot-network` (bridge driver)
- Allows `frontend` service to reach `backend` service at `http://backend:${API_PORT}`

**Volumes**:
- `backend-data`: Named volume for persistent SQLite data

## Environment Variables Reference

| Variable | Default | Description |
|----------|---------|-------------|
| `APP_NAME` | FlowingBot | Application name (used in container names) |
| `API_PORT` | 5000 | Backend API port |
| `FRONTEND_PORT` | 80 | Frontend HTTP port |
| `ASPNETCORE_ENVIRONMENT` | Production | ASP.NET Core environment mode |
| `DB_PATH` | /app/data/FlowingBot.db | SQLite database path inside container |
| `OLLAMA_MODEL_NAME` | llama3.2:latest | Ollama model name |
| `OPENAI_API_KEY` | (empty) | OpenAI API key |
| `OPENAI_MODEL_NAME` | (empty) | OpenAI model name |
| `VITE_API_BASE_URL` | http://localhost:5000/api | Frontend API endpoint |
| `VITE_APP_NAME` | FlowingBot | Frontend application name |

## Common Tasks

### View Logs

```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f backend
docker-compose logs -f frontend
```

### Stop Containers

```bash
docker-compose down
```

### Rebuild Containers

```bash
docker-compose up --build --force-recreate
```

### Access Backend Bash

```bash
docker-compose exec backend bash
```

### Access Frontend Bash

```bash
docker-compose exec frontend sh
```

### Clean Up Everything (Including Data)

```bash
docker-compose down -v
```

## Troubleshooting

### Containers Won't Start
1. Verify `.env` file exists and has all required variables
2. Check port availability: `docker-compose up` may fail if ports are in use
3. Run `docker-compose logs` to see startup errors

### Database Errors
1. SQLite data is persisted in the `backend-data` volume
2. To reset database: `docker-compose down -v` (removes all volumes)
3. Data directory permissions: Set to `755` for SQLite access

### Frontend Can't Connect to Backend
1. Verify `VITE_API_BASE_URL` in `.env` matches `API_PORT`
2. Check nginx proxy configuration in `Frontend/nginx.conf`
3. Ensure backend service is healthy: `docker-compose ps`

### API Health Check Failing
1. Check backend container logs: `docker-compose logs backend`
2. Verify API is responding: `curl http://localhost:5000/health`
3. Check `API_PORT` configuration in `.env`

## Development vs Production

### Development Mode

Set in `.env`:
```
ASPNETCORE_ENVIRONMENT=Development
```

This enables:
- Swagger/OpenAPI UI at `/swagger`
- Detailed error messages
- Development logging

### Production Mode

Set in `.env`:
```
ASPNETCORE_ENVIRONMENT=Production
```

This:
- Disables Swagger UI
- Minimizes error details
- Optimizes performance

## Additional Resources

- [Docker Documentation](https://docs.docker.com/)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [ASP.NET Core Docker Guide](https://learn.microsoft.com/en-us/dotnet/architecture/containerized-lifecycle/design-develop-containerized-apps/docker-apps-development-environment)
- [Vite Documentation](https://vitejs.dev/)

## Notes

- The backend uses SQLite with data persisted in a Docker named volume
- Environment variables are case-sensitive in Linux containers
- The frontend proxies API requests to `http://backend:${API_PORT}` within the Docker network
- Cross-origin requests (CORS) are configured to allow all origins in the backend
