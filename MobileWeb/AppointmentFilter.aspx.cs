using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebForms;
using OpenDentBusiness;
using OpenDentBusiness.Mobile;

namespace MobileWeb {
	public partial class AppointmentFilter:System.Web.UI.Page {
		private long CustomerNum=0;
		private long ProvNum=0;
		private Util util=new Util();
		protected void Page_Load(object sender,EventArgs e) {
			try {
				CustomerNum=util.GetCustomerNum(Message);
				if(CustomerNum==0) {
					return;
				}
				List<Providerm> providermList=Providerms.GetProviderms(CustomerNum);
				//the elements inthe drop down are padded left and right so that they are centered.
				for(int i=0;i<providermList.Count;i++) {
					string abbr=providermList[i].Abbr;
					int PadLength=14-abbr.Length/2;
					providermList[i].Abbr=abbr.PadLeft(PadLength,' ').PadRight(PadLength,' ').Replace(" ","&nbsp;");
				}
				Repeater1.DataSource=providermList;
				Repeater1.DataBind();
			}
			catch(Exception ex) {
				LabelError.Text=Util.ErrorMessage;
				Logger.LogError(ex);
			}
		}

		public string GetSelected(Providerm pv) {
			try {
				if(Session["ProvNum"]!=null) {
					Int64.TryParse(Session["ProvNum"].ToString(),out ProvNum);
				}
				if(pv.ProvNum==ProvNum){
					return @" selected=""selected""";
				}
				else{
					return "";
				}
			}
			catch(Exception ex) {
				Logger.LogError("CustomerNum="+CustomerNum+ " pv.ProvNum="+pv.ProvNum,ex);
				return "";
			}
		}
	}
}