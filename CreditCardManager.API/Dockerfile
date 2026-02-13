FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 5177

ENV ASPNETCORE_URLS=http://+:5177

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["CreditCardManager.API/CreditCardManager.csproj", "CreditCardManager.API/"]
RUN dotnet restore "CreditCardManager.API/CreditCardManager.csproj"
COPY . .
WORKDIR "/src/CreditCardManager.API"
RUN dotnet build "CreditCardManager.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "CreditCardManager.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CreditCardManager.dll"]
