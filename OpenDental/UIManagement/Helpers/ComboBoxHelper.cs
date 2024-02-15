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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using CodeBase;

namespace OpenDental.UIManagement {
	/*
	///<summary></summary>
	public class ComboBoxHelper{
		public static ComboBox CreateComboBox(OpenDental.UI.ComboBox comboBox){
			ComboBox comboBoxNew=new ComboBox();
			comboBoxNew.Width=comboBox.Width;
			comboBoxNew.Height=comboBox.Height;
			comboBoxNew.MaxDropDownHeight=2000;//but it still doesn't expand upward
			if(comboBox.SelectionModeMulti){
				throw new Exception("Multiselection comboBoxes are not yet supported.");
			//https://stackoverflow.com/questions/508506/is-there-any-native-wpf-multiselect-combobox-available
			//https://www.codeproject.com/Articles/45782/A-WPF-Combo-Box-with-Multiple-Selection
			//But that's all hard.  Instead, I might add an ellipsis button next to a read only textBox. The button would open a form with a list and ok/cancel buttons.
				//comboBoxNew.SelectionMode=SelectionMode.Extended;
			}
			comboBoxNew.IsEnabled=comboBox.Enabled;
			for(int i=0;i<comboBox.Items.Count;i++){
				Item_Add(comboBoxNew,comboBox,comboBox.Items.GetTextShowingAt(i));
			}
			return comboBoxNew;
		}

		public static void Item_Add(ComboBox wComboBox,OpenDental.UI.ComboBox oComboBox,string txt){
			ListBoxItem listBoxItem=new ListBoxItem();
			listBoxItem.Content=txt;
			float heightFont=oComboBox.Font.GetHeight();
			listBoxItem.Height=heightFont+.5d;//to exactly match old winforms
			listBoxItem.Padding=new Thickness(0);//changing this to 1 looks really terrible.
			wComboBox.Items.Add(listBoxItem);
		}

	}*/
}
