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
	public class LabelHelper{
		public static Label CreateLabel(System.Windows.Forms.Label label){
			//WPF label does not support wrapping and textblock does not support alignment.  Combine the two to get desired behavior.
			//https://www.c-sharpcorner.com/Resources/880/wrap-text-in-a-wpf-label.aspx
			//or add a style to the existing textblock
			//https://stackoverflow.com/questions/5013067/how-can-i-wrap-text-in-a-label-using-wpf
			Label labelNew=new Label();
			labelNew.Padding=new Thickness(left:3,0,right:5,0);//duplicates the WinForms label
			TextBlock textBlock=new TextBlock();
			labelNew.Content=textBlock;
			//labelNew.Background=Brushes.LightBlue;//just for testing
			//textBlock.Background=Brushes.Pink;//just for testing
			labelNew.Background=new SolidColorBrush(ColorOD.ToWpf(label.BackColor));
			textBlock.Text=label.Text;
			textBlock.TextWrapping=TextWrapping.Wrap;
			labelNew.HorizontalContentAlignment=HorizontalAlignment.Left;
			if(label.TextAlign.In(System.Drawing.ContentAlignment.TopRight,System.Drawing.ContentAlignment.BottomRight,System.Drawing.ContentAlignment.MiddleRight)){
				labelNew.HorizontalContentAlignment=HorizontalAlignment.Right;
				textBlock.TextAlignment=TextAlignment.Right;
			}
			if(label.TextAlign.In(System.Drawing.ContentAlignment.TopCenter,System.Drawing.ContentAlignment.MiddleCenter,System.Drawing.ContentAlignment.BottomCenter)){
				labelNew.HorizontalContentAlignment=HorizontalAlignment.Center;
				textBlock.TextAlignment=TextAlignment.Center;
			}
			labelNew.VerticalContentAlignment=VerticalAlignment.Top;
			if(label.TextAlign.In(System.Drawing.ContentAlignment.MiddleLeft,System.Drawing.ContentAlignment.MiddleCenter,System.Drawing.ContentAlignment.MiddleRight)){
				labelNew.VerticalContentAlignment=VerticalAlignment.Center;
			}
			if(label.TextAlign.In(System.Drawing.ContentAlignment.BottomLeft,System.Drawing.ContentAlignment.BottomCenter,System.Drawing.ContentAlignment.BottomRight)){
				labelNew.VerticalContentAlignment=VerticalAlignment.Bottom;
			}
			return labelNew;
		}

		public static Wui.LinkLabel CreateLinkLabel(System.Windows.Forms.LinkLabel fLinkLabel){
			Wui.LinkLabel wLinkLabel=new Wui.LinkLabel();
			wLinkLabel.Background=Brushes.Pink;
			wLinkLabel.Text=fLinkLabel.Text;
			wLinkLabel.LinkStart=fLinkLabel.LinkArea.Start;
			wLinkLabel.LinkLength=fLinkLabel.LinkArea.Length;
			wLinkLabel.HorizontalContentAlignment=HorizontalAlignment.Left;
			if(fLinkLabel.TextAlign.In(System.Drawing.ContentAlignment.TopRight,System.Drawing.ContentAlignment.BottomRight,System.Drawing.ContentAlignment.MiddleRight)){
				wLinkLabel.HorizontalContentAlignment=HorizontalAlignment.Right;
				//textBlock.TextAlignment=TextAlignment.Right;//handled internally
			}
			if(fLinkLabel.TextAlign.In(System.Drawing.ContentAlignment.TopCenter,System.Drawing.ContentAlignment.MiddleCenter,System.Drawing.ContentAlignment.BottomCenter)){
				wLinkLabel.HorizontalContentAlignment=HorizontalAlignment.Center;
				//textBlock.TextAlignment=TextAlignment.Center;
			}
			wLinkLabel.VerticalContentAlignment=VerticalAlignment.Top;
			if(fLinkLabel.TextAlign.In(System.Drawing.ContentAlignment.MiddleLeft,System.Drawing.ContentAlignment.MiddleCenter,System.Drawing.ContentAlignment.MiddleRight)){
				wLinkLabel.VerticalContentAlignment=VerticalAlignment.Center;
			}
			if(fLinkLabel.TextAlign.In(System.Drawing.ContentAlignment.BottomLeft,System.Drawing.ContentAlignment.BottomCenter,System.Drawing.ContentAlignment.BottomRight)){
				wLinkLabel.VerticalContentAlignment=VerticalAlignment.Bottom;
			}
			//wLinkLabel.LinkClicked+=(sender,e)=>ControlHelper.InvokeClick(button);
			//fLinkLabel.LinkClicked
			//buttonNew.Click+=(sender,routedEventArgs)=>ControlHelper.InvokeClick(button);
			return wLinkLabel;
		}

		private static void WLinkLabel_LinkClicked(object sender,EventArgs e) {
			throw new NotImplementedException();
		}
	}*/
}
