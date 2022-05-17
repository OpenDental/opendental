using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDental.ReportingComplex;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental {
	///<summary></summary>
	public partial class FormRpBrokenAppointments:FormODBase {

		private List<Clinic> _listClinics;
		private List<Def> _listPosAdjTypes=new List<Def>();
		private List<BrokenApptProcedure> _listBrokenProcOptions=new List<BrokenApptProcedure>();
		private List<Provider> _listProviders;
		private bool _hasClinicsEnabled;

		///<summary></summary>
		public FormRpBrokenAppointments() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormRpBrokenAppointments_Load(object sender,EventArgs e) {
			if(PrefC.HasClinicsEnabled) {
				_hasClinicsEnabled=true;
			}
			else {
				_hasClinicsEnabled=false;
			}
			_listProviders=Providers.GetListReports();
			dateStart.SelectionStart=DateTime.Today;
			dateEnd.SelectionStart=DateTime.Today;
			listProvs.Items.AddList(_listProviders,x => x.GetLongDesc());
			if(!_hasClinicsEnabled) {
				listClinics.Visible=false;
				labelClinics.Visible=false;
				checkAllClinics.Visible=false;
			}
			else {
				_listClinics=Clinics.GetForUserod(Security.CurUser);
				if(!Security.CurUser.ClinicIsRestricted) {
					listClinics.Items.Add(Lan.g(this,"Unassigned"));
					listClinics.SetSelected(0);
				}
				for(int i=0;i<_listClinics.Count;i++) {
					listClinics.Items.Add(_listClinics[i].Abbr);
					if(Clinics.ClinicNum==0) {
						listClinics.SetSelected(listClinics.Items.Count-1);
						checkAllClinics.Checked=true;
					}
					if(_listClinics[i].ClinicNum==Clinics.ClinicNum) {
						listClinics.SelectedIndices.Clear();
						listClinics.SetSelected(listClinics.Items.Count-1);
					}
				}
			}
			int value=PrefC.GetInt(PrefName.BrokenApptProcedure);
			if(value==(int)BrokenApptProcedure.None) {//
				radioProcs.Visible=false;
			}
			if(value>0){
				radioProcs.Checked=true;
			}
			else if(PrefC.GetBool(PrefName.BrokenApptAdjustment)) {
				radioAdj.Checked=true;
			}
			else {
				radioAptStatus.Checked=true;
			}
		}

		private void checkAllProvs_Click(object sender,EventArgs e) {
			if(checkAllProvs.Checked) {
				listProvs.ClearSelected();
			}
		}

		private void checkAllClinics_Click(object sender,EventArgs e) {
			if(checkAllClinics.Checked) {
				listClinics.SetAll(true);
			}
			else {
				listClinics.ClearSelected();
			}
		}

		private void listProvs_Click(object sender,EventArgs e) {
			if(listProvs.SelectedIndices.Count>0) {
				checkAllProvs.Checked=false;
			}
		}

		private void listClinics_Click(object sender,EventArgs e) {
			if(listClinics.SelectedIndices.Count>0) {
				checkAllClinics.Checked=false;
			}
		}

		private void radioProcs_CheckedChanged(object sender,EventArgs e) {
			if(radioProcs.Checked) {
				listOptions.Items.Clear();
				listOptions.SelectionMode=OpenDental.UI.SelectionMode.One;
				_listBrokenProcOptions.Clear();
				BrokenApptProcedure brokenApptCodeDB=(BrokenApptProcedure)PrefC.GetInt(PrefName.BrokenApptProcedure);
				switch(brokenApptCodeDB) {
					case BrokenApptProcedure.None:
					case BrokenApptProcedure.Missed:
						_listBrokenProcOptions.Add(BrokenApptProcedure.Missed);
						listOptions.Items.Add(Lans.g(this,brokenApptCodeDB.ToString())+": (D9986)");
						labelDescr.Text=Lan.g(this,"Broken appointments based on ADA code D9986");
					break;
					case BrokenApptProcedure.Cancelled:
						_listBrokenProcOptions.Add(BrokenApptProcedure.Cancelled);
						listOptions.Items.Add(Lans.g(this,brokenApptCodeDB.ToString())+": (D9987)");
						labelDescr.Text=Lan.g(this,"Broken appointments based on ADA code D9987");
					break;
					case BrokenApptProcedure.Both:
						_listBrokenProcOptions.Add(BrokenApptProcedure.Missed);
						_listBrokenProcOptions.Add(BrokenApptProcedure.Cancelled);
						_listBrokenProcOptions.Add(BrokenApptProcedure.Both);
						listOptions.Items.Add(Lans.g(this,BrokenApptProcedure.Missed.ToString())+": (D9986)");
						listOptions.Items.Add(Lans.g(this,BrokenApptProcedure.Cancelled.ToString())+": (D9987)");
						listOptions.Items.Add(Lans.g(this,brokenApptCodeDB.ToString()));
						labelDescr.Text=Lan.g(this,"Broken appointments based on ADA code D9986 or D9987");
					break;
				}
				//Can't be negative since 'None' would not show this ListBox and Missed/Cancelled/Both will always have at least one item.
				listOptions.SetSelected(listOptions.Items.Count-1);
				listOptions.Visible=true;
			}
		}

		private void radioAdj_CheckedChanged(object sender,EventArgs e) {
			if(radioAdj.Checked) {
				labelDescr.Text=Lan.g(this,"Broken appointments based on broken appointment adjustments");
				listOptions.Items.Clear();
				_listPosAdjTypes.Clear();
				listOptions.SelectionMode=OpenDental.UI.SelectionMode.MultiExtended;
				_listPosAdjTypes=Defs.GetPositiveAdjTypes();
				long brokenApptAdjDefNum=PrefC.GetLong(PrefName.BrokenAppointmentAdjustmentType);
				for(int i=0; i<_listPosAdjTypes.Count;i++) {
					listOptions.Items.Add(_listPosAdjTypes[i].ItemName);
					if(_listPosAdjTypes[i].DefNum==brokenApptAdjDefNum) {
						listOptions.SetSelected(i);
					}
				}
				listOptions.Visible=true;
			}
		}

		private void radioAptStatus_CheckedChanged(object sender,EventArgs e) {
			if(radioAptStatus.Checked) {
				labelDescr.Text=Lan.g(this,"Broken appointments based on appointment status");
				listOptions.Items.Clear();
				listOptions.Visible=false;
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!checkAllProvs.Checked && listProvs.SelectedIndices.Count==0) {
				MsgBox.Show(this,"At least one provider must be selected.");
				return;
			}
			if(_hasClinicsEnabled) {
				if(!checkAllClinics.Checked && listClinics.SelectedIndices.Count==0) {
					MsgBox.Show(this,"At least one clinic must be selected.");
					return;
				}
			}
			if(radioAdj.Checked && listOptions.SelectedIndices.Count==0) {
				MsgBox.Show(this,"At least one adjustment type must be selected.");
				return;
			}
			if(radioProcs.Checked && listOptions.SelectedIndices.Count==0) {
				MsgBox.Show(this,"At least one procedure code option must be selected.");
				return;
			}
			List<long> listClinicNums=new List<long>();
			for(int i=0;i<listClinics.SelectedIndices.Count;i++) {
				if(Security.CurUser.ClinicIsRestricted) {
						listClinicNums.Add(_listClinics[listClinics.SelectedIndices[i]].ClinicNum);//we know that the list is a 1:1 to _listClinics
					}
				else {
					if(listClinics.SelectedIndices[i]==0) {
						listClinicNums.Add(0);
					}
					else {
						listClinicNums.Add(_listClinics[listClinics.SelectedIndices[i]-1].ClinicNum);//Minus 1 from the selected index
					}
				}
			}
			if(checkAllClinics.Checked) {//Add all hidden unrestricted clinics to the list
				listClinicNums=listClinicNums.Union(Clinics.GetAllForUserod(Security.CurUser).Select(x => x.ClinicNum)).ToList();
			}
			List<long> listProvNums=new List<long>();
			if(checkAllProvs.Checked) {
				for(int i = 0;i<_listProviders.Count;i++) {
					listProvNums.Add(_listProviders[i].ProvNum);
				}
			}
			else {
				for(int i=0;i<listProvs.SelectedIndices.Count;i++) {
				listProvNums.Add(_listProviders[listProvs.SelectedIndices[i]].ProvNum);
				}
			}
			List<long> listAdjDefNums=new List<long>();
			if(radioAdj.Checked) {
				for(int i=0;i<listOptions.SelectedIndices.Count;i++) {
					listAdjDefNums.Add(_listPosAdjTypes[listOptions.SelectedIndices[i]].DefNum);
				}
			}
			BrokenApptProcedure brokenApptSelection=BrokenApptProcedure.None;
			if(radioProcs.Checked) {
				brokenApptSelection=_listBrokenProcOptions[listOptions.SelectedIndex];
			}
			ReportComplex reportComplex=new ReportComplex(true,false);
			DataTable table = new DataTable();
			table=RpBrokenAppointments.GetBrokenApptTable(dateStart.SelectionStart,dateEnd.SelectionStart,listProvNums,listClinicNums,listAdjDefNums,brokenApptSelection
				,checkAllClinics.Checked,radioProcs.Checked,radioAptStatus.Checked,radioAdj.Checked,_hasClinicsEnabled);
			string subtitleProvs="";
			string subtitleClinics="";
			if(checkAllProvs.Checked) {
				subtitleProvs=Lan.g(this,"All Providers");
			}
			else {
				for(int i=0;i<listProvs.SelectedIndices.Count;i++) {
					if(i>0) {
						subtitleProvs+=", ";
					}
					subtitleProvs+=_listProviders[listProvs.SelectedIndices[i]].Abbr;
				}
			}
			if(_hasClinicsEnabled) {
				if(checkAllClinics.Checked) {
					subtitleClinics=Lan.g(this,"All Clinics");
				}
				else {
					for(int i=0;i<listClinics.SelectedIndices.Count;i++) {
						if(i>0) {
							subtitleClinics+=", ";
						}
						if(Security.CurUser.ClinicIsRestricted) {
							subtitleClinics+=_listClinics[listClinics.SelectedIndices[i]].Abbr;
						}
						else {
							if(listClinics.SelectedIndices[i]==0) {
								subtitleClinics+=Lan.g(this,"Unassigned");
							}
							else {
								subtitleClinics+=_listClinics[listClinics.SelectedIndices[i]-1].Abbr;//Minus 1 from the selected index
							}
						}
					}
				}
			}
			Font font=new Font("Tahoma",10);
			Font fontBold=new Font("Tahoma",10,FontStyle.Bold);
			Font fontTitle=new Font("Tahoma",17,FontStyle.Bold);
			Font fontSubTitle=new Font("Tahoma",11,FontStyle.Bold);
			reportComplex.ReportName=Lan.g(this,"Broken Appointments");
			reportComplex.AddTitle("Title",Lan.g(this,"Broken Appointments"),fontTitle);
			if(radioProcs.Checked) {//Report looking at ADA procedure code D9986
				string codes="";
				switch(brokenApptSelection) {
					case BrokenApptProcedure.None:
					case BrokenApptProcedure.Missed:
						codes="D9986";
					break;
					case BrokenApptProcedure.Cancelled:
						codes="D9987";
					break;
					case BrokenApptProcedure.Both:
						codes="D9986 or D9987";
					break;
				}
				reportComplex.AddSubTitle("Report Description",Lan.g(this,"By ADA Code "+codes),fontSubTitle);
			}
			else if(radioAdj.Checked) {//Report looking at broken appointment adjustments
				reportComplex.AddSubTitle("Report Description",Lan.g(this,"By Broken Appointment Adjustment"),fontSubTitle);
			}
			else {//Report looking at appointments with a status of 'Broken'
				reportComplex.AddSubTitle("Report Description",Lan.g(this,"By Appointment Status"),fontSubTitle);
			}
			reportComplex.AddSubTitle("Providers",subtitleProvs,fontSubTitle);
			reportComplex.AddSubTitle("Clinics",subtitleClinics,fontSubTitle);
			QueryObject queryObject;
			if(PrefC.HasClinicsEnabled) {//Split the query up by clinics.
				queryObject=reportComplex.AddQuery(table,Lan.g(this,"Date")+": "+DateTime.Today.ToString("d"),"ClinicDesc",SplitByKind.Value,0,true);
			}
			else {
				queryObject=reportComplex.AddQuery(table,Lan.g(this,"Date")+": "+DateTime.Today.ToString("d"),"",SplitByKind.None,0,true);
			}
			//Add columns to report
			if(radioProcs.Checked) {//Report looking at ADA procedure code D9986 or D9987
				queryObject.AddColumn(Lan.g(this,"Date"),85,FieldValueType.Date,font);
				queryObject.AddColumn(Lan.g(this,"Provider"),180,FieldValueType.String,font);
				if(brokenApptSelection==BrokenApptProcedure.Both) {
					queryObject.AddColumn(Lan.g(this,"Code"),75,FieldValueType.String,font);
				}
				queryObject.AddColumn(Lan.g(this,"Patient"),220,FieldValueType.String,font);
				queryObject.AddColumn(Lan.g(this,"Fee"),200,FieldValueType.Number,font);
				queryObject.AddGroupSummaryField(Lan.g(this,"Total Broken Appointment Fees")+":",Lan.g(this,"Fee"),"ProcFee",SummaryOperation.Sum,
					font:fontBold,offSetX:0,offSetY:10);
				queryObject.AddGroupSummaryField(Lan.g(this,"Total Broken Appointments")+":",Lan.g(this,"Fee"),"ProcFee",SummaryOperation.Count,
					font:fontBold,offSetX:0,offSetY:10);
			}
			else if(radioAdj.Checked) {//Report looking at broken appointment adjustments
				queryObject.AddColumn(Lan.g(this,"Date"),85,FieldValueType.Date,font);
				queryObject.AddColumn(Lan.g(this,"Provider"),100,FieldValueType.String,font);
				queryObject.AddColumn(Lan.g(this,"Patient"),220,FieldValueType.String,font);
				queryObject.AddColumn(Lan.g(this,"Amount"),80,FieldValueType.Number,font);
				queryObject.AddColumn(Lan.g(this,"Note"),300,FieldValueType.String,font);
				queryObject.AddGroupSummaryField(Lan.g(this,"Total Broken Appointment Adjustment Amount")+":",
					Lan.g(this,"Amount"),"AdjAmt",SummaryOperation.Sum,font:fontBold,offSetX:0,offSetY:10);
				queryObject.AddGroupSummaryField(Lan.g(this,"Total Broken Appointments")+":",
					Lan.g(this,"Amount"),"AdjAmt",SummaryOperation.Count,font:fontBold,offSetX:0,offSetY:10);
			}
			else {//Report looking at appointments with a status of 'Broken'
				queryObject.AddColumn(Lan.g(this,"AptDate"),85,FieldValueType.Date,font);
				queryObject.AddColumn(Lan.g(this,"Patient"),220,FieldValueType.String,font);
				queryObject.AddColumn(Lan.g(this,"Doctor"),165,FieldValueType.String,font);
				queryObject.AddColumn(Lan.g(this,"Hygienist"),165,FieldValueType.String,font);
				queryObject.AddColumn(Lan.g(this,"IsHyg"),50,FieldValueType.Boolean,font);
				queryObject.GetColumnDetail(Lan.g(this,"IsHyg")).ContentAlignment = ContentAlignment.MiddleCenter;
				queryObject.AddGroupSummaryField(Lan.g(this,"Total Broken Appointments")+":",Lan.g(this,"IsHyg"),"AptDateTime",SummaryOperation.Count,
					font:fontBold,offSetX:0,offSetY:10);
			}
			queryObject.ContentAlignment=ContentAlignment.MiddleRight;
			reportComplex.AddPageNum(font);
			//execute query
			if(!reportComplex.SubmitQueries()) {
				return;
			}
			//display report
			using FormReportComplex formReportComplex=new FormReportComplex(reportComplex);
			//FormR.MyReport=report;
			formReportComplex.ShowDialog();
			font.Dispose();
			fontBold.Dispose();
			fontTitle.Dispose();
			fontSubTitle.Dispose();
			DialogResult=DialogResult.OK;
		}

	}
}