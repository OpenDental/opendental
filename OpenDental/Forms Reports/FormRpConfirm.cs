using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
///<summary></summary>
	public class FormRpConfirm : FormODBase{
		private System.Windows.Forms.Label labelPatient;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Label label1;
		private OpenDental.UI.ListBoxOD listSelect;
		private OpenDental.UI.ListBoxOD listSelect2;
		private long[] AptNums;
  
		///<summary></summary>
		public FormRpConfirm(long[] aptNums) {
			InitializeComponent();
			InitializeLayoutManager();
			AptNums=(long[])aptNums.Clone();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpConfirm));
			this.labelPatient = new System.Windows.Forms.Label();
			this.listSelect = new OpenDental.UI.ListBoxOD();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.listSelect2 = new OpenDental.UI.ListBoxOD();
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
			// listSelect
			// 
			this.listSelect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listSelect.Location = new System.Drawing.Point(12,30);
			this.listSelect.Name = "listSelect";
			this.listSelect.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listSelect.Size = new System.Drawing.Size(170,355);
			this.listSelect.TabIndex = 5;
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
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
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
			// listSelect2
			// 
			this.listSelect2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listSelect2.Location = new System.Drawing.Point(208,30);
			this.listSelect2.Name = "listSelect2";
			this.listSelect2.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listSelect2.Size = new System.Drawing.Size(170,355);
			this.listSelect2.TabIndex = 9;
			// 
			// FormRpConfirm
			// 
			this.AcceptButton = this.butOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5,13);
			this.ClientSize = new System.Drawing.Size(538,408);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listSelect2);
			this.Controls.Add(this.labelPatient);
			this.Controls.Add(this.listSelect);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormRpConfirm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Confirmation Report";
			this.Load += new System.EventHandler(this.FormRpConfirm_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void FormRpConfirm_Load(object sender, System.EventArgs e){
			listSelect.Items.Add("LName");
			listSelect.Items.Add("FName"); 
			listSelect.Items.Add("MiddleI");
      listSelect.Items.Add("Preferred");
			listSelect.Items.Add("Salutation"); 
      listSelect.Items.Add("Address"); 
      listSelect.Items.Add("Address2");
			listSelect.Items.Add("City"); 
			listSelect.Items.Add("State");
			listSelect.Items.Add("Zip");
			listSelect.Items.Add("HmPhone");
			listSelect.Items.Add("WkPhone"); 
			listSelect.Items.Add("WirelessPhone"); 
			listSelect.Items.Add("Birthdate");
			listSelect.Items.Add("AptDateTime");
			listSelect.SetAll(true);
      listSelect2.Items.Add("BillingType");      
      listSelect2.Items.Add("CreditType");
			listSelect2.Items.Add("SSN");  
			listSelect2.Items.Add("ChartNumber");   
			listSelect2.Items.Add("FeeSched");
			listSelect2.Items.Add("Position"); 
			listSelect2.Items.Add("Gender");
			listSelect2.Items.Add("PatStatus");
			listSelect2.Items.Add("PatNum"); 
      listSelect2.Items.Add("Email");
      listSelect2.Items.Add("EstBalance"); 
      listSelect2.Items.Add("AddrNote"); 
      listSelect2.Items.Add("FamFinUrgNote"); 
      listSelect2.Items.Add("MedUrgNote"); 
			listSelect2.Items.Add("ApptModNote");
      listSelect2.Items.Add("PriCarrier");
      listSelect2.Items.Add("PriRelationship");
			listSelect2.Items.Add("SecCarrier");
			listSelect2.Items.Add("SecRelationship");
			//listSelect2.Items.Add("RecallInterval"); 
			listSelect2.Items.Add("StudentStatus");
      listSelect2.Items.Add("SchoolName"); 
			listSelect2.Items.Add("PriProv"); 
      listSelect2.Items.Add("SecProv");
			//listSelect2.Items.Add("NextAptNum"); 
			listSelect2.Items.Add("Guarantor"); 
			listSelect2.Items.Add("ImageFolder");
			
 		}

		private void butOK_Click(object sender, System.EventArgs e) {
			List<string> fieldsSelected=new List<string>(); 
			if(listSelect.SelectedIndices.Count==0 && listSelect2.SelectedIndices.Count==0){
				MsgBox.Show(this,"At least one field must be selected.");
				return;
			}
			for(int i=0;i<listSelect.SelectedIndices.Count;i++) {
				fieldsSelected.Add(listSelect.Items.GetTextShowingAt(listSelect.SelectedIndices[i]));
			}
			for(int i = 0;i<listSelect2.SelectedIndices.Count;i++) {
				fieldsSelected.Add(listSelect2.Items.GetTextShowingAt(listSelect2.SelectedIndices[i]));
			}
			string command="SELECT ";
			for(int i=0;i<fieldsSelected.Count;i++){
				if(i>0){
					command+=",";
				}
				if(fieldsSelected[i]=="AptDateTime"){
					command+="appointment.AptDateTime";
				}
				else if(fieldsSelected[i]=="PriCarrier") {
					command+="(SELECT carrier.CarrierName FROM patplan,inssub,insplan,carrier WHERE patplan.PatNum=patient.PatNum "
						+"AND patplan.InsSubNum=inssub.InsSubNum AND inssub.PlanNum=insplan.PlanNum AND insplan.CarrierNum=carrier.CarrierNum AND patplan.Ordinal=1 "+DbHelper.LimitAnd(1)+") PriCarrier";
				}
				else if(fieldsSelected[i]=="PriRelationship") {
					command+="(SELECT Relationship FROM patplan WHERE patplan.PatNum=patient.PatNum AND patplan.Ordinal=1 "+DbHelper.LimitAnd(1)+") PriRelationship";
				}
				else if(fieldsSelected[i]=="SecCarrier") {
					command+="(SELECT carrier.CarrierName FROM patplan,inssub,insplan,carrier WHERE patplan.PatNum=patient.PatNum "
						+"AND patplan.InsSubNum=inssub.InsSubNum AND inssub.PlanNum=insplan.PlanNum AND insplan.CarrierNum=carrier.CarrierNum AND patplan.Ordinal=2 "+DbHelper.LimitAnd(1)+") SecCarrier";
				}
				else if(fieldsSelected[i]=="SecRelationship") {
					command+="(SELECT Relationship FROM patplan WHERE patplan.PatNum=patient.PatNum AND patplan.Ordinal=2 "+DbHelper.LimitAnd(1)+") SecRelationship";
				}
				else{
					command+="patient."+fieldsSelected[i];
				}
			}
			command+=" FROM patient,appointment "
				+"WHERE patient.PatNum=appointment.PatNum AND(";       
			for(int i=0;i<AptNums.Length;i++){
				if(i>0){ 
					command+=" OR";
				}
				command+=" appointment.AptNum='"+AptNums[i]+"'";
			}
			command+=")";
			ReportSimpleGrid report=new ReportSimpleGrid();
			report.Query=command;
			using FormQuery FormQ=new FormQuery(report);
			FormQ.IsReport=false;
			FormQ.SubmitQuery();	
      FormQ.textQuery.Text=report.Query;					
			FormQ.ShowDialog();		
      DialogResult=DialogResult.OK; 
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		

	}
}
