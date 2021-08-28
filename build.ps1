# restore and builds all projects as release.
# creates NuGet package at \artifacts
dotnet --version

$versionPrefix = "4.14.0" # Also update version for minor versions in appveyor.yml 
$versionSuffix = ""
$versionFile = $versionPrefix + "." + ${env:APPVEYOR_BUILD_NUMBER}
if ($env:APPVEYOR_PULL_REQUEST_NUMBER) {
    $versionPrefix = $versionFile
    $versionSuffix = "PR" + $env:APPVEYOR_PULL_REQUEST_NUMBER
}

msbuild NLog.Web.sln /t:restore,rebuild /p:configuration=release /p:ContinuousIntegrationBuild=true /verbosity:minimal
if (-Not $LastExitCode -eq 0) {
    exit $LastExitCode 
}

msbuild src\NLog.Web /t:rebuild,pack /p:configuration=release /verbosity:minimal /p:IncludeSymbols=true /p:SymbolPackageFormat=snupkg /p:ContinuousIntegrationBuild=true /p:VersionPrefix=$versionPrefix /p:VersionSuffix=$versionSuffix /p:FileVersion=$versionFile
if (-Not $LastExitCode -eq 0) {
    exit $LastExitCode 
}

msbuild src\NLog.Web.AspNetCore /t:rebuild,pack /p:configuration=release /verbosity:minimal /p:IncludeSymbols=true /p:SymbolPackageFormat=snupkg /p:ContinuousIntegrationBuild=true /p:VersionPrefix=$versionPrefix /p:VersionSuffix=$versionSuffix /p:FileVersion=$versionFile
if (-Not $LastExitCode -eq 0) {
    exit $LastExitCode 
}

exit $LastExitCode
