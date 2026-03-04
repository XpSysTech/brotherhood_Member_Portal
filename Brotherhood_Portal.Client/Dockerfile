# ---------- BUILD ----------
FROM node:20.11-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm install

COPY . .
RUN npx ng build Brotherhood_Portal.Client --configuration=docker

# ---------- SERVE ----------
FROM nginx:1.27-alpine

COPY --from=build /app/dist/brotherhood-portal.client/browser /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]