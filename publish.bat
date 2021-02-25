dotnet publish src/ProofOfTestApi/ProofOfTestApi.csproj -o publish/ProofOfTestApi
REM dotnet publish src/HolderAppApi/HolderAppApi.csproj -o publish/HolderAppApi
REM dotnet publish src/VerifierAppApi/VerifierAppApi.csproj -o publish/VerifierAppApi
dotnet publish src/CmsSigner/CmsSigner.csproj -o publish/Tools/CmsSigner -p:PublishProfile=Publish-Win-x64
