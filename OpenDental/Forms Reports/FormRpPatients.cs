using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using OpenDentBusiness;
using System.Collections.Generic;

namespace OpenDental {
	///<summary></summary>
	public partial class FormRpPatients:FormODBase {
		private FormQuery FormQuery2;
		private string SQLselect;
		private string SQLfrom;
		private string SQLwhereComparison;
		private string SQLwhereRelation;
		private string SQLgroup;
		private string sItem;//just used in local loops
		private string ProcLogLastDate;
		private string ProcLogFirstDate;
		private List<string> _listPatFilter;
		private List<bool> UsingInsPlans;//this is outdated.
		private List<bool> UsingProcLogFirst;
		private List<bool> UsingProcLogLast;
		private List<bool> UsingRefDent;
		private List<bool> UsingRefPat;
		private List<bool> UsingRecall;
		private bool IsText;
		private bool IsDate;
		private bool IsDropDown;
		private bool NeedInsPlan=false;
		private bool NeedRefDent=false;
		private bool NeedRefPat=false;
		private bool NeedProcLogLast=false;
		private bool NeedProcLogFirst=false;
		private bool NeedRecall=false;
		private bool IsWhereRelation=false;
		private bool RefToSel;
		private bool RefFromSel;
		private List<FeeSched> _listFeeScheds;
		private List<Provider> _listProviders;
		private List<Def> _listBillingTypeDefs;
		private List<Def> _listRecallUnschedStatusDefs;

		///<summary></summary>
		public FormRpPatients() {
			InitializeComponent();
			InitializeLayoutManager();
			_listPatFilter=new List<string>();
			UsingInsPlans=new List<bool>();
			UsingRefDent=new List<bool>();
			UsingRefPat=new List<bool>();
			UsingProcLogFirst=new List<bool>();
			UsingProcLogLast=new List<bool>();
			UsingRecall=new List<bool>();
			Fill();
			SQLselect="";
			SQLfrom="FROM patient ";
			SQLwhereComparison="";
			SQLwhereRelation="";
			SQLgroup="";
			listConditions.SelectedIndex=0;
			IsText=false;
			IsDate=false;
			IsDropDown=false;
			TextValidAge.MinVal=0;
			TextValidAge.MaxVal=125;
			_listProviders=Providers.GetDeepCopy();
			_listFeeScheds=FeeScheds.GetDeepCopy();
			_listBillingTypeDefs=Defs.GetDefsForCategory(DefCat.BillingTypes);
			_listRecallUnschedStatusDefs=Defs.GetDefsForCategory(DefCat.RecallUnschedStatus);
			Lan.F(this);
		}

		#region Fill

		private void Fill() {
			FillPatFilterList();
			FillRefToSelectList();
			FillRefFromSelectList();
			FillPatientSelectList();
			FillFilterDropList();
		}

		private void FillPatFilterList() {
			_listPatFilter.Add("Address");
			_listPatFilter.Add("Address2");
			_listPatFilter.Add("Age");
			_listPatFilter.Add("ApptModNote");
			_listPatFilter.Add("BillingType");
			_listPatFilter.Add("Birthdate");
			_listPatFilter.Add("Birthday");//new, need to add functionality
			_listPatFilter.Add("City");
			_listPatFilter.Add("ChartNumber");
			_listPatFilter.Add("CreditType");
			_listPatFilter.Add("Email");
			_listPatFilter.Add("EstBalance");
			_listPatFilter.Add("FamAddrNote");
			_listPatFilter.Add("FamFinUrgNote");
			_listPatFilter.Add("FeeSched");
			_listPatFilter.Add("First Visit Date");//new, need to add functionality  
			_listPatFilter.Add("FName");
			_listPatFilter.Add("Gender");
			_listPatFilter.Add("HmPhone");
			_listPatFilter.Add("Last Visit Date");//new, need to add functionality 
			_listPatFilter.Add("LName");
			_listPatFilter.Add("MedUrgNote");
			_listPatFilter.Add("MiddleI");
			//_listPatFilter.Add("NextAptNum");
			_listPatFilter.Add("PatNum");
			_listPatFilter.Add("PatStatus");
			_listPatFilter.Add("Position");
			_listPatFilter.Add("Preferred");
			//_listPatFilter.Add("Primary Carrier"); 
			_listPatFilter.Add("PriProv");
			//_listPatFilter.Add("PriRelationship"); 
			//_listPatFilter.Add("RecallInterval"); 
			_listPatFilter.Add("RecallStatus");
			_listPatFilter.Add("Referred From Provider");//new, need to add functionality
			_listPatFilter.Add("Referred From Patient");//new, need to add functionality 
			_listPatFilter.Add("Salutation");
			//_listPatFilter.Add("Secondary Carrier");
			_listPatFilter.Add("SecProv");
			_listPatFilter.Add("SecRelationship");
			_listPatFilter.Add("SchoolName");
			_listPatFilter.Add("SSN");
			_listPatFilter.Add("State");
			_listPatFilter.Add("StudentStatus");
			_listPatFilter.Add("WirelessPhone");
			_listPatFilter.Add("WkPhone");
			_listPatFilter.Add("Zip");
		}

