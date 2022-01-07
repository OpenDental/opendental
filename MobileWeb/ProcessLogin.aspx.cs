using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Security.Cryptography;
using System.Threading;
using WebForms;

namespace MobileWeb {
	public partial class ProcessLogin:System.Web.UI.Page {
		private Util util=new Util();
		private string LoginFailedMessage="Login Failed. Please try again";
		private long DentalOfficeID=0;

		protected void Page_Load(object sender,EventArgs e) {
			try {
				//delete later util.SetMobileDbConnection();//connects to db if not already connected.
				String username="";
				String password="";
				bool RememberMe=false; 
				Message.Text="";
					if(Request.Form["username"]!=null) {
						username=Request.Form["username"].ToString().Trim();
					}
					if(Request.Form["password"]!=null) {
						password=Request.Form["password"].ToString().Trim();
					}
					if(Request.Form["rememberusername"]!=null) {
						if(Request.Form["rememberusername"].ToString().Trim()=="true") {
							RememberMe=true;
						}
					}
					DentalOfficeID=util.GetDentalOfficeID(username,password);
					//DentalOfficeID=;// !!! or testing only comment out this line on prod
					if(DentalOfficeID>0) {
						Session["CustomerNum"]=DentalOfficeID;
						Message.Text="CorrectLogin";
					}
					else {
						Message.Text=LoginFailedMessage;
					}
					HttpCookie UserNameCookie=new HttpCookie("UserNameCookie");
					if(RememberMe) {
						UserNameCookie.Value=username;
						UserNameCookie.Expires=DateTime.Now.AddYears(1);
						Response.Cookies.Add(UserNameCookie);
					}
					else {
						UserNameCookie.Expires=DateTime.Now.AddDays(-1);
						Response.Cookies.Add(UserNameCookie);
					}
			
			}
			catch(Exception ex) {
				Message.Text=LoginFailedMessage;
				Logger.LogError(ex);
			}
		}
		

	}
}