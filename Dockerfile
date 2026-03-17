FROM node:20-alpine AS build
WORKDIR /app
COPY react_frontend/package*.json react_frontend/
RUN cd react_frontend && npm ci
COPY react_frontend/ react_frontend/
RUN cd react_frontend && npm run build

FROM node:20-alpine
WORKDIR /app
RUN npm install -g serve
COPY --from=build /app/react_frontend/dist ./dist
EXPOSE 3000
CMD ["sh", "-c", "serve dist -l ${PORT:-3000}"]
