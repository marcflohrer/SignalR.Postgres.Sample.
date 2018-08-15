docker stop $(docker ps -a -q) \
   && docker container prune -f  \
   && docker rmi signalrchat:latest \
   && docker image prune -f
   