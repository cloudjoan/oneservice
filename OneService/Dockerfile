#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["OneService/OneService.csproj", "OneService/"]
#RUN curl https://api.nuget.org/v3/index.json -k
#RUN dotnet restore "OneService/OneService.csproj"
COPY . .
WORKDIR "/src/OneService"
#RUN dotnet build "OneService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OneService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OneService.dll"]