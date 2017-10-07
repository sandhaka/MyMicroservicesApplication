FROM microsoft/aspnetcore:1.1

# Copy application files
WORKDIR /app
COPY bin/Debug/netcoreapp1.1/publish /app

EXPOSE 443

ENTRYPOINT /bin/bash -c "dotnet Orders.Application.dll"