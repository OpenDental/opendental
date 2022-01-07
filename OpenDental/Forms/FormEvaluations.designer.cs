namespace OpenDental{
	partial class FormEvaluations {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEvaluations));
			this.butCancel = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.comboInstructor = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.groupStudents = new System.Windows.Forms.GroupBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textProvNum = new System.Windows.Forms.TextBox();
			this.butRefresh = new OpenDental.UI.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.textFirstName = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.textLastName = new System.Windows.Forms.TextBox();
			this.butAdd = new OpenDental.UI.Button();
			this.textDateEnd = new ValidDate();
			this.textDateStart = new ValidDate();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.groupAdmin = new System.Windows.Forms.GroupBox();
			this.comboCourse = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.butReport = new OpenDental.UI.Button();
			this.groupStudents.SuspendLayout();
			this.groupAdmin.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(883, 564);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 8;
			this.butCancel.Text = "Close";
			this.butCancel.Click += new System.EventHandler(this.butClose_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.HScrollVisible = true;
			this.gridMain.Location = new System.Drawing.Point(12, 36);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(673, 522);
			this.gridMain.TabIndex = 15;
			this.gridMain.Title = "Evaluations";
			this.gridMain.TranslationName = "TableEvaluationSetup";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// comboInstructor
			// 
			this.comboInstructor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboInstructor.FormattingEnabled = true;
			this.comboInstructor.ItemHeight = 13;
			this.comboInstructor.Location = new System.Drawing.Point(101, 19);
			this.comboInstructor.Name = "comboInstructor";
			this.comboInstructor.Size = new System.Drawing.Size(166, 21);
			this.comboInstructor.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(6, 19);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(93, 18);
			this.label2.TabIndex = 24;
			this.label2.Text = "Instructor";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupStudents
			// 
			this.groupStudents.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupStudents.Controls.Add(this.label3);
			this.groupStudents.Controls.Add(this.textProvNum);
			this.groupStudents.Controls.Add(this.butRefresh);
			this.groupStudents.Controls.Add(this.label5);
			this.groupStudents.Controls.Add(this.textFirstName);
			this.groupStudents.Controls.Add(this.label9);
			this.groupStudents.Controls.Add(this.textLastName);
			this.groupStudents.Location = new System.Drawing.Point(691, 85);
			this.groupStudents.Name = "groupStudents";
			this.groupStudents.Size = new System.Drawing.Size(273, 120);
			this.groupStudents.TabIndex = 4;
			this.groupStudents.TabStop = false;
			this.groupStudents.Text = "Student Filters:";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(11, 65);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(90, 18);
			this.label3.TabIndex = 25;
			this.label3.Text = "ProvNum";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textProvNum
			// 
			this.textProvNum.Location = new System.Drawing.Point(101, 64);
			this.textProvNum.MaxLength = 15;
			this.textProvNum.Name = "textProvNum";
			this.textProvNum.Size = new System.Drawing.Size(166, 20);
			this.textProvNum.TabIndex = 3;
			// 
			// butRefresh
			// 
			this.butRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butRefresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butRefresh.Location = new System.Drawing.Point(192, 90);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(75, 24);
			this.butRefresh.TabIndex = 4;
			this.butRefresh.Text = "Refresh";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(11, 43);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(90, 18);
			this.label5.TabIndex = 23;
			this.label5.Text = "First Name";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textFirstName
			// 
			this.textFirstName.Location = new System.Drawing.Point(101, 42);
			this.textFirstName.MaxLength = 15;
			this.textFirstName.Name = "textFirstName";
			this.textFirstName.Size = new System.Drawing.Size(166, 20);
			this.textFirstName.TabIndex = 2;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(11, 21);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(90, 18);
			this.label9.TabIndex = 21;
			this.label9.Text = "Last Name";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textLastName
			// 
			this.textLastName.Location = new System.Drawing.Point(101, 20);
			this.textLastName.MaxLength = 15;
			this.textLastName.Name = "textLastName";
			this.textLastName.Size = new System.Drawing.Size(166, 20);
			this.textLastName.TabIndex = 1;
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(883, 269);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(75, 24);
			this.butAdd.TabIndex = 6;
			this.butAdd.Text = "Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// textDateEnd
			// 
			this.textDateEnd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textDateEnd.Location = new System.Drawing.Point(842, 36);
			this.textDateEnd.Name = "textDateEnd";
			this.textDateEnd.Size = new System.Drawing.Size(70, 20);
			this.textDateEnd.TabIndex = 2;
			// 
			// textDateStart
			// 
			this.textDateStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textDateStart.Location = new System.Drawing.Point(741, 36);
			this.textDateStart.Name = "textDateStart";
			this.textDateStart.Size = new System.Drawing.Size(70, 20);
			this.textDateStart.TabIndex = 1;
			// 
			// label6
			// 
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label6.Location = new System.Drawing.Point(815, 36);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(23, 18);
			this.label6.TabIndex = 38;
			this.label6.Text = "to";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label7
			// 
			this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label7.Location = new System.Drawing.Point(697, 36);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(40, 18);
			this.label7.TabIndex = 39;
			this.label7.Text = "Date:";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupAdmin
			// 
			this.groupAdmin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupAdmin.Controls.Add(this.comboInstructor);
			this.groupAdmin.Controls.Add(this.label2);
			this.groupAdmin.Location = new System.Drawing.Point(691, 206);
			this.groupAdmin.Name = "groupAdmin";
			this.groupAdmin.Size = new System.Drawing.Size(273, 57);
			this.groupAdmin.TabIndex = 5;
			this.groupAdmin.TabStop = false;
			this.groupAdmin.Text = "Admin Filters:";
			// 
			// comboCourse
			// 
			this.comboCourse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboCourse.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboCourse.FormattingEnabled = true;
			this.comboCourse.ItemHeight = 13;
			this.comboCourse.Location = new System.Drawing.Point(741, 62);
			this.comboCourse.Name = "comboCourse";
			this.comboCourse.Size = new System.Drawing.Size(171, 21);
			this.comboCourse.TabIndex = 3;
			this.comboCourse.SelectionChangeCommitted += new System.EventHandler(this.comboCourse_SelectionChangeCommitted);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(691, 62);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(48, 18);
			this.label1.TabIndex = 42;
			this.label1.Text = "Course";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butReport
			// 
			this.butReport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butReport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butReport.Location = new System.Drawing.Point(883, 395);
			this.butReport.Name = "butReport";
			this.butReport.Size = new System.Drawing.Size(75, 24);
			this.butReport.TabIndex = 7;
			this.butReport.Text = "Reports";
			this.butReport.Click += new System.EventHandler(this.butReport_Click);
			// 
			// FormEvaluations
			// 
			this.ClientSize = new System.Drawing.Size(974, 600);
			this.Controls.Add(this.butReport);
			this.Controls.Add(this.comboCourse);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.groupAdmin);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.textDateStart);
			this.Controls.Add(this.textDateEnd);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.groupStudents);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEvaluations";
			this.Text = "Evaluations";
			this.Load += new System.EventHandler(this.FormEvaluations_Load);
			this.groupStudents.ResumeLayout(false);
			this.groupStudents.PerformLayout();
			this.groupAdmin.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butCancel;
		private UI.GridOD gridMain;
		private System.Windows.Forms.ComboBox comboInstructor;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox groupStudents;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textProvNum;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textFirstName;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox textLastName;
		private UI.Button butAdd;
		private ValidDate textDateEnd;
		private ValidDate textDateStart;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.GroupBox groupAdmin;
		private UI.Button butRefresh;
		private System.Windows.Forms.ComboBox comboCourse;
		private System.Windows.Forms.Label label1;
		private UI.Button butReport;
	}
}