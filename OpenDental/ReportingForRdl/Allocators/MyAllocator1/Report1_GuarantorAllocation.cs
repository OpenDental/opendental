using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using OpenDentBusiness;

namespace OpenDental.Reporting.Allocators.MyAllocator1.SupportingCode
{
	public class Report1_GuarantorAllocation
	{
		
		private bool _ReportIsFilled = false;
		// Report Variables (For Test Form)
#region Report Variables/Properties that outside can get
		private string _Title = "No Title Set";
		private DataTable _MainReportTable = null;
		/// <summary>
		/// The description used for the _SummaryTable
		/// </summary>
		private List<string> _SummaryTableHeader = null;
		private DataTable _SummaryTable = null;
		public string TITLE { get { return this._Title; } }
		public DataTable MAIN_REPORT { get { return this._MainReportTable; } }
		public DataTable SUMMARY { get { return this._SummaryTable; } }
		public bool IS_FILLED { get { return this._ReportIsFilled; } }
#endregion
		#region Public Methods
		public void FILL(int Guarantor)
		{
			Set_Queries_CurReport(Guarantor);
			_ReportIsFilled = true;
		}
		public void ShowReportPreview(int Guarantor)
		{
			if (!_ReportIsFilled)
				Set_Queries_CurReport(Guarantor);
			System.Windows.Forms.MessageBox.Show("This Report Preview Not Implemented Yet");
			///// Commented Out Since 5.7 does not have this availabele
			//FormQuery FormQuery2 = new FormQuery();
			//FormQuery2.IsReport = true;
			//FormQuery2.SetupReport(false); //Uses TableQ to set up fields used in Layout. (ie dimensions of array to hold column widths and such)
			//SetCurReportProperties();
			////FormQuery2.TablePadding = 2;
			//FormQuery2.ShowDialog();
			#region Method needs to be added to FormQuery.cs
			/// <summary>
		/// You call the Queries.SubmitCur() first.  This allows you to alter or set
		/// the report.TableQ first to your liking and not be limited to the query.
		/// </summary>
		//public void SetupReport(bool autosizecols)
		//{
		//    //Queries.SubmitCur();
		//    report.ColWidth = new int[report.TableQ.Columns.Count];
		//    report.ColPos = new int[report.TableQ.Columns.Count + 1];
		//    report.ColPos[0] = 0;
		//    report.ColCaption = new string[report.TableQ.Columns.Count];
		//    report.ColAlign = new HorizontalAlignment[report.TableQ.Columns.Count];
		//    report.ColTotal = new double[report.TableQ.Columns.Count];
		//    grid2.TableStyles.Clear();
		//    grid2.SetDataBinding(report.TableQ, "");//why set the binding Here?
		//    myGridTS = new DataGridTableStyle();
		//    grid2.TableStyles.Add(myGridTS);
		//    //report.TableQ = MakeReadable(report.TableQ);//?
		//    grid2.SetDataBinding(report.TableQ, "");//because MakeReadable trashes the TableQ
		//    if (autosizecols)
		//        AutoSizeColumns(); 
			//}
			#endregion
		}

