using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using CodeBase;
using DataConnectionBase;

namespace OpenDentBusiness {
	public class AccountModules {

		///<summary>Data that is needed to load the Account module.</summary>
		public class LoadData {
			[XmlIgnore]
			public DataSet DataSetMain;
			public Family Fam;
			public PatientNote PatNote;
			public PatField[] ArrPatFields;
			public List<InsSub> ListInsSubs;
			public List<InsPlan> ListInsPlans;
			public List<PatPlan> ListPatPlans;
			public List<Benefit> ListBenefits;
			public List<Claim> ListClaims;
			public List<ClaimProcHist> HistList;
			///<summary>All splits that are associated to some sort of unearned type. Includes splits for the entire family as well as splits for patients
			///outside of the family. E.g. splits for when a family member makes a payment for another patient outside of the family.</summary>
			public List<PaySplit> ListUnearnedSplits;
			public RepeatCharge[] ArrRepeatCharges;
			///<summary>Key: PatPlanNum, Value: The date of the last ortho claim for this plan.</summary>
			public SerializableDictionary<long,DateTime> DictDateLastOrthoClaims;
			public DateTime FirstOrthoProcDate;
			public List<FieldDefLink> ListFieldDefLinksAcct;
			public List<PatientLink> ListMergeLinks;
			public DiscountPlan DiscountPlan;
			public DiscountPlanSub DiscountPlanSub;

			[XmlElement(nameof(DataSetMain))]
			public string DataSetMainXml {
				get {
					if(DataSetMain==null) {
						return null;
					}
					return XmlConverter.DsToXml(DataSetMain);
				}
				set {
					if(value==null) {
						DataSetMain=null;
						return;
					}
					DataSetMain=XmlConverter.XmlToDs(value);
				}
			}
		}

