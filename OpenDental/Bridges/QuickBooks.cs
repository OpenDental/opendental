using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interop.QBFC10;
using OpenDentBusiness;

namespace OpenDental.Bridges {
	///<summary>Contains all logic for QuickBook connections and requests to the QB company file.</summary>
	public class QuickBooks {
		private static QBSessionManager SessionManager;
		private static IMsgSetRequest RequestMsgSet;
		private static IMsgSetResponse ResponseMsgSet;
		private static bool SessionBegun;
		private static bool ConnectionOpen;

		///<summary></summary>
		public QuickBooks() {

		}
		
		///<summary>Creates a new QB connection and begins the session.  Session will be left open until CloseConnection is called.  Major and minor version refer to the implementation version of the paticular QB request you are trying to run.  The connection will fail if the version you pass in does not support the type of request you are trying to run.</summary>
		private static void OpenConnection(short majorVer,short minorVer,string companyPath) {
			SessionManager=new QBSessionManager();
			//Create the message set request object to hold our request.
			RequestMsgSet=SessionManager.CreateMsgSetRequest("US",majorVer,minorVer);
			RequestMsgSet.Attributes.OnError=ENRqOnError.roeContinue;
			//Connect to QuickBooks and begin a session
			SessionManager.OpenConnection("","Open Dental");
			ConnectionOpen=true;
			SessionManager.BeginSession(companyPath,ENOpenMode.omDontCare);
			SessionBegun=true;
		}

		///<summary>Runs the request that has been built.  QB connection must be open before calling this method.</summary>
		private static void DoRequests() {
			if(!ConnectionOpen) {
				return;
			}
			ResponseMsgSet=SessionManager.DoRequests(RequestMsgSet);
		}

		///<summary>Ends the session and then closes the connection.</summary>
		private static void CloseConnection() {
			if(SessionBegun) {
				SessionManager.EndSession();
				SessionBegun=false;
			}
			if(ConnectionOpen) {
				SessionManager.CloseConnection();
				ConnectionOpen=false;
			}
		}
		
		///<summary>Simplest connection test to QB.  Users have to connect to their QB company file with OD and QB running at the same time the first time they connect.  This is just a simple tool to let them get this connection out of the way.  QB will prompt the user to set permissions / access rights for OD and then from there on QB does not need to be open in the background.</summary>
		public static string TestConnection(string companyPath) {
			try {
				OpenConnection(1,0,companyPath);
				//Send the empty request and get the response from QuickBooks.
				ResponseMsgSet=SessionManager.DoRequests(RequestMsgSet);
				CloseConnection();
				return "Connection to QuickBooks was successful.";
			}
			catch(Exception e) {
				if(SessionBegun) {
					SessionManager.EndSession();
				}
				if(ConnectionOpen) {
					SessionManager.CloseConnection();
				}
				return "Error: "+e.Message;
			}
		}

		///<summary>Returns list of all active accounts.</summary>
		public static List<string> GetListOfAccounts() {
			List<string> accountList=new List<string>();
			try {
				OpenConnection(8,0,PrefC.GetString(PrefName.QuickBooksCompanyFile));
				QueryListOfAccounts();
				DoRequests();
				CloseConnection();
			}
			catch(Exception e) {
				if(SessionBegun) {
					SessionManager.EndSession();
				}
				if(ConnectionOpen) {
					SessionManager.CloseConnection();
				}
				throw e;
			}
			if(ResponseMsgSet==null) {
				return accountList;
			}
			IResponseList responseList=ResponseMsgSet.ResponseList;
			if(responseList==null) {
				return accountList;
			}
			//Loop through the list to pick out the AccountQueryRs section.
			for(int i=0;i<responseList.Count;i++) {
				IResponse response = responseList.GetAt(i);
				//Check the status code of the response, 0=ok, >0 is warning.
				if(response.StatusCode >= 0) {
					//The request-specific response is in the details, make sure we have some.
					if(response.Detail!=null) {
						//Make sure the response is the type we're expecting.
						ENResponseType responseType=(ENResponseType)response.Type.GetValue();
						if(responseType==ENResponseType.rtAccountQueryRs) {
							//Upcast to more specific type here, this is safe because we checked with response.Type check above.
							IAccountRetList AccountRetList=(IAccountRetList)response.Detail;
							for(int j=0;j<AccountRetList.Count;j++) {
								IAccountRet AccountRet=AccountRetList.GetAt(j);
								if(AccountRet.FullName!=null) {
									accountList.Add(AccountRet.FullName.GetValue());
								}
							}
						}
					}
				}
			}
			return accountList;
		}