		private void FillPatientSelectList() {
			listPatientSelect.Items.Clear();
			listPatientSelect.Items.Add("PatNum");
			listPatientSelect.Items.Add("LName");
			listPatientSelect.Items.Add("FName");
			listPatientSelect.Items.Add("MiddleI");
			listPatientSelect.Items.Add("Preferred");
			listPatientSelect.Items.Add("Salutation");
			listPatientSelect.Items.Add("Address");
			listPatientSelect.Items.Add("Address2");
			listPatientSelect.Items.Add("City");
			listPatientSelect.Items.Add("State");
			listPatientSelect.Items.Add("Zip");
			listPatientSelect.Items.Add("HmPhone");
			listPatientSelect.Items.Add("WkPhone");
			listPatientSelect.Items.Add("WirelessPhone");
			listPatientSelect.Items.Add("Birthdate");
			listPatientSelect.Items.Add("Email");
			listPatientSelect.Items.Add("SSN");
			listPatientSelect.Items.Add("Gender");
			listPatientSelect.Items.Add("PatStatus");
			listPatientSelect.Items.Add("Position");
			listPatientSelect.Items.Add("CreditType");
			listPatientSelect.Items.Add("BillingType");
			listPatientSelect.Items.Add("ChartNumber");
			listPatientSelect.Items.Add("PriProv");
			listPatientSelect.Items.Add("SecProv");
			listPatientSelect.Items.Add("FeeSched");
			listPatientSelect.Items.Add("ApptModNote");
			listPatientSelect.Items.Add("AddrNote");
			listPatientSelect.Items.Add("EstBalance");
			listPatientSelect.Items.Add("FamFinUrgNote");
			listPatientSelect.Items.Add("Guarantor");
			listPatientSelect.Items.Add("ImageFolder");
			listPatientSelect.Items.Add("MedUrgNote");
			//listPatientSelect.Items.Add("NextAptNum"); 
			//listPatientSelect.Items.Add("PriPlanNum");//Primary Carrier?
			//listPatientSelect.Items.Add("PriRelationship");// ?
			//listPatientSelect.Items.Add("SecPlanNum");//Secondary Carrier? 
			//listPatientSelect.Items.Add("SecRelationship");// ?
			//listPatientSelect.Items.Add("RecallInterval")); 
			listPatientSelect.Items.Add("RecallStatus");
			listPatientSelect.Items.Add("SchoolName");
			listPatientSelect.Items.Add("StudentStatus");
			listPatientSelect.Items.Add("MedicaidID");
			listPatientSelect.Items.Add("Bal_0_30");
			listPatientSelect.Items.Add("Bal_31_60");
			listPatientSelect.Items.Add("Bal_61_90");
			listPatientSelect.Items.Add("BalOver90");
			listPatientSelect.Items.Add("InsEst");
			//listPatientSelect.Items.Add("PrimaryTeeth");
			listPatientSelect.Items.Add("BalTotal");
			listPatientSelect.Items.Add("EmployerNum");
			//EmploymentNote
			//listPatientSelect.Items.Add("Race"); //This column is depricated.
			listPatientSelect.Items.Add("County");
			//listPatientSelect.Items.Add("GradeSchool");
			listPatientSelect.Items.Add("GradeLevel");
			listPatientSelect.Items.Add("Urgency");
			listPatientSelect.Items.Add("DateFirstVisit");
			//listPatientSelect.Items.Add("PriPending");
			//listPatientSelect.Items.Add("SecPending");
			listPatientSelect.Items.Add("ClinicNum");
			listPatientSelect.Items.Add("HasIns");
			listPatientSelect.Items.Add("TrophyFolder");
			//listPatientSelect.Items.Add("PlannedIsDone");
			//listPatientSelect.Items.Add("PreMed");
			listPatientSelect.Items.Add("Ward");
			//listPatientSelect.Items.Add("PreferConfirmMethod");
			//listPatientSelect.Items.Add("PreferContactMethod");
			//listPatientSelect.Items.Add("PreferRecallMethod");
			//listPatientSelect.Items.Add("SchedBeforeTime");
			//listPatientSelect.Items.Add("SchedAfterTime");
			//listPatientSelect.Items.Add("SchedDayOfWeek");
			//listPatientSelect.Items.Add("Language");
			listPatientSelect.Items.Add("AdmitDate");
			listPatientSelect.Items.Add("Title");
			listPatientSelect.Items.Add("PayPlanDue");
			listPatientSelect.Items.Add("SiteNum");
			listPatientSelect.Items.Add("DateTStamp");
			listPatientSelect.Items.Add("ResponsParty");
			//listPatientSelect.Items.Add("CanadianEligibilityCode");
			//listPatientSelect.Items.Add("AskToArriveEarly");
			SQLselect="";
		}

		private void FillRefToSelectList() {
			listReferredToSelect.Items.Clear();
			listReferredToSelect.Items.Add("LName");
			listReferredToSelect.Items.Add("FName");
			listReferredToSelect.Items.Add("MName");
			listReferredToSelect.Items.Add("Title");
			listReferredToSelect.Items.Add("Address");
			listReferredToSelect.Items.Add("Address2");
			listReferredToSelect.Items.Add("City");
			listReferredToSelect.Items.Add("ST");
			listReferredToSelect.Items.Add("Zip");
			listReferredToSelect.Items.Add("Telephone");
			listReferredToSelect.Items.Add("Phone2");
			listReferredToSelect.Items.Add("Email");
			listReferredToSelect.Items.Add("IsHidden");
			listReferredToSelect.Items.Add("NotPerson");
			listReferredToSelect.Items.Add("PatNum");
			listReferredToSelect.Items.Add("ReferralNum");
			listReferredToSelect.Items.Add("Specialty");
			listReferredToSelect.Items.Add("SSN");
			listReferredToSelect.Items.Add("UsingTIN");
			listReferredToSelect.Items.Add("Note");
			SQLselect="";
		}

