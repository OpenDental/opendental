using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Windows.Forms;
using DataConnectionBase;
using OpenDental.Reporting.Allocators.MyAllocator1;
using OpenDentBusiness;

namespace OpenDental.Reporting.Allocators {
	class MyAllocator1_ProviderPayment:Allocator,IAllocate {
		public static readonly string TABLENAME = "allocation_provider";
		/// <summary>
		/// The id of this allocator.  Programmer assigned.
		/// </summary>
		public static int AllocationTypeID = 1;
		/// <summary>
		/// These are the columns used in the table defined in TABLENAME
		/// </summary>
		public static readonly string[] TABLE_COLUMNS = 
				{ "AllocNum", "AllocType", "Guarantor", 
				  "ProvNum", "PayTableSource", "PaySourceNum",
				  "AllocToTableSource", "AllocToSourceNum",
			      "Amount", "IsFullyAllocated" };
		public static readonly string Pref_AllocatorProvider1_Use = "AllocatorProvider1_Use";
		public static readonly string Pref_AllocatorProvider1_ToolHasRun = "AllocatorProvider1_ToolHasRun";
		public MyAllocator1_ProviderPayment()
			: base(TABLENAME,TABLE_COLUMNS) {
			//SetDbaseTable_and_Columns(TABLENAME, TABLE_COLUMNS);
		}
		#region IAllocate Members & Mandatory Overrides
		/// <summary>
		/// Where it All Happens.  The Ledger is Filled.  The EqualizePaymentsV2 runs the 
		/// algorithim that allocates the Ledger AND writes the values to 
		/// the table allocation_provider.  No Table is written to other than allocation_provider
		/// Fill uses only logic from the data in allocation_provider and current status of 
		/// various opendental tables to determine what needs to be changed.
		/// </summary>
		public override bool Allocate(int iGuarantor) {

			return AllocateWithToolCheck(iGuarantor);

		}
		/// <summary>
		/// Does not check to see if Preferences are set in OD to handle this allocator.  This is only used in the Batch
		/// processing of the Allocator Tool.  Do not want the overhead of checking the preferences everytime the tool is run.
		/// Probably not a big deal unless it tries to get the preference from the Dbase every time.
		/// </summary>
		private bool AllocateWithOutToolCheck(int iGuarantor) {
			bool AllocatedNormally = true;
			try {
				_AllocateExecute(iGuarantor);
			}
			catch {
				AllocatedNormally = false;
			}
			return AllocatedNormally;
		}
		/// <summary>
		/// Where the actual allocation occurs.  Put it in this method so that we could run the allocator in different 
		/// circumstances.
		/// </summary>
		/// <param name="iGuarantor"></param>
		private static void _AllocateExecute(long iGuarantor) {
			OpenDental.Reporting.Allocators.MyAllocator1.GuarantorLedgerItemsCollection Ledger =
								new OpenDental.Reporting.Allocators.MyAllocator1.GuarantorLedgerItemsCollection(iGuarantor);
			Ledger.Fill(false);
			Ledger.EqualizePaymentsV2();

		}

		/// <summary>
		/// Calls Allocate but first checks for existance of prefences and an indication that The tool has run.
		/// The reason for the overlaid method is becuase the tool when it runs
		/// <href = "AllocatorCollection.CallAll_Allocators>  See the AllocatorCollection.CallAll_Allocators </href
		/// </summary>
		/// <param name="iGuarantor"></param>
		/// <returns></returns>
		public static bool AllocateWithToolCheck(long iGuarantor) {
			bool AllocatedNomally = false;
			try {
				bool toolRan = false;
				bool isUsing = false;
				if(PrefC.ContainsKey(MyAllocator1_ProviderPayment.Pref_AllocatorProvider1_ToolHasRun))
					toolRan = PrefC.GetRaw(MyAllocator1_ProviderPayment.Pref_AllocatorProvider1_ToolHasRun)=="1";
				if(PrefC.ContainsKey(MyAllocator1_ProviderPayment.Pref_AllocatorProvider1_Use))
					isUsing = PrefC.GetRaw(MyAllocator1_ProviderPayment.Pref_AllocatorProvider1_Use)=="1";
				if(toolRan & isUsing) {
					_AllocateExecute(iGuarantor);
					AllocatedNomally = true;
				}
			}
			catch {
				AllocatedNomally = false;
			}
			return AllocatedNomally;
		}


