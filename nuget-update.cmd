
@REM git rm src/Perspex.ReactiveUI/src
@REM git submodule add -f https://github.com/akrisiun/ReactiveUI.git src/Avalonia.ReactiveUI/src

copy lib\SharpDx.Reactive.dll bin\*.* /Y
copy lib\SharpDx.Reactive.pdb bin\*.* /Y

git submodule sync
git submodule update --init

nuget install -outputDirectory Packages Splat -version 1.6.2
nuget install -outputDirectory Packages SharpDX -version 3.0.2
nuget install -outputDirectory Packages Sprache -version 2.0.0.52

nuget install -outputDirectory Packages System.Reactive.Core -version 3.0.0
nuget install -outputDirectory Packages System.Reactive.Linq -version 3.0.0
nuget install -outputDirectory Packages System.Reactive.PlatformServices -version 3.0.0 
nuget install -outputDirectory Packages System.Reactive.Interfaces -version 3.0.0 
nuget install -outputDirectory Packages System.Reactive -version 3.0.0
nuget install -outputDirectory Packages JetBrains.Annotations -version 10.0.0
nuget install -outputDirectory Packages SharpDX.Direct2D1 -version 3.0.2
nuget install -outputDirectory Packages SharpDX.DXGI -version 3.0.2

cd src\Avalonia.ReactiveUI\src\src\
call build.cmd

cd ..\..\..\..\

nuget update Avalonia.VS2013.sln

@PAUSE