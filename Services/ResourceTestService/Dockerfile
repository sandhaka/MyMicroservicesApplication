FROM microsoft/aspnetcore:1.1 

# Copy application files
WORKDIR /app
COPY bin/Debug/netcoreapp1.1/publish /app

ENV ASPNETCORE_URLS http://*:5000
EXPOSE 5000

ENTRYPOINT /bin/bash -c "dotnet ResourceTestService.dll"