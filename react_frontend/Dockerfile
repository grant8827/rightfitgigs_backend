FROM nginx:alpine

# Remove default nginx config
RUN rm -f /etc/nginx/conf.d/default.conf

# Copy startup script and pre-built dist
COPY start.sh /start.sh
RUN chmod +x /start.sh
COPY dist /usr/share/nginx/html

EXPOSE 80

# Override nginx's ENTRYPOINT so /start.sh runs directly as PID 1
ENTRYPOINT []
CMD ["/start.sh"]
