using System;
using System.Windows.Forms;

namespace ECSFlow
{
    class Program
    {
         static void Main(string[] args)
        {
            A();
        }

        public static void A()
        {
            MessageBox.Show("A Body");
            B();
        }

        public static void B()
        {
            MessageBox.Show("B Body");
            C();
        }

        public static void C()
        {
            MessageBox.Show("C Body");
            D();
        }

        public static void D()
        {
            MessageBox.Show("D Body");
            throw new OutOfMemoryException();
        }
    }
}
