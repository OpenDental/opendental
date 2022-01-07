using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormRpCourseGrades {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpCourseGrades));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.comboSchoolCourse = new System.Windows.Forms.ComboBox();
			this.comboSchoolClass = new System.Windows.Forms.ComboBox();
			this.label21 = new System.Windows.Forms.Label();
			this.label22 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(364,153);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75,26);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(364,112);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75,26);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// comboSchoolCourse
			// 
			this.comboSchoolCourse.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboSchoolCourse.Location = new System.Drawing.Point(127,64);
			this.comboSchoolCourse.MaxDropDownItems = 30;
			this.comboSchoolCourse.Name = "comboSchoolCourse";
			this.comboSchoolCourse.Size = new System.Drawing.Size(130,21);
			this.comboSchoolCourse.TabIndex = 93;
			// 
			// comboSchoolClass
			// 
			this.comboSchoolClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboSchoolClass.Location = new System.Drawing.Point(127,41);
			this.comboSchoolClass.MaxDropDownItems = 30;
			this.comboSchoolClass.Name = "comboSchoolClass";
			this.comboSchoolClass.Size = new System.Drawing.Size(130,21);
			this.comboSchoolClass.TabIndex = 92;
			// 
			// label21
			// 
			this.label21.Location = new System.Drawing.Point(43,68);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(82,16);
			this.label21.TabIndex = 91;
			this.label21.Text = "Course";
			this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label22
			// 
			this.label22.Location = new System.Drawing.Point(43,45);
			this.label22.Name = "label22";
			this.label22.Size = new System.Drawing.Size(82,16);
			this.label22.TabIndex = 90;
			this.label22.Text = "Class";
			this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormRpCourseGrades
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5,13);
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(458,204);
			this.Controls.Add(this.comboSchoolCourse);
			this.Controls.Add(this.comboSchoolClass);
			this.Controls.Add(this.label21);
			this.Controls.Add(this.label22);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormRpCourseGrades";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Course Grades";
			this.Load += new System.EventHandler(this.FormRpCourseGrades_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.ComboBox comboSchoolCourse;
		private System.Windows.Forms.ComboBox comboSchoolClass;
		private System.Windows.Forms.Label label21;
		private System.Windows.Forms.Label label22;
	}
}
