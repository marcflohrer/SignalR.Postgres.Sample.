#!/bin/bash
rm -rf /Users/marc/.db/stocktickr/ \
    && docker rmi stocktickr:latest \
    && docker rmi postgresdb \
    && docker container prune -f \
    && ./buildandstartapp.sh 