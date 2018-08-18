#!/bin/bash
docker container prune -f \
   && docker-compose up -d  \
   && docker image prune -f \
   && docker logs -f stocktickr_stocktickr_1 