using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CodeBase;

namespace WpfControls.UI{
/*
Jordan is the only one allowed to edit this file.

How to use the MonthCalendar control:
Size in Appt Module is 227x162
We use 183 x 150 in our datePicker.
It's designed to scale all the way down to 100 x 115 and still be sort of usable.
But to look good, don't go below 142 x 115. 

*/
	///<summary></summary>
	public partial class MonthCalendar : UserControl{
		#region Fields - Public
		///<summary>Set this to false to prevent user from clicking the top text to manually set date. Useful, for example, if this monthCalendar is a dropdown, so there's already a box for that.</summary>
		public bool AllowClickingTopText=true;
		#endregion Fields - Public

		#region Fields -Private
		///<summary>The dates that are showing in each cell.  This is a 7x6 array.</summary>
		private DateTime[,] _dateTimeArray;
		private Border borderDateSelected;
		private Border borderHover;
		private Border borderToday;
		///<summary>Because we can go to different months without changing the selected date. We always use the first day of the month. Can't be changed externally.</summary>
		private DateTime _dateMonthShowing;
		///<summary></summary>
		private DateTime _dateSelected;
		private EnumHeightRange _enumHeightRange=EnumHeightRange.Tall;
		private EnumWidthRange _enumWidthRange=EnumWidthRange.Wide;
		///<summary>This is all the date numbers as well as the names of the weekdays.</summary>
		private List<TextBlock> _listTextBlocks=new List<TextBlock>();
		#endregion Fields -Private

		#region Constructor
		public MonthCalendar(){
			InitializeComponent();
			Focusable=true;
			_dateSelected=DateTime.Today;//new DateTime(2023,9,12);//September chosen because it's the longest
			_dateMonthShowing=new DateTime(_dateSelected.Year,_dateSelected.Month,1);
			borderDateSelected=new Border();
			borderDateSelected.Margin=new Thickness(2,0,2,0);
			borderDateSelected.Background=new SolidColorBrush(Color.FromRgb(70,120,180));//smokey blue to let the orange pop. (70,120,240));Pretty good. Solid pleasant blue.
			System.Windows.Controls.Grid.SetZIndex(borderDateSelected,1);
			borderDateSelected.Visibility=Visibility.Collapsed;
			gridMain.Children.Add(borderDateSelected);
			borderToday=new Border();
			//borderToday.SnapsToDevicePixels=true;//this doesn't look good
			borderToday.Margin=new Thickness(2,0,2,0);
			borderToday.BorderThickness=new Thickness(1);
			borderToday.BorderBrush=new SolidColorBrush(Color.FromRgb(255,130,0));
			System.Windows.Controls.Grid.SetZIndex(borderToday,2);
			borderToday.Visibility=Visibility.Collapsed;
			gridMain.Children.Add(borderToday);
			borderHover=new Border();
			borderHover.Background=new SolidColorBrush(Color.FromRgb(229,239,251));
			borderHover.Visibility=Visibility.Collapsed;
			//gridMain.Children.Add(borderDateSelected);
			MouseMove+=MonthCalendar_MouseMove;
			MouseLeave+=MonthCalendar_MouseLeave;
			MouseLeftButtonDown+=MonthCalendar_MouseLeftButtonDown;
			SizeChanged+=MonthCalendar_SizeChanged;
			DrawAll();
		}
		#endregion Constructor

		#region Events
		///<summary>Occurs when user clicks to change date.  Does not fire in response to programmatic data changes.</summary>
		[Category("OD")]
		[Description("Occurs when user clicks to change date.  Does not get raised in response to programmatic data changes.")]
		public event EventHandler DateChanged;
		#endregion Events

		#region Enums
		private enum EnumHeightRange{
			///<summary>From any height down to 145.</summary>
			Tall,
			///<summary>From 144 down to 135. Weekday underline moves down one pixel</summary>
			Medium,
			///<summary>From 134 down to 116 gridMain gets fixed row heights for dates</summary>
			MediumShort,
			///<summary>At 115, Everything fixed heights. Doesn't look good at less than 115.</summary>
			Short,
		}

