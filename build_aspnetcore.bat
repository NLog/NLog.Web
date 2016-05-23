echo off

rem update project.json for version number
call dotnet pack NLog.Web.AspNetCore --configuration release 