		public override bool DeAllocate(int iGuarantor) {
			throw new Exception("The method or operation is not implemented.");
		}


		///// <summary>
		///// Sets the inherited properties.
		///// Really did not wan't this member public
		///// </summary>
		///// <param name="tableName"></param>
		///// <param name="Columns"></param>
		//public override void SetDbaseTable_and_Columns(string tableName, string[] Columns)
		//{
		//    this.DbaseStorageTable = tableName;
		//    this.DbaseTableColumns = Columns;
		//}

		#endregion

		#region SQL Interaction Methods
		public static string CreatTableString(DatabaseType type1) {
			// Note command to create table
			// //Db.NonQ(MyAllocator1_ProviderPayment.CreatTableString());
			// Put here for reference not for implementation of code. CreatTableString does not check for existance of table.
			string command = "";
			if(type1 == DatabaseType.MySql) {
				command = "CREATE TABLE " + TABLENAME + " ("
			   + @"AllocNum INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
					AllocType TINYINT,
					Guarantor BIGINT NOT NULL,
					ProvNum bigint DEFAULT 0,
					PayTableSource TINYINT,
					PaySourceNum bigint,
					AllocToTableSource TINYINT,
					AllocToSourceNum int,
					Amount DECIMAL (10,2) NOT NULL,
					IsFullyAllocated TINYINT
					)
					DEFAULT CHARSET=utf8";
			}
			//else if (type1 == OpenDentBusiness.DatabaseType.Oracle)
			//;// not implemented yet
			return command;


		}
		/// <summary>
		/// Generates a string in form of '(#,#,#,#,#,#, ##.##, 0)' just so things match up.
		/// Designed to be used with a statement such as INSERT INTO `account` VALUES (1,'Checking Account',0,'',0,-1),(2,'Ca ....
		/// </summary>
		public static string ValueString(int iAllocType,uint uiGuarantor,int iProvNum,
										 int iTableSource,ulong ulSourceNum,
										 int iAllocatedTo_TableSource,ulong ulAllocatedTo_SourceNum,
										decimal dAmmount,bool IsFullyAlloc1) {
			// Example of query
			// LOCK TABLES `account` WRITE;
			//INSERT INTO `account` VALUES (1,'Checking Account',0,'',0,-1),(2,'Cash Box',0,'',0,-
			//,(4,'Equipment',0,'',0,-1),(5,'Accumulated Depreciation, Equipment',0,'',0,-1),(6,'B
			//ated Capital',2,'',0,-1),(8,'Retained Earnings',2,'',0,-1),(9,'Patient Fee Income',3
			//'',0,-1),(11,'Supplies',4,'',0,-1),(12,'Services',4,'',0,-1),(13,'Wages',4,'',0,-1);
			//UNLOCK TABLES;
			string rVal =  "(" + iAllocType + " , " + uiGuarantor 
					+ " , " + iProvNum + " , " + iTableSource + " , " + ulSourceNum
					+ " , " + iAllocatedTo_TableSource + " , " + ulAllocatedTo_SourceNum 
					+ " , " + dAmmount + " , " + (IsFullyAlloc1 ? 1 : 0).ToString() +")";
			return rVal;
		}
		/// <summary>
		/// The Header of the insert statement.  'INSERT INTO [tablename] (field1, field2...) VALUES '
		/// use with ValueString(int ...)
		/// </summary>
		public static string ValueStringHeader() {
			return "INSERT INTO " + MyAllocator1_ProviderPayment.TABLENAME
				+ "(AllocType, Guarantor, ProvNum, "
				+ "PayTableSource, PaySourceNum, AllocToTableSource, AllocToSourceNum, Amount, IsFullyAllocated) VALUES ";
		}

