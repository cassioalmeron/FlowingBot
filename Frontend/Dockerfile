# ========================================
# Stage 1: Build
# ========================================
FROM node:20-alpine AS build

WORKDIR /app
COPY package*.json ./
RUN npm ci

COPY . .

# Accept build arguments for Vite environment variables
# Placed AFTER COPY . . to optimize layer caching
# Changing these values won't invalidate npm ci cache
ARG VITE_API_BASE_URL=http://localhost:5000/api
ARG VITE_APP_NAME=FlowingBot
ENV VITE_API_BASE_URL=${VITE_API_BASE_URL}
ENV VITE_APP_NAME=${VITE_APP_NAME}

RUN npm run build

# ========================================
# Stage 2: Runtime with Nginx
# ========================================
FROM nginx:alpine

# Install envsubst to replace environment variables in nginx config
RUN apk add --no-cache gettext

# Copy nginx configuration template
COPY nginx.conf /etc/nginx/conf.d/default.conf.template

# Copy built application
COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 80

# Set default API_PORT, can be overridden by environment variable
ENV API_PORT=5000

# Use shell form to allow environment variable substitution
CMD /bin/sh -c 'sed "s/__API_PORT__/$API_PORT/g" /etc/nginx/conf.d/default.conf.template > /etc/nginx/conf.d/default.conf && nginx -g "daemon off;"'
