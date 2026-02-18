#!/bin/sh
# Remplacer l'URL API dans appsettings.json au demarrage du conteneur
if [ -n "$API_BASE_URL" ]; then
  sed -i "s|https://localhost:7094/api/|${API_BASE_URL}|g" \
    /usr/share/nginx/html/appsettings.json
  # Aussi dans les fichiers appsettings environnement
  find /usr/share/nginx/html -name "appsettings.*.json" -exec \
    sed -i "s|https://localhost:7094/api/|${API_BASE_URL}|g" {} \;
fi
# Remplacer le port dans nginx.conf (Render injecte $PORT)
export PORT="${PORT:-80}"
envsubst '${PORT}' < /etc/nginx/conf.d/default.conf.template > /etc/nginx/conf.d/default.conf
exec nginx -g 'daemon off;'
