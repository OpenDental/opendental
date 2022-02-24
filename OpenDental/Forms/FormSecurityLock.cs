using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public class FormSecurityLock:FormODBase {
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private Label label1;
		private CheckBox checkAdmin;
		private ValidDate textDate;
		private TextBox textDays;
		private Label label3;
		private Label label4;
		private Label label2;
		private Label label5;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		///<summary></summary>
		public FormSecurityLock()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSecurityLock));
			this.label1 = new System.Windows.Forms.Label();
			this.checkAdmin = new System.Windows.Forms.CheckBox();
			this.textDays = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.textDate = new OpenDental.ValidDate();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(445, 159);
			this.label1.TabIndex = 2;
			this.label1.Text = resources.GetString("label1.Text");
			// 
			// checkAdmin
			// 
			this.checkAdmin.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAdmin.Location = new System.Drawing.Point(15, 232);
			this.checkAdmin.Name = "checkAdmin";
			this.checkAdmin.Size = new System.Drawing.Size(224, 16);
			this.checkAdmin.TabIndex = 58;
			this.checkAdmin.Text = "Lock includes administrators";
			// 
			// textDays
			// 
			this.textDays.Location = new System.Drawing.Point(88, 202);
			this.textDays.Name = "textDays";
			this.textDays.Size = new System.Drawing.Size(46, 20);
			this.textDays.TabIndex = 59;
			this.textDays.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textDays_KeyDown);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(21, 176);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(65, 18);
			this.label3.TabIndex = 60;
			this.label3.Text = "Date";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(17, 203);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(70, 16);
			this.label4.TabIndex = 61;
			this.label4.Text = "Days";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDate
			// 
			this.textDate.Location = new System.Drawing.Point(88, 176);
			this.textDate.Name = "textDate";
			this.textDate.Size = new System.Drawing.Size(100, 20);
			this.textDate.TabIndex = 62;
			this.textDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textDate_KeyDown);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(298, 259);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(379, 259);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(139, 204);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(234, 16);
			this.label2.TabIndex = 63;
			this.label2.Text = "1 means only today, 0 to disable.";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(194, 177);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(234, 16);
			this.label5.TabIndex = 64;
			this.label5.Text = "Locks this date and prior dates";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormSecurityLock
			// 
			this.ClientSize = new System.Drawing.Size(470, 299);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textDate);
			this.Controls.Add(this.textDays);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.checkAdmin);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormSecurityLock";
			this.ShowInTaskbar = false;
			this.Text = "Lock Date";
			this.Load += new System.EventHandler(this.FormSecurityLock_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private void FormSecurityLock_Load(object sender,EventArgs e) {
			if(PrefC.GetDate(PrefName.SecurityLockDate).Year>1880){
				textDate.Text=PrefC.GetDate(PrefName.SecurityLockDate).ToShortDateString();
			}
			if(PrefC.GetInt(PrefName.SecurityLockDays)>0) {
				textDays.Text=PrefC.GetInt(PrefName.SecurityLockDays).ToString();
			}
			checkAdmin.Checked=PrefC.GetBool(PrefName.SecurityLockIncludesAdmin);
		}

		private void textDate_KeyDown(object sender,System.Windows.Forms.KeyEventArgs e) {
			textDays.Text="";
		}

		private void textDays_KeyDown(object sender,System.Windows.Forms.KeyEventArgs e) {
			textDate.Text="";
			textDate.Validate();
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!textDate.IsValid()) {
				MsgBox.Show(this,"Please fix error first.");
				return;
			}
			int days = 0;
			if(!int.TryParse(textDays.Text,out days) && !string.IsNullOrEmpty(textDays.Text)) {
				MsgBox.Show(this,"Invalid number of days.");
				return;
			}
			if(days<0) {
				days=0;
			}
			if(days>3650) {
				switch(MessageBox.Show("Lock days set to greater than ten years, would you like to disable lock days instead?","",MessageBoxButtons.YesNoCancel)) {
					case DialogResult.Cancel:
						return;
					case DialogResult.OK:
					case DialogResult.Yes:
						days=0;//disable instead of using large value.
						break;
					default:
						break;
				}
			}
			DateTime date=PIn.Date(textDate.Text);
			if(Prefs.UpdateString(PrefName.SecurityLockDate,POut.Date(date,false))
				| Prefs.UpdateInt(PrefName.SecurityLockDays,days)
				| Prefs.UpdateBool(PrefName.SecurityLockIncludesAdmin,checkAdmin.Checked)  )
			{
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		


	}
}





















