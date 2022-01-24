#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AutoReviewGenerator.csproj", "."]
RUN dotnet restore "./AutoReviewGenerator.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "AutoReviewGenerator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AutoReviewGenerator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AutoReviewGenerator.dll"]