FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY backend.csproj ./
RUN dotnet restore backend.csproj

COPY . ./
RUN dotnet publish backend.csproj -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

EXPOSE 8080

# Use shell form so Railway's $PORT variable expands at container runtime
CMD ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT:-8080} dotnet backend.dll"]