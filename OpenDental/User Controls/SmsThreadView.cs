using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using OpenDentBusiness;
using CodeBase;
using System.Text.RegularExpressions;

namespace OpenDental {
	///<summary>For SMS Text Messaging.  Used in the Text Messaging window to display an SMS message thread much like a cell phone.
	///Since users are used to seeing text message threads on cell phones, this control will be intuitive to users.</summary>
	public partial class SmsThreadView:UserControl {

		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		private List<SmsThreadMessage> _listSmsThreadMessages=null;
		///<summary>Keeps track of what page we're on.</summary>
		private int _smsThreadPage=1;
		///<summary>Set this value externally before showing the control.</summary>
		public List<SmsThreadMessage> ListSmsThreadMessages {
			get {
				return _listSmsThreadMessages;
			}
			set {
				_listSmsThreadMessages=value;
				FillMessageThread();
			}
		}
		///<summary>The number of text messages to display per page.</summary>
		public int CountMessagesToDisplay=100;
		///<summary>The list of messages to display on this page.</summary>
		private List<SmsThreadMessage> _listSmsThreadToDisplay;

		public SmsThreadView() {
			InitializeComponent();
			DoubleBuffered=true;
			SetDoubleBuffered(panelScroll,true);
		}

		private void DisposeChildrenRecursive(Control control,bool doDisposeParent) {
			if(control.HasChildren) {
				for(int i=control.Controls.Count-1;i>=0;i--) {
					ODException.SwallowAnyException(() => DisposeChildrenRecursive(control.Controls[i],doDisposeParent:true));
				}
			}
			if(doDisposeParent) {
				ODException.SwallowAnyException(() => control.Dispose());
			}
		}

