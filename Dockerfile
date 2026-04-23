FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["VIP-Planning.csproj", "."]
RUN dotnet restore "VIP-Planning.csproj"
COPY . .
RUN dotnet publish "VIP-Planning.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
# Zorg dat de app op poort 80 of 10000 luistert voor Render
ENV ASPNETCORE_URLS=http://+:10000
ENTRYPOINT ["dotnet", "VIP-Planning.dll"]