		private void FillRefFromSelectList() {
			listReferredFromSelect.Items.Clear();
			listReferredFromSelect.Items.Add("LName");
			listReferredFromSelect.Items.Add("FName");
			listReferredFromSelect.Items.Add("MName");
			listReferredFromSelect.Items.Add("Title");
			listReferredFromSelect.Items.Add("Address");
			listReferredFromSelect.Items.Add("Address2");
			listReferredFromSelect.Items.Add("City");
			listReferredFromSelect.Items.Add("ST");
			listReferredFromSelect.Items.Add("Zip");
			listReferredFromSelect.Items.Add("Telephone");
			listReferredFromSelect.Items.Add("Phone2");
			listReferredFromSelect.Items.Add("Email");
			listReferredFromSelect.Items.Add("IsHidden");
			listReferredFromSelect.Items.Add("NotPerson");
			listReferredFromSelect.Items.Add("PatNum");
			listReferredFromSelect.Items.Add("ReferralNum");
			listReferredFromSelect.Items.Add("Specialty");
			listReferredFromSelect.Items.Add("SSN");
			listReferredFromSelect.Items.Add("UsingTIN");
			listReferredFromSelect.Items.Add("Note");
			SQLselect="";
		}

		private void FillFilterDropList() {
			for(int i = 0;i<_listPatFilter.Count;i++) {
				DropListFilter.Items.Add(_listPatFilter[i]);
			}
		}

		private void FillSQLbox() {
			TextSQL.Text=SQLselect+SQLfrom+SQLwhereRelation+SQLwhereComparison+SQLgroup;
		}

		#endregion

		#region CreateSQL    

		private void CreateSQL() {
			GetTablesNeeded();
			CreateSQLselect();
			CreateSQLfrom();
			CreateSQLwhereRelation();
			CreateSQLwhereComparison();
			CreateSQLgroup();
			FillSQLbox();
		}

		private void GetTablesNeeded() {
			IsWhereRelation=false;
			NeedInsPlan=false;
			NeedRefDent=false;
			NeedRefPat=false;
			NeedProcLogFirst=false;
			NeedProcLogLast=false;
			NeedRecall=false;
			for(int i = 0;i<UsingInsPlans.Count;i++) {
				if(UsingInsPlans[i]) {
					NeedInsPlan=true;
					IsWhereRelation=true;
				}
				else if(UsingRefDent[i]) {
					NeedRefDent=true;
					IsWhereRelation=true;
				}
				else if(UsingRefPat[i]) {
					NeedRefPat=true;
					IsWhereRelation=true;
				}
				else if(UsingProcLogFirst[i]) {
					NeedProcLogFirst=true;
					IsWhereRelation=true;
				}
				else if(UsingProcLogLast[i]) {
					NeedProcLogLast=true;
					IsWhereRelation=true;
				}
				else if(UsingRecall[i]) {
					NeedRecall=true;
					IsWhereRelation=true;
				}
			}
			for(int i = 0;i<listPatientSelect.SelectedIndices.Count;i++) {
				if((string)listPatientSelect.Items.GetObjectAt(listPatientSelect.SelectedIndices[i])=="RecallStatus") {
					NeedRecall=true;
					IsWhereRelation=true;
					break;
				}
			}
		}

		private void CreateSQLselect() {
			bool PatSel=false;
			RefToSel=false;
			RefFromSel=false;
			SQLselect="";
			butOK.Enabled=false;
			string field;
			if(listPatientSelect.SelectedIndices.Count>0) {
				PatSel=true;
				SQLselect="SELECT ";
				for(int i = 0;i<listPatientSelect.SelectedIndices.Count;i++) {
					field=(string)listPatientSelect.Items.GetObjectAt(listPatientSelect.SelectedIndices[i]);
					if(i>0) {
						SQLselect+=",";
					}
					if(field=="RecallStatus") {
						SQLselect+="recall";
					}
					else {
						SQLselect+="patient";
					}
					SQLselect+="."+field;
				}
				SQLselect+=" ";
				butOK.Enabled=true;
			}
			if(listReferredToSelect.SelectedIndices.Count>0) {
				RefToSel=true;
				if(PatSel==false) {
					SQLselect="SELECT ";
				}
				else {
					SQLselect+=",";
				}
				for(int i = 0;i<listReferredToSelect.SelectedIndices.Count;i++) {
					field=(string)listReferredToSelect.Items.GetObjectAt(listReferredToSelect.SelectedIndices[i]);
					if(i>0) {
						SQLselect+=",";
					}
					SQLselect+="referral."+field+" "+field+"_1";
				}
				SQLselect+=" ";
				butOK.Enabled=true;
			}
			if(listReferredFromSelect.SelectedIndices.Count>0) {
				RefFromSel=true;
				if(PatSel==false && RefToSel==false) {
					SQLselect="SELECT ";
				}
				else {
					SQLselect+=",";
				}
				for(int i = 0;i<listReferredFromSelect.SelectedIndices.Count;i++) {
					field=(string)listReferredFromSelect.Items.GetObjectAt(listReferredFromSelect.SelectedIndices[i]);
					if(i>0) {
						SQLselect+=",";
					}
					SQLselect+="referral."+field+" "+field+"_2";
				}
				SQLselect+=" ";
				butOK.Enabled=true;
			}
		}

		private void CreateSQLfrom() {
			SQLfrom="FROM patient";
			if(RefToSel || RefFromSel || NeedRefPat || NeedRefDent) {
				SQLfrom+=",referral,refattach";
			}
			if(NeedRecall) {
				SQLfrom+=",recall";
			}
			if(NeedInsPlan) {
				SQLfrom+=",insplan";
			}
			if(NeedProcLogFirst || NeedProcLogLast) {
				SQLfrom+=",procedurelog";
			}
			SQLfrom+=" ";
		}

