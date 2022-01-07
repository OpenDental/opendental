using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebForms;

namespace MobileWeb {
	public partial class Index:System.Web.UI.Page {
		public Random random=new Random();
		protected void Page_Load(object sender,EventArgs e) {
			if(Request.Cookies["UserNameCookie"] != null) {
				HttpCookie UserNameCookie=Request.Cookies["UserNameCookie"];
				username.Text=UserNameCookie.Value;
				rememberusername.Checked=true;
			}
		}
	}
}