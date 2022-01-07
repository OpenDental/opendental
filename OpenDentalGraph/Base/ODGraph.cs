using CodeBase;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace OpenDentalGraph {
	///<summary>Must be implemented in order to facilitate printing from dashboard.</summary>
	public interface IODGraphPrinter {
		void PrintPreview();
	}

	///<summary>Legend Items have their own unique class. ODGraphLegendItems can be clicked to disable/re-enable their series' in the graph.</summary>
	public class ODGraphLegendItem {
		///<summary>Static variable that increments every time a new instance of this class is created.</summary>
		private static long _idCounter;

		///<summary>Unique ID for this legend item.</summary>
		public long ID { get; private set; }

		///<summary>SeriesName that shows in the legend.</summary>
		public string ItemName { get; set; }

		///<summary>Color associated to this legend item.</summary>
		public Color ItemColor { get; set; }

		///<summary>Tells the control whether or not to grey this item out and temporarily exclude it from the graph.</summary>
		public bool IsEnabled { get; set; }

		///<summary>The location of the colored box indicating this legendItem's color. Does not include the text.</summary>
		public Rectangle LocationBox { get; set; }

		///<summary>Not really used for anything right now.</summary>
		public object Tag { get; set; }

		///<summary>For ordering. The value associated to this legendItem. 
		///This is set to the sum of the values for all the points associated to this series.</summary>
		public double Val { get; set; }

		///<summary>Tells the control whether or not to show the background hovered box.</summary>
		public bool Hovered { get; set; }

		///<summary>Delegate for filtering the legend when the user clicks.</summary>
		public FilterLegend OnFilterLegend;
		public delegate void FilterLegend(ODGraphLegendItem legendItem);

		///<summary>Called to filter the legend and chart when the user disables or re-enables this legendItem.</summary>
		public void Filter() {
			if(OnFilterLegend!=null) {
				OnFilterLegend(this);
			}
		}

		///<summary>Constructor, increments the ID counter.</summary>
		public ODGraphLegendItem() {
			ID=_idCounter++;
		}
	}

	///<summary>Wraps JSON serialization. Any class that extends this class should NEVER change variable names after being released. 
	///The variable names are stored as plain-text json and neeed to remain back-compatible.
	///Adding new fields is OK.</summary>
	public class ODGraphSettingsBase {
		public string Title { get; set; }
		public string SubTitle { get; set; }
		public Enumerations.QuickRange QuickRangePref { get; set; }
		public DateTime DateFrom { get; set; }
		public DateTime DateTo { get; set; }
		///<summary>Helper property to give easy access to the current DashboardFilter settings for this graph.</summary>
		public Cache.DashboardFilter Filter {
			get {
				return new Cache.DashboardFilter() { DateTo=this.DateTo,DateFrom=this.DateFrom,UseDateFilter=this.QuickRangePref!=Enumerations.QuickRange.allTime };
			}
		}

		///<summary>Helper method to convert from json string to ODGraphBaseSettingsAbs. Most importantly, provides filter settings for a given DashboardCell.</summary>
		public static ODGraphSettingsBase Deserialize(string json) {
			ODGraphSettingsBase ret=JsonConvert.DeserializeObject<ODGraphSettingsBase>(json);
			Cache.DashboardFilter filter=GetDatesFromQuickRange(ret.QuickRangePref,ret.DateFrom,ret.DateTo);
			ret.DateFrom=filter.DateFrom;
			ret.DateTo=filter.DateTo;
			return ret;
		}

		///<summary>If quickRange==QuickRange.custom then return a filter containing customDateFrom and customDateTo. 
		///In all other cases, the date range will be calculated given the quickRange.</summary>
		public static Cache.DashboardFilter GetDatesFromQuickRange(Enumerations.QuickRange quickRange,DateTime customDateFrom,DateTime customDateTo) {
			Cache.DashboardFilter filter=new Cache.DashboardFilter();
			switch(quickRange) {
				case Enumerations.QuickRange.custom:
					filter.DateTo=customDateTo;
					filter.DateFrom=customDateFrom;
					break;
				case Enumerations.QuickRange.last7Days:
					filter.DateTo=DateTime.Today;
					filter.DateFrom=filter.DateTo.AddDays(-7);
					break;
				case Enumerations.QuickRange.last30Days:
					filter.DateTo=DateTime.Today;
					filter.DateFrom=filter.DateTo.AddDays(-30);
					break;
				case Enumerations.QuickRange.last365Days:
					filter.DateTo=DateTime.Today;
					filter.DateFrom=filter.DateTo.AddDays(-365);
					break;
				case Enumerations.QuickRange.last12Months:
					filter.DateTo=new DateTime(DateTime.Today.Year,DateTime.Today.Month,1).AddDays(-1);
					filter.DateFrom=filter.DateTo.AddMonths(-12).AddDays(1);
					break;
				case Enumerations.QuickRange.previousWeek:
					filter.DateFrom=DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek).AddDays(-7);
					filter.DateTo=filter.DateFrom.AddDays(7);
					break;
				case Enumerations.QuickRange.previousMonth:
					filter.DateFrom=new DateTime(DateTime.Today.Year,DateTime.Today.Month,1).AddMonths(-1);
					filter.DateTo=filter.DateFrom.AddMonths(1);
					break;
				case Enumerations.QuickRange.previousYear:
					filter.DateFrom=new DateTime(DateTime.Today.Year-1,1,1);
					filter.DateTo=filter.DateFrom.AddYears(1);
					break;
				case Enumerations.QuickRange.thisWeek:
					filter.DateFrom=DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
					filter.DateTo=filter.DateFrom.AddDays(7);
					break;
				case Enumerations.QuickRange.thisMonth:
					filter.DateFrom=new DateTime(DateTime.Today.Year,DateTime.Today.Month,1);
					filter.DateTo=filter.DateFrom.AddMonths(1);
					break;
				case Enumerations.QuickRange.thisYear:
					filter.DateFrom=new DateTime(DateTime.Today.Year,1,1);
					filter.DateTo=filter.DateFrom.AddYears(1);
					break;
				case Enumerations.QuickRange.weekToDate:
					filter.DateFrom=DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
					filter.DateTo=DateTime.Today;
					break;
				case Enumerations.QuickRange.monthToDate:
					filter.DateFrom=new DateTime(DateTime.Today.Year,DateTime.Today.Month,1);
					filter.DateTo=DateTime.Today;
					break;
				case Enumerations.QuickRange.yearToDate:
					filter.DateFrom=new DateTime(DateTime.Today.Year,1,1);
					filter.DateTo=DateTime.Today;
					break;
				case Enumerations.QuickRange.allTime:
					filter.DateFrom=new DateTime(1880,1,1);
					filter.DateTo=filter.DateFrom.AddYears(300);
					filter.UseDateFilter=false;
					break;
				default:
					throw new Exception("Unsupported QuickRange: "+quickRange.ToString());
			}
			return filter;
		}

		public static string Serialize(ODGraphSettingsBase obj) {
			return JsonConvert.SerializeObject(obj);
		}
		public static T Deserialize<T>(string json) {
			return JsonConvert.DeserializeObject<T>(json);
		}
	}

	///<summary>Wrapper class which allows for 2 json fields. 1 for the graph and 1 for the filter.</summary>
	public class ODGraphJson {
		public string GraphJson;
		public string FilterJson;

		public static string Serialize(ODGraphJson obj) {
			return JsonConvert.SerializeObject(obj);
		}

		public static ODGraphJson Deserialize(string json) {
			return JsonConvert.DeserializeObject<ODGraphJson>(json);
		}
	}	
}

