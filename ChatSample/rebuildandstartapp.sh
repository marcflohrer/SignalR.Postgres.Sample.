#!/bin/bash
docker container prune -f && docker rmi chatsample:latest && ./buildandstartapp.sh 