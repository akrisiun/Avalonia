@REM nuget restore

"c:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe" Perspex.sln /v:m
"c:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe" Perspex-Gtk.sln /v:m
@REM call msbuild Perspex-Net45.sln

@PAUSE