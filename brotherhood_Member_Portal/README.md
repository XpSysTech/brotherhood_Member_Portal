# brotherhood_Member_Portal_Client

> Frontend Operations Guide  
> This document is only for the Angular frontend.

## Overview

Frontend stack:
- Angular
- Tailwind CSS
- Nginx
- Docker (`global-nginx`)

Production:
https://www.boenainvestmentfund.com

## Important Folder Path

```bash
~/apps/brotherhood_Member_Portal/brotherhood_Member_Portal/Brotherhood_Portal.Client
```

## Standard Frontend Update Process

```bash
cd ~/apps/brotherhood_Member_Portal/brotherhood_Member_Portal/Brotherhood_Portal.Client
npx @angular/cli build --configuration production
docker cp dist/brotherhood-portal.client/browser/. global-nginx:/usr/share/nginx/html
docker restart global-nginx
```

Hard refresh browser:

```text
CTRL + SHIFT + R
```

## Production Client Update & Deploy (One Command)

Use this command whenever you make changes to the Angular client/frontend and want to publish the latest version to the live production website.

This will:

1. Open the frontend project folder  
2. Build the Angular app for production  
3. Copy the new compiled files into the Nginx container  
4. Restart Nginx so the live site uses the latest version

```bash
cd ~/apps/brotherhood_Member_Portal/brotherhood_Member_Portal/Brotherhood_Portal.Client && npx @angular/cli build --configuration production && docker cp dist/brotherhood-portal.client/browser/. global-nginx:/usr/share/nginx/html && docker restart global-nginx
```

## Check Containers

```bash
docker ps
```

Expected:
- boena-api
- boena-postgres
- boena-pgadmin
- global-nginx

## Restart Services

```bash
docker restart global-nginx
docker restart boena-api
docker compose restart
```

## View Logs

```bash
docker logs global-nginx
docker logs boena-api
docker logs -f boena-api
```

## Important Files

```text
src/app/client/pages/home/home.component.html
src/styles.css
tailwind.config.js
src/app/app.routes.ts
~/apps/brotherhood_Member_Portal/conf.d/default.conf
```

## Troubleshooting

### Styling Breaks

```bash
npm install
npx @angular/cli build --configuration production
```

### Blank White Page

```bash
docker logs global-nginx
docker logs boena-api
docker exec -it global-nginx ls /usr/share/nginx/html
```

### Angular Routes 404

Use nginx fallback:

```nginx
location / {
    try_files $uri $uri/ /index.html;
}
```

Restart nginx.

### HTTPS Problems

```bash
ls /etc/letsencrypt
docker restart global-nginx
```

## Safe Workflow

1. Backup files
2. Make small changes
3. Build
4. Deploy
5. Test homepage
6. Test login
7. Test API calls
8. Test mobile view

## Notes

- Frontend changes require rebuild.
- Backend changes require rebuilding API container.
- Browser cache may show old files.

## Current Status

- Domain live
- Angular live
- API live
- GraphQL live
- SSL enabled
- Tailwind working
