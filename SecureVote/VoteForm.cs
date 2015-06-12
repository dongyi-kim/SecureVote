using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Json;

namespace SecureVote
{
    public partial class VoteForm : Form
    {
        String user_id;
        String vote_id;

        public VoteForm()
        {
            InitializeComponent();
        }

        private void VoteForm_Load(object sender, EventArgs e)
        {
            bool logined = LoginForm.DoLogin(this, ref user_id, ref vote_id);
            if (logined)
            {
                try
                {
                    JsonObjectCollection col = SecureProtocol.Req_Info(user_id, vote_id);
                    richText.AppendText((String)col["vote_desc"].GetValue());
                    cmb_choice.Items.Clear();

                    JsonObjectCollection list = (JsonObjectCollection)col["vote_choie"].GetValue();
                    int i = 0;
                    while (list[i.ToString()] != null)
                    {
                        cmb_choice.Items.Add((String)list[i.ToString()].GetValue());
                        i++;
                    }
                    cmb_choice.SelectedIndex = 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[!] Failed to load vote information");
                }
                
            }
            else
            {
                Console.WriteLine("[!] Login Failed");
                this.Close();
            }
        }

        private void btnOkay_Click(object sender, EventArgs e)
        {
            String choice_idx = cmb_choice.SelectedIndex.ToString();
            bool result = SecureProtocol.Req_Choice(user_id, vote_id, choice_idx);
            if (result)
            {
                MessageBox.Show("투표가 반영되었습니다.");
            }
            else
            {

            }
            this.Close();
        }
    }
}
