# -------- Build --------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

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
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 80

COPY --from=build /app .
ENTRYPOINT ["dotnet", "FriterieShop.API.dll"]
