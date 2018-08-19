#!/bin/bash
rm -rf ./StockTickR/obj/ \
   && rm -rf ./StockTickR/bin/ \
   && rm -rf ./CsharpClient/obj/ \
   && rm -rf ./CsharpClient/bin/ \
   && docker container prune -f \
   && docker-compose up -d  \
   && docker image prune -f \
   && docker logs -f stocktickr_stocktickr_1 