namespace OpenDental{
	partial class FormHL7DefSegmentEdit {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormHL7DefSegmentEdit));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.checkCanRepeat = new System.Windows.Forms.CheckBox();
			this.checkIsOptional = new System.Windows.Forms.CheckBox();
			this.comboSegmentName = new System.Windows.Forms.ComboBox();
			this.labelItemOrder = new System.Windows.Forms.Label();
			this.textNote = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.butDelete = new OpenDental.UI.Button();
			this.labelDelete = new System.Windows.Forms.Label();
			this.butAdd = new OpenDental.UI.Button();
			this.textItemOrder = new OpenDental.ValidNum();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(477, 401);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(563, 401);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(17, 140);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(621, 255);
			this.gridMain.TabIndex = 7;
			this.gridMain.Title = "Fields";
			this.gridMain.TranslationName = "TableFields";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// checkCanRepeat
			// 
			this.checkCanRepeat.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkCanRepeat.Location = new System.Drawing.Point(26, 70);
			this.checkCanRepeat.Name = "checkCanRepeat";
			this.checkCanRepeat.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkCanRepeat.Size = new System.Drawing.Size(137, 18);
			this.checkCanRepeat.TabIndex = 13;
			this.checkCanRepeat.TabStop = false;
			this.checkCanRepeat.Text = "Can Repeat";
			// 
			// checkIsOptional
			// 
			this.checkIsOptional.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIsOptional.Location = new System.Drawing.Point(26, 90);
			this.checkIsOptional.Name = "checkIsOptional";
			this.checkIsOptional.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkIsOptional.Size = new System.Drawing.Size(137, 18);
			this.checkIsOptional.TabIndex = 14;
			this.checkIsOptional.TabStop = false;
			this.checkIsOptional.Text = "Is Optional";
			// 
			// comboSegmentName
			// 
			this.comboSegmentName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboSegmentName.Location = new System.Drawing.Point(148, 22);
			this.comboSegmentName.MaxDropDownItems = 100;
			this.comboSegmentName.Name = "comboSegmentName";
			this.comboSegmentName.Size = new System.Drawing.Size(138, 21);
			this.comboSegmentName.TabIndex = 11;
			// 
			// labelItemOrder
			// 
			this.labelItemOrder.Location = new System.Drawing.Point(23, 47);
			this.labelItemOrder.Name = "labelItemOrder";
			this.labelItemOrder.Size = new System.Drawing.Size(125, 18);
			this.labelItemOrder.TabIndex = 8;
			this.labelItemOrder.Text = "Item Order";
			this.labelItemOrder.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textNote
			// 
			this.textNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textNote.Location = new System.Drawing.Point(353, 17);
			this.textNote.Multiline = true;
			this.textNote.Name = "textNote";
			this.textNote.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textNote.Size = new System.Drawing.Size(285, 109);
			this.textNote.TabIndex = 15;
			// 
			// label12
			// 
			this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label12.Location = new System.Drawing.Point(242, 17);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(110, 18);
			this.label12.TabIndex = 9;
			this.label12.Text = "Note";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(23, 21);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(125, 18);
			this.label10.TabIndex = 10;
			this.label10.Text = "Segment Name";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(17, 401);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(85, 24);
			this.butDelete.TabIndex = 19;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// labelDelete
			// 
			this.labelDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelDelete.Location = new System.Drawing.Point(108, 399);
			this.labelDelete.Name = "labelDelete";
			this.labelDelete.Size = new System.Drawing.Size(264, 28);
			this.labelDelete.TabIndex = 66;
			this.labelDelete.Text = "This HL7Def is internal. To edit this HL7Def you must first copy it to the Custom" +
    " list.";
			this.labelDelete.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelDelete.Visible = false;
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(378, 401);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(80, 24);
			this.butAdd.TabIndex = 67;
			this.butAdd.Text = "Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// textItemOrder
			// 
			this.textItemOrder.Location = new System.Drawing.Point(148, 47);
			this.textItemOrder.MaxVal = 255;
			this.textItemOrder.MinVal = 0;
			this.textItemOrder.Name = "textItemOrder";
			this.textItemOrder.Size = new System.Drawing.Size(34, 20);
			this.textItemOrder.TabIndex = 12;
			// 
			// FormHL7DefSegmentEdit
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.ClientSize = new System.Drawing.Size(654, 441);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.textItemOrder);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.labelDelete);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.checkCanRepeat);
			this.Controls.Add(this.checkIsOptional);
			this.Controls.Add(this.comboSegmentName);
			this.Controls.Add(this.labelItemOrder);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.gridMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormHL7DefSegmentEdit";
			this.ShowInTaskbar = false;
			this.Text = "HL7 Def Segment Edit";
			this.Load += new System.EventHandler(this.FormHL7DefSegmentEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private UI.GridOD gridMain;
		private System.Windows.Forms.CheckBox checkCanRepeat;
		private System.Windows.Forms.CheckBox checkIsOptional;
		private System.Windows.Forms.ComboBox comboSegmentName;
		private System.Windows.Forms.Label labelItemOrder;
		private System.Windows.Forms.TextBox textNote;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label10;
		private UI.Button butDelete;
		private System.Windows.Forms.Label labelDelete;
		private UI.Button butAdd;
		private ValidNum textItemOrder;
	}
}
