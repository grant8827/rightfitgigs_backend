#!/bin/sh
# Replace PORT_PLACEHOLDER with the actual $PORT value, then start nginx
sed "s/PORT_PLACEHOLDER/${PORT:-80}/g" /etc/nginx/conf.d/nginx.conf.template > /etc/nginx/conf.d/default.conf
nginx -g "daemon off;"
