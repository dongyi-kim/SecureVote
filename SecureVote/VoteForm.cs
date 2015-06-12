using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SecureVote
{
    public partial class VoteForm : Form
    {
        public VoteForm()
        {
            InitializeComponent();
        }

        private void VoteForm_Load(object sender, EventArgs e)
        {
            bool logined = LoginForm.DoLogin(this);
            if (logined)
            {

            }
            else
            {
                Console.WriteLine("[!] Login Failed");
            }
            if (!logined)
                this.Close();
        }
    }
}
