using System;
using System.Windows.Forms;
using System.Collections.Generic;
using OpenDentBusiness;
using OpenDentBusiness.HL7;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormMedLabPatSelect:FormODBase {
		public List<MedLab> ListMedLabs;
		public Patient PatCur;

		public FormMedLabPatSelect() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormMedLabPatSelect_Load(object sender,EventArgs e) {
			if(PatCur!=null) {
				textName.Text=PatCur.GetNameFL();
			}
			FillGridPatInfo();
			FillGridLabs();
		}

		///<summary>Fills patient information from message contents, not from PatCur.</summary>
		private void FillGridPatInfo() {
			gridPidInfo.BeginUpdate();
			gridPidInfo.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn("Patient Name",335);
			gridPidInfo.ListGridColumns.Add(col);
			col=new GridColumn("Birthdate",150);
			gridPidInfo.ListGridColumns.Add(col);
			col=new GridColumn("Gender",150);
			gridPidInfo.ListGridColumns.Add(col);
			col=new GridColumn("SSN",150);
			gridPidInfo.ListGridColumns.Add(col);
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
			HL7Def hl7DefCur=HL7Defs.GetOneDeepEnabled(true);
			if(hl7DefCur==null) {
				MsgBox.Show(this,"There must be an enabled MedLab HL7 interface in order to parse the message details.");
				return null;
			}
			HL7DefMessage hl7defmsg=null;
			for(int i=0;i<hl7DefCur.hl7DefMessages.Count;i++) {
				//for now there are only incoming ORU messages supported, so there should only be one defined message type and it should be inbound
				if(hl7DefCur.hl7DefMessages[i].MessageType==MessageTypeHL7.ORU && hl7DefCur.hl7DefMessages[i].InOrOut==InOutHL7.Incoming) {
					hl7defmsg=hl7DefCur.hl7DefMessages[i];
					break;
				}
			}
			if(hl7defmsg==null) {
				MsgBox.Show(this,"There must be a message definition for an inbound ORU message in order to parse this message.");
				return null;
			}
			//for MedLab interfaces, we limit the ability to rearrange the message structure, so the PID segment is always in position 1
			if(hl7defmsg.hl7DefSegments.Count<2) {
				MsgBox.Show(this,"The message definition for an inbound ORU message does not have the correct number of segments.");
				return null;
			}
			HL7DefSegment pidSegDef=hl7defmsg.hl7DefSegments[1];
			if(pidSegDef.SegmentName!=SegmentNameHL7.PID) {
				MsgBox.Show(this,"The message definition for an inbound ORU message does not have the PID segment as the second segment.");
				return null;
			}
			for(int i=0;i<ListMedLabs.Count;i++) {
				string[] fields=ListMedLabs[i].OriginalPIDSegment.Split(new string[] { "|" },StringSplitOptions.None);
				List<FieldHL7> listFields=new List<FieldHL7>();
				for(int j=0;j<fields.Length;j++) {
					listFields.Add(new FieldHL7(fields[j]));
				}
				string patName="";
				string birthdate="";
				string gender="";
				string ssn="";
				for(int j=0;j<pidSegDef.hl7DefFields.Count;j++) {
					int itemOrder=pidSegDef.hl7DefFields[j].OrdinalPos;
					if(itemOrder>listFields.Count-1) {
						continue;
					}
					switch(pidSegDef.hl7DefFields[j].FieldName) {
						case "pat.nameLFM":
							patName=listFields[itemOrder].GetComponentVal(1);
							if(patName!="" && listFields[itemOrder].GetComponentVal(2)!="") {
								patName+=" ";
							}
							patName+=listFields[itemOrder].GetComponentVal(2);
							if(patName!="" && listFields[itemOrder].GetComponentVal(0)!="") {
								patName+=" ";
							}
							patName+=listFields[itemOrder].GetComponentVal(0);
							continue;
						case "patBirthdateAge":
							//LabCorp sends the birthdate and age in years, months, and days like yyyyMMdd^YYY^MM^DD
							birthdate=FieldParser.DateTimeParse(listFields[itemOrder].GetComponentVal(0)).ToShortDateString();
							continue;
						case "pat.Gender":
							gender=Lan.g("enumPatientGender",FieldParser.GenderParse(listFields[itemOrder].GetComponentVal(0)).ToString());
							continue;
						case "pat.SSN":
							if(listFields[itemOrder].GetComponentVal(0).Length>3) {
								ssn="***-**-";
								ssn+=listFields[itemOrder].GetComponentVal(0).Substring(listFields[itemOrder].GetComponentVal(0).Length-4,4);
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
			gridLabs.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn("Test Description",175);
			gridLabs.ListGridColumns.Add(col);
			col=new GridColumn("Provider",100);
			gridLabs.ListGridColumns.Add(col);
			col=new GridColumn("Placer Specimen ID",120);//should be the ID sent on the specimen container to lab
			gridLabs.ListGridColumns.Add(col);
			col=new GridColumn("Filler Specimen ID",120);//lab assigned specimen ID
			gridLabs.ListGridColumns.Add(col);
			col=new GridColumn("Date & Time Entered",135);
			col.SortingStrategy=GridSortingStrategy.DateParse;
			gridLabs.ListGridColumns.Add(col);
			col=new GridColumn("Date & Time Reported",135);
			col.SortingStrategy=GridSortingStrategy.DateParse;
			gridLabs.ListGridColumns.Add(col);
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
			using FormPatientSelect FormPS=new FormPatientSelect();
			FormPS.ShowDialog();
			if(FormPS.DialogResult!=DialogResult.OK) {
				return;
			}
			if(PatCur!=null && PatCur.PatNum==FormPS.SelectedPatNum) {
				return;
			}
			PatCur=Patients.GetPat(FormPS.SelectedPatNum);
			textName.Text=PatCur.GetNameFL();
			labelExistingLab.Visible=true;
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(PatCur==null) {
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
