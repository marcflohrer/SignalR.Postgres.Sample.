## Prerequisite
* docker installation
* top-level .env file with the following keys: DB_PASSWORD, DB_USER, PATH_TO_PERSISTENT_DB

Ensure that the path to your persistent path is added to the file sharing path list of docker.
See here for more: https://docs.docker.com/docker-for-mac/osxfs/#namespaces

## StockTickR Server
The server application is located in the `StockTickRApp` folder.

To build and run:
```
$ ./buildandstartapp.sh
```

To run when it is already built:
```
$ ./startapp.sh
```

To stop the app:
```
$ ./stopapp.sh
```

Application is hosted on `http://localhost:80` by default.

## Node Client
The node client is located in the `nodeClient` folder.
Instructions are in the README there.
