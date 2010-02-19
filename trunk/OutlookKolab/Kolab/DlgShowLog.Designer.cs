namespace OutlookKolab.Kolab
{
    partial class DlgShowLog
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
            this.btnClose = new System.Windows.Forms.Button();
            this.dsStatus1 = new OutlookKolab.Kolab.Provider.DSStatus();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.timeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.taskDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.itemsDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.localChangedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.remoteChangedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.localNewDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.remoteNewDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.localDeletedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.remoteDeletedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.conflictedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnClearLog = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dsStatus1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(1018, 443);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // dsStatus1
            // 
            this.dsStatus1.DataSetName = "DSStatus";
            this.dsStatus1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // bindingSource1
            // 
            this.bindingSource1.DataMember = "StatusEntry";
            this.bindingSource1.DataSource = this.dsStatus1;
            this.bindingSource1.Sort = "";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.timeDataGridViewTextBoxColumn,
            this.taskDataGridViewTextBoxColumn,
            this.itemsDataGridViewTextBoxColumn,
            this.localChangedDataGridViewTextBoxColumn,
            this.remoteChangedDataGridViewTextBoxColumn,
            this.localNewDataGridViewTextBoxColumn,
            this.remoteNewDataGridViewTextBoxColumn,
            this.localDeletedDataGridViewTextBoxColumn,
            this.remoteDeletedDataGridViewTextBoxColumn,
            this.conflictedDataGridViewTextBoxColumn});
            this.dataGridView1.DataSource = this.bindingSource1;
            this.dataGridView1.Location = new System.Drawing.Point(12, 12);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(1081, 425);
            this.dataGridView1.TabIndex = 1;
            // 
            // timeDataGridViewTextBoxColumn
            // 
            this.timeDataGridViewTextBoxColumn.DataPropertyName = "time";
            this.timeDataGridViewTextBoxColumn.HeaderText = "time";
            this.timeDataGridViewTextBoxColumn.Name = "timeDataGridViewTextBoxColumn";
            this.timeDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // taskDataGridViewTextBoxColumn
            // 
            this.taskDataGridViewTextBoxColumn.DataPropertyName = "task";
            this.taskDataGridViewTextBoxColumn.HeaderText = "task";
            this.taskDataGridViewTextBoxColumn.Name = "taskDataGridViewTextBoxColumn";
            this.taskDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // itemsDataGridViewTextBoxColumn
            // 
            this.itemsDataGridViewTextBoxColumn.DataPropertyName = "items";
            this.itemsDataGridViewTextBoxColumn.HeaderText = "items";
            this.itemsDataGridViewTextBoxColumn.Name = "itemsDataGridViewTextBoxColumn";
            this.itemsDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // localChangedDataGridViewTextBoxColumn
            // 
            this.localChangedDataGridViewTextBoxColumn.DataPropertyName = "localChanged";
            this.localChangedDataGridViewTextBoxColumn.HeaderText = "localChanged";
            this.localChangedDataGridViewTextBoxColumn.Name = "localChangedDataGridViewTextBoxColumn";
            this.localChangedDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // remoteChangedDataGridViewTextBoxColumn
            // 
            this.remoteChangedDataGridViewTextBoxColumn.DataPropertyName = "remoteChanged";
            this.remoteChangedDataGridViewTextBoxColumn.HeaderText = "remoteChanged";
            this.remoteChangedDataGridViewTextBoxColumn.Name = "remoteChangedDataGridViewTextBoxColumn";
            this.remoteChangedDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // localNewDataGridViewTextBoxColumn
            // 
            this.localNewDataGridViewTextBoxColumn.DataPropertyName = "localNew";
            this.localNewDataGridViewTextBoxColumn.HeaderText = "localNew";
            this.localNewDataGridViewTextBoxColumn.Name = "localNewDataGridViewTextBoxColumn";
            this.localNewDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // remoteNewDataGridViewTextBoxColumn
            // 
            this.remoteNewDataGridViewTextBoxColumn.DataPropertyName = "remoteNew";
            this.remoteNewDataGridViewTextBoxColumn.HeaderText = "remoteNew";
            this.remoteNewDataGridViewTextBoxColumn.Name = "remoteNewDataGridViewTextBoxColumn";
            this.remoteNewDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // localDeletedDataGridViewTextBoxColumn
            // 
            this.localDeletedDataGridViewTextBoxColumn.DataPropertyName = "localDeleted";
            this.localDeletedDataGridViewTextBoxColumn.HeaderText = "localDeleted";
            this.localDeletedDataGridViewTextBoxColumn.Name = "localDeletedDataGridViewTextBoxColumn";
            this.localDeletedDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // remoteDeletedDataGridViewTextBoxColumn
            // 
            this.remoteDeletedDataGridViewTextBoxColumn.DataPropertyName = "remoteDeleted";
            this.remoteDeletedDataGridViewTextBoxColumn.HeaderText = "remoteDeleted";
            this.remoteDeletedDataGridViewTextBoxColumn.Name = "remoteDeletedDataGridViewTextBoxColumn";
            this.remoteDeletedDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // conflictedDataGridViewTextBoxColumn
            // 
            this.conflictedDataGridViewTextBoxColumn.DataPropertyName = "conflicted";
            this.conflictedDataGridViewTextBoxColumn.HeaderText = "conflicted";
            this.conflictedDataGridViewTextBoxColumn.Name = "conflictedDataGridViewTextBoxColumn";
            this.conflictedDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // btnClearLog
            // 
            this.btnClearLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearLog.Location = new System.Drawing.Point(937, 443);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(75, 23);
            this.btnClearLog.TabIndex = 2;
            this.btnClearLog.Text = "Clear Log";
            this.btnClearLog.UseVisualStyleBackColor = true;
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            // 
            // DlgShowLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1105, 478);
            this.Controls.Add(this.btnClearLog);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.btnClose);
            this.Name = "DlgShowLog";
            this.Text = "Log";
            this.Load += new System.EventHandler(this.DlgShowLog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dsStatus1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private OutlookKolab.Kolab.Provider.DSStatus dsStatus1;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn timeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn taskDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn itemsDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn localChangedDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn remoteChangedDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn localNewDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn remoteNewDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn localDeletedDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn remoteDeletedDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn conflictedDataGridViewTextBoxColumn;
        private System.Windows.Forms.Button btnClearLog;
    }
}