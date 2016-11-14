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

        public static void OutOfMemoryExceptionHandlerValue(State state, OutOfMemoryException value)
        {
            state.Items.Add(value);
            MessageBox.Show("OutOfMemoryException caught:" + value.ToString());
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

        public static void InjectedMethod_NoValue(State state)
        {
            MessageBox.Show("No Value");
        }

        public static void InjectedMethod_Value(State state, int value)
        {
            MessageBox.Show("Value:" + value);
        }
    }
}
