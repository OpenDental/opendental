using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormMassEmailAnalytics:FormODBase {

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
			dateRangeAnalytics.SetDateTimeFrom(dateRangeAnalytics.DefaultDateTimeFrom);
			dateRangeAnalytics.SetDateTimeTo(dateRangeAnalytics.DefaultDateTimeTo);
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
			if(gridAnalytics.Columns.Count==0) {		
				GridColumn col=new GridColumn(Lans.g(gridAnalytics.TranslationName,"Name"),100,GridSortingStrategy.StringCompare);
				gridAnalytics.Columns.Add(col);
				col=new GridColumn(Lans.g(gridAnalytics.TranslationName,"Type"),100,GridSortingStrategy.StringCompare);
				gridAnalytics.Columns.Add(col);
				if(PrefC.HasClinicsEnabled) {
					col=new GridColumn(Lans.g(gridAnalytics.TranslationName,"Clinic"),100,GridSortingStrategy.StringCompare);
					gridAnalytics.Columns.Add(col);
				}
				col=new GridColumn(Lans.g(gridAnalytics.TranslationName,"Date Created"),100,GridSortingStrategy.DateParse);
				gridAnalytics.Columns.Add(col);
				col=new GridColumn(Lans.g(gridAnalytics.TranslationName,"Total"),85,HorizontalAlignment.Center);
				gridAnalytics.Columns.Add(col);
				col=new GridColumn(Lans.g(gridAnalytics.TranslationName,"Pending"),85,HorizontalAlignment.Center);
				gridAnalytics.Columns.Add(col);
				col=new GridColumn(Lans.g(gridAnalytics.TranslationName,"Unopened"),85,HorizontalAlignment.Center);
				gridAnalytics.Columns.Add(col);
				col=new GridColumn(Lans.g(gridAnalytics.TranslationName,"Opened"),85,HorizontalAlignment.Center);
				gridAnalytics.Columns.Add(col);
				col=new GridColumn(Lans.g(gridAnalytics.TranslationName,"Bounced"),85,HorizontalAlignment.Center);
				gridAnalytics.Columns.Add(col);
				col=new GridColumn(Lans.g(gridAnalytics.TranslationName,"Complaint"),85,HorizontalAlignment.Center);
				gridAnalytics.Columns.Add(col);
				col=new GridColumn(Lans.g(gridAnalytics.TranslationName,"Failed"),85,HorizontalAlignment.Center);
				gridAnalytics.Columns.Add(col);
				col=new GridColumn(Lans.g(gridAnalytics.TranslationName,"Unsubscribed"),85,HorizontalAlignment.Center);
				gridAnalytics.Columns.Add(col);
			}
			gridAnalytics.ListGridRows.Clear();
			long clinicNum=PrefC.HasClinicsEnabled ? comboClinicAnalytics.SelectedClinicNum : -1;
			DateTime dateTimeAnalyticsFrom=dateRangeAnalytics.GetDateTimeFrom();
			DateTime dateTimeAnalyticsTo=dateRangeAnalytics.GetDateTimeTo();
			List<PromotionAnalytic> listPromotionAnalytics=Promotions.GetAnalytics(dateTimeAnalyticsFrom,dateTimeAnalyticsTo,clinicNum)
				.OrderByDescending(x => x.Promotion.DateTimeCreated)
				.ThenByDescending(x => x.Promotion.PromotionNum)
				.ToList();
			for(int i=0;i<listPromotionAnalytics.Count;i++) {
				GridRow row=new GridRow();
				row.Cells.Add(listPromotionAnalytics[i].Promotion.PromotionName);
				row.Cells.Add(listPromotionAnalytics[i].Promotion.TypePromotion.GetDescription());
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add(Clinics.GetDesc(listPromotionAnalytics[i].Promotion.ClinicNum));
				}
				row.Cells.Add(listPromotionAnalytics[i].Promotion.DateTimeCreated.ToShortDateString());
				//Combine all statuses together to get the total.
				int total=listPromotionAnalytics[i].DictionaryCounts.Sum(x => x.Value);
				row.Cells.Add(total.ToString());
				row.Cells.Add(GetCountString(listPromotionAnalytics[i],total,PromotionLogStatus.Pending));
				row.Cells.Add(GetCountString(listPromotionAnalytics[i],total,PromotionLogStatus.Delivered));
				row.Cells.Add(GetCountString(listPromotionAnalytics[i],total,PromotionLogStatus.Opened));
				row.Cells.Add(GetCountString(listPromotionAnalytics[i],total,PromotionLogStatus.Bounced));
				row.Cells.Add(GetCountString(listPromotionAnalytics[i],total,PromotionLogStatus.Complaint));
				row.Cells.Add(GetCountString(listPromotionAnalytics[i],total,PromotionLogStatus.Failed));
				row.Cells.Add(GetCountString(listPromotionAnalytics[i],total,PromotionLogStatus.Unsubscribed));
				gridAnalytics.ListGridRows.Add(row);
			}
			gridAnalytics.EndUpdate();
		}

		private string GetCountString(PromotionAnalytic promotionAnalytic,int total,params PromotionLogStatus[] promotionLogStatusArray) {
			int count=0;
			for(int i=0;i<promotionLogStatusArray.Length;i++) {
				//If this is not in the dictionary, count will be 0.
				//int countStatus=promotionAnalytic.DictionaryCounts[promotionLogStatusArray[i]];
				promotionAnalytic.DictionaryCounts.TryGetValue(promotionLogStatusArray[i],out int countStatus);
				count+=countStatus;
			}
			return $"{count} \r\n({((float)count/total*100).ToString("N")})%";
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