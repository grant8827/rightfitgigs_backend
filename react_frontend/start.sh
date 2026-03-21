#!/bin/sh
set -e

# Write nginx config directly - ${PORT} is shell-expanded, \$uri etc are nginx vars
cat > /etc/nginx/conf.d/default.conf << EOF
server {
    listen ${PORT};
    root /usr/share/nginx/html;
    index index.html;

    location / {
        try_files \$uri \$uri/ /index.html;
    }
}
EOF

echo "nginx config written for port ${PORT}:"
cat /etc/nginx/conf.d/default.conf
exec nginx -g "daemon off;"
