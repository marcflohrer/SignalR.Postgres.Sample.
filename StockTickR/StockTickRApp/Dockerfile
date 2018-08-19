FROM microsoft/dotnet:2.1-sdk-alpine3.7 AS build
WORKDIR /src

COPY *.csproj ./
COPY NuGet.config ./
RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/
RUN dotnet build -c Release -o /app -r alpine.3.7-x64 

FROM build AS publish
RUN dotnet publish -c Release -o /app -r alpine.3.7-x64 

FROM lohrer/aspnetcore-alpine:alpine3.8 as base

# Define the port that is exposed by the application
ENV PORT=8081
# Define the name of the application
ENV EXE=/StockTickR
# Define the working directory inside the container
ENV WD=/app

ENV ASPNETCORE_URLS=http://+:${PORT} \
    # Enable detection of running in a container
    DOTNET_RUNNING_IN_CONTAINER=true \
    # Set the invariant mode since icu_libs isn't included (see [dotnet issue #20](https://github.com/dotnet/announcements/issues/20))
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true \
    POSTGRES_USER=${POSTGRES_USER} \
    POSTGRES_PASSWORD=${POSTGRES_PASSWORD} \
    POSTGRES_PASSWORD=${POSTGRES_PASSWORD}

EXPOSE ${PORT}

WORKDIR ${WD}
COPY --from=publish ${WD} .
RUN chmod +x ${WD}${EXE} \
    && find $sysdirs -xdev \( \
       -name chmod -o \
        \) -delete

USER appuser

ENTRYPOINT ${WD}${EXE}