		private enum EnumWidthRange{
			///<summary>From any width down to 200.</summary>
			ExtraWide,
			///<summary>From 199 down to 183. Buttons at top get narrower.</summary>
			Wide,
			///<summary>From 182 down to 161. Text month is shorter.</summary>
			MediumWide,
			///<summary>From 160 down to 142. Weekdays go to two letter and text month goes to two digit year.</summary>
			Medium,
			///<summary>From 141 down to 130. Today box at bottom goes away and text month goes to just numbers.</summary>
			MediumNarrow,
			///<summary>From 129 down to 100. Weekdays go to one letter.</summary>
			Narrow,
		}
		#endregion

		#region Methods - Public
		///<summary>Use this in place of MonthCalendar.SelectionStart.</summary>
		public DateTime GetDateSelected() {
			return _dateSelected;
		}

		///<summary>Use this in place of MonthCalendar.SetDate().</summary>
		public void SetDateSelected(DateTime date) {
			_dateSelected=date;
			_dateMonthShowing=new DateTime(_dateSelected.Year,_dateSelected.Month,1);
			DrawAll();
		}
		#endregion Methods - Public

		#region Methods private
		private void DrawAll(){
			//ZIndex explanation==========================================================================================
			//0-Hover effect
			//1-DateSelected blue box
			//2-Text and red Today outline
			//Fill arrayDates=============================================================================================
			_dateTimeArray=new DateTime[7,6];
			//First day of week is not Sunday in China, for example.  So:
			DayOfWeek firstDayOfWeek=DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek;
			//Which day does the first fall on?
			DayOfWeek dayOfFirst=(DayOfWeek)((int)_dateMonthShowing.DayOfWeek-(int)firstDayOfWeek);
			DateTime dateFilling=_dateMonthShowing.AddDays(-(int)dayOfFirst);
			int row=0;
			int col=0;
			while(true){
				_dateTimeArray[col,row]=dateFilling;
				dateFilling=dateFilling.AddDays(1);
				if(col==6){
					col=0;
					row++;
				}
				else{
					col++;
				}
				if(row==6){
					break;
				}
			}
			//Date selected===============================================================================================
			borderDateSelected.Visibility=Visibility.Collapsed;
			for(int c = 0;c<7;c++) {
				for(int r = 0;r<6;r++) {
					if(_dateTimeArray[c,r]!=_dateSelected) {
						continue;
					}
					borderDateSelected.Visibility=Visibility.Visible;
					System.Windows.Controls.Grid.SetRow(borderDateSelected,r+1);
					System.Windows.Controls.Grid.SetColumn(borderDateSelected,c);
					goto EndOfLoops;
				}
			}
			EndOfLoops:
			//Today outline===============================================================================================
			borderToday.Visibility=Visibility.Collapsed;
			for(int c = 0;c<7;c++) {
				for(int r = 0;r<6;r++) {
					if(_dateTimeArray[c,r]!=DateTime.Today) {
						continue;
					}
					borderToday.Visibility=Visibility.Visible;
					System.Windows.Controls.Grid.SetRow(borderToday,r+1);
					System.Windows.Controls.Grid.SetColumn(borderToday,c);
					goto EndOfLoops2;
				}
			}
			EndOfLoops2:
			//Header======================================================================================================
			//SetHeader();
			if(_enumWidthRange==EnumWidthRange.Wide || _enumWidthRange==EnumWidthRange.ExtraWide){
				textMonth.Text=_dateMonthShowing.ToString("MMMM yyyy");
			}
			else if(_enumWidthRange==EnumWidthRange.MediumWide){
				textMonth.Text=_dateMonthShowing.ToString("MMM yyyy");
			}
			else if(_enumWidthRange==EnumWidthRange.Medium){
				textMonth.Text=_dateMonthShowing.ToString("MMM")+" '"+_dateMonthShowing.ToString("yy");
			}
			else{
				textMonth.Text=_dateMonthShowing.ToString("M/yy");
			}
			//Days of week================================================================================================
			for(int i=_listTextBlocks.Count-1;i>=0;i--){
				gridMain.Children.Remove(_listTextBlocks[i]);
				_listTextBlocks[i]=null;
			}
			_listTextBlocks=new List<TextBlock>();
			for(int i = 0;i<7;i++) {
				//First day of week is not Sunday in China, for example.  So:
				int intDay = i;
				intDay+=(int)firstDayOfWeek;
				if(intDay>6) {
					intDay-=7;
				}
				TextBlock textBlock=new TextBlock();
				if(_enumWidthRange==EnumWidthRange.ExtraWide || _enumWidthRange==EnumWidthRange.Wide || _enumWidthRange==EnumWidthRange.MediumWide){
					textBlock.Text=DateTimeFormatInfo.CurrentInfo.GetAbbreviatedDayName((DayOfWeek)intDay);
				}
				if(_enumWidthRange==EnumWidthRange.Medium || _enumWidthRange==EnumWidthRange.MediumNarrow){
					textBlock.Text=GetAbbr2((DayOfWeek)intDay);
				}
				if(_enumWidthRange==EnumWidthRange.Narrow){
					textBlock.Text=GetAbbr1((DayOfWeek)intDay);
				}
				textBlock.HorizontalAlignment=HorizontalAlignment.Center;
				textBlock.VerticalAlignment=VerticalAlignment.Center;
				textBlock.TextAlignment=TextAlignment.Center;
				System.Windows.Controls.Grid.SetZIndex(textBlock,2);
				System.Windows.Controls.Grid.SetColumn(textBlock,i);
				gridMain.Children.Add(textBlock);
				_listTextBlocks.Add(textBlock);
			}
			//Dates=======================================================================================================
			for(int c = 0;c<7;c++) {
				for(int r = 0;r<6;r++) {
					TextBlock textBlock=new TextBlock();
					textBlock.Text=_dateTimeArray[c,r].ToString("%d");
					textBlock.HorizontalAlignment=HorizontalAlignment.Center;
					textBlock.VerticalAlignment=VerticalAlignment.Center;
					textBlock.TextAlignment=TextAlignment.Center;
					if(_dateTimeArray[c,r]==_dateSelected) {
						textBlock.Foreground=Brushes.White;
					}
					else if(_dateTimeArray[c,r].Month==_dateMonthShowing.Month) {
						textBlock.Foreground=Brushes.Black;
					}
					else {
						textBlock.Foreground=new SolidColorBrush(OpenDental.ColorOD.Gray_Wpf(170));
					}
					System.Windows.Controls.Grid.SetZIndex(textBlock,2);
					System.Windows.Controls.Grid.SetRow(textBlock,r+1);
					System.Windows.Controls.Grid.SetColumn(textBlock,c);
					gridMain.Children.Add(textBlock);
					_listTextBlocks.Add(textBlock);
				}
			}
			//Footer======================================================================================================
			textToday.Text=OpenDental.Lang.g("Calendar","Today:")+" "+DateTime.Today.ToShortDateString();
		}

