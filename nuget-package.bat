call "D:\Program Files\Microsoft Visual Studio 12.0\Common7\Tools\VsDevCmd.bat"
msbuild /p:Configuration=Release
del *.nupkg
..\NuGet.exe pack -Prop Configuration=Release
pause



FOR %%F IN (*.nupkg) DO (
 set filename=%%F
 goto tests
)
:tests
..\NuGet.exe Push "%filename%"


