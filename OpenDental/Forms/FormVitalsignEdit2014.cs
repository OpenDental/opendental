using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental;
using OpenDental.UI;
using System.Diagnostics;

namespace OpenDental {
	public partial class FormVitalsignEdit2014:FormODBase {
		public Vitalsign VitalsignCur;
		public int AgeBeforeJanFirst;
		private Patient _patient;
		private List<Loinc> _listLoincsHeightCodes;
		private List<Loinc> _listLoincsWeightCodes;
		private List<Loinc> _listLoincsBMICodes;
		private long _diseaseDefNumPreg;//used to keep track of the def we will update VitalsignCur.PregDiseaseNum with
		private string _pregDefaultText;
		private string _pregManualText;
		private InterventionCodeSet _interventionCodeSet;
		private List<GenderAge_LMS> _listGenderAge_LMSs;

		public FormVitalsignEdit2014() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormVitalsignEdit2014_Load(object sender,EventArgs e) {
			_listGenderAge_LMSs=Vitalsigns.GetListLMS();
			_pregDefaultText="A diagnosis of pregnancy with this code will be added to the patient's medical history with a start date equal to the date of this exam.";
			_pregManualText="Selecting a code that is not in the recommended list of pregnancy codes may not exclude this patient from certain CQM calculations.";
			labelPregNotice.Text=_pregDefaultText;
			if(VitalsignCur.Pulse!=0) {
				textPulse.Text=VitalsignCur.Pulse.ToString();
			}
			#region SetHWBPAndVisibility
			_diseaseDefNumPreg=0;
			groupInterventions.Visible=false;
			_patient=Patients.GetPat(VitalsignCur.PatNum);
			textDateTaken.Text=VitalsignCur.DateTaken.ToShortDateString();
			AgeBeforeJanFirst=PIn.Date(textDateTaken.Text).Year-_patient.Birthdate.Year-1;
			if(VitalsignCur.Height!=0) {
				textHeight.Text=VitalsignCur.Height.ToString();
			}
			if(VitalsignCur.Weight!=0) {
				textWeight.Text=VitalsignCur.Weight.ToString();
			}
			CalcBMI();
			if(VitalsignCur.BpDiastolic!=0) {
				textBPd.Text=VitalsignCur.BpDiastolic.ToString();
			}
			if(VitalsignCur.BpSystolic!=0) {
				textBPs.Text=VitalsignCur.BpSystolic.ToString();
			}
			#endregion
			#region SetBMIHeightWeightExamCodes
			_listLoincsHeightCodes=new List<Loinc>();
			_listLoincsWeightCodes=new List<Loinc>();
			_listLoincsBMICodes=new List<Loinc>();
			FillBMICodeLists();
			textBMIPercentileCode.Text=VitalsignCur.BMIExamCode;//Only ever going to be blank or LOINC 59576-9 - Body mass index (BMI) [Percentile] Per age and gender
			comboHeightExamCode.Items.AddNone<Loinc>();
			comboHeightExamCode.SelectedIndex=0;
			for(int i=0;i<_listLoincsHeightCodes.Count;i++) {
				if(_listLoincsHeightCodes[i].NameShort=="" || _listLoincsHeightCodes[i].NameLongCommon.Length<30) {//30 is roughly the number of characters that will fit in the combo box
					comboHeightExamCode.Items.Add(_listLoincsHeightCodes[i].NameLongCommon,_listLoincsHeightCodes[i]);
				}
				else {
					comboHeightExamCode.Items.Add(_listLoincsHeightCodes[i].NameShort,_listLoincsHeightCodes[i]);
				}
				if(i==0 || VitalsignCur.HeightExamCode==_listLoincsHeightCodes[i].LoincCode) {
					comboHeightExamCode.SelectedIndex=i+1;
				}
			}
			comboWeightExamCode.Items.AddNone<Loinc>();
			comboWeightExamCode.SelectedIndex=0;
			for(int i=0;i<_listLoincsWeightCodes.Count;i++) {
				if(_listLoincsWeightCodes[i].NameShort=="" || _listLoincsWeightCodes[i].NameLongCommon.Length<30) {//30 is roughly the number of characters that will fit in the combo box
					comboWeightExamCode.Items.Add(_listLoincsWeightCodes[i].NameLongCommon,_listLoincsWeightCodes[i]);
				}
				else {
					comboWeightExamCode.Items.Add(_listLoincsWeightCodes[i].NameShort,_listLoincsWeightCodes[i]);
				}
				if(i==0 || VitalsignCur.WeightExamCode==_listLoincsWeightCodes[i].LoincCode) {
					comboWeightExamCode.SelectedIndex=i+1;
				}
			}
			#endregion
			#region SetPregCodeAndDescript
			if(VitalsignCur.PregDiseaseNum>0) {
				checkPregnant.Checked=true;
				SetPregCodeAndDescript();//if after this function disdefNumCur=0, the PregDiseaseNum is pointing to an invalid disease or diseasedef, or the default is set to 'none'
				if(VitalsignCur.PregDiseaseNum==0) {
					checkPregnant.Checked=false;
					textPregCode.Clear();
					textPregCodeDescript.Clear();
					labelPregNotice.Visible=false;
				}
				else if(_diseaseDefNumPreg>0) {
					labelPregNotice.Visible=true;
					butChangeDefault.Text="Go to Problem";
				}
			}
			#endregion
			#region SetEhrNotPerformedReasonCodeAndDescript
			if(VitalsignCur.EhrNotPerformedNum<=0) {
				return;
			}
			checkNotPerf.Checked=true;
			EhrNotPerformed ehrNotPerformed=EhrNotPerformeds.GetOne(VitalsignCur.EhrNotPerformedNum);
			if(ehrNotPerformed==null) {
				VitalsignCur.EhrNotPerformedNum=0;//if this vital sign is pointing to an EhrNotPerformed item that no longer exists, we will just remove the pointer
				checkNotPerf.Checked=false;
				return;
			}
			textReasonCode.Text=ehrNotPerformed.CodeValueReason;
			//all reasons not performed are snomed codes
			Snomed snomed=Snomeds.GetByCode(ehrNotPerformed.CodeValueReason);
			if(snomed!=null) {
				textReasonDescript.Text=snomed.Description;
			}
			#endregion
		}

