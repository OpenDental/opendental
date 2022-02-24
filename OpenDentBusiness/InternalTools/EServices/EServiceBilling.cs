using CodeBase;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

//HQ only. TableType, S-Class, and CRUD are all contained in this file.
#region TableType
namespace OpenDentBusiness {
	///<summary>Only used internally by OpenDental, Inc.  Not used by anyone else. Aggregates customer charges for EServices.</summary>
	[Serializable()]
	[CrudTable(IsMissingInGeneral=true)]//Remove this line to perform one-time CRUD updated to this class. Place back before committing changes.
	public class EServiceBilling:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EServiceBillingNum;
		///<summary>Should be unique in this table per DateUsage.</summary>
		public long RegistrationKeyNum;
		///<summary>Should be unique in this table per DateUsage.</summary>
		public long CustPatNum;
		///<summary>From patient.BillingCycleDay. The day of the month that this patient receives their bill.</summary>
		public int BillingCycleDay;
		///<summary>Timestamp when this row is entered.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime DateTimeEntry;
		///<summary>Timestamp when this row was processed by RepeatCharge tool and procedures were posted. If MinVal then this row has not been processed yet.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeProceduresPosted;
		///<summary>Date only. Indicates the exact date on which the bill should post to the procedure log.</summary>
		public DateTime DateOfBill;
		///<summary>Date only. Indicates which calendar month this usage metric is for. Should be 1st of month at midnight. Example '2012-01-01 00:00:00'.
		///Will include charges from both the previous month's usage and the coming month's service activations.</summary>
		public DateTime MonthOfBill;
		///<summary>Date only. Start of the coming month's service activation.</summary>
		public DateTime BillCycleStart;
		///<summary>Date only. End of the coming month's service activation.</summary>
		public DateTime BillCycleEnd;
		///<summary>Date only. TextingUsage, TextingAccess, ConfirmationRequest will all be billed from the start of the prior calendar month to the end of the prior calendar month.
		///Amounts can vary based on usage so they cannot be billed in advanced like other charges.</summary>
		public DateTime UsageCycleStart;
		///<summary>Date only. TextingUsage, TextingAccess, ConfirmationRequest will all be billed from the start of the prior calendar month to the end of the prior calendar month.
		///Amounts can vary based on usage so they cannot be billed in advanced like other charges.</summary>
		public DateTime UsageCycleEnd;
		///<summary>The produced output of the billing algorithm is a json serialized collection (List) of procedures. 
		///This will later be deserialized by OD proper and inserted into the procedurelog table.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string ProceduresJson;
		///<summary>Dictionary of eServiceCode, ChargeUSD. Will be deserialized by Broadcast Monitor to show performance by eService in a graph.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string ChargesJson;
		///<summary>Information was previously held in SmsBilling table as structured data.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string NexmoInfoJson;
		///<summary>Human readable explanation of how the confirmation charges were calculated.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string LogInfo;
		///<summary>1 line per charge per clinic. To be used by accounting for invoicing.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string ItemizedCharges;

