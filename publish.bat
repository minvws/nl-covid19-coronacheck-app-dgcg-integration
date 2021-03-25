dotnet publish src/ProofOfTestApi/ProofOfTestApi.csproj -o publish/ProofOfTestApi
dotnet publish src/IssuerApi/IssuerApi.csproj -o publish/IssuerApi
dotnet publish src/StaticProofApi/StaticProofApi.csproj -o publish/StaticProofApi
dotnet publish src/CmsSigner/CmsSigner.csproj -o publish/Tools/CmsSigner -p:PublishProfile=Publish-Win-x64

REM dotnet publish src/HolderAppApi/HolderAppApi.csproj -o publish/HolderAppApi
REM dotnet publish src/VerifierAppApi/VerifierAppApi.csproj -o publish/VerifierAppApi