		///<summary>Recursive. Each pass finds the immediate parent, and it keeps going until it reaches a grid.</summary>
		private DependencyObject FindParentGrid(DependencyObject dependencyObjectChild){
			if(dependencyObjectChild is System.Windows.Controls.Grid){
				return dependencyObjectChild;//self. User clicked directly on a grid instead of a child
			}
			DependencyObject dependencyObjectParent = VisualTreeHelper.GetParent(dependencyObjectChild);
			if(dependencyObjectParent==null){
				return null;
			}
			if(dependencyObjectParent is System.Windows.Controls.Grid){
				return dependencyObjectParent;
			}
			return FindParentGrid(dependencyObjectParent);
		}
		
		private string GetAbbr1(DayOfWeek dayOfWeek) {
			if(dayOfWeek==DayOfWeek.Sunday) {
				return "S"; 
			}
			if(dayOfWeek==DayOfWeek.Monday){
				return "M"; 
			}
			if(dayOfWeek==DayOfWeek.Tuesday){
				return "T"; 
			}
			if(dayOfWeek==DayOfWeek.Wednesday){
				return "W"; 
			}
			if(dayOfWeek==DayOfWeek.Thursday){
				return "T"; 
			}
			if(dayOfWeek==DayOfWeek.Friday){
				return "F"; 
			}
			if(dayOfWeek==DayOfWeek.Saturday){
				return "S"; 
			}
			return "";
		}

