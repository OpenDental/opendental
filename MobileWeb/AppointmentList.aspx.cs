using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading;
using System.Drawing;
using WebForms;
using OpenDentBusiness;
using OpenDentBusiness.Mobile;

namespace MobileWeb {
	public partial class AppointmentList:System.Web.UI.Page {
		private long CustomerNum=0;
		private Util util=new Util();
		public int PreviousDateDay=0;
		public int PreviousDateMonth=0;
		public int PreviousDateYear=0;
		public int NextDateDay=0;
		public int NextDateMonth=0;
		public int NextDateYear=0;
		

		protected void Page_Load(object sender,EventArgs e) {
			try {
				CustomerNum=util.GetCustomerNum(Message);
				if(CustomerNum==0) {
					return;
				}
				#region process dates
					int Year=0;
					int Month=0; 
					int Day=0;
					DateTime AppointmentDate=DateTime.MinValue;
					if(Request["year"]!=null && Request["month"]!=null && Request["day"]!=null) {
						Int32.TryParse(Request["year"].ToString().Trim(),out Year);
						Int32.TryParse(Request["month"].ToString().Trim(),out Month);
						Int32.TryParse(Request["day"].ToString().Trim(),out Day);
						AppointmentDate= new DateTime(Year,Month,Day);
					}
					else {
						//dennis set cookies here this would be read by javascript on the client browser.
						HttpCookie DemoDateCookieY=new HttpCookie("DemoDateCookieY");
						HttpCookie DemoDateCookieM=new HttpCookie("DemoDateCookieM");
						HttpCookie DemoDateCookieD=new HttpCookie("DemoDateCookieD");
						if(CustomerNum==util.GetDemoDentalOfficeID()) {
							AppointmentDate=util.GetDemoTodayDate();//for demo only. The date is set to a preset date in webconfig.
							DemoDateCookieY.Value=AppointmentDate.Year+"";
							DemoDateCookieM.Value=AppointmentDate.Month+"";
							DemoDateCookieD.Value=AppointmentDate.Day+"";
						}
						else {
							DemoDateCookieY.Value="";// these are explicitely set to empty, because the javascript on the browser is picking values from previously set cookies
							DemoDateCookieM.Value="";
							DemoDateCookieD.Value="";
							AppointmentDate=DateTime.Today;
						}
						Response.Cookies.Add(DemoDateCookieY);// if expiry is not specified the cookie lasts till the end of session
						Response.Cookies.Add(DemoDateCookieM);
						Response.Cookies.Add(DemoDateCookieD);
					}
					DayLabel.Text=AppointmentDate.ToString("ddd")+", "+AppointmentDate.ToString("MMM")+AppointmentDate.ToString("dd");
					DateTime PreviousDate=AppointmentDate.AddDays(-1);
					PreviousDateDay=PreviousDate.Day;
					PreviousDateMonth=PreviousDate.Month;
					PreviousDateYear=PreviousDate.Year;
					DateTime NextDate=AppointmentDate.AddDays(1);
					NextDateDay=NextDate.Day;
					NextDateMonth=NextDate.Month;
					NextDateYear=NextDate.Year;
				#endregion

				#region process providers
					long ProvNum=0;
					if(Request["ProvNum"]==null) {
						if(Session["ProvNum"]!=null) {
							Int64.TryParse(Session["ProvNum"].ToString(),out ProvNum);
						}
					}else{
						Int64.TryParse(Request["ProvNum"].ToString().Trim(),out ProvNum);
						Session["ProvNum"]=ProvNum.ToString();
						}
				#endregion
				List<Appointmentm> appointmentmList;
				if(ProvNum==0){
					appointmentmList=Appointmentms.GetAppointmentms(CustomerNum,AppointmentDate,AppointmentDate);
				}else{
					appointmentmList=Appointmentms.GetAppointmentms(CustomerNum,ProvNum,AppointmentDate,AppointmentDate);
				}
				appointmentmList=appointmentmList.Where(a=>a.AptStatus!=ApptStatus.UnschedList && a.AptStatus!=ApptStatus.Planned).ToList();//exclude unscheduled and planned appointments.
				Repeater1.DataSource=appointmentmList;
				Repeater1.DataBind();
			}
			catch(Exception ex) {
				LabelError.Text=Util.ErrorMessage;
				Logger.LogError(ex); 
			}
		}

		public string GetPatientName(long PatNum) {
			return util.GetPatientName(PatNum,CustomerNum);
		}

		public string GetProviderColor(Appointmentm ap) {
			string HexColor="#FFFFFF";
			try {
				Providerm pv;
				if(ap.AptStatus==ApptStatus.Complete) {
					return "#808080"; //gray
				}
				if(ap.IsHygiene) {
					if(ap.ProvHyg==0) { //no hygenist
						return HexColor;
					}
					pv=Providerms.GetOne(CustomerNum,ap.ProvHyg);
				}else{
					pv=Providerms.GetOne(CustomerNum,ap.ProvNum);
				}
				if(pv!=null) {
					HexColor=ColorTranslator.ToHtml(pv.ProvColor);
				}
				return HexColor;
			}
			catch(Exception ex) {
				Logger.LogError("CustomerNum="+CustomerNum+ " ap.ProvNum="+ap.ProvNum,ex);
				return HexColor;
			}
		}


		public string GetApptBrokenStatus(Appointmentm ap) {
			if(ap.AptStatus==ApptStatus.Broken){
				return @"<div style=""font-style: italic;color: #FF0000;font-weight:normal"">Broken</div>";
			}
			return "";
		}



	}
}