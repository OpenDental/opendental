using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormMassEmailExamples:FormODBase {
		///<summary>Selected clinic num. This is the clinic we will be adding the template to.</summary>
		public long ClinicNum;
		public FormMassEmailExamples() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormMassEmailExamples_Load(object sender,EventArgs e) {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Available Templates"),400);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Email Type"),120);
			gridMain.ListGridColumns.Add(col);
			List<EmailHostingTemplate> listEmailHostingTemplates=EmailHostingTemplates.GetExamples()
				.FindAll(x => x.TemplateType == PromotionType.Manual);//Matches what FormMassEmailTemplates shows in the main grid
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<listEmailHostingTemplates.Count;i++) {
				row=new GridRow();
				row.Cells.Add(listEmailHostingTemplates[i].TemplateName);
				row.Cells.Add(listEmailHostingTemplates[i].EmailTemplateType.GetDescription());
				row.Tag=listEmailHostingTemplates[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			EmailHostingTemplate emailHostingTemplateSelected=gridMain.SelectedTag<EmailHostingTemplate>();
			if(emailHostingTemplateSelected==null) {
				MsgBox.Show("Template not selected.");
				return;
			}
			emailHostingTemplateSelected.ClinicNum=ClinicNum;
			emailHostingTemplateSelected.IsNew=true;
			using FormMassEmailTemplate formMassEmailTemplate=new FormMassEmailTemplate(emailHostingTemplateSelected);
			formMassEmailTemplate.ShowDialog();
			if(formMassEmailTemplate.DialogResult==DialogResult.OK) {
				DialogResult=DialogResult.OK;
			}
		}

		/// <summary>Inserts copy of template into db with the passed in clinic num. Returns false if the api throws</summary>
		private bool TryAddTemplate() {
			EmailHostingTemplate emailHostingTemplateSelected=gridMain.SelectedTag<EmailHostingTemplate>();
			if(emailHostingTemplateSelected==null) {
				MsgBox.Show("Please select a template to add.");
				return false;
			}
			emailHostingTemplateSelected.ClinicNum=ClinicNum;
			IAccountApi api=EmailHostingTemplates.GetAccountApi(ClinicNum);
			try {
				CreateTemplateResponse response=api.CreateTemplate(new CreateTemplateRequest { 
					Template=new Template { 
						TemplateName=emailHostingTemplateSelected.TemplateName,
						TemplateBodyHtml=emailHostingTemplateSelected.BodyHTML,
						TemplateBodyPlainText=emailHostingTemplateSelected.BodyPlainText,
						TemplateSubject=emailHostingTemplateSelected.Subject,
					},
				});
				emailHostingTemplateSelected.TemplateId=response.TemplateNum;
			}
			catch(Exception ex) {
				FriendlyException.Show("Failed to create template. Please try again.",ex);
				return false;
			}
			EmailHostingTemplates.Insert(emailHostingTemplateSelected);
			return true;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(TryAddTemplate()) { 
				DialogResult=DialogResult.OK;
			}
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}