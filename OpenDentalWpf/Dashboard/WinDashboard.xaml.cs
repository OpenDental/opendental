using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Printing;
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
using System.Windows.Markup;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using OpenDentBusiness;
using System.Linq;

namespace OpenDentalWpf {
	/// <summary></summary>
	public partial class WinDashboard:Window {

		public WinDashboard() {
			InitializeComponent();
		}

		private void Window_Loaded(object sender,RoutedEventArgs e) {
			contrDashProvList.FillData();
			//prodProvs
			List<Color> listColorsProd=DashboardQueries.GetProdProvColors();
			List<List<int>> listDataProd=DashboardQueries.GetProdProvs(contrDashProdProvs.DateStart,contrDashProdProvs.DateEnd);
			contrDashProdProvs.FillData(Lans.g(this,"Production by Prov"),1000,listColorsProd,listDataProd);
			//A/R
			List<Color> listColorsAR=new List<Color>();
			listColorsAR.Add(Colors.Firebrick);
			List<DashboardAR> listDashARIn=DashboardARs.Refresh(contrDashAR.DateStart);
			if(listDashARIn.Count==0) {
				//Make a guess as to how long the user might have to wait.
				double agingInMilliseconds=Ledgers.GetAgingComputationTime();
				//Aging will be run a total of 13 times.
				agingInMilliseconds=agingInMilliseconds*13;
				TimeSpan timeSpan=TimeSpan.FromMilliseconds(agingInMilliseconds);
				string timeEstimate="";
				if(timeSpan.Minutes>0) {
					timeEstimate+=timeSpan.Minutes+" "+Lans.g(this,"minutes and")+" ";
				}
				timeEstimate+=timeSpan.Seconds+" "+Lans.g(this,"seconds");
				MessageBoxResult result=MessageBox.Show(Lans.g(this,
					"A one-time routine needs to be run that will take about")+"\r\n"
					+timeEstimate+".  "+Lans.g(this,"Continue?"),"",MessageBoxButton.OKCancel);
				if(result!=MessageBoxResult.OK) {
					Close();
					return;
				}
			}
			List<DashboardAR> listDashAROut=DashboardQueries.GetAR(contrDashAR.DateStart,contrDashAR.DateEnd,listDashARIn);
			List<List<int>> listDataAR=new List<List<int>>();
			//1 dimensional for now.
			listDataAR.Add(listDashAROut.OrderBy(x => x.DateCalc).Select(x => (int)x.BalTotal).ToList());
			contrDashAR.FillData(Lans.g(this,"Accounts Receivable"),1000,listColorsAR,listDataAR);
			//ProdInc
			contrDashProdInc.FillData();
			//new pat
			List<Color> listColorsNP=new List<Color>();
			listColorsNP.Add(Colors.Chocolate);
			List<List<int>> listDataNP=DashboardQueries.GetNewPatients(contrDashNewPat.DateStart,contrDashNewPat.DateEnd);
			contrDashNewPat.FillData(Lans.g(this,"New Patients"),1,listColorsNP,listDataNP);
		}

		private void Window_Activated(object sender,EventArgs e) {
			
		}

		private void contrDashProvList_SelectionChanged(object sender,SelectionChangedEventArgs e) {
			contrDashProdProvs.VisibleIndices=contrDashProvList.SelectedIndices;
		}

