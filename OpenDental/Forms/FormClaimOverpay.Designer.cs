namespace OpenDental{
	partial class FormClaimOverpay {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormClaimOverpay));
			this.label1 = new System.Windows.Forms.Label();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.textOverpayTotal = new OpenDental.ValidDouble();
			this.textUnderpayTotal = new OpenDental.ValidDouble();
			this.butToSupplemental = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(387, 278);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(84, 16);
			this.label1.TabIndex = 117;
			this.label1.Text = "Totals";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(1075, 324);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 25);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(986, 324);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 25);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(8, 12);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.OneCell;
			this.gridMain.Size = new System.Drawing.Size(1142, 257);
			this.gridMain.TabIndex = 125;
			this.gridMain.Title = "Procedures";
			this.gridMain.TranslationName = "TableClaimProc";
			this.gridMain.CellTextChanged += new System.EventHandler(this.gridMain_CellTextChanged);
			// 
			// textOverpayTotal
			// 
			this.textOverpayTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textOverpayTotal.Location = new System.Drawing.Point(477, 274);
			this.textOverpayTotal.MaxVal = 100000000D;
			this.textOverpayTotal.MinVal = -100000000D;
			this.textOverpayTotal.Name = "textOverpayTotal";
			this.textOverpayTotal.ReadOnly = true;
			this.textOverpayTotal.Size = new System.Drawing.Size(63, 20);
			this.textOverpayTotal.TabIndex = 141;
			this.textOverpayTotal.TabStop = false;
			this.textOverpayTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textUnderpayTotal
			// 
			this.textUnderpayTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textUnderpayTotal.Location = new System.Drawing.Point(542, 274);
			this.textUnderpayTotal.MaxVal = 100000000D;
			this.textUnderpayTotal.MinVal = -100000000D;
			this.textUnderpayTotal.Name = "textUnderpayTotal";
			this.textUnderpayTotal.ReadOnly = true;
			this.textUnderpayTotal.Size = new System.Drawing.Size(63, 20);
			this.textUnderpayTotal.TabIndex = 142;
			this.textUnderpayTotal.TabStop = false;
			this.textUnderpayTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// butToSupplemental
			// 
			this.butToSupplemental.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butToSupplemental.Location = new System.Drawing.Point(533, 324);
			this.butToSupplemental.Name = "butToSupplemental";
			this.butToSupplemental.Size = new System.Drawing.Size(92, 25);
			this.butToSupplemental.TabIndex = 143;
			this.butToSupplemental.Text = "&To Supplemental";
			this.butToSupplemental.Click += new System.EventHandler(this.butToSupplemental_Click);
			// 
			// FormClaimOverpay
			// 
			this.ClientSize = new System.Drawing.Size(1156, 363);
			this.Controls.Add(this.butToSupplemental);
			this.Controls.Add(this.textUnderpayTotal);
			this.Controls.Add(this.textOverpayTotal);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormClaimOverpay";
			this.ShowInTaskbar = false;
			this.Text = "Enter Pending Supplemental Insurance Overpayments/Underpayments";
			this.Load += new System.EventHandler(this.FormClaimOverpay_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion
		private System.Windows.Forms.Label label1;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.GridOD gridMain;
		private ValidDouble textOverpayTotal;
		private ValidDouble textUnderpayTotal;
		private UI.Button butToSupplemental;
	}
}