		///<summary>Sets the pregnancy code and description text box with either the attached pregnancy dx if exists or the default preg dx set in FormEhrSettings or a manually selected def.  If the pregnancy diseasedef with the default pregnancy code and code system does not exist, it will be inserted.  The pregnancy problem will be inserted when closing if necessary.</summary>
		private void SetPregCodeAndDescript() {
			labelPregNotice.Text=_pregDefaultText;
			_diseaseDefNumPreg=0;//this will be set to the correct problem def at the end of this function and will be the def of the problem we will insert/attach this exam to
			string pregCode="";
			string descript="";
			Disease disease=null;
			DiseaseDef diseaseDef=null;
			DateTime dateExam=PIn.Date(textDateTaken.Text);//this may be different than the saved Vitalsign.DateTaken if user edited
			#region Get DiseaseDefNum from attached pregnancy problem
			if(VitalsignCur.PregDiseaseNum>0) {//already pointing to a disease, get that one
				disease=Diseases.GetOne(VitalsignCur.PregDiseaseNum);//get disease this vital sign is pointing to, see if it exists
				if(disease==null) {
					VitalsignCur.PregDiseaseNum=0;
				}
				else {
					if(dateExam.Year<1880 || disease.DateStart>dateExam.Date || (disease.DateStop.Year>1880 && disease.DateStop<dateExam.Date)) {
						VitalsignCur.PregDiseaseNum=0;
						disease=null;
					}
					else {
						diseaseDef=DiseaseDefs.GetItem(disease.DiseaseDefNum);
						if(diseaseDef==null) {
							VitalsignCur.PregDiseaseNum=0;
							disease=null;
						}
						else {//disease points to valid def
							_diseaseDefNumPreg=diseaseDef.DiseaseDefNum;
						}
					}
				}
			}
			#endregion
			if(VitalsignCur.PregDiseaseNum==0) {//not currently attached to a disease
				#region Get DiseaseDefNum from existing pregnancy problem
				if(dateExam.Year>1880) {//only try to find existing problem if a valid exam date is entered before checking the box, otherwise we do not know what date to compare to the active dates of the pregnancy dx
					List<DiseaseDef> listDiseaseDefsPreg=DiseaseDefs.GetAllPregDiseaseDefs();
					List<Disease> listDiseasesPatient=Diseases.Refresh(VitalsignCur.PatNum,true);
					for(int i=0;i<listDiseasesPatient.Count;i++) {//loop through all diseases for this patient, shouldn't be very many
						if(listDiseasesPatient[i].DateStart>dateExam.Date //startdate for current disease is after the exam date set in form
							|| (listDiseasesPatient[i].DateStop.Year>1880 && listDiseasesPatient[i].DateStop<dateExam.Date))//or current disease has a stop date and stop date before exam date
						{
							continue;
						}
						for(int j=0;j<listDiseaseDefsPreg.Count;j++) {//loop through preg disease defs in the db, shouldn't be very many
							if(listDiseasesPatient[i].DiseaseDefNum!=listDiseaseDefsPreg[j].DiseaseDefNum) {//see if this problem is a pregnancy problem
								continue;
							}
							if(disease==null || listDiseasesPatient[i].DateStart>disease.DateStart) {//if we haven't found a disease match yet or this match is more recent (later start date)
								disease=listDiseasesPatient[i];
								break;
							}
						}
					}
				}
				if(disease!=null) {
					_diseaseDefNumPreg=disease.DiseaseDefNum;
					VitalsignCur.PregDiseaseNum=disease.DiseaseNum;
				}
				#endregion
				else {//we are going to insert either the default pregnancy problem or a manually selected problem
					#region Get DiseaseDefNum from global default pregnancy problem
					//if preg dx doesn't exist, use the default pregnancy code if set to something other than blank or 'none'
					pregCode=PrefC.GetString(PrefName.PregnancyDefaultCodeValue);//could be 'none' which disables the automatic dx insertion
					if(pregCode!="" && pregCode!="none") {//default pregnancy code set to a code other than 'none', should never be blank, we set in ConvertDB and don't allow blank
						_diseaseDefNumPreg=DiseaseDefs.GetDefNumForDefaultPreg(pregCode);
					}
					#endregion
					#region Get DiseaseDefNum from manually selected pregnancy problem
					else if(pregCode=="none") {//if pref for default preg dx is 'none', make user choose a problem from list
						using FormDiseaseDefs formDiseaseDefs=new FormDiseaseDefs();
						formDiseaseDefs.IsSelectionMode=true;
						formDiseaseDefs.IsMultiSelect=false;
						formDiseaseDefs.ShowDialog();
						if(formDiseaseDefs.DialogResult!=DialogResult.OK) {
							checkPregnant.Checked=false;
							textPregCode.Clear();
							textPregCodeDescript.Clear();
							labelPregNotice.Visible=false;
							butChangeDefault.Text="Change Default";
							return;
						}
						labelPregNotice.Text=_pregManualText;
						//the list should only ever contain one item.
						_diseaseDefNumPreg=formDiseaseDefs.ListDiseaseDefsSelected[0].DiseaseDefNum;
					}
					#endregion
				}
			}
			#region Set description and code from DiseaseDefNum
			if(_diseaseDefNumPreg==0) {
				textPregCode.Clear();
				textPregCodeDescript.Clear();
				labelPregNotice.Visible=false;
				return;
			}
			diseaseDef=DiseaseDefs.GetItem(_diseaseDefNumPreg);
			if(diseaseDef.ICD9Code!="") {
				ICD9 icd9=ICD9s.GetByCode(diseaseDef.ICD9Code);
				if(icd9!=null) {
					pregCode=icd9.ICD9Code;
					descript=icd9.Description;
				}
			}
			else if(diseaseDef.Icd10Code!="") {
				Icd10 icd10=Icd10s.GetByCode(diseaseDef.Icd10Code);
				if(icd10!=null) {
					pregCode=icd10.Icd10Code;
					descript=icd10.Description;
				}
			}
			else if(diseaseDef.SnomedCode!="") {
				Snomed snomed=Snomeds.GetByCode(diseaseDef.SnomedCode);
				if(snomed!=null) {
					pregCode=snomed.SnomedCode;
					descript=snomed.Description;
				}
			}
			if(pregCode=="none" || pregCode=="") {
				descript=diseaseDef.DiseaseName;
			}
			#endregion
			textPregCode.Text=pregCode;
			textPregCodeDescript.Text=descript;
		}

