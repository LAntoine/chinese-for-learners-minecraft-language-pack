namespace MinecraftTranslator.Infrastructure

// #r "nuget: Azure.AI.Translation.Text"


module AzureTransliterator = 
    open System
    open Azure
    open Azure.Core
    open Azure.AI.Translation.Text
    open MinecraftTranslator.Domain

    type TargetLanguage = string

    let key = Environment.GetEnvironmentVariable("AZURE_TRANSLATOR_KEY")
    let credentials = AzureKeyCredential(key)
    let clientOptions = TextTranslationClientOptions()
    clientOptions.Retry.Delay <- TimeSpan.FromSeconds(5)
    clientOptions.Retry.Mode <- RetryMode.Exponential
    clientOptions.Retry.MaxRetries <- 10
    let client = TextTranslationClient(credentials,"global" , clientOptions)
    
    let mapToOutputLanguage originalText transliteratedText outputLanguage =
        match outputLanguage with
            | Language.zh_ln -> $"{originalText} ({transliteratedText})"
            | Language.zh_py -> transliteratedText

    let transliterateLanguageFile (file: LanguageFile) (outputLanguage: Language)  =
        let language = "zh-Hans"
        let fromScript = "Hans"
        let toScript = "Latn"
        let englishText = Map.keys file
        let originalText =  Map.values file
        let transliterations = Seq.chunkBySize 10 originalText 
                                |> Seq.map (fun textChunk -> TextTranslationTransliterateOptions(language, fromScript, toScript, textChunk))
                                |> Seq.map (fun textChunk -> client.Transliterate textChunk) 
                                |> Seq.collect (fun (response) -> response.Value) 
        let resultLanguageFile = Seq.zip3 englishText originalText transliterations
                                |> Seq.map (fun (eng,orig,trans) -> (eng,mapToOutputLanguage orig trans.Text outputLanguage))
                                |> Map.ofSeq
        resultLanguageFile

    