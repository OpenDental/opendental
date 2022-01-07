using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormReqStudentEdit {
		///<summary>Required designer variable.</summary>
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormReqStudentEdit));
			this.label1 = new System.Windows.Forms.Label();
			this.textStudent = new System.Windows.Forms.TextBox();
			this.textAppointment = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textPatient = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.textDateCompleted = new System.Windows.Forms.TextBox();
			this.comboInstructor = new OpenDental.UI.ComboBoxOD();
			this.label11 = new System.Windows.Forms.Label();
			this.butNow = new OpenDental.UI.Button();
			this.butDetachPat = new OpenDental.UI.Button();
			this.butDetachApt = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textCourse = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.butSelectPat = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(44, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(89, 17);
			this.label1.TabIndex = 2;
			this.label1.Text = "Student";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textStudent
			// 
			this.textStudent.Location = new System.Drawing.Point(133, 8);
			this.textStudent.Name = "textStudent";
			this.textStudent.ReadOnly = true;
			this.textStudent.Size = new System.Drawing.Size(319, 20);
			this.textStudent.TabIndex = 0;
			// 
			// textAppointment
			// 
			this.textAppointment.Location = new System.Drawing.Point(133, 133);
			this.textAppointment.Name = "textAppointment";
			this.textAppointment.ReadOnly = true;
			this.textAppointment.Size = new System.Drawing.Size(319, 20);
			this.textAppointment.TabIndex = 103;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(44, 134);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(89, 17);
			this.label4.TabIndex = 104;
			this.label4.Text = "Appointment";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPatient
			// 
			this.textPatient.Location = new System.Drawing.Point(133, 108);
			this.textPatient.Name = "textPatient";
			this.textPatient.ReadOnly = true;
			this.textPatient.Size = new System.Drawing.Size(319, 20);
			this.textPatient.TabIndex = 106;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(44, 109);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(89, 17);
			this.label5.TabIndex = 107;
			this.label5.Text = "Patient";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(8, 84);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(123, 17);
			this.label6.TabIndex = 110;
			this.label6.Text = "Date Completed";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDateCompleted
			// 
			this.textDateCompleted.Location = new System.Drawing.Point(133, 83);
			this.textDateCompleted.Name = "textDateCompleted";
			this.textDateCompleted.Size = new System.Drawing.Size(114, 20);
			this.textDateCompleted.TabIndex = 111;
			// 
			// comboInstructor
			// 
			this.comboInstructor.Location = new System.Drawing.Point(133, 158);
			this.comboInstructor.Name = "comboInstructor";
			this.comboInstructor.Size = new System.Drawing.Size(158, 21);
			this.comboInstructor.TabIndex = 121;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(44, 161);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(89, 17);
			this.label11.TabIndex = 120;
			this.label11.Text = "Instructor";
			this.label11.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butNow
			// 
			this.butNow.Location = new System.Drawing.Point(249, 81);
			this.butNow.Name = "butNow";
			this.butNow.Size = new System.Drawing.Size(75, 24);
			this.butNow.TabIndex = 118;
			this.butNow.Text = "Now";
			this.butNow.Click += new System.EventHandler(this.butNow_Click);
			// 
			// butDetachPat
			// 
			this.butDetachPat.Location = new System.Drawing.Point(458, 105);
			this.butDetachPat.Name = "butDetachPat";
			this.butDetachPat.Size = new System.Drawing.Size(75, 24);
			this.butDetachPat.TabIndex = 108;
			this.butDetachPat.Text = "Detach";
			this.butDetachPat.Click += new System.EventHandler(this.butDetachPat_Click);
			// 
			// butDetachApt
			// 
			this.butDetachApt.Location = new System.Drawing.Point(458, 130);
			this.butDetachApt.Name = "butDetachApt";
			this.butDetachApt.Size = new System.Drawing.Size(75, 24);
			this.butDetachApt.TabIndex = 105;
			this.butDetachApt.Text = "Detach";
			this.butDetachApt.Click += new System.EventHandler(this.butDetachApt_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(27, 245);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(81, 26);
			this.butDelete.TabIndex = 4;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(458, 245);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 2;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(536, 245);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 9;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textDescription
			// 
			this.textDescription.Location = new System.Drawing.Point(133, 58);
			this.textDescription.Name = "textDescription";
			this.textDescription.ReadOnly = true;
			this.textDescription.Size = new System.Drawing.Size(319, 20);
			this.textDescription.TabIndex = 123;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(12, 59);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(121, 17);
			this.label2.TabIndex = 124;
			this.label2.Text = "Requirement";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCourse
			// 
			this.textCourse.Location = new System.Drawing.Point(133, 33);
			this.textCourse.Name = "textCourse";
			this.textCourse.ReadOnly = true;
			this.textCourse.Size = new System.Drawing.Size(319, 20);
			this.textCourse.TabIndex = 125;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(44, 34);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(89, 17);
			this.label10.TabIndex = 126;
			this.label10.Text = "Course";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butSelectPat
			// 
			this.butSelectPat.Location = new System.Drawing.Point(536, 105);
			this.butSelectPat.Name = "butSelectPat";
			this.butSelectPat.Size = new System.Drawing.Size(75, 24);
			this.butSelectPat.TabIndex = 127;
			this.butSelectPat.Text = "Select";
			this.butSelectPat.Click += new System.EventHandler(this.butSelectPat_Click);
			// 
			// FormReqStudentEdit
			// 
			this.ClientSize = new System.Drawing.Size(657, 289);
			this.Controls.Add(this.butSelectPat);
			this.Controls.Add(this.textCourse);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.butNow);
			this.Controls.Add(this.textDateCompleted);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.comboInstructor);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.butDetachPat);
			this.Controls.Add(this.textPatient);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.butDetachApt);
			this.Controls.Add(this.textAppointment);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.textStudent);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormReqStudentEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Student Requirement";
			this.Load += new System.EventHandler(this.FormReqStudentEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textStudent;
		private OpenDental.UI.Button butDelete;
		private TextBox textAppointment;
		private Label label4;
		private OpenDental.UI.Button butDetachApt;
		private OpenDental.UI.Button butDetachPat;
		private TextBox textPatient;
		private Label label5;
		private Label label6;
		private TextBox textDateCompleted;
		private OpenDental.UI.ComboBoxOD comboInstructor;
		private Label label11;
		private OpenDental.UI.Button butNow;
		private TextBox textDescription;
		private Label label2;
		private TextBox textCourse;
		private Label label10;
		private OpenDental.UI.Button butSelectPat;
	}
}