		private void FillMessageThread() {
			List<Control> listControls=panelScroll.Controls.Cast<Control>().ToList();
			listControls.ForEach(x => x.Parent=null);
			Point location=panelScroll.Location;
			panelScroll.Dispose();//Clears the empty scroll space.
			panelScroll=new Panel();
			panelScroll.Location=location;
			panelScroll.Size=new Size(Width,Height-panelScroll.Location.Y);
			panelScroll.Anchor=AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
			panelScroll.AutoScroll=true;
			LayoutManager.Add(panelScroll,this);
			Invalidate();
			if(_listSmsThreadMessages==null || _listSmsThreadMessages.Count<1) {
				listControls.ForEach(x => x.Dispose());
				panelNavigation.Visible=false;
				LayoutManager.MoveLocation(panelScroll,new Point(0,0));
				return;
			}
			int bodyWidth=GetBodyWidth();
			int verticalPadding=5;
			int horizontalMargin=(int)(bodyWidth*0.02);
			int y=0;
			Control controlHighlighted=null;
			panelScroll.SuspendLayout();
			BuildListMessages();
			//Remove message controls for messages which have been removed from the given list of messages.
			for(int i=listControls.Count-1;i>=0;i--) {
				Control control=listControls[i];//Freeze the variable so we can remove it before disposing it below.
				if(!control.Name.Contains("_")) {
					continue;
				}
				string id=control.Name.Substring(control.Name.IndexOf("_")+1);
				if(!_listSmsThreadToDisplay.Exists(x => x.ID==id)) {
					//LayoutManager.Remove() currently only supports tab pages.
					listControls.Remove(control);
					control.Dispose();//This control does not have any child controls.
				}
			}
			foreach(Control control in listControls) {
				LayoutManager.Add(control,panelScroll);
			}
			//Find last sms sent to mobile
			SmsThreadMessage smsThreadMessageLastSent=_listSmsThreadToDisplay.FindAll(x => !x.IsAlignedLeft).LastOrDefault();
			OpenDental.Drawing.Graphics g=OpenDental.Drawing.Graphics.MeasureBegin();
			//Loop through and update existing control sizes, text, borders, etc.  Add new controls for messages not already represented.
			for(int i=0;i<_listSmsThreadToDisplay.Count;i++) {
				SmsThreadMessage smsThreadMessage=_listSmsThreadToDisplay[i];
				y+=verticalPadding;
				Label labelMessageHeader=new Label();
				SetDoubleBuffered(labelMessageHeader,true);
				//labelMessageHeader.MouseWheel+=MouseWheel_Scroll;//Labels automatically pass their scroll events through to their parent controls.
				labelMessageHeader.Name="labelMessageHeader_"+smsThreadMessage.ID;
				labelMessageHeader.Text=((smsThreadMessage.UserName==null)?"":(smsThreadMessage.UserName+"  "))+smsThreadMessage.MsgDateTime.ToString();
				Size size=TextRenderer.MeasureText(labelMessageHeader.Text,panelScroll.Font,
					new Size(bodyWidth,Int32.MaxValue),TextFormatFlags.WordBreak);
				size.Height+=10;//Extra vertical padding to ensure that the text will fit when including the border.
				if(smsThreadMessage.IsAiMessage && smsThreadMessage.Tag is WebChatMessage msg) {
					string flagButtonText=msg.NeedsFollowUp?"Unflag":"Flag";
					UI.Button flagButton=new UI.Button {
						Name="aiFlagButton_"+smsThreadMessage.ID,
						Location=new Point(size.Width+3,y),//Align bottoms of UI 
						Text=flagButtonText,
						Font=panelScroll.Font,
						Height=size.Height,
						AutoSize=false,
						Tag=msg,
					};
					flagButton.Click+=FlagButton_Click;
					SetDoubleBuffered(flagButton,true);
					AddOrUpdatePanelScrollChildControl(flagButton);
				}
				if(smsThreadMessage.IsAlignedLeft) {
					labelMessageHeader.TextAlign=ContentAlignment.BottomLeft;
				}
				else {//Aligned right
					labelMessageHeader.TextAlign=ContentAlignment.BottomRight;
				}
				labelMessageHeader.Width=bodyWidth;
				labelMessageHeader.Height=size.Height;
				labelMessageHeader.Location=new Point(0,y);
				labelMessageHeader.Font=panelScroll.Font;
				AddOrUpdatePanelScrollChildControl(labelMessageHeader);
				y+=size.Height+1;
				Label labelMessageStatus=new Label();
				SetDoubleBuffered(labelMessageStatus,true);
				if(smsThreadMessage.SmsDeliveryStatusCur!=SmsDeliveryStatus.None && !_listSmsThreadToDisplay[i].IsAlignedLeft){
					labelMessageStatus.Name="labelMessageStatus_"+smsThreadMessage.ID;
					labelMessageStatus.ForeColor=System.Drawing.Color.Gray;
					//Display label on every failed sms
					if(smsThreadMessage.SmsDeliveryStatusCur==SmsDeliveryStatus.FailWithCharge || smsThreadMessage.SmsDeliveryStatusCur==SmsDeliveryStatus.FailNoCharge){
						labelMessageStatus.Text="Not Delivered";
						labelMessageStatus.ForeColor=System.Drawing.Color.Red;
					}
					//Display "delivered" label if this message is the most recently delivered message.
					else if(smsThreadMessage.SmsDeliveryStatusCur==SmsDeliveryStatus.DeliveryConf && _listSmsThreadToDisplay[i].ID==smsThreadMessageLastSent.ID) {
						labelMessageStatus.Text="Delivered";
					}
					//Display "sent" label if this message is the most recently sent message.
					else if((smsThreadMessage.SmsDeliveryStatusCur==SmsDeliveryStatus.DeliveryUnconf || smsThreadMessage.SmsDeliveryStatusCur==SmsDeliveryStatus.Pending) && _listSmsThreadToDisplay[i].ID==smsThreadMessageLastSent.ID) {
						labelMessageStatus.Text="Sent";
					}
					labelMessageStatus.Font=new Font("Microsoft Sans Serif", 7);
					labelMessageStatus.TextAlign=ContentAlignment.MiddleRight;
					size=TextRenderer.MeasureText(labelMessageStatus.Text,labelMessageStatus.Font,
						new Size(bodyWidth,Int32.MaxValue),TextFormatFlags.WordBreak);
					labelMessageStatus.Width=bodyWidth;
					labelMessageStatus.Height=size.Height+2;//Extra vertical padding to ensure that the text will fit when including the border.
					//Display 'Not delivered' before text message
					if(labelMessageStatus.Text=="Not Delivered"){
						labelMessageStatus.Location=new Point(0,y);
						AddOrUpdatePanelScrollChildControl(labelMessageStatus);
						y+=labelMessageStatus.Height;
					}
				}
				RichTextBox richTextBoxMessage=new RichTextBox();
				richTextBoxMessage.DetectUrls=true;
				richTextBoxMessage.ScrollBars=RichTextBoxScrollBars.None;
				richTextBoxMessage.LinkClicked+=TextBoxMessage_LinkClicked;
				richTextBoxMessage.MouseDown+=TextBoxMessage_MouseDown;
				richTextBoxMessage.SelectionBackColor=Color.Yellow;
				SetDoubleBuffered(richTextBoxMessage,true);
				richTextBoxMessage.MouseWheel+=MouseWheel_Scroll;//Textboxes handle their own scroll events, because they have their own scroll bars.
				richTextBoxMessage.Font=panelScroll.Font;
				richTextBoxMessage.BackColor=smsThreadMessage.BackColor;
				if(smsThreadMessage.IsImportant) {
					//richTextBoxMessage.ForeColor=Color.Red;
				}
				richTextBoxMessage.Name="textSmsThreadMsg_"+smsThreadMessage.ID;
				richTextBoxMessage.BorderStyle=BorderStyle.None;
				richTextBoxMessage.Multiline=true;
				string message=smsThreadMessage.Message.Trim();//Trim leading and trailing whitespace for 2 reasons: 1) Prevents scrolling within the textbox when trying to highlight and copy the text 2) Matches the web view behavior.
				if(smsThreadMessage.IsAiMessage) {
					message=CleanAiResponse(message);
				}
				richTextBoxMessage.Text=message.Replace("\r\n","\n").Replace("\n","\r\n");//Normalize \n coming from RichTextBox to \r\n for TextBox.
				//Each message wraps horizontally.
				OpenDental.Drawing.Font od_Font=new OpenDental.Drawing.Font();
				System.Windows.Size size2=g.MeasureString(richTextBoxMessage.Text,od_Font,widthMax:bodyWidth*0.7);
				richTextBoxMessage.Width=(int)Math.Ceiling(size2.Width)+4;//Extra horizontal padding to ensure that the text will fit when including the border.
				richTextBoxMessage.Height=(int)Math.Ceiling(size2.Height)+4;//Extra vertical padding to ensure that the text will fit when including the border.
				richTextBoxMessage.ReadOnly=true;
				Panel panelBorder=new Panel() {
					Width=richTextBoxMessage.Width+4,
					Height=richTextBoxMessage.Height+4,
					BackColor=Color.Black,
				};
				Panel panelPadding=new Panel() {
					Width=panelBorder.Width-2,
					Height=panelBorder.Height-2,
					BackColor=smsThreadMessage.BackColor,
					Location=new Point(1,1)
				};
				panelBorder.Name="msgBorder_"+smsThreadMessage.ID;
				if(smsThreadMessage.IsAlignedLeft) {
					panelBorder.Location=new Point(horizontalMargin,y);
				}
				else {//Right aligned
					panelBorder.Location=new Point(bodyWidth-horizontalMargin-panelBorder.Width,y);
				}
				richTextBoxMessage.Location=new Point(panelBorder.Location.X+2,panelBorder.Location.Y+2);
				richTextBoxMessage=(RichTextBox)AddOrUpdatePanelScrollChildControl(richTextBoxMessage);
				if(smsThreadMessage.IsHighlighted) {
					controlHighlighted=richTextBoxMessage;
				}
				panelBorder.Controls.Add(panelPadding);
				AddOrUpdatePanelScrollChildControl(panelBorder);
				y+=panelBorder.Height;
				//We set the tags here so that we can update SMS chat bubbles by index in SetLoadingIndicatorMessage(...)
				panelBorder.Tag=panelPadding;
				richTextBoxMessage.Tag=panelBorder;
				//Display 'Delivered'/'Sent' after text message
				if(labelMessageStatus.Text=="Sent" || labelMessageStatus.Text=="Delivered") {
					labelMessageStatus.Location=new Point(0,y);
					AddOrUpdatePanelScrollChildControl(labelMessageStatus);
					y+=labelMessageStatus.Height;
				}
			}
			Label labelBottomSpacer=new Label();
			SetDoubleBuffered(labelBottomSpacer,true);
			//labelBottomSpacer.MouseWheel+=MouseWheel_Scroll;//Labels automatically pass their scroll events through to their parent controls.
			labelBottomSpacer.Name="labelBottomSpacer";
			labelBottomSpacer.Width=bodyWidth;
			labelBottomSpacer.Height=verticalPadding;
			labelBottomSpacer.Location=new Point(0,y);
			AddOrUpdatePanelScrollChildControl(labelBottomSpacer);
			y+=labelBottomSpacer.Height;
			if(controlHighlighted==null) {
				controlHighlighted=labelBottomSpacer;
			}
			if(panelScroll.VerticalScroll.Value!=panelScroll.VerticalScroll.Maximum) {
				//We have to set the scroll value twice or else the scroll will occur but the UI of the scrollbar will be wrong.
				//https://stackoverflow.com/questions/5565653/scrollbar-does-not-update-on-changing-the-scroll-value
				panelScroll.VerticalScroll.Value=panelScroll.VerticalScroll.Maximum;
				panelScroll.VerticalScroll.Value=panelScroll.VerticalScroll.Maximum;//Intentionally set twice.
			}
			panelScroll.ResumeLayout();
		}

