echo off

rem update NLog.Web.AspNetCore.csproj for version number

cd NLog.Web.AspNetCore
msbuild    /t:restore /t:build /p:configuration=release /verbosity:minimal
cd ..
