using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;
using CodeBase;
using System.Collections.Generic;

namespace OpenDental{
	/// <summary>Only used when adding an insurance plan from ContrFamily.  Lets user select the subscriber from </summary>
	public class FormSubscriberSelect : FormODBase {
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.ListBoxOD listPats;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private OpenDental.UI.Button butMore;
		private Family FamCur;
		private System.Windows.Forms.Label label1;

		///<summary>When the form closes with OK, this will contain the patient num selected.</summary>
		public long SelectedPatNum;
		private List<PatientLink> _listMergeLinks;

		///<summary></summary>
		public FormSubscriberSelect(Family famCur)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			FamCur=famCur;
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSubscriberSelect));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.listPats = new OpenDental.UI.ListBoxOD();
			this.butMore = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(277,241);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(92,26);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(277,200);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(92,26);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// listPats
			// 
			this.listPats.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listPats.Location = new System.Drawing.Point(32,79);
			this.listPats.Name = "listPats";
			this.listPats.Size = new System.Drawing.Size(187,186);
			this.listPats.TabIndex = 2;
			this.listPats.DoubleClick += new System.EventHandler(this.listPats_DoubleClick);
			// 
			// butMore
			// 
			this.butMore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butMore.Location = new System.Drawing.Point(277,79);
			this.butMore.Name = "butMore";
			this.butMore.Size = new System.Drawing.Size(92,26);
			this.butMore.TabIndex = 3;
			this.butMore.Text = "More Patients";
			this.butMore.Click += new System.EventHandler(this.butMore_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(33,8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(287,38);
			this.label1.TabIndex = 4;
			this.label1.Text = "If subscriber has not been entered, cancel and add them before continuing.";
			// 
			// FormSubscriberSelect
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5,13);
			this.ClientSize = new System.Drawing.Size(401,295);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butMore);
			this.Controls.Add(this.listPats);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormSubscriberSelect";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Select Subscriber";
			this.Load += new System.EventHandler(this.FormSubscriberSelect_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void FormSubscriberSelect_Load(object sender, System.EventArgs e) {
			_listMergeLinks=PatientLinks.GetLinks(FamCur.ListPats.Select(x => x.PatNum).ToList(),PatientLinkType.Merge);
			FillList();
		}

		private void FillList() {
			listPats.Items.Clear();
			for(int i=0;i<FamCur.ListPats.Length;i++) {
				if(PatientLinks.WasPatientMerged(FamCur.ListPats[i].PatNum,_listMergeLinks)) {
					continue;//Don't show merged patients
				}
				listPats.Items.Add(FamCur.ListPats[i].GetNameFL()+" ("+Lan.g("enumPatientStatus",
					FamCur.ListPats[i].PatStatus.GetDescription())+")",FamCur.ListPats[i]);
			}
		}
		
		private void listPats_DoubleClick(object sender, System.EventArgs e) {
			if(listPats.SelectedIndex==-1){
				return;
			}
			SelectedPatNum=listPats.GetSelected<Patient>().PatNum;
			DialogResult=DialogResult.OK;
		}

		private void butMore_Click(object sender, System.EventArgs e) {
			using FormPatientSelect FormP=new FormPatientSelect();
			FormP.SelectionModeOnly=true;
			FormP.ShowDialog();
			if(FormP.DialogResult!=DialogResult.OK){
				return;
			}
			SelectedPatNum=FormP.SelectedPatNum;
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(listPats.SelectedIndex==-1){
				MsgBox.Show(this,"Please pick a patient first.");
				return;
			}
			SelectedPatNum=listPats.GetSelected<Patient>().PatNum;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}





















