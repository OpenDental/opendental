using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Linq;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormScreenGroupEdit:FormODBase {
		private ScreenGroup _screenGroup;
		private List<OpenDentBusiness.Screen> _listScreens;
		private List<ScreenPat> _listScreenPats;
		///<summary>Stale deep copy of _listScreenPats to use with sync.</summary>
		private List<ScreenPat> _listScreenPatsOld;
		private List<Provider> _listProviders;
		private List<SheetDef> _listSheetDefsScreening;

		///<summary></summary>
		public FormScreenGroupEdit(ScreenGroup screenGroup) {
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			gridScreenPats.ContextMenu=patContextMenu;
			Lan.F(this);
			_screenGroup=screenGroup;
		}

		private void FormScreenGroup_Load(object sender, System.EventArgs e) {
			if(_screenGroup.IsNew) {
				ScreenGroups.Insert(_screenGroup);
			}
			_listScreenPats=ScreenPats.GetForScreenGroup(_screenGroup.ScreenGroupNum);
			_listScreenPatsOld=_listScreenPats.Select(x => x.Clone()).ToList();
			FillGrid();
			FillScreenPats();
			textScreenDate.Text=_screenGroup.SGDate.ToShortDateString();
			textDescription.Text=_screenGroup.Description;
			textProvName.Text=_screenGroup.ProvName;//has to be filled before provnum
			_listProviders=Providers.GetDeepCopy(true);
			for(int i=0;i<_listProviders.Count;i++) {
				comboProv.Items.Add(_listProviders[i].Abbr);
				if(_screenGroup.ProvNum==_listProviders[i].ProvNum) {
					comboProv.SelectedIndex=i;
				}
			}
			List<string> listStringCountyNames=Counties.GetListNames();
			comboCounty.Items.AddList(listStringCountyNames);
			if(_screenGroup.County==null) {
				_screenGroup.County="";//prevents the next line from crashing
			}
			for(int i=0;i<comboCounty.Items.Count;i++) {
				string countyName=comboCounty.Items.GetObjectAt(i).ToString();
				if(countyName==_screenGroup.County) {
					comboCounty.SelectedIndex=i;
					break;
				}
			}
			List<string> listGradeSchoolNames=Sites.GetDeepCopy().Select(x => x.Description).ToList();
			comboGradeSchool.Items.AddList(listGradeSchoolNames);
			if(_screenGroup.GradeSchool==null) {
				_screenGroup.GradeSchool="";//prevents the next line from crashing
			}
			for(int i=0;i<comboGradeSchool.Items.Count;i++) {
				string gradeSchoolName=comboGradeSchool.Items.GetObjectAt(i).ToString();
				if(gradeSchoolName==_screenGroup.GradeSchool) {
					comboGradeSchool.SelectedIndex=i;
					break;
				}
			}
			//comboGradeSchool.SelectedIndex=comboGradeSchool.Items.IndexOf(_screenGroup.GradeSchool);//"" etc OK
			comboPlaceService.Items.AddList(Enum.GetNames(typeof(PlaceOfService)));
			comboPlaceService.SelectedIndex=(int)_screenGroup.PlaceService;
			_listSheetDefsScreening=SheetDefs.GetCustomForType(SheetTypeEnum.Screening);
			if(PrefC.GetBool(PrefName.ScreeningsUseSheets)) {
				comboSheetDefs.Items.Add(Lan.g(this,"Default"));
				for(int i=0;i<_listSheetDefsScreening.Count();i++) {
					comboSheetDefs.Items.Add(_listSheetDefsScreening[i].Description);
				}
				if(_screenGroup.SheetDefNum==0) {
					comboSheetDefs.SelectedIndex=0;
				}
				else {
					int idx=_listSheetDefsScreening.FindIndex(x => x.SheetDefNum==_screenGroup.SheetDefNum);
					if(idx==-1) {//Sheet def deleted
						comboSheetDefs.SelectedIndex=0;//Default
					}
					else {
						comboSheetDefs.SelectedIndex=idx+1;
					}
				}
				return;
			}
			labelSheet.Visible=false;
			comboSheetDefs.Visible=false;
		}

		private void FillGrid() {
			_listScreens=Screens.GetScreensForGroup(_screenGroup.ScreenGroupNum);
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"#"),30);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Name"),100);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Grade"),55);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Age"),40);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Race"),105);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Sex"),45);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Urgency"),70);
			gridMain.Columns.Add(col);
			if(!Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				col=new GridColumn(Lan.g(this,"Caries"),45,HorizontalAlignment.Center);
				gridMain.Columns.Add(col);
				col=new GridColumn(Lan.g(this,"ECC"),30,HorizontalAlignment.Center);
				gridMain.Columns.Add(col);
				col=new GridColumn(Lan.g(this,"CarExp"),50,HorizontalAlignment.Center);
				gridMain.Columns.Add(col);
				col=new GridColumn(Lan.g(this,"ExSeal"),45,HorizontalAlignment.Center);
				gridMain.Columns.Add(col);
				col=new GridColumn(Lan.g(this,"NeedSeal"),60,HorizontalAlignment.Center);
				gridMain.Columns.Add(col);
				col=new GridColumn(Lan.g(this,"NoTeeth"),55,HorizontalAlignment.Center);
				gridMain.Columns.Add(col);
			}
			col=new GridColumn(Lan.g(this,"Comments"),100);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			List<OpenDentBusiness.Screen> listScreensToDelete=new List<OpenDentBusiness.Screen>();
			List<Sheet> listSheets=Sheets.GetSheets(_listScreens.Select(x=>x.SheetNum).ToList());
			for(int i=0;i<_listScreens.Count();i++) {
				row=new GridRow();
				ScreenPat screenPat=_listScreenPats.FirstOrDefault(x => x.ScreenPatNum==_listScreens[i].ScreenPatNum);
				Sheet sheet=listSheets.FirstOrDefault(x=>x.SheetNum==_listScreens[i].SheetNum);
				if(screenPat!=null && sheet!=null && sheet.PatNum!=screenPat.PatNum) {
					listScreensToDelete.Add(_listScreens[i]);
					continue;
				}
				row.Cells.Add(_listScreens[i].ScreenGroupOrder.ToString());
				row.Cells.Add((screenPat==null)?"Anonymous":Patients.GetLim(screenPat.PatNum).GetNameLF());
				row.Cells.Add(_listScreens[i].GradeLevel.ToString());
				row.Cells.Add(_listScreens[i].Age.ToString());
				row.Cells.Add(_listScreens[i].RaceOld.ToString());
				row.Cells.Add(_listScreens[i].Gender.ToString());
				row.Cells.Add(_listScreens[i].Urgency.ToString());
				if(!Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
					row.Cells.Add(_listScreens[i].HasCaries==YN.Yes ? "X":"");
					row.Cells.Add(_listScreens[i].EarlyChildCaries==YN.Yes ? "X":"");
					row.Cells.Add(_listScreens[i].CariesExperience==YN.Yes ? "X":"");
					row.Cells.Add(_listScreens[i].ExistingSealants==YN.Yes ? "X":"");
					row.Cells.Add(_listScreens[i].NeedsSealants==YN.Yes ? "X":"");
					row.Cells.Add(_listScreens[i].MissingAllTeeth==YN.Yes ? "X":"");
				}
				row.Cells.Add(_listScreens[i].Comments);
				gridMain.ListGridRows.Add(row);
			}
			if(listScreensToDelete.Count>0) {
				List<long> listScreenPatNumsToShow=listScreensToDelete.Select(y=>y.ScreenPatNum).ToList();
				//Get the PatNum from the list of ScreenPat objects.
				List<long> listPatNumsToShow=_listScreenPats.FindAll(x=>listScreenPatNumsToShow.Contains(x.ScreenPatNum)).Select(x=>x.PatNum).ToList();
				List<Patient> listPatients=Patients.GetLimForPats(listPatNumsToShow);
				string removedSheetPatientNames="";
				for(int i=0;i<listPatients.Count();i++) {
					if(i > 0) {
						removedSheetPatientNames+=", ";
					}
					if(i==listPatients.Count-1) {
						removedSheetPatientNames+=" and ";
					}
					removedSheetPatientNames+=listPatients[i].GetNameLF();
				}
				MsgBox.Show($"Screening sheet mismatch(es) encountered, removing the sheet(s) for {removedSheetPatientNames} from the screening group." );
				List<long> listScreenNumsToRemove=listScreensToDelete.Select(x=>x.ScreenNum).ToList();
				Screens.DeleteScreens(listScreenNumsToRemove);
				_listScreens.RemoveAll(x=>listScreenNumsToRemove.Contains(x.ScreenNum));
			}
			gridMain.Title=Lan.g(this,"Screenings")+" - "+_listScreens.Count;
			gridMain.EndUpdate();
		}

		private void FillScreenPats() {
			gridScreenPats.BeginUpdate();
			gridScreenPats.Title=Lan.g(this,"Patients for Screening")+" - "+_listScreenPats.Count;
			gridScreenPats.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"Patient"),200);
			gridScreenPats.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Permission"),100);
			gridScreenPats.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Screened"),90,HorizontalAlignment.Center);
			gridScreenPats.Columns.Add(col);
			gridScreenPats.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listScreenPats.Count();i++) {
				row=new GridRow();
				Patient pat=Patients.GetLim(_listScreenPats[i].PatNum);
				row.Cells.Add(pat.GetNameLF());
				row.Cells.Add(_listScreenPats[i].PatScreenPerm.ToString());
				OpenDentBusiness.Screen screen=_listScreens.FirstOrDefault(x => x.ScreenPatNum==_listScreenPats[i].ScreenPatNum);
				row.Cells.Add((screen==null)?"":"X");
				gridScreenPats.ListGridRows.Add(row);
			}
			gridScreenPats.EndUpdate();
		}

		///<summary></summary>
		private void AddAnonymousScreens() {
			while(true) {
				using FormScreenEdit formScreenEdit=new FormScreenEdit();
				formScreenEdit.ScreenGroupCur=_screenGroup;
				formScreenEdit.IsNew=true;
				if(_listScreens.Count==0) {
					formScreenEdit.ScreenCur=new OpenDentBusiness.Screen();
					formScreenEdit.ScreenCur.ScreenGroupOrder=1;
				}
				else {
					formScreenEdit.ScreenCur=_listScreens[_listScreens.Count-1];//'remembers' the last entry
					formScreenEdit.ScreenCur.ScreenGroupOrder=formScreenEdit.ScreenCur.ScreenGroupOrder+1;//increments for next
				}
				//For Anonymous patients always default to unknowns.
				formScreenEdit.ScreenCur.Gender=PatientGender.Unknown;
				formScreenEdit.ScreenCur.RaceOld=PatientRaceOld.Unknown;
				formScreenEdit.ShowDialog();
				if(formScreenEdit.DialogResult!=DialogResult.OK) {
					return;
				}
				formScreenEdit.ScreenCur.ScreenGroupOrder++;
				FillGrid();
			}
		}

		///<summary></summary>
		private void AddAnonymousScreensForSheets() {
			//Get the first custom Screening sheet or use the internal one
			SheetDef sheetDef=SheetDefs.GetInternalOrCustom(SheetInternalType.Screening);
			Sheet sheet=SheetUtil.CreateSheet(sheetDef);
			sheet.IsNew=true;
			SheetParameter.SetParameter(sheet,"ScreenGroupNum",_screenGroup.ScreenGroupNum);
			SheetFiller.FillFields(sheet);
			SheetUtil.CalculateHeights(sheet);
			//Create a valid screen so that we can create a screening sheet with the corresponding ScreenNum.
			OpenDentBusiness.Screen screen=new OpenDentBusiness.Screen();
			screen.ScreenGroupNum=_screenGroup.ScreenGroupNum;
			screen.ScreenGroupOrder=1;
			if(_listScreens.Count!=0) {
				screen.ScreenGroupOrder=_listScreens.Last().ScreenGroupOrder+1;//increments for next
			}
			while(true) {
				//Trick the sheet into thinking it is a "new" screen but keep all the other values from the previous screening.
				screen.IsNew=true;
				screen.ScreenNum=0;
				//For Anonymous patients always default to unknowns.
				List<SheetField> listSheetFields=sheet.SheetFields.FindAll(x => x.FieldType==SheetFieldType.ComboBox);
				for(int i=0;i<listSheetFields.Count();i++) {
					int startIndex=listSheetFields[i].FieldValue.IndexOf(';');
					if(startIndex < 0) {
						continue;//Not a valid value for sheet field type of combo box, do nothing.
					}
					switch(listSheetFields[i].FieldName) {
						case "Race/Ethnicity":
						case "Gender":
							listSheetFields[i].FieldValue="Unknown"+listSheetFields[i].FieldValue.Substring(startIndex);
							break;
					}
				}
				using FormSheetFillEdit formSheetFillEdit=new FormSheetFillEdit();
				formSheetFillEdit.SheetCur=sheet;
				formSheetFillEdit.ShowDialog();
				if(formSheetFillEdit.DialogResult!=DialogResult.OK) {
					return;
				}
				Screens.CreateScreenFromSheet(sheet,screen);
				screen.ScreenGroupOrder++;
				FillGrid();
			}
		}

		private void StartScreensForPats() {
			if(_listScreenPats.Count==0) {
				MsgBox.Show(this,"No patients for screening.");
				return;
			}
			int selectedIdx=gridScreenPats.GetSelectedIndex();
			int i=selectedIdx;
			if(i==-1) {
				i=0;
			}
			while(true) {
				ScreenPat screenPat=_listScreenPats[i];
				if(screenPat.PatScreenPerm!=PatScreenPerm.Allowed) {
					i=(i+1)%_listScreenPats.Count;//Causes the index to loop around when it gets to the end of the list so we can get to the beginning again.
					if(i==selectedIdx && selectedIdx!=-1) {
						break;
					}
					if(i==0 && selectedIdx==-1) {
						break;
					}
					continue;//Skip people who aren't allowed
				}
				if(_listScreens.FirstOrDefault(x => x.ScreenPatNum==screenPat.ScreenPatNum)!=null) {
					i=(i+1)%_listScreenPats.Count;//Causes the index to loop around when it gets to the end of the list so we can get to the beginning again.
					if(i==selectedIdx && selectedIdx!=-1) {
						break;
					}
					if(i==0 && selectedIdx==-1) {
						break;
					}
					continue;//If they already have a screen, don't make a new one.  We might think about opening up their old one for editing at this point.
				}
				using FormScreenEdit formScreenEdit=new FormScreenEdit();
				formScreenEdit.ScreenGroupCur=_screenGroup;
				formScreenEdit.IsNew=true;
				formScreenEdit.ScreenPatCur=screenPat;
				if(_listScreens.Count==0) {
					formScreenEdit.ScreenCur=new OpenDentBusiness.Screen();
					formScreenEdit.ScreenCur.ScreenGroupOrder=1;
				}
				else {
					formScreenEdit.ScreenCur=_listScreens[_listScreens.Count-1].Copy();//'remembers' the last entry, needs to be a deep copy so we don't change screen information.
					formScreenEdit.ScreenCur.ScreenGroupOrder=formScreenEdit.ScreenCur.ScreenGroupOrder+1;//increments for next
				}
				Patient patient=Patients.GetPat(screenPat.PatNum);//Get a patient so we can pre-fill some of the information (age/sex/birthdate/grade)
				if (patient.Birthdate!=DateTime.MinValue) {
					formScreenEdit.ScreenCur.Age=(byte)patient.Age;
					formScreenEdit.ScreenCur.Birthdate=patient.Birthdate;
				}
				formScreenEdit.ScreenCur.Gender=patient.Gender;//Default value in pat edit is male. No way of knowing if it's intentional or not, just use it.
				if (patient.GradeLevel==0) {
					formScreenEdit.ScreenCur.GradeLevel=patient.GradeLevel;
				}
				formScreenEdit.ScreenCur.RaceOld=PatientRaceOld.Unknown;//Default to unknown. Patient Edit doesn't have the same type of race as screen edit.
				formScreenEdit.ScreenCur.Urgency=patient.Urgency;
				if(formScreenEdit.ShowDialog()!=DialogResult.OK) {
					break;
				}
				formScreenEdit.ScreenCur.ScreenGroupOrder++;
				FillGrid();
				i=(i+1)%_listScreenPats.Count;//Causes the index to loop around when it gets to the end of the list so we can get to the beginning again.
				if(i==selectedIdx && selectedIdx!=-1) {
					break;
				}
				if(i==0 && selectedIdx==-1) {
					break;
				}
			}
			FillScreenPats();
		}

		private void StartScreensForPatsWithSheets() {
			//Get the first custom Screening sheet or use the internal one
			SheetDef sheetDef=null;
			if(comboSheetDefs.SelectedIndex==0) {
				sheetDef=SheetsInternal.GetSheetDef(SheetInternalType.Screening);
			}
			else {
				sheetDef=_listSheetDefsScreening[comboSheetDefs.SelectedIndex-1];
				SheetDefs.GetFieldsAndParameters(sheetDef);
			}
			int selectedIdx=gridScreenPats.GetSelectedIndex();
			int i=selectedIdx;
			if(i==-1) {
				i=0;
			}
			while(true) {
				ScreenPat screenPat=_listScreenPats[i];
				if(screenPat.PatScreenPerm!=PatScreenPerm.Allowed) {
					i=(i+1)%_listScreenPats.Count;//Causes the index to loop around when it gets to the end of the list so we can get to the beginning again.
					if(i==selectedIdx && selectedIdx!=-1) {
						break;
					}
					if(i==0 && selectedIdx==-1) {
						break;
					}
					continue;//Skip people who aren't allowed
				}
				if(_listScreens.FirstOrDefault(x => x.ScreenPatNum==screenPat.ScreenPatNum)!=null) {
					i=(i+1)%_listScreenPats.Count;//Causes the index to loop around when it gets to the end of the list so we can get to the beginning again.
					if(i==selectedIdx && selectedIdx!=-1) {
						break;
					}
					if(i==0 && selectedIdx==-1) {
						break;
					}
					continue;//If they already have a screen, don't make a new one.  We might think about opening up their old one for editing at this point.
				}
				Sheet sheet=SheetUtil.CreateSheet(sheetDef);
				sheet.IsNew=true;
				sheet.PatNum=screenPat.PatNum;
				long provNum=0;
				if(comboProv.SelectedIndex!=-1) {
					provNum=_listProviders[comboProv.SelectedIndex].ProvNum;
				}
				SheetParameter.SetParameter(sheet,"ScreenGroupNum",_screenGroup.ScreenGroupNum);//I think we may need this.
				SheetParameter.SetParameter(sheet,"PatNum",screenPat.PatNum);
				SheetParameter.SetParameter(sheet,"ProvNum",provNum);
				SheetFiller.FillFields(sheet);
				SheetUtil.CalculateHeights(sheet);
				//Create a valid screen so that we can create a screening sheet with the corresponding ScreenNum.
				OpenDentBusiness.Screen screen=new OpenDentBusiness.Screen();
				screen.ScreenPatNum=screenPat.ScreenPatNum;
				screen.ScreenGroupNum=_screenGroup.ScreenGroupNum;
				screen.ScreenGroupOrder=1;
				if(_listScreens.Count!=0) {
					screen.ScreenGroupOrder=_listScreens.Last().ScreenGroupOrder+1;//increments for next
				}
				List<SheetField> listSheetFieldsChartOrigVals=new List<SheetField>();
				SheetField sheetFieldTreatment=sheet.SheetFields.Find(x=>x.FieldName=="ChartSealantTreatment" && x.FieldType==SheetFieldType.ScreenChart);
				SheetField sheetFieldComplete=sheet.SheetFields.Find(x=>x.FieldName=="ChartSealantComplete" && x.FieldType==SheetFieldType.ScreenChart);
				if (sheetFieldTreatment==null) {
					listSheetFieldsChartOrigVals.Add(null);//Adds null entry if it's not found, which is fine.
				}
				else {
					listSheetFieldsChartOrigVals.Add(sheetFieldTreatment.Copy());
				}
				if (sheetFieldComplete==null) {
					listSheetFieldsChartOrigVals.Add(null);//Adds null entry if it's not found, which is fine.
				}
				else {
					listSheetFieldsChartOrigVals.Add(sheetFieldComplete.Copy());
				}
				List<SheetField> listSheetFieldProcOrigVals=new List<SheetField>();
				SheetField sheetFieldAssess=sheet.SheetFields.Find(x=>x.FieldName=="AssessmentProc" && x.FieldType==SheetFieldType.CheckBox);
				SheetField sheetFieldFluoride=sheet.SheetFields.Find(x=>x.FieldName=="FluorideProc" && x.FieldType==SheetFieldType.CheckBox);
				if(sheetFieldAssess!=null) {
					listSheetFieldProcOrigVals.Add(sheetFieldAssess.Copy());
				}
				if(sheetFieldFluoride!=null) {
					listSheetFieldProcOrigVals.Add(sheetFieldFluoride.Copy());
				}
				List<SheetField> listSheetFieldsProcs=sheet.SheetFields.FindAll(x => x.FieldName.StartsWith("Proc") && x.FieldType==SheetFieldType.CheckBox);
				listSheetFieldsProcs.ForEach(x => listSheetFieldProcOrigVals.Add(x.Copy()));//Adds other proc checkbox fields.
				using FormSheetFillEdit formSheetFillEdit=new FormSheetFillEdit();
				formSheetFillEdit.SheetCur=sheet;
				formSheetFillEdit.ShowDialog();
				if(formSheetFillEdit.DialogResult!=DialogResult.OK) {
					break;
				}
				if(formSheetFillEdit.SheetCur!=null) {//It wasn't deleted, create a screen.
					Screens.CreateScreenFromSheet(sheet,screen);
					//Now try and process the screening chart for treatment planned sealants.
					if(ProcedureCodes.GetCodeNum("D1351")==0) {
						MsgBox.Show(this,"The required sealant code is not present in the database.  The screening chart will not be processed.");
						break;
					}
					//Process both TP and Compl charts.
					Screens.ProcessScreenChart(sheet,ScreenChartType.TP|ScreenChartType.C,provNum,formSheetFillEdit.SheetCur.SheetNum,listSheetFieldsChartOrigVals,listSheetFieldProcOrigVals);
				}
				screen.ScreenGroupOrder++;
				FillGrid();
				i=(i+1)%_listScreenPats.Count;//Causes the index to loop around when it gets to the end of the list so we can get to the beginning again.
				if(i==selectedIdx && selectedIdx!=-1) {
					break;
				}
				if(i==0 && selectedIdx==-1) {
					break;
				}
			}
			FillScreenPats();
		}

		private void ViewScreenForPat(OpenDentBusiness.Screen screenCur) {
			using FormScreenEdit formScreenEdit=new FormScreenEdit();
			formScreenEdit.ScreenGroupCur=_screenGroup;
			formScreenEdit.IsNew=false;
			formScreenEdit.ScreenCur=screenCur;
			ScreenPat screenPat=_listScreenPats.FirstOrDefault(x => x.ScreenPatNum==screenCur.ScreenPatNum);
			formScreenEdit.ScreenPatCur=screenPat;//Null represents anonymous.
			formScreenEdit.ShowDialog();
		}

		private void ViewScreenForPatWithSheets(OpenDentBusiness.Screen screenCur) {
			Sheet sheet=Sheets.GetSheet(screenCur.SheetNum);
			if(sheet==null) {
				MsgBox.Show(this,"Sheet no longer exists.  It may have been deleted from the Chart Module.");
				return;
			}
			List<SheetField> listSheetFieldsChartOrigVals=new List<SheetField>();
			SheetField sheetFieldTreatment=sheet.SheetFields.Find(x=>x.FieldName=="ChartSealantTreatment");
			SheetField sheetFieldComplete=sheet.SheetFields.Find(x=>x.FieldName=="ChartSealantComplete");
			if (sheetFieldTreatment==null) {
				listSheetFieldsChartOrigVals.Add(null); //Adds null entry if it's not found, which is fine.
			}
			else {
				listSheetFieldsChartOrigVals.Add(sheetFieldTreatment.Copy());
			}
			if (sheetFieldComplete==null) {
				listSheetFieldsChartOrigVals.Add(null); //Adds null entry if it's not found, which is fine.
			}
			else {
				listSheetFieldsChartOrigVals.Add(sheetFieldComplete.Copy());
			}
			List<SheetField> listSheetFieldsProcOrigVals=new List<SheetField>();
			SheetField sheetFieldAssess=sheet.SheetFields.Find(x=>x.FieldName=="AssessmentProc");
			SheetField sheetFieldFluoride=sheet.SheetFields.Find(x=>x.FieldName=="FluorideProc");
			if(sheetFieldAssess!=null) {
				listSheetFieldsProcOrigVals.Add(sheetFieldAssess.Copy());
			}
			if(sheetFieldFluoride!=null) {
				listSheetFieldsProcOrigVals.Add(sheetFieldFluoride.Copy());
			}
			List<SheetField> listSheetFieldProcs=sheet.SheetFields.FindAll(x => x.FieldName.StartsWith("Proc") && x.FieldType==SheetFieldType.CheckBox);
			listSheetFieldProcs.ForEach(x => listSheetFieldsProcOrigVals.Add(x.Copy()));//Adds other proc checkbox fields.
			using FormSheetFillEdit formSheetFillEdit=new FormSheetFillEdit();
			formSheetFillEdit.SheetCur=sheet;
			if(formSheetFillEdit.ShowDialog()==DialogResult.OK) {
				//Now try and process the screening chart for completed sealants.
				if(formSheetFillEdit.SheetCur==null) {
					return;
				}
				if(ProcedureCodes.GetCodeNum("D1351")==0) {
					MsgBox.Show(this,"The required sealant code is not present in the database.  The screening chart will not be processed.");
					return;
				}
				long provNum=0;
				if(comboProv.SelectedIndex!=-1) {
					provNum=_listProviders[comboProv.SelectedIndex].ProvNum;
				}
				Screens.ProcessScreenChart(sheet,ScreenChartType.TP|ScreenChartType.C,provNum,formSheetFillEdit.SheetCur.SheetNum,listSheetFieldsChartOrigVals,listSheetFieldsProcOrigVals);						
			}
			_listScreens[gridMain.GetSelectedIndex()]=Screens.CreateScreenFromSheet(sheet,_listScreens[gridMain.GetSelectedIndex()]);
		}

		private void patContextMenuItem_Click(object sender,EventArgs e) {
			if(gridScreenPats.GetSelectedIndex()==-1) {
				return;
			}
			int idx=patContextMenu.MenuItems.IndexOf((MenuItem)sender);
			_listScreenPats[gridScreenPats.GetSelectedIndex()].PatScreenPerm=(PatScreenPerm)idx;
			FillScreenPats();
		}

		private void listMain_DoubleClick(object sender, System.EventArgs e) {
			using FormScreenEdit formScreenEdit=new FormScreenEdit();
			formScreenEdit.ScreenCur=_listScreens[gridMain.SelectedIndices[0]];
			formScreenEdit.ScreenGroupCur=_screenGroup;
			formScreenEdit.ShowDialog();
			if(formScreenEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			FillGrid();
		}

		private void textScreenDate_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
			try{
				DateTime.Parse(textScreenDate.Text);
			}
			catch{
				MessageBox.Show("Date invalid");
				e.Cancel=true;
			}
		}

		private void textProvName_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e) {
			comboProv.SelectedIndex=-1;//set the provnum to none.
		}

		private void comboProv_SelectedIndexChanged(object sender, System.EventArgs e) {
			if(comboProv.SelectedIndex!=-1) {//if a prov was selected
				//set the provname accordingly
				textProvName.Text=_listProviders[comboProv.SelectedIndex].LName+", "
					+_listProviders[comboProv.SelectedIndex].FName;
			}
		}

		private void comboProv_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if(e.KeyCode==Keys.Back || e.KeyCode==Keys.Delete){
				comboProv.SelectedIndex=-1;
			}
		}

		private void comboCounty_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if(e.KeyCode==Keys.Back || e.KeyCode==Keys.Delete){
				comboCounty.SelectedIndex=-1;
			}
		}

		private void comboGradeSchool_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if(e.KeyCode==Keys.Back || e.KeyCode==Keys.Delete){
				comboGradeSchool.SelectedIndex=-1;
			}
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(PrefC.GetBool(PrefName.ScreeningsUseSheets)) {
				ViewScreenForPatWithSheets(_listScreens[e.Row]);
			}
			else {
				ViewScreenForPat(_listScreens[e.Row]);
			}
			FillGrid();
			FillScreenPats();
		}
		
		private void gridScreenPats_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.PatientEdit)) {
				return;
			}
			Family family=Patients.GetFamily(_listScreenPats[gridScreenPats.GetSelectedIndex()].PatNum);
			Patient patient=family.GetPatient(_listScreenPats[gridScreenPats.GetSelectedIndex()].PatNum);
			using FormPatientEdit formPatientEdit=new FormPatientEdit(patient,family);
			if(formPatientEdit.ShowDialog()!=DialogResult.OK) {//Information may have changed. Look for screens for this patient that may need changing.
				return;
			}
			ScreenPat screenPat=_listScreenPats.FirstOrDefault(x => x.PatNum==patient.PatNum);
			for(int i=0;i<_listScreens.Count();i++) {
				if(_listScreens[i].ScreenPatNum!=screenPat.ScreenPatNum) {//Found a screen belonging to the edited patient.
					continue;
				}
				//Don't unintelligently overwrite the screen's values.  They may have entered in patient information not pertaining to the screen such as address.
				if (patient.Birthdate!=DateTime.MinValue) {
					_listScreens[i].Birthdate=patient.Birthdate;
					_listScreens[i].Age=(byte)patient.Age;
				}
				_listScreens[i].Gender=patient.Gender;//Default value in pat edit is male. No way of knowing if it's intentional or not, just use it.
				if (patient.GradeLevel!=0) {
					_listScreens[i].GradeLevel=patient.GradeLevel;
				}
				Screens.Update(_listScreens[i]);
				FillGrid();
			}
		}

		private void butAddAnonymous_Click(object sender,System.EventArgs e) {
			if(PrefC.GetBool(PrefName.ScreeningsUseSheets)) {
				AddAnonymousScreensForSheets();
			}
			else {
				AddAnonymousScreens();
			}
		}

		private void butAddPat_Click(object sender,EventArgs e) {
			using FormPatientSelect formPatientSelect=new FormPatientSelect();
			formPatientSelect.ShowDialog();
			if(formPatientSelect.DialogResult!=DialogResult.OK) {
				return;
			}
			ScreenPat screenPat=_listScreenPats.FirstOrDefault(x => x.PatNum==formPatientSelect.PatNumSelected);
			if(screenPat!=null) {
				MsgBox.Show(this,"Cannot add patient already in screen group.");
				for(int i=0;i<_listScreenPats.Count;i++) {
					if(_listScreenPats[i].ScreenPatNum==screenPat.ScreenPatNum) {
						gridScreenPats.SetSelected(i,true);
						break;
					}
				}
				return;
			}
			screenPat=new ScreenPat();
			screenPat.PatNum=formPatientSelect.PatNumSelected;
			screenPat.PatScreenPerm=PatScreenPerm.Unknown;
			screenPat.ScreenGroupNum=_screenGroup.ScreenGroupNum;
			ScreenPats.Insert(screenPat);
			_listScreenPats.Add(screenPat);
			_listScreenPatsOld.Add(screenPat.Clone());
			FillScreenPats();
		}
		
		private void butRemovePat_Click(object sender,EventArgs e) {
			if(gridScreenPats.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Please select a patient to remove first.");
				return;
			}
			_listScreenPats.RemoveAt(gridScreenPats.GetSelectedIndex());
			FillScreenPats();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Are you sure you want to delete this screening group? All screenings in this group will be deleted.")) {
				return;
			}
			ScreenGroups.Delete(_screenGroup);//Also deletes screens.
			DialogResult=DialogResult.OK;
		}

		private void butStartScreens_Click(object sender,EventArgs e) {
			if(_listScreenPats.Count==0) {
				MsgBox.Show(this,"No patients to screen.");
				return;
			}
			if(PrefC.GetBool(PrefName.ScreeningsUseSheets)) {
				StartScreensForPatsWithSheets();
			}
			else {
				StartScreensForPats();
			}
		}

		private void butSave_Click(object sender,System.EventArgs e) {
			if(textDescription.Text=="") {
				MsgBox.Show(this,"Description field cannot be blank.");
				textDescription.Focus();
				return;
			}
			_screenGroup.SGDate=PIn.Date(textScreenDate.Text);
			_screenGroup.Description=textDescription.Text;
			_screenGroup.ProvName=textProvName.Text;
			if (comboProv.SelectedIndex==-1) {
				_screenGroup.ProvNum=0;//ProvNum 0 is OK for screens.
			}
			else {
				_screenGroup.ProvNum=_listProviders[comboProv.SelectedIndex].ProvNum;
			}
			if(comboCounty.SelectedIndex==-1) {
				_screenGroup.County="";
			}
			else {
				_screenGroup.County=comboCounty.SelectedItem.ToString();
			}
			if(comboGradeSchool.SelectedIndex==-1) {
				_screenGroup.GradeSchool="";
			}
			else {
				_screenGroup.GradeSchool=comboGradeSchool.SelectedItem.ToString();
			}
			_screenGroup.PlaceService=(PlaceOfService)comboPlaceService.SelectedIndex;
			if(comboSheetDefs.SelectedIndex<1) {
				_screenGroup.SheetDefNum=0;
			}
			else {
				_screenGroup.SheetDefNum=_listSheetDefsScreening[comboSheetDefs.SelectedIndex-1].SheetDefNum;
			}
			ScreenPats.Sync(_listScreenPats,_listScreenPatsOld);
			ScreenGroups.Update(_screenGroup);
			DialogResult=DialogResult.OK;
		}

		private void FormScreenGroupEdit_FormClosing(object sender,FormClosingEventArgs e) {
			if(_screenGroup.IsNew && DialogResult==DialogResult.Cancel) {
				ScreenGroups.Delete(_screenGroup);//Also deletes screens.
			}
		}

	}
}