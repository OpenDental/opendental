using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEhrSummaryCcdEdit:FormODBase {
		public string StrXmlFilePath;
		public bool DidPrint;
		///<summary>Will be set to the patient that this CCD message is indicated for.  Can be null if not meant for incorporating into a patient's account.</summary>
		private Patient _patCur;

		///<summary>Patient can be null.  If null, or if PatNum is 0, reconciles will not be available.</summary>
		public FormEhrSummaryCcdEdit(string strXmlFilePath,Patient patCur) {
			InitializeComponent();
			InitializeLayoutManager();
			StrXmlFilePath=strXmlFilePath;
			_patCur=patCur;
		}

		private void FormEhrSummaryCcdEdit_Load(object sender,EventArgs e) {
			if(_patCur==null || _patCur.PatNum==0) {//No patient is currently selected.  Do not show reconcile UI.
				labelReconcile.Visible=false;
				butReconcileAllergies.Visible=false;
				butReconcileMedications.Visible=false;
				butReconcileProblems.Visible=false;
			}
			Cursor=Cursors.WaitCursor;
			webBrowser1.Url=new Uri(StrXmlFilePath);
			Cursor=Cursors.Default;
		}

		///<summary>Can only be called if IsReconcile is true.  This function is for EHR module b.4.</summary>
		private void butReconcileMedications_Click(object sender,EventArgs e) {
			XmlDocument xmlDocCcd=new XmlDocument();
			try {
				string strXmlText=File.ReadAllText(StrXmlFilePath);
				xmlDocCcd.LoadXml(strXmlText);				
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Error reading file")+": "+ex.Message);
				return;
			}
			using FormReconcileMedication formRM=new FormReconcileMedication(_patCur);
			formRM.ListMedicationPatNew=new List<MedicationPat>();
			EhrCCD.GetListMedicationPats(xmlDocCcd,formRM.ListMedicationPatNew);
			formRM.ShowDialog();
		}

		///<summary>Can only be called if IsReconcile is true.  This function is for EHR module b.4.</summary>
		private void butReconcileProblems_Click(object sender,EventArgs e) {
			XmlDocument xmlDocCcd=new XmlDocument();
			try {
				string strXmlText=File.ReadAllText(StrXmlFilePath);
				xmlDocCcd.LoadXml(strXmlText);
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Error reading file")+": "+ex.Message);
				return;
			}
			using FormReconcileProblem formRP=new FormReconcileProblem(_patCur);
			formRP.ListProblemNew=new List<Disease>();
			formRP.ListProblemDefNew=new List<DiseaseDef>();
			EhrCCD.GetListDiseases(xmlDocCcd,formRP.ListProblemNew,formRP.ListProblemDefNew);
			formRP.ShowDialog();
		}

		///<summary>Can only be called if IsReconcile is true.  This function is for EHR module b.4.</summary>
		private void butReconcileAllergies_Click(object sender,EventArgs e) {
			XmlDocument xmlDocCcd=new XmlDocument();
			try {
				string strXmlText=File.ReadAllText(StrXmlFilePath);
				xmlDocCcd.LoadXml(strXmlText);
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Error reading file")+": "+ex.Message);
				return;
			}
			using FormReconcileAllergy formRA=new FormReconcileAllergy(_patCur);
			formRA.ListAllergyNew=new List<Allergy>();
			formRA.ListAllergyDefNew=new List<AllergyDef>();
			EhrCCD.GetListAllergies(xmlDocCcd,formRA.ListAllergyNew,formRA.ListAllergyDefNew);
			formRA.ShowDialog();
		}

		private void butShowXml_Click(object sender,EventArgs e) {
			string strCcd=File.ReadAllText(StrXmlFilePath);
			//Reformat to add newlines after each element to make more readable.
			strCcd=strCcd.Replace("\r\n","").Replace("\n","").Replace("\r","");//Remove existsing newlines.
			strCcd=strCcd.Replace(">",">\r\n");
			using MsgBoxCopyPaste msgbox=new MsgBoxCopyPaste(strCcd);
			msgbox.ShowDialog();
		}

		private void butPrint_Click(object sender,EventArgs e) {
			//use the modeless version, which also allows user to choose printer
			webBrowser1.ShowPrintDialog();
			DidPrint = true;			
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}
