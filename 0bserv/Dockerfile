#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["0bserv.csproj", "."]
RUN dotnet restore "./0bserv.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./0bserv.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./0bserv.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "0bserv.dll"]

# Stage 3: Final image with the application and MSSQL Server
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "0bserv.dll"]

# Set up environment variables for MSSQL Server
ENV ACCEPT_EULA=Y
ENV SA_PASSWORD=YourStrong!Passw0rd

# Install MSSQL Server
RUN apt-get update \
    && apt-get install -y wget \
    && wget -qO- https://packages.microsoft.com/keys/microsoft.asc | apt-key add - \
    && wget -qO- https://packages.microsoft.com/config/debian/10/prod.list > /etc/apt/sources.list.d/mssql-release.list \
    && apt-get update \
    && apt-get install -y --no-install-recommends \
       msodbcsql17 \
       mssql-tools \
       unixodbc-dev \
       curl \
       gnupg \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

# Install .NET SDK and SQL Server Tools
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
       dotnet-sdk-8.0 \
       mssql-tools \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*