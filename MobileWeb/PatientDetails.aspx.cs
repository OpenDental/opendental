using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading;
using WebForms;
using OpenDentBusiness;
using OpenDentBusiness.Mobile;

namespace MobileWeb {
	public partial class PatientDetails:System.Web.UI.Page {
		public Patientm pat;
		public string DialLinkHmPhone="";
		public string DialLinkWkPhone="";
		public string DialLinkWirelessPhone="";
		public string EmailString="";
		public string PatName="";
		private long PatNum=0;
		private long CustomerNum=0;
		private Util util=new Util();
		protected void Page_Load(object sender,EventArgs e) {
			try {
				CustomerNum=util.GetCustomerNum(Message);
				if(CustomerNum==0) {
					return;
				}
				if(Request["PatNum"]!=null) {
					Int64.TryParse(Request["PatNum"].ToString().Trim(),out PatNum);
				}
				Int64.TryParse(Session["CustomerNum"].ToString(),out CustomerNum);
				pat=Patientms.GetOne(CustomerNum,PatNum);
				pat.Age=Patientms.DateToAge(pat.Birthdate);
				PatName=util.GetPatientName(pat);
				String DialString1=@"&nbsp;&nbsp;&nbsp;<a href=""tel:";
				String DialString2=@""" class=""style2"">dial</a>";
				if(!String.IsNullOrEmpty(pat.HmPhone)) {
					DialLinkHmPhone=DialString1+pat.HmPhone+DialString2;
				} 
				if(!String.IsNullOrEmpty(pat.WkPhone)) {
					DialLinkWkPhone=DialString1+pat.WkPhone+DialString2;
				}
				if(!String.IsNullOrEmpty(pat.WirelessPhone)) {
					DialLinkWirelessPhone=DialString1+pat.WirelessPhone+DialString2;
				}
				if(!String.IsNullOrEmpty(pat.Email)) {
					EmailString=@"<a href=""mailto:"+pat.Email+@""" class=""style2"">"+pat.Email+"</a>";
				}
				List<Appointmentm> appointmentmList=Appointmentms.GetAppointmentms(CustomerNum,PatNum);
				appointmentmList=appointmentmList.Where(a=>a.AptStatus!=ApptStatus.UnschedList && a.AptStatus!=ApptStatus.Planned).ToList();//exclude unscheduled and planned appointments.
				Repeater1.DataSource=appointmentmList;
				Repeater1.DataBind();
				List<RxPatm> rxList=RxPatms.GetRxPatms(CustomerNum,PatNum);
				Repeater2.DataSource=rxList;
				Repeater2.DataBind();
				List<Allergym> allergyList=Allergyms.GetAllergyms(CustomerNum,PatNum);
				Repeater3.DataSource=allergyList;
				Repeater3.DataBind();
			}
			catch(Exception ex) {
				LabelError.Text=Util.ErrorMessage;
				Logger.LogError(ex);
			}
		}

		private bool SetCustomerNum() {
			Message.Text="";
			if(Session["CustomerNum"]==null) {
				return false;
			}
			Int64.TryParse(Session["CustomerNum"].ToString(),out CustomerNum);
			if(CustomerNum!=0) {
				Message.Text="LoggedIn";
			}
			return true;
		}
	
	}
}