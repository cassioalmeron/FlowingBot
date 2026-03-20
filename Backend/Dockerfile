# ========================================
# Stage 1: Build
# ========================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files for restore
COPY *.sln .
COPY FlowingBot.Api/FlowingBot.Api.csproj FlowingBot.Api/
COPY FlowingBot.Core/FlowingBot.Core.csproj FlowingBot.Core/
COPY FlowingBot.Console/FlowingBot.Console.csproj FlowingBot.Console/
COPY FlowingBot.Tests/FlowingBot.Tests.csproj FlowingBot.Tests/

# Restore dependencies - SOLUTION LEVEL for multi-project dependencies
RUN dotnet restore *.sln

# Copy source code
COPY . .

# Build the solution - SOLUTION LEVEL to validate all projects
RUN dotnet build *.sln -c Release -o /app/build

# ========================================
# Stage 2: Publish
# ========================================
FROM build AS publish
RUN dotnet publish FlowingBot.Api/FlowingBot.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# ========================================
# Stage 3: Runtime
# ========================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# ARG for configurable port
ARG API_PORT=5000

# Install curl for health checks and clean up apt cache to minimize layer size
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Create data directory for SQLite with proper permissions
RUN mkdir -p /app/data && chmod 755 /app/data

# Copy published application
COPY --from=publish /app/publish .

# Expose application port
EXPOSE ${API_PORT}

# Set environment variables
ENV ASPNETCORE_URLS=http://+:${API_PORT}

# Health check for container orchestration
HEALTHCHECK --interval=30s --timeout=10s --retries=3 --start-period=40s \
    CMD curl -f http://localhost:${API_PORT}/health || exit 1

# Run the application
ENTRYPOINT ["dotnet", "FlowingBot.Api.dll"]
