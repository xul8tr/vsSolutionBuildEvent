call "%~dp0_config"

REM # Configuration
set cfgname=CI_Release_net45

REM # Version of MSBuild tool
set _msbuild=14.0

call "%~dp0_build"