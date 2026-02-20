# Deploiement FriterieShop sur Raspberry Pi 3

Guide complet pour deployer FriterieShop (API .NET 10 + Blazor WASM + PostgreSQL) sur un Raspberry Pi 3.

## Prerequis

### Materiel
- Raspberry Pi 3 Model B (1 Go RAM)
- Carte microSD 16 Go minimum (32 Go recommande)
- Alimentation 5V / 2.5A
- Connexion reseau (Ethernet recommande)

### OS 64-bit obligatoire

**.NET 10 ne supporte plus ARM 32-bit.** Le Pi 3 a un processeur 64-bit (Cortex-A53), mais beaucoup sont livres avec un OS 32-bit. Il faut imperativement utiliser **Raspberry Pi OS 64-bit**.

Pour verifier votre architecture actuelle :
```bash
uname -m
# aarch64 = 64-bit (OK)
# armv7l  = 32-bit (il faut re-flasher)
```

### PC de developpement
- .NET 10 SDK installe
- Git Bash ou WSL (pour les scripts bash)
- Acces SSH au Raspberry Pi

## Etape 1 : Flasher Raspberry Pi OS 64-bit

1. Telecharger [Raspberry Pi Imager](https://www.raspberrypi.com/software/)
2. Choisir **Raspberry Pi OS Lite (64-bit)** (Debian Bookworm, sans bureau)
   - "Lite" est recommande : pas de bureau graphique = plus de RAM pour Docker
3. Dans les parametres avances (engrenage) :
   - Activer SSH
   - Definir le nom d'utilisateur et mot de passe
   - Configurer le Wi-Fi si necessaire
4. Flasher sur la carte microSD
5. Inserer la carte dans le Pi et demarrer

## Etape 2 : Preparer le Raspberry Pi

Connectez-vous au Pi en SSH :
```bash
ssh pi@<ip-du-pi>
```

Transferez et executez le script de setup :
```bash
# Depuis votre PC
scp deploy/rpi/setup-rpi.sh pi@<ip-du-pi>:/tmp/
ssh pi@<ip-du-pi> 'sudo bash /tmp/setup-rpi.sh'
```

Ce script :
- Verifie que l'OS est bien 64-bit
- Met a jour le systeme
- Configure 2 Go de swap (critique avec 1 Go de RAM)
- Installe Docker et Docker Compose
- Configure le pare-feu (ports 22, 80, 8080)
- Cree le repertoire de deploiement

**Important :** Deconnectez-vous et reconnectez-vous apres le setup (pour le groupe Docker).

## Etape 3 : Deployer

### Configuration

Avant le premier deploiement, creez le fichier `deploy/rpi/.env` :
```bash
DB_PASSWORD=VotreMotDePassePostgres
JWT_SECRET_KEY=VotreCleSecretJWTMinimum32Caracteres!
```

### Lancer le deploiement

Depuis la racine du projet, sur votre PC :
```bash
bash deploy/rpi/deploy.sh <ip-du-pi> [utilisateur]
# Exemple :
bash deploy/rpi/deploy.sh 192.168.1.50 pi
```

Le script :
1. Compile l'API pour linux-arm64
2. Compile le frontend Blazor WASM
3. Transfere tout vers le Pi via SCP
4. Lance `docker compose up --build` sur le Pi

Le premier demarrage peut prendre quelques minutes (build des images Docker, creation de la base de donnees).

## Etape 4 : Verification

```bash
# Health check API
curl http://<ip-du-pi>:8080/health

# Frontend Blazor
curl http://<ip-du-pi>

# Logs des conteneurs
ssh pi@<ip-du-pi> 'cd ~/friterieshop && docker compose logs -f'

# Consommation memoire
ssh pi@<ip-du-pi> 'cd ~/friterieshop && docker stats --no-stream'
```

## Architecture du deploiement

```
PC Windows                              Raspberry Pi 3
-----------                             ---------------
1. dotnet publish (linux-arm64)
2. dotnet publish (Blazor WASM)
3. scp fichiers ----------------------> /home/pi/friterieshop/
                                        4. docker compose up --build
                                           +-- postgres:16 (ARM64)  [port 5432]
                                           +-- API .NET 10 (ARM64)  [port 8080]
                                           +-- nginx:alpine (WASM)  [port 80]
```

### Pourquoi cross-compiler ?

Avec 1 Go de RAM, le Pi ne peut pas :
- Executer le SDK .NET (build)
- Faire des multi-stage Docker builds
- Compiler des projets .NET complexes

On compile donc sur le PC (rapide, beaucoup de RAM) et on envoie les binaires prets a l'emploi sur le Pi. Les Dockerfiles sur le Pi sont "runtime-only" (pas de stage de build).

## Commandes utiles

```bash
# Redemarrer les services
ssh pi@<ip-du-pi> 'cd ~/friterieshop && docker compose restart'

# Arreter les services
ssh pi@<ip-du-pi> 'cd ~/friterieshop && docker compose down'

# Voir les logs d'un service
ssh pi@<ip-du-pi> 'cd ~/friterieshop && docker compose logs api'
ssh pi@<ip-du-pi> 'cd ~/friterieshop && docker compose logs postgres'

# Reinitialiser la base de donnees
ssh pi@<ip-du-pi> 'cd ~/friterieshop && docker compose down -v && docker compose up -d --build'

# Espace disque
ssh pi@<ip-du-pi> 'df -h && docker system df'

# Nettoyer les images Docker inutilisees
ssh pi@<ip-du-pi> 'docker system prune -af'
```

## Depannage

### L'API ne demarre pas
```bash
# Verifier les logs
ssh pi@<ip-du-pi> 'cd ~/friterieshop && docker compose logs api'
# Verifier que PostgreSQL est pret
ssh pi@<ip-du-pi> 'cd ~/friterieshop && docker compose logs postgres'
```

### Erreur "exec format error"
L'OS n'est pas en 64-bit. Verifiez avec `uname -m` (doit afficher `aarch64`).

### Out of memory
```bash
# Verifier le swap
ssh pi@<ip-du-pi> 'free -h'
# Le swap doit afficher ~2 Go
# Si ce n'est pas le cas, relancez setup-rpi.sh
```

### Conteneurs qui redemarrent en boucle
```bash
# Regarder les logs pour comprendre
ssh pi@<ip-du-pi> 'cd ~/friterieshop && docker compose logs --tail=50'
# Verifier la memoire
ssh pi@<ip-du-pi> 'docker stats --no-stream'
```

## Limites memoire

Repartition cible pour 1 Go RAM + 2 Go swap :

| Service    | Limite  | Description                |
|------------|---------|----------------------------|
| PostgreSQL | 300 Mo  | Base de donnees            |
| API .NET   | 256 Mo  | Backend                    |
| nginx      | 64 Mo   | Frontend (fichiers statiques) |
| Systeme    | ~380 Mo | OS + Docker daemon         |
| **Total**  | ~1 Go   | RAM physique               |

Le swap de 2 Go absorbe les pics ponctuels.

## Mise a jour

Pour redeployer apres des modifications du code :
```bash
# Depuis votre PC, relancez simplement :
bash deploy/rpi/deploy.sh <ip-du-pi>
```

Le script recompile, retransfere et relance les conteneurs automatiquement.
