using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenDentBusiness;

namespace OpenDentalWpf {
	

	/// <summary></summary>
	public partial class ContrDashProvList:UserControl {
		DataTable table;
		DateTime DateShowing;

		public ContrDashProvList() {
			InitializeComponent();
			DateShowing=DateTime.Today;
			gridMain.SelectionMode=DataGridSelectionMode.Extended;
			gridMain.SelectionUnit=DataGridSelectionUnit.FullRow;
		}

		public void FillData() {
			Cursor=Cursors.Wait;
			textDate.Text=DateShowing.ToString("ddd")+" "+DateShowing.ToShortDateString();
			table=DashboardQueries.GetProvList(DateShowing);
			FillScreen();
			Cursor=Cursors.Arrow;
		}

		public List<int> SelectedIndices{
			get{
				List<int> retVal=new List<int>();
				for(int i=0;i<gridMain.SelectedItems.Count;i++){
					retVal.Add(gridMain.Items.IndexOf(gridMain.SelectedItems[i]));
				}
				return retVal;
			}
		}

		private void FillScreen() {
			List<ContrDashProvList_Row> ListProv=new List<ContrDashProvList_Row>();
			ContrDashProvList_Row row;
			for(int i=0;i<table.Rows.Count;i++){
				row=new ContrDashProvList_Row();
				row.ProvName=Providers.GetAbbr(PIn.Long(table.Rows[i]["ProvNum"].ToString()));
				System.Drawing.Color c1=Providers.GetColor(PIn.Long(table.Rows[i]["ProvNum"].ToString()));
				row.ProvColor=Color.FromArgb(c1.A,c1.R,c1.G,c1.B);
				decimal production=PIn.Decimal(table.Rows[i]["production"].ToString());
				if(production==0){
					row.Production="";
				}
				else{
					row.Production=production.ToString("c0");
				}
				decimal income=PIn.Decimal(table.Rows[i]["income"].ToString());
				if(income==0){
					row.Income="";
				}
				else{
					row.Income=income.ToString("c0");
				}
				ListProv.Add(row);
			}
			//Style style=new Style(typeof(TextBlock));
			//style.Setters.Add(new Setter(TextBlock.HorizontalAlignmentProperty,HorizontalAlignment.Right));
			//((DataGridTextColumn)gridMain.Columns[2]).ElementStyle=style;
			//((DataGridTextColumn)gridMain.Columns[2]).Binding=new Binding("Production"); 
			//((DataGridTextColumn)gridMain.Columns[2]).Binding.StringFormat="c0";
			gridMain.ItemsSource=ListProv;
		}

		private void labelR_MouseDown(object sender,MouseButtonEventArgs e) {
			DateShowing=DateShowing.AddDays(1);
			FillData();
		}

		private void labelL_MouseDown(object sender,MouseButtonEventArgs e) {
			DateShowing=DateShowing.AddDays(-1);
			FillData();
		}

		//private void gridMain_SelectionChanged(object sender,SelectionChangedEventArgs e) {
			
		//}

		

	}

	/// <summary></summary>
	public class ContrDashProvList_Row {
		/// <summary>Abbr</summary>
		public string ProvName { get; set; }
		/// <summary>Media.Color is different than Drawing.Color</summary>
		public Color ProvColor { get; set; }
		/// <summary>Abbr</summary>
		public string Production { get; set; }
		/// <summary></summary>
		public string Income { get; set; }
	}

}
