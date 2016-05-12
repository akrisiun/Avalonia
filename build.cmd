@REM git submodule update --init
@REM nuget restore

@set msbuild="%ProgramFiles%\msbuild\14.0\Bin\MSBuild.exe"
@if not exist %msbuild% @set msbuild="%ProgramFiles%\MSBuild\14.0\Bin\MSBuild.exe"
@if not exist %msbuild% @set msbuild="%ProgramFiles(x86)%\MSBuild\12.0\Bin\MSBuild.exe"
@if not exist %msbuild% @set msbuild="%ProgramFiles%\MSBuild\12.0\Bin\MSBuild.exe"

%msbuild% Perspex.sln /v:m /m /p:nowarn=1591 "/p:Configuration=Debug" /p:Platform="Any CPU"
@REM %msbuild% Perspex-Gtk.sln /v:m
@REM %msbuild% Perspex-Net45.sln

@PAUSE