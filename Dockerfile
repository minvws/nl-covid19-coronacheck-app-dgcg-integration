# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source

# copy csproj and restore dependenies as distinct layer (to save time)
COPY . .
RUN dotnet restore src/nl-covid19-coronatester-app-backend.sln

# copy and build everything
COPY . .
WORKDIR /source
RUN dotnet build src/ProofOfTestApi/ProofOfTestApi.csproj
RUN dotnet build src/HolderAppApi/HolderAppApi.csproj
RUN dotnet build src/VerifierAppApi/VerifierAppApi.csproj
RUN dotnet build src/CmsSigner/CmsSigner.csproj

# publish everything
FROM build AS publish
RUN dotnet publish src/ProofOfTestApi/ProofOfTestApi.csproj --no-build -o publish/ProofOfTestApi
RUN dotnet publish src/HolderAppApi/HolderAppApi.csproj --no-build -o publish/HolderAppApi
RUN dotnet publish src/VerifierAppApi/VerifierAppApi.csproj --no-build -o publish/VerifierAppApi
RUN dotnet publish src/CmsSigner/CmsSigner.csproj --no-build -o publish/Tools/CmsSigner

# final stage/image
# FROM mcr.microsoft.com/dotnet/runtime:5.0
# WORKDIR /publish
# COPY --from=publish publish/ProofOfTestApi .
# ENTRYPOINT ["dotnet", "complexapp.dll"]