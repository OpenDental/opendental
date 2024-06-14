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
	public class CheckBoxHelper{
		public static CheckBox CreateCheckBox(OpenDental.UI.CheckBox checkBox){
			CheckBox checkBoxNew=new CheckBox();
			checkBoxNew.Width=checkBox.Width;
			checkBoxNew.Height=checkBox.Height;
			TextBlock textBlock=new TextBlock();
			checkBoxNew.Content=textBlock;
			textBlock.Text=checkBox.Text;
			textBlock.TextWrapping=TextWrapping.Wrap;
			if(checkBox.CheckAlign==System.Drawing.ContentAlignment.TopLeft){
				checkBoxNew.HorizontalContentAlignment=HorizontalAlignment.Left;
				checkBoxNew.VerticalContentAlignment=VerticalAlignment.Top;
			}
			else if(checkBox.CheckAlign==System.Drawing.ContentAlignment.MiddleLeft){
				checkBoxNew.HorizontalContentAlignment=HorizontalAlignment.Left;
				checkBoxNew.VerticalContentAlignment=VerticalAlignment.Center;
			}
			else if(checkBox.CheckAlign==System.Drawing.ContentAlignment.MiddleRight){
				checkBoxNew.HorizontalContentAlignment=HorizontalAlignment.Left;//but when we change the flow direction, this turns into right.
				checkBoxNew.VerticalContentAlignment=VerticalAlignment.Center;
				checkBoxNew.FlowDirection=FlowDirection.RightToLeft;
				Style stylePath=new Style();//the check itself is now backward, so we need to fix that
				stylePath.TargetType=typeof(Path);
				Setter setter=new Setter();
				setter.Property=Path.FlowDirectionProperty;
				setter.Value=FlowDirection.LeftToRight;
				stylePath.Setters.Add(setter);
				checkBoxNew.Resources.Add(typeof(Path),stylePath);
			}
			else{
				throw new Exception("Only the three alignments above are allowed.");
			}
			if(checkBox.ThreeState){
				checkBoxNew.IsThreeState=true;
				switch(checkBox.CheckState){
					case System.Windows.Forms.CheckState.Checked:
						checkBoxNew.IsChecked=true;
						break;
					case System.Windows.Forms.CheckState.Unchecked:
						checkBoxNew.IsChecked=false;
						break;
					case System.Windows.Forms.CheckState.Indeterminate:
						checkBoxNew.IsChecked=null;
						break;
				}
			}
			else{//2 state
				checkBoxNew.IsChecked=checkBox.Checked;
			}
			//Remember that the lambdas below don't fire until needed.
			checkBoxNew.Click+=(sender,routedEventArgs)=>EventHelper.InvokeClick(checkBox);
			//might need to add a mouseMove
			return checkBoxNew;
		}

		

	}*/
}
