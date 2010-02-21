namespace OutlookKolab.Kolab.Sync
{
    partial class DlgConflictDialog
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
            this.components = new System.ComponentModel.Container();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btnClose = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnUseLocal = new System.Windows.Forms.Button();
            this.btnUseAllLocal = new System.Windows.Forms.Button();
            this.btnUseAllRemote = new System.Windows.Forms.Button();
            this.btnUseRemote = new System.Windows.Forms.Button();
            this.txtLocal = new System.Windows.Forms.TextBox();
            this.txtRemote = new System.Windows.Forms.TextBox();
            this.lbStatus = new System.Windows.Forms.Label();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.LocalItemText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RemoteItemText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.localItemDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cacheEntryDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.messageDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.localItemTextDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.remoteItemTextDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.LocalItemText,
            this.RemoteItemText,
            this.localItemDataGridViewTextBoxColumn,
            this.cacheEntryDataGridViewTextBoxColumn,
            this.messageDataGridViewTextBoxColumn,
            this.localItemTextDataGridViewTextBoxColumn,
            this.remoteItemTextDataGridViewTextBoxColumn});
            this.dataGridView1.DataSource = this.bindingSource1;
            this.dataGridView1.Location = new System.Drawing.Point(12, 12);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(802, 174);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(739, 529);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 192);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(802, 331);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtLocal);
            this.groupBox1.Controls.Add(this.btnUseAllLocal);
            this.groupBox1.Controls.Add(this.btnUseLocal);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(395, 325);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Local";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtRemote);
            this.groupBox2.Controls.Add(this.btnUseAllRemote);
            this.groupBox2.Controls.Add(this.btnUseRemote);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(404, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(395, 325);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Remote";
            // 
            // btnUseLocal
            // 
            this.btnUseLocal.Location = new System.Drawing.Point(6, 19);
            this.btnUseLocal.Name = "btnUseLocal";
            this.btnUseLocal.Size = new System.Drawing.Size(75, 23);
            this.btnUseLocal.TabIndex = 0;
            this.btnUseLocal.Text = "Use";
            this.btnUseLocal.UseVisualStyleBackColor = true;
            this.btnUseLocal.Click += new System.EventHandler(this.btnUseLocal_Click);
            // 
            // btnUseAllLocal
            // 
            this.btnUseAllLocal.Location = new System.Drawing.Point(87, 19);
            this.btnUseAllLocal.Name = "btnUseAllLocal";
            this.btnUseAllLocal.Size = new System.Drawing.Size(75, 23);
            this.btnUseAllLocal.TabIndex = 1;
            this.btnUseAllLocal.Text = "Use all";
            this.btnUseAllLocal.UseVisualStyleBackColor = true;
            this.btnUseAllLocal.Click += new System.EventHandler(this.btnUseAllLocal_Click);
            // 
            // btnUseAllRemote
            // 
            this.btnUseAllRemote.Location = new System.Drawing.Point(87, 19);
            this.btnUseAllRemote.Name = "btnUseAllRemote";
            this.btnUseAllRemote.Size = new System.Drawing.Size(75, 23);
            this.btnUseAllRemote.TabIndex = 3;
            this.btnUseAllRemote.Text = "Use all";
            this.btnUseAllRemote.UseVisualStyleBackColor = true;
            this.btnUseAllRemote.Click += new System.EventHandler(this.btnUseAllRemote_Click);
            // 
            // btnUseRemote
            // 
            this.btnUseRemote.Location = new System.Drawing.Point(6, 19);
            this.btnUseRemote.Name = "btnUseRemote";
            this.btnUseRemote.Size = new System.Drawing.Size(75, 23);
            this.btnUseRemote.TabIndex = 2;
            this.btnUseRemote.Text = "Use";
            this.btnUseRemote.UseVisualStyleBackColor = true;
            this.btnUseRemote.Click += new System.EventHandler(this.btnUseRemote_Click);
            // 
            // txtLocal
            // 
            this.txtLocal.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLocal.Location = new System.Drawing.Point(6, 48);
            this.txtLocal.Multiline = true;
            this.txtLocal.Name = "txtLocal";
            this.txtLocal.ReadOnly = true;
            this.txtLocal.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLocal.Size = new System.Drawing.Size(383, 271);
            this.txtLocal.TabIndex = 2;
            // 
            // txtRemote
            // 
            this.txtRemote.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRemote.Location = new System.Drawing.Point(6, 48);
            this.txtRemote.Multiline = true;
            this.txtRemote.Name = "txtRemote";
            this.txtRemote.ReadOnly = true;
            this.txtRemote.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtRemote.Size = new System.Drawing.Size(383, 271);
            this.txtRemote.TabIndex = 3;
            // 
            // lbStatus
            // 
            this.lbStatus.AutoSize = true;
            this.lbStatus.Location = new System.Drawing.Point(18, 534);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(0, 13);
            this.lbStatus.TabIndex = 5;
            // 
            // bindingSource1
            // 
            this.bindingSource1.DataSource = typeof(OutlookKolab.Kolab.Sync.SyncContext);
            // 
            // LocalItemText
            // 
            this.LocalItemText.DataPropertyName = "LocalItemText";
            this.LocalItemText.HeaderText = "LocalItemText";
            this.LocalItemText.Name = "LocalItemText";
            this.LocalItemText.ReadOnly = true;
            this.LocalItemText.Width = 300;
            // 
            // RemoteItemText
            // 
            this.RemoteItemText.DataPropertyName = "RemoteItemText";
            this.RemoteItemText.HeaderText = "RemoteItemText";
            this.RemoteItemText.Name = "RemoteItemText";
            this.RemoteItemText.ReadOnly = true;
            this.RemoteItemText.Width = 300;
            // 
            // localItemDataGridViewTextBoxColumn
            // 
            this.localItemDataGridViewTextBoxColumn.DataPropertyName = "LocalItem";
            this.localItemDataGridViewTextBoxColumn.HeaderText = "LocalItem";
            this.localItemDataGridViewTextBoxColumn.Name = "localItemDataGridViewTextBoxColumn";
            this.localItemDataGridViewTextBoxColumn.ReadOnly = true;
            this.localItemDataGridViewTextBoxColumn.Visible = false;
            // 
            // cacheEntryDataGridViewTextBoxColumn
            // 
            this.cacheEntryDataGridViewTextBoxColumn.DataPropertyName = "CacheEntry";
            this.cacheEntryDataGridViewTextBoxColumn.HeaderText = "CacheEntry";
            this.cacheEntryDataGridViewTextBoxColumn.Name = "cacheEntryDataGridViewTextBoxColumn";
            this.cacheEntryDataGridViewTextBoxColumn.ReadOnly = true;
            this.cacheEntryDataGridViewTextBoxColumn.Visible = false;
            // 
            // messageDataGridViewTextBoxColumn
            // 
            this.messageDataGridViewTextBoxColumn.DataPropertyName = "Message";
            this.messageDataGridViewTextBoxColumn.HeaderText = "Message";
            this.messageDataGridViewTextBoxColumn.Name = "messageDataGridViewTextBoxColumn";
            this.messageDataGridViewTextBoxColumn.ReadOnly = true;
            this.messageDataGridViewTextBoxColumn.Visible = false;
            // 
            // localItemTextDataGridViewTextBoxColumn
            // 
            this.localItemTextDataGridViewTextBoxColumn.DataPropertyName = "LocalItemText";
            this.localItemTextDataGridViewTextBoxColumn.HeaderText = "LocalItemText";
            this.localItemTextDataGridViewTextBoxColumn.Name = "localItemTextDataGridViewTextBoxColumn";
            this.localItemTextDataGridViewTextBoxColumn.ReadOnly = true;
            this.localItemTextDataGridViewTextBoxColumn.Visible = false;
            // 
            // remoteItemTextDataGridViewTextBoxColumn
            // 
            this.remoteItemTextDataGridViewTextBoxColumn.DataPropertyName = "RemoteItemText";
            this.remoteItemTextDataGridViewTextBoxColumn.HeaderText = "RemoteItemText";
            this.remoteItemTextDataGridViewTextBoxColumn.Name = "remoteItemTextDataGridViewTextBoxColumn";
            this.remoteItemTextDataGridViewTextBoxColumn.ReadOnly = true;
            this.remoteItemTextDataGridViewTextBoxColumn.Visible = false;
            // 
            // DlgConflictDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(826, 564);
            this.Controls.Add(this.lbStatus);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.dataGridView1);
            this.Name = "DlgConflictDialog";
            this.Text = "Resolve conflicts";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtRemote;
        private System.Windows.Forms.Button btnUseAllRemote;
        private System.Windows.Forms.Button btnUseRemote;
        private System.Windows.Forms.TextBox txtLocal;
        private System.Windows.Forms.Button btnUseAllLocal;
        private System.Windows.Forms.Button btnUseLocal;
        private System.Windows.Forms.Label lbStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn LocalItemText;
        private System.Windows.Forms.DataGridViewTextBoxColumn RemoteItemText;
        private System.Windows.Forms.DataGridViewTextBoxColumn localItemDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn cacheEntryDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn messageDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn localItemTextDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn remoteItemTextDataGridViewTextBoxColumn;
    }
}