		private void CreateSQLwhereRelation() {
			List<string> listWhereClauses=new List<string>();
			if(RefToSel || RefFromSel || NeedRefPat || NeedRefDent) {
				listWhereClauses.Add("patient.patnum=refattach.patnum");
				listWhereClauses.Add("referral.referralnum=refattach.referralnum");
				if(RefToSel || RefFromSel) {
					listWhereClauses.Add("refattach.RefType="+POut.Int((int)(RefToSel ? ReferralType.RefTo : ReferralType.RefFrom)));
				}
			}
			if(NeedInsPlan) {
				listWhereClauses.Add("(patient.priplannum=insplan.plannum OR patient.secplannum=insplan.plannum)");
			}
			if(NeedProcLogFirst || NeedProcLogLast) {
				listWhereClauses.Add("procedurelog.patnum=patient.patnum");
			}
			if(NeedRecall) {
				listWhereClauses.Add("recall.PatNum=patient.PatNum");
			}
			if(!IsWhereRelation && !RefToSel && !RefFromSel) {
				SQLwhereRelation="";
			}
			else {
				SQLwhereRelation="WHERE "+string.Join(" AND ",listWhereClauses)+" ";
			}
		}

		private void CreateSQLwhereComparison() {
			int count=0;
			if(!IsWhereRelation && !RefToSel && !RefFromSel && listPrerequisites.Items.Count>0) {
				SQLwhereComparison="WHERE ";
			}
			else {
				//HAVING statements will be in ListPrerequisites so that users can delete them if necessary.
				//If a HAVING statement is the first in the list, it must be skipped so that there is no leading AND. 
				//The AND for HAVING statements will be taken care of in CreateSQLgroup().
				if(SQLwhereRelation!="" && listPrerequisites.Items.Count>0 && ((string)listPrerequisites.Items.GetObjectAt(0)).Substring(0,1)!="*") {
					SQLwhereComparison="AND ";
				}
				else {
					SQLwhereComparison="";
				}
			}
			string field;
			for(int i = 0;i<listPrerequisites.Items.Count;i++) {
				field=(string)listPrerequisites.Items.GetObjectAt(i);
				if(field.Substring(0,1)=="*") {
					continue;//Skip HAVING statements which will have a leading asterisk. They will be taken care of in CreateSQLgroup().
				}
				if(count==0 && !IsWhereRelation) {
					SQLwhereComparison+=field;
				}
				else if(field.Substring(0,2)=="OR") {
					SQLwhereComparison+=" "+field;
				}
				else {
					SQLwhereComparison+=((i==0) ? " " : " AND ")+field;
				}
				count++;
			}
		}

		private void CreateSQLgroup() {
			if(NeedProcLogLast && !NeedProcLogFirst) {
				SQLgroup=" GROUP BY procedurelog.patnum HAVING "+ProcLogLastDate;
			}
			else if(NeedProcLogLast && NeedProcLogFirst) {
				SQLgroup=" GROUP BY procedurelog.patnum HAVING "+ProcLogLastDate+" AND "+ProcLogFirstDate;
			}
			else if(NeedProcLogFirst && !NeedProcLogLast) {
				SQLgroup=" GROUP BY procedurelog.patnum HAVING "+ProcLogFirstDate;
			}
			else {
				SQLgroup="";
			}
		}

		#endregion

		#region SetConditions

		private void SetListBoxConditions() {
			listBoxColumns.Visible=true;
			TextDate.Visible=false;
			TextBox.Visible=false;
			TextValidAge.Visible=false;
			IsDropDown=true;
			IsDate=false;
			IsText=false;
			listConditions.Enabled=true;
			listBoxColumns.SelectedIndex=-1;
			butAddFilter.Enabled=false;
			labelHelp.Visible=false;
		}

		private void SetTextBoxConditions() {
			TextBox.Clear();
			listConditions.Enabled=true;
			TextBox.Visible=true;
			TextDate.Visible=false;
			listBoxColumns.Visible=false;
			TextValidAge.Visible=false;
			TextBox.Select();
			IsText=true;
			IsDate=false;
			IsDropDown=false;
			butAddFilter.Enabled=true;
			labelHelp.Visible=false;
		}

		private void SetDateConditions() {
			TextDate.Visible=true;
			TextBox.Visible=false;
			listBoxColumns.Visible=false;
			TextValidAge.Visible=false;
			IsDate=true;
			IsText=false;
			IsDropDown=false;
			TextDate.Clear();
			TextDate.Select();
			listConditions.Enabled=true;
			butAddFilter.Enabled=true;
			labelHelp.Visible=true;
		}

		#endregion

		#region Selected Index Changes

