using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormEmployeeEdit {
		private System.ComponentModel.IContainer components = null;

		///<summary></summary>
		protected override void Dispose( bool disposing ){
			if( disposing ){
				if(components != null){
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		private void InitializeComponent(){
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEmployeeEdit));
			this.textLName = new System.Windows.Forms.TextBox();
			this.textFName = new System.Windows.Forms.TextBox();
			this.textMI = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.checkIsHidden = new System.Windows.Forms.CheckBox();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.textPhoneExt = new System.Windows.Forms.TextBox();
			this.labelPhoneExt = new System.Windows.Forms.Label();
			this.textPayrollID = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textWirelessPhone = new OpenDental.ValidPhone();
			this.label2 = new System.Windows.Forms.Label();
			this.textEmailWork = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textEmailPersonal = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.checkIsFurloughed = new System.Windows.Forms.CheckBox();
			this.checkIsWorkingHome = new System.Windows.Forms.CheckBox();
			this.label5 = new System.Windows.Forms.Label();
			this.comboReportsTo = new OpenDental.UI.ComboBoxOD();
			this.SuspendLayout();
			// 
			// textLName
			// 
			this.textLName.Location = new System.Drawing.Point(150, 51);
			this.textLName.MaxLength = 100;
			this.textLName.Name = "textLName";
			this.textLName.Size = new System.Drawing.Size(174, 20);
			this.textLName.TabIndex = 0;
			// 
			// textFName
			// 
			this.textFName.Location = new System.Drawing.Point(150, 77);
			this.textFName.MaxLength = 100;
			this.textFName.Name = "textFName";
			this.textFName.Size = new System.Drawing.Size(174, 20);
			this.textFName.TabIndex = 1;
			// 
			// textMI
			// 
			this.textMI.Location = new System.Drawing.Point(150, 103);
			this.textMI.MaxLength = 100;
			this.textMI.Name = "textMI";
			this.textMI.Size = new System.Drawing.Size(88, 20);
			this.textMI.TabIndex = 2;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(79, 55);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(70, 14);
			this.label10.TabIndex = 31;
			this.label10.Text = "Last Name";
			this.label10.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(79, 81);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(70, 14);
			this.label8.TabIndex = 29;
			this.label8.Text = "First Name";
			this.label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(75, 107);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(70, 14);
			this.label7.TabIndex = 28;
			this.label7.Text = "MI";
			this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// checkIsHidden
			// 
			this.checkIsHidden.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsHidden.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIsHidden.Location = new System.Drawing.Point(93, 27);
			this.checkIsHidden.Name = "checkIsHidden";
			this.checkIsHidden.Size = new System.Drawing.Size(70, 18);
			this.checkIsHidden.TabIndex = 32;
			this.checkIsHidden.Text = "Hidden";
			this.checkIsHidden.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(406, 389);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 25);
			this.butCancel.TabIndex = 35;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(325, 389);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 25);
			this.butOK.TabIndex = 10;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(45, 389);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(81, 25);
			this.butDelete.TabIndex = 36;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// textPhoneExt
			// 
			this.textPhoneExt.Location = new System.Drawing.Point(150, 155);
			this.textPhoneExt.MaxLength = 100;
			this.textPhoneExt.Name = "textPhoneExt";
			this.textPhoneExt.Size = new System.Drawing.Size(70, 20);
			this.textPhoneExt.TabIndex = 4;
			// 
			// labelPhoneExt
			// 
			this.labelPhoneExt.Location = new System.Drawing.Point(79, 159);
			this.labelPhoneExt.Name = "labelPhoneExt";
			this.labelPhoneExt.Size = new System.Drawing.Size(70, 14);
			this.labelPhoneExt.TabIndex = 40;
			this.labelPhoneExt.Text = "Phone Ext.";
			this.labelPhoneExt.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textPayrollID
			// 
			this.textPayrollID.Location = new System.Drawing.Point(150, 129);
			this.textPayrollID.MaxLength = 100;
			this.textPayrollID.Name = "textPayrollID";
			this.textPayrollID.Size = new System.Drawing.Size(88, 20);
			this.textPayrollID.TabIndex = 3;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(79, 133);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(70, 14);
			this.label1.TabIndex = 42;
			this.label1.Text = "Payroll ID";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textWirelessPhone
			// 
			this.textWirelessPhone.Location = new System.Drawing.Point(150, 181);
			this.textWirelessPhone.MaxLength = 30;
			this.textWirelessPhone.Name = "textWirelessPhone";
			this.textWirelessPhone.Size = new System.Drawing.Size(242, 20);
			this.textWirelessPhone.TabIndex = 5;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(59, 185);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(90, 14);
			this.label2.TabIndex = 44;
			this.label2.Text = "Wireless Phone";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textEmailWork
			// 
			this.textEmailWork.Location = new System.Drawing.Point(150, 207);
			this.textEmailWork.MaxLength = 100;
			this.textEmailWork.Name = "textEmailWork";
			this.textEmailWork.Size = new System.Drawing.Size(272, 20);
			this.textEmailWork.TabIndex = 6;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(59, 211);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(90, 14);
			this.label3.TabIndex = 46;
			this.label3.Text = "Email Work";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textEmailPersonal
			// 
			this.textEmailPersonal.Location = new System.Drawing.Point(150, 233);
			this.textEmailPersonal.MaxLength = 100;
			this.textEmailPersonal.Name = "textEmailPersonal";
			this.textEmailPersonal.Size = new System.Drawing.Size(272, 20);
			this.textEmailPersonal.TabIndex = 7;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(59, 237);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(90, 14);
			this.label4.TabIndex = 48;
			this.label4.Text = "Email Personal";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// checkIsFurloughed
			// 
			this.checkIsFurloughed.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsFurloughed.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIsFurloughed.Location = new System.Drawing.Point(46, 260);
			this.checkIsFurloughed.Name = "checkIsFurloughed";
			this.checkIsFurloughed.Size = new System.Drawing.Size(117, 18);
			this.checkIsFurloughed.TabIndex = 8;
			this.checkIsFurloughed.Text = "Furloughed";
			this.checkIsFurloughed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkIsWorkingHome
			// 
			this.checkIsWorkingHome.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsWorkingHome.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIsWorkingHome.Location = new System.Drawing.Point(46, 282);
			this.checkIsWorkingHome.Name = "checkIsWorkingHome";
			this.checkIsWorkingHome.Size = new System.Drawing.Size(117, 18);
			this.checkIsWorkingHome.TabIndex = 9;
			this.checkIsWorkingHome.Text = "Working From Home";
			this.checkIsWorkingHome.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(59, 309);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(90, 14);
			this.label5.TabIndex = 49;
			this.label5.Text = "Reports to";
			this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// comboReportsTo
			// 
			this.comboReportsTo.Location = new System.Drawing.Point(150, 305);
			this.comboReportsTo.Name = "comboReportsTo";
			this.comboReportsTo.Size = new System.Drawing.Size(174, 21);
			this.comboReportsTo.TabIndex = 50;
			// 
			// FormEmployeeEdit
			// 
			this.AcceptButton = this.butOK;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(500, 431);
			this.Controls.Add(this.comboReportsTo);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.checkIsWorkingHome);
			this.Controls.Add(this.checkIsFurloughed);
			this.Controls.Add(this.textEmailPersonal);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textEmailWork);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textWirelessPhone);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textPayrollID);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textPhoneExt);
			this.Controls.Add(this.labelPhoneExt);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.checkIsHidden);
			this.Controls.Add(this.textLName);
			this.Controls.Add(this.textFName);
			this.Controls.Add(this.textMI);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label7);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormEmployeeEdit";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Employee Edit";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.FormEmployeeEdit_Closing);
			this.Load += new System.EventHandler(this.FormEmployeeEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.TextBox textLName;
		private System.Windows.Forms.TextBox textFName;
		private System.Windows.Forms.TextBox textMI;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label7;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.CheckBox checkIsHidden;
		private OpenDental.UI.Button butDelete;
		private TextBox textPhoneExt;
		private Label labelPhoneExt;
		private TextBox textPayrollID;
		private Label label1;
		private Label label2;
		private TextBox textEmailWork;
		private Label label3;
		private TextBox textEmailPersonal;
		private Label label4;
		private CheckBox checkIsFurloughed;
		private CheckBox checkIsWorkingHome;
		private Label label5;
		private UI.ComboBoxOD comboReportsTo;
		private ValidPhone textWirelessPhone;
	}
}
