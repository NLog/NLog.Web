echo off

rem update project.json for version number
cd NLog.Web.AspNetCore.Tests 
call dotnet restore 
call dotnet build  --configuration release 
call dotnet xunit -framework netcoreapp1.0
cd ..