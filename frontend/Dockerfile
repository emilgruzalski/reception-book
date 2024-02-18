FROM node:18-alpine AS build
WORKDIR /app

COPY package.json package-lock.json ./
RUN npm ci

COPY . .
RUN npm run build

FROM nginx:1.25-alpine

# Templates are processed with envsubst when the container starts.
COPY nginx/default.conf.template /etc/nginx/templates/default.conf.template
COPY --from=build /app/dist/reception-book /usr/share/nginx/html

ENV API_URL=http://host.docker.internal:8080
EXPOSE 80
