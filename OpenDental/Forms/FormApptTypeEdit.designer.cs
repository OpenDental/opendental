namespace OpenDental{
	partial class FormApptTypeEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormApptTypeEdit));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.label9 = new System.Windows.Forms.Label();
			this.butColorClear = new OpenDental.UI.Button();
			this.butColor = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.textName = new System.Windows.Forms.TextBox();
			this.checkIsHidden = new System.Windows.Forms.CheckBox();
			this.butDelete = new OpenDental.UI.Button();
			this.butSlider = new System.Windows.Forms.Button();
			this.tbTime = new OpenDental.TableTimeBar();
			this.labelTime = new System.Windows.Forms.Label();
			this.listBoxProcCodes = new OpenDental.UI.ListBoxOD();
			this.butAdd = new OpenDental.UI.Button();
			this.butRemove = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.butClear = new OpenDental.UI.Button();
			this.textTime = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(252, 551);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 5;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(333, 551);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 4;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(29, 66);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(95, 20);
			this.label9.TabIndex = 182;
			this.label9.Text = "Color";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butColorClear
			// 
			this.butColorClear.Location = new System.Drawing.Point(155, 63);
			this.butColorClear.Name = "butColorClear";
			this.butColorClear.Size = new System.Drawing.Size(39, 24);
			this.butColorClear.TabIndex = 181;
			this.butColorClear.Text = "none";
			this.butColorClear.Click += new System.EventHandler(this.butColorClear_Click);
			// 
			// butColor
			// 
			this.butColor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butColor.Location = new System.Drawing.Point(129, 66);
			this.butColor.Name = "butColor";
			this.butColor.Size = new System.Drawing.Size(20, 20);
			this.butColor.TabIndex = 180;
			this.butColor.Click += new System.EventHandler(this.butColor_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(29, 36);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(95, 20);
			this.label2.TabIndex = 184;
			this.label2.Text = "Name";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textName
			// 
			this.textName.Location = new System.Drawing.Point(129, 36);
			this.textName.Name = "textName";
			this.textName.Size = new System.Drawing.Size(272, 20);
			this.textName.TabIndex = 183;
			// 
			// checkIsHidden
			// 
			this.checkIsHidden.Checked = true;
			this.checkIsHidden.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkIsHidden.Location = new System.Drawing.Point(30, 275);
			this.checkIsHidden.Name = "checkIsHidden";
			this.checkIsHidden.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkIsHidden.Size = new System.Drawing.Size(113, 24);
			this.checkIsHidden.TabIndex = 185;
			this.checkIsHidden.Text = "Hidden";
			this.checkIsHidden.UseVisualStyleBackColor = true;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(28, 551);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 24);
			this.butDelete.TabIndex = 186;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butSlider
			// 
			this.butSlider.BackColor = System.Drawing.SystemColors.ControlDark;
			this.butSlider.Location = new System.Drawing.Point(9, 96);
			this.butSlider.Name = "butSlider";
			this.butSlider.Size = new System.Drawing.Size(12, 15);
			this.butSlider.TabIndex = 188;
			this.butSlider.UseVisualStyleBackColor = false;
			this.butSlider.MouseDown += new System.Windows.Forms.MouseEventHandler(this.butSlider_MouseDown);
			this.butSlider.MouseMove += new System.Windows.Forms.MouseEventHandler(this.butSlider_MouseMove);
			this.butSlider.MouseUp += new System.Windows.Forms.MouseEventHandler(this.butSlider_MouseUp);
			// 
			// tbTime
			// 
			this.tbTime.BackColor = System.Drawing.SystemColors.Window;
			this.tbTime.Location = new System.Drawing.Point(7, 12);
			this.tbTime.Name = "tbTime";
			this.tbTime.ScrollValue = 150;
			this.tbTime.SelectedIndices = new int[0];
			this.tbTime.SelectionMode = System.Windows.Forms.SelectionMode.None;
			this.tbTime.Size = new System.Drawing.Size(15, 561);
			this.tbTime.TabIndex = 187;
			this.tbTime.CellClicked += new OpenDental.ContrTable.CellEventHandler(this.tbTime_CellClicked);
			// 
			// labelTime
			// 
			this.labelTime.Location = new System.Drawing.Point(29, 95);
			this.labelTime.Name = "labelTime";
			this.labelTime.Size = new System.Drawing.Size(95, 20);
			this.labelTime.TabIndex = 190;
			this.labelTime.Text = "Time";
			this.labelTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// listBoxProcCodes
			// 
			this.listBoxProcCodes.Location = new System.Drawing.Point(129, 126);
			this.listBoxProcCodes.Name = "listBoxProcCodes";
			this.listBoxProcCodes.SelectionMode = UI.SelectionMode.MultiExtended;
			this.listBoxProcCodes.Size = new System.Drawing.Size(120, 147);
			this.listBoxProcCodes.TabIndex = 192;
			// 
			// butAdd
			// 
			this.butAdd.Location = new System.Drawing.Point(252, 126);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(65, 24);
			this.butAdd.TabIndex = 194;
			this.butAdd.Text = "Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butRemove
			// 
			this.butRemove.Location = new System.Drawing.Point(252, 154);
			this.butRemove.Name = "butRemove";
			this.butRemove.Size = new System.Drawing.Size(65, 24);
			this.butRemove.TabIndex = 193;
			this.butRemove.Text = "Remove";
			this.butRemove.Click += new System.EventHandler(this.butRemove_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(32, 126);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(95, 20);
			this.label1.TabIndex = 195;
			this.label1.Text = "Procedures";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butClear
			// 
			this.butClear.Location = new System.Drawing.Point(282, 93);
			this.butClear.Name = "butClear";
			this.butClear.Size = new System.Drawing.Size(65, 24);
			this.butClear.TabIndex = 198;
			this.butClear.Text = "Clear";
			this.butClear.Click += new System.EventHandler(this.butClear_Click);
			// 
			// textTime
			// 
			this.textTime.Location = new System.Drawing.Point(129, 95);
			this.textTime.Name = "textTime";
			this.textTime.ReadOnly = true;
			this.textTime.Size = new System.Drawing.Size(147, 20);
			this.textTime.TabIndex = 197;
			// 
			// FormApptTypeEdit
			// 
			this.ClientSize = new System.Drawing.Size(420, 587);
			this.Controls.Add(this.butClear);
			this.Controls.Add(this.textTime);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.butRemove);
			this.Controls.Add(this.listBoxProcCodes);
			this.Controls.Add(this.labelTime);
			this.Controls.Add(this.butSlider);
			this.Controls.Add(this.tbTime);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.checkIsHidden);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textName);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.butColorClear);
			this.Controls.Add(this.butColor);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormApptTypeEdit";
			this.Text = "Appointment Type Edit";
			this.Load += new System.EventHandler(this.FormApptTypeEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private UI.Button butOK;
		private UI.Button butCancel;
		private System.Windows.Forms.Label label9;
		private UI.Button butColorClear;
		private System.Windows.Forms.Button butColor;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textName;
		private System.Windows.Forms.CheckBox checkIsHidden;
		private UI.Button butDelete;
		private System.Windows.Forms.Button butSlider;
		private TableTimeBar tbTime;
		private System.Windows.Forms.Label labelTime;
		private OpenDental.UI.ListBoxOD listBoxProcCodes;
		private UI.Button butAdd;
		private UI.Button butRemove;
		private System.Windows.Forms.Label label1;
		private UI.Button butClear;
		private System.Windows.Forms.TextBox textTime;
	}
}