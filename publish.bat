dotnet publish src/ProofOfTestApi/ProofOfTestApi.csproj -o publish/ProofOfTestApi
IF %ERRORLEVEL% NEQ 0 EXIT /B %ERRORLEVEL%

dotnet publish src/IssuerApi/IssuerApi.csproj -o publish/IssuerApi
IF %ERRORLEVEL% NEQ 0 EXIT /B %ERRORLEVEL%

dotnet publish src/StaticProofApi/StaticProofApi.csproj -o publish/StaticProofApi
IF %ERRORLEVEL% NEQ 0 EXIT /B %ERRORLEVEL%

dotnet publish src/CmsSigner/CmsSigner.csproj -o publish/Tools/CmsSigner -p:PublishProfile=Publish-Win-x64
IF %ERRORLEVEL% NEQ 0 EXIT /B %ERRORLEVEL%

