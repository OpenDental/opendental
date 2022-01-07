using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data;
using WebForms;
using WebHostSynch;
using OpenDentBusiness;
using OpenDentBusiness.Mobile;

namespace MobileWeb {
	public class Util {
		public static string ErrorMessage="There has been an error in processing your request.";
		
		public void SetMobileDbConnection() {
			string connectStr=Properties.Settings.Default.DBMobileWeb;
			OpenDentBusiness.DataConnection dc=new OpenDentBusiness.DataConnection();
			dc.SetDb(connectStr,"",DatabaseType.MySql,true);
		}

		public long GetDentalOfficeID(string username,string password) {
			long DentalOfficeID=0;
			String md5password=new WebHostSynch.Util().MD5Encrypt(password);
			try {
				// a query involving both username and password is used because 2 dental offices could potentially have the same username
				String command="SELECT * FROM userm WHERE UserName='"+POut.String(username)+"' AND Password='" +POut.String(md5password)+"'";
				Userm um=Userms.GetOne(command);
				if(um==null) {
					DentalOfficeID=0;//user password combination incorrect- specify message if necessary
				}
				else {
					DentalOfficeID=um.CustomerNum;
				}

			}
			catch(Exception ex) {
				Logger.LogError(ex);
				return DentalOfficeID;
			}
			if(username.ToLower()=="demo") {//for demo only
				DentalOfficeID=GetDemoDentalOfficeID();
			}
			return DentalOfficeID;
		}
		/// <summary>
		/// If Properties.Settings.Default.something is used in AppointmentList.aspx.cs page it give a  MobileWeb.Properties.Settings is inaccessible due to its protection level
		/// </summary>
		public long GetDemoDentalOfficeID() {
			return Properties.Settings.Default.DemoDentalOfficeID;
		}

		/// <summary>
		/// If Properties.Settings.Default.something is used in AppointmentList.aspx.cs page it give a  MobileWeb.Properties.Settings is inaccessible due to its protection level
		/// </summary>
		public DateTime GetDemoTodayDate() {
			return Properties.Settings.Default.DemoTodayDate;
		}

		public string GetPatientName(long PatNum,long CustomerNum) {
			try{
				String PatName="";
				Patientm pat=Patientms.GetOne(CustomerNum,PatNum);
				PatName=GetPatientName(pat);
				return PatName;
			}
			catch(Exception ex) {
				Logger.LogError(ex);
				return "";
			}
		}

		public string GetPatientName(Patientm pat) {
			try {
				String PatName="";
				PatName+=pat.LName +", ";
				if(!String.IsNullOrEmpty(pat.Preferred)) {
					PatName+="'"+pat.Preferred +"'";
				}
				PatName+=" "+pat.FName +" ";
				if(!String.IsNullOrEmpty(pat.MiddleI)) {
					PatName+=pat.MiddleI +".";
				}
				return PatName;
			}
			catch(Exception ex) {
				Logger.LogError(ex);
				return "";
			}
		}

		public long GetCustomerNum(System.Web.UI.WebControls.Literal Message) {
			long CustomerNum=0;
			try {
				Message.Text="";
				if(HttpContext.Current.Session["CustomerNum"]==null) {
					return 0;
				}
				Int64.TryParse(HttpContext.Current.Session["CustomerNum"].ToString(),out CustomerNum);
				if(CustomerNum!=0) {
					Message.Text="LoggedIn";
				}
			}
			catch(Exception ex) {
				Logger.LogError(ex);
				return CustomerNum;
			}
			return CustomerNum;
		}

	}
}