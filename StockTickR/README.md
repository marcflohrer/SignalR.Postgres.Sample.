## Prerequisite
* docker installation
* top-level .env file with the following keys: POSTGRES_PASSWORD, POSTGRES_USER, POSTGRES_DB

Ensure that the path to your persistent path is added to the file sharing path list of docker.
See here for more: https://docs.docker.com/docker-for-mac/osxfs/#namespaces

## StockTickR Server
The server application is located in the `StockTickRApp` folder.

Run this script to build and start the app:
```
$ ./buildandstartapp.sh
```

Run this script to start the app when the app is already built:
```
$ ./startapp.sh
```

To stop the app run this script:
```
$ ./stopapp.sh
```

Application is hosted on `http://localhost:80` by default.

## Node Client
The node client is located in the `nodeClient` folder.
Instructions are in the README there.