		public EServiceBilling Clone() {
			return (EServiceBilling)this.MemberwiseClone();
		}		
	}
	
	///<summary>Not a table type. Used to store structured data useful for generating eService invoices.</summary>
	public class EServiceInvoiceLineItem {
		[JsonProperty(PropertyName="A")]
		public long RegKeyNum;
		///<summary>The PatNum of this particular SuperFamily member. Looked up by 'eService Clinic' pat field.</summary>
		[JsonProperty(PropertyName="B")]
		public long PatNum;
		[JsonProperty(PropertyName="C")]
		public long ClinicNum;
		[JsonProperty(PropertyName="D")]
		public DateTime DateOfInvoice;
		[JsonProperty(PropertyName="E")]
		public string ProcCode;
		[JsonProperty(PropertyName="F")]
		public string ClinicTitle;
		[JsonProperty(PropertyName="G")]
		public string Description;
		[JsonProperty(PropertyName="H")]
		public double Charges;
		
		public string ExcelRow {
			get {
				return
					DateOfInvoice.ToShortDateString()
					//From PatField 'eService ClinicNum'
					+"\t"+PatNum.ToString()
					+"\t"+ClinicTitle
					+"\t"+ProcCode
					+"\t"+Description
					+"\t"+Math.Round(Charges,2).ToString("0.00");
			}
		}

		public override string ToString() {
			return DateOfInvoice.ToShortDateString()+"\t"+ClinicTitle+"\t"+Description+"\t"+Math.Round(Charges,2).ToString("0.00");
		}

		///<summary>Must be connected to customers db. Gets itemized charges which have been serialized to specific eServiceBilling row for given regKeyNum and billDate.</summary>
		public static List<EServiceInvoiceLineItem> GetByRegKeyNumAndBillDate(long regKeyNum,DateTime billDate) {
			//HQ only. No remoting role needed.
			string command="SELECT * FROM eservicebilling WHERE RegistrationKeyNum = "+POut.Long(regKeyNum)+" AND DateOfBill="+POut.Date(billDate);
			EServiceBilling esb=Crud.EServiceBillingCrud.SelectOne(command);
			if(esb==null||string.IsNullOrEmpty(esb.ItemizedCharges)) {
				return new List<EServiceInvoiceLineItem>();
			}
			return JsonConvert.DeserializeObject<List<EServiceInvoiceLineItem>>(esb.ItemizedCharges);
		}

		public static List<string> GetEServiceTaxAdjustmentsByPatNumAndBillDate(long patNum,DateTime billDate) {
			string command=$@"
				SELECT a.AdjDate, pc.ProcCode, pc.Descript,a.AdjAmt
				 FROM adjustment a 
				 INNER JOIN procedurelog pl ON a.ProcNum=pl.ProcNum
				 INNER JOIN procedurecode pc ON pc.CodeNum=pl.CodeNum
				 INNER JOIN eservicecodelink ecl ON ecl.CodeNum=pl.CodeNum
				 WHERE 
					a.PatNum = {POut.Long(patNum)} AND 
					-- Month of invoice
					a.ProcDate = {POut.Date(billDate,true)} AND
					-- Tax DefNum	
					a.AdjType=574  
				 ORDER BY pc.Descript";
			var fromDb=DataCore.GetTable(command).AsEnumerable()
				.Select(x => new {
					DateOfInvoice = PIn.Date(x["AdjDate"].ToString()),
					ClinicTitle = "Sales Tax",
					ProcCode = PIn.String(x["ProcCode"].ToString()),
					Description = PIn.String(x["Descript"].ToString()),
					Charges = PIn.Float(x["AdjAmt"].ToString())
				});
			var ret=fromDb.Select(x => {
				return
					x.DateOfInvoice.ToShortDateString()
					+"\t--" //Placeholder for ClinicNum (patField) column. Does not apply to sales tax row.
					+"\t"+x.ClinicTitle
					+"\t"+x.ProcCode
					+"\t"+x.Description
					+"\t"+Math.Round(x.Charges,2).ToString("0.00");
			}).ToList();
			return ret;
		}
	}
}
#endregion

#region S-Class
namespace OpenDentBusiness {
	public class EServiceBillings {		
		///<summary></summary>
		public static long Insert(EServiceBilling eServiceBilling){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				eServiceBilling.EServiceBillingNum=Meth.GetLong(MethodBase.GetCurrentMethod(),eServiceBilling);
				return eServiceBilling.EServiceBillingNum;
			}
			return Crud.EServiceBillingCrud.Insert(eServiceBilling);
		}
		
		///<summary>Should only be called if ODHQ.</summary>
		public static List<Procedure> AddEServiceRepeatingChargesHelper(DateTime dateRun,List<RepeatCharge> listRepeatCharges) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Procedure>>(MethodBase.GetCurrentMethod(),dateRun,listRepeatCharges);
			}
			DateTime monthRun=new DateTime(dateRun.Year,dateRun.Month,1);
			//Get all bills that are due to be posted as of this date.
			string command="SELECT * FROM eservicebilling "+
				"WHERE DateOfBill <= "+POut.Date(dateRun.Date,true)+" AND DateTimeProceduresPosted = '0001-01-01 00:00:00'";			
			List<EServiceBilling> listBillsDue=Crud.EServiceBillingCrud.SelectMany(command);
			//This table will all ProcedureCodes which are included in the EServiceCodeLink table.
			command="SELECT * FROM procedurecode "
				+"INNER JOIN eservicecodelink ON procedurecode.CodeNum=eservicecodelink.CodeNum";
			List<ProcedureCode> listProcCodes=OpenDentBusiness.Crud.ProcedureCodeCrud.TableToList(DataCore.GetTable(command));
			//Get completed procedures that have already been posted.
			List<Procedure> listProcsComplete=Procedures.GetCompletedForDateRange(monthRun,dateRun,listProcCodes.Select(x => x.CodeNum).ToList());			
			List<Procedure> retVal=new List<Procedure>();
			foreach(EServiceBilling eServiceBilling in listBillsDue) {
				//List of procedures for this billing cycle was serialized to EServiceBilling.ProceduresJson by AccountMaint thread. Deserialize and post them.
				List<Procedure> listProcs=JsonConvert.DeserializeObject<List<Procedure>>(eServiceBilling.ProceduresJson);
				foreach(Procedure proc in listProcs) {
					//For a short time in May 2017 this procedure would have been checked against procedures already posted for the same month for the same eService.
					//If a duplicate was found then this procedure would have been skipped. This blocking code was removed and now a similar procedure can be posted on the same month.
					//This needs to be allowed to happen so that the first month can have 2 charges. 
					//1) For the pro-rated few days until their next BillingCycleDay is reached. 
					//     Note, pro-rating is a future feature that may be allowed to happen via the signup portal. It is not currently but this particular code would no longer prevent it.
					//2) For their typicaly BillingCycleDay charge.
					Procedures.Insert(proc);
					retVal.Add(proc);
					listProcsComplete.Add(proc);
					RepeatCharge repeatCharge=listRepeatCharges.Where(x => x.PatNum==proc.PatNum)
						.FirstOrDefault(x => x.ProcCode==ProcedureCodes.GetStringProcCode(proc.CodeNum,listProcCodes));
					if(repeatCharge!=null) {
						RepeatCharges.AllocateUnearned(repeatCharge,proc,dateRun);
					}
				}
				eServiceBilling.DateTimeProceduresPosted=DateTime_.Now;
				Crud.EServiceBillingCrud.Update(eServiceBilling);
			}
			return retVal;
		}
	}
}
#endregion

