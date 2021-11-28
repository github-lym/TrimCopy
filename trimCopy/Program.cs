using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;

namespace trimCopy
{
    static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        /*[STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }*/
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Boolean bCreatedNew;
	        //Create a new mutex using specific mutex name
	        Mutex m = new Mutex(false, "myUniqueName", out bCreatedNew);

	        if (bCreatedNew)
	            Application.Run(new Form1());
        }
    }
}