		private string GetAbbr2(DayOfWeek dayOfWeek) {
			if(dayOfWeek==DayOfWeek.Sunday) {
				return "Su"; 
			}
			if(dayOfWeek==DayOfWeek.Monday){
				return "Mo"; 
			}
			if(dayOfWeek==DayOfWeek.Tuesday){
				return "Tu"; 
			}
			if(dayOfWeek==DayOfWeek.Wednesday){
				return "We"; 
			}
			if(dayOfWeek==DayOfWeek.Thursday){
				return "Th"; 
			}
			if(dayOfWeek==DayOfWeek.Friday){
				return "Fr"; 
			}
			if(dayOfWeek==DayOfWeek.Saturday){
				return "Sa"; 
			}
			return "";
		}

		private int GetColumn(System.Windows.Controls.Grid grid,Point pointMouse){
			int column=0;
			double widthAccum = 0.0;
			for(int i=0;i<grid.ColumnDefinitions.Count;i++){
				widthAccum += grid.ColumnDefinitions[i].ActualWidth;
				if(widthAccum >= pointMouse.X){
					break;
				}
				column++;
			}
			return column;
		}

		private int GetRow(System.Windows.Controls.Grid grid,Point pointMouse){
			int row = 0;
			double heightAccum =0;
			if(grid.Name=="gridMain"){
				heightAccum=gridBig.RowDefinitions[0].ActualHeight;
			}
			for(int i=0;i<grid.RowDefinitions.Count;i++){
				heightAccum += grid.RowDefinitions[i].ActualHeight;
				if(heightAccum >= pointMouse.Y){
					break;
				}
				row++;
			}
			return row;
		}

		///<summary>Either returns one of the three grids or null.</summary>
		private System.Windows.Controls.Grid HitTestGrid(Point pointMouse){
			HitTestResult hitTestResult = VisualTreeHelper.HitTest(this,pointMouse);
			if(hitTestResult==null) {
				return null;
			}
			DependencyObject dependencyObjectGrid2=FindParentGrid(hitTestResult.VisualHit);
			if(dependencyObjectGrid2==null) {
				return null;//shouldn't happen
			}
			System.Windows.Controls.Grid grid=(System.Windows.Controls.Grid)dependencyObjectGrid2;
			if(!grid.Name.In("gridHeader","gridMain","gridFooter")){
				return null;//shouldn't happen
			}
			return grid;
		}

