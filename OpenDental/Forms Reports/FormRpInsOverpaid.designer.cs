using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormRpInsOverpaid {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpInsOverpaid));
            this.butCancel = new OpenDental.UI.Button();
            this.butOK = new OpenDental.UI.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.radioGroupByProc = new System.Windows.Forms.RadioButton();
            this.radioGroupByPat = new System.Windows.Forms.RadioButton();
            this.checkAllClin = new System.Windows.Forms.CheckBox();
            this.listClin = new OpenDental.UI.ListBoxOD();
            this.labelClin = new System.Windows.Forms.Label();
            this.dateEnd = new System.Windows.Forms.MonthCalendar();
            this.dateStart = new System.Windows.Forms.MonthCalendar();
            this.SuspendLayout();
            // 
            // butCancel
            // 
            this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.butCancel.Location = new System.Drawing.Point(445, 454);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(75, 26);
            this.butCancel.TabIndex = 19;
            this.butCancel.Text = "&Cancel";
            // 
            // butOK
            // 
            this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.butOK.Location = new System.Drawing.Point(349, 454);
            this.butOK.Name = "butOK";
            this.butOK.Size = new System.Drawing.Size(75, 26);
            this.butOK.TabIndex = 18;
            this.butOK.Text = "&OK";
            this.butOK.Click += new System.EventHandler(this.butOK_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(21, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(501, 38);
            this.label1.TabIndex = 20;
            this.label1.Text = "Helps find situations where the insurance payment plus any writeoff exceeds the f" +
    "ee for procedures in the date range.  See the manual for suggestions on how to h" +
    "andle the results.";
            // 
            // radioGroupByProc
            // 
            this.radioGroupByProc.Checked = true;
            this.radioGroupByProc.Location = new System.Drawing.Point(255, 268);
            this.radioGroupByProc.Name = "radioGroupByProc";
            this.radioGroupByProc.Size = new System.Drawing.Size(189, 17);
            this.radioGroupByProc.TabIndex = 21;
            this.radioGroupByProc.TabStop = true;
            this.radioGroupByProc.Text = "Filter results by procedure (default)";
            this.radioGroupByProc.UseVisualStyleBackColor = true;
            // 
            // radioGroupByPat
            // 
            this.radioGroupByPat.Location = new System.Drawing.Point(255, 291);
            this.radioGroupByPat.Name = "radioGroupByPat";
            this.radioGroupByPat.Size = new System.Drawing.Size(160, 48);
            this.radioGroupByPat.TabIndex = 21;
            this.radioGroupByPat.Text = "Filter results by patient and date (will show different results, see manual)";
            this.radioGroupByPat.UseVisualStyleBackColor = true;
            // 
            // checkAllClin
            // 
            this.checkAllClin.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkAllClin.Location = new System.Drawing.Point(82, 249);
            this.checkAllClin.Name = "checkAllClin";
            this.checkAllClin.Size = new System.Drawing.Size(154, 16);
            this.checkAllClin.TabIndex = 57;
            this.checkAllClin.Text = "All (Includes hidden)";
            this.checkAllClin.Click += new System.EventHandler(this.checkAllClin_Click);
            // 
            // listClin
            // 
            this.listClin.Location = new System.Drawing.Point(82, 268);
            this.listClin.Name = "listClin";
            this.listClin.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
            this.listClin.Size = new System.Drawing.Size(154, 212);
            this.listClin.TabIndex = 56;
            this.listClin.Click += new System.EventHandler(this.listClin_Click);
            // 
            // labelClin
            // 
            this.labelClin.Location = new System.Drawing.Point(79, 231);
            this.labelClin.Name = "labelClin";
            this.labelClin.Size = new System.Drawing.Size(104, 16);
            this.labelClin.TabIndex = 55;
            this.labelClin.Text = "Clinics";
            this.labelClin.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // dateEnd
            // 
            this.dateEnd.Location = new System.Drawing.Point(293, 60);
            this.dateEnd.Name = "dateEnd";
            this.dateEnd.TabIndex = 59;
            // 
            // dateStart
            // 
            this.dateStart.Location = new System.Drawing.Point(26, 60);
            this.dateStart.Name = "dateStart";
            this.dateStart.TabIndex = 58;
            // 
            // FormRpInsOverpaid
            // 
            this.AcceptButton = this.butOK;
            this.CancelButton = this.butCancel;
            this.ClientSize = new System.Drawing.Size(545, 496);
            this.Controls.Add(this.dateEnd);
            this.Controls.Add(this.dateStart);
            this.Controls.Add(this.checkAllClin);
            this.Controls.Add(this.listClin);
            this.Controls.Add(this.labelClin);
            this.Controls.Add(this.radioGroupByPat);
            this.Controls.Add(this.radioGroupByProc);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.butCancel);
            this.Controls.Add(this.butOK);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormRpInsOverpaid";
            this.ShowInTaskbar = false;
            this.Text = "Insurance Overpaid Report";
            this.Load += new System.EventHandler(this.FormRpInsOverpaid_Load);
            this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private Label label1;
		private RadioButton radioGroupByProc;
		private RadioButton radioGroupByPat;
		private CheckBox checkAllClin;
		private OpenDental.UI.ListBoxOD listClin;
		private Label labelClin;
		private MonthCalendar dateEnd;
		private MonthCalendar dateStart;
	}
}
