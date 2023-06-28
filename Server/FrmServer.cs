using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    public partial class FrmServer : Form
    {
        Server server = new Server();
        public FrmServer()
        {
            InitializeComponent();
        }

        private void btnUnesi_Click(object sender, EventArgs e)
        {
            int number;
            if(int.TryParse(txtBroj.Text,out number))
            {
                if(server.Brojevi.Where(x => x == number).Count() < 2)
                {
                    server.Brojevi.Insert(0, number);
                }
                else
                {
                    MessageBox.Show("Vec postoje dva broja " + number.ToString());
                }
            }
            txtBrojevi.Text = "";
            foreach (int i in server.Brojevi)
            {
                txtBrojevi.Text += i.ToString() + " ";
            }
            txtBroj.Text = "";
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                if(server.Brojevi.Count >= 4)
                {
                    server.Start();
                    Thread serverskaNit = new Thread(server.HandleClients);
                    serverskaNit.IsBackground = true;
                    serverskaNit.Start();
                    lblStatus.Text = "Server je pokrenut";
                    btnStart.Visible = false;
                    btnStop.Visible = true;
                    btnUnesi.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            server.Stop();
            btnStart.Visible = true;
            btnStop.Visible = false;
            btnUnesi.Visible = true;
        }
    }
}
