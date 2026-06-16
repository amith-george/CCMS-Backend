FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy solution and project files
COPY *.slnx .
COPY src/CCMS.API/*.csproj ./src/CCMS.API/
COPY src/CCMS.Application/*.csproj ./src/CCMS.Application/
COPY src/CCMS.Domain/*.csproj ./src/CCMS.Domain/
COPY src/CCMS.Infrastructure/*.csproj ./src/CCMS.Infrastructure/
COPY tests/CCMS.API.Tests/*.csproj ./tests/CCMS.API.Tests/
COPY tests/CCMS.Domain.Tests/*.csproj ./tests/CCMS.Domain.Tests/
COPY tests/CCMS.Infrastructure.Tests/*.csproj ./tests/CCMS.Infrastructure.Tests/

# Restore dependencies
RUN dotnet restore src/CCMS.API/CCMS.API.csproj

# Copy all source code
COPY . .

# Build and publish
WORKDIR /app/src/CCMS.API
RUN dotnet publish -c Release -o /out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /out .

# Expose port and run
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "CCMS.API.dll"]
