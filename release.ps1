dotnet publish Flow.Launcher.Plugin.FlowTamer -c Release -r win-x64 --no-self-contained
Compress-Archive -LiteralPath Flow.Launcher.Plugin.FlowTamer/bin/Release/win-x64/publish -DestinationPath Flow.Launcher.Plugin.FlowTamer/bin/FlowTamer.zip -Force