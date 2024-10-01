namespace OpenDental{
	partial class FormHL7DefFieldEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormHL7DefFieldEdit));
			this.label1 = new System.Windows.Forms.Label();
			this.listFieldNames = new OpenDental.UI.ListBox();
			this.comboDataType = new OpenDental.UI.ComboBox();
			this.textTableId = new System.Windows.Forms.TextBox();
			this.labelItemOrder = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.labelDelete = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textFixedText = new System.Windows.Forms.TextBox();
			this.textItemOrder = new OpenDental.ValidNum();
			this.butSave = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(11, 57);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(125, 18);
			this.label1.TabIndex = 14;
			this.label1.Text = "Item Order";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// listFieldNames
			// 
			this.listFieldNames.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listFieldNames.Location = new System.Drawing.Point(136, 103);
			this.listFieldNames.Name = "listFieldNames";
			this.listFieldNames.Size = new System.Drawing.Size(196, 511);
			this.listFieldNames.TabIndex = 13;
			this.listFieldNames.Click += new System.EventHandler(this.listFieldNames_Click);
			// 
			// comboDataType
			// 
			this.comboDataType.Location = new System.Drawing.Point(136, 10);
			this.comboDataType.Name = "comboDataType";
			this.comboDataType.Size = new System.Drawing.Size(138, 21);
			this.comboDataType.TabIndex = 11;
			// 
			// textTableId
			// 
			this.textTableId.Location = new System.Drawing.Point(136, 34);
			this.textTableId.Name = "textTableId";
			this.textTableId.Size = new System.Drawing.Size(83, 20);
			this.textTableId.TabIndex = 12;
			// 
			// labelItemOrder
			// 
			this.labelItemOrder.Location = new System.Drawing.Point(11, 34);
			this.labelItemOrder.Name = "labelItemOrder";
			this.labelItemOrder.Size = new System.Drawing.Size(125, 18);
			this.labelItemOrder.TabIndex = 8;
			this.labelItemOrder.Text = "Table ID";
			this.labelItemOrder.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(11, 80);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(125, 18);
			this.label12.TabIndex = 9;
			this.label12.Text = "Fixed Text";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(11, 10);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(125, 18);
			this.label10.TabIndex = 10;
			this.label10.Text = "Data Type";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelDelete
			// 
			this.labelDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelDelete.Location = new System.Drawing.Point(11, 566);
			this.labelDelete.Name = "labelDelete";
			this.labelDelete.Size = new System.Drawing.Size(125, 69);
			this.labelDelete.TabIndex = 66;
			this.labelDelete.Text = "This HL7Def is internal. To edit this HL7Def you must first copy it to the Custom" +
    " list.";
			this.labelDelete.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelDelete.Visible = false;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(11, 103);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(125, 18);
			this.label2.TabIndex = 67;
			this.label2.Text = "Field Name";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textFixedText
			// 
			this.textFixedText.Location = new System.Drawing.Point(136, 80);
			this.textFixedText.Name = "textFixedText";
			this.textFixedText.Size = new System.Drawing.Size(199, 20);
			this.textFixedText.TabIndex = 68;
			this.textFixedText.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textFixedText_KeyUp);
			// 
			// textItemOrder
			// 
			this.textItemOrder.Location = new System.Drawing.Point(136, 57);
			this.textItemOrder.MaxVal = 255;
			this.textItemOrder.MinVal = 1;
			this.textItemOrder.Name = "textItemOrder";
			this.textItemOrder.Size = new System.Drawing.Size(34, 20);
			this.textItemOrder.TabIndex = 15;
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(257, 642);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 3;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 642);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(85, 24);
			this.butDelete.TabIndex = 65;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// FormHL7DefFieldEdit
			// 
			this.ClientSize = new System.Drawing.Size(344, 678);
			this.Controls.Add(this.butSave);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Controls.Add(this.textFixedText);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textItemOrder);
			this.Controls.Add(this.labelDelete);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listFieldNames);
			this.Controls.Add(this.comboDataType);
			this.Controls.Add(this.textTableId);
			this.Controls.Add(this.labelItemOrder);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.label10);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormHL7DefFieldEdit";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "HL7 Def Field Edit";
			this.Load += new System.EventHandler(this.FormHL7DefFieldEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butSave;
		private System.Windows.Forms.Label label1;
		private OpenDental.UI.ListBox listFieldNames;
		private OpenDental.UI.ComboBox comboDataType;
		private System.Windows.Forms.TextBox textTableId;
		private System.Windows.Forms.Label labelItemOrder;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label10;
		private UI.Button butDelete;
		private System.Windows.Forms.Label labelDelete;
		private ValidNum textItemOrder;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textFixedText;
	}
}
