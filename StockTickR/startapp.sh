#!/bin/bash
rm -rf ./StockTickR/obj/ \
   && rm -rf ./StockTickR/bin/ \
   && rm -rf ./CsharpClient/obj/ \
   && rm -rf ./CsharpClient/bin/ \
   && date \
   && docker container prune -f \
   && date \
   && docker-compose up -d  \
   && date \
   && docker logs -f stocktickr_stocktickr_1 \
   && date 