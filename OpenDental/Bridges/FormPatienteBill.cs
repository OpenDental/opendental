/*=================================================================
Created by Practice-Web Inc. (R) 2009. http://www.practice-web.com
Retain this text in redistributions.
==================================================================*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using System.Xml;
using System.IO;
using System.Web;
using System.Net;
using CodeBase;

namespace OpenDental {
	public partial class FormPatienteBill:FormODBase {
		ArrayList PatientList = new ArrayList();
		XmlDocument Doc = new XmlDocument();
		string AuthenticationID = string.Empty;
		string fileName;
		string PatienteBillServerAddress;
		string finalResponse = string.Empty;
		string DentalxChangeContactInfo = " Call Dentalxchange at 800-576-6412.";
		string PWContactInfo = " Call Practice-Web Inc. at 800-845-9379";

		public FormPatienteBill(ArrayList _PatientList) {
			InitializeComponent();
			InitializeLayoutManager();
			PatientList = _PatientList;
			Lan.F(this);
		}

		private void FormPatienteBill_Load(object sender,EventArgs e) {
			// Make sure Practionar has valid credentials
			if(isValidCredentials() == false || isValidFolder() == false) {
				this.Close();
			}
			else {
				if(PrepareEbill() == true) {
					// Transmit File
					Transmit();
				}
			}
		}

		# region Check Credential
		protected bool isValidCredentials() {
			Progress("Validate Clearing House Setup..");
			// Make Sure Clearing House is ClaimConnect
			Clearinghouse clearinghouseHq = Clearinghouses.GetDefaultDental();
			if(clearinghouseHq == null) {
				Error("No clearinghouse is set as default.");
				return false;
			}
			if(clearinghouseHq.CommBridge != EclaimsCommBridge.ClaimConnect) {
				Error("Your Clearinghouse does not offer Patient eBill functionality.");
				return false;
			}

			Progress("Extract Login Credentials..");
			// Read LoginID & Password
			string loginID;
			string passWord;

			// Get Login / Password
			Clearinghouse clearinghouseClin=Clearinghouses.OverrideFields(clearinghouseHq,Clinics.ClinicNum);
			if(clearinghouseClin!=null) {
				loginID = clearinghouseClin.LoginID;
				passWord = clearinghouseClin.Password;
			}
			else {
				loginID = "";
				passWord = "";
			}
			if(loginID == "") {
				Error("Missing Login ID/Password." + DentalxChangeContactInfo);
				Cursor = Cursors.Default;
				return false;
			}

			Progress("Get Clearinghouse Authentication & Authorization..");
			// Make 1st HTTP Call And Get the authenticaton
			PatienteBillServerAddress = PrefC.GetRaw("PatienteBillServerAddress");
			if(PatienteBillServerAddress.Length == 0) {
				Error("Missing Patient eBill Server Information."+PWContactInfo);
				return false;
			}

			// 
			string PatienteBillServer = PatienteBillServerAddress;
			string authRequest = @"Function=Auth&Source=STM&Username="+loginID+"&Password="+passWord+
                    "&UploaderName=Practice-Web&UploaderVersion=3.0";
			HttpUtility.UrlEncode(authRequest,System.Text.Encoding.Default);
			HttpWebRequest request = null;
			HttpWebResponse response = null;
			StreamReader sr = null;
			System.Diagnostics.ConsoleTraceListener trace = new System.Diagnostics.ConsoleTraceListener();
			request = (HttpWebRequest)WebRequest.Create(PatienteBillServer + '?' + authRequest);
			request.AllowAutoRedirect = true;
			request.Method = "GET";

			// Get The Respons
			response = (HttpWebResponse)request.GetResponse();
			sr = new StreamReader(response.GetResponseStream());
			String loginResponse = sr.ReadLine();
			sr.Close();

			string[] parseResponse;
			char[] separator = { '&' };
			parseResponse = loginResponse.Split(separator);
			int responseStatus;
			string[] statusRespose;
			char[] separatorequal = { '=' };
			statusRespose = parseResponse[0].Split(separatorequal);
			responseStatus = PIn.Int(statusRespose[1]);

			Progress("Process Clearinghouse Authorization Response..");
			string errormessage = string.Empty;
			switch(responseStatus) {
				case 0:
					AuthenticationID = parseResponse[3].Remove(0,17);
					break;
				case 1:
					errormessage = "Authentication Failed.\r\n" + parseResponse[4].Remove(0,13);
					break;
				case 2:
					errormessage = "Cannot authorized at this time.\r\n" + parseResponse[4].Remove(0,13);
					break;
				case 3:
					errormessage = "Invalid Request.\r\n" + parseResponse[4].Remove(0,13);
					break;
				case 4:
					errormessage = "Invalid PMS Version.\r\n" + parseResponse[4].Remove(0,13);
					break;
				case 5:
					errormessage = "No Customer Contract.\r\n" + parseResponse[4].Remove(0,13);
					break;
				default:
					errormessage = "Unknown Status(" + responseStatus + ").\r\n" + parseResponse[4].Remove(0,13);
					break;
			}
			if(errormessage.Length > 0) {
				Error(errormessage + DentalxChangeContactInfo);
				return false;
			}
			// No error hence return true
			return true;
		}
		# endregion

		# region Check Folder
		private bool isValidFolder() {
			try {
				Progress("Verify Folder Location..");
				if(Directory.Exists(PrefC.GetRaw("PatienteBillPath")) == false) {
					// Create Directory
					Directory.CreateDirectory(PrefC.GetRaw("PatienteBillPath"));
				}
			}
			catch(Exception ex) {
				Error("Error Creating Folder "+ex.Message.ToString());
			}
			return true;
		}
		#endregion

		# region Prepare EBill
		//This method creates XML document for Patient eBilling
		private bool PrepareEbill() {
			try {
				Progress("Process Dental office information..");
				//*******************************************************************************
				// EIStatementFile, SubmitDate, PrimarySubmitter, Transmitter, & Practice(Empry)
				//*******************************************************************************
				// Create EISStatementFile (upto <Practice> Tag)
				// Add Processing Instruction
				XmlProcessingInstruction ProcessingInstrction = Doc.CreateProcessingInstruction("xml","version = \"1.0\" standalone=\"yes\"");
				Doc.AppendChild(ProcessingInstrction);
				// Prepare EISStatementFile Root Element
				XmlNode EISStatementFile = Doc.CreateNode(XmlNodeType.Element,"EISStatementFile","");
				Doc.AppendChild(EISStatementFile);

				// Prepare VendorID Attribute
				XmlAttribute VendorID = Doc.CreateAttribute("VendorID");
				VendorID.Value = "18";
				EISStatementFile.Attributes.Append(VendorID);

				// Prepare EISStatementFileOutputFormat Attribute
				XmlAttribute EISStatementFileOutputFormat = Doc.CreateAttribute("OutputFormat");
				EISStatementFileOutputFormat.Value = "StmOut_Blue6Col";
				EISStatementFile.Attributes.Append(EISStatementFileOutputFormat);

				// Prepare SubmitDate Element
				XmlNode SubmitDate = Doc.CreateNode(XmlNodeType.Element,"SubmitDate","");
				SubmitDate.InnerText = DateTime.Now.ToString("u");
				EISStatementFile.AppendChild(SubmitDate);

				// Prepare PrimarySubmitter Element
				XmlNode PrimarySubmitter = Doc.CreateNode(XmlNodeType.Element,"PrimarySubmitter","");
				PrimarySubmitter.InnerText = "PWEB";
				EISStatementFile.AppendChild(PrimarySubmitter);

				// Prepare Transmitter Element
				XmlNode Transmitter = Doc.CreateNode(XmlNodeType.Element,"Transmitter","");
				Transmitter.InnerText = "DXC";
				EISStatementFile.AppendChild(Transmitter);

				// Prepare Practice Element
				XmlNode Practice = Doc.CreateNode(XmlNodeType.Element,"Practice","");
				EISStatementFile.AppendChild(Practice);

				// Prepare AccountNumber Attribute
				XmlAttribute AccountNumber = Doc.CreateAttribute("AccountNumber");
				AccountNumber.Value = PrefC.GetRaw("PWClientAccountNumber");
				Practice.Attributes.Append(AccountNumber);

				//**********************************************
				// Invoke PreparePractice to Complete Practice XML (upto rendering Provider)
				//**********************************************
				PreparePractice(Practice);
				// No Exception return true
				return true;
			}
			catch(Exception ex) {
				Error("Exception Thown while preparing Patient Statement "+ex.Message);
				return false;
			}
		}
		#endregion

		# region XML Generated Code
		/// <summary>
		/// This Method will prepare XML for Provider
		/// </summary>
		private void PreparePractice(XmlNode Practice) {

			//****************************************************************
			// Practice Name,Address and Phone
			//****************************************************************
			// Prepare Name Element for Practice Element
			XmlNode PracticeName = Doc.CreateNode(XmlNodeType.Element,"Name","");
			XmlCDataSection CDataPracticeName;
			CDataPracticeName = Doc.CreateCDataSection(PrefC.GetString(PrefName.PracticeTitle));
			PracticeName.AppendChild(CDataPracticeName);
			Practice.AppendChild(PracticeName);

			// Prepare Address1 Element for Practice Element
			XmlNode PracticeAddress1 = Doc.CreateNode(XmlNodeType.Element,"Address1","");
			XmlCDataSection CDataPracticeAddress1;
			CDataPracticeAddress1 = Doc.CreateCDataSection(PrefC.GetString(PrefName.PracticeAddress));
			PracticeAddress1.AppendChild(CDataPracticeAddress1);
			Practice.AppendChild(PracticeAddress1);

			// Prepare Address2 Element for Practice Element
			XmlNode PracticeAddress2 = Doc.CreateNode(XmlNodeType.Element,"Address2","");
			XmlCDataSection CDataPracticeAddress2;
			CDataPracticeAddress2 = Doc.CreateCDataSection(PrefC.GetString(PrefName.PracticeAddress2));
			PracticeAddress2.AppendChild(CDataPracticeAddress2);
			Practice.AppendChild(PracticeAddress2);

			// Prepare City Element for Practice Element
			XmlNode PracticeCity = Doc.CreateNode(XmlNodeType.Element,"City","");
			XmlCDataSection CDataPracticeCity;
			CDataPracticeCity = Doc.CreateCDataSection(PrefC.GetString(PrefName.PracticeCity));
			PracticeCity.AppendChild(CDataPracticeCity);
			Practice.AppendChild(PracticeCity);

			// Prepare State Element for Practice Element
			XmlNode PracticeState = Doc.CreateNode(XmlNodeType.Element,"State","");
			PracticeState.InnerText = PrefC.GetString(PrefName.PracticeST);
			Practice.AppendChild(PracticeState);

			// Prepare Zip Element for Practice Element
			XmlNode PracticeZip = Doc.CreateNode(XmlNodeType.Element,"Zip","");
			PracticeZip.InnerText = PrefC.GetString(PrefName.PracticeZip);
			Practice.AppendChild(PracticeZip);

			// Format Phone -- Start
			string formatPhone = PrefC.GetString(PrefName.PracticePhone);
			if(formatPhone.Length > 0)
				formatPhone = "(" + formatPhone.Substring(0,3) + ")" + formatPhone.Substring(3,3) + "-" + formatPhone.Substring(6);
			// Format Phone -- End

			// Prepare Phone Element for Practice Element
			XmlNode PracticePhone = Doc.CreateNode(XmlNodeType.Element,"Phone","");
			PracticePhone.InnerText = formatPhone;
			Practice.AppendChild(PracticePhone);

			//****************************************************************
			// Rendering Provider
			//****************************************************************

			// Prepare RemitAddress Element for Practice Element
			XmlNode PracticeRemitAddress = Doc.CreateNode(XmlNodeType.Element,"RemitAddress","");
			Practice.AppendChild(PracticeRemitAddress);
			// If Billing Address is blank then Remit address is same as Practionar address
			if(PrefC.GetString(PrefName.PracticeBillingAddress).Length == 0) {
				// Append Name, Address and Phone of Practice
				// Prepare Name, Address, and Phone for Remit Address
				XmlNode RemitAddressName = Doc.CreateNode(XmlNodeType.Element,"Name","");
				XmlCDataSection CDataRemitAddressName;
				CDataRemitAddressName = Doc.CreateCDataSection(PrefC.GetString(PrefName.PracticeTitle));
				RemitAddressName.AppendChild(CDataRemitAddressName);
				PracticeRemitAddress.AppendChild(RemitAddressName);

				// Prepare Address1 Element for RemitAddress Element
				XmlNode RemitAddressAddress1 = Doc.CreateNode(XmlNodeType.Element,"Address1","");
				XmlCDataSection CDataRemitAddressAddress1;
				CDataRemitAddressAddress1 = Doc.CreateCDataSection(PrefC.GetString(PrefName.PracticeAddress));
				RemitAddressAddress1.AppendChild(CDataRemitAddressAddress1);
				PracticeRemitAddress.AppendChild(RemitAddressAddress1);

				// Prepare Address2 Element for RemitAddress Element
				XmlNode RemitAddressAddress2 = Doc.CreateNode(XmlNodeType.Element,"Address2","");
				XmlCDataSection CDataRemitAddressAddress2;
				CDataRemitAddressAddress2 = Doc.CreateCDataSection(PrefC.GetString(PrefName.PracticeAddress2));
				RemitAddressAddress2.AppendChild(CDataRemitAddressAddress2);
				PracticeRemitAddress.AppendChild(RemitAddressAddress2);

				// Prepare City Element for RemitAddress Element
				XmlNode RemitAddressCity = Doc.CreateNode(XmlNodeType.Element,"City","");
				XmlCDataSection CDataRemitAddressCity;
				CDataRemitAddressCity = Doc.CreateCDataSection(PrefC.GetString(PrefName.PracticeCity));
				RemitAddressCity.AppendChild(CDataRemitAddressCity);
				PracticeRemitAddress.AppendChild(RemitAddressCity);

				// Prepare State Element for RemitAddress Element
				XmlNode RemitAddressState = Doc.CreateNode(XmlNodeType.Element,"State","");
				RemitAddressState.InnerText = PrefC.GetString(PrefName.PracticeST);
				PracticeRemitAddress.AppendChild(RemitAddressState);

				// Prepare Zip Element for RemitAddress Element
				XmlNode RemitAddressZip = Doc.CreateNode(XmlNodeType.Element,"Zip","");
				RemitAddressZip.InnerText = PrefC.GetString(PrefName.PracticeZip);
				PracticeRemitAddress.AppendChild(RemitAddressZip);

				// Prepare Phone Element for RemitAddress Element
				XmlNode RemitAddressPhone = Doc.CreateNode(XmlNodeType.Element,"Phone","");
				RemitAddressPhone.InnerText = formatPhone;
				PracticeRemitAddress.AppendChild(RemitAddressPhone);
			}
			else {
				// Prepare Name, Address, and Phone for Remit Address
				XmlNode RemitAddressName = Doc.CreateNode(XmlNodeType.Element,"Name","");
				XmlCDataSection CDataRemitAddressName;
				CDataRemitAddressName = Doc.CreateCDataSection(PrefC.GetString(PrefName.PracticeTitle));
				RemitAddressName.AppendChild(CDataRemitAddressName);
				PracticeRemitAddress.AppendChild(RemitAddressName);

				// Prepare Address1 Element for RemitAddress Element
				XmlNode RemitAddressAddress1 = Doc.CreateNode(XmlNodeType.Element,"Address","");
				XmlCDataSection CDataRemitAddressAddress1;
				CDataRemitAddressAddress1 = Doc.CreateCDataSection(PrefC.GetString(PrefName.PracticeBillingAddress));
				RemitAddressAddress1.AppendChild(CDataRemitAddressAddress1);
				PracticeRemitAddress.AppendChild(RemitAddressAddress1);

				// Prepare Address2 Element for RemitAddress Element
				XmlNode RemitAddressAddress2 = Doc.CreateNode(XmlNodeType.Element,"Address2","");
				XmlCDataSection CDataRemitAddressAddress2;
				CDataRemitAddressAddress2 = Doc.CreateCDataSection(PrefC.GetString(PrefName.PracticeBillingAddress2));
				RemitAddressAddress2.AppendChild(CDataRemitAddressAddress2);
				PracticeRemitAddress.AppendChild(RemitAddressAddress2);

				// Prepare City Element for RemitAddress Element
				XmlNode RemitAddressCity = Doc.CreateNode(XmlNodeType.Element,"City","");
				XmlCDataSection CDataRemitAddressCity;
				CDataRemitAddressCity = Doc.CreateCDataSection(PrefC.GetString(PrefName.PracticeBillingCity));
				RemitAddressCity.AppendChild(CDataRemitAddressCity);
				PracticeRemitAddress.AppendChild(RemitAddressCity);

				// Prepare State Element for RemitAddress Element
				XmlNode RemitAddressState = Doc.CreateNode(XmlNodeType.Element,"State","");
				RemitAddressState.InnerText = PrefC.GetString(PrefName.PracticeBillingST);
				PracticeRemitAddress.AppendChild(RemitAddressState);

				// Prepare Zip Element for RemitAddress Element
				XmlNode RemitAddressZip = Doc.CreateNode(XmlNodeType.Element,"Zip","");
				RemitAddressZip.InnerText = PrefC.GetString(PrefName.PracticeBillingZip);
				PracticeRemitAddress.AppendChild(RemitAddressZip);

				// Prepare Phone Element for RemitAddress Element
				XmlNode RemitAddressPhone = Doc.CreateNode(XmlNodeType.Element,"Phone","");
				RemitAddressPhone.InnerText = formatPhone;
				PracticeRemitAddress.AppendChild(RemitAddressPhone);
			}
			// Get Rendering Provider Information
			PreapreRendringProvider(Practice);
			//************************************************
			// get Patient Information
			//************************************************
			PreparePatient(Practice);
		}

		/// <summary>
		/// This Method will populate Rendering Provider Information
		/// </summary>
		private void PreapreRendringProvider(XmlNode Practice) {
			string RPName;
			string RPLicense;
			if(PrefC.GetString(PrefName.PracticeDefaultProv).Length > 0) {				
				DataTable RenderingTable = Providers.GetDefaultPracticeProvider();
				if(RenderingTable.Rows.Count > 0) {
					RPName = RenderingTable.Rows[0]["FName"].ToString() + " " + RenderingTable.Rows[0]["LName"].ToString() + " " + RenderingTable.Rows[0]["Suffix"].ToString();
					RPLicense = RenderingTable.Rows[0]["StateLicense"].ToString();
				}
				else {
					RPName = PrefC.GetString(PrefName.PracticeTitle).ToString();
					RPLicense = "";
				}
			}
			else {
				RPName = PrefC.GetString(PrefName.PracticeTitle).ToString();
				RPLicense = "";
			}
			// Prepare RenderingProvider Element for Practice Element
			XmlNode PracticeRenderingProvider = Doc.CreateNode(XmlNodeType.Element,"RenderingProvider","");
			Practice.AppendChild(PracticeRenderingProvider);

			// Prepare Name Element for RenderingProvider Element
			XmlNode RenderingProviderName = Doc.CreateNode(XmlNodeType.Element,"Name","");
			RenderingProviderName.InnerText = RPName;
			PracticeRenderingProvider.AppendChild(RenderingProviderName);

			// Prepare LicenseNumber Element for RenderingProvider Element
			XmlNode RenderingProviderLicenseNumber = Doc.CreateNode(XmlNodeType.Element,"LicenseNumber","");
			RenderingProviderLicenseNumber.InnerText = RPLicense;
			PracticeRenderingProvider.AppendChild(RenderingProviderLicenseNumber);

			// Prepare State Element for RenderingProvider Element
			XmlNode RenderingProviderState = Doc.CreateNode(XmlNodeType.Element,"State","");
			RenderingProviderState.InnerText = PrefC.GetString(PrefName.PracticeST);
			PracticeRenderingProvider.AppendChild(RenderingProviderState);
		}

		/// <summary>
		/// This Method will prepare Patient XML
		/// </summary>
		private void PreparePatient(XmlNode Practice) {
			int PatientID;
			string FName,MiddleI,LName,PatientNm,Guarantor,Address;
			string Address2,City,State,Zip,Email,EstBalance;
			string BalTotal,Bal_0_30,Bal_31_60,Bal_61_90,BalOver90;
			int StmtCalcDueDate;
			Progress("Process Patient Information..");
			for(int i = 0;i < PatientList.Count;i++) {
				PatientID = (int)PatientList[i];
				DataTable PatientTable = Patients.GetGuarantorInfo(PatientID);
				if(PatientTable.Rows.Count > 0) {
					FName = PatientTable.Rows[0]["FName"].ToString();
					MiddleI = PatientTable.Rows[0]["MiddleI"].ToString();
					LName = PatientTable.Rows[0]["LName"].ToString();
					if(MiddleI.Length > 0)
						PatientNm = FName + " " + MiddleI + " " + LName;
					else
						PatientNm = FName + " " + LName;
					Guarantor = PatientTable.Rows[0]["Guarantor"].ToString();
					Address = PatientTable.Rows[0]["Address"].ToString();
					Address2 = PatientTable.Rows[0]["Address2"].ToString();
					City = PatientTable.Rows[0]["City"].ToString();
					State = PatientTable.Rows[0]["State"].ToString();
					Zip = PatientTable.Rows[0]["Zip"].ToString();
					Email = PatientTable.Rows[0]["Email"].ToString();
					EstBalance = PatientTable.Rows[0]["EstBalance"].ToString();
					BalTotal = PatientTable.Rows[0]["BalTotal"].ToString();
					Bal_0_30 = PatientTable.Rows[0]["Bal_0_30"].ToString();
					Bal_31_60 = PatientTable.Rows[0]["Bal_31_60"].ToString();
					Bal_61_90 = PatientTable.Rows[0]["Bal_61_90"].ToString();
					BalOver90 = PatientTable.Rows[0]["BalOver90"].ToString();
				}
				else {
					// Skip it because Patient is not a guarantor 
					continue;
				}

				Progress(PatientNm+"..");
				// Prepare EisStatement Element for Practice Element
				XmlNode PracticeEisStatement = Doc.CreateNode(XmlNodeType.Element,"EisStatement","");
				Practice.AppendChild(PracticeEisStatement);

				// Prepare EISStatementOutputFormat Attribute
				XmlAttribute EISStatementOutputFormat = Doc.CreateAttribute("OutputFormat");
				EISStatementOutputFormat.Value = "StmOut_Blue6Col";
				PracticeEisStatement.Attributes.Append(EISStatementOutputFormat);

				// Prepare CreditCardChoice Attribute
				XmlAttribute EISStatementCreditCardChoice = Doc.CreateAttribute("CreditCardChoice");
				EISStatementCreditCardChoice.Value = "MC,V,D,A";
				PracticeEisStatement.Attributes.Append(EISStatementCreditCardChoice);

				// Prepare Patient Element For EisStatement Element
				XmlNode EisStatementPatient = Doc.CreateNode(XmlNodeType.Element,"Patient","");
				PracticeEisStatement.AppendChild(EisStatementPatient);

				// Prepare Name Element For Patient Element
				XmlNode PatientName = Doc.CreateNode(XmlNodeType.Element,"Name","");
				XmlCDataSection CDataPatientName;
				CDataPatientName = Doc.CreateCDataSection(PatientNm);
				PatientName.AppendChild(CDataPatientName);
				EisStatementPatient.AppendChild(PatientName);

				// Prepare Account Element For Patient Element
				XmlNode PatientAccount = Doc.CreateNode(XmlNodeType.Element,"Account","");
				PatientAccount.InnerText = Guarantor;

				// Append Account Element to Patiend Element
				EisStatementPatient.AppendChild(PatientAccount);

				// Prepare Address1 Element For Patient Element
				XmlNode PatientAddress1 = Doc.CreateNode(XmlNodeType.Element,"Address1","");
				XmlCDataSection CDataPatientAddress1;
				CDataPatientAddress1 = Doc.CreateCDataSection(Address);
				PatientAddress1.AppendChild(CDataPatientAddress1);
				EisStatementPatient.AppendChild(PatientAddress1);

				// Prepare Address2 Element For Patient Element
				XmlNode PatientAddress2 = Doc.CreateNode(XmlNodeType.Element,"Address2","");
				XmlCDataSection CDataPatientAddress2;
				CDataPatientAddress2 = Doc.CreateCDataSection(Address2);
				PatientAddress2.AppendChild(CDataPatientAddress2);
				EisStatementPatient.AppendChild(PatientAddress2);

				// Prepare City Element For Patient Element
				XmlNode PatientCity = Doc.CreateNode(XmlNodeType.Element,"City","");
				XmlCDataSection CDataPatientCity;
				CDataPatientCity = Doc.CreateCDataSection(City);
				PatientCity.AppendChild(CDataPatientCity);
				EisStatementPatient.AppendChild(PatientCity);

				// Prepare State Element For Patient Element
				XmlNode PatientState = Doc.CreateNode(XmlNodeType.Element,"State","");
				PatientState.InnerText =  State;
				EisStatementPatient.AppendChild(PatientState);

				// Prepare Zip Element For Patient Element
				XmlNode PatientZip = Doc.CreateNode(XmlNodeType.Element,"Zip","");
				PatientZip.InnerText = Zip;
				EisStatementPatient.AppendChild(PatientZip);

				// Prepare Email Element For Patient Element
				XmlNode PatientEmail = Doc.CreateNode(XmlNodeType.Element,"Email","");
				XmlCDataSection CDataPatientEmail;
				CDataPatientEmail = Doc.CreateCDataSection(Email);
				PatientEmail.AppendChild(CDataPatientEmail);
				EisStatementPatient.AppendChild(PatientEmail);

				//************************************************************
				// Prepare Account Summary
				//************************************************************
				// Prepare AccountSummary Element For Patient Element
				XmlNode PatientAccountSummary = Doc.CreateNode(XmlNodeType.Element,"AccountSummary","");
				EisStatementPatient.AppendChild(PatientAccountSummary);

				// Prepare PriorStatementDate Element For AccountSummary Element
				XmlNode PriorStatementDate = Doc.CreateNode(XmlNodeType.Element,"PriorStatementDate","");
				PriorStatementDate.InnerText = DateTime.Now.AddDays(-30).ToString("d");
				PatientAccountSummary.AppendChild(PriorStatementDate);

				// Prepare DueDate Element For AccountSummary Element
				XmlNode DueDate = Doc.CreateNode(XmlNodeType.Element,"DueDate","");
				StmtCalcDueDate = PrefC.GetInt(PrefName.StatementsCalcDueDate);
				if(StmtCalcDueDate != -1)
					DueDate.InnerText = DateTime.Now.AddDays(StmtCalcDueDate).ToString("d");
				else
					DueDate.InnerText = DateTime.Now.AddDays(10).ToString("d");
				PatientAccountSummary.AppendChild(DueDate);

				// Prepare StatementDate Element For AccountSummary Element
				XmlNode StatementDate = Doc.CreateNode(XmlNodeType.Element,"StatementDate","");
				StatementDate.InnerText = DateTime.Now.ToString("d"); ;
				PatientAccountSummary.AppendChild(StatementDate);

				// Prepare PriorBalance Element For AccountSummary Element
				XmlNode PriorBalance = Doc.CreateNode(XmlNodeType.Element,"PriorBalance","");
				PriorBalance.InnerText = "0.00";
				PatientAccountSummary.AppendChild(PriorBalance);

				// Prepare RunningBalance Element For AccountSummary Element
				XmlNode RunningBalance = Doc.CreateNode(XmlNodeType.Element,"RunningBalance","");
				RunningBalance.InnerText = "0.00";
				PatientAccountSummary.AppendChild(RunningBalance);

				// Prepare Adjustments Element For AccountSummary Element
				XmlNode Adjustments = Doc.CreateNode(XmlNodeType.Element,"Adjustments","");
				Adjustments.InnerText = "0.00";
				PatientAccountSummary.AppendChild(Adjustments);

				// Prepare NewCharges Element For AccountSummary Element
				XmlNode NewCharges = Doc.CreateNode(XmlNodeType.Element,"NewCharges","");
				NewCharges.InnerText = "0.00";
				PatientAccountSummary.AppendChild(NewCharges);

				// Prepare FinanceCharges Element For AccountSummary Element
				XmlNode FinanceCharges = Doc.CreateNode(XmlNodeType.Element,"FinanceCharges","");
				FinanceCharges.InnerText = "0.00";
				PatientAccountSummary.AppendChild(FinanceCharges);

				// Prepare Credits Element For AccountSummary Element
				XmlNode Credits = Doc.CreateNode(XmlNodeType.Element,"Credits","");
				Credits.InnerText = "0.00";
				PatientAccountSummary.AppendChild(Credits);

				// Prepare EstInsPayments Element For AccountSummary Element
				XmlNode EstInsPayments = Doc.CreateNode(XmlNodeType.Element,"EstInsPayments","");
				EstInsPayments.InnerText = "0.00";
				PatientAccountSummary.AppendChild(EstInsPayments);

				// Prepare PatientShare Element For AccountSummary Element
				XmlNode PatientShare = Doc.CreateNode(XmlNodeType.Element,"PatientShare","");
				PatientShare.InnerText = EstBalance;
				PatientAccountSummary.AppendChild(PatientShare);

				// Prepare CurrentBalance Element For AccountSummary Element
				XmlNode CurrentBalance = Doc.CreateNode(XmlNodeType.Element,"CurrentBalance","");
				CurrentBalance.InnerText = BalTotal;
				PatientAccountSummary.AppendChild(CurrentBalance);

				// Prepare PastDue30 Element For AccountSummary Element
				XmlNode PastDue30 = Doc.CreateNode(XmlNodeType.Element,"PastDue30","");
				PastDue30.InnerText = Bal_31_60;
				PatientAccountSummary.AppendChild(PastDue30);

				// Prepare PastDue60 Element For AccountSummary Element
				XmlNode PastDue60 = Doc.CreateNode(XmlNodeType.Element,"PastDue60","");
				PastDue60.InnerText = Bal_61_90;
				PatientAccountSummary.AppendChild(PastDue60);

				// Prepare PastDue90 Element For AccountSummary Element
				XmlNode PastDue90 = Doc.CreateNode(XmlNodeType.Element,"PastDue90","");
				PastDue90.InnerText = BalOver90;
				PatientAccountSummary.AppendChild(PastDue90);

				// Prepare PastDue120 Element For AccountSummary Element
				XmlNode PastDue120 = Doc.CreateNode(XmlNodeType.Element,"PastDue120","");
				PastDue120.InnerText = "0.00";
				PatientAccountSummary.AppendChild(PastDue120);
				//****************************************************************
				// Add Notes
				//****************************************************************
				PrepareNotes(PatientID,EisStatementPatient);

				//****************************************************************
				// Add Detail Detail Items
				//****************************************************************
				PrepareDetailItems(PatientID,EisStatementPatient);
			}

		}

		/// <summary>
		/// This Method will prepare XML for Notes
		/// </summary>
		private void PrepareNotes(int PatientID,XmlNode EisStatementPatient) {
			string note = string.Empty;
			DataTable NoteTable = Statements.GetStatementNotesPracticeWeb(PatientID);

			if(NoteTable.Rows.Count > 0) {
				note = NoteTable.Rows[0]["Note"].ToString();
			}

			// Prepare Notes Element For Patient Element
			XmlNode PatientNotes = Doc.CreateNode(XmlNodeType.Element,"Notes","");
			EisStatementPatient.AppendChild(PatientNotes);

			// Prepare Note1 Element For Notes Element
			XmlNode Note1 = Doc.CreateNode(XmlNodeType.Element,"Note1","");
			XmlCDataSection CDataNote1;
			CDataNote1 = Doc.CreateCDataSection(note);
			Note1.AppendChild(CDataNote1);
			PatientNotes.AppendChild(Note1);
		}

		/// <summary>
		/// This Method will prepare XML for Detail items
		/// </summary>
		private void PrepareDetailItems(int PatientID,XmlNode EisStatementPatient) {
			// Prepare DetailItems Element For Patient Element
			XmlNode PatientDetailItems = Doc.CreateNode(XmlNodeType.Element,"DetailItems","");
			EisStatementPatient.AppendChild(PatientDetailItems);
			//js 2/13/11, this next line is flawed.  You wouldn't get a statement by using pk of the patient.
			//But that's the way PW wrote it, so I'm leaving it alone.
			Statement stmt = Statements.GetStatementInfoPracticeWeb(PatientID);
			//js I had to add this section in 7.8 to make it compile.  Not tested.
			if(stmt==null) {
				stmt.SinglePatient = true;
				stmt.DateRangeFrom = DateTime.Today;
				stmt.DateRangeTo = DateTime.Today;
				stmt.Intermingled = true;
				stmt.PatNum=(long)PatientID;
			}
			DataSet dataSet;
			dataSet = AccountModules.GetStatementDataSet(stmt);
			DataTable tableAccount = dataSet.Tables["account"];
			string tablename;
			// accounttable name is account+patientID
			// Iterate through all table and pickup table
			for(int i = 0;i < dataSet.Tables.Count;i++) {
				tablename = dataSet.Tables[i].TableName;
				if(tablename.StartsWith("account")) {
					tableAccount = dataSet.Tables[i];
				}
			}

			string date,patient,ProcCode,tth,description,fulldesc,charges,credits,balance;
			for(int p = 0;p < tableAccount.Rows.Count;p++) {
				date = tableAccount.Rows[p]["date"].ToString();
				patient = tableAccount.Rows[p]["patient"].ToString();
				ProcCode = tableAccount.Rows[p]["ProcCode"].ToString();
				tth = tableAccount.Rows[p]["tth"].ToString();
				description = tableAccount.Rows[p]["description"].ToString();
				fulldesc = ProcCode + " " + tth + " " + description;
				charges = tableAccount.Rows[p]["charges"].ToString();
				credits = tableAccount.Rows[p]["credits"].ToString();
				balance = tableAccount.Rows[p]["balance"].ToString();
				//add Appripriate Tags
				// Prepare Item Element For DetailItem Element
				XmlNode Item = Doc.CreateNode(XmlNodeType.Element,"Item","");
				PatientDetailItems.AppendChild(Item);
				// Prepare Date Element For Item Element
				XmlNode ItemDate = Doc.CreateNode(XmlNodeType.Element,"Date","");
				ItemDate.InnerText = date;
				Item.AppendChild(ItemDate);

				// Prepare PatientName Element For Item Element
				XmlNode ItemPatientName = Doc.CreateNode(XmlNodeType.Element,"PatientName","");
				XmlCDataSection CDataItemPatientName;
				CDataItemPatientName = Doc.CreateCDataSection(patient);
				ItemPatientName.AppendChild(CDataItemPatientName);
				Item.AppendChild(ItemPatientName);

				// Prepare Description Element For Item Element
				XmlNode ItemDescription = Doc.CreateNode(XmlNodeType.Element,"Description","");
				XmlCDataSection CDataItemDescription;
				CDataItemDescription = Doc.CreateCDataSection(fulldesc);
				ItemDescription.AppendChild(CDataItemDescription);
				Item.AppendChild(ItemDescription);

				// Prepare Charges Element For Item Element
				XmlNode ItemCharges = Doc.CreateNode(XmlNodeType.Element,"Charges","");
				ItemCharges.InnerText = charges;
				Item.AppendChild(ItemCharges);

				// Prepare Credits Element For Item Element
				XmlNode ItemCredits = Doc.CreateNode(XmlNodeType.Element,"Credits","");
				ItemCredits.InnerText = credits;
				Item.AppendChild(ItemCredits);

				// Prepare Balance Element For Item Element
				XmlNode ItemBalance = Doc.CreateNode(XmlNodeType.Element,"Balance","");
				ItemBalance.InnerText = balance;
				Item.AppendChild(ItemBalance);
			}

		}
		#endregion

		# region Store PatientStatement File
		// Store file
		private bool isPatientFileCreated() {
			Progress("Store Statements for Transmission..");
			fileName = PrefC.GetRaw("PatienteBillPath") + "\\patientebill.xml";
			try {
				// remove any prior trasmitted file
				if(File.Exists(fileName)) {
					File.Delete(fileName);
				}
				Doc.Save(fileName);
				return true;
			}
			catch(Exception ex) {
				Error("Patient eBill File Create Error "+ex.Message.ToString());
				return false;
			}
		}
		# endregion

		# region Transmit File
		// Transmit file
		private void Transmit() {
			if(isPatientFileCreated() == true) {
				try {
					Progress("Begin Transmit..");
					string gc_MimeSep = "---------------------------7d13e425b00d0";
					string gc_MimeFunction = "Content-Disposition: form-data; name=\"Function\"";
					string gc_MimeSource = "Content-Disposition: form-data; name=\"Source\"";
					string gc_MimeAuth = "Content-Disposition: form-data; name=\"AuthenticationID\"";
					string gc_MimeFile = "Content-Disposition: form-data; name=\"File\"; filename=\"";
					string tempFile = PrefC.GetRaw("PatienteBillPath") + "\\tmp.xml";
					string RequestStr = "--" + gc_MimeSep + "\r\n" + gc_MimeFunction +
                                             "\r\n\r\n" + "Upload\r\n" + "--" + gc_MimeSep + "\r\n" +
                                              gc_MimeSource + "\r\n\r\n" + "STM\r\n" + "--" + gc_MimeSep + "\r\n"
                                              + gc_MimeAuth + "\r\n\r\n" + AuthenticationID + "\r\n" +
                                                   "--" + gc_MimeSep + "\r\n" + gc_MimeFile + tempFile + '\"' +
                                              "\r\n" + "Content-Type: text/plain" + "\r\n\r\n";

					StreamReader xmlStream = new StreamReader(fileName);
					StreamWriter xmlTmp = new StreamWriter(tempFile);
					xmlTmp.Write(RequestStr);
					xmlTmp.Write(xmlStream.ReadToEnd());
					xmlTmp.Close();
					xmlStream.Close();

					StreamReader xmlUpload = new StreamReader(tempFile);
					byte[] fileContents = Encoding.Default.GetBytes(xmlUpload.ReadToEnd());
					xmlUpload.Close();

					HttpWebRequest request1 = (HttpWebRequest)WebRequest.Create(PatienteBillServerAddress + "?" + RequestStr);
					request1.ContentType = "multipart/form-data; boundary=" + gc_MimeSep;
					request1.Method = "POST";
					request1.ContentLength = fileContents.Length;
					request1.AllowAutoRedirect = false;

					Stream requestStream = request1.GetRequestStream();
					requestStream.Write(fileContents,0,fileContents.Length);
					requestStream.Close();


					/* ------------------------- Get Response -----------------------*/
					HttpWebResponse response = (HttpWebResponse)request1.GetResponse();
					StreamReader sr = new StreamReader(response.GetResponseStream());
					finalResponse = sr.ReadToEnd();
					// Delete temp file
					FileInfo trASH = new FileInfo(tempFile);
					trASH.Delete();
					Progress("End Transmit..");
					ProcessFinalResponse(finalResponse);
				}
				catch(Exception ex) {
					Error("Transmit failed. "+ex.Message.ToString());
				}
			}
		}
		# endregion

		# region Process Final respose

		private void ProcessFinalResponse(string finalResponse) {
			try {
				Progress("Process Clearinghouse Response..");
				string[] parseResponse;
				char[] separator = { '&' };
				parseResponse = finalResponse.Split(separator);
				int responseStatus;
				string[] statusRespose;
				char[] separatorequal = { '=' };
				statusRespose = parseResponse[0].Split(separatorequal);
				responseStatus = PIn.Int(statusRespose[1]);

				string errormessage = string.Empty;
				switch(responseStatus) {
					case 0:
						Progress("Patient eBill transmission completed Successfully.");
						MessageBox.Show(this,"Patient eBill transmission completed Successfully.");
						this.Close();
						break;
					case 1:
						errormessage = "Authentication Failed.\r\n" + parseResponse[3].Remove(0,13);
						break;
					case 2:
						errormessage = "Cannot upload at this time.\r\n" + parseResponse[3].Remove(0,13);
						break;
					default:
						errormessage = "Unknown Status(" + responseStatus + ").\r\n" + parseResponse[3].Remove(0,13);
						break;
				}
				if(errormessage.Length > 0)
					Error(errormessage + DentalxChangeContactInfo);
			}
			catch(Exception ex) {
				Error("Error Parsing Final response "+ex.Message.ToString());
			}
		}

		# endregion

		# region Status Display
		private void Progress(string statusMsg) {
			this.tBStatus.Text = this.tBStatus.Text + statusMsg + "\r\n";
		}
		private void Error(string statusMsg) {
			this.tBError.Text = this.tBStatus.Text + statusMsg + "\r\n";
		}
		#endregion


		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}
	}
}
