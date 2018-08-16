#!/bin/bash
docker container prune -f && docker rmi whiteboard:latest && ./buildandstartapp.sh 