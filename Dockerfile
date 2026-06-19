# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy the API project file
COPY ["MorgueManager.API/MorgueManager.API.csproj", "MorgueManager.API/"]
RUN dotnet restore "MorgueManager.API/MorgueManager.API.csproj"

# Copy the rest of the API code
COPY MorgueManager.API/ MorgueManager.API/
WORKDIR "/src/MorgueManager.API"

# Build and publish
RUN dotnet build "MorgueManager.API.csproj" -c Release -o /app/build
RUN dotnet publish "MorgueManager.API.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose port 80 for Render
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

# Chạy migration khi khởi động (tuỳ chọn)
ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "MorgueManager.API.dll"]
