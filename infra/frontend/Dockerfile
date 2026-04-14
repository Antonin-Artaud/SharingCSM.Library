# ── Stage 1 : Build Angular ──────────────────────────────────────────────────
FROM node:22-alpine AS build

WORKDIR /app

COPY package*.json ./
RUN npm ci

COPY . .
RUN npm run build -- --configuration production

# ── Stage 2 : Serve avec nginx ───────────────────────────────────────────────
FROM nginx:alpine

# Supprimer la config nginx par défaut
RUN rm /etc/nginx/conf.d/default.conf

# Template nginx – envsubst est exécuté automatiquement au démarrage
# par le script 20-envsubst-on-templates.sh de l'image nginx
COPY default.conf.template /etc/nginx/templates/default.conf.template

# App Angular buildée
COPY --from=build /app/dist/library-ui/browser /usr/share/nginx/html

EXPOSE 80