		public static string Create_AP_temp_table_string(DatabaseType type1) {
			string command = "";
			if(type1 == DatabaseType.MySql) {
				command = "CREATE TABLE " + TABLENAME + "_temp \n(\n"
					+ "   tempIndex INT NOT NULL AUTO_INCREMENT PRIMARY KEY, \n"
					+ "   Guarantor bigint NOT NULL, \n"
					+ "   AllocStatus TINYINT\n)\n"
					+ "DEFAULT CHARSET=utf8";
			}
			//else if(type1 == OpenDentBusiness.DatabaseType.Oracle)
			//	;// not implemented yet
			return command;
		}
		#endregion

		#region Methods for Batch Processing
		// When the allocator is first used it will take a lot of time to update.  It should be written
		// in its own thread so it can be cancled and completed in a timely manner.
		// 
		//  create new temporary table  allocation_provider_temp
		//  3 fields
		//	Index, Guarantor, ProcessingState
		//
		//  enum ProcessingState {NeverStarted, Started, Complete}
		// This way we can look for any Started Objects that were not Completed incase of a Dbase Connection Failure
		//  and fix the problem.
		//
		//  Fill table first with all Guantors,  Markethem as NeverStarted
		//  Run the Equalizer in batches of 50;
		// Match Signature of DoWorkEventHandler which is void bw_DoWork(object sender, DoWorkEventArgs e)
		enum ProcessingState { NeverStarted, Started_and_Incomplete, Complete };
		public void StartBatchAllocation() {
			FormProgressViewer fpv = new FormProgressViewer();
			fpv.SET_WORKER_METHOD = new System.ComponentModel.DoWorkEventHandler(StartBatch_DoWork_Handler);
			fpv.START_WORK();
			fpv.ShowDialog();
		}
		private void StartBatch_DoWork_Handler(object sender,System.ComponentModel.DoWorkEventArgs e) {


			System.ComponentModel.BackgroundWorker bw = (System.ComponentModel.BackgroundWorker)sender;
			if(bw == null) {
				PU.MB = "This method is to be called by a a background worker.\n" + PU.Method;
				return;
			}
			DateTime StartTime = DateTime.Now;
			bw.ReportProgress(0,"Batch Processing Started at: " + String.Format("{0}h:{1}m.{2}",StartTime.Hour,StartTime.Minute,StartTime.Second));
			// Generate list of Guarantors to allocate
			int[] GuarantorsToAllocate = Generate_GuarantorList_ToAllocate();
			if(GuarantorsToAllocate == null || GuarantorsToAllocate.Length == 0) {
				MessageBox.Show(Lan.g(this,"No Guarantors to Allocate. Exiting."));
				return;
			}
			int BatchSize = 50;
			int iProgress = 0;
			OpenDental.Reporting.Allocators.MyAllocator1_ProviderPayment allocator = new OpenDental.Reporting.Allocators.MyAllocator1_ProviderPayment();
			for(int i = 0;i < GuarantorsToAllocate.Length + BatchSize;i = i + BatchSize) {

				int from = i;
				int to = (i + BatchSize > GuarantorsToAllocate.Length ? GuarantorsToAllocate.Length : i + BatchSize);
				iProgress = (to * 100) / (GuarantorsToAllocate.Length - 1);
				bw.ReportProgress(iProgress,"Beginning Batch " + from + "-" + to + " of " + GuarantorsToAllocate.Length + " Guarantors.");
				for(int j = from;j <to;j++) {
					string UpdateTempTableCommand = "INSERT INTO " + TABLENAME + "_temp (Guarantor,AllocStatus) VALUES "
						+ "( " + GuarantorsToAllocate[j].ToString() + ", " + ((int)ProcessingState.Started_and_Incomplete).ToString() + " ) ";
					Db.NonQOld(UpdateTempTableCommand);
					allocator.AllocateWithOutToolCheck(GuarantorsToAllocate[j]);
					iProgress = (j*100) / (GuarantorsToAllocate.Length - 1);
					if(bw.CancellationPending) // Try to cancel between allocations which does a lot of writting
					{
						i = GuarantorsToAllocate.Length; // end loop
						j = to;
						iProgress = (j * 100) / (GuarantorsToAllocate.Length - 1); // recalculate progrss.
						bw.ReportProgress(iProgress,"User Cancelled Requested");
						e.Cancel = true;
					}
					UpdateTempTableCommand = "UPDATE " + TABLENAME + "_temp SET AllocStatus =" + ((int)ProcessingState.Complete).ToString() 
							+"  WHERE Guarantor = " + GuarantorsToAllocate[j].ToString();
					Db.NonQOld(UpdateTempTableCommand);
				}
				iProgress = (to * 100) / (GuarantorsToAllocate.Length - 1);
				bw.ReportProgress(iProgress,"Finished Batch " + from + "-" + to + " of " + GuarantorsToAllocate.Length + " Guarantors.");
			}
			DateTime EndTime = DateTime.Now;
			bw.ReportProgress(iProgress,"Batch Processing Ended at: " + String.Format("{0}h:{1}m.{2}",EndTime.Hour,EndTime.Minute,EndTime.Second));
			TimeSpan timeSpan1 = new TimeSpan(EndTime.Ticks - StartTime.Ticks);
			bw.ReportProgress(iProgress,"Batch Processing Total Time is: " + String.Format("{0}h:{1}m.{2}",timeSpan1.Hours,timeSpan1.Minutes,timeSpan1.Seconds));
			e.Result = "Completed Processing of all " + GuarantorsToAllocate.Length + " Guarantors.";

		}
		private bool CheckTableStatus() {
			bool rValOK_TO_RUN = false;
			if(!TableExists(TABLENAME)) {
				DialogResult dr = MessageBox.Show(Lan.g(this,"The Table for holding provider split infomation generated\n"
					+ "by MyAllocator1 does not exist."
					+ "Creation of this table is required create set up allocation by provider\n"
					+ "according to the rules in MyAllocator1.\n\n"
					+ "Do you want to create this table?"),Lan.g(this,"Create Table"),MessageBoxButtons.YesNoCancel);
				if(dr == DialogResult.Yes) {
					Db.NonQOld(MyAllocator1_ProviderPayment.CreatTableString(DatabaseType.MySql));
				}
			}
			rValOK_TO_RUN = TableExists(TABLENAME);
			return rValOK_TO_RUN;
		}
		/// <summary>
		/// Simple utility method to tell you if a table exits or not.
		/// </summary>
		private bool TableExists(string tblName) {
			bool rvalExists = false;
			string cmd = "SHOW TABLES LIKE '" + tblName + "'";//will only conflict with views if view name exactly matches table name.
			DataTable dt = Db.GetTableOld(cmd);
			if(dt != null && dt.Rows.Count != 0 && dt.Columns.Count != 0) {
				if(dt.Rows[0][0].ToString() == tblName)
					rvalExists = true;
			}
			return rvalExists;
		}
		/// <summary>
		/// Returns a list of guarantors that require allocation. 
		/// Will look for a half finished job from previous.
		/// Returns null if user does not want to continue a suspended job.
		/// </summary>
		private int[] Generate_GuarantorList_ToAllocate() {
			int[] rValue = null;
			// Check status of an incomplete last run.  Did it complete well?
			string TempTableName = TABLENAME + "_temp";
			bool TempTableExists = TableExists(TempTableName);
			int[] FullyProcessedGuarantors = null;
			int[] PartiallyProcessedGuarantors = null;
			List<int> OD_Guarantors =  new List<int>();
			List<int> OD_Guarantors_NeedProcessing = null;// new List<int>();

			// Find fully processed Guarantors
			#region Check with users on continuing an incomplete batch run
			if(TempTableExists) {
				DialogResult dr = MessageBox.Show(Lan.g(this,"Processing was incomplete during last\n"
					+ "run of the batch allocation process. Do you want to start over?\n"
					+ "This will likely result in a loss of data but the data will\n"
					+ "be rebuilt\n\nDO YOU WANT TO START OVER?"),"Warning",MessageBoxButtons.YesNo);
				if(dr == DialogResult.Yes) // THEY Want to START OVER --DROP TABLE AND RECREATE
				{
					string dropCommand = "DROP TABLE " + TABLENAME + "_temp";
					Db.NonQOld(dropCommand);
					dropCommand = "DROP TABLE " + TABLENAME;
					Db.NonQOld(dropCommand);
					Db.NonQOld(CreatTableString(DatabaseType.MySql));
					Db.NonQOld(Create_AP_temp_table_string(DatabaseType.MySql));
				}
				if(dr == DialogResult.No) // Don't want to start over 
				{

					DialogResult dr2 = MessageBox.Show(Lan.g(this,"Do you want to continue from where you left of?\n"
						+ "If you state no the allocation process will be aborted.\n\nContinue?"),
						Lan.g(this,"Continue?"),MessageBoxButtons.YesNo);
					if(dr2 == DialogResult.No)
						return null; // don't want to continue
				}

			}
			else // Temp table does not exists so create it!
			{
				Db.NonQOld(Create_AP_temp_table_string(DatabaseType.MySql));
			}
			#endregion
			//  Here is what needs to be done:
			//		1) Find Guarantors with incomplete processing
			//		2) Drop the entries in the allocation_provier table associated with this guarantor
			//		3) Set _temp table Status for this guarantor
			//		4) Generate a list of unprocessed guarantors.

			// continuing
			#region Handle Incomplete Proccessed Guarantors
			string cmd2 = "SELECT tempIndex, Guarantor, AllocStatus FROM " + TempTableName
					+ " WHERE AllocStatus = " + ((int)ProcessingState.Started_and_Incomplete).ToString();
			DataTable dt = Db.GetTableOld(cmd2);
			if(dt.Rows.Count != 0) {
				string deleteCommand1 = "DELETE FROM " + TABLENAME + " WHERE ";
				string updateCommand1 = "UPDATE " + TABLENAME + "_temp SET AllocStatus = " + ((int)ProcessingState.NeverStarted).ToString() + "\n WHERE ";
				PartiallyProcessedGuarantors = new int[dt.Rows.Count];
				for(int i = 0;i < dt.Rows.Count;i++) {
					PartiallyProcessedGuarantors[i] = Int32.Parse(dt.Rows[i][1].ToString());
					deleteCommand1 += "\nGuarantor = " + PartiallyProcessedGuarantors[i].ToString() + "\n";
					updateCommand1 += "\nGuarantor = " + PartiallyProcessedGuarantors[i].ToString() + "\n";
					if(i < dt.Rows.Count - 1) {
						deleteCommand1 += "OR ";
						updateCommand1 += "OR ";
					}
				}
				Db.NonQOld(deleteCommand1); // deletes entries from allocation_provider
				Db.NonQOld(updateCommand1);// updates status in allocation_provider_temp
			}
			#endregion
			#region Generate a list of unprocessed guarantors
			string ProccessedGuarantors_command = "SELECT Guarantor FROM " + TABLENAME + "_temp WHERE AllocStatus = "
					+ ((int)ProcessingState.Complete);
			DataTable dt3 = Db.GetTableOld(ProccessedGuarantors_command);
			DataTable dtODGuarantors = Db.GetTableOld("SELECT DISTINCT(Guarantor) FROM Patient");
			for(int i = 0;i < dtODGuarantors.Rows.Count;i++)
				OD_Guarantors.Add(Int32.Parse(dtODGuarantors.Rows[i][0].ToString()));
			if(dt3.Rows.Count != 0) {
				FullyProcessedGuarantors = new int[dt3.Rows.Count];
				OD_Guarantors_NeedProcessing = new List<int>();
				OD_Guarantors_NeedProcessing.AddRange(OD_Guarantors);
				for(int i = 0;i < dt3.Rows.Count;i++) {
					FullyProcessedGuarantors[i] = Int32.Parse(dt3.Rows[i][0].ToString());
					if(OD_Guarantors_NeedProcessing.Contains(FullyProcessedGuarantors[i]))
						OD_Guarantors_NeedProcessing.Remove(FullyProcessedGuarantors[i]);
				}
			}
			else
				OD_Guarantors_NeedProcessing = OD_Guarantors;
			#endregion

			if(OD_Guarantors_NeedProcessing != null)
				rValue = OD_Guarantors_NeedProcessing.ToArray();
			return rValue;
		}


		#endregion
	}
}
