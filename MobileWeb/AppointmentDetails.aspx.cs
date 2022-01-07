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
	public partial class AppointmentDetails:System.Web.UI.Page {
		private long AptNum=0;
		private long CustomerNum=0;
		private Util util=new Util();
		public Appointmentm apt;
		public Patientm pat;
		public String PatName="";
		
		protected void Page_Load(object sender,EventArgs e) {
			try {
				CustomerNum=util.GetCustomerNum(Message);
				if(CustomerNum==0) {
					return;
				}
				if(Request["AptNum"]!=null) {
					Int64.TryParse(Request["AptNum"].ToString().Trim(),out AptNum);
				}
				apt=Appointmentms.GetOne(CustomerNum,AptNum);
				pat=Patientms.GetOne(CustomerNum,apt.PatNum);
				PatName=util.GetPatientName(pat);
			}
			catch(Exception ex) {
				LabelError.Text=Util.ErrorMessage;
				Logger.LogError(ex);
			}
		}

		public string GetPatientName(long PatNum) {
			return util.GetPatientName(PatNum,CustomerNum);
		}


	}
}