dotnet test --verbosity quiet
IF %ERRORLEVEL% NEQ 0 EXIT /B %ERRORLEVEL%

security-scan nl-covid19-coronacheck-app-backend.sln
IF %ERRORLEVEL% NEQ 0 EXIT /B %ERRORLEVEL%