		///<summary>Updates the last RichTextBox in the view with the given message. Returns false if there are no messages.</summary>
		public bool UpdateLastRichTextBoxText(string msg) {
			if(_listSmsThreadToDisplay.IsNullOrEmpty()) {
				return false;
			}
			return UpdateRichTextBoxText(_listSmsThreadToDisplay.Count-1,msg);
		}

		///<summary>Updates the RichTextBox for the given index and message. Returns false if index is invalid.</summary>
		public bool UpdateRichTextBoxText(int index,string msg) {
			RichTextBox[] richTextBoxes=panelScroll.Controls.AsEnumerable<Control>().OfType<RichTextBox>().ToArray();
			if(!index.Between(0,richTextBoxes.Length-1)) {
				return false;
			}
			RichTextBox richTextBox=richTextBoxes[index];
			Size size=TextRenderer.MeasureText(msg,panelScroll.Font,
				new Size((int)(GetBodyWidth()*0.7),Int32.MaxValue),TextFormatFlags.WordBreak|TextFormatFlags.TextBoxControl);
			richTextBox.Width=size.Width+4;//Extra horizontal padding to ensure that the text will fit when including the border.
			richTextBox.Height=size.Height+4;//Extra vertical padding to ensure that the text will fit when including the border.
			if(richTextBox.Tag is Panel panelBorder) {
				panelBorder.Width= richTextBox.Width+4;
				panelBorder.Height= richTextBox.Height+4;
				if(panelBorder.Tag is Panel panelPadding) {
					panelPadding.Width=panelBorder.Width-2;
					panelPadding.Height=panelBorder.Height-2;
				}
			}
			richTextBox.Text=msg;
			return true;
		}