		///<summary>Adds an account query to the request message.  A QB connection must be open before calling this method. Requires connection with version 8.0</summary>
		private static void QueryListOfAccounts() {
			if(!ConnectionOpen) {
				return;
			}
			//Build the account query add append it to the request message.
			IAccountQuery AccountQueryRq=RequestMsgSet.AppendAccountQueryRq();
			//Filters
			AccountQueryRq.ORAccountListQuery.AccountListFilter.ActiveStatus.SetValue(ENActiveStatus.asActiveOnly);
		}

		///<summary>Returns list of all QB "class" names.</summary>
		public static List<string> GetListOfClasses() {
			List<string> listClasses=new List<string>();
			try {
				OpenConnection(8,0,PrefC.GetString(PrefName.QuickBooksCompanyFile));
				QueryListOfClasses();
				DoRequests();
				CloseConnection();
			}
			catch(Exception e) {
				if(SessionBegun) {
					SessionManager.EndSession();
				}
				if(ConnectionOpen) {
					SessionManager.CloseConnection();
				}
				throw e;
			}
			if(ResponseMsgSet==null) {
				return listClasses;
			}
			IResponseList responseList=ResponseMsgSet.ResponseList;
			if(responseList==null) {
				return listClasses;
			}
			//Loop through the list to pick out the ClassQueryRs section.
			for(int i = 0;i<responseList.Count;i++) {
				IResponse response = responseList.GetAt(i);
				//Check the status code of the response, 0=ok, >0 is warning.
				if(response.StatusCode >= 0) {
					//The request-specific response is in the details, make sure we have some.
					if(response.Detail!=null) {
						//Make sure the response is the type we're expecting.
						ENResponseType responseType=(ENResponseType)response.Type.GetValue();
						if(responseType==ENResponseType.rtClassQueryRs) {
							//Upcast to more specific type here, this is safe because we checked with response.Type check above.
							IClassRetList ClassRetList = (IClassRetList)response.Detail;
							for(int j = 0;j<ClassRetList.Count;j++) {
								IClassRet ClassRet=ClassRetList.GetAt(j);
								if(ClassRet.FullName!=null) {
									listClasses.Add(ClassRet.FullName.GetValue());
								}
							}
						}
					}
				}
			}
			return listClasses;
		}

		///<summary>Adds a class query to the request message.  A QB connection must be open before calling this method. Requires connection with version 8.0</summary>
		private static void QueryListOfClasses() {
			if(!ConnectionOpen) {
				return;
			}
			//Build the class query add append it to the request message.
			IClassQuery ClassQueryRq=RequestMsgSet.AppendClassQueryRq();
			//Filters
			ClassQueryRq.ORListQuery.ListFilter.ActiveStatus.SetValue(ENActiveStatus.asActiveOnly);
		}

		///<summary>Creates a deposit within QuickBooks.</summary>
		public static void CreateDeposit(DateTime txnDate,string depositAcct,string incomeAcct,double amount,string memo,string classRef) {
			try {
				OpenConnection(9,0,PrefC.GetString(PrefName.QuickBooksCompanyFile));
				BuildDepositAddRq(txnDate,depositAcct,incomeAcct,amount,memo,classRef);
				DoRequests();
				CloseConnection();
				ValidateDepositAddRs();
			}
			catch(Exception e) {
				if(SessionBegun) {
					SessionManager.EndSession();
				}
				if(ConnectionOpen) {
					SessionManager.CloseConnection();
				}
				throw e;
			}
		}

		///<summary>Creates a deposit within QuickBooks.  A QB connection must be open before calling this method. Requires connection with version 9.0. ClassRef is only used if clinics is turned on.</summary>
		private static void BuildDepositAddRq(DateTime txnDate,string depositAcct,string incomeAcct,double amount,string memo,string classRef) {
			if(!ConnectionOpen) {
				return;
			}
			//Build the deposit add request and append it to the request message.
			IDepositAdd DepositAddRq=RequestMsgSet.AppendDepositAddRq();
			//Set field value for TxnDate
			DepositAddRq.TxnDate.SetValue(txnDate);
			//Set field value for FullName.
			DepositAddRq.DepositToAccountRef.FullName.SetValue(depositAcct);
			//Set field value for Memo
			DepositAddRq.Memo.SetValue(memo);
			//Set deposit info attributes.
			IDepositLineAdd DepositLineAdd1=DepositAddRq.DepositLineAddList.Append();
			//Set field value for income account
			DepositLineAdd1.ORDepositLineAdd.DepositInfo.AccountRef.FullName.SetValue(incomeAcct);
			//Set field value for Amount
			DepositLineAdd1.ORDepositLineAdd.DepositInfo.Amount.SetAsString(amount.ToString("F"));
			//Set field value for ClassRef which is a way they can group deposits together
			if(!string.IsNullOrEmpty(classRef) && PrefC.GetBool(PrefName.QuickBooksClassRefsEnabled)) {
				DepositLineAdd1.ORDepositLineAdd.DepositInfo.ClassRef.FullName.SetValue(classRef);
			}
		}

