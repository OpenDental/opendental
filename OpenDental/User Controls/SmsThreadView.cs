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
			//Clear controls and reset navigation bars.
			panelScroll.SuspendLayout();
			DisposeChildrenRecursive(panelScroll,doDisposeParent:false);
			panelScroll.ResumeLayout();
			panelScroll.Controls.Clear();
			panelNavigation.Visible=false;
			panelScroll.Location=new Point(0,0);
			Invalidate();
			if(_listSmsThreadMessages==null || _listSmsThreadMessages.Count<1) {
				return;
			}
			BuildListMessages();
			int bodyWidth=panelScroll.Width-SystemInformation.VerticalScrollBarWidth;
			int verticalPadding=5;
			int horizontalMargin=(int)(bodyWidth*0.02);
			int y=0;
			Control controlHighlighted=null;
			panelScroll.SuspendLayout();
			for(int i=0;i<_listSmsThreadToDisplay.Count;i++) {
				SmsThreadMessage msg=_listSmsThreadToDisplay[i];
				y+=verticalPadding;
				Label labelMessageHeader=new Label();
				SetDoubleBuffered(labelMessageHeader,true);
				//labelMessageHeader.MouseWheel+=MouseWheel_Scroll;//Labels automatically pass their scroll events through to their parent controls.
				labelMessageHeader.Name="labelMessageHeader"+i;
				labelMessageHeader.Text=((msg.UserName==null)?"":(msg.UserName+"  "))+msg.MsgDateTime.ToString();
				if(msg.IsAlignedLeft) {
					labelMessageHeader.TextAlign=ContentAlignment.MiddleLeft;
				}
				else {//Aligned right
					labelMessageHeader.TextAlign=ContentAlignment.MiddleRight;
				}
				Size textSize=TextRenderer.MeasureText(labelMessageHeader.Text,panelScroll.Font,
					new Size(bodyWidth,Int32.MaxValue),TextFormatFlags.WordBreak);
				labelMessageHeader.Width=bodyWidth;
				labelMessageHeader.Height=textSize.Height+2;//Extra vertical padding to ensure that the text will fit when including the border.
				labelMessageHeader.Location=new Point(0,y);
				LayoutManager.Add(labelMessageHeader,panelScroll);
				y+=labelMessageHeader.Height;
				RichTextBox textBoxMessage=new RichTextBox();
				textBoxMessage.DetectUrls=true;
				textBoxMessage.ScrollBars=RichTextBoxScrollBars.None;
				textBoxMessage.LinkClicked+=TextBoxMessage_LinkClicked;
				textBoxMessage.MouseDown+=TextBoxMessage_MouseDown;
				textBoxMessage.SelectionBackColor=Color.Yellow;
				SetDoubleBuffered(textBoxMessage,true);
				textBoxMessage.MouseWheel+=MouseWheel_Scroll;//Textboxes handle their own scroll events, because they have their own scroll bars.
				textBoxMessage.Font=panelScroll.Font;
				textBoxMessage.BackColor=msg.BackColor;
				if(msg.IsHighlighted) {
					controlHighlighted=textBoxMessage;
				}
				if(msg.IsImportant) {
					textBoxMessage.ForeColor=Color.Red;
				}
				textBoxMessage.Name="textSmsThreadMsg"+i;
				textBoxMessage.BorderStyle=BorderStyle.None;
				textBoxMessage.Multiline=true;
				textBoxMessage.Text=msg.Message.Replace("\r\n","\n").Replace("\n","\r\n");//Normalize \n coming from RichTextBox to \r\n for TextBox.
				//Each message wraps horizontally.
				textSize=TextRenderer.MeasureText(textBoxMessage.Text,panelScroll.Font,
					new Size((int)(bodyWidth*0.7),Int32.MaxValue),TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl);
				textBoxMessage.Width=textSize.Width+4;//Extra horizontal padding to ensure that the text will fit when including the border.
				textBoxMessage.Height=textSize.Height+4;//Extra vertical padding to ensure that the text will fit when including the border.
				textBoxMessage.ReadOnly=true;
				Panel border=new Panel() {
					Width=textBoxMessage.Width+2,
					Height=textBoxMessage.Height+2,
					BackColor=Color.Black,
				};
				if(msg.IsAlignedLeft) {
					border.Location=new Point(horizontalMargin,y);
				}
				else {//Right aligned
					border.Location=new Point(bodyWidth-horizontalMargin-border.Width,y);
				}
				textBoxMessage.Location=new Point(border.Location.X+1,border.Location.Y+1);
				LayoutManager.Add(textBoxMessage,panelScroll);
				LayoutManager.Add(border,panelScroll);
				y+=border.Height;
			}
			Label labelBottomSpacer=new Label();
			SetDoubleBuffered(labelBottomSpacer,true);
			//labelBottomSpacer.MouseWheel+=MouseWheel_Scroll;//Labels automatically pass their scroll events through to their parent controls.
			labelBottomSpacer.Name="labelBottomSpacer";
			labelBottomSpacer.Width=bodyWidth;
			labelBottomSpacer.Height=verticalPadding;
			labelBottomSpacer.Location=new Point(0,y);
			LayoutManager.Add(labelBottomSpacer,panelScroll);
			y+=labelBottomSpacer.Height;
			if(controlHighlighted==null) {
				controlHighlighted=labelBottomSpacer;
			}
			if(panelScroll.VerticalScroll.Value!=panelScroll.VerticalScroll.Maximum) {
				panelScroll.VerticalScroll.Value=panelScroll.VerticalScroll.Maximum; //scroll to the end first then scroll to control
			}
			panelScroll.ResumeLayout();
			panelScroll.ScrollControlIntoView(controlHighlighted);//Scroll to highlighted control, or if none highlighted, then scroll to the end.
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
				panelScroll.Location=new Point(0,0);
			}
			else {
				panelNavigation.Visible=true;
				panelScroll.Location=new Point(0,panelNavigation.Location.Y+panelNavigation.Height);	//Buttress up against the panelNavigation
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
				menuItem.Click+=(o,e) => Clipboard.SetText(text);
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
		///<summary>If not null, then shows next to the date and time.</summary>
		public string UserName;

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

		public SmsThreadMessage(DateTime msgDateTime,string message,bool isAlignedLeft,bool isImportant,bool isHighlighted,string userName=null) {
			MsgDateTime=msgDateTime;
			Message=message;
			IsAlignedLeft=isAlignedLeft;
			IsImportant=isImportant;
			IsHighlighted=isHighlighted;
			UserName=userName;
		}

		public static int CompareMessages(SmsThreadMessage msg1,SmsThreadMessage msg2) {
			return msg1.MsgDateTime.CompareTo(msg2.MsgDateTime);
		}

	}

}
