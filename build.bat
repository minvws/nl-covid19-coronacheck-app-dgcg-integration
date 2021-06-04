dotnet build src/CmsSigner/CmsSigner.csproj
IF %ERRORLEVEL% NEQ 0 EXIT /B %ERRORLEVEL%

dotnet build src/DigitalGreenGatewayTool/DigitalGreenGatewayTool.csproj
IF %ERRORLEVEL% NEQ 0 EXIT /B %ERRORLEVEL%
