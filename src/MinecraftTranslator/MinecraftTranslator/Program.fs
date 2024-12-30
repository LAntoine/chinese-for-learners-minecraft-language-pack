open CommandLine
open MinecraftTranslator.Domain
open MinecraftTranslator.Infrastructure
open System.Text.Json
open System.IO
open System.Text

let parseLanguage (language: string) =
  match language.ToLower() with
    | "zh_ln" -> Language.zh_ln
    | "zh_py" -> Language.zh_py
    | _ -> failwith "invalid language"

let parseVersion (version: string) =
  match version.ToLower() with
    | "latest" -> MinecraftDownloader.getLatestVersion()
    | _ -> version

[<Verb("translate", HelpText = "Translate Minecraft to a given language.")>]
type TranslateOptions = {
  [<Option(Default = "zh_ln", HelpText = "Target language.")>] language : string;
  [<Option(Default = "latest", HelpText = "Minecraft version.")>] version : string;
}

[<Verb("list-versions", HelpText = "List Minecraft versions.")>]
type ListVersionsOptions = {
  [<Option(longName = "latest-only", HelpText = "Show only the latest version.")>] latestOnly : bool;
}

let runTranslateAndReturnExitCode (opts: TranslateOptions) =
    let language = parseLanguage opts.language
    let version = parseVersion opts.version
    printfn $"Translating Minecraft version {version} to language {(language.ToString())}"
    let languageFile = MinecraftDownloader.getLanguageFile "minecraft/lang/zh_cn.json"
    //let transliteration = AzureTransliterator.transliterateLanguageFile languageFile language
    let jsonOptions = JsonSerializerOptions()
    jsonOptions.WriteIndented <- true
    jsonOptions.Encoder <- Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping 
    let json = JsonSerializer.Serialize(languageFile, jsonOptions )
    File.WriteAllText($"../../../Chinese for learners language pack/assets/minecraft/lang/{language}.json", json)
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
    | _ -> failwith "invalid input"
  | :? CommandLine.NotParsed<obj> -> 1
  | _ -> failwith "invalid input"
