using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ECSFlowAttributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class MethodLogging : Attribute
    {
        private Stopwatch stopWatch;

        public Stopwatch StopWatch
        {
            get
            {
                return stopWatch;
            }

            set
            {
                stopWatch = value;
            }
        }

        public void PreMethod(string name, params object[] arguments)
        {
            MessageBox.Show(string.Format("{0} Enter method: '{1}' Parameter: '{2}'", DateTime.Now, name, string.Join(", ", arguments)));
            StopWatch = new Stopwatch();
            StopWatch.Start();
        }

        public void PostMethod(string name, params object[] arguments)
        {
            StopWatch.Stop();
            MessageBox.Show(string.Format("{0} Leaving method: '{1}' Parameter: '{2}' Duration: '{3} ms'", DateTime.Now, name, string.Join(", ", arguments), StopWatch.ElapsedMilliseconds));
        }
    }
}
