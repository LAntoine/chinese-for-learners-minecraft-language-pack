open CommandLine

[<Verb("translate", HelpText = "Translate Minecraft to a given language.")>]
type TranslateOptions = {
  [<Option(Default = "zh_ln", HelpText = "Target language (zh_ln or zh_py).")>] language : string;
  [<Option(Default = "latest", HelpText = "Minecraft version.")>] version : string;
}

[<Verb("list-versions", HelpText = "List Minecraft versions.")>]
type ListVersionsOptions = {
  [<Option(longName = "latest-only", HelpText = "Show only the latest version.")>] latestOnly : bool;
}

let runTranslateAndReturnExitCode opts =
    let translation = MinecraftDownloader.getTranslation "minecraft/lang/zh_cn.json"
    0

let runListVersionstAndReturnExitCode (opts: ListVersionsOptions) = 
    match opts.latestOnly with
        | true -> printfn "%s" (MinecraftDownloader.getLatestVersion())
        | false -> Seq.iter (printfn "%s") (MinecraftDownloader.getVersionsNames())
    0

[<EntryPoint>]
let main args =
  let result = Parser.Default.ParseArguments<TranslateOptions, ListVersionsOptions> args
  match result with
  | :? CommandLine.Parsed<obj> as command ->
    match command.Value with
    | :? TranslateOptions as opts -> runTranslateAndReturnExitCode opts
    | :? ListVersionsOptions as opts -> runListVersionstAndReturnExitCode opts
  | :? CommandLine.NotParsed<obj> -> 1
