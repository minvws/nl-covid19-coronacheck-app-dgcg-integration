dotnet publish src/ProofOfTestApi/ProofOfTestApi.csproj -o publish/ProofOfTestApi
tar -czvf publish/ProofOfTestApi.tar.gz publish/Tools/CmsSigner
rm -r publish/ProofOfTestApi

#dotnet publish src/HolderAppApi/HolderAppApi.csproj -o publish/HolderAppApi
#dotnet publish src/VerifierAppApi/VerifierAppApi.csproj -o publish/VerifierAppApi
dotnet publish src/CmsSigner/CmsSigner.csproj -o publish/Tools/CmsSigner
tar -czvf publish/CmsSigner.tar.gz publish/Tools/CmsSigner
rm -r publish/Tools/CmsSigner
