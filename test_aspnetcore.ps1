set-location NLog.Web.AspNetCore.Tests 
dotnet restore 
if (-Not $LastExitCode -eq 0) { exit $LastExitCode }

dotnet build  --configuration release 
if (-Not $LastExitCode -eq 0) { exit $LastExitCode }

dotnet xunit
if (-Not $LastExitCode -eq 0) { exit $LastExitCode }

set-location ..