# Use the official .NET SDK image to build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["QuranSchool.slnx", "./"]
COPY ["QuranSchool.API/QuranSchool.API.csproj", "QuranSchool.API/"]
COPY ["QuranSchool.Application/QuranSchool.Application.csproj", "QuranSchool.Application/"]
COPY ["QuranSchool.Domain/QuranSchool.Domain.csproj", "QuranSchool.Domain/"]
COPY ["QuranSchool.Infrastructure/QuranSchool.Infrastructure.csproj", "QuranSchool.Infrastructure/"]
COPY ["Directory.Build.props", "./"]

# Restore dependencies
RUN dotnet restore "QuranSchool.API/QuranSchool.API.csproj"

# Copy the rest of the source code
COPY . .

# Build the application
WORKDIR "/src/QuranSchool.API"
RUN dotnet build "QuranSchool.API.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "QuranSchool.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage: Run the application
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QuranSchool.API.dll"]
