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
    public partial class LoginForm : Form
    {
        private bool isSuccess = false;
        public static bool DoLogin(Form form, ref String user_id, ref String vote_id)
        {
            LoginForm loginForm = new LoginForm();
            loginForm.ShowDialog(form);
            if (loginForm.isSuccess)
            {
                user_id =  loginForm.getUserID();
                vote_id = loginForm.getVoteID();
            }
            return loginForm.isSuccess;
        }

        public LoginForm()
        {
            InitializeComponent();
        }

        public String getUserID() { return txtID.Text; }
        public String getVoteID() { return txtVoteID.Text; }
       
        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtID.Text.Length < 3 || 10 < txtID.Text.Length)
                    throw new Exception("아이디는 3~10글자이어야 합니다.");
                if (txtPW.Text.Length < 5 || 20 < txtPW.Text.Length)
                    throw new Exception("비밀번호는 5~20글자이어야 합니다.");
                if (Convert.ToInt32(txtVoteID.Text) <= 0)
                    throw new Exception("투표 번호가 잘못되었습니다.");
                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            isSuccess = SecureProtocol.Req_PublicKey(txtID.Text)
                    && SecureProtocol.Req_Auth(txtID.Text, txtPW.Text);
            if (isSuccess)
            {
                this.Close();
            }
            else
            {
                MessageBox.Show("로그인에 실패하였습니다.");
            }
        }

        private void LoginForm_Shown(object sender, EventArgs e)
        {
            Console.WriteLine("[!] 로그인이 필요합니다.");
        }
    }
}
