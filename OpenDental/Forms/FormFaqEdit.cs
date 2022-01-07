using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormFaqEdit:FormODBase {
		///<summary>Represents either a new faq object that will be inserted into the database or an existing faq object that needs to be updated.</summary>
		private readonly Faq _faqCur;
		///<summary>Holds a list of all unique manualpage names.</summary>
		private List<string> _listManualPagesAll;
		///<summary>Only set when creating a new FAQ object. This list holds versions selected by the user.</summary>
		private List<int> _listVersions;
		///<summary>Set to try when called from the quick add button in FormHelpBrowser</summary>
		public bool IsQuickAdd=false;
		public string ManualPage="";
		public string Version="";

		///<summary>Pass in an existing faq object to edit it.</summary>
		public FormFaqEdit(Faq faq=null) {
			InitializeComponent();
			InitializeLayoutManager();
			_faqCur=faq??new Faq() { IsNew=true };
		}

		private void FormFaqEdit_Load(object sender,EventArgs e) {
			//Jordan's manual page table has an entry for each page and version. 
			_listManualPagesAll=Faqs.GetAllManualPageNames();
			FillFaqInfo();
			if(_faqCur.IsNew) {
				butDelete.Visible=false;
				//New FAQs can really be more than one FAQ.  For this reason, we must only allow the user to pick versions for new FAQs.
				//If the FAQ is not new, it has one specific version and cannot be changed (must delete)
				butVersionPick.Visible=true;
			}
			FillGridManualPages(_listManualPagesAll);
		}

		private void FormFaqEdit_Shown(object sender,EventArgs e) {
			FillPrieview();
		}

		///<summary>If editing an existing FAQ this method will fill the UI with its values.</summary>
		private void FillFaqInfo() {
			if(_faqCur.IsNew && !IsQuickAdd) {//Creating a new faq, no data to load
				return;
			}
			if(IsQuickAdd) {//We're going to have a new FAQ we're creating but we do have a new FAQ object that won't have a bunch of data set yet.
				textManualVersion.Text=Version;
				listBoxLinkedPages.Items.AddRange(_listManualPagesAll.Where(x=>x==ManualPage).ToArray());
				textEmbeddedMediaURL.Visible=false;
				textImagePath.Visible=false;
				labelImagePath.Visible=false;
				labelEmbeddedMediaUrl.Visible=false;
			}
			else {
				textQuestion.Text=_faqCur.QuestionText;
				textAnswer.Text=_faqCur.AnswerText;
				textImagePath.Text=_faqCur.ImageUrl;
				textEmbeddedMediaURL.Text=_faqCur.EmbeddedMediaUrl;
				textManualVersion.Text=_faqCur.ManualVersion.ToString();
				checkSticky.Checked=_faqCur.IsStickied;
				//Fill listbox with the manual pages the Faq object is currently linked to
				List<string> listLinkedPages=Faqs.GetLinkedManualPages(_faqCur.FaqNum);
				listBoxLinkedPages.Items.AddRange(listLinkedPages.ToArray());
			}
			FillGridManualPages(_listManualPagesAll);
		}

		private void FillPrieview() {
			string question=RemoveScriptTags(textQuestion.Text);
			string answer=RemoveScriptTags(textAnswer.Text);
			string image=RemoveScriptTags(textImagePath.Text);
			string embeded=RemoveScriptTags(textEmbeddedMediaURL.Text);
			//This css is based on index.css found in the ODHelpFaqSite project.
			StringBuilder cssBuilder=new StringBuilder("<style>" +
			@".panel-default {
				margin: 5px 0;
			}" +
			@".panel-default > .panel-heading {
				color: white;
				background-color: #41616A;
			}" +
			@".panel-default > .panel-heading > a {
				font-size: 1.3rem;
				color: #fff;
				text-decoration: none;
				font-weight: normal;
				display: block;
				padding: 0 5px;
			}" +
			@".panel-collapse {
				padding: 0 10px 10px;
				border: 10px solid #41616A;
				border-top: none;
			}" +
			"</style>");
			//This html is based on index.cshtml found in the ODHelpFaqSite project.
			StringBuilder htmlBuilder=new StringBuilder("<body><div class=\"panel panel-default\">");
			htmlBuilder.Append("<div class=\"panel-heading\">"+" &nbsp;&nbsp;&nbsp;&nbsp;- "+question+"</div>");
			htmlBuilder.Append("<div class=\"panel-collapse collapse\">");
			if(answer!="") {
				htmlBuilder.Append("<div style=\"white-space: pre-line\" class=\"panel-body\">"+answer);
			}
			if(image!="" || embeded!="") {
				htmlBuilder.Append("<div class=\"container-fluid\">");
				if(textImagePath.Text!="") {
					htmlBuilder.Append("<div class=\"row\"><div style=\"margin-top: 5px\"><a style=\"block;\">");
					htmlBuilder.Append("<img src=\""+image+"\" style=\"max-width:100%;max-height:100%;\"><br>");
					htmlBuilder.Append("</a></div></div>");
				}
				if(embeded!="") {
					htmlBuilder.Append("<div class=\"row\">");
					htmlBuilder.Append("<iframe style=\"margin-top: 5px;height: 400px;width: 100%;\" src=\""+embeded+"\"></iframe>");
					htmlBuilder.Append("</div>");
				}
				htmlBuilder.Append("</div>");
			}
			if(answer!="") {
				htmlBuilder.Append("</div>");
			}
			htmlBuilder.Append("</div></div></body>");
			string document=$"<!DOCTYPE html><html><head>{cssBuilder}</head>{htmlBuilder}</html>";
			webPreview.DocumentText=document;
		}

		string RemoveScriptTags(string toParse) {
			return toParse.Replace("<script>","").Replace("</script>","");
		}

		///<summary>Helper method that ensures the required UI fields have valid data.</summary>
		private bool IsValid() {
			StringBuilder strbErrorMsg=new StringBuilder();
			//Check that a question and answer were specified. Must have both
			if(String.IsNullOrEmpty(textQuestion.Text)||String.IsNullOrEmpty(textAnswer.Text)) {
				strbErrorMsg.AppendLine("Cannot create an FAQ without a question or answer.");
			}
			//Must have a version
			if(String.IsNullOrEmpty(textManualVersion.Text)) {
				strbErrorMsg.AppendLine("Must specify a manual version.");
			}
			//Must be linked to at least 1 manual page
			if(listBoxLinkedPages.Items.Count<1) {
				strbErrorMsg.AppendLine("The FAQ must be linked to at least one manual page.");
			}
			if(!String.IsNullOrEmpty(strbErrorMsg.ToString())) {//Errors found, inform user
				MessageBox.Show(this,"Missing/Incorrect Data:\r\n"+strbErrorMsg.ToString());
				return false;
			}
			return true;
		}

		///<summary>Helper method that inserts the specified html tag at the current cursor position. 
		///If text is selected it will be wrapped in the specified tag.</summary>
		private void HtmlTextHelper(string tagStart,string tagEnd,Func<string,string> innerTextAction=null,bool doInsertWhenNoSelection=true) {
			if(textAnswer.SelectedText.Length>0) {//user has text highlighted
				string selectedText=textAnswer.Text.Substring(textAnswer.SelectionStart,textAnswer.SelectionLength);
				selectedText=(innerTextAction?.Invoke(selectedText)??selectedText);
				string newText=tagStart+selectedText+tagEnd;
				textAnswer.Text=textAnswer.Text.Substring(0,textAnswer.SelectionStart)+newText+textAnswer.Text.Substring(textAnswer.SelectionStart+textAnswer.SelectionLength);
				//Place cursor at the end of the current text.
				textAnswer.SelectionStart=textAnswer.Text.Length;
			}
			else if(doInsertWhenNoSelection) {//No text selected, insert tags at cursor position
				textAnswer.Text=textAnswer.Text.Insert(textAnswer.SelectionStart,tagStart+tagEnd);
				//Be a bro and put the cursor between the html tags after inserting them
				textAnswer.SelectionStart=textAnswer.Text.Length-tagEnd.Length;
			}
			textAnswer.SelectionLength=0;
			textAnswer.Select();
		}

		///<summary>A method that runs the TextLinkPage_KeyUp logic. This helper allows the grid to be refreshed whilst maintaining the user's filter.</summary>
		private void FilterHelper() {
			string textEntered=textLinkPage.Text;
			List<string> listResults=new List<string>();
			foreach(string manualPage in _listManualPagesAll) {
				if(manualPage.Contains(textEntered)) {
					listResults.Add(manualPage);
				}
			}
			FillGridManualPages(listResults);
		}

		///<summary>Fills the manual page grid with the given list. This method should be called after listBoxLinkedPages has been filled.</summary>
		private void FillGridManualPages(List<string> listPages) {
			listAvailableManualPages.Items.Clear();
			listAvailableManualPages.Items.AddRange(listPages.ToArray());
		}

		private void ButtonBold_Click(object sender,EventArgs e) {
			HtmlTextHelper("<b>","</b>");
		}

		private void ButtonItalics_Click(object sender,EventArgs e) {
			HtmlTextHelper("<i>","</i>");
		}

		private void buttonUnderline_Click(object sender,EventArgs e) {
			HtmlTextHelper("<u>","</u>");
		}

		private void buttonHyperlink_Click(object sender,EventArgs e) {
			HtmlTextHelper("<a target=\"_blank\" href=\"Enter URL here\">","</a>");
		}

		///<summary>Filters the list of manual pages down to those that start with the entered text.</summary>
		private void TextLinkPage_KeyUp(object sender,KeyEventArgs e) {
			FilterHelper();
		}

		private void textQuestion_TextChanged(object sender,EventArgs e) {
			FillPrieview();
		}

		private void textAnswer_TextChanged(object sender,EventArgs e) {
			FillPrieview();
		}

		private void textImagePath_TextChanged(object sender,EventArgs e) {
			FillPrieview();
		}

		private void textEmbeddedMediaURL_TextChanged(object sender,EventArgs e) {
			FillPrieview();
		}

		private void ButRight_Click(object sender,EventArgs e) {
			if(listAvailableManualPages.SelectedIndex==-1) {
				return;
			}
			string selectedPage=listAvailableManualPages.SelectedItem.ToString();
			if(listBoxLinkedPages.Items.Contains(selectedPage)) {
				return;
			}
			listBoxLinkedPages.Items.Add(selectedPage);
			FilterHelper();
		}

		private void ButLeft_Click(object sender,EventArgs e) {
			listBoxLinkedPages.Items.RemoveAt(listBoxLinkedPages.SelectedIndex);
			FilterHelper();
		}

		///<summary>Opens a picker populated with every version available for the FAQ system.</summary>
		private void ButVersionPick_Click(object sender,EventArgs e) {
			//Get all versions after 18.4 (that is when the FAQ feature was introduced)
			List<int> listVersions=VersionReleases.GetVersionsAfter(18,4).Distinct().ToList();
			using FormFaqVersionPicker formFVP=new FormFaqVersionPicker(listVersions,_faqCur.IsNew);
			formFVP.ShowDialog();
			if(formFVP.DialogResult!=DialogResult.OK) {
				return;
			}
				_listVersions=formFVP.ListSelectedVersions;
			textManualVersion.Text=String.Join(",",_listVersions);
		}

		///<summary></summary>
		private void ButOK_Click(object sender,EventArgs e) {
			if(!IsValid()) {
				return;
			}
			List<string> listPagesToLink=listBoxLinkedPages.Items.GetAll<string>();
			_faqCur.QuestionText=PIn.String(textQuestion.Text);
			_faqCur.AnswerText=PIn.String(textAnswer.Text);
			_faqCur.EmbeddedMediaUrl=PIn.String(textEmbeddedMediaURL.Text);
			_faqCur.IsStickied=checkSticky.Checked;
			_faqCur.ImageUrl=PIn.String(textImagePath.Text);
			if(_listVersions==null || _listVersions.Count==0) {//This could be the case if we are in a quick add. We know the version text isn't empty because IsValid didn't return false.
				_listVersions=_listVersions??new List<int>();
				_listVersions.Add(PIn.Int(textManualVersion.Text));
			}
			if(_faqCur.IsNew) {
				//When creating a new faq the user can pick multiple versions for it to apply to.
				//Because Faq objects are versioned we must insert a copy of the faq for each version specified.
				foreach(int version in _listVersions) {
					Faq faq=_faqCur.Copy();
					faq.ManualVersion=version;
					long faqNum=Faqs.Insert(faq);
					//Create the links that the user defined.
					Faqs.CreateManualPageLinks(faq.FaqNum,listPagesToLink,faq.ManualVersion);
				}
			}
			else {//Users can only edit 1 Faq for a specific version at a time.
				_faqCur.ManualVersion=PIn.Int(textManualVersion.Text);
				Faqs.Update(_faqCur);
				//We now need to sync the old manual page links with the new ones. If I cared enough I would create a sync method
				//but since the faqmanualpagelink table will be small it's easier (and probably faster) to just remove all current 
				//links and re-add them. If this proves to cause issues then switch to a sync paradigm.
				Faqs.DeleteManualPageLinkForFaqNum(_faqCur.FaqNum);
				//Create the links that the user defined.
				Faqs.CreateManualPageLinks(_faqCur.FaqNum,listPagesToLink,_faqCur.ManualVersion);
			}
			DialogResult=DialogResult.OK;
		}

		private void ButDelete_Click(object sender,EventArgs e) {
			if(MessageBox.Show("You are about to delete this FAQ. Continue?","",MessageBoxButtons.YesNo)!=DialogResult.Yes) {
				return;
			}
			Faqs.DeleteManualPageLinkForFaqNum(_faqCur.FaqNum);
			Faqs.Delete(_faqCur.FaqNum);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void butBullet_Click(object sender,EventArgs e) {
			if(textAnswer.ReadOnly) {
				return;
			}
			HtmlTextHelper("<ul>","</ul>",(selectedText) => {
				string[] selectedLines=selectedText.Split("\n".ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
				return string.Join("",selectedLines.Select(x => $"<li>{x}</li>"));
			},false);
		}
	}
}
