dotnet publish src/ProofOfTestApi/ProofOfTestApi.csproj -o publish/ProofOfTestApi
tar -czvf publish/ProofOfTestApi.tar.gz publish/ProofOfTestApi
rm -r publish/ProofOfTestApi

dotnet publish src/IssuerApi/IssuerApi.csproj -o publish/IssuerApi
tar -czvf publish/IssuerApi.tar.gz publish/IssuerApi
rm -r publish/IssuerApi

dotnet publish src/CmsSigner/CmsSigner.csproj -o publish/Tools/CmsSigner
tar -czvf publish/CmsSigner.tar.gz publish/Tools/CmsSigner
rm -r publish/Tools/CmsSigner

#dotnet publish src/HolderAppApi/HolderAppApi.csproj -o publish/HolderAppApi
#dotnet publish src/VerifierAppApi/VerifierAppApi.csproj -o publish/VerifierAppApi
