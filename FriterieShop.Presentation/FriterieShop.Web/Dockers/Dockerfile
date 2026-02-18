# -------- Build --------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

RUN apt-get update && apt-get install -y curl \
    && curl -fsSL https://deb.nodesource.com/setup_20.x | bash - \
    && apt-get install -y nodejs \
    && npm install -g npm@latest

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
COPY FriterieShop.Presentation/FriterieShop.Web/nginx.conf /etc/nginx/conf.d/default.conf.template
COPY FriterieShop.Presentation/FriterieShop.Web/Dockers/docker-entrypoint.sh /docker-entrypoint.sh
RUN chmod +x /docker-entrypoint.sh

EXPOSE 80
ENTRYPOINT ["/docker-entrypoint.sh"]
