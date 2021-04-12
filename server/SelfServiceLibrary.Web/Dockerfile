#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["SelfServiceLibrary.Web/SelfServiceLibrary.Web.csproj", "SelfServiceLibrary.Web/"]
COPY ["CVUT.Auth/CVUT.Auth.csproj", "CVUT.Auth/"]
COPY ["SelfServiceLibrary.BL/SelfServiceLibrary.BL.csproj", "SelfServiceLibrary.BL/"]
COPY ["SelfServiceLibrary.DAL/SelfServiceLibrary.DAL.csproj", "SelfServiceLibrary.DAL/"]
COPY ["SelfServiceLibrary.Email/SelfServiceLibrary.Email.csproj", "SelfServiceLibrary.Email/"]
COPY ["CVUT.Usermap/CVUT.Usermap.csproj", "CVUT.Usermap/"]
COPY ["SelfServiceLibrary.CSV/SelfServiceLibrary.CSV.csproj", "SelfServiceLibrary.CSV/"]
COPY ["SelfServiceLibrary.Mapping/SelfServiceLibrary.Mapping.csproj", "SelfServiceLibrary.Mapping/"]
COPY ["SelfServiceLibrary.Card.Authentication/SelfServiceLibrary.Card.Authentication.csproj", "SelfServiceLibrary.Card.Authentication/"]
COPY ["Google.Books.API/Google.Books.API.csproj", "Google.Books.API/"]
RUN dotnet restore "SelfServiceLibrary.Web/SelfServiceLibrary.Web.csproj"
COPY . .
WORKDIR "/src/SelfServiceLibrary.Web"
RUN dotnet build "SelfServiceLibrary.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SelfServiceLibrary.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SelfServiceLibrary.Web.dll"]