# Stage 1: Build
FROM node:20-alpine AS builder
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

# Stage 2: Serve
FROM node:20-alpine
RUN npm install -g serve
COPY --from=builder /app/dist /app
ENV PORT=3000
EXPOSE 3000
CMD ["sh", "-c", "serve -s /app --listen tcp://0.0.0.0:$PORT"]
