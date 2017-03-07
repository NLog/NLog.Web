echo off


msbuild NLog.Web.AspNetCore /t:restore /t:build /t:pack /verbosity:minimal /p:configuration=release
