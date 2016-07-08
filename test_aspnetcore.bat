echo off

rem update project.json for version number

call dotnet restore NLog.Web.AspNetCore.Tests 
call dotnet test NLog.Web.AspNetCore.Tests --configuration release 