#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["SelfServiceLibrary.BG/SelfServiceLibrary.BG.csproj", "SelfServiceLibrary.BG/"]
COPY ["SelfServiceLibrary.BL/SelfServiceLibrary.BL.csproj", "SelfServiceLibrary.BL/"]
COPY ["SelfServiceLibrary.DAL/SelfServiceLibrary.DAL.csproj", "SelfServiceLibrary.DAL/"]
COPY ["SelfServiceLibrary.Email/SelfServiceLibrary.Email.csproj", "SelfServiceLibrary.Email/"]
COPY ["SelfServiceLibrary.Mapping/SelfServiceLibrary.Mapping.csproj", "SelfServiceLibrary.Mapping/"]
COPY ["CVUT.Usermap/CVUT.Usermap.csproj", "CVUT.Usermap/"]
COPY ["CVUT.Auth/CVUT.Auth.csproj", "CVUT.Auth/"]
COPY ["SelfServiceLibrary.CSV/SelfServiceLibrary.CSV.csproj", "SelfServiceLibrary.CSV/"]
RUN dotnet restore "SelfServiceLibrary.BG/SelfServiceLibrary.BG.csproj"
COPY . .
WORKDIR "/src/SelfServiceLibrary.BG"
RUN dotnet build "SelfServiceLibrary.BG.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SelfServiceLibrary.BG.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SelfServiceLibrary.BG.dll"]