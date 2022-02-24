using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	///<summary></summary>
	public class FormRpRecall : FormODBase {
		private System.Windows.Forms.Label labelPatient;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.ComponentModel.Container components = null;
		private string SQLselect;
		private string SQLfrom;
	private string SQLwhere;
	private string SQLstatement;
		private System.Windows.Forms.Label label1;
		private OpenDental.UI.ListBoxOD listPatientSelect2;
		private OpenDental.UI.ListBoxOD listPatientSelect;
		private List<long> RecallNumList;

		///<summary></summary>
		public FormRpRecall(){
			InitializeComponent();
			InitializeLayoutManager();
			SQLselect="";
			SQLfrom="FROM patient "; 
			SQLwhere="WHERE ";
			FillPatientSelectList(); 
			Lan.F(this); 
		}

		///<summary></summary>
		public FormRpRecall(List<long> recallNumList) {
			InitializeComponent();
			InitializeLayoutManager();
			RecallNumList=new List<long>(recallNumList);
			SQLselect="";
			SQLfrom="FROM patient ";
			SQLwhere="WHERE ";
			FillPatientSelectList();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpRecall));
			this.labelPatient = new System.Windows.Forms.Label();
			this.listPatientSelect = new OpenDental.UI.ListBoxOD();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.listPatientSelect2 = new OpenDental.UI.ListBoxOD();
			this.SuspendLayout();
			// 
			// labelPatient
			// 
			this.labelPatient.Location = new System.Drawing.Point(12,16);
			this.labelPatient.Name = "labelPatient";
			this.labelPatient.Size = new System.Drawing.Size(170,14);
			this.labelPatient.TabIndex = 6;
			this.labelPatient.Text = "Standard Fields";
			this.labelPatient.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// listPatientSelect
			// 
			this.listPatientSelect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left)));
			this.listPatientSelect.Location = new System.Drawing.Point(12,30);
			this.listPatientSelect.Name = "listPatientSelect";
			this.listPatientSelect.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listPatientSelect.Size = new System.Drawing.Size(170,355);
			this.listPatientSelect.TabIndex = 5;
			this.listPatientSelect.SelectedIndexChanged += new System.EventHandler(this.ListPatientSelect_SelectedIndexChanged);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(434,362);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75,26);
			this.butCancel.TabIndex = 8;
			this.butCancel.Text = "&Cancel";
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Enabled = false;
			this.butOK.Location = new System.Drawing.Point(434,326);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75,26);
			this.butOK.TabIndex = 7;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(208,16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(170,14);
			this.label1.TabIndex = 10;
			this.label1.Text = "Other Available Fields";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// listPatientSelect2
			// 
			this.listPatientSelect2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left)));
			this.listPatientSelect2.Location = new System.Drawing.Point(208,30);
			this.listPatientSelect2.Name = "listPatientSelect2";
			this.listPatientSelect2.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listPatientSelect2.Size = new System.Drawing.Size(170,355);
			this.listPatientSelect2.TabIndex = 9;
			this.listPatientSelect2.SelectedIndexChanged += new System.EventHandler(this.listPatientSelect2_SelectedIndexChanged);
			// 
			// FormRpRecall
			// 
			this.AcceptButton = this.butOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5,13);
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(538,408);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listPatientSelect2);
			this.Controls.Add(this.labelPatient);
			this.Controls.Add(this.listPatientSelect);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormRpRecall";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Recall Report";
			this.ResumeLayout(false);

		}
		#endregion
 
		private void FillPatientSelectList(){ 
			listPatientSelect.Items.Add(Lan.g(this,"LName"));
			listPatientSelect.Items.Add(Lan.g(this,"FName"));
			listPatientSelect.Items.Add(Lan.g(this,"MiddleI"));
			listPatientSelect.Items.Add(Lan.g(this,"Preferred"));
			listPatientSelect.Items.Add(Lan.g(this,"Salutation"));
			listPatientSelect.Items.Add(Lan.g(this,"Address")); 
			listPatientSelect.Items.Add(Lan.g(this,"Address2"));
			listPatientSelect.Items.Add(Lan.g(this,"City")); 
			listPatientSelect.Items.Add(Lan.g(this,"State"));
			listPatientSelect.Items.Add(Lan.g(this,"Zip"));
			listPatientSelect.Items.Add(Lan.g(this,"HmPhone"));
			listPatientSelect.Items.Add(Lan.g(this,"WkPhone")); 
			listPatientSelect.Items.Add(Lan.g(this,"WirelessPhone"));
			listPatientSelect.Items.Add(Lan.g(this,"Birthdate"));
			listPatientSelect.Items.Add(Lan.g(this,"RecallStatus"));
			listPatientSelect.Items.Add(Lan.g(this,"DateDue"));
			listPatientSelect.SetAll(true);
			listPatientSelect2.Items.Add(Lan.g(this,"BillingType"));
			listPatientSelect2.Items.Add(Lan.g(this,"CreditType"));
			listPatientSelect2.Items.Add(Lan.g(this,"SSN"));
			listPatientSelect2.Items.Add(Lan.g(this,"ChartNumber"));
			listPatientSelect2.Items.Add(Lan.g(this,"FeeSched"));
			listPatientSelect2.Items.Add(Lan.g(this,"Position")); 
			listPatientSelect2.Items.Add(Lan.g(this,"Gender"));
			listPatientSelect2.Items.Add(Lan.g(this,"PatStatus"));
			listPatientSelect2.Items.Add(Lan.g(this,"PatNum")); 
			listPatientSelect2.Items.Add(Lan.g(this,"Email"));
			listPatientSelect2.Items.Add(Lan.g(this,"EstBalance")); 
			listPatientSelect2.Items.Add(Lan.g(this,"AddrNote"));
			listPatientSelect2.Items.Add(Lan.g(this,"FamFinUrgNote"));
			listPatientSelect2.Items.Add(Lan.g(this,"MedUrgNote"));
			listPatientSelect2.Items.Add(Lan.g(this,"ApptModNote"));
			//listPatientSelect2.Items.Add(Lan.g(this,"PriPlanNum"));//Primary Carrier?
			//listPatientSelect2.Items.Add(Lan.g(this,"PriRelationship"));// ?
			//listPatientSelect2.Items.Add(Lan.g(this,"SecPlanNum"));//Secondary Carrier? 
			//listPatientSelect2.Items.Add(Lan.g(this,"SecRelationship"));// ?
			listPatientSelect2.Items.Add(Lan.g(this,"RecallInterval"));
			listPatientSelect2.Items.Add(Lan.g(this,"StudentStatus"));
			listPatientSelect2.Items.Add(Lan.g(this,"SchoolName"));
			listPatientSelect2.Items.Add(Lan.g(this,"PriProv"));
			listPatientSelect2.Items.Add(Lan.g(this,"SecProv"));
			//listPatientSelect2.Items.Add(Lan.g(this,"NextAptNum"));
			listPatientSelect2.Items.Add(Lan.g(this,"Guarantor"));
			listPatientSelect2.Items.Add(Lan.g(this,"ImageFolder"));
		}

		private void FillSQLstatement(){
			SQLstatement=SQLselect+SQLfrom+SQLwhere;
		}

	private void CreateSQL(){
		CreateSQLselect();
		CreateSQLfrom();
		CreateSQLwhere();
		FillSQLstatement();
	}

	private void CreateSQLselect(){
		SQLselect="";
		if(listPatientSelect.SelectedIndices.Count==0 && listPatientSelect2.SelectedIndices.Count==0){
				butOK.Enabled=false;
				return;
			}
			SQLselect="SELECT ";
			List<string> PatFieldsSelected=new List<string>();
			for(int i=0;i<listPatientSelect.SelectedIndices.Count;i++) {
				PatFieldsSelected.Add(listPatientSelect.Items.GetTextShowingAt(listPatientSelect.SelectedIndices[i]));
			}
			for(int i=0;i<listPatientSelect2.SelectedIndices.Count;i++) {
				PatFieldsSelected.Add(listPatientSelect2.Items.GetTextShowingAt(listPatientSelect2.SelectedIndices[i]));
			}
			for(int i=0;i<PatFieldsSelected.Count;i++){
				if(i>0){
					SQLselect+=",";
				}
				if(PatFieldsSelected[i]=="RecallInterval"){
					SQLselect+="ROUND(recall.RecallInterval/65536) AS MonthInterval";
					//returns the months.  It will malfunction if a year is present
				}
				else if(PatFieldsSelected[i]=="DateDue"
					|| PatFieldsSelected[i]=="RecallStatus"){
					SQLselect+="recall."+PatFieldsSelected[i];
				}
				else{
					SQLselect+="patient."+PatFieldsSelected[i];
				}
			}
			butOK.Enabled=true;
		}

	private void CreateSQLfrom(){ 
	  SQLfrom=" FROM patient,recall ";
	}

	private void CreateSQLwhere() {
	  SQLwhere="WHERE patient.PatNum=recall.PatNum ";
			if(RecallNumList.Count>0) {
				SQLwhere+=$"AND recall.RecallNum IN({string.Join(",",RecallNumList.Select(x => POut.Long(x)))})";
			}
	}

		private void ListPatientSelect_SelectedIndexChanged(object sender, System.EventArgs e) {
			CreateSQL();
		}

		private void listPatientSelect2_SelectedIndexChanged(object sender, System.EventArgs e) {
			CreateSQL();
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			ReportSimpleGrid report=new ReportSimpleGrid();
			report.Query=SQLstatement;
			using FormQuery FormQuery2=new FormQuery(report);
			FormQuery2.IsReport=false;
			FormQuery2.SubmitQuery();	
			FormQuery2.textQuery.Text=report.Query;
			FormQuery2.ShowDialog();
			DialogResult=DialogResult.OK; 
		}

		

	}
}