		private void FillBMICodeLists() {
			bool isInLoincTable=true;
			List<Loinc> listLoincs=Vitalsigns.GetLoincsBMI();
			if(listLoincs.Count<3) {
				isInLoincTable=false;
			}
			_listLoincsBMICodes.AddRange(listLoincs);
			listLoincs=Vitalsigns.GetLoincsHeight();
			if(listLoincs.Count<6) {
				isInLoincTable=false;
			}
			_listLoincsHeightCodes.AddRange(listLoincs);
			listLoincs=Vitalsigns.GetLoincsWeight();
			if(listLoincs.Count<6) {
				isInLoincTable=false;
			}
			_listLoincsWeightCodes.AddRange(listLoincs);
			if(!isInLoincTable) {
				MsgBox.Show(this,"The LOINC table does not contain one or more codes used to report vitalsign exam statistics.  The LOINC table should be updated by running the Code System Importer tool found in Setup | Chart | EHR.");
			}
		}

		private void CalcBMI() {
			labelWeightCode.Text="";
			//BMI = (lbs*703)/(in^2)
			float height;
			float weight;
			try {
				height=float.Parse(textHeight.Text);
				weight=float.Parse(textWeight.Text);
			}
			catch {
				textBMIExamCode.Text="";
				return;
			}
			if(height<=0) {
				textBMIExamCode.Text="";
				return;
			}
			if(weight<=0) {
				textBMIExamCode.Text="";
				return;
			}
			float bmi=Vitalsigns.CalcBMI(weight,height);// ((float)(weight*703)/(height*height));
			textBMI.Text=bmi.ToString("n1");
			labelWeightCode.Text=CalcOverUnderBMIHelper(bmi);
			int bmiPercentile=-1;
			if(AgeBeforeJanFirst<17 && AgeBeforeJanFirst>2) {//calc and show BMI percentile if patient is >= 3 and < 17 on 01/01 of the year of the exam
				bmiPercentile=Vitalsigns.GetBMIPercentile(bmi,_patient,PIn.Date(textDateTaken.Text),_listGenderAge_LMSs);
			}
			if(bmiPercentile>-1) {
				labelBMIPercentile.Visible=true;
				textBMIPercentile.Visible=true;
				labelBMIPercentileCode.Visible=true;
				textBMIPercentileCode.Visible=true;
				textBMIPercentile.Text=bmiPercentile.ToString();
				if(Loincs.GetByCode("59576-9")!=null) {
					textBMIPercentileCode.Text="LOINC 59576-9";//Body mass index (BMI) [Percentile] Per age and gender, only code we will allow for percentile
				}
			}
			if(bmiPercentile==-1) {
				labelBMIPercentile.Visible=false;
				textBMIPercentile.Visible=false;
				labelBMIPercentileCode.Visible=false;
				textBMIPercentileCode.Visible=false;
				textBMIPercentile.Text=bmiPercentile.ToString();//We will set vitalsign.BMIPercentile=-1 if they are not in the age range or if there is an error calculating BMI percentile
			}
			bool isIntGroupVisible=false;
			string childGroupLabel="Child Counseling for Nutrition and Physical Activity";
			string overUnderGroupLabel="Intervention for BMI Above or Below Normal";
			if(AgeBeforeJanFirst<17) {
				isIntGroupVisible=true;
				if(groupInterventions.Text!=childGroupLabel) {
					groupInterventions.Text=childGroupLabel;
				}
				FillGridInterventions(true);
			}
			else if(AgeBeforeJanFirst>=18 && labelWeightCode.Text!="") {//if 18 or over and given an over/underweight code due to BMI
				isIntGroupVisible=true;
				if(groupInterventions.Text!=overUnderGroupLabel) {
					groupInterventions.Text=overUnderGroupLabel;
				}
				FillGridInterventions(false);
			}
			groupInterventions.Visible=isIntGroupVisible;
			if(Loincs.GetByCode("39156-5")!=null) {
				textBMIExamCode.Text="LOINC 39156-5";//This is the only code allowed for the BMI procedure.  It is not stored with this vitalsign object, we will display it if they have the code in the loinc table and will calculate CQM's with the assumption that all vitalsign objects with valid height and weight are this code if they have it in the LOINC table.
			}
			return;
		}

