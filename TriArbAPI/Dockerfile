FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["TriArbAPI/TriArbAPI.csproj", "TriArbAPI/"]

RUN dotnet restore "TriArbAPI/TriArbAPI.csproj"
COPY . .
WORKDIR "/src/TriArbAPI"
RUN dotnet build "TriArbAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TriArbAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TriArbAPI.dll"]