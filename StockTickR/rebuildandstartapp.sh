#!/bin/bash
rm -rf /Users/marc/.db/stocktickr/ \
    && docker container prune -f \
    && ./buildandstartapp.sh 