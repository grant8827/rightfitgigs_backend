FROM node:20-alpine AS build
WORKDIR /app

COPY package.json package-lock.json ./
RUN npm install

COPY . .
RUN npm run build

FROM node:20-alpine AS runtime
WORKDIR /app

RUN npm install -g serve

COPY --from=build /app/dist ./dist
COPY --from=build /app/public/serve.json ./dist/serve.json

EXPOSE 3000

CMD ["sh", "-c", "serve -s dist --listen tcp://0.0.0.0:${PORT:-3000}"]
