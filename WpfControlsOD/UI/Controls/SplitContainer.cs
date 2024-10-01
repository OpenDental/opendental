using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfControls.UI {
/*
How to use the SplitContainer control.
This is an extremely thin wrapper around a stock WPF Grid control.
Copy and paste the XAML code below, and then edit:
		<ui:SplitContainer Margin="30,30,0,0" Width="400" Height="400">
			<ui:SplitContainer.RowDefinitions>
				<RowDefinition/>
				<RowDefinition Height="Auto" />
				<RowDefinition/>
			</ui:SplitContainer.RowDefinitions>
			<Border BorderBrush="#FFC1C0C0" BorderThickness="1" Grid.RowSpan="3"/>
			<ui:Panel x:Name="panel1" HorizontalAlignment="Stretch" VerticalAlignment="Stretch">
				<!--Controls here-->
			</ui:Panel>
			<GridSplitter Grid.Row="1" Height="5" HorizontalAlignment="Stretch" Background="Silver"/>
			<ui:Panel x:Name="panel2" Grid.Row="2"  HorizontalAlignment="Stretch" VerticalAlignment="Stretch">
				<!--Controls here-->
			</ui:Panel>
		</ui:SplitContainer>

In code you can do things like this:
splitContainer.SetCollapsed(panel2,isCollapsed:true);
splitContainer.SetCollapsed(panel2,isCollapsed:false);
*/
	public class SplitContainer:System.Windows.Controls.Grid {
		///<summary>WPF doesn't have very good support for collapse/expand, so we need this.</summary>
		private List<OriginalHeight> _listOriginalHeights=new List<OriginalHeight>();

		static SplitContainer(){
			//This is just to get the default alignment set to UL 
			DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitContainer), new FrameworkPropertyMetadata(typeof(SplitContainer)));
			//WidthProperty.OverrideMetadata(typeof(SplitContainer), new FrameworkPropertyMetadata(400.0));
			//HeightProperty.OverrideMetadata(typeof(SplitContainer), new FrameworkPropertyMetadata(400.0));
		}

		///<summary>Collapses or uncollapses the specified panel.</summary>
		public void SetCollapsed(Panel panel,bool doCollapse){
			int rowIdx=System.Windows.Controls.Grid.GetRow(panel);
			//We currently only support vertical stacking with panel1/splitter/panel2
			int rowIdx2=0;
			if(rowIdx==0){
				rowIdx2=2;
			}
			else if(rowIdx==2){
				rowIdx2=0;
			}
			else{
				throw new Exception("Only rows 0 and 2 can be collapsed or uncollapsed,");
			}
			if(doCollapse){
				if(panel.Visibility==Visibility.Collapsed){
					return;//already collapsed
				}
				SaveOriginalHeight(rowIdx);
				RowDefinitions[rowIdx].Height=new GridLength(0);
				RowDefinitions[rowIdx2].Height=new GridLength(1,GridUnitType.Star);
				panel.Visibility=Visibility.Collapsed;
			}
			else{//expand
				if(panel.Visibility==Visibility.Visible){
					return;//already expanded
				}
				double height=GetOriginalHeight(rowIdx);
				RowDefinitions[rowIdx].Height=new GridLength(height);
				RowDefinitions[rowIdx2].Height=new GridLength(1,GridUnitType.Star);
				panel.Visibility=Visibility.Visible;
			}
		}

		private double GetOriginalHeight(int rowIdx){
			OriginalHeight originalHeight=_listOriginalHeights.Find(x=>x.RowIdx==rowIdx);
			if(originalHeight==null){
				return 0;
			}
			return originalHeight.Height;
		}

		private void SaveOriginalHeight(int rowIdx){
			OriginalHeight originalHeight=_listOriginalHeights.Find(x=>x.RowIdx==rowIdx);
			if(originalHeight!=null){
				originalHeight.Height=RowDefinitions[rowIdx].ActualHeight;
				return;
			}
			originalHeight=new OriginalHeight();
			originalHeight.RowIdx=rowIdx;
			originalHeight.Height=RowDefinitions[rowIdx].ActualHeight;
			_listOriginalHeights.Add(originalHeight);
		}

		private class OriginalHeight{
			public int RowIdx;
			public double Height;
		}
	}
}