		private void MonthCalendar_MouseLeftButtonDown(object sender,MouseButtonEventArgs e) {
			Point pointMouse = e.GetPosition(this);
			System.Windows.Controls.Grid gridHit=HitTestGrid(pointMouse);
			if(gridHit==null){
				return;
			}
			int row=GetRow(gridHit,pointMouse);
			int column=GetColumn(gridHit,pointMouse);
			if(gridHit==gridMain){//click on a date
				if(row==0){//day of week
					return;
				}
				_dateSelected=_dateTimeArray[column,row-1];
				DateChanged?.Invoke(this,new EventArgs());
				if(_dateSelected.Month!=_dateMonthShowing.Month){
					_dateMonthShowing=new DateTime(_dateSelected.Year,_dateSelected.Month,1);
				}
				DrawAll();
			}
			if(gridHit==gridHeader){
				if(column==0){
					_dateMonthShowing=_dateMonthShowing.AddYears(-1);
				}
				if(column==1){
					_dateMonthShowing=_dateMonthShowing.AddMonths(-1);
				}
				if(column==2 && AllowClickingTopText) {//top text
					OpenDental.FrmDatePicker frmDatePicker = new OpenDental.FrmDatePicker();
					frmDatePicker.DateEntered=_dateSelected;
					frmDatePicker.WidthFormWPF=(int)ActualWidth;
					Point pointLocalStart=new Point(2,gridHeader.ActualHeight);
					Point pointScreenStart=PointToScreen(pointLocalStart);
					frmDatePicker.PointStartLocation=new System.Drawing.Point((int)pointScreenStart.X,(int)pointScreenStart.Y);
						//new Point(0,_rectangleTopText.Bottom+LayoutManager.Scale(5)));
					frmDatePicker.ShowDialog();
					if(frmDatePicker.IsDialogOK) {
						_dateSelected=frmDatePicker.DateEntered;
						if(_dateSelected.Month!=_dateMonthShowing.Month || _dateSelected.Year!=_dateMonthShowing.Year) {
							_dateMonthShowing=new DateTime(_dateSelected.Year,_dateSelected.Month,1);
						}
						DateChanged?.Invoke(this,new EventArgs());
					}
				}
				if(column==3){
					_dateMonthShowing=_dateMonthShowing.AddMonths(1);
				}
				if(column==4){
					_dateMonthShowing=_dateMonthShowing.AddYears(1);
				}
				DrawAll();
			}
			if(gridHit==gridFooter && column==1) {
				_dateSelected=DateTime.Today;
				DateChanged?.Invoke(this,new EventArgs());
				if(DateTime.Today.Month!=_dateMonthShowing.Month || DateTime.Today.Year!=_dateMonthShowing.Year) {
					_dateMonthShowing=new DateTime(DateTime.Today.Year,DateTime.Today.Month,1);
				}
				DrawAll();
			}
		}

		private void MonthCalendar_MouseLeave(object sender,MouseEventArgs e) {
			borderHover.Visibility=Visibility.Collapsed;
		}

		private void MonthCalendar_MouseMove(object sender,MouseEventArgs e) {
			Point pointMouse = e.GetPosition(this);
			System.Windows.Controls.Grid gridHit=HitTestGrid(pointMouse);
			if(gridHit==null){
				return;
			}
			int row=GetRow(gridHit,pointMouse);
			int column=GetColumn(gridHit,pointMouse);
			if(gridHit.Name=="gridHeader" && column==2 && !AllowClickingTopText){
				borderHover.Visibility=Visibility.Collapsed;
				return;
			}
			if(gridHit.Name=="gridMain" && row==0){
				borderHover.Visibility=Visibility.Collapsed;
				return;
			}
			if(gridHit.Name=="gridFooter"){
				if(column==0 || column==2){
					borderHover.Visibility=Visibility.Collapsed;
					return;
				}
			}
			borderHover.Visibility=Visibility.Visible;
			DependencyObject dependencyObjectGrid=FindParentGrid(borderHover);
			if(dependencyObjectGrid!=null){
				System.Windows.Controls.Grid gridParent=(System.Windows.Controls.Grid)dependencyObjectGrid;
				gridParent.Children.Remove(borderHover);
			}
			gridHit.Children.Add(borderHover);
			if(gridHit.Name=="gridHeader" && column==2){
				if(_enumWidthRange==EnumWidthRange.Narrow){
					borderHover.Margin=new Thickness(0);
				}
				else if(_enumWidthRange==EnumWidthRange.MediumNarrow || _enumWidthRange==EnumWidthRange.Medium){
					borderHover.Margin=new Thickness(0);
				}
				else{
					borderHover.Margin=new Thickness(10,0,10,3);
				}
			}
			else if(gridHit.Name=="gridMain"){
				borderHover.Margin=new Thickness(2,0,2,0);
			}
			else{
				borderHover.Margin=new Thickness(0);
			}
			System.Windows.Controls.Grid.SetRow(borderHover,row);
			System.Windows.Controls.Grid.SetColumn(borderHover,column);
		}

		private void MonthCalendar_SizeChanged(object sender,System.Windows.SizeChangedEventArgs e) {
			EnumWidthRange enumWidthRange=EnumWidthRange.ExtraWide;
			if(ActualWidth<200){
				enumWidthRange=EnumWidthRange.Wide;
			}
			if(ActualWidth<183){
				enumWidthRange=EnumWidthRange.MediumWide;
			}
			if(ActualWidth<161){
				enumWidthRange=EnumWidthRange.Medium;
			}
			if(ActualWidth<142){
				enumWidthRange=EnumWidthRange.MediumNarrow;
			}
			if(ActualWidth<130){
				enumWidthRange=EnumWidthRange.Narrow;
			}
			EnumHeightRange enumHeightRange=EnumHeightRange.Tall;
			if(ActualHeight<145){
				enumHeightRange=EnumHeightRange.Medium;
			}
			if(ActualHeight<135){
				enumHeightRange=EnumHeightRange.MediumShort;
			}
			if(ActualHeight<116){
				enumHeightRange=EnumHeightRange.Short;
			}
			if(enumWidthRange==_enumWidthRange && enumHeightRange==_enumHeightRange){
				return;
			}
			//A threshold was crossed, so redo everything
			_enumWidthRange=enumWidthRange;
			_enumHeightRange=enumHeightRange;
			//Widths:
			if(_enumWidthRange==EnumWidthRange.MediumNarrow || _enumWidthRange==EnumWidthRange.Narrow){
				borderTodayBottom.Visibility=Visibility.Collapsed;
			}
			else{
				borderTodayBottom.Visibility=Visibility.Visible;
			}
			if(_enumWidthRange==EnumWidthRange.ExtraWide){
				gridHeader.ColumnDefinitions[0].Width=new GridLength(25);
				gridHeader.ColumnDefinitions[1].Width=new GridLength(25);
				gridHeader.ColumnDefinitions[2].Width=new GridLength(1,GridUnitType.Star);
				gridHeader.ColumnDefinitions[3].Width=new GridLength(25);
				gridHeader.ColumnDefinitions[4].Width=new GridLength(25);
				canvasPoly1.Margin=new Thickness(-9,0,0,0);
				canvasPoly2.Margin=new Thickness(-15,0,0,0);
				canvasPoly3.Margin=new Thickness(12,0,0,0);
				canvasPoly4.Margin=new Thickness(10,0,0,0);
			}
			else {
				gridHeader.ColumnDefinitions[0].Width=new GridLength(18);
				gridHeader.ColumnDefinitions[1].Width=new GridLength(18);
				gridHeader.ColumnDefinitions[2].Width=new GridLength(1,GridUnitType.Star);
				gridHeader.ColumnDefinitions[3].Width=new GridLength(18);
				gridHeader.ColumnDefinitions[4].Width=new GridLength(18);
				canvasPoly1.Margin=new Thickness(-5,0,0,0);
				canvasPoly2.Margin=new Thickness(-11,0,0,0);
				canvasPoly3.Margin=new Thickness(8,0,0,0);
				canvasPoly4.Margin=new Thickness(6,0,0,0);
			}
			//Heights:
			if(_enumHeightRange==EnumHeightRange.Tall){
				borderUnderDays.Margin=new Thickness(0);
			}
			else{
				borderUnderDays.Margin=new Thickness(0,0,0,-1);//move down one pixel
			}
			if(_enumHeightRange==EnumHeightRange.Tall || _enumHeightRange==EnumHeightRange.Medium){
				gridBig.RowDefinitions[0].Height=new GridLength(29);
				gridBig.RowDefinitions[1].Height=new GridLength(1,GridUnitType.Star);
				gridBig.RowDefinitions[2].Height=new GridLength(21);
			}
			if(_enumHeightRange==EnumHeightRange.MediumShort){
				gridBig.RowDefinitions[0].Height=new GridLength(29,GridUnitType.Star);
				gridBig.RowDefinitions[1].Height=new GridLength(84);
				gridBig.RowDefinitions[2].Height=new GridLength(21,GridUnitType.Star);
			}
			if(_enumHeightRange==EnumHeightRange.Short){
				gridBig.RowDefinitions[0].Height=new GridLength(15);
				gridBig.RowDefinitions[1].Height=new GridLength(84);
				gridBig.RowDefinitions[2].Height=new GridLength(16);
			}
			DrawAll();
		}
		#endregion Methods private

	}

	
}
