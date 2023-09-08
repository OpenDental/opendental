using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormAutoNoteCompose:FormODBase {
		public string StrCompletedNote;
		public string StrMainTextNote;
		///<summary>On load, the UserOdPref that contains the comma delimited list of expanded category DefNums is retrieved from the database.  On close
		///the UserOdPref is updated with the current expanded DefNums.</summary>
		private UserOdPref _userOdPref;
		private List<Def> _listDefs;

		public FormAutoNoteCompose() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormAutoNoteCompose_Load(object sender,EventArgs e) {
			_listDefs=Defs.GetDefsForCategory(DefCat.AutoNoteCats,true);
			_userOdPref=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.AutoNoteExpandedCats).FirstOrDefault();
			FillListTree();
		}

		private void FormAutoNoteCompose_Shown(object sender,EventArgs e) {
			if(!string.IsNullOrEmpty(StrMainTextNote)) {
				PromptForAutoNotes(StrMainTextNote,new List<AutoNoteListItem>());
			}
		}

		private void FillListTree() {
			List<long> listDefNumsExpanded=new List<long>();
			if(_userOdPref!=null) {//if this is the fill on load, the node count will be 0, expanded node list from pref
				listDefNumsExpanded=_userOdPref.ValueString.Split(',').Where(x => x!="" && x!="0").Select(x => PIn.Long(x)).ToList();
			}
			//clear current tree contents
			treeListMain.BeginUpdate();
			treeListMain.SelectedNode=null;
			treeListMain.Nodes.Clear();
			//add categories with all auto notes that are assigned to that category
			List<long> listDefNumsCats=_listDefs.Select(x => x.DefNum).ToList();
			//call recursive function GetNodeAndChildren for any root cat (where def.ItemValue is blank) and any def with invalid parent def num (ItemValue)
			_listDefs.FindAll(x => string.IsNullOrWhiteSpace(x.ItemValue) || !listDefNumsCats.Contains(PIn.Long(x.ItemValue)))
				.ForEach(x => treeListMain.Nodes.Add(CreateNodeAndChildren(x)));//child cats and categorized auto notes added in recursive function
			//add any uncategorized auto notes after the categorized ones and only for the root nodes
			List<AutoNote> listAutoNotes=AutoNotes.GetWhere(x => x.Category==0 || !listDefNumsCats.Contains(x.Category));
			listAutoNotes.ForEach(x => treeListMain.Nodes.Add(new TreeNode(x.AutoNoteName,1,1) { Tag=x }));
			if(listDefNumsExpanded.Count>0) {
				treeListMain.Nodes.OfType<TreeNode>().SelectMany(x => GetNodeAndChildren(x))
					.Where(x => x.Tag is Def && listDefNumsExpanded.Contains(((Def)x.Tag).DefNum)).ToList()
					.ForEach(x => x.Expand());
			}
			treeListMain.EndUpdate();
		}

		///<summary>Recursive function, returns a tree node with all descendants, including all auto note children for this def cat and all children for
		///any cat within this this cat.  Auto Notes that are at the 'root' level (considered uncategorized) have to be added separately after filling the
		///rest of the tree with this function and will be at the bottom of the root node list.</summary>
		private TreeNode CreateNodeAndChildren(Def def) {
			List<TreeNode> listTreeNodesChildren=_listDefs
				.Where(x => !string.IsNullOrWhiteSpace(x.ItemValue) && x.ItemValue==def.DefNum.ToString())
				.Select(CreateNodeAndChildren).ToList();
			listTreeNodesChildren.AddRange(AutoNotes.GetWhere(x => x.Category==def.DefNum)
				.Select(x => new TreeNode(x.AutoNoteName,1,1) { Tag=x }));
			return new TreeNode(def.ItemName,0,0,listTreeNodesChildren.OrderBy(x => x.Tag is AutoNote).ThenBy(x => x.Name).ToArray()) { Tag=def };
		}

		///<summary>Returns a flat list containing this TreeNode and all of its descendant TreeNodes.  Recursive function to walk the full depth of the
		///tree starting at this TreeNode.</summary>
		private List<TreeNode> GetNodeAndChildren(TreeNode treeNode) {
			return new[] { treeNode }.Concat(treeNode.Nodes.OfType<TreeNode>().SelectMany(x => GetNodeAndChildren(x))).ToList();
		}

		private void treeListMain_DoubleClick(object sender,EventArgs e) {
			InsertSelectedAutoNote();
		}

		private void butInsert_Click(object sender,EventArgs e) {
			InsertSelectedAutoNote();
		}

		private void InsertSelectedAutoNote() {
			if(treeListMain.SelectedNode==null || !(treeListMain.SelectedNode.Tag is AutoNote)) {
				return;
			}
			string note=((AutoNote)treeListMain.SelectedNode.Tag).MainText;
			PromptForAutoNotes(note,new List<AutoNoteListItem>());
			if(IsDisposedOrClosed(this)) {
				return;
			}
			//This form could be disposed during auto log off before one of its child windows.
			//If this form is disposed, then all of its child controls will also be disposed, including treeListMain.
			treeListMain.SelectedNode=null;//clear selected node
		}

		private void textMain_TextChanged(object sender,EventArgs e) {
			if(textMain.Text.Trim()!="") {
				butOK.Visible=true;
			}
			else {
				butOK.Visible=false;
			}
		}

		///<summary>Returns the length prompt responses added to textMain.Text.</summary>
		private int PromptForAutoNotes(string noteToParse,List<AutoNoteListItem> listAutoNoteItems) {
			//AutoNote.MainText which should have all text and the prompts.
			string strNote=noteToParse;
			#region Insert Auto Note Text
			//Logic for determining where the note should go based on the users cursor location.
			int selectionStart=textMain.SelectionStart;
			if(selectionStart==0) {//Cursor is at the beginning of the textbox.
				textMain.Text=strNote+textMain.Text;
			}
			else if(selectionStart==textMain.Text.Length-1) {//Cursor at end of textbox
				textMain.Text=textMain.Text+strNote;
			}
			else if(selectionStart==-1) {//If cursor location is unknown just append the text to the end
				textMain.Text=textMain.Text+strNote;
			}
			else {//Cursor is in between text. Insert at the selected position.
				textMain.Text=textMain.Text.Substring(0,selectionStart)+strNote+textMain.Text.Substring(selectionStart);
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
			int matchLoc;//This is the index of the Prompt location in textMain. 
			int startLoc=0;
			int retVal=0;//the length of the notes added to textMain.Text.
			//used to keep track of the start position. This is needed set matchloc. Without this, match loc might find the wrong index.
			Stack<int> stackLoc = new Stack<int>();
			stackLoc.Push(startLoc);
			bool isAutoNote=false;
			//Loop through all valid prompts for the given auto note.
			for(int i=0;i<listMatchesPrompts.Count;i++) {
				//Find prompt location in the text and highlight yellow.
				matchLoc=textMain.Text.IndexOf(listMatchesPrompts[i].Value,startLoc);
				if(matchLoc==-1 || matchLoc>textMain.TextLength) {	//The value wasn't found
					continue;
				}
				startLoc=matchLoc+1;//Add one to look after the last match location.
				textMain.Select(matchLoc,listMatchesPrompts[i].Value.Length);
				textMain.SelectionBackColor=Color.Yellow;
				textMain.SelectionLength=0;
				Application.DoEvents();//refresh the textbox so the yellow will show
				//Holds the PromptName from [Prompt: "PromptName"]
				autoNoteDescript=listMatchesPrompts[i].Value.Substring(9,listMatchesPrompts[i].Value.Length-11);
				autoNoteControl=AutoNoteControls.GetByDescript(autoNoteDescript);//should never be null since we removed nulls above
				strPromptResponse="";
				if(autoNoteControl.ControlType=="Text") {//Response just inserts text. No choosing options here.
					using FormAutoNotePromptText formAutoNotePromptText=new FormAutoNotePromptText(autoNoteDescript);
					formAutoNotePromptText.PromptText=autoNoteControl.ControlLabel;
					formAutoNotePromptText.ResultText=autoNoteControl.ControlOptions;
					isAutoNote=false;
					if(i>0) {
						formAutoNotePromptText.IsGoBack=true;//user can go back if at least one item in the list exist
					}
					if(listAutoNoteItems.Count>i) {
						formAutoNotePromptText.PromptResponseCur=listAutoNoteItems[i].StrAutoNotePromptResponse;//pass the previous response to the form
					}
					formAutoNotePromptText.ShowDialog();
					if(formAutoNotePromptText.DialogResult==DialogResult.Retry) {//user clicked the go back button
						GoBack(i,listAutoNoteItems);
						stackLoc.Pop();//remove the start location
						startLoc=stackLoc.Peek();//set the new start location
						i-=2;
						continue;
					}
					if(formAutoNotePromptText.DialogResult==DialogResult.OK) {
						strPromptResponse=formAutoNotePromptText.ResultText;
						if(listAutoNoteItems.Count>i) {//reponse already exist for this control type. Update it
							//We need to update retVal with the length of the new promptResponse. First, remove the previous prompts length.
							retVal-=listAutoNoteItems[i].AutoNoteTextEndPos;
							//Update the retVal with the new promptResponse.Length.
							retVal+=strPromptResponse.Length;
							//Update the rest of the AutoNoteItem with the new promptResponse.
							listAutoNoteItems[i].StrAutoNotePromptResponse=strPromptResponse;
							listAutoNoteItems[i].AutoNoteTextStartPos=matchLoc;
							listAutoNoteItems[i].AutoNoteTextEndPos=strPromptResponse.Length;
						}
						else {
							//This is a new response. Create a new AutoNoteListItem and add to retVal.
							retVal+=strPromptResponse.Length;
							listAutoNoteItems.Add(new AutoNoteListItem(listMatchesPrompts[i].Value,strPromptResponse,matchLoc,strPromptResponse.Length));//add new response to the list
						}
					}
					else {//User cancelled out of the auto note prompt text form or auto log off.
						ResetTextMain();
						return -1;
					}
				}
				else if(autoNoteControl.ControlType=="OneResponse") {
					using FormAutoNotePromptOneResp formAutoNotePromptOneResp=new FormAutoNotePromptOneResp(autoNoteDescript);
					formAutoNotePromptOneResp.PromptText=autoNoteControl.ControlLabel;
					formAutoNotePromptOneResp.PromptOptions=autoNoteControl.ControlOptions;
					if(i>0) {
						formAutoNotePromptOneResp.IsGoBack=true;//user can go back if at least one item in the list exist
					}
					if(listAutoNoteItems.Count>i) {
						formAutoNotePromptOneResp.PromptResponseCur=listAutoNoteItems[i].StrAutoNotePromptResponse;//pass the previous response if exist to the form.
					}
					formAutoNotePromptOneResp.ShowDialog();
					if(formAutoNotePromptOneResp.DialogResult==DialogResult.Retry) {//user clicked the go back button
						GoBack(i,listAutoNoteItems);
						stackLoc.Pop();//remove the start location
						startLoc=stackLoc.Peek();//set the new start location
						i-=2;
						continue;
					}
					if(formAutoNotePromptOneResp.DialogResult==DialogResult.OK) {
						strPromptResponse=formAutoNotePromptOneResp.ResultText;
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
							listAutoNoteItems[i].AutoNoteTextStartPos=matchLoc;
							listAutoNoteItems[i].AutoNoteTextEndPos=strPromptResponse.Length;
						}
						else if(isAutoNote) {//The response is an auto note. Recursively call this method.
							//Remove the response from textMain.Text. The response was already saved. Since this is an AutoNote, the response does not need to stay 
							//in the textMain.Text since more than likely more prompts will happen after we call the recursive method below.
							string textMainWithoutResponse=textMain.Text.Substring(0,matchLoc)+GetAutoNoteResponseText(strPromptResponse);
							if(textMain.Text.Length>matchLoc+listMatchesPrompts[i].Value.Length) {
								textMainWithoutResponse+=textMain.Text.Substring(matchLoc+listMatchesPrompts[i].Value.Length);
							}
							matchLoc+=GetAutoNoteResponseText(strPromptResponse).Length;
							//set the textMain.Text to the new result. This removes the promptResponse.
							textMain.Text=textMainWithoutResponse;
							textMain.SelectionStart=matchLoc;//This is needed in order for the recursive method call below.
							//Pass in the AutoNotes note in the recursive call. 
							int lengthOfAutoNoteAdded=PromptForAutoNotes(autoNoteString,new List<AutoNoteListItem>());
							if(lengthOfAutoNoteAdded==-1) {
								return -1;
							}
							//When we get back from the recursive method, we need to figure out what was added so we can create the AutoNoteListItem
							strPromptResponse=textMain.Text.Substring(matchLoc,lengthOfAutoNoteAdded);
							//Update the retVal with the new promptResponse.Length.
							retVal+=strPromptResponse.Length;
							//if response already exist, update it.
							if(listAutoNoteItems.Count>i) {
								//We need to update retVal with the length of the new promptResponse. First, remove the previous prompts length.
								retVal-=listAutoNoteItems[i].AutoNoteTextEndPos;
								//Update the rest of the AutoNoteItem with the new promptResponse.
								listAutoNoteItems[i].StrAutoNotePromptResponse=strPromptResponse;//should be the same. Updating just in case.
								listAutoNoteItems[i].AutoNoteTextStartPos=matchLoc;
								listAutoNoteItems[i].AutoNoteTextEndPos=strPromptResponse.Length;//This is the length of what was added.
							}
							else {//New Response. Add a new AutoNoteListItem.
								//Add the response to listAutoNoteItem. This will allow the user to go back. Make the end position equal to the length of the AutoNote
								listAutoNoteItems.Add(new AutoNoteListItem(listMatchesPrompts[i].Value,strPromptResponse,matchLoc,strPromptResponse.Length));
							}
						}
						else {
							//This is a new response. Create a new AutoNoteListItem and add to retVal.
							retVal+=strPromptResponse.Length;
							listAutoNoteItems.Add(new AutoNoteListItem(listMatchesPrompts[i].Value,strPromptResponse,matchLoc,strPromptResponse.Length));//add new response to the list
						}
					}
					else {//User cancelled out of the auto note response form or auto log off.
						ResetTextMain();
						return -1;
					}
				}
				else if(autoNoteControl.ControlType=="MultiResponse") {
					using FormAutoNotePromptMultiResp formAutoNotePromptMultiResp=new FormAutoNotePromptMultiResp(autoNoteDescript);
					formAutoNotePromptMultiResp.PromptText=autoNoteControl.ControlLabel;
					formAutoNotePromptMultiResp.PromptOptions=autoNoteControl.ControlOptions;
					isAutoNote=false;
					if(i>0) {
						formAutoNotePromptMultiResp.IsGoBack=true;//user can go back if at least one item in the list exist
					}
					if(listAutoNoteItems.Count>i) {
						formAutoNotePromptMultiResp.PromptResponseCur=listAutoNoteItems[i].StrAutoNotePromptResponse;//pass the previous response if exist to the form.
					}
					formAutoNotePromptMultiResp.ShowDialog();
					if(formAutoNotePromptMultiResp.DialogResult==DialogResult.Retry) {//user clicked the go back button
						GoBack(i,listAutoNoteItems);
						stackLoc.Pop();//remove the start location
						startLoc=stackLoc.Peek();//set the new start location
						i-=2;
						continue;
					}
					if(formAutoNotePromptMultiResp.DialogResult==DialogResult.OK) {
						strPromptResponse=formAutoNotePromptMultiResp.ResultText;
						if(listAutoNoteItems.Count>i) {//reponse already exist for this control type. Update it
							//We need to update retval with the length of the new promptresponse.First, remove the previous prompts length.
							retVal-=listAutoNoteItems[i].AutoNoteTextEndPos;
							//update the retval with the new promptResponse.Length.
							retVal+=strPromptResponse.Length;
							//Update the rest of the AutoNoteItem with the new promptResponse.
							listAutoNoteItems[i].StrAutoNotePromptResponse=strPromptResponse;
							listAutoNoteItems[i].AutoNoteTextStartPos=matchLoc;
							listAutoNoteItems[i].AutoNoteTextEndPos=strPromptResponse.Length;
						}
						else {
							//This is a new response. Create a new AutoNoteListItem and add to retVal.
							retVal+=strPromptResponse.Length;
							listAutoNoteItems.Add(new AutoNoteListItem(listMatchesPrompts[i].Value,strPromptResponse,matchLoc,strPromptResponse.Length));//add new response to the list
						}
					}
					else {//User cancelled out of the auto note response form or auto log off.
						ResetTextMain();
						return -1;
					}
				}
				string strResults=textMain.Text.Substring(0,matchLoc)+strPromptResponse;
				if(!isAutoNote && textMain.Text.Length > matchLoc+listMatchesPrompts[i].Value.Length) {
					strResults+=textMain.Text.Substring(matchLoc+listMatchesPrompts[i].Value.Length);
				}
				else if(isAutoNote && textMain.Text.Length > matchLoc+strPromptResponse.Length) {
					//if any of the prompts had an AutoNote and textmain has more text, add the rest of textMain. 
					strResults+=textMain.Text.Substring(matchLoc+strPromptResponse.Length);
					//update the startLoc to include the promptResponse of the AutoNote.
					startLoc+=strPromptResponse.Length-1;
				}
				textMain.Text=strResults;
				ResetTextMain();
				if(string.IsNullOrEmpty(strPromptResponse)) {
					//if prompt was removed, add the previous start location onto the stack. 
					startLoc=stackLoc.Peek();
				}
				stackLoc.Push(startLoc);
				Application.DoEvents();//refresh the textbox
			}
			ResetTextMain();
			listAutoNoteItems.Clear();
			return retVal;
		}

		private void ResetTextMain() {
			if(IsDisposedOrClosed(this)) {
				//This form could be disposed during auto log off before one of its child windows.
				//If this form is disposed, then all of its child controls will also be disposed, including textMain.
				return;
			}
			textMain.SelectAll();
			textMain.SelectionBackColor=Color.White;
			textMain.Select(textMain.Text.Length,0);
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
			StrCompletedNote=textMain.Text.Replace("\n","\r\n");
			DialogResult=DialogResult.OK;
		}

		///<summary>Removes previous response from the listAutoNoteItems and inserts the AutoNote prompt into textMain.</summary>
		private void GoBack(int pos,List<AutoNoteListItem> listAutoNoteListItems) {
			textMain.SelectAll();
			textMain.SelectionBackColor=Color.White;
			textMain.Text=textMain.Text.Remove(listAutoNoteListItems[pos-1].AutoNoteTextStartPos,listAutoNoteListItems[pos-1].AutoNoteTextEndPos)
				.Insert(listAutoNoteListItems[pos-1].AutoNoteTextStartPos,listAutoNoteListItems[pos-1].AutoNotePromptText);
			if(listAutoNoteListItems[pos-1].StrAutoNotePromptResponse==listAutoNoteListItems[pos-1].AutoNotePromptText) {
				listAutoNoteListItems[pos-1].StrAutoNotePromptResponse="";
			}
			Application.DoEvents();
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormAutoNoteCompose_FormClosing(object sender,FormClosingEventArgs e) {
			//store the current node expanded state for this user
			List<long> listDefNumsExpanded=treeListMain.Nodes.OfType<TreeNode>().SelectMany(x => GetNodeAndChildren(x))
				.Where(x => x.Tag is Def && x.IsExpanded).Select(x => ((Def)x.Tag).DefNum).Where(x => x>0).ToList();
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