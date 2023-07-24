FROM mcr.microsoft.com/dotnet/aspnet:8.0-preview as runtime
WORKDIR /app
COPY bin .
ADD settings .
EXPOSE 7188
ENTRYPOINT ["dotnet", "Phys.Lib.Api.Admin.dll"]