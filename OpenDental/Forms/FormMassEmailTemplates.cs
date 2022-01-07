using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormMassEmailTemplates:FormODBase {
		///<summary>Selected template. Viewable here only. User needs to make edits in a different form.</summary>
		private List<EmailHostingTemplate> _listEmailHostingTemplates;

		public FormMassEmailTemplates() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormMassEmailTemplates_Load(object sender,EventArgs e) {
			FillGrid();
			SelectAndLoadFirstTemplate();
			UpdateClinicIsSignedUp();
		}

		///<summary>Also refreshes data for the list of email hosting templates.</summary>
		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			_listEmailHostingTemplates=EmailHostingTemplates.Refresh().FindAll(x => x.TemplateType==PromotionType.Manual
				&& x.ClinicNum==comboClinic.SelectedClinicNum);
			GridColumn column=new GridColumn(Lans.g(gridMain.TranslationName,"Saved Templates"),240);
			gridMain.ListGridColumns.Add(column);
			column=new GridColumn(Lans.g(gridMain.TranslationName,"Email Type"),35);
			gridMain.ListGridColumns.Add(column);
			gridMain.ListGridRows.Clear();
			for(int i=0;i<_listEmailHostingTemplates.Count;i++){
				GridRow row=new GridRow();
				row.Cells.Add(_listEmailHostingTemplates[i].TemplateName);
				row.Cells.Add(Lans.g(this,_listEmailHostingTemplates[i].EmailTemplateType.GetDescription()));
				row.Tag=_listEmailHostingTemplates[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void SelectAndLoadFirstTemplate() {
			//usually called when the user cancels out of editing without saving changes for a new template.
			if(gridMain.GetSelectedIndex()==-1) {
				userControlEmailTemplate.RefreshView("","",EmailType.Html);
				return;
			}
			gridMain.SetSelected(0,true);
			EmailHostingTemplate selectedTemplate=gridMain.SelectedTag<EmailHostingTemplate>();
			userControlEmailTemplate.RefreshView(selectedTemplate.BodyPlainText,selectedTemplate.BodyHTML,selectedTemplate.EmailTemplateType);
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			EmailHostingTemplate selectedTemplate=gridMain.SelectedTag<EmailHostingTemplate>();
			userControlEmailTemplate.RefreshView(selectedTemplate.BodyPlainText,selectedTemplate.BodyHTML,selectedTemplate.EmailTemplateType);
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormMassEmailTemplate formMassEmailTemplate=new FormMassEmailTemplate(gridMain.SelectedTag<EmailHostingTemplate>());
			if(formMassEmailTemplate.ShowDialog()!=DialogResult.OK) {
				return;
			}
			FillGrid();
			SelectAndLoadFirstTemplate();
		}

		private void butNewTemplate_Click(object sender,EventArgs e) {
			EmailHostingTemplate emailHostingTemplate=new EmailHostingTemplate();
			emailHostingTemplate.IsNew=true;
			emailHostingTemplate.ClinicNum=comboClinic.SelectedClinicNum;
			using FormMassEmailTemplate formMassEmailTemplate=new FormMassEmailTemplate(emailHostingTemplate);
			if(formMassEmailTemplate.ShowDialog()!=DialogResult.OK) {
				SelectAndLoadFirstTemplate();
				return;
			}
			FillGrid();
			emailHostingTemplate=_listEmailHostingTemplates.FirstOrDefault(x => x.EmailHostingTemplateNum==formMassEmailTemplate.NewTemplateCurPriKey);
			if(gridMain.SelectedTag<EmailHostingTemplate>()!=null) {
				gridMain.SetSelected(_listEmailHostingTemplates.IndexOf(emailHostingTemplate),true);
				userControlEmailTemplate.RefreshView(emailHostingTemplate.BodyPlainText,emailHostingTemplate.BodyHTML,emailHostingTemplate.EmailTemplateType);
			}
			else {
				SelectAndLoadFirstTemplate();
			}
		}

		private void butImport_Click(object sender,EventArgs e) {
			#region Get Imported HTML 
			using OpenFileDialog openFileDialog=new OpenFileDialog();
			openFileDialog.Multiselect=false;
			if(Directory.Exists(PrefC.GetString(PrefName.ExportPath))) {
				openFileDialog.InitialDirectory=PrefC.GetString(PrefName.ExportPath);
			}
			else if(Directory.Exists("C:\\")) {
				openFileDialog.InitialDirectory="C:\\";
			}
			if(openFileDialog.ShowDialog()!=DialogResult.OK) {
				return;
			}
			string fileName=Path.GetFileName(openFileDialog.FileName);
			string path=openFileDialog.FileName;
			if(!File.Exists(path)) {
				MsgBox.Show(this,"File does not exist or cannot be read.");
				return;
			}
			if(Path.GetExtension(fileName)!=".html") {
				MsgBox.Show("Not a valid selection. Only HTML files can be imported.");
				return;
			}
			string documentText="";
			try {
				documentText=File.ReadAllText(path);
			}
			catch(Exception ex) {
				FriendlyException.Show(ex.Message,ex);
			}
			#endregion
			#region Show Email Edit window for user to make any desired adjustments to HTML
			//note, users could uncheck 'raw' and turn this into a regular html email in this window, so we need to do some extra checking.
			EmailHostingTemplate emailHostingTemplate=new EmailHostingTemplate();
			emailHostingTemplate.IsNew=true;
			emailHostingTemplate.ClinicNum=comboClinic.SelectedClinicNum;
			using FormEmailEdit formEmailEdit=new FormEmailEdit();
			formEmailEdit.IsMassEmail=true;
			formEmailEdit.MarkupText=documentText;
			formEmailEdit.IsRawAllowed=true;
			formEmailEdit.IsRaw=true;
			formEmailEdit.AreReplacementsAllowed=true;
			if(formEmailEdit.ShowDialog()!=DialogResult.OK) {
				SelectAndLoadFirstTemplate();
				return;
			}
			emailHostingTemplate.BodyHTML=formEmailEdit.MarkupText;
			if(string.IsNullOrEmpty(emailHostingTemplate.BodyHTML)) {//user decided against making an html email. We may even want to break out here. 
				emailHostingTemplate.EmailTemplateType=EmailType.Regular;
			}
			else if(formEmailEdit.IsRaw) {
				emailHostingTemplate.EmailTemplateType=EmailType.RawHtml;
			}
			else {//user decided they wanted to use the master template.
				emailHostingTemplate.EmailTemplateType=EmailType.Html;//Our special wiki replcement that will turn into html.
			}
			#endregion
			#region User sets plain text
			//ALL emails need to have plain text. Will will attempt to convert, but user needs to verify. 
			using FormMassEmailTemplate formMassEmailTemplate=new FormMassEmailTemplate(emailHostingTemplate);
			if(formMassEmailTemplate.ShowDialog()!=DialogResult.OK) {
				//user chose to not set plain text, this is no longer a valid template. 
				SelectAndLoadFirstTemplate();
				return;
			}
			FillGrid();
			emailHostingTemplate=_listEmailHostingTemplates.FirstOrDefault(x => x.EmailHostingTemplateNum==formMassEmailTemplate.NewTemplateCurPriKey);
			if(emailHostingTemplate!=null) {
				gridMain.SetSelected(_listEmailHostingTemplates.IndexOf(emailHostingTemplate),true);
				userControlEmailTemplate.RefreshView(emailHostingTemplate.BodyPlainText,emailHostingTemplate.BodyHTML,emailHostingTemplate.EmailTemplateType);
			}
			else {
				SelectAndLoadFirstTemplate();
			}
			#endregion
		}

		private void butEditTemplate_Click(object sender,EventArgs e) {
			EmailHostingTemplate selectedTemplate=gridMain.SelectedTag<EmailHostingTemplate>();
			if(selectedTemplate==null) {
				MsgBox.Show("Please select a template to edit, or add a new template.");
				return;
			}
			using FormMassEmailTemplate formMassEmailTemplate=new FormMassEmailTemplate(selectedTemplate);
			if(formMassEmailTemplate.ShowDialog()!=DialogResult.OK) {
				return;
			}
			FillGrid();
			userControlEmailTemplate.RefreshView(selectedTemplate.BodyPlainText,selectedTemplate.BodyHTML,selectedTemplate.EmailTemplateType);
		}

		private void butExamples_Click(object sender,EventArgs e) {
			if(comboClinic.SelectedClinicNum==-1) {
				MsgBox.Show("Please select a clinic.");
				return;
			}
			using FormMassEmailExamples formMassEmailExamples=new FormMassEmailExamples();
			formMassEmailExamples.ClinicNum=comboClinic.SelectedClinicNum;
			formMassEmailExamples.ShowDialog();
			if(formMassEmailExamples.DialogResult==DialogResult.OK) {
				FillGrid();
				MsgBox.Show("Template added successfully.");
			}
		}

		private void comboClinicPatient_SelectionChangeCommitted(object sender,EventArgs e) {
			userControlEmailTemplate.RefreshView("","",EmailType.Html);
			FillGrid();
			UpdateClinicIsSignedUp();
		}

		private void UpdateClinicIsSignedUp() {
			bool isAllowed=Security.IsAuthorized(Permissions.EServicesSetup,true);
			bool isClinicSignedUp=Clinics.IsMassEmailSignedUp(comboClinic.SelectedClinicNum);
			labelNotSignedUp.Visible=!isClinicSignedUp;
			butNewTemplate.Enabled=isAllowed && isClinicSignedUp;
			butImport.Enabled=isAllowed && isClinicSignedUp;
			butEditTemplate.Enabled=isAllowed && isClinicSignedUp;
			butCopy.Enabled=isAllowed && isClinicSignedUp;
			gridMain.Enabled=isAllowed && isClinicSignedUp;
			butExamples.Enabled=isAllowed && isClinicSignedUp;
		}

		private void butCopy_Click(object sender,EventArgs e) {
			EmailHostingTemplate selectedTemplate=gridMain.SelectedTag<EmailHostingTemplate>();
			if(selectedTemplate==null) {
				MsgBox.Show(this,"No valid template selected to copy from.");
				return;
			}
			EmailHostingTemplate copyTemplate=GenericTools.DeepCopy<EmailHostingTemplate,EmailHostingTemplate>(selectedTemplate);
			copyTemplate.TemplateName.Append('1');
			copyTemplate.IsNew=true;
			copyTemplate.ClinicNum=comboClinic.SelectedClinicNum;
			using FormMassEmailTemplate formMassEmailTemplate=new FormMassEmailTemplate(copyTemplate);
			if(formMassEmailTemplate.ShowDialog()!=DialogResult.OK) {
				SelectAndLoadFirstTemplate();
				return;
			}
			FillGrid();
			copyTemplate=_listEmailHostingTemplates.FirstOrDefault(x => x.EmailHostingTemplateNum==formMassEmailTemplate.NewTemplateCurPriKey);
			if(copyTemplate!=null) {
				gridMain.SetSelected(_listEmailHostingTemplates.IndexOf(copyTemplate),true);
				userControlEmailTemplate.RefreshView(copyTemplate.BodyPlainText,copyTemplate.BodyHTML,copyTemplate.EmailTemplateType);
			}
			else {
				SelectAndLoadFirstTemplate();
			}
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

	}
}