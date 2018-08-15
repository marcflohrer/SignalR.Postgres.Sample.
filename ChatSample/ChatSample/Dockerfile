FROM alpine:3.8 as base
RUN echo http://mirror.yandex.ru/mirrors/alpine/v3.8/main > /etc/apk/repositories; \
    echo http://mirror.yandex.ru/mirrors/alpine/v3.8/community >> /etc/apk/repositories

RUN apk update \
    && apk add --no-cache \
        ca-certificates \
        \
        # .NET Core dependencies
        krb5-libs \
        libgcc \
        libintl \
        libssl1.0 \
        libstdc++ \
        tzdata \
        userspace-rcu \
        zlib \
    && apk -X https://nl.alpinelinux.org/alpine/edge/main/ add --no-cache \
        lttng-ust

# Configure Kestrel web server to bind to port 8081 when present
ENV ASPNETCORE_URLS=http://+:8081 \
    # Enable detection of running in a container
    DOTNET_RUNNING_IN_CONTAINER=true \
    # Set the invariant mode since icu_libs isn't included (see https://github.com/dotnet/announcements/issues/20)
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true
WORKDIR /app
EXPOSE 8081

FROM microsoft/dotnet:2.1-sdk-alpine3.7 AS build
WORKDIR /src

COPY *.csproj ./
COPY NuGet.config ./
RUN dotnet restore -nowarn:msb3202,nu1503 ChatSample.csproj
COPY . .
WORKDIR /src/
RUN dotnet build -c Release -o /app -r alpine.3.7-x64 ChatSample.csproj

FROM build AS publish
RUN dotnet publish -c Release -o /app -r alpine.3.7-x64 ChatSample.csproj

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

# set -e: Exit immediately if a command exits with a non-zero status.
RUN set -e \
    #
    # Print a trace of simple commands and their arguments after they are expanded and before they are executed.
    && set -x \
    #
    # Find files with permission setuid and setgid (+6000) and change their permissions to 'all but setuid' (a-s):
    #
    # When set-user identification (setuid) permission is set on an executable file, a process that runs this file 
    # is granted access based on the owner of the file (usually root), rather than the user who is running the executable file
    #
    # The set-group identification (setgid) permission is similar to setuid, except that the process's 
    # effective group ID (GID) is changed to the group owner of the file, and a user is granted access 
    # based on permissions granted to that group.
    && for i in `find / -perm +6000 -type f 2>/dev/null`; do chmod a-s $i; done \
    #
    # This line replaces anything that starts with the name user:!: with user:x:, in-place in /etc/shadow:
    # /etc/shadow contains encrypted password as well as other information such as account or password expiration values
    && sed -i -r "s/^user:!:/user:x:/" /etc/shadow \
    #
    # Be informative after successful login:
    && echo -e "\n\nApp container image built on $(date)." > /etc/motd \
    #
    # Remove existing crontabs, if any:
    #
    # The crontab (cron derives from chronos, Greek for time; tab stands for table) command, found in Unix and Unix-like operating systems,
    # is used to schedule commands to be executed periodically.	
    && rm -fr /var/spool/cron \ 
    && rm -fr /etc/crontabs \
    && rm -fr /etc/periodic \
    #
    # Make /app/ChatSample an executable
    && chmod +x /app/ChatSample \
    #
    # Add a user 'appuser' without privileges in group 'appgroup'
    && addgroup -S appgroup && adduser -S -h /app -G appgroup appuser \
    #
    # Remove unnecessary user accounts.
    && sed -i -r "/^(appuser|root|sshd)/!d" /etc/group \
    && sed -i -r "/^(appuser|root|sshd)/!d" /etc/passwd \
    #
    # Remove interactive login shell for everybody but user.
    && sed -i -r '/^appuser:/! s#^(.*):[^:]*$#\1:/sbin/nologin#' /etc/passwd \ 
    && sysdirs="/bin /etc /lib /sbin /usr " \
    #
    # Remove all but a handful of admin commands:
    && find /sbin /usr/sbin ! -type d \
            -a ! -name login_duo \
            -a ! -name nologin \
            -a ! -name setup-proxy \
            -a ! -name sshd \
            -a ! -name start.sh \
            -delete    

USER appuser

ENTRYPOINT ["/app/ChatSample"]
