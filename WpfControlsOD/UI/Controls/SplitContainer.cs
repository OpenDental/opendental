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
Google WPF GridSplitter for more hints on how to use it.
Copy and paste the XAML code below, and then edit:
		<ui:SplitContainer Margin="30,30,0,0" Width="400" Height="400">
			<ui:SplitContainer.RowDefinitions>
				<RowDefinition/>
				<RowDefinition Height="Auto" />
				<RowDefinition/>
			</ui:SplitContainer.RowDefinitions>
			<Border BorderBrush="#FFC1C0C0" BorderThickness="1" Grid.RowSpan="3"/>
			<GridSplitter Grid.Row="1" Height="5" HorizontalAlignment="Stretch" Background="Silver"/>
			<ui:Panel HorizontalAlignment="Stretch" VerticalAlignment="Stretch">
				<!--Controls here-->
			</ui:Panel>
			<ui:Panel Grid.Row="2"  HorizontalAlignment="Stretch" VerticalAlignment="Stretch">
				<!--Controls here-->
			</ui:Panel>
		</ui:SplitContainer>
*/
	public class SplitContainer:System.Windows.Controls.Grid {
		static SplitContainer(){
			//This is just to get the default alignment set to UL 
			DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitContainer), new FrameworkPropertyMetadata(typeof(SplitContainer)));
			//WidthProperty.OverrideMetadata(typeof(SplitContainer), new FrameworkPropertyMetadata(400.0));
			//HeightProperty.OverrideMetadata(typeof(SplitContainer), new FrameworkPropertyMetadata(400.0));
		}
	}
}
