using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class InputDialog : Form
    {
        public string UserInput { get; private set; }

        public InputDialog(string prompt)
        {
            InitializeComponent();
            this.Text = prompt;
            this.CenterToParent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            UserInput = txtInput.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