		public int GetBodyWidth() {
			return panelScroll.Width-SystemInformation.VerticalScrollBarWidth;
		}

		private void FlagButton_Click(object sender, EventArgs e) {
			if(sender is Button button && button.Tag is WebChatMessage aiChatMessage) {
				button.Text=WebChatMessages.ToggleNeedsFollowUp(aiChatMessage)?"Unflag":"Flag";
			}
		}

		///<summary>Removes un-wanted text that OpenAi includes in their response.</summary>
		private string CleanAiResponse(string text) {
			string regexPattern=@"\?\d+†source\?";
			return Regex.Replace(text,regexPattern,string.Empty);
		}

		///<summary>Returns the control which previsouly existing in the panel or returns the new control if it was added.</summary>
		private Control AddOrUpdatePanelScrollChildControl(Control control) {
			//Control name never changes once set on a control above.
			Control existingControl=panelScroll.Controls.AsEnumerable<Control>().FirstOrDefault(x => x.Name==control.Name);
			if(existingControl==null) {
				LayoutManager.Add(control,panelScroll);
				return control;
			}
			//The properties updated here must match any properties which can change from their original values above.
			//Double buffering never changes once set on a control above.
			existingControl.Text=control.Text;
			if(existingControl is Label existingLabel && control is Label label) {
				existingLabel.TextAlign=label.TextAlign;
			}
			existingControl.Width=control.Width;
			existingControl.Height=control.Height;
			existingControl.Location=control.Location;
			existingControl.ForeColor=control.ForeColor;
			control.Dispose();//This control does not have any child controls.
			LayoutManager.Move(existingControl,existingControl.Bounds);
			return existingControl;
		}

