@REM nuget restore
nuget update Perspex.sln 
@REM -MSBuildVersion 14

@set msbuild="%ProgramFiles%\msbuild\14.0\Bin\MSBuild.exe"
@if not exist %msbuild% @set msbuild="%ProgramFiles%\MSBuild\12.0\Bin\MSBuild.exe"
@if not exist %msbuild% @set msbuild="%ProgramFiles(x86)%\MSBuild\12.0\Bin\MSBuild.exe"
@if not exist %msbuild% @set msbuild="%ProgramFiles%\MSBuild\12.0\Bin\MSBuild.exe"

%msbuild% Perspex.sln /v:m
@REM %msbuild% Perspex-Gtk.sln /v:m
@REM %msbuild%  Perspex-Net45.sln

@PAUSE