docker container prune -f \
   && docker-compose up -d  \
   && docker image prune -f \
   && docker logs -f $(docker ps -a -q) 