FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["./GuidBot/GuidBot.csproj", "GuidBot/"]
RUN dotnet restore "GuidBot/GuidBot.csproj"
COPY . .
WORKDIR "/src/GuidBot"
RUN dotnet build "GuidBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GuidBot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GuidBot.dll"]