		private void butPrint_Click(object sender,RoutedEventArgs e) {
			//move this first section, including the dlg into PrintHelper, analogous to OpenDental.PrinterL.  Or maybe into OpenDentalWpf.PrinterL?
			PrintDialog dlg=new PrintDialog();
			PrintQueue pq=LocalPrintServer.GetDefaultPrintQueue();
			PrintTicket tick=pq.DefaultPrintTicket;
			tick.PageOrientation=PageOrientation.Landscape;
			dlg.PrintTicket=tick;
			dlg.PrintQueue=pq;
			if(dlg.ShowDialog()!=true){
				return;
			}
			FixedDocument document=new FixedDocument();
			document.PrintTicket=dlg.PrintTicket;
			document.DocumentPaginator.PageSize=new Size(dlg.PrintableAreaWidth, dlg.PrintableAreaHeight);
			Canvas canvas1=PrintHelper.GetCanvas(document);
			//set up a grid for printing that's the same as the main grid except for the bottom section with the buttons
			Grid gridPrint=new Grid();
			gridPrint.Width=906;
			gridPrint.Height=603;
			//5 columns 
			gridPrint.ColumnDefinitions.Add(new ColumnDefinition());
			ColumnDefinition colDef=new ColumnDefinition();
			colDef.Width=new GridLength(3);
			gridPrint.ColumnDefinitions.Add(colDef);
			gridPrint.ColumnDefinitions.Add(new ColumnDefinition());
			colDef=new ColumnDefinition();
			colDef.Width=new GridLength(3);
			gridPrint.ColumnDefinitions.Add(colDef);
			gridPrint.ColumnDefinitions.Add(new ColumnDefinition());
			//3 rows
			gridPrint.RowDefinitions.Add(new RowDefinition());
			RowDefinition rowDef=new RowDefinition();
			rowDef.Height=new GridLength(3);
			gridPrint.RowDefinitions.Add(rowDef);
			gridPrint.RowDefinitions.Add(new RowDefinition());
			//draw rectangles to separate sections
			//3 vert:
			Rectangle rect;
			rect=new Rectangle();
			rect.Fill=Brushes.LightGray;
			rect.Width=3;
			rect.Height=300;
			Grid.SetRow(rect,0);
			Grid.SetColumn(rect,1);
			gridPrint.Children.Add(rect);
			rect=new Rectangle();
			rect.Fill=Brushes.LightGray;
			rect.Width=3;
			rect.Height=3;
			Grid.SetRow(rect,1);
			Grid.SetColumn(rect,1);
			gridPrint.Children.Add(rect);
			rect=new Rectangle();
			rect.Fill=Brushes.LightGray;
			rect.Width=3;
			rect.Height=300;
			Grid.SetRow(rect,2);
			Grid.SetColumn(rect,1);
			gridPrint.Children.Add(rect);
			//1 horiz
			rect=new Rectangle();
			rect.Fill=Brushes.LightGray;
			rect.Width=300;
			rect.Height=3;
			Grid.SetRow(rect,1);
			Grid.SetColumn(rect,2);
			gridPrint.Children.Add(rect);
			//3 more vert:
			rect=new Rectangle();
			rect.Fill=Brushes.LightGray;
			rect.Width=3;
			rect.Height=300;
			Grid.SetRow(rect,0);
			Grid.SetColumn(rect,3);
			gridPrint.Children.Add(rect);
			rect=new Rectangle();
			rect.Fill=Brushes.LightGray;
			rect.Width=3;
			rect.Height=3;
			Grid.SetRow(rect,1);
			Grid.SetColumn(rect,3);
			gridPrint.Children.Add(rect);
			rect=new Rectangle();
			rect.Fill=Brushes.LightGray;
			rect.Width=3;
			rect.Height=300;
			Grid.SetRow(rect,2);
			Grid.SetColumn(rect,3);
			gridPrint.Children.Add(rect);
			//1 more horiz
			rect=new Rectangle();
			rect.Fill=Brushes.LightGray;
			rect.Width=300;
			rect.Height=3;
			Grid.SetRow(rect,1);
			Grid.SetColumn(rect,4);
			gridPrint.Children.Add(rect);
			//add the grid to the canvas
			canvas1.Children.Add(gridPrint);
			double center=canvas1.Width/2d;
			Canvas.SetLeft(gridPrint,(canvas1.Width/2d)-(gridPrint.Width/2));
			//draw a rectangle around the entire grid
			rect=new Rectangle();
			rect.Stroke=Brushes.DarkGray;
			rect.StrokeThickness=1;
			rect.Width=906;
			rect.Height=603;
			Canvas.SetLeft(rect,(canvas1.Width/2d)-(rect.Width/2));
			canvas1.Children.Add(rect);
			//add the five dashboard controls
			gridMain.Children.Remove(contrDashProvList);
			gridPrint.Children.Add(contrDashProvList);
			gridMain.Children.Remove(contrDashProdProvs);
			gridPrint.Children.Add(contrDashProdProvs);
			gridMain.Children.Remove(contrDashAR);
			gridPrint.Children.Add(contrDashAR);
			gridMain.Children.Remove(contrDashProdInc);
			gridPrint.Children.Add(contrDashProdInc);
			gridMain.Children.Remove(contrDashNewPat);
			gridPrint.Children.Add(contrDashNewPat);
			//Canvas.SetTop(contrDashProdInc,
			#if DEBUG
				WinPrintPreview pp=new WinPrintPreview();
				pp.Owner=this;
				pp.Document=document;
				//warning! Only use the print preview in debug.  It will crash if your mouse moves into the top toolbar.
				pp.ShowDialog();
			#else
				//dlg.PrintDocument(document.DocumentPaginator,"Dashboard");//old
				XpsDocumentWriter writer = PrintQueue.CreateXpsDocumentWriter(dlg.PrintQueue);
				Cursor=Cursors.Wait;
				writer.Write(document, dlg.PrintTicket);//use WriteAsynch usually, but we can't here because we "borrowed" the controls from the screen.
				Cursor=Cursors.Arrow;
			#endif
			//myPanel.Measure(new Size(dialog.PrintableAreaWidth,dialog.PrintableAreaHeight));
			//myPanel.Arrange(new Rect(new Point(0, 0),myPanel.DesiredSize));
			//dlg.PrintVisual(gridMain,"Dashboard");
			gridPrint.Children.Remove(contrDashProvList);
			gridMain.Children.Add(contrDashProvList);
			gridPrint.Children.Remove(contrDashProdProvs);
			gridMain.Children.Add(contrDashProdProvs);
			gridPrint.Children.Remove(contrDashAR);
			gridMain.Children.Add(contrDashAR);
			gridPrint.Children.Remove(contrDashProdInc);
			gridMain.Children.Add(contrDashProdInc);
			gridPrint.Children.Remove(contrDashNewPat);
			gridMain.Children.Add(contrDashNewPat);
		}

		private void butClose_Click(object sender,RoutedEventArgs e) {
			Close();
		}

		



	}
}

