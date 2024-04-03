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

# Install MSSQL Server
# Here, you may want to replace the password with a strong, secure password
ENV SA_PASSWORD=YourStrong!Passw0rd
ENV ACCEPT_EULA=Y
RUN apt-get update && apt-get install -y curl gnupg
RUN curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add -
RUN curl https://packages.microsoft.com/config/ubuntu/20.04/mssql-server-2019.list | tee /etc/apt/sources.list.d/mssql-server.list
RUN apt-get update && apt-get install -y mssql-server
CMD /opt/mssql/bin/sqlservr
EXPOSE 1433