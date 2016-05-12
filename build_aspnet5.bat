echo off
call dnvm install 1.0.0-rc1-update1

call dnu restore --quiet

rem update project.json for version number
call dnu pack NLog.Web.ASPNET5\project.json --configuration release --quiet