#!/bin/bash
docker container prune -f && docker rmi stocktickr:latest && ./buildandstartapp.sh 