#region CRUD
namespace OpenDentBusiness.Crud {
	public class EServiceBillingCrud {
		///<summary>Gets one EServiceBilling object from the database using the primary key.  Returns null if not found.</summary>
		public static EServiceBilling SelectOne(long eServiceBillingNum) {
			string command="SELECT * FROM eservicebilling "
				+"WHERE EServiceBillingNum = "+POut.Long(eServiceBillingNum);
			List<EServiceBilling> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one EServiceBilling object from the database using a query.</summary>
		public static EServiceBilling SelectOne(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<EServiceBilling> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of EServiceBilling objects from the database using a query.</summary>
		public static List<EServiceBilling> SelectMany(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<EServiceBilling> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<EServiceBilling> TableToList(DataTable table) {
			List<EServiceBilling> retVal=new List<EServiceBilling>();
			EServiceBilling eServiceBilling;
			foreach(DataRow row in table.Rows) {
				eServiceBilling=new EServiceBilling();
				eServiceBilling.EServiceBillingNum      = PIn.Long  (row["EServiceBillingNum"].ToString());
				eServiceBilling.RegistrationKeyNum      = PIn.Long  (row["RegistrationKeyNum"].ToString());
				eServiceBilling.CustPatNum              = PIn.Long  (row["CustPatNum"].ToString());
				eServiceBilling.BillingCycleDay         = PIn.Int   (row["BillingCycleDay"].ToString());
				eServiceBilling.DateTimeEntry           = PIn.DateT (row["DateTimeEntry"].ToString());
				eServiceBilling.DateTimeProceduresPosted= PIn.DateT (row["DateTimeProceduresPosted"].ToString());
				eServiceBilling.DateOfBill              = PIn.Date  (row["DateOfBill"].ToString());
				eServiceBilling.MonthOfBill             = PIn.Date  (row["MonthOfBill"].ToString());
				eServiceBilling.BillCycleStart          = PIn.Date  (row["BillCycleStart"].ToString());
				eServiceBilling.BillCycleEnd            = PIn.Date  (row["BillCycleEnd"].ToString());
				eServiceBilling.UsageCycleStart         = PIn.Date  (row["UsageCycleStart"].ToString());
				eServiceBilling.UsageCycleEnd           = PIn.Date  (row["UsageCycleEnd"].ToString());
				eServiceBilling.ProceduresJson          = PIn.String(row["ProceduresJson"].ToString());
				eServiceBilling.ChargesJson             = PIn.String(row["ChargesJson"].ToString());
				eServiceBilling.NexmoInfoJson           = PIn.String(row["NexmoInfoJson"].ToString());
				eServiceBilling.LogInfo                 = PIn.String(row["LogInfo"].ToString());
				eServiceBilling.ItemizedCharges         = PIn.String(row["ItemizedCharges"].ToString());
				retVal.Add(eServiceBilling);
			}
			return retVal;
		}

		///<summary>Converts a list of EServiceBilling into a DataTable.</summary>
		public static DataTable ListToTable(List<EServiceBilling> listEServiceBillings,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="EServiceBilling";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("EServiceBillingNum");
			table.Columns.Add("RegistrationKeyNum");
			table.Columns.Add("CustPatNum");
			table.Columns.Add("BillingCycleDay");
			table.Columns.Add("DateTimeEntry");
			table.Columns.Add("DateTimeProceduresPosted");
			table.Columns.Add("DateOfBill");
			table.Columns.Add("MonthOfBill");
			table.Columns.Add("BillCycleStart");
			table.Columns.Add("BillCycleEnd");
			table.Columns.Add("UsageCycleStart");
			table.Columns.Add("UsageCycleEnd");
			table.Columns.Add("ProceduresJson");
			table.Columns.Add("ChargesJson");
			table.Columns.Add("NexmoInfoJson");
			table.Columns.Add("LogInfo");
			table.Columns.Add("ItemizedCharges");
			foreach(EServiceBilling eServiceBilling in listEServiceBillings) {
				table.Rows.Add(new object[] {
					POut.Long  (eServiceBilling.EServiceBillingNum),
					POut.Long  (eServiceBilling.RegistrationKeyNum),
					POut.Long  (eServiceBilling.CustPatNum),
					POut.Int   (eServiceBilling.BillingCycleDay),
					POut.DateT (eServiceBilling.DateTimeEntry,false),
					POut.DateT (eServiceBilling.DateTimeProceduresPosted,false),
					POut.DateT (eServiceBilling.DateOfBill,false),
					POut.DateT (eServiceBilling.MonthOfBill,false),
					POut.DateT (eServiceBilling.BillCycleStart,false),
					POut.DateT (eServiceBilling.BillCycleEnd,false),
					POut.DateT (eServiceBilling.UsageCycleStart,false),
					POut.DateT (eServiceBilling.UsageCycleEnd,false),
					            eServiceBilling.ProceduresJson,
					            eServiceBilling.ChargesJson,
					            eServiceBilling.NexmoInfoJson,
					            eServiceBilling.LogInfo,
					            eServiceBilling.ItemizedCharges,
				});
			}
			return table;
		}

		///<summary>Inserts one EServiceBilling into the database.  Returns the new priKey.</summary>
		public static long Insert(EServiceBilling eServiceBilling) {
			return Insert(eServiceBilling,false);
		}

		///<summary>Inserts one EServiceBilling into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(EServiceBilling eServiceBilling,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				eServiceBilling.EServiceBillingNum=ReplicationServers.GetKey("eservicebilling","EServiceBillingNum");
			}
			string command="INSERT INTO eservicebilling (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="EServiceBillingNum,";
			}
			command+="RegistrationKeyNum,CustPatNum,BillingCycleDay,DateTimeEntry,DateTimeProceduresPosted,DateOfBill,MonthOfBill,BillCycleStart,BillCycleEnd,UsageCycleStart,UsageCycleEnd,ProceduresJson,ChargesJson,NexmoInfoJson,LogInfo,ItemizedCharges) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(eServiceBilling.EServiceBillingNum)+",";
			}
			command+=
				     POut.Long  (eServiceBilling.RegistrationKeyNum)+","
				+    POut.Long  (eServiceBilling.CustPatNum)+","
				+    POut.Int   (eServiceBilling.BillingCycleDay)+","
				+    DbHelper.Now()+","
				+    POut.DateT (eServiceBilling.DateTimeProceduresPosted)+","
				+    POut.Date  (eServiceBilling.DateOfBill)+","
				+    POut.Date  (eServiceBilling.MonthOfBill)+","
				+    POut.Date  (eServiceBilling.BillCycleStart)+","
				+    POut.Date  (eServiceBilling.BillCycleEnd)+","
				+    POut.Date  (eServiceBilling.UsageCycleStart)+","
				+    POut.Date  (eServiceBilling.UsageCycleEnd)+","
				+    DbHelper.ParamChar+"paramProceduresJson,"
				+    DbHelper.ParamChar+"paramChargesJson,"
				+    DbHelper.ParamChar+"paramNexmoInfoJson,"
				+    DbHelper.ParamChar+"paramLogInfo,"
				+    DbHelper.ParamChar+"paramItemizedCharges)";
			if(eServiceBilling.ProceduresJson==null) {
				eServiceBilling.ProceduresJson="";
			}
			OdSqlParameter paramProceduresJson=new OdSqlParameter("paramProceduresJson",OdDbType.Text,POut.StringParam(eServiceBilling.ProceduresJson));
			if(eServiceBilling.ChargesJson==null) {
				eServiceBilling.ChargesJson="";
			}
			OdSqlParameter paramChargesJson=new OdSqlParameter("paramChargesJson",OdDbType.Text,POut.StringParam(eServiceBilling.ChargesJson));
			if(eServiceBilling.NexmoInfoJson==null) {
				eServiceBilling.NexmoInfoJson="";
			}
			OdSqlParameter paramNexmoInfoJson=new OdSqlParameter("paramNexmoInfoJson",OdDbType.Text,POut.StringParam(eServiceBilling.NexmoInfoJson));
			if(eServiceBilling.LogInfo==null) {
				eServiceBilling.LogInfo="";
			}
			OdSqlParameter paramLogInfo=new OdSqlParameter("paramLogInfo",OdDbType.Text,POut.StringParam(eServiceBilling.LogInfo));
			if(eServiceBilling.ItemizedCharges==null) {
				eServiceBilling.ItemizedCharges="";
			}
			OdSqlParameter paramItemizedCharges=new OdSqlParameter("paramItemizedCharges",OdDbType.Text,POut.StringParam(eServiceBilling.ItemizedCharges));
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command,paramProceduresJson,paramChargesJson,paramNexmoInfoJson,paramLogInfo,paramItemizedCharges);
			}
			else {
				eServiceBilling.EServiceBillingNum=Db.NonQ(command,true,"EServiceBillingNum","eServiceBilling",paramProceduresJson,paramChargesJson,paramNexmoInfoJson,paramLogInfo,paramItemizedCharges);
			}
			return eServiceBilling.EServiceBillingNum;
		}

		///<summary>Inserts one EServiceBilling into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(EServiceBilling eServiceBilling) {
			return InsertNoCache(eServiceBilling,false);
		}

		///<summary>Inserts one EServiceBilling into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(EServiceBilling eServiceBilling,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO eservicebilling (";
			if(!useExistingPK && isRandomKeys) {
				eServiceBilling.EServiceBillingNum=ReplicationServers.GetKeyNoCache("eservicebilling","EServiceBillingNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="EServiceBillingNum,";
			}
			command+="RegistrationKeyNum,CustPatNum,BillingCycleDay,DateTimeEntry,DateTimeProceduresPosted,DateOfBill,MonthOfBill,BillCycleStart,BillCycleEnd,UsageCycleStart,UsageCycleEnd,ProceduresJson,ChargesJson,NexmoInfoJson,LogInfo,ItemizedCharges) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(eServiceBilling.EServiceBillingNum)+",";
			}
			command+=
				     POut.Long  (eServiceBilling.RegistrationKeyNum)+","
				+    POut.Long  (eServiceBilling.CustPatNum)+","
				+    POut.Int   (eServiceBilling.BillingCycleDay)+","
				+    DbHelper.Now()+","
				+    POut.DateT (eServiceBilling.DateTimeProceduresPosted)+","
				+    POut.Date  (eServiceBilling.DateOfBill)+","
				+    POut.Date  (eServiceBilling.MonthOfBill)+","
				+    POut.Date  (eServiceBilling.BillCycleStart)+","
				+    POut.Date  (eServiceBilling.BillCycleEnd)+","
				+    POut.Date  (eServiceBilling.UsageCycleStart)+","
				+    POut.Date  (eServiceBilling.UsageCycleEnd)+","
				+    DbHelper.ParamChar+"paramProceduresJson,"
				+    DbHelper.ParamChar+"paramChargesJson,"
				+    DbHelper.ParamChar+"paramNexmoInfoJson,"
				+    DbHelper.ParamChar+"paramLogInfo,"
				+    DbHelper.ParamChar+"paramItemizedCharges)";
			if(eServiceBilling.ProceduresJson==null) {
				eServiceBilling.ProceduresJson="";
			}
			OdSqlParameter paramProceduresJson=new OdSqlParameter("paramProceduresJson",OdDbType.Text,POut.StringParam(eServiceBilling.ProceduresJson));
			if(eServiceBilling.ChargesJson==null) {
				eServiceBilling.ChargesJson="";
			}
			OdSqlParameter paramChargesJson=new OdSqlParameter("paramChargesJson",OdDbType.Text,POut.StringParam(eServiceBilling.ChargesJson));
			if(eServiceBilling.NexmoInfoJson==null) {
				eServiceBilling.NexmoInfoJson="";
			}
			OdSqlParameter paramNexmoInfoJson=new OdSqlParameter("paramNexmoInfoJson",OdDbType.Text,POut.StringParam(eServiceBilling.NexmoInfoJson));
			if(eServiceBilling.LogInfo==null) {
				eServiceBilling.LogInfo="";
			}
			OdSqlParameter paramLogInfo=new OdSqlParameter("paramLogInfo",OdDbType.Text,POut.StringParam(eServiceBilling.LogInfo));
			if(eServiceBilling.ItemizedCharges==null) {
				eServiceBilling.ItemizedCharges="";
			}
			OdSqlParameter paramItemizedCharges=new OdSqlParameter("paramItemizedCharges",OdDbType.Text,POut.StringParam(eServiceBilling.ItemizedCharges));
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command,paramProceduresJson,paramChargesJson,paramNexmoInfoJson,paramLogInfo,paramItemizedCharges);
			}
			else {
				eServiceBilling.EServiceBillingNum=Db.NonQ(command,true,"EServiceBillingNum","eServiceBilling",paramProceduresJson,paramChargesJson,paramNexmoInfoJson,paramLogInfo,paramItemizedCharges);
			}
			return eServiceBilling.EServiceBillingNum;
		}

		///<summary>Updates one EServiceBilling in the database.</summary>
		public static void Update(EServiceBilling eServiceBilling) {
			string command="UPDATE eservicebilling SET "
				+"RegistrationKeyNum      =  "+POut.Long  (eServiceBilling.RegistrationKeyNum)+", "
				+"CustPatNum              =  "+POut.Long  (eServiceBilling.CustPatNum)+", "
				+"BillingCycleDay         =  "+POut.Int   (eServiceBilling.BillingCycleDay)+", "
				//DateTimeEntry not allowed to change
				+"DateTimeProceduresPosted=  "+POut.DateT (eServiceBilling.DateTimeProceduresPosted)+", "
				+"DateOfBill              =  "+POut.Date  (eServiceBilling.DateOfBill)+", "
				+"MonthOfBill             =  "+POut.Date  (eServiceBilling.MonthOfBill)+", "
				+"BillCycleStart          =  "+POut.Date  (eServiceBilling.BillCycleStart)+", "
				+"BillCycleEnd            =  "+POut.Date  (eServiceBilling.BillCycleEnd)+", "
				+"UsageCycleStart         =  "+POut.Date  (eServiceBilling.UsageCycleStart)+", "
				+"UsageCycleEnd           =  "+POut.Date  (eServiceBilling.UsageCycleEnd)+", "
				+"ProceduresJson          =  "+DbHelper.ParamChar+"paramProceduresJson, "
				+"ChargesJson             =  "+DbHelper.ParamChar+"paramChargesJson, "
				+"NexmoInfoJson           =  "+DbHelper.ParamChar+"paramNexmoInfoJson, "
				+"LogInfo                 =  "+DbHelper.ParamChar+"paramLogInfo, "
				+"ItemizedCharges         =  "+DbHelper.ParamChar+"paramItemizedCharges "
				+"WHERE EServiceBillingNum = "+POut.Long(eServiceBilling.EServiceBillingNum);
			if(eServiceBilling.ProceduresJson==null) {
				eServiceBilling.ProceduresJson="";
			}
			OdSqlParameter paramProceduresJson=new OdSqlParameter("paramProceduresJson",OdDbType.Text,POut.StringParam(eServiceBilling.ProceduresJson));
			if(eServiceBilling.ChargesJson==null) {
				eServiceBilling.ChargesJson="";
			}
			OdSqlParameter paramChargesJson=new OdSqlParameter("paramChargesJson",OdDbType.Text,POut.StringParam(eServiceBilling.ChargesJson));
			if(eServiceBilling.NexmoInfoJson==null) {
				eServiceBilling.NexmoInfoJson="";
			}
			OdSqlParameter paramNexmoInfoJson=new OdSqlParameter("paramNexmoInfoJson",OdDbType.Text,POut.StringParam(eServiceBilling.NexmoInfoJson));
			if(eServiceBilling.LogInfo==null) {
				eServiceBilling.LogInfo="";
			}
			OdSqlParameter paramLogInfo=new OdSqlParameter("paramLogInfo",OdDbType.Text,POut.StringParam(eServiceBilling.LogInfo));
			if(eServiceBilling.ItemizedCharges==null) {
				eServiceBilling.ItemizedCharges="";
			}
			OdSqlParameter paramItemizedCharges=new OdSqlParameter("paramItemizedCharges",OdDbType.Text,POut.StringParam(eServiceBilling.ItemizedCharges));
			Db.NonQ(command,paramProceduresJson,paramChargesJson,paramNexmoInfoJson,paramLogInfo,paramItemizedCharges);
		}

		///<summary>Updates one EServiceBilling in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(EServiceBilling eServiceBilling,EServiceBilling oldEServiceBilling) {
			string command="";
			if(eServiceBilling.RegistrationKeyNum != oldEServiceBilling.RegistrationKeyNum) {
				if(command!="") { command+=",";}
				command+="RegistrationKeyNum = "+POut.Long(eServiceBilling.RegistrationKeyNum)+"";
			}
			if(eServiceBilling.CustPatNum != oldEServiceBilling.CustPatNum) {
				if(command!="") { command+=",";}
				command+="CustPatNum = "+POut.Long(eServiceBilling.CustPatNum)+"";
			}
			if(eServiceBilling.BillingCycleDay != oldEServiceBilling.BillingCycleDay) {
				if(command!="") { command+=",";}
				command+="BillingCycleDay = "+POut.Int(eServiceBilling.BillingCycleDay)+"";
			}
			//DateTimeEntry not allowed to change
			if(eServiceBilling.DateTimeProceduresPosted != oldEServiceBilling.DateTimeProceduresPosted) {
				if(command!="") { command+=",";}
				command+="DateTimeProceduresPosted = "+POut.DateT(eServiceBilling.DateTimeProceduresPosted)+"";
			}
			if(eServiceBilling.DateOfBill.Date != oldEServiceBilling.DateOfBill.Date) {
				if(command!="") { command+=",";}
				command+="DateOfBill = "+POut.Date(eServiceBilling.DateOfBill)+"";
			}
			if(eServiceBilling.MonthOfBill.Date != oldEServiceBilling.MonthOfBill.Date) {
				if(command!="") { command+=",";}
				command+="MonthOfBill = "+POut.Date(eServiceBilling.MonthOfBill)+"";
			}
			if(eServiceBilling.BillCycleStart.Date != oldEServiceBilling.BillCycleStart.Date) {
				if(command!="") { command+=",";}
				command+="BillCycleStart = "+POut.Date(eServiceBilling.BillCycleStart)+"";
			}
			if(eServiceBilling.BillCycleEnd.Date != oldEServiceBilling.BillCycleEnd.Date) {
				if(command!="") { command+=",";}
				command+="BillCycleEnd = "+POut.Date(eServiceBilling.BillCycleEnd)+"";
			}
			if(eServiceBilling.UsageCycleStart.Date != oldEServiceBilling.UsageCycleStart.Date) {
				if(command!="") { command+=",";}
				command+="UsageCycleStart = "+POut.Date(eServiceBilling.UsageCycleStart)+"";
			}
			if(eServiceBilling.UsageCycleEnd.Date != oldEServiceBilling.UsageCycleEnd.Date) {
				if(command!="") { command+=",";}
				command+="UsageCycleEnd = "+POut.Date(eServiceBilling.UsageCycleEnd)+"";
			}
			if(eServiceBilling.ProceduresJson != oldEServiceBilling.ProceduresJson) {
				if(command!="") { command+=",";}
				command+="ProceduresJson = "+DbHelper.ParamChar+"paramProceduresJson";
			}
			if(eServiceBilling.ChargesJson != oldEServiceBilling.ChargesJson) {
				if(command!="") { command+=",";}
				command+="ChargesJson = "+DbHelper.ParamChar+"paramChargesJson";
			}
			if(eServiceBilling.NexmoInfoJson != oldEServiceBilling.NexmoInfoJson) {
				if(command!="") { command+=",";}
				command+="NexmoInfoJson = "+DbHelper.ParamChar+"paramNexmoInfoJson";
			}
			if(eServiceBilling.LogInfo != oldEServiceBilling.LogInfo) {
				if(command!="") { command+=",";}
				command+="LogInfo = "+DbHelper.ParamChar+"paramLogInfo";
			}
			if(eServiceBilling.ItemizedCharges != oldEServiceBilling.ItemizedCharges) {
				if(command!="") { command+=",";}
				command+="ItemizedCharges = "+DbHelper.ParamChar+"paramItemizedCharges";
			}
			if(command=="") {
				return false;
			}
			if(eServiceBilling.ProceduresJson==null) {
				eServiceBilling.ProceduresJson="";
			}
			OdSqlParameter paramProceduresJson=new OdSqlParameter("paramProceduresJson",OdDbType.Text,POut.StringParam(eServiceBilling.ProceduresJson));
			if(eServiceBilling.ChargesJson==null) {
				eServiceBilling.ChargesJson="";
			}
			OdSqlParameter paramChargesJson=new OdSqlParameter("paramChargesJson",OdDbType.Text,POut.StringParam(eServiceBilling.ChargesJson));
			if(eServiceBilling.NexmoInfoJson==null) {
				eServiceBilling.NexmoInfoJson="";
			}
			OdSqlParameter paramNexmoInfoJson=new OdSqlParameter("paramNexmoInfoJson",OdDbType.Text,POut.StringParam(eServiceBilling.NexmoInfoJson));
			if(eServiceBilling.LogInfo==null) {
				eServiceBilling.LogInfo="";
			}
			OdSqlParameter paramLogInfo=new OdSqlParameter("paramLogInfo",OdDbType.Text,POut.StringParam(eServiceBilling.LogInfo));
			if(eServiceBilling.ItemizedCharges==null) {
				eServiceBilling.ItemizedCharges="";
			}
			OdSqlParameter paramItemizedCharges=new OdSqlParameter("paramItemizedCharges",OdDbType.Text,POut.StringParam(eServiceBilling.ItemizedCharges));
			command="UPDATE eservicebilling SET "+command
				+" WHERE EServiceBillingNum = "+POut.Long(eServiceBilling.EServiceBillingNum);
			Db.NonQ(command,paramProceduresJson,paramChargesJson,paramNexmoInfoJson,paramLogInfo,paramItemizedCharges);
			return true;
		}

		///<summary>Returns true if Update(EServiceBilling,EServiceBilling) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(EServiceBilling eServiceBilling,EServiceBilling oldEServiceBilling) {
			if(eServiceBilling.RegistrationKeyNum != oldEServiceBilling.RegistrationKeyNum) {
				return true;
			}
			if(eServiceBilling.CustPatNum != oldEServiceBilling.CustPatNum) {
				return true;
			}
			if(eServiceBilling.BillingCycleDay != oldEServiceBilling.BillingCycleDay) {
				return true;
			}
			//DateTimeEntry not allowed to change
			if(eServiceBilling.DateTimeProceduresPosted != oldEServiceBilling.DateTimeProceduresPosted) {
				return true;
			}
			if(eServiceBilling.DateOfBill.Date != oldEServiceBilling.DateOfBill.Date) {
				return true;
			}
			if(eServiceBilling.MonthOfBill.Date != oldEServiceBilling.MonthOfBill.Date) {
				return true;
			}
			if(eServiceBilling.BillCycleStart.Date != oldEServiceBilling.BillCycleStart.Date) {
				return true;
			}
			if(eServiceBilling.BillCycleEnd.Date != oldEServiceBilling.BillCycleEnd.Date) {
				return true;
			}
			if(eServiceBilling.UsageCycleStart.Date != oldEServiceBilling.UsageCycleStart.Date) {
				return true;
			}
			if(eServiceBilling.UsageCycleEnd.Date != oldEServiceBilling.UsageCycleEnd.Date) {
				return true;
			}
			if(eServiceBilling.ProceduresJson != oldEServiceBilling.ProceduresJson) {
				return true;
			}
			if(eServiceBilling.ChargesJson != oldEServiceBilling.ChargesJson) {
				return true;
			}
			if(eServiceBilling.NexmoInfoJson != oldEServiceBilling.NexmoInfoJson) {
				return true;
			}
			if(eServiceBilling.LogInfo != oldEServiceBilling.LogInfo) {
				return true;
			}
			if(eServiceBilling.ItemizedCharges != oldEServiceBilling.ItemizedCharges) {
				return true;
			}
			return false;
		}

		///<summary>Deletes one EServiceBilling from the database.</summary>
		public static void Delete(long eServiceBillingNum) {
			string command="DELETE FROM eservicebilling "
				+"WHERE EServiceBillingNum = "+POut.Long(eServiceBillingNum);
			Db.NonQ(command);
		}

	}
}
#endregion

