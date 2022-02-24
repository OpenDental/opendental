using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Web;




namespace OpenDentBusiness.Mobile {
	public class Prefms {
		#region Only used on Patient Portal
			public static PrefmC LoadPreferences(long customerNum) {
				string command="SELECT * FROM preferencem WHERE CustomerNum = "+POut.Long(customerNum); 
				DataTable table=Db.GetTable(command);
				Prefm prefm=new Prefm(); 
				PrefmC prefmc=new PrefmC();
				for(int i=0;i<table.Rows.Count;i++) {
					prefm=new Prefm();
					if(table.Columns.Contains("PrefNum")) {
						prefm.PrefNum=PIn.Long(table.Rows[i]["PrefNum"].ToString());
					}
					prefm.PrefmName=PIn.String(table.Rows[i]["PrefName"].ToString());
					prefm.ValueString=PIn.String(table.Rows[i]["ValueString"].ToString());
					prefmc.Dict.Add(prefm.PrefmName,prefm);
				}
				HttpContext.Current.Session["prefmC"]=prefmc;
				return prefmc;
			}

			/// <summary>Load the preferences from the session. If it is not found in the session it's loaded from the database</summary>
			public static PrefmC LoadPreferences() {
				PrefmC prefmc=(PrefmC)HttpContext.Current.Session["prefmC"];
				if(prefmc==null) {
					if(HttpContext.Current.Session["Patient"]!=null) {
						long DentalOfficeID=((Patientm)HttpContext.Current.Session["Patient"]).CustomerNum;
						prefmc=LoadPreferences(DentalOfficeID);
					}
				}
				return prefmc;
			}
		#endregion

		#region Only used on OD
		/// <summary>converts a Pref to a Prefm object</summary>
		 public static Prefm ConvertToM(Pref pref){
			Prefm prefm=new Prefm();
			prefm.PrefNum=pref.PrefNum;
			prefm.PrefmName=pref.PrefName;
			prefm.ValueString=pref.ValueString;
			return prefm;
		 }

		 /// <summary>Returns a Prefm object when provided with the PrefName. Note that the CustomerNum field of the return object is not populated. </summary>
		 public static Prefm GetPrefm(String PrefName) {
			 Pref pref = Prefs.GetPref(PrefName);
			 Prefm prefm=ConvertToM(pref);
			 return prefm;
		 }
		#endregion

		#region Only used on WebHostSynch
			///<summary>Returns true if a change was required, or false if no change needed. This method is no longer used and may be deleted later. Dennis Mathew: Dec 24, 2011</summary>
			public void UpdateString(long customerNum,PrefmName prefmName,string newValue) {
				string command="SELECT * FROM preferencem "
					+"WHERE CustomerNum =" +POut.Long(customerNum)+" AND PrefName = '"+POut.String(prefmName.ToString())+"'";
				DataTable table=Db.GetTable(command);
				if(table.Rows.Count>0) {
					command = "UPDATE preferencem SET "
					+"ValueString = '"+POut.String(newValue)+"' "
					+"WHERE CustomerNum =" +POut.Long(customerNum)+" AND PrefName = '"+POut.String(prefmName.ToString())+"'";
					Db.NonQ(command);
				}
				else {
					command = "INSERT into preferencem " 
					+"(CustomerNum,PrefName,ValueString) VALUES "
					+"("+POut.Long(customerNum)+",'"+POut.String(prefmName.ToString())+"','"+POut.String(newValue)+"')";
					Db.NonQ(command);
				}
			}
	
			public static void UpdatePreference(Prefm prefm) {
				string command="SELECT * FROM preferencem "
					+"WHERE CustomerNum =" +POut.Long(prefm.CustomerNum)+" AND PrefNum = "+POut.Long(prefm.PrefNum);
				DataTable table=Db.GetTable(command);
				if(table.Rows.Count>0) {
					command = "UPDATE preferencem SET "
					+"ValueString = '"+POut.String(prefm.ValueString)+"' "
					+"WHERE CustomerNum =" +POut.Long(prefm.CustomerNum)+" AND PrefNum = "+POut.Long(prefm.PrefNum);
					Db.NonQ(command);
				}
				else {
					command = "INSERT into preferencem " 
					+"(CustomerNum,PrefNum,PrefName,ValueString) VALUES "
					+"("+POut.Long(prefm.CustomerNum)+","+POut.Long(prefm.PrefNum)+",'"+POut.String(prefm.PrefmName.ToString())+"','"+POut.String(prefm.ValueString)+"')";
					Db.NonQ(command);
				}
			}
		
		#endregion


	}
}