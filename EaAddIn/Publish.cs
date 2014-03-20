using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EaAddIn
{
    public partial class Publish : Form
    {
        public const string REFRESH_ALL = "Refresh all";

        public Publish()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        public void SetPublishUrls(List<string> publishedTo)
        {
            if(publishedTo.Count > 0)
                PublishUrls.Items.Add(REFRESH_ALL);

            foreach (var url in publishedTo)
            {
                PublishUrls.Items.Add(url);
            }
        }

        public string SelectedUrl
        {
            get
            {
                return PublishUrls.Text;
            }
        }
    }
}
