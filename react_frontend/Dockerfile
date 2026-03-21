FROM nginx:alpine

# Copy pre-built dist to nginx html folder
COPY dist /usr/share/nginx/html

# SPA routing: serve index.html for all unknown routes
RUN echo 'server { 
  listen $PORT; 
  root /usr/share/nginx/html; 
  index index.html; 
  location / { 
    try_files $uri $uri/ /index.html; 
  } 
}' > /etc/nginx/templates/default.conf.template

# Railway sets $PORT dynamically
ENV PORT=8080
EXPOSE 8080

CMD ["nginx", "-g", "daemon off;"]
