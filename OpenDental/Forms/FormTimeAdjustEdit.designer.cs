using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormTimeAdjustEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTimeAdjustEdit));
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textTimeEntry = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textNote = new System.Windows.Forms.TextBox();
			this.checkOvertime = new System.Windows.Forms.CheckBox();
			this.textHours = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.butDelete = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.radioAuto = new System.Windows.Forms.RadioButton();
			this.radioManual = new System.Windows.Forms.RadioButton();
			this.label5 = new System.Windows.Forms.Label();
			this.comboPTO = new OpenDental.UI.ComboBoxOD();
			this.labelPTO = new System.Windows.Forms.Label();
			this.checkUnpaidProtectedLeave = new System.Windows.Forms.CheckBox();
			this.textUser = new System.Windows.Forms.TextBox();
			this.labelUser = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(37, 49);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(126, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "Date/Time Entry";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(37, 76);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(126, 20);
			this.label2.TabIndex = 0;
			this.label2.Text = "Hours";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textTimeEntry
			// 
			this.textTimeEntry.Location = new System.Drawing.Point(164, 50);
			this.textTimeEntry.Name = "textTimeEntry";
			this.textTimeEntry.Size = new System.Drawing.Size(155, 20);
			this.textTimeEntry.TabIndex = 3;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(37, 203);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(126, 20);
			this.label4.TabIndex = 0;
			this.label4.Text = "Note";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textNote
			// 
			this.textNote.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textNote.Location = new System.Drawing.Point(164, 204);
			this.textNote.Multiline = true;
			this.textNote.Name = "textNote";
			this.textNote.Size = new System.Drawing.Size(377, 96);
			this.textNote.TabIndex = 7;
			// 
			// checkOvertime
			// 
			this.checkOvertime.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkOvertime.Location = new System.Drawing.Point(39, 103);
			this.checkOvertime.Name = "checkOvertime";
			this.checkOvertime.Size = new System.Drawing.Size(139, 17);
			this.checkOvertime.TabIndex = 5;
			this.checkOvertime.Text = "Overtime Adjustment";
			this.checkOvertime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkOvertime.UseVisualStyleBackColor = true;
			// 
			// textHours
			// 
			this.textHours.Location = new System.Drawing.Point(164, 77);
			this.textHours.Name = "textHours";
			this.textHours.Size = new System.Drawing.Size(68, 20);
			this.textHours.TabIndex = 4;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(179, 101);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(300, 18);
			this.label3.TabIndex = 0;
			this.label3.Text = "(the hours will be shifted from regular time to overtime)";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(37, 320);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(79, 24);
			this.butDelete.TabIndex = 90;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(385, 320);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 98;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(466, 320);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 99;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// radioAuto
			// 
			this.radioAuto.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioAuto.Location = new System.Drawing.Point(39, 10);
			this.radioAuto.Name = "radioAuto";
			this.radioAuto.Size = new System.Drawing.Size(139, 18);
			this.radioAuto.TabIndex = 1;
			this.radioAuto.TabStop = true;
			this.radioAuto.Text = "Automatically entered";
			this.radioAuto.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioAuto.UseVisualStyleBackColor = true;
			// 
			// radioManual
			// 
			this.radioManual.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioManual.Location = new System.Drawing.Point(39, 27);
			this.radioManual.Name = "radioManual";
			this.radioManual.Size = new System.Drawing.Size(139, 18);
			this.radioManual.TabIndex = 2;
			this.radioManual.TabStop = true;
			this.radioManual.Text = "Manually entered";
			this.radioManual.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioManual.UseVisualStyleBackColor = true;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(179, 27);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(170, 18);
			this.label5.TabIndex = 0;
			this.label5.Text = "(protected from auto deletion)";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// comboPTO
			// 
			this.comboPTO.Location = new System.Drawing.Point(164, 149);
			this.comboPTO.Name = "comboPTO";
			this.comboPTO.Size = new System.Drawing.Size(121, 21);
			this.comboPTO.TabIndex = 6;
			// 
			// labelPTO
			// 
			this.labelPTO.Location = new System.Drawing.Point(37, 148);
			this.labelPTO.Name = "labelPTO";
			this.labelPTO.Size = new System.Drawing.Size(126, 20);
			this.labelPTO.TabIndex = 0;
			this.labelPTO.Text = "PTO Type";
			this.labelPTO.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkUnpaidProtectedLeave
			// 
			this.checkUnpaidProtectedLeave.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUnpaidProtectedLeave.Location = new System.Drawing.Point(1, 126);
			this.checkUnpaidProtectedLeave.Name = "checkUnpaidProtectedLeave";
			this.checkUnpaidProtectedLeave.Size = new System.Drawing.Size(177, 17);
			this.checkUnpaidProtectedLeave.TabIndex = 100;
			this.checkUnpaidProtectedLeave.Text = "Protected Leave";
			this.checkUnpaidProtectedLeave.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUnpaidProtectedLeave.UseVisualStyleBackColor = true;
			this.checkUnpaidProtectedLeave.Click += new System.EventHandler(this.checkUnpaidProtectedLeave_Click);
			// 
			// textUser
			// 
			this.textUser.Location = new System.Drawing.Point(164, 177);
			this.textUser.Name = "textUser";
			this.textUser.ReadOnly = true;
			this.textUser.Size = new System.Drawing.Size(121, 20);
			this.textUser.TabIndex = 102;
			this.textUser.TabStop = false;
			// 
			// labelUser
			// 
			this.labelUser.Location = new System.Drawing.Point(59, 179);
			this.labelUser.Name = "labelUser";
			this.labelUser.Size = new System.Drawing.Size(104, 17);
			this.labelUser.TabIndex = 103;
			this.labelUser.Text = "User";
			this.labelUser.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// FormTimeAdjustEdit
			// 
			this.ClientSize = new System.Drawing.Size(567, 364);
			this.Controls.Add(this.textUser);
			this.Controls.Add(this.labelUser);
			this.Controls.Add(this.checkUnpaidProtectedLeave);
			this.Controls.Add(this.labelPTO);
			this.Controls.Add(this.comboPTO);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.radioManual);
			this.Controls.Add(this.radioAuto);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textHours);
			this.Controls.Add(this.checkOvertime);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textTimeEntry);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormTimeAdjustEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Time Adjustment";
			this.Load += new System.EventHandler(this.FormTimeAdjustEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private Label label1;
		private Label label2;
		private TextBox textTimeEntry;
		private Label label4;
		private TextBox textNote;
		private CheckBox checkOvertime;
		private OpenDental.UI.Button butDelete;
		private TextBox textHours;
		private Label label3;
		private RadioButton radioAuto;
		private RadioButton radioManual;
		private Label label5;
		private OpenDental.UI.ComboBoxOD comboPTO;
		private Label labelPTO;
		private CheckBox checkUnpaidProtectedLeave;
		private TextBox textUser;
		private Label labelUser;
	}
}
