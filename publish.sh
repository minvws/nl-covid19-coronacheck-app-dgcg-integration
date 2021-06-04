dotnet publish src/CmsSigner/CmsSigner.csproj -o publish/Tools/CmsSigner
tar -czvf publish/CmsSigner.tar.gz publish/Tools/CmsSigner
rm -r publish/Tools/CmsSigner

dotnet publish src/DigitalGreenGatewayTool/DigitalGreenGatewayTool.csproj -o publish/Tools/DigitalGreenGatewayTool
tar -czvf publish/DigitalGreenGatewayTool.tar.gz publish/Tools/DigitalGreenGatewayTool
rm -r publish/Tools/DigitalGreenGatewayTool
