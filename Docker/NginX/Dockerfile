FROM nginx:latest
 
COPY planitapi.conf /etc/nginx/conf.d
 
EXPOSE 8081
EXPOSE 81
 
CMD ["nginx", "-g", "daemon off;"]