		#endregion
		private void Set_Queries_CurReport(int Guarantor)
		{
			ReportSimpleGrid report = new ReportSimpleGrid();
			report.Query = DetailReport(Guarantor);
			report.SubmitQuery(); // Fill the report.TableQ
			report.TableQ = (new Report1_GuarantorAllocation()).ConvertTableToAllStrings(report.TableQ);
			AddColBalance(report);// Calculates and Adds the Balance Column to the Query. // Also Adds Providers Abbreviations
			report.Title = "Allocation to Guarantor ";

			DataTable dtGuarantorName = Db.GetTableOld("SELECT "+DbHelper.Concat("LName","' '","FName")+"FROM Patient WHERE PatNum = " + Guarantor);
			string GuarantorName = dtGuarantorName.Rows[0][0].ToString();
			report.SubTitle.Add("Guarantor Account: " + GuarantorName + " " + DateTime.Now.ToShortDateString());
			this._MainReportTable = report.TableQ;
			this._Title = report.Title + "\n" + report.SubTitle[0];
			BuildSummaryTable(Guarantor, DateTime.MinValue, DateTime.MaxValue);
			report.Summary = SummaryTable_as_Strings(); // No Summary Table until built
			
				//DataTable dtSummary = Db.GetTable(ProviderRevenueByGuarantor(Guarantor));
			
		}
		/// <summary>
		/// This expects that the last column will be Amount.  If not It will do nothing.
		/// RunDetialReport(Guarantor) query first.  // Adds Provider's abbreviations to Table as well
		/// 
		/// Adds a column to report.TableQ
		/// </summary>
		private static void AddColBalance(ReportSimpleGrid report) //to report.TableQ
		{
			if (report.TableQ != null && report.TableQ.Columns.Count == 10)
				if (report.TableQ.Columns[8].ColumnName == "Amount")
				{

					decimal RunningBalance = 0;
					// Find Provider Abbreviations
					System.Collections.Hashtable htProvs = new System.Collections.Hashtable();
					try
					{
						DataTable dt = Db.GetTableOld("SELECT ProvNum, Abbr FROM Provider");

						if (dt.Rows.Count != 0)
							foreach (DataRow dr in dt.Rows)
								if (!htProvs.ContainsKey(dr[0].ToString()))
									htProvs[dr[0].ToString()] = dr[1].ToString();
						if (!htProvs.ContainsKey("0"))
							htProvs["0"] = "UnEarned";

					}
					catch
					{
					}

					for (int i = 0; i < report.TableQ.Rows.Count; i++)
					{
						try
						{

							decimal amount = 0;
							try
							{
								amount = decimal.Parse(report.TableQ.Rows[i][8].ToString());
							}
							catch { }

							RunningBalance += amount;

							if (amount > 0)
							{
								report.TableQ.Rows[i][5] = amount.ToString("F2");
								report.TableQ.Rows[i][6] = "";
							}
							else
							{
								report.TableQ.Rows[i][5] = "";
								report.TableQ.Rows[i][6] = amount.ToString("F2");
							}
							report.TableQ.Rows[i][7] = RunningBalance.ToString("F2");
							// Fix Provider Column From # to Abreviation,  Need to do it here
							// because Prov# = 0 is undefined
							if (htProvs.Count != 0)
								if (htProvs.ContainsKey(report.TableQ.Rows[i][9].ToString()))
									report.TableQ.Rows[i][4] = htProvs[report.TableQ.Rows[i][9].ToString()].ToString();
							// Change Column 0 Date Display if Date on previous row has been displayed
							if (i > 0)
								if (report.TableQ.Rows[i][0].ToString() == report.TableQ.Rows[i - 1][0].ToString())
									report.TableQ.Rows[i][0] = ""; // makes Dates more readable if not redundant.
						}
						catch
						{
							report.TableQ.Rows[i][7] = "ERROR";
						}
					}


					report.TableQ.Columns.RemoveAt(8);// Remove Amount
					report.TableQ.Columns.RemoveAt(8); // Remove ProvNums
					DataRow dr2 = report.TableQ.NewRow();
					report.TableQ.Rows.Add(dr2); // add blank row
					dr2 = report.TableQ.NewRow();
					dr2[6] = "Total";
					dr2[7] = RunningBalance.ToString("F");
					report.TableQ.Rows.Add(dr2);
				}

		}
		
		/*
		/// <summary>
		/// Need to have the CurReport.ColWidth,ColCaption, and ColPos arrays initialized.
		/// </summary>
		private static void SetCurReportProperties(ReportSimpleGrid report)//jsparks This doesn't even seem to be used
		{
			if (report.ColCaption != null && report.TableQ.Columns.Count == report.ColCaption.Length)
				for (int i = 0; i < report.ColCaption.Length; i++ )
				{
					report.ColCaption[i] = report.TableQ.Columns[i].ColumnName;
					
				}
			// This Report should have 7 columns
			report.ColWidth[0] = 70; report.ColAlign[0] = System.Windows.Forms.HorizontalAlignment.Center;
			report.ColWidth[1] = 206; report.ColAlign[1] = System.Windows.Forms.HorizontalAlignment.Left;
			report.ColWidth[2] = 90; report.ColAlign[2] = System.Windows.Forms.HorizontalAlignment.Center;
			report.ColWidth[3] = 85; report.ColAlign[3] = System.Windows.Forms.HorizontalAlignment.Center;
			report.ColWidth[4] = 51; report.ColAlign[4] = System.Windows.Forms.HorizontalAlignment.Center;
			report.ColWidth[5] = 48; report.ColAlign[5] = System.Windows.Forms.HorizontalAlignment.Right;
			report.ColWidth[6] = 45; report.ColAlign[6] = System.Windows.Forms.HorizontalAlignment.Right;
			report.ColWidth[7] = 50; report.ColAlign[7] = System.Windows.Forms.HorizontalAlignment.Right;
			//report.ColWidth[] = ;
			for (int i = 1; i < report.ColPos.Length; i++)
				report.ColPos[i] = report.ColPos[i - 1] + report.ColWidth[i - 1];

		}*/


