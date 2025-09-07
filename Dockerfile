# ===== Build stage =====
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY SandStats.csproj ./
RUN dotnet restore SandStats.csproj

COPY . .
RUN dotnet publish SandStats.csproj -c Release -o /app

# ===== Runtime stage =====
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "SandStats.dll"]
