# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar apenas o arquivo .csproj primeiro para cache de dependências
RUN free -h
COPY ["Project/RestWithASPNET.csproj", "Project/"]
RUN dotnet restore "Project/RestWithASPNET.csproj"

# Agora, copie os arquivos restantes
COPY . .

# Compilar e publicar
WORKDIR "/src/Project"
RUN dotnet publish "RestWithASPNET.csproj" -c $BUILD_CONFIGURATION -o /app/publish --no-restore

# Final image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "RestWithASPNET.dll"]
