module MinecraftDownloader

open System.IO
open System.Text.Json
// #r "nuget: CmlLib.Core"
open CmlLib.Core
open MinecraftTranslator.Domain

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
    let path = MinecraftPath(Path.Combine(Directory.GetCurrentDirectory(), "Minecraft"))
    printfn $"MinecraftPath: {path}"
    let launcher = MinecraftLauncher(path)
    launcher.ByteProgressChanged.Add(fun args -> printfn "%d bytes / %d bytes" args.ProgressedBytes args.TotalBytes)
    launcher.InstallAsync(latestVersion).AsTask()
        |> Async.AwaitTask 
        |> Async.RunSynchronously
    path

type MinecraftObject = { Hash: string; Size: int }

type Index = { Objects: Map<string,MinecraftObject> }



let getTranslation fileName =
    let path = download
    let files = Directory.GetFiles(Path.Combine(path.Assets, "indexes"))
    let filePath = Seq.maxBy (fun file -> file) files
    let json = File.ReadAllText filePath
    let options = JsonSerializerOptions(PropertyNameCaseInsensitive=true)
    let index = JsonSerializer.Deserialize<Index>(json, options)
    let fileHash = index.Objects[fileName].Hash.ToString()
    let json = File.ReadAllText (Path.Combine(path.Assets, "objects", fileHash.Substring(0, 2), fileHash))
    let translations = JsonSerializer.Deserialize<LanguageFile> json
    translations
    