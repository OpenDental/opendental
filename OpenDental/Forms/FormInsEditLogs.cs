using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormInsEditLogs:FormODBase {
		private InsPlan _insPlan;
		private List<Benefit> _listBenefits;
		private List<InsEditLog> _listInsEditLogs;

		///<summary>Opens the window with the passed-in parameters set as the default.</summary>
		public FormInsEditLogs(InsPlan insPlan, List<Benefit> listBenefits) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_insPlan=insPlan;
			_listBenefits=listBenefits;
		}

		private void FormInsEditLogs_Load(object sender,EventArgs e) {
			SetFilterControlsAndAction(() => FillGrid(),
				(int)TimeSpan.FromSeconds(0.5).TotalMilliseconds,
				textDateFrom,textDateTo);
			textDateFrom.Text=DateTime.Now.AddMonths(-1).ToString();
			textDateTo.Text=DateTime.Now.ToString();//Triggers the query via event handler, will cause FillGrid().
			//Need to refill grid to get the vertical scroll bar to show up properly. It's annoying, but doesn't do a Db call, so not super terrible.
			FillGrid();
		}

		private void FillGrid() {
			if(_listInsEditLogs==null) {
				Cursor=Cursors.WaitCursor;
				_listInsEditLogs=InsEditLogs.GetLogsForPlan(_insPlan.PlanNum,_insPlan.CarrierNum,_insPlan.EmployerNum);
				TranslateBeforeAndAfter();
				Cursor=Cursors.Default;
			}
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			gridMain.Columns.Add(new GridColumn("",25)); //for the drop down arrows.
			gridMain.Columns.Add(new GridColumn("Log Date",135));
			gridMain.Columns.Add(new GridColumn("User",100));
			gridMain.Columns.Add(new GridColumn("Table",100));
			gridMain.Columns.Add(new GridColumn("Key",55));
			gridMain.Columns.Add(new GridColumn("Description",150));
			gridMain.Columns.Add(new GridColumn("Field",110));
			gridMain.Columns.Add(new GridColumn("Before",150));
			gridMain.Columns.Add(new GridColumn("After",150));
			gridMain.ListGridRows.Clear();
			List<GridRow> listGridRows = ConstructGridRows();
			for(int i=0;i<listGridRows.Count;i++) {
				gridMain.ListGridRows.Add(listGridRows[i]);
			}
			gridMain.EndUpdate();
		}

		///<summary>Actually creates the GridRows and returns them in a list. Takes care of linking dropdown rows.</summary>
		private List<GridRow> ConstructGridRows() {
			DateTime dateFrom=PIn.Date(textDateFrom.Text);
			DateTime dateTo=PIn.Date(textDateTo.Text);
			if(dateTo==DateTime.MinValue) {
				dateTo=DateTime.Now;
			}
			GridRow row;
			List<GridRow> listGridRows=new List<GridRow>();
			List<Userod> listUserods = Userods.GetDeepCopy();
			for(int i=0;i<_listInsEditLogs.Count;i++) {
				if(!_listInsEditLogs[i].DateTStamp.Between(dateFrom,dateTo)) {
					continue;
				}
				row = new GridRow();
				row.Cells.Add("");
				row.Cells.Add(_listInsEditLogs[i].DateTStamp.ToString());
				Userod userod = listUserods.Find(x=>x.UserNum==_listInsEditLogs[i].UserNum);
				if(userod==null) {
					row.Cells.Add(Lan.g(this,"Unknown")+"("+POut.Long(_listInsEditLogs[i].UserNum)+")");//Unable to find the corresponding user.  
				}
				else {
					row.Cells.Add(userod.UserName); 
				}
				row.Cells.Add(_listInsEditLogs[i].LogType.ToString());
				row.Cells.Add(_listInsEditLogs[i].FKey.ToString());
				row.Cells.Add(_listInsEditLogs[i].Description);
				row.Cells.Add(_listInsEditLogs[i].FieldName);
				row.Cells.Add(_listInsEditLogs[i].OldValue);
				row.Cells.Add(_listInsEditLogs[i].NewValue);
				row.Tag=_listInsEditLogs[i];
				listGridRows.Add(row);
			}
			//To store the InsEditLog of the Parent for each InsEditLog.LogType below
			//link drop down rows to drop down parents.
			//Benefits - Parent key is always the insPlan.PlanNum of the associated insurance Plan
			for(int i=0;i<listGridRows.Count;i++){
				InsEditLog insEditLog=(InsEditLog)listGridRows[i].Tag;
				if(insEditLog.LogType!=InsEditLogType.Benefit || insEditLog.NewValue!="DELETED"){
					continue;
				}
				if(insEditLog.FieldName=="BenefitNum"){//child cannot be BenefitNum
					continue;
				}
				//see if this row has a parent
				for(int j=0;j<listGridRows.Count;j++){
					InsEditLog insEditLogParent=(InsEditLog)listGridRows[j].Tag;
					if(insEditLogParent.LogType!=InsEditLogType.Benefit || insEditLogParent.NewValue!="DELETED"){
						continue;
					}
					if(insEditLogParent.FieldName!="BenefitNum"){//parent must be BenefitNum
						continue;
					}
					if(insEditLog.FKey!=insEditLogParent.FKey){
						continue;
					}
					listGridRows[i].DropDownParent=listGridRows[j];
				}
			}
			/*
			Jordan 2022-04-19- I removed this code because it seems useless. Benefits are the only rows that can have parents.
			//Carrier.CarrierNum - Parent key is always 0
			for(int i=0;i<listGridRows.Count;i++){
				InsEditLog insEditLog=(InsEditLog)listGridRows[i].Tag;
				if(insEditLog.LogType!=InsEditLogType.Carrier || insEditLog.NewValue!="DELETED"){
					continue;
				}
				if(insEditLog.FieldName=="CarrierNum"){//child cannot be CarrierNum
					continue;
				}
				//see if this row has a parent
				for(int j=0;j<listGridRows.Count;j++){
					InsEditLog insEditLogParent=(InsEditLog)listGridRows[j].Tag;
					if(insEditLogParent.LogType!=InsEditLogType.Carrier || insEditLogParent.NewValue!="DELETED"){
						continue;
					}
					if(insEditLogParent.FieldName!="CarrierNum"){//parent must be CarrierNum
						continue;
					}
					if(insEditLog.FKey!=insEditLogParent.FKey){
						continue;
					}
					listGridRows[i].DropDownParent=listGridRows[j];
				}
			}
			//Employer.EmployerNum - Parent key is always 0
			for(int i=0;i<listGridRows.Count;i++){
				InsEditLog insEditLog=(InsEditLog)listGridRows[i].Tag;
				if(insEditLog.LogType!=InsEditLogType.Employer || insEditLog.NewValue!="DELETED"){
					continue;
				}
				if(insEditLog.FieldName=="Employer"){//child cannot be EmployerNum
					continue;
				}
				//see if this row has a parent
				for(int j=0;j<listGridRows.Count;j++){
					InsEditLog insEditLogParent=(InsEditLog)listGridRows[j].Tag;
					if(insEditLogParent.LogType!=InsEditLogType.Employer || insEditLogParent.NewValue!="DELETED"){
						continue;
					}
					if(insEditLogParent.FieldName!="EmployerNum"){//parent must be EmployerNum
						continue;
					}
					if(insEditLog.FKey!=insEditLogParent.FKey){
						continue;
					}
					listGridRows[i].DropDownParent=listGridRows[j];
				}
			}
			//InsPlan.PlanNum - Parent key is always 0
			for(int i=0;i<listGridRows.Count;i++){
				InsEditLog insEditLog=(InsEditLog)listGridRows[i].Tag;
				if(insEditLog.LogType!=InsEditLogType.InsPlan || insEditLog.NewValue!="DELETED"){
					continue;
				}
				if(insEditLog.FieldName=="PlanNum"){//child cannot be PlanNum
					continue;
				}
				//see if this row has a parent
				for(int j=0;j<listGridRows.Count;j++){
					InsEditLog insEditLogParent=(InsEditLog)listGridRows[j].Tag;
					if(insEditLogParent.LogType!=InsEditLogType.Employer || insEditLogParent.NewValue!="DELETED"){
						continue;
					}
					if(insEditLogParent.FieldName!="PlanNum"){//parent must be PlanNum
						continue;
					}
					if(insEditLog.FKey!=insEditLogParent.FKey){
						continue;
					}
					listGridRows[i].DropDownParent=listGridRows[j];
				}
			}*/
			return listGridRows;
		}
		
		///<summary>Makes the "Before" and "After" columns human-readable for certain logs.</summary>
		private void TranslateBeforeAndAfter() {
			for(int i=0;i<_listInsEditLogs.Count;i++) {
				long beforeKey = PIn.Long(_listInsEditLogs[i].OldValue,false);
				long afterKey = PIn.Long(_listInsEditLogs[i].NewValue,false);
				switch(_listInsEditLogs[i].FieldName) {
					case "CarrierNum":
						if(_listInsEditLogs[i].LogType == InsEditLogType.Carrier) {
							break;
						}
						string carrierNameBefore=Carriers.GetCarrier(beforeKey).CarrierName;
						string carrierNameAfter=Carriers.GetCarrier(afterKey).CarrierName;
						if(_listInsEditLogs[i].LogType==InsEditLogType.InsPlan && carrierNameBefore==carrierNameAfter) {//Edits to carrier.
							break;//Don't translate CarrierNum to CarrierName when both carriers have the same name, loses too much useful detail.
						}
						if(beforeKey!=0){
							_listInsEditLogs[i].OldValue = carrierNameBefore;
						}
						if(afterKey!=0){
							_listInsEditLogs[i].NewValue = carrierNameAfter;
						}
						break;
					case "EmployerNum":
						if(_listInsEditLogs[i].LogType == InsEditLogType.Employer) {
							break;
						}
						if(beforeKey!=0){
							_listInsEditLogs[i].OldValue = Employers.GetName(beforeKey);
						}
						if(afterKey!=0){
							_listInsEditLogs[i].NewValue = Employers.GetName(afterKey);
						}
						break;
					case "FeeSched":
					case "CopayFeeSched":
					case "AllowedFeeSched":
						if(beforeKey!=0){
							_listInsEditLogs[i].OldValue = FeeScheds.GetDescription(beforeKey);
						}
						if(afterKey!=0){
							_listInsEditLogs[i].NewValue = FeeScheds.GetDescription(afterKey);
						}
						break;
					case "BenefitType":
						if(beforeKey!=0){
							_listInsEditLogs[i].OldValue = Enum.GetName(typeof(InsBenefitType),beforeKey);
						}
						if(afterKey!=0){
							_listInsEditLogs[i].NewValue = Enum.GetName(typeof(InsBenefitType),afterKey);
						}
						break;
					case "CovCatNum":
						if(beforeKey!=0){
							_listInsEditLogs[i].OldValue = CovCats.GetDesc(beforeKey);
						}
						if(afterKey!=0){
							_listInsEditLogs[i].NewValue = CovCats.GetDesc(afterKey);
						}
						break;
					case "CodeNum":
						if(beforeKey!=0){
							_listInsEditLogs[i].OldValue = ProcedureCodes.GetStringProcCode(beforeKey);
						}
						if(afterKey!=0){
							_listInsEditLogs[i].NewValue = ProcedureCodes.GetStringProcCode(afterKey);
						}
						break;
					default:
						break;
				}
			}
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}
	}
}