		private void TextBoxMessage_LinkClicked(object sender,LinkClickedEventArgs e) {	
			try {
				string [] links=e.LinkText.Split(new string[] {" ","\t" },StringSplitOptions.RemoveEmptyEntries);
				string link=e.LinkText;
				if(links.Length>0) {
					link=links[0];
				}
				Process.Start(link);
			}
			catch {
				MessageBox.Show(Lans.g(this,"Failed to open web browser.  Please make sure you have a default browser set and are connected to the internet then try again."),Lans.g(this,"Attention"));
			}

		}

		///<summary>Allows us to set the protected DoubleBuffered property on any Control.</summary>
		private void SetDoubleBuffered(Control control,bool isDoubleBuffered) {
			control.GetType().InvokeMember("DoubleBuffered",
				BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.SetProperty,
				null,control,new object[] { true });
		}

		private void BuildListMessages() {
			_listSmsThreadToDisplay=new List<SmsThreadMessage>();	//We'll hold what messages are to be shown in this list.
			//Sort and reverse it so the messages are in order when they're added.
			_listSmsThreadMessages=_listSmsThreadMessages.OrderByDescending(x => x.MsgDateTime).ToList();	
			int maxPage=(int)Math.Ceiling((double)_listSmsThreadMessages.Count/CountMessagesToDisplay);	//# messages per page, #/count-1=page index
			if(_smsThreadPage > maxPage) {	
				_smsThreadPage=maxPage;
			}
			labelCurrentPage.Text=(_smsThreadPage).ToString() +" "+Lan.g(this,"of")+" "+ (maxPage).ToString();
			//Here we fill the reference list that is displayed depending on which page we're on.
			int firstMessageIdx=CountMessagesToDisplay*(_smsThreadPage-1);
			int lastMessageIdx=Math.Min(CountMessagesToDisplay*_smsThreadPage,_listSmsThreadMessages.Count)-1;
			for(int i=firstMessageIdx;i<=lastMessageIdx;i++) {	
				_listSmsThreadToDisplay.Add(_listSmsThreadMessages[i]);
			}
			//Reverse order so older messages are at the top, new at the bottom.
			_listSmsThreadToDisplay=_listSmsThreadToDisplay.OrderBy(x => x.MsgDateTime).ToList();	
			if(_listSmsThreadMessages.Count<=CountMessagesToDisplay) {
				panelNavigation.Visible=false;
				LayoutManager.MoveLocation(panelScroll,new Point(0,0));
			}
			else {
				panelNavigation.Visible=true;
				LayoutManager.MoveLocation(panelScroll,new Point(0,panelNavigation.Location.Y+panelNavigation.Height));//Just below panelNavigation
				LayoutManager.MoveHeight(panelScroll,Height-panelScroll.Location.Y);
			}			
			if(_smsThreadPage==maxPage) {
				butBackPage.Enabled=false;
				butEnd.Enabled=false;
				butForwardPage.Enabled=true;
				butBeginning.Enabled=true;
			}
			else if(_smsThreadPage==1) {
				butBackPage.Enabled=true;
				butEnd.Enabled=true;
				butForwardPage.Enabled=false;
				butBeginning.Enabled=false;
			}
			else {
				butBackPage.Enabled=true;
				butEnd.Enabled=true;
				butForwardPage.Enabled=true;
				butBeginning.Enabled=true;
			}
		}

