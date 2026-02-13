# =========================
# Build stage
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

# Copy solution and project files first (better Docker layer cache)
COPY AgroSolutions.Identity.API/AgroSolutions.Identity.API.csproj AgroSolutions.Identity.API/
COPY AgroSolutions.Identity.Application/AgroSolutions.Identity.Application.csproj AgroSolutions.Identity.Application/
COPY AgroSolutions.Identity.Domain/AgroSolutions.Identity.Domain.csproj AgroSolutions.Identity.Domain/
COPY AgroSolutions.Identity.Infrastructure/AgroSolutions.Identity.Infrastructure.csproj AgroSolutions.Identity.Infrastructure/

# Restore dependencies
RUN dotnet restore AgroSolutions.Identity.API/AgroSolutions.Identity.API.csproj

# Copy remaining source code
COPY . .

# Publish API project
WORKDIR /src/AgroSolutions.Identity.API
RUN dotnet publish -c Release -o /app/publish --no-restore

# =========================
# Runtime stage (Alpine)
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS runtime
WORKDIR /app

# Install ICU for globalization support
RUN apk add --no-cache icu-libs

# Create non-root user (security best practice)
RUN addgroup -S appgroup && adduser -S appuser -G appgroup
USER appuser

# Copy published output
COPY --from=build /app/publish .

# Expose port for Kubernetes
EXPOSE 8080

# Production environment variables
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Healthcheck (Kubernetes-friendly)
HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "AgroSolutions.Identity.API.dll"]