		/// <summary>
		/// Table As Such:
		/// __________________________________________________
		/// |            |                       | Patient ID  |
		/// |____________|_______________________|_____________|
		/// |            |                       |             |   
		/// | Guarantor  | Painter, Matt         |    1001     |
		/// |____________|_______________________|_____________|
		/// | Accounts:  | 1 Painter, Matt       |    1001     |
		/// |            | 2 Hummel, Robbie      |    1001     |
		/// |            | 3 Kramer, Chris       |    1002     |
		/// |            | 4 Moore, E'twan       |    1045     |
		/// |____________|_______________________|_____________|
		/// 
		/// Returns Null if _Guarantor = 0
		/// 
		/// </summary>
		private static DataTable GetFamilyTable(int Guarantor)
		{
			if (Guarantor == 0)
				return null;
			string cmd = "SELECT PatNum, FName, LName FROM Patient WHERE Guarantor = " + Guarantor + " ORDER BY PatNum ";
			DataTable rawTbl = Db.GetTableOld(cmd);
			DataTable TblReturn = new DataTable();
			DataColumn dc1 = new DataColumn();
			DataColumn dc2 = new DataColumn();
			DataColumn dc3 = new DataColumn();
			TblReturn.Columns.Add(dc1);
			TblReturn.Columns.Add(dc2);
			TblReturn.Columns.Add(dc3);

			// 1st Row
			DataRow newRow = TblReturn.NewRow();
			newRow[0] = "";
			newRow[1] = "";
			newRow[2] = "Patient ID";
			TblReturn.Rows.Add(newRow);

			// 2nd Row
			newRow = TblReturn.NewRow();
			newRow[0] = "Guarantor";
			newRow[1] = "Not Found";
			newRow[2] = "";
			TblReturn.Rows.Add(newRow);

			if (rawTbl != null && rawTbl.Rows.Count != 0)
			{
				for (int i = 0; i < rawTbl.Rows.Count; i++)
				{
					newRow = TblReturn.NewRow();
					if (i == 0)
						newRow[0] = "Accounts:";
					newRow[1] = i + " " + rawTbl.Rows[i][2].ToString() + ", " + rawTbl.Rows[i][1].ToString();
					newRow[2] = rawTbl.Rows[i][0].ToString();
					TblReturn.Rows.Add(newRow);

					if (rawTbl.Rows[i][0].ToString() == Guarantor.ToString()) //Set info in row that says Guarantor
					{
						TblReturn.Rows[1][1] = rawTbl.Rows[i][2].ToString() + ", " + rawTbl.Rows[i][1].ToString(); //Name
						TblReturn.Rows[1][2] = newRow[2].ToString(); //Number
					}


				}
			}
			return TblReturn;
		}

