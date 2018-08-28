
# Prerequisite

* docker installation
* top-level .env file with a password below the following key: SA_PASSWORD 
* * For the current password rules see here: <https://docs.microsoft.com/en-us/sql/relational-databases/security/password-policy?view=sql-server-2017>

Ensure that the path to your persistent path is added to the file sharing path list of docker.
See here for more: <https://docs.docker.com/docker-for-mac/osxfs/#namespaces>

# StockTickR Server

The server application is located in the `StockTickRApp` folder.

Run this script to build and start the app:

```bash
#!/bin/bash
 ./buildandstartapp.sh
```

Run this script to start the app when the app is already built:

```bash
#!/bin/bash
 ./startapp.sh
```

To stop the app run this script:
```
$ ./stopapp.sh
```

Application is hosted on `http://localhost:80` by default.

# Node Client

The node client is located in the `nodeClient` folder.
Instructions are in the README there.

# LICENSE

Copyright 2018 Marc Lohrer

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.