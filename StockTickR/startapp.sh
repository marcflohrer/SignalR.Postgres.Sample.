#!/bin/bash
date \
   && docker container prune -f \
   && date \
   && docker-compose up -d  \
   && date \
   && docker logs -f stocktickr_stocktickr_1