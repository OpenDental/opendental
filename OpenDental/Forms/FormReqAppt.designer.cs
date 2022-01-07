using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormReqAppt {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormReqAppt));
			this.label2 = new System.Windows.Forms.Label();
			this.comboCourse = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.comboClass = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.comboInstructor = new OpenDental.UI.ComboBoxOD();
			this.butOK = new OpenDental.UI.Button();
			this.gridReqs = new OpenDental.UI.GridOD();
			this.butAdd = new OpenDental.UI.Button();
			this.butRemove = new OpenDental.UI.Button();
			this.gridAttached = new OpenDental.UI.GridOD();
			this.gridStudents = new OpenDental.UI.GridOD();
			this.butCancel = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(566, 39);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(77, 18);
			this.label2.TabIndex = 22;
			this.label2.Text = "Course";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboCourse
			// 
			this.comboCourse.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboCourse.FormattingEnabled = true;
			this.comboCourse.Location = new System.Drawing.Point(647, 39);
			this.comboCourse.Name = "comboCourse";
			this.comboCourse.Size = new System.Drawing.Size(234, 21);
			this.comboCourse.TabIndex = 21;
			this.comboCourse.SelectionChangeCommitted += new System.EventHandler(this.comboCourse_SelectionChangeCommitted);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(563, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(81, 18);
			this.label1.TabIndex = 20;
			this.label1.Text = "Class";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboClass
			// 
			this.comboClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboClass.FormattingEnabled = true;
			this.comboClass.Location = new System.Drawing.Point(647, 12);
			this.comboClass.Name = "comboClass";
			this.comboClass.Size = new System.Drawing.Size(234, 21);
			this.comboClass.TabIndex = 19;
			this.comboClass.SelectionChangeCommitted += new System.EventHandler(this.comboClass_SelectionChangeCommitted);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(497, 66);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(146, 18);
			this.label3.TabIndex = 29;
			this.label3.Text = "Instructor";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboInstructor
			// 
			this.comboInstructor.Location = new System.Drawing.Point(647, 66);
			this.comboInstructor.Name = "comboInstructor";
			this.comboInstructor.Size = new System.Drawing.Size(234, 21);
			this.comboInstructor.TabIndex = 28;
			this.comboInstructor.SelectionChangeCommitted += new System.EventHandler(this.comboInstructor_SelectionChangeCommitted);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(806, 591);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 27;
			this.butOK.Text = "OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// gridReqs
			// 
			this.gridReqs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridReqs.Location = new System.Drawing.Point(223, 12);
			this.gridReqs.Name = "gridReqs";
			this.gridReqs.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridReqs.Size = new System.Drawing.Size(268, 637);
			this.gridReqs.TabIndex = 26;
			this.gridReqs.Title = "Requirements";
			this.gridReqs.TranslationName = "TableReqStudentMany";
			// 
			// butAdd
			// 
			this.butAdd.Image = global::OpenDental.Properties.Resources.down;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(497, 273);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(90, 26);
			this.butAdd.TabIndex = 25;
			this.butAdd.Text = "Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butRemove
			// 
			this.butRemove.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butRemove.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butRemove.Location = new System.Drawing.Point(596, 273);
			this.butRemove.Name = "butRemove";
			this.butRemove.Size = new System.Drawing.Size(90, 26);
			this.butRemove.TabIndex = 24;
			this.butRemove.Text = "Remove";
			this.butRemove.Click += new System.EventHandler(this.butRemove_Click);
			// 
			// gridAttached
			// 
			this.gridAttached.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridAttached.Location = new System.Drawing.Point(497, 305);
			this.gridAttached.Name = "gridAttached";
			this.gridAttached.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridAttached.Size = new System.Drawing.Size(384, 225);
			this.gridAttached.TabIndex = 23;
			this.gridAttached.Title = "Currently Attached Requirements";
			this.gridAttached.TranslationName = "TableReqStudentMany";
			this.gridAttached.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridAttached_CellDoubleClick);
			// 
			// gridStudents
			// 
			this.gridStudents.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridStudents.Location = new System.Drawing.Point(10, 12);
			this.gridStudents.Name = "gridStudents";
			this.gridStudents.Size = new System.Drawing.Size(207, 637);
			this.gridStudents.TabIndex = 3;
			this.gridStudents.Title = "Students";
			this.gridStudents.TranslationName = "TableReqStudentMany";
			this.gridStudents.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridStudents_CellClick);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(806, 623);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormReqAppt
			// 
			this.ClientSize = new System.Drawing.Size(893, 661);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.comboInstructor);
			this.Controls.Add(this.gridReqs);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.butRemove);
			this.Controls.Add(this.gridAttached);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.comboCourse);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.comboClass);
			this.Controls.Add(this.gridStudents);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormReqAppt";
			this.ShowInTaskbar = false;
			this.Text = "Student Requirements for Appointment";
			this.Load += new System.EventHandler(this.FormReqAppt_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.GridOD gridStudents;
		private Label label2;
		private ComboBox comboCourse;
		private Label label1;
		private ComboBox comboClass;
		private UI.GridOD gridAttached;
		private OpenDental.UI.Button butRemove;
		private OpenDental.UI.Button butAdd;
		private UI.GridOD gridReqs;
		private OpenDental.UI.Button butOK;
		private Label label3;
		private ComboBoxOD comboInstructor;

	}
}
