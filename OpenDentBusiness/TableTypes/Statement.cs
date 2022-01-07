using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace OpenDentBusiness{

	///<summary>Represents one statement for one family.  Usually already sent, but could still be waiting to send.</summary>
	[Serializable,CrudTable(HasBatchWriteMethods=true)]
	public class Statement : TableBase {
		/// <summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long StatementNum;
		/// <summary>FK to patient.PatNum. Typically the guarantor.  Can also be the patient for walkout statements.</summary>
		public long PatNum;
		///<summary>FK to patient.PatNum.  Typically zero unless a super family statement is desired.
		///Will be non-zero if the patient is associated with a super family and a super family statement is desired.</summary>
		public long SuperFamily;
		/// <summary>This will always be a valid and reasonable date regardless of whether it's actually been sent yet.</summary>
		public DateTime DateSent;
		/// <summary>Typically 45 days before dateSent</summary>
		public DateTime DateRangeFrom;
		/// <summary>Any date >= year 2200 is considered max val.  We generally try to automate this value to be the same date as the statement rather than the max val.  This is so that when payment plans are displayed, we can add approximately 10 days to effectively show the charge that will soon be due.  Adding the 10 days is not done until display time.</summary>
		public DateTime DateRangeTo;
		/// <summary>Can include line breaks.  This ordinary note will be in the standard font.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Note;
		/// <summary>More important notes may go here.  Font will be bold.  Color and size of text will be customizable in setup.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string NoteBold;
		/// <summary>Enum:StatementMode Mail, InPerson, Email, Electronic.</summary>
		public StatementMode Mode_;
		/// <summary>Set true to hide the credit card section, and the please pay box.</summary>
		public bool HidePayment;
		/// <summary>One patient on statement instead of entire family.</summary>
		public bool SinglePatient;
		/// <summary>If entire family, then this determines whether they are all intermingled into one big grid, or whether they are all listed in separate grids.</summary>
		public bool Intermingled;
		/// <summary>True</summary>
		public bool IsSent;
		/// <summary>FK to document.DocNum when a pdf has been archived.</summary>
		public long DocNum;
		/// <summary>Date/time last altered.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime DateTStamp;
		///<summary>The only effect of this flag is to change the text at the top of a statement from "statement" to "receipt".  It might later do more.</summary>
		public bool IsReceipt;
		///<summary>This flag is for marking a statement as Invoice.  In this case, it must have procedures and/or adjustments attached.</summary>
		public bool IsInvoice;
		///<summary>Only used if IsInvoice=true.  The first printout will not be a copy.  Subsequent printouts will show "copy" on them.</summary>
		public bool IsInvoiceCopy;
		///<summary>Empty string by default.  Only used to override BillingEmailSubject pref when emailing statements.  Only set when statements are created from the Billing Options window.  No UI for editing.</summary>
		public string EmailSubject;
		///<summary>Empty string by default.  Only used to override BillingEmailBodyText pref when emailing statements.  Only set when statements are created from the Billing Options window.  No UI for editing.  Limit in db: 16M char.</summary>
//TODO: This column may need to be changed to the TextIsClobNote attribute to remove more than 50 consecutive new line characters.
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string EmailBody;
		///<summary>True for statements generated in version 16.1 or greater. Older statements did not store InsEst or BalTotal. </summary>
		public bool IsBalValid;
		/// <summary>Insurance Estimate for entire family, taken from garantor at time of statement being sent/saved.</summary>
		public double InsEst;
		/// <summary>Total balance for entire family before insurance estimate.  
		/// Not the same as the sum of the 4 aging balances because this can be negative.</summary>
		public double BalTotal;
		///<summary>Enum:StmtType Statement, Receipt, Invoice, LimitedStatement.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public StmtType StatementType;
		///<summary>A short alphanumeric string used to uniquely identify this statement.</summary>
		public string ShortGUID;
		///<summary>A URL that can be visited to view this statement.</summary>
		public string StatementURL;
		///<summary>A short URL that can be visited to view this statement. Useful to include in text messages.</summary>
		public string StatementShortURL;
		///<summary>Enum:AutoCommStatus Stores what should be done or was done in regards to SMS messaging for this statement.</summary>
		public AutoCommStatus SmsSendStatus;

		///<summary>List of attached adjustment.AdjNums for this statement.  Limit in db: 16M char.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public List<long> _listAdjNums;
		///<summary>List of attached paysplit.PaySplitNums for this statement.  Limit in db: 16M char.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public List<long> _listPaySplitNums;
		///<summary>List of attached procedure.ProcNums for this statement.  Limit in db: 16M char.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public List<long> _listProcNums;
		///<summary>List of attached claim.ClaimNums for this statement to track insurance payments.  Limit in db: 16M char.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public List<long> _listInsPayClaimNums;
		///<summary>List of installment plans for this account. If this is a super family statement, will contain installment plans for the entire
		///super family.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public List<InstallmentPlan> ListInstallmentPlans;

		///<summary>Set list to null to force refresh.</summary>
		[XmlIgnore,JsonIgnore]
		public List<long> ListAdjNums {
			get {
				if(_listAdjNums==null) {
					_listAdjNums=StmtLinks.GetForStatementAndType(StatementNum, StmtLinkTypes.Adj);
				}
				return _listAdjNums;
			}
			set {
				_listAdjNums=value;
			}
		}

		///<summary>Set list to null to force refresh.</summary>
		[XmlIgnore,JsonIgnore]
		public List<long> ListPaySplitNums {
			get {
				if(_listPaySplitNums==null) {
					_listPaySplitNums=StmtLinks.GetForStatementAndType(StatementNum, StmtLinkTypes.PaySplit);
				}
				return _listPaySplitNums;
			}
			set {
				_listPaySplitNums=value;
			}
		}

		///<summary>Set list to null to force refresh.</summary>
		[XmlIgnore,JsonIgnore]
		public List<long> ListProcNums {
			get {
				if(_listProcNums==null) {
					if(IsInvoice) {
						_listProcNums=Procedures.GetForInvoice(StatementNum);
					}
					else {
						_listProcNums=StmtLinks.GetForStatementAndType(StatementNum, StmtLinkTypes.Proc);
					}
				}
				return _listProcNums;
			}
			set {
				_listProcNums=value;
			}
		}

		///<summary>Set list to null to force refresh.</summary>
		[XmlIgnore,JsonIgnore]
		public List<long> ListInsPayClaimNums {
			get {
				if(_listInsPayClaimNums==null) {
						_listInsPayClaimNums=StmtLinks.GetForStatementAndType(StatementNum, StmtLinkTypes.ClaimPay);
				}
				return _listInsPayClaimNums;
			}
			set {
				_listInsPayClaimNums=value;
			}
		}

		public Statement() {
			this.StatementURL="";
			this.StatementShortURL="";
			this.ShortGUID="";
		}
		
		public Statement Copy(){
			Statement retval=(Statement)this.MemberwiseClone();
			retval.ListAdjNums=ListAdjNums.ToList();//deep copy because of value type
			retval.ListPaySplitNums=ListPaySplitNums.ToList();//deep copy because of value type
			retval.ListProcNums=ListProcNums.ToList();//deep copy because of value type
			retval.ListInsPayClaimNums=ListInsPayClaimNums.ToList();//deep copy because of value type
			retval.ListInstallmentPlans=ListInstallmentPlans?.Select(x => x.Copy()).ToList();
			return retval;
		}
	}

	///<summary>The type of this statement.  This will eventually replace IsReceipt and IsInvoice. Stored as EnumAsString.</summary>
	public enum StmtType {
		///<summary>Regular statement.</summary>
		NotSet=0,
		/////<summary>Makes the top of the statement say receipt.</summary>
		//Receipt,
		/////<summary>Marks the statement as Invoice, must have procedures and/or adjustments attached.</summary>
		//Invoice,
		///<summary>Contains information about specific procedures.</summary>
		LimitedStatement
	}
}
