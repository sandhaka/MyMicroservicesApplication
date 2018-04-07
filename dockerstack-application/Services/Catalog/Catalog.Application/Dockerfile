FROM microsoft/aspnetcore:2.0.3

# Copy application files
WORKDIR /app
COPY bin/Debug/netcoreapp2.0/publish /app

EXPOSE 80

ENTRYPOINT /bin/bash -c "dotnet Catalog.Application.dll"