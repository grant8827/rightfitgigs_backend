FROM node:20-alpine
RUN npm install -g serve
COPY dist /app
EXPOSE 3000
CMD ["sh", "-c", "serve -s /app --listen tcp://0.0.0.0:${PORT:-3000}"]
