using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;
using System.Windows.Forms;
using PushPost.ClientSide.HtmlGenerators.PostTypes;
using PushPost.ClientSide.HtmlGenerators.Embedded;
using TidyManaged;
using System.IO;

namespace PushPost
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        //
        // TODO !!
        //      Start hacking together the UI
        //
        //      Should take new posts, load matching categorier from the db, then combine into one list.
        //      Pass that list off to PageBuilder generate and save the pages.
        //      Commit new posts to the database.

        private void button1_Click(object sender, EventArgs e)
        {
            TestPageBuilder();
        }

        private void TestPageBuilder()
        {
            ClientSide.HtmlGenerators.PageBuilder.TestHarness();
        }

        private void TestDB()
        {
            ClientSide.Database.DatabaseTestHarness.TestWrites();
            System.Threading.Thread.Sleep(2000);

            ClientSide.Database.DatabaseTestHarness.TestRead();
        }

        private void TestArchive()
        {
            ClientSide.Database.Archive.TestHarness();
        }

        private void TestPage()
        {
            using(StreamWriter buffer = File.CreateText("generated_page.html"))
            {
                buffer.Write(ClientSide.HtmlGenerators.Page.TestHarness());
            }
        }
    }
}
