using System;
using System.ComponentModel;
using OpenDentBusiness;

namespace OpenDental {
	///<summary>A simple text box that will automatically format phone numbers for US and Canadian users as the user types.</summary>
	public partial class ValidPhone:System.Windows.Forms.TextBox {

		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Controls whether the content typed in will be automatically formatted (US and Canada only).")]
		public bool IsFormattingEnabled { get;set; }=true;

		///<summary></summary>
		public ValidPhone() {
			InitializeComponent();
		}

		private void ValidPhone_TextChanged(object sender, System.EventArgs e) {
			if(sender.GetType()!=typeof(ValidPhone)) {
				return;
			}
			if(!IsFormattingEnabled) { 
				return;
			}
			ValidPhone textPhone=(ValidPhone)sender;
			string formattedText=TelephoneNumbers.AutoFormat(textPhone.Text);
			if(textPhone.Text==formattedText) {
				return;
			}
			//If there are characters that get removed from calling AutoFormat (i.e. spaces) and the cursor was at the start (which happens if/when the
			//ValidPhone control initially gets filled with a value) the calculated new selection start index would be an invalid value, so Max with 0.
			//Move cursor forward the difference in text length, i.e. if this adds a '(' character, move the cursor ahead one index.
			int newSelectionStartPosition=Math.Max(textPhone.SelectionStart+formattedText.Length-textPhone.Text.Length,0);
			//remove this event handler from the TextChanged event so that setting the text here doesn't cause this to run again
			textPhone.TextChanged-=ValidPhone_TextChanged;
			textPhone.Text=formattedText;
			//add this event handler back to the TextChanged event
			textPhone.TextChanged+=ValidPhone_TextChanged;
			textPhone.SelectionStart=newSelectionStartPosition;
		}

	}
}










