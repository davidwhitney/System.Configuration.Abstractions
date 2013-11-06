call build.bat
if %errorlevel% neq 0 exit /b %errorlevel%

.nuget\nuget pack "System.Configuration.Abstractions\System.Configuration.Abstractions.csproj" -Properties Configuration=Release
if %errorlevel% neq 0 exit /b %errorlevel%