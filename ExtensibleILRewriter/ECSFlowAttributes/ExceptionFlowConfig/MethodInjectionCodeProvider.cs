using ExtensibleILRewriter.CodeInjection;
using ExtensibleILRewriter.Processors.Methods;
using Mono.Cecil;
using System;
using System.IO;
using System.Windows.Forms;

namespace ECSFlowAttributes.MethodCodeInjectingProcessor
{
    public partial class MethodInjectionCodeProvider : CodeProvider<MethodCodeInjectingCodeProviderArgument>
    {
        public static void TranslationFileHandler(State state)
        {
            // if (TranslationFile != "translation.xml")
            // {
                MessageBox.Show(
                            string.Format(
                                    "Could not load translation file '{0}'"),
                                    "Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);

            // }
        }

        public static void ResourceXmlHandler(State state)
        {
            // MessageBox.Show(ex.Message, string.Format(GetString("Error with settings file '{0}'"), TranslationFile), MessageBoxButtons.OK, MessageBoxIcon.Error);
            MessageBox.Show("ex.Message", "Error with settings file '{0}'", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void VariablesExceptionHandler(State state)
        {
        }

        public static void FormatExceptionHandler(State state)
        {
            // this.characterSize.Width = -1; || this.characterSize.Height = -1;
        }

        public static void LevelsSettingsHandler(State state)
        {
            // return base.ConvertFrom(context, culture, value);
        }

        public static void PrintErrorHandlerException(State state)
        {
            MessageBox.Show(
                "PrintError", // + ":" +ex.Message,
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        public static void ArgumentException(State state)
        {
            MessageBox.Show(
                            "Unable to select this font",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error,
                            MessageBoxDefaultButton.Button1,
                            MessageBoxOptions.ServiceNotification);
        }

        public static void OutOfMemoryException(State state)
        {
            state = new State();
            state.Items.Add(typeof(OutOfMemoryException));

            MessageBox.Show(
                "Unknown or Unsupported File",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        public static void FileNotFoundException(State state)
        {
            state = new State();
            state.Items.Add(typeof(FileNotFoundException));

            MessageBox.Show(
                "File Not Found",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
