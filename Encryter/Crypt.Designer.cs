namespace Encryter
{
    partial class Crypt
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            progressBar = new ProgressBar();
            txtFilePath = new TextBox();
            txtKey = new TextBox();
            btnChooseFile = new Button();
            btnEncrypt = new Button();
            btnDecrypt = new Button();
            label1 = new Label();
            label2 = new Label();
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // progressBar
            // 
            progressBar.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel1.SetColumnSpan(progressBar, 3);
            progressBar.Location = new Point(33, 211);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(360, 29);
            progressBar.TabIndex = 0;
            // 
            // txtFilePath
            // 
            txtFilePath.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel1.SetColumnSpan(txtFilePath, 2);
            txtFilePath.Location = new Point(155, 18);
            txtFilePath.Name = "txtFilePath";
            txtFilePath.Size = new Size(238, 27);
            txtFilePath.TabIndex = 1;
            // 
            // txtKey
            // 
            txtKey.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel1.SetColumnSpan(txtKey, 2);
            txtKey.Location = new Point(155, 82);
            txtKey.Name = "txtKey";
            txtKey.Size = new Size(238, 27);
            txtKey.TabIndex = 2;
            // 
            // btnChooseFile
            // 
            btnChooseFile.Anchor = AnchorStyles.Right;
            btnChooseFile.Location = new Point(33, 140);
            btnChooseFile.Name = "btnChooseFile";
            btnChooseFile.Size = new Size(116, 39);
            btnChooseFile.TabIndex = 3;
            btnChooseFile.Text = "Обрати файл";
            btnChooseFile.UseVisualStyleBackColor = true;
            btnChooseFile.Click += btnChooseFile_Click;
            // 
            // btnEncrypt
            // 
            btnEncrypt.Anchor = AnchorStyles.None;
            btnEncrypt.Location = new Point(155, 140);
            btnEncrypt.Name = "btnEncrypt";
            btnEncrypt.Size = new Size(116, 39);
            btnEncrypt.TabIndex = 4;
            btnEncrypt.Text = "Зашифрувати";
            btnEncrypt.UseVisualStyleBackColor = true;
            btnEncrypt.Click += btnEncrypt_Click;
            // 
            // btnDecrypt
            // 
            btnDecrypt.Anchor = AnchorStyles.Left;
            btnDecrypt.Location = new Point(277, 140);
            btnDecrypt.Name = "btnDecrypt";
            btnDecrypt.Size = new Size(116, 39);
            btnDecrypt.TabIndex = 5;
            btnDecrypt.Text = "Дешифрувати";
            btnDecrypt.UseVisualStyleBackColor = true;
            btnDecrypt.Click += btnDecrypt_Click;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Location = new Point(103, 22);
            label1.Name = "label1";
            label1.Size = new Size(46, 20);
            label1.TabIndex = 6;
            label1.Text = "Шлях";
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Location = new Point(103, 86);
            label2.Name = "label2";
            label2.Size = new Size(46, 20);
            label2.TabIndex = 7;
            label2.Text = "Ключ";
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 5;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 7.142857F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28.5714283F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28.5714283F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28.5714283F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 7.142857F));
            tableLayoutPanel1.Controls.Add(txtKey, 2, 1);
            tableLayoutPanel1.Controls.Add(txtFilePath, 2, 0);
            tableLayoutPanel1.Controls.Add(label1, 1, 0);
            tableLayoutPanel1.Controls.Add(label2, 1, 1);
            tableLayoutPanel1.Controls.Add(btnDecrypt, 3, 2);
            tableLayoutPanel1.Controls.Add(btnEncrypt, 2, 2);
            tableLayoutPanel1.Controls.Add(btnChooseFile, 1, 2);
            tableLayoutPanel1.Controls.Add(progressBar, 1, 3);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.Size = new Size(429, 259);
            tableLayoutPanel1.TabIndex = 9;
            // 
            // Crypt
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(429, 259);
            Controls.Add(tableLayoutPanel1);
            Name = "Crypt";
            Text = "Crypter";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ProgressBar progressBar;
        private TextBox txtFilePath;
        private TextBox txtKey;
        private Button btnChooseFile;
        private Button btnEncrypt;
        private Button btnDecrypt;
        private Label label1;
        private Label label2;
        private TableLayoutPanel tableLayoutPanel1;
    }
}
