#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Techgen/Techgen.csproj", "Techgen/"]
COPY ["Techgen.DAL/Techgen.DAL.csproj", "Techgen.DAL/"]
COPY ["Techgen.Domain/Techgen.Domain.csproj", "Techgen.Domain/"]
COPY ["Techgen.Models/Techgen.Models.csproj", "Techgen.Models/"]
COPY ["Techgen.Common/Techgen.Common.csproj", "Techgen.Common/"]
COPY ["Techgen.ResourceLibrary/Techgen.ResourceLibrary.csproj", "Techgen.ResourceLibrary/"]
COPY ["Techgen.Services/Techgen.Services.csproj", "Techgen.Services/"]
COPY ["Techgen.EmailService/Techgen.EmailService.csproj", "Techgen.EmailService/"]
RUN dotnet restore "Techgen/Techgen.csproj"
COPY . .
WORKDIR "/src/Techgen"
RUN dotnet build "Techgen.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Techgen.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY TechGen.db /app
ENTRYPOINT ["dotnet", "Techgen.dll"]