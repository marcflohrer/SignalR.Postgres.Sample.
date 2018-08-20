#!/bin/bash
date
docker-compose build \
   && date \
   && ./startapp.sh \
   && date 