# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:8.0-preview as build
WORKDIR /src
COPY /src/PhysProj/ .
RUN dotnet restore "Phys.Lib.Api.Admin/Phys.Lib.Api.Admin.csproj"
RUN dotnet publish "Phys.Lib.Api.Admin/Phys.Lib.Api.Admin.csproj" -c Release -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0-preview as runtime
WORKDIR /app
COPY --from=build /publish .
EXPOSE 7188
ENTRYPOINT ["dotnet", "Phys.Lib.Api.Admin.dll"]