		#region Queries
		/// <summary>
		/// A string to make a Query that will Output the following Table
		/// ________________________________________________________________________________________________________________________________________________________
		/// |   Date    |   Description                    | Patient ID  |    Table Source |    ProvNum     | Credits | Debits | Balance | Amount  |    ProvNum     |
		/// |___________|__________________________________|_____________|_________________|________________|_________|________|_________|_________|________________|
		/// | 01/29/08  |   Prophy                         | Hummell, R  |    0			   | empty to hold  |    empty 3 columns to be   | 50.00   |    10          |
		/// |___________|__________________________________|_____________|_________________|_place for Abbr_|____filled in later by code_|_________|________________|
		/// | 01/29/08  |   Exam                           | Hummell, R  |    1			   |    ""          |         |        |         | 45.00   |    3           |
		/// |___________|__________________________________|_____________|_________________|________________|_____using the Amount Value_|_________|________________|
		/// | 01/29/08  |   4 BWX                          | Hummell, R  |    0			   |    ""          |         |        |         | 25.00   |    10          |
		/// |___________|__________________________________|_____________|_________________|________________|_________|________|_________|_________|________________|
		/// | 01/29/08  |                                  | Hummell, R  |    1			   |    ""          |         |        |         | -50.00  |    10          |
		/// |___________|__________________________________|_____________|_________________|________________|_________|________|_________|_________|________________|
		/// | 01/29/08  |                                  | Hummell, R  |    1			   |    ""          |         |        |         | -44.45  |    3           |
		/// |___________|__________________________________|_____________|_________________|________________|_________|________|_________|_________|________________|
		/// | 01/29/08  |                                  | Hummell, R  |    1			   |    ""          |         |        |         | -4.45   |    0           |
		/// |___________|__________________________________|_____________|_________________|___(unearned)___|_________|________|_________|_________|________________|
		/// 
		/// Note that we use ProvNum = 0 for unearned income.  So there is no Abbr in 
		/// the database for this value.  We have to put it in progrmatically later.
		/// 
		/// Ammount keeps comming in a a decimal or double value.  This makes it difficult to establish a 
		/// numeric value so I added the 3 columns here with only an empty string value
		/// </summary>
		private static string DetailReport(int Guarantor)
		{
			string outstring = "";
			outstring = "#Select Procedures First "
		+ "\nSELECT"
				+ "\n  pl.ProcDate as 'Date', "
				+ "\n  pc.descript as 'Description', "
				+ "\n  "+DbHelper.Concat("pt.LName","', '","pt.FName")+" as 'Patient', "
				+ "\n  'Procedure' as 'TableSource', # meaning 0=Procedurelog "
				+ "\n  ' ' as 'Provider', "
				+ "\n  ' ' as 'Charges', "
				+ "\n  ' ' as 'Credits', "
				+ "\n  ' ' as 'Balance', "
				+ "\n  pl.ProcFee as 'Amount',"
				+ "\n  pl.ProvNum as 'ProviderNum' "
		+ "\nFROM "
				+ "\n  procedurelog as pl, "
				+ "\n  Patient as pt, "
				+ "\n  procedurecode as pc "
		+ "\nWHERE "
				+ "\n    pt.Patnum = pl.PatNum "
				+ "\n    AND pc.CodeNum = pl.CodeNum "
				+ "\n    AND pt.Guarantor = " + Guarantor.ToString()
				+ "\n    AND pl.ProcStatus = " +  ((int)OpenDentBusiness.ProcStat.C).ToString()
			+"\n# Regular Payments (MC/Visa/Check/Cash) Insurance Payments from the allocator_provider table\n UNION ALL "
			+ "\nSELECT "
					+ "\n Payment.PayDate, "
					+ "\n 'Cash/MC/Visa' as 'Description', "
					+ "\n "+DbHelper.Concat("pt.LName","', '","pt.FName")+" as 'Patient', "
					+ "\n 'Reg Payment'   as 'Type', "					
					+ "\n ' ' as 'Provider', "
					+ "\n  ' ' as 'Charges', "
					+ "\n  ' ' as 'Credits', "
					+ "\n  ' ' as 'Balance', "
					+ "\n ap.amount as 'Amount', "
					+ "\n ap.ProvNum as 'ProviderNum' "
			+ "\n From "
					+ "\n  allocation_provider as ap, "
					+ "\n  patient as pt, "
					+ "\n  payment "
			+ "\n WHERE "
					+ "\n  payment.paynum = ap.PaySourceNum "
					+ "\n AND pt.PatNum = payment.PatNum "
					+ "\n  AND   pt.Guarantor = " + Guarantor.ToString()
				//	+ "\n  AND PayTableSource = " + ((int)MyAllocator1.ODTablesUsed.ODTablesPulled.Payment).ToString()
			+ "\nUNION ALL\n"

			+ "\n  #Insurance Payments "
			+ "\n SELECT "
					+ "\n  # ap.AllocNum, "
 					+ "\n  cp.DateEntry, "
 					+ "\n  'Insurance Payment' as 'Description', "
 					+ "\n  "+DbHelper.Concat("pt.LName","', '","pt.FName")+" as 'Patient', "
 					+ "\n  'Ins Payment' as 'Type', "
 					+ "\n  ' ' as 'Provider', "
  					+ "\n  ' ' as 'Charges', "
  					+ "\n  ' ' as 'Credits', "
  					+ "\n  ' ' as 'Balance', "
 					+ "\n  ap.amount as 'Amount', "
 					+ "\n  ap.ProvNum as 'ProviderNum' "
			+ "\n  From "
  					+ "\n  allocation_provider as ap, "
  					+ "\n  patient as pt, "
  					+ "\n  claimproc as cp "
			+ "\n  WHERE "
  					+ "\n  ap.PaySourceNum = cp.ClaimProcNum "
  					+ "\n  AND pt.PatNum = ap.Guarantor "
  					+ "\n  AND ap.PayTableSource =  " + ((int)ODTablesUsed.ODTablesPulled.ClaimProcPayment).ToString()
					+ "\n  AND   pt.Guarantor =  " + Guarantor
			+ "\nUNION ALL\n"

			+ "#Adjustments "
			+ "\nSELECT "
					+ "\n   a.AdjDate as 'Date', "
					+ "\n   'Account Adjustment against Provider' as 'Description', "
					+ "\n   "+DbHelper.Concat("pt.LName","', '","pt.FName")+" as 'Patient',  "
					+ "\n   'Adjustment' as 'Type', "
					+ "\n   ' ' as 'Provider', "
					+ "\n  ' ' as 'Charges', "
					+ "\n  ' ' as 'Credits', "
					+ "\n  ' ' as 'Balance', "
					+ "\n   a.AdjAmt as 'Amount', "
					+ "\n   a.ProvNum as 'Provider' "
				+ "\n  FROM "
					+ "\n   adjustment as a, "
					+ "\n   patient as pt "
				+ "\n  WHERE "
					+ "\n   pt.PatNum = a.PatNum "
					+ "\n   AND pt.Guarantor = " + Guarantor
			+ "\nUNION ALL\n"
			+ "#INSURANCE WRITE OFF "
			+ "\nSELECT "
					+ "\n   cp.DateEntry as 'Date', "
					+ "\n   CONCAT('Ins Write Off against Code ', cp.CodeSent) as 'Description', "
					+ "\n   "+DbHelper.Concat("pt.LName","', '","pt.FName")+" as 'Patient', "
					+ "\n   'Ins Write Off' as 'Type', "
					+ "\n   ' ' as 'Provider', "
					+ "\n  ' ' as 'Charges', "
					+ "\n  ' ' as 'Credits', "
					+ "\n  ' ' as 'Balance', "
					+ "\n   -cp.WriteOff as 'Amount', "
					+ "\n   cp.ProvNum as 'Provider' "
			+ "\nFROM "
					+ "\n   claimproc as cp, "
					+ "\n   patient as pt "
			+ "\nWHERE "
					+ "\n   pt.PatNum = cp.PatNum "
					+ "\n   AND cp.WriteOff != 0 "
					+ "\n   AND cp.Status = " + ((int)OpenDentBusiness.ClaimProcStatus.Received).ToString()
					+ "\n   AND pt.Guarantor = " + Guarantor
			+ "\nORDER By Date";
			return outstring;

		}
		///<summary>
		/// A string to make a Query that will Output the following Table
		/// ProvNum Amount Type 
		/// |   3    |    6334    |    Charges        |    
		/// |   5    |    176     |    Charges        |    
		/// |   10   |    700     |    Charges        |    
		/// |   14   |    279     |    Charges        |    
		/// |   17   |    59      |    Charges        |    
		/// |   0    |    -100    |    Revenue        |    
		/// |   3    |    -2522   |    Revenue        |    
		/// |   5    |    -40     |    Revenue        |    
		/// |   10   |    -749    |    Revenue        |    
		/// |   14   |    -279    |    Revenue        |    
		/// |   17   |    -59     |    Revenue        |    
		/// |   3    |    -3844   |    Adjustments    |   //These should include writeoffs for Ins 
		/// |   5    |    -136    |    Adjustments    |    
		/// |   10   |    -19     |    Adjustments    | 
		///  use -ve number if you want sumary for all guarantors
		///</summary>
		private static string ProviderRevenueByGuarantor(int Guarantor, DateTime dtFrom, DateTime dtTo)
		{
			bool AllDates = false;
			/// Debugging
			//dtFrom = new DateTime(2007, 12, 31);
			//dtTo = new DateTime(2007, 12, 31);
			//AllDates = false;
			/// end Debugging
			
			if (dtFrom  == DateTime.MinValue && dtTo == DateTime.MaxValue)
				AllDates = true;


			string outstring = "";
			string strFromDate = String.Format("'{0}-{1}-{2}'",dtFrom.Year,dtFrom.Month,dtFrom.Day);
			string strToDate = String.Format("'{0}-{1}-{2}'",dtTo.Year,dtTo.Month,dtTo.Day);
			outstring =
			"#Charges found from procedurelog" +
			"\nSELECT "
				+ "\npl.ProvNum, "
				+ "\nSUM(pl.ProcFee) as Amount, "
				+ "\n'Charges' as Type "
			+ "\nFROM "
				+ "\nprocedurelog as pl, "
				+ "\npatient as pt "
			+ "\nWHERE "
				+ "\npl.PatNum = pt.PatNum ";
			if (Guarantor >= 0)
				outstring += "\nAND pt.Guarantor = " + Guarantor;

			outstring += "\nAND pl.ProcStatus = " + ((int)OpenDentBusiness.ProcStat.C).ToString();

			if (!AllDates)
				outstring += "\nAND pl.DateEntryC >= " + strFromDate
							 + "\nAND pl.DateEntryC <= " + strToDate;
			outstring += "\nGROUP BY pl.ProvNum ";


			//////////////////---Revenue ---///////////////////////
			outstring += "\nUNION ALL ";
			
			outstring +=
			 "\n#Revenue from allocation_provider of type ClaimProcPayment or Payment\n"
			+" SELECT ProvNum, SUM(Amount) as Amount, 'Revenue' as Type FROM ( "
			////// ClaimProcs Payments Unioned with Payments /////
			+ "\nSELECT "
				+ "\n ap.ProvNum, "
				+ "\n SUM(ap.Amount) as Amount, "
				+ "\n'Revenue' as Type "
			+ "\nFROM "
				+ "\n allocation_provider ap, claimproc as cp "
			+ "\nWHERE ";
			if (Guarantor >= 0)
				outstring += "\n ap.Guarantor = " + Guarantor + " \nAND ";

			outstring += " ap.PayTableSource =  " + ((int)ODTablesUsed.ODTablesPulled.ClaimProcPayment).ToString()
				+ "\n  AND ap.PaySourceNum = cp.ClaimProcNum ";

			if (!AllDates)
				outstring += "\nAND cp.DateEntry >= " + strFromDate
							 + "\nAND cp.DateEntry <= " + strToDate;
			outstring += "\n GROUP BY ProvNum ";


			outstring += "\nUNION ALL"; // Payment table next
			outstring += "\nSELECT "
				+ "\n ap.ProvNum, "
				+ "\n SUM(ap.Amount) as Amount, "
				+ "\n'Revenue' as Type "
			+ "\nFROM "
				+ "\n allocation_provider ap,  Payment as py "
			+ "\nWHERE ";
			if (Guarantor >= 0)
				outstring += "\n ap.Guarantor = " + Guarantor + " \nAND ";

			outstring += " ap.PayTableSource =  " + ((int)ODTablesUsed.ODTablesPulled.Payment).ToString()
				+ "\n  AND ap.PaySourceNum = py.PayNum ";

			if (!AllDates)
				outstring += "\nAND py.DateEntry >= " + strFromDate
							 + "\nAND py.DateEntry <= " + strToDate;
			outstring += "\n GROUP BY ProvNum ";

			outstring += " \n\n) as tbl1 \n GROUP BY ProvNum ";

			//////////////////---Adjustments ---///////////////////////
			outstring  += "\nUNION ALL "
			+ "\nSELECT tbl1.ProvNum, tbl1.Amount, tbl1.Type "
			+ "\nFROM "
			+ "\n( "
			+ "\nSELECT "
				+ "\naj.ProvNum, "
				+ "\nSUM(aj.AdjAmt) as Amount, "
				+ "\n'Adjustments' as 'Type' "
			+ "\nFROM "
				+ "\nAdjustment as aj, "
				+ "\npatient as pt "
			+ "\nWHERE "
				+ "\naj.PatNum = pt.PatNum ";
			outstring += "\nAND aj.AdjDate >= " + strFromDate
							 + "\nAND aj.AdjDate <= " + strToDate;
			if (Guarantor >= 0)
				outstring += "\nAND pt.Guarantor = " + Guarantor;

			outstring += "\nGROUP BY aj.ProvNum ";

			outstring += "\nUNION ALL ";
		outstring += "\nSELECT "
				+ "\n ap.ProvNum, "
				+ "\n SUM(ap.Amount) as Amount, "
				+ "\n'WriteOff' as Type "
			+ "\nFROM  "
				+ "\nallocation_provider ap, claimproc as cp "
			+ "\nWHERE ";
			if (Guarantor >= 0)
				outstring += "\n ap.Guarantor = " + Guarantor + "\nAND ";

			outstring += "PayTableSource =  " + ((int)ODTablesUsed.ODTablesPulled.ClaimProcWriteOff).ToString()
				+ "\n  AND ap.PaySourceNum = cp.ClaimProcNum ";

			if (!AllDates)
				outstring += "\nAND cp.DateCP >= " + strFromDate //DateEntry or DateCP??? That is the question
							 + "\nAND cp.DateCP <= " + strToDate;
			outstring += "\nGROUP BY ProvNum "
			+ "\n) as tbl1 ";
			return outstring;
		}

		
		private static string ProviderRevenueByGuarantor(int Guarantor)
		{
			return ProviderRevenueByGuarantor(Guarantor,DateTime.MinValue, DateTime.MinValue);
		}
			

		

