using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	/*
Conversion Checklist====================================================================================================================
Questions (do not edit)                                              |Answers might include "yes", "ok", "no", "n/a", "done", etc.
-Review this form. Any unsupported controls or properties?           |no
   Search for "progress". Any progress bars?                         |no
   Anything in the Tray?                                             |ImageList of 2 images for treeView-handled
   Search for "filter". Any use of SetFilterControlsAndAction?       |no
   If yes, then STOP here. Either talk to Jordan, or don't convert.  |
-Look in the code for any references to other Forms. If those forms  |
   have not been converted, then STOP.  Convert those forms first.   |done
-Will we include TabIndexes?  If so, up to what index?  This applies |no
   even if one single control is set so that cursor will start there |
-Grids: get familiar with properties in bold and with events.        |n/a
-Run UnitTests FormWpfConverter, type in Form name, TabI, and convert|done
-Any conversion exceptions? Consider reverting.                      |no
-In WpfControlsOD/Frms, include the new files in the project.        |done
-Switch to using this checklist in the new Frm. Delete the other one-|done
-Do the red areas and issues at top look fixable? Consider reverting |yes
-Does convert script need any changes instead of fixing manually?    |probably not
-Fix all the red areas.                                              |done
-Address all the issues at the top. Leave in place for review.       |done
-Verify that the Button click events converted automatically.        |done
-Attach all orphaned event handlers to events in constructor.        |done
-Possibly make some labels or other controls slightly bigger due to  |n/a
   font change.                                                      |
-Change OK button to Save and get rid of Cancel button (in Edit      |done
   windows). Put any old Cancel button functionality into a Close    |
   event handler.                                                    |
-Change all places where the form is called to now call the new Frm. |Just one spot so far
-Test thoroughly                                                     |
-Are behavior and look absolutely identical? List any variation.     |
-Copy original Form.cs into WpfControlsOD/Frms temporarily for review|
-Review with Jordan                                                  |
-Commit                                                              |
-Delete the old Winform files. That gets reviewed on the next round  |
End of Checklist=========================================================================================================================
*/
	public partial class FrmAutoNoteCompose:FrmODBase {
		public string StrCompletedNote;
		public string StrMainTextNote;
		///<summary>On load, the UserOdPref that contains the comma delimited list of expanded category DefNums is retrieved from the database.  On close
		///the UserOdPref is updated with the current expanded DefNums.</summary>
		private UserOdPref _userOdPref;
		private List<Def> _listDefs;

		public FrmAutoNoteCompose() {
			InitializeComponent();
			Lang.F(this);
			Load+=FrmAutoNoteCompose_Load;
			FormClosing+=FrmAutoNoteCompose_FormClosing;
			Shown+=FrmAutoNoteCompose_Shown;
			treeView.MouseDoubleClick+=TreeView_MouseDoubleClick;
			textRichMain.TextChanged+=TextRichMain_TextChanged;
		}

		private void FrmAutoNoteCompose_Load(object sender,EventArgs e) {
			_listDefs=Defs.GetDefsForCategory(DefCat.AutoNoteCats,true);
			_userOdPref=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.AutoNoteExpandedCats).FirstOrDefault();
			FillTreeView();
			butOK.Visible=false;//save button not visible until text present
		}

		private void FrmAutoNoteCompose_Shown(object sender,EventArgs e) {
			if(!string.IsNullOrEmpty(StrMainTextNote)) {
				PromptForAutoNotes(StrMainTextNote,new List<AutoNoteListItem>());
			}
		}

		private void FillTreeView() {
			List<long> listDefNumsExpanded=new List<long>();
			if(_userOdPref!=null) {//if this is the fill on load, the node count will be 0, expanded node list from pref
				listDefNumsExpanded=_userOdPref.ValueString.Split(',').Where(x => x!="" && x!="0").Select(x => PIn.Long(x)).ToList();
			}
			//clear current tree contents
			treeView.SelectedItem=null;
			treeView.Items.Clear();
			//add categories with all auto notes that are assigned to that category
			List<long> listDefNumsCats=_listDefs.Select(x => x.DefNum).ToList();
			//Get a list of root cats (where def.ItemValue is blank) or any def with invalid parent def num (ItemValue)
			List<Def> listDefsRoots=_listDefs.FindAll(x => string.IsNullOrWhiteSpace(x.ItemValue) || !listDefNumsCats.Contains(PIn.Long(x.ItemValue)));
			for(int i=0;i<listDefsRoots.Count;i++){
				treeView.Items.Add(CreateNodeAndChildren(listDefsRoots[i]));//child cats and categorized auto notes added in recursive function
			}
			//add any uncategorized auto notes after the categorized ones and only for the root nodes
			List<AutoNote> listAutoNotes=AutoNotes.GetWhere(x => x.Category==0 || !listDefNumsCats.Contains(x.Category));
			for(int i=0;i<listAutoNotes.Count;i++){
				TreeViewItem treeViewItem=new TreeViewItem();
				treeViewItem.Text=listAutoNotes[i].AutoNoteName;
				treeViewItem.Icon=EnumIcons.ImageSelectorDoc;
				treeViewItem.Tag=listAutoNotes[i];
				treeView.Items.Add(treeViewItem);
			}
			List<TreeViewItem> listTreeViewItemsFlat=new List<TreeViewItem>();
			for(int i=0;i<treeView.Items.Count;i++){
				List<TreeViewItem> listTreeViewItemsChildren=GetSelfAndChildren(treeView.Items[i]);
				listTreeViewItemsFlat.AddRange(listTreeViewItemsChildren);
			}
			for(int i=0;i<listTreeViewItemsFlat.Count;i++){
				if(!(listTreeViewItemsFlat[i].Tag is Def)){
					continue;
				}
				Def def=(Def)listTreeViewItemsFlat[i].Tag;
				if(listDefNumsExpanded.Contains(def.DefNum)){
					listTreeViewItemsFlat[i].IsExpanded=true;
				}
			}
		}

		///<summary>Recursive. Returns a tree node with all descendants, including all auto note children for this def cat and all children for
		///any cat within this this cat.  Auto Notes that are at the 'root' level (considered uncategorized) have to be added separately after filling the
		///rest of the tree with this method and will be at the bottom of the root node list.</summary>
		private TreeViewItem CreateNodeAndChildren(Def def) {
			//this will be all child folders and all child autonotes, all nested into a tree.
			List<TreeViewItem> listTreeViewItemsChildren=new List<TreeViewItem>();
			//child folders
			List<Def> listDefsFolders=_listDefs.FindAll(x => x.ItemValue==def.DefNum.ToString());
			for(int i=0;i<listDefsFolders.Count;i++){
				listTreeViewItemsChildren.Add(CreateNodeAndChildren(listDefsFolders[i]));
			}
			//autonotes
			List<AutoNote> listAutoNotes=AutoNotes.GetWhere(x => x.Category==def.DefNum);
			for(int i=0;i<listAutoNotes.Count;i++){
				TreeViewItem treeViewItemAN=new TreeViewItem();
				treeViewItemAN.Text=listAutoNotes[i].AutoNoteName;
				treeViewItemAN.Icon=EnumIcons.ImageSelectorDoc;
				treeViewItemAN.Tag=listAutoNotes[i];
				listTreeViewItemsChildren.Add(treeViewItemAN);
			}
			TreeViewItem treeViewItem=new TreeViewItem();
			treeViewItem.Text=def.ItemName;
			treeViewItem.Icon=EnumIcons.ImageSelectorFolder;
			treeViewItem.Tag=def;
			listTreeViewItemsChildren=listTreeViewItemsChildren.OrderBy(x => x.Tag is AutoNote).ThenBy(x => x.Name).ToList();
			for(int i=0;i<listTreeViewItemsChildren.Count;i++){
				treeViewItem.Items.Add(listTreeViewItemsChildren[i]);
			}
			return treeViewItem;
		}

		///<summary>Returns a flat list containing this TreeViewItem and all of its descendant TreeViewItems.  Recursive function to walk the full depth of the tree starting at this TreeViewItem.</summary>
		private List<TreeViewItem> GetSelfAndChildren(TreeViewItem treeViewItem) {
			List<TreeViewItem> listTreeViewItems=new List<TreeViewItem>();
			listTreeViewItems.Add(treeViewItem);//add self
			for(int i=0;i<treeViewItem.Items.Count;i++){
				List<TreeViewItem> listTreeNodesChildren=GetSelfAndChildren((TreeViewItem)treeViewItem.Items[i]);
				listTreeViewItems.AddRange(listTreeNodesChildren);
			}
			return listTreeViewItems;
		}

		private void TreeView_MouseDoubleClick(object sender,MouseButtonEventArgs e) {
			InsertSelectedAutoNote();
		}

		private void butInsert_Click(object sender,EventArgs e) {
			InsertSelectedAutoNote();
		}

		private void InsertSelectedAutoNote() {
			if(treeView.SelectedItem==null || !(treeView.SelectedItem.Tag is AutoNote)) {
				return;
			}
			string note=((AutoNote)treeView.SelectedItem.Tag).MainText;
			PromptForAutoNotes(note,new List<AutoNoteListItem>());
			treeView.SelectedItem=null;//clear selected node
		}

		private void TextRichMain_TextChanged(object sender,EventArgs e) {
			if(textRichMain.Text.Trim()=="") {
				butOK.Visible=false;
			}
			else {
				butOK.Visible=true;
			}
		}

		///<summary>Returns the length prompt responses added to textMain.Text.</summary>
		private int PromptForAutoNotes(string noteToParse,List<AutoNoteListItem> listAutoNoteItems) {
			//AutoNote.MainText which should have all text and the prompts.
			string strNote=noteToParse;
			#region Insert Auto Note Text
			//Logic for determining where the note should go based on the users cursor location.
			int selectionStart=textRichMain.SelectionStart;
			if(selectionStart==0) {//Cursor is at the beginning of the textbox.
				textRichMain.Text=strNote+textRichMain.Text;
			}
			else if(selectionStart==textRichMain.Text.Length-1) {//Cursor at end of textbox
				textRichMain.Text=textRichMain.Text+strNote;
			}
			else if(selectionStart==-1) {//If cursor location is unknown just append the text to the end
				textRichMain.Text=textRichMain.Text+strNote;
			}
			else {//Cursor is in between text. Insert at the selected position.
				textRichMain.Text=textRichMain.Text.Substring(0,selectionStart)+strNote+textRichMain.Text.Substring(selectionStart);
			}
			#endregion
			//List of prompts for the auto note
			List<AutoNoteControl> listAutoNoteControlsPrompts=new List<AutoNoteControl>();
			//Prompts are stored in the form [Prompt: "PromptName"]
			List<Match> listMatchesPrompts=AutoNoteControls.GetPrompts(strNote);
			//Remove all matched prompts that do not exist in the database.
			listMatchesPrompts.RemoveAll(x => AutoNoteControls.GetByDescript(x.Value.Substring(9,x.Value.Length-11))==null);
			//Holds the PromptName from [Prompt: "PromptName"]
			string autoNoteDescript;
			AutoNoteControl autoNoteControl;
			string strPromptResponse;
			int startLoc=0;
			int retVal=0;//the length of the notes added to textMain.Text.
			//used to keep track of the start position. This is needed set idxMatch. Without this, match loc might find the wrong index:
			Stack<int> stackIdxs = new Stack<int>();
			stackIdxs.Push(startLoc);
			bool isAutoNote=false;
			//Loop through all valid prompts for the given auto note.
			for(int i=0;i<listMatchesPrompts.Count;i++) {
				//Find prompt location in the text and highlight yellow.
				int idxMatch=textRichMain.Text.IndexOf(listMatchesPrompts[i].Value,startLoc);
				if(idxMatch==-1 || idxMatch>textRichMain.Text.Length) {	//The value wasn't found
					continue;
				}
				startLoc=idxMatch+1;//Add one to look after the last match location.
				textRichMain.Select(idxMatch,listMatchesPrompts[i].Value.Length);
				//TextSelection textSelection=textRichMain.GetSelection();
				textRichMain.SelectionBackColor(Colors.Yellow);
				textRichMain.SelectionLength=0;
				DoEvents();//refresh the textbox so the yellow will show
				//Holds the PromptName from [Prompt: "PromptName"]
				autoNoteDescript=listMatchesPrompts[i].Value.Substring(9,listMatchesPrompts[i].Value.Length-11);
				autoNoteControl=AutoNoteControls.GetByDescript(autoNoteDescript);//should never be null since we removed nulls above
				strPromptResponse="";
				if(autoNoteControl.ControlType=="Text") {//Response just inserts text. No choosing options here.
					FrmAutoNotePromptText frmAutoNotePromptText=new FrmAutoNotePromptText(autoNoteDescript);
					frmAutoNotePromptText.PromptText=autoNoteControl.ControlLabel;
					frmAutoNotePromptText.ResultText=autoNoteControl.ControlOptions;
					isAutoNote=false;
					if(i>0) {
						frmAutoNotePromptText.IsGoBack=true;//user can go back if at least one item in the list exist
					}
					if(listAutoNoteItems.Count>i) {
						frmAutoNotePromptText.PromptResponseCur=listAutoNoteItems[i].StrAutoNotePromptResponse;//pass the previous response to the form
					}
					frmAutoNotePromptText.ShowDialog();
					if(!frmAutoNotePromptText.IsDialogOK && frmAutoNotePromptText.IsRetry) {//user clicked the go back button
						GoBack(i,listAutoNoteItems);
						stackIdxs.Pop();//remove the start location
						startLoc=stackIdxs.Peek();//set the new start location
						i-=2;
						continue;
					}
					else if(frmAutoNotePromptText.IsDialogOK) {
						strPromptResponse=frmAutoNotePromptText.ResultText;
						if(listAutoNoteItems.Count>i) {//reponse already exist for this control type. Update it
							//We need to update retVal with the length of the new promptResponse. First, remove the previous prompts length.
							retVal-=listAutoNoteItems[i].AutoNoteTextEndPos;
							//Update the retVal with the new promptResponse.Length.
							retVal+=strPromptResponse.Length;
							//Update the rest of the AutoNoteItem with the new promptResponse.
							listAutoNoteItems[i].StrAutoNotePromptResponse=strPromptResponse;
							listAutoNoteItems[i].AutoNoteTextStartPos=idxMatch;
							listAutoNoteItems[i].AutoNoteTextEndPos=strPromptResponse.Length;
						}
						else {
							//This is a new response. Create a new AutoNoteListItem and add to retVal.
							retVal+=strPromptResponse.Length;
							listAutoNoteItems.Add(new AutoNoteListItem(listMatchesPrompts[i].Value,strPromptResponse,idxMatch,strPromptResponse.Length));//add new response to the list
						}
					}
					else {//User cancelled out of the auto note prompt text form or auto log off.
						ResetTextMain();
						return -1;
					}
				}
				else if(autoNoteControl.ControlType=="OneResponse") {
					FrmAutoNotePromptList frmAutoNotePromptList=new FrmAutoNotePromptList(autoNoteDescript);
					frmAutoNotePromptList.PromptText=autoNoteControl.ControlLabel;
					frmAutoNotePromptList.PromptOptions=autoNoteControl.ControlOptions;
					if(i>0) {
						frmAutoNotePromptList.IsGoBack=true;//user can go back if at least one item in the list exist
					}
					if(listAutoNoteItems.Count>i) {
						frmAutoNotePromptList.PreviousResponse=listAutoNoteItems[i].StrAutoNotePromptResponse;//pass the previous response if exist to the form.
					}
					frmAutoNotePromptList.ShowDialog();
					if(!frmAutoNotePromptList.IsDialogOK && frmAutoNotePromptList.IsRetry) {//user clicked the go back button
						GoBack(i,listAutoNoteItems);
						stackIdxs.Pop();//remove the start location
						startLoc=stackIdxs.Peek();//set the new start location
						i-=2;
						continue;
					}
					else if(frmAutoNotePromptList.IsDialogOK) {
						strPromptResponse=frmAutoNotePromptList.ResultText;
						//The promptResponse will have the AutoNoteName in the format "Auto Note Response : {AutoNoteName}". Use the name to get the note that 
						//will be used in the recursive call below.
						string autoNoteString="";
						isAutoNote=false;
						string autoNoteName=AutoNotes.GetAutoNoteName(strPromptResponse);
						if(!string.IsNullOrEmpty(autoNoteName)) {
							isAutoNote=true;
							//For some reason the Auto Note string always contains a new line and a return character at the end of the note. Must be trimmed
							autoNoteString=AutoNotes.GetByTitle(autoNoteName).TrimEnd('\n').TrimEnd('\r');//Returns empty string If no AutoNote is found. 
						}
						if(listAutoNoteItems.Count>i && !isAutoNote) {//The response already exist for this control type and it is note an AutoNote. Update it
							//We need to update retval with the length of the new promptResponse. First, remove the previous promptResponse length.
							retVal-=listAutoNoteItems[i].AutoNoteTextEndPos;
							//Update the retval with the new promptResponse.Length.
							retVal+=strPromptResponse.Length;
							//Update the rest of the AutoNoteItem with the new promptResponse.
							listAutoNoteItems[i].StrAutoNotePromptResponse=strPromptResponse;
							listAutoNoteItems[i].AutoNoteTextStartPos=idxMatch;
							listAutoNoteItems[i].AutoNoteTextEndPos=strPromptResponse.Length;
						}
						else if(isAutoNote) {//The response is an auto note. Recursively call this method.
							//Remove the response from textMain.Text. The response was already saved. Since this is an AutoNote, the response does not need to stay 
							//in the textMain.Text since more than likely more prompts will happen after we call the recursive method below.
							string textMainWithoutResponse=textRichMain.Text.Substring(0,idxMatch)+GetAutoNoteResponseText(strPromptResponse);
							if(textRichMain.Text.Length>idxMatch+listMatchesPrompts[i].Value.Length) {
								textMainWithoutResponse+=textRichMain.Text.Substring(idxMatch+listMatchesPrompts[i].Value.Length);
							}
							idxMatch+=GetAutoNoteResponseText(strPromptResponse).Length;
							//set the textMain.Text to the new result. This removes the promptResponse.
							textRichMain.Text=textMainWithoutResponse;
							textRichMain.SelectionStart=idxMatch;//This is needed in order for the recursive method call below.
							//Pass in the AutoNotes note in the recursive call. 
							int lengthOfAutoNoteAdded=PromptForAutoNotes(autoNoteString,new List<AutoNoteListItem>());
							if(lengthOfAutoNoteAdded==-1) {
								return -1;
							}
							//When we get back from the recursive method, we need to figure out what was added so we can create the AutoNoteListItem
							strPromptResponse=textRichMain.Text.Substring(idxMatch,lengthOfAutoNoteAdded);
							//Update the retVal with the new promptResponse.Length.
							retVal+=strPromptResponse.Length;
							//if response already exist, update it.
							if(listAutoNoteItems.Count>i) {
								//We need to update retVal with the length of the new promptResponse. First, remove the previous prompts length.
								retVal-=listAutoNoteItems[i].AutoNoteTextEndPos;
								//Update the rest of the AutoNoteItem with the new promptResponse.
								listAutoNoteItems[i].StrAutoNotePromptResponse=strPromptResponse;//should be the same. Updating just in case.
								listAutoNoteItems[i].AutoNoteTextStartPos=idxMatch;
								listAutoNoteItems[i].AutoNoteTextEndPos=strPromptResponse.Length;//This is the length of what was added.
							}
							else {//New Response. Add a new AutoNoteListItem.
								//Add the response to listAutoNoteItem. This will allow the user to go back. Make the end position equal to the length of the AutoNote
								listAutoNoteItems.Add(new AutoNoteListItem(listMatchesPrompts[i].Value,strPromptResponse,idxMatch,strPromptResponse.Length));
							}
						}
						else {
							//This is a new response. Create a new AutoNoteListItem and add to retVal.
							retVal+=strPromptResponse.Length;
							listAutoNoteItems.Add(new AutoNoteListItem(listMatchesPrompts[i].Value,strPromptResponse,idxMatch,strPromptResponse.Length));//add new response to the list
						}
					}
					else {//User cancelled out of the auto note response form or auto log off.
						ResetTextMain();
						return -1;
					}
				}
				else if(autoNoteControl.ControlType=="MultiResponse") {
					FrmAutoNotePromptList frmAutoNotePromptList=new FrmAutoNotePromptList(autoNoteDescript);
					frmAutoNotePromptList.IsMultiResponse=true;
					frmAutoNotePromptList.PromptText=autoNoteControl.ControlLabel;
					frmAutoNotePromptList.PromptOptions=autoNoteControl.ControlOptions;
					isAutoNote=false;
					if(i>0) {
						frmAutoNotePromptList.IsGoBack=true;//user can go back if at least one item in the list exist
					}
					if(listAutoNoteItems.Count>i) {
						frmAutoNotePromptList.PreviousResponse=listAutoNoteItems[i].StrAutoNotePromptResponse;//pass the previous response if exist to the form.
					}
					frmAutoNotePromptList.ShowDialog();
					if(frmAutoNotePromptList.IsRetry) {//user clicked the go back button
						GoBack(i,listAutoNoteItems);
						stackIdxs.Pop();//remove the start location
						startLoc=stackIdxs.Peek();//set the new start location
						i-=2;
						continue;
					}
					if(frmAutoNotePromptList.IsDialogOK) {
						strPromptResponse=frmAutoNotePromptList.ResultText;
						if(listAutoNoteItems.Count>i) {//reponse already exist for this control type. Update it
							//We need to update retval with the length of the new promptresponse.First, remove the previous prompts length.
							retVal-=listAutoNoteItems[i].AutoNoteTextEndPos;
							//update the retval with the new promptResponse.Length.
							retVal+=strPromptResponse.Length;
							//Update the rest of the AutoNoteItem with the new promptResponse.
							listAutoNoteItems[i].StrAutoNotePromptResponse=strPromptResponse;
							listAutoNoteItems[i].AutoNoteTextStartPos=idxMatch;
							listAutoNoteItems[i].AutoNoteTextEndPos=strPromptResponse.Length;
						}
						else {
							//This is a new response. Create a new AutoNoteListItem and add to retVal.
							retVal+=strPromptResponse.Length;
							listAutoNoteItems.Add(new AutoNoteListItem(listMatchesPrompts[i].Value,strPromptResponse,idxMatch,strPromptResponse.Length));//add new response to the list
						}
					}
					else {//User cancelled out of the auto note response form or auto log off.
						ResetTextMain();
						return -1;
					}
				}
				string strResults=textRichMain.Text.Substring(0,idxMatch)+strPromptResponse;
				if(!isAutoNote && textRichMain.Text.Length > idxMatch+listMatchesPrompts[i].Value.Length) {
					strResults+=textRichMain.Text.Substring(idxMatch+listMatchesPrompts[i].Value.Length);
				}
				else if(isAutoNote && textRichMain.Text.Length > idxMatch+strPromptResponse.Length) {
					//if any of the prompts had an AutoNote and textmain has more text, add the rest of textMain. 
					strResults+=textRichMain.Text.Substring(idxMatch+strPromptResponse.Length);
					//update the startLoc to include the promptResponse of the AutoNote.
					startLoc+=strPromptResponse.Length-1;
				}
				textRichMain.Text=strResults;
				ResetTextMain();
				if(string.IsNullOrEmpty(strPromptResponse)) {
					//if prompt was removed, add the previous start location onto the stack. 
					startLoc=stackIdxs.Peek();
				}
				stackIdxs.Push(startLoc);
				DoEvents();//refresh the textbox
			}
			ResetTextMain();
			listAutoNoteItems.Clear();
			return retVal;
		}

		private void ResetTextMain() {
			textRichMain.SelectAll();
			textRichMain.SelectionBackColor(Colors.White);
			textRichMain.Select(textRichMain.Text.Length,0);
		}

		private string GetAutoNoteResponseText(string promptResponse) {
			string retVal="";
			//AutoNoteResponseText should be in the format "Auto Note Response Text : {AutoNoteName}". 
			//The response text will be everything to the left of the ':'
			int colonPos=promptResponse.IndexOf(':');
			if(colonPos>-1) {
				retVal=promptResponse.Substring(0,colonPos)+"\n";
			}
			return retVal;
		}

		private void butOK_Click(object sender,EventArgs e) {
			StrCompletedNote=textRichMain.Text;//.Replace("\n","\r\n");
			IsDialogOK=true;
		}

		///<summary>Removes previous response from the listAutoNoteItems and inserts the AutoNote prompt into textMain.</summary>
		private void GoBack(int pos,List<AutoNoteListItem> listAutoNoteListItems) {
			textRichMain.SelectAll();
//			textMain.SelectionBackColor=Color.White;
			textRichMain.Text=textRichMain.Text.Remove(listAutoNoteListItems[pos-1].AutoNoteTextStartPos,listAutoNoteListItems[pos-1].AutoNoteTextEndPos)
				.Insert(listAutoNoteListItems[pos-1].AutoNoteTextStartPos,listAutoNoteListItems[pos-1].AutoNotePromptText);
			if(listAutoNoteListItems[pos-1].StrAutoNotePromptResponse==listAutoNoteListItems[pos-1].AutoNotePromptText) {
				listAutoNoteListItems[pos-1].StrAutoNotePromptResponse="";
			}
			DoEvents();
		}

		private void FrmAutoNoteCompose_FormClosing(object sender,CancelEventArgs e) {
			//store the current node expanded state for this user
			List<TreeViewItem> listTreeViewItemsFlat=new List<TreeViewItem>();
			for(int i=0;i<treeView.Items.Count;i++){
				List<TreeViewItem> listTreeViewItemsChildren=GetSelfAndChildren(treeView.Items[i]);
				listTreeViewItemsFlat.AddRange(listTreeViewItemsChildren);
			}
			List<long> listDefNumsExpanded=new List<long>();
			for(int i=0;i<listTreeViewItemsFlat.Count;i++){
				if(!(listTreeViewItemsFlat[i].Tag is Def)){
					continue;
				}
				Def def=(Def)listTreeViewItemsFlat[i].Tag;
				if(listTreeViewItemsFlat[i].IsExpanded){
					listDefNumsExpanded.Add(def.DefNum);
				}
			}
			if(_userOdPref==null) {
				UserOdPref userOdPref=new UserOdPref();
				userOdPref.UserNum=Security.CurUser.UserNum;
				userOdPref.FkeyType=UserOdFkeyType.AutoNoteExpandedCats;
				userOdPref.ValueString=string.Join(",",listDefNumsExpanded);
				UserOdPrefs.Insert(userOdPref);
				DataValid.SetInvalid(InvalidType.UserOdPrefs);
			}
			else {
				UserOdPref userOdPrefOld=_userOdPref.Clone();
				_userOdPref.ValueString=string.Join(",",listDefNumsExpanded);
				if(UserOdPrefs.Update(_userOdPref,userOdPrefOld)) {
					//Only need to signal cache refresh on change.
					DataValid.SetInvalid(InvalidType.UserOdPrefs);
				}
			}
		}

		///<summary>This holds AutoNote prompt responses. Mostly used when using the back button within the Prompt Responses.</summary>
		private class AutoNoteListItem {
			///<summary>The AutoNote Prompt Text. The "PromptName" from the following example [Prompt: "PromptName"].</summary>
			public string AutoNotePromptText;
			///<summary>The Prompt response string.</summary>
			public string StrAutoNotePromptResponse;
			///<summary>The location of the start of the AutoNotePromptText.</summary>
			public int AutoNoteTextStartPos;
			///<summary>The location of the end position. Ususally the length of AutoNoteTextString except when response text has an AutoNote (Recursion).
			///In this case, we set it to the length of the entire AutoNote.</summary>
			public int AutoNoteTextEndPos;

			public AutoNoteListItem(string autoNotePrompt,string autoNotePromptResponseString,int autoNoteStartPos,int autoNoteEndPos) {
				AutoNotePromptText=autoNotePrompt;
				StrAutoNotePromptResponse=autoNotePromptResponseString;
				AutoNoteTextStartPos=autoNoteStartPos;
				AutoNoteTextEndPos=autoNoteEndPos;
			}
		}
	}
}