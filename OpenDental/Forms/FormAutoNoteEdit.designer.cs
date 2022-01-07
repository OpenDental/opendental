namespace OpenDental
{
    partial class FormAutoNoteEdit
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAutoNoteEdit));
			this.textBoxAutoNoteName = new System.Windows.Forms.TextBox();
			this.labelName = new System.Windows.Forms.Label();
			this.textMain = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.butAdd = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butInsert = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// textBoxAutoNoteName
			// 
			this.textBoxAutoNoteName.Location = new System.Drawing.Point(107, 12);
			this.textBoxAutoNoteName.Name = "textBoxAutoNoteName";
			this.textBoxAutoNoteName.Size = new System.Drawing.Size(356, 20);
			this.textBoxAutoNoteName.TabIndex = 0;
			// 
			// labelName
			// 
			this.labelName.Location = new System.Drawing.Point(19, 15);
			this.labelName.Name = "labelName";
			this.labelName.Size = new System.Drawing.Size(86, 13);
			this.labelName.TabIndex = 1;
			this.labelName.Text = "Name";
			this.labelName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textMain
			// 
			this.textMain.AcceptsReturn = true;
			this.textMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.textMain.HideSelection = false;
			this.textMain.Location = new System.Drawing.Point(6, 59);
			this.textMain.Multiline = true;
			this.textMain.Name = "textMain";
			this.textMain.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textMain.Size = new System.Drawing.Size(504, 552);
			this.textMain.TabIndex = 108;
			this.textMain.Leave += new System.EventHandler(this.textMain_Leave);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(5, 43);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(86, 13);
			this.label1.TabIndex = 109;
			this.label1.Text = "Text";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(766, 8);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(79, 24);
			this.butAdd.TabIndex = 110;
			this.butAdd.Text = "Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butDelete
			// 
			this.butDelete.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(6, 620);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(79, 24);
			this.butDelete.TabIndex = 107;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(603, 38);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(242, 573);
			this.gridMain.TabIndex = 106;
			this.gridMain.Title = "Available Prompts";
			this.gridMain.TranslationName = "FormAutoNoteEdit";
			// 
			// butInsert
			// 
			this.butInsert.Image = global::OpenDental.Properties.Resources.Left;
			this.butInsert.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butInsert.Location = new System.Drawing.Point(516, 240);
			this.butInsert.Name = "butInsert";
			this.butInsert.Size = new System.Drawing.Size(79, 24);
			this.butInsert.TabIndex = 105;
			this.butInsert.Text = "Insert";
			this.butInsert.Click += new System.EventHandler(this.butInsert_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(683, 620);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(78, 24);
			this.butOK.TabIndex = 5;
			this.butOK.Text = "OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(767, 620);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(78, 24);
			this.butCancel.TabIndex = 6;
			this.butCancel.Text = "Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormAutoNoteEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(857, 656);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textMain);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butInsert);
			this.Controls.Add(this.labelName);
			this.Controls.Add(this.textBoxAutoNoteName);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormAutoNoteEdit";
			this.ShowInTaskbar = false;
			this.Text = "Auto Note Edit";
			this.Load += new System.EventHandler(this.FormAutoNoteEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxAutoNoteName;
				private System.Windows.Forms.Label labelName;
        private OpenDental.UI.Button butCancel;
				private OpenDental.UI.Button butOK;
				private OpenDental.UI.Button butInsert;
				private OpenDental.UI.Button butDelete;
				private OpenDental.UI.GridOD gridMain;
				private System.Windows.Forms.TextBox textMain;
				private System.Windows.Forms.Label label1;
				private OpenDental.UI.Button butAdd;
    }
}