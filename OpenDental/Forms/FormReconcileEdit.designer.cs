using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormReconcileEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormReconcileEdit));
			this.labelDate = new System.Windows.Forms.Label();
			this.labelStart = new System.Windows.Forms.Label();
			this.labelEnd = new System.Windows.Forms.Label();
			this.textTarget = new System.Windows.Forms.TextBox();
			this.labelTarget = new System.Windows.Forms.Label();
			this.labelSum = new System.Windows.Forms.Label();
			this.textSum = new System.Windows.Forms.TextBox();
			this.checkLocked = new System.Windows.Forms.CheckBox();
			this.textFindAmount = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.butPrint = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.textEnd = new OpenDental.ValidDouble();
			this.textStart = new OpenDental.ValidDouble();
			this.textDate = new OpenDental.ValidDate();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butExport = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// labelDate
			// 
			this.labelDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelDate.Location = new System.Drawing.Point(350, 8);
			this.labelDate.Name = "labelDate";
			this.labelDate.Size = new System.Drawing.Size(116, 17);
			this.labelDate.TabIndex = 3;
			this.labelDate.Text = "Date";
			this.labelDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelStart
			// 
			this.labelStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelStart.Location = new System.Drawing.Point(350, 51);
			this.labelStart.Name = "labelStart";
			this.labelStart.Size = new System.Drawing.Size(116, 17);
			this.labelStart.TabIndex = 5;
			this.labelStart.Text = "Starting Balance";
			this.labelStart.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelEnd
			// 
			this.labelEnd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelEnd.Location = new System.Drawing.Point(350, 72);
			this.labelEnd.Name = "labelEnd";
			this.labelEnd.Size = new System.Drawing.Size(116, 17);
			this.labelEnd.TabIndex = 7;
			this.labelEnd.Text = "Ending Balance";
			this.labelEnd.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textTarget
			// 
			this.textTarget.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textTarget.Location = new System.Drawing.Point(467, 91);
			this.textTarget.Name = "textTarget";
			this.textTarget.ReadOnly = true;
			this.textTarget.Size = new System.Drawing.Size(114, 20);
			this.textTarget.TabIndex = 9;
			this.textTarget.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// labelTarget
			// 
			this.labelTarget.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTarget.Location = new System.Drawing.Point(350, 93);
			this.labelTarget.Name = "labelTarget";
			this.labelTarget.Size = new System.Drawing.Size(116, 17);
			this.labelTarget.TabIndex = 10;
			this.labelTarget.Text = "Target Change";
			this.labelTarget.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelSum
			// 
			this.labelSum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelSum.Location = new System.Drawing.Point(350, 114);
			this.labelSum.Name = "labelSum";
			this.labelSum.Size = new System.Drawing.Size(116, 17);
			this.labelSum.TabIndex = 13;
			this.labelSum.Text = "Sum of Transactions";
			this.labelSum.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSum
			// 
			this.textSum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textSum.Location = new System.Drawing.Point(467, 112);
			this.textSum.Name = "textSum";
			this.textSum.ReadOnly = true;
			this.textSum.Size = new System.Drawing.Size(114, 20);
			this.textSum.TabIndex = 12;
			this.textSum.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// checkLocked
			// 
			this.checkLocked.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkLocked.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkLocked.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkLocked.Location = new System.Drawing.Point(350, 29);
			this.checkLocked.Name = "checkLocked";
			this.checkLocked.Size = new System.Drawing.Size(130, 17);
			this.checkLocked.TabIndex = 14;
			this.checkLocked.Text = "Locked";
			this.checkLocked.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkLocked.UseVisualStyleBackColor = true;
			this.checkLocked.Click += new System.EventHandler(this.checkLocked_Click);
			// 
			// textFindAmount
			// 
			this.textFindAmount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textFindAmount.Location = new System.Drawing.Point(467, 187);
			this.textFindAmount.Name = "textFindAmount";
			this.textFindAmount.Size = new System.Drawing.Size(114, 20);
			this.textFindAmount.TabIndex = 16;
			this.textFindAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textFindAmount.TextChanged += new System.EventHandler(this.textFindAmount_TextChanged);
			this.textFindAmount.Leave += new System.EventHandler(this.textFindAmount_Leave);
			// 
			// label6
			// 
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label6.Location = new System.Drawing.Point(350, 189);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(116, 17);
			this.label6.TabIndex = 17;
			this.label6.Text = "Find Amount";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrint;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(269, 648);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(75, 26);
			this.butPrint.TabIndex = 19;
			this.butPrint.Text = "Print";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(6, 648);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 26);
			this.butDelete.TabIndex = 15;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(6, 6);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(338, 636);
			this.gridMain.TabIndex = 11;
			this.gridMain.Title = "Transactions";
			this.gridMain.TranslationName = "TableJournal";
			this.gridMain.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellClick);
			// 
			// textEnd
			// 
			this.textEnd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textEnd.Location = new System.Drawing.Point(467, 70);
			this.textEnd.MaxVal = 100000000D;
			this.textEnd.MinVal = -100000000D;
			this.textEnd.Name = "textEnd";
			this.textEnd.Size = new System.Drawing.Size(114, 20);
			this.textEnd.TabIndex = 8;
			this.textEnd.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textEnd.TextChanged += new System.EventHandler(this.textEnd_TextChanged);
			// 
			// textStart
			// 
			this.textStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textStart.Location = new System.Drawing.Point(467, 49);
			this.textStart.MaxVal = 100000000D;
			this.textStart.MinVal = -100000000D;
			this.textStart.Name = "textStart";
			this.textStart.Size = new System.Drawing.Size(114, 20);
			this.textStart.TabIndex = 6;
			this.textStart.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textStart.TextChanged += new System.EventHandler(this.textStart_TextChanged);
			// 
			// textDate
			// 
			this.textDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textDate.Location = new System.Drawing.Point(467, 6);
			this.textDate.Name = "textDate";
			this.textDate.Size = new System.Drawing.Size(114, 20);
			this.textDate.TabIndex = 2;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(425, 648);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(506, 648);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butExport
			// 
			this.butExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butExport.Image = global::OpenDental.Properties.Resources.butExport;
			this.butExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butExport.Location = new System.Drawing.Point(188, 648);
			this.butExport.Name = "butExport";
			this.butExport.Size = new System.Drawing.Size(75, 26);
			this.butExport.TabIndex = 20;
			this.butExport.Text = "Export";
			this.butExport.Click += new System.EventHandler(this.butExport_Click);
			// 
			// FormReconcileEdit
			// 
			this.ClientSize = new System.Drawing.Size(587, 680);
			this.Controls.Add(this.butExport);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.textFindAmount);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.checkLocked);
			this.Controls.Add(this.labelSum);
			this.Controls.Add(this.textSum);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.labelTarget);
			this.Controls.Add(this.textTarget);
			this.Controls.Add(this.textEnd);
			this.Controls.Add(this.labelEnd);
			this.Controls.Add(this.textStart);
			this.Controls.Add(this.labelStart);
			this.Controls.Add(this.labelDate);
			this.Controls.Add(this.textDate);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormReconcileEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Reconcile";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormReconcileEdit_FormClosing);
			this.Load += new System.EventHandler(this.FormReconcileEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private ValidDate textDate;
		private Label labelDate;
		private Label labelStart;
		private ValidDouble textStart;
		private ValidDouble textEnd;
		private Label labelEnd;
		private TextBox textTarget;
		private Label labelTarget;
		private OpenDental.UI.GridOD gridMain;
		private Label labelSum;
		private TextBox textSum;
		private CheckBox checkLocked;
		private OpenDental.UI.Button butDelete;
		private TextBox textFindAmount;
		private Label label6;
		private OpenDental.UI.Button butPrint;
		private UI.Button butExport;
	}
}