		///<summary>If intermingled=true, the patnum of any family member will get entire family intermingled.</summary>
		public static LoadData GetAll(long patNum,DateTime fromDate,DateTime toDate,bool intermingled,bool showProcBreakdown,
			bool showPayNotes,bool showAdjNotes,bool doMakeSecLog,bool doGetOrtho) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<LoadData>(MethodBase.GetCurrentMethod(),patNum,fromDate,toDate,intermingled,showProcBreakdown,
					showPayNotes,showAdjNotes,doMakeSecLog,doGetOrtho);
			}
			Family fam=new Family();
			Patient pat=new Patient();
			LoadData retVal=new LoadData();
			retVal.DataSetMain=new DataSet();
			bool singlePatient=!intermingled;//so one or the other will be true
			decimal patientPayPlanDue=0;
			decimal dynamicPayPlanDue=0;
			decimal balanceForward=0;
			//Gets 3 tables: account(or account###,account###,etc), patient, payplan.
			DataSet dataSetAccount=new DataSet();
			//Done building the tree.
			//Get Patient and family fetching out of the way.  These would be called first if a part of the tree anyway.
			Logger.LogAction("Patients.GetFamily",LogPath.AccountModule,() => fam=Patients.GetFamily(patNum));
			if(intermingled){
				patNum=fam.ListPats[0].PatNum;//guarantor
			}
			Logger.LogAction("Family.GetPatient",LogPath.AccountModule,() => pat=fam.GetPatient(patNum));
			//We've gotten the patient and family objects, so now we can make a plethora of actions to run in parallel.
			#region Actions
			//Actions that have leaves
			Action refreshForFam=new Action(() => {
				Logger.LogAction("InsSubs.RefreshForFam",LogPath.AccountModule,() => retVal.ListInsSubs=InsSubs.RefreshForFam(fam));
				Logger.LogAction("InsPlans.RefreshSubList",LogPath.AccountModule,() => retVal.ListInsPlans=InsPlans.RefreshForSubList(retVal.ListInsSubs));
			});//InsSubs.RefreshForFam
			Action refreshOrGetFirstOrthoProcDate=new Action(() => {
				Logger.LogAction("PatientNotes.Refresh",LogPath.AccountModule,() => retVal.PatNote=PatientNotes.Refresh(pat.PatNum,pat.Guarantor));
				if(doGetOrtho) {
					Logger.LogAction("Procedures.GetFirstOrthoProcDate",LogPath.AccountModule,() => retVal.FirstOrthoProcDate=Procedures.GetFirstOrthoProcDate(retVal.PatNote));
				}
			});//PatientNotes.Refresh / Procedures.GetFirstOrthoProcDate
			Action refreshBenefits=new Action(() => {
				Logger.LogAction("Benefits.Refresh",LogPath.AccountModule,() => retVal.ListBenefits=Benefits.Refresh(retVal.ListPatPlans,retVal.ListInsSubs));
			});//Benefits.Refresh
			Action getAccount=new Action(() => {
				Logger.LogAction("Get Account",LogPath.AccountModule,() => dataSetAccount=GetAccount(patNum,fromDate,toDate,intermingled,singlePatient,0
				,showProcBreakdown,showPayNotes,false,showAdjNotes,false,pat,fam,out patientPayPlanDue,out dynamicPayPlanDue,out balanceForward));
				for(int i=0;i<dataSetAccount.Tables.Count;i++) {
					retVal.DataSetMain.Tables.Add(dataSetAccount.Tables[i].Copy());
				}
			});//GetAccount
			//Actions that are leaves
			Action getMergeLinks=new Action(() => {
				Logger.LogAction("PatientLinks.GetLinks",LogPath.AccountModule,() => 
					retVal.ListMergeLinks=PatientLinks.GetLinks(fam.GetPatNums(),PatientLinkType.Merge));
			});
			Action getPrePayForFam=new Action(() => {
				Logger.LogAction("PaySplits.GetPrepayForFam",LogPath.AccountModule,() => 
					retVal.ListUnearnedSplits=PaySplits.GetUnearnedForAccount(
						fam.GetPatNums().Union(Patients.GetAllFamilyPatNumsForSuperFam(new List<long> { pat.SuperFamily })).ToList()));
			});//PaySplits.GetPrepayForFam
			Action refreshRepeatCharges=new Action(() => {
				Logger.LogAction("RepeatCharges.Refresh",LogPath.AccountModule,() => retVal.ArrRepeatCharges=RepeatCharges.Refresh(pat.PatNum));
			});//RepeatCharges.Refresh
			Action getProgNotesCommLog=new Action(() => {
				Logger.LogAction("GetCommLog",LogPath.AccountModule,() => retVal.DataSetMain.Tables.Add(GetCommLog(pat,fam)));
			});//GetProgNotes / GetCommLog
			Action refreshPatFields=new Action(() => {
				Logger.LogAction("PatFields.Refresh",LogPath.AccountModule,() => retVal.ArrPatFields=PatFields.Refresh(pat.PatNum));
			});//PatFields.Refresh
			Action dateOrthoLastClaims=new Action(() => {
				if(doGetOrtho) {
					retVal.DictDateLastOrthoClaims=new SerializableDictionary<long,DateTime>();
					foreach(PatPlan patPlan in retVal.ListPatPlans) {
						InsPlan plan=new InsPlan();
						Logger.LogAction("InsPlans.GetPlan",LogPath.AccountModule,() => plan=InsPlans.GetPlan(InsSubs.GetSub(patPlan.InsSubNum,retVal.ListInsSubs).PlanNum,retVal.ListInsPlans));
						Logger.LogAction("DateLastOrthoClaims",LogPath.AccountModule,() => retVal.DictDateLastOrthoClaims.Add(patPlan.PatPlanNum,Claims.GetDateLastOrthoClaim(patPlan,plan.OrthoType)));
					}
				}
			});//InsPlans.GetPlans / DateOrthoLastClaims
			Action refreshClaims=new Action(() => {
				Logger.LogAction("Claims.Refresh",LogPath.AccountModule,() => retVal.ListClaims=Claims.Refresh(pat.PatNum));			
			});//Claims.Refresh
			Action claimProcsGetHistList=new Action(() => {
				Logger.LogAction("ClaimProcs.GetHistList",LogPath.AccountModule,() => retVal.HistList=ClaimProcs.GetHistList(pat.PatNum,retVal.ListBenefits,retVal.ListPatPlans
				,retVal.ListInsPlans,DateTime.Today,retVal.ListInsSubs));
			});//ClaimProcs.GetHistList
			Action getMisc=new Action(() => {
				Logger.LogAction("GetMisc",LogPath.AccountModule,() =>	
					retVal.DataSetMain.Tables.Add(GetMisc(fam,patNum,patientPayPlanDue,dynamicPayPlanDue,balanceForward,StmtType.NotSet,null)));
			});//GetMisc
			Action getFamily=new Action(() => {
				//GetFamily is called twice because we need to refresh the family data after running aging.
				Logger.LogAction("Patients.GetFamily",LogPath.AccountModule,() => retVal.Fam=Patients.GetFamily(patNum));//have to get family after dataset due to aging calc.
			});//Patients.GetFamily
			Action refreshPatPlans=new Action(() => {
				Logger.LogAction("PatPlans.Refresh",LogPath.AccountModule,() => {
					retVal.ListPatPlans=PatPlans.Refresh(pat.PatNum);
					if(!PatPlans.IsPatPlanListValid(retVal.ListPatPlans)) {//PatPlans had invalid references and need to be refreshed.
						retVal.ListPatPlans=PatPlans.Refresh(pat.PatNum);
					}
				});
			});//PatPlans.Refresh
			Action getFieldDefLinksForLocation=new Action(() => {
				Logger.LogAction("FieldDefLinks.GetForLocation",LogPath.AccountModule,() => retVal.ListFieldDefLinksAcct=FieldDefLinks.GetForLocation(FieldLocations.Account));
			});//FieldDefLinks.GetForLocation
			Action getDiscountPlan=new Action(() => {
				Logger.LogAction("DiscountPlans.GetForPats",LogPath.AccountModule,() => retVal.DiscountPlan=DiscountPlans.GetForPats(ListTools.FromSingle(patNum)).FirstOrDefault());
			});
			Action getDiscountPlanSub=new Action(() => {
				Logger.LogAction("DiscountPlanSubs.GetSubForPat",LogPath.AccountModule,() => retVal.DiscountPlanSub=DiscountPlanSubs.GetSubForPat(patNum));
			});
			#endregion Actions
			//Put each of our actions in sets in the order they must execute.  actionSetMisc is the only set where order doesn't matter.
			Action actionSetFamily=() => {
				getAccount();
				getMisc();
				getFamily();
			};
			Action actionSetClaims=() => {
				refreshForFam();
				refreshPatPlans();
				dateOrthoLastClaims();
				refreshBenefits();
				claimProcsGetHistList();
			};
			Action actionSetMisc=() => {
				getProgNotesCommLog();
				refreshRepeatCharges();
				refreshPatFields();
				refreshClaims();
				getPrePayForFam();
				getMergeLinks();
				getFieldDefLinksForLocation();
				getDiscountPlan();
				getDiscountPlanSub();
				if(doMakeSecLog) {
					SecurityLogs.MakeLogEntry(Permissions.AccountModule,patNum,"");
				}
			};
			//Limiting to 4 threads so we don't get a "Too many connections" error from the Db.
			//Allow to run until all thread complete; no timeout.
			ODThread.RunParallel(new List<Action> { actionSetFamily,refreshOrGetFirstOrthoProcDate,actionSetClaims,actionSetMisc},Timeout.Infinite,4);
			return retVal;
		}

		///<summary>If intermingled=true the patnum of any family member will get entire family intermingled.  toDate should not be Max, or PayPlan amort will include too many charges.  The 10 days will not be added to toDate until creating the actual amortization schedule.</summary>
		public static DataSet GetStatementDataSet(Statement stmt,bool isComputeAging=true,bool doIncludePatLName=false){
			//long patNum,bool singlePatient,DateTime fromDate,DateTime toDate,bool intermingled) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetDS(MethodBase.GetCurrentMethod(),stmt,isComputeAging,doIncludePatLName); 
			}
			long patNum;
			if(stmt.SuperFamily!=0) {//Superfamily statement
				patNum=stmt.SuperFamily;
			}
			else {
				patNum=stmt.PatNum;
			}
			if(!stmt.SinglePatient && stmt.Intermingled) {
				patNum=Patients.GetLim(patNum).Guarantor;
			}
			//Gets 3 tables: account(or account###,account###,etc), patient, payplan.
			DataSet retVal;
			if(stmt.SuperFamily!=0) {//Superfamily statement, Intermingled and SinglePatient should always be false
				retVal=GetSuperFamAccount(stmt,isComputeAging,doIncludePatLName,doShowHiddenPaySplits:stmt.IsReceipt);//GetSuperFamAccount will use stmt.SuperFamily as the superhead
			}
			else {
				retVal=GetAccount(patNum,stmt,isComputeAging,doIncludePatLName,doShowHiddenPaySplits:stmt.IsReceipt);
			}
			return retVal;
		}

		private static DataTable GetPayPlanAmortTable(long payPlanNum) {
			//No need to check RemotingRole; private static.
			DataConnection dcon=new DataConnection();
			DataTable table=new DataTable("payplanamort");
			DataRow row;
			SetTableColumns(table);
			List<DataRow> rows=new List<DataRow>();
			string command="SELECT ChargeDate,Interest,Note,PayPlanChargeNum,Principal,ProvNum,PatNum,SecDateTEntry "
				+"FROM payplancharge "
				+"WHERE PayPlanNum="+POut.Long(payPlanNum)+" AND ChargeType="+POut.Int((int)PayPlanChargeType.Debit);//for v1, debits are the only ChargeType
			DataTable rawCharge=dcon.GetTable(command);
			DateTime dateT;
			decimal principal;
			decimal interest;
			decimal total;
			for(int i=0;i<rawCharge.Rows.Count;i++){
				interest=PIn.Decimal(rawCharge.Rows[i]["Interest"].ToString());
				principal=PIn.Decimal(rawCharge.Rows[i]["Principal"].ToString());
				total=principal+interest;
				row=table.NewRow();
				row["AdjNum"]=0;
				row["balance"]="";//fill this later
				row["balanceDouble"]=0;//fill this later
				row["chargesDouble"]=total;
				row["charges"]=((decimal)row["chargesDouble"]).ToString("n");
				row["ClaimNum"]=0;
				row["ClaimPaymentNum"]="0";
				row["colorText"]=Color.Black.ToArgb().ToString();
				row["creditsDouble"]=0;
				row["credits"]="";//((double)row["creditsDouble"]).ToString("n");
				dateT=PIn.DateT(rawCharge.Rows[i]["ChargeDate"].ToString());
				row["DateTime"]=dateT;
				row["date"]=dateT.ToShortDateString();
				row["dateTimeSort"]=PIn.DateT(rawCharge.Rows[i]["SecDateTEntry"].ToString());//SecDateTEntry will be used for sorting if RandomKeys is enabled
				row["description"]="";//"Princ: "+principal.ToString("n")+
				if(interest!=0){
					row["description"]+="Interest: "+interest.ToString("n");//+"Princ: "+principal.ToString("n")+;
				}
				if(rawCharge.Rows[i]["Note"].ToString()!=""){
					if(row["description"].ToString()!=""){
						row["description"]+="  ";	
					}
					row["description"]+=rawCharge.Rows[i]["Note"].ToString();
				}
				//row["extraDetail"]="";
				row["patient"]="";
				row["PatNum"]=PIn.Long(rawCharge.Rows[i]["PatNum"].ToString());
				row["PayNum"]=0;
				row["PayPlanNum"]=0;
				row["PayPlanChargeNum"]=rawCharge.Rows[i]["PayPlanChargeNum"].ToString();
				row["ProcCode"]=Lans.g("AccountModule","PPcharge");
				row["ProcNum"]="0";
				row["procsOnObj"]="";
				row["prov"]=Providers.GetAbbr(PIn.Long(rawCharge.Rows[i]["ProvNum"].ToString()));
				row["signed"]="";
				row["StatementNum"]=0;
				row["tth"]="";
				rows.Add(row);
			}
			long payPlanPlanNum=0;
			PayPlan payPlanCur=PayPlans.GetOne(payPlanNum);
			if(payPlanCur!=null) {
				payPlanPlanNum=payPlanCur.PlanNum;
			}
			if(payPlanPlanNum==0) {//not a insurance payment plan
				//Paysplits
				command="SELECT CheckNum,DatePay,paysplit.PatNum,PayAmt,paysplit.PayNum,PayPlanNum,PayType,ProcDate,ProvNum,SplitAmt,paysplit.SecDateTEdit "
					+"FROM paysplit "
					+"LEFT JOIN payment ON paysplit.PayNum=payment.PayNum "
					+"WHERE paysplit.PayPlanNum="+POut.Long(payPlanNum);
			}
			else {//insurance payment plan
				//Ins Payments
				//MAX functions added to preserve behavior in Oracle.
				command="SELECT ClaimNum,MAX(CheckNum) CheckNum,DateCP,MAX(PatNum) PatNum,MAX(CheckAmt) CheckAmt,claimproc.ClaimPaymentNum,"
					+"MAX(PayPlanNum) PayPlanNum,MAX(PayType) PayType,MAX(ProcDate) ProcDate,SUM(InsPayAmt) InsPayAmt,"
					+"(SELECT ProvTreat FROM claim WHERE claimproc.ClaimNum=claim.ClaimNum) ProvNum,MAX(claimproc.SecDateTEdit) SecDateTEdit "
					+"FROM claimproc "
					+"LEFT JOIN claimpayment ON claimproc.ClaimPaymentNum=claimpayment.ClaimPaymentNum "
					+"WHERE PayPlanNum="+POut.Long(payPlanNum)+" "
					+"AND (Status=1 OR Status=4 OR Status=5) "//received or supplemental or capclaim
					+"GROUP BY ClaimNum,DateCP,claimproc.ClaimPaymentNum";
			}
			DataTable rawPay=dcon.GetTable(command);
			decimal payamt;
			decimal amt;
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.AccountColors);
			for(int i=0;i<rawPay.Rows.Count;i++){
				row=table.NewRow();
				row["AdjNum"]=0;
				row["balance"]="";//fill this later
				row["balanceDouble"]=0;//fill this later
				row["chargesDouble"]=0;
				row["charges"]="";
				row["ClaimNum"]=0;
				row["ClaimPaymentNum"]="0";
				row["colorText"]=listDefs[3].ItemColor.ToArgb().ToString();
				if(payPlanPlanNum!=0) {//ins payments
					row["ClaimNum"]=PIn.Long(rawPay.Rows[i]["ClaimNum"].ToString());
					row["ClaimPaymentNum"]=rawPay.Rows[i]["ClaimPaymentNum"].ToString();
					row["colorText"]=listDefs[7].ItemColor.ToArgb().ToString();
				}
				if(payPlanPlanNum==0) {
					amt=PIn.Decimal(rawPay.Rows[i]["SplitAmt"].ToString());
				}
				else {
					amt=PIn.Decimal(rawPay.Rows[i]["InsPayAmt"].ToString());
				}
				row["creditsDouble"]=amt;
				row["credits"]=((decimal)row["creditsDouble"]).ToString("n");
				if(payPlanPlanNum==0) {
					dateT=PIn.DateT(rawPay.Rows[i]["DatePay"].ToString());
				}
				else {
					dateT=PIn.DateT(rawPay.Rows[i]["DateCP"].ToString());//this may be changed to ProcDate in the future
				}
				row["DateTime"]=dateT;
				row["date"]=dateT.ToShortDateString();
				row["dateTimeSort"]=PIn.DateT(rawPay.Rows[i]["SecDateTEdit"].ToString());//SecDateTEdit will be used for sorting if RandomKeys is enabled
				if(payPlanPlanNum==0) {
					row["description"]=Defs.GetName(DefCat.PaymentTypes,PIn.Long(rawPay.Rows[i]["PayType"].ToString()));
					payamt=PIn.Decimal(rawPay.Rows[i]["PayAmt"].ToString());
				}
				else {
					row["description"]=Defs.GetName(DefCat.InsurancePaymentType,PIn.Long(rawPay.Rows[i]["PayType"].ToString()));
					payamt=PIn.Decimal(rawPay.Rows[i]["CheckAmt"].ToString());
				}
				if(rawPay.Rows[i]["CheckNum"].ToString()!=""){
					row["description"]+=" #"+rawPay.Rows[i]["CheckNum"].ToString();
				}
				if(payPlanPlanNum!=0 && rawPay.Rows[i]["ClaimPaymentNum"].ToString()=="0") {//attached to claim but no check (claimpayment) created
					row["description"]=Lans.g("AccountModule","No Insurance Check Created");
				}
				else {
					row["description"]+=" "+payamt.ToString("c");
					if(payamt!=amt){
						row["description"]+=" "+Lans.g("ContrAccount","(split)");
					}
				}
				//we might use DatePay/DateCP here to add to description
				//row["extraDetail"]="";
				row["patient"]="";
				row["PatNum"]=PIn.Long(rawPay.Rows[i]["PatNum"].ToString());
				if(payPlanPlanNum==0) {
					row["PayNum"]=PIn.Long(rawPay.Rows[i]["PayNum"].ToString());
				}
				else {
					row["PayNum"]=0;
				}
				row["PayPlanNum"]=0;
				row["PayPlanChargeNum"]="0";
				if(payPlanPlanNum==0) {
					row["ProcCode"]=Lans.g("AccountModule","Pay");
				}
				else {
					row["ProcCode"]=Lans.g("AccountModule","InsPay");
				}
				row["ProcNum"]="0";
				row["procsOnObj"]="";
				row["prov"]=Providers.GetAbbr(PIn.Long(rawPay.Rows[i]["ProvNum"].ToString()));
				row["signed"]="";
				row["StatementNum"]=0;
				row["tth"]="";
				rows.Add(row);
			}
			//Sorting-----------------------------------------------------------------------------------------
			rows.Sort(new AccountLineComparer());
			//Add # indicators to charges
			int num=1;
			for(int i=0;i<rows.Count;i++) {
				if(rows[i]["PayPlanChargeNum"].ToString()=="0"){//if not a payplancharge
					continue;
				}
				rows[i]["description"]="#"+num.ToString()+" "+rows[i]["description"].ToString();
				num++;
			}
			//Compute balances-------------------------------------------------------------------------------------
			decimal bal=0;
			for(int i=0;i<rows.Count;i++) {
				bal+=(decimal)rows[i]["chargesDouble"];
				bal-=(decimal)rows[i]["creditsDouble"];
				rows[i]["balanceDouble"]=bal;
				//if(rows[i]["ClaimPaymentNum"].ToString()=="0" && rows[i]["ClaimNum"].ToString()!="0"){//claims
				//	rows[i]["balance"]="";
				//}
				//else{
					rows[i]["balance"]=bal.ToString("n");
				//}
			}
			for(int i=0;i<rows.Count;i++) {
				table.Rows.Add(rows[i]);
			}
			return table;
		}

		public static DataTable GetCommLog(Patient pat,Family fam) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),pat,fam);
			}
			DataConnection dcon=new DataConnection();
			DataTable table=new DataTable("Commlog");
			DataRow row;
			//columns that start with lowercase are altered for display rather than being raw data.
			table.Columns.Add("CommDateTime",typeof(DateTime));
			table.Columns.Add("commDate");
			table.Columns.Add("commTime");
			table.Columns.Add("CommlogNum");
			table.Columns.Add("CommSource");
			table.Columns.Add("commName");
			table.Columns.Add("commType");
			table.Columns.Add("colorText");
			table.Columns.Add("EmailMessageNum");
			table.Columns.Add("FormPatNum");
			table.Columns.Add("mode");
			table.Columns.Add("Note");
			table.Columns.Add("patName");
			table.Columns.Add("PatNum");
			table.Columns.Add("SheetNum");
			table.Columns.Add("EmailMessageHideIn");
			table.Columns.Add("WebChatSessionNum");
			//but we won't actually fill this table with rows until the very end.  It's more useful to use a List<> for now.
			List<DataRow> rows=new List<DataRow>();
			string familyPatNums=POut.Long(pat.PatNum);//just in case, fam should never be null so this will be replaced by the patnums from fam.ListPats
			Dictionary<string,string> dictPatFNames=new Dictionary<string,string>() { { pat.PatNum.ToString(),pat.FName } };
			if(fam!=null && fam.ListPats!=null && fam.ListPats.Length>0) {
				familyPatNums=string.Join(",",fam.ListPats.Select(x => POut.Long(x.PatNum)));
				dictPatFNames=fam.ListPats.ToDictionary(x => x.PatNum.ToString(),x => x.FName);
			}
			#region commlog
			List<Def> listCommLogTypeDefs=Defs.GetDefsForCategory(DefCat.CommLogTypes);
			long podiumProgramNum=Programs.GetCur(ProgramName.Podium).ProgramNum;
			bool showPodiumCommlogs=PIn.Bool(ProgramProperties.GetPropVal(podiumProgramNum,Podium.PropertyDescs.ShowCommlogsInChartAndAccount));
			string andNotPodiumCommlog=" AND (commlog.CommSource!="+POut.Int((int)CommItemSource.ProgramLink)+" "
				+"OR commlog.ProgramNum!="+POut.Long(podiumProgramNum)+")";
			string command="SELECT CommDateTime,CommType,Mode_,SentOrReceived,Note,CommlogNum,commlog.PatNum,CommSource "
				+"FROM commlog "
				+"WHERE PatNum IN ("+familyPatNums+")"
				+(showPodiumCommlogs?"":andNotPodiumCommlog);//Rows are ordered at the end
			DataTable rawComm=dcon.GetTable(command);
			DateTime dateT;
			for(int i=0;i<rawComm.Rows.Count;i++) {
				DataRow rowCur=rawComm.Rows[i];
				row=table.NewRow();
				dateT=PIn.DateT(rowCur["CommDateTime"].ToString());
				long commTypeDefNum=PIn.Long(rowCur["CommType"].ToString());
				Def commlogType=listCommLogTypeDefs.FirstOrDefault(x => x.DefNum==commTypeDefNum);
				//If Def exists and not an empty color use that color. Otherwise, leave row blank.
				if(commlogType!=null && commlogType.ItemColor.ToArgb()!=Color.Empty.ToArgb()) {
					row["colorText"]=commlogType.ItemColor.ToArgb().ToString();
				}
				row["CommDateTime"]=dateT;
				row["commDate"]=dateT.ToString(Lans.GetShortDateTimeFormat());
				row["commTime"]="";
				if(dateT.TimeOfDay!=TimeSpan.Zero) {
					row["commTime"]=dateT.ToString("h:mm")+dateT.ToString("%t").ToLower();
				}
				row["CommlogNum"]=rowCur["CommlogNum"].ToString();
				row["CommSource"]=rowCur["CommSource"].ToString();
				Def commTypeDef=Defs.GetDef(DefCat.CommLogTypes,commTypeDefNum,listCommLogTypeDefs);
				if(commTypeDef!=null) {
					row["commName"]=commTypeDef.ItemName;
					row["commType"]=commTypeDef.ItemValue;
				}
				else {
					row["commName"]="";
					row["commType"]="";
				}
				row["EmailMessageNum"]="0";
				row["FormPatNum"]="0";
				row["mode"]="";
				if(rowCur["Mode_"].ToString()!="0"){//anything except none
					row["mode"]=Lans.g("enumCommItemMode",((CommItemMode)PIn.Long(rowCur["Mode_"].ToString())).ToString());
				}
				row["Note"]=rowCur["Note"].ToString();
				string patName;
				if(!dictPatFNames.TryGetValue(rowCur["PatNum"].ToString(),out patName)) {
					patName="";
				}
				row["patName"]=patName;
				row["PatNum"]=rowCur["PatNum"].ToString();
				row["SheetNum"]="0";
				row["EmailMessageHideIn"]="0";
				row["WebChatSessionNum"]="0";
				rows.Add(row);
			}
			#endregion commlog
			#region webchatnote - ODHQ Only
			if(PrefC.IsODHQ) {
				//connect to the webchat db for this query
				List<WebChatSession> listWebChatSessions=new List<WebChatSession>();
				WebChatMisc.DbAction(delegate() {
					command=@"SELECT *
									FROM webchatsession
									WHERE webchatsession.PatNum IN ("+familyPatNums+@") 
									ORDER BY webchatsession.DateTcreated";
					listWebChatSessions=Crud.WebChatSessionCrud.SelectMany(command);
				});
				//Since this is an internal database on another server, be defensive to allow chart access if the other server is down for any reason.
				if(listWebChatSessions!=null) {//Will be null if the query failed or the server is unavailable or any reason.
					foreach(WebChatSession session in listWebChatSessions) {
						row=table.NewRow();
						dateT=PIn.DateT(session.DateTcreated.ToString());//use DateTend since DateTcreated is a TimeStamp and will be updated upon editing.
						row["CommDateTime"]=dateT;
						row["commDate"]=dateT.ToString(Lans.GetShortDateTimeFormat());
						row["commTime"]="";
						if(dateT.TimeOfDay!=TimeSpan.Zero) {
							row["commTime"]=dateT.ToString("h:mm")+dateT.ToString("%t").ToLower();
						}
						row["CommlogNum"]="0";
						row["CommSource"]="";
						row["commType"]="Web Chat Session - "+session.WebChatSessionNum.ToString();
						row["EmailMessageNum"]="0";
						row["FormPatNum"]="0";
						row["mode"]="";
						row["Note"]=session.Note;
						string patName;
						if(!dictPatFNames.TryGetValue(session.PatNum.ToString(),out patName)) {
							patName="";
						}
						row["patName"]=patName;
						row["PatNum"]=session.PatNum.ToString();
						row["SheetNum"]="0";
						row["WebChatSessionNum"]=session.WebChatSessionNum.ToString();
						row["EmailMessageHideIn"]="0";
						rows.Add(row);
					}
				}
			}
			#endregion webchatnote
			#region emailmessage
			List<EmailSentOrReceived> listAckTypes=EmailMessages.GetUnsentTypes(EmailPlatform.Ack).Concat(EmailMessages.GetSentTypes(EmailPlatform.Ack)).ToList();
			string ackTypesStr=string.Join(",",listAckTypes.Select(x => POut.Int((int)x)));
			//Get all emails for the entire family.  If a user creates an email that is attached to a patient, it will show up here for everyone.
			command="SELECT emailmessage.MsgDateTime,emailmessage.SentOrReceived,emailmessage.Subject,emailmessage.EmailMessageNum, "
				+"emailmessage.PatNum,emailmessage.RecipientAddress,emailmessage.HideIn "
				+"FROM emailmessage "
				+"WHERE emailmessage.PatNum IN ("+familyPatNums+") "
				+"AND emailmessage.SentOrReceived NOT IN ("+ackTypesStr+") ";//Do not show Direct message acknowledgements. Rows are ordered at the end
			DataTable rawEmail=dcon.GetTable(command);
			string txt;
			for(int i=0;i<rawEmail.Rows.Count;i++) {
				DataRow rowCur=rawEmail.Rows[i];
				row=table.NewRow();
				dateT=PIn.DateT(rowCur["MsgDateTime"].ToString());
				row["CommDateTime"]=dateT;
				row["commDate"]=dateT.ToShortDateString();
				if(dateT.TimeOfDay!=TimeSpan.Zero){
					row["commTime"]=dateT.ToString("h:mm")+dateT.ToString("%t").ToLower();
				}
				row["CommlogNum"]="0";
				row["CommSource"]="";
				row["EmailMessageNum"]=rowCur["EmailMessageNum"].ToString();
				row["FormPatNum"]="0";
				row["mode"]=Lans.g("enumCommItemMode",CommItemMode.Email.ToString());
				txt="";
				if(rowCur["SentOrReceived"].ToString()=="0") {
					txt="("+Lans.g("AccountModule","Unsent")+") ";
				}
				row["Note"]=txt+rowCur["Subject"].ToString();
				string patName;
				if(!dictPatFNames.TryGetValue(rowCur["PatNum"].ToString(),out patName)) {
					patName="";
				}
				row["patName"]=patName;
				row["PatNum"]=rowCur["PatNum"].ToString();
				row["EmailMessageHideIn"]=rawEmail.Rows[i]["HideIn"].ToString();
				row["SheetNum"]="0";
				row["WebChatSessionNum"]="0";
				rows.Add(row);
			}
			#endregion emailmessage
			#region formpat
			command="SELECT FormDateTime,FormPatNum "
				+"FROM formpat WHERE PatNum ="+POut.Long(pat.PatNum);//Rows are ordered at the end
			DataTable rawForm=dcon.GetTable(command);
			for(int i=0;i<rawForm.Rows.Count;i++) {
				DataRow rowCur=rawForm.Rows[i];
				row=table.NewRow();
				dateT=PIn.DateT(rowCur["FormDateTime"].ToString());
				row["CommDateTime"]=dateT;
				row["commDate"]=dateT.ToShortDateString();
				if(dateT.TimeOfDay!=TimeSpan.Zero) {
					row["commTime"]=dateT.ToString("h:mm")+dateT.ToString("%t").ToLower();
				}
				row["CommlogNum"]="0";
				row["CommSource"]="";
				row["commType"]=Lans.g("AccountModule","Questionnaire");
				row["EmailMessageNum"]="0";
				row["FormPatNum"]=rowCur["FormPatNum"].ToString();
				row["mode"]="";
				row["Note"]="";
				row["patName"]="";
				row["PatNum"]="0";//PatNum is not selected in the query, the patName column will be blank which causes it to show in the commlog grid.
				row["SheetNum"]="0";
				row["EmailMessageHideIn"]="0";
				row["WebChatSessionNum"]="0";
				rows.Add(row);
			}
			#endregion formpat
			#region sheet
			command="SELECT DateTimeSheet,SheetNum,SheetType,Description,PatNum "
				+"FROM sheet "
				+"WHERE IsDeleted=0 "//Don't show deleted sheets in the Account module Communications Log section.
				+"AND SheetType!="+POut.Long((int)SheetTypeEnum.Rx)+" "//rx are only accesssible from within Rx edit window.
				+"AND PatNum IN ("+familyPatNums+")";//Rows are ordered at the end
			DataTable rawSheet=dcon.GetTable(command);
			for(int i=0;i<rawSheet.Rows.Count;i++) {
				DataRow rowCur=rawSheet.Rows[i];
				row=table.NewRow();
				dateT=PIn.DateT(rowCur["DateTimeSheet"].ToString());
				row["CommDateTime"]=dateT;
				row["commDate"]=dateT.ToShortDateString();
				if(dateT.TimeOfDay!=TimeSpan.Zero) {
					row["commTime"]=dateT.ToString("h:mm")+dateT.ToString("%t").ToLower();
				}
				row["CommlogNum"]="0";
				row["CommSource"]="";
				row["commType"]=Lans.g("AccountModule","Sheet");//
				row["EmailMessageNum"]="0";
				row["FormPatNum"]="0";
				row["mode"]="";
				row["Note"]=rowCur["Description"].ToString();
				string patName;
				if(!dictPatFNames.TryGetValue(rowCur["PatNum"].ToString(),out patName)) {
					patName="";
				}
				row["patName"]=patName;
				row["PatNum"]=rowCur["PatNum"].ToString();
				row["SheetNum"]=rowCur["SheetNum"].ToString();
				row["EmailMessageHideIn"]="0";
				row["WebChatSessionNum"]="0";
				rows.Add(row);
			}
			#endregion sheet
			rows.ForEach(x => table.Rows.Add(x));
			DataView view = table.DefaultView;
			view.Sort = "CommDateTime";
			table = view.ToTable();
			return table;
		}

		///<summary>Adds columns to the datatable needed for the account module Dataset. Column changes made should consider SheetUtil.GetTable_StatementMain(...).</summary>
		private static void SetTableColumns(DataTable table){
			//No need to check RemotingRole; no call to db.
			//columns that start with lowercase are altered for display rather than being raw data.
			table.Columns.Add("AdjNum",typeof(long));
			table.Columns.Add("AbbrDesc");
			table.Columns.Add("balance");
			table.Columns.Add("balanceDouble",typeof(decimal));
			table.Columns.Add("charges");
			table.Columns.Add("chargesDouble",typeof(decimal));
			table.Columns.Add("ClaimNum",typeof(long));
			table.Columns.Add("ClaimPaymentNum");//if this is set, also set ClaimNum.  Set to either 0 or 1, not ClaimPaymentNum
			table.Columns.Add("clinic");
			table.Columns.Add("ClinicNum");
			table.Columns.Add("colorText");
			table.Columns.Add("credits");
			table.Columns.Add("creditsDouble",typeof(decimal));
			table.Columns.Add("date");
			table.Columns.Add("DateTime",typeof(DateTime));
			table.Columns.Add("dateTimeSort",typeof(DateTime));
			table.Columns.Add("description");
			//table.Columns.Add("extraDetail");
			table.Columns.Add("InvoiceNum"); //statementNum for procedures attached to invoices
			table.Columns.Add("IsTransfer");//notes when a claimproc is from a claimproc transfer
			table.Columns.Add("patient");
			table.Columns.Add("PatNum",typeof(long));
			table.Columns.Add("paymentsOnObj");
			table.Columns.Add("PayNum",typeof(long));//even though we only show split objects
			table.Columns.Add("PayPlanNum",typeof(long));
			table.Columns.Add("PayPlanChargeNum");
			table.Columns.Add("ProcCode");
			table.Columns.Add("ProcNum");
			table.Columns.Add("ProcNumLab");
			table.Columns.Add("procsOnObj");//for a claim or payment, the ProcNums, comma delimited.
			table.Columns.Add("adjustsOnObj");//for a payment, the AdjNums, comma delimited.
			table.Columns.Add("prov");
			table.Columns.Add("signed");
			table.Columns.Add("StatementNum",typeof(long));
			table.Columns.Add("ToothNum");
			table.Columns.Add("ToothRange");
			table.Columns.Add("tth");
			table.Columns.Add("SuperFamily");
		}
		
		///<summary>Returns a data set that is designed for a super family statement.
		///This means that GetAccount will be run for every guarantor that HasSuperBilling within the super family.
		///If separatePayPlan is true, then all payplan charges will be moved to the payplan table instead of being in the account table.</summary>
		public static DataSet GetSuperFamAccount(Statement stmtCur,bool isComputeAging=true,bool doIncludePatLName=false
			,bool doShowHiddenPaySplits=false,bool doExcludeTxfrs=false)
		{
			//This method does not call the database directly but still requires a remoting role check because it calls a method that uses out variables.
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetDS(MethodBase.GetCurrentMethod(),stmtCur,isComputeAging,doIncludePatLName,doShowHiddenPaySplits,doExcludeTxfrs);
			}
			DataSet retVal=new DataSet();
			List<Patient> listSuperFamilyGuars=new List<Patient>();
			if(stmtCur.IsInvoice) {
				//Just add the super family head to the list of patients to include for the super statement.
				listSuperFamilyGuars.Add(Patients.GetPat(stmtCur.SuperFamily));
			}
			else {//Regular super family statement.
				listSuperFamilyGuars=Patients.GetSuperFamilyGuarantors(stmtCur.SuperFamily);
			}
			bool showProcBreakdown=false;
			if(!stmtCur.IsInvoice) {
				showProcBreakdown=PrefC.GetBool(PrefName.StatementShowProcBreakdown);
			}
			List<long> listPayPlanNums=new List<long>();
			foreach(Patient guarantor in listSuperFamilyGuars) {
				if(!guarantor.HasSuperBilling) {
					continue;
				}
				//Add each family account to the data set that is included in superfamily billing.
				Family fam=Patients.GetFamily(guarantor.PatNum);
				decimal patientPayPlanDue=0;
				decimal dynamicPayPlanDue=0;
				decimal balanceForward=0;
				DataSet account=GetAccount(guarantor.PatNum,stmtCur.DateRangeFrom,stmtCur.DateRangeTo,true,false,stmtCur.StatementNum,showProcBreakdown,
					PrefC.GetBool(PrefName.StatementShowNotes),stmtCur.IsInvoice,PrefC.GetBool(PrefName.StatementShowAdjNotes),true,guarantor,fam,
					out patientPayPlanDue,out dynamicPayPlanDue,out balanceForward,stmtCur,isComputeAging,doIncludePatLName,listPayPlanNums,
					doShowHiddenPaySplits:doShowHiddenPaySplits,doExcludeTxfrs:doExcludeTxfrs);
				//Setting the PatNum for all rows to the guarantor so that each family will be interminged in one grid. 
				account.Tables["account"].Rows.Cast<DataRow>().ToList().ForEach(x => x["PatNum"]=guarantor.PatNum);
				account.Tables.Add(GetApptTable(fam,false,guarantor.PatNum));
				account.Tables.Add(GetMisc(fam,guarantor.PatNum,patientPayPlanDue,dynamicPayPlanDue,balanceForward,stmtCur.StatementType,account));
				listPayPlanNums=listPayPlanNums.Union(account.Tables["payplan"].Select().Select(x => PIn.Long(x["PayPlanNum"].ToString()))
					.Where(x => x > 0)).ToList();
				retVal.Merge(account);//This works for the purposes we need it for.  Sheets framework auto-splits entries by patnum.
			}
			//Sort rows in table by cloning table, sorting rows, then re-adding table to DataSet.
			List<DataRow> listRows=retVal.Tables["account"].Rows.OfType<DataRow>().ToList();
			//Sort the data rows first by PatNum then by date.
			//This will potentially change the order of rows to where the balance column does not make sense.
			//Recalculate balances after the sort runs.
			listRows.Sort(SortRowsForStatmentPrinting);
			decimal bal=0;
			string patNumPrev="0";
			foreach(DataRow row in listRows) {
				if(row["PatNum"].ToString()!=patNumPrev) {
					bal=0;
					patNumPrev=row["PatNum"].ToString();
					if(row["description"].ToString()==Lans.g("AccountModule","Balance Forward")) {
						bal=(decimal)row["balanceDouble"];
					}
				}
				bal+=(decimal)row["chargesDouble"];
				bal-=(decimal)row["creditsDouble"];
				row["balanceDouble"]=bal;
				if(row["ClaimPaymentNum"].ToString()=="0" && row["ClaimNum"].ToString()!="0"){//claims
					row["balance"]="";
				}
				else if(row["StatementNum"].ToString()=="0"){
					row["balance"]=bal.ToString("n");
				}
			}
			List<DataRow> listPayPlanRows=retVal.Tables["payplan"].Rows.OfType<DataRow>()
				.OrderBy(x => x["PatNum"].ToString()).ThenBy(x => PIn.DateT(x["DateTime"].ToString())).ToList();
			decimal payplanBal=0;
			foreach(DataRow row in listPayPlanRows) {
				payplanBal+=(decimal)row["chargesDouble"];
				payplanBal-=(decimal)row["creditsDouble"];
				if(row["PayNum"].ToString()=="0" && row["PayPlanChargeNum"].ToString()=="0") {//this is the payplan description row
					row["balanceDouble"]=0;
					row["balance"]="";
				}
				else {
					row["balanceDouble"]=payplanBal;
					row["balance"]=payplanBal.ToString("n");
				}
			}
			DataTable accountSorted=retVal.Tables["account"].Clone();//Easy way to copy the columns.
			listRows.ForEach(x=>accountSorted.Rows.Add(x.ItemArray));
			retVal.Tables.Remove(retVal.Tables["account"]);
			retVal.Tables.Add(accountSorted);
			DataTable payplanSorted=retVal.Tables["payplan"].Clone();//Easy way to copy the columns.
			listPayPlanRows.ForEach(x => payplanSorted.Rows.Add(x.ItemArray));
			retVal.Tables.Remove(retVal.Tables["payplan"]);
			retVal.Tables.Add(payplanSorted);
			return retVal;
		}

		///<summary>Also gets the patient table, which has one row for each family member. Also currently runs aging.  Also gets payplan table.  
		///If stmt.StatementNum is not zero, then it's for a statement, and the resulting payplan table looks totally different.  
		///If stmt.IsInvoice or stmt.StatementType==StmtType.LimitedStatement, this does some extra filtering.</summary>
		public static DataSet GetAccount(long patNum,Statement stmt,bool isComputeAging=true,bool doIncludePatLName=false
			,bool doShowHiddenPaySplits=false,bool doExcludeTxfrs=false)
		{
			//This method does not call the database directly but still requires a remoting role check because it calls a method that uses out variables.
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetDS(MethodBase.GetCurrentMethod(),patNum,stmt,isComputeAging,doIncludePatLName,doShowHiddenPaySplits,doExcludeTxfrs);
			}
			bool showProcBreakdown=false;
			if(!stmt.IsInvoice) {
				showProcBreakdown=PrefC.GetBool(PrefName.StatementShowProcBreakdown);
			}
			Family fam=Patients.GetFamily(patNum);
			Patient pat=fam.GetPatient(patNum);
			decimal patientPayPlanDue=0;
			decimal dynamicPayPlanDue=0;
			decimal balanceForward=0;
			DataSet retVal=GetAccount(patNum,stmt.DateRangeFrom,stmt.DateRangeTo,stmt.Intermingled,stmt.SinglePatient,stmt.StatementNum,showProcBreakdown,
				PrefC.GetBool(PrefName.StatementShowNotes),stmt.IsInvoice,PrefC.GetBool(PrefName.StatementShowAdjNotes),true,pat,fam,out patientPayPlanDue,
				out dynamicPayPlanDue,out balanceForward,stmt,isComputeAging,doIncludePatLName,doShowHiddenPaySplits:doShowHiddenPaySplits,doExcludeTxfrs:doExcludeTxfrs);
			retVal.Tables.Add(GetApptTable(fam,stmt.SinglePatient,patNum));
			retVal.Tables.Add(GetMisc(fam,patNum,patientPayPlanDue,dynamicPayPlanDue,balanceForward,stmt.StatementType,retVal));//table=misc; Just holds some info we can't find anywhere else.
			return retVal;
		}

		///<summary>Also gets the patient table, which has one row for each family member. Also currently runs aging.  Also gets payplan table.  
		///If StatementNum is not zero, then it's for a statement, and the resulting payplan table looks totally different.  
		///If IsInvoice or statementType==StmtType.LimitedStatement, this does some extra filtering.
		///This method cannot be called from the Middle Tier as long as it uses out parameters.
		///If !isComputeAging (ONLY if AgingIsEnterprise and printing/sending statements), we assume that aging has been run for all pats for the current
		///date and doesn't need to run again for this fam.  Used so aging doesn't run for each statement after we just ran it for all patients when the
		///statement list was generated.  We use DateLastAging to determine if computing aging is necessary.</summary>
		private static DataSet GetAccount(long patNum,DateTime fromDate,DateTime toDate,bool intermingled,bool singlePatient,long statementNum
			,bool showProcBreakdown,bool showPayNotes,bool isInvoice,bool showAdjNotes,bool isForStatementPrinting
			,Patient pat,Family fam,out decimal patientPayPlanDue,out decimal dynamicPayPlanDue,out decimal balanceForward,Statement stmt=null
			,bool isComputeAging=true,bool doIncludePatLName=false,List<long> listPayPlanNumsExclude=null,bool doShowHiddenPaySplits=false,bool doExcludeTxfrs=false) 
		{
			//No need to check RemotingRole; this method contains out parameters.
			if(stmt==null) {
				//for when we are loading account data for the actual account module.
				stmt=new Statement() { StatementType=StmtType.NotSet };
			}
			DataSet retVal=new DataSet();
			patientPayPlanDue=0;
			dynamicPayPlanDue=0;
			balanceForward=0;
			bool isReseller=false;//Used to display data in the account module differently when patient is a reseller.
			//HQ only, find out if this patient is a reseller.
			if(PrefC.IsODHQ && Resellers.IsResellerFamily(fam.ListPats[0])) {
				isReseller=true;
			}
			DataConnection dcon=new DataConnection();
			DataTable table=new DataTable("account");
			if(isComputeAging) {
				//run aging.-------------------------------------------------------
				if(PrefC.GetBool(PrefName.AgingCalculatedMonthlyInsteadOfDaily)) {
					Ledgers.ComputeAging(pat.Guarantor,PrefC.GetDate(PrefName.DateLastAging));
				}
				else {
					Ledgers.ComputeAging(pat.Guarantor,DateTime.Today);
				}
			}
			//Now, back to getting the tables------------------------------------------------------------------
			DataRow row;
			SetTableColumns(table);
			//but we won't actually fill this table with rows until the very end.  It's more useful to use a List<> for now.
			List<DataRow> rows=new List<DataRow>();
			DateTime dateT;
			decimal qty;
			decimal amt;
			string command;
			string familyPatNums="";
			if(fam!=null && fam.ListPats!=null && fam.ListPats.Length>0) {//Currently fam is always defined and ListPats always has a reference this pat.
				familyPatNums=string.Join(",",fam.ListPats.Select(x => POut.Long(x.PatNum)));
			}
			string adjNumsForLimited="";
			string paySplitNumsForLimited="";
			string procNumsForLimited="";
			string claimNumsForLimited="";
			if(stmt.StatementType==StmtType.LimitedStatement && statementNum>0) {
				adjNumsForLimited=string.Join(",",stmt.ListAdjNums.Select(x => POut.Long(x)));
				paySplitNumsForLimited=string.Join(",",stmt.ListPaySplitNums.Select(x => POut.Long(x)));
				procNumsForLimited=string.Join(",",stmt.ListProcNums.Select(x => POut.Long(x)));
				claimNumsForLimited=string.Join(",",stmt.ListInsPayClaimNums.Select(x => POut.Long(x)));
			}
			#region Claimprocs
			//claimprocs (ins payments)----------------------------------------------------------------------------
			command="SELECT ClaimNum,MAX(ClaimPaymentNum) ClaimPaymentNum,MAX(claimproc.ClinicNum) ClinicNum,DateCP,"
				+"SUM(CASE WHEN claimproc.PayPlanNum=0 THEN InsPayAmt ELSE 0 END) InsPayAmt_,"//ins payments attached to payment plans tracked there
				+"SUM(CASE WHEN claimproc.PayPlanNum!=0 THEN InsPayAmt ELSE 0 END) InsPayAmtPayPlan,"
				+"MAX(claimproc.PatNum) PatNum,MAX(claimproc.ProcDate) ProcDate,"//MAX functions added to preserve behavior in Oracle.
				//+"MAX(ProvNum) ProvNum,
				+"SUM(WriteOff) WriteOff_,IsTransfer, "
				//js 1/28/13  The following line has been the source of many complaints in the past.  
				//When it was claim.ProvBill, it didn't match daily payment report or the account Claim row entry.
				//When it was MAX(claimproc.ProvNum), the user had no control over it because it was one prov at random.
 				//By switching to claim.ProvTreat, we are more closely matching the P&I report and the account Claim row.  ProvBill is not very meaningful outside of the claim itself.
				+"(SELECT ProvTreat FROM claim WHERE claimproc.ClaimNum=claim.ClaimNum) provNum_,"
				+"MAX(claimproc.PayPlanNum) PayPlanNum,"//MAX PayPlanNum will return 0 or the num of the payplan tracking the payments. Each claim will have at most 1 PayPlanNum.
				+"MAX(claimproc.SecDateTEdit) SecDateTEdit, "
				+"MAX(claimproc.ProcNum) HasProc "//Any value other than 0 will indicate that this claim has actual procedures associated (not just pay as totals).
				+"FROM claimproc "
				+$"WHERE Status IN ({POut.Int((int)ClaimProcStatus.Received)},{POut.Int((int)ClaimProcStatus.Supplemental)},{POut.Int((int)ClaimProcStatus.CapClaim)}) "
				+"AND (WriteOff!=0 OR InsPayAmt!=0) ";
			if(familyPatNums!="") {
				command+="AND PatNum IN ("+familyPatNums+") ";
			}
			List<string> strLimitedWhereClauses=new List<string>();
			if(stmt.StatementType==StmtType.LimitedStatement) {
				if(!string.IsNullOrEmpty(procNumsForLimited)) {
					strLimitedWhereClauses.Add("claimproc.ProcNum IN ("+procNumsForLimited+")");
				}
				if(!string.IsNullOrEmpty(claimNumsForLimited)) {
					strLimitedWhereClauses.Add("claimproc.ClaimNum IN ("+claimNumsForLimited+")");
				}
				if(strLimitedWhereClauses.Count>0) {
					command+="AND ("+string.Join(" OR ",strLimitedWhereClauses)+") ";
				}
			}
			command+="GROUP BY claimproc.ClaimNum,claimproc.DateCP,claimproc.Status,"
				//Differentiate multiple supplemental payments on the same day.
				+$"CASE WHEN claimproc.Status={POut.Int((int)ClaimProcStatus.Supplemental)} THEN claimproc.ClaimPaymentNum ELSE 0 END";
			DataTable rawClaimPay=new DataTable();
			if(!isInvoice && (stmt.StatementType!=StmtType.LimitedStatement || procNumsForLimited!="")) {//don't run if IsInvoice or if LimitedStatement with no procs
				rawClaimPay=dcon.GetTable(command);
			}
			DateTime procdate;
			decimal writeoff;
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.AccountColors);
			foreach(DataRow rawClaimPayRow in rawClaimPay.Rows) {//0 rows if isInvoice or is LimitedStatement with no procs
				//Hide transfers from showing so that they do not confuse the user / patient.
				//However, only hide transfers associated to claims with actual procedures attached.
				//There was a bug with databases from conversions where there were claims with no procedures but with a claimproc (pay as total).
				if(PIn.Bool(rawClaimPayRow["IsTransfer"].ToString()) && PIn.Bool(rawClaimPayRow["HasProc"].ToString())) {
					continue;
				}
				row=table.NewRow();
				row["AbbrDesc"]="";//fill this later
				row["AdjNum"]=0;
				row["balance"]="";//fill this later
				row["balanceDouble"]=0;//fill this later
				row["chargesDouble"]=0;
				row["charges"]="";
				row["ClaimNum"]=PIn.Long(rawClaimPayRow["ClaimNum"].ToString());
				//jsalmon - I do not agree with the next line but am leaving it here so as to not break unknown parts of the program.  Something like this should never be done.
				//          We either need to create a separate column using a naming convention that leads programmers to think it is a boolean or
				//          we need to make the column lowercase "claimPaymentNum".  Making the first character lowercase will at least lead OD developers to this line 
				//          so that they can then learn that this variable is not to be trusted and that it is in fact a boolean...
				row["ClaimPaymentNum"]="1";//this is now just a boolean flag indicating that it is a payment.
				//this is because it will frequently not be attached to an actual claim payment.
				long clinicNumCur=PIn.Long(rawClaimPayRow["ClinicNum"].ToString());
				row["clinic"]=Clinics.GetDesc(clinicNumCur);
				row["ClinicNum"]=clinicNumCur;
				row["colorText"]=listDefs[7].ItemColor.ToArgb().ToString();
				amt=PIn.Decimal(rawClaimPayRow["InsPayAmt_"].ToString());//payments tracked in payment plans will show in the payment plan grid
				writeoff=PIn.Decimal(rawClaimPayRow["WriteOff_"].ToString());
				if(rawClaimPayRow["PayPlanNum"].ToString()!="0" && amt+writeoff==0) {//payplan payments are tracked in the payplan, so nothing to display.
					continue;//Does not add a row, so don't worry about setting the remaining columns.
				}
				row["creditsDouble"]=amt+writeoff;
				row["credits"]=((decimal)row["creditsDouble"]).ToString("n");
				dateT=PIn.DateT(rawClaimPayRow["DateCP"].ToString());
				row["DateTime"]=dateT;
				row["date"]=dateT.ToString(Lans.GetShortDateTimeFormat());
				row["dateTimeSort"]=PIn.DateT(rawClaimPayRow["SecDateTEdit"].ToString());//MAX SecDateTEdit will be used for sorting if RandomKeys is enabled
				procdate=PIn.DateT(rawClaimPayRow["ProcDate"].ToString());
				row["description"]=Lans.g("AccountModule","Insurance Payment for Claim")+" "+procdate.ToShortDateString();
				if(rawClaimPayRow["PayPlanNum"].ToString()!="0") {
					row["description"]+="\r\n("+Lans.g("AccountModule","Payments Tracked in Payment Plan")+")";
				}
				if(rawClaimPayRow["PayPlanNum"].ToString()!="0" || writeoff!=0) {
					row["description"]+="\r\n"+Lans.g("AccountModule","Payment")+": "+amt.ToString("c");
				}
				if(writeoff!=0) {
					string writeoffDescript=PrefC.GetString(PrefName.InsWriteoffDescript);
					if(writeoffDescript=="") {
						writeoffDescript=Lans.g("AccountModule","Writeoff");
					}
					row["description"]+="\r\n"+writeoffDescript+": "+writeoff.ToString("c");
				}
				if(!isForStatementPrinting && amt!=0 && rawClaimPayRow["ClaimPaymentNum"].ToString()=="0") {
					//Not all claim payments have been finalized and are not yet attached to claim payments (checks).
					//Indicate to the user that they need to finalize this payment before reports will be accurate.
					row["description"]+="\r\n"+Lans.g("AccountModule","PAYMENT NEEDS TO BE FINALIZED");
				}
				row["IsTransfer"]=PIn.Bool(rawClaimPayRow["IsTransfer"].ToString());
				long patNumCur=PIn.Long(rawClaimPayRow["PatNum"].ToString());
				row["patient"]=GetPatName(patNumCur,fam,doIncludePatLName);
				row["PatNum"]=patNumCur;
				row["PayNum"]=0;
				row["PayPlanNum"]=0;
				row["PayPlanChargeNum"]="0";
				row["ProcCode"]=Lans.g("AccountModule","InsPay");
				row["ProcNum"]="0";
				row["ProcNumLab"]="";
				row["procsOnObj"]="";
				row["adjustsOnObj"]="";
				row["prov"]=Providers.GetAbbr(PIn.Long(rawClaimPayRow["provNum_"].ToString()));
				row["signed"]="";
				row["StatementNum"]=0;
				row["ToothNum"]="";
				row["ToothRange"]="";
				row["tth"]="";
				rows.Add(row);
			}
			#endregion Claimprocs
			#region Procedures
			//Procedures------------------------------------------------------------------------------------------
			command=@"SELECT 
				(SELECT SUM(AdjAmt) FROM adjustment WHERE procedurelog.ProcNum=adjustment.ProcNum 
				AND adjustment.ProcNum!=0) adj_,"//Prevents long load time in a patient with thousands of entries.
				+$@"procedurelog.BaseUnits,procedurelog.BillingNote,procedurelog.ClinicNum,procedurecode.CodeNum,procedurecode.AbbrDesc,Descript,
				SUM(CASE WHEN claimproc.Status IN ({string.Join(",",ClaimProcs.GetInsPaidStatuses().Select(x => (int)x))}) 
					THEN claimproc.InsPayAmt END) insPayAmt_,
				SUM(CASE WHEN claimproc.Status={POut.Int((int)ClaimProcStatus.NotReceived)} THEN claimproc.InsPayEst END) insPayEst_,
				LaymanTerm,procedurelog.MedicalCode,MAX(claimproc.NoBillIns) noBillIns_,procedurelog.PatNum,
				(SELECT SUM(paysplit.SplitAmt) FROM paysplit WHERE procedurelog.ProcNum=paysplit.ProcNum 
				AND paysplit.ProcNum!=0) patPay_,"//Prevents long load time in a patient with thousands of entries.
				+@"ProcCode,procedurelog.ProcDate procDate_,ProcFee,procedurelog.ProcNum,procedurelog.ProcNumLab,procedurelog.ProvNum,procedurelog.Surf,
				ToothNum,ToothRange,UnitQty,"
				+@"SUM(CASE WHEN claimproc.Status IN("+POut.Int((int)ClaimProcStatus.NotReceived)+","+POut.Int((int)ClaimProcStatus.Received)+","+POut.Int((int)ClaimProcStatus.Supplemental)+","+POut.Int((int)ClaimProcStatus.CapClaim)+@") THEN claimproc.WriteOff END) writeOff_,
				MIN(CASE WHEN claimproc.Status!="+POut.Int((int)ClaimProcStatus.CapComplete)+@" 
					AND insplan.IsMedical=(CASE WHEN procedurelog.MedicalCode!='' THEN 1 ELSE 0 END) 
					THEN (CASE WHEN claimproc.Status IN ("+POut.Int((int)ClaimProcStatus.Estimate)+","+POut.Int((int)ClaimProcStatus.CapEstimate)+","+POut.Int((int)ClaimProcStatus.InsHist)
					+@") THEN 0 ELSE 1 END) END) unsent_,
				SUM(CASE WHEN claimproc.Status="+POut.Int((int)ClaimProcStatus.CapComplete)+@" THEN claimproc.WriteOff END) writeOffCap_,
				procedurelog.StatementNum,procedurelog.DateTStamp
				FROM procedurelog
				INNER JOIN procedurecode ON procedurelog.CodeNum=procedurecode.CodeNum "
				//indexAcctCov will always exists because the convert script fails if it can't be added.
				+"LEFT JOIN claimproc "+DbHelper.UseIndex("indexAcctCov","claimproc")+@" ON procedurelog.ProcNum=claimproc.ProcNum 
				LEFT JOIN insplan ON insplan.PlanNum=claimproc.PlanNum
				WHERE ProcStatus="+POut.Int((int)ProcStat.C)+" ";
			if(familyPatNums!="") {
				command+="AND procedurelog.PatNum IN ("+familyPatNums+") ";
			}
			if(stmt.StatementType==StmtType.LimitedStatement && procNumsForLimited!="") {
				command+="AND procedurelog.ProcNum IN ("+procNumsForLimited+") ";
			}
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				command+="GROUP BY procedurelog.ClinicNum,procedurelog.BaseUnits,procedurelog.BillingNote,procedurecode.CodeNum,procedurecode.AbbrDesc,"
					+"Descript,LaymanTerm,procedurelog.MedicalCode,procedurelog.PatNum,ProcCode,procedurelog.ProcDate,ProcFee,procedurelog.ProcNum,"
					+"procedurelog.ProcNumLab,procedurelog.ProvNum,procedurelog.Surf,ToothNum,ToothRange,UnitQty,procedurelog.StatementNum ";
			}
			else{//mysql. Including Descript in the GROUP BY causes mysql to lock up sometimes.  Unsure why.
				command+="GROUP BY procedurelog.ProcNum ";
			}
			if(isInvoice) {
				//different query here.  Include all column names.
				command="SELECT '' AS adj_,procedurelog.BaseUnits,procedurelog.BillingNote,procedurelog.ClinicNum,procedurecode.CodeNum,procedurecode.AbbrDesc,procedurecode.Descript,"
					+"'' AS insPayAmt_,'' AS insPayEst_,procedurecode.LaymanTerm,procedurelog.MedicalCode,'' AS noBillIns_,procedurelog.PatNum,"
					+"'' AS patPay_,procedurecode.ProcCode,"+DbHelper.DtimeToDate("procedurelog.ProcDate")+" procDate_,procedurelog.ProcFee,procedurelog.ProcNum,procedurelog.ProcNumLab,"
					+"procedurelog.ProvNum,procedurelog.Surf,procedurelog.ToothNum,procedurelog.ToothRange,procedurelog.UnitQty,"
					+"'' AS writeOff_,'' AS unsent_,'' AS writeOffCap_,procedurelog.StatementNum,procedurelog.DateTStamp "
					+"FROM procedurelog "
					+"LEFT JOIN procedurecode ON procedurelog.CodeNum=procedurecode.CodeNum "
					+"WHERE StatementNum="+POut.Long(statementNum);
			}
			DataTable rawProc=new DataTable();
			if(stmt.StatementType!=StmtType.LimitedStatement || procNumsForLimited!="") {//Don't run if this is a limited statement with no procs
				rawProc=dcon.GetTable(command);
			}
			List<long> listSignedProcNums=new List<long>();//filled with subset of procnums from the rawProc table where most recent ProcNote is signed
			if(!isForStatementPrinting //not for a statement
				&& familyPatNums!="" //and we have a family
				&& DisplayFields.GetForCategory(DisplayFieldCategory.AccountModule).Any(x => x.InternalName=="Signed")) //"Signed" is displayed in acct grid
			{
				listSignedProcNums=ProcNotes.GetIsProcNoteSigned(rawProc.Select().Select(x => PIn.Long(x["ProcNum"].ToString())).ToList());
			}
			decimal insPayAmt;
			decimal insPayEst;
			decimal writeOff;
			decimal writeOffCap;
			decimal patPort;
			decimal patPay;
			bool isNoBill;
			decimal adjAmt;
			string extraDetail;
			List<DataRow> labRows=new List<DataRow>();//Canadian lab procs, which must be added in a loop at the very end.
			for(int i=0;i<rawProc.Rows.Count;i++){
				DataRow rawProcRow=rawProc.Rows[i];
				row=table.NewRow();
				row["AbbrDesc"]=rawProcRow["AbbrDesc"].ToString();
				row["AdjNum"]=0;
				row["balance"]="";//fill this later
				row["balanceDouble"]=0;//fill this later
				qty=Math.Max(1,PIn.Long(rawProcRow["UnitQty"].ToString()) + PIn.Long(rawProcRow["BaseUnits"].ToString()));
				amt=PIn.Decimal(rawProcRow["ProcFee"].ToString())*qty;
				writeOffCap=PIn.Decimal(rawProcRow["writeOffCap_"].ToString());
				amt-=writeOffCap;
				row["chargesDouble"]=amt;
				row["charges"]=((decimal)row["chargesDouble"]).ToString("n");
				row["ClaimNum"]=0;
				row["ClaimPaymentNum"]="0";
				long clinicNumCur=PIn.Long(rawProcRow["ClinicNum"].ToString());
				row["clinic"]=Clinics.GetDesc(clinicNumCur);
				row["ClinicNum"]=clinicNumCur;
				string procCode=rawProcRow["ProcCode"].ToString();
				if(procCode=="D9986") {//Broken appointment procedure
					row["colorText"]=Defs.GetDefByExactName(DefCat.AccountColors,"Broken Appointment Procedure").ItemColor.ToArgb().ToString();
				}
				else if(procCode=="D9987") {//Canceled appointment procedure
					row["colorText"]=Defs.GetDefByExactName(DefCat.AccountColors,"Canceled Appointment Procedure").ItemColor.ToArgb().ToString();
				}
				else {//Not a broken appointment procedure.
					row["colorText"]=Defs.GetDefByExactName(DefCat.AccountColors,"Default").ItemColor.ToArgb().ToString();
				}
				row["creditsDouble"]=0;
				row["credits"]="";
				dateT=PIn.DateT(rawProcRow["procDate_"].ToString());
				row["DateTime"]=dateT;
				row["date"]=dateT.ToString(Lans.GetShortDateTimeFormat());
				row["dateTimeSort"]=PIn.DateT(rawProcRow["DateTStamp"].ToString());//DateTStamp will be used for sorting if RandomKeys is enabled
				long codeNum=PIn.Long(rawProcRow["CodeNum"].ToString());
				string surf=rawProcRow["Surf"].ToString();
				string toothNum=rawProcRow["ToothNum"].ToString();
				row["description"]=Procedures.ConvertProcToString(codeNum,surf,toothNum,true)+" ";
				if(rawProcRow["MedicalCode"].ToString()!=""){
					row["description"]+=Lans.g("ContrAccount","(medical)")+" ";
				}
				string toothRange=rawProcRow["ToothRange"].ToString();
				if(toothRange!=""){
					row["description"]+=" #"+Tooth.FormatRangeForDisplay(toothRange);
				}
				isNoBill=(rawProcRow["noBillIns_"].ToString()!="" && rawProcRow["noBillIns_"].ToString()!="0");
				if(isNoBill){
					row["description"]+=" "+Lans.g("ContrAccount","(No Bill Ins)");
				}
				bool isShowUnsent=(!isNoBill && rawProcRow["unsent_"].ToString()=="0");//no claim attached and marked to bill insurance
				string strProcNumLab=rawProcRow["ProcNumLab"].ToString();
				if(!isNoBill && CultureInfo.CurrentCulture.Name.EndsWith("CA") && strProcNumLab!="0") {//Canadian. en-CA or fr-CA, lab fee.
					//true if the parent proc does not have a claim attached and this lab is not marked "no bill ins".  Lab is unsent if parent proc is unsent
					isShowUnsent=rawProc.Select().Any(x => x["ProcNum"].ToString()==strProcNumLab && x["unsent_"].ToString()=="0");
				}
				long procNum=PIn.Long(rawProcRow["ProcNum"].ToString());
				if(ProcMultiVisits.IsProcInProcess(procNum)) {
					row["description"]+=" "+Lans.g("ContrAccount","(In Process)");
				}
				else if(isShowUnsent) {
					row["description"]+=" "+Lans.g("ContrAccount","(unsent)");
				}
				insPayAmt=PIn.Decimal(rawProcRow["insPayAmt_"].ToString());
				insPayEst=PIn.Decimal(rawProcRow["insPayEst_"].ToString());
				writeOff=PIn.Decimal(rawProcRow["writeOff_"].ToString());
				patPort=amt-insPayAmt-insPayEst-writeOff;
				patPay=PIn.Decimal(rawProcRow["patPay_"].ToString());
				adjAmt=PIn.Decimal(rawProcRow["adj_"].ToString());
				extraDetail="";
				if(patPay>0){
					extraDetail+=Lans.g("AccountModule","Pat Paid: ")+patPay.ToString("c");
				}
				if(adjAmt!=0){
					patPort+=adjAmt;//adjAmt can be negative or positive
					if(extraDetail!=""){
						extraDetail+=", ";
					}
					extraDetail+=Lans.g("AccountModule","Adj: ")+adjAmt.ToString("c");
				}
				if(insPayAmt>0 || writeOff>0){
					if(extraDetail!=""){
						extraDetail+=", ";
					}
					extraDetail+=Lans.g("AccountModule","Ins Paid: ")+insPayAmt.ToString("c");
					if(writeOff>0) {
						string writeoffDescript=PrefC.GetString(PrefName.InsWriteoffDescript);
						if(writeoffDescript=="") {
							writeoffDescript=Lans.g("AccountModule","Writeoff");
						}
						extraDetail+=", "+writeoffDescript+": "+writeOff.ToString("c");
					}
				}
				if(insPayEst>0) {
					if(extraDetail!="") {
						extraDetail+=", ";
					}
					extraDetail+=Lans.g("AccountModule","Ins Est: ")+insPayEst.ToString("c");
				}
				if(patPort>0 && writeOffCap==0){//if there is a cap writeoff, showing a patient portion would calculate wrong.
					if(extraDetail!="") {
						extraDetail+=", ";
					}
					extraDetail+=Lans.g("AccountModule","Pat Port: ")+patPort.ToString("c");
				}
				if(showProcBreakdown) {
					if(extraDetail!="") {
						row["description"]+="\r\n"+extraDetail;
					}
				}
				string billingNote=PIn.String(rawProcRow["BillingNote"].ToString());
				if(billingNote!="") {
					row["description"]+="\r\n"+billingNote;
				}
				long patNumCur=PIn.Long(rawProcRow["PatNum"].ToString());
				//for printing statements. Don't show zeros, just blanks.
				row["InvoiceNum"]=rawProcRow["StatementNum"].ToString()=="0" ? "" : rawProcRow["StatementNum"].ToString(); 
				row["patient"]=GetPatName(patNumCur,fam,(isReseller || doIncludePatLName));
				row["PatNum"]=patNumCur;
				row["PayNum"]=0;
				row["PayPlanNum"]=0;
				row["PayPlanChargeNum"]="0";
				row["ProcCode"]=procCode;
				row["ProcNum"]=procNum.ToString();
				row["ProcNumLab"]=strProcNumLab;
				row["procsOnObj"]="";
				row["adjustsOnObj"]="";
				row["prov"]=Providers.GetAbbr(PIn.Long(rawProcRow["ProvNum"].ToString()));
				row["signed"]=listSignedProcNums.Contains(procNum)?"Signed":"";
				row["StatementNum"]=0;
				row["ToothNum"]=toothNum;
				row["ToothRange"]=toothRange;
				row["tth"]=Tooth.GetToothLabel(toothNum);
				if(strProcNumLab=="0") {//normal proc
					rows.Add(row);
				}
				else {
					row["description"]="^ ^ "+row["description"].ToString();
					labRows.Add(row);//these will be added in the loop at the end
				}
			}
			#endregion Procedures
			#region Adjustments
			//Adjustments---------------------------------------------------------------------------------------
			command=$@"SELECT adjustment.AdjAmt,adjustment.AdjDate,adjustment.AdjNum,adjustment.AdjType,adjustment.ClinicNum,adjustment.PatNum,
				adjustment.ProcNum,adjustment.ProvNum,adjustment.AdjNote,adjustment.SecDateTEdit,
				SUM({(string.IsNullOrEmpty(familyPatNums)?"COALESCE(":$"IF(paysplit.PatNum IN ({familyPatNums}),")}paysplit.SplitAmt,0)) SumAmt
				FROM adjustment
				LEFT JOIN paysplit ON paysplit.AdjNum>0 AND adjustment.AdjNum=paysplit.AdjNum ";
				//paysplit.AdjNum>0 added because MySQL 5.5 ignores all paysplit indexes without it. Runtime reduced from 3.3 sec to 0.005 sec for one user.
			if(isInvoice) {
				command+="WHERE adjustment.StatementNum="+POut.Long(statementNum)+" ";
			}
			else if(stmt.StatementType==StmtType.LimitedStatement) {
				List<string> listAdjWhereOR=new List<string>();
				if(adjNumsForLimited!="") {
					listAdjWhereOR.Add("adjustment.AdjNum IN ("+adjNumsForLimited+")");//adjustments highlighted by user
				}
				if(procNumsForLimited!="") {
					listAdjWhereOR.Add("adjustment.ProcNum IN ("+procNumsForLimited+")");//add adjustments for selected procs whether user highlighted or not
				}
				if(listAdjWhereOR.Count>0) {
					command+="WHERE ("+string.Join(" OR ",listAdjWhereOR)+") ";
				}
			}
			else if(familyPatNums!="") {
				command+="WHERE adjustment.PatNum IN ("+familyPatNums+") ";
			}
			command+="GROUP BY adjustment.AdjNum ";
			DataTable rawAdj=new DataTable();
			//don't run query if LimitedStatement and both lists are empty
			if(stmt.StatementType!=StmtType.LimitedStatement || adjNumsForLimited!="" || procNumsForLimited!="") {
				rawAdj=dcon.GetTable(command);
			}
			for(int i=0;i<rawAdj.Rows.Count;i++){
				row=table.NewRow();
				row["AbbrDesc"]="";
				row["AdjNum"]=PIn.Long(rawAdj.Rows[i]["AdjNum"].ToString());
				row["balance"]="";//fill this later
				row["balanceDouble"]=0;//fill this later
				amt=PIn.Decimal(rawAdj.Rows[i]["AdjAmt"].ToString());
				if(amt<0){
					row["chargesDouble"]=0;
					row["charges"]="";
					row["creditsDouble"]=-amt;
					row["credits"]=(-amt).ToString("n");
				}
				else{
					row["chargesDouble"]=amt;
					row["charges"]=amt.ToString("n");
					row["creditsDouble"]=0;
					row["credits"]="";
				}
				row["ClaimNum"]=0;
				row["ClaimPaymentNum"]="0";
				long clinicNumCur=PIn.Long(rawAdj.Rows[i]["ClinicNum"].ToString());
				row["clinic"]=Clinics.GetDesc(clinicNumCur);
				row["ClinicNum"]=clinicNumCur;
				row["colorText"]=listDefs[1].ItemColor.ToArgb().ToString();
				dateT=PIn.DateT(rawAdj.Rows[i]["AdjDate"].ToString());
				row["DateTime"]=dateT;
				row["date"]=dateT.ToString(Lans.GetShortDateTimeFormat());
				row["dateTimeSort"]=PIn.DateT(rawAdj.Rows[i]["SecDateTEdit"].ToString());//SecDateTEdit will be used for sorting if RandomKeys is enabled
				row["description"]=Defs.GetName(DefCat.AdjTypes,PIn.Long(rawAdj.Rows[i]["AdjType"].ToString()));
				decimal sumAmt=PIn.Decimal(rawAdj.Rows[i]["SumAmt"].ToString());
				long procNum=PIn.Long(rawAdj.Rows[i]["ProcNum"].ToString());
				bool isShowingProc=PrefC.GetBool(PrefName.StatementShowProcBreakdown);
				if(sumAmt!=0 && procNum==0) {
					if(statementNum>0 && isShowingProc) {//Is a statement, use global pref.
						row["description"]+="\r\n"+Lans.g("AccountModule","Pat Paid: ")+sumAmt.ToString("c");
					}
					else if(statementNum==0 && showProcBreakdown){//Not a statement, use user pref.
						row["description"]+="\r\n"+Lans.g("AccountModule","Pat Paid: ")+sumAmt.ToString("c");
					}
				}
				if(rawAdj.Rows[i]["AdjNote"].ToString() !="" && showAdjNotes) {
					row["description"]+="\r\n"+rawAdj.Rows[i]["AdjNote"].ToString();
				}
				long patNumCur=PIn.Long(rawAdj.Rows[i]["PatNum"].ToString());
				row["patient"]=GetPatName(patNumCur,fam,(isReseller || doIncludePatLName));
				row["PatNum"]=patNumCur;
				row["PayNum"]=0;
				row["PayPlanNum"]=0;
				row["PayPlanChargeNum"]="0";
				row["ProcCode"]=Lans.g("AccountModule","Adjust");
				row["ProcNum"]="0";
				row["ProcNumLab"]="";
				row["procsOnObj"]=rawAdj.Rows[i]["ProcNum"].ToString();
				row["adjustsOnObj"]="";
				row["prov"]=Providers.GetAbbr(PIn.Long(rawAdj.Rows[i]["ProvNum"].ToString()));
				row["signed"]="";
				row["StatementNum"]=0;
				row["ToothNum"]="";
				row["ToothRange"]="";
				row["tth"]="";
				rows.Add(row);
			}
			#endregion Adjustments
			#region Paysplits
			//paysplits-----------------------------------------------------------------------------------------
			List<long> listHiddenUnearnedDefNums=Defs.GetDefsForCategory(DefCat.PaySplitUnearnedType)
				.FindAll(x => !string.IsNullOrEmpty(x.ItemValue))
				.Select(x => x.DefNum).ToList();
			List<string> listWhereClauses=new List<string>();
			string familyPayPlanNums=String.Join(",",PayPlans.GetForPats(fam.ListPats.Select(x=>x.PatNum).ToList(),pat.PatNum)
				.Select(y => y.PayPlanNum).ToList());
			if(familyPatNums!="") {
				//grab any paysplits for the family or that are attached to payment plans for the family.
				//This is so we can associate payments to payment plans for this family, even if someone from a different family is making the payments.
				string whereFamilyPatPayPlan="(paysplit.PatNum IN ("+familyPatNums+")";
				if(familyPayPlanNums!="") {
					whereFamilyPatPayPlan+="OR paysplit.PayPlanNum IN ("+familyPayPlanNums+")";
				}
				whereFamilyPatPayPlan+=")";
				listWhereClauses.Add(whereFamilyPatPayPlan);
			}
			if(stmt.StatementType==StmtType.LimitedStatement) {
				List<string> listLimitedWhere=new List<string>();
				if(paySplitNumsForLimited!="") {
					listLimitedWhere.Add("paysplit.SplitNum IN ("+paySplitNumsForLimited+")");//add paysplits highlighted by the user
				}
				if(procNumsForLimited!="") {
					listLimitedWhere.Add("paysplit.ProcNum IN ("+procNumsForLimited+")");//add paysplits for selected procs whether user highlighted or not
				}
				if(listLimitedWhere.Count>0) {
					listWhereClauses.Add("("+string.Join(" OR ",listLimitedWhere)+")");
				}
			}
			if(listHiddenUnearnedDefNums.Count > 0 && !doShowHiddenPaySplits) {
				listWhereClauses.Add("paysplit.UnearnedType NOT IN ("+string.Join(",",listHiddenUnearnedDefNums)+") ");
			}
			string whereClause="";//create whereClause variable so both queries get the same paysplits
			if(listWhereClauses.Count>0) {
				whereClause="WHERE "+string.Join(" AND ",listWhereClauses);
			}
			//Payment query 1
			//Column names with MAX left the same as they should not be considered aggregate (even though they are).
			//MAX function used to preserve behavior in Oracle.
			command="SELECT MAX(CheckNum) CheckNum,paysplit.ClinicNum,MAX(DatePay) DatePay,paysplit.PatNum,MAX(payment.PatNum) patNumPayment_,"
				+"MAX(PayAmt) PayAmt,paysplit.PayNum,paysplit.PayPlanNum,MAX(PayType) PayType,MAX(ProcDate) ProcDate,'' AS ProcNums_,MAX(ProvNum) ProvNum,"
				+"SUM(SplitAmt) splitAmt_,MAX(payment.PayNote) PayNote,MAX(paysplit.UnearnedType) UnearnedType,MAX(paysplit.SecDateTEdit) SecDateTEdit,'' as AdjNums_,"
				+"0 isOutOfFamily_ "
				+"FROM paysplit "
				+"LEFT JOIN payment ON paysplit.PayNum=payment.PayNum "
				+whereClause+" "
				//if this GROUP BY changes, the foreach loop below must be changed to match
				+"GROUP BY paysplit.DatePay,paysplit.PayPlanNum,paysplit.PayNum,paysplit.PatNum,paysplit.ClinicNum ";
			//Payment query 2
			if(!string.IsNullOrWhiteSpace(familyPatNums) && stmt.StatementType!=StmtType.LimitedStatement) {//Should not show in limited statements
				//familyPatNums always contains at least 'patNum'.
				//Gets all payments paid by this patient but split to someone outside the family, if we don't do this then splits paid out of family will only be 
				//visible on the account of the patient it was paid _to_ and not on the patient who made the payment, which can cause confusion. This will, 
				//instead, display a payment row on their account with a 0 splitAmt, but clicking into it will allow them to see where the money was paid to and 
				//track it down - which they could not do before hand.
				//The only differences between the selections below and those from above are that we use 'payment.PatNum' and '0 AS splitAmt_'.
				command+="UNION ALL "
					+"SELECT MAX(CheckNum) CheckNum,paysplit.ClinicNum,MAX(DatePay) DatePay,payment.PatNum,MAX(payment.PatNum) patNumPayment_,"
					+"MAX(payment.PayAmt) PayAmt,paysplit.PayNum,MAX(paysplit.PayPlanNum),MAX(PayType) PayType,MAX(paysplit.ProcDate) ProcDate,'' AS ProcNums_,MAX(paysplit.ProvNum) ProvNum,"
					//$0 splitAmt because these rows are for payments that were not split to(on behalf of) the current account.
					+"0 AS splitAmt_,MAX(payment.PayNote) PayNote,MAX(paysplit.UnearnedType) UnearnedType,MAX(paysplit.SecDateTEdit) SecDateTEdit,'' as AdjNums_, "
					+"1 isOutOfFamily_ "
					+"FROM payment "
					//paysplit must be to someone outside this family (query will include payments split both inside/outside the family.  Duplicates removed later).
					+"INNER JOIN paysplit ON payment.PayNum=paysplit.PayNum AND paysplit.PatNum NOT IN ("+familyPatNums+") "
					//payment must be made by this patient.
					+$"WHERE payment.PatNum={POut.Long(pat.PatNum)} "
					//paysplits to another family but on a payplan for the current family already included in first half of UNION
					+((string.IsNullOrWhiteSpace(familyPayPlanNums)) ? "" : "AND paysplit.PayPlanNum NOT IN ("+familyPayPlanNums+") ")
					//A simple grouping because we only need to indicate this patient made a payment to someone outside the family.  Finer level of detail 
					//can be accessed by double-clicking into the payment.
					+"GROUP BY payment.PayNum ";
			}
			DataTable rawPay=new DataTable();
			//don't run query if isInvoice or if it's a LimitedStatement and no paysplits or procs were selected
			if(!isInvoice && (stmt.StatementType!=StmtType.LimitedStatement || paySplitNumsForLimited!="" || procNumsForLimited!="")) {
				rawPay=dcon.GetTable(command);
			}
			//Payment query 3
			command="SELECT * FROM paysplit "+whereClause;
			List<PaySplit> listPaySplits=new List<PaySplit>();
			//don't run query if isInvoice or if it's a LimitedStatement and no paysplits or procs were selected
			if(!isInvoice && (stmt.StatementType!=StmtType.LimitedStatement || paySplitNumsForLimited!="" || procNumsForLimited!="")) {
				listPaySplits=Crud.PaySplitCrud.SelectMany(command);
			}
			foreach(DataRow rowRp in rawPay.Rows) {//Each row is a payment
				if(listPaySplits.Count==0) {
					break;
				}
				//these are the GROUP BY columns from the rawPay query above, used to select the ProcNums from all of the paysplits using the same grouping
				DateTime rpDatePay=PIn.Date(rowRp["DatePay"].ToString());
				long rpPayPlanNum=PIn.Long(rowRp["PayPlanNum"].ToString());
				long rpPayNum=PIn.Long(rowRp["PayNum"].ToString());
				long rpPatNum=PIn.Long(rowRp["PatNum"].ToString());
				long rpClinicNum=PIn.Long(rowRp["ClinicNum"].ToString());
				//if the GROUP BY for the query used to fill table rawPay changes, this Linq needs to be changed to match exactly
				List<PaySplit> listPaySplitMatches=listPaySplits.FindAll(x => x.DatePay==rpDatePay
					&& x.PayPlanNum==rpPayPlanNum
					&& x.PayNum==rpPayNum
					&& x.PatNum==rpPatNum
					&& x.ClinicNum==rpClinicNum
					&& x.ProcNum>0);
				if(listPaySplitMatches.Count>0) {
					rowRp["ProcNums_"]=string.Join(",",listPaySplitMatches.Select(x => x.ProcNum));
				}
				listPaySplitMatches=listPaySplits.FindAll(x => x.DatePay==rpDatePay
					&& x.PayPlanNum==rpPayPlanNum
					&& x.PayNum==rpPayNum
					&& x.PatNum==rpPatNum
					&& x.ClinicNum==rpClinicNum
					&& x.AdjNum>0);
				if(listPaySplitMatches.Count>0) {
					rowRp["AdjNums_"]+=string.Join(",",listPaySplitMatches.Select(x => x.AdjNum));
				}
			}
			PayPlanVersions payPlanVersionCur=(PayPlanVersions)PrefC.GetInt(PrefName.PayPlansVersion);
			decimal payamt;
			//Expand the family check to include super family for the purpose of $0 paysplits to out of family (but in super family) payplan payments
			List<long> listSuperFamPatNums=(rawPay.Rows.Count>0) ? Patients.GetAllFamilyPatNumsForSuperFam(new List<long>{pat.SuperFamily}) : new List<long>();
			//if isInvoice or if it's a LimitedStatement and no paysplits or procs were selected there will be 0 rows and this loop will be skipped
			for(int i=0;i<rawPay.Rows.Count;i++) {
				long rowPatNum=PIn.Long(rawPay.Rows[i]["PatNum"].ToString());
				//Skip payments where the patnum (this is either paysplit.PatNum or payment.PatNum) is not in the current family OR superfamily.
				if(!ListTools.In(rowPatNum,fam.ListPats.Select(x => x.PatNum).Concat(listSuperFamPatNums))) {
					continue;
				}
				//There are 'fake' payment rows created for the scenario where a payment was made to a patient in a different family / super family.
				bool isOutOfFamily=PIn.Bool(rawPay.Rows[i]["isOutOfFamily_"].ToString());
				long payNum=PIn.Long(rawPay.Rows[i]["PayNum"].ToString());
				if(isOutOfFamily && ListTools.In(payNum,listPaySplits.Select(x => x.PayNum))) {
					//The 'PaidToOtherFamily' query may create extra payment rows if the payment was split to someone inside and someone outside the 
					//family.  Remove these duplicate rows here.
					continue;
				}
				string strDescript="";
				if(rawPay.Rows[i]["PayPlanNum"].ToString()!="0" && !isOutOfFamily){
					if(payPlanVersionCur==PayPlanVersions.DoNotAge || payPlanVersionCur==PayPlanVersions.AgeCreditsOnly) {
						continue;//in v1&3, do not add rows that are attached to payment plans
					}
					else {
						strDescript=" "+Lans.g("ContrAccount","(Attached to payment plan)"); //in v2, add the rows and show that they are attached to a payplan.
					}
				}
				if(doExcludeTxfrs && rawPay.Rows[i]["PayType"].ToString()=="0") {
					continue;
				}
				double hiddenPaySplitAmountTotal=listPaySplits.Where(x => 
					listHiddenUnearnedDefNums.Contains(x.UnearnedType) && x.PayNum==PIn.Long(rawPay.Rows[i]["PayNum"].ToString()
				)).Sum(x => x.SplitAmt);
				row=table.NewRow();
				row["AbbrDesc"]="";
				row["AdjNum"]=0;
				row["balance"]="";//fill this later
				row["balanceDouble"]=0;//fill this later
				row["chargesDouble"]=0;
				row["charges"]="";
				row["ClaimNum"]=0;
				row["ClaimPaymentNum"]="0";
				long clinicNumCur=PIn.Long(rawPay.Rows[i]["ClinicNum"].ToString());
				row["clinic"]=Clinics.GetDesc(clinicNumCur);
				row["ClinicNum"]=clinicNumCur;
				row["colorText"]=listDefs[3].ItemColor.ToArgb().ToString();
				amt=PIn.Decimal(rawPay.Rows[i]["splitAmt_"].ToString());
				row["creditsDouble"]=amt-(decimal)hiddenPaySplitAmountTotal;
				row["credits"]=((decimal)row["creditsDouble"]).ToString("n");
				dateT=PIn.DateT(rawPay.Rows[i]["DatePay"].ToString());//was ProcDate in earlier versions
				row["DateTime"]=dateT;
				row["date"]=dateT.ToString(Lans.GetShortDateTimeFormat());
				row["dateTimeSort"]=PIn.DateT(rawPay.Rows[i]["SecDateTEdit"].ToString());//MAX SecDateTEdit will be used for sorting if RandomKeys is enabled
				row["description"]=Defs.GetName(DefCat.PaymentTypes,PIn.Long(rawPay.Rows[i]["PayType"].ToString()));
				if(rawPay.Rows[i]["CheckNum"].ToString()!=""){
					row["description"]+=" #"+rawPay.Rows[i]["CheckNum"].ToString();
				}
				payamt=PIn.Decimal(rawPay.Rows[i]["PayAmt"].ToString());
				row["description"]+=" "+payamt.ToString("c");
				if(rawPay.Rows[i]["PatNum"].ToString() != rawPay.Rows[i]["patNumPayment_"].ToString()){
					row["description"]+=" ("+Lans.g("ContrAccount","Paid by ")
						+fam.GetNameInFamFirstOrPreferredOrLast(PIn.Long(rawPay.Rows[i]["patNumPayment_"].ToString()))+")";
				}
				if(payamt!=amt) {
					//Both Payment query 1 andPayment query 3 have the same 'where' statements.
					//payNum is pulled from Payment query 1 UNION Payment query 2.
					//So if payNum does not exist in listPaysplits then it must be coming from Payment query 2.
					//Payment query 2 rows represent payments that were split to another family.
					if(ListTools.In(payNum,listPaySplits.Select(x => x.PayNum))) {
						row["description"]+=" "+Lans.g("ContrAccount","(split)");
					}
					else {//That is not in the list of paysplits to this family.
						row["description"]+=" "+Lans.g("ContrAccount","(split to another family)");
					}
				}
				if(rawPay.Rows[i]["UnearnedType"].ToString()!="0") {
					row["description"]+=" - "+Defs.GetName(DefCat.PaySplitUnearnedType,PIn.Long(rawPay.Rows[i]["UnearnedType"].ToString()));
				}
				if(rawPay.Rows[i]["PayType"].ToString()=="0") {//if a txfr, clear the description
					row["description"]="";
				}
				//we might use DatePay here to add to description
				if(rawPay.Rows[i]["PayNote"].ToString() !="" && showPayNotes) {
					if(rawPay.Rows[i]["PayType"].ToString()!="0") {//if not a txfr
						row["description"]+="\r\n";
					}
					row["description"]+=rawPay.Rows[i]["PayNote"].ToString();
				}
				if(PrefC.GetBool(PrefName.AccountShowPaymentNums)) {
					row["description"]+="\r\n"+Lans.g("AccountModule","Payment Number: ")+rawPay.Rows[i]["PayNum"].ToString();
				}
				row["description"]+=strDescript;
				long patNumCur=PIn.Long(rawPay.Rows[i]["PatNum"].ToString());
				//The following code is very old and would never do anything, commenting out per Allen and Nathan
				//who decided we will wait for someone to complain about this before deciding it is a bug.
				//string patname=fam.GetNameInFamFirst(patNumCur);
				//if(isReseller) {
				//	patname=fam.GetNameInFamLF(patNumCur);
				//}
				//row["patient"]=patname;
				row["patient"]=fam.GetNameInFamFirst(patNumCur);
				row["PatNum"]=patNumCur;
				long payNumCur=PIn.Long(rawPay.Rows[i]["PayNum"].ToString());
				row["paymentsOnObj"]=string.Join(",",payNumCur);
				row["PayNum"]=payNumCur;
				row["PayPlanNum"]=0;
				row["PayPlanChargeNum"]="0";
				if(rawPay.Rows[i]["PayType"].ToString()=="0") {//if a txfr
					row["ProcCode"]=Lans.g("AccountModule","Txfr");
				}
				else {
					row["ProcCode"]=Lans.g("AccountModule","Pay");
				}
				row["ProcNum"]="0";
				row["ProcNumLab"]="";
				row["procsOnObj"]=rawPay.Rows[i]["ProcNums_"];
				row["adjustsOnObj"]=rawPay.Rows[i]["AdjNums_"];
				//Odd that this shows only one provider on the payment when there could be multiple, but there is no easy way to fix this currently.
				row["prov"]=Providers.GetAbbr(PIn.Long(rawPay.Rows[i]["ProvNum"].ToString()));
				row["signed"]="";
				row["StatementNum"]=0;
				row["ToothNum"]="";
				row["ToothRange"]="";
				row["tth"]="";
				rows.Add(row);
			}
			#endregion Paysplits
			#region Claims
			//claims (do not affect balance)-------------------------------------------------------------------------
			string whereAndClause="";//create whereAndClause variable so both queries get the same claims
			if(familyPatNums!="") {
				whereAndClause="AND claim.PatNum IN ("+familyPatNums+") ";
			}
			command="SELECT CarrierName,ClaimFee,claim.ClaimNum,ClaimStatus,ClaimType,claim.ClinicNum,DateReceived,DateService,"
				+"claim.DedApplied,claim.InsPayEst,claim.InsPayAmt,claim.PatNum,ProvTreat,claim.ReasonUnderPaid,claim.WriteOff,"
				+DbHelper.GroupConcat("claimproc.ProcNum",distinct:true)+" ProcNums_,MAX(claim.SecDateTEdit) SecDateTEdit "
				+"FROM claim "
				+"LEFT JOIN insplan ON claim.PlanNum=insplan.PlanNum "
				+"LEFT JOIN carrier ON carrier.CarrierNum=insplan.CarrierNum "
				+"INNER JOIN claimproc ON claimproc.ClaimNum=claim.ClaimNum "
				+"WHERE ClaimType != 'PreAuth' "
				+whereAndClause
				+"GROUP BY carrier.CarrierName,claim.ClaimNum,claim.ClaimFee,claim.ClaimStatus,claim.ClaimType,"
				+"claim.ClinicNum,claim.DateReceived,claim.DateService,claim.DedApplied,claim.InsPayEst,claim.InsPayAmt,"
				+"claim.PatNum,claim.ProvTreat,claim.ReasonUnderPaid,claim.WriteOff";
			DataTable rawClaim=new DataTable();
			if(!isInvoice && stmt.StatementType!=StmtType.LimitedStatement) {
				rawClaim=dcon.GetTable(command);
				rawClaim.Columns.Add(new DataColumn("procAmt_"));
				rawClaim.Columns.Add(new DataColumn("adjAmt_"));
			}
			//Select the claimprocs attached to claims for this patient using the same list of pats from family where the claim is not a preauth
			//and there is a ProcNum on the claimproc.  Used to highlight the procs attached to a claim when the claim is selected.
			command="SELECT claim.ClaimNum,claimproc.ProcNum,ProcFee*(BaseUnits+UnitQty) procAmt_ "
				+"FROM claim "
				+"INNER JOIN claimproc ON claimproc.ClaimNum=claim.ClaimNum AND claimproc.ProcNum>0 "
				+"LEFT JOIN procedurelog ON procedurelog.ProcNum=claimproc.ProcNum " 
				+"WHERE claim.ClaimType!='PreAuth' "
				+whereAndClause;
			DataTable rawProcNumsClaim=new DataTable();
			if(!isInvoice && stmt.StatementType!=StmtType.LimitedStatement) {
				rawProcNumsClaim=dcon.GetTable(command);
			}
			foreach(DataRow rcRow in rawClaim.Rows) {//rawClaim will have 0 rows if isInvoice or StatementType is LimitedStatement
				if(rawProcNumsClaim.Rows.Count==0) {
					break;
				}
				List<DataRow> listRowMatches=rawProcNumsClaim.Rows.OfType<DataRow>().ToList().FindAll(x => x["ClaimNum"].ToString()==rcRow["ClaimNum"].ToString());
				Dictionary<long,List<DataRow>> dictProcNumRows=listRowMatches.GroupBy(x => x.Field<long>("ProcNum"))
					.ToDictionary(x => x.Key,x => x.ToList());
				if(dictProcNumRows.Count>0) {
					rcRow["ProcNums_"]=string.Join(",",dictProcNumRows.Keys);
					decimal procAmt_=0;
					foreach(long key in dictProcNumRows.Keys) {
						DataRow procRow=dictProcNumRows[key].First();
						procAmt_+=PIn.Decimal(procRow["procAmt_"].ToString());
					}
					rcRow["procAmt_"]=procAmt_;
					//Take every unique ProcNum and get every adjustment associated.
					rcRow["adjAmt_"]=Adjustments.GetTotForProcs(dictProcNumRows.Keys.ToList());
				}
			}
			DateTime daterec;
			decimal amtpaid;//can be different than amt if claims show UCR.
			decimal procAmt;
			decimal insest;
			decimal deductible;
			decimal patport;
			adjAmt=0;
			string claimStatus;
			for(int i=0;i<rawClaim.Rows.Count;i++){//rawClaim will have 0 rows if isInvoice or StatementType is LimitedStatement
				row=table.NewRow();
				row["AbbrDesc"]="";
				row["AdjNum"]=0;
				row["balance"]="";//fill this later
				row["balanceDouble"]=0;//fill this later
				row["chargesDouble"]=0;
				row["charges"]="";
				row["ClaimNum"]=PIn.Long(rawClaim.Rows[i]["ClaimNum"].ToString());
				row["ClaimPaymentNum"]="0";
				long clinicNumCur=PIn.Long(rawClaim.Rows[i]["ClinicNum"].ToString());
				row["clinic"]=Clinics.GetDesc(clinicNumCur);
				row["ClinicNum"]=clinicNumCur;
				row["colorText"]=listDefs[4].ItemColor.ToArgb().ToString();
					//might be changed lower down based on claim status
				row["creditsDouble"]=0;
				row["credits"]="";
				dateT=PIn.DateT(rawClaim.Rows[i]["DateService"].ToString());
				row["DateTime"]=dateT;
				row["date"]=dateT.ToString(Lans.GetShortDateTimeFormat());
				row["dateTimeSort"]=PIn.DateT(rawClaim.Rows[i]["SecDateTEdit"].ToString());//MAX SecDateTEdit will be used for sorting if RandomKeys is enabled
				if(rawClaim.Rows[i]["ClaimType"].ToString()=="P"){
					row["description"]=Lans.g("ContrAccount","Pri")+" ";
				}
				else if(rawClaim.Rows[i]["ClaimType"].ToString()=="S"){
					row["description"]=Lans.g("ContrAccount","Sec")+" ";
				}
				//else if(rawClaim.Rows[i]["ClaimType"].ToString()=="PreAuth"){
				//	row["description"]=Lans.g("ContrAccount","PreAuth")+" ";
				//}
				else if(rawClaim.Rows[i]["ClaimType"].ToString()=="Other"){
					row["description"]="";
				}
				else if(rawClaim.Rows[i]["ClaimType"].ToString()=="Cap"){
					row["description"]=Lans.g("ContrAccount","Cap")+" ";
				}
				amt=PIn.Decimal(rawClaim.Rows[i]["ClaimFee"].ToString());
				row["description"]+=Lans.g("ContrAccount","Claim")+" "+amt.ToString("c")+" "
					+rawClaim.Rows[i]["CarrierName"].ToString();
				daterec=PIn.DateT(rawClaim.Rows[i]["DateReceived"].ToString());
				claimStatus=rawClaim.Rows[i]["ClaimStatus"].ToString();
				if(claimStatus=="R"){
					row["description"]+="\r\n"+Lans.g("ContrAccount","Received")+" ";
					if(daterec.Year<1880){
						row["description"]+=Lans.g("ContrAccount","(no date)");//although I don't think UI allows this
					}
					else{
						row["description"]+=daterec.ToShortDateString();
					}
					row["colorText"] = listDefs[8].ItemColor.ToArgb().ToString();
				} 
				else if(claimStatus=="U"){
					row["description"]+="\r\n"+Lans.g("ContrAccount","Unsent");
				} 
				else if(claimStatus=="H"){
					row["description"]+="\r\n"+Lans.g("ContrAccount","Hold until Pri received");
				} 
				else if(claimStatus=="W"){
					row["description"]+="\r\n"+Lans.g("ContrAccount","Waiting to Send");
				} 
				else if(claimStatus=="S"){
					row["description"]+="\r\n"+Lans.g("ContrAccount","Sent");
				}
				decimal claimLabFeeTotalAmt=0;
				//For Canada, add lab fee amounts into total claim amount.
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
					string[] arrayProcNumsForClaim=PIn.ByteArray(rawClaim.Rows[i]["ProcNums_"]).Split(',').Distinct().ToArray();
					for(int j=0;j<arrayProcNumsForClaim.Length;j++) {
						long procNum=PIn.Long(arrayProcNumsForClaim[j]);
						if(procNum==0) {//ProcNum will be 0 for Total Payments on claims.
							continue;
						}
						for(int k=0;k<rawProc.Rows.Count;k++) {//For each procedure attached to the claim, add the lab fees into the total amount. The lab fees show in the account because they are complete.
							long procNumLab=PIn.Long(rawProc.Rows[k]["ProcNumLab"].ToString());
							if(procNumLab==procNum) {
								claimLabFeeTotalAmt+=PIn.Decimal(rawProc.Rows[k]["ProcFee"].ToString());
							}
						}
					}
				}
				if(claimLabFeeTotalAmt>0) {
					row["description"]+="\r\n"+Lans.g("ContrAccount","Lab Fees")+" "+claimLabFeeTotalAmt.ToString("c");
				}
				procAmt=PIn.Decimal(rawClaim.Rows[i]["procAmt_"].ToString());
				adjAmt=PIn.Decimal(rawClaim.Rows[i]["adjAmt_"].ToString());
				insest=PIn.Decimal(rawClaim.Rows[i]["InsPayEst"].ToString());
				amtpaid=PIn.Decimal(rawClaim.Rows[i]["InsPayAmt"].ToString());
				writeoff=PIn.Decimal(rawClaim.Rows[i]["WriteOff"].ToString());
				deductible=PIn.Decimal(rawClaim.Rows[i]["DedApplied"].ToString());
				if(!PrefC.GetBool(PrefName.BalancesDontSubtractIns) 
					&& (claimStatus=="W" || claimStatus=="S")
					&& rawClaim.Rows[i]["ClaimType"].ToString()!="Cap")
				{
					if (amtpaid != 0 && ((insest - amtpaid) >= 0)) {//show additional info on resubmits
						row["description"] += "\r\n" + Lans.g("ContrAccount", "Remaining Est. Payment Pending:") + " " + (insest - amtpaid).ToString("c");
					}
					else {
						row["description"] += "\r\n" + Lans.g("ContrAccount", "Estimated Payment Pending:") + " " + insest.ToString("c");
					}
				}
				if(rawClaim.Rows[i]["ClaimType"].ToString()!="Cap"){
					if(amtpaid != 0){
						row["description"]+="\r\n"+Lans.g("ContrAccount","Payment:")+" "+amtpaid.ToString("c");
					} 
					else if(amtpaid==0 && (claimStatus=="R")){
						row["description"]+="\r\n"+Lans.g("ContrAccount", "NO PAYMENT");
					}
				}
				if(writeoff!=0) {
					string writeoffDesctipt=PrefC.GetString(PrefName.InsWriteoffDescript);
					if(writeoffDesctipt=="") {
						writeoffDesctipt=Lans.g("ContrAccount","Writeoff");
					}
					row["description"]+="\r\n"+writeoffDesctipt+": "+writeoff.ToString("c");
				}
				if(deductible!=0){
					row["description"]+="\r\n"+Lans.g("ContrAccount","Deductible Applied:")+" "+deductible.ToString("c");
				}
				if(!PrefC.GetBool(PrefName.BalancesDontSubtractIns) 
					&&	(claimStatus=="W" || claimStatus=="S")
					&& rawClaim.Rows[i]["ClaimType"].ToString()!="Cap")
				{
					patport=procAmt-insest-writeoff+adjAmt; //Add amtAdj
					if(patport<0){
						patport=0;
					}
					row["description"]+="\r\n"+Lans.g("ContrAccount","Est. Patient Portion:")+" "+patport.ToString("c");
				}
				if(rawClaim.Rows[i]["ReasonUnderPaid"].ToString()!=""){
					row["description"]+="\r\n"+rawClaim.Rows[i]["ReasonUnderPaid"].ToString();
				}
				//row["extraDetail"]="";
				long patNumCur=PIn.Long(rawClaim.Rows[i]["PatNum"].ToString());
				row["patient"]=GetPatName(patNumCur,fam,false);
				row["PatNum"]=patNumCur;
				row["PayNum"]=0;
				row["PayPlanNum"]=0;
				row["PayPlanChargeNum"]="0";
				row["ProcCode"]=Lans.g("AccountModule","Claim");
				row["ProcNum"]="0";
				row["ProcNumLab"]="";
				row["procsOnObj"]=rawClaim.Rows[i]["ProcNums_"];
				row["adjustsOnObj"]="";
				row["prov"]=Providers.GetAbbr(PIn.Long(rawClaim.Rows[i]["ProvTreat"].ToString()));
				row["signed"]="";
				row["StatementNum"]=0;
				row["ToothNum"]="";
				row["ToothRange"]="";
				row["tth"]="";
				rows.Add(row);
			}
			#endregion Claims
			#region Statements
			//Statement----------------------------------------------------------------------------------------
			List<long> listPatNums=fam.ListPats.ToList().Select(x => x.PatNum).Distinct().ToList();
			command="SELECT DateSent,IsSent,Mode_,StatementNum,PatNum,Note,NoteBold,IsInvoice,SuperFamily,DateTStamp "
				+"FROM statement "
				+"WHERE (PatNum IN ("+string.Join(",",listPatNums)+") ";
			//Always include all statements from the super family if a super family is set.  They will be filtered out later.
			if(fam.ListPats[0].SuperFamily > 0) {
				command+="OR SuperFamily ="+POut.Long(fam.ListPats[0].SuperFamily);//Get all statements for the superfamily as well.
			}
			command+=") ";
			if(statementNum>0) {
				command+="AND StatementNum != "+POut.Long(statementNum);
			}
			DataTable rawState=new DataTable();
			if(!isInvoice && stmt.StatementType!=StmtType.LimitedStatement) {
				rawState=dcon.GetTable(command);
			}
			StatementMode _mode;
			//if we are getting a DataSet for a super statement and this guar in the super family is not the super head, skip super statement rows
			bool isSuperStmtSkipped=(isForStatementPrinting && stmt.SuperFamily>0 && stmt.SuperFamily!=pat.PatNum);
			foreach(DataRow rowCur in rawState.Rows) {//rawState will have 0 rows if isInvoice or StatementType is LimitedStatement
				long patNumCur=PIn.Long(rowCur["PatNum"].ToString());
				long superFamNum=PIn.Long(rowCur["SuperFamily"].ToString());
				if(isSuperStmtSkipped && superFamNum>0 && superFamNum!=pat.PatNum) {//skip super stmt rows for all members of super fam except for the super head
					continue;
				}
				row=table.NewRow();
				row["AbbrDesc"]="";
				row["AdjNum"]=0;
				row["balance"]="";//fill this later
				row["balanceDouble"]=0;//fill this later
				row["chargesDouble"]=0;
				row["charges"]="";
				row["ClaimNum"]=0;
				row["ClaimPaymentNum"]="0";
				row["clinic"]="";
				row["ClinicNum"]="0";
				row["colorText"]=listDefs[5].ItemColor.ToArgb().ToString();
				row["creditsDouble"]=0;
				row["credits"]="";
				dateT=PIn.DateT(rowCur["DateSent"].ToString());
				row["DateTime"]=dateT;
				row["date"]=dateT.ToString(Lans.GetShortDateTimeFormat());
				row["dateTimeSort"]=PIn.DateT(rowCur["DateTStamp"].ToString());//DateTStamp will be used for sorting if RandomKeys is enabled
				if(rowCur["IsInvoice"].ToString()=="0") {//not an invoice
					row["description"]+=Lans.g("ContrAccount","Statement");
				}
				else {//Must be invoice
					row["description"]+=Lans.g("ContrAccount","Invoice")+" #"+rowCur["StatementNum"].ToString();
				}
				_mode=(StatementMode)PIn.Long(rowCur["Mode_"].ToString());
				row["description"]+="-"+Lans.g("enumStatementMode",_mode.ToString());
				if(rowCur["IsSent"].ToString()=="0"){
					row["description"]+=" "+Lans.g("ContrAccount","(unsent)");
				}
				row["patient"]=GetPatName(patNumCur,fam,isReseller);
				row["PatNum"]=patNumCur;
				row["PayNum"]=0;
				row["PayPlanNum"]=0;
				row["PayPlanChargeNum"]="0";
				row["ProcCode"]=Lans.g("AccountModule","Stmt");
				row["ProcNum"]="0";
				row["ProcNumLab"]="";
				row["procsOnObj"]="";
				row["adjustsOnObj"]="";
				row["prov"]="";
				row["signed"]="";
				row["StatementNum"]=PIn.Long(rowCur["StatementNum"].ToString());
				row["ToothNum"]="";
				row["ToothRange"]="";
				row["tth"]="";
				row["SuperFamily"]=superFamNum.ToString();
				rows.Add(row);
			}
			#endregion Statements
			#region Payment Plans
			//Payment plans----------------------------------------------------------------------------------
			//get all payment plans for members of this family. For V1, this means that the payplan row in the ledger will correctly appear as a credit.
			//this also helps populate the payment plans grid in the account module by getting passed into GetPayPlans().
			command="SELECT SUM(COALESCE(payplancharge.Principal,0)) principal_,SUM(COALESCE(payplancharge.Interest,0)) interest_,"
				+"SUM(CASE WHEN payplancharge.ChargeDate<="+DbHelper.Curdate()+" THEN payplancharge.Principal ELSE 0 END) principalDue_,"
				+"SUM(CASE WHEN payplancharge.ChargeDate<="+DbHelper.Curdate()+" THEN payplancharge.Interest ELSE 0 END) interestDue_,"
				+"MAX(carrier.CarrierName) CarrierName,payplan.CompletedAmt,payplan.Guarantor,payplan.PatNum,payplan.PayPlanDate,payplan.PayPlanNum,"
				+"payplan.APR,payplan.DownPayment,payplan.ChargeFrequency,payplan.PayAmt,payplan.DateInterestStart,payplan.DatePayPlanStart,"
				+"payplan.PlanNum,payplan.IsClosed,payplan.PlanCategory,MAX(payplancharge.SecDateTEntry) SecDateTEntry, "
				+"COALESCE(MAX(payplancharge.ClinicNum),0) ClinicNum,payplan.IsDynamic "
				+"FROM payplan "
				+"LEFT JOIN payplancharge ON payplancharge.PayPlanNum=payplan.PayPlanNum "
					+"AND payplancharge.ChargeType="+POut.Int((int)PayPlanChargeType.Debit)+" "
				+"LEFT JOIN insplan ON insplan.PlanNum=payplan.PlanNum "
				+"LEFT JOIN carrier ON carrier.CarrierNum=insplan.CarrierNum "
				+"WHERE payplan.PatNum IN ("+familyPatNums+") OR payplan.Guarantor IN ("+familyPatNums+") "
				+"GROUP BY payplan.PayPlanNum";
			if(DataConnection.DBtype==DatabaseType.Oracle){
				command+=",payplan.CompletedAmt,payplan.Guarantor,payplan.PatNum,payplan.PayPlanDate,payplan.PlanNum,"
					+"payplan.IsClosed,payplan.PlanCategory";
			}
			DataTable rawPayPlan=new DataTable();
			if(!isInvoice && stmt.StatementType!=StmtType.LimitedStatement) {
				rawPayPlan=dcon.GetTable(command);
			}
			if(payPlanVersionCur==PayPlanVersions.DoNotAge) {
				//0 rows if isInvoice or statement type is LimitedStatement.  In spite of this, the payment plans breakdown will still show at the top of invoices.
				for(int i=0;i<rawPayPlan.Rows.Count;i++) {
					//Version 1. If the payment plan's patnum isn't in the current family, then skip. We only want it to show as a credit for the patient of the payment plan.
					if(!fam.ListPats.Select(x => x.PatNum).Contains(PIn.Long(rawPayPlan.Rows[i]["PatNum"].ToString()))){
						continue;
					}
					row=table.NewRow();
					row["AbbrDesc"]="";
					row["AdjNum"]=0;
					row["balance"]="";//fill this later
					row["balanceDouble"]=0;//fill this later
					row["chargesDouble"]=0;
					row["charges"]="";
					row["ClaimNum"]=0;
					row["ClaimPaymentNum"]="0";
					long clinicNumCur=PIn.Long(rawPayPlan.Rows[i]["ClinicNum"].ToString());
					row["clinic"]=Clinics.GetDesc(clinicNumCur);
					row["ClinicNum"]=clinicNumCur;
					row["colorText"]=listDefs[6].ItemColor.ToArgb().ToString();
					amt=PIn.Decimal(rawPayPlan.Rows[i]["CompletedAmt"].ToString());
					row["creditsDouble"]=amt;
					row["credits"]=((decimal)row["creditsDouble"]).ToString("n");
					dateT=PIn.DateT(rawPayPlan.Rows[i]["PayPlanDate"].ToString());
					row["DateTime"]=dateT;
					row["date"]=dateT.ToString(Lans.GetShortDateTimeFormat());
					row["dateTimeSort"]=PIn.DateT(rawPayPlan.Rows[i]["SecDateTEntry"].ToString());//MAX SecDateTEntry will be used for sorting if RandomKeys is enabled
					if(rawPayPlan.Rows[i]["PlanNum"].ToString()=="0"){
						row["description"]=Lans.g("ContrAccount","Payment Plan");
					}
					else{
						row["description"]=Lans.g("ContrAccount","Expected payments from ")
							+rawPayPlan.Rows[i]["CarrierName"].ToString();
					}
					//row["extraDetail"]="";
					long patNumCur=PIn.Long(rawPayPlan.Rows[i]["PatNum"].ToString());
					row["patient"]=GetPatName(patNumCur,fam,isReseller);
					row["PatNum"]=patNumCur;
					row["PayNum"]=0;
					row["PayPlanNum"]=PIn.Long(rawPayPlan.Rows[i]["PayPlanNum"].ToString());
					row["PayPlanChargeNum"]="0";
					row["ProcCode"]=Lans.g("AccountModule","PayPln");
					row["ProcNum"]="0";
					row["ProcNumLab"]="";
					row["procsOnObj"]="";
					row["adjustsOnObj"]="";
					row["prov"]="";
					row["signed"]="";
					row["StatementNum"]=0;
					row["ToothNum"]="";
					row["ToothRange"]="";
					row["tth"]="";
					rows.Add(row);
				}
			}
			#endregion Payment Plans
			#region Payment Plans Version 2 - Credits and Debits
			if(payPlanVersionCur==PayPlanVersions.AgeCreditsAndDebits) { //this information is only required for v2
				string patnums=familyPatNums;
				List<Patient> listFamilyMembers=fam.ListPats.ToList();
				//If we are make a superfamily invoice, we want to grab the payplan charges from all family members instead of just the superguarantor.
				if(stmt.SuperFamily!=0) {
					List<Patient> listSuperFamilyMembers=Patients.GetBySuperFamily(stmt.SuperFamily);
					if(stmt.IsInvoice) {
						patnums=String.Join(",",listSuperFamilyMembers.Select(x => POut.Long(x.PatNum)));
					}	
					listFamilyMembers=listSuperFamilyMembers;	
				}				
				command="SELECT payplancharge.ChargeDate,payplancharge.PatNum,payplancharge.Guarantor,payplancharge.ProvNum,payplancharge.ClinicNum,"
					+"payplancharge.Note,payplancharge.Principal,payplancharge.ChargeType,payplancharge.Interest,payplancharge.PayPlanNum,payplan.IsClosed,"
					+"payplancharge.PayPlanChargeNum,payplancharge.ProcNum,payplan.PlanNum,carrier.CarrierName,payplan.PlanCategory,payplancharge.StatementNum,"
					+"payplancharge.SecDateTEntry "
					+"FROM payplancharge "
					+"INNER JOIN payplan ON payplan.PayPlanNum = payplancharge.PayPlanNum "
					+"LEFT JOIN insplan ON insplan.PlanNum = payplan.PlanNum "
					+"LEFT JOIN carrier ON carrier.CarrierNum = insplan.CarrierNum "
					+"WHERE (payplancharge.Guarantor IN ("+patnums+") OR payplancharge.PatNum IN ("+patnums+")) ";
				if(isInvoice) {
					command+="AND payplancharge.ChargeDate<="+DbHelper.DateAddMonth(DbHelper.Curdate(),"3")+" "
						+"AND payplancharge.StatementNum="+POut.Long(statementNum)+" ";
				}
				else {
					command+="AND payplancharge.ChargeDate<="+DbHelper.Curdate()+" ";
				}
				DataTable rawPayPlan2=new DataTable();
				if(stmt.StatementType!=StmtType.LimitedStatement) {
					rawPayPlan2=dcon.GetTable(command);
				}
				//0 rows if isInvoice or statement type is LimitedStatement.  In spite of this, the payment plans breakdown will still show at the top of invoices.
				for(int i=0;i<rawPayPlan2.Rows.Count;i++) {
					PayPlanChargeType chargetype=PIn.Enum<PayPlanChargeType>(rawPayPlan2.Rows[i]["ChargeType"].ToString());
					if(rawPayPlan2.Rows[i]["PlanNum"].ToString()!="0" && chargetype==PayPlanChargeType.Debit) {
						//debits attached to insurance payplans do not get shown in the account module.
						continue;
					}
					long guarNumCur=PIn.Long(rawPayPlan2.Rows[i]["Guarantor"].ToString());
					if(chargetype == PayPlanChargeType.Debit
						&& !listFamilyMembers.Select(x => x.PatNum).Contains(guarNumCur)) 
					{
						//Any debit of which no one in the current family is the guarantor should be skipped. 
						//Credits get to pass, as they are posted to the patient of the payplan.
						continue;
					}
					row=table.NewRow();
					row["AbbrDesc"]="";
					row["AdjNum"]=0;
					row["balance"]="";//fill this later
					row["balanceDouble"]=0;//fill this later
					row["chargesDouble"]=0;
					amt=PIn.Decimal(rawPayPlan2.Rows[i]["Principal"].ToString());
					if(chargetype==PayPlanChargeType.Debit) {//show principle amount as a charge if it's a debit chargeType.
						amt+=PIn.Decimal(rawPayPlan2.Rows[i]["Interest"].ToString());
						row["chargesDouble"]=amt;
						row["charges"]=amt.ToString("n");
						row["creditsDouble"]=0;
						row["credits"]="";
					}
					else if(chargetype==PayPlanChargeType.Credit) {//show principle amount as a credit if it's a credit chargeType.
						row["chargesDouble"]=0;
						row["charges"]="";
						row["creditsDouble"]=amt;
						row["credits"]=amt.ToString("n");
					}
					row["ClaimNum"]=0;
					row["ClaimPaymentNum"]="0";
					long clinicNumCur=PIn.Long(rawPayPlan2.Rows[i]["ClinicNum"].ToString());
					row["clinic"]=Clinics.GetDesc(clinicNumCur);
					row["ClinicNum"]=clinicNumCur;
					row["colorText"]=listDefs[6].ItemColor.ToArgb().ToString();
					dateT=PIn.DateT(rawPayPlan2.Rows[i]["ChargeDate"].ToString());
					row["DateTime"]=dateT;
					row["date"]=dateT.ToString(Lans.GetShortDateTimeFormat());
					row["dateTimeSort"]=PIn.DateT(rawPayPlan2.Rows[i]["SecDateTEntry"].ToString());//SecDateTEntry will be used for sorting if RandomKeys is enabled
					if(rawPayPlan2.Rows[i]["PlanNum"].ToString()=="0") { //not an insurance payplan
						row["description"]=rawPayPlan2.Rows[i]["Note"].ToString();
					}
					else {//an insurance payplan
						row["description"]=Lans.g("ContrAccount","Expected payments from ")
							+rawPayPlan2.Rows[i]["CarrierName"].ToString();
					}
					//row["extraDetail"]="";
					long patNumCur=PIn.Long(rawPayPlan2.Rows[i]["PatNum"].ToString());
					row["patient"]=GetPatName(patNumCur,fam,(isReseller || doIncludePatLName));
					if(rawPayPlan2.Rows[i]["PlanNum"].ToString()=="0") { //not an insurance payplan
						//The guarantor on the payplancharge is always set to the account it should appear in for patient payment plans.
						row["PatNum"]=guarNumCur;
					}
					else {//an insurance payplan
						//For insurance payment plans, the charges should appear on the patient's account.
						row["PatNum"]=patNumCur;
					}
					row["PayNum"]=0;
					row["PayPlanNum"]=PIn.Long(rawPayPlan2.Rows[i]["PayPlanNum"].ToString());
					row["PayPlanChargeNum"]=rawPayPlan2.Rows[i]["PayPlanChargeNum"].ToString();
					row["ProcCode"]=Lans.g("AccountModule","PayPln:")+" "+chargetype;
					row["ProcNum"]="0";
					row["ProcNumLab"]="";
					row["procsOnObj"]=rawPayPlan2.Rows[i]["ProcNum"].ToString();
					row["adjustsOnObj"]="";
					row["prov"]=Providers.GetAbbr(PIn.Long(rawPayPlan2.Rows[i]["ProvNum"].ToString()));
					row["signed"]="";
					row["StatementNum"]=0;
					row["ToothNum"]="";
					row["ToothRange"]="";
					row["tth"]="";
					rows.Add(row);
				}
			}
			#endregion Payment Plans Version 2 - Credits and Debits
			#region Payment Plans Version 3 - Credits Only
			if(payPlanVersionCur==PayPlanVersions.AgeCreditsOnly) {
				command="SELECT payplancharge.ChargeDate,payplancharge.PatNum,payplancharge.Guarantor,payplancharge.ProvNum,payplancharge.ClinicNum,"
					+"payplancharge.Note,payplancharge.Principal,payplancharge.ChargeType,payplancharge.Interest,payplancharge.PayPlanNum,payplan.IsClosed,"
					+"payplancharge.PayPlanChargeNum,payplancharge.ProcNum,payplan.PlanNum,carrier.CarrierName,payplan.PlanCategory,payplancharge.SecDateTEntry "
					+"FROM payplancharge "
					+"INNER JOIN payplan ON payplan.PayPlanNum = payplancharge.PayPlanNum "
					+"LEFT JOIN insplan ON insplan.PlanNum = payplan.PlanNum "
					+"LEFT JOIN carrier ON carrier.CarrierNum = insplan.CarrierNum "
					+"WHERE (payplancharge.Guarantor IN ("+familyPatNums+") OR payplancharge.PatNum IN ("+familyPatNums+")) "
					+"AND payplancharge.ChargeType = "+POut.Int((int)PayPlanChargeType.Credit)+" "
					+"AND payplancharge.ChargeDate<="+DbHelper.Curdate();
				DataTable rawPayPlan3 = new DataTable();
				if(!isInvoice && stmt.StatementType!=StmtType.LimitedStatement) {
					rawPayPlan3=dcon.GetTable(command);
				}
				//0 rows if isInvoice or statement type is LimitedStatement.  In spite of this, the payment plans breakdown will still show at the top of invoices.
				for(int i = 0;i<rawPayPlan3.Rows.Count;i++) {
					row=table.NewRow();
					row["AbbrDesc"]="";
					row["AdjNum"]=0;
					row["balance"]="";//fill this later
					row["balanceDouble"]=0;//fill this later
					row["chargesDouble"]=0;
					amt=PIn.Decimal(rawPayPlan3.Rows[i]["Principal"].ToString());
					row["chargesDouble"]=0;
					row["charges"]="";
					row["creditsDouble"]=amt;
					row["credits"]=amt.ToString("n");
					row["ClaimNum"]=0;
					row["ClaimPaymentNum"]="0";
					long clinicNumCur=PIn.Long(rawPayPlan3.Rows[i]["ClinicNum"].ToString());
					row["clinic"]=Clinics.GetDesc(clinicNumCur);
					row["ClinicNum"]=clinicNumCur;
					row["colorText"]=listDefs[6].ItemColor.ToArgb().ToString();
					dateT=PIn.DateT(rawPayPlan3.Rows[i]["ChargeDate"].ToString());
					row["DateTime"]=dateT;
					row["date"]=dateT.ToString(Lans.GetShortDateTimeFormat());
					row["dateTimeSort"]=PIn.DateT(rawPayPlan3.Rows[i]["SecDateTEntry"].ToString());//SecDateTEntry will be used for sorting if RandomKeys is enabled
					if(rawPayPlan3.Rows[i]["PlanNum"].ToString()=="0") { //not an insurance payplan
						row["description"]=rawPayPlan3.Rows[i]["Note"].ToString();
					}
					else {//an insurance payplan
						row["description"]=Lans.g("ContrAccount","Expected payments from ")
							+rawPayPlan3.Rows[i]["CarrierName"].ToString();
					}
					//row["extraDetail"]="";
					long patNumCur=PIn.Long(rawPayPlan3.Rows[i]["PatNum"].ToString());
					row["patient"]=GetPatName(patNumCur,fam,(isReseller || doIncludePatLName));
					if(rawPayPlan3.Rows[i]["PlanNum"].ToString()=="0") { //not an insurance payplan
						//The guarantor on the payplancharge is always set to the account it should appear in for patient payment plans.
						row["PatNum"]=PIn.Long(rawPayPlan3.Rows[i]["Guarantor"].ToString());
					}
					else {//an insurance payplan
						//For insurance payment plans, the charges should appear on the patient's account.
						row["PatNum"]=patNumCur;
					}
					row["PayNum"]=0;
					row["PayPlanNum"]=PIn.Long(rawPayPlan3.Rows[i]["PayPlanNum"].ToString());
					row["PayPlanChargeNum"]=rawPayPlan3.Rows[i]["PayPlanChargeNum"].ToString();
					row["ProcCode"]=Lans.g("AccountModule","PayPln:")+" "+PIn.Enum<PayPlanChargeType>(rawPayPlan3.Rows[i]["ChargeType"].ToString());
					row["ProcNum"]="0";
					row["ProcNumLab"]="";
					row["procsOnObj"]=rawPayPlan3.Rows[i]["ProcNum"].ToString();
					row["adjustsOnObj"]="";
					row["prov"]=Providers.GetAbbr(PIn.Long(rawPayPlan3.Rows[i]["ProvNum"].ToString()));
					row["signed"]="";
					row["StatementNum"]=0;
					row["ToothNum"]="";
					row["ToothRange"]="";
					row["tth"]="";
					rows.Add(row);
				}
			}
			#endregion Payment Plans Version 3 - Credits Only
			#region Payment Plans Version 4 - No Charges
			if(payPlanVersionCur==PayPlanVersions.NoCharges) {
				//For No Charges payment plans, we DO NOT want to show the payment plan charges in the Account Module.  This is intentional.
			}
			#endregion
			#region Dynamic Payment Plans (Credits)
			decimal totPlannedFee=0;//Used to calculate the entire planned fee for a payplan.
			if(payPlanVersionCur==PayPlanVersions.AgeCreditsAndDebits || payPlanVersionCur==PayPlanVersions.AgeCreditsOnly) {
				//Dynamic payment plan charges will have been picked up by their cooresponding version query above. However, credits for dynamic payment plans
				//are not made from paymentplancharges when using dynamic payment plans,they are their own table. We need to handle the credits here 
				//according to what the user wants to see based on their payment plans version. 
				command=$@"
					SELECT payplanlink.AmountOverride,payplanlink.FKey,payplanlink.LinkType,payplanlink.SecDateTEntry,
						payplan.PatNum,payplan.dynamicPayPlanTPOption,payplan.PayPlanNum,
						prodlink.Fee,prodlink.Num,prodlink.ClinicNum,prodlink.ProvNum,prodlink.ProcStatus,prodlink.DiscountPlanAmt 
					FROM payplanlink
					INNER JOIN payplan ON payplan.PayPlanNum = payplanlink.PayPlanNum 
					LEFT JOIN (
						SELECT procedurelog.PatNum,
								(procedurelog.ProcFee*(procedurelog.UnitQty+procedurelog.BaseUnits)
									+COALESCE(procAdj.AdjAmt,0)+COALESCE(procClaimProc.InsPay,0)
									+COALESCE(procClaimProc.WriteOff,0)
									+COALESCE(procSplit.SplitAmt,0)
								) Fee, 
								procedurelog.ProcNum Num,payplanlink.LinkType,payplanlink.PayPlanLinkNum,procedurelog.ClinicNum,procedurelog.ProvNum,
								procedurelog.ProcStatus,procedurelog.DiscountPlanAmt 
						FROM payplanlink 
						INNER JOIN procedurelog ON procedurelog.ProcNum=payplanlink.FKey AND payplanlink.LinkType={POut.Int((int)PayPlanLinkType.Procedure)} 
						LEFT JOIN (
							SELECT SUM(adjustment.AdjAmt) AdjAmt,adjustment.ProcNum,adjustment.PatNum,adjustment.ProvNum,adjustment.ClinicNum
							FROM adjustment
							WHERE adjustment.PatNum IN ({familyPatNums})
							GROUP BY adjustment.ProcNum,adjustment.PatNum,adjustment.ProvNum,adjustment.ClinicNum
						)procAdj ON procAdj.ProcNum=procedurelog.ProcNum
							AND procAdj.PatNum=procedurelog.PatNum
							AND procAdj.ProvNum=procedurelog.ProvNum
							AND procAdj.ClinicNum=procedurelog.ClinicNum
						LEFT JOIN (
							SELECT SUM(COALESCE((CASE WHEN claimproc.Status IN (
									{POut.Int((int)ClaimProcStatus.Received)},{POut.Int((int)ClaimProcStatus.Supplemental)},{POut.Int((int)ClaimProcStatus.CapComplete)}
								) THEN claimproc.InsPayAmt 
								WHEN claimproc.InsEstTotalOverride!=-1 THEN claimproc.InsEstTotalOverride ELSE claimproc.InsPayEst END),0)*-1) InsPay
							,SUM(COALESCE((CASE WHEN claimproc.Status IN (
									{POut.Int((int)ClaimProcStatus.Received)},{POut.Int((int)ClaimProcStatus.Supplemental)},{POut.Int((int)ClaimProcStatus.CapComplete)}
								)	THEN claimproc.WriteOff 
								WHEN claimproc.WriteOffEstOverride!=-1 THEN claimproc.WriteOffEstOverride 
								WHEN claimproc.WriteOffEst!=-1 THEN claimproc.WriteOffEst ELSE 0 END),0)*-1) WriteOff
							,claimproc.ProcNum
							FROM claimproc 
							WHERE claimproc.Status!={POut.Int((int)ClaimProcStatus.Preauth)}
							AND claimproc.PatNum IN ({familyPatNums})
							GROUP BY claimproc.ProcNum
						)procClaimProc ON procClaimProc.ProcNum=procedurelog.ProcNum 
						LEFT JOIN (
							SELECT SUM(paysplit.SplitAmt)*-1 SplitAmt,paysplit.ProcNum
							FROM paysplit
							WHERE paysplit.PayPlanNum=0
							AND paysplit.PatNum IN ({familyPatNums})
							GROUP BY paysplit.ProcNum
						)procSplit ON procSplit.ProcNum=procedurelog.ProcNum
						UNION ALL
						SELECT adjustment.PatNum,adjustment.AdjAmt + COALESCE(adjSplit.SplitAmt,0) Fee,adjustment.AdjNum Num,payplanlink.LinkType
							,payplanlink.PayPlanLinkNum,adjustment.ClinicNum,adjustment.ProvNum,'0','0'
							FROM payplanlink 
							INNER JOIN adjustment ON adjustment.AdjNum=payplanlink.FKey 
								AND payplanlink.LinkType={POut.Int((int)PayPlanLinkType.Adjustment)}
							LEFT JOIN (
								SELECT SUM(COALESCE(paysplit.SplitAmt,0))*-1 SplitAmt,paysplit.AdjNum
								FROM paysplit
								WHERE paysplit.PayPlanNum=0
								AND paysplit.PatNum IN ({familyPatNums})
								GROUP BY paysplit.AdjNum
							)adjSplit ON adjSplit.AdjNum=adjustment.AdjNum
					) prodlink ON prodlink.PayPlanLinkNum=payplanlink.PayPlanLinkNum 
					WHERE (payplan.Guarantor IN ({familyPatNums}) OR payplan.PatNum IN ({familyPatNums}))";
				DataTable payplanLinks=new DataTable();
				if(!isInvoice && stmt.StatementType!=StmtType.LimitedStatement) {
					payplanLinks=dcon.GetTable(command);
				}
				for(int i = 0;i < payplanLinks.Rows.Count;i++) {
					row=table.NewRow();
					string num=payplanLinks.Rows[i]["Num"].ToString();
					string adjNum="";
					string procNum="";
					amt=PIn.Decimal(payplanLinks.Rows[i]["AmountOverride"].ToString());
					if(amt==0) {
						amt=PIn.Decimal(payplanLinks.Rows[i]["Fee"].ToString());
					}
					if(PIn.Enum<ProcStat>(payplanLinks.Rows[i]["ProcStatus"].ToString())!=ProcStat.C) {
						amt-=PIn.Decimal(payplanLinks.Rows[i]["DiscountPlanAmt"].ToString());
					}
					if(PIn.Int(payplanLinks.Rows[i]["LinkType"].ToString())==(int)PayPlanLinkType.Adjustment) {
						adjNum=num;
					}
					else {//Procedure Link
						procNum=num;
						if(payplanLinks.Rows[i]["dynamicPayPlanTPOption"].ToString()==((int)DynamicPayPlanTPOptions.AwaitComplete).ToString() 
							&& payplanLinks.Rows[i]["ProcStatus"].ToString()==((int)ProcStat.TP).ToString()) 
						{
							totPlannedFee+=PIn.Decimal(payplanLinks.Rows[i]["Fee"].ToString());
							amt=0;
						}
					}
					row["AbbrDesc"]="";
					row["AdjNum"]=0;
					row["balance"]="";//fill this later
					row["balanceDouble"]=0;//fill this later
					row["chargesDouble"]=0;
					row["charges"]="";
					row["creditsDouble"]=amt;
					row["credits"]=amt.ToString("n");
					row["ClaimNum"]=0;
					row["ClaimPaymentNum"]="0";
					long clinicNumCur=PIn.Long(payplanLinks.Rows[i]["ClinicNum"].ToString());
					row["clinic"]=Clinics.GetDesc(clinicNumCur);
					row["ClinicNum"]=clinicNumCur;
					row["colorText"]=listDefs[6].ItemColor.ToArgb().ToString();
					dateT=PIn.DateT(payplanLinks.Rows[i]["SecDateTEntry"].ToString());
					row["DateTime"]=dateT.Date;
					row["date"]=dateT.ToString(Lans.GetShortDateTimeFormat());
					row["dateTimeSort"]=PIn.DateT(payplanLinks.Rows[i]["SecDateTEntry"].ToString());//SecDateTEntry will be used for sorting if RandomKeys is enabled
					row["description"]="";
					long patNumCur=PIn.Long(payplanLinks.Rows[i]["PatNum"].ToString());
					row["patient"]=GetPatName(patNumCur,fam,(isReseller || doIncludePatLName));
					//For insurance payment plans, the charges should appear on the patient's account.
					row["PatNum"]=patNumCur;
					row["PayNum"]=0;
					row["PayPlanNum"]=PIn.Long(payplanLinks.Rows[i]["PayPlanNum"].ToString());
					row["PayPlanChargeNum"]=0;//may need to make a new functional column here?
					row["ProcCode"]=Lans.g("AccountModule","PayPln:")+" Credit";
					row["ProcNum"]="0";
					row["ProcNumLab"]="";
					row["procsOnObj"]=procNum;
					row["adjustsOnObj"]=adjNum;
					row["prov"]=Providers.GetAbbr(PIn.Long(payplanLinks.Rows[i]["ProvNum"].ToString()));
					row["signed"]="";
					row["StatementNum"]=0;
					row["ToothNum"]="";
					row["ToothRange"]="";
					row["tth"]="";
					rows.Add(row);
				}
			}
			SetPrincipalAndInterestForDynamicPayPlans(rawPayPlan,fam);
			#endregion
			#region Installment Plans
			//Installment plans----------------------------------------------------------------------------------
			if(statementNum==0) {
				command="SELECT * FROM installmentplan WHERE PatNum IN ("+familyPatNums+")";
				DataTable rawInstall=Db.GetTable(command);
				retVal.Tables.Add(GetPayPlans(rawPayPlan,rawPay,rawInstall,rawClaimPay,fam,pat,singlePatient));
			}
			else {
				//Always includes the payment plan breakdown for statements, receipts, and invoices.  LimitedStatements will return an empty payplan table.
				//Called twice. Once for regular pay plans and again for dynamic payplans as these now have separate grids on statements.
				retVal.Tables.Add(GetPayPlansForStatement(rawPayPlan,rawPay,fromDate,toDate,singlePatient,rawClaimPay,fam,pat,out patientPayPlanDue,
					stmt.StatementType,listPayPlanNumsExclude));
				retVal.Tables.Add(GetPayPlansForStatement(rawPayPlan,rawPay,fromDate,toDate,singlePatient,rawClaimPay,fam,pat,out dynamicPayPlanDue,
					stmt.StatementType,listPayPlanNumsExclude,true,totalPlannedFee:totPlannedFee));
			}
			#endregion Installment Plans
			//Sorting-----------------------------------------------------------------------------------------
			if(isForStatementPrinting && !intermingled) {
				rows.Sort(SortRowsForStatmentPrinting);
			}
			else {
				rows.Sort(new AccountLineComparer());
			}
			//Canadian lab procedures need to come immediately after their corresponding proc---------------------------------
			foreach(DataRow labRow in labRows) {
				int labRowIndex=rows.FindIndex(x => x["ProcNum"].ToString()==labRow["ProcNumLab"].ToString())+1;//+1 insert just after proc
				if(labRowIndex>0) {//must come after proc, so if index==0 no proc was found, don't insert
					rows.Insert(labRowIndex,labRow);
				}
			}
			//Pass off all the rows for the whole family in order to compute the patient balances----------------
			retVal.Tables.Add(GetPatientTable(fam,rows,isInvoice,stmt.StatementType));
			//Regroup rows by patient---------------------------------------------------------------------------
			DataTable[] rowsByPat=null;//will only used if multiple patients not intermingled
			if(singlePatient) {//This is usually used for Account module grid.  Always gets used for superstatements.
				Patient patGuarantor=fam.ListPats[0];
				if(!fam.IsInFamily(patNum)) {//patNum is a pat from a different family (maybe not possible, but just to retain current behavior)
					patGuarantor=Patients.GetFamily(patNum).ListPats[0];
				}
				rows.RemoveAll(x => PIn.Long(x["SuperFamily"].ToString())!=0 && !patGuarantor.HasSuperBilling);
				rows.RemoveAll(x => PIn.Long(x["SuperFamily"].ToString())==0 && PIn.Long(x["PatNum"].ToString())!=patNum);
			}
			else if(!intermingled) {//multiple patients not intermingled.  This is most common for an ordinary statement.  Never gets used with superstatements.
				rows.ForEach(x => table.Rows.Add(x));
				rowsByPat=new DataTable[fam.ListPats.Length];
				DataTable tableCur;
				for(int i=0;i<rowsByPat.Length;i++) {
					tableCur=new DataTable();
					SetTableColumns(tableCur);
					rows.FindAll(x => x["PatNum"].ToString()==fam.ListPats[i].PatNum.ToString()).ForEach(x => tableCur.ImportRow(x));
					rowsByPat[i]=tableCur;
				}
			}
			//Compute balances-------------------------------------------------------------------------------------
			decimal bal;
			if(rowsByPat==null){//just one table
				bal=0;
				foreach(DataRow rowCur in rows) {
					bal+=(decimal)rowCur["chargesDouble"];
					bal-=(decimal)rowCur["creditsDouble"];
					rowCur["balanceDouble"]=bal;
					if(rowCur["ClaimPaymentNum"].ToString()=="0" && rowCur["ClaimNum"].ToString()!="0"){//claims
						rowCur["balance"]="";
					}
					else if(rowCur["StatementNum"].ToString()=="0") {
						rowCur["balance"]=bal.ToString("n");
					}
				}
			}
			else {//family rows
				foreach(DataTable patTable in rowsByPat) {
					bal=0;
					foreach(DataRow rowCur in patTable.Rows) {
						bal+=(decimal)rowCur["chargesDouble"];
						bal-=(decimal)rowCur["creditsDouble"];
						rowCur["balanceDouble"]=bal;
						if(rowCur["ClaimPaymentNum"].ToString()=="0" && rowCur["ClaimNum"].ToString()!="0") {//claims
							rowCur["balance"]="";
						}
						else if(rowCur["StatementNum"].ToString()=="0") {
							rowCur["balance"]=bal.ToString("n");
						}
					}
				}
			}
			//Remove rows outside of daterange-------------------------------------------------------------------
			bool foundBalForward;
			long pnum=pat.Guarantor;//the patnum that should be put on the Balance foreward row.
			if(rowsByPat==null && stmt.StatementType!=StmtType.LimitedStatement) {//LimitedStatements don't have a balance forward row
				foundBalForward=false;
				for(int i=rows.Count-1;i>=0;i--) {//go backwards and remove from end
					if(((DateTime)rows[i]["DateTime"])>toDate){
						rows.RemoveAt(i);
					}
					else if(((DateTime)rows[i]["DateTime"])<fromDate){
						if(!foundBalForward){
							foundBalForward=true;
							balanceForward=(decimal)rows[i]["balanceDouble"];
						}
						rows.RemoveAt(i);
					}
				}
				//Add balance forward row
				if(foundBalForward && (rows.Count>0 || !CompareDecimal.IsZero(balanceForward))) {//don't add bal forward row if it will be the only row and there is no balance
					row=table.NewRow();
					SetBalForwardRow(row,balanceForward,pnum);
					rows.Insert(0,row);
				}
			}
			else if(stmt.StatementType!=StmtType.LimitedStatement) {//LimitedStatements don't have a balance forward row
				for(int p=0;p<rowsByPat.Length;p++){
					foundBalForward=false;
					for(int i=rowsByPat[p].Rows.Count-1;i>=0;i--) {//go backwards and remove from end
						if(((DateTime)rowsByPat[p].Rows[i]["DateTime"])>toDate){
							rowsByPat[p].Rows.RemoveAt(i);
						}
						else if(((DateTime)rowsByPat[p].Rows[i]["DateTime"])<fromDate){
							if(!foundBalForward){
								foundBalForward=true;
								balanceForward=(decimal)rowsByPat[p].Rows[i]["balanceDouble"];
							}
							long.TryParse(rowsByPat[p].Rows[i]["PatNum"].ToString(),out pnum);//using TryParse so no try-catch and pnum will be 0 if invalid
							rowsByPat[p].Rows.RemoveAt(i);
						}
					}
					//Add balance forward row
					if(foundBalForward && (rowsByPat[p].Rows.Count>0 || !CompareDecimal.IsZero(balanceForward))) {//don't add bal forward row if it will be the only row and there is no balance
						row=rowsByPat[p].NewRow();
						SetBalForwardRow(row,balanceForward,pnum);
						rowsByPat[p].Rows.InsertAt(row,0);
					}
				}
			}
			//Finally, add rows to new table(s)-----------------------------------------------------------------------
			if(rowsByPat==null){
				table.Rows.Clear();
				rows.ForEach(x => table.Rows.Add(x));
				retVal.Tables.Add(table);
			}
			else{
				DataTable tablep;
				for(int p=0;p<rowsByPat.Length;p++){
					Patient patRowCur=fam.ListPats[p];
					if(p>0 && statementNum>0 && patRowCur.PatStatus!=PatientStatus.Patient && patRowCur.EstBalance==0 ){
						continue;
					}
					tablep=new DataTable("account"+patRowCur.PatNum.ToString());
					SetTableColumns(tablep);
					rowsByPat[p].Rows.OfType<DataRow>().ToList().ForEach(x => tablep.ImportRow(x));
					retVal.Tables.Add(tablep);
				}
			}
			return retVal;
		}

		///<summary>Constructs the patient's name in the appropriate format.</summary>
		private static string GetPatName(long patNum,Family fam,bool doIncludePatLName) {
			string patName="";
			if(doIncludePatLName) {
				patName=fam.GetNameInFamLF(patNum);
			}
			else {
				patName=fam.GetNameInFamFirst(patNum);
			}
			return patName;
		}

		///<summary>Calculates and sets the correct values for rows that represent dynamic pay plans in the rawPayPlan table created in GetAccount().
		///This is needed because GetAccount() sets the principal_ and interest_ columns to the sum of charges and interest that have been posted,
		///but doesn't calculate expected charges and interest for dynamic pay plans.</summary>
		private static void SetPrincipalAndInterestForDynamicPayPlans(DataTable rawPayPlan,Family fam) {
			#region Get Data
			//Get list of PayPlanNums for dynamic pay plans from table.
			List<long> listDynamicPayPlanNums=rawPayPlan.Select()
				.Where(x => PIn.Bool(x["IsDynamic"].ToString()))
				.Select(x => PIn.Long(x["PayPlanNum"].ToString())).ToList();
			//Get all dynamic pay plans.
			List<PayPlan> listDynamicPayPlans=PayPlans.GetMany(listDynamicPayPlanNums.ToArray());
			//Get all charges for dynamic pay plans.
			Dictionary<long,List<PayPlanCharge>> dictPayPlanCharges=PayPlanCharges.GetForPayPlans(listDynamicPayPlanNums)
					.GroupBy(x => x.PayPlanNum)
					.ToDictionary(x => x.Key,x => x.ToList());
			//Get all splits for dynamic pay plans.
			Dictionary<long,List<PaySplit>> dictPaySplits=PaySplits.GetForPayPlans(listDynamicPayPlanNums)
					.GroupBy(x => x.PayPlanNum)
					.ToDictionary(x => x.Key,x => x.ToList());
			//Get all PayPlanLinks for dynamic pay plans.
			Dictionary<long,List<PayPlanLink>> dictPayPlanLinks=PayPlanLinks.GetForPayPlans(listDynamicPayPlanNums)
					.GroupBy(x => x.PayPlanNum)
					.ToDictionary(x => x.Key,x => x.ToList());
			#endregion Get Data
			foreach(PayPlan payPlanCur in listDynamicPayPlans) {
				if(!dictPayPlanCharges.TryGetValue(payPlanCur.PayPlanNum,out List<PayPlanCharge> listPayPlanChargesInDb)) {
					listPayPlanChargesInDb=new List<PayPlanCharge>();
				}
				if(!dictPaySplits.TryGetValue(payPlanCur.PayPlanNum,out List<PaySplit> listPaySplits)) {
					listPaySplits=new List<PaySplit>();
				}
				if(!dictPayPlanLinks.TryGetValue(payPlanCur.PayPlanNum,out List<PayPlanLink> listPayPlanLinks)) {
					listPayPlanLinks=new List<PayPlanLink>();
				}
				PayPlanTerms terms=PayPlanEdit.GetPayPlanTerms(payPlanCur,listPayPlanLinks);
				List<PayPlanCharge> listExpectedCharges=
					PayPlanEdit.GetListExpectedCharges(listPayPlanChargesInDb,terms,fam,listPayPlanLinks,payPlanCur,false,listPaySplits:listPaySplits);
				DataRow rowPayPlanCur=rawPayPlan.Select().First(x => PIn.Long(x["PayPlanNum"].ToString())==payPlanCur.PayPlanNum);
				rowPayPlanCur["principal_"]=terms.PrincipalAmount;
				rowPayPlanCur["interest_"]=(decimal)(listPayPlanChargesInDb.Sum(x => x.Interest)+listExpectedCharges.Sum(x => x.Interest));
			}
		}

		private static void SetBalForwardRow(DataRow row,decimal amt,long patNum){
			//No need to check RemotingRole; no call to db.
			row["AdjNum"]=0;
			row["balance"]=amt.ToString("n");
			row["balanceDouble"]=amt;
			row["chargesDouble"]=0;
			row["charges"]="";
			row["ClaimNum"]=0;
			row["ClaimPaymentNum"]="0";
			row["colorText"]=Color.Black.ToArgb().ToString();
			row["creditsDouble"]="0";
			row["credits"]="";
			row["DateTime"]=DateTime.MinValue;
			row["date"]="";
			row["dateTimeSort"]=DateTime.MinValue;
			row["description"]=Lans.g("AccountModule","Balance Forward");
			row["patient"]="";
			row["PatNum"]=patNum;
			row["PayNum"]=0;
			row["PayPlanNum"]=0;
			row["PayPlanChargeNum"]="0";
			row["ProcCode"]="";
			row["ProcNum"]="0";
			row["procsOnObj"]="";
			row["adjustsOnObj"]="";
			row["prov"]="";
			row["signed"]="";
			row["StatementNum"]=0;
			row["tth"]="";
		}

		///<summary>Gets payment plans for the family.  This defines what will show in the PayPlans grid in the account module.
		///RawPay will include any paysplits for anyone in the family plus any paysplits for payment plans being paid by someone outside the family. </summary>
		private static DataTable GetPayPlans(DataTable rawPayPlan,DataTable rawPay,DataTable rawInstall,DataTable rawClaimPay,Family fam,Patient pat
			,bool isSingle) 
		{
			//No need to check RemotingRole; no call to db.
			DataConnection dcon=new DataConnection();
			DataTable table=new DataTable("payplan");
			DataRow row;
			table.Columns.Add("accumDue");
			table.Columns.Add("balance");
			table.Columns.Add("date");
			table.Columns.Add("DateTime",typeof(DateTime));
			table.Columns.Add("dateTimeSort",typeof(DateTime));
			table.Columns.Add("due");
			table.Columns.Add("guarantor");
			table.Columns.Add("Guarantor");
			table.Columns.Add("InstallmentPlanNum");
			table.Columns.Add("IsClosed");
			table.Columns.Add("paid");
			table.Columns.Add("patient");
			table.Columns.Add("PatNum",typeof(long));
			table.Columns.Add("PayPlanNum",typeof(long));
			table.Columns.Add("PlanCategory");
			table.Columns.Add("principal");
			table.Columns.Add("princPaid");
			table.Columns.Add("totalCost");
			table.Columns.Add("type");
			List<DataRow> rows=new List<DataRow>();
			DateTime dateT;
			decimal paid;
			decimal princ;
			decimal princDue;
			decimal interestDue;
			decimal accumDue;
			decimal princPaid;
			decimal totCost;
			decimal due;
			decimal balance;
			for(int i=0;i<rawPayPlan.Rows.Count;i++) {
				long patNumCur=PIn.Long(rawPayPlan.Rows[i]["PatNum"].ToString());
				long guarNumCur=PIn.Long(rawPayPlan.Rows[i]["Guarantor"].ToString());
				if(patNumCur!=pat.PatNum && guarNumCur!=pat.PatNum && isSingle) {
					continue;//skip pay plans that are not associated to this patient/guarantor
				}
				//first, calculate the numbers-------------------------------------------------------------
				paid=0;
				for(int p=0;p<rawPay.Rows.Count;p++){
					if(rawPay.Rows[p]["PayPlanNum"].ToString()==rawPayPlan.Rows[i]["PayPlanNum"].ToString()){
						paid+=PIn.Decimal(rawPay.Rows[p]["splitAmt_"].ToString());
					}
				}
				for(int c=0;c<rawClaimPay.Rows.Count;c++) {
					if(rawClaimPay.Rows[c]["PayPlanNum"].ToString()==rawPayPlan.Rows[i]["PayPlanNum"].ToString()) {
						paid+=PIn.Decimal(rawClaimPay.Rows[c]["InsPayAmtPayPlan"].ToString());
					}
				}
				princ=PIn.Decimal(rawPayPlan.Rows[i]["principal_"].ToString());
				princDue=PIn.Decimal(rawPayPlan.Rows[i]["principalDue_"].ToString());
				interestDue=PIn.Decimal(rawPayPlan.Rows[i]["interestDue_"].ToString());
				accumDue=princDue+interestDue;
				princPaid=paid-interestDue;
				totCost=princ+PIn.Decimal(rawPayPlan.Rows[i]["interest_"].ToString());
				due=accumDue-paid;
				balance=princ-princPaid;
				//then fill the row----------------------------------------------------------------------
				row=table.NewRow();
				row["accumDue"]=accumDue.ToString("n");
				row["balance"]=balance.ToString("n");
				dateT=PIn.DateT(rawPayPlan.Rows[i]["PayPlanDate"].ToString());
				row["DateTime"]=dateT;
				row["date"]=dateT.ToShortDateString();
				row["dateTimeSort"]=PIn.DateT(rawPayPlan.Rows[i]["SecDateTEntry"].ToString());//SecDateTEntry will be used for sorting if RandomKeys is enabled
				if(rawPayPlan.Rows[i]["PlanNum"].ToString()=="0") {
					row["due"]=due.ToString("n");
				}
				else {
					row["due"]=0.ToString("n");
				}
				row["guarantor"]=fam.GetNameInFamLF(guarNumCur);
				row["Guarantor"]=guarNumCur;
				row["InstallmentPlanNum"]="0";
				row["IsClosed"]=rawPayPlan.Rows[i]["IsClosed"].ToString();
				row["paid"]=paid.ToString("n");
				row["patient"]=GetPatName(patNumCur,fam,true);
				row["PatNum"]=patNumCur;
				row["PayPlanNum"]=PIn.Long(rawPayPlan.Rows[i]["PayPlanNum"].ToString());
				row["PlanCategory"]=rawPayPlan.Rows[i]["PlanCategory"].ToString();
				row["principal"]=princ.ToString("n");
				row["princPaid"]=Math.Max(0,princPaid).ToString("n");
				row["totalCost"]=totCost.ToString("n");
				if(rawPayPlan.Rows[i]["IsDynamic"].ToString()=="1") {
					row["type"]="DPP";
				}
				else if(rawPayPlan.Rows[i]["PlanNum"].ToString()=="0"){
					row["type"]="PP";
				}
				else{
					row["type"]="Ins";
				}
				rows.Add(row);
			}
			//Installment plans-------------------------------------------------------------------------
			for(int i=0;i<rawInstall.Rows.Count;i++){
				long guarNumCur=PIn.Long(rawInstall.Rows[i]["PatNum"].ToString());//Pat for installment plan will always be guarantor
				if(guarNumCur!=pat.PatNum && isSingle) {
					continue;//skip installment plans that are not associated to this patient
				}
				row=table.NewRow();
				row["accumDue"]="";
				row["balance"]="";
				dateT=PIn.DateT(rawInstall.Rows[i]["DateAgreement"].ToString());
				row["DateTime"]=dateT;
				row["date"]=dateT.ToShortDateString();
				row["dateTimeSort"]=dateT;//no DateTStamp columns on the installmentplan table, use DateAgreement I guess?
				row["due"]=PIn.Decimal(rawInstall.Rows[i]["MonthlyPayment"].ToString()).ToString("f");
				row["guarantor"]="";
				row["InstallmentPlanNum"]=PIn.Long(rawInstall.Rows[i]["InstallmentPlanNum"].ToString());
				row["IsClosed"]="0"; //installment plans are never closed.
				row["paid"]="";
				long patNumCur=PIn.Long(rawInstall.Rows[i]["PatNum"].ToString());
				row["patient"]=GetPatName(patNumCur,fam,true);
				row["PatNum"]=patNumCur;
				row["PayPlanNum"]=0;
				row["principal"]="";
				row["princPaid"]="";
				row["totalCost"]="";
				row["type"]="IP";
				rows.Add(row);
			}
			for(int i=0;i<rows.Count;i++) {
				table.Rows.Add(rows[i]);
			}
			return table;
		}

		///<summary>Gets payment plans for the family.  RawPay will include any paysplits for anyone in the family plus splits for payment plans being 
		///paid by someone outside the family.  fromDate and toDate are only used if isForStatement.  From date lets us restrict how many
		///amortization items to show.  toDate is typically 10 days in the future.  This method cannot be called by the Middle Tier due to its use of an
		///out parameter.  LimitedStatements will return an empty DataTable.</summary>
		private static DataTable GetPayPlansForStatement(DataTable rawPayPlan,DataTable rawPay,DateTime fromDate,DateTime toDate,bool singlePatient,
			DataTable rawClaimPay,Family fam,Patient pat,out decimal payPlanDue,StmtType statementType,List<long> listPayPlanNumsExclude,
			bool isForDynamic=false,decimal totalPlannedFee=0)
		{
			//No need to check RemotingRole; no call to db.
			//We may need to add installment plans to this grid some day.  No time right now.
			string tableName="payplan";
			string descriptionPayPlanType="Payment Plan.";
			string descriptionPrincipal="Original Loan Principal:";
			string descriptionInterest="Total Interest:";
			string descriptionPlannedPrincipal="";
			if(isForDynamic) {
				tableName="dynamicPayPlan";
				descriptionPayPlanType="Dynamic Payment Plan.";
				descriptionPrincipal="Total Estimated Principal:";
				if(!decimal.Equals(totalPlannedFee,decimal.Zero)) { 
					descriptionPlannedPrincipal="Treatment Planned Remaining:";
				}
				descriptionInterest="Total Estimated Interest:";
			}
			DataTable table=new DataTable(tableName);
			DataRow row;
			SetTableColumns(table);//this will allow it to later be fully integrated into a single grid.
			payPlanDue=0;
			if(statementType==StmtType.LimitedStatement) {//don't include payment plans on LimitedStatements
				return table;
			}
			List<DataRow> rows=new List<DataRow>();
			decimal princ;
			decimal interest;
			decimal bal;
			DataTable rawAmort;
			long payPlanNum;
			for(int i=0;i<rawPayPlan.Rows.Count;i++){//loop through the payment plans (usually zero or one)
				if(!isForDynamic && PIn.Bool(rawPayPlan.Rows[i]["IsDynamic"].ToString())) {
					continue;
				}
				if(isForDynamic && !PIn.Bool(rawPayPlan.Rows[i]["IsDynamic"].ToString())) {
					continue;
				}
				payPlanNum=PIn.Long(rawPayPlan.Rows[i]["PayPlanNum"].ToString());
				if(listPayPlanNumsExclude!=null && ListTools.In(payPlanNum,listPayPlanNumsExclude)) {
					continue;//likely this payment plan has already shown up on another super family member
				}
				princ=PIn.Decimal(rawPayPlan.Rows[i]["principal_"].ToString());
				interest=PIn.Decimal(rawPayPlan.Rows[i]["interest_"].ToString());
				bal=princ+interest;
				for(int p=0;p<rawPay.Rows.Count;p++){
					if(rawPay.Rows[p]["PayPlanNum"].ToString()==rawPayPlan.Rows[i]["PayPlanNum"].ToString()){
						bal-=PIn.Decimal(rawPay.Rows[p]["splitAmt_"].ToString());
					}
				}
				for(int c=0;c<rawClaimPay.Rows.Count;c++) {
					if(rawClaimPay.Rows[c]["PayPlanNum"].ToString()==rawPayPlan.Rows[i]["PayPlanNum"].ToString()) {
						bal-=PIn.Decimal(rawClaimPay.Rows[c]["InsPayAmtPayPlan"].ToString());
					}
				}       
				//If on version 1, don't show closed plans with nothing due. If on version 2, don't show closed payplans at all.
				if(rawPayPlan.Rows[i]["IsClosed"].ToString()=="1" && (PrefC.GetInt(PrefName.PayPlansVersion) == 2 || bal == 0)) {
					continue;
				}
				//summary row----------------------------------------------------------------------
				row=table.NewRow();
				row["AdjNum"]=0;
				row["balance"]="";
				row["balanceDouble"]=0;
				row["chargesDouble"]=0;
				row["charges"]="";
				row["ClaimNum"]=0;
				row["ClaimPaymentNum"]="0";
				row["colorText"]=Color.Black.ToArgb().ToString();
				row["creditsDouble"]=0;
				row["credits"]="";
				row["DateTime"]=DateTime.MinValue;
				row["date"]="";
				row["dateTimeSort"]=PIn.DateT(rawPayPlan.Rows[i]["SecDateTEntry"].ToString());//SecDateTEntry will be used for sorting if RandomKeys is enabled
				string description=Lans.g("AccountModule",descriptionPayPlanType)+"\r\n"
					+Lans.g("AccountModule",descriptionPrincipal)+" "+princ.ToString("c")+"\r\n";
				if(interest!=0) {
					description+=Lans.g("AccountModule",descriptionInterest)+" "+interest.ToString("c")+"\r\n";
				}
				if(isForDynamic && !decimal.Equals(totalPlannedFee,decimal.Zero)) {
					description+=Lans.g("AccountModule",descriptionPlannedPrincipal)+" "+totalPlannedFee.ToString("c")+"\r\n";
				}
				description+=Lans.g("AccountModule","Amount Remaining:")+" "+bal.ToString("c");			
				row["description"]=description;
				if(rawPayPlan.Rows[i]["PlanNum"].ToString()!="0"){
					//Although if they are properly deleting insurance pp charges, no such pp's will be here anyway.
					continue;//don't show insurance payment plans on statements.
				}
				//so all payment plans will have a patient.
				//If Single Patient is checked, then we only want to show payment plans where the selected patient is either the PayPlan's Patient or Guarantor.
				long patNumCur=PIn.Long(rawPayPlan.Rows[i]["PatNum"].ToString());
				long guarNumCur=PIn.Long(rawPayPlan.Rows[i]["Guarantor"].ToString());
				if(singlePatient && guarNumCur!=pat.PatNum && patNumCur!=pat.PatNum) {
					continue;
				}
				string descriptionPatient="\r\nPatient: "+fam.GetNameInFamLF(patNumCur);
				//Include guarantor information if they are outside of the current family.
				if(!fam.IsInFamily(guarNumCur)) {
					descriptionPatient+="\r\nGuarantor: "+Patients.GetLim(guarNumCur).GetNameLF();
				}
				row["description"]+=descriptionPatient;
				row["patient"]="";
				row["PatNum"]=patNumCur;
				row["PayNum"]=0;
				row["PayPlanNum"]=payPlanNum;
				row["PayPlanChargeNum"]="0";
				row["ProcCode"]="";
				row["ProcNum"]="0";
				row["procsOnObj"]="";
				row["adjustsOnObj"]="";
				row["prov"]="";
				row["signed"]="";
				row["StatementNum"]=0;
				row["tth"]="";
				rows.Add(row);
				//detail rows-------------------------------------------------------------------------------
				rawAmort=GetPayPlanAmortTable(payPlanNum);
				//remove future entries, going backwards
				for(int d=rawAmort.Rows.Count-1;d>=0;d--) {
					if((DateTime)rawAmort.Rows[d]["DateTime"]>toDate.AddDays(PrefC.GetLong(PrefName.PayPlansBillInAdvanceDays))) {
						rawAmort.Rows.RemoveAt(d);
					}
				}
				//grab the payPlanDue amount from the last row
				if(rawAmort.Rows.Count>0) {
					payPlanDue+=(decimal)rawAmort.Rows[rawAmort.Rows.Count-1]["balanceDouble"];
				}
				//remove old entries, going backwards
				for(int d=rawAmort.Rows.Count-1;d>=0;d--){
					if((DateTime)rawAmort.Rows[d]["DateTime"]<fromDate){
						rawAmort.Rows.RemoveAt(d);
					}
				}
				for(int d=0;d<rawAmort.Rows.Count;d++){
					row=table.NewRow();
					row["AdjNum"]=0;
					row["balance"]=rawAmort.Rows[d]["balance"];
					row["balanceDouble"]=rawAmort.Rows[d]["balanceDouble"];
					row["chargesDouble"]=rawAmort.Rows[d]["chargesDouble"];
					row["charges"]=rawAmort.Rows[d]["charges"];
					row["ClaimNum"]=0;
					row["ClaimPaymentNum"]="0";
					row["colorText"]=Color.Black.ToArgb().ToString();
					row["creditsDouble"]=rawAmort.Rows[d]["creditsDouble"];
					row["credits"]=rawAmort.Rows[d]["credits"];
					row["DateTime"]=rawAmort.Rows[d]["DateTime"];
					row["date"]=rawAmort.Rows[d]["date"];
					row["dateTimeSort"]=rawAmort.Rows[d]["dateTimeSort"];
					row["description"]=rawAmort.Rows[d]["description"];
					row["patient"]=rawAmort.Rows[d]["patient"];
					row["PatNum"]=rawAmort.Rows[d]["PatNum"];
					row["PayNum"]=rawAmort.Rows[d]["PayNum"];
					row["PayPlanNum"]=payPlanNum;
					row["PayPlanChargeNum"]=rawAmort.Rows[d]["PayPlanChargeNum"];
					row["ProcCode"]="";
					row["ProcNum"]="0";
					row["procsOnObj"]="";
					row["adjustsOnObj"]="";
					row["prov"]=rawAmort.Rows[d]["prov"];
					row["signed"]="";
					row["StatementNum"]=0;
					row["tth"]="";
					rows.Add(row);
				}
			}
			for(int i=0;i<rows.Count;i++) {
				table.Rows.Add(rows[i]);
			}
			return table;
		}

		///<summary>All rows for the entire family are getting passed in here.  (Except Invoices)  The rows have already been sorted.  Balances have not been computed, and we will do that here, separately for each patient (except invoices).</summary>
		private static DataTable GetPatientTable(Family fam,List<DataRow> rows,bool isInvoice,StmtType statementType){
			//No need to check RemotingRole; no call to db.
			//Create a helper dictionary in order to save on trying to find all corresponding rows for a specific patient when we loop through the family.
			Dictionary<long,List<DataRow>> dictPatientRows=rows.GroupBy(x => PIn.Long(x["PatNum"].ToString()))
				.ToDictionary(x => x.Key,x => x.ToList());
			DataTable table=new DataTable("patient");
			DataRow row;
			table.Columns.Add("balance");
			table.Columns.Add("balanceDouble",typeof(decimal));
			table.Columns.Add("name");
			table.Columns.Add("PatNum");
			List<DataRow> rowspat=new List<DataRow>();
			decimal bal;
			decimal balfam=0;
			for(int p=0;p<fam.ListPats.Length;p++){
				row=table.NewRow();
				bal=0;
				List<DataRow> listPatientRows;
				if(dictPatientRows.TryGetValue(fam.ListPats[p].PatNum,out listPatientRows)) {
					foreach(DataRow rowPatient in listPatientRows) {
						bal+=(decimal)rowPatient["chargesDouble"];
						bal-=(decimal)rowPatient["creditsDouble"];
					}
				}
				balfam+=bal;
				row["balanceDouble"]=bal;
				row["balance"]=bal.ToString("f");
				row["name"]=fam.ListPats[p].GetNameLF();
				row["PatNum"]=fam.ListPats[p].PatNum.ToString();
				rowspat.Add(row);
				if(isInvoice || statementType==StmtType.LimitedStatement) {
					//we don't have all the rows, so we don't want to try to compute balance
				}
				else {
					if((double)bal!=fam.ListPats[p].EstBalance) {
						Patient patnew=fam.ListPats[p].Copy();
						patnew.EstBalance=(double)bal;
						Patients.Update(patnew,fam.ListPats[p]);
					}
				}
			}
			//Row for entire family
			row=table.NewRow();
			row["balanceDouble"]=balfam;
			row["balance"]=balfam.ToString("f");
			row["name"]=Lans.g("AccountModule","Entire Family");
			row["PatNum"]=fam.ListPats[0].PatNum.ToString();
			rowspat.Add(row);
			for(int i=0;i<rowspat.Count;i++) {
				table.Rows.Add(rowspat[i]);
			}
			return table;
		}

		///<summary>Future appointments.</summary>
		private static DataTable GetApptTable(Family fam,bool singlePatient,long patNum) {
			//No need to check RemotingRole; private static.
			DataConnection dcon=new DataConnection();
			DataTable table=new DataTable("appts");
			DataRow row;
			table.Columns.Add("descript");
			table.Columns.Add("PatNum");
			List<DataRow> rows=new List<DataRow>();
			string command="SELECT AptDateTime,PatNum,ProcDescript "
				+"FROM appointment "
				+"WHERE AptDateTime > "+POut.DateT(DateTime.Now)+" "//Today.AddDays(1) midnight tonight
				+"AND AptStatus !="+POut.Long((int)ApptStatus.Broken)+" "
				+"AND AptStatus !="+POut.Long((int)ApptStatus.PtNote)+" "
				+"AND AptStatus !="+POut.Long((int)ApptStatus.PtNoteCompleted)+" "
				+"AND AptStatus !="+POut.Long((int)ApptStatus.UnschedList)+" "
				+"AND (";
			if(singlePatient){
				command+="PatNum ="+POut.Long(patNum);
			}
			else{
				for(int i=0;i<fam.ListPats.Length;i++){
					if(i!=0){
						command+="OR ";
					}
					command+="PatNum ="+POut.Long(fam.ListPats[i].PatNum)+" ";
				}
			}
			command+=") ORDER BY PatNum,AptDateTime";
			DataTable raw=dcon.GetTable(command);
			DateTime dateT;
			long patNumm;
			for(int i=0;i<raw.Rows.Count;i++){
				row=table.NewRow();
				patNumm=PIn.Long(raw.Rows[i]["PatNum"].ToString());
				dateT=PIn.DateT(raw.Rows[i]["AptDateTime"].ToString());
				row["descript"]=fam.GetNameInFamFL(patNumm)+":  "
					+dateT.ToString("dddd")+",  "
					+dateT.ToShortDateString()
					+",  "+dateT.ToShortTimeString()+",  "+raw.Rows[i]["ProcDescript"].ToString();
				row["PatNum"]=patNumm.ToString();
				rows.Add(row);
			}
			for(int i=0;i<rows.Count;i++) {
				table.Rows.Add(rows[i]);
			}
			return table;
		}

		public static DataTable GetMisc(Family fam,long patNum,decimal patientPayPlanDue,decimal dynamicPayPlanDue,decimal balanceForward
			,StmtType statementType,DataSet ds)
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),fam,patNum,patientPayPlanDue,dynamicPayPlanDue,balanceForward,statementType,ds);
			}
			DataTable table=new DataTable("misc");
			DataRow row;
			table.Columns.Add("descript");
			table.Columns.Add("value");
			List<DataRow> rows=new List<DataRow>();
			//FamFinancial note--------------------
			string command = 
				"SELECT FamFinancial "
				+"FROM patientnote WHERE patnum ="+POut.Long(fam.ListPats[0].PatNum);
			DataTable raw=Db.GetTable(command);
			row=table.NewRow();
			row["descript"]="FamFinancial";
			row["value"]="";
			if(raw.Rows.Count==1){
				row["value"]=PIn.String(raw.Rows[0][0].ToString());
			}
			rows.Add(row);
			//payPlanDue---------------------------
			row=table.NewRow();
			row["descript"]="patientPayPlanDue";
			row["value"]=POut.Decimal(patientPayPlanDue);
			rows.Add(row);
			row=table.NewRow();
			row["descript"]="dynamicPayPlanDue";
			row["value"]=POut.Decimal(dynamicPayPlanDue);
			rows.Add(row);
			row=table.NewRow();
			row["descript"]="payPlanDue";
			row["value"]=POut.Decimal(dynamicPayPlanDue+patientPayPlanDue);
			rows.Add(row);
			//balanceForward-----------------------
			row=table.NewRow();
			row["descript"]="balanceForward";
			row["value"]=POut.Decimal(balanceForward);
			rows.Add(row);
			//patInsEst----------------------------
			string procNumsForInsEst="";
			command="SELECT COALESCE(SUM(inspayest+writeoff),0) FROM claimproc "
				+"WHERE status = 0 ";//not received
			if(statementType!=StmtType.LimitedStatement) {
				command+="AND PatNum="+POut.Long(patNum);
			}
			else {
				procNumsForInsEst=string.Join(",",ds.Tables.OfType<DataTable>()//only reference to ds, should never be null if it's a LimitedStatement
					.Where(x => x.TableName.StartsWith("account"))
					.SelectMany(x => x.Rows.OfType<DataRow>()
						.Select(y => POut.String(y["ProcNum"].ToString()))
						.Where(y => y!="0")));
				command+="AND ProcNum IN ("+procNumsForInsEst+")";
			}
			row=table.NewRow();
			if(statementType!=StmtType.LimitedStatement || procNumsForInsEst!="") {//don't run if LimitedStatement and no procs selected
				raw=Db.GetTable(command);
				row["descript"]="patInsEst";
				row["value"]=raw.Rows[0][0].ToString();
			}
			rows.Add(row);
			//Unearned income----------------------
			command="SELECT SUM(SplitAmt) FROM paysplit WHERE "
				+"UnearnedType>0 AND (";
			for(int i=0;i<fam.ListPats.Length;i++) {
				if(i>0) {
					command+=" OR ";
				}
				command+="PatNum= "+POut.Long(fam.ListPats[i].PatNum);
			}
			command+=")";
			//Unearned Amount from this datatable is deprecated.  Account module uses S-class methods to calculate it now.
			double unearnedAmt=PIn.Double(Db.GetScalar(command));
			row=table.NewRow();
			row["descript"]="unearnedIncome";
			row["value"]=unearnedAmt;
			rows.Add(row);
			//final prep:
			for(int i=0;i<rows.Count;i++) {
				table.Rows.Add(rows[i]);
			}
			return table;
		}

		///<summary>Used to resort data rows used for printing main account grid on statements.  The supplied DataRows must include the following columns:
		///ProcNum,AdjNum,PayNum,ClaimNum,ClaimPaymentNum,PayPlanNum,StatementNum,dateTimeSort,DateTime,(Priority not needed),ToothRange,ToothNum,ProcCode.
		///This sorts all objects in Account module based on their types, dates,toothrange, toothnum, and proccode (for procedures with columns present,
		///will also sort by priority and status).  Times are ignored for the DateTime column if present, but not for the dateTimeSort column.</summary>
		public static int SortRowsForStatmentPrinting(DataRow x,DataRow y) {
			if(x["PatNum"].ToString()!=y["PatNum"].ToString()
				&& x["PatNum"].ToString()!="0"
				&& y["PatNum"].ToString()!="0")
			{
				return ((long)x["PatNum"]).CompareTo((long)y["PatNum"]);
			}
			//if dates are different, then sort by date
			if(((DateTime)x["DateTime"]).Date!=((DateTime)y["DateTime"]).Date) {
				return (((DateTime)x["DateTime"]).Date).CompareTo(((DateTime)y["DateTime"]).Date);
			}
			#region Sort By Type
			#region Procedures
			if(x["ProcNum"].ToString()!="0" && y["ProcNum"].ToString()=="0") {
				return -1;
			}
			if(x["ProcNum"].ToString()=="0" && y["ProcNum"].ToString()!="0") {
				return 1;
			}
			if(x["ProcNum"].ToString()!="0" && y["ProcNum"].ToString()!="0") {//if both are procedures
				return ProcedureLogic.CompareProcedures(x,y);//Sort procedures by status, priority, tooth region/num, proc code
			}
			#endregion Procedures
			#region Adjustments
			if(x["AdjNum"].ToString()!="0" && y["AdjNum"].ToString()=="0") {
				return -1;
			}
			if(x["AdjNum"].ToString()=="0" && y["AdjNum"].ToString()!="0") {
				return 1;
			}
			if(!PrefC.RandomKeys && x["AdjNum"].ToString()!="0" && y["AdjNum"].ToString()!="0") {//if RandomKeys enabled, sorted by dateTimeSort below
				return ((long)x["AdjNum"]).CompareTo((long)y["AdjNum"]);//if both adjustments on same date, order by primary key
			}
			#endregion Adjustments
			#region Payments
			if(x["PayNum"].ToString()!="0" && y["PayNum"].ToString()=="0") {
				return -1;
			}
			if(x["PayNum"].ToString()=="0" && y["PayNum"].ToString()!="0") {
				return 1;
			}
			if(!PrefC.RandomKeys && x["PayNum"].ToString()!="0" && y["PayNum"].ToString()!="0") {//if RandomKeys enabled, sorted by dateTimeSort below
				return ((long)x["PayNum"]).CompareTo((long)y["PayNum"]);//if both payments on same date, order by primary key
			}
			#endregion Payments
			#region Claims
			if(x["ClaimNum"].ToString()!="0" && y["ClaimNum"].ToString()=="0") {
				return -1;
			}
			if(x["ClaimNum"].ToString()=="0" && y["ClaimNum"].ToString()!="0") {
				return 1;
			}
			if(x["ClaimNum"].ToString()!="0" && y["ClaimNum"].ToString()!="0") {
				if(x["ClaimPaymentNum"].ToString()!="0" && y["ClaimPaymentNum"].ToString()=="0") {//claims before claimpayments
					return 1;
				}
				if(x["ClaimPaymentNum"].ToString()=="0" && y["ClaimPaymentNum"].ToString()!="0") {//claims before claimpayments
					return -1;
				}
				if(!PrefC.RandomKeys && x["ClaimPaymentNum"].ToString()=="0" && y["ClaimPaymentNum"].ToString()=="0") {//if RandomKeys enabled, sorted by dateTimeSort below
					return ((long)x["ClaimNum"]).CompareTo((long)y["ClaimNum"]);//if both claims on same date, order by primary key
				}
				//if both ClaimPaymentNums!=0, then both ClaimPaymentNums=="1" so we have to rely on sorting by dateTimeSort below
			}
			#endregion Claims
			#region Payplans
			if(x["PayPlanNum"].ToString()!="0" && y["PayPlanNum"].ToString()=="0") {
				return -1;
			}
			if(x["PayPlanNum"].ToString()=="0" && y["PayPlanNum"].ToString()!="0") {
				return 1;
			}
			if(!PrefC.RandomKeys && x["PayPlanNum"].ToString()!="0" && y["PayPlanNum"].ToString()!="0") {//if RandomKeys enabled, sorted by dateTimeSort below
				return ((long)x["PayPlanNum"]).CompareTo((long)y["PayPlanNum"]);//if both payplans (or payplancharges) on same date, order by primary key
			}
			#endregion Payplans
			#region Statements
			if(x["StatementNum"].ToString()!="0" && y["StatementNum"].ToString()=="0") {
				return -1;
			}
			if(x["StatementNum"].ToString()=="0" && y["StatementNum"].ToString()!="0") {
				return 1;
			}
			if(!PrefC.RandomKeys && x["StatementNum"].ToString()!="0" && y["StatementNum"].ToString()!="0") {//if RandomKeys enabled, sorted by dateTimeSort below
				return ((long)x["StatementNum"]).CompareTo((long)y["StatementNum"]);//if both statments on same date, order by primary key
			}
			#endregion Statements
			#endregion Sort By Type
			if(PrefC.RandomKeys) {//if RandomKeys enabled, sort by dateTimeSort (SecDateTEntry if exists, otherwise DateTStamp or SecDateTEdit if available)
				return ((DateTime)x["dateTimeSort"]).CompareTo((DateTime)y["dateTimeSort"]);
			}
			return 0;
		}


		///<summary>Returns all the necessary patient payment and insurance payment information for the payments grid in invoices.
		///Also updates the returned payments and claimprocs' StatementNums, unless 0 is passed in for stmtNum.</summary>
		public static DataTable GetPaymentsForInvoice(List<long> listPatNums,List<long> listProcNums,DateTime dateTimeInvoice) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),listPatNums,listProcNums,dateTimeInvoice);
			}
			if(listPatNums==null || listPatNums.Count==0) {
				return new DataTable();
			}
			string command = "SELECT tran.PriKey, patient.LName, patient.FName, tran.Date, tran.Amt, COALESCE(provider.Abbr,'') Provider, "
				+"procedurecode.ProcCode, "+DbHelper.Concat("definition.ItemName","': '","tran.CheckNum","'   '","tran.Note")+" Descript, tran.TranType "
				+ "FROM ("
					+ "SELECT paysplit.SplitNum PriKey, paysplit.PatNum, paysplit.DatePay Date, -paysplit.SplitAmt Amt, "
					+ "paysplit.ProvNum, paysplit.ProcNum, payment.PayType, payment.PayNote Note, payment.CheckNum, 'PAT' TranType "
					+ "FROM paysplit "
					+ "INNER JOIN payment ON payment.PayNum = paysplit.PayNum "
					+ "WHERE paysplit.PatNum IN ("+POut.String(string.Join(",",listPatNums))+") "
					+ "UNION ALL "
					+ "SELECT claimproc.ClaimProcNum PriKey, claimproc.PatNum, claimproc.DateCP Date, -claimproc.InsPayAmt Amt, "
					+ "claimproc.ProvNum, claimproc.ProcNum, claimpayment.PayType, claimpayment.Note, claimpayment.CheckNum, 'INS' TranType "
					+ "FROM claimproc "
					+ "INNER JOIN claimpayment ON claimpayment.ClaimPaymentNum = claimproc.ClaimPaymentNum "
					+ "WHERE claimproc.PatNum IN ("+POut.String(string.Join(",",listPatNums))+") ";
			if(PrefC.GetBool(PrefName.InvoicePaymentsGridShowNetProd)) {
				command+= ""
					//adjustments can already be manually selected to be included in invoices.
					//+ "UNION ALL "
					//+ "SELECT adjustment.AdjNum, adjustment.PatNum, adjustment.AdjDate, adjustment.AdjAmt, adjustment.ProvNum, adjustment.ProcNum, "
					//+ "adjustment.AdjType, adjustment.AdjNote, '', 'ADJ' TranType "
					//+ "FROM adjustment "
					//+ "WHERE PatNum IN ("+POut.String(string.Join(",",listPatNums))+") "
					+ "UNION ALL "
					+ "SELECT claimproc.ClaimProcNum PriKey, claimproc.PatNum, claimproc.DateCP Date, -claimproc.WriteOff Amt, "
					+ "claimproc.ProvNum, claimproc.ProcNum, '', claimproc.Remarks, '', 'WO' TranType "
					+ "FROM claimproc "
					+ "WHERE claimproc.Status IN("+POut.Int((int)ClaimProcStatus.NotReceived)+","+POut.Int((int)ClaimProcStatus.Received)+","
						+POut.Int((int)ClaimProcStatus.Supplemental)+","+POut.Int((int)ClaimProcStatus.CapComplete)+") "
					+ "AND PatNum IN ("+POut.String(string.Join(",",listPatNums))+") ";
			}
			command+=") tran "
				+ "LEFT JOIN definition ON definition.DefNum = tran.PayType "
				+ "LEFT JOIN provider ON provider.ProvNum = tran.ProvNum "
				+ "INNER JOIN patient ON patient.PatNum = tran.PatNum "
				+ "LEFT JOIN procedurelog ON procedurelog.ProcNum = tran.ProcNum "
				+ "LEFT JOIN procedurecode ON procedurecode.CodeNum = procedurelog.CodeNum "
				+ "WHERE ((tran.Date = "+POut.Date(dateTimeInvoice)+" AND tran.ProcNum = 0) ";
			if(listProcNums!=null && listProcNums.Count > 0) {
				command+= "OR tran.ProcNum IN ("+POut.String(string.Join(",",listProcNums))+") ";
			}
			command+=")";
			//logic to get all paysplits/payment info for payments made today or on procs on the invoice.
			return Db.GetTable(command); ;
		}

		///<summary>Returns a list of AccountEntry objects that represents all account transactions for the entities passed in.
		///1. Get a list of procedures and what's owed on them. (if displaying CreditCalcType.FIFO, this is just their procFee).
		///2. Get a list of unattached charges (payplan charge debits, unallocated positive adjustments) or don't (if not using FIFO logic) and
		///add both into AccountEntry list sorted by date.
		///3. Take all unattached credits (unallocated paysplits, inspays by total, unallocated negative adjustments, unallocated payplan credits) and apply to the list, item by item.
		///generate list of procs with explicit payments split to them.
		///Sets the Tag object of each AccountEntry to its corresponding procedure, payplancharge, or adjustment.  Type check accordingly.</summary>
		public static List<AccountEntry> GetListUnpaidAccountCharges(List<Procedure> listProcs,List<Adjustment> listAdjs,List<PaySplit> listPaySplits,
			List<ClaimProc> listClaimProcs,List<PayPlanCharge> listPayPlanCharges,List<ClaimProc> listInsPayTot,CreditCalcType calcType,
			List<PaySplit> listSplitsCur=null) 
		{
			//No need to check RemotingRole; no call to db.
			//Prefer the SplitAmt for splits in memory instead of the SplitAmt found within the database.
			//This can cause "incorrect" calculations to show to the user if they are in the middle of editing a payment and its splits.
			if(!listSplitsCur.IsNullOrEmpty()) {
				//Change the SplitAmt for any splits that are found in both lists, one from the database and one from memory.
				foreach(PaySplit paySplit in listPaySplits.FindAll(x => listSplitsCur.Any(y => x.IsSame(y)))) {
					paySplit.SplitAmt=listSplitsCur.First(x => x.IsSame(paySplit)).SplitAmt;
				}
			}
			List<AccountEntry> listAccountEntries=Procedures.GetProcExtendedEntriesFromProcedures(listProcs,listAdjs,listPaySplits,listClaimProcs,
					listPayPlanCharges,listSplitsCur)
				.Select(x => new AccountEntry(x))
				.ToList();
			//Include positive adjustment
			List<AccountEntry> listAdjEntries=listAdjs.Where(x => x.ProcNum == 0 && x.AdjAmt > 0).Select(x => new AccountEntry(x)).ToList();
			foreach(AccountEntry adjEntry in listAdjEntries) {
				List<PaySplit> listAdjSplits=listPaySplits.FindAll(x => x.AdjNum==adjEntry.AdjNum);
				if(listAdjSplits.IsNullOrEmpty()) {
					continue;
				}
				adjEntry.AmountEnd=adjEntry.AmountEnd-(decimal)listAdjSplits.Sum(x => x.SplitAmt);
			}
			listAccountEntries.AddRange(listAdjEntries);
			//get a list of unattached charges and add into account entry
			if(calcType == CreditCalcType.IncludeAll) {
				listAccountEntries.AddRange(listPayPlanCharges.Where(x => x.ChargeType == PayPlanChargeType.Debit).Select(x => new AccountEntry(x)));
				listAccountEntries=listAccountEntries.OrderBy(x => x.Date).ToList(); //order by date so credits get applied to oldest first.
				//get all unattached credits
				decimal creditTotal=0;
				creditTotal-=listAdjs.Where(x => x.ProcNum == 0 && x.AdjAmt <0).Sum(x => (decimal)x.AdjAmt); //negative adjustments are credits
				creditTotal+=listPaySplits.Where(x => x.ProcNum == 0).Sum(x => (decimal)x.SplitAmt);
				creditTotal+=listInsPayTot.Where(x => x.ProcNum == 0).Sum(x => (decimal)x.InsPayAmt);
				creditTotal+=listPayPlanCharges.Where(x => x.ProcNum == 0 && x.ChargeType == PayPlanChargeType.Credit).Sum(x => (decimal)x.Principal);
				//apply unattached credits to account entries
				foreach(AccountEntry entryCur in listAccountEntries) {
					if(CompareDecimal.IsLessThanOrEqualToZero(creditTotal)) {
						break;
					}
					if(CompareDecimal.IsLessThanOrEqualToZero(entryCur.AmountEnd)) {
						continue;
					}
					decimal amtToRemove=Math.Min(entryCur.AmountEnd,creditTotal);
					entryCur.AmountAvailable-=amtToRemove;
					entryCur.AmountEnd-=amtToRemove;
					creditTotal-=amtToRemove;
				}
			}
			listAccountEntries=listAccountEntries.OrderBy(x => x.Date).ToList();
			return listAccountEntries;
		}

		///<summary>Gets the data needed to create a claim.</summary>
		public static CreateClaimData GetCreateClaimData(Patient pat,Family fam) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<CreateClaimData>(MethodBase.GetCurrentMethod(),pat,fam);
			}
			CreateClaimData data=new CreateClaimData();
			data.ListPatPlans=PatPlans.Refresh(pat.PatNum);
			data.ListInsSubs=InsSubs.RefreshForFam(fam);
			data.ListInsPlans=InsPlans.RefreshForSubList(data.ListInsSubs);
			data.ListClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			data.ListProcs=Procedures.Refresh(pat.PatNum);
			data.PatNote=PatientNotes.Refresh(pat.PatNum,pat.Guarantor);
			return data;
		}


		///<summary>Attempts to create and insert a new claim.
		///If any claimprocs in ClaimProcList are modified, then will be updated to db.  Refresh claimprocs after calling if needed.</summary>
		///<returns>bool: True if the claim was created successfully.
		///					Claim: The claim that was created. Will be a new claim if success was not achieved.
		///					string: Any errors from attempting to create the claim.</returns>
		public static ODTuple<bool,Claim,string> CreateClaim(Claim ClaimCur,string claimType,List<PatPlan> PatPlanList,
			List<InsPlan> planList,List<ClaimProc> ClaimProcList,List<Procedure> procsForPat,List<InsSub> subList,
			Patient pat,PatientNote patNote,List<Procedure> listSelectedProcs,string claimError,InsPlan PlanCur,
			InsSub SubCur,Relat relatOther,List<Fee> listFees=null) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<ODTuple<bool,Claim,string>>(MethodBase.GetCurrentMethod(),ClaimCur,claimType,PatPlanList,planList,ClaimProcList,
					procsForPat,subList,pat,patNote,listSelectedProcs,claimError,PlanCur,SubCur,relatOther,listFees);
			}
			Procedure proc;
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				listSelectedProcs.RemoveAll(x => x.ProcNumLab!=0);//Canadian lab procs should not be considered "selected".
			}
			//If the sorting logic changes here, then also update the sorting logic in ContrTreat.FillMainDisplay() to match.
			List<Procedure> listProcs=Procedures.SortListByTreatPlanPriority(listSelectedProcs).ToList();
			if (listProcs.Count==0) {
				claimError=StringTools.AppendLine(claimError,Lans.g("ContrAccount","Not allowed to create a claim with no procedures."));
				return new ODTuple<bool,Claim,string>(false,new Claim(),claimError);
			}
			if((claimType=="P" || claimType=="S") && Procedures.GetUniqueDiagnosticCodes(listProcs,false).Count>4) {
				claimError=StringTools.AppendLine(claimError,Lans.g("ContrAccount","Claim has more than 4 unique diagnosis codes.  Create multiple claims instead."));
				return new ODTuple<bool,Claim,string>(false,new Claim(),claimError);
			}
			if(Procedures.GetUniqueDiagnosticCodes(listProcs,true).Count>12) {
				claimError=StringTools.AppendLine(claimError,Lans.g("ContrAccount","Claim has more than 12 unique diagnosis codes.  Create multiple claims instead."));
				return new ODTuple<bool,Claim,string>(false,new Claim(),claimError);
			}
			for(int i=0;i<listProcs.Count;i++){
				proc=listProcs[i];
				if(Procedures.NoBillIns(proc,ClaimProcList,PlanCur.PlanNum)){
					claimError=StringTools.AppendLine(claimError,Lans.g("ContrAccount","Not allowed to send procedures to insurance that are marked 'Do not bill to ins'."));
					return new ODTuple<bool,Claim,string>(false,new Claim(),claimError);
				}
			}
			for(int i=0;i<listProcs.Count;i++){
				proc=listProcs[i];
				if(Procedures.IsAlreadyAttachedToClaim(proc,ClaimProcList,SubCur.InsSubNum)){
					claimError=StringTools.AppendLine(claimError,Lans.g("ContrAccount","Not allowed to send a procedure to the same insurance company twice."));
					return new ODTuple<bool,Claim,string>(false,new Claim(),claimError);
				}
			}
			proc=listProcs[0];
			long clinicNum=proc.ClinicNum;
			PlaceOfService placeService=proc.PlaceService;
			for(int i=1;i<listProcs.Count;i++){//skips 0
				proc=listProcs[i];
				if(PrefC.HasClinicsEnabled && clinicNum!=proc.ClinicNum){
					claimError=StringTools.AppendLine(claimError,Lans.g("ContrAccount","All procedures do not have the same clinic."));
					return new ODTuple<bool,Claim,string>(false,new Claim(),claimError);
				}
				if(!PrefC.GetBool(PrefName.EasyHidePublicHealth) && proc.PlaceService!=placeService) {
					claimError=StringTools.AppendLine(claimError,Lans.g("ContrAccount","All procedures do not have the same place of service."));
					return new ODTuple<bool,Claim,string>(false,new Claim(),claimError);
				}
			}
			List<Procedure> listInProcessProcs=new List<Procedure>();
			for(int i=0;i<listProcs.Count;i++) {
				proc=listProcs[i];
				if(ProcMultiVisits.IsProcInProcess(proc.ProcNum)) {
					listInProcessProcs.Add(proc);
				}
			}
			if(listInProcessProcs.Count > 0) {
				claimError=StringTools.AppendLine(claimError,Lans.g("ContrAccount","Not allowed to send procedures which are currently in process.\r\nProcs: ")
					+String.Join(",",ProcedureCodes.GetCodesForCodeNums(listInProcessProcs.Select(x => x.CodeNum).ToList()).Select(x => x.ProcCode)));
				return new ODTuple<bool,Claim,string>(false,new Claim(),claimError);
			}
			ClaimProc[] claimProcs=new ClaimProc[listProcs.Count];//1:1 with listProcs
			for(int i=0;i<listProcs.Count;i++){//loop through selected procs
				//and try to find an estimate that can be used
				claimProcs[i]=Procedures.GetClaimProcEstimate(listProcs[i].ProcNum,ClaimProcList,PlanCur,SubCur.InsSubNum);
			}
			for(int i=0;i<claimProcs.Length;i++){//loop through each claimProc
				//and create any missing estimates. This handles claims to 3rd and 4th ins co's.
				if(claimProcs[i]==null){
					claimProcs[i]=new ClaimProc();
					proc=listProcs[i];//1:1
					ClaimProcs.CreateEst(claimProcs[i],proc,PlanCur,SubCur);
					if(claimProcs[i].NoBillIns) {
						claimError=StringTools.AppendLine(claimError,Lans.g("ContrAccount","Not allowed to send procedures to insurance that are marked 'Do not bill to ins'."));
						return new ODTuple<bool,Claim,string>(false,new Claim(),claimError);
					}
				}
			}
			ClaimCur=Claims.GetClaim(Claims.Insert(ClaimCur));//Insert to retreive Claim.ClaimNum and GetClaim to retreive SecDateEntry for permission check
			//now, all claimProcs have a valid value
			//for any CapComplete, need to make a copy so that original doesn't get attached.
			for(int i=0;i<claimProcs.Length;i++){
				if(claimProcs[i].Status==ClaimProcStatus.CapComplete){
					claimProcs[i].ClaimNum=ClaimCur.ClaimNum;
					claimProcs[i]=claimProcs[i].Copy();
					claimProcs[i].WriteOff=0;
					claimProcs[i].CopayAmt=-1;
					claimProcs[i].CopayOverride=-1;
					//status will get changed down below
					ClaimProcs.Insert(claimProcs[i]);//this makes a duplicate in db with different claimProcNum
				}
			}
			ClaimCur.PatNum=pat.PatNum;
			//All procedures in each multi visit group attached to this claim are set complete.
			//Therefore, the procedures are no longer in process and will not be in the cache.
			//We must get the group data from the database directly so we can calculate max group dates.
			List<ProcMultiVisit> listClaimPmvs=ProcMultiVisits.GetGroupsForProcsFromDb(claimProcs.Select(x => x.ProcNum).ToArray());
			List<Procedure> listPatProcs=Procedures.Refresh(pat.PatNum);//We need a fresh list of procs so we can get current ProcDates.			
			foreach(ClaimProc cp in claimProcs) {
				//It is common for only some of the procedures in a multi visit group to be attached to the claim while others are not.
				//For example, the Crown code with fee would be attached to the claim,
				//while the Seat code is not attached to the claim because its fee is usually $0, which the UI blocks from attaching to the claim.
				//We are required to use the greatest date for all procedures in the group when reporting the attached procedure dates on the claim.
				ProcMultiVisit pmvForProc=listClaimPmvs.FirstOrDefault(x => x.ProcNum==cp.ProcNum);
				if(pmvForProc==null) {//Procedure does not belong to a multi visit group.
					//A procedure which is not part of a multi visit group might be attached to the claim along with a group of procedures.
					//In this case, there will be no procvisitmulti row available.
					//This case might also happen if one user deletes a procedure in the group right when another user is sending the claim.
					Procedure procForCp=listPatProcs.FirstOrDefault(x => x.ProcNum==cp.ProcNum);
					if(procForCp!=null) {//procForCp can be null if cp is for a "By Total" payment (cp.ProcNum=0).
						//For procs which are not multi visit, ensure the claimproc.ProcDate matches the procedure.ProcDate, to maintain old behavior.
						cp.ProcDate=procForCp.ProcDate;
					}
				}
				else {//Procedure is part of a multi visit group.
					List<ProcMultiVisit> listGroupPmvs=listClaimPmvs.FindAll(x => x.GroupProcMultiVisitNum==pmvForProc.GroupProcMultiVisitNum);
					//Using the maximum group date on the claimproc.ProcDate will cause the insurance billable date to be the delivery date,
					//while the attached proc.ProcDate will remain unchanged and be the billing date in the account to preserve statement history.
					DateTime dateForGroup=DateTime.MinValue;//Max ProcDate for the group.
					foreach(ProcMultiVisit pmv in listGroupPmvs) {
						Procedure procCur=listPatProcs.FirstOrDefault(x => x.ProcNum==pmv.ProcNum);
						if(procCur!=null && procCur.ProcDate > dateForGroup) {//Can be null if the user deleted the proc after the multi visit group was created.
							dateForGroup=procCur.ProcDate;
						}
					}
					//If all procs were deleted from the multi visit group (possible?), then leave the cp.ProcDate alone because it is probably correct.
					if(dateForGroup > DateTime.MinValue) {
						cp.ProcDate=dateForGroup;
					}
				}
				//The cp changes will be updated to the database at the end of this function.
			}
			if(listClaimPmvs.Count > 0) {//A multi visit claim.
				//For most dental claims, the ProcDate will be the same for all procedures.
				//For multiple visit procedures attached to claims, we must send the greatest date of the procedures in the group.
				//(ex the delivery date of a crown)
				ClaimCur.DateService=claimProcs.Max(x => x.ProcDate);//Max date of attached procs, after considering group date maximums.
			}
			else {//Not a multi visit claim.
				ClaimCur.DateService=claimProcs[claimProcs.Length-1].ProcDate;//Last proc from TP sort above.  Not sure why we chose this date.  Old behavior.
			}
			ClaimCur.ClinicNum=clinicNum;
			ClaimCur.PlaceService=proc.PlaceService;
			ClaimCur.AttachedFlags="Mail";
			ClaimCur.ClaimIdentifier=string.IsNullOrWhiteSpace(ClaimCur.ClaimIdentifier) ? Claims.ConvertClaimId(ClaimCur,pat) : ClaimCur.ClaimIdentifier;
			//datesent
			//ClaimStatus does not change.  Set before calling this function.
			//datereceived
			InsSub sub;
			ClaimCur.PlanNum=PlanCur.PlanNum;
			ClaimCur.InsSubNum=SubCur.InsSubNum;
			switch(claimType){
				case "P":
					ClaimCur.PatRelat=PatPlans.GetRelat(PatPlanList,PatPlans.GetOrdinal(PriSecMed.Primary,PatPlanList,planList,subList));
					ClaimCur.ClaimType="P";
					ClaimCur.InsSubNum2=PatPlans.GetInsSubNum(PatPlanList,PatPlans.GetOrdinal(PriSecMed.Secondary,PatPlanList,planList,subList));
					sub=InsSubs.GetSub(ClaimCur.InsSubNum2,subList);
					if(sub.PlanNum>0 && InsPlans.RefreshOne(sub.PlanNum).IsMedical) {
						ClaimCur.PlanNum2=0;//no sec ins
						ClaimCur.PatRelat2=Relat.Self;
					}
					else {
						ClaimCur.PlanNum2=sub.PlanNum;//might be 0 if no sec ins
						ClaimCur.PatRelat2=PatPlans.GetRelat(PatPlanList,PatPlans.GetOrdinal(PriSecMed.Secondary,PatPlanList,planList,subList));
					}
					break;
				case "S":
					ClaimCur.PatRelat=PatPlans.GetRelat(PatPlanList,PatPlans.GetOrdinal(PriSecMed.Secondary,PatPlanList,planList,subList));
					ClaimCur.ClaimType="S";
					ClaimCur.InsSubNum2=PatPlans.GetInsSubNum(PatPlanList,PatPlans.GetOrdinal(PriSecMed.Primary,PatPlanList,planList,subList));
					sub=InsSubs.GetSub(ClaimCur.InsSubNum2,subList);
					ClaimCur.PlanNum2=sub.PlanNum;
					ClaimCur.PatRelat2=PatPlans.GetRelat(PatPlanList,PatPlans.GetOrdinal(PriSecMed.Primary,PatPlanList,planList,subList));
					break;
				case "Med":
					ClaimCur.PatRelat=PatPlans.GetFromList(PatPlanList,SubCur.InsSubNum).Relationship;
					ClaimCur.ClaimType="Other";
					if(PrefC.GetBool(PrefName.ClaimMedTypeIsInstWhenInsPlanIsMedical)){
						ClaimCur.MedType=EnumClaimMedType.Institutional;
					}
					else{
						ClaimCur.MedType=EnumClaimMedType.Medical;
					}
					break;
				case "Other":
					ClaimCur.PatRelat=relatOther;
					ClaimCur.ClaimType="Other";
					//plannum2 is not automatically filled in.
					ClaimCur.ClaimForm=0;
					if(PlanCur.IsMedical){
						if(PrefC.GetBool(PrefName.ClaimMedTypeIsInstWhenInsPlanIsMedical)){
							ClaimCur.MedType=EnumClaimMedType.Institutional;
						}
						else{
							ClaimCur.MedType=EnumClaimMedType.Medical;
						}
					}
					break;
			}
			if(PlanCur.PlanType=="c"){//if capitation
				ClaimCur.ClaimType="Cap";
			}
			ClaimCur.ProvTreat=listProcs[0].ProvNum;
			for(int i=0;i<listProcs.Count;i++){
				proc=listProcs[i];
				if(!Providers.GetIsSec(proc.ProvNum)){//if not a hygienist
					ClaimCur.ProvTreat=proc.ProvNum;
				}
			}
			if(Providers.GetIsSec(ClaimCur.ProvTreat)){
				ClaimCur.ProvTreat=pat.PriProv;
				//OK if 0, because auto select first in list when open claim
			}
			//claimfee calcs in ClaimEdit
			//inspayest ''
			//inspayamt
			//ClaimCur.DedApplied=0;//calcs in ClaimEdit.
			//preauthstring, etc, etc
			ClaimCur.IsProsthesis="N";
			//int clinicInsBillingProv=0;
			//bool useClinic=false;
			//if(ClaimCur.ClinicNum>0){
			//	useClinic=true;
			//	clinicInsBillingProv=Clinics.GetClinic(ClaimCur.ClinicNum).InsBillingProv;
			//}
			ClaimCur.ProvBill=Providers.GetBillingProvNum(ClaimCur.ProvTreat,ClaimCur.ClinicNum);//,useClinic,clinicInsBillingProv);//OK if zero, because it will get fixed in claim
			Provider prov=Providers.GetProv(ClaimCur.ProvTreat);
			if(prov.ProvNumBillingOverride!=0) {
				ClaimCur.ProvBill=prov.ProvNumBillingOverride;
			}
			ClaimCur.EmployRelated=YN.No;
			ClaimCur.ClaimForm=PlanCur.ClaimFormNum;
			//attach procedures
			//for(int i=0;i<tbAccount.SelectedIndices.Length;i++){
			List<long> listCodeNums=new List<long>();//List of codeNums that have had their default note added to the claim.
			for(int i=0;i<claimProcs.Length;i++){
				proc=listProcs[i];//1:1
				//Force claimproc prov to match procedure prov to stop mismatches.  We do not check ProcProvChangesClaimProcWithClaim here because that pref
				//only applies to claimprocs attached to an existing claim.  These claimprocs are still estimates and will be set to NotReceived below.
				claimProcs[i].ProvNum=proc.ProvNum;
				//ClaimProc ClaimProcCur=new ClaimProc();
				//ClaimProcCur.ProcNum=ProcCur.ProcNum;
				claimProcs[i].ClaimNum=ClaimCur.ClaimNum;
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//For Canada, update claimNums for labs associated to the current proc.
					List<Procedure> listLabProcs=procsForPat.FindAll(x => x.ProcNumLab==proc.ProcNum);
					List<long> listLabProcNums=listLabProcs.Select(x => x.ProcNum).ToList();
					if(listLabProcNums.Count>0) {
						//Limit by status to ignore claimprocs for preauth vs regular claim in case the claim proc is attached to both regular claim and preauth.
						ClaimProcList.FindAll(x => listLabProcNums.Contains(x.ProcNum)
							&& x.PlanNum==ClaimCur.PlanNum && x.InsSubNum==ClaimCur.InsSubNum && x.Status==claimProcs[i].Status)
						.ForEach(x =>
							{
								x.Status=(PlanCur.PlanType=="c"?ClaimProcStatus.CapClaim:ClaimProcStatus.NotReceived);//Keep lab proc status in sync with parents below.
								ClaimProcs.Update(x);
							}
						);
					}
				}
				//ClaimProcCur.PatNum=Patients.Cur.PatNum;
				//ClaimProcCur.ProvNum=ProcCur.ProvNum;
				//ClaimProcs.Cur.FeeBilled=;//handle in call to ClaimL.CalculateAndUpdate()
				//inspayest ''
				//dedapplied ''
				if(PlanCur.PlanType=="c")//if capitation
					claimProcs[i].Status=ClaimProcStatus.CapClaim;
				else
					claimProcs[i].Status=ClaimProcStatus.NotReceived;
				//inspayamt=0
				//remarks
				//claimpaymentnum=0
				//ClaimProcCur.PlanNum=Claims.Cur.PlanNum;
				//ClaimProcCur.DateCP=ProcCur.ProcDate;
				//writeoff handled in ClaimL.CalculateAndUpdate()
				#region CodeSent - If the logic in this region changes, then be sure to also modify Procedures.GetClaimDescript(). 
				if(PlanCur.UseAltCode && (ProcedureCodes.GetProcCode(proc.CodeNum).AlternateCode1!="")){
					claimProcs[i].CodeSent=ProcedureCodes.GetProcCode(proc.CodeNum).AlternateCode1;
				}
				else if(PlanCur.IsMedical && proc.MedicalCode!=""){
					claimProcs[i].CodeSent=proc.MedicalCode;
				}
				else{
					claimProcs[i].CodeSent=ProcedureCodes.GetProcCode(proc.CodeNum).ProcCode;
					if(claimProcs[i].CodeSent.Length>5 && claimProcs[i].CodeSent.Substring(0,1)=="D"){
						claimProcs[i].CodeSent=claimProcs[i].CodeSent.Substring(0,5);
					}
					if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
						if(claimProcs[i].CodeSent.Length>5) { //In Canadian electronic claims, codes can contain letters or numbers and cannot be longer than 5 characters.
							claimProcs[i].CodeSent=claimProcs[i].CodeSent.Substring(0,5);
						}
					}
				}
				#endregion CodeSent
				//Set the claim note to a concatenation of all the default procedure notes for new claims.
				ProcedureCode procCodeCur =ProcedureCodes.GetProcCode(proc.CodeNum);
				if(ClaimCur.ClaimNote==null) {
					ClaimCur.ClaimNote="";
				}
				if(!listCodeNums.Contains(procCodeCur.CodeNum)) {
					if(ClaimCur.ClaimNote.Length > 0 && !string.IsNullOrEmpty(procCodeCur.DefaultClaimNote)) {
						ClaimCur.ClaimNote+="\n";
					}
					ClaimCur.ClaimNote+=procCodeCur.DefaultClaimNote;
					listCodeNums.Add(procCodeCur.CodeNum);
				}
				if(!ClaimCur.IsOrtho && PrefC.GetBool(PrefName.OrthoClaimMarkAsOrtho)) {//if it's already marked as Ortho (from a previous procedure), just skip this logic.
					CovCat orthoCategory=CovCats.GetFirstOrDefault(x => x.EbenefitCat == EbenefitCategory.Orthodontics,true);
					if(orthoCategory!=null) {
						if(CovSpans.IsCodeInSpans(procCodeCur.ProcCode,CovSpans.GetWhere(x => x.CovCatNum==orthoCategory.CovCatNum).ToArray()))	{
							ClaimCur.IsOrtho=true;
							//ClaimCur.OrthoTotalM is a byte and patNote.OrthoMonthsTreatOverride is an integer so make sure it can fit.
							if(patNote.OrthoMonthsTreatOverride > 255) {
								ClaimCur.OrthoTotalM=255;
							}
							else if(!Byte.TryParse(patNote.OrthoMonthsTreatOverride.ToString(),out ClaimCur.OrthoTotalM)) {
								ClaimCur.OrthoTotalM=PrefC.GetByte(PrefName.OrthoDefaultMonthsTreat);
							}
							if(PrefC.GetBool(PrefName.OrthoClaimUseDatePlacement)) {
								DateTime orthoProcDate = Procedures.GetFirstOrthoProcDate(patNote);
								if(orthoProcDate != DateTime.MinValue) {
									ClaimCur.OrthoDate=orthoProcDate;
									//find OrthoTotalM minus the number of months that have passed since the OrthoDate.
									DateSpan dateDiff = new DateSpan(orthoProcDate,DateTime.Today);
									int txTimeInMonths=(dateDiff.YearsDiff * 12) + dateDiff.MonthsDiff + (dateDiff.DaysDiff < 15? 0: 1);
									try {
										ClaimCur.OrthoRemainM=PIn.Byte((ClaimCur.OrthoTotalM-txTimeInMonths).ToString());
									}
									catch { //catches anything that doesn't fit into a byte (eg, negatives) and just substitues 0.
										ClaimCur.OrthoRemainM=0;
									}
								}
							}
						}
					}
				}
			}//for claimProc
			List <ClaimProc> listClaimProcs=new List<ClaimProc>(claimProcs);
			for(int i=0;i<listClaimProcs.Count;i++) {
				listClaimProcs[i].LineNumber=(byte)(i+1);
				ClaimProcs.Update(listClaimProcs[i]);
			}
			Claims.CalculateAndUpdate(procsForPat,planList,ClaimCur,PatPlanList,Benefits.Refresh(PatPlanList,subList),pat,subList);
			//Insert claim snapshots for historical reporting purposes.
			if(PrefC.GetBool(PrefName.ClaimSnapshotEnabled) 
				&& PIn.Enum<ClaimSnapshotTrigger>(PrefC.GetString(PrefName.ClaimSnapshotTriggerType),true)==ClaimSnapshotTrigger.ClaimCreate
				&& claimType!="PreAuth")
			{
				ClaimSnapshots.CreateClaimSnapshot(ClaimProcs.Refresh(pat.PatNum).FindAll(x => x.ClaimNum==ClaimCur.ClaimNum),ClaimSnapshotTrigger.ClaimCreate,claimType);
			}
			return new ODTuple<bool,Claim,string>(true,ClaimCur,claimError);
		}

		///<summary>A class simply used to transfer data that is used for creating a claim.</summary>
		[Serializable]
		public class CreateClaimData {
			public List<PatPlan> ListPatPlans;
			public List<InsSub> ListInsSubs;
			public List<InsPlan> ListInsPlans;
			public List<ClaimProc> ListClaimProcs;
			public List<Procedure> ListProcs;
			public PatientNote PatNote;
		}

	}

	///<summary>The supplied DataRows must include the following columns: ProcNum,AdjNum,PayNum,ClaimNum,ClaimPaymentNum,PayPlanNum,StatementNum,
	///dateTimeSort,DateTime,(Priority not needed),ToothRange,ToothNum,ProcCode.  This sorts all objects in Account module based on their types, dates,
	///toothrange, toothnum, and proccode (for procedures with columns present, will also sort by priority and status).  Times are ignored for the
	///DateTime column if present, but not for the dateTimeSort column.</summary>
	class AccountLineComparer:IComparer<DataRow> {

		///<summary>Account entry types in this order: Procedures, Adjustments, Payments, Claims, ClaimPayments, PayPlans, Statements.</summary>
		public int Compare(DataRow x,DataRow y) {
			//if dates are different, then sort by date
			if(((DateTime)x["DateTime"]).Date!=((DateTime)y["DateTime"]).Date) {
				return (((DateTime)x["DateTime"]).Date).CompareTo(((DateTime)y["DateTime"]).Date);
			}
			#region Sort By Type
			#region Procedures
			if(x["ProcNum"].ToString()!="0" && y["ProcNum"].ToString()=="0") {
				return -1;
			}
			if(x["ProcNum"].ToString()=="0" && y["ProcNum"].ToString()!="0") {
				return 1;
			}
			if(x["ProcNum"].ToString()!="0" && y["ProcNum"].ToString()!="0") {
				return ProcedureLogic.CompareProcedures(x,y);//Sort procedures by status, priority, tooth region/num, proc code
			}
			#endregion Procedures
			#region Adjustments
			if(x["AdjNum"].ToString()!="0" && y["AdjNum"].ToString()=="0") {
				return -1;
			}
			if(x["AdjNum"].ToString()=="0" && y["AdjNum"].ToString()!="0") {
				return 1;
			}
			if(!PrefC.RandomKeys && x["AdjNum"].ToString()!="0" && y["AdjNum"].ToString()!="0") {//if RandomKeys enabled, sorted by dateTimeSort below
				return ((long)x["AdjNum"]).CompareTo((long)y["AdjNum"]);//if both adjustments on same date, order by primary key
			}
			#endregion Adjustments
			#region Payments
			if(x["PayNum"].ToString()!="0" && y["PayNum"].ToString()=="0") {
				return -1;
			}
			if(x["PayNum"].ToString()=="0" && y["PayNum"].ToString()!="0") {
				return 1;
			}
			if(!PrefC.RandomKeys && x["PayNum"].ToString()!="0" && y["PayNum"].ToString()!="0") {//if RandomKeys enabled, sorted by dateTimeSort below
				return ((long)x["PayNum"]).CompareTo((long)y["PayNum"]);//if both payments on same date, order by primary key
			}
			#endregion Payments
			#region Claims
			if(x["ClaimNum"].ToString()!="0" && y["ClaimNum"].ToString()=="0") {
				return -1;
			}
			if(x["ClaimNum"].ToString()=="0" && y["ClaimNum"].ToString()!="0") {
				return 1;
			}
			if(x["ClaimNum"].ToString()!="0" && y["ClaimNum"].ToString()!="0") {
				if(x["ClaimPaymentNum"].ToString()!="0" && y["ClaimPaymentNum"].ToString()=="0") {//claims before claimpayments
					return 1;
				}
				if(x["ClaimPaymentNum"].ToString()=="0" && y["ClaimPaymentNum"].ToString()!="0") {//claims before claimpayments
					return -1;
				}
				if(!PrefC.RandomKeys && x["ClaimPaymentNum"].ToString()=="0" && y["ClaimPaymentNum"].ToString()=="0") {//if RandomKeys enabled, sorted by dateTimeSort below
					return ((long)x["ClaimNum"]).CompareTo((long)y["ClaimNum"]);//if both claims on same date, order by primary key
				}
				//if both ClaimPaymentNums!=0, then both ClaimPaymentNums=="1" so we have to rely on sorting by dateTimeSort below
			}
			#endregion Claims
			#region Payplans
			if(x["PayPlanNum"].ToString()!="0" && y["PayPlanNum"].ToString()=="0") {
				return -1;
			}
			if(x["PayPlanNum"].ToString()=="0" && y["PayPlanNum"].ToString()!="0") {
				return 1;
			}
			if(!PrefC.RandomKeys && x["PayPlanNum"].ToString()!="0" && y["PayPlanNum"].ToString()!="0") {//if RandomKeys enabled, sorted by dateTimeSort below
				return ((long)x["PayPlanNum"]).CompareTo((long)y["PayPlanNum"]);//if both payplans (or payplancharges) on same date, order by primary key
			}
			#endregion Payplans
			#region Statements
			if(x["StatementNum"].ToString()!="0" && y["StatementNum"].ToString()=="0") {
				return -1;
			}
			if(x["StatementNum"].ToString()=="0" && y["StatementNum"].ToString()!="0") {
				return 1;
			}
			if(!PrefC.RandomKeys && x["StatementNum"].ToString()!="0" && y["StatementNum"].ToString()!="0") {//if RandomKeys enabled, sorted by dateTimeSort below
				return ((long)x["StatementNum"]).CompareTo((long)y["StatementNum"]);//if both statments on same date, order by primary key
			}
			#endregion Statements
			#endregion Sort By Type
			if(PrefC.RandomKeys) {//RandomKeys enabled, sort by dateTimeSort (SecDateTEntry if exists, otherwise DateTStamp or SecDateTEdit if available)
				return ((DateTime)x["dateTimeSort"]).CompareTo((DateTime)y["dateTimeSort"]);
			}
			return 0;
		}
	}


}
