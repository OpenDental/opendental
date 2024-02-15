using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WpfControls;

namespace OpenDental.UIManagement {
	/*
	public class TextBoxHelper{
		public static TextBox CreateTextBox(System.Windows.Forms.TextBox textBox){
			//Todo: We have a problem that WPF textboxes always automatically expand to fit available space.
			TextBox textBoxNew=new TextBox();
			textBoxNew.Text=textBox.Text;
			if(textBox.Multiline){
				//There's no WPF property that corresponds to Multiline.
				//Wordwrap is unfortunately set to true on many single line winforms textboxes.
				//I don't think that's what they want, so we'll ignore that and set TextWrapping according to Multiline.
				//Any time we want to test for Multiline, we will instead test for TextWrapping.
				textBoxNew.TextWrapping=TextWrapping.Wrap;//default is NoWrap
				textBoxNew.AcceptsReturn=textBox.AcceptsReturn;//default is false. Even on multiline, we will respect the Winforms setting.
			}
			textBoxNew.IsReadOnly=textBox.ReadOnly;
			textBoxNew.VerticalContentAlignment=VerticalAlignment.Center;
			if(textBox.TextAlign==System.Windows.Forms.HorizontalAlignment.Left){
				textBoxNew.HorizontalContentAlignment=HorizontalAlignment.Left;
			}
			if(textBox.TextAlign==System.Windows.Forms.HorizontalAlignment.Center){
				textBoxNew.HorizontalContentAlignment=HorizontalAlignment.Center;
			}
			if(textBox.TextAlign==System.Windows.Forms.HorizontalAlignment.Right){
				textBoxNew.HorizontalContentAlignment=HorizontalAlignment.Right;
			}
			textBoxNew.Background=new SolidColorBrush(ColorOD.ToWpf(textBox.BackColor));
			textBoxNew.TextChanged+=(sender,textChangedEventArgs)=>EventHelper.InvokeTextChanged(textBox);
			return textBoxNew;
		}

		public static Wui.TextBoxValidDate CreateTextBoxValidDate(ValidDate validDate){
			Wui.TextBoxValidDate textBoxValidDate=new Wui.TextBoxValidDate();
			textBoxValidDate.Text=validDate.Text;
			if(validDate.TextAlign==System.Windows.Forms.HorizontalAlignment.Left) {
				textBoxValidDate.HorizontalContentAlignment=HorizontalAlignment.Left;
			}
			if(validDate.TextAlign==System.Windows.Forms.HorizontalAlignment.Center) {
				textBoxValidDate.HorizontalContentAlignment=HorizontalAlignment.Center;
			}
			if(validDate.TextAlign==System.Windows.Forms.HorizontalAlignment.Right) {
				textBoxValidDate.HorizontalContentAlignment=HorizontalAlignment.Right;
			}
			return textBoxValidDate;
		}*/

		/*This was terrible.  Didn't look good at all
		public static DatePickerTextBox CreateDatePickerTextBox(ValidDate validDate){
			DatePickerTextBox datePickerTextBox=new DatePickerTextBox();
			datePickerTextBox.Text=validDate.Text;
			datePickerTextBox.IsReadOnly=validDate.ReadOnly;
			datePickerTextBox.VerticalContentAlignment=VerticalAlignment.Center;
			if(validDate.TextAlign==System.Windows.Forms.HorizontalAlignment.Left){
				datePickerTextBox.HorizontalContentAlignment=HorizontalAlignment.Left;
			}
			if(validDate.TextAlign==System.Windows.Forms.HorizontalAlignment.Center){
				datePickerTextBox.HorizontalContentAlignment=HorizontalAlignment.Center;
			}
			if(validDate.TextAlign==System.Windows.Forms.HorizontalAlignment.Right){
				datePickerTextBox.HorizontalContentAlignment=HorizontalAlignment.Right;
			}
			datePickerTextBox.Background=new SolidColorBrush(ColorOD.ToWpf(validDate.BackColor));
			datePickerTextBox.BorderBrush=Brushes.DarkGray;
			datePickerTextBox.BorderThickness=new Thickness(1);
			datePickerTextBox.TextChanged+=(sender,textChangedEventArgs)=>ControlHelper.InvokeTextChanged(validDate);
			return datePickerTextBox;
		}*/
	//}
}
