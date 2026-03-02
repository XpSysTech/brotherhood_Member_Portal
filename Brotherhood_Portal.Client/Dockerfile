# ---------- BUILD ----------
FROM node:20.11-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm install

COPY . .
RUN npx ng build Brotherhood_Portal.Client --configuration=docker

# ---------- SERVE ----------
FROM nginx:1.27-alpine

# MUST match angular.json outputPath exactly
COPY --from=build /app/dist/brotherhood-portal.client /usr/share/nginx/html

EXPOSE 80