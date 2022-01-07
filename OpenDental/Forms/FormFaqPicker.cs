using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormFaqPicker:FormODBase {
		private FormFaqEdit _formFaqEdit;
		private string _manualPageUrl;
		private string _programVersion;
		private List<ManualPage> _listUnlinkedFaqs=new List<ManualPage>();

		public FormFaqPicker(string manualUrl="", string programVersion="") {
			_manualPageUrl=manualUrl;
			_programVersion=programVersion;
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormFaqPicker_Load(object sender,EventArgs e) {
			dataGridMain.AutoGenerateColumns=false;
			textManualPage.Text=_manualPageUrl;
			textManualVersion.Text=_programVersion;
			checkShowUnlinked.Checked=false;
			FillGrid();
		}

		///<summary>Fills the grid with Faq objects for the given manual page and version.
		///Should almost always call RefreshGrid() instead of this method directly.</summary>
		private void FillGrid() {
			if(!int.TryParse(textManualVersion.Text,out int version) && version!=0) {
				MessageBox.Show(this,"Please enter a valid manual version. For example, 19.1 should be entered as '191'.");
				return;
			}
			List<Faq> listFaqForPageName=new List<Faq>();
			try {
				listFaqForPageName=GetListFaqs(PIn.String(textManualPage.Text),version);
			}
			catch(Exception ex) {
				MsgBox.Show(ex.Message);
				return;
			}
			dataGridMain.Columns.Clear();
			dataGridMain.Columns[dataGridMain.Columns.Add("QuestionText","Question Text")].AutoSizeMode=DataGridViewAutoSizeColumnMode.Fill;
			dataGridMain.Columns.Add("Version","Version");
			dataGridMain.Columns.Add("IsStickied","Is Stickied");
			if(checkShowUnlinked.Checked) {
				dataGridMain.Columns.Add("LinkedStatus","Linked Status");
			}
			DataGridViewRow row;
			foreach(Faq faq in listFaqForPageName) {
				int rowId=dataGridMain.Rows.Add();
				row=dataGridMain.Rows[rowId];
				row.Cells["QuestionText"].Value=faq.QuestionText;
				row.Cells["Version"].Value=faq.ManualVersion.ToString();
				row.Cells["IsStickied"].Value=faq.IsStickied;
				if(!checkShowUnlinked.Checked) {
					row.Tag=faq;
					continue;
				}
				//below is for unlinked
				ManualPage unlinkedItem=_listUnlinkedFaqs.FirstOrDefault(x=>x.FaqNum==faq.FaqNum);
				if(unlinkedItem is null) {//Null because the FaqNum is linked to a valid manual page
					row.Cells["LinkedStatus"].Value="Linked";
				}
				else if(unlinkedItem.ManualPageNum==0) {//FaqNum not linked to manual page 
					row.Cells["LinkedStatus"].Value="Unlinked";
				}
				else {//FaqNum is linked to a manual page that no longer exists in jordans_mp database
					row.Cells["LinkedStatus"].Value="Manual page does not exist";
				}
				row.Tag=faq;
			}
		}

		///<summary> If checkShowUnlinked is checked, this grabs all FAQs from the database and creates a list of FaqNums with invalid ManualPageNums.
		/// Otherwise, uses the values of the filter and version text boxes to grab FAQs from the database as needed. 
		/// Assumes that the version has already been validated before getting passed in
		///</summary>
		private List<Faq> GetListFaqs(string text,int version) { 
			List<Faq> retVal=new List<Faq>();
			if(checkShowUnlinked.Checked) {
				retVal=Faqs.GetAll();
				_listUnlinkedFaqs=Faqs.GetListUnlinkedFaq();
			}
			else {
				retVal=Faqs.GetAllForNameAndVersion(text,version);
			}
			return retVal;
		}

		/// <summary>Helper method to initialize _formFaqEdit with a FormClosedEventHandler and show FormFaqEdit.</summary>
		private void ShowFormFaqEdit(Faq faq=null) {
			if(_formFaqEdit==null || _formFaqEdit.IsDisposed) {
				_formFaqEdit=new FormFaqEdit(faq);
				_formFaqEdit.FormClosed+=(sender,e) => FillGrid();
			}
			else {
				MsgBox.Show(this,"Another FAQ is currently being edited.");
			}
			_formFaqEdit.Show();
			if(_formFaqEdit.WindowState==FormWindowState.Minimized) {
				_formFaqEdit.WindowState=FormWindowState.Normal;
			}
			_formFaqEdit.BringToFront();
		}

		private void ButRefresh_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void DataGridMain_CellDoubleClick(object sender,DataGridViewCellEventArgs e) {
			if(e.RowIndex<0) {
				return;//we don't need to edit anything if we're double clicking the header
			}
			Faq faq=null;
			if(dataGridMain.CurrentRow!=null) {
				faq=(Faq)dataGridMain.CurrentRow.Tag;
			}
			ShowFormFaqEdit(faq);
		}

		private void ButAdd_Click(object sender,EventArgs e) {
			ShowFormFaqEdit();
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
			Close();
		}
	}
}
