FROM mcr.microsoft.com/dotnet/aspnet:8.0-preview as runtime
WORKDIR /app
COPY bin .
ADD settings .
ENTRYPOINT ["dotnet", "Phys.Lib.App.dll"]