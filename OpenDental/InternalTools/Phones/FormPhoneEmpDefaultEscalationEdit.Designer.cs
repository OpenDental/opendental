namespace OpenDental {
	partial class FormPhoneEmpDefaultEscalationEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPhoneEmpDefaultEscalationEdit));
			this.gridEscalation = new OpenDental.UI.GridOD();
			this.gridEmployees = new OpenDental.UI.GridOD();
			this.butUp = new OpenDental.UI.Button();
			this.butDown = new OpenDental.UI.Button();
			this.butRight = new OpenDental.UI.Button();
			this.butLeft = new OpenDental.UI.Button();
			this.butSave = new OpenDental.UI.Button();
			this.labelAvail = new System.Windows.Forms.Label();
			this.comboEscalationViewEdit = new OpenDental.UI.ComboBox();
			this.labelEscalationGroup = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// gridEscalation
			// 
			this.gridEscalation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridEscalation.Location = new System.Drawing.Point(247, 37);
			this.gridEscalation.Name = "gridEscalation";
			this.gridEscalation.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridEscalation.Size = new System.Drawing.Size(175, 464);
			this.gridEscalation.TabIndex = 65;
			this.gridEscalation.Title = "Escalation Order";
			this.gridEscalation.TranslationName = "TableEscalation";
			// 
			// gridEmployees
			// 
			this.gridEmployees.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridEmployees.Location = new System.Drawing.Point(12, 37);
			this.gridEmployees.Name = "gridEmployees";
			this.gridEmployees.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridEmployees.Size = new System.Drawing.Size(175, 464);
			this.gridEmployees.TabIndex = 62;
			this.gridEmployees.Title = "Employees";
			this.gridEmployees.TranslationName = "TableEmployee";
			// 
			// butUp
			// 
			this.butUp.Image = global::OpenDental.Properties.Resources.up;
			this.butUp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butUp.Location = new System.Drawing.Point(444, 195);
			this.butUp.Name = "butUp";
			this.butUp.Size = new System.Drawing.Size(71, 24);
			this.butUp.TabIndex = 67;
			this.butUp.Text = "&Up";
			this.butUp.Click += new System.EventHandler(this.butUp_Click);
			// 
			// butDown
			// 
			this.butDown.Image = global::OpenDental.Properties.Resources.down;
			this.butDown.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDown.Location = new System.Drawing.Point(444, 229);
			this.butDown.Name = "butDown";
			this.butDown.Size = new System.Drawing.Size(71, 24);
			this.butDown.TabIndex = 66;
			this.butDown.Text = "&Down";
			this.butDown.Click += new System.EventHandler(this.butDown_Click);
			// 
			// butRight
			// 
			this.butRight.Image = global::OpenDental.Properties.Resources.Right;
			this.butRight.Location = new System.Drawing.Point(201, 194);
			this.butRight.Name = "butRight";
			this.butRight.Size = new System.Drawing.Size(35, 26);
			this.butRight.TabIndex = 64;
			this.butRight.Click += new System.EventHandler(this.butRight_Click);
			// 
			// butLeft
			// 
			this.butLeft.AdjustImageLocation = new System.Drawing.Point(-1, 0);
			this.butLeft.Image = global::OpenDental.Properties.Resources.Left;
			this.butLeft.Location = new System.Drawing.Point(201, 228);
			this.butLeft.Name = "butLeft";
			this.butLeft.Size = new System.Drawing.Size(35, 26);
			this.butLeft.TabIndex = 63;
			this.butLeft.Click += new System.EventHandler(this.butLeft_Click);
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(444, 477);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 13;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// labelAvail
			// 
			this.labelAvail.Location = new System.Drawing.Point(428, 265);
			this.labelAvail.Name = "labelAvail";
			this.labelAvail.Size = new System.Drawing.Size(96, 139);
			this.labelAvail.TabIndex = 91;
			this.labelAvail.Text = "The Avail view is ordered by those in Available status sorted from most amount of" +
    " time in this status to least then by those in Train status then by those in Bac" +
    "kup status.";
			// 
			// comboEscalationViewEdit
			// 
			this.comboEscalationViewEdit.Location = new System.Drawing.Point(135, 9);
			this.comboEscalationViewEdit.Name = "comboEscalationViewEdit";
			this.comboEscalationViewEdit.Size = new System.Drawing.Size(175, 21);
			this.comboEscalationViewEdit.TabIndex = 92;
			this.comboEscalationViewEdit.Text = "comboBox1";
			this.comboEscalationViewEdit.SelectionChangeCommitted += new System.EventHandler(this.comboEscalationViewEdit_SelectionChangeCommitted);
			// 
			// labelEscalationGroup
			// 
			this.labelEscalationGroup.Location = new System.Drawing.Point(15, 11);
			this.labelEscalationGroup.Name = "labelEscalationGroup";
			this.labelEscalationGroup.Size = new System.Drawing.Size(120, 18);
			this.labelEscalationGroup.TabIndex = 93;
			this.labelEscalationGroup.Text = "Escalation Group";
			this.labelEscalationGroup.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormPhoneEmpDefaultEscalationEdit
			// 
			this.ClientSize = new System.Drawing.Size(529, 514);
			this.Controls.Add(this.labelEscalationGroup);
			this.Controls.Add(this.comboEscalationViewEdit);
			this.Controls.Add(this.butSave);
			this.Controls.Add(this.labelAvail);
			this.Controls.Add(this.butUp);
			this.Controls.Add(this.butDown);
			this.Controls.Add(this.gridEscalation);
			this.Controls.Add(this.butRight);
			this.Controls.Add(this.butLeft);
			this.Controls.Add(this.gridEmployees);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormPhoneEmpDefaultEscalationEdit";
			this.Text = "Escalation Edit";
			this.ResumeLayout(false);

		}

		#endregion

		private UI.Button butSave;
		private UI.GridOD gridEmployees;
		private UI.Button butRight;
		private UI.Button butLeft;
		private UI.GridOD gridEscalation;
		private UI.Button butUp;
		private UI.Button butDown;
		private System.Windows.Forms.Label labelAvail;
		private UI.ComboBox comboEscalationViewEdit;
		private System.Windows.Forms.Label labelEscalationGroup;
	}
}