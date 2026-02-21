# -------- Build --------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Installer Node.js (pour npm)
#RUN apt-get update && apt-get install -y nodejs npm
# Installer Node.js LTS
RUN apt-get update && apt-get install -y curl \
    && curl -fsSL https://deb.nodesource.com/setup_20.x | bash - \
    && apt-get install -y nodejs \
    && npm install -g npm@latest

# Copier le csproj et restaurer
COPY FriterieShop.Presentation/FriterieShop.API/FriterieShop.API.csproj FriterieShop.Presentation/FriterieShop.API/
COPY FriterieShop.Infrastructure/FriterieShop.Infrastructure.csproj FriterieShop.Infrastructure/
COPY FriterieShop.ServiceDefaults/FriterieShop.ServiceDefaults.csproj FriterieShop.ServiceDefaults/
COPY FriterieShop.Application/FriterieShop.Application.csproj FriterieShop.Application/
COPY FriterieShop.Domain/FriterieShop.Domain.csproj FriterieShop.Domain/

RUN dotnet restore FriterieShop.Presentation/FriterieShop.API/FriterieShop.API.csproj

# Copier le reste du code
COPY FriterieShop.Presentation/FriterieShop.API/ FriterieShop.Presentation/FriterieShop.API/
COPY FriterieShop.Infrastructure/ FriterieShop.Infrastructure/
COPY FriterieShop.ServiceDefaults/ FriterieShop.ServiceDefaults/
COPY FriterieShop.Application/ FriterieShop.Application/
COPY FriterieShop.Domain/ FriterieShop.Domain/

# Publier
RUN dotnet publish FriterieShop.Presentation/FriterieShop.API/FriterieShop.API.csproj -c Release -o /app


# -------- Runtime --------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app .


# Variables d'environnement
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["dotnet", "FriterieShop.API.dll"]
