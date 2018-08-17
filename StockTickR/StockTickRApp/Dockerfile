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

ENV PORT=8081
ENV EXE=/app/StockTickR

ENV ASPNETCORE_URLS=http://+:${PORT} \
    # Enable detection of running in a container
    DOTNET_RUNNING_IN_CONTAINER=true \
    # Set the invariant mode since icu_libs isn't included (see [dotnet issue #20](https://github.com/dotnet/announcements/issues/20))
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true
EXPOSE ${PORT}

WORKDIR /app
COPY --from=publish /app .
RUN chmod +x ${EXE} \
    && find $sysdirs -xdev \( \
       -name chmod -o \
        \) -delete

USER appuser
ENTRYPOINT ${EXE}
