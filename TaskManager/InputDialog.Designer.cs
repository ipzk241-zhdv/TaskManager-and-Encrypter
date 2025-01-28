namespace WinFormsApp1
{
    partial class InputDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtInput = new TextBox();
            button1 = new Button();
            button2 = new Button();
            SuspendLayout();
            // 
            // txtInput
            // 
            txtInput.Location = new Point(62, 37);
            txtInput.Name = "txtInput";
            txtInput.Size = new Size(174, 27);
            txtInput.TabIndex = 0;
            // 
            // button1
            // 
            button1.Location = new Point(34, 98);
            button1.Name = "button1";
            button1.Size = new Size(94, 29);
            button1.TabIndex = 3;
            button1.Text = "Відміна";
            button1.UseVisualStyleBackColor = true;
            button1.Click += btnCancel_Click;
            // 
            // button2
            // 
            button2.Location = new Point(164, 98);
            button2.Name = "button2";
            button2.Size = new Size(94, 29);
            button2.TabIndex = 1;
            button2.Text = "ОК";
            button2.UseVisualStyleBackColor = true;
            button2.Click += btnOk_Click;
            // 
            // InputDialog
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(293, 151);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(txtInput);
            Name = "InputDialog";
            Text = "Form2";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtInput;
        private Button button1;
        private Button button2;
    }
}