namespace OpenDentalGraph.Enumerations {
	public enum QuickRange {
		///<summary>0</summary>
		allTime,
		///<summary>1</summary>
		custom,
		///<summary>2</summary>
		thisWeek,
		///<summary>3</summary>
		weekToDate,
		///<summary>4</summary>
		thisMonth,
		///<summary>5</summary>
		monthToDate,
		///<summary>6</summary>
		thisYear,
		///<summary>7</summary>
		yearToDate,
		///<summary>8</summary>
		previousWeek,
		///<summary>9</summary>
		previousMonth,
		///<summary>10</summary>
		previousYear,
		///<summary>11</summary>
		last7Days,
		///<summary>12</summary>
		last30Days,
		///<summary>13</summary>
		last365Days,
		///<summary>14</summary>
		last12Months,
	}

	public enum BreakdownType {
		///<summary>0 - Show every series as it's own series. Do not group.</summary>
		all,
		///<summary>1 - Show only 1 series as the group of all series combined.</summary>
		none,
		///<summary>2 - Show top x items each in their own series where x is defined by BreakdownVal. All remaining series will be grouped as one series.</summary>
		items,
		///<summary>3 - Show top x percentage of items each in their own series where x is defined by BreakdownVal. All remaining series will be grouped as one series.</summary>
		percent
	}

	public enum QuantityType {
		///<summary>0</summary>
		money,
		///<summary>1</summary>
		count,
		///<summary>2</summary>
		decimalPoint,
	}

	public enum LegendDockType {
		Bottom,
		Left,
		None
	}

	///<summary>HQ only. Broadcast monitor.</summary>
	public enum ConfirmationGraphType {
		ByReason,
		ByResponseTime
	}

}
