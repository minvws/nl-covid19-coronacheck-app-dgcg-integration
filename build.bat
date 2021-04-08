dotnet build src/ProofOfTestApi/ProofOfTestApi.csproj
IF %ERRORLEVEL% NEQ 0 EXIT /B %ERRORLEVEL%

dotnet build src/IssuerApi/IssuerApi.csproj
IF %ERRORLEVEL% NEQ 0 EXIT /B %ERRORLEVEL%

dotnet build src/StaticProofApi/StaticProofApi.csproj
IF %ERRORLEVEL% NEQ 0 EXIT /B %ERRORLEVEL%

dotnet build src/CmsSigner/CmsSigner.csproj
IF %ERRORLEVEL% NEQ 0 EXIT /B %ERRORLEVEL%

dotnet build src/IssuerInteropBenchmark/IssuerInteropBenchmark.csproj
IF %ERRORLEVEL% NEQ 0 EXIT /B %ERRORLEVEL%