#region CREATE TABLE Script
//CREATE TABLE eservicebilling(
//	EServiceBillingNum bigint NOT NULL auto_increment PRIMARY KEY,
//	RegistrationKeyNum bigint NOT NULL,
//	CustPatNum bigint NOT NULL,
//	BillingCycleDay int NOT NULL,
//	DateTimeEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
//	DateTimeProceduresPosted datetime NOT NULL DEFAULT '0001-01-01 00:00:00',						
//	DateOfBill date NOT NULL DEFAULT '0001-01-01',
//	MonthOfBill date NOT NULL DEFAULT '0001-01-01',
//	BillCycleStart date NOT NULL DEFAULT '0001-01-01',
//	BillCycleEnd date NOT NULL DEFAULT '0001-01-01',
//	UsageCycleStart date NOT NULL DEFAULT '0001-01-01',
//	UsageCycleEnd date NOT NULL DEFAULT '0001-01-01',
//	ProceduresJson text NOT NULL,
//	ChargesJson text NOT NULL,
//	NexmoInfoJson text NOT NULL,
//	LogInfo text NOT NULL,
//	ItemizedCharges text NOT NULL,
//	INDEX(RegistrationKeyNum),
//    INDEX(CustPatNum)
//	) DEFAULT CHARSET=utf8
#endregion