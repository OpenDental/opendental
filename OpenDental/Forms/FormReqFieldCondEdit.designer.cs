namespace OpenDental{
	partial class FormReqFieldCondEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormReqFieldCondEdit));
			this.labelConditionValue2 = new System.Windows.Forms.Label();
			this.listConditionType = new OpenDental.UI.ListBoxOD();
			this.label1 = new System.Windows.Forms.Label();
			this.textConditionValue1 = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.listConditionValues = new OpenDental.UI.ListBoxOD();
			this.label3 = new System.Windows.Forms.Label();
			this.comboOperator2 = new System.Windows.Forms.ComboBox();
			this.textConditionValue2 = new System.Windows.Forms.TextBox();
			this.comboOperator1 = new System.Windows.Forms.ComboBox();
			this.listRelationships = new OpenDental.UI.ListBoxOD();
			this.butPickProv = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// labelConditionValue2
			// 
			this.labelConditionValue2.Location = new System.Drawing.Point(232, 260);
			this.labelConditionValue2.Name = "labelConditionValue2";
			this.labelConditionValue2.Size = new System.Drawing.Size(161, 20);
			this.labelConditionValue2.TabIndex = 30;
			this.labelConditionValue2.Text = "Format mm/dd/yyyy";
			this.labelConditionValue2.Visible = false;
			// 
			// listConditionType
			// 
			this.listConditionType.Location = new System.Drawing.Point(12, 46);
			this.listConditionType.Name = "listConditionType";
			this.listConditionType.Size = new System.Drawing.Size(111, 147);
			this.listConditionType.TabIndex = 28;
			this.listConditionType.SelectedIndexChanged += new System.EventHandler(this.listConditionType_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 26);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(111, 17);
			this.label1.TabIndex = 25;
			this.label1.Text = "Condition Type";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textConditionValue1
			// 
			this.textConditionValue1.Location = new System.Drawing.Point(232, 199);
			this.textConditionValue1.Name = "textConditionValue1";
			this.textConditionValue1.Size = new System.Drawing.Size(161, 20);
			this.textConditionValue1.TabIndex = 21;
			this.textConditionValue1.Visible = false;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(145, 51);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(70, 17);
			this.label2.TabIndex = 24;
			this.label2.Text = "Operator";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			this.label2.Visible = false;
			// 
			// listConditionValues
			// 
			this.listConditionValues.Location = new System.Drawing.Point(232, 46);
			this.listConditionValues.Name = "listConditionValues";
			this.listConditionValues.SelectionMode = UI.SelectionMode.MultiExtended;
			this.listConditionValues.Size = new System.Drawing.Size(132, 147);
			this.listConditionValues.TabIndex = 172;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(232, 26);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(111, 17);
			this.label3.TabIndex = 171;
			this.label3.Text = "Condition Value";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// comboOperator2
			// 
			this.comboOperator2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboOperator2.FormattingEnabled = true;
			this.comboOperator2.Location = new System.Drawing.Point(147, 134);
			this.comboOperator2.Name = "comboOperator2";
			this.comboOperator2.Size = new System.Drawing.Size(52, 21);
			this.comboOperator2.TabIndex = 173;
			this.comboOperator2.Visible = false;
			// 
			// textConditionValue2
			// 
			this.textConditionValue2.Location = new System.Drawing.Point(232, 237);
			this.textConditionValue2.Name = "textConditionValue2";
			this.textConditionValue2.Size = new System.Drawing.Size(161, 20);
			this.textConditionValue2.TabIndex = 176;
			this.textConditionValue2.Visible = false;
			// 
			// comboOperator1
			// 
			this.comboOperator1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboOperator1.FormattingEnabled = true;
			this.comboOperator1.Location = new System.Drawing.Point(148, 71);
			this.comboOperator1.Name = "comboOperator1";
			this.comboOperator1.Size = new System.Drawing.Size(52, 21);
			this.comboOperator1.TabIndex = 178;
			this.comboOperator1.Visible = false;
			// 
			// listRelationships
			// 
			this.listRelationships.Location = new System.Drawing.Point(147, 98);
			this.listRelationships.Name = "listRelationships";
			this.listRelationships.Size = new System.Drawing.Size(68, 30);
			this.listRelationships.TabIndex = 179;
			// 
			// butPickProv
			// 
			this.butPickProv.Location = new System.Drawing.Point(370, 46);
			this.butPickProv.Name = "butPickProv";
			this.butPickProv.Size = new System.Drawing.Size(23, 21);
			this.butPickProv.TabIndex = 170;
			this.butPickProv.Text = "...";
			this.butPickProv.Visible = false;
			this.butPickProv.Click += new System.EventHandler(this.butPickProv_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 288);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(83, 24);
			this.butDelete.TabIndex = 31;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(232, 288);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(318, 288);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormReqFieldCondEdit
			// 
			this.ClientSize = new System.Drawing.Size(405, 324);
			this.Controls.Add(this.listRelationships);
			this.Controls.Add(this.comboOperator1);
			this.Controls.Add(this.textConditionValue2);
			this.Controls.Add(this.comboOperator2);
			this.Controls.Add(this.listConditionValues);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.butPickProv);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.labelConditionValue2);
			this.Controls.Add(this.listConditionType);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textConditionValue1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormReqFieldCondEdit";
			this.Text = "Edit Required Field Condition";
			this.Load += new System.EventHandler(this.FormReqFieldCondEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label labelConditionValue2;
		private OpenDental.UI.ListBoxOD listConditionType;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textConditionValue1;
		private System.Windows.Forms.Label label2;
		private UI.Button butDelete;
		private UI.Button butPickProv;
		private OpenDental.UI.ListBoxOD listConditionValues;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox comboOperator2;
		private System.Windows.Forms.TextBox textConditionValue2;
		private System.Windows.Forms.ComboBox comboOperator1;
		private OpenDental.UI.ListBoxOD listRelationships;
	}
}