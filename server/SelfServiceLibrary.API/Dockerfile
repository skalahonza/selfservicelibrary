#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["SelfServiceLibrary.API/SelfServiceLibrary.API.csproj", "SelfServiceLibrary.API/"]
COPY ["SelfServiceLibrary.BL/SelfServiceLibrary.BL.csproj", "SelfServiceLibrary.BL/"]
COPY ["SelfServiceLibrary.DAL/SelfServiceLibrary.DAL.csproj", "SelfServiceLibrary.DAL/"]
COPY ["SelfServiceLibrary.CSV/SelfServiceLibrary.CSV.csproj", "SelfServiceLibrary.CSV/"]
COPY ["SelfServiceLibrary.Mapping/SelfServiceLibrary.Mapping.csproj", "SelfServiceLibrary.Mapping/"]
COPY ["CVUT.Usermap/CVUT.Usermap.csproj", "CVUT.Usermap/"]
COPY ["CVUT.Auth/CVUT.Auth.csproj", "CVUT.Auth/"]
COPY ["SelfServiceLibrary.Card.Authentication/SelfServiceLibrary.Card.Authentication.csproj", "SelfServiceLibrary.Card.Authentication/"]
RUN dotnet restore "SelfServiceLibrary.API/SelfServiceLibrary.API.csproj"
COPY . .
WORKDIR "/src/SelfServiceLibrary.API"
RUN dotnet build "SelfServiceLibrary.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SelfServiceLibrary.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SelfServiceLibrary.API.dll"]