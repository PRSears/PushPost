using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PushPost.ClientSide.HtmlGenerators.PostTypes;

namespace PushPost
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //ClientSide.Database.DatabaseTestHarness.TestWrites();
            //System.Threading.Thread.Sleep(2000);

            //ClientSide.Database.DatabaseTestHarness.TestRead();

            this.TestHarnessBox.Text = PushPost.ClientSide.HtmlGenerators.Breadcrumbs.TestHarness();
        }
    }
}
