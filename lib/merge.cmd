@REM ..\packages\Sprache.2.0.0.52\lib\portable-net4+netcore45+win8+wp8+sl5+MonoAndroid+Xamarin.iOS10+MonoTouch\Sprache.dll

ilmerge /wildcards /log:ILMerge.log /allowdup  /targetplatform:"v4,C:/Windows/Microsoft.NET/Framework/v4.0.30319" /out:Sharp.Reactive.dll src1/Splat.dll src1/System.Reactive.Windows.Threading.dll src1/Sprache.dll src1/System.Reactive.PlatformServices.dll src1/System.Reactive.Linq.dll src1/SharpDX.DXGI.dll src1/SharpDX.Direct2D1.dll src1/SharpDX.dll src1/System.Reactive.Interfaces.dll src1/System.Reactive.Core.dll

@PAUSE
@REM http://www.hurryupandwait.io/blog/what-you-should-know-about-running-ilmerge-on-net-4-5-assemblies-targeting-net-4-0