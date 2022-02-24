using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormAutoNoteCompose:FormODBase {
		public string CompletedNote;
		public string MainTextNote;
		///<summary>On load, the UserOdPref that contains the comma delimited list of expanded category DefNums is retrieved from the database.  On close
		///the UserOdPref is updated with the current expanded DefNums.</summary>
		private UserOdPref _userOdCurPref;
		private List<Def> _listAutoNoteCatDefs;

		public FormAutoNoteCompose() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormAutoNoteCompose_Load(object sender,EventArgs e) {
			_listAutoNoteCatDefs=Defs.GetDefsForCategory(DefCat.AutoNoteCats,true);
			_userOdCurPref=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.AutoNoteExpandedCats).FirstOrDefault();
			FillListTree();
		}

		private void FormAutoNoteCompose_Shown(object sender,EventArgs e) {
			if(!string.IsNullOrEmpty(MainTextNote)) {
				PromptForAutoNotes(MainTextNote,new List<AutoNoteListItem>());
			}
		}

		private void FillListTree() {
			List<long> listExpandedDefNums=new List<long>();
			if(_userOdCurPref!=null) {//if this is the fill on load, the node count will be 0, expanded node list from pref
				listExpandedDefNums=_userOdCurPref.ValueString.Split(',').Where(x => x!="" && x!="0").Select(x => PIn.Long(x)).ToList();
			}
			//clear current tree contents
			treeListMain.BeginUpdate();
			treeListMain.SelectedNode=null;
			treeListMain.Nodes.Clear();
			//add categories with all auto notes that are assigned to that category
			List<long> listCatDefNums=_listAutoNoteCatDefs.Select(x => x.DefNum).ToList();
			//call recursive function GetNodeAndChildren for any root cat (where def.ItemValue is blank) and any def with invalid parent def num (ItemValue)
			_listAutoNoteCatDefs.FindAll(x => string.IsNullOrWhiteSpace(x.ItemValue) || !listCatDefNums.Contains(PIn.Long(x.ItemValue)))
				.ForEach(x => treeListMain.Nodes.Add(CreateNodeAndChildren(x)));//child cats and categorized auto notes added in recursive function
			//add any uncategorized auto notes after the categorized ones and only for the root nodes
			AutoNotes.GetWhere(x => x.Category==0 || !listCatDefNums.Contains(x.Category))
				.ForEach(x => treeListMain.Nodes.Add(new TreeNode(x.AutoNoteName,1,1) { Tag=x }));
			if(listExpandedDefNums.Count>0) {
				treeListMain.Nodes.OfType<TreeNode>().SelectMany(x => GetNodeAndChildren(x))
					.Where(x => x.Tag is Def && listExpandedDefNums.Contains(((Def)x.Tag).DefNum)).ToList()
					.ForEach(x => x.Expand());
			}
			treeListMain.EndUpdate();
		}

		///<summary>Recursive function, returns a tree node with all descendants, including all auto note children for this def cat and all children for
		///any cat within this this cat.  Auto Notes that are at the 'root' level (considered uncategorized) have to be added separately after filling the
		///rest of the tree with this function and will be at the bottom of the root node list.</summary>
		private TreeNode CreateNodeAndChildren(Def defCur) {
			List<TreeNode> listChildNodes=_listAutoNoteCatDefs
				.Where(x => !string.IsNullOrWhiteSpace(x.ItemValue) && x.ItemValue==defCur.DefNum.ToString())
				.Select(CreateNodeAndChildren).ToList();
			listChildNodes.AddRange(AutoNotes.GetWhere(x => x.Category==defCur.DefNum)
				.Select(x => new TreeNode(x.AutoNoteName,1,1) { Tag=x }));
			return new TreeNode(defCur.ItemName,0,0,listChildNodes.OrderBy(x => x.Tag is AutoNote).ThenBy(x => x.Name).ToArray()) { Tag=defCur };
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
			if(!IsDisposedOrClosed(this)) {
				//This form could be disposed during auto log off before one of its child windows.
				//If this form is disposed, then all of its child controls will also be disposed, including treeListMain.
				treeListMain.SelectedNode=null;//clear selected node
			}
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
		private int PromptForAutoNotes(string noteToParse,List<AutoNoteListItem> listAutoNoteItem) {
			//AutoNote.MainText which should have all text and the prompts.
			string note=noteToParse;
			#region Insert Auto Note Text
			//Logic for determining where the note should go based on the users cursor location.
			int selectionStart=textMain.SelectionStart;
			if(selectionStart==0) {//Cursor is at the beginning of the textbox.
				textMain.Text=note+textMain.Text;
			}
			else if(selectionStart==textMain.Text.Length-1) {//Cursor at end of textbox
				textMain.Text=textMain.Text+note;
			}
			else if(selectionStart==-1) {//If cursor location is unknown just append the text to the end
				textMain.Text=textMain.Text+note;
			}
			else {//Cursor is in between text. Insert at the selected position.
				textMain.Text=textMain.Text.Substring(0,selectionStart)+note+textMain.Text.Substring(selectionStart);
			}
			#endregion
			//List of prompts for the auto note
			List<AutoNoteControl> prompts=new List<AutoNoteControl>();
			//Prompts are stored in the form [Prompt: "PromptName"]
			List<Match> listPrompts=AutoNoteControls.GetPrompts(note);
			//Remove all matched prompts that do not exist in the database.
			listPrompts.RemoveAll(x => AutoNoteControls.GetByDescript(x.Value.Substring(9,x.Value.Length-11))==null);
			//Holds the PromptName from [Prompt: "PromptName"]
			string autoNoteDescript;
			AutoNoteControl control;
			string promptResponse;
			int matchloc;//This is the index of the Prompt location in textMain. 
			int startLoc=0;
			int retVal=0;//the length of the notes added to textMain.Text.
			//used to keep track of the start position. This is needed set matchloc. Without this, match loc might find the wrong index.
			Stack<int> stackLoc = new Stack<int>();
			stackLoc.Push(startLoc);
			bool isAutoNote=false;
			//Loop through all valid prompts for the given auto note.
			for(int i=0;i<listPrompts.Count;i++) {
				//Find prompt location in the text and highlight yellow.
				matchloc=textMain.Text.IndexOf(listPrompts[i].Value,startLoc);
				if(matchloc==-1 || matchloc>textMain.TextLength) {	//The value wasn't found
					continue;
				}
				startLoc=matchloc+1;//Add one to look after the last match location.
				textMain.Select(matchloc,listPrompts[i].Value.Length);
				textMain.SelectionBackColor=Color.Yellow;
				textMain.SelectionLength=0;
				Application.DoEvents();//refresh the textbox so the yellow will show
				//Holds the PromptName from [Prompt: "PromptName"]
				autoNoteDescript=listPrompts[i].Value.Substring(9,listPrompts[i].Value.Length-11);
				control=AutoNoteControls.GetByDescript(autoNoteDescript);//should never be null since we removed nulls above
				promptResponse="";
				if(control.ControlType=="Text") {//Response just inserts text. No choosing options here.
					using FormAutoNotePromptText FormT=new FormAutoNotePromptText(autoNoteDescript);
					FormT.PromptText=control.ControlLabel;
					FormT.ResultText=control.ControlOptions;
					isAutoNote=false;
					if(i>0) {
						FormT.IsGoBack=true;//user can go back if at least one item in the list exist
					}
					if(listAutoNoteItem.Count>i) {
						FormT.CurPromptResponse=listAutoNoteItem[i].AutoNotePromptResponseString;//pass the previous response to the form
					}
					FormT.ShowDialog();
					if(FormT.DialogResult==DialogResult.Retry) {//user clicked the go back button
						GoBack(i,listAutoNoteItem);
						stackLoc.Pop();//remove the start location
						startLoc=stackLoc.Peek();//set the new start location
						i-=2;
						continue;
					}
					if(FormT.DialogResult==DialogResult.OK) {
						promptResponse=FormT.ResultText;
						if(listAutoNoteItem.Count>i) {//reponse already exist for this control type. Update it
							//We need to update retVal with the length of the new promptResponse. First, remove the previous prompts length.
							retVal-=listAutoNoteItem[i].AutoNoteTextEndPos;
							//Update the retVal with the new promptResponse.Length.
							retVal+=promptResponse.Length;
							//Update the rest of the AutoNoteItem with the new promptResponse.
							listAutoNoteItem[i].AutoNotePromptResponseString=promptResponse;
							listAutoNoteItem[i].AutoNoteTextStartPos=matchloc;
							listAutoNoteItem[i].AutoNoteTextEndPos=promptResponse.Length;
						}
						else {
							//This is a new response. Create a new AutoNoteListItem and add to retVal.
							retVal+=promptResponse.Length;
							listAutoNoteItem.Add(new AutoNoteListItem(listPrompts[i].Value,promptResponse,matchloc,promptResponse.Length));//add new response to the list
						}
					}
					else {//User cancelled out of the auto note prompt text form or auto log off.
						ResetTextMain();
						return -1;
					}
				}
				else if(control.ControlType=="OneResponse") {
					using FormAutoNotePromptOneResp FormOR=new FormAutoNotePromptOneResp(autoNoteDescript);
					FormOR.PromptText=control.ControlLabel;
					FormOR.PromptOptions=control.ControlOptions;
					if(i>0) {
						FormOR.IsGoBack=true;//user can go back if at least one item in the list exist
					}
					if(listAutoNoteItem.Count>i) {
						FormOR.CurPromptResponse=listAutoNoteItem[i].AutoNotePromptResponseString;//pass the previous response if exist to the form.
					}
					FormOR.ShowDialog();
					if(FormOR.DialogResult==DialogResult.Retry) {//user clicked the go back button
						GoBack(i,listAutoNoteItem);
						stackLoc.Pop();//remove the start location
						startLoc=stackLoc.Peek();//set the new start location
						i-=2;
						continue;
					}
					if(FormOR.DialogResult==DialogResult.OK) {
						promptResponse=FormOR.ResultText;
						//The promptResponse will have the AutoNoteName in the format "Auto Note Response : {AutoNoteName}". Use the name to get the note that 
						//will be used in the recursive call below.
						string autoNoteString="";
						isAutoNote=false;
						string autoNoteName=GetAutoNoteName(promptResponse);
						if(!string.IsNullOrEmpty(autoNoteName)) {
							isAutoNote=true;
							//For some reason the Auto Note string always contains a new line and a return character at the end of the note. Must be trimmed
							autoNoteString=AutoNotes.GetByTitle(autoNoteName).TrimEnd('\n').TrimEnd('\r');//Returns empty string If no AutoNote is found. 
						}
						if(listAutoNoteItem.Count>i && !isAutoNote) {//The response already exist for this control type and it is note an AutoNote. Update it
							//We need to update retval with the length of the new promptResponse. First, remove the previous promptResponse length.
							retVal-=listAutoNoteItem[i].AutoNoteTextEndPos;
							//Update the retval with the new promptResponse.Length.
							retVal+=promptResponse.Length;
							//Update the rest of the AutoNoteItem with the new promptResponse.
							listAutoNoteItem[i].AutoNotePromptResponseString=promptResponse;
							listAutoNoteItem[i].AutoNoteTextStartPos=matchloc;
							listAutoNoteItem[i].AutoNoteTextEndPos=promptResponse.Length;
						}
						else if(isAutoNote) {//The response is an auto note. Recursively call this method.
							//Remove the response from textMain.Text. The response was already saved. Since this is an AutoNote, the response does not need to stay 
							//in the textMain.Text since more than likely more prompts will happen after we call the recursive method below.
							string textMainWithoutResponse=textMain.Text.Substring(0,matchloc)+GetAutoNoteResponseText(promptResponse);
							if(textMain.Text.Length>matchloc+listPrompts[i].Value.Length) {
								textMainWithoutResponse+=textMain.Text.Substring(matchloc+listPrompts[i].Value.Length);
							}
							matchloc+=GetAutoNoteResponseText(promptResponse).Length;
							//set the textMain.Text to the new result. This removes the promptResponse.
							textMain.Text=textMainWithoutResponse;
							textMain.SelectionStart=matchloc;//This is needed in order for the recursive method call below.
							//Pass in the AutoNotes note in the recursive call. 
							int lenthOfAutoNoteAdded=PromptForAutoNotes(autoNoteString,new List<AutoNoteListItem>());
							if(lenthOfAutoNoteAdded==-1) {
								return -1;
							}
							//When we get back from the recursive method, we need to figure out what was added so we can create the AutoNoteListItem
							promptResponse=textMain.Text.Substring(matchloc,lenthOfAutoNoteAdded);
							//Update the retVal with the new promptResponse.Length.
							retVal+=promptResponse.Length;
							//if response already exist, update it.
							if(listAutoNoteItem.Count>i) {
								//We need to update retVal with the length of the new promptResponse. First, remove the previous prompts length.
								retVal-=listAutoNoteItem[i].AutoNoteTextEndPos;
								//Update the rest of the AutoNoteItem with the new promptResponse.
								listAutoNoteItem[i].AutoNotePromptResponseString=promptResponse;//should be the same. Updating just in case.
								listAutoNoteItem[i].AutoNoteTextStartPos=matchloc;
								listAutoNoteItem[i].AutoNoteTextEndPos=promptResponse.Length;//This is the length of what was added.
							}
							else {//New Response. Add a new AutoNoteListItem.
								//Add the response to listAutoNoteItem. This will allow the user to go back. Make the end position equal to the length of the AutoNote
								listAutoNoteItem.Add(new AutoNoteListItem(listPrompts[i].Value,promptResponse,matchloc,promptResponse.Length));
							}
						}
						else {
							//This is a new response. Create a new AutoNoteListItem and add to retVal.
							retVal+=promptResponse.Length;
							listAutoNoteItem.Add(new AutoNoteListItem(listPrompts[i].Value,promptResponse,matchloc,promptResponse.Length));//add new response to the list
						}
					}
					else {//User cancelled out of the auto note response form or auto log off.
						ResetTextMain();
						return -1;
					}
				}
				else if(control.ControlType=="MultiResponse") {
					using FormAutoNotePromptMultiResp FormMR=new FormAutoNotePromptMultiResp(autoNoteDescript);
					FormMR.PromptText=control.ControlLabel;
					FormMR.PromptOptions=control.ControlOptions;
					isAutoNote=false;
					if(i>0) {
						FormMR.IsGoBack=true;//user can go back if at least one item in the list exist
					}
					if(listAutoNoteItem.Count>i) {
						FormMR.CurPromptResponse=listAutoNoteItem[i].AutoNotePromptResponseString;//pass the previous response if exist to the form.
					}
					FormMR.ShowDialog();
					if(FormMR.DialogResult==DialogResult.Retry) {//user clicked the go back button
						GoBack(i,listAutoNoteItem);
						stackLoc.Pop();//remove the start location
						startLoc=stackLoc.Peek();//set the new start location
						i-=2;
						continue;
					}
					if(FormMR.DialogResult==DialogResult.OK) {
						promptResponse=FormMR.ResultText;
						if(listAutoNoteItem.Count>i) {//reponse already exist for this control type. Update it
							//We need to update retval with the length of the new promptresponse.First, remove the previous prompts length.
							retVal-=listAutoNoteItem[i].AutoNoteTextEndPos;
							//update the retval with the new promptResponse.Length.
							retVal+=promptResponse.Length;
							//Update the rest of the AutoNoteItem with the new promptResponse.
							listAutoNoteItem[i].AutoNotePromptResponseString=promptResponse;
							listAutoNoteItem[i].AutoNoteTextStartPos=matchloc;
							listAutoNoteItem[i].AutoNoteTextEndPos=promptResponse.Length;
						}
						else {
							//This is a new response. Create a new AutoNoteListItem and add to retVal.
							retVal+=promptResponse.Length;
							listAutoNoteItem.Add(new AutoNoteListItem(listPrompts[i].Value,promptResponse,matchloc,promptResponse.Length));//add new response to the list
						}
					}
					else {//User cancelled out of the auto note response form or auto log off.
						ResetTextMain();
						return -1;
					}
				}
				string resultstr=textMain.Text.Substring(0,matchloc)+promptResponse;
				if(!isAutoNote && textMain.Text.Length > matchloc+listPrompts[i].Value.Length) {
					resultstr+=textMain.Text.Substring(matchloc+listPrompts[i].Value.Length);
				}
				else if(isAutoNote && textMain.Text.Length > matchloc+promptResponse.Length) {
					//if any of the prompts had an AutoNote and textmain has more text, add the rest of textMain. 
					resultstr+=textMain.Text.Substring(matchloc+promptResponse.Length);
					//update the startLoc to include the promptResponse of the AutoNote.
					startLoc+=promptResponse.Length-1;
				}
				textMain.Text=resultstr;
				ResetTextMain();
				if(string.IsNullOrEmpty(promptResponse)) {
					//if prompt was removed, add the previous start location onto the stack. 
					startLoc=stackLoc.Peek();
				}
				stackLoc.Push(startLoc);
				Application.DoEvents();//refresh the textbox
			}
			ResetTextMain();
			listAutoNoteItem.Clear();
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

		///<summary>Returns the AutoNoteName from the passed in promptRepsonse. Returns empty string if no valid AutoNote is found.</summary>
		private string GetAutoNoteName(string promptResponse) {
			string retVal="";
			Stack<int> stackBrackets=new Stack<int>();
			//The AutoNoteResponseText should be in format "Auto Note Response Text : {AutoNoteName}". Go through each of the charactors in promptResponse
			//and find each of the possible AutoNote names. We need to do this in case the AutoNote name has brackets("{,}") in the name. 
			for(int posCur=0;posCur<promptResponse.Length;posCur++) {
				if(promptResponse[posCur]=='{') {
					stackBrackets.Push(posCur);//add the position of the '{' to the stack.
					continue;
				}
				if(promptResponse[posCur]!='}' || stackBrackets.Count()==0) {//continue if the the stack does not have an open '{', or this is not a '}'
					continue;
				}
				//The posOpenBracket will include the '{'. We will have to remove it.
				int posOpenBracket=stackBrackets.Peek();
				//Get the length of the possible autonote. The length will include the closing '}'. We will also need to remove it.
				int length=posCur-posOpenBracket;
				if(length<1) {
					stackBrackets.Pop();
					continue;
				}
				//Get string of possible AutoNoteName. Remove the bracket from the beginning and end. 
				string autoNoteName=promptResponse.Substring(posOpenBracket+1,length-1);
				if(!string.IsNullOrEmpty(autoNoteName) && AutoNotes.IsValidAutoNote(autoNoteName)) {
					retVal=autoNoteName;
					break;
				}
				//no match found. Remove position from stack and continue.
				stackBrackets.Pop();
			}
			return retVal;//could be empty string if no valid autonote name is found
		}

		private void butOK_Click(object sender,EventArgs e) {
			CompletedNote=textMain.Text.Replace("\n","\r\n");
			DialogResult=DialogResult.OK;
		}

		///<summary>Removes previous response from the listAutoNoteItems and inserts the AutoNote prompt into textMain.</summary>
		private void GoBack(int pos,List<AutoNoteListItem> listAutoNoteItem) {
			textMain.SelectAll();
			textMain.SelectionBackColor=Color.White;
			textMain.Text=textMain.Text.Remove(listAutoNoteItem[pos-1].AutoNoteTextStartPos,listAutoNoteItem[pos-1].AutoNoteTextEndPos)
				.Insert(listAutoNoteItem[pos-1].AutoNoteTextStartPos,listAutoNoteItem[pos-1].AutoNotePromptText);
			if(listAutoNoteItem[pos-1].AutoNotePromptResponseString==listAutoNoteItem[pos-1].AutoNotePromptText) {
				listAutoNoteItem[pos-1].AutoNotePromptResponseString="";
			}
			Application.DoEvents();
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormAutoNoteCompose_FormClosing(object sender,FormClosingEventArgs e) {
			//store the current node expanded state for this user
			List<long> listExpandedDefNums=treeListMain.Nodes.OfType<TreeNode>().SelectMany(x => GetNodeAndChildren(x))
				.Where(x => x.Tag is Def && x.IsExpanded).Select(x => ((Def)x.Tag).DefNum).Where(x => x>0).ToList();
			if(_userOdCurPref==null) {
				UserOdPrefs.Insert(new UserOdPref() {
					UserNum=Security.CurUser.UserNum,
					FkeyType=UserOdFkeyType.AutoNoteExpandedCats,
					ValueString=string.Join(",",listExpandedDefNums)
				});
			}
			else {
				_userOdCurPref.ValueString=string.Join(",",listExpandedDefNums);
				UserOdPrefs.Update(_userOdCurPref);
			}
		}

		///<summary>This holds AutoNote prompt responses. Mostly used when using the back button within the Prompt Responses.</summary>
		private class AutoNoteListItem {
			///<summary>The AutoNote Prompt Text. The "PromptName" from the following example [Prompt: "PromptName"].</summary>
			public string AutoNotePromptText;
			///<summary>The Prompt response string.</summary>
			public string AutoNotePromptResponseString;
			///<summary>The location of the start of the AutoNotePromptText.</summary>
			public int AutoNoteTextStartPos;
			///<summary>The location of the end position. Ususally the length of AutoNoteTextString except when response text has an AutoNote (Recursion).
			///In this case, we set it to the length of the entire AutoNote.</summary>
			public int AutoNoteTextEndPos;

			public AutoNoteListItem(string autoNotePrompt,string autoNotePromptResponseString,int autoNoteStartPos,int autoNoteEndPos) {
				AutoNotePromptText=autoNotePrompt;
				AutoNotePromptResponseString=autoNotePromptResponseString;
				AutoNoteTextStartPos=autoNoteStartPos;
				AutoNoteTextEndPos=autoNoteEndPos;
			}
		}
	}
}