dotnet publish src/ProofOfTestApi/ProofOfTestApi.csproj -o publish -p:PublishProfile=FolderProfileProofOfTestApi.csproj
dotnet publish src/HolderAppApi/HolderAppApi.csproj -o publish/HolderAppApi -p:PublishProfile=FolderProfileHolderAppApi.csproj
dotnet publish src/VerifierAppApi/VerifierAppApi.csproj -o publish/VerifierAppApi -p:PublishProfile=FolderProfileVerifierAppApi.csproj
dotnet publish src/CmsSigner/CmsSigner.csproj -o publish/Tools/CmsSigner -p:PublishProfile=Publish-Linux-x64