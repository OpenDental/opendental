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
	public class PanelHelper{
		public static FrameworkElement CreatePanel(System.Windows.Forms.Panel panel){
			//for a Panel, we will use a Border-ScrollViewer-Grid arrangement.
			Border border=new Border();
			if(panel.BorderStyle==System.Windows.Forms.BorderStyle.FixedSingle){
				border.BorderBrush=Brushes.Gray;//MS panel gives us no color choice
				border.BorderThickness=new Thickness(1);
			}
			//warning: this needs to be changed so that scrollViewer is only present if autoScroll is turned on.
			//DataGrid inside a scrollViewer will lose virtualization and have bad performance.
			ScrollViewer scrollViewer=new ScrollViewer();
			if(panel.AutoScroll){
				scrollViewer.HorizontalScrollBarVisibility=ScrollBarVisibility.Auto;
				scrollViewer.VerticalScrollBarVisibility=ScrollBarVisibility.Auto;
			}
			else{
				scrollViewer.HorizontalScrollBarVisibility=ScrollBarVisibility.Disabled;
				scrollViewer.VerticalScrollBarVisibility=ScrollBarVisibility.Disabled;
			}
			Grid gridNew=new Grid();//just one cell
			gridNew.Background=new SolidColorBrush(ColorOD.ToWpf(panel.BackColor));
			scrollViewer.Content=gridNew;
			border.Child=scrollViewer;
			gridNew.MouseDown+=(sender,mouseButtonEventArgs)=>EventHelper.InvokeClick(panel);
			return border;
		}

		public static void AddControlToPanel(FrameworkElement frameworkElementControl,FrameworkElement frameworkElementPanel){
			Border border=(Border)frameworkElementPanel;
			ScrollViewer scrollViewer=(ScrollViewer)border.Child;
			Grid grid=(Grid)scrollViewer.Content;
			grid.Children.Add(frameworkElementControl);
		}

		public static void SetPositionOnPanel(FrameworkElement frameworkElementControl,float x,float y){
			Thickness thickness=new Thickness(left:x,top:y,right:0,bottom:0);
			//grid allows absolute positioning relative to grid boundaries.
			//It also announces its desired size to the scrollViewer to allow that to function.
			frameworkElementControl.Margin=thickness;
			frameworkElementControl.VerticalAlignment=VerticalAlignment.Top;
			frameworkElementControl.HorizontalAlignment=HorizontalAlignment.Left;
		}
		
		public static void SetBackColor(Proxy proxy, Color color){
			Border border = (Border)proxy.FrameworkElement_;
			ScrollViewer scrollViewer = (ScrollViewer)border.Child;
			Grid grid = (Grid)scrollViewer.Content;
			grid.Background=new SolidColorBrush(color);
		}

		public static Color GetBackColor(Proxy proxy){
			Border border = (Border)proxy.FrameworkElement_;
			ScrollViewer scrollViewer = (ScrollViewer)border.Child;
			Grid grid = (Grid)scrollViewer.Content;
			return ((SolidColorBrush)grid.Background).Color;
		}



	}*/
}
