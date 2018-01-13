using System;
using System.Diagnostics;

namespace Ecalia
{

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
		[STAThread]
        static void Main(string[] args)
        {
            using (AppDelegate app = new AppDelegate())
            {
                app.Init();
            }
        }
    }


}

