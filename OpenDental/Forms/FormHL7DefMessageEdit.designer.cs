namespace OpenDental{
	partial class FormHL7DefMessageEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormHL7DefMessageEdit));
			this.radioOut = new System.Windows.Forms.RadioButton();
			this.radioIn = new System.Windows.Forms.RadioButton();
			this.comboMessageStructure = new System.Windows.Forms.ComboBox();
			this.comboMsgType = new System.Windows.Forms.ComboBox();
			this.labelItemOrder = new System.Windows.Forms.Label();
			this.textNote = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.labelDelete = new System.Windows.Forms.Label();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.butAdd = new OpenDental.UI.Button();
			this.textItemOrder = new OpenDental.ValidNum();
			this.SuspendLayout();
			// 
			// radioOut
			// 
			this.radioOut.Location = new System.Drawing.Point(78, 85);
			this.radioOut.Name = "radioOut";
			this.radioOut.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.radioOut.Size = new System.Drawing.Size(100, 18);
			this.radioOut.TabIndex = 15;
			this.radioOut.TabStop = true;
			this.radioOut.Text = "Outgoing";
			this.radioOut.UseVisualStyleBackColor = true;
			// 
			// radioIn
			// 
			this.radioIn.Location = new System.Drawing.Point(78, 65);
			this.radioIn.Name = "radioIn";
			this.radioIn.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.radioIn.Size = new System.Drawing.Size(100, 18);
			this.radioIn.TabIndex = 14;
			this.radioIn.TabStop = true;
			this.radioIn.Text = "Incoming";
			this.radioIn.UseVisualStyleBackColor = true;
			// 
			// comboMessageStructure
			// 
			this.comboMessageStructure.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboMessageStructure.Location = new System.Drawing.Point(164, 42);
			this.comboMessageStructure.MaxDropDownItems = 100;
			this.comboMessageStructure.Name = "comboMessageStructure";
			this.comboMessageStructure.Size = new System.Drawing.Size(138, 21);
			this.comboMessageStructure.TabIndex = 13;
			// 
			// comboMsgType
			// 
			this.comboMsgType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboMsgType.Location = new System.Drawing.Point(164, 21);
			this.comboMsgType.MaxDropDownItems = 100;
			this.comboMsgType.Name = "comboMsgType";
			this.comboMsgType.Size = new System.Drawing.Size(138, 21);
			this.comboMsgType.TabIndex = 12;
			// 
			// labelItemOrder
			// 
			this.labelItemOrder.Location = new System.Drawing.Point(38, 105);
			this.labelItemOrder.Name = "labelItemOrder";
			this.labelItemOrder.Size = new System.Drawing.Size(125, 18);
			this.labelItemOrder.TabIndex = 10;
			this.labelItemOrder.Text = "Item Order";
			this.labelItemOrder.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textNote
			// 
			this.textNote.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.textNote.Location = new System.Drawing.Point(424, 16);
			this.textNote.Multiline = true;
			this.textNote.Name = "textNote";
			this.textNote.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textNote.Size = new System.Drawing.Size(285, 109);
			this.textNote.TabIndex = 17;
			// 
			// label12
			// 
			this.label12.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.label12.Location = new System.Drawing.Point(313, 16);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(110, 18);
			this.label12.TabIndex = 8;
			this.label12.Text = "Note";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(38, 20);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(125, 18);
			this.label10.TabIndex = 9;
			this.label10.Text = "Message Type";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(38, 42);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(125, 18);
			this.label8.TabIndex = 11;
			this.label8.Text = "Message Structure";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelDelete
			// 
			this.labelDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelDelete.Location = new System.Drawing.Point(108, 492);
			this.labelDelete.Name = "labelDelete";
			this.labelDelete.Size = new System.Drawing.Size(311, 28);
			this.labelDelete.TabIndex = 66;
			this.labelDelete.Text = "This HL7Def is internal. To edit this HL7Def you must first copy it to the Custom" +
    " list.";
			this.labelDelete.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelDelete.Visible = false;
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(17, 140);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(692, 348);
			this.gridMain.TabIndex = 7;
			this.gridMain.Title = "Segments";
			this.gridMain.TranslationName = "TableSegments";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(548, 494);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(634, 494);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(17, 494);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(85, 24);
			this.butDelete.TabIndex = 18;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(425, 494);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(80, 24);
			this.butAdd.TabIndex = 0;
			this.butAdd.Text = "Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// textItemOrder
			// 
			this.textItemOrder.Location = new System.Drawing.Point(164, 103);
			this.textItemOrder.MaxVal = 255;
			this.textItemOrder.MinVal = 0;
			this.textItemOrder.Name = "textItemOrder";
			this.textItemOrder.Size = new System.Drawing.Size(34, 20);
			this.textItemOrder.TabIndex = 67;
			// 
			// FormHL7DefMessageEdit
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.ClientSize = new System.Drawing.Size(725, 534);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.textItemOrder);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.labelDelete);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.radioOut);
			this.Controls.Add(this.radioIn);
			this.Controls.Add(this.comboMessageStructure);
			this.Controls.Add(this.comboMsgType);
			this.Controls.Add(this.labelItemOrder);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.gridMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormHL7DefMessageEdit";
			this.ShowInTaskbar = false;
			this.Text = "HL7 Def Message Edit";
			this.Load += new System.EventHandler(this.FormHL7DefMessageEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private UI.GridOD gridMain;
		private System.Windows.Forms.RadioButton radioOut;
		private System.Windows.Forms.RadioButton radioIn;
		private System.Windows.Forms.ComboBox comboMessageStructure;
		private System.Windows.Forms.ComboBox comboMsgType;
		private System.Windows.Forms.Label labelItemOrder;
		private System.Windows.Forms.TextBox textNote;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label8;
		private UI.Button butDelete;
		private System.Windows.Forms.Label labelDelete;
		private UI.Button butAdd;
		private ValidNum textItemOrder;
	}
}