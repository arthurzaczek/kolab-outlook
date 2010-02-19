namespace OutlookKolab.Kolab.Settings
{
    partial class DlgSettings
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSelectOutlookContactsFolder = new System.Windows.Forms.Button();
            this.txtContactsOutlookFolder = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSelectContactsFolder = new System.Windows.Forms.Button();
            this.txtContactsIMAPFolder = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnSelectOutlookCalendarFolder = new System.Windows.Forms.Button();
            this.btnSelectCalendarFolder = new System.Windows.Forms.Button();
            this.txtCalendarOutlookFolder = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCalendarIMAPFolder = new System.Windows.Forms.TextBox();
            this.btnClearContactsFolder = new System.Windows.Forms.Button();
            this.btnClearOutlookContactsFolder = new System.Windows.Forms.Button();
            this.btnClearCalendarFolder = new System.Windows.Forms.Button();
            this.btnClearOutlookCalendarFolder = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(487, 327);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(406, 327);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnClearOutlookContactsFolder);
            this.groupBox1.Controls.Add(this.btnClearContactsFolder);
            this.groupBox1.Controls.Add(this.btnSelectOutlookContactsFolder);
            this.groupBox1.Controls.Add(this.txtContactsOutlookFolder);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.btnSelectContactsFolder);
            this.groupBox1.Controls.Add(this.txtContactsIMAPFolder);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(550, 149);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Contacts";
            // 
            // btnSelectOutlookContactsFolder
            // 
            this.btnSelectOutlookContactsFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectOutlookContactsFolder.Location = new System.Drawing.Point(388, 52);
            this.btnSelectOutlookContactsFolder.Name = "btnSelectOutlookContactsFolder";
            this.btnSelectOutlookContactsFolder.Size = new System.Drawing.Size(75, 23);
            this.btnSelectOutlookContactsFolder.TabIndex = 5;
            this.btnSelectOutlookContactsFolder.Text = "Select";
            this.btnSelectOutlookContactsFolder.UseVisualStyleBackColor = true;
            this.btnSelectOutlookContactsFolder.Click += new System.EventHandler(this.btnSelectOutlookContactsFolder_Click);
            // 
            // txtContactsOutlookFolder
            // 
            this.txtContactsOutlookFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtContactsOutlookFolder.Location = new System.Drawing.Point(88, 54);
            this.txtContactsOutlookFolder.Name = "txtContactsOutlookFolder";
            this.txtContactsOutlookFolder.ReadOnly = true;
            this.txtContactsOutlookFolder.Size = new System.Drawing.Size(294, 20);
            this.txtContactsOutlookFolder.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Outlook Folder";
            // 
            // btnSelectContactsFolder
            // 
            this.btnSelectContactsFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectContactsFolder.Location = new System.Drawing.Point(388, 26);
            this.btnSelectContactsFolder.Name = "btnSelectContactsFolder";
            this.btnSelectContactsFolder.Size = new System.Drawing.Size(75, 23);
            this.btnSelectContactsFolder.TabIndex = 2;
            this.btnSelectContactsFolder.Text = "Select";
            this.btnSelectContactsFolder.UseVisualStyleBackColor = true;
            this.btnSelectContactsFolder.Click += new System.EventHandler(this.btnSelectContactsFolder_Click);
            // 
            // txtContactsIMAPFolder
            // 
            this.txtContactsIMAPFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtContactsIMAPFolder.Location = new System.Drawing.Point(88, 28);
            this.txtContactsIMAPFolder.Name = "txtContactsIMAPFolder";
            this.txtContactsIMAPFolder.ReadOnly = true;
            this.txtContactsIMAPFolder.Size = new System.Drawing.Size(294, 20);
            this.txtContactsIMAPFolder.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "IMAP Folder";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.btnClearOutlookCalendarFolder);
            this.groupBox2.Controls.Add(this.btnClearCalendarFolder);
            this.groupBox2.Controls.Add(this.btnSelectOutlookCalendarFolder);
            this.groupBox2.Controls.Add(this.btnSelectCalendarFolder);
            this.groupBox2.Controls.Add(this.txtCalendarOutlookFolder);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.txtCalendarIMAPFolder);
            this.groupBox2.Location = new System.Drawing.Point(12, 167);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(550, 149);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Calendar";
            // 
            // btnSelectOutlookCalendarFolder
            // 
            this.btnSelectOutlookCalendarFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectOutlookCalendarFolder.Location = new System.Drawing.Point(388, 49);
            this.btnSelectOutlookCalendarFolder.Name = "btnSelectOutlookCalendarFolder";
            this.btnSelectOutlookCalendarFolder.Size = new System.Drawing.Size(75, 23);
            this.btnSelectOutlookCalendarFolder.TabIndex = 8;
            this.btnSelectOutlookCalendarFolder.Text = "Select";
            this.btnSelectOutlookCalendarFolder.UseVisualStyleBackColor = true;
            this.btnSelectOutlookCalendarFolder.Click += new System.EventHandler(this.btnSelectOutlookCalendarFolder_Click);
            // 
            // btnSelectCalendarFolder
            // 
            this.btnSelectCalendarFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectCalendarFolder.Location = new System.Drawing.Point(388, 23);
            this.btnSelectCalendarFolder.Name = "btnSelectCalendarFolder";
            this.btnSelectCalendarFolder.Size = new System.Drawing.Size(75, 23);
            this.btnSelectCalendarFolder.TabIndex = 5;
            this.btnSelectCalendarFolder.Text = "Select";
            this.btnSelectCalendarFolder.UseVisualStyleBackColor = true;
            this.btnSelectCalendarFolder.Click += new System.EventHandler(this.btnSelectCalendarFolder_Click);
            // 
            // txtCalendarOutlookFolder
            // 
            this.txtCalendarOutlookFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCalendarOutlookFolder.Location = new System.Drawing.Point(88, 51);
            this.txtCalendarOutlookFolder.Name = "txtCalendarOutlookFolder";
            this.txtCalendarOutlookFolder.ReadOnly = true;
            this.txtCalendarOutlookFolder.Size = new System.Drawing.Size(294, 20);
            this.txtCalendarOutlookFolder.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Outlook Folder";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "IMAP Folder";
            // 
            // txtCalendarIMAPFolder
            // 
            this.txtCalendarIMAPFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCalendarIMAPFolder.Location = new System.Drawing.Point(88, 25);
            this.txtCalendarIMAPFolder.Name = "txtCalendarIMAPFolder";
            this.txtCalendarIMAPFolder.ReadOnly = true;
            this.txtCalendarIMAPFolder.Size = new System.Drawing.Size(294, 20);
            this.txtCalendarIMAPFolder.TabIndex = 4;
            // 
            // btnClearContactsFolder
            // 
            this.btnClearContactsFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearContactsFolder.Location = new System.Drawing.Point(469, 26);
            this.btnClearContactsFolder.Name = "btnClearContactsFolder";
            this.btnClearContactsFolder.Size = new System.Drawing.Size(75, 23);
            this.btnClearContactsFolder.TabIndex = 6;
            this.btnClearContactsFolder.Text = "Clear";
            this.btnClearContactsFolder.UseVisualStyleBackColor = true;
            this.btnClearContactsFolder.Click += new System.EventHandler(this.btnClearContactsFolder_Click);
            // 
            // btnClearOutlookContactsFolder
            // 
            this.btnClearOutlookContactsFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearOutlookContactsFolder.Location = new System.Drawing.Point(469, 52);
            this.btnClearOutlookContactsFolder.Name = "btnClearOutlookContactsFolder";
            this.btnClearOutlookContactsFolder.Size = new System.Drawing.Size(75, 23);
            this.btnClearOutlookContactsFolder.TabIndex = 7;
            this.btnClearOutlookContactsFolder.Text = "Clear";
            this.btnClearOutlookContactsFolder.UseVisualStyleBackColor = true;
            this.btnClearOutlookContactsFolder.Click += new System.EventHandler(this.btnClearOutlookContactsFolder_Click);
            // 
            // btnClearCalendarFolder
            // 
            this.btnClearCalendarFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearCalendarFolder.Location = new System.Drawing.Point(469, 23);
            this.btnClearCalendarFolder.Name = "btnClearCalendarFolder";
            this.btnClearCalendarFolder.Size = new System.Drawing.Size(75, 23);
            this.btnClearCalendarFolder.TabIndex = 8;
            this.btnClearCalendarFolder.Text = "Clear";
            this.btnClearCalendarFolder.UseVisualStyleBackColor = true;
            this.btnClearCalendarFolder.Click += new System.EventHandler(this.btnClearCalendarFolder_Click);
            // 
            // btnClearOutlookCalendarFolder
            // 
            this.btnClearOutlookCalendarFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearOutlookCalendarFolder.Location = new System.Drawing.Point(469, 49);
            this.btnClearOutlookCalendarFolder.Name = "btnClearOutlookCalendarFolder";
            this.btnClearOutlookCalendarFolder.Size = new System.Drawing.Size(75, 23);
            this.btnClearOutlookCalendarFolder.TabIndex = 9;
            this.btnClearOutlookCalendarFolder.Text = "Clear";
            this.btnClearOutlookCalendarFolder.UseVisualStyleBackColor = true;
            this.btnClearOutlookCalendarFolder.Click += new System.EventHandler(this.btnClearOutlookCalendarFolder_Click);
            // 
            // DlgSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(574, 362);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.MinimumSize = new System.Drawing.Size(450, 400);
            this.Name = "DlgSettings";
            this.Text = "Settings";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnSelectContactsFolder;
        private System.Windows.Forms.TextBox txtContactsIMAPFolder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSelectCalendarFolder;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtCalendarIMAPFolder;
        private System.Windows.Forms.Button btnSelectOutlookContactsFolder;
        private System.Windows.Forms.TextBox txtContactsOutlookFolder;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnSelectOutlookCalendarFolder;
        private System.Windows.Forms.TextBox txtCalendarOutlookFolder;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnClearContactsFolder;
        private System.Windows.Forms.Button btnClearOutlookContactsFolder;
        private System.Windows.Forms.Button btnClearOutlookCalendarFolder;
        private System.Windows.Forms.Button btnClearCalendarFolder;
    }
}