echo off

rem update project.json for version number

call dotnet restore NLog.Web.AspNetCore\NLog.Web.AspNetCore.csproj 
call dotnet pack NLog.Web.AspNetCore\NLog.Web.AspNetCore.csproj --configuration release 