		private void DropListFilter_SelectedIndexChanged(object sender,System.EventArgs e) {
			switch(DropListFilter.SelectedItem.ToString()) {
				case "Address":
				case "Address2":
				case "ApptModNote":
				case "ChartNumber":
				case "City":
				case "CreditType":
				case "Email":
				case "EstBalance":
				case "FamAddrNote":
				case "FamFinUrgNote":
				case "FName":
				case "HmPhone":
				case "LName":
				case "MedUrgNote":
				case "MiddleI":
				case "NextAptNum":
				case "PatNum":
				case "Preferred":
				//case "RecallInterval":
				case "Salutation":
				case "SchoolName":
				case "State":
				case "StudentStatus":
				case "WirelessPhone":
				case "WkPhone":
				case "Zip":
					SetTextBoxConditions();
					break;
				case "SSN":
					SetTextBoxConditions();
					labelHelp.Visible=true;
					labelHelp.Text="Type in SSN as 123456789";
					break;
				case "Primary Carrier":
				case "Secondary Carrier":
					SetTextBoxConditions();
					labelHelp.Visible=true;
					labelHelp.Text="Type in Name of Insurance Company";
					break;
				case "Referred From Provider":
					labelHelp.Visible=true;
					SetTextBoxConditions();
					labelHelp.Text="Type in last name of provider";
					break;
				case "Referred From Patient":
					SetTextBoxConditions();
					labelHelp.Visible=true;
					labelHelp.Text="Type in last name of patient";
					break;
				case "Age":
					TextValidAge.Clear();
					listConditions.Enabled=true;
					TextBox.Visible=false;
					TextDate.Visible=false;
					listBoxColumns.Visible=false;
					TextValidAge.Visible=true;
					TextValidAge.Select();
					IsText=false;
					IsDate=false;
					IsDropDown=false;
					butAddFilter.Enabled=true;
					labelHelp.Text="Please Input a number";
					break;
				case "Birthdate":
				case "Last Visit Date":
				case "First Visit Date":
					SetDateConditions();
					labelHelp.Text="Type Date as mm/dd/yyyy";
					break;
				case "Birthday":
					SetDateConditions();
					labelHelp.Text="Type Date as mm/dd";
					break;
				case "PatStatus":
					SetListBoxConditions();
					listBoxColumns.Items.Clear();
					listBoxColumns.Items.Add("Patient");
					listBoxColumns.Items.Add("NonPatient");
					listBoxColumns.Items.Add("Inactive");
					listBoxColumns.Items.Add("Archived");
					listBoxColumns.Items.Add("Deleted");
					listBoxColumns.Items.Add("Deceased");
					listBoxColumns.Items.Add("Prospective");
					break;
				case "Gender":
					SetListBoxConditions();
					listBoxColumns.Items.Clear();
					listBoxColumns.Items.Add("Male");
					listBoxColumns.Items.Add("Female");
					listBoxColumns.Items.Add("Unknown");
					break;
				case "Position":
					SetListBoxConditions();
					listConditions.SelectedIndex=1;
					listConditions.Enabled=false;
					listBoxColumns.Items.Clear();
					listBoxColumns.Items.Add("Single");
					listBoxColumns.Items.Add("Married");
					listBoxColumns.Items.Add("Child");
					break;
				case "FeeSched":
					SetListBoxConditions();
					listBoxColumns.Items.Clear();
					for(int i = 0;i<_listFeeScheds.Count;i++) {
						sItem=_listFeeScheds[i].Description;
						if(_listFeeScheds[i].IsHidden) {
							sItem+="(hidden)";
						}
						listBoxColumns.Items.Add(sItem);
					}
					break;
				case "BillingType":
					SetListBoxConditions();
					listBoxColumns.Items.Clear();
					for(int i = 0;i<_listBillingTypeDefs.Count;i++) {
						sItem=_listBillingTypeDefs[i].ItemName.ToString();
						if(_listBillingTypeDefs[i].IsHidden) {
							sItem+="(hidden)";
						}
						listBoxColumns.Items.Add(sItem);
					}
					break;
				case "RecallStatus":
					SetListBoxConditions();
					listBoxColumns.Items.Clear();
					for(int i = 0;i<_listRecallUnschedStatusDefs.Count;i++) {
						sItem=_listRecallUnschedStatusDefs[i].ItemName.ToString();
						if(_listRecallUnschedStatusDefs[i].IsHidden) {
							sItem+="(hidden)";
						}
						listBoxColumns.Items.Add(sItem);
					}
					break;
				case "PriProv":
				case "SecProv":
					SetListBoxConditions();
					listBoxColumns.Items.Clear();
					for(int i = 0;i<_listProviders.Count;i++) {
						sItem=_listProviders[i].LName+", "
							+_listProviders[i].MI+" "+_listProviders[i].FName;
						if(_listProviders[i].IsHidden) {
							sItem+="(hidden)";
						}
						if(_listProviders[i].ProvStatus==ProviderStatus.Deleted) {
							sItem+="(deleted)";
						}
						listBoxColumns.Items.Add(sItem);
					}
					break;
				case "PriRelationship":
				case "SecRelationship":
					SetListBoxConditions();
					listBoxColumns.Items.Clear();
					listBoxColumns.Items.Add("Self");
					listBoxColumns.Items.Add("Spouse");
					listBoxColumns.Items.Add("Child");
					listBoxColumns.Items.Add("Employee");
					listBoxColumns.Items.Add("HandicapDep");
					listBoxColumns.Items.Add("SignifOther");
					listBoxColumns.Items.Add("InjuredPlantiff");
					listBoxColumns.Items.Add("LifePartner");
					listBoxColumns.Items.Add("Dependent");
					break;
			}
		}

		private void listBoxColumns_SelectedIndexChanged(object sender,System.EventArgs e) {
			if(listBoxColumns.SelectedIndices.Count>0) {
				butAddFilter.Enabled=true;
			}
			else {
				butAddFilter.Enabled=false;
			}
		}

		private void listPrerequisites_SelectedIndexChanged(object sender,System.EventArgs e) {
			butDeleteFilter.Enabled=false;
			if(listPrerequisites.Items.Count>0 && listPrerequisites.SelectedIndices.Count>0) {
				butDeleteFilter.Enabled=true;
			}
		}

		private void listPatientSelect_SelectedIndexChanged(object sender,System.EventArgs e) {
			CreateSQL();
		}

		private void listReferredToSelect_SelectedIndexChanged(object sender,System.EventArgs e) {
			CreateSQL();
		}

		private void listReferredFromSelect_SelectedIndexChanged(object sender,System.EventArgs e) {
			CreateSQL();
		}

