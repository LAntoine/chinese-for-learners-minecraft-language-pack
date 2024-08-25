
[<EntryPoint>]
let main args =
    let minecraftPath = MinecraftDownloader.download
    printfn "Minecraft path: %s" (minecraftPath.ToString())
    0
