using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	/*
Conversion Checklist====================================================================================================================
Questions (do not edit)                                              |Answers might include "yes", "ok", "no", "n/a", "done", etc.
-Review this form. Any unsupported controls or properties?           |done
   Search for "progress". Any progress bars?                         |done
   Anything in the Tray?                                             |no
   Search for "filter". Any use of SetFilterControlsAndAction?       |no
   If yes, then STOP here. Talk to Jordan for strategy               |
-Look in the code for any references to other Forms. If those forms  |no
   have not been converted, then STOP.  Convert those forms first.   |
-Will we include TabIndexes?  If so, up to what index?  This applies |no
   even if one single control is set so that cursor will start there |
-Grids: get familiar with properties in bold and with events.        |n/a
-Run UnitTests FormWpfConverter, type in Form name, TabI, and convert|done
-Any conversion exceptions? Consider reverting.                      |yes, System.Exception: 'Button image alignment not supported yet: MiddleCenter'
-In WpfControlsOD/Frms, include the new files in the project.        |done
-Switch to using this checklist in the new Frm. Delete the other one-|done
-Do the red areas and issues at top look fixable? Consider reverting |yes
-Does convert script need any changes instead of fixing manually?    |no
-Fix all the red areas.                                              |no, line 177 webPreview.DocumentText=document; and FormHelpBrowser lines 164,179,and 276 and FormFaqPicker lines 80 and 94
-Address all the issues at the top. Leave in place for review.       |Done note:couldn't figure out how to under the "U" for the underline button
-Verify that the Button click events converted automatically.        |Done
-Attach all orphaned event handlers to events in constructor.        |Done
-Possibly make some labels or other controls slightly bigger due to  |Done (label: "Filter Page Names")
   font change.                                                      |
-Change OK button to Save and get rid of Cancel button (in Edit      |Done
   windows). Put any old Cancel button functionality into a Close    |
   event handler.                                                    |
-Change all places where the form is called to now call the new Frm. |Done
-Test thoroughly                                                     |Unable to
-Are behavior and look absolutely identical? List any variation.     |
   Exceptions include taborders only applying to textboxes           |
   and minor control color variations if they are not annoying       |
-Copy original Form.cs into WpfControlsOD/Frms temporarily for review|
-Review with Jordan                                                  |
-Commit                                                              |
-Delete the old Winform files. That gets reviewed on the next round  |
End of Checklist=========================================================================================================================
*/
	public partial class FrmFaqEdit:FrmODBase {
		///<summary>Represents either a new faq object that will be inserted into the database or an existing faq object that needs to be updated.</summary>
		public Faq FaqCur;
		///<summary>Holds a list of all unique manualpage names corresponding to the selected version(s).</summary>
		private List<string> _listManualPagesForVersions;
		///<summary>Only set when creating a new FAQ object. This list holds versions selected by the user.</summary>
		private List<int> _listVersions;
		///<summary>Set to try when called from the quick add button in FormHelpBrowser</summary>
		public bool IsQuickAdd=false;

		///<summary>Pass in an existing faq object to edit it.</summary>
		public FrmFaqEdit() {
			InitializeComponent();
			Load+=FrmFaqEdit_Load;
			Shown+=FrmFaqEdit_Shown;
			textLinkPage.KeyUp+=textLinkPage_KeyUp;
			textQuestion.TextChanged+=textQuestion_TextChanged;
			textAnswer.TextChanged+=textAnswer_TextChanged;
			textImagePath.TextChanged+=textImagePath_TextChanged;
			textEmbeddedMediaURL.TextChanged+=textEmbeddedMediaURL_TextChanged;
			PreviewKeyDown+=FrmFaqEdit_PreviewKeyDown;
		}

		private void FrmFaqEdit_Load(object sender,EventArgs e) {
			//Jordan's manual page table has an entry for each page and version.
			_listVersions=new List<int>();
			if(FaqCur.ManualVersion!=0) {
				_listVersions.Add(FaqCur.ManualVersion);
			}
			_listManualPagesForVersions=Faqs.GetAllManualPageNamesForVersions(_listVersions);
			FillFaqInfo();
			if(FaqCur.IsNew) {
				butDelete.Visible=false;
				//New FAQs can really be more than one FAQ.  For this reason, we must only allow the user to pick versions for new FAQs.
				//If the FAQ is not new, it has one specific version and cannot be changed (must delete)
				butVersionPick.Visible=true;
			}
			FillGridManualPages(_listManualPagesForVersions);
		}

		private void FrmFaqEdit_Shown(object sender,EventArgs e) {
			FillPrieview();
		}

		///<summary>If editing an existing FAQ this method will fill the UI with its values.</summary>
		private void FillFaqInfo() {
			if(FaqCur.IsNew && !IsQuickAdd) {//Creating a new faq, no data to load
				return;
			}
			textManualVersion.Text="";
			if(FaqCur.ManualVersion!=0) {
				textManualVersion.Text=FaqCur.ManualVersion.ToString();
			}
			if(IsQuickAdd) {//We're going to have a new FAQ we're creating but we do have a new FAQ object that won't have a bunch of data set yet.
				List<string> listLinkedManualPageNames=_listManualPagesForVersions.FindAll(x=>x==FaqCur.ManualPageName).ToList();
				listBoxLinkedPages.Items.AddList(listLinkedManualPageNames,x=>x);
				textEmbeddedMediaURL.Visible=false;
				textImagePath.Visible=false;
				labelImagePath.Visible=false;
				labelEmbeddedMediaUrl.Visible=false;
			}
			else {
				textQuestion.Text=FaqCur.QuestionText;
				textAnswer.Text=FaqCur.AnswerText;
				textImagePath.Text=FaqCur.ImageUrl;
				textEmbeddedMediaURL.Text=FaqCur.EmbeddedMediaUrl;
				textUserCreated.Text=Userods.GetName(FaqCur.UserNumCreated);
				textUserEdited.Text=Userods.GetName(FaqCur.UserNumEdited);
				checkSticky.Checked=FaqCur.IsStickied;
				//Fill listbox with the manual pages the Faq object is currently linked to
				List<string> listLinkedPages=Faqs.GetLinkedManualPages(FaqCur.FaqNum);
				listBoxLinkedPages.Items.AddList(listLinkedPages,x=>x);
			}
			FillGridManualPages(_listManualPagesForVersions);
		}

		private void FillPrieview() {
			string question=RemoveScriptTags(textQuestion.Text);
			string answer=RemoveScriptTags(textAnswer.Text);
			string image=RemoveScriptTags(textImagePath.Text);
			string embeded=RemoveScriptTags(textEmbeddedMediaURL.Text);
			//This css is based on index.css found in the ODHelpFaqSite project.
			StringBuilder stringBuilderCss=new StringBuilder("<style>" +
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
			StringBuilder stringBuilderHtml=new StringBuilder("<body><div class=\"panel panel-default\">");
			stringBuilderHtml.Append("<div class=\"panel-heading\">"+" &nbsp;&nbsp;&nbsp;&nbsp;- "+question+"</div>");
			stringBuilderHtml.Append("<div class=\"panel-collapse collapse\">");
			if(answer!="") {
				stringBuilderHtml.Append("<div style=\"white-space: pre-line\" class=\"panel-body\">"+answer);
			}
			if(image!="" || embeded!="") {
				stringBuilderHtml.Append("<div class=\"container-fluid\">");
				if(textImagePath.Text!="") {
					stringBuilderHtml.Append("<div class=\"row\"><div style=\"margin-top: 5px\"><a style=\"block;\">");
					stringBuilderHtml.Append("<img src=\""+image+"\" style=\"max-width:100%;max-height:100%;\"><br>");
					stringBuilderHtml.Append("</a></div></div>");
				}
				if(embeded!="") {
					stringBuilderHtml.Append("<div class=\"row\">");
					stringBuilderHtml.Append("<iframe style=\"margin-top: 5px;height: 400px;width: 100%;\" src=\""+embeded+"\"></iframe>");
					stringBuilderHtml.Append("</div>");
				}
				stringBuilderHtml.Append("</div>");
			}
			if(answer!="") {
				stringBuilderHtml.Append("</div>");
			}
			stringBuilderHtml.Append("</div></div></body>");
			string document=$"<!DOCTYPE html><html><head>{stringBuilderCss}</head>{stringBuilderHtml}</html>";
			webPreview.NavigateToString(document);
		}

		string RemoveScriptTags(string toParse) {
			return toParse.Replace("<script>","").Replace("</script>","");
		}

		///<summary>Helper method that ensures the required UI fields have valid data.</summary>
		private bool IsValid() {
			StringBuilder stringBuilderErrMsg=new StringBuilder();
			//Check that a question and answer were specified. Must have both
			if(String.IsNullOrEmpty(textQuestion.Text)||String.IsNullOrEmpty(textAnswer.Text)) {
				stringBuilderErrMsg.AppendLine("Cannot create an FAQ without a question or answer.");
			}
			//Must have a version
			if(String.IsNullOrEmpty(textManualVersion.Text)) {
				stringBuilderErrMsg.AppendLine("Must specify a manual version.");
			}
			//Must be linked to at least 1 manual page
			if(listBoxLinkedPages.Items.Count<1) {
				stringBuilderErrMsg.AppendLine("The FAQ must be linked to at least one manual page.");
			}
			if(!String.IsNullOrEmpty(stringBuilderErrMsg.ToString())) {//Errors found, inform user
				MsgBox.Show(this,"Missing/Incorrect Data:\r\n"+stringBuilderErrMsg.ToString());
				return false;
			}
			return true;
		}

		///<summary>Helper method that inserts the specified html tag at the current cursor position. 
		///If text is selected it will be wrapped in the specified tag.</summary>
		private void HtmlTextHelper(string tagStart,string tagEnd,Func<string,string> innerTextAction=null,bool doInsertWhenNoSelection=true) {
			int startIndex=textAnswer.SelectionStart;
			if(textAnswer.SelectedText.Length>0) {//user has text highlighted
				string selectedText=textAnswer.Text.Substring(startIndex,textAnswer.SelectionLength);
				selectedText=(innerTextAction?.Invoke(selectedText)??selectedText);
				string newText=tagStart+selectedText+tagEnd;
				textAnswer.Text=textAnswer.Text.Substring(0,startIndex)+newText+textAnswer.Text.Substring(startIndex+textAnswer.SelectionLength);
				//Place cursor at the end of the current text.
				textAnswer.SelectionStart=startIndex+newText.Length;
			}
			else if(doInsertWhenNoSelection) {//No text selected, insert tags at cursor position
				textAnswer.Text=textAnswer.Text.Insert(startIndex,tagStart+tagEnd);
				//Be a bro and put the cursor between the html tags after inserting them
				textAnswer.SelectionStart=startIndex+tagStart.Length;
			}
			else {
				//Warn the user for buttons that require selected text. Currently only possible when clicking butBullet.
				MsgBox.Show(this, "Text must be highlighted to use this button.");
			}
			textAnswer.SelectionLength=0;
			textAnswer.Focus();//.Select();
		}

		///<summary>A method that runs the TextLinkPage_KeyUp logic. This helper allows the grid to be refreshed whilst maintaining the user's filter.</summary>
		private void FilterHelper() {
			string textEntered=textLinkPage.Text;
			List<string> listResults=new List<string>();
			for(int i=0;i<_listManualPagesForVersions.Count();i++) {
				if(_listManualPagesForVersions[i].Contains(textEntered)) {
					listResults.Add(_listManualPagesForVersions[i]);
				}
			}
			FillGridManualPages(listResults);
		}

		///<summary>Fills the manual page grid with the given list. This method should be called after listBoxLinkedPages has been filled.</summary>
		private void FillGridManualPages(List<string> listPages) {
			listAvailableManualPages.Items.Clear();
			listAvailableManualPages.Items.AddList(listPages,x=>x);
		}

		private void butBold_Click(object sender,EventArgs e) {
			HtmlTextHelper("<b>","</b>");
		}

		private void butItalics_Click(object sender,EventArgs e) {
			HtmlTextHelper("<i>","</i>");
		}

		private void butUnderline_Click(object sender,EventArgs e) {
			HtmlTextHelper("<u>","</u>");
		}

		private void butHyperlink_Click(object sender,EventArgs e) {
			HtmlTextHelper("<a target=\"_blank\" href=\"Enter URL here\">","</a>");
		}

		///<summary>Filters the list of manual pages down to those that start with the entered text.</summary>
		private void textLinkPage_KeyUp(object sender,KeyEventArgs e) {
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

		private void butRight_Click(object sender,EventArgs e) {
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

		private void butLeft_Click(object sender,EventArgs e) {
			listBoxLinkedPages.Items.RemoveAt(listBoxLinkedPages.SelectedIndex);
			FilterHelper();
		}

		///<summary>Opens a picker populated with every version available for the FAQ system.</summary>
		private void butVersionPick_Click(object sender,EventArgs e) {
			//Get all versions after 18.4 (that is when the FAQ feature was introduced)
			List<int> listVersions=VersionReleases.GetVersionsAfter(18,4).Distinct().ToList();
			FrmFaqVersionPicker frmFaqVersionPicker=new FrmFaqVersionPicker(listVersions,FaqCur.IsNew);
			frmFaqVersionPicker.ShowDialog();
			if(!frmFaqVersionPicker.IsDialogOK) {
				return;
			}
			_listVersions=frmFaqVersionPicker.ListSelectedVersions;
			textManualVersion.Text=String.Join(",",_listVersions);
			_listManualPagesForVersions=Faqs.GetAllManualPageNamesForVersions(_listVersions);
			FillGridManualPages(_listManualPagesForVersions);
		}

		private void FrmFaqEdit_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
		}

		///<summary></summary>
		private void butSave_Click(object sender,EventArgs e) {
			if(!IsValid()) {
				return;
			}
			FaqCur.UserNumEdited=Security.CurUser.UserNum;
			List<string> listPagesToLink=listBoxLinkedPages.Items.GetAll<string>();
			FaqCur.QuestionText=PIn.String(textQuestion.Text);
			FaqCur.AnswerText=PIn.String(textAnswer.Text);
			FaqCur.EmbeddedMediaUrl=PIn.String(textEmbeddedMediaURL.Text);
			FaqCur.IsStickied=checkSticky.Checked==true;
			FaqCur.ImageUrl=PIn.String(textImagePath.Text);
			if(_listVersions==null || _listVersions.Count==0) {//This could be the case if we are in a quick add. We know the version text isn't empty because IsValid didn't return false.
				_listVersions=_listVersions??new List<int>();
				_listVersions.Add(PIn.Int(textManualVersion.Text));
			}
			if(FaqCur.IsNew) {
				FaqCur.UserNumCreated=Security.CurUser.UserNum;
				//When creating a new faq the user can pick multiple versions for it to apply to.
				//Because Faq objects are versioned we must insert a copy of the faq for each version specified.
				for(int i=0;i<_listVersions.Count();i++) {
					Faq faq=FaqCur.Copy();
					faq.ManualVersion=_listVersions[i];
					long faqNum=Faqs.Insert(faq);
					//Create the links that the user defined.
					Faqs.CreateManualPageLinks(faq.FaqNum,listPagesToLink,faq.ManualVersion);
				}
			}
			else {//Users can only edit 1 Faq for a specific version at a time.
				FaqCur.ManualVersion=PIn.Int(textManualVersion.Text);
				Faqs.Update(FaqCur);
				//We now need to sync the old manual page links with the new ones. If I cared enough I would create a sync method
				//but since the faqmanualpagelink table will be small it's easier (and probably faster) to just remove all current 
				//links and re-add them. If this proves to cause issues then switch to a sync paradigm.
				Faqs.DeleteManualPageLinkForFaqNum(FaqCur.FaqNum);
				//Create the links that the user defined.
				Faqs.CreateManualPageLinks(FaqCur.FaqNum,listPagesToLink,FaqCur.ManualVersion);
			}
			Close();//non-modal
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(MessageBox.Show("You are about to delete this FAQ. Continue?","",MessageBoxButton.YesNo)!=MessageBoxResult.Yes) {
				return;
			}
			Faqs.DeleteManualPageLinkForFaqNum(FaqCur.FaqNum);
			Faqs.Delete(FaqCur.FaqNum);
			Close();//non-modal
		}

		private void butBullet_Click(object sender,EventArgs e) {
			if(textAnswer.ReadOnly) {
				return;
			}
			HtmlTextHelper("<ul>","</ul>",(selectedText) => {
				string[] selectedLines=selectedText.Split("\r\n".ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
				return string.Join("",selectedLines.Select(x => $"<li>{x}</li>"));
			},false);
		}
	}
}
