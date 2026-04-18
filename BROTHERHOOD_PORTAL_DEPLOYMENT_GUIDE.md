# Brotherhood Portal Deployment Guide

## Overview

This repository contains the Brotherhood Portal stack:

* **Frontend:** Angular
* **Backend:** ASP.NET Core API
* **Database:** PostgreSQL
* **Reverse Proxy:** Nginx
* **Containers:** Docker Compose
* **Auth:** JWT
* **Optional Admin Tool:** pgAdmin

Use this guide to set up locally or deploy to a VPS.

---

# 1. Prerequisites (VPS)

Recommended server:

* Ubuntu 24.04 LTS
* 2 vCPU minimum
* 4GB RAM recommended
* 40GB+ SSD

Install packages:

```bash
sudo apt update && sudo apt upgrade -y
sudo apt install -y git curl ufw
```

Install Docker:

```bash
curl -fsSL https://get.docker.com | sh
sudo usermod -aG docker $USER
newgrp docker
```

Install Docker Compose plugin:

```bash
docker compose version
```

---

# 2. Clone Repository

```bash
git clone <YOUR_GITHUB_REPO_URL>
cd <YOUR_PROJECT_FOLDER>
```

---

# 3. Environment File Setup

Create `.env` in project root:

```env
POSTGRES_DB=brotherhood_db
POSTGRES_USER=boena_postgres
POSTGRES_PASSWORD=CHANGE_ME_STRONG_PASSWORD
JWT_TOKEN_KEY=CHANGE_ME_LONG_RANDOM_SECRET
JWT_ISSUER=Brotherhood
JWT_AUDIENCE=BrotherhoodUsers
JWT_EXPIRY_MINUTES=15
SETUP_KEY=CHANGE_ME_SETUP_SECRET
CLIENT_URL=https://yourdomain.com
ASPNETCORE_ENVIRONMENT=Production
EnableSwagger=false
```

---

# 4. Docker Compose Deployment

Start all services:

```bash
docker compose up -d --build
```

Check status:

```bash
docker ps
```

Check logs:

```bash
docker logs boena-api
```

---

# 5. Angular Frontend Notes

Expected production environment values:

```ts
apiBaseUrl: 'https://yourdomain.com/api/v1/'
graphQLBaseUrl: 'https://yourdomain.com/graphql'
```

If building manually:

```bash
cd Brotherhood_Portal.Client
npm install
npm run build
```

---

# 6. ASP.NET Core API Notes

API runs in container and should expose:

* REST: `/api/v1/...`
* GraphQL: `/graphql`
* Health: `/health`

Test:

```bash
curl http://localhost:5001/health
```

---

# 7. PostgreSQL Notes

Database runs in Docker volume for persistence.

Connect:

```bash
docker exec -it boena-postgres psql -U boena_postgres -d brotherhood_db
```

List tables:

```sql
\dt
```

Backup:

```bash
docker exec -t boena-postgres pg_dump -U boena_postgres brotherhood_db > backup.sql
```

Restore:

```bash
cat backup.sql | docker exec -i boena-postgres psql -U boena_postgres -d brotherhood_db
```

---

# 8. Nginx Reverse Proxy

Should route:

* `/` -> Angular frontend
* `/api/` -> API
* `/graphql` -> API

After config changes:

```bash
docker restart global-nginx
```

---

# 9. SSL Setup (Recommended)

Use Certbot or Nginx Proxy Manager.

Example:

```bash
sudo apt install certbot
```

---

# 10. Common Commands

Restart stack:

```bash
docker compose restart
```

Rebuild stack:

```bash
docker compose down
docker compose up -d --build
```

Stop stack:

```bash
docker compose down
```

---

# 11. Readiness Checklist

* [ ] Domain pointed to VPS IP
* [ ] `.env` created
  n- [ ] Docker installed
* [ ] Containers healthy
* [ ] Swagger tested privately
* [ ] GraphQL tested
* [ ] Login tested
* [ ] SSL enabled
* [ ] Backups configured
* [ ] Firewall enabled

---

# 12. Security Checklist

```bash
sudo ufw allow OpenSSH
sudo ufw allow 80
sudo ufw allow 443
sudo ufw enable
```

Also:

* Disable Swagger publicly after testing
* Rotate secrets regularly
* Keep Ubuntu updated

---

# 13. Troubleshooting

## API not starting

```bash
docker logs boena-api
```

## DB connection issue

Check `.env` values and compose connection string.

## Frontend 404 routes

Ensure Nginx uses SPA fallback:

```nginx
try_files $uri $uri/ /index.html;
```

---

# 14. Deployment Flow

1. Push code to GitHub
2. Pull on VPS
3. Create `.env`
4. `docker compose up -d --build`
5. Test app
6. Add SSL
7. Enable backups

---

# 15. Future Improvements

* CI/CD with GitHub Actions
* Automated backups to cloud storage
* Monitoring (Uptime Kuma / Grafana)
* Staging environment
* Blue/Green deployments