		///<summary>Checks if the status code for the deposit is "ok".</summary>
		private static void ValidateDepositAddRs() {
			if(ResponseMsgSet == null) {
				return;
			}
			IResponseList responseList = ResponseMsgSet.ResponseList;
			if(responseList == null) {
				return;
			}
			//if we sent only one request, there is only one response, we'll walk the list for this sample
			for(int i=0;i<responseList.Count;i++) {
				IResponse response = responseList.GetAt(i);
				//check the status code of the response, 0=ok, >0 is warning
				if(response.StatusCode > 0) {
					throw new Exception(response.StatusMessage);
				}
			}
		}

		#region Entities (payees)
		//We are not currently using entities
		/*
		///<summary>Adds an entity query to the request message.  A QB connection must be open before calling this method.</summary>
		public static void QueryEntities() {
			if(!connectionOpen) {
				return;
			}
			//Build the entity query add append it to the request message.
			IEntityQuery EntityQueryRq=requestMsgSet.AppendEntityQueryRq();
			//Filters
			EntityQueryRq.ORListQuery.ListFilter.ActiveStatus.SetValue(ENActiveStatus.asActiveOnly);
		}

		///<summary>Returns list of all active Customers, Employees, Vendors and "Other" entities.  QueryListOfAccounts must have been part of your request.</summary>
		public static List<string> GetListOfEntities() {
			List<string> entityList=new List<string>();
			if(responseMsgSet==null) {
				return entityList;
			}
			IResponseList responseList=responseMsgSet.ResponseList;
			if(responseList==null) {
				return entityList;
			}
			//Loop through the list to pick out the EntityQueryRs section.
			for(int i=0;i<responseList.Count;i++) {
				IResponse response=responseList.GetAt(i);
				//Check the status code of the response, 0=ok, >0 is warning.
				if(response.StatusCode >= 0) {
					//The request-specific response is in the details, make sure we have some.
					if(response.Detail!=null) {
						//Make sure the response is the type we're expecting.
						ENResponseType responseType=(ENResponseType)response.Type.GetValue();
						if(responseType==ENResponseType.rtEntityQueryRs) {
							//Upcast to more specific type here, this is safe because we checked with response.Type check above.
							IOREntityRetList EntityRetList=(IOREntityRetList)response.Detail;
							for(int j=0;j<EntityRetList.Count;j++) {
								IOREntityRet EntityRet=EntityRetList.GetAt(j);
								if(EntityRet.CustomerRet!=null && EntityRet.CustomerRet.FullName!=null) {
									entityList.Add(EntityRet.CustomerRet.FullName.GetValue());
								}
								if(EntityRet.EmployeeRet!=null && EntityRet.EmployeeRet.Name!=null) {
									entityList.Add(EntityRet.EmployeeRet.Name.GetValue());
								}
								if(EntityRet.VendorRet!=null && EntityRet.VendorRet.Name!=null) {
									entityList.Add(EntityRet.VendorRet.Name.GetValue());
								}
								if(EntityRet.OtherNameRet!=null && EntityRet.OtherNameRet.Name!=null) {
									entityList.Add(EntityRet.OtherNameRet.Name.GetValue());
								}
							}
						}
					}
				}
			}
			return entityList;
		}
		 */
		#endregion

	}
}


/* The template for most functions that makes a QB connection:
 ****************************************************************
  public static string TestConnection(string companyPath) {
			try {
				OpenConnection(companyPath);
				//Build method query request here:
				//Send the request and get the response from QuickBooks.
				responseMsgSet=sessionManager.DoRequests(requestMsgSet);
				CloseConnection();
				return "";
			}
			catch(Exception e) {
				if(sessionBegun) {
					sessionManager.EndSession();
				}
				if(connectionOpen) {
					sessionManager.CloseConnection();
				}
				return "Error: "+e.Message;
			}
		}
	}
 ****************************************************************
*/