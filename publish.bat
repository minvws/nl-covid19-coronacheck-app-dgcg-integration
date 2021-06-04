dotnet publish src/CmsSigner/CmsSigner.csproj -o publish/Tools/CmsSigner -p:PublishProfile=Publish-Win-x64
IF %ERRORLEVEL% NEQ 0 EXIT /B %ERRORLEVEL%

dotnet publish src/DigitalGreenGatewayTool/DigitalGreenGatewayTool.csproj -o publish/Tools/DigitalGreenGatewayTool -p:PublishProfile=Publish-Win-x64
IF %ERRORLEVEL% NEQ 0 EXIT /B %ERRORLEVEL%
