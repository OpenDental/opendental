using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;

namespace OpenDental{
///<summary></summary>
	public class FormSchoolClasses : FormODBase {
		private OpenDental.UI.Button butClose;
		private OpenDental.UI.Button butAdd;
		private System.ComponentModel.Container components = null;
		private OpenDental.UI.ListBoxOD listMain;
		private List<SchoolClass> _listSchoolCasses;

		///<summary></summary>
		public FormSchoolClasses(){
			InitializeComponent();
			InitializeLayoutManager();
			//Providers.Selected=-1;
			Lan.F(this);
		}

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSchoolClasses));
			this.listMain = new OpenDental.UI.ListBoxOD();
			this.butClose = new OpenDental.UI.Button();
			this.butAdd = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// listMain
			// 
			this.listMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listMain.Location = new System.Drawing.Point(16, 12);
			this.listMain.Name = "listMain";
			this.listMain.Size = new System.Drawing.Size(265, 381);
			this.listMain.TabIndex = 4;
			this.listMain.DoubleClick += new System.EventHandler(this.listMain_DoubleClick);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(209, 417);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 26);
			this.butClose.TabIndex = 3;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(16, 416);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(79, 26);
			this.butAdd.TabIndex = 10;
			this.butAdd.Text = "&Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// FormSchoolClasses
			// 
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(300, 459);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.listMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormSchoolClasses";
			this.ShowInTaskbar = false;
			this.Text = "Dental School Classes";
			this.Load += new System.EventHandler(this.FormSchoolClasses_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void FormSchoolClasses_Load(object sender, System.EventArgs e) {
			_listSchoolCasses=SchoolClasses.GetDeepCopy();
			FillList();
		}

		private void FillList(){
			listMain.Items.Clear();
			for(int i=0;i<_listSchoolCasses.Count;i++){
				listMain.Items.Add(_listSchoolCasses[i].GradYear.ToString()+" - "+_listSchoolCasses[i].Descript);
			}
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			SchoolClass cur=new SchoolClass();
			using FormSchoolClassEdit FormS=new FormSchoolClassEdit(cur);
			FormS.IsNew=true;
			FormS.ShowDialog();
			if(FormS.DialogResult!=DialogResult.OK){
				return;
			}
			_listSchoolCasses=SchoolClasses.GetDeepCopy();
			FillList();
			listMain.SelectedIndex=-1;
		}

		private void listMain_DoubleClick(object sender, System.EventArgs e) {
			if(listMain.SelectedIndex==-1) {
				return;
			}
			using FormSchoolClassEdit FormS=new FormSchoolClassEdit(_listSchoolCasses[listMain.SelectedIndex]);
			FormS.ShowDialog();
			if(FormS.DialogResult!=DialogResult.OK){
				return;
			}
			_listSchoolCasses=SchoolClasses.GetDeepCopy();
			FillList();
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}
	}
}
