echo off

rem update project.json for version number

cd NLog.Web.AspNetCore
msbuild    /t:restore /t:pack /p:configuration=release /verbosity:minimal
cd ..
