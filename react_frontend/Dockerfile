FROM nginx:alpine

# Remove default nginx config
RUN rm -f /etc/nginx/conf.d/default.conf

# Copy nginx template and startup script
COPY nginx.conf /etc/nginx/conf.d/app.conf.template
COPY start.sh /start.sh
RUN chmod +x /start.sh

# Copy pre-built dist
COPY dist /usr/share/nginx/html

ENV PORT=80
EXPOSE 80

CMD ["/start.sh"]
