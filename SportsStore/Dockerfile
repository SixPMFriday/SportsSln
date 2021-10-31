#### FROM TEXTBOOK

#FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
#FROM mcr.microsoft.com/dotnet/core/sdk:3.1
#COPY /bin/Release/netcoreapp3.1/publish/ SportsStore/
#ENV ASPNETCORE_ENVIRONMENT Production
#EXPOSE 5000
#WORKDIR /SportsStore
#ENTRYPOINT ["dotnet", "SportsStore.dll",  "--urls=http://0.0.0.0:5000"]




#### FROM MS DOCS

#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["./SportsStore.csproj", "SportsStore/"]
RUN dotnet restore "SportsStore/SportsStore.csproj"
COPY . .
WORKDIR "/src/SportsStore"
RUN dotnet build "SportsStore.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SportsStore.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SportsStore.dll", "--urls=http://0.0.0.0:5000"]


