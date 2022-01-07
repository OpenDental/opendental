//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OpenDentBusiness.Crud{
	public class PayPlanCrud {
		///<summary>Gets one PayPlan object from the database using the primary key.  Returns null if not found.</summary>
		public static PayPlan SelectOne(long payPlanNum) {
			string command="SELECT * FROM payplan "
				+"WHERE PayPlanNum = "+POut.Long(payPlanNum);
			List<PayPlan> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one PayPlan object from the database using a query.</summary>
		public static PayPlan SelectOne(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<PayPlan> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of PayPlan objects from the database using a query.</summary>
		public static List<PayPlan> SelectMany(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<PayPlan> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<PayPlan> TableToList(DataTable table) {
			List<PayPlan> retVal=new List<PayPlan>();
			PayPlan payPlan;
			foreach(DataRow row in table.Rows) {
				payPlan=new PayPlan();
				payPlan.PayPlanNum            = PIn.Long  (row["PayPlanNum"].ToString());
				payPlan.PatNum                = PIn.Long  (row["PatNum"].ToString());
				payPlan.Guarantor             = PIn.Long  (row["Guarantor"].ToString());
				payPlan.PayPlanDate           = PIn.Date  (row["PayPlanDate"].ToString());
				payPlan.APR                   = PIn.Double(row["APR"].ToString());
				payPlan.Note                  = PIn.String(row["Note"].ToString());
				payPlan.PlanNum               = PIn.Long  (row["PlanNum"].ToString());
				payPlan.CompletedAmt          = PIn.Double(row["CompletedAmt"].ToString());
				payPlan.InsSubNum             = PIn.Long  (row["InsSubNum"].ToString());
				payPlan.PaySchedule           = (OpenDentBusiness.PaymentSchedule)PIn.Int(row["PaySchedule"].ToString());
				payPlan.NumberOfPayments      = PIn.Int   (row["NumberOfPayments"].ToString());
				payPlan.PayAmt                = PIn.Double(row["PayAmt"].ToString());
				payPlan.DownPayment           = PIn.Double(row["DownPayment"].ToString());
				payPlan.IsClosed              = PIn.Bool  (row["IsClosed"].ToString());
				payPlan.Signature             = PIn.String(row["Signature"].ToString());
				payPlan.SigIsTopaz            = PIn.Bool  (row["SigIsTopaz"].ToString());
				payPlan.PlanCategory          = PIn.Long  (row["PlanCategory"].ToString());
				payPlan.IsDynamic             = PIn.Bool  (row["IsDynamic"].ToString());
				payPlan.ChargeFrequency       = (OpenDentBusiness.PayPlanFrequency)PIn.Int(row["ChargeFrequency"].ToString());
				payPlan.DatePayPlanStart      = PIn.Date  (row["DatePayPlanStart"].ToString());
				payPlan.IsLocked              = PIn.Bool  (row["IsLocked"].ToString());
				payPlan.DateInterestStart     = PIn.Date  (row["DateInterestStart"].ToString());
				payPlan.DynamicPayPlanTPOption= (OpenDentBusiness.DynamicPayPlanTPOptions)PIn.Int(row["DynamicPayPlanTPOption"].ToString());
				payPlan.MobileAppDeviceNum    = PIn.Long  (row["MobileAppDeviceNum"].ToString());
				payPlan.SecurityHash          = PIn.String(row["SecurityHash"].ToString());
				retVal.Add(payPlan);
			}
			return retVal;
		}

		///<summary>Converts a list of PayPlan into a DataTable.</summary>
		public static DataTable ListToTable(List<PayPlan> listPayPlans,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="PayPlan";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("PayPlanNum");
			table.Columns.Add("PatNum");
			table.Columns.Add("Guarantor");
			table.Columns.Add("PayPlanDate");
			table.Columns.Add("APR");
			table.Columns.Add("Note");
			table.Columns.Add("PlanNum");
			table.Columns.Add("CompletedAmt");
			table.Columns.Add("InsSubNum");
			table.Columns.Add("PaySchedule");
			table.Columns.Add("NumberOfPayments");
			table.Columns.Add("PayAmt");
			table.Columns.Add("DownPayment");
			table.Columns.Add("IsClosed");
			table.Columns.Add("Signature");
			table.Columns.Add("SigIsTopaz");
			table.Columns.Add("PlanCategory");
			table.Columns.Add("IsDynamic");
			table.Columns.Add("ChargeFrequency");
			table.Columns.Add("DatePayPlanStart");
			table.Columns.Add("IsLocked");
			table.Columns.Add("DateInterestStart");
			table.Columns.Add("DynamicPayPlanTPOption");
			table.Columns.Add("MobileAppDeviceNum");
			table.Columns.Add("SecurityHash");
			foreach(PayPlan payPlan in listPayPlans) {
				table.Rows.Add(new object[] {
					POut.Long  (payPlan.PayPlanNum),
					POut.Long  (payPlan.PatNum),
					POut.Long  (payPlan.Guarantor),
					POut.DateT (payPlan.PayPlanDate,false),
					POut.Double(payPlan.APR),
					            payPlan.Note,
					POut.Long  (payPlan.PlanNum),
					POut.Double(payPlan.CompletedAmt),
					POut.Long  (payPlan.InsSubNum),
					POut.Int   ((int)payPlan.PaySchedule),
					POut.Int   (payPlan.NumberOfPayments),
					POut.Double(payPlan.PayAmt),
					POut.Double(payPlan.DownPayment),
					POut.Bool  (payPlan.IsClosed),
					            payPlan.Signature,
					POut.Bool  (payPlan.SigIsTopaz),
					POut.Long  (payPlan.PlanCategory),
					POut.Bool  (payPlan.IsDynamic),
					POut.Int   ((int)payPlan.ChargeFrequency),
					POut.DateT (payPlan.DatePayPlanStart,false),
					POut.Bool  (payPlan.IsLocked),
					POut.DateT (payPlan.DateInterestStart,false),
					POut.Int   ((int)payPlan.DynamicPayPlanTPOption),
					POut.Long  (payPlan.MobileAppDeviceNum),
					            payPlan.SecurityHash,
				});
			}
			return table;
		}

		///<summary>Inserts one PayPlan into the database.  Returns the new priKey.</summary>
		public static long Insert(PayPlan payPlan) {
			return Insert(payPlan,false);
		}

		///<summary>Inserts one PayPlan into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(PayPlan payPlan,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				payPlan.PayPlanNum=ReplicationServers.GetKey("payplan","PayPlanNum");
			}
			string command="INSERT INTO payplan (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="PayPlanNum,";
			}
			command+="PatNum,Guarantor,PayPlanDate,APR,Note,PlanNum,CompletedAmt,InsSubNum,PaySchedule,NumberOfPayments,PayAmt,DownPayment,IsClosed,Signature,SigIsTopaz,PlanCategory,IsDynamic,ChargeFrequency,DatePayPlanStart,IsLocked,DateInterestStart,DynamicPayPlanTPOption,MobileAppDeviceNum,SecurityHash) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(payPlan.PayPlanNum)+",";
			}
			command+=
				     POut.Long  (payPlan.PatNum)+","
				+    POut.Long  (payPlan.Guarantor)+","
				+    POut.Date  (payPlan.PayPlanDate)+","
				+		 POut.Double(payPlan.APR)+","
				+    DbHelper.ParamChar+"paramNote,"
				+    POut.Long  (payPlan.PlanNum)+","
				+		 POut.Double(payPlan.CompletedAmt)+","
				+    POut.Long  (payPlan.InsSubNum)+","
				+    POut.Int   ((int)payPlan.PaySchedule)+","
				+    POut.Int   (payPlan.NumberOfPayments)+","
				+		 POut.Double(payPlan.PayAmt)+","
				+		 POut.Double(payPlan.DownPayment)+","
				+    POut.Bool  (payPlan.IsClosed)+","
				+    DbHelper.ParamChar+"paramSignature,"
				+    POut.Bool  (payPlan.SigIsTopaz)+","
				+    POut.Long  (payPlan.PlanCategory)+","
				+    POut.Bool  (payPlan.IsDynamic)+","
				+    POut.Int   ((int)payPlan.ChargeFrequency)+","
				+    POut.Date  (payPlan.DatePayPlanStart)+","
				+    POut.Bool  (payPlan.IsLocked)+","
				+    POut.Date  (payPlan.DateInterestStart)+","
				+    POut.Int   ((int)payPlan.DynamicPayPlanTPOption)+","
				+    POut.Long  (payPlan.MobileAppDeviceNum)+","
				+"'"+POut.String(payPlan.SecurityHash)+"')";
			if(payPlan.Note==null) {
				payPlan.Note="";
			}
			OdSqlParameter paramNote=new OdSqlParameter("paramNote",OdDbType.Text,POut.StringParam(payPlan.Note));
			if(payPlan.Signature==null) {
				payPlan.Signature="";
			}
			OdSqlParameter paramSignature=new OdSqlParameter("paramSignature",OdDbType.Text,POut.StringParam(payPlan.Signature));
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command,paramNote,paramSignature);
			}
			else {
				payPlan.PayPlanNum=Db.NonQ(command,true,"PayPlanNum","payPlan",paramNote,paramSignature);
			}
			return payPlan.PayPlanNum;
		}

		///<summary>Inserts many PayPlans into the database.</summary>
		public static void InsertMany(List<PayPlan> listPayPlans) {
			InsertMany(listPayPlans,false);
		}

		///<summary>Inserts many PayPlans into the database.  Provides option to use the existing priKey.</summary>
		public static void InsertMany(List<PayPlan> listPayPlans,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				foreach(PayPlan payPlan in listPayPlans) {
					Insert(payPlan);
				}
			}
			else {
				StringBuilder sbCommands=null;
				int index=0;
				int countRows=0;
				while(index < listPayPlans.Count) {
					PayPlan payPlan=listPayPlans[index];
					StringBuilder sbRow=new StringBuilder("(");
					bool hasComma=false;
					if(sbCommands==null) {
						sbCommands=new StringBuilder();
						sbCommands.Append("INSERT INTO payplan (");
						if(useExistingPK) {
							sbCommands.Append("PayPlanNum,");
						}
						sbCommands.Append("PatNum,Guarantor,PayPlanDate,APR,Note,PlanNum,CompletedAmt,InsSubNum,PaySchedule,NumberOfPayments,PayAmt,DownPayment,IsClosed,Signature,SigIsTopaz,PlanCategory,IsDynamic,ChargeFrequency,DatePayPlanStart,IsLocked,DateInterestStart,DynamicPayPlanTPOption,MobileAppDeviceNum,SecurityHash) VALUES ");
						countRows=0;
					}
					else {
						hasComma=true;
					}
					if(useExistingPK) {
						sbRow.Append(POut.Long(payPlan.PayPlanNum)); sbRow.Append(",");
					}
					sbRow.Append(POut.Long(payPlan.PatNum)); sbRow.Append(",");
					sbRow.Append(POut.Long(payPlan.Guarantor)); sbRow.Append(",");
					sbRow.Append(POut.Date(payPlan.PayPlanDate)); sbRow.Append(",");
					sbRow.Append(POut.Double(payPlan.APR)); sbRow.Append(",");
					sbRow.Append("'"+POut.String(payPlan.Note)+"'"); sbRow.Append(",");
					sbRow.Append(POut.Long(payPlan.PlanNum)); sbRow.Append(",");
					sbRow.Append(POut.Double(payPlan.CompletedAmt)); sbRow.Append(",");
					sbRow.Append(POut.Long(payPlan.InsSubNum)); sbRow.Append(",");
					sbRow.Append(POut.Int((int)payPlan.PaySchedule)); sbRow.Append(",");
					sbRow.Append(POut.Int(payPlan.NumberOfPayments)); sbRow.Append(",");
					sbRow.Append(POut.Double(payPlan.PayAmt)); sbRow.Append(",");
					sbRow.Append(POut.Double(payPlan.DownPayment)); sbRow.Append(",");
					sbRow.Append(POut.Bool(payPlan.IsClosed)); sbRow.Append(",");
					sbRow.Append("'"+POut.String(payPlan.Signature)+"'"); sbRow.Append(",");
					sbRow.Append(POut.Bool(payPlan.SigIsTopaz)); sbRow.Append(",");
					sbRow.Append(POut.Long(payPlan.PlanCategory)); sbRow.Append(",");
					sbRow.Append(POut.Bool(payPlan.IsDynamic)); sbRow.Append(",");
					sbRow.Append(POut.Int((int)payPlan.ChargeFrequency)); sbRow.Append(",");
					sbRow.Append(POut.Date(payPlan.DatePayPlanStart)); sbRow.Append(",");
					sbRow.Append(POut.Bool(payPlan.IsLocked)); sbRow.Append(",");
					sbRow.Append(POut.Date(payPlan.DateInterestStart)); sbRow.Append(",");
					sbRow.Append(POut.Int((int)payPlan.DynamicPayPlanTPOption)); sbRow.Append(",");
					sbRow.Append(POut.Long(payPlan.MobileAppDeviceNum)); sbRow.Append(",");
					sbRow.Append("'"+POut.String(payPlan.SecurityHash)+"'"); sbRow.Append(")");
					if(sbCommands.Length+sbRow.Length+1 > TableBase.MaxAllowedPacketCount && countRows > 0) {
						Db.NonQ(sbCommands.ToString());
						sbCommands=null;
					}
					else {
						if(hasComma) {
							sbCommands.Append(",");
						}
						sbCommands.Append(sbRow.ToString());
						countRows++;
						if(index==listPayPlans.Count-1) {
							Db.NonQ(sbCommands.ToString());
						}
						index++;
					}
				}
			}
		}

		///<summary>Inserts one PayPlan into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(PayPlan payPlan) {
			return InsertNoCache(payPlan,false);
		}

		///<summary>Inserts one PayPlan into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(PayPlan payPlan,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO payplan (";
			if(!useExistingPK && isRandomKeys) {
				payPlan.PayPlanNum=ReplicationServers.GetKeyNoCache("payplan","PayPlanNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="PayPlanNum,";
			}
			command+="PatNum,Guarantor,PayPlanDate,APR,Note,PlanNum,CompletedAmt,InsSubNum,PaySchedule,NumberOfPayments,PayAmt,DownPayment,IsClosed,Signature,SigIsTopaz,PlanCategory,IsDynamic,ChargeFrequency,DatePayPlanStart,IsLocked,DateInterestStart,DynamicPayPlanTPOption,MobileAppDeviceNum,SecurityHash) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(payPlan.PayPlanNum)+",";
			}
			command+=
				     POut.Long  (payPlan.PatNum)+","
				+    POut.Long  (payPlan.Guarantor)+","
				+    POut.Date  (payPlan.PayPlanDate)+","
				+	   POut.Double(payPlan.APR)+","
				+    DbHelper.ParamChar+"paramNote,"
				+    POut.Long  (payPlan.PlanNum)+","
				+	   POut.Double(payPlan.CompletedAmt)+","
				+    POut.Long  (payPlan.InsSubNum)+","
				+    POut.Int   ((int)payPlan.PaySchedule)+","
				+    POut.Int   (payPlan.NumberOfPayments)+","
				+	   POut.Double(payPlan.PayAmt)+","
				+	   POut.Double(payPlan.DownPayment)+","
				+    POut.Bool  (payPlan.IsClosed)+","
				+    DbHelper.ParamChar+"paramSignature,"
				+    POut.Bool  (payPlan.SigIsTopaz)+","
				+    POut.Long  (payPlan.PlanCategory)+","
				+    POut.Bool  (payPlan.IsDynamic)+","
				+    POut.Int   ((int)payPlan.ChargeFrequency)+","
				+    POut.Date  (payPlan.DatePayPlanStart)+","
				+    POut.Bool  (payPlan.IsLocked)+","
				+    POut.Date  (payPlan.DateInterestStart)+","
				+    POut.Int   ((int)payPlan.DynamicPayPlanTPOption)+","
				+    POut.Long  (payPlan.MobileAppDeviceNum)+","
				+"'"+POut.String(payPlan.SecurityHash)+"')";
			if(payPlan.Note==null) {
				payPlan.Note="";
			}
			OdSqlParameter paramNote=new OdSqlParameter("paramNote",OdDbType.Text,POut.StringParam(payPlan.Note));
			if(payPlan.Signature==null) {
				payPlan.Signature="";
			}
			OdSqlParameter paramSignature=new OdSqlParameter("paramSignature",OdDbType.Text,POut.StringParam(payPlan.Signature));
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command,paramNote,paramSignature);
			}
			else {
				payPlan.PayPlanNum=Db.NonQ(command,true,"PayPlanNum","payPlan",paramNote,paramSignature);
			}
			return payPlan.PayPlanNum;
		}

		///<summary>Updates one PayPlan in the database.</summary>
		public static void Update(PayPlan payPlan) {
			string command="UPDATE payplan SET "
				+"PatNum                =  "+POut.Long  (payPlan.PatNum)+", "
				+"Guarantor             =  "+POut.Long  (payPlan.Guarantor)+", "
				+"PayPlanDate           =  "+POut.Date  (payPlan.PayPlanDate)+", "
				+"APR                   =  "+POut.Double(payPlan.APR)+", "
				+"Note                  =  "+DbHelper.ParamChar+"paramNote, "
				+"PlanNum               =  "+POut.Long  (payPlan.PlanNum)+", "
				+"CompletedAmt          =  "+POut.Double(payPlan.CompletedAmt)+", "
				+"InsSubNum             =  "+POut.Long  (payPlan.InsSubNum)+", "
				+"PaySchedule           =  "+POut.Int   ((int)payPlan.PaySchedule)+", "
				+"NumberOfPayments      =  "+POut.Int   (payPlan.NumberOfPayments)+", "
				+"PayAmt                =  "+POut.Double(payPlan.PayAmt)+", "
				+"DownPayment           =  "+POut.Double(payPlan.DownPayment)+", "
				+"IsClosed              =  "+POut.Bool  (payPlan.IsClosed)+", "
				+"Signature             =  "+DbHelper.ParamChar+"paramSignature, "
				+"SigIsTopaz            =  "+POut.Bool  (payPlan.SigIsTopaz)+", "
				+"PlanCategory          =  "+POut.Long  (payPlan.PlanCategory)+", "
				+"IsDynamic             =  "+POut.Bool  (payPlan.IsDynamic)+", "
				+"ChargeFrequency       =  "+POut.Int   ((int)payPlan.ChargeFrequency)+", "
				+"DatePayPlanStart      =  "+POut.Date  (payPlan.DatePayPlanStart)+", "
				+"IsLocked              =  "+POut.Bool  (payPlan.IsLocked)+", "
				+"DateInterestStart     =  "+POut.Date  (payPlan.DateInterestStart)+", "
				+"DynamicPayPlanTPOption=  "+POut.Int   ((int)payPlan.DynamicPayPlanTPOption)+", "
				+"MobileAppDeviceNum    =  "+POut.Long  (payPlan.MobileAppDeviceNum)+", "
				+"SecurityHash          = '"+POut.String(payPlan.SecurityHash)+"' "
				+"WHERE PayPlanNum = "+POut.Long(payPlan.PayPlanNum);
			if(payPlan.Note==null) {
				payPlan.Note="";
			}
			OdSqlParameter paramNote=new OdSqlParameter("paramNote",OdDbType.Text,POut.StringParam(payPlan.Note));
			if(payPlan.Signature==null) {
				payPlan.Signature="";
			}
			OdSqlParameter paramSignature=new OdSqlParameter("paramSignature",OdDbType.Text,POut.StringParam(payPlan.Signature));
			Db.NonQ(command,paramNote,paramSignature);
		}

		///<summary>Updates one PayPlan in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(PayPlan payPlan,PayPlan oldPayPlan) {
			string command="";
			if(payPlan.PatNum != oldPayPlan.PatNum) {
				if(command!="") { command+=",";}
				command+="PatNum = "+POut.Long(payPlan.PatNum)+"";
			}
			if(payPlan.Guarantor != oldPayPlan.Guarantor) {
				if(command!="") { command+=",";}
				command+="Guarantor = "+POut.Long(payPlan.Guarantor)+"";
			}
			if(payPlan.PayPlanDate.Date != oldPayPlan.PayPlanDate.Date) {
				if(command!="") { command+=",";}
				command+="PayPlanDate = "+POut.Date(payPlan.PayPlanDate)+"";
			}
			if(payPlan.APR != oldPayPlan.APR) {
				if(command!="") { command+=",";}
				command+="APR = "+POut.Double(payPlan.APR)+"";
			}
			if(payPlan.Note != oldPayPlan.Note) {
				if(command!="") { command+=",";}
				command+="Note = "+DbHelper.ParamChar+"paramNote";
			}
			if(payPlan.PlanNum != oldPayPlan.PlanNum) {
				if(command!="") { command+=",";}
				command+="PlanNum = "+POut.Long(payPlan.PlanNum)+"";
			}
			if(payPlan.CompletedAmt != oldPayPlan.CompletedAmt) {
				if(command!="") { command+=",";}
				command+="CompletedAmt = "+POut.Double(payPlan.CompletedAmt)+"";
			}
			if(payPlan.InsSubNum != oldPayPlan.InsSubNum) {
				if(command!="") { command+=",";}
				command+="InsSubNum = "+POut.Long(payPlan.InsSubNum)+"";
			}
			if(payPlan.PaySchedule != oldPayPlan.PaySchedule) {
				if(command!="") { command+=",";}
				command+="PaySchedule = "+POut.Int   ((int)payPlan.PaySchedule)+"";
			}
			if(payPlan.NumberOfPayments != oldPayPlan.NumberOfPayments) {
				if(command!="") { command+=",";}
				command+="NumberOfPayments = "+POut.Int(payPlan.NumberOfPayments)+"";
			}
			if(payPlan.PayAmt != oldPayPlan.PayAmt) {
				if(command!="") { command+=",";}
				command+="PayAmt = "+POut.Double(payPlan.PayAmt)+"";
			}
			if(payPlan.DownPayment != oldPayPlan.DownPayment) {
				if(command!="") { command+=",";}
				command+="DownPayment = "+POut.Double(payPlan.DownPayment)+"";
			}
			if(payPlan.IsClosed != oldPayPlan.IsClosed) {
				if(command!="") { command+=",";}
				command+="IsClosed = "+POut.Bool(payPlan.IsClosed)+"";
			}
			if(payPlan.Signature != oldPayPlan.Signature) {
				if(command!="") { command+=",";}
				command+="Signature = "+DbHelper.ParamChar+"paramSignature";
			}
			if(payPlan.SigIsTopaz != oldPayPlan.SigIsTopaz) {
				if(command!="") { command+=",";}
				command+="SigIsTopaz = "+POut.Bool(payPlan.SigIsTopaz)+"";
			}
			if(payPlan.PlanCategory != oldPayPlan.PlanCategory) {
				if(command!="") { command+=",";}
				command+="PlanCategory = "+POut.Long(payPlan.PlanCategory)+"";
			}
			if(payPlan.IsDynamic != oldPayPlan.IsDynamic) {
				if(command!="") { command+=",";}
				command+="IsDynamic = "+POut.Bool(payPlan.IsDynamic)+"";
			}
			if(payPlan.ChargeFrequency != oldPayPlan.ChargeFrequency) {
				if(command!="") { command+=",";}
				command+="ChargeFrequency = "+POut.Int   ((int)payPlan.ChargeFrequency)+"";
			}
			if(payPlan.DatePayPlanStart.Date != oldPayPlan.DatePayPlanStart.Date) {
				if(command!="") { command+=",";}
				command+="DatePayPlanStart = "+POut.Date(payPlan.DatePayPlanStart)+"";
			}
			if(payPlan.IsLocked != oldPayPlan.IsLocked) {
				if(command!="") { command+=",";}
				command+="IsLocked = "+POut.Bool(payPlan.IsLocked)+"";
			}
			if(payPlan.DateInterestStart.Date != oldPayPlan.DateInterestStart.Date) {
				if(command!="") { command+=",";}
				command+="DateInterestStart = "+POut.Date(payPlan.DateInterestStart)+"";
			}
			if(payPlan.DynamicPayPlanTPOption != oldPayPlan.DynamicPayPlanTPOption) {
				if(command!="") { command+=",";}
				command+="DynamicPayPlanTPOption = "+POut.Int   ((int)payPlan.DynamicPayPlanTPOption)+"";
			}
			if(payPlan.MobileAppDeviceNum != oldPayPlan.MobileAppDeviceNum) {
				if(command!="") { command+=",";}
				command+="MobileAppDeviceNum = "+POut.Long(payPlan.MobileAppDeviceNum)+"";
			}
			if(payPlan.SecurityHash != oldPayPlan.SecurityHash) {
				if(command!="") { command+=",";}
				command+="SecurityHash = '"+POut.String(payPlan.SecurityHash)+"'";
			}
			if(command=="") {
				return false;
			}
			if(payPlan.Note==null) {
				payPlan.Note="";
			}
			OdSqlParameter paramNote=new OdSqlParameter("paramNote",OdDbType.Text,POut.StringParam(payPlan.Note));
			if(payPlan.Signature==null) {
				payPlan.Signature="";
			}
			OdSqlParameter paramSignature=new OdSqlParameter("paramSignature",OdDbType.Text,POut.StringParam(payPlan.Signature));
			command="UPDATE payplan SET "+command
				+" WHERE PayPlanNum = "+POut.Long(payPlan.PayPlanNum);
			Db.NonQ(command,paramNote,paramSignature);
			return true;
		}

		///<summary>Returns true if Update(PayPlan,PayPlan) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(PayPlan payPlan,PayPlan oldPayPlan) {
			if(payPlan.PatNum != oldPayPlan.PatNum) {
				return true;
			}
			if(payPlan.Guarantor != oldPayPlan.Guarantor) {
				return true;
			}
			if(payPlan.PayPlanDate.Date != oldPayPlan.PayPlanDate.Date) {
				return true;
			}
			if(payPlan.APR != oldPayPlan.APR) {
				return true;
			}
			if(payPlan.Note != oldPayPlan.Note) {
				return true;
			}
			if(payPlan.PlanNum != oldPayPlan.PlanNum) {
				return true;
			}
			if(payPlan.CompletedAmt != oldPayPlan.CompletedAmt) {
				return true;
			}
			if(payPlan.InsSubNum != oldPayPlan.InsSubNum) {
				return true;
			}
			if(payPlan.PaySchedule != oldPayPlan.PaySchedule) {
				return true;
			}
			if(payPlan.NumberOfPayments != oldPayPlan.NumberOfPayments) {
				return true;
			}
			if(payPlan.PayAmt != oldPayPlan.PayAmt) {
				return true;
			}
			if(payPlan.DownPayment != oldPayPlan.DownPayment) {
				return true;
			}
			if(payPlan.IsClosed != oldPayPlan.IsClosed) {
				return true;
			}
			if(payPlan.Signature != oldPayPlan.Signature) {
				return true;
			}
			if(payPlan.SigIsTopaz != oldPayPlan.SigIsTopaz) {
				return true;
			}
			if(payPlan.PlanCategory != oldPayPlan.PlanCategory) {
				return true;
			}
			if(payPlan.IsDynamic != oldPayPlan.IsDynamic) {
				return true;
			}
			if(payPlan.ChargeFrequency != oldPayPlan.ChargeFrequency) {
				return true;
			}
			if(payPlan.DatePayPlanStart.Date != oldPayPlan.DatePayPlanStart.Date) {
				return true;
			}
			if(payPlan.IsLocked != oldPayPlan.IsLocked) {
				return true;
			}
			if(payPlan.DateInterestStart.Date != oldPayPlan.DateInterestStart.Date) {
				return true;
			}
			if(payPlan.DynamicPayPlanTPOption != oldPayPlan.DynamicPayPlanTPOption) {
				return true;
			}
			if(payPlan.MobileAppDeviceNum != oldPayPlan.MobileAppDeviceNum) {
				return true;
			}
			if(payPlan.SecurityHash != oldPayPlan.SecurityHash) {
				return true;
			}
			return false;
		}

		///<summary>Deletes one PayPlan from the database.</summary>
		public static void Delete(long payPlanNum) {
			string command="DELETE FROM payplan "
				+"WHERE PayPlanNum = "+POut.Long(payPlanNum);
			Db.NonQ(command);
		}

		///<summary>Deletes many PayPlans from the database.</summary>
		public static void DeleteMany(List<long> listPayPlanNums) {
			if(listPayPlanNums==null || listPayPlanNums.Count==0) {
				return;
			}
			string command="DELETE FROM payplan "
				+"WHERE PayPlanNum IN("+string.Join(",",listPayPlanNums.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}

	}
}