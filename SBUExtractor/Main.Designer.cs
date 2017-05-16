namespace SBUExtractor
{
    partial class Main
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.chooseFileGroup = new System.Windows.Forms.GroupBox();
            this.fileTextBox = new System.Windows.Forms.TextBox();
            this.openButton = new System.Windows.Forms.Button();
            this.chooseDataGroup = new System.Windows.Forms.GroupBox();
            this.countButton = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.selectedCheckBox = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dataName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.extractDataGroup = new System.Windows.Forms.GroupBox();
            this.extractDataButton = new System.Windows.Forms.Button();
            this.extractAllButton = new System.Windows.Forms.Button();
            this.chooseFileGroup.SuspendLayout();
            this.chooseDataGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.extractDataGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // chooseFileGroup
            // 
            this.chooseFileGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.chooseFileGroup.Controls.Add(this.fileTextBox);
            this.chooseFileGroup.Controls.Add(this.openButton);
            this.chooseFileGroup.Location = new System.Drawing.Point(12, 12);
            this.chooseFileGroup.Name = "chooseFileGroup";
            this.chooseFileGroup.Size = new System.Drawing.Size(627, 58);
            this.chooseFileGroup.TabIndex = 0;
            this.chooseFileGroup.TabStop = false;
            this.chooseFileGroup.Text = "Choose .SBU file";
            // 
            // fileTextBox
            // 
            this.fileTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fileTextBox.Location = new System.Drawing.Point(7, 20);
            this.fileTextBox.Name = "fileTextBox";
            this.fileTextBox.ReadOnly = true;
            this.fileTextBox.Size = new System.Drawing.Size(533, 20);
            this.fileTextBox.TabIndex = 1;
            this.fileTextBox.Text = "C:\\whatever.SBU";
            // 
            // openButton
            // 
            this.openButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.openButton.Location = new System.Drawing.Point(546, 19);
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(75, 23);
            this.openButton.TabIndex = 0;
            this.openButton.Text = "Open";
            this.openButton.UseVisualStyleBackColor = true;
            this.openButton.Click += new System.EventHandler(this.openButton_Click);
            // 
            // chooseDataGroup
            // 
            this.chooseDataGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.chooseDataGroup.Controls.Add(this.countButton);
            this.chooseDataGroup.Controls.Add(this.dataGridView1);
            this.chooseDataGroup.Location = new System.Drawing.Point(13, 76);
            this.chooseDataGroup.Name = "chooseDataGroup";
            this.chooseDataGroup.Size = new System.Drawing.Size(626, 344);
            this.chooseDataGroup.TabIndex = 1;
            this.chooseDataGroup.TabStop = false;
            this.chooseDataGroup.Text = "Choose data to extract";
            // 
            // countButton
            // 
            this.countButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.countButton.Location = new System.Drawing.Point(501, 315);
            this.countButton.Name = "countButton";
            this.countButton.Size = new System.Drawing.Size(119, 23);
            this.countButton.TabIndex = 1;
            this.countButton.Text = "Count selected data";
            this.countButton.UseVisualStyleBackColor = true;
            this.countButton.Click += new System.EventHandler(this.countButton_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.selectedCheckBox,
            this.dataName,
            this.dataCount});
            this.dataGridView1.Location = new System.Drawing.Point(7, 20);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.Size = new System.Drawing.Size(613, 289);
            this.dataGridView1.TabIndex = 0;
            // 
            // selectedCheckBox
            // 
            this.selectedCheckBox.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.selectedCheckBox.Frozen = true;
            this.selectedCheckBox.HeaderText = "Selected";
            this.selectedCheckBox.Name = "selectedCheckBox";
            this.selectedCheckBox.Width = 55;
            // 
            // dataName
            // 
            this.dataName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataName.HeaderText = "Data";
            this.dataName.Name = "dataName";
            this.dataName.ReadOnly = true;
            // 
            // dataCount
            // 
            this.dataCount.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataCount.HeaderText = "Count";
            this.dataCount.Name = "dataCount";
            this.dataCount.ReadOnly = true;
            this.dataCount.Width = 60;
            // 
            // extractDataGroup
            // 
            this.extractDataGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.extractDataGroup.Controls.Add(this.extractDataButton);
            this.extractDataGroup.Controls.Add(this.extractAllButton);
            this.extractDataGroup.Location = new System.Drawing.Point(12, 427);
            this.extractDataGroup.Name = "extractDataGroup";
            this.extractDataGroup.Size = new System.Drawing.Size(627, 52);
            this.extractDataGroup.TabIndex = 2;
            this.extractDataGroup.TabStop = false;
            this.extractDataGroup.Text = "Extract data";
            // 
            // extractDataButton
            // 
            this.extractDataButton.Location = new System.Drawing.Point(7, 20);
            this.extractDataButton.Name = "extractDataButton";
            this.extractDataButton.Size = new System.Drawing.Size(100, 23);
            this.extractDataButton.TabIndex = 1;
            this.extractDataButton.Text = "Extract selected data";
            this.extractDataButton.UseVisualStyleBackColor = true;
            this.extractDataButton.Click += new System.EventHandler(this.extractDataButton_Click);
            // 
            // extractAllButton
            // 
            this.extractAllButton.Location = new System.Drawing.Point(113, 20);
            this.extractAllButton.Name = "extractAllButton";
            this.extractAllButton.Size = new System.Drawing.Size(98, 23);
            this.extractAllButton.TabIndex = 0;
            this.extractAllButton.Text = "Extract all data";
            this.extractAllButton.UseVisualStyleBackColor = true;
            this.extractAllButton.Click += new System.EventHandler(this.extractAllButton_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(655, 487);
            this.Controls.Add(this.extractDataGroup);
            this.Controls.Add(this.chooseDataGroup);
            this.Controls.Add(this.chooseFileGroup);
            this.Name = "Main";
            this.Text = "SBU Extractor by Lord_Luncher";
            this.Load += new System.EventHandler(this.Main_Load);
            this.chooseFileGroup.ResumeLayout(false);
            this.chooseFileGroup.PerformLayout();
            this.chooseDataGroup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.extractDataGroup.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox chooseFileGroup;
        private System.Windows.Forms.TextBox fileTextBox;
        private System.Windows.Forms.Button openButton;
        private System.Windows.Forms.GroupBox chooseDataGroup;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button countButton;
        private System.Windows.Forms.DataGridViewCheckBoxColumn selectedCheckBox;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataCount;
        private System.Windows.Forms.GroupBox extractDataGroup;
        private System.Windows.Forms.Button extractDataButton;
        private System.Windows.Forms.Button extractAllButton;
    }
}

