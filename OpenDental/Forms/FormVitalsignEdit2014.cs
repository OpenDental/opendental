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
		private Patient patCur;
		public int ageBeforeJanFirst;
		private List<Loinc> listHeightCodes;
		private List<Loinc> listWeightCodes;
		private List<Loinc> listBMICodes;
		private long pregDisDefNumCur;//used to keep track of the def we will update VitalsignCur.PregDiseaseNum with
		private string pregDefaultText;
		private string pregManualText;
		private InterventionCodeSet intervCodeSet;
		private Dictionary<string,List<float>> _dictLMS;

		public FormVitalsignEdit2014() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormVitalsignEdit2014_Load(object sender,EventArgs e) {
			FillDictLMS();
			pregDefaultText="A diagnosis of pregnancy with this code will be added to the patient's medical history with a start date equal to the date of this exam.";
			pregManualText="Selecting a code that is not in the recommended list of pregnancy codes may not exclude this patient from certain CQM calculations.";
			labelPregNotice.Text=pregDefaultText;
			if(VitalsignCur.Pulse!=0) {
				textPulse.Text=VitalsignCur.Pulse.ToString();
			}
			#region SetHWBPAndVisibility
			pregDisDefNumCur=0;
			groupInterventions.Visible=false;
			patCur=Patients.GetPat(VitalsignCur.PatNum);
			textDateTaken.Text=VitalsignCur.DateTaken.ToShortDateString();
			ageBeforeJanFirst=PIn.Date(textDateTaken.Text).Year-patCur.Birthdate.Year-1;
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
			listHeightCodes=new List<Loinc>();
			listWeightCodes=new List<Loinc>();
			listBMICodes=new List<Loinc>();
			FillBMICodeLists();
			textBMIPercentileCode.Text=VitalsignCur.BMIExamCode;//Only ever going to be blank or LOINC 59576-9 - Body mass index (BMI) [Percentile] Per age and gender
			comboHeightExamCode.Items.Add("none");
			comboHeightExamCode.SelectedIndex=0;
			for(int i=0;i<listHeightCodes.Count;i++) {
				if(listHeightCodes[i].NameShort=="" || listHeightCodes[i].NameLongCommon.Length<30) {//30 is roughly the number of characters that will fit in the combo box
					comboHeightExamCode.Items.Add(listHeightCodes[i].NameLongCommon);
				}
				else {
					comboHeightExamCode.Items.Add(listHeightCodes[i].NameShort);
				}
				if(i==0 || VitalsignCur.HeightExamCode==listHeightCodes[i].LoincCode) {
					comboHeightExamCode.SelectedIndex=i+1;
				}
			}
			comboWeightExamCode.Items.Add("none");
			comboWeightExamCode.SelectedIndex=0;
			for(int i=0;i<listWeightCodes.Count;i++) {
				if(listWeightCodes[i].NameShort=="" || listWeightCodes[i].NameLongCommon.Length<30) {//30 is roughly the number of characters that will fit in the combo box
					comboWeightExamCode.Items.Add(listWeightCodes[i].NameLongCommon);
				}
				else {
					comboWeightExamCode.Items.Add(listWeightCodes[i].NameShort);
				}
				if(i==0 || VitalsignCur.WeightExamCode==listWeightCodes[i].LoincCode) {
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
				else if(pregDisDefNumCur>0) {
					labelPregNotice.Visible=true;
					butChangeDefault.Text="Go to Problem";
				}
			}
			#endregion
			#region SetEhrNotPerformedReasonCodeAndDescript
			if(VitalsignCur.EhrNotPerformedNum>0) {
				checkNotPerf.Checked=true;
				EhrNotPerformed notPerfCur=EhrNotPerformeds.GetOne(VitalsignCur.EhrNotPerformedNum);
				if(notPerfCur==null) {
					VitalsignCur.EhrNotPerformedNum=0;//if this vital sign is pointing to an EhrNotPerformed item that no longer exists, we will just remove the pointer
					checkNotPerf.Checked=false;
				}
				else {
					textReasonCode.Text=notPerfCur.CodeValueReason;
					//all reasons not performed are snomed codes
					Snomed sCur=Snomeds.GetByCode(notPerfCur.CodeValueReason);
					if(sCur!=null) {
						textReasonDescript.Text=sCur.Description;
					}
				}
			}
			#endregion
		}

		///<summary>Sets the pregnancy code and description text box with either the attached pregnancy dx if exists or the default preg dx set in FormEhrSettings or a manually selected def.  If the pregnancy diseasedef with the default pregnancy code and code system does not exist, it will be inserted.  The pregnancy problem will be inserted when closing if necessary.</summary>
		private void SetPregCodeAndDescript() {
			labelPregNotice.Text=pregDefaultText;
			pregDisDefNumCur=0;//this will be set to the correct problem def at the end of this function and will be the def of the problem we will insert/attach this exam to
			string pregCode="";
			string descript="";
			Disease disCur=null;
			DiseaseDef disdefCur=null;
			DateTime examDate=PIn.Date(textDateTaken.Text);//this may be different than the saved Vitalsign.DateTaken if user edited
			#region Get DiseaseDefNum from attached pregnancy problem
			if(VitalsignCur.PregDiseaseNum>0) {//already pointing to a disease, get that one
				disCur=Diseases.GetOne(VitalsignCur.PregDiseaseNum);//get disease this vital sign is pointing to, see if it exists
				if(disCur==null) {
					VitalsignCur.PregDiseaseNum=0;
				}
				else {
					if(examDate.Year<1880 || disCur.DateStart>examDate.Date || (disCur.DateStop.Year>1880 && disCur.DateStop<examDate.Date)) {
						VitalsignCur.PregDiseaseNum=0;
						disCur=null;
					}
					else {
						disdefCur=DiseaseDefs.GetItem(disCur.DiseaseDefNum);
						if(disdefCur==null) {
							VitalsignCur.PregDiseaseNum=0;
							disCur=null;
						}
						else {//disease points to valid def
							pregDisDefNumCur=disdefCur.DiseaseDefNum;
						}
					}
				}
			}
			#endregion
			if(VitalsignCur.PregDiseaseNum==0) {//not currently attached to a disease
				#region Get DiseaseDefNum from existing pregnancy problem
				if(examDate.Year>1880) {//only try to find existing problem if a valid exam date is entered before checking the box, otherwise we do not know what date to compare to the active dates of the pregnancy dx
					List<DiseaseDef> listPregDisDefs=DiseaseDefs.GetAllPregDiseaseDefs();
					List<Disease> listPatDiseases=Diseases.Refresh(VitalsignCur.PatNum,true);
					for(int i=0;i<listPatDiseases.Count;i++) {//loop through all diseases for this patient, shouldn't be very many
						if(listPatDiseases[i].DateStart>examDate.Date //startdate for current disease is after the exam date set in form
							|| (listPatDiseases[i].DateStop.Year>1880 && listPatDiseases[i].DateStop<examDate.Date))//or current disease has a stop date and stop date before exam date
						{
							continue;
						}
						for(int j=0;j<listPregDisDefs.Count;j++) {//loop through preg disease defs in the db, shouldn't be very many
							if(listPatDiseases[i].DiseaseDefNum!=listPregDisDefs[j].DiseaseDefNum) {//see if this problem is a pregnancy problem
								continue;
							}
							if(disCur==null || listPatDiseases[i].DateStart>disCur.DateStart) {//if we haven't found a disease match yet or this match is more recent (later start date)
								disCur=listPatDiseases[i];
								break;
							}
						}
					}
				}
				if(disCur!=null) {
					pregDisDefNumCur=disCur.DiseaseDefNum;
					VitalsignCur.PregDiseaseNum=disCur.DiseaseNum;
				}
				#endregion
				else {//we are going to insert either the default pregnancy problem or a manually selected problem
					#region Get DiseaseDefNum from global default pregnancy problem
					//if preg dx doesn't exist, use the default pregnancy code if set to something other than blank or 'none'
					pregCode=PrefC.GetString(PrefName.PregnancyDefaultCodeValue);//could be 'none' which disables the automatic dx insertion
					string pregCodeSys=PrefC.GetString(PrefName.PregnancyDefaultCodeSystem);//if 'none' for code, code system will default to 'SNOMEDCT', display will be ""
					if(pregCode!="" && pregCode!="none") {//default pregnancy code set to a code other than 'none', should never be blank, we set in ConvertDB and don't allow blank
						pregDisDefNumCur=DiseaseDefs.GetNumFromCode(pregCode);//see if the code is attached to a valid diseasedef
						if(pregDisDefNumCur==0) {//no diseasedef in db for the default code, create and insert def
							disdefCur=new DiseaseDef();
							disdefCur.DiseaseName="Pregnant";
							switch(pregCodeSys) {
								case "ICD9CM":
									disdefCur.ICD9Code=pregCode;
									break;
								case "ICD10CM":
									disdefCur.Icd10Code=pregCode;
									break;
								case "SNOMEDCT":
									disdefCur.SnomedCode=pregCode;
									break;
							}
							pregDisDefNumCur=DiseaseDefs.Insert(disdefCur);
							DiseaseDefs.RefreshCache();
							DataValid.SetInvalid(InvalidType.Diseases);
							SecurityLogs.MakeLogEntry(Permissions.ProblemEdit,0,disdefCur.DiseaseName+" added.");
						}
					}
					#endregion
					#region Get DiseaseDefNum from manually selected pregnancy problem
					else if(pregCode=="none") {//if pref for default preg dx is 'none', make user choose a problem from list
						using FormDiseaseDefs FormDD=new FormDiseaseDefs();
						FormDD.IsSelectionMode=true;
						FormDD.IsMultiSelect=false;
						FormDD.ShowDialog();
						if(FormDD.DialogResult!=DialogResult.OK) {
							checkPregnant.Checked=false;
							textPregCode.Clear();
							textPregCodeDescript.Clear();
							labelPregNotice.Visible=false;
							butChangeDefault.Text="Change Default";
							return;
						}
						labelPregNotice.Text=pregManualText;
						//the list should only ever contain one item.
						pregDisDefNumCur=FormDD.ListDiseaseDefsSelected[0].DiseaseDefNum;
					}
					#endregion
				}
			}
			#region Set description and code from DiseaseDefNum
			if(pregDisDefNumCur==0) {
				textPregCode.Clear();
				textPregCodeDescript.Clear();
				labelPregNotice.Visible=false;
				return;
			}
			disdefCur=DiseaseDefs.GetItem(pregDisDefNumCur);
			if(disdefCur.ICD9Code!="") {
				ICD9 i9Preg=ICD9s.GetByCode(disdefCur.ICD9Code);
				if(i9Preg!=null) {
					pregCode=i9Preg.ICD9Code;
					descript=i9Preg.Description;
				}
			}
			else if(disdefCur.Icd10Code!="") {
				Icd10 i10Preg=Icd10s.GetByCode(disdefCur.Icd10Code);
				if(i10Preg!=null) {
					pregCode=i10Preg.Icd10Code;
					descript=i10Preg.Description;
				}
			}
			else if(disdefCur.SnomedCode!="") {
				Snomed sPreg=Snomeds.GetByCode(disdefCur.SnomedCode);
				if(sPreg!=null) {
					pregCode=sPreg.SnomedCode;
					descript=sPreg.Description;
				}
			}
			if(pregCode=="none" || pregCode=="") {
				descript=disdefCur.DiseaseName;
			}
			#endregion
			textPregCode.Text=pregCode;
			textPregCodeDescript.Text=descript;
		}

		private void FillBMICodeLists() {
			bool isInLoincTable=true;
			//The list returned will only contain the Loincs that are actually in the loinc table.
			List<Loinc> listLoincs=Loincs.GetForCodeList("59574-4,59575-1,59576-9");//Body mass index (BMI) [Percentile],Body mass index (BMI) [Percentile] Per age,Body mass index (BMI) [Percentile] Per age and gender
			if(listLoincs.Count<3) {
				isInLoincTable=false;
			}
			listBMICodes.AddRange(listLoincs);
			listLoincs=Loincs.GetForCodeList("8302-2,3137-7,3138-5,8306-3,8307-1,8308-9");//Body height,Body height Measured,Body height Stated,Body height --lying,Body height --pre surgery,Body height --standing
			if(listLoincs.Count<6) {
				isInLoincTable=false;
			}
			listHeightCodes.AddRange(listLoincs);
			listLoincs=Loincs.GetForCodeList("29463-7,18833-4,3141-9,3142-7,8350-1,8351-9");//Body weight,First Body weight,Body weight Measured,Body weight Stated,Body weight Measured --with clothes,Body weight Measured --without clothes
			if(listLoincs.Count<6) {
				isInLoincTable=false;
			}
			listWeightCodes.AddRange(listLoincs);
			if(!isInLoincTable) {
				MsgBox.Show(this,"The LOINC table does not contain one or more codes used to report vitalsign exam statistics.  The LOINC table should be updated by running the Code System Importer tool found in Setup | Chart | EHR.");
			}
		}

		///<summary>Fills the dictionary with gender (m or f) and age in months as the key and a list of three longs, the L=power of Box-Cox transformation, M=median, and S=generalized coefficient of variation, as the value.  The L, M, and S values are from the CDC website http://www.cdc.gov/nchs/data/series/sr_11/sr11_246.pdf page 178-186.</summary>
		private void FillDictLMS() {
			_dictLMS=new Dictionary<string,List<float>>();
			_dictLMS.Add("m36",new List<float>() { -1.419991255f,16.00030401f,0.072634432f });
			_dictLMS.Add("m37",new List<float>() { -1.404277619f,15.96304277f,0.072327649f });
			_dictLMS.Add("m38",new List<float>() { -1.39586317f,15.92695418f,0.07206864f });
			_dictLMS.Add("m39",new List<float>() { -1.394935252f,15.89202582f,0.071856805f });
			_dictLMS.Add("m40",new List<float>() { -1.401671596f,15.85824093f,0.071691278f });
			_dictLMS.Add("m41",new List<float>() { -1.416100312f,15.82558822f,0.071571093f });
			_dictLMS.Add("m42",new List<float>() { -1.438164899f,15.79405728f,0.071495113f });
			_dictLMS.Add("m43",new List<float>() { -1.467669032f,15.76364255f,0.071462106f });
			_dictLMS.Add("m44",new List<float>() { -1.504376347f,15.73433668f,0.071470646f });
			_dictLMS.Add("m45",new List<float>() { -1.547942838f,15.70613566f,0.071519218f });
			_dictLMS.Add("m46",new List<float>() { -1.597896397f,15.67904062f,0.071606277f });
			_dictLMS.Add("m47",new List<float>() { -1.653732283f,15.65305192f,0.071730167f });
			_dictLMS.Add("m48",new List<float>() { -1.714869347f,15.62817269f,0.071889214f });
			_dictLMS.Add("m49",new List<float>() { -1.780673181f,15.604408f,0.072081737f });
			_dictLMS.Add("m50",new List<float>() { -1.850468473f,15.58176458f,0.072306081f });
			_dictLMS.Add("m51",new List<float>() { -1.923551865f,15.56025067f,0.072560637f });
			_dictLMS.Add("m52",new List<float>() { -1.999220429f,15.5398746f,0.07284384f });
			_dictLMS.Add("m53",new List<float>() { -2.076707178f,15.52064993f,0.073154324f });
			_dictLMS.Add("m54",new List<float>() { -2.155348017f,15.50258427f,0.073490667f });
			_dictLMS.Add("m55",new List<float>() { -2.234438552f,15.48568973f,0.073851672f });
			_dictLMS.Add("m56",new List<float>() { -2.313321723f,15.46997718f,0.074236235f });
			_dictLMS.Add("m57",new List<float>() { -2.391381273f,15.45545692f,0.074643374f });
			_dictLMS.Add("m58",new List<float>() { -2.468032491f,15.44213961f,0.075072264f });
			_dictLMS.Add("m59",new List<float>() { -2.542781541f,15.43003207f,0.075522104f });
			_dictLMS.Add("m60",new List<float>() { -2.61516595f,15.41914163f,0.07599225f });
			_dictLMS.Add("m61",new List<float>() { -2.684789516f,15.40947356f,0.076482128f });
			_dictLMS.Add("m62",new List<float>() { -2.751316949f,15.40103139f,0.076991232f });
			_dictLMS.Add("m63",new List<float>() { -2.81445945f,15.39381785f,0.077519149f });
			_dictLMS.Add("m64",new List<float>() { -2.87402476f,15.38783094f,0.07806539f });
			_dictLMS.Add("m65",new List<float>() { -2.92984048f,15.38306945f,0.078629592f });
			_dictLMS.Add("m66",new List<float>() { -2.981796828f,15.37952958f,0.079211369f });
			_dictLMS.Add("m67",new List<float>() { -3.029831343f,15.37720582f,0.079810334f });
			_dictLMS.Add("m68",new List<float>() { -3.073924224f,15.37609107f,0.080426086f });
			_dictLMS.Add("m69",new List<float>() { -3.114093476f,15.37617677f,0.081058206f });
			_dictLMS.Add("m70",new List<float>() { -3.15039004f,15.37745304f,0.081706249f });
			_dictLMS.Add("m71",new List<float>() { -3.182893018f,15.37990886f,0.082369741f });
			_dictLMS.Add("m72",new List<float>() { -3.21170511f,15.38353217f,0.083048178f });
			_dictLMS.Add("m73",new List<float>() { -3.23694834f,15.38831005f,0.083741021f });
			_dictLMS.Add("m74",new List<float>() { -3.25876011f,15.39422883f,0.0844477f });
			_dictLMS.Add("m75",new List<float>() { -3.277281546f,15.40127496f,0.085167651f });
			_dictLMS.Add("m76",new List<float>() { -3.292683774f,15.40943252f,0.085900184f });
			_dictLMS.Add("m77",new List<float>() { -3.305124073f,15.41868691f,0.086644667f });
			_dictLMS.Add("m78",new List<float>() { -3.314768951f,15.42902273f,0.087400421f });
			_dictLMS.Add("m79",new List<float>() { -3.321785992f,15.44042439f,0.088166744f });
			_dictLMS.Add("m80",new List<float>() { -3.326345795f,15.45287581f,0.088942897f });
			_dictLMS.Add("m81",new List<float>() { -3.328602731f,15.46636218f,0.089728202f });
			_dictLMS.Add("m82",new List<float>() { -3.328725277f,15.48086704f,0.090521875f });
			_dictLMS.Add("m83",new List<float>() { -3.32687018f,15.49637465f,0.091323162f });
			_dictLMS.Add("m84",new List<float>() { -3.323188896f,15.51286936f,0.092131305f });
			_dictLMS.Add("m85",new List<float>() { -3.317827016f,15.53033563f,0.092945544f });
			_dictLMS.Add("m86",new List<float>() { -3.310923871f,15.54875807f,0.093765118f });
			_dictLMS.Add("m87",new List<float>() { -3.302612272f,15.56812143f,0.09458927f });
			_dictLMS.Add("m88",new List<float>() { -3.293018361f,15.58841065f,0.095417247f });
			_dictLMS.Add("m89",new List<float>() { -3.282260813f,15.60961101f,0.096248301f });
			_dictLMS.Add("m90",new List<float>() { -3.270454609f,15.63170735f,0.097081694f });
			_dictLMS.Add("m91",new List<float>() { -3.257703616f,15.65468563f,0.097916698f });
			_dictLMS.Add("m92",new List<float>() { -3.244108214f,15.67853139f,0.098752593f });
			_dictLMS.Add("m93",new List<float>() { -3.229761713f,15.70323052f,0.099588675f });
			_dictLMS.Add("m94",new List<float>() { -3.214751287f,15.72876911f,0.100424251f });
			_dictLMS.Add("m95",new List<float>() { -3.199158184f,15.75513347f,0.101258643f });
			_dictLMS.Add("m96",new List<float>() { -3.18305795f,15.78231007f,0.102091189f });
			_dictLMS.Add("m97",new List<float>() { -3.166520664f,15.8102856f,0.102921245f });
			_dictLMS.Add("m98",new List<float>() { -3.1496103f,15.83904708f,0.103748189f });
			_dictLMS.Add("m99",new List<float>() { -3.132389637f,15.86858123f,0.104571386f });
			_dictLMS.Add("m100",new List<float>() { -3.114911153f,15.89887562f,0.105390269f });
			_dictLMS.Add("m101",new List<float>() { -3.097226399f,15.92991765f,0.106204258f });
			_dictLMS.Add("m102",new List<float>() { -3.079383079f,15.96169481f,0.107012788f });
			_dictLMS.Add("m103",new List<float>() { -3.061423765f,15.99419489f,0.107815327f });
			_dictLMS.Add("m104",new List<float>() { -3.043386071f,16.02740607f,0.108611374f });
			_dictLMS.Add("m105",new List<float>() { -3.025310003f,16.0613159f,0.109400388f });
			_dictLMS.Add("m106",new List<float>() { -3.007225737f,16.09591292f,0.110181915f });
			_dictLMS.Add("m107",new List<float>() { -2.989164598f,16.13118532f,0.110955478f });
			_dictLMS.Add("m108",new List<float>() { -2.971148225f,16.16712234f,0.111720691f });
			_dictLMS.Add("m109",new List<float>() { -2.953208047f,16.20371168f,0.112477059f });
			_dictLMS.Add("m110",new List<float>() { -2.935363951f,16.24094239f,0.1132242f });
			_dictLMS.Add("m111",new List<float>() { -2.917635157f,16.27880346f,0.113961734f });
			_dictLMS.Add("m112",new List<float>() { -2.900039803f,16.31728385f,0.114689291f });
			_dictLMS.Add("m113",new List<float>() { -2.882593796f,16.35637267f,0.115406523f });
			_dictLMS.Add("m114",new List<float>() { -2.865311266f,16.39605916f,0.116113097f });
			_dictLMS.Add("m115",new List<float>() { -2.848204697f,16.43633265f,0.116808702f });
			_dictLMS.Add("m116",new List<float>() { -2.831285052f,16.47718256f,0.117493042f });
			_dictLMS.Add("m117",new List<float>() { -2.81456189f,16.51859843f,0.11816584f });
			_dictLMS.Add("m118",new List<float>() { -2.79804347f,16.56056987f,0.118826835f });
			_dictLMS.Add("m119",new List<float>() { -2.781736856f,16.60308661f,0.119475785f });
			_dictLMS.Add("m120",new List<float>() { -2.765648008f,16.64613844f,0.120112464f });
			_dictLMS.Add("m121",new List<float>() { -2.749782197f,16.68971518f,0.120736656f });
			_dictLMS.Add("m122",new List<float>() { -2.734142443f,16.73380695f,0.121348181f });
			_dictLMS.Add("m123",new List<float>() { -2.718732873f,16.77840363f,0.121946849f });
			_dictLMS.Add("m124",new List<float>() { -2.703555506f,16.82349538f,0.122532501f });
			_dictLMS.Add("m125",new List<float>() { -2.688611957f,16.86907238f,0.123104991f });
			_dictLMS.Add("m126",new List<float>() { -2.673903164f,16.91512487f,0.123664186f });
			_dictLMS.Add("m127",new List<float>() { -2.659429443f,16.96164317f,0.124209969f });
			_dictLMS.Add("m128",new List<float>() { -2.645190534f,17.00861766f,0.124742239f });
			_dictLMS.Add("m129",new List<float>() { -2.631185649f,17.05603879f,0.125260905f });
			_dictLMS.Add("m130",new List<float>() { -2.617413511f,17.10389705f,0.125765895f });
			_dictLMS.Add("m131",new List<float>() { -2.603872392f,17.15218302f,0.126257147f });
			_dictLMS.Add("m132",new List<float>() { -2.590560148f,17.20088732f,0.126734613f });
			_dictLMS.Add("m133",new List<float>() { -2.577474253f,17.25000062f,0.12719826f });
			_dictLMS.Add("m134",new List<float>() { -2.564611831f,17.29951367f,0.127648067f });
			_dictLMS.Add("m135",new List<float>() { -2.551969684f,17.34941726f,0.128084023f });
			_dictLMS.Add("m136",new List<float>() { -2.539539972f,17.39970308f,0.128506192f });
			_dictLMS.Add("m137",new List<float>() { -2.527325681f,17.45036072f,0.128914497f });
			_dictLMS.Add("m138",new List<float>() { -2.515320235f,17.50138161f,0.129309001f });
			_dictLMS.Add("m139",new List<float>() { -2.503519447f,17.55275674f,0.129689741f });
			_dictLMS.Add("m140",new List<float>() { -2.491918934f,17.60447714f,0.130056765f });
			_dictLMS.Add("m141",new List<float>() { -2.480514136f,17.6565339f,0.130410133f });
			_dictLMS.Add("m142",new List<float>() { -2.469300331f,17.70891811f,0.130749913f });
			_dictLMS.Add("m143",new List<float>() { -2.458272656f,17.76162094f,0.131076187f });
			_dictLMS.Add("m144",new List<float>() { -2.447426113f,17.81463359f,0.131389042f });
			_dictLMS.Add("m145",new List<float>() { -2.436755595f,17.86794729f,0.131688579f });
			_dictLMS.Add("m146",new List<float>() { -2.426255887f,17.92155332f,0.131974905f });
			_dictLMS.Add("m147",new List<float>() { -2.415921689f,17.97544299f,0.132248138f });
			_dictLMS.Add("m148",new List<float>() { -2.405747619f,18.02960765f,0.132508403f });
			_dictLMS.Add("m149",new List<float>() { -2.395728233f,18.08403868f,0.132755834f });
			_dictLMS.Add("m150",new List<float>() { -2.385858029f,18.1387275f,0.132990575f });
			_dictLMS.Add("m151",new List<float>() { -2.376131459f,18.19366555f,0.133212776f });
			_dictLMS.Add("m152",new List<float>() { -2.366542942f,18.24884431f,0.133422595f });
			_dictLMS.Add("m153",new List<float>() { -2.357086871f,18.3042553f,0.133620197f });
			_dictLMS.Add("m154",new List<float>() { -2.347757625f,18.35989003f,0.133805756f });
			_dictLMS.Add("m155",new List<float>() { -2.338549576f,18.41574009f,0.133979452f });
			_dictLMS.Add("m156",new List<float>() { -2.3294571f,18.47179706f,0.13414147f });
			_dictLMS.Add("m157",new List<float>() { -2.320474586f,18.52805255f,0.134292005f });
			_dictLMS.Add("m158",new List<float>() { -2.311596446f,18.5844982f,0.134431256f });
			_dictLMS.Add("m159",new List<float>() { -2.302817124f,18.64112567f,0.134559427f });
			_dictLMS.Add("m160",new List<float>() { -2.294131107f,18.69792663f,0.134676731f });
			_dictLMS.Add("m161",new List<float>() { -2.285532933f,18.75489278f,0.134783385f });
			_dictLMS.Add("m162",new List<float>() { -2.277017201f,18.81201584f,0.134879611f });
			_dictLMS.Add("m163",new List<float>() { -2.268578584f,18.86928753f,0.134965637f });
			_dictLMS.Add("m164",new List<float>() { -2.260211837f,18.92669959f,0.135041695f });
			_dictLMS.Add("m165",new List<float>() { -2.251911809f,18.98424378f,0.135108024f });
			_dictLMS.Add("m166",new List<float>() { -2.243673453f,19.04191185f,0.135164867f });
			_dictLMS.Add("m167",new List<float>() { -2.235491842f,19.09969557f,0.135212469f });
			_dictLMS.Add("m168",new List<float>() { -2.227362173f,19.15758672f,0.135251083f });
			_dictLMS.Add("m169",new List<float>() { -2.21927979f,19.21557707f,0.135280963f });
			_dictLMS.Add("m170",new List<float>() { -2.211240187f,19.27365839f,0.135302371f });
			_dictLMS.Add("m171",new List<float>() { -2.203239029f,19.33182247f,0.135315568f });
			_dictLMS.Add("m172",new List<float>() { -2.195272161f,19.39006106f,0.135320824f });
			_dictLMS.Add("m173",new List<float>() { -2.187335625f,19.44836594f,0.135318407f });
			_dictLMS.Add("m174",new List<float>() { -2.179425674f,19.50672885f,0.135308594f });
			_dictLMS.Add("m175",new List<float>() { -2.171538789f,19.56514153f,0.135291662f });
			_dictLMS.Add("m176",new List<float>() { -2.163671689f,19.62359571f,0.135267891f });
			_dictLMS.Add("m177",new List<float>() { -2.155821357f,19.6820831f,0.135237567f });
			_dictLMS.Add("m178",new List<float>() { -2.147985046f,19.74059538f,0.135200976f });
			_dictLMS.Add("m179",new List<float>() { -2.140160305f,19.7991242f,0.135158409f });
			_dictLMS.Add("m180",new List<float>() { -2.132344989f,19.85766121f,0.135110159f });
			_dictLMS.Add("m181",new List<float>() { -2.124537282f,19.916198f,0.135056522f });
			_dictLMS.Add("m182",new List<float>() { -2.116735712f,19.97472615f,0.134997797f });
			_dictLMS.Add("m183",new List<float>() { -2.108939167f,20.03323719f,0.134934285f });
			_dictLMS.Add("m184",new List<float>() { -2.10114692f,20.09172262f,0.134866291f });
			_dictLMS.Add("m185",new List<float>() { -2.093358637f,20.15017387f,0.134794121f });
			_dictLMS.Add("m186",new List<float>() { -2.085574403f,20.20858236f,0.134718085f });
			_dictLMS.Add("m187",new List<float>() { -2.077794735f,20.26693944f,0.134638494f });
			_dictLMS.Add("m188",new List<float>() { -2.070020599f,20.32523642f,0.134555663f });
			_dictLMS.Add("m189",new List<float>() { -2.062253431f,20.38346455f,0.13446991f });
			_dictLMS.Add("m190",new List<float>() { -2.054495145f,20.44161501f,0.134381553f });
			_dictLMS.Add("m191",new List<float>() { -2.046748156f,20.49967894f,0.134290916f });
			_dictLMS.Add("m192",new List<float>() { -2.039015385f,20.5576474f,0.134198323f });
			_dictLMS.Add("m193",new List<float>() { -2.031300282f,20.6155114f,0.134104101f });
			_dictLMS.Add("m194",new List<float>() { -2.023606828f,20.67326189f,0.134008581f });
			_dictLMS.Add("m195",new List<float>() { -2.015942013f,20.73088905f,0.133912066f });
			_dictLMS.Add("m196",new List<float>() { -2.008305745f,20.7883851f,0.133814954f });
			_dictLMS.Add("m197",new List<float>() { -2.000706389f,20.84574003f,0.133717552f });
			_dictLMS.Add("m198",new List<float>() { -1.993150137f,20.90294449f,0.1336202f });
			_dictLMS.Add("m199",new List<float>() { -1.985643741f,20.95998909f,0.133523244f });
			_dictLMS.Add("m200",new List<float>() { -1.97819451f,21.01686433f,0.133427032f });
			_dictLMS.Add("m201",new List<float>() { -1.970810308f,21.07356067f,0.133331914f });
			_dictLMS.Add("m202",new List<float>() { -1.96349954f,21.1300685f,0.133238245f });
			_dictLMS.Add("m203",new List<float>() { -1.956271141f,21.18637813f,0.133146383f });
			_dictLMS.Add("m204",new List<float>() { -1.949134561f,21.24247982f,0.13305669f });
			_dictLMS.Add("m205",new List<float>() { -1.942099744f,21.29836376f,0.132969531f });
			_dictLMS.Add("m206",new List<float>() { -1.935177101f,21.35402009f,0.132885274f });
			_dictLMS.Add("m207",new List<float>() { -1.92837748f,21.40943891f,0.132804292f });
			_dictLMS.Add("m208",new List<float>() { -1.921712136f,21.46461026f,0.132726962f });
			_dictLMS.Add("m209",new List<float>() { -1.915192685f,21.51952414f,0.132653664f });
			_dictLMS.Add("m210",new List<float>() { -1.908831065f,21.57417053f,0.132584784f });
			_dictLMS.Add("m211",new List<float>() { -1.902639482f,21.62853937f,0.132520711f });
			_dictLMS.Add("m212",new List<float>() { -1.896630358f,21.68262062f,0.132461838f });
			_dictLMS.Add("m213",new List<float>() { -1.890816268f,21.73640419f,0.132408563f });
			_dictLMS.Add("m214",new List<float>() { -1.885209876f,21.78988003f,0.132361289f });
			_dictLMS.Add("m215",new List<float>() { -1.879823505f,21.84303819f,0.132320427f });
			_dictLMS.Add("f36",new List<float>() { -2.096828937f,15.69924188f,0.078605255f });
			_dictLMS.Add("f37",new List<float>() { -2.189211877f,15.65523282f,0.078378696f });
			_dictLMS.Add("f38",new List<float>() { -2.279991982f,15.61321371f,0.078196674f });
			_dictLMS.Add("f39",new List<float>() { -2.368732949f,15.57316843f,0.078058667f });
			_dictLMS.Add("f40",new List<float>() { -2.455021314f,15.53508019f,0.077964169f });
			_dictLMS.Add("f41",new List<float>() { -2.538471972f,15.49893145f,0.077912684f });
			_dictLMS.Add("f42",new List<float>() { -2.618732901f,15.46470384f,0.077903716f });
			_dictLMS.Add("f43",new List<float>() { -2.695488973f,15.43237817f,0.077936763f });
			_dictLMS.Add("f44",new List<float>() { -2.768464816f,15.40193436f,0.078011309f });
			_dictLMS.Add("f45",new List<float>() { -2.837426693f,15.37335154f,0.078126817f });
			_dictLMS.Add("f46",new List<float>() { -2.902178205f,15.34660842f,0.078282739f });
			_dictLMS.Add("f47",new List<float>() { -2.962580386f,15.32168181f,0.078478449f });
			_dictLMS.Add("f48",new List<float>() { -3.018521987f,15.29854897f,0.078713325f });
			_dictLMS.Add("f49",new List<float>() { -3.069936555f,15.27718618f,0.078986694f });
			_dictLMS.Add("f50",new List<float>() { -3.116795864f,15.2575692f,0.079297841f });
			_dictLMS.Add("f51",new List<float>() { -3.159107331f,15.23967338f,0.079646006f });
			_dictLMS.Add("f52",new List<float>() { -3.196911083f,15.22347371f,0.080030389f });
			_dictLMS.Add("f53",new List<float>() { -3.230276759f,15.20894491f,0.080450145f });
			_dictLMS.Add("f54",new List<float>() { -3.259300182f,15.19606152f,0.080904391f });
			_dictLMS.Add("f55",new List<float>() { -3.284099963f,15.18479799f,0.081392203f });
			_dictLMS.Add("f56",new List<float>() { -3.30481415f,15.17512871f,0.081912623f });
			_dictLMS.Add("f57",new List<float>() { -3.321596954f,15.16702811f,0.082464661f });
			_dictLMS.Add("f58",new List<float>() { -3.334615646f,15.16047068f,0.083047295f });
			_dictLMS.Add("f59",new List<float>() { -3.344047622f,15.15543107f,0.083659478f });
			_dictLMS.Add("f60",new List<float>() { -3.35007771f,15.15188405f,0.084300139f });
			_dictLMS.Add("f61",new List<float>() { -3.352893805f,15.14980479f,0.0849682f });
			_dictLMS.Add("f62",new List<float>() { -3.352691376f,15.14916825f,0.085662539f });
			_dictLMS.Add("f63",new List<float>() { -3.34966438f,15.14994984f,0.086382035f });
			_dictLMS.Add("f64",new List<float>() { -3.343998803f,15.15212585f,0.087125591f });
			_dictLMS.Add("f65",new List<float>() { -3.335889574f,15.15567186f,0.087892047f });
			_dictLMS.Add("f66",new List<float>() { -3.325522491f,15.16056419f,0.088680264f });
			_dictLMS.Add("f67",new List<float>() { -3.31307846f,15.16677947f,0.089489106f });
			_dictLMS.Add("f68",new List<float>() { -3.298732648f,15.17429464f,0.090317434f });
			_dictLMS.Add("f69",new List<float>() { -3.282653831f,15.18308694f,0.091164117f });
			_dictLMS.Add("f70",new List<float>() { -3.265003896f,15.1931339f,0.092028028f });
			_dictLMS.Add("f71",new List<float>() { -3.245937506f,15.20441335f,0.092908048f });
			_dictLMS.Add("f72",new List<float>() { -3.225606516f,15.21690296f,0.093803033f });
			_dictLMS.Add("f73",new List<float>() { -3.204146115f,15.2305815f,0.094711916f });
			_dictLMS.Add("f74",new List<float>() { -3.181690237f,15.24542745f,0.095633595f });
			_dictLMS.Add("f75",new List<float>() { -3.158363475f,15.26141966f,0.096566992f });
			_dictLMS.Add("f76",new List<float>() { -3.134282833f,15.27853728f,0.097511046f });
			_dictLMS.Add("f77",new List<float>() { -3.109557879f,15.29675967f,0.09846471f });
			_dictLMS.Add("f78",new List<float>() { -3.084290931f,15.31606644f,0.099426955f });
			_dictLMS.Add("f79",new List<float>() { -3.058577292f,15.33643745f,0.100396769f });
			_dictLMS.Add("f80",new List<float>() { -3.032505499f,15.35785274f,0.101373159f });
			_dictLMS.Add("f81",new List<float>() { -3.0061576f,15.38029261f,0.10235515f });
			_dictLMS.Add("f82",new List<float>() { -2.979609448f,15.40373754f,0.103341788f });
			_dictLMS.Add("f83",new List<float>() { -2.952930993f,15.42816819f,0.104332139f });
			_dictLMS.Add("f84",new List<float>() { -2.926186592f,15.45356545f,0.105325289f });
			_dictLMS.Add("f85",new List<float>() { -2.899435307f,15.47991037f,0.106320346f });
			_dictLMS.Add("f86",new List<float>() { -2.872731211f,15.50718419f,0.10731644f });
			_dictLMS.Add("f87",new List<float>() { -2.846123683f,15.53536829f,0.108312721f });
			_dictLMS.Add("f88",new List<float>() { -2.819657704f,15.56444426f,0.109308364f });
			_dictLMS.Add("f89",new List<float>() { -2.793374145f,15.5943938f,0.110302563f });
			_dictLMS.Add("f90",new List<float>() { -2.767310047f,15.6251988f,0.111294537f });
			_dictLMS.Add("f91",new List<float>() { -2.741498897f,15.65684126f,0.112283526f });
			_dictLMS.Add("f92",new List<float>() { -2.715970894f,15.68930333f,0.113268793f });
			_dictLMS.Add("f93",new List<float>() { -2.690753197f,15.7225673f,0.114249622f });
			_dictLMS.Add("f94",new List<float>() { -2.665870146f,15.75661555f,0.115225321f });
			_dictLMS.Add("f95",new List<float>() { -2.641343436f,15.79143062f,0.116195218f });
			_dictLMS.Add("f96",new List<float>() { -2.617192204f,15.82699517f,0.117158667f });
			_dictLMS.Add("f97",new List<float>() { -2.593430614f,15.86329241f,0.118115073f });
			_dictLMS.Add("f98",new List<float>() { -2.570076037f,15.90030484f,0.119063807f });
			_dictLMS.Add("f99",new List<float>() { -2.547141473f,15.93801545f,0.12000429f });
			_dictLMS.Add("f100",new List<float>() { -2.524635245f,15.97640787f,0.120935994f });
			_dictLMS.Add("f101",new List<float>() { -2.502569666f,16.01546483f,0.121858355f });
			_dictLMS.Add("f102",new List<float>() { -2.48095189f,16.05516984f,0.12277087f });
			_dictLMS.Add("f103",new List<float>() { -2.459785573f,16.09550688f,0.123673085f });
			_dictLMS.Add("f104",new List<float>() { -2.439080117f,16.13645881f,0.124564484f });
			_dictLMS.Add("f105",new List<float>() { -2.418838304f,16.17800955f,0.125444639f });
			_dictLMS.Add("f106",new List<float>() { -2.399063683f,16.22014281f,0.126313121f });
			_dictLMS.Add("f107",new List<float>() { -2.379756861f,16.26284277f,0.127169545f });
			_dictLMS.Add("f108",new List<float>() { -2.360920527f,16.30609316f,0.128013515f });
			_dictLMS.Add("f109",new List<float>() { -2.342557728f,16.34987759f,0.128844639f });
			_dictLMS.Add("f110",new List<float>() { -2.324663326f,16.39418118f,0.129662637f });
			_dictLMS.Add("f111",new List<float>() { -2.307240716f,16.43898741f,0.130467138f });
			_dictLMS.Add("f112",new List<float>() { -2.290287663f,16.48428082f,0.131257852f });
			_dictLMS.Add("f113",new List<float>() { -2.273803847f,16.53004554f,0.132034479f });
			_dictLMS.Add("f114",new List<float>() { -2.257782149f,16.57626713f,0.132796819f });
			_dictLMS.Add("f115",new List<float>() { -2.242227723f,16.62292864f,0.133544525f });
			_dictLMS.Add("f116",new List<float>() { -2.227132805f,16.67001572f,0.134277436f });
			_dictLMS.Add("f117",new List<float>() { -2.212495585f,16.71751288f,0.134995324f });
			_dictLMS.Add("f118",new List<float>() { -2.19831275f,16.76540496f,0.135697996f });
			_dictLMS.Add("f119",new List<float>() { -2.184580762f,16.81367689f,0.136385276f });
			_dictLMS.Add("f120",new List<float>() { -2.171295888f,16.86231366f,0.137057004f });
			_dictLMS.Add("f121",new List<float>() { -2.158454232f,16.91130036f,0.137713039f });
			_dictLMS.Add("f122",new List<float>() { -2.146051754f,16.96062216f,0.138353254f });
			_dictLMS.Add("f123",new List<float>() { -2.134084303f,17.0102643f,0.138977537f });
			_dictLMS.Add("f124",new List<float>() { -2.122547629f,17.06021213f,0.139585795f });
			_dictLMS.Add("f125",new List<float>() { -2.111437411f,17.11045106f,0.140177947f });
			_dictLMS.Add("f126",new List<float>() { -2.100749266f,17.16096656f,0.140753927f });
			_dictLMS.Add("f127",new List<float>() { -2.090478774f,17.21174424f,0.141313686f });
			_dictLMS.Add("f128",new List<float>() { -2.080621484f,17.26276973f,0.141857186f });
			_dictLMS.Add("f129",new List<float>() { -2.071172932f,17.31402878f,0.142384404f });
			_dictLMS.Add("f130",new List<float>() { -2.062128649f,17.3655072f,0.142895332f });
			_dictLMS.Add("f131",new List<float>() { -2.053484173f,17.4171909f,0.143389972f });
			_dictLMS.Add("f132",new List<float>() { -2.045235058f,17.46906585f,0.143868341f });
			_dictLMS.Add("f133",new List<float>() { -2.03737688f,17.52111811f,0.144330469f });
			_dictLMS.Add("f134",new List<float>() { -2.029906684f,17.57333347f,0.144776372f });
			_dictLMS.Add("f135",new List<float>() { -2.022817914f,17.62569869f,0.145206138f });
			_dictLMS.Add("f136",new List<float>() { -2.016107084f,17.67819987f,0.145619819f });
			_dictLMS.Add("f137",new List<float>() { -2.009769905f,17.7308234f,0.146017491f });
			_dictLMS.Add("f138",new List<float>() { -2.003802134f,17.78355575f,0.146399239f });
			_dictLMS.Add("f139",new List<float>() { -1.998199572f,17.83638347f,0.146765161f });
			_dictLMS.Add("f140",new List<float>() { -1.992958064f,17.88929321f,0.147115364f });
			_dictLMS.Add("f141",new List<float>() { -1.988073505f,17.94227168f,0.147449967f });
			_dictLMS.Add("f142",new List<float>() { -1.983541835f,17.9953057f,0.147769097f });
			_dictLMS.Add("f143",new List<float>() { -1.979359041f,18.04838216f,0.148072891f });
			_dictLMS.Add("f144",new List<float>() { -1.975521156f,18.10148804f,0.148361495f });
			_dictLMS.Add("f145",new List<float>() { -1.972024258f,18.15461039f,0.148635067f });
			_dictLMS.Add("f146",new List<float>() { -1.968864465f,18.20773639f,0.148893769f });
			_dictLMS.Add("f147",new List<float>() { -1.966037938f,18.26085325f,0.149137776f });
			_dictLMS.Add("f148",new List<float>() { -1.963540872f,18.31394832f,0.14936727f });
			_dictLMS.Add("f149",new List<float>() { -1.961369499f,18.36700902f,0.149582439f });
			_dictLMS.Add("f150",new List<float>() { -1.959520079f,18.42002284f,0.149783482f });
			_dictLMS.Add("f151",new List<float>() { -1.9579889f,18.47297739f,0.149970604f });
			_dictLMS.Add("f152",new List<float>() { -1.956772271f,18.52586035f,0.15014402f });
			_dictLMS.Add("f153",new List<float>() { -1.95586652f,18.57865951f,0.15030395f });
			_dictLMS.Add("f154",new List<float>() { -1.955267984f,18.63136275f,0.150450621f });
			_dictLMS.Add("f155",new List<float>() { -1.954973011f,18.68395801f,0.15058427f });
			_dictLMS.Add("f156",new List<float>() { -1.954977947f,18.73643338f,0.150705138f });
			_dictLMS.Add("f157",new List<float>() { -1.955279136f,18.788777f,0.150813475f });
			_dictLMS.Add("f158",new List<float>() { -1.955872909f,18.84097713f,0.150909535f });
			_dictLMS.Add("f159",new List<float>() { -1.956755579f,18.89302212f,0.150993582f });
			_dictLMS.Add("f160",new List<float>() { -1.957923436f,18.94490041f,0.151065883f });
			_dictLMS.Add("f161",new List<float>() { -1.959372737f,18.99660055f,0.151126714f });
			_dictLMS.Add("f162",new List<float>() { -1.9610997f,19.04811118f,0.151176355f });
			_dictLMS.Add("f163",new List<float>() { -1.963100496f,19.09942105f,0.151215094f });
			_dictLMS.Add("f164",new List<float>() { -1.96537124f,19.15051899f,0.151243223f });
			_dictLMS.Add("f165",new List<float>() { -1.967907983f,19.20139397f,0.151261042f });
			_dictLMS.Add("f166",new List<float>() { -1.970706706f,19.25203503f,0.151268855f });
			_dictLMS.Add("f167",new List<float>() { -1.973763307f,19.30243131f,0.151266974f });
			_dictLMS.Add("f168",new List<float>() { -1.977073595f,19.35257209f,0.151255713f });
			_dictLMS.Add("f169",new List<float>() { -1.980633277f,19.40244671f,0.151235395f });
			_dictLMS.Add("f170",new List<float>() { -1.984437954f,19.45204465f,0.151206347f });
			_dictLMS.Add("f171",new List<float>() { -1.988483106f,19.50135548f,0.151168902f });
			_dictLMS.Add("f172",new List<float>() { -1.992764085f,19.55036888f,0.151123398f });
			_dictLMS.Add("f173",new List<float>() { -1.997276103f,19.59907464f,0.15107018f });
			_dictLMS.Add("f174",new List<float>() { -2.002014224f,19.64746266f,0.151009595f });
			_dictLMS.Add("f175",new List<float>() { -2.00697335f,19.69552294f,0.150942f });
			_dictLMS.Add("f176",new List<float>() { -2.012148213f,19.7432456f,0.150867753f });
			_dictLMS.Add("f177",new List<float>() { -2.017533363f,19.79062086f,0.150787221f });
			_dictLMS.Add("f178",new List<float>() { -2.023123159f,19.83763907f,0.150700774f });
			_dictLMS.Add("f179",new List<float>() { -2.028911755f,19.88429066f,0.150608788f });
			_dictLMS.Add("f180",new List<float>() { -2.034893091f,19.9305662f,0.150511645f });
			_dictLMS.Add("f181",new List<float>() { -2.041060881f,19.97645636f,0.150409731f });
			_dictLMS.Add("f182",new List<float>() { -2.047408604f,20.02195192f,0.15030344f });
			_dictLMS.Add("f183",new List<float>() { -2.05392949f,20.06704377f,0.150193169f });
			_dictLMS.Add("f184",new List<float>() { -2.060616513f,20.11172291f,0.150079322f });
			_dictLMS.Add("f185",new List<float>() { -2.067462375f,20.15598047f,0.149962308f });
			_dictLMS.Add("f186",new List<float>() { -2.074459502f,20.19980767f,0.14984254f });
			_dictLMS.Add("f187",new List<float>() { -2.081600029f,20.24319586f,0.149720441f });
			_dictLMS.Add("f188",new List<float>() { -2.088875793f,20.28613648f,0.149596434f });
			_dictLMS.Add("f189",new List<float>() { -2.096278323f,20.32862109f,0.149470953f });
			_dictLMS.Add("f190",new List<float>() { -2.103798828f,20.37064138f,0.149344433f });
			_dictLMS.Add("f191",new List<float>() { -2.111428194f,20.41218911f,0.149217319f });
			_dictLMS.Add("f192",new List<float>() { -2.119156972f,20.45325617f,0.14909006f });
			_dictLMS.Add("f193",new List<float>() { -2.126975375f,20.49383457f,0.14896311f });
			_dictLMS.Add("f194",new List<float>() { -2.134873266f,20.5339164f,0.148836931f });
			_dictLMS.Add("f195",new List<float>() { -2.142840157f,20.57349387f,0.148711989f });
			_dictLMS.Add("f196",new List<float>() { -2.150865204f,20.61255929f,0.148588757f });
			_dictLMS.Add("f197",new List<float>() { -2.158937201f,20.65110506f,0.148467715f });
			_dictLMS.Add("f198",new List<float>() { -2.167044578f,20.6891237f,0.148349348f });
			_dictLMS.Add("f199",new List<float>() { -2.175176987f,20.72660728f,0.14823412f });
			_dictLMS.Add("f200",new List<float>() { -2.183317362f,20.76355011f,0.148122614f });
			_dictLMS.Add("f201",new List<float>() { -2.191457792f,20.79994337f,0.148015249f });
			_dictLMS.Add("f202",new List<float>() { -2.199583649f,20.83578051f,0.147912564f });
			_dictLMS.Add("f203",new List<float>() { -2.207681525f,20.87105449f,0.147815078f });
			_dictLMS.Add("f204",new List<float>() { -2.215737645f,20.90575839f,0.147723315f });
			_dictLMS.Add("f205",new List<float>() { -2.223739902f,20.93988477f,0.147637768f });
			_dictLMS.Add("f206",new List<float>() { -2.231667995f,20.97342858f,0.147559083f });
			_dictLMS.Add("f207",new List<float>() { -2.239511942f,21.00638171f,0.147487716f });
			_dictLMS.Add("f208",new List<float>() { -2.247257081f,21.0387374f,0.14742421f });
			_dictLMS.Add("f209",new List<float>() { -2.254885145f,21.07048996f,0.147369174f });
			_dictLMS.Add("f210",new List<float>() { -2.26238209f,21.10163241f,0.147323144f });
			_dictLMS.Add("f211",new List<float>() { -2.269731517f,21.13215845f,0.147286698f });
			_dictLMS.Add("f212",new List<float>() { -2.276917229f,21.16206171f,0.147260415f });
			_dictLMS.Add("f213",new List<float>() { -2.283925442f,21.1913351f,0.147244828f });
			_dictLMS.Add("f214",new List<float>() { -2.290731442f,21.21997472f,0.147240683f });
			_dictLMS.Add("f215",new List<float>() { -2.29732427f,21.24797262f,0.147248467f });
		}

		private void CalcBMI() {
			labelWeightCode.Text="";
			//BMI = (lbs*703)/(in^2)
			float height;
			float weight;
			try {
				height = float.Parse(textHeight.Text);
				weight = float.Parse(textWeight.Text);
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
			if(ageBeforeJanFirst<17 && ageBeforeJanFirst>2) {//calc and show BMI percentile if patient is >= 3 and < 17 on 01/01 of the year of the exam
				bmiPercentile=CalcBMIPercentile(bmi);
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
			if(ageBeforeJanFirst<17) {
				isIntGroupVisible=true;
				if(groupInterventions.Text!=childGroupLabel) {
					groupInterventions.Text=childGroupLabel;
				}
				FillGridInterventions(true);
			}
			else if(ageBeforeJanFirst>=18 && labelWeightCode.Text!="") {//if 18 or over and given an over/underweight code due to BMI
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
			if(ageBeforeJanFirst<18) {//Do not clasify children as over/underweight
				intervCodeSet=InterventionCodeSet.Nutrition;//we will sent Nutrition to FormInterventionEdit, but this could also be a physical activity intervention
				return "";
			}
			else if(ageBeforeJanFirst<65) {
				if(bmi<18.5) {
					intervCodeSet=InterventionCodeSet.BelowNormalWeight;
					return "Underweight";
				}
				else if(bmi<25) {
					return "";
				}
				else {
					intervCodeSet=InterventionCodeSet.AboveNormalWeight;
					return "Overweight";
				}
			}
			else {
				if(bmi<23) {
					intervCodeSet=InterventionCodeSet.BelowNormalWeight;
					return "Underweight";
				}
				else if(bmi<30) {
					return "";
				}
				else {
					intervCodeSet=InterventionCodeSet.AboveNormalWeight;
					return "Overweight";
				}
			}
			//do not save to DB until butOK_Click
		}

		///<summary>Only called if patient is under 18 at the time of the exam.</summary>
		private int CalcBMIPercentile(float bmi) {
			//get age at time of exam for BMI percentile in months.  Examples: 13 years 11 months = 13*12+11 = 167.
			DateTime dateExam=PIn.Date(textDateTaken.Text);
			if(dateExam==DateTime.MinValue || dateExam<patCur.Birthdate || patCur.Birthdate==DateTime.MinValue) {
				return -1;
			}
			int years=dateExam.Year-patCur.Birthdate.Year;
			if(dateExam.Month<patCur.Birthdate.Month || (dateExam.Month==patCur.Birthdate.Month && dateExam.Day<patCur.Birthdate.Day)) {//have not had birthday this year
				years--;
			}
			int months=dateExam.Month-patCur.Birthdate.Month;
			if(patCur.Birthdate.Day>dateExam.Day) {
				months--;
			}
			if(months<0) {
				months=months+12;
			}
			months=months+(years*12);
			string genderAgeGroup="";
			if(patCur.Gender==PatientGender.Male) {
				genderAgeGroup="m";
			}
			else if(patCur.Gender==PatientGender.Female) {
				genderAgeGroup="f";
			}
			else {
				return -1;
			}
			genderAgeGroup+=months.ToString();
			//get L, M, and S for the patient's gender and age from dict
			List<float> listLMS=_dictLMS[genderAgeGroup];
			//use (((bmi/M)^L)-1)/(L*S) to get z-score
			float zScore=(((float)Math.Pow(bmi/listLMS[1],listLMS[0]))-1)/(listLMS[0]*listLMS[2]);
			//use GetPercentile helper function to get percentile from z-score
			return GetPercentile(zScore);
		}

		private static int GetPercentile(float z) {
			List<float> listPercentiles=new List<float>();
			#region Add All Percentiles z-score Table Lookup
			listPercentiles.Add(-2.325f);//0th percentile
			listPercentiles.Add(-2.055f);//1st percentile
			listPercentiles.Add(-1.885f);//2nd percentile
			listPercentiles.Add(-1.755f);//3rd
			listPercentiles.Add(-1.645f);//4th
			listPercentiles.Add(-1.555f);//5th
			listPercentiles.Add(-1.475f);//6th
			listPercentiles.Add(-1.405f);//7th
			listPercentiles.Add(-1.345f);//8th
			listPercentiles.Add(-1.285f);//9th
			listPercentiles.Add(-1.225f);//10th
			listPercentiles.Add(-1.175f);//11th
			listPercentiles.Add(-1.125f);//12th
			listPercentiles.Add(-1.085f);//13th
			listPercentiles.Add(-1.035f);//14th
			listPercentiles.Add(-0.995f);//15th
			listPercentiles.Add(-0.955f);//16th
			listPercentiles.Add(-0.915f);//17th
			listPercentiles.Add(-0.875f);//18th
			listPercentiles.Add(-0.845f);//19th
			listPercentiles.Add(-0.805f);//20th
			listPercentiles.Add(-0.775f);//21st
			listPercentiles.Add(-0.735f);//22nd
			listPercentiles.Add(-0.705f);//23rd
			listPercentiles.Add(-0.675f);//24th
			listPercentiles.Add(-0.645f);//25th
			listPercentiles.Add(-0.615f);//26th
			listPercentiles.Add(-0.585f);//27th
			listPercentiles.Add(-0.555f);//28th
			listPercentiles.Add(-0.525f);//29th
			listPercentiles.Add(-0.495f);//30th
			listPercentiles.Add(-0.465f);//31st
			listPercentiles.Add(-0.435f);//32nd
			listPercentiles.Add(-0.415f);//33rd
			listPercentiles.Add(-0.385f);//34th
			listPercentiles.Add(-0.355f);//35th
			listPercentiles.Add(-0.335f);//36th
			listPercentiles.Add(-0.305f);//37th
			listPercentiles.Add(-0.275f);//38th
			listPercentiles.Add(-0.255f);//39th
			listPercentiles.Add(-0.225f);//40th
			listPercentiles.Add(-0.205f);//41st
			listPercentiles.Add(-0.175f);//42nd
			listPercentiles.Add(-0.155f);//43rd
			listPercentiles.Add(-0.125f);//44th
			listPercentiles.Add(-0.105f);//45th
			listPercentiles.Add(-0.075f);//46th
			listPercentiles.Add(-0.055f);//47th
			listPercentiles.Add(-0.025f);//48th
			listPercentiles.Add(-0.005f);//49th
			listPercentiles.Add(0.025f);//50th
			listPercentiles.Add(0.055f);//51st
			listPercentiles.Add(0.075f);//52nd
			listPercentiles.Add(0.105f);//53rd
			listPercentiles.Add(0.125f);//54th
			listPercentiles.Add(0.155f);//55th
			listPercentiles.Add(0.175f);//56th
			listPercentiles.Add(0.205f);//57th
			listPercentiles.Add(0.225f);//58th
			listPercentiles.Add(0.255f);//59th
			listPercentiles.Add(0.275f);//60th
			listPercentiles.Add(0.305f);//61st
			listPercentiles.Add(0.335f);//62nd
			listPercentiles.Add(0.355f);//63rd
			listPercentiles.Add(0.385f);//64th
			listPercentiles.Add(0.415f);//65th
			listPercentiles.Add(0.435f);//66th
			listPercentiles.Add(0.465f);//67th
			listPercentiles.Add(0.495f);//68th
			listPercentiles.Add(0.525f);//69th
			listPercentiles.Add(0.555f);//70th
			listPercentiles.Add(0.585f);//71st
			listPercentiles.Add(0.615f);//72nd
			listPercentiles.Add(0.645f);//73rd
			listPercentiles.Add(0.675f);//74th
			listPercentiles.Add(0.705f);//75th
			listPercentiles.Add(0.735f);//76th
			listPercentiles.Add(0.775f);//77th
			listPercentiles.Add(0.805f);//78th
			listPercentiles.Add(0.845f);//79th
			listPercentiles.Add(0.875f);//80th
			listPercentiles.Add(0.915f);//81st
			listPercentiles.Add(0.955f);//82nd
			listPercentiles.Add(0.995f);//83rd
			listPercentiles.Add(1.035f);//84th
			listPercentiles.Add(1.085f);//85th
			listPercentiles.Add(1.125f);//86th
			listPercentiles.Add(1.175f);//87th
			listPercentiles.Add(1.225f);//88th
			listPercentiles.Add(1.285f);//89th
			listPercentiles.Add(1.345f);//90th
			listPercentiles.Add(1.405f);//91st
			listPercentiles.Add(1.475f);//92nd
			listPercentiles.Add(1.555f);//93rd
			listPercentiles.Add(1.645f);//94th
			listPercentiles.Add(1.755f);//95th
			listPercentiles.Add(1.885f);//96th
			listPercentiles.Add(2.055f);//97th
			listPercentiles.Add(2.325f);//98th
			listPercentiles.Add(float.MaxValue);//99th
			#endregion
			for(int i=0;i<listPercentiles.Count;i++) {
				if(z<listPercentiles[i]) {
					return i;
				}
			}
			return -1;
		}

		private void FillGridInterventions(bool isChild) {
			DateTime examDate=PIn.Date(textDateTaken.Text);//this may be different than the saved VitalsignCur.DateTaken if user edited and has not hit ok to save
			#region GetInterventionsThatApply
			List<Intervention> listIntervention=new List<Intervention>();
			if(isChild) {
				listIntervention=Interventions.Refresh(VitalsignCur.PatNum,InterventionCodeSet.Nutrition);
				listIntervention.AddRange(Interventions.Refresh(VitalsignCur.PatNum,InterventionCodeSet.PhysicalActivity));
			}
			else {
				listIntervention=Interventions.Refresh(VitalsignCur.PatNum,InterventionCodeSet.AboveNormalWeight);
				listIntervention.AddRange(Interventions.Refresh(VitalsignCur.PatNum,InterventionCodeSet.BelowNormalWeight));
			}
			for(int i=listIntervention.Count-1;i>-1;i--) {
				if(listIntervention[i].DateEntry.Date<=examDate.Date && listIntervention[i].DateEntry.Date>examDate.AddMonths(-6).Date) {
					continue;
				}
				listIntervention.RemoveAt(i);
			}
			#endregion
			#region GetMedicationOrdersThatApply
			List<MedicationPat> listMedPats=new List<MedicationPat>();
			if(!isChild) {
				listMedPats=MedicationPats.Refresh(VitalsignCur.PatNum,true);
				for(int i=listMedPats.Count-1;i>-1;i--) {
					if(listMedPats[i].DateStart.Date<examDate.AddMonths(-6).Date || listMedPats[i].DateStart.Date>examDate.Date) {
						listMedPats.RemoveAt(i);
					}					
				}
				//if still meds that have start date within exam date and exam date -6 months, check the rxnorm against valid ehr meds
				if(listMedPats.Count>0) {
					List<EhrCode> listEhrMeds=EhrCodes.GetForValueSetOIDs(new List<string> { "2.16.840.1.113883.3.600.1.1498","2.16.840.1.113883.3.600.1.1499" },true);//Above Normal Medications RxNorm Value Set, Below Normal Medications RxNorm Value Set
					//listEhrMeds will only contain 7 medications for above/below normal weight and only if those exist in the rxnorm table
					for(int i=listMedPats.Count-1;i>-1;i--) {
						bool found=false;
						for(int j=0;j<listEhrMeds.Count;j++) {
							if(listMedPats[i].RxCui.ToString()==listEhrMeds[j].CodeValue) {
								found=true;
								break;
							}
						}
						if(!found) {
							listMedPats.RemoveAt(i);
						}						
					}
				}
			}
			#endregion
			gridInterventions.BeginUpdate();
			gridInterventions.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Date",70);
			gridInterventions.ListGridColumns.Add(col);
			col=new GridColumn("Intervention Type",115);
			gridInterventions.ListGridColumns.Add(col);
			col=new GridColumn("Code Description",200);
			gridInterventions.ListGridColumns.Add(col);
			gridInterventions.ListGridRows.Clear();
			GridRow row;
			#region AddInterventionRows
			for(int i=0;i<listIntervention.Count;i++) {
				row=new GridRow();
				row.Cells.Add(listIntervention[i].DateEntry.ToShortDateString());
				row.Cells.Add(listIntervention[i].CodeSet.ToString());
				//Description of Intervention---------------------------------------------
				//to get description, first determine which table the code is from.  Interventions are allowed to be SNOMEDCT, ICD9, ICD10, HCPCS, or CPT.
				string descript="";
				switch(listIntervention[i].CodeSystem) {
					case "SNOMEDCT":
						Snomed sCur=Snomeds.GetByCode(listIntervention[i].CodeValue);
						if(sCur!=null) {
							descript=sCur.Description;
						}
						break;
					case "ICD9CM":
						ICD9 i9Cur=ICD9s.GetByCode(listIntervention[i].CodeValue);
						if(i9Cur!=null) {
							descript=i9Cur.Description;
						}
						break;
					case "ICD10CM":
						Icd10 i10Cur=Icd10s.GetByCode(listIntervention[i].CodeValue);
						if(i10Cur!=null) {
							descript=i10Cur.Description;
						}
						break;
					case "HCPCS":
						Hcpcs hCur=Hcpcses.GetByCode(listIntervention[i].CodeValue);
						if(hCur!=null) {
							descript=hCur.DescriptionShort;
						}
						break;
					case "CPT":
						Cpt cptCur=Cpts.GetByCode(listIntervention[i].CodeValue);
						if(cptCur!=null) {
							descript=cptCur.Description;
						}
						break;
				}
				row.Cells.Add(descript);
				row.Tag=listIntervention[i];
				gridInterventions.ListGridRows.Add(row);
			}
			#endregion
			#region AddMedicationRows
			for(int i=0;i<listMedPats.Count;i++) {
				row=new GridRow();
				row.Cells.Add(listMedPats[i].DateStart.ToShortDateString());
				if(listMedPats[i].RxCui==314153 || listMedPats[i].RxCui==692876) {
					row.Cells.Add(InterventionCodeSet.AboveNormalWeight.ToString()+" Medication");
				}
				else {
					row.Cells.Add(InterventionCodeSet.BelowNormalWeight.ToString()+" Medication");
				}
				//Description of Medication----------------------------------------------
				//only meds in EHR table are from RxNorm table
				string descript=RxNorms.GetDescByRxCui(listMedPats[i].RxCui.ToString());
				row.Cells.Add(descript);
				row.Tag=listMedPats[i];
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
			DateTime examDate=PIn.Date(textDateTaken.Text);
			ageBeforeJanFirst=examDate.Year-patCur.Birthdate.Year-1;//This is how old this patient was before any birthday in the year the vital sign was taken, can be negative if patient born the year taken or if value in textDateTaken is empty or not a valid date
			if(!checkPregnant.Checked || VitalsignCur.PregDiseaseNum==0) {
				CalcBMI();//This will use new year taken to determine age at start of that year to show over/underweight if applicable using age specific criteria
				return;
			}
			Disease disCur=Diseases.GetOne(VitalsignCur.PregDiseaseNum);
			if(disCur!=null) {//the currently attached preg disease is valid, will be null if they checked the box on new exam but haven't hit ok to save
				if(examDate.Date<disCur.DateStart || (disCur.DateStop.Year>1880 && disCur.DateStop<examDate.Date)) {//if this exam is not in the active date range of the problem
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
			if(pregDisDefNumCur==0) {
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
			if(butChangeDefault.Text=="Go to Problem") {//text is "To Problem" only when vital sign has a valid PregDiseaseNum and the preg box is checked
				Disease disCur=Diseases.GetOne(VitalsignCur.PregDiseaseNum);
				if(disCur==null) {//should never happen, the only way the button will say "To Problem" is if this exam is pointing to a valid problem
					butChangeDefault.Text="Change Default";
					labelPregNotice.Visible=false;
					textPregCode.Clear();
					textPregCodeDescript.Clear();
					VitalsignCur.PregDiseaseNum=0;
					checkPregnant.Checked=false;
					return;
				}
				if(DiseaseDefs.GetItem(disCur.DiseaseDefNum)==null) {
					MessageBox.Show(Lan.g(this,"Invalid disease.  Please run database maintenance method")+" "
						+nameof(DatabaseMaintenances.DiseaseWithInvalidDiseaseDef));
					return;
				}
				using FormDiseaseEdit FormDis=new FormDiseaseEdit(disCur);
				FormDis.IsNew=false;
				FormDis.ShowDialog();
				if(FormDis.DialogResult==DialogResult.OK) {
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
					if(pregDisDefNumCur==0) {
						labelPregNotice.Visible=false;
						butChangeDefault.Text="Change Default";
					}
				}
			}
			else {
				if(!Security.IsAuthorized(Permissions.SecurityAdmin,false)) {
					return;
				}
				using FormEhrSettings FormEhr=new FormEhrSettings();
				FormEhr.ShowDialog();
				if(FormEhr.DialogResult!=DialogResult.OK || checkPregnant.Checked==false) {
					return;
				}
				labelPregNotice.Visible=false;
				SetPregCodeAndDescript();
				if(pregDisDefNumCur>0) {
					labelPregNotice.Visible=true;
				}
			}
		}

		private void checkNotPerf_Click(object sender,EventArgs e) {
			if(checkNotPerf.Checked) {
				using FormEhrNotPerformedEdit FormNP=new FormEhrNotPerformedEdit();
				if(VitalsignCur.EhrNotPerformedNum==0) {
					FormNP.EhrNotPerfCur=new EhrNotPerformed();
					FormNP.EhrNotPerfCur.IsNew=true;
					FormNP.EhrNotPerfCur.PatNum=patCur.PatNum;
					FormNP.EhrNotPerfCur.ProvNum=patCur.PriProv;
					FormNP.SelectedItemIndex=(int)EhrNotPerformedItem.BMIExam;//The code and code value will be set in FormEhrNotPerformedEdit, set the selected index to the EhrNotPerformedItem enum index for BMIExam
					FormNP.EhrNotPerfCur.DateEntry=PIn.Date(textDateTaken.Text);
					FormNP.IsDateReadOnly=true;//if this not performed item will be linked to this exam, force the dates to match.  User can change exam date and recheck the box to affect the not performed item date, but forcing them to be the same will allow us to avoid other complications.
				}
				else {
					FormNP.EhrNotPerfCur=EhrNotPerformeds.GetOne(VitalsignCur.EhrNotPerformedNum);
					FormNP.EhrNotPerfCur.IsNew=false;
					FormNP.SelectedItemIndex=(int)EhrNotPerformedItem.BMIExam;
				}
				FormNP.ShowDialog();
				if(FormNP.DialogResult==DialogResult.OK) {
					VitalsignCur.EhrNotPerformedNum=FormNP.EhrNotPerfCur.EhrNotPerformedNum;
					textReasonCode.Text=FormNP.EhrNotPerfCur.CodeValueReason;
					Snomed sCur=Snomeds.GetByCode(FormNP.EhrNotPerfCur.CodeValueReason);
					if(sCur!=null) {
						textReasonDescript.Text=sCur.Description;
					}
				}
				else {
					checkNotPerf.Checked=false;
					textReasonCode.Clear();
					textReasonDescript.Clear();
					if(EhrNotPerformeds.GetOne(VitalsignCur.EhrNotPerformedNum)==null) {//could be linked to a not performed item that no longer exists or has been deleted
						VitalsignCur.EhrNotPerformedNum=0;
					}
				}
			}
			else {
				textReasonCode.Clear();
				textReasonDescript.Clear();
			}
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormInterventionEdit FormInt=new FormInterventionEdit();
			FormInt.InterventionCur=new Intervention();
			FormInt.InterventionCur.IsNew=true;
			FormInt.InterventionCur.PatNum=patCur.PatNum;
			FormInt.InterventionCur.ProvNum=patCur.PriProv;
			FormInt.InterventionCur.DateEntry=PIn.Date(textDateTaken.Text);
			FormInt.InterventionCur.CodeSet=intervCodeSet;
			FormInt.IsAllTypes=false;
			FormInt.IsSelectionMode=true;
			FormInt.ShowDialog();
			if(FormInt.DialogResult==DialogResult.OK) {
				bool child=ageBeforeJanFirst<17;
				FillGridInterventions(child);
			}
		}

		private void gridInterventions_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			object objCur=gridInterventions.ListGridRows[e.Row].Tag;
			if(objCur.GetType().Name=="Intervention") {//grid can contain MedicationPat or Intervention objects, launch appropriate window
				using FormInterventionEdit FormInt=new FormInterventionEdit();
				FormInt.InterventionCur=(Intervention)objCur;
				FormInt.IsAllTypes=false;
				FormInt.IsSelectionMode=false;
				FormInt.ShowDialog();
			}
			if(objCur.GetType().Name=="MedicationPat") {
				using FormMedPat FormMP=new FormMedPat();
				FormMP.MedicationPatCur=(MedicationPat)objCur;
				FormMP.IsNew=false;
				FormMP.ShowDialog();
			}
			FillGridInterventions(ageBeforeJanFirst<17);
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

		private void butOK_Click(object sender,EventArgs e) {
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
			if(date<patCur.Birthdate) {
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
			int BPsys=0;
			int BPdia=0;
			try {
				if(textBPs.Text!="") {
					BPsys = int.Parse(textBPs.Text);
				}
				if(textBPd.Text!="") {
					BPdia = int.Parse(textBPd.Text);
				}
			}
			catch {
				MsgBox.Show(this,"Please fix BP first.");
				return;
			}
			if(BPsys<0 || BPdia<0) {
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
			#endregion
			#region Save
			VitalsignCur.DateTaken=date;
			VitalsignCur.Pulse=pulse;
			VitalsignCur.Height=height;
			VitalsignCur.Weight=weight;
			VitalsignCur.BpDiastolic=BPdia;
			VitalsignCur.BpSystolic=BPsys;
			//textBMIPercentile will be the calculated percentile or -1 if not in age range or there is an error calculating the percentile.
			//In this case the text box will be not visible, but the text will be set to -1 and we will store it that way to indicate no valid BMIPercentile
			VitalsignCur.BMIPercentile=PIn.Int(textBMIPercentile.Text);//could be -1 if not in age range or error calculating percentile
			VitalsignCur.BMIExamCode="";
			if(textBMIPercentileCode.Visible && textBMIPercentileCode.Text!="") {
				VitalsignCur.BMIExamCode="59576-9";//Body mass index (BMI) [Percentile] Per age and gender, only code used for percentile, only visible if under 17 at time of exam
			}
			if(comboHeightExamCode.SelectedIndex>0) {
				VitalsignCur.HeightExamCode=listHeightCodes[comboHeightExamCode.SelectedIndex-1].LoincCode;
			}
			if(comboWeightExamCode.SelectedIndex>0) {
				VitalsignCur.WeightExamCode=listWeightCodes[comboWeightExamCode.SelectedIndex-1].LoincCode;
			}
			switch(labelWeightCode.Text) {
				case "Overweight":
					if(Snomeds.GetByCode("238131007")!=null) {
						VitalsignCur.WeightCode="238131007";
					}
					break;
				case "Underweight":
					if(Snomeds.GetByCode("248342006")!=null) {
						VitalsignCur.WeightCode="248342006";
					}
					break;
				case "":
				default:
					VitalsignCur.WeightCode="";
					break;
			}
			#region PregnancyDx
			if(checkPregnant.Checked) {//pregnant, add pregnant dx if necessary
				if(pregDisDefNumCur==0) {
					//shouldn't happen, if checked this must be set to either an existing problem def or a new problem that requires inserting, return to form with checkPregnant unchecked
					MsgBox.Show(this,"This exam must point to a valid pregnancy diagnosis.");
					checkPregnant.Checked=false;
					labelPregNotice.Visible=false;
					return;
				}
				if(VitalsignCur.PregDiseaseNum==0) {//insert new preg disease and update vitalsign to point to it
					Disease pregDisNew=new Disease();
					pregDisNew.PatNum=VitalsignCur.PatNum;
					pregDisNew.DiseaseDefNum=pregDisDefNumCur;
					pregDisNew.DateStart=VitalsignCur.DateTaken;
					pregDisNew.ProbStatus=ProblemStatus.Active;
					VitalsignCur.PregDiseaseNum=Diseases.Insert(pregDisNew);
				}
				else {
					Disease disCur=Diseases.GetOne(VitalsignCur.PregDiseaseNum);
					if(VitalsignCur.DateTaken<disCur.DateStart
						|| (disCur.DateStop.Year>1880 && VitalsignCur.DateTaken>disCur.DateStop))
					{//the current exam is no longer within dates of preg problem, uncheck the pregnancy box and remove the pointer to the disease
						MsgBox.Show(this,"This exam is not within the active dates of the attached pregnancy problem.");
						checkPregnant.Checked=false;
						textPregCode.Clear();
						textPregCodeDescript.Clear();
						labelPregNotice.Visible=false;
						VitalsignCur.PregDiseaseNum=0;
						butChangeDefault.Text="Change Default";
						return;
					}
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
				using FormCDSIntervention FormCDSI=new FormCDSIntervention();
				FormCDSI.ListCDSInterventions=EhrTriggers.TriggerMatch(VitalsignCur,Patients.GetPat(VitalsignCur.PatNum));
				FormCDSI.ShowIfRequired(false);
			}
			#endregion
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
		

	

	


	}
}