		#endregion

		#region Button Clicks

		private void butAddFilter_Click(object sender,System.EventArgs e) {
			if(!TextDate.IsValid() || !TextValidAge.IsValid() || (TextDate.Text=="" &&  IsDate)) {
				MessageBox.Show("Please fix data entry errors first.");
				return;
			}
			if(DropListFilter.SelectedItem == null) {
				return;
			}
			if(TextValidAge.Text=="" && DropListFilter.SelectedItem.ToString()=="Age") {
				MessageBox.Show("Please enter age.");
				return;
			}
			if(listConditions.SelectedIndex==-1) {
				MessageBox.Show("Please select a condition.");
				return;
			}
			UsingRefDent.Add(false);
			UsingRefPat.Add(false);
			UsingProcLogFirst.Add(false);
			UsingProcLogLast.Add(false);
			UsingRecall.Add(false);
			UsingInsPlans.Add(false);
			#region IsText
			if(IsText) {
				if(DropListFilter.SelectedItem.ToString()=="Primary Carrier") {
					if(listConditions.SelectedIndex==0) {
						//spaces around = are necessary
						listPrerequisites.Items.Add("insplan.Carrier LIKE '%"+TextBox.Text+"%'");
					}
					else {
						listPrerequisites.Items.Add("insplan.Carrier "+listConditions.SelectedItem.ToString()+" '"+TextBox.Text+"'");
					}
					UsingInsPlans[UsingInsPlans.Count-1]=true;
				}//end if(DropListFilter.SelectedItem.ToString()=="Primary Carrier")
				else if(DropListFilter.SelectedItem.ToString()=="Secondary Carrier") {
					if(listConditions.SelectedIndex==0) {
						listPrerequisites.Items.Add("insplan.Carrier LIKE '%"+TextBox.Text+"%'");
					}
					else {
						listPrerequisites.Items.Add("insplan.Carrier "
							+listConditions.SelectedItem.ToString()+" '"+TextBox.Text+"'");
					}
					UsingInsPlans[UsingInsPlans.Count-1]=true;
				}//	end	else if(DropListFilter.SelectedItem.ToString()=="Secondary Carrier"){
				else if(DropListFilter.SelectedItem.ToString()=="Referred From Provider") {
					if(listConditions.SelectedIndex==0) {
						listPrerequisites.Items.Add("referral.patnum=0 AND referral.LName LIKE '%" +TextBox.Text+"%'");
					}
					else {
						listPrerequisites.Items.Add("referral.patnum=0 AND referral.LName "+listConditions.SelectedItem.ToString()
						 +" '"+TextBox.Text+"'");
					}
					UsingRefDent[UsingInsPlans.Count-1]=true;
				}
				else if(DropListFilter.SelectedItem.ToString()=="Referred From Patient") {
					if(listConditions.SelectedIndex==0) {
						listPrerequisites.Items.Add("referral.patnum > '0' AND referral.LName LIKE '%" +TextBox.Text+"%'");
					}
					else {
						listPrerequisites.Items.Add("referral.patnum > '0' AND referral.LName "+listConditions.SelectedItem.ToString()
						 +" '"+TextBox.Text+"'");
					}
					UsingRefPat[UsingInsPlans.Count-1]=true;
				}
				else {
					if(listConditions.SelectedIndex==0) {
						listPrerequisites.Items.Add("patient."+DropListFilter.SelectedItem.ToString()+" LIKE '%"+TextBox.Text+"%'");
					}
					else {
						listPrerequisites.Items.Add("patient."+DropListFilter.SelectedItem.ToString()+" "
							+listConditions.SelectedItem.ToString()+" '"+TextBox.Text+"'");
					}
				}
			}
			#endregion IsText
			#region Age
			else if(DropListFilter.SelectedItem.ToString()=="Age") {
				if(listConditions.SelectedIndex==0) {
					listPrerequisites.Items.Add("patient.BirthDate LIKE '%"
					 +DateTime.Now.AddYears(-Convert.ToInt32(TextValidAge.Text)).ToString("yyyy-MM-dd")+"%'");
				}
				else {
					listPrerequisites.Items.Add("patient.Birthdate "+listConditions.SelectedItem.ToString()+" '"
						+DateTime.Now.AddYears(-Convert.ToInt32(TextValidAge.Text)).ToString("yyyy-MM-dd")+"'");
				}
			}
			#endregion Age
			#region IsDate
			else if(IsDate) {
				if(DropListFilter.SelectedItem.ToString()=="First Visit Date") {
					if(listConditions.SelectedIndex==0) {
						//Add the HAVING statement to ListPrerequisites with a leading asterisk so that it shows up in the UI so that users can delete it.
						//It is added with a leading asterisk so that it gets skipped in CreateSQLwhereComparison().
						listPrerequisites.Items.Add("*HAVING MIN(procdate) LIKE '%"+POut.Date(DateTime.Parse(TextDate.Text),false)+"%'");
						//Set the class wide variable without the *HAVING portion. If ProcLogFirstDate has a value, it will be used in CreateSQLgroup().
						ProcLogFirstDate="MIN(procdate) LIKE '%"+POut.Date(DateTime.Parse(TextDate.Text),false)+"%'";
					}
					else {
						listPrerequisites.Items.Add("*HAVING MIN(procdate) "+listConditions.SelectedItem.ToString()
							+" "+POut.Date(DateTime.Parse(TextDate.Text)));
						ProcLogFirstDate="MIN(procdate) "+listConditions.SelectedItem.ToString()
							+" "+POut.Date(DateTime.Parse(TextDate.Text));
					}
					UsingProcLogFirst[UsingInsPlans.Count-1]=true;
				}
				else if(DropListFilter.SelectedItem.ToString()=="Last Visit Date") {
					if(listConditions.SelectedIndex==0) {
						//See comment above where ProcLogFirstDate is handled regarding the reasoning for leading the having statement with an asterisk.
						listPrerequisites.Items.Add("*HAVING MAX(procdate) LIKE '%"+POut.Date(DateTime.Parse(TextDate.Text),false)+"%'");
						ProcLogLastDate="MAX(procdate) LIKE '%"+POut.Date(DateTime.Parse(TextDate.Text),false)+"%'";
					}
					else {
						listPrerequisites.Items.Add("*HAVING MAX(procdate) "+listConditions.SelectedItem.ToString()
							+" "+POut.Date(DateTime.Parse(TextDate.Text)));
						ProcLogLastDate="MAX(procdate) "+listConditions.SelectedItem.ToString()
							+" "+POut.Date(DateTime.Parse(TextDate.Text));
					}
					UsingProcLogLast[UsingInsPlans.Count-1]=true;
				}
				else if(DropListFilter.SelectedItem.ToString()=="Birthday") {
					if(listConditions.SelectedIndex==0) {
						listPrerequisites.Items.Add("MONTH(Birthdate) "
							+"= '"
							+DateTime.Parse(TextDate.Text).Month.ToString()+"'");
					}
					else {
						listPrerequisites.Items.Add("SUBSTRING(Birthdate,6,5) "
							+listConditions.SelectedItem.ToString()+" '"
							+DateTime.Parse(TextDate.Text).ToString("MM")+"-"
							+DateTime.Parse(TextDate.Text).ToString("dd")+"'");
					}
				}
				else {
					if(listConditions.SelectedIndex==0) {
						listPrerequisites.Items.Add(DropListFilter.SelectedItem.ToString()
							+" Like '%"+POut.Date(DateTime.Parse(TextDate.Text),false)+"%'");
					}
					else {
						listPrerequisites.Items.Add(DropListFilter.SelectedItem.ToString()+" "
							+listConditions.SelectedItem.ToString()+" "+POut.Date(DateTime.Parse(TextDate.Text)));
					}
				}
			}
			#endregion IsDate
			#region IsDropDown
			else if(IsDropDown) {
				if(DropListFilter.SelectedItem.ToString()=="FeeSched") {
					sItem="";
					for(int i = 0;i<listBoxColumns.SelectedIndices.Count;i++) {
						if(i>0) {
							UsingInsPlans.Add(false);
							UsingRefDent.Add(false);
							UsingRefPat.Add(false);
							UsingProcLogFirst.Add(false);
							UsingProcLogLast.Add(false);
							UsingRecall.Add(false);
						}
						if(i==0) {
							sItem="(";
						}
						else {
							sItem="OR ";
						}
						sItem+="patient.FeeSched "+listConditions.SelectedItem.ToString()+" '"
							+_listFeeScheds[listBoxColumns.SelectedIndices[i]].FeeSchedNum.ToString()+"'";
						if(i==listBoxColumns.SelectedIndices.Count-1) {
							sItem+=")";
						}
						listPrerequisites.Items.Add(sItem);
					}
				}//end if
				else if(DropListFilter.SelectedItem.ToString()=="BillingType") {
					sItem="";
					for(int i = 0;i<listBoxColumns.SelectedIndices.Count;i++) {
						if(i>0) {
							UsingInsPlans.Add(false);
							UsingRefDent.Add(false);
							UsingRefPat.Add(false);
							UsingProcLogFirst.Add(false);
							UsingProcLogLast.Add(false);
							UsingRecall.Add(false);
						}
						if(i==0) {
							sItem="(";
						}
						else {
							sItem="OR ";
						}
						sItem+="patient.BillingType "+listConditions.SelectedItem.ToString()+" '"
							+_listBillingTypeDefs[listBoxColumns.SelectedIndices[i]].DefNum.ToString()+"'";
						if(i==listBoxColumns.SelectedIndices.Count-1) {
							sItem+=")";
						}
						listPrerequisites.Items.Add(sItem);
					}
				}
				else if(DropListFilter.SelectedItem.ToString()=="RecallStatus") {
					sItem="";
					UsingRecall[UsingRecall.Count-1]=true;
					for(int i = 0;i<listBoxColumns.SelectedIndices.Count;i++) {
						if(i>0) {
							UsingInsPlans.Add(false);
							UsingRefDent.Add(false);
							UsingRefPat.Add(false);
							UsingProcLogFirst.Add(false);
							UsingProcLogLast.Add(false);
							UsingRecall.Add(true);
						}
						if(i==0) {
							sItem="(";
						}
						else {
							sItem="OR ";
						}
						sItem+="recall.RecallStatus "+listConditions.SelectedItem.ToString()+" '"
							+_listRecallUnschedStatusDefs[listBoxColumns.SelectedIndices[i]]
							.DefNum.ToString()+"'";
						if(i==listBoxColumns.SelectedIndices.Count-1) {
							sItem+=")";
						}
						listPrerequisites.Items.Add(sItem);
					}
				}
				else if(DropListFilter.SelectedItem.ToString()=="PriProv") {
					sItem="";
					for(int i = 0;i<listBoxColumns.SelectedIndices.Count;i++) {
						if(i>0) {
							UsingInsPlans.Add(false);
							UsingRefDent.Add(false);
							UsingRefPat.Add(false);
							UsingProcLogFirst.Add(false);
							UsingProcLogLast.Add(false);
							UsingRecall.Add(false);
						}
						if(i==0) {
							sItem="(";
						}
						else {
							sItem="OR ";
						}
						sItem+="patient.PriProv "+listConditions.SelectedItem.ToString()+" '"
							+_listProviders[listBoxColumns.SelectedIndices[i]].ProvNum.ToString()+"'";
						if(i==listBoxColumns.SelectedIndices.Count-1) {
							sItem+=")";
						}
						listPrerequisites.Items.Add(sItem);
					}
				}
				else if(DropListFilter.SelectedItem.ToString()=="SecProv") {
					sItem="";
					for(int i = 0;i<listBoxColumns.SelectedIndices.Count;i++) {
						if(i>0) {
							UsingInsPlans.Add(false);
							UsingRefDent.Add(false);
							UsingRefPat.Add(false);
							UsingProcLogFirst.Add(false);
							UsingProcLogLast.Add(false);
							UsingRecall.Add(false);
						}
						if(i==0) {
							sItem="(";
						}
						else {
							sItem="OR ";
						}
						sItem+="patient.SecProv "+listConditions.SelectedItem.ToString()+" '"
							+_listProviders[listBoxColumns.SelectedIndices[i]].ProvNum.ToString()+"'";
						if(i==listBoxColumns.SelectedIndices.Count-1) {
							sItem+=")";
						}
						listPrerequisites.Items.Add(sItem);
					}
				}
				else {
					//PatStatus
					//Gender
					//Position
					//PriRelationship
					//SecRelationship
					for(int i = 0;i<listBoxColumns.SelectedIndices.Count;i++) {
						if(i>0) {
							UsingInsPlans.Add(false);
							UsingRefDent.Add(false);
							UsingRefPat.Add(false);
							UsingProcLogFirst.Add(false);
							UsingProcLogLast.Add(false);
							UsingRecall.Add(false);
						}
						if(listConditions.SelectedIndex==0) {
							listPrerequisites.Items.Add(DropListFilter.SelectedItem.ToString()+" LIKE '%"
								+listBoxColumns.SelectedIndices[i].ToString()+"%'");
						}
						else {
							listPrerequisites.Items.Add(DropListFilter.SelectedItem.ToString()+" "
								+listConditions.SelectedItem.ToString()+" '"
								+listBoxColumns.SelectedIndices[i].ToString()+"'");
						}
					}//end for
				}
				listBoxColumns.SelectedIndex=-1;
				butAddFilter.Enabled=false;
			}
			#endregion IsDropDown
			CreateSQL();
			FillSQLbox();
			listConditions.Enabled=true;
			TextBox.Clear();
			TextDate.Clear();
			TextValidAge.Clear();
		}

