using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L6Window
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var window = new SimpleOpenGLSample())
            {
                window.Width = 800;
                window.Height = 600;
                window.Run();
            }
        }
    }
}
