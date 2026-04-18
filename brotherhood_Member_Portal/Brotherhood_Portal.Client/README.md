# Brotherhood Portal Client (Angular)

## Overview

Frontend application for the Brotherhood Portal.

Built with:

* Angular 19
* TypeScript
* Tailwind CSS
* RxJS
* JWT Authentication
* REST + GraphQL integration

---

# 1. Requirements

Install:

* Node.js 22+
* npm 10+
* Angular CLI 19+

Check versions:

```bash
node -v
npm -v
ng version
```

Install Angular CLI globally if needed:

```bash
npm install -g @angular/cli
```

---

# 2. Install Dependencies

Inside client folder:

```bash
cd Brotherhood_Portal.Client
npm install
```

---

# 3. Development Server

Run locally:

```bash
ng serve
```

Open:

```text
http://localhost:4200
```

Hot reload enabled.

---

# 4. Environment Configuration

## Development

`src/environments/environment.ts`

```ts
export const environment = {
  production: false,
  apiBaseUrl: 'http://localhost/api/v1/',
  graphQLBaseUrl: 'http://localhost/graphql'
};
```

## Production Example

```ts
export const environment = {
  production: true,
  apiBaseUrl: 'https://yourdomain.com/api/v1/',
  graphQLBaseUrl: 'https://yourdomain.com/graphql'
};
```

---

# 5. Build Application

```bash
ng build
```

Output:

```text
dist/
```

Production build:

```bash
ng build --configuration production
```

---

# 6. Docker Build (If Containerized)

```bash
docker build -t boena-client .
```

Run:

```bash
docker run -p 4200:80 boena-client
```

---

# 7. Common Angular Commands

Generate component:

```bash
ng generate component features/dashboard
```

Generate service:

```bash
ng generate service core/services/member-service
```

Run tests:

```bash
ng test
```

---

# 8. Tailwind CSS Notes

If styles fail:

```bash
npm install tailwindcss @tailwindcss/postcss postcss
```

Ensure PostCSS config exists.

---

# 9. Auth Flow

Login stores:

* JWT token
* user object

Usually in localStorage:

```text
user
token
```

Logout clears storage.

---

# 10. Deployment Flow

1. Update production environment URLs
2. Run build
3. Copy dist files or build Docker image
4. Serve with Nginx
5. Verify API connectivity

---

# 11. Troubleshooting

## API 404

Check `apiBaseUrl`

## Double slash URLs

Use consistent trailing slash strategy.

## CORS errors

Allow frontend domain in API CORS config.

## Blank page after refresh

Nginx must support Angular SPA fallback:

```nginx
try_files $uri $uri/ /index.html;
```

## Tailwind errors

Check package versions.

---

# 12. Recommended Structure

* `core/` shared services
* `features/` feature modules
* `shared/` reusable UI
* `environments/` config

---

# 13. Pre-Deployment Checklist

* [ ] `ng build` passes
* [ ] API URLs correct
* [ ] Login works
* [ ] Protected routes work
* [ ] Tailwind styles load
* [ ] No console errors
* [ ] Mobile responsive check complete

---

# 14. Useful Commands

```bash
npm install
ng serve
ng build
ng test
npm update
```