		private string CalcOverUnderBMIHelper(float bmi) {
			_interventionCodeSet=Vitalsigns.GetBMIInterventionCode(bmi,AgeBeforeJanFirst);
			switch(_interventionCodeSet) {
				case InterventionCodeSet.BelowNormalWeight:
					return "Underweight";
				case InterventionCodeSet.AboveNormalWeight:
					return "Overweight";
				case InterventionCodeSet.Nutrition:
				case InterventionCodeSet.None:
				default:
					return "";
			}
			//do not save to DB until butOK_Click
		}

		private void FillGridInterventions(bool isChild) {
			DateTime dateExam=PIn.Date(textDateTaken.Text);//this may be different than the saved VitalsignCur.DateTaken if user edited and has not hit ok to save
			#region GetInterventionsThatApply
			List<Intervention> listInterventions=new List<Intervention>();
			if(isChild) {
				listInterventions=Interventions.Refresh(VitalsignCur.PatNum,InterventionCodeSet.Nutrition);
				listInterventions.AddRange(Interventions.Refresh(VitalsignCur.PatNum,InterventionCodeSet.PhysicalActivity));
			}
			else {
				listInterventions=Interventions.Refresh(VitalsignCur.PatNum,InterventionCodeSet.AboveNormalWeight);
				listInterventions.AddRange(Interventions.Refresh(VitalsignCur.PatNum,InterventionCodeSet.BelowNormalWeight));
			}
			for(int i=listInterventions.Count-1;i>-1;i--) {
				if(listInterventions[i].DateEntry.Date<=dateExam.Date && listInterventions[i].DateEntry.Date>dateExam.AddMonths(-6).Date) {
					continue;
				}
				listInterventions.RemoveAt(i);
			}
			#endregion
			#region GetMedicationOrdersThatApply
			List<MedicationPat> listMedicationPats=new List<MedicationPat>();
			if(!isChild) {
				listMedicationPats=MedicationPats.Refresh(VitalsignCur.PatNum,true);
				for(int i=listMedicationPats.Count-1;i>-1;i--) {
					if(listMedicationPats[i].DateStart.Date<dateExam.AddMonths(-6).Date || listMedicationPats[i].DateStart.Date>dateExam.Date) {
						listMedicationPats.RemoveAt(i);
					}					
				}
				//if still meds that have start date within exam date and exam date -6 months, check the rxnorm against valid ehr meds
				if(listMedicationPats.Count>0) {
					List<EhrCode> listEhrCodes=EhrCodes.GetForValueSetOIDs(new List<string> { "2.16.840.1.113883.3.600.1.1498","2.16.840.1.113883.3.600.1.1499" },true);//Above Normal Medications RxNorm Value Set, Below Normal Medications RxNorm Value Set
					//listEhrMeds will only contain 7 medications for above/below normal weight and only if those exist in the rxnorm table
					for(int i=listMedicationPats.Count-1;i>-1;i--) {
						bool isFound=false;
						for(int j=0;j<listEhrCodes.Count;j++) {
							if(listMedicationPats[i].RxCui.ToString()==listEhrCodes[j].CodeValue) {
								isFound=true;
								break;
							}
						}
						if(!isFound) {
							listMedicationPats.RemoveAt(i);
						}						
					}
				}
			}
			#endregion
			gridInterventions.BeginUpdate();
			gridInterventions.Columns.Clear();
			GridColumn col=new GridColumn("Date",70);
			gridInterventions.Columns.Add(col);
			col=new GridColumn("Intervention Type",115);
			gridInterventions.Columns.Add(col);
			col=new GridColumn("Code Description",200);
			gridInterventions.Columns.Add(col);
			gridInterventions.ListGridRows.Clear();
			GridRow row;
			#region AddInterventionRows
			for(int i=0;i<listInterventions.Count;i++) {
				row=new GridRow();
				row.Cells.Add(listInterventions[i].DateEntry.ToShortDateString());
				row.Cells.Add(listInterventions[i].CodeSet.ToString());
				//Description of Intervention---------------------------------------------
				//to get description, first determine which table the code is from.  Interventions are allowed to be SNOMEDCT, ICD9, ICD10, HCPCS, or CPT.
				string descript="";
				switch(listInterventions[i].CodeSystem) {
					case "SNOMEDCT":
						Snomed snomed=Snomeds.GetByCode(listInterventions[i].CodeValue);
						if(snomed!=null) {
							descript=snomed.Description;
						}
						break;
					case "ICD9CM":
						ICD9 icd9=ICD9s.GetByCode(listInterventions[i].CodeValue);
						if(icd9!=null) {
							descript=icd9.Description;
						}
						break;
					case "ICD10CM":
						Icd10 icd10=Icd10s.GetByCode(listInterventions[i].CodeValue);
						if(icd10!=null) {
							descript=icd10.Description;
						}
						break;
					case "HCPCS":
						Hcpcs hcpcs=Hcpcses.GetByCode(listInterventions[i].CodeValue);
						if(hcpcs!=null) {
							descript=hcpcs.DescriptionShort;
						}
						break;
					case "CPT":
						Cpt cpt=Cpts.GetByCode(listInterventions[i].CodeValue);
						if(cpt!=null) {
							descript=cpt.Description;
						}
						break;
				}
				row.Cells.Add(descript);
				row.Tag=listInterventions[i];
				gridInterventions.ListGridRows.Add(row);
			}
			#endregion
			#region AddMedicationRows
			for(int i=0;i<listMedicationPats.Count;i++) {
				row=new GridRow();
				row.Cells.Add(listMedicationPats[i].DateStart.ToShortDateString());
				if(listMedicationPats[i].RxCui==314153 || listMedicationPats[i].RxCui==692876) {
					row.Cells.Add(InterventionCodeSet.AboveNormalWeight.ToString()+" Medication");
				}
				else {
					row.Cells.Add(InterventionCodeSet.BelowNormalWeight.ToString()+" Medication");
				}
				//Description of Medication----------------------------------------------
				//only meds in EHR table are from RxNorm table
				string descript=RxNorms.GetDescByRxCui(listMedicationPats[i].RxCui.ToString());
				row.Cells.Add(descript);
				row.Tag=listMedicationPats[i];
				gridInterventions.ListGridRows.Add(row);
			}
			#endregion
			gridInterventions.EndUpdate();
		}