		///<summary>Logic for the below method of adapting TextBox's copy method as that funcionality does not exist for RichTextBox</summary>
		private void TextBoxMessage_MouseDown(object sender,MouseEventArgs e) {
			if(!(sender is RichTextBox richTextBox)) {
				return;
			}
			if(e.Button==System.Windows.Forms.MouseButtons.Right) {
				ContextMenu contextMenu=new ContextMenu();
				MenuItem menuItem=new MenuItem("Copy");
				string text=richTextBox.SelectedText;
				if(richTextBox.SelectedText==""){
					text=richTextBox.Text;
				}
				if(text.Length==0) {
					return;
				}
				menuItem.Click+=(o,e) => ODClipboard.SetClipboard(text);
				contextMenu.MenuItems.Add(menuItem);
				richTextBox.ContextMenu=contextMenu;
			}
		}

		private void butBeginning_Click(object sender,EventArgs e) {
			if(_smsThreadPage==1) {
				return;	//Skip redrawing what we already have.
			}
			_smsThreadPage=1;
			FillMessageThread();
		}

		private void butForwardPage_Click(object sender,EventArgs e) {
			if(_smsThreadPage==1) {
				return;	//Don't go before the first page.
			}
			_smsThreadPage--;
			FillMessageThread();
		}

		private void butBackPage_Click(object sender,EventArgs e) {
			_smsThreadPage++;	//If we're on the last page, this variable will be fixed in the grid fill area.
			FillMessageThread();
		}

		private void butEnd_Click(object sender,EventArgs e) {
			_smsThreadPage=Int32.MaxValue;	//This is reset back to the maximum page, so we arrive directly at the end.
			FillMessageThread();
		}

		private void MouseWheel_Scroll(object sender,MouseEventArgs e) {
			//e.Delta is the actual scroll amount moved, despite what the comments on it say.
			int val=panelScroll.VerticalScroll.Value-e.Delta;
			if(val < panelScroll.VerticalScroll.Minimum) {
				val=panelScroll.VerticalScroll.Minimum;
			}
			else if(val > panelScroll.VerticalScroll.Maximum) {
				val=panelScroll.VerticalScroll.Maximum;
			}
			panelScroll.VerticalScroll.Value=val;
			panelScroll.PerformLayout();//Without this, setting the VerticalScroll.Value will not take effect, sometimes!
		}
	}

	public class SmsThreadMessage {
		///<summary>Context specific.  Must be a unique ID which can identify the message within the SmsThreadView control.</summary>
		public string ID;
		///<summary>The date and time the message was sent or received.</summary>
		public DateTime MsgDateTime;
		///<summary>The message itself.</summary>
		public string Message;
		///<summary>If true, the message will be left aligned.  Otherwise the message will be right aligned.  Left aligned messages will be messages from
		///the patient, and right aligned messages will be from the office.  The left/right alignment is used as a quick way to show the user who
		///wrote each part of the message thread.</summary>
		public bool IsAlignedLeft;
		///<summary>Causes the message text to show in red.</summary>
		public bool IsImportant;
		public bool IsHighlighted;
		public SmsDeliveryStatus SmsDeliveryStatusCur;
		///<summary>If not null, then shows next to the date and time.</summary>
		public string UserName;
		public bool IsAiMessage;
		public object Tag;

		public Color BackColor {
			get {
				Color retVal;
				if(IsAlignedLeft) {//From Customer
					retVal=Color.FromArgb(244,255,244);
					if(IsHighlighted) {
						retVal=Color.FromArgb(220,255,220);
					}
				}
				else {//Right aligned
					retVal=Color.White;
					if(IsHighlighted) {
						retVal=Color.FromArgb(220,220,220);
					}
				}
				return retVal;
			}
		}

		public SmsThreadMessage(string id,DateTime msgDateTime,string message,bool isAlignedLeft,bool isImportant,bool isHighlighted,SmsDeliveryStatus smsDeliveryStatus,string userName=null,bool isAiMessage=false,object tag=null) {
			ID=id;
			MsgDateTime=msgDateTime;
			Message=message;
			IsAlignedLeft=isAlignedLeft;
			IsImportant=isImportant;
			IsHighlighted=isHighlighted;
			SmsDeliveryStatusCur=smsDeliveryStatus;
			UserName=userName;
			IsAiMessage=isAiMessage;
			Tag=tag;
		}

		public static int CompareMessages(SmsThreadMessage msg1,SmsThreadMessage msg2) {
			return msg1.MsgDateTime.CompareTo(msg2.MsgDateTime);
		}

	}

}
