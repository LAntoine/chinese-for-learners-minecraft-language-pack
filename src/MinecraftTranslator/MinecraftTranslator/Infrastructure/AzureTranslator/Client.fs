namespace MinecraftTranslator.Infrastructure

// #r "nuget: Azure.AI.Translation.Text"


module AzureTranslator = 
    open System
    open Azure
    open Azure.AI.Translation.Text
    // input
    type RequestBodyElement = { Text : string }

    type TargetLanguage = string

    // output
    type TranslationResult = {
        Text: string
        To: string
        Script: string
    }

    type Response = {
        Translations: List<TranslationResult>
    }

    // client
    let key = Environment.GetEnvironmentVariable("AZURE_TRANSLATOR_KEY")
    let credentials = AzureKeyCredential(key)
    let endpoint = Uri("https://api.cognitive.microsofttranslator.com/")
    let client = TextTranslationClient(endpoint)
    let language = "zh-Hans"
    let fromScript = "Hans"
    let toScript = "Latn"

    let inputText = "这是个测试。"
    let toTransliterate = TextTranslationTransliterateOptions(language, fromScript, toScript, [inputText])
    let translation = client.Transliterate toTransliterate