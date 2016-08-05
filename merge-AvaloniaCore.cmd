
cd bin/merge/
mkdir src2

copy ..\Avalonia.Animation.dll src2/Avalonia.Animation.dll /Y
copy ..\Avalonia.Base.dll src2/Avalonia.Base.dll /Y
copy ..\Avalonia.SceneGraph.dll src2/Avalonia.SceneGraph.dll /Y
copy ..\Avalonia.Input.dll src2/Avalonia.Input.dll /Y
copy ..\Avalonia.Interactivity.dll src2/Avalonia.Interactivity.dll /Y

@REM /lib:src2 
ILMErge /wildcards /log:ILMerge.log /allowdup  /targetplatform:"v4,C:/Windows/Microsoft.NET/Framework/v4.0.30319" /lib:src3 src2/Avalonia.Animation.dll src2/Avalonia.Base.dll src2/Avalonia.SceneGraph.dll src2/Avalonia.Input.dll src2/Avalonia.Interactivity.dll /out:"Avalonia.Core.dll"

copy Avalonia.Core.* ..\*.* /Y
cd ..\..\