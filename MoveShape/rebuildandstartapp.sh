#!/bin/bash
docker container prune -f && docker rmi moveshape:latest && ./buildandstartapp.sh 