		#endregion
		


		/// <summary>
		/// Builds a DataTable that details the summary that this report generates then
		/// places it int the _SummaryTable.  Also Fills the  _SummaryTableHeader which
		/// is used with the FormQuery Printing feature instead of the _SummaryTable
		/// (becuase the FormQuery does not provide support for a second table)
		/// </summary>
		/// <param name="Guarantor"></param>
		private void BuildSummaryTable(int Guarantor, DateTime dtFrom, DateTime dtTo)
		{
			_SummaryTable = GenerateSummaryTable(Guarantor,dtFrom, dtTo); // Set this first becuase SummaryTable_as_Strings() will check for _SummaryTable != null
			_SummaryTableHeader = SummaryTable_as_Strings();
		}
		/// <summary>
		/// Generates a Summary Table Outlining the Provider Charges and Revenues
		/// Illustrating Unearned Income
		/// use MinDate and MaxDate to flag all dates
		/// </summary>
		public static DataTable GenerateSummaryTable(int Guarantor,DateTime dtFrom, DateTime dtTo)
		{
			DataTable rValReturnTable = null;
			DataTable dtProviderBalance = Db.GetTableOld(ProviderRevenueByGuarantor(Guarantor,dtFrom,dtTo));
			DataTable dtProvAbbr = Db.GetTableOld("SELECT ProvNum, Abbr FROM Provider");
			if (dtProviderBalance != null && dtProviderBalance.Rows.Count != 0)
			{
				// Get Provider Abbreviations
				System.Collections.Hashtable htAllProvs = new System.Collections.Hashtable();
				for (int i = 0; i < dtProvAbbr.Rows.Count; i++)
					if (!htAllProvs.ContainsKey(dtProvAbbr.Rows[i][0].ToString()))
						htAllProvs[dtProvAbbr.Rows[i][0].ToString()] = dtProvAbbr.Rows[i][1].ToString();
				if (!htAllProvs.ContainsKey("0"))
					htAllProvs["0"] = "Unearned";
				// First establish a list of providers
				System.Collections.ArrayList alProvs = new System.Collections.ArrayList();
				for (int i = 0; i < dtProviderBalance.Rows.Count; i++)
					if (!alProvs.Contains(dtProviderBalance.Rows[i][0].ToString()))
						alProvs.Add(dtProviderBalance.Rows[i][0].ToString());

				// Fill and Create DataTable
				DataTable dtBalancesTable = new DataTable();
				dtBalancesTable.Columns.Add(new DataColumn("Provider", typeof(string)));
				dtBalancesTable.Columns.Add(new DataColumn("Charges", typeof(string)));
				dtBalancesTable.Columns.Add(new DataColumn("Adjustments", typeof(string)));
				dtBalancesTable.Columns.Add(new DataColumn("Revenue", typeof(string)));
				dtBalancesTable.Columns.Add(new DataColumn("Balance", typeof(string)));
				DataRow drWorkingRow = null;
				int ColumnIndex = -1;

				try
				{
					System.Collections.Hashtable htAllProvs_Indexes = new System.Collections.Hashtable();
					for (int i = 0; i < alProvs.Count; i++) // For Each Provider add a Row in the Balances Table
					{
						dtBalancesTable.Rows.Add(dtBalancesTable.NewRow());
						dtBalancesTable.Rows[i][0] = alProvs[i].ToString();
					}

					for (int i = 0; i < dtProviderBalance.Rows.Count; i++)
					{
						drWorkingRow = dtBalancesTable.Rows[alProvs.IndexOf(dtProviderBalance.Rows[i][0].ToString())];
						drWorkingRow[0] = htAllProvs[dtProviderBalance.Rows[i][0].ToString()].ToString(); // the abbreviation

						// Which Column of the Row to add the number to?  Find the column
						// stated in the query's column 2
						string ColumnName_FromQuery = dtProviderBalance.Rows[i][2].ToString();
						ColumnIndex = dtBalancesTable.Columns.IndexOf(ColumnName_FromQuery);
						if (ColumnIndex > 0)
							drWorkingRow[ColumnIndex] = dtProviderBalance.Rows[i][1].ToString(); // note is not cast as number so numeric specifies cannot apply //ToString("F2");
						else
							drWorkingRow[4] = "ERROR!";


						ColumnIndex = -1;
					}
					decimal[] Totals = new decimal[4];
					// Fill in Null and Empty Entries
					for (int j = 0; j < dtBalancesTable.Rows.Count; j++)
						for (int k = 1; k < dtBalancesTable.Columns.Count; k++)
						{
							if (dtBalancesTable.Rows[j][k] == null || dtBalancesTable.Rows[j][k].ToString().Trim() == "")
								dtBalancesTable.Rows[j][k] = "0.00";
							if (k == dtBalancesTable.Columns.Count - 1)
							{
								decimal Balance = decimal.Parse(dtBalancesTable.Rows[j][1].ToString())
								+ decimal.Parse(dtBalancesTable.Rows[j][2].ToString())
								+ decimal.Parse(dtBalancesTable.Rows[j][3].ToString());
								dtBalancesTable.Rows[j][4] = Balance.ToString("F2");
							}
							// Add to the Totals to be placed at the bottom of the Table
							Totals[k - 1] += decimal.Parse(dtBalancesTable.Rows[j][k].ToString());
							// Format the Numbers Better
							dtBalancesTable.Rows[j][k] = decimal.Parse(dtBalancesTable.Rows[j][k].ToString()).ToString("F2");
						}
					DataRow TotalRow = dtBalancesTable.NewRow();
					TotalRow[0] = "Totals-->";
					for (int i = 1; i < dtBalancesTable.Columns.Count; i++)
						TotalRow[i] = Totals[i - 1].ToString("F2");
					dtBalancesTable.Rows.Add(TotalRow);
				}
				catch
				{
				}
				rValReturnTable =  dtBalancesTable; // Set this first becuase SummaryTable_as_Strings() will check for _SummaryTable != null
			//	_SummaryTableHeader = SummaryTable_as_Strings();
			}
			return rValReturnTable;
		}


