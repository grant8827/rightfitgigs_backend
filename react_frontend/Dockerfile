FROM node:20-alpine
RUN npm install -g serve
COPY dist /app
ENV PORT=3000
EXPOSE 3000
CMD ["sh", "-c", "serve -s /app --listen tcp://0.0.0.0:$PORT"]
