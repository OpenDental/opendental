using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OpenDental.UIManagement {
	/*
	///<summary>This class is for internal use by UIManagement. Calls from OD proper should use Um.</summary>
	public class RadioButtonHelper{
		public static RadioButton CreateRadioButton(System.Windows.Forms.RadioButton radioButton){
			RadioButton radioButtonNew=new RadioButton();
			radioButtonNew.Width=radioButton.Width;
			radioButtonNew.Height=radioButton.Height;
			TextBlock textBlock=new TextBlock();
			radioButtonNew.Content=textBlock;
			textBlock.Text=radioButton.Text;
			textBlock.TextWrapping=TextWrapping.Wrap;
			if(radioButton.CheckAlign==System.Drawing.ContentAlignment.TopLeft){
				radioButtonNew.HorizontalContentAlignment=HorizontalAlignment.Left;
				radioButtonNew.VerticalContentAlignment=VerticalAlignment.Top;
			}
			else if(radioButton.CheckAlign==System.Drawing.ContentAlignment.MiddleLeft){
				radioButtonNew.HorizontalContentAlignment=HorizontalAlignment.Left;
				radioButtonNew.VerticalContentAlignment=VerticalAlignment.Center;
			}
			else if(radioButton.CheckAlign==System.Drawing.ContentAlignment.MiddleRight){
				radioButtonNew.HorizontalContentAlignment=HorizontalAlignment.Left;//but when we change the flow direction, this turns into right.
				radioButtonNew.VerticalContentAlignment=VerticalAlignment.Center;
				radioButtonNew.FlowDirection=FlowDirection.RightToLeft;
			}
			else{
				throw new Exception("Only the three alignments above are allowed.");
			}
			radioButtonNew.IsChecked=radioButton.Checked;
			//Remember that the lambdas below don't fire until needed.
			radioButtonNew.Click+=(sender,routedEventArgs)=>EventHelper.InvokeClick(radioButton);
			//might need to add a mouseMove
			return radioButtonNew;
		}

		

	}*/
}
