# Use the ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the project file and restore any dependencies
COPY ["Project/RestWithASPNET.csproj", "Project/"]
RUN dotnet restore "Project/RestWithASPNET.csproj"

# Copy the entire source code to the working directory
COPY . .

# Build the application
WORKDIR "/src/Project"
RUN dotnet build "RestWithASPNET.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "RestWithASPNET.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage to run the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish . 
ENTRYPOINT ["dotnet", "RestWithASPNET.dll"]
