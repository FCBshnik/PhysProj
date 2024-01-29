FROM mcr.microsoft.com/dotnet/aspnet:8.0 as runtime
WORKDIR /app
COPY bin .
ADD settings .
ENTRYPOINT ["dotnet", "Phys.Lib.Site.Api.dll"]