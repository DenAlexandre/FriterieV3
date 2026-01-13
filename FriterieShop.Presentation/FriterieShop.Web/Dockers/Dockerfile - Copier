# -------- Build --------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Installer Node.js (pour npm)
RUN apt-get update && apt-get install -y nodejs npm


COPY FriterieShop.Presentation/FriterieShop.Web/FriterieShop.Web.csproj FriterieShop.Presentation/FriterieShop.Web/
COPY FriterieShop.Presentation/FriterieShop.Web.Shared/FriterieShop.Web.Shared.csproj FriterieShop.Presentation/FriterieShop.Web.Shared/

RUN dotnet restore FriterieShop.Presentation/FriterieShop.Web/FriterieShop.Web.csproj

COPY FriterieShop.Presentation/FriterieShop.Web/ FriterieShop.Presentation/FriterieShop.Web/
COPY FriterieShop.Presentation/FriterieShop.Web.Shared/ FriterieShop.Presentation/FriterieShop.Web.Shared/
RUN dotnet publish FriterieShop.Presentation/FriterieShop.Web/FriterieShop.Web.csproj -c Release -o /app

# -------- Runtime (Nginx) --------
FROM nginx:alpine
WORKDIR /usr/share/nginx/html

RUN rm -rf ./*

COPY --from=build /app/wwwroot .
COPY FriterieShop.Presentation/FriterieShop.Web/nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
