echo off

rem fallback if not passed
set nuget_version=2.2.0

call :read_params %*

nuget restore NLog.Web.sln -verbosity quiet
msbuild NLog.Web.sln /verbosity:minimal /t:rebuild /p:configuration=release
nuget pack NLog.Web\NLog.Web.csproj -properties Configuration=Release;Platform=AnyCPU -version %nuget_version%
nuget pack NLog.Web\NLog.Web.csproj -properties Configuration=Release;Platform=AnyCPU -version %nuget_version% -symbols

rem read pass parameters by name
:read_params
if not %1/==/ (
    if not "%__var%"=="" (
        if not "%__var:~0,1%"=="-" (
            endlocal
            goto read_params
        )
        endlocal & set %__var:~1%=%~1
    ) else (
        setlocal & set __var=%~1
    )
    shift
    goto read_params
)
exit /B