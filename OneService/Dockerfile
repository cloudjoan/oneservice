#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        libldap-2.4-2 \
        realmd \
        gss-ntlmssp \
    && rm -rf /var/lib/apt/lists/*

RUN apt-get update \
    &&  DEBIAN_FRONTEND=noninteractive apt-get install -y --no-install-recommends tzdata
    
RUN TZ=Asia/Taipei \
    && ln -snf /usr/share/zoneinfo/$TZ /etc/localtime \
    && echo $TZ > /etc/timezone \
    && dpkg-reconfigure -f noninteractive tzdata 


FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["OneService/OneService.csproj", "OneService/"]
RUN dotnet restore "OneService/OneService.csproj"
COPY . .
WORKDIR "/src/OneService"
RUN dotnet build "OneService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OneService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OneService.dll"]