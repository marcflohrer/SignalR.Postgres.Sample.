#!/bin/bash
docker stop $(docker ps -a -q) \
   && docker container prune -f  \
   && docker network prune -f \
   && docker volume prune -f
  
   