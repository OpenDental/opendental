using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OpenDentBusiness;
using OpenDentBusiness.Mobile;

namespace MobileWeb {
	public partial class PharmacyList:System.Web.UI.Page {
		private long CustomerNum=0;
		private Util util=new Util();
		protected void Page_Load(object sender,EventArgs e) {
			CustomerNum=util.GetCustomerNum(Message);
			if(CustomerNum==0) {
				return;
			}
			List<Pharmacym> pharmacymList=Pharmacyms.GetPharmacyms(CustomerNum);
			Repeater1.DataSource=pharmacymList;
			Repeater1.DataBind();
		}
	}
}