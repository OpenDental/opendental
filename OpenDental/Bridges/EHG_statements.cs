using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental.Bridges {
	public class EHG_statements {
		//these are temporary:
		//private static string vendorID="68";
		//private static string vendorPMScode="144";
		//private static string clientAccountNumber="8011";//the dental office number set by EHG
		//private static string creditCardChoices="MC,D,V,A";//MasterCard,Discover,Visa,AmericanExpress
		//private static string userName="";
		//private static string password="";

		///<summary>Returns empty list if no errors.  Otherwise returns a list with error messages.</summary>
		public static List <string> Validate(long clinicNum) {
			List <string> listErrors=new List<string>();
			Clinic clinic=Clinics.GetClinic(clinicNum);
			Ebill eBillClinic=Ebills.GetForClinic(clinicNum);
			Ebill eBillDefault=Ebills.GetForClinic(0);
			EHG_Address addressRemit=null;
			if(eBillClinic==null) {
				addressRemit=GetAddress(eBillDefault.RemitAddress,clinic);
			}
			else {
				addressRemit=GetAddress(eBillClinic.RemitAddress,clinic);
			}
			if(addressRemit.Address1.Trim().Length==0 || addressRemit.City.Trim().Length==0
				|| addressRemit.State.Trim().Length==0 || addressRemit.Zip.Trim().Length==0)
			{
				listErrors.Add(Lan.g("EHG_Statements","invalid")+" "+Lan.g("EHG_Statements",addressRemit.Source));
			}
			return listErrors;
		}

		///<summary>Generates all the xml up to the point where the first statement would go.</summary>
		public static void GeneratePracticeInfo(XmlWriter writer,long clinicNum) {
			Clinic clinic=Clinics.GetClinic(clinicNum);
			Ebill eBillClinic=Ebills.GetForClinic(clinicNum);
			Ebill eBillDefault=Ebills.GetForClinic(0);
			writer.WriteProcessingInstruction("xml","version = \"1.0\" standalone=\"yes\"");
			writer.WriteStartElement("EISStatementFile");
			writer.WriteAttributeString("VendorID",PrefC.GetString(PrefName.BillingElectVendorId));
			writer.WriteAttributeString("OutputFormat","StmOut_Blue6Col");
			writer.WriteAttributeString("Version","2");
			writer.WriteElementString("SubmitDate",DateTime.Today.ToString("yyyy-MM-dd"));
			writer.WriteElementString("PrimarySubmitter",PrefC.GetString(PrefName.BillingElectVendorPMSCode));
			writer.WriteElementString("Transmitter","EHG");
			writer.WriteStartElement("Practice");
			string billingClientAccountNumber=eBillDefault.ClientAcctNumber;
			if(eBillClinic!=null && eBillClinic.ClientAcctNumber!="") {//clinic eBill entry exists, check the fields for overrides
				billingClientAccountNumber=eBillClinic.ClientAcctNumber;
			}
			writer.WriteAttributeString("AccountNumber",billingClientAccountNumber);
			//sender address----------------------------------------------------------
			writer.WriteStartElement("SenderAddress");
			if(clinic==null) {
				writer.WriteElementString("Name",PrefC.GetString(PrefName.PracticeTitle));
			}
			else {
				writer.WriteElementString("Name",clinic.Description);
			}
			if(eBillClinic==null) {
				WriteAddress(writer,eBillDefault.PracticeAddress,clinic);
			}
			else {
				WriteAddress(writer,eBillClinic.PracticeAddress,clinic);
			}
			writer.WriteEndElement();//senderAddress
			//remit address----------------------------------------------------------
			writer.WriteStartElement("RemitAddress");
			if(clinic==null) {
				writer.WriteElementString("Name",PrefC.GetString(PrefName.PracticeTitle));
			}
			else {
				writer.WriteElementString("Name",clinic.Description);
			}
			if(eBillClinic==null) {
				WriteAddress(writer,eBillDefault.RemitAddress,clinic);				
			}
			else {
				WriteAddress(writer,eBillClinic.RemitAddress,clinic);
			}
			writer.WriteEndElement();//remitAddress
			//Rendering provider------------------------------------------------------
			Provider prov=Providers.GetProv(PrefC.GetLong(PrefName.PracticeDefaultProv));
			writer.WriteStartElement("RenderingProvider");
			writer.WriteElementString("Name",prov.GetFormalName());
			ProviderClinic provClinic=ProviderClinics.GetOneOrDefault(prov.ProvNum,clinicNum);
			writer.WriteElementString("LicenseNumber",(provClinic==null ? "" : provClinic.StateLicense));
			writer.WriteElementString("State",PrefC.GetString(PrefName.PracticeST));
			writer.WriteEndElement();//Rendering provider
		}

		private static void WriteAddress(XmlWriter writer,EbillAddress eBillAddress,Clinic clinic) {
			EHG_Address address=GetAddress(eBillAddress,clinic);
			writer.WriteElementString("Address1",address.Address1);
			writer.WriteElementString("Address2",address.Address2);
			writer.WriteElementString("City",address.City);
			writer.WriteElementString("State",address.State);
			writer.WriteElementString("Zip",address.Zip);
			writer.WriteElementString("Phone",address.Phone);
		}

		///<summary>The clinic variable can be null.</summary>
		public static EHG_Address GetAddress(EbillAddress eBillAddress,Clinic clinic) {
			EHG_Address address=new EHG_Address();
			//If using practice information or using the default (no clinic) Ebill and a clinic enum is specified, use the practice level information.
			if(eBillAddress==EbillAddress.PracticePhysical || (clinic==null && eBillAddress==EbillAddress.ClinicPhysical)) {
				address.Address1=PrefC.GetString(PrefName.PracticeAddress);
				address.Address2=PrefC.GetString(PrefName.PracticeAddress2);
				address.City=PrefC.GetString(PrefName.PracticeCity);
				address.State=PrefC.GetString(PrefName.PracticeST);
				address.Zip=PrefC.GetString(PrefName.PracticeZip);
				address.Phone=PrefC.GetString(PrefName.PracticePhone);//enforced to be 10 digit fairly rigidly by the UI
				address.Source="Practice Physical Treating Address";
			}
			else if(eBillAddress==EbillAddress.PracticePayTo || (clinic==null && eBillAddress==EbillAddress.ClinicPayTo)) {
				address.Address1=PrefC.GetString(PrefName.PracticePayToAddress);
				address.Address2=PrefC.GetString(PrefName.PracticePayToAddress2);
				address.City=PrefC.GetString(PrefName.PracticePayToCity);
				address.State=PrefC.GetString(PrefName.PracticePayToST);
				address.Zip=PrefC.GetString(PrefName.PracticePayToZip);
				address.Phone=PrefC.GetString(PrefName.PracticePayToPhone);//enforced to be 10 digit fairly rigidly by the UI
				address.Source="Practice Pay To Address";
			}
			else if(eBillAddress==EbillAddress.PracticeBilling || (clinic==null && eBillAddress==EbillAddress.ClinicBilling)) {
				address.Address1=PrefC.GetString(PrefName.PracticeBillingAddress);
				address.Address2=PrefC.GetString(PrefName.PracticeBillingAddress2);
				address.City=PrefC.GetString(PrefName.PracticeBillingCity);
				address.State=PrefC.GetString(PrefName.PracticeBillingST);
				address.Zip=PrefC.GetString(PrefName.PracticeBillingZip);
				address.Phone=PrefC.GetString(PrefName.PracticeBillingPhone);//enforced to be 10 digit fairly rigidly by the UI
				address.Source="Practice Billing Address";
			}
			else if(eBillAddress==EbillAddress.ClinicPhysical) {
				address.Address1=clinic.Address;
				address.Address2=clinic.Address2;
				address.City=clinic.City;
				address.State=clinic.State;
				address.Zip=clinic.Zip;
				address.Phone=clinic.Phone;//enforced to be 10 digit fairly rigidly by the UI
				address.Source="Clinic Physical Treating Address";
			}
			else if(eBillAddress==EbillAddress.ClinicPayTo) {
				address.Address1=clinic.PayToAddress;
				address.Address2=clinic.PayToAddress2;
				address.City=clinic.PayToCity;
				address.State=clinic.PayToState;
				address.Zip=clinic.PayToZip;
				address.Phone=clinic.Phone;//enforced to be 10 digit fairly rigidly by the UI
				address.Source="Clinic Pay To Address";
			}
			else if(eBillAddress==EbillAddress.ClinicBilling) {
				address.Address1=clinic.BillingAddress;
				address.Address2=clinic.BillingAddress2;
				address.City=clinic.BillingCity;
				address.State=clinic.BillingState;
				address.Zip=clinic.BillingZip;
				address.Phone=clinic.Phone;//enforced to be 10 digit fairly rigidly by the UI
				address.Source="Clinic Billing Address";
			}
			return address;
		}

		///<summary>Adds the xml for one statement. Validation is performed here. Throws an exception if there is a validation failure.</summary>
		public static void GenerateOneStatement(XmlWriter writer,Statement stmt,Patient pat,Family fam,DataSet dataSet){
			DataTable tableMisc=dataSet.Tables["misc"];
			DataTable tableAccount=dataSet.Tables.OfType<DataTable>().FirstOrDefault(x => x.TableName.StartsWith("account"));
			Patient guar=fam.ListPats[0];
			if(!Regex.IsMatch(guar.State,"^[A-Z]{2}$")) {
				throw new ApplicationException(Lan.g("EHG_Statements","Guarantor state must be two uppercase characters.")+" "+guar.FName+" "+guar.LName+" #"+guar.PatNum);
			}
			writer.WriteStartElement("EisStatement");
			writer.WriteAttributeString("OutputFormat","StmOut_Blue6Col");
			writer.WriteAttributeString("CreditCardChoice",PrefC.GetString(PrefName.BillingElectCreditCardChoices));
			writer.WriteStartElement("Patient");
			writer.WriteElementString("Name",guar.GetNameFLFormal());
			writer.WriteElementString("Account",guar.PatNum.ToString());
			writer.WriteElementString("Address1",guar.Address);
			writer.WriteElementString("Address2",guar.Address2);
			writer.WriteElementString("City",guar.City);
			writer.WriteElementString("State",guar.State);
			writer.WriteElementString("Zip",guar.Zip);
			writer.WriteElementString("EMail",guar.Email);
			//Account summary-----------------------------------------------------------------------
			writer.WriteStartElement("AccountSummary");
			if(stmt.DateRangeFrom.Year<1880) {//make up a statement date.
				writer.WriteElementString("PriorStatementDate",DateTime.Today.AddMonths(-1).ToString("MM/dd/yyyy"));
			}
			else {
				writer.WriteElementString("PriorStatementDate",stmt.DateRangeFrom.AddDays(-1).ToString("MM/dd/yyyy"));
			}
			DateTime dueDate;
			if(PrefC.GetLong(PrefName.StatementsCalcDueDate)==-1){
				dueDate=DateTime.Today.AddDays(10);
			}
			else{
				dueDate=DateTime.Today.AddDays(PrefC.GetLong(PrefName.StatementsCalcDueDate));
			}
			writer.WriteElementString("DueDate",dueDate.ToString("MM/dd/yyyy"));
			writer.WriteElementString("StatementDate",stmt.DateSent.ToString("MM/dd/yyyy"));
			double balanceForward=tableMisc.Rows.OfType<DataRow>().Where(x => x["descript"].ToString()=="balanceForward")
				.Select(x => PIn.Double(x["value"].ToString())).FirstOrDefault();//defaults to 0
			writer.WriteElementString("PriorBalance",balanceForward.ToString("F2"));
			writer.WriteElementString("RunningBalance","");//for future use
			writer.WriteElementString("PerPayAdj","");//optional
			writer.WriteElementString("InsPayAdj","");//optional
			writer.WriteElementString("Adjustments","");//for future use
			double charges=tableAccount.Rows.OfType<DataRow>().Sum(x => PIn.Double(x["chargesDouble"].ToString()));
			writer.WriteElementString("NewCharges",charges.ToString("F2"));//optional
			writer.WriteElementString("FinanceCharges","");//for future use
			double credits=tableAccount.Rows.OfType<DataRow>().Sum(x => PIn.Double(x["creditsDouble"].ToString()));
			writer.WriteElementString("Credits",credits.ToString("F2"));
			//On a regular printed statement, the amount due at the top might be different from the balance at the middle right due to payplan balances.
			//But in e-bills, there is only one amount due.  Insurance estimate is already subtracted, and payment plan balance is already added.
			double amountDue=guar.BalTotal;
			if(PrefC.GetInt(PrefName.PayPlansVersion)==1) {//with version 2, payplan debits/credits are aged individually and are included in guar.BalTotal
				amountDue+=tableMisc.Rows.OfType<DataRow>().Where(x => x["descript"].ToString()=="payPlanDue")
					.Select(x => PIn.Double(x["value"].ToString())).DefaultIfEmpty(0).Sum();//add payplan(s) due amt
			}
			double insEst=0;
			if(!PrefC.GetBool(PrefName.BalancesDontSubtractIns)) {//this is typical
				insEst=guar.InsEst;
			}
			InstallmentPlan installPlan=InstallmentPlans.GetOneForFam(guar.PatNum);
			if(installPlan!=null && installPlan.MonthlyPayment<(amountDue-insEst)) {
				amountDue=installPlan.MonthlyPayment;
				insEst=0;
			}
			writer.WriteElementString("EstInsPayments",insEst.ToString("F2"));//optional.
			writer.WriteElementString("PatientShare",(amountDue-insEst).ToString("F2"));
			writer.WriteElementString("CurrentBalance",amountDue.ToString("F2"));//this is ambiguous.  It seems to be AmountDue, but it could be 0-30 days aging
			writer.WriteElementString("PastDue30",guar.Bal_31_60.ToString("F2"));//optional
			writer.WriteElementString("PastDue60",guar.Bal_61_90.ToString("F2"));//optional
			writer.WriteElementString("PastDue90",guar.BalOver90.ToString("F2"));//optional
			writer.WriteElementString("PastDue120","");//optional
			writer.WriteEndElement();//AccountSummary
			//Notes-----------------------------------------------------------------------------------
			writer.WriteStartElement("Notes");
			if(stmt.NoteBold!="") {
				writer.WriteStartElement("Note");
				writer.WriteAttributeString("FgColor","Red");//ColorToHexString(Color.DarkRed));
				//writer.WriteAttributeString("BgColor",ColorToHexString(Color.White));
				writer.WriteCData(StringTools.Truncate(stmt.NoteBold,500));//Limit of 500 char on notes.
				writer.WriteEndElement();//Note
			}
			if(stmt.Note!="") {
				writer.WriteStartElement("Note");
				//writer.WriteAttributeString("FgColor",ColorToHexString(Color.Black));
				//writer.WriteAttributeString("BgColor",ColorToHexString(Color.White));
				writer.WriteCData(StringTools.Truncate(stmt.Note,500));//Limit of 500 char on notes.
				writer.WriteEndElement();//Note
			}
			writer.WriteEndElement();//Notes
			//Detail items------------------------------------------------------------------------------
			writer.WriteStartElement("DetailItems");
			List<string> lines;
			int seq=0;
			//Jessica at DentalXchange says limit is 120.  Specs say limit is 30.
			//If we send more than 50 characters, DentalXChange will break the line at the 50th character, even if it is in the middle of a word, and wrap
			//the rest of the line, so up to 70 chars onto line 2, which could easily extend past the end of the description field.  The wrapped line will
			//also have a different line spacing than if sent as a separate xml element.  See the examples in 
			//...\Programmers Documents\eClaims Clearinghouse and Carrier Specific Details\DentalXChange ClaimConnect\ClaimConnect - EHG - WebClaim\EHG Statements\Examples
			//Example: original description:
				//line 1: 'D6103 5  bone graft for repair of periimplant defect - does not include flap entry and closure.  Placement of a barrier membrane or biologic materials to aid in osseous regeneration are reported separately'
			//version 16.2 sent as:
				//line 1: 'D6103 5  bone graft for repair of periimplant defect - does not include flap entry and closure.  Placement of a barrier '
				//line 2: 'membrane or biologic materials to aid in osseous regeneration are reported separately'
			//DentalXChange displayed lines as (with line 2 extended past the end of the description field):
				//line 1: 'D6103 5 bone graft for repair of periimplant defe -'
				//line 2: 'ct - does not include flap entry and closure. Placement of a barrier'
				//line 3: 'membrane or biologic materials to aid in osseous r -'
				//line 4: 'egeneration are reported separately'
			//version 16.3 and up sent and displayed as:
				//line 1: 'D6103 5  bone graft for repair of periimplant'
				//line 2: 'defect - does not include flap entry and closure.'
				//line 3: 'Placement of a barrier membrane or biologic'
				//line 4: 'materials to aid in osseous regeneration are'
				//line 5: 'reported separately'
			const int lineMaxLen=50;
			int firstIndexNewLine;
			string lineCur;
			bool doJoinProcCode;
			foreach(DataRow rowCur in tableAccount.Rows) {
				//If this is not an adjustment (AdjNum==0) then include the ProcCode.
				//Otherwise, only join the proc code ('Adjust') if the preference is enabled.
				doJoinProcCode=(rowCur["AdjNum"].ToString()=="0" || PrefC.GetBool(PrefName.BillingElectIncludeAdjustDescript));//False if is adjustment and pref is false.
				//There are frequently CRs within a procedure description for things like ins est.
				lines=string.Join(" ",new[] { doJoinProcCode?rowCur["ProcCode"].ToString():"",rowCur["tth"].ToString(),rowCur["description"].ToString() })
					.Split(new string[] { "\r\n" },StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
				for(int li=0;li<lines.Count;li++) {
					lineCur=lines[li];
					if(lineCur.Length<1 || lineCur.All(x => char.IsWhiteSpace(x))) {//nothing to write
						continue;
					}
					writer.WriteStartElement("DetailItem");//has a child item. We won't add optional child note
					writer.WriteAttributeString("sequence",seq.ToString());
					writer.WriteStartElement("Item");
					writer.WriteElementString("Date",li==0?PIn.Date(rowCur["DateTime"].ToString()).ToString("MM/dd/yyyy"):"");
					writer.WriteElementString("PatientName",li==0?rowCur["patient"].ToString():"");
					if(lineCur.Length>lineMaxLen) {
						firstIndexNewLine=lineMaxLen;
						for(int c=lineMaxLen-2;c>-1;c--) {//-2, 1 for length to index and 1 so we can safely check index and index+1
							if(!char.IsWhiteSpace(lineCur[c]) && char.IsWhiteSpace(lineCur[c+1])) {
								firstIndexNewLine=c+1;
								break;
							}
						}
						lines.Insert(li+1,lineCur.Substring(firstIndexNewLine).Trim());
						lines[li]=StringTools.Truncate(lineCur,firstIndexNewLine);
					}
					writer.WriteStartElement("Description");
					writer.WriteCData(lines[li]);//CData to allow any string, including punctuation, syntax characters and special characters.
					writer.WriteEndElement();//Description
					writer.WriteElementString("Charges",li==0?rowCur["charges"].ToString():"");
					writer.WriteElementString("Credits",li==0?rowCur["credits"].ToString():"");
					writer.WriteElementString("Balance",li==0?rowCur["balance"].ToString():"");
					writer.WriteEndElement();//Item
					writer.WriteEndElement();//DetailItem
					seq++;
				}
				#region Notes Don't Display On Statements
				/*The code below just didn't work because notes don't get displayed on the statement.
				linedesc=lines[0];
				note="";
				if(linedesc.Length>30) {
					note=linedesc.Substring(30);
					linedesc=linedesc.Substring(0,30);
				}
				for(int l=1;l<lines.Length;l++) {
					if(note!="") {
						note+="\r\n";
					}
					note+=lines[l];
				}
				
				if(note!="") {
					writer.WriteStartElement("Note");
					//we're not going to specify colors here since they're optional
					writer.WriteCData(note);
					writer.WriteEndElement();//Note
				}*/
				#endregion Notes Don't Display On Statements
			}
			writer.WriteEndElement();//DetailItems
			writer.WriteEndElement();//Patient
			writer.WriteEndElement();//EisStatement
		}

		///<summary>Converts a .net color to a hex string.  Includes the #.</summary>
		private static string ColorToHexString(Color color) {
			char[] hexDigits={'0','1','2','3','4','5','6','7','8','9','A','B','C','D','E','F'};
			byte[] bytes = new byte[3];
			bytes[0] = color.R;
			bytes[1] = color.G;
			bytes[2] = color.B;
			char[] chars=new char[bytes.Length * 2];
			for(int i=0;i<bytes.Length;i++){
				int b=bytes[i];
				chars[i*2]=hexDigits[b >> 4];
				chars[i*2+1]=hexDigits[b & 0xF];
			}
			string retVal=new string(chars);
			retVal="#"+retVal;
			return retVal;
		}

		///<summary>After statements are added, this adds the necessary closing xml elements.</summary>
		public static void GenerateWrapUp(XmlWriter writer) {
			writer.WriteEndElement();//Practice
			writer.WriteEndElement();//EISStatementFile
		}

		///<summary>If str.length>len, returns the substring from index 0 of str to a total length of len.  If str.Length&lt;=len, returns str.</summary>
		private static string Tidy(string str,int len) {
			if(str.Length>len) {
				return str.Substring(0,len);
			}
			return str;
		}

		///<summary>Surround with try catch.  The "data" is the previously constructed xml.  If the internet connection is lost or unavailable, then the exception thrown will be a 404 error similar to the following: "The remote server returned an error: (404) Not Found"</summary>
		public static void Send(string data,long clinicNum) {
			//Validate the structure of the XML before sending.
			StringReader sr=new StringReader(data);
			try {
				XmlReader xmlr=XmlReader.Create(sr);
				while(xmlr.Read()) { //Read every node an ensure that there are no exceptions thrown.
				}
			}
			catch(Exception ex) {
				throw new ApplicationException("Invalid XML in statement batch: "+ex.Message);
			}
			finally {
				sr.Dispose();
			}
			string strHistoryFile="";
			if(PrefC.GetBool(PrefName.BillingElectSaveHistory)) {
				string strHistoryDir=CodeBase.ODFileUtils.CombinePaths(ImageStore.GetPreferredAtoZpath(),"EHG_History");
				if(!Directory.Exists(strHistoryDir)) {
					Directory.CreateDirectory(strHistoryDir);
				}
				strHistoryFile=CodeBase.ODFileUtils.CreateRandomFile(strHistoryDir,".txt");
				ODFileUtils.WriteAllText(strHistoryFile,data);
			}
			//Step 1: Post authentication request:
			Version myVersion=new Version(Application.ProductVersion);
			HttpWebRequest webReq;
			WebResponse response;
			StreamReader readStream;
			string str;
			string[] responseParams;
			string status="";
			string group="";
			string userid="";
			string authid="";
			string errormsg="";
			string alertmsg="";
			string curParam="";
			string serverName="https://claimconnect.dentalxchange.com/dci/upload.svl";//live URL for claims (According to phone call with Dentalxchange)
			string serverNameOverride = PrefC.GetString(PrefName.BillingElectStmtUploadURL);
			if(!string.IsNullOrEmpty(serverNameOverride)) {
				serverName=serverNameOverride;
			}
			if(ODBuild.IsDebug()) {
				//serverName="https://prelive.dentalxchange.com/dci/upload.svl";      //test URL for claims
				//serverName="https://claimconnect.dentalxchange.com/dci/upload.svl"; //live URL for claims
				//serverName="https://prelive.dentalxchange.com/dci/upload.svl";      //test URL for Stmts
				//serverName="https://billconnect.dentalxchange.com/dci/upload.svl";  //live URL for Stmts; probably the correct one to use.
			}
			webReq=(HttpWebRequest)WebRequest.Create(serverName);
			Ebill ebillDefault=Ebills.GetForClinic(0);
			string billingUserName=ebillDefault.ElectUserName;
			string billingPasswordEnc=ebillDefault.ElectPassword;
			if(PrefC.HasClinicsEnabled && clinicNum!=0) {
				Ebill eBill=Ebills.GetForClinic(clinicNum);
				if(eBill!=null) {//eBill entry exists, check the fields for overrides.
					if(eBill.ElectUserName!="") {
						billingUserName=eBill.ElectUserName;
					}
					if(eBill.ElectPassword!="") {
						billingPasswordEnc=eBill.ElectPassword;
					}
				}
			}
			if(!CDT.Class1.Decrypt(billingPasswordEnc,out string billingPassword)) {
				billingPassword=billingPasswordEnc;//If decryption is successful we won't get here. Instead the out parameter is used.
			}
			string postData=
				"Function=Auth"//CONSTANT; signifies that this is an authentication request
				+"&Source=STM"//CONSTANT; file format
				+"&UploaderName=OpenDental"//CONSTANT
				+"&UploaderVersion="+myVersion.Major.ToString()+"."+myVersion.Minor.ToString()+"."+myVersion.Build.ToString()//eg 12.3.24			
				+"&Username="+billingUserName
				+"&Password="+billingPassword;
			webReq.KeepAlive=false;
			webReq.Method="POST";
			webReq.ContentType="application/x-www-form-urlencoded";
			webReq.ContentLength=postData.Length;
			ASCIIEncoding encoding=new ASCIIEncoding();
			byte[] bytes=encoding.GetBytes(postData);
			Stream streamOut=webReq.GetRequestStream();
			streamOut.Write(bytes,0,bytes.Length);
			streamOut.Close();
			response=webReq.GetResponse();
			//Process the authentication response:
			readStream=new StreamReader(response.GetResponseStream(),Encoding.ASCII);
			str=readStream.ReadToEnd();
			readStream.Close();
			if(strHistoryFile!="") {//Tack the response onto the end of the saved history file if one was created above.
				File.AppendAllText(strHistoryFile,"\r\n\r\nCONNECTION REQUEST: postData.Length="+postData.Length+" bytes.Length="+bytes.Length+"==============\r\n"
					+" RESPONSE TO CONNECTION REQUEST================================================================\r\n"+str);
			}
			//Debug.WriteLine(str);
			//MessageBox.Show(str);
			responseParams=str.Split('&');
			for(int i=0;i<responseParams.Length;i++) {
				curParam=GetParam(responseParams[i]);
				switch(curParam) {
					case "Status":
						status=GetParamValue(responseParams[i]);
						break;
					case "GROUP":
						group=GetParamValue(responseParams[i]);
						break;
					case "UserID":
						userid=GetParamValue(responseParams[i]);
						break;
					case "AuthenticationID":
						authid=GetParamValue(responseParams[i]);
						break;
					case "ErrorMessage":
						errormsg=GetParamValue(responseParams[i]);
						break;
					case "AlertMessage":
						alertmsg=GetParamValue(responseParams[i]);
						break;
					default:
						throw new Exception("Unexpected parameter: "+curParam);
				}
			}
			//Process response for errors:
			if(alertmsg!="") {
				MessageBox.Show(alertmsg);
			}
			switch(status) {
				case "0":
					//MessageBox.Show("Authentication successful.");
					break;
				case "1":
					throw new Exception("Authentication failed. "+errormsg);
				case "2":
					throw new Exception("Cannot authenticate at this time. "+errormsg);
				case "3":
					throw new Exception("Invalid authentication request. "+errormsg);
				case "4":
					throw new Exception("Invalid program version. "+errormsg);
				case "5":
					throw new Exception("No customer contract. "+errormsg);
				default://some as-yet-undefined error
					throw new Exception("Error "+status+". "+errormsg);
			}
			//Step 2: Post upload request:
			//string fileName=Directory.GetFiles(clearhouse.ExportPath)[0];
			string boundary="------------7d13e425b00d0";
			postData=
				"--"+boundary+"\r\n"
				+"Content-Disposition: form-data; name=\"Function\"\r\n"
				+"\r\n"
				+"Upload\r\n"
				+"--"+boundary+"\r\n"
				+"Content-Disposition: form-data; name=\"Source\"\r\n"
				+"\r\n"
				+"STM\r\n"
				+"--"+boundary+"\r\n"
				+"Content-Disposition: form-data; name=\"AuthenticationID\"\r\n"
				+"\r\n"
				+authid+"\r\n"
				+"--"+boundary+"\r\n"
				+"Content-Disposition: form-data; name=\"File\"; filename=\""+"stmt.xml"+"\"\r\n"
				+"Content-Type: text/plain\r\n"
				+"\r\n"
			//using(StreamReader sr=new StreamReader(fileName)) {
			//	postData+=sr.ReadToEnd()+"\r\n"
				+data+"\r\n"
				+"--"+boundary+"--";
			//}
			//Debug.WriteLine(postData);
			//MessageBox.Show(postData);
			webReq=(HttpWebRequest)WebRequest.Create(serverName);
			//Timeout documentation: https://msdn.microsoft.com/en-us/library/system.net.httpwebrequest.timeout(v=vs.110).aspx.
			//Timeout: "Gets or sets the time-out value in milliseconds for the GetResponse and GetRequestStream methods."
			//Timeout default is 100 seconds, which should be sufficient in waiting for a reply from dentalxchange, since the reply is small.
			//ReadWriteTimeout documentation: https://msdn.microsoft.com/en-us/library/system.net.httpwebrequest.readwritetimeout%28v=vs.110%29.aspx
			//ReadWriteTimeout: "Gets or sets a time-out in milliseconds when writing to or reading from a stream."
			//ReadWriteTimeout default is 300 seconds (5 minutes).
			//Our message box that tells the user to wait up to 10 minutes for bills to send, therefore we need at least a 10 minute ReadWriteTimeout.
			//The user sees progress in the UI when sending.  We can increase timeouts as much as we want without making the program look like it crashed.
			webReq.ReadWriteTimeout=600000;//10 minutes = 10*60 seconds = 600 seconds = 600*1000 milliseconds = 600,000 milliseconds.
			webReq.KeepAlive=false;
			webReq.Method="POST";
			webReq.ContentType="multipart/form-data; boundary="+boundary;
			webReq.ContentLength=postData.Length;
			bytes=encoding.GetBytes(postData);
			streamOut=webReq.GetRequestStream();
			streamOut.Write(bytes,0,bytes.Length);
			streamOut.Close();
			response=webReq.GetResponse();
			//Process the response
			readStream=new StreamReader(response.GetResponseStream(),Encoding.ASCII);
			str=readStream.ReadToEnd();
			readStream.Close();
			if(strHistoryFile!="") {//Tack the response onto the end of the saved history file if one was created above.
				File.AppendAllText(strHistoryFile,"\r\n\r\nUPLOAD REQUEST: postData.Length="+postData.Length+" bytes.Length="+bytes.Length+"==============\r\n"
					+" RESPONSE TO DATA UPLOAD================================================================\r\n"+str);
			}
			errormsg="";
			status="";
			str=str.Replace("\r\n","");
			//Debug.Write(str);
			if(str.Length>300) {
				throw new Exception("Unknown lengthy error message received.");
			}
			responseParams=str.Split('&');
			for(int i=0;i<responseParams.Length;i++){
				curParam=GetParam(responseParams[i]);
				switch(curParam){
					case "Status":
						status=GetParamValue(responseParams[i]);
						break;
					case "Error Message":
					case "ErrorMessage":
						errormsg=GetParamValue(responseParams[i]);
						break;
					case "Filename":
					case "Timestamp":
						break;
					case ""://errorMessage blank
						break;
					default:
						throw new Exception(str);//"Unexpected parameter: "+str);//curParam+"*");
				}
			}
			switch(status){
				case "0":
					//MessageBox.Show("Upload successful.");
					break;
				case "1":
					throw new Exception("Authentication failed. "+errormsg);
				case "2":
					throw new Exception("Cannot upload at this time. "+errormsg);
			}
		}

		private static string GetParam(string paramAndValue) {
			if(paramAndValue=="") {
				return "";
			}
			string[] pair=paramAndValue.Split('=');
			//if(pair.Length!=2){
			//	throw new Exception("Unexpected parameter from server: "+paramAndValue);
			return pair[0];
		}

		private static string GetParamValue(string paramAndValue) {
			if(paramAndValue=="") {
				return "";
			}
			string[] pair=paramAndValue.Split('=');
			//if(pair.Length!=2){
			//	throw new Exception("Unexpected parameter from server: "+paramAndValue);
			//}
			if(pair.Length==1) {
				return "";
			}
			return pair[1];
		}


	}

	public class EHG_Address {
		public string Address1;
		public string Address2;
		public string City;
		public string State;
		public string Zip;
		public string Phone;
		public string Source;
	}

}