		private void textWeight_TextChanged(object sender,EventArgs e) {
			CalcBMI();
		}

		private void textHeight_TextChanged(object sender,EventArgs e) {
			CalcBMI();
		}

		private void textBPs_TextChanged(object sender,EventArgs e) {
			if(textBPs.Text=="0") {
				textBPsExamCode.Text="";
				return;
			}
			try {
				int.Parse(textBPs.Text);
			}
			catch {
				textBPsExamCode.Text="";
				return;
			}
			if(Loincs.GetByCode("8480-6")!=null) {
				textBPsExamCode.Text="LOINC 8480-6";//This is the only code allowed for the BP Systolic exam.  It is not stored with this vitalsign object, we will display it if they have the code in the loinc table and will calculate CQM's with the assumption that all vitalsign objects with valid Systolic BP are this code if they have it in the LOINC table.
			}
		}

		private void textBPd_TextChanged(object sender,EventArgs e) {
			if(textBPd.Text=="0") {
				textBPdExamCode.Text="";
				return;
			}
			try {
				int.Parse(textBPd.Text);
			}
			catch {
				textBPdExamCode.Text="";
				return;
			}
			if(Loincs.GetByCode("8462-4")!=null) {
				textBPdExamCode.Text="LOINC 8462-4";//This is the only code allowed for the BP Diastolic exam.  It is not stored with this vitalsign object, we will display it if they have the code in the loinc table and will calculate CQM's with the assumption that all vitalsign objects with valid Diastolic BP are this code if they have it in the LOINC table.
			}
		}

