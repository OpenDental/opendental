namespace OpenDental{
	partial class FormPhoneNumbersManage {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPhoneNumbersManage));
			this.label1 = new System.Windows.Forms.Label();
			this.textName = new System.Windows.Forms.TextBox();
			this.textWkPhone = new OpenDental.ValidPhone();
			this.label2 = new System.Windows.Forms.Label();
			this.textHmPhone = new OpenDental.ValidPhone();
			this.label3 = new System.Windows.Forms.Label();
			this.textWirelessPhone = new OpenDental.ValidPhone();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.listOther = new OpenDental.UI.ListBoxOD();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.textAddrNotes = new OpenDental.ODtextBox();
			this.butDelete = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butAdd = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(26, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(123, 16);
			this.label1.TabIndex = 4;
			this.label1.Text = "Office Name";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textName
			// 
			this.textName.Location = new System.Drawing.Point(151, 20);
			this.textName.Name = "textName";
			this.textName.ReadOnly = true;
			this.textName.Size = new System.Drawing.Size(425, 20);
			this.textName.TabIndex = 5;
			// 
			// textWkPhone
			// 
			this.textWkPhone.Location = new System.Drawing.Point(151, 46);
			this.textWkPhone.Name = "textWkPhone";
			this.textWkPhone.Size = new System.Drawing.Size(224, 20);
			this.textWkPhone.TabIndex = 7;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(26, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(123, 16);
			this.label2.TabIndex = 6;
			this.label2.Text = "Work Phone";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textHmPhone
			// 
			this.textHmPhone.Location = new System.Drawing.Point(151, 72);
			this.textHmPhone.Name = "textHmPhone";
			this.textHmPhone.Size = new System.Drawing.Size(224, 20);
			this.textHmPhone.TabIndex = 9;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(26, 74);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(123, 16);
			this.label3.TabIndex = 8;
			this.label3.Text = "Home Phone";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textWirelessPhone
			// 
			this.textWirelessPhone.Location = new System.Drawing.Point(151, 98);
			this.textWirelessPhone.Name = "textWirelessPhone";
			this.textWirelessPhone.Size = new System.Drawing.Size(224, 20);
			this.textWirelessPhone.TabIndex = 11;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(26, 100);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(123, 16);
			this.label4.TabIndex = 10;
			this.label4.Text = "Wireless Phone";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(42, 191);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(107, 16);
			this.label5.TabIndex = 12;
			this.label5.Text = "Other Numbers";
			this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// listOther
			// 
			this.listOther.Location = new System.Drawing.Point(151, 189);
			this.listOther.Name = "listOther";
			this.listOther.Size = new System.Drawing.Size(178, 186);
			this.listOther.TabIndex = 13;
			this.listOther.DoubleClick += new System.EventHandler(this.listOther_DoubleClick);
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(148, 408);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(225, 55);
			this.label6.TabIndex = 15;
			this.label6.Text = "The change will not show immediately in the phone grid, but will instead show the" +
    " next time the patient calls.";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(2, 125);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(147, 28);
			this.label7.TabIndex = 17;
			this.label7.Text = "Address and Phone Notes";
			this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textAddrNotes
			// 
			this.textAddrNotes.AcceptsTab = true;
			this.textAddrNotes.BackColor = System.Drawing.SystemColors.Window;
			this.textAddrNotes.DetectLinksEnabled = false;
			this.textAddrNotes.DetectUrls = false;
			this.textAddrNotes.Location = new System.Drawing.Point(151, 124);
			this.textAddrNotes.Name = "textAddrNotes";
			this.textAddrNotes.QuickPasteType = OpenDentBusiness.QuickPasteType.PatAddressNote;
			this.textAddrNotes.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textAddrNotes.Size = new System.Drawing.Size(224, 59);
			this.textAddrNotes.TabIndex = 16;
			this.textAddrNotes.Text = "";
			// 
			// butDelete
			// 
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(151, 381);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(79, 24);
			this.butDelete.TabIndex = 14;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(533, 395);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(533, 433);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butAdd
			// 
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(250, 381);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(79, 24);
			this.butAdd.TabIndex = 14;
			this.butAdd.Text = "Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// FormPhoneNumbersManage
			// 
			this.ClientSize = new System.Drawing.Size(633, 484);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.textAddrNotes);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.listOther);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textWirelessPhone);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textHmPhone);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textWkPhone);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textName);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormPhoneNumbersManage";
			this.Text = "Phone Numbers";
			this.Load += new System.EventHandler(this.FormPhoneNumbersManage_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textName;
		private ValidPhone textWkPhone;
		private System.Windows.Forms.Label label2;
		private ValidPhone textHmPhone;
		private System.Windows.Forms.Label label3;
		private ValidPhone textWirelessPhone;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private OpenDental.UI.ListBoxOD listOther;
		private OpenDental.UI.Button butDelete;
		private System.Windows.Forms.Label label6;
		private ODtextBox textAddrNotes;
		private System.Windows.Forms.Label label7;
		private UI.Button butAdd;
	}
}