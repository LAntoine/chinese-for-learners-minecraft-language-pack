using CmlLib.Core;

System.Net.ServicePointManager.DefaultConnectionLimit = 256;

// initialize the launcher
var path = new MinecraftPath();
Console.WriteLine(path);
var launcher = new MinecraftLauncher(path);

launcher.ByteProgressChanged += (sender, args) =>
{
    Console.WriteLine($"{args.ProgressedBytes} bytes / {args.TotalBytes} bytes");
};

// get all versions
// var versions = await launcher.GetAllVersionsAsync();
// foreach (var v in versions)
// {
//     Console.WriteLine(v.Name);
// }

// install the game
await launcher.InstallAsync("1.20.6");

Console.WriteLine("Done.");