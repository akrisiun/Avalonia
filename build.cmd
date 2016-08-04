@REM 
git submodule update --init

nuget restore Avalonia.VS2013.sln

@set msbuild="%ProgramFiles%\msbuild\14.0\Bin\MSBuild.exe"
@if not exist %msbuild% @set msbuild="%ProgramFiles%\MSBuild\14.0\Bin\MSBuild.exe"
@if not exist %msbuild% @set msbuild="%ProgramFiles(x86)%\MSBuild\12.0\Bin\MSBuild.exe"
@if not exist %msbuild% @set msbuild="%ProgramFiles%\MSBuild\12.0\Bin\MSBuild.exe"

%msbuild% /v:m /m /p:nowarn=1591 "/p:Configuration=Debug" /p:Platform="Any CPU" Avalonia.VS2013.sln
@REM %msbuild% Perspex-Gtk.sln /v:m
@REM %msbuild% Perspex-Net45.sln

@PAUSE