		/// <summary>
		/// Just looks at _SummaryTable and pulls off the values in a nice orderly manner to build a string[]
		/// </summary>
		private List<string> SummaryTable_as_Strings()
		{
			List<string> outstrings = new List<string>();
			//if (_SummaryTable != null)
			//	outstrings = new string[_SummaryTable.Rows.Count + 3];// 3 headers 
			//else
			//	outstrings = new string[4];

			DateTime nt = DateTime.Now; // nt = now time
			int nt_leftSpaces = 40 - nt.ToShortDateString().Length / 2;
			

			outstrings.Add( String.Format("{0,29}", "Provider Revenue Report"));
			outstrings.Add( String.Format("{0," + nt_leftSpaces + "}{1," + (79 - nt_leftSpaces) + "}", nt.ToShortDateString(), " "));
			string format = "{0,-16}  {1,16}{2,16}{3,16}{4,14}";// I added 2 spaces just to give the illusion of the numbers being centered.
			outstrings.Add( String.Format(format, "Provider", "Charges", "Adjustments", "Revenue", "Balance"));
			if (_SummaryTable != null)
				for (int i = 0; i < this._SummaryTable.Rows.Count; i++)
				{
					outstrings.Add( String.Format(format, _SummaryTable.Rows[i][0],
						_SummaryTable.Rows[i][1],
						_SummaryTable.Rows[i][2],
						_SummaryTable.Rows[i][3],
						_SummaryTable.Rows[i][4]));
				}
			else
				outstrings.Add("ERROR - No Summary Table Found");
			return outstrings;
		}

		/// <summary>
		/// Converts all values in DataTable to string values
		/// DateTime Values will be converted to ShortDateStrings mm/dd/yyyy
		/// </summary>
		private DataTable ConvertTableToAllStrings(DataTable source)
		{// Realized this will goof up Date Representations
			DataTable outTable = null;
			if (source != null)
				if (source.Columns.Count != 0)
				{
					outTable = new DataTable();
					for (int i = 0; i < source.Columns.Count; i++)
					{
						DataColumn dc = new DataColumn(source.Columns[i].ColumnName, typeof(string));
						outTable.Columns.Add(dc);
					}
					if (source.Rows.Count != 0)
					{
						for (int j = 0; j < source.Rows.Count; j++)
						{
							DataRow dr = outTable.NewRow();
							for (int k = 0; k < outTable.Columns.Count; k++)
							{
								if (source.Rows[j][k].GetType() == typeof(DateTime))
									dr[k] = ((DateTime)source.Rows[j][k]).ToShortDateString();
								else
									dr[k] = source.Rows[j][k].ToString();
							}
							outTable.Rows.Add(dr);
						}
					}
				}


			return outTable;
		}
	}
}
