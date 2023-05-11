namespace OpenDental{
	partial class FormERoutingDefEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormERoutingDefEdit));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.textBoxDescription = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.butDown = new OpenDental.UI.Button();
			this.butUp = new OpenDental.UI.Button();
			this.butRemove = new OpenDental.UI.Button();
			this.gridPatientERoutingActions = new OpenDental.UI.GridOD();
			this.groupBoxOD1 = new OpenDental.UI.GroupBox();
			this.comboActionType = new OpenDental.UI.ComboBox();
			this.butAdd = new OpenDental.UI.Button();
			this.groupBoxActions = new OpenDental.UI.GroupBox();
			this.groupBoxOD2 = new OpenDental.UI.GroupBox();
			this.butRemoveLinkType = new OpenDental.UI.Button();
			this.gridLinkTypes = new OpenDental.UI.GridOD();
			this.groupBoxOD3 = new OpenDental.UI.GroupBox();
			this.labelGenAppts = new System.Windows.Forms.Label();
			this.butAddLinkType = new OpenDental.UI.Button();
			this.butAddSpecificTypes = new OpenDental.UI.Button();
			this.comboLinkType = new OpenDental.UI.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBoxOD1.SuspendLayout();
			this.groupBoxActions.SuspendLayout();
			this.groupBoxOD2.SuspendLayout();
			this.groupBoxOD3.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(765, 425);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 8;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(846, 425);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 9;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 429);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 24);
			this.butDelete.TabIndex = 7;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// textBoxDescription
			// 
			this.textBoxDescription.Location = new System.Drawing.Point(12, 35);
			this.textBoxDescription.Name = "textBoxDescription";
			this.textBoxDescription.Size = new System.Drawing.Size(200, 20);
			this.textBoxDescription.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(10, 11);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(156, 18);
			this.label1.TabIndex = 11;
			this.label1.Text = "Description";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butDown
			// 
			this.butDown.Image = global::OpenDental.Properties.Resources.down;
			this.butDown.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDown.Location = new System.Drawing.Point(226, 140);
			this.butDown.Name = "butDown";
			this.butDown.Size = new System.Drawing.Size(75, 24);
			this.butDown.TabIndex = 6;
			this.butDown.Text = "&Down";
			this.butDown.Click += new System.EventHandler(this.butDown_Click);
			// 
			// butUp
			// 
			this.butUp.Image = global::OpenDental.Properties.Resources.up;
			this.butUp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butUp.Location = new System.Drawing.Point(226, 110);
			this.butUp.Name = "butUp";
			this.butUp.Size = new System.Drawing.Size(75, 24);
			this.butUp.TabIndex = 5;
			this.butUp.Text = "&Up";
			this.butUp.Click += new System.EventHandler(this.butUp_Click);
			// 
			// butRemove
			// 
			this.butRemove.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butRemove.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butRemove.Location = new System.Drawing.Point(226, 243);
			this.butRemove.Name = "butRemove";
			this.butRemove.Size = new System.Drawing.Size(75, 24);
			this.butRemove.TabIndex = 3;
			this.butRemove.Text = "&Remove";
			this.butRemove.Click += new System.EventHandler(this.butRemove_Click);
			// 
			// gridPatientFlowActions
			// 
			this.gridPatientERoutingActions.Location = new System.Drawing.Point(3, 21);
			this.gridPatientERoutingActions.Name = "gridPatientFlowActions";
			this.gridPatientERoutingActions.Size = new System.Drawing.Size(214, 246);
			this.gridPatientERoutingActions.TabIndex = 5;
			this.gridPatientERoutingActions.Title = "Actions";
			this.gridPatientERoutingActions.TitleVisible = false;
			// 
			// groupBoxOD1
			// 
			this.groupBoxOD1.Controls.Add(this.comboActionType);
			this.groupBoxOD1.Controls.Add(this.butAdd);
			this.groupBoxOD1.Location = new System.Drawing.Point(223, 21);
			this.groupBoxOD1.Name = "groupBoxOD1";
			this.groupBoxOD1.Size = new System.Drawing.Size(174, 83);
			this.groupBoxOD1.TabIndex = 14;
			this.groupBoxOD1.Text = "Add Action";
			// 
			// comboActionType
			// 
			this.comboActionType.Location = new System.Drawing.Point(3, 19);
			this.comboActionType.Name = "comboActionType";
			this.comboActionType.Size = new System.Drawing.Size(155, 21);
			this.comboActionType.TabIndex = 4;
			this.comboActionType.Text = "comboBoxActionType";
			// 
			// butAdd
			// 
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(3, 46);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(75, 24);
			this.butAdd.TabIndex = 2;
			this.butAdd.Text = "&Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// groupBoxActions
			// 
			this.groupBoxActions.Controls.Add(this.gridPatientERoutingActions);
			this.groupBoxActions.Controls.Add(this.groupBoxOD1);
			this.groupBoxActions.Controls.Add(this.butDown);
			this.groupBoxActions.Controls.Add(this.butUp);
			this.groupBoxActions.Controls.Add(this.butRemove);
			this.groupBoxActions.Location = new System.Drawing.Point(12, 111);
			this.groupBoxActions.Name = "groupBoxActions";
			this.groupBoxActions.Size = new System.Drawing.Size(412, 286);
			this.groupBoxActions.TabIndex = 15;
			this.groupBoxActions.Text = "Actions";
			// 
			// groupBoxOD2
			// 
			this.groupBoxOD2.Controls.Add(this.butRemoveLinkType);
			this.groupBoxOD2.Controls.Add(this.gridLinkTypes);
			this.groupBoxOD2.Controls.Add(this.groupBoxOD3);
			this.groupBoxOD2.Location = new System.Drawing.Point(433, 111);
			this.groupBoxOD2.Name = "groupBoxOD2";
			this.groupBoxOD2.Size = new System.Drawing.Size(473, 286);
			this.groupBoxOD2.TabIndex = 16;
			this.groupBoxOD2.Text = "ERouting Triggers";
			// 
			// butRemoveLinkType
			// 
			this.butRemoveLinkType.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butRemoveLinkType.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butRemoveLinkType.Location = new System.Drawing.Point(260, 243);
			this.butRemoveLinkType.Name = "butRemoveLinkType";
			this.butRemoveLinkType.Size = new System.Drawing.Size(75, 24);
			this.butRemoveLinkType.TabIndex = 15;
			this.butRemoveLinkType.Text = "&Remove";
			this.butRemoveLinkType.Click += new System.EventHandler(this.butRemoveLinkType_Click);
			// 
			// gridLinkTypes
			// 
			this.gridLinkTypes.Location = new System.Drawing.Point(3, 21);
			this.gridLinkTypes.Name = "gridLinkTypes";
			this.gridLinkTypes.Size = new System.Drawing.Size(248, 246);
			this.gridLinkTypes.TabIndex = 17;
			this.gridLinkTypes.TitleVisible = false;
			// 
			// groupBoxOD3
			// 
			this.groupBoxOD3.Controls.Add(this.labelGenAppts);
			this.groupBoxOD3.Controls.Add(this.butAddLinkType);
			this.groupBoxOD3.Controls.Add(this.butAddSpecificTypes);
			this.groupBoxOD3.Controls.Add(this.comboLinkType);
			this.groupBoxOD3.Location = new System.Drawing.Point(257, 21);
			this.groupBoxOD3.Name = "groupBoxOD3";
			this.groupBoxOD3.Size = new System.Drawing.Size(199, 143);
			this.groupBoxOD3.TabIndex = 17;
			this.groupBoxOD3.Text = "Add Trigger Type";
			// 
			// labelGenAppts
			// 
			this.labelGenAppts.Location = new System.Drawing.Point(81, 40);
			this.labelGenAppts.Name = "labelGenAppts";
			this.labelGenAppts.Size = new System.Drawing.Size(115, 36);
			this.labelGenAppts.TabIndex = 19;
			this.labelGenAppts.Text = "Add with no Appt Type";
			this.labelGenAppts.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butAddLinkType
			// 
			this.butAddLinkType.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddLinkType.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddLinkType.Location = new System.Drawing.Point(3, 46);
			this.butAddLinkType.Name = "butAddLinkType";
			this.butAddLinkType.Size = new System.Drawing.Size(75, 24);
			this.butAddLinkType.TabIndex = 7;
			this.butAddLinkType.Text = "&Add";
			this.butAddLinkType.Click += new System.EventHandler(this.butAddLinkType_Click);
			// 
			// butAddSpecificTypes
			// 
			this.butAddSpecificTypes.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddSpecificTypes.Location = new System.Drawing.Point(3, 76);
			this.butAddSpecificTypes.Name = "butAddSpecificTypes";
			this.butAddSpecificTypes.Size = new System.Drawing.Size(94, 24);
			this.butAddSpecificTypes.TabIndex = 6;
			this.butAddSpecificTypes.Text = "Add Appt Types";
			this.butAddSpecificTypes.Click += new System.EventHandler(this.butAddSpecificTypes_Click);
			// 
			// comboLinkType
			// 
			this.comboLinkType.Location = new System.Drawing.Point(3, 17);
			this.comboLinkType.Name = "comboLinkType";
			this.comboLinkType.Size = new System.Drawing.Size(174, 23);
			this.comboLinkType.TabIndex = 18;
			this.comboLinkType.Text = "comboBoxOD1";
			this.comboLinkType.SelectionChangeCommitted += new System.EventHandler(this.comboLinkTypes_SelectionChangeCommitted);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(12, 81);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(430, 24);
			this.label2.TabIndex = 17;
			this.label2.Text = "Determines what actions should take place in this ERoutingand their order.";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(435, 66);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(413, 40);
			this.label3.TabIndex = 18;
			this.label3.Text = "Specifies the situations for which this ERouting Def will be available. If none e" +
    "ntered, the eRouting is treated as \"General\".";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// FormFlowDefEdit
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(933, 465);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.groupBoxOD2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBoxDescription);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.groupBoxActions);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormFlowDefEdit";
			this.Text = "ERouting Def Edit";
			this.Load += new System.EventHandler(this.FormPatientFlowEdit_Load);
			this.groupBoxOD1.ResumeLayout(false);
			this.groupBoxActions.ResumeLayout(false);
			this.groupBoxOD2.ResumeLayout(false);
			this.groupBoxOD3.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private UI.Button butDelete;
		private System.Windows.Forms.TextBox textBoxDescription;
		private System.Windows.Forms.Label label1;
		private UI.Button butDown;
		private UI.Button butUp;
		private UI.Button butRemove;
		private UI.GridOD gridPatientERoutingActions;
		private UI.GroupBox groupBoxOD1;
		private UI.ComboBox comboActionType;
		private UI.Button butAdd;
		private UI.GroupBox groupBoxActions;
		private UI.GroupBox groupBoxOD2;
		private UI.GridOD gridLinkTypes;
		private UI.GroupBox groupBoxOD3;
		private UI.Button butAddLinkType;
		private UI.ComboBox comboLinkType;
		private UI.Button butRemoveLinkType;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private UI.Button butAddSpecificTypes;
		private System.Windows.Forms.Label labelGenAppts;
	}
}