### RUNTIME CONTAINER
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY ../.publish/ ./

EXPOSE 80
# For added security, opt out of the diagnostic pipeline. This allows the container to run as read-only.
ENV DOTNET_EnableDiagnostics=0
ENTRYPOINT ["dotnet", "Api.dll"]


