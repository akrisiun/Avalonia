cd bin/merge/
mkdir src1

copy ..\Splat.dll src1/Splat.dll /Y
copy ..\System.Reactive.Windows.Threading.dll src1/System.Reactive.Windows.Threading.dll /Y
copy ..\Sprache.dll src1/Sprache.dll /Y
copy ..\System.Reactive.PlatformServices.dll src1/System.Reactive.PlatformServices.dll /Y
copy ..\System.Reactive.Linq.dll src1/System.Reactive.Linq.dll /Y
copy ..\SharpDX.DXGI.dll src1/SharpDX.DXGI.dll /Y
copy ..\SharpDX.Direct2D1.dll src1/SharpDX.Direct2D1.dll /Y
copy ..\SharpDX.dll src1/SharpDX.dll /Y
copy ..\System.Reactive.Interfaces.dll src1/System.Reactive.Interfaces.dll /Y
copy ..\System.Reactive.Core.dll src1/System.Reactive.Core.dll /Y

ILMErge /wildcards /log:ILMerge.log /allowdup  /targetplatform:"v4,C:/Windows/Microsoft.NET/Framework/v4.0.30319" src1/Splat.dll src1/System.Reactive.Windows.Threading.dll src1/Sprache.dll src1/System.Reactive.PlatformServices.dll src1/System.Reactive.Linq.dll src1/SharpDX.DXGI.dll src1/SharpDX.Direct2D1.dll src1/SharpDX.dll src1/System.Reactive.Interfaces.dll src1/System.Reactive.Core.dll  /out:"Sharp.Reactive.dll"
             
copy Sharp.Reactive.* ..\*.* /Y
cd ..\..\