		private void butDeleteFilter_Click(object sender,System.EventArgs e) {
			int selectedIndex;
			while(listPrerequisites.SelectedIndices.Count>0) {
				selectedIndex=listPrerequisites.SelectedIndices[listPrerequisites.SelectedIndices.Count-1];//must start from the end and work backwards
				UsingInsPlans.RemoveAt(selectedIndex);
				UsingRefDent.RemoveAt(selectedIndex);
				UsingRefPat.RemoveAt(selectedIndex);
				UsingProcLogFirst.RemoveAt(selectedIndex);
				UsingProcLogLast.RemoveAt(selectedIndex);
				RemoveListPrerequisitesItem(selectedIndex);
			}
			CreateSQL();
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			ReportSimpleGrid report=new ReportSimpleGrid();
			report.Query=TextSQL.Text;
			FormQuery2=new FormQuery(report);
			FormQuery2.IsReport=false;
			FormQuery2.SubmitQuery();
			FormQuery2.textQuery.Text=report.Query;
			FormQuery2.ShowDialog();
			FormQuery2.Dispose();
		}

		#endregion

		private void butCancel_Click(object sender,System.EventArgs e) {

		}

		private void RemoveListPrerequisitesItem(int index) {
			string item=(string)listPrerequisites.Items.GetObjectAt(index);
			if(item.Contains("(") && item.Contains(")")) {
				listPrerequisites.Items.RemoveAt(index);
				listPrerequisites.SelectedIndices.RemoveAt(listPrerequisites.SelectedIndices.Count-1);
			}
			//Check if item being removed has a ) to move to previous item.
			else if(item.Contains(")")) {
				string temp=((string)listPrerequisites.Items.GetObjectAt(index-1))+")";
				listPrerequisites.Items.SetValue(index-1,temp);
				listPrerequisites.Items.RemoveAt(index);
				listPrerequisites.SelectedIndices.RemoveAt(listPrerequisites.SelectedIndices.Count-1);
			}
			//Check if item being removed has a ( to move to next item.
			else if(item.Contains("(")) {
				string temp="";
				//check to see if need to remove the OR or AND from the next item.
				if(((string)listPrerequisites.Items.GetObjectAt(index+1)).Substring(0,2)=="OR") {
					temp="("+((string)listPrerequisites.Items.GetObjectAt(index+1)).Substring(3);
				}
				else if(((string)listPrerequisites.Items.GetObjectAt(index+1)).Substring(0,3)=="AND") {
					temp="("+((string)listPrerequisites.Items.GetObjectAt(index+1)).Substring(4);
				}
				listPrerequisites.Items.SetValue(index+1,temp);
				listPrerequisites.Items.RemoveAt(index);
				listPrerequisites.SelectedIndices.RemoveAt(listPrerequisites.SelectedIndices.Count-1);
			}
			else {
				listPrerequisites.Items.RemoveAt(index);
				listPrerequisites.SelectedIndices.RemoveAt(listPrerequisites.SelectedIndices.Count-1);
			}
		}
	}
}
