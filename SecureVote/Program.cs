using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
namespace SecureVote
{
    class Program
    {
        private static void Main(string[] args)
        {
            Thread thread = new Thread(openForm);
            thread.Start();

            Console.WriteLine("[Secure Vote Client Program]");
        }

        private static void openForm()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new VoteForm());
        }
    }
}
