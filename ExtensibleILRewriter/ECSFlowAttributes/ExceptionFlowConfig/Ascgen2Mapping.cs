using ECSFlowAttributes;
using ECSFlowAttributes.MethodCodeInjectingProcessor;

[assembly: ExceptionRaiseSite("rSiteConvertImage", "FormBatchConversion.Convert")]
[assembly: ExceptionChannel("channelConvertImage", "System.IO.FileNotFoundException", "rSiteConvertImage")]
[assembly: ExceptionChannel("channelConvertImage", "System.OutOfMemoryException", "rSiteConvertImage")]
[assembly: ExceptionHandler("channelConvertImage", "FormConvertImage.LoadImage", "System.IO.FileNotFoundException", nameof(MethodInjectionCodeProvider.FileNotFoundException))]
[assembly: ExceptionHandler("channelConvertImage", "FormConvertImage.LoadImage", "System.OutOfMemoryException", nameof(MethodInjectionCodeProvider.OutOfMemoryException))]

[assembly: ExceptionRaiseSite("rSitePrint", "FormConvertImage.DoPrint")]
[assembly: ExceptionChannel("channelPrint", "System.Exception", "rSitePrint")]
[assembly: ExceptionHandler("channelPrint", "FormConvertImage.DoPrint", "System.Exception", nameof(MethodInjectionCodeProvider.PrintErrorHandlerException))]

[assembly: ExceptionRaiseSite("rSitePrint2", "FormConvertImage.MenuFilePageSetup_Click")]
[assembly: ExceptionChannel("channelPrint2", "System.Exception", "rSitePrint2")]
[assembly: ExceptionHandler("channelPrin2t", "FormConvertImage.DoPrint", "System.Exception", nameof(MethodInjectionCodeProvider.PrintErrorHandlerException))]

[assembly: ExceptionRaiseSite("rSiteFont", "FormConvertImage.ShowFontDialog")]
[assembly: ExceptionChannel("channelFont", "System.ArgumentException", "rSiteFont")]
[assembly: ExceptionHandler("channelFont", "FormConvertImage.ShowFontDialog", "System.ArgumentException", nameof(MethodInjectionCodeProvider.ArgumentException))]

[assembly: ExceptionRaiseSite("rSiteLevels", "LevelsSettingsConverter.ConvertFrom")]
[assembly: ExceptionChannel("channelLevels", "System.Object", "rSiteLevels")]
[assembly: ExceptionHandler("channelLevels", "FormConvertImage.ShowFontDialog", "System.ArgumentException", nameof(MethodInjectionCodeProvider.LevelsSettingsHandler))]

// Nesta classe CharacterSizeDialog tem os metodos: TbxHeight_TextChanged e TbxWidth_TextChanged
// Nesta classe FormConvertImage tem os metodos: TbxHeight_TextChanged e TbxWidth_TextChanged
// Onde tratar erros de eventos de tela?
[assembly: ExceptionRaiseSite("rSiteCharacterSize", "CharacterSizeDialog, FormConvertImage")]
[assembly: ExceptionChannel("channelCharacterSize", "System.FormatException", "rSiteCharacterSize")]
[assembly: ExceptionHandler("channelCharacterSize", "Main", "System.FormatException", nameof(MethodInjectionCodeProvider.FormatExceptionHandler))]

// Resources
[assembly: ExceptionRaiseSite("rSiteResource", "CharacterSizeDialog")]
[assembly: ExceptionChannel("channelResource", "System.XmlException", "rSiteResource")]
[assembly: ExceptionChannel("channelResource", "System.IO.FileNotFoundException", "rSiteResource")]
[assembly: ExceptionHandler("channelResource", "Main", "System.XmlException", nameof(MethodInjectionCodeProvider.ResourceXmlHandler))]
[assembly: ExceptionHandler("channelResource", "Main", "System.IO.FileNotFoundException", nameof(MethodInjectionCodeProvider.TranslationFileHandler))]

[assembly: ExceptionRaiseSite("rSiteVariables", "Variables.Load, Variables.Save")]
[assembly: ExceptionChannel("channelVariables", "System.Exception", "rSiteVariables")]
[assembly: ExceptionHandler("channelVariables", "Main", "System.Exception", nameof(MethodInjectionCodeProvider.VariablesExceptionHandler))]