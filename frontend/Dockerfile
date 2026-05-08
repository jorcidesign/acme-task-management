# Etapa 1: Compilación de Angular
FROM node:20-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm ci

COPY . .
RUN npm run build --configuration=production

# Etapa 2: Servir con Nginx
FROM nginx:alpine AS final

# Eliminar el default.conf de Nginx y copiar el nuestro
RUN rm /etc/nginx/conf.d/default.conf
COPY nginx.conf /etc/nginx/conf.d/

# Copiar el build compilado desde la etapa 1
# (Asegúrate de que la ruta dist/frontend/browser coincida con angular.json)
COPY --from=build /app/dist/frontend/browser /usr/share/nginx/html

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]