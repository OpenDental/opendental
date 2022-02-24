using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormClaimReports {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormClaimReports));
			this.labelRetrieving = new System.Windows.Forms.Label();
			this.comboClearhouse = new System.Windows.Forms.ComboBox();
			this.butRetrieve = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// labelRetrieving
			// 
			this.labelRetrieving.Font = new System.Drawing.Font("Microsoft Sans Serif",8.25F,System.Drawing.FontStyle.Bold,System.Drawing.GraphicsUnit.Point,((byte)(0)));
			this.labelRetrieving.Location = new System.Drawing.Point(12,72);
			this.labelRetrieving.Name = "labelRetrieving";
			this.labelRetrieving.Size = new System.Drawing.Size(366,20);
			this.labelRetrieving.TabIndex = 1;
			this.labelRetrieving.Text = "Retrieving reports from selected clearinghouse.";
			this.labelRetrieving.Visible = false;
			// 
			// comboClearhouse
			// 
			this.comboClearhouse.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboClearhouse.Location = new System.Drawing.Point(18,32);
			this.comboClearhouse.Name = "comboClearhouse";
			this.comboClearhouse.Size = new System.Drawing.Size(187,21);
			this.comboClearhouse.TabIndex = 2;
			// 
			// butRetrieve
			// 
			this.butRetrieve.Location = new System.Drawing.Point(222,29);
			this.butRetrieve.Name = "butRetrieve";
			this.butRetrieve.Size = new System.Drawing.Size(90,26);
			this.butRetrieve.TabIndex = 5;
			this.butRetrieve.Text = "Retrieve";
			this.butRetrieve.Click += new System.EventHandler(this.butRetrieve_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(289,152);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75,26);
			this.butClose.TabIndex = 0;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// FormClaimReports
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5,13);
			this.ClientSize = new System.Drawing.Size(401,202);
			this.Controls.Add(this.butRetrieve);
			this.Controls.Add(this.comboClearhouse);
			this.Controls.Add(this.labelRetrieving);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormClaimReports";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "E-claim Reports";
			this.Load += new System.EventHandler(this.FormClaimReports_Load);
			this.Shown += new System.EventHandler(this.FormClaimReports_Shown);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butClose;
		private System.Windows.Forms.Label labelRetrieving;
		private System.Windows.Forms.ComboBox comboClearhouse;
		private OpenDental.UI.Button butRetrieve;
	}
}
