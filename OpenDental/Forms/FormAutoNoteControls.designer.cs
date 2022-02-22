namespace OpenDental
{
	partial class FormAutoNoteControls
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
					System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAutoNoteControls));
					this.gridMain = new OpenDental.UI.GridOD();
					this.butAdd = new OpenDental.UI.Button();
					this.butOK = new OpenDental.UI.Button();
					this.butCancel = new OpenDental.UI.Button();
					this.butEdit = new OpenDental.UI.Button();
					this.SuspendLayout();
					// 
					// gridMain
					// 
					this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
					this.gridMain.Location = new System.Drawing.Point(6,12);
					this.gridMain.Name = "gridMain";
					this.gridMain.Size = new System.Drawing.Size(842,597);
					this.gridMain.TabIndex = 106;
					this.gridMain.Title = "Controls";
					this.gridMain.TranslationName = "FormAutoNoteEdit";
					this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
					// 
					// butAdd
					// 
					this.butAdd.AdjustImageLocation = new System.Drawing.Point(0,1);
					this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
					this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
					this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
					this.butAdd.Location = new System.Drawing.Point(489,619);
					this.butAdd.Name = "butAdd";
					this.butAdd.Size = new System.Drawing.Size(78,24);
					this.butAdd.TabIndex = 105;
					this.butAdd.Text = "Add";
					this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
					// 
					// butOK
					// 
					this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
					this.butOK.Location = new System.Drawing.Point(686,619);
					this.butOK.Name = "butOK";
					this.butOK.Size = new System.Drawing.Size(78,24);
					this.butOK.TabIndex = 5;
					this.butOK.Text = "OK";
					this.butOK.Click += new System.EventHandler(this.butOK_Click);
					// 
					// butCancel
					// 
					this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
					this.butCancel.Location = new System.Drawing.Point(770,619);
					this.butCancel.Name = "butCancel";
					this.butCancel.Size = new System.Drawing.Size(78,24);
					this.butCancel.TabIndex = 6;
					this.butCancel.Text = "Cancel";
					this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
					// 
					// butEdit
					// 
					this.butEdit.AdjustImageLocation = new System.Drawing.Point(0,1);
					this.butEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
					this.butEdit.Image = global::OpenDental.Properties.Resources.editPencil;
					this.butEdit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
					this.butEdit.Location = new System.Drawing.Point(342,619);
					this.butEdit.Name = "butEdit";
					this.butEdit.Size = new System.Drawing.Size(78,24);
					this.butEdit.TabIndex = 107;
					this.butEdit.Text = "Edit";
					this.butEdit.Click += new System.EventHandler(this.butEdit_Click);
					// 
					// FormAutoNoteControls
					// 
					this.AutoScaleDimensions = new System.Drawing.SizeF(6F,13F);
					this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
					this.ClientSize = new System.Drawing.Size(867,656);
					this.Controls.Add(this.butEdit);
					this.Controls.Add(this.gridMain);
					this.Controls.Add(this.butAdd);
					this.Controls.Add(this.butOK);
					this.Controls.Add(this.butCancel);
					this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
					this.Name = "FormAutoNoteControls";
					this.ShowInTaskbar = false;
					this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
					this.Text = "Auto Note Controls";
					this.Load += new System.EventHandler(this.FormAutoNoteControls_Load);
					this.ResumeLayout(false);

        }

        #endregion

				private OpenDental.UI.Button butCancel;
				private OpenDental.UI.Button butOK;
				private OpenDental.UI.Button butAdd;
				private OpenDental.UI.GridOD gridMain;
				private OpenDental.UI.Button butEdit;
    }
}