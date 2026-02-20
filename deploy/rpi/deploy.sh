#!/bin/bash
# =============================================================================
# deploy.sh - Déploiement de FriterieShop sur Raspberry Pi 3
# À exécuter sur le PC (Git Bash / WSL / Linux)
# Usage : bash deploy.sh <ip-du-pi> [utilisateur]
# Exemple : bash deploy.sh 192.168.1.50 pi
# =============================================================================

set -euo pipefail

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

log()  { echo -e "${GREEN}[OK]${NC} $1"; }
warn() { echo -e "${YELLOW}[!]${NC} $1"; }
fail() { echo -e "${RED}[ERREUR]${NC} $1"; exit 1; }

# --- Arguments ---
PI_HOST="${1:-}"
PI_USER="${2:-pi}"
PI_DIR="/home/$PI_USER/friterieshop"

if [ -z "$PI_HOST" ]; then
    echo "Usage : bash deploy.sh <ip-du-pi> [utilisateur]"
    echo "Exemple : bash deploy.sh 192.168.1.50 pi"
    exit 1
fi

# --- Vérification : on est à la racine du projet ---
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"

if [ ! -f "$PROJECT_ROOT/FriterieShop.sln" ]; then
    fail "FriterieShop.sln non trouvé. Lancez ce script depuis le dépôt."
fi

echo "============================================"
echo "  Déploiement FriterieShop → Raspberry Pi"
echo "============================================"
echo "  Cible : $PI_USER@$PI_HOST:$PI_DIR"
echo ""

# --- Répertoire temporaire de publication ---
PUBLISH_DIR="$SCRIPT_DIR/publish"
rm -rf "$PUBLISH_DIR"
mkdir -p "$PUBLISH_DIR/api" "$PUBLISH_DIR/web"

# --- 1. Publier l'API (.NET 10, linux-arm64) ---
log "Publication de l'API (linux-arm64)..."
dotnet publish "$PROJECT_ROOT/FriterieShop.Presentation/FriterieShop.API/FriterieShop.API.csproj" \
    -c Release \
    -r linux-arm64 \
    --self-contained false \
    -o "$PUBLISH_DIR/api"

log "API publiée ($(du -sh "$PUBLISH_DIR/api" | cut -f1))"

# --- 2. Publier le frontend Blazor WASM ---
# Blazor WASM = fichiers statiques (WebAssembly), pas besoin de runtime ARM
log "Publication du frontend Blazor WASM..."
dotnet publish "$PROJECT_ROOT/FriterieShop.Presentation/FriterieShop.Web/FriterieShop.Web.csproj" \
    -c Release \
    -o "$PUBLISH_DIR/web"

log "Frontend publié ($(du -sh "$PUBLISH_DIR/web" | cut -f1))"

# --- 3. Copier les fichiers de config ---
log "Copie des fichiers de configuration..."
cp "$PROJECT_ROOT/FriterieShop.Presentation/FriterieShop.Web/nginx.conf" "$SCRIPT_DIR/nginx.conf"
cp "$PROJECT_ROOT/FriterieShop.Presentation/FriterieShop.Web/Dockers/docker-entrypoint.sh" "$SCRIPT_DIR/docker-entrypoint.sh"

# --- 4. Créer un .env par défaut s'il n'existe pas ---
if [ ! -f "$SCRIPT_DIR/.env" ]; then
    warn "Fichier .env non trouvé, création avec les valeurs par défaut..."
    cat > "$SCRIPT_DIR/.env" << 'ENVEOF'
DB_PASSWORD=postgres
JWT_SECRET_KEY=ChangeMeInProduction_MinLength32Chars!
ENVEOF
    warn "IMPORTANT : Modifiez deploy/rpi/.env avant de déployer en production !"
fi

# --- 5. Transfert vers le Raspberry Pi ---
log "Transfert des fichiers vers $PI_USER@$PI_HOST..."

ssh "$PI_USER@$PI_HOST" "mkdir -p $PI_DIR/publish/api $PI_DIR/publish/web"

# Transfert des fichiers publiés
scp -r "$PUBLISH_DIR/api/"* "$PI_USER@$PI_HOST:$PI_DIR/publish/api/"
log "API transférée"

scp -r "$PUBLISH_DIR/web/"* "$PI_USER@$PI_HOST:$PI_DIR/publish/web/"
log "Frontend transféré"

# Transfert des fichiers Docker et config
scp "$SCRIPT_DIR/docker-compose.rpi.yml" "$PI_USER@$PI_HOST:$PI_DIR/docker-compose.yml"
scp "$SCRIPT_DIR/Dockerfile.api.rpi"     "$PI_USER@$PI_HOST:$PI_DIR/Dockerfile.api.rpi"
scp "$SCRIPT_DIR/Dockerfile.web.rpi"     "$PI_USER@$PI_HOST:$PI_DIR/Dockerfile.web.rpi"
scp "$SCRIPT_DIR/postgres-init.conf"     "$PI_USER@$PI_HOST:$PI_DIR/postgres-init.conf"
scp "$SCRIPT_DIR/nginx.conf"             "$PI_USER@$PI_HOST:$PI_DIR/nginx.conf"
scp "$SCRIPT_DIR/docker-entrypoint.sh"   "$PI_USER@$PI_HOST:$PI_DIR/docker-entrypoint.sh"
scp "$SCRIPT_DIR/.env"                   "$PI_USER@$PI_HOST:$PI_DIR/.env"
log "Fichiers de configuration transférés"

# --- 6. Build et lancement sur le Pi ---
log "Lancement de Docker Compose sur le Raspberry Pi..."
ssh "$PI_USER@$PI_HOST" "cd $PI_DIR && docker compose up -d --build"

# --- 7. Vérification ---
echo ""
log "Déploiement lancé ! Vérification en cours..."
sleep 10

echo ""
echo "--- État des conteneurs ---"
ssh "$PI_USER@$PI_HOST" "cd $PI_DIR && docker compose ps"

echo ""
echo "--- Health check API ---"
if ssh "$PI_USER@$PI_HOST" "curl -sf http://localhost:8080/health" 2>/dev/null; then
    echo ""
    log "API accessible !"
else
    warn "L'API n'est pas encore prête. Elle peut prendre ~60s au premier démarrage."
    warn "Vérifiez avec : ssh $PI_USER@$PI_HOST 'cd $PI_DIR && docker compose logs api'"
fi

# --- Nettoyage local ---
rm -rf "$PUBLISH_DIR"
rm -f "$SCRIPT_DIR/nginx.conf" "$SCRIPT_DIR/docker-entrypoint.sh"
log "Fichiers temporaires nettoyés"

echo ""
echo "============================================"
echo -e "  ${GREEN}Déploiement terminé !${NC}"
echo "============================================"
echo ""
echo "  Frontend : http://$PI_HOST"
echo "  API      : http://$PI_HOST:8080"
echo "  Health   : http://$PI_HOST:8080/health"
echo ""
echo "  Commandes utiles :"
echo "    ssh $PI_USER@$PI_HOST 'cd $PI_DIR && docker compose logs -f'"
echo "    ssh $PI_USER@$PI_HOST 'cd $PI_DIR && docker stats'"
echo "    ssh $PI_USER@$PI_HOST 'cd $PI_DIR && docker compose restart'"
echo ""
