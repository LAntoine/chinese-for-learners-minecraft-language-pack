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
        let transliterations = Seq.chunkBySize 10 translatedText 
                                |> Seq.map (fun textChunk -> TextTranslationTransliterateOptions(language, fromScript, toScript, textChunk))
                                |> Seq.map (fun textChunk -> client.Transliterate textChunk) 
                                |> Seq.collect (fun (response) -> response.Value) 
        let resultLanguageFile = Seq.zip englishText transliterations
                                |> Seq.map (fun (eng,trans) -> (eng,trans.Text))
                                |> Map.ofSeq
        resultLanguageFile