using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using CodeBase;

namespace OpenDental {
	public partial class FormMedLabHL7MsgText:FormODBase {
		public List<string[]> ListFileNamesDatesMod; 

 		public FormMedLabHL7MsgText() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormMedLabHL7MsgText_Load(object sender,EventArgs e) {
			for(int i=0;i<ListFileNamesDatesMod.Count;i++) {
				string dateAndName="";
				if(ListFileNamesDatesMod[i][1]!="") {
					dateAndName+=ListFileNamesDatesMod[i][1];
				}
				dateAndName+="  -  "+ListFileNamesDatesMod[i][0];
				listFileNames.Items.Add(dateAndName);
			}
			listFileNames.SelectedIndex=0;
		}

		private void listFileNames_SelectedIndexChanged(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			textMain.Clear();
			string msgText="";
			try {
				msgText=File.ReadAllText(ListFileNamesDatesMod[listFileNames.SelectedIndex][0]);
			}
			catch(Exception ex) {
				ex.DoNothing();
				Cursor=Cursors.Default;
				MsgBox.Show(this,"The selected file could not be read.");
				return;
			}
			textMain.Text=msgText;
			Cursor=Cursors.Default;
			return;
		}

		//We may want to implement this as a way to revert any changes made manually or to re-create the embedded PDFs from the message text.
		//private void butReprocessMsgs_Click(object sender,EventArgs e) {
		//	///<summary>Used to revert any changes made to the MedLab object 
		//	///OR create embedded files attached to the currently selected patient.
		//	///This will re-process the original HL7 message(s) from the archived file(s).
		//	///Since there may be more than one HL7 message that comprises the information on the form, all messages for this specimen will be re-processed.
		//	///The old MedLab object(s) and any MedLabResult, MedLabSpecimen, or MedLabFacAttach objects linked will be deleted and the form will fill with
		//	///the new object based on the original file data.  Right now users are only allowed to change the patient and provider, but more changes may be
		//	///possible in the future that will be reverted.</summary>
		//	//Key=file path to message file relative to the preferred AtoZ folder, value=HL7 message content
		//	Dictionary<string,string> dictFileNameFileText=new Dictionary<string,string>();
		//	for(int i=0;i<ListMedLabs.Count;i++) {
		//		string filePath=ODFileUtils.CombinePaths(ImageStore.GetPreferredAtoZpath(),ListMedLabs[i].FileName);
		//		try { //File IO, surround with try catch
		//			if(!dictFileNameFileText.ContainsKey(ListMedLabs[i].FileName)) {
		//				string fileTextCur=File.ReadAllText(filePath);
		//				dictFileNameFileText.Add(ListMedLabs[i].FileName,fileTextCur);
		//			}
		//		}
		//		catch(Exception ex) {
		//			throw new Exception(Lan.g(this,"Could not read the MedLab HL7 message text file located at")+" "+ListMedLabs[i].FileName+".");
		//		}
		//	}
		//	List<long> listMedLabNumsNew=new List<long>();
		//	foreach(KeyValuePair<string,string> fileCur in dictFileNameFileText) {
		//		MessageHL7 msg=new MessageHL7(fileCur.Value);
		//		List<long> listMedLabNumsCur=MessageParserMedLab.Process(msg,fileCur.Key,false,PatCur);//re-creates the documents from the ZEF segments
		//		if(listMedLabNumsCur==null || listMedLabNumsCur.Count<1) {
		//			throw new Exception(Lan.g(this,"The HL7 message processed did not produce any MedLab objects."));
		//		}
		//		MedLabs.UpdateFileNames(listMedLabNumsCur,fileCur.Key);
		//		listMedLabNumsNew.AddRange(listMedLabNumsCur);
		//	}
		//	//Delete all records except the ones just created
		//	DeleteLabsAndResults(_medLabCur,listMedLabNumsNew);
		//	ListMedLabs=MedLabs.GetForPatAndSpecimen(PatCur.PatNum,_medLabCur.SpecimenID,_medLabCur.SpecimenIDFiller);//handles PatNum=0
		//	_medLabCur=ListMedLabs[0];
		//	SetFields();
		//}

		private void butClose_Click(object sender,EventArgs e) {
			this.Close();
		}
		
	}
}
