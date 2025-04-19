# syntax=docker/dockerfile:1.4

###################################
# 1) Build your React app with Node
###################################
FROM node:18-alpine AS build
WORKDIR /app

# Copy only package files first (for layer cache)
COPY package*.json ./
RUN npm ci

# Copy the rest of your source and build
COPY . .
RUN npm run build         # make sure this outputs into "dist/" for Vite

###################################
# 2) Serve the built files with NGINX
###################################
FROM nginx:stable-alpine
# Overwrite default nginx.conf to enable SPA history‑mode fallback
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copy the build output from `build` stage
COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
