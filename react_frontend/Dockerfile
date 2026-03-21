FROM nginx:alpine

# Copy pre-built dist
COPY dist /usr/share/nginx/html

# Copy nginx config template and startup script
COPY nginx.conf /etc/nginx/conf.d/nginx.conf.template
COPY start.sh /start.sh
RUN chmod +x /start.sh

# Remove default nginx config
RUN rm -f /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["/start.sh"]
