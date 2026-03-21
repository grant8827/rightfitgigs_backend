FROM node:20-alpine
WORKDIR /app

# Install serve globally
RUN npm install -g serve

# Copy the pre-built dist folder
COPY dist ./dist

# Expose port
EXPOSE 3000

# Serve the app
CMD ["sh", "-c", "serve -s dist --listen tcp://0.0.0.0:${PORT:-3000}"]
