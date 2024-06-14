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
using System.Windows.Shapes;
using CodeBase;

namespace OpenDental.UIManagement {
	/*
	///<summary></summary>
	public class GroupBoxHelper{
		public static FrameworkElement CreateGroupBox(OpenDental.UI.GroupBox groupBox){
			//for a GroupBox, we will use a border-canvas-textBlock
			Border border=new Border();
			border.BorderBrush=Brushes.Silver;
			border.BorderThickness=new Thickness(1);
			border.CornerRadius=new CornerRadius(4);
			Canvas canvas=new Canvas();
			border.Child=canvas;
			TextBlock textBlock=new TextBlock();
			textBlock.Text=groupBox.Text;
			Canvas.SetLeft(textBlock,6);
			Canvas.SetTop(textBlock,0);
			canvas.Children.Add(textBlock);
			return border;
		}

		public static void AddControlToGroupBox(FrameworkElement frameworkElementControl,FrameworkElement frameworkElementGroupBox){
			Border border=(Border)frameworkElementGroupBox;
			Canvas canvas=(Canvas)border.Child;
			canvas.Children.Add(frameworkElementControl);
		}
	}*/
}
