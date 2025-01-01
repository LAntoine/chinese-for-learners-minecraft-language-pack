module MinecraftDownloader

open System.IO
open System.Text.Json
// #r "nuget: CmlLib.Core"
open CmlLib.Core
open MinecraftTranslator.Domain

let getVersions () = 
    MinecraftLauncher().GetAllVersionsAsync().AsTask()
    |> Async.AwaitTask
    |> Async.RunSynchronously
    |> Seq.filter (fun version -> version.Type = "release")

let getVersionsNames () = 
    getVersions()
    |> Seq.sortBy (fun version -> version.ReleaseTime)  
    |> Seq.map (fun version -> version.Name)  

let getLatestVersion () =
    getVersions()
    |> Seq.maxBy (fun version -> version.ReleaseTime)
    |> (fun version -> version.Name)

/// Download Minecraft and return the path where it has been saved.
let download () =
    System.Net.ServicePointManager.DefaultConnectionLimit <- 256
    let path = MinecraftPath(Path.Combine(Directory.GetCurrentDirectory(), "Minecraft"))
    printfn $"MinecraftPath: {path}"
    let launcher = MinecraftLauncher(path)
    launcher.ByteProgressChanged.Add(fun args -> printfn "%d bytes / %d bytes" args.ProgressedBytes args.TotalBytes)
    launcher.InstallAsync(getLatestVersion()).AsTask()
        |> Async.AwaitTask 
        |> Async.RunSynchronously
    path

type MinecraftObject = { Hash: string; Size: int }

type Index = { Objects: Map<string,MinecraftObject> }



let getLanguageFile fileName =
    let path = download()
    let files = Directory.GetFiles(Path.Combine(path.Assets, "indexes"))
    let filePath = Seq.maxBy (fun file -> file) files
    let json = File.ReadAllText filePath
    let options = JsonSerializerOptions(PropertyNameCaseInsensitive=true)
    let index = JsonSerializer.Deserialize<Index>(json, options)
    let fileHash = index.Objects[fileName].Hash.ToString()
    let json = File.ReadAllText (Path.Combine(path.Assets, "objects", fileHash.Substring(0, 2), fileHash))
    let languageFile = JsonSerializer.Deserialize<LanguageFile> json
    languageFile
    