		///<summary>If they change the date of the exam and it is attached to a pregnancy problem and the date is now outside the active dates of the problem, tell them you are removing the problem and unchecking the pregnancy box.</summary>
		private void textDateTaken_Leave(object sender,EventArgs e) {
			DateTime dateExam=PIn.Date(textDateTaken.Text);
			AgeBeforeJanFirst=dateExam.Year-_patient.Birthdate.Year-1;//This is how old this patient was before any birthday in the year the vital sign was taken, can be negative if patient born the year taken or if value in textDateTaken is empty or not a valid date
			if(!checkPregnant.Checked || VitalsignCur.PregDiseaseNum==0) {
				CalcBMI();//This will use new year taken to determine age at start of that year to show over/underweight if applicable using age specific criteria
				return;
			}
			Disease disease=Diseases.GetOne(VitalsignCur.PregDiseaseNum);
			if(disease!=null) {//the currently attached preg disease is valid, will be null if they checked the box on new exam but haven't hit ok to save
				if(dateExam.Date<disease.DateStart || (disease.DateStop.Year>1880 && disease.DateStop<dateExam.Date)) {//if this exam is not in the active date range of the problem
					if(!MsgBox.Show(this,MsgBoxButtons.YesNo,@"The exam date is no longer within the active dates of the attached pregnancy diagnosis.
Do you want to remove the pregnancy diagnosis?")) 
					{
					 textDateTaken.Focus();
					 return;
					}
				}
				else {
					CalcBMI();
					return;
				}
			}
			//if we get here, either the pregnant check box is not checked, there is not a currently attached preg disease, there is an attached disease but this exam is no longer in the active dates and the user said to remove it, or the current PregDiseaseNum is invalid
			VitalsignCur.PregDiseaseNum=0;
			checkPregnant.Checked=false;
			labelPregNotice.Visible=false;
			textPregCode.Clear();
			textPregCodeDescript.Clear();
			butChangeDefault.Text="Change Default";
			CalcBMI();
		}

		private void checkPregnant_Click(object sender,EventArgs e) {
			labelPregNotice.Visible=false;
			butChangeDefault.Text="Change Default";
			textPregCode.Clear();
			textPregCodeDescript.Clear();
			if(!checkPregnant.Checked) {
				return;
			}
			SetPregCodeAndDescript();
			if(_diseaseDefNumPreg==0) {
				checkPregnant.Checked=false;
				return;
			}
			if(VitalsignCur.PregDiseaseNum>0) {//if the current vital sign exam linked to a pregnancy dx set the Change Default button to "To Problem" so they can modify the existing problem.
				butChangeDefault.Text="Go to Problem";
			}
			else {//only show warning if we are now attached to a preg dx, add it is not a previously existing problem so we have to insert it.
				labelPregNotice.Visible=true;
			}
		}

		private void butChangeDefault_Click(object sender,EventArgs e) {
			if(butChangeDefault.Text!="Go to Problem") {
				if(!Security.IsAuthorized(EnumPermType.SecurityAdmin,false)) {
					return;
				}
				using FormEhrSettings formEhrSettings=new FormEhrSettings();
				formEhrSettings.ShowDialog();
				if(formEhrSettings.DialogResult!=DialogResult.OK || checkPregnant.Checked==false) {
					return;
				}
				labelPregNotice.Visible=false;
				SetPregCodeAndDescript();
				if(_diseaseDefNumPreg>0) {
					labelPregNotice.Visible=true;
				}
				return;
			}
			//text is "To Problem" only when vital sign has a valid PregDiseaseNum and the preg box is checked
			Disease disease=Diseases.GetOne(VitalsignCur.PregDiseaseNum);
			if(disease==null) {//should never happen, the only way the button will say "To Problem" is if this exam is pointing to a valid problem
				butChangeDefault.Text="Change Default";
				labelPregNotice.Visible=false;
				textPregCode.Clear();
				textPregCodeDescript.Clear();
				VitalsignCur.PregDiseaseNum=0;
				checkPregnant.Checked=false;
				return;
			}
			if(DiseaseDefs.GetItem(disease.DiseaseDefNum)==null) {
				MessageBox.Show(Lan.g(this,"Invalid disease.  Please run database maintenance method")+" "
					+nameof(DatabaseMaintenances.DiseaseWithInvalidDiseaseDef));
				return;
			}
			FrmDiseaseEdit frmDiseaseEdit=new FrmDiseaseEdit(disease);
			frmDiseaseEdit.IsNew=false;
			frmDiseaseEdit.ShowDialog();
			if(frmDiseaseEdit.IsDialogOK) {
				VitalsignCur.PregDiseaseNum=Vitalsigns.GetOne(VitalsignCur.VitalsignNum).PregDiseaseNum;//if unlinked in FormDiseaseEdit, refresh PregDiseaseNum from db
				if(VitalsignCur.PregDiseaseNum==0) {
					butChangeDefault.Text="Change Default";
					labelPregNotice.Visible=false;
					textPregCode.Clear();
					textPregCodeDescript.Clear();
					checkPregnant.Checked=false;
					return;
				}
				SetPregCodeAndDescript();
				if(_diseaseDefNumPreg==0) {
					labelPregNotice.Visible=false;
					butChangeDefault.Text="Change Default";
				}
			}
		}

		private void checkNotPerf_Click(object sender,EventArgs e) {
			if(!checkNotPerf.Checked) {
				textReasonCode.Clear();
				textReasonDescript.Clear();
				return;
			}
			using FormEhrNotPerformedEdit formEhrNotPerformedEdit=new FormEhrNotPerformedEdit();
			if(VitalsignCur.EhrNotPerformedNum==0) {
				formEhrNotPerformedEdit.EhrNotPerfCur=new EhrNotPerformed();
				formEhrNotPerformedEdit.EhrNotPerfCur.IsNew=true;
				formEhrNotPerformedEdit.EhrNotPerfCur.PatNum=_patient.PatNum;
				formEhrNotPerformedEdit.EhrNotPerfCur.ProvNum=_patient.PriProv;
				formEhrNotPerformedEdit.SelectedItemIndex=(int)EhrNotPerformedItem.BMIExam;//The code and code value will be set in FormEhrNotPerformedEdit, set the selected index to the EhrNotPerformedItem enum index for BMIExam
				formEhrNotPerformedEdit.EhrNotPerfCur.DateEntry=PIn.Date(textDateTaken.Text);
				formEhrNotPerformedEdit.IsDateReadOnly=true;//if this not performed item will be linked to this exam, force the dates to match.  User can change exam date and recheck the box to affect the not performed item date, but forcing them to be the same will allow us to avoid other complications.
			}
			else {
				formEhrNotPerformedEdit.EhrNotPerfCur=EhrNotPerformeds.GetOne(VitalsignCur.EhrNotPerformedNum);
				formEhrNotPerformedEdit.EhrNotPerfCur.IsNew=false;
				formEhrNotPerformedEdit.SelectedItemIndex=(int)EhrNotPerformedItem.BMIExam;
			}
			formEhrNotPerformedEdit.ShowDialog();
			if(formEhrNotPerformedEdit.DialogResult!=DialogResult.OK) {
				checkNotPerf.Checked=false;
				textReasonCode.Clear();
				textReasonDescript.Clear();
				if(EhrNotPerformeds.GetOne(VitalsignCur.EhrNotPerformedNum)==null) {//could be linked to a not performed item that no longer exists or has been deleted
					VitalsignCur.EhrNotPerformedNum=0;
				}
				return;
			}
			VitalsignCur.EhrNotPerformedNum=formEhrNotPerformedEdit.EhrNotPerfCur.EhrNotPerformedNum;
			textReasonCode.Text=formEhrNotPerformedEdit.EhrNotPerfCur.CodeValueReason;
			Snomed snomed=Snomeds.GetByCode(formEhrNotPerformedEdit.EhrNotPerfCur.CodeValueReason);
			if(snomed!=null) {
				textReasonDescript.Text=snomed.Description;
			}
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormInterventionEdit formInterventionEdit=new FormInterventionEdit();
			formInterventionEdit.InterventionCur=new Intervention();
			formInterventionEdit.InterventionCur.IsNew=true;
			formInterventionEdit.InterventionCur.PatNum=_patient.PatNum;
			formInterventionEdit.InterventionCur.ProvNum=_patient.PriProv;
			formInterventionEdit.InterventionCur.DateEntry=PIn.Date(textDateTaken.Text);
			formInterventionEdit.InterventionCur.CodeSet=_interventionCodeSet;
			formInterventionEdit.IsAllTypes=false;
			formInterventionEdit.IsSelectionMode=true;
			formInterventionEdit.ShowDialog();
			if(formInterventionEdit.DialogResult==DialogResult.OK) {
				bool isChild=AgeBeforeJanFirst<17;
				FillGridInterventions(isChild);
			}
		}

		private void gridInterventions_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			object object_=gridInterventions.ListGridRows[e.Row].Tag;
			if(object_ is Intervention intervention) { //grid can contain MedicationPat or Intervention objects, launch appropriate window
				using FormInterventionEdit formInterventionEdit=new FormInterventionEdit();
				formInterventionEdit.InterventionCur=intervention;
				formInterventionEdit.IsAllTypes=false;
				formInterventionEdit.IsSelectionMode=false;
				formInterventionEdit.ShowDialog();
			}
			if(object_ is MedicationPat medicationPat) { 
				using FormMedPat formMedPat=new FormMedPat();
				formMedPat.MedicationPatCur=medicationPat;
				formMedPat.IsNew=false;
				formMedPat.ShowDialog();
			}
			FillGridInterventions(AgeBeforeJanFirst<17);
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(VitalsignCur.IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")) {
				return;
			}
			if(VitalsignCur.EhrNotPerformedNum!=0) {
				EhrNotPerformeds.Delete(VitalsignCur.EhrNotPerformedNum);
			}
			Vitalsigns.Delete(VitalsignCur.VitalsignNum);
			DialogResult=DialogResult.Cancel;
		}

		private void butSave_Click(object sender,EventArgs e) {
			#region Validate
			DateTime date;
			if(textDateTaken.Text=="") {
				MsgBox.Show(this,"Please enter a date.");
				return;
			}
			try {
				date=DateTime.Parse(textDateTaken.Text);
			}
			catch {
				MsgBox.Show(this,"Please fix date first.");
				return;
			}
			if(date<_patient.Birthdate) {
				MsgBox.Show(this,"Exam date cannot be before the patient's birthdate.");
				return;
			}
			//validate height
			float height=0;
			try {
				if(textHeight.Text!="") {
					height = float.Parse(textHeight.Text);
				}
			}
			catch {
				MsgBox.Show(this,"Please fix height first.");
				return;
			}
			if(height<0) {
				MsgBox.Show(this,"Please fix height first.");
				return;
			}
			//validate weight
			float weight=0;
			try {
				if(textWeight.Text!="") {
					weight = float.Parse(textWeight.Text);
				}
			}
			catch {
				MsgBox.Show(this,"Please fix weight first.");
				return;
			}
			if(weight<0) {
				MsgBox.Show(this,"Please fix weight first.");
				return;
			}
			//validate bp
			int bpSys=0;
			int bpDia=0;
			try {
				if(textBPs.Text!="") {
					bpSys=int.Parse(textBPs.Text);
				}
				if(textBPd.Text!="") {
					bpDia=int.Parse(textBPd.Text);
				}
			}
			catch {
				MsgBox.Show(this,"Please fix BP first.");
				return;
			}
			if(bpSys<0 || bpDia<0) {
				MsgBox.Show(this,"Please fix BP first.");
				return;
			}
			//validate pulse
			int pulse=0;
			try {
				if(textPulse.Text!="") {
					pulse=int.Parse(textPulse.Text);
				}
			}
			catch {
				MsgBox.Show(this,"Please fix Pulse first.");
				return;
			}
			if(pulse<0) {
				MsgBox.Show(this,"Please fix Pulse first.");
				return;
			}
			Loinc loincSelectedHeightCode=comboHeightExamCode.GetSelected<Loinc>();
			if(loincSelectedHeightCode==null) {
				MsgBox.Show(this,"Please select Height code first.");
				return;
			}
			Loinc loincSelectedWeightCode=comboWeightExamCode.GetSelected<Loinc>();
			if(loincSelectedWeightCode==null) {
				MsgBox.Show(this,"Please select Weight code first.");
				return;
			}
			#endregion
			#region Save
			VitalsignCur=Vitalsigns.SetFields(VitalsignCur,date,pulse,height,weight,bpDia,bpSys,PIn.Int(textBMIPercentile.Text),loincSelectedHeightCode,loincSelectedWeightCode);
			VitalsignCur.WeightCode=Vitalsigns.SetWeightCodes(_interventionCodeSet);
			#region PregnancyDx
			if(checkPregnant.Checked) {//pregnant, add pregnant dx if necessary
				try {
					VitalsignCur.PregDiseaseNum=Vitalsigns.SetPregnancyDisease(VitalsignCur,_diseaseDefNumPreg);
				}
				catch(Exception ex) {
					MsgBox.Show(this,ex.Message);
					checkPregnant.Checked=false;
					textPregCode.Clear();
					textPregCodeDescript.Clear();
					labelPregNotice.Visible=false;
					VitalsignCur.PregDiseaseNum=0;
					butChangeDefault.Text="Change Default";
					return;
				}
			}
			else {//checkPregnant not checked
				VitalsignCur.PregDiseaseNum=0;
			}
			#endregion
			#region EhrNotPerformed
			if(!checkNotPerf.Checked) {
				if(VitalsignCur.EhrNotPerformedNum!=0) {
					EhrNotPerformeds.Delete(VitalsignCur.EhrNotPerformedNum);
					VitalsignCur.EhrNotPerformedNum=0;
				}
			}
			if(VitalsignCur.IsNew) {
				Vitalsigns.Insert(VitalsignCur);
			}
			else {
				Vitalsigns.Update(VitalsignCur);
			}
			#endregion
			#endregion
			#region CDS Intervention Trigger
			if(CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowCDS && CDSPermissions.GetForUser(Security.CurUser.UserNum).VitalCDS) {
				using FormCDSIntervention formCDSIntervention=new FormCDSIntervention();
				formCDSIntervention.ListCDSInterventions=EhrTriggers.TriggerMatch(VitalsignCur,Patients.GetPat(VitalsignCur.PatNum));
				formCDSIntervention.ShowIfRequired();
			}
			#endregion
			DialogResult=DialogResult.OK;
		}

	}
}
