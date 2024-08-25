module MinecraftDownloader

open System.IO
open System.Text.Json
// #r "nuget: CmlLib.Core"
open CmlLib.Core

let latestVersion =
    MinecraftLauncher().GetAllVersionsAsync().AsTask()
    |> Async.AwaitTask
    |> Async.RunSynchronously
    |> Seq.filter (fun version -> version.Type = "release")
    |> Seq.maxBy (fun version -> version.ReleaseTime)
    |> (fun version -> version.Name)

/// Download Minecraft and return the path where it has been saved.
let download =
    System.Net.ServicePointManager.DefaultConnectionLimit <- 256
    let path = MinecraftPath()
    printfn $"MinecraftPath: {path}"
    let launcher = MinecraftLauncher(path)
    launcher.ByteProgressChanged.Add(fun args -> printfn "%d bytes / %d bytes" args.ProgressedBytes args.TotalBytes)
    launcher.InstallAsync(latestVersion).AsTask()
        |> Async.AwaitTask 
        |> Async.RunSynchronously
    path

type MinecraftObject = { Hash: string; Size: int }

type Index = { Objects: Map<string,MinecraftObject> }

type LangFile = Map<string,string>

let getResourceFile fileName =
    let path = download
    let files = Directory.GetFiles(Path.Combine(path.Assets, "indexes"))
    let filePath = Seq.maxBy (fun file -> file) files
    let json = File.ReadAllText filePath
    let index = JsonSerializer.Deserialize<Index> json
    let fileHash = index.Objects[fileName].ToString()
    let json = File.ReadAllText (Path.Combine(path.Assets, "objects", fileHash.Substring(0, 2)))
    let translations = JsonSerializer.Deserialize<LangFile> json
    translations


getResourceFile "minecraft/lang/zh_cn.json"
