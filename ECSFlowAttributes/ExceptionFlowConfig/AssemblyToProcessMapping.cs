using ECSFlowAttributes;
using System.Windows.Forms;

[assembly: ExceptionRaiseSite("rSite1", "Program.D")]
[assembly: ExceptionChannel("EEC1", "System.OutOfMemoryException", "rSite1")]
[assembly: ExceptionHandler("EEC1", "Program.Main", "System.OutOfMemoryException", nameof(AssemblyToProcessMapping.OutOfMemoryExceptionHandler))]


public struct AssemblyToProcessMapping
{
    public static void OutOfMemoryExceptionHandler(System.OutOfMemoryException e)
    {
        MessageBox.Show("OutOfMemoryException caught");
        MessageBox.Show(e.Message);
    }
}