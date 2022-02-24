using OpenDentalGraph.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace OpenDentalGraph {
	public partial class GraphIncome:UserControl {

		#region Private Data
		private List<DataPoint> _rawData=new List<DataPoint>();
		private Dictionary<DateTime,double> _formattedData=new Dictionary<DateTime,double>();
		private IntervalType _selectedGroupBy {
			get {
				return comboGroupBy.SelectedItem==null?IntervalType.Days:(IntervalType)((ComboItemIntValue)comboGroupBy.SelectedItem).Value;
			}
		}
		private DateTimeIntervalType _selectedMajorGridInterval {
			get {
				return (DateTimeIntervalType)Enum.Parse(typeof(DateTimeIntervalType),_selectedGroupBy.ToString(),true);
			}
		}
		private string _dateFormat {
			get {
				switch(_selectedGroupBy) {
					case IntervalType.Months:
						return "MMM yyyy";
					case IntervalType.Years:
						return "yyyy";
					case IntervalType.Weeks:
					case IntervalType.Days:
					default:
						return "M/dd/yy";
				}
			}
		}
		private int _groupInterval {
			get {
				switch(_selectedGroupBy) {
					case IntervalType.Months:
					case IntervalType.Years:
						return 1;
					case IntervalType.Weeks:
						return 2;
					case IntervalType.Days:
					default:
						return 7;
				}
			}
		}
		private ChartArea _chartAreaGroup {
			get {
				return chart1.ChartAreas["Group"];
			}
		}
		private ChartArea _chartAreaDefault {
			get {
				return chart1.ChartAreas["Default"];
			}
		}
		#endregion

		#region Public Properties
		public int DailyLabelsShown {
			get { return (int)numberDailyLabelDensity.Value; }
			set { numberDailyLabelDensity.Value=value; UpdateIntervals(); }
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public List<DataPoint> RawData {
			get { return _rawData; }
			set {
				_rawData=value;
				_formattedData=_rawData
					.OrderBy(x => x.DateStamp)
					.GroupBy(x => new { x.DateStamp.Year,x.DateStamp.Month,x.DateStamp.Day })
					.ToDictionary(x => new DateTime(x.Key.Year,x.Key.Month,x.Key.Day),x => x.Sum(y => y.Val));
				DateTime minDate=DateTime.Now;
				DateTime maxDate=DateTime.Now;
				if(_rawData.Count>0) {
					minDate=_rawData.Min(x => x.DateStamp);
					maxDate=_rawData.Max(x => x.DateStamp);
				}
				dateTimeFrom.MinDate=minDate;
				dateTimeFrom.MaxDate=maxDate;
				dateTimeTo.MinDate=minDate;
				dateTimeTo.MaxDate=maxDate;
				dateTimeFrom.Value=minDate;
				dateTimeTo.Value=maxDate;
				FilterData();
			}
		}

		public Dictionary<DateTime,double> FormattedData {
			get { return _formattedData; }
		}

		public string ChartTitle {
			get { return chart1.Titles["ChartTitle"].Text; }
			set { chart1.Titles["ChartTitle"].Text=value; }
		}
		#endregion

		#region Ctor
		public GraphIncome() {
			InitializeComponent();			
			comboGroupBy.SetDataToEnums<IntervalType>(false,false,(int)IntervalType.Years,(int)IntervalType.Weeks);
			comboGroupBy.SelectedIndex=2;
		}
		#endregion

		private void FilterData() {
			Dictionary<DateTime,double> filteredData=_rawData
				.Where(x => x.DateStamp.Date>=dateTimeFrom.Value.Date && x.DateStamp.Date<=dateTimeTo.Value.Date)
				.OrderBy(x => x.DateStamp)
				.GroupBy(x => new { x.DateStamp.Year,x.DateStamp.Month,x.DateStamp.Day })
				.ToDictionary(x => new DateTime(x.Key.Year,x.Key.Month,x.Key.Day),x => x.Sum(y => y.Val));
			chart1.Series["Daily"].Points.DataBindXY(filteredData.Keys,filteredData.Values);
			UpdateChart();
		}

		public void SetFakeData(DateTime dateStart,DateTime dateEnd,double maxValuePerDay) {
			Random random = new Random();
			int i=0;
			List<DataPoint> data=new List<DataPoint>();
			int daysBack=(int)dateEnd.Subtract(dateStart).TotalDays;
			while(i++<daysBack) {
				data.Add(new DataPoint() { DateStamp=dateStart.AddDays(-daysBack).AddDays(i),Val=random.NextDouble()*maxValuePerDay });
			}
			RawData=data;			
		}
					
		private void UpdateChart() {
			chart1.DataManipulator.Group("SUM, X:FIRST",1,_selectedGroupBy,"Daily","Group"); 			
			_chartAreaGroup.AxisX.LabelStyle.Format = _dateFormat;
			_chartAreaGroup.AxisX.IntervalType = _selectedMajorGridInterval;			
			_chartAreaDefault.AxisX.ScaleView.ZoomReset();
			chart1.Titles["titleGroup"].Text="Grouped By "+comboGroupBy.Text;
			checkShowDailyLabels_CheckedChanged(this,new EventArgs());
			checkShowGroupLabels_CheckedChanged(this,new EventArgs());
			UpdateIntervals();			
		}

		private void UpdateIntervals() {
			double viewable=_chartAreaDefault.AxisX.ScaleView.ViewMaximum-_chartAreaDefault.AxisX.ScaleView.ViewMinimum;
			if(Double.IsNaN(viewable)) {
				viewable=chart1.Series["Daily"].Points.Count;			
			}
			double interval=Math.Ceiling(Math.Max(1,viewable/DailyLabelsShown));
			_chartAreaDefault.AxisX.LabelStyle.Interval=interval;
			_chartAreaDefault.AxisX.Interval=interval;
		}

		private void comboGroupBy_SelectedIndexChanged(object sender,EventArgs e) {
			UpdateChart();
		}

		private void checkShowDailyLabels_CheckedChanged(object sender,EventArgs e) {
			chart1.Series["Daily"].IsValueShownAsLabel=checkShowDailyLabels.Checked;
		}

		private void checkShowGroupLabels_CheckedChanged(object sender,EventArgs e) {
			chart1.Series["Group"].IsValueShownAsLabel=checkShowGroupLabels.Checked;
		}

		private void chart1_AxisViewChanged(object sender,ViewEventArgs e) {
			UpdateIntervals();	
		}
		
		public class DataPoint {
			public DateTime DateStamp;
			public double Val;
		}

		private void numberDailyLabelDensity_ValueChanged(object sender,EventArgs e) {
			UpdateIntervals();
		}

		private void dateTimeFilter_ValueChanged(object sender,EventArgs e) {
			FilterData();
		}
	}
}
