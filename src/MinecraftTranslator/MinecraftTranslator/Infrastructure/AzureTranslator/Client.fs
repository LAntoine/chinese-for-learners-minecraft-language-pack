namespace MinecraftTranslator.Infrastructure

// #r "nuget: Azure.AI.Translation.Text"


module AzureTransliterator = 
    open System
    open Azure
    open Azure.AI.Translation.Text
    open MinecraftTranslator.Domain

    type TargetLanguage = string

    let key = Environment.GetEnvironmentVariable("AZURE_TRANSLATOR_KEY")
    let credentials = AzureKeyCredential(key)
    let client = TextTranslationClient(credentials)
    

    let transliterateLanguageFile file: LanguageFile =
        let language = "zh-Hans"
        let fromScript = "Hans"
        let toScript = "Latn"
        let englishText = Map.keys file
        let translatedText =  Map.values file
        let toTransliterate = TextTranslationTransliterateOptions(language, fromScript, toScript, translatedText)
        let transliteration = client.Transliterate toTransliterate
        let resultLanguageFile = Seq.zip englishText transliteration.Value
                                |> Seq.map (fun (eng,trans) -> (eng,trans.Text)) 
                                |> Map.ofSeq
        resultLanguageFile