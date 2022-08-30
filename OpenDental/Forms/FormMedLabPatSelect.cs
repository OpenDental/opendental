using System;
using System.Windows.Forms;
using System.Collections.Generic;
using OpenDentBusiness;
using OpenDentBusiness.HL7;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormMedLabPatSelect:FormODBase {
		public List<MedLab> ListMedLabs;
		public Patient PatientCur;

		public FormMedLabPatSelect() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormMedLabPatSelect_Load(object sender,EventArgs e) {
			if(PatientCur!=null) {
				textName.Text=PatientCur.GetNameFL();
			}
			FillGridPatInfo();
			FillGridLabs();
		}

		///<summary>Fills patient information from message contents, not from PatCur.</summary>
		private void FillGridPatInfo() {
			gridPidInfo.BeginUpdate();
			gridPidInfo.Columns.Clear();
			GridColumn col;
			col=new GridColumn("Patient Name",335);
			gridPidInfo.Columns.Add(col);
			col=new GridColumn("Birthdate",150);
			gridPidInfo.Columns.Add(col);
			col=new GridColumn("Gender",150);
			gridPidInfo.Columns.Add(col);
			col=new GridColumn("SSN",150);
			gridPidInfo.Columns.Add(col);
			gridPidInfo.ListGridRows.Clear();
			List<string[]> listPats=GetPatInfoFromPidSegments();
			GridRow row;
			for(int i=0;i<listPats.Count;i++) {
				if(listPats[i].Length<4) {//should never happen
					continue;
				}
				row=new GridRow();
				row.Cells.Add(listPats[i][0]);//patName
				row.Cells.Add(listPats[i][1]);//Birthdate
				row.Cells.Add(listPats[i][2]);//Gender
				row.Cells.Add(listPats[i][3]);//SSN
				gridPidInfo.ListGridRows.Add(row);
			}
			gridPidInfo.EndUpdate();
		}

		///<summary>Gets the patient info from the MedLab.OriginalPIDSegments.  Returns null if there is an error processing the PID segments.</summary>
		private List<string[]> GetPatInfoFromPidSegments() {
			List<string[]> listPats=new List<string[]>();
			HL7Def hL7Def=HL7Defs.GetOneDeepEnabled(true);
			if(hL7Def==null) {
				MsgBox.Show(this,"There must be an enabled MedLab HL7 interface in order to parse the message details.");
				return null;
			}
			HL7DefMessage hL7DefMessage=null;
			for(int i=0;i<hL7Def.hl7DefMessages.Count;i++) {
				//for now there are only incoming ORU messages supported, so there should only be one defined message type and it should be inbound
				if(hL7Def.hl7DefMessages[i].MessageType==MessageTypeHL7.ORU && hL7Def.hl7DefMessages[i].InOrOut==InOutHL7.Incoming) {
					hL7DefMessage=hL7Def.hl7DefMessages[i];
					break;
				}
			}
			if(hL7DefMessage==null) {
				MsgBox.Show(this,"There must be a message definition for an inbound ORU message in order to parse this message.");
				return null;
			}
			//for MedLab interfaces, we limit the ability to rearrange the message structure, so the PID segment is always in position 1
			if(hL7DefMessage.ListHL7DefSegments.Count<2) {
				MsgBox.Show(this,"The message definition for an inbound ORU message does not have the correct number of segments.");
				return null;
			}
			HL7DefSegment hL7DefSegmentPid=hL7DefMessage.ListHL7DefSegments[1];
			if(hL7DefSegmentPid.SegmentName!=SegmentNameHL7.PID) {
				MsgBox.Show(this,"The message definition for an inbound ORU message does not have the PID segment as the second segment.");
				return null;
			}
			for(int i=0;i<ListMedLabs.Count;i++) {
				string[] fields=ListMedLabs[i].OriginalPIDSegment.Split(new string[] { "|" },StringSplitOptions.None);
				List<FieldHL7> listFieldHL7s=new List<FieldHL7>();
				for(int j=0;j<fields.Length;j++) {
					listFieldHL7s.Add(new FieldHL7(fields[j]));
				}
				string patName="";
				string birthdate="";
				string gender="";
				string ssn="";
				for(int j=0;j<hL7DefSegmentPid.hl7DefFields.Count;j++) {
					int itemOrder=hL7DefSegmentPid.hl7DefFields[j].OrdinalPos;
					if(itemOrder>listFieldHL7s.Count-1) {
						continue;
					}
					switch(hL7DefSegmentPid.hl7DefFields[j].FieldName) {
						case "pat.nameLFM":
							patName=listFieldHL7s[itemOrder].GetComponentVal(1);
							if(patName!="" && listFieldHL7s[itemOrder].GetComponentVal(2)!="") {
								patName+=" ";
							}
							patName+=listFieldHL7s[itemOrder].GetComponentVal(2);
							if(patName!="" && listFieldHL7s[itemOrder].GetComponentVal(0)!="") {
								patName+=" ";
							}
							patName+=listFieldHL7s[itemOrder].GetComponentVal(0);
							continue;
						case "patBirthdateAge":
							//LabCorp sends the birthdate and age in years, months, and days like yyyyMMdd^YYY^MM^DD
							birthdate=FieldParser.DateTimeParse(listFieldHL7s[itemOrder].GetComponentVal(0)).ToShortDateString();
							continue;
						case "pat.Gender":
							gender=Lan.g("enumPatientGender",FieldParser.GenderParse(listFieldHL7s[itemOrder].GetComponentVal(0)).ToString());
							continue;
						case "pat.SSN":
							if(listFieldHL7s[itemOrder].GetComponentVal(0).Length>3) {
								ssn="***-**-";
								ssn+=listFieldHL7s[itemOrder].GetComponentVal(0).Substring(listFieldHL7s[itemOrder].GetComponentVal(0).Length-4,4);
							}
							continue;
						default:
							continue;
					}
				}
				bool isDuplicate=false;
				for(int j=0;j<listPats.Count;j++) {
					if(listPats[j].Length<4) {//should never happen
						continue;
					}
					if(listPats[j][0]==patName &&  listPats[j][1]==birthdate &&  listPats[j][2]==gender &&  listPats[j][3]==ssn) {
						isDuplicate=true;
					}
				}
				if(!isDuplicate) {
					listPats.Add(new string[] { patName,birthdate,gender,ssn });
				}
			}
			return listPats;
		}

		private void FillGridLabs() {
			gridLabs.BeginUpdate();
			gridLabs.Columns.Clear();
			GridColumn col;
			col=new GridColumn("Test Description",175);
			gridLabs.Columns.Add(col);
			col=new GridColumn("Provider",100);
			gridLabs.Columns.Add(col);
			col=new GridColumn("Placer Specimen ID",120);//should be the ID sent on the specimen container to lab
			gridLabs.Columns.Add(col);
			col=new GridColumn("Filler Specimen ID",120);//lab assigned specimen ID
			gridLabs.Columns.Add(col);
			col=new GridColumn("Date & Time Entered",135);
			col.SortingStrategy=GridSortingStrategy.DateParse;
			gridLabs.Columns.Add(col);
			col=new GridColumn("Date & Time Reported",135);
			col.SortingStrategy=GridSortingStrategy.DateParse;
			gridLabs.Columns.Add(col);
			gridLabs.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<ListMedLabs.Count;i++) {
				row=new GridRow();
				row.Cells.Add(ListMedLabs[i].ObsTestDescript);
				row.Cells.Add(Providers.GetAbbr(ListMedLabs[i].ProvNum));//if ProvNum=0 this will return an empty string
				row.Cells.Add(ListMedLabs[i].SpecimenID);
				row.Cells.Add(ListMedLabs[i].SpecimenIDFiller);
				row.Cells.Add(ListMedLabs[i].DateTimeEntered.ToString("MM/dd/yyyy hh:mm tt"));//DT format matches LabCorp examples (US only company)
				row.Cells.Add(ListMedLabs[i].DateTimeReported.ToString("MM/dd/yyyy hh:mm tt"));//DT format matches LabCorp examples (US only company)
				gridLabs.ListGridRows.Add(row);
			}
			gridLabs.EndUpdate();
		}

		private void butPatSelect_Click(object sender,EventArgs e) {
			using FormPatientSelect formPatientSelect=new FormPatientSelect();
			formPatientSelect.ShowDialog();
			if(formPatientSelect.DialogResult!=DialogResult.OK) {
				return;
			}
			if(PatientCur!=null && PatientCur.PatNum==formPatientSelect.PatNumSelected) {
				return;
			}
			PatientCur=Patients.GetPat(formPatientSelect.PatNumSelected);
			textName.Text=PatientCur.GetNameFL();
			labelExistingLab.Visible=true;
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(PatientCur==null) {
				MsgBox.Show(this,"Please attach to patient first.");
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}
