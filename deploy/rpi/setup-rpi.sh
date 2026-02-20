#!/bin/bash
# =============================================================================
# setup-rpi.sh - Préparation du Raspberry Pi 3 pour FriterieShop
# À exécuter sur le Raspberry Pi (Raspberry Pi OS 64-bit)
# Usage : sudo bash setup-rpi.sh
# =============================================================================

set -euo pipefail

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

log()  { echo -e "${GREEN}[OK]${NC} $1"; }
warn() { echo -e "${YELLOW}[!]${NC} $1"; }
fail() { echo -e "${RED}[ERREUR]${NC} $1"; exit 1; }

# --- Vérifications préliminaires ---
if [ "$(id -u)" -ne 0 ]; then
    fail "Ce script doit être exécuté en root (sudo)"
fi

ARCH=$(uname -m)
if [ "$ARCH" != "aarch64" ]; then
    fail "Architecture détectée : $ARCH. Il faut un OS 64-bit (aarch64).
    Flashez Raspberry Pi OS 64-bit Lite avec Raspberry Pi Imager."
fi

echo "============================================"
echo "  Setup Raspberry Pi pour FriterieShop"
echo "============================================"
echo ""

# --- 1. Mise à jour du système ---
log "Mise à jour du système..."
apt-get update && apt-get upgrade -y

# --- 2. Configuration du swap (2 Go) ---
log "Configuration du swap à 2 Go..."
if [ -f /etc/dphys-swapfile ]; then
    # Arrêter le swap actuel
    dphys-swapfile swapoff || true

    # Configurer 2 Go de swap
    sed -i 's/^CONF_SWAPSIZE=.*/CONF_SWAPSIZE=2048/' /etc/dphys-swapfile

    # Redémarrer le swap
    dphys-swapfile setup
    dphys-swapfile swapon
    log "Swap configuré à 2048 Mo"
else
    warn "/etc/dphys-swapfile non trouvé, création d'un swapfile manuel..."
    fallocate -l 2G /swapfile
    chmod 600 /swapfile
    mkswap /swapfile
    swapon /swapfile
    echo '/swapfile none swap sw 0 0' >> /etc/fstab
    log "Swapfile de 2 Go créé"
fi

# Vérification
free -h | grep -i swap
echo ""

# --- 3. Installation de Docker ---
if command -v docker &> /dev/null; then
    log "Docker est déjà installé : $(docker --version)"
else
    log "Installation de Docker..."
    curl -fsSL https://get.docker.com | sh

    # Ajouter l'utilisateur courant au groupe docker
    REAL_USER="${SUDO_USER:-pi}"
    usermod -aG docker "$REAL_USER"
    log "Docker installé. Utilisateur '$REAL_USER' ajouté au groupe docker."
fi

# Activer Docker au démarrage
systemctl enable docker
systemctl start docker

# --- 4. Installation de Docker Compose (plugin) ---
if docker compose version &> /dev/null; then
    log "Docker Compose est déjà installé : $(docker compose version)"
else
    log "Installation du plugin Docker Compose..."
    apt-get install -y docker-compose-plugin
    log "Docker Compose installé."
fi

# --- 5. Création des répertoires ---
DEPLOY_DIR="/home/${SUDO_USER:-pi}/friterieshop"
log "Création du répertoire de déploiement : $DEPLOY_DIR"
mkdir -p "$DEPLOY_DIR"
chown "${SUDO_USER:-pi}:${SUDO_USER:-pi}" "$DEPLOY_DIR"

# --- 6. Installation d'utilitaires ---
log "Installation des utilitaires (curl, ufw)..."
apt-get install -y curl ufw

# --- 7. Configuration du pare-feu ---
log "Configuration du pare-feu (ufw)..."
ufw allow 22/tcp    comment 'SSH'
ufw allow 80/tcp    comment 'HTTP - Frontend Blazor'
ufw allow 8080/tcp  comment 'API .NET'

# Activer ufw si pas déjà actif
if ufw status | grep -q "inactive"; then
    warn "Activation du pare-feu. Assurez-vous que SSH (port 22) est autorisé !"
    echo "y" | ufw enable
fi

ufw status verbose
echo ""

# --- 8. Optimisations système ---
log "Optimisations système pour Docker..."

# Réduire le swappiness (utiliser le swap seulement si nécessaire)
echo 'vm.swappiness=10' > /etc/sysctl.d/99-friterieshop.conf
sysctl -p /etc/sysctl.d/99-friterieshop.conf

# --- Résumé ---
echo ""
echo "============================================"
echo -e "  ${GREEN}Setup terminé avec succès !${NC}"
echo "============================================"
echo ""
echo "Prochaines étapes :"
echo "  1. Déconnectez-vous et reconnectez-vous (pour le groupe docker)"
echo "  2. Sur votre PC, lancez : bash deploy.sh <ip-du-pi>"
echo "  3. Vérifiez avec : curl http://localhost:8080/health"
echo ""
echo "Répertoire de déploiement : $DEPLOY_DIR"
echo ""
