using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDentBusiness;

namespace WpfControls {
	public class PatientL {
		public static void ValidPhone_TextChanged(object sender, System.EventArgs e) {
			if(sender.GetType()!=typeof(UI.TextBox)) {
				return;
			}
			//if(!IsFormattingEnabled) { If this was set to false in the original, then don't even bother connecting to this eventHandler.
			//	return;
			//}
			UI.TextBox textBox=(UI.TextBox)sender;
			string formattedText=TelephoneNumbers.AutoFormat(textBox.Text);
			if(textBox.Text==formattedText) {
				return;
			}
			//If there are characters that get removed from calling AutoFormat (i.e. spaces) and the cursor was at the start (which happens if/when the
			//ValidPhone control initially gets filled with a value) the calculated new selection start index would be an invalid value, so Max with 0.
			//Move cursor forward the difference in text length, i.e. if this adds a '(' character, move the cursor ahead one index.
			int newSelectionStartPosition=Math.Max(textBox.SelectionStart+formattedText.Length-textBox.Text.Length,0);
			//remove this event handler from the TextChanged event so that setting the text here doesn't cause this to run again
			textBox.TextChanged-=ValidPhone_TextChanged;
			textBox.Text=formattedText;
			//add this event handler back to the TextChanged event
			textBox.TextChanged+=ValidPhone_TextChanged;
			textBox.SelectionStart=newSelectionStartPosition;
		}
	}
}
