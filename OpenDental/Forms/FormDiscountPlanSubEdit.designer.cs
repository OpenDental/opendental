namespace OpenDental{
	partial class FormDiscountPlanSubEdit {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDiscountPlanSubEdit));
            this.groupSubscriber = new System.Windows.Forms.GroupBox();
            this.labelSubNote = new System.Windows.Forms.Label();
            this.textName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textSubNote = new OpenDental.ODtextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textDateTerm = new OpenDental.ValidDate();
            this.textDateEffective = new OpenDental.ValidDate();
            this.label5 = new System.Windows.Forms.Label();
            this.butDrop = new OpenDental.UI.Button();
            this.butDiscountPlans = new OpenDental.UI.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textFeeSched = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textDescript = new System.Windows.Forms.TextBox();
            this.butOK = new OpenDental.UI.Button();
            this.butCancel = new OpenDental.UI.Button();
            this.textAdjustmentType = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textPlanNum = new System.Windows.Forms.TextBox();
            this.labelPlanNum = new System.Windows.Forms.Label();
            this.labelPlanNote = new System.Windows.Forms.Label();
            this.textPlanNote = new OpenDental.ODtextBox();
            this.groupSubscriber.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupSubscriber
            // 
            this.groupSubscriber.Controls.Add(this.labelSubNote);
            this.groupSubscriber.Controls.Add(this.textName);
            this.groupSubscriber.Controls.Add(this.label4);
            this.groupSubscriber.Controls.Add(this.textSubNote);
            this.groupSubscriber.Controls.Add(this.label6);
            this.groupSubscriber.Controls.Add(this.textDateTerm);
            this.groupSubscriber.Controls.Add(this.textDateEffective);
            this.groupSubscriber.Controls.Add(this.label5);
            this.groupSubscriber.Location = new System.Drawing.Point(16, 174);
            this.groupSubscriber.Name = "groupSubscriber";
            this.groupSubscriber.Size = new System.Drawing.Size(394, 142);
            this.groupSubscriber.TabIndex = 2;
            this.groupSubscriber.TabStop = false;
            this.groupSubscriber.Text = "Subscriber Information";
            // 
            // labelSubNote
            // 
            this.labelSubNote.Location = new System.Drawing.Point(1, 73);
            this.labelSubNote.Name = "labelSubNote";
            this.labelSubNote.Size = new System.Drawing.Size(109, 15);
            this.labelSubNote.TabIndex = 47;
            this.labelSubNote.Text = "Subscriber Note";
            this.labelSubNote.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textName
            // 
            this.textName.Location = new System.Drawing.Point(110, 19);
            this.textName.Name = "textName";
            this.textName.ReadOnly = true;
            this.textName.Size = new System.Drawing.Size(266, 20);
            this.textName.TabIndex = 41;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(69, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Name";
            // 
            // textSubNote
            // 
            this.textSubNote.AcceptsTab = true;
            this.textSubNote.BackColor = System.Drawing.SystemColors.Window;
            this.textSubNote.DetectLinksEnabled = false;
            this.textSubNote.DetectUrls = false;
            this.textSubNote.Location = new System.Drawing.Point(110, 71);
            this.textSubNote.Name = "textSubNote";
            this.textSubNote.QuickPasteType = OpenDentBusiness.QuickPasteType.InsPlan;
            this.textSubNote.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.textSubNote.Size = new System.Drawing.Size(266, 60);
            this.textSubNote.TabIndex = 4;
            this.textSubNote.Text = "";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(230, 48);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(20, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "To";
            // 
            // textDateTerm
            // 
            this.textDateTerm.Location = new System.Drawing.Point(256, 45);
            this.textDateTerm.Name = "textDateTerm";
            this.textDateTerm.Size = new System.Drawing.Size(120, 20);
            this.textDateTerm.TabIndex = 2;
            // 
            // textDateEffective
            // 
            this.textDateEffective.Location = new System.Drawing.Point(110, 45);
            this.textDateEffective.Name = "textDateEffective";
            this.textDateEffective.Size = new System.Drawing.Size(114, 20);
            this.textDateEffective.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(30, 48);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Effective Dates";
            // 
            // butDrop
            // 
            this.butDrop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.butDrop.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.butDrop.Location = new System.Drawing.Point(12, 332);
            this.butDrop.Name = "butDrop";
            this.butDrop.Size = new System.Drawing.Size(75, 24);
            this.butDrop.TabIndex = 11;
            this.butDrop.Text = "Drop";
            this.butDrop.Click += new System.EventHandler(this.butDrop_Click);
            // 
            // butDiscountPlans
            // 
            this.butDiscountPlans.Location = new System.Drawing.Point(372, 32);
            this.butDiscountPlans.Name = "butDiscountPlans";
            this.butDiscountPlans.Size = new System.Drawing.Size(20, 20);
            this.butDiscountPlans.TabIndex = 1;
            this.butDiscountPlans.Text = "...";
            this.butDiscountPlans.Click += new System.EventHandler(this.butDiscountPlans_Click);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(13, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 18);
            this.label2.TabIndex = 29;
            this.label2.Text = "Fee Schedule";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textFeeSched
            // 
            this.textFeeSched.Location = new System.Drawing.Point(126, 58);
            this.textFeeSched.Name = "textFeeSched";
            this.textFeeSched.ReadOnly = true;
            this.textFeeSched.Size = new System.Drawing.Size(266, 20);
            this.textFeeSched.TabIndex = 22;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(13, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 18);
            this.label1.TabIndex = 27;
            this.label1.Text = "Discount Plan";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textDescript
            // 
            this.textDescript.Location = new System.Drawing.Point(126, 33);
            this.textDescript.Name = "textDescript";
            this.textDescript.ReadOnly = true;
            this.textDescript.Size = new System.Drawing.Size(241, 20);
            this.textDescript.TabIndex = 21;
            // 
            // butOK
            // 
            this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.butOK.Location = new System.Drawing.Point(260, 332);
            this.butOK.Name = "butOK";
            this.butOK.Size = new System.Drawing.Size(75, 24);
            this.butOK.TabIndex = 5;
            this.butOK.Text = "&OK";
            this.butOK.Click += new System.EventHandler(this.butOK_Click);
            // 
            // butCancel
            // 
            this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.butCancel.Location = new System.Drawing.Point(343, 332);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(75, 24);
            this.butCancel.TabIndex = 7;
            this.butCancel.Text = "&Cancel";
            this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
            // 
            // textAdjustmentType
            // 
            this.textAdjustmentType.Location = new System.Drawing.Point(126, 82);
            this.textAdjustmentType.Name = "textAdjustmentType";
            this.textAdjustmentType.ReadOnly = true;
            this.textAdjustmentType.Size = new System.Drawing.Size(266, 20);
            this.textAdjustmentType.TabIndex = 23;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(13, 84);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 18);
            this.label3.TabIndex = 41;
            this.label3.Text = "Adjustment Type";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textPlanNum
            // 
            this.textPlanNum.Location = new System.Drawing.Point(126, 8);
            this.textPlanNum.Name = "textPlanNum";
            this.textPlanNum.ReadOnly = true;
            this.textPlanNum.Size = new System.Drawing.Size(114, 20);
            this.textPlanNum.TabIndex = 20;
            // 
            // labelPlanNum
            // 
            this.labelPlanNum.Location = new System.Drawing.Point(16, 8);
            this.labelPlanNum.Name = "labelPlanNum";
            this.labelPlanNum.Size = new System.Drawing.Size(110, 18);
            this.labelPlanNum.TabIndex = 44;
            this.labelPlanNum.Text = "Discount Plan ID";
            this.labelPlanNum.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelPlanNote
            // 
            this.labelPlanNote.Location = new System.Drawing.Point(14, 108);
            this.labelPlanNote.Name = "labelPlanNote";
            this.labelPlanNote.Size = new System.Drawing.Size(112, 18);
            this.labelPlanNote.TabIndex = 48;
            this.labelPlanNote.Text = "Plan Note";
            this.labelPlanNote.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textPlanNote
            // 
            this.textPlanNote.AcceptsTab = true;
            this.textPlanNote.BackColor = System.Drawing.SystemColors.Window;
            this.textPlanNote.DetectLinksEnabled = false;
            this.textPlanNote.DetectUrls = false;
            this.textPlanNote.Location = new System.Drawing.Point(126, 108);
            this.textPlanNote.Name = "textPlanNote";
            this.textPlanNote.QuickPasteType = OpenDentBusiness.QuickPasteType.InsPlan;
            this.textPlanNote.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.textPlanNote.Size = new System.Drawing.Size(266, 60);
            this.textPlanNote.TabIndex = 47;
            this.textPlanNote.Text = "";
            // 
            // FormDiscountPlanSubEdit
            // 
            this.ClientSize = new System.Drawing.Size(430, 368);
            this.Controls.Add(this.labelPlanNote);
            this.Controls.Add(this.textPlanNote);
            this.Controls.Add(this.labelPlanNum);
            this.Controls.Add(this.textPlanNum);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textAdjustmentType);
            this.Controls.Add(this.groupSubscriber);
            this.Controls.Add(this.butDrop);
            this.Controls.Add(this.butDiscountPlans);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textFeeSched);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textDescript);
            this.Controls.Add(this.butOK);
            this.Controls.Add(this.butCancel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormDiscountPlanSubEdit";
            this.Text = "Discount Plan Subscriber Edit";
            this.Load += new System.EventHandler(this.FormDiscountPlanSubEdit_Load);
            this.groupSubscriber.ResumeLayout(false);
            this.groupSubscriber.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox groupSubscriber;
		private System.Windows.Forms.Label label6;
		private ValidDate textDateTerm;
		private ValidDate textDateEffective;
		private System.Windows.Forms.Label label5;
		private UI.Button butDrop;
		private UI.Button butDiscountPlans;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textFeeSched;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textDescript;
		private UI.Button butOK;
		private UI.Button butCancel;
		private System.Windows.Forms.TextBox textAdjustmentType;
		private System.Windows.Forms.TextBox textName;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textPlanNum;
		private System.Windows.Forms.Label labelPlanNum;
		private OpenDental.ODtextBox textSubNote;
		private System.Windows.Forms.Label labelSubNote;
        private System.Windows.Forms.Label labelPlanNote;
        private ODtextBox textPlanNote;
    }
}