FROM microsoft/dotnet:2.1-sdk-alpine3.7 AS build
WORKDIR /src

COPY *.csproj ./
COPY NuGet.config ./
RUN dotnet restore -nowarn:nu1701 -r alpine.3.7-x64
COPY . .
RUN dotnet build -c Release -o /app -r alpine.3.7-x64 --no-restore

FROM build AS publish
RUN dotnet publish -c Release -o /app -r alpine.3.7-x64 --no-restore 

FROM lohrer/aspnetcore-alpine:alpine3.8 as base

# Define the port that is exposed by the application
ENV PORT=8083

ENV ASPNETCORE_URLS=http://+:${PORT} \
    # Enable detection of running in a container
    DOTNET_RUNNING_IN_CONTAINER=true \
    # Since icu_libs (International Components for Unicode library (libraries)) are included
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

EXPOSE ${PORT}

WORKDIR /app
COPY --from=publish /app .
RUN chmod +x /app/StockCollector \
    && find $sysdirs -xdev \( \
       -name chmod -o \
        \) -delete

USER appuser

ENTRYPOINT ["./StockCollector"]
