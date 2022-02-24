using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormMassEmailAnalytics:FormODBase {
		///<summary>The lower bound of the analytics date range.</summary>
		private DateTime _dateTimeAnalyticsFrom {
			get { return dateRangeAnalytics.GetDateTimeFrom(); }
			set { dateRangeAnalytics.SetDateTimeFrom(value); }
		}

		///<summary>The upper bound of the analytics date range.</summary>
		private DateTime _dateTimeAnalyticsTo {
			get { return dateRangeAnalytics.GetDateTimeTo(); }
			set { dateRangeAnalytics.SetDateTimeTo(value); }
		}

		public FormMassEmailAnalytics() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormMassEmailAnalytics_Load(object sender,EventArgs e) {
			//The default date time's do not get set until after Form.OnLoad. However, the first time we fill the analytics grid
			//is on load and we look at the date range's properties so we set both.
			dateRangeAnalytics.DefaultDateTimeFrom=DateTime.Now.AddMonths(-1).Date;
			dateRangeAnalytics.DefaultDateTimeTo=DateTime.Now.Date;
			_dateTimeAnalyticsFrom=dateRangeAnalytics.DefaultDateTimeFrom;
			_dateTimeAnalyticsTo=dateRangeAnalytics.DefaultDateTimeTo;
			FillGridAnalytics();
			UpdateClinicActivated();
		}

		private void comboClinicAnalytics_SelectionChangeCommitted(object sender,EventArgs e) {
			UpdateClinicActivated();
		}

		private void UpdateClinicActivated() {
			long clinicNum=comboClinicAnalytics.SelectedClinicNum;
			labelNotActivated.Visible=!Clinics.IsMassEmailSignedUp(clinicNum) && !Clinics.IsSecureEmailSignedUp(clinicNum);
		}

		///<summary>Fills the main grid on the analytics tab. Hits the database everytime this method is called.</summary>
		private void FillGridAnalytics() {
			gridAnalytics.BeginUpdate();
			if(gridAnalytics.ListGridColumns.Count==0) {		
				GridColumn col=new GridColumn(Lans.g(gridAnalytics.TranslationName,"Name"),100,GridSortingStrategy.StringCompare);
				gridAnalytics.ListGridColumns.Add(col);
				col=new GridColumn(Lans.g(gridAnalytics.TranslationName,"Type"),100,GridSortingStrategy.StringCompare);
				gridAnalytics.ListGridColumns.Add(col);
				if(PrefC.HasClinicsEnabled) {
					col=new GridColumn(Lans.g(gridAnalytics.TranslationName,"Clinic"),100,GridSortingStrategy.StringCompare);
					gridAnalytics.ListGridColumns.Add(col);
				}
				col=new GridColumn(Lans.g(gridAnalytics.TranslationName,"Date Created"),100,GridSortingStrategy.DateParse);
				gridAnalytics.ListGridColumns.Add(col);
				col=new GridColumn(Lans.g(gridAnalytics.TranslationName,"Total"),85,HorizontalAlignment.Center);
				gridAnalytics.ListGridColumns.Add(col);
				col=new GridColumn(Lans.g(gridAnalytics.TranslationName,"Pending"),85,HorizontalAlignment.Center);
				gridAnalytics.ListGridColumns.Add(col);
				col=new GridColumn(Lans.g(gridAnalytics.TranslationName,"Unopened"),85,HorizontalAlignment.Center);
				gridAnalytics.ListGridColumns.Add(col);
				col=new GridColumn(Lans.g(gridAnalytics.TranslationName,"Opened"),85,HorizontalAlignment.Center);
				gridAnalytics.ListGridColumns.Add(col);
				col=new GridColumn(Lans.g(gridAnalytics.TranslationName,"Bounced"),85,HorizontalAlignment.Center);
				gridAnalytics.ListGridColumns.Add(col);
				col=new GridColumn(Lans.g(gridAnalytics.TranslationName,"Complaint"),85,HorizontalAlignment.Center);
				gridAnalytics.ListGridColumns.Add(col);
				col=new GridColumn(Lans.g(gridAnalytics.TranslationName,"Failed"),85,HorizontalAlignment.Center);
				gridAnalytics.ListGridColumns.Add(col);
				col=new GridColumn(Lans.g(gridAnalytics.TranslationName,"Unsubscribed"),85,HorizontalAlignment.Center);
				gridAnalytics.ListGridColumns.Add(col);
			}
			gridAnalytics.ListGridRows.Clear();
			long clinicNum=PrefC.HasClinicsEnabled ? comboClinicAnalytics.SelectedClinicNum : -1;
			List<PromotionAnalytic> listAnalytics=Promotions.GetAnalytics(_dateTimeAnalyticsFrom,_dateTimeAnalyticsTo,clinicNum)
				.OrderByDescending(x => x.Promotion.DateTimeCreated)
				.ThenByDescending(x => x.Promotion.PromotionNum)
				.ToList();
			string GetCountString(PromotionAnalytic analytic,int total,params PromotionLogStatus[] statuses) {
				int count=0;
				foreach(PromotionLogStatus status in statuses) {
					//If this is not in the dictionary, count will be 0.
					analytic.DictionaryCounts.TryGetValue(status,out int countStatus);
					count+=countStatus;
				}
				return $"{count} \r\n({((float)count/total*100).ToString("N")})%";
			}
			foreach(PromotionAnalytic analytic in listAnalytics) {
				GridRow row=new GridRow();
				row.Cells.Add(analytic.Promotion.PromotionName);
				row.Cells.Add(analytic.Promotion.TypePromotion.GetDescription());
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add(Clinics.GetDesc(analytic.Promotion.ClinicNum));
				}
				row.Cells.Add(analytic.Promotion.DateTimeCreated.ToShortDateString());
				//Combine all statuses together to get the total.
				int total=analytic.DictionaryCounts.Sum(x => x.Value);
				row.Cells.Add(total.ToString());
				row.Cells.Add(GetCountString(analytic,total,PromotionLogStatus.Pending));
				row.Cells.Add(GetCountString(analytic,total,PromotionLogStatus.Delivered));
				row.Cells.Add(GetCountString(analytic,total,PromotionLogStatus.Opened));
				row.Cells.Add(GetCountString(analytic,total,PromotionLogStatus.Bounced));
				row.Cells.Add(GetCountString(analytic,total,PromotionLogStatus.Complaint));
				row.Cells.Add(GetCountString(analytic,total,PromotionLogStatus.Failed));
				row.Cells.Add(GetCountString(analytic,total,PromotionLogStatus.Unsubscribed));
				gridAnalytics.ListGridRows.Add(row);
			}
			gridAnalytics.EndUpdate();
		}

		private void butRefreshAnalytics_Click(object sender,EventArgs e) {
			FillGridAnalytics();
		}

		private void butOK_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
	}
}