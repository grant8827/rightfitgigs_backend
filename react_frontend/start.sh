#!/bin/sh
# Single-quoted '$PORT' prevents shell expansion - envsubst only substitutes PORT
envsubst '$PORT' < /etc/nginx/conf.d/app.conf.template > /etc/nginx/conf.d/default.conf
nginx -g "daemon off;"
