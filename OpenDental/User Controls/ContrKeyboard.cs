using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace OpenDental.User_Controls {
	public delegate void KeyboardClickEventHandler(object sender,KeyboardClickEventArgs e);

	public partial class ContrKeyboard:UserControl {
		[Category("Action"),Description("Occurs when a key is clicked.")]
		//public event KeyboardClickEventHandler KeyClick=null;
		public event KeyboardClickEventHandler KeyClick=null;

		public ContrKeyboard() {
			InitializeComponent();
			this.SetStyle(ControlStyles.Selectable,false);//this doesn't seem to work.  Grrr.
			//MessageBox.Show(this.CanFocus.ToString());
			//this.CanFocus=false;
		}

		///<summary></summary>
		protected void OnKeyClicked(string myTxt,Keys myKeyData){
			KeyboardClickEventArgs kArgs=new KeyboardClickEventArgs(myTxt,myKeyData);
			//KeyEventArgs kArgs=new KeyEventArgs( KeyboardClickEventArgs(myTxt);
			if(KeyClick!=null)
				KeyClick(this,kArgs);
		}

		private void but_Click(object sender,EventArgs e) {
			string txt=((OpenDental.UI.Button)sender).Text;
			//MessageBox.Show(txt);
			OnKeyClicked(txt,Keys.None);
		}

		private void butBack_Click(object sender,EventArgs e) {
			OnKeyClicked("",Keys.Back);
		}

		private void butSpace_Click(object sender,EventArgs e) {
			OnKeyClicked(" ",Keys.None);
		}

		private void but_MouseDown(object sender,MouseEventArgs e) {
			this.OnMouseDown(e);
		}

		


	}

	///<summary></summary>
	public class KeyboardClickEventArgs {
		private string txt;
		private Keys keyData;
		
		public KeyboardClickEventArgs(string myTxt,Keys myKeyData) {
			txt=myTxt;
			keyData=myKeyData;
		}

		///<summary></summary>
		public string Txt {
			get {
				return txt;
			}
		}

		///<summary></summary>
		public Keys KeyData {
			get {
				return keyData;
			}
		}

	}

}
