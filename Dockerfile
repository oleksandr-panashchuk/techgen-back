FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src 
COPY ["Techgen/Techgen.csproj","Techgen/"]
COPY ["Techgen.Common/Techgen.Common.csproj","Techgen.Common/"]
COPY ["Techgen.DAL/Techgen.DAL.csproj","Techgen.DAL/"]
COPY ["Techgen.Domain/Techgen.Domain.csproj","Techgen.Domain/"]
COPY ["Techgen.EmailService/Techgen.EmailService.csproj","Techgen.EmailService/"]
COPY ["Techgen.Models/Techgen.Models.csproj","Techgen.Models/"]
COPY ["Techgen.ResourceLibrary/Techgen.ResourceLibrary.csproj","Techgen.ResourceLibrary/"] 
COPY ["Techgen.Services/Techgen.Services.csproj","Techgen.Services/"] 
RUN dotnet restore "Techgen/Techgen.csproj"
COPY . .
WORKDIR "/src/Techgen"
RUN dotnet build "Techgen.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Techgen.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet","Techgen.dll" ]
