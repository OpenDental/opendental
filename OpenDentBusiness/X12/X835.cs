using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using CodeBase;
using Newtonsoft.Json;

namespace OpenDentBusiness {
	///<summary>X12 835 Health Care Claim Payment/Advice. This transaction type is a response to an 837 claim submission. The 835 will always come after a 277 is received and a 277 will always come after a 999. Neither the 277 nor the 999 are required, so it is possible that an 835 will be received directly after the 837. The 835 is not required either, so it is possible that none of the 997, 999, 277 or 835 reports will be returned from the carrier.</summary>
	public class X835:X12object {
		
		///<summary>This is the Etrans entry which the X12 for this X835 came from.</summary>
		public Etrans EtransSource;
		///<summary>ST02 Empty string if this instance corresponds to the first transaction (EOB) within the 835.  Otherwise, a specific Transaction Set Identifer for a particular transaction (EOB) within the 835.</summary>
		private string _tranSetId;
		///<summary>All segments within the 835 report.</summary>
    private List<X12Segment> _listSegments;
		///<summary>The list of all provider level adjustments (one level above the claim level) within this 835.</summary>
		private List<Hx835_ProvAdj> _listProvAdjustments;
		///<summary>The list of all claim EOBs within this 835.</summary>
		private List<Hx835_Claim> _listClaimsPaid;
		///<summary>BPR01 converted into a human readable form.</summary>
		private string _transactionHandlingDescript;
		///<summary>BPR02 converted to decimal.</summary>
		private decimal _insPaid;
		///<summary>BPR03 converted to bool.</summary>
		private bool _isCredit;
		///<summary>BPR04 Payment Method Code.  Required.</summary>
		public string _paymentMethodCode;
		///<summary>BPR04 converted into a human readable form.</summary>
		private string _payMethodDescript;
		///<summary>BPR15 As many as 4 trailing digits of the account the payment was deposited into (only if payment was made electronically).  If not present, will be blank.</summary>
		private string _accountNumReceiving;
		///<summary>BPR16 The date of EFT of the date the check was printed.  If not present, will be set to 01/01/0001.</summary>
		private DateTime _dateEffective;
		///<summary>TRN02 Even through TRN02 is called the transaction reference number, it can include other characters besides digits.</summary>
		private string _transRefNum;
		///<summary>N1*PR N102</summary>
		private string _payerName;
		///<summary>N1*PR N104</summary>
		private string _payerId;
		///<summary>N3*PR N301</summary>
		private string _payerAddress;
		///<summary>N4*PR N401</summary>
		private string _payerCity;
		///<summary>N4*PR N402</summary>
		private string _payerState;
		///<summary>N4*PR N403</summary>
		private string _payerZip;
		///<summary>Represents all the data from the entire 1000A PER segment.</summary>
		private string _payerContactInfo;
		///<summary>N1*PE N102 loop 1000B.</summary>
		private string _payeeName;
		///<summary>N1*PE N103 loop 1000B.</summary>
		private string _payeeIdType;
		///<summary>N1*PE N104 loop 1000B.  Usually the NPI number.</summary>
		private string _payeeId;
		///<summary>ST02 Transaction Set Control Number (page 68)</summary>
		private string _controlId;

		///<summary>The list of all provider level adjustments (one level above the claim level) within this 835.</summary>
		public List<Hx835_ProvAdj> ListProvAdjustments { get { return _listProvAdjustments; }  set { _listProvAdjustments=value; } }
		///<summary>The list of all claim EOBs within this 835.</summary>
		public List<Hx835_Claim> ListClaimsPaid { get { return _listClaimsPaid; } set { _listClaimsPaid=value; } }
		///<summary>BPR01 converted into a human readable form.</summary>
		public string TransactionHandlingDescript { get { return _transactionHandlingDescript; } }
		///<summary>BPR02 converted to decimal.</summary>
		public decimal InsPaid { get { return _insPaid; } }
		///<summary>BPR03 converted to bool.</summary>
		public bool IsCredit { get { return _isCredit; } }
		///<summary>BPR04 converted into a human readable form.</summary>
		public string PayMethodDescript { get { return _payMethodDescript; } }
		///<summary>BPR15 As many as 4 trailing digits of the account the payment was deposited into (only if payment was made electronically).</summary>
		public string AccountNumReceiving { get { return _accountNumReceiving; } }
		///<summary>BPR16 The date of EFT or the date the check was printed.  If not present, will be set to 01/01/0001.</summary>
		public DateTime DateEffective { get { return _dateEffective; } }
		///<summary>TRN02 Even through TRN02 is called the transaction reference number, it can include other characters besides digits.</summary>
		public string TransRefNum { get { return _transRefNum; } }
		///<summary>N1*PR N104</summary>
		public string PayerName { get { return _payerName; } }
		///<summary>N1*PR N104</summary>
		public string PayerId { get { return _payerId; } }
		///<summary>N3*PR N301</summary>
		public string PayerAddress { get { return _payerAddress; } }
		///<summary>N4*PR N401</summary>
		public string PayerCity { get { return _payerCity; } }
		///<summary>N4*PR N402</summary>
		public string PayerState { get { return _payerState; } }
		///<summary>N4*PR N403</summary>
		public string PayerZip { get { return _payerZip; } }
		///<summary>Represents all the data from the entire 1000A PER segment.</summary>
		public string PayerContactInfo { get { return _payerContactInfo; } }
		///<summary>N1*PE N102 loop 1000B.</summary>
		public string PayeeName { get { return _payeeName; } }
		///<summary>N1*PE N103 loop 1000B.</summary>
		public string PayeeIdType { get { return _payeeIdType; } }
		///<summary>N1*PE N104 loop 1000B.  Usually the NPI number.</summary>
		public string PayeeId { get { return _payeeId; } }
		///<summary>ST02 Transaction Set Control Number (page 68)</summary>
		public string ControlId { get{ return _controlId; } }

		///<summary>If the 835 was paid electronically (EFT), then will return the effective date.  Otherwise, for physical checks, returns today's date.</summary>
		public DateTime DateReceived {
			get {
				if(_paymentMethodCode=="NON") {//No payment
					return DateTime.Today;
				}
				if(_paymentMethodCode=="ACH" && DateEffective.Year>1980) {//Electronic check
					return DateEffective;
				}
				if(_paymentMethodCode=="BOP" && DateEffective.Year>1980) {//Financial institution option.
					//I doubt we will ever see this status on our end.  A bank will usually see this status, then strip it out and replace it before it gets to us.
					//Safe to assume most banks will probably choose electronic deposit.
					return DateEffective;
				}
				if(_paymentMethodCode=="CHK") {//Physical check
					return DateTime.Today;
				}
				if(_paymentMethodCode=="FWT" && DateEffective.Year>1980) {//Wire transfer
					return DateEffective;
				}
				return DateTime.Today;
			}
		}

		#region Static Globals

		public static bool Is835(X12object xobj) {
			if(xobj.FunctGroups.Count!=1) {//Exactly 1 GS segment in each 835.
				return false;
			}
			if(xobj.FunctGroups[0].Header.Get(1)=="HP") {//GS01 (pg. 279)
				return true;
			}
			return false;
		}

		///<summary>Returns true if the required Application Sender's Code within the GS - Functional Group Header is "DENTICAL".
		///Returns false if there is no FunctGroups or the sender's code is not "DENTICAL".</summary>
		public static bool IsDentical(X12object xobj) {
			if(xobj.FunctGroups==null || xobj.FunctGroups.Count!=1) {//Exactly 1 GS segment in each 835.
				return false;
			}
			//Check the Application Sender's Code.  Code identifying party sending transmission; codes agreed to by trading partners.
			if(xobj.FunctGroups[0].Header.Get(2).Trim().ToUpper()=="DENTICAL") {//GS02 (standard guide pg. 279)
				return true;
			}
			return false;
		}

		#endregion Static Globals

		///<summary>This override is never explicitly used.  For serialization.</summary>
		public X835():base() {
			
		}

		///<summary>See guide page 62 for format outline.  Specify a specific Transaction Set Identifier (ST02) for a particular EOB within the 835.
		///Or set tranSetId to empty string to load the first EOB in the 835.  This class is only capable of loading a single transaction (EOB).
		///If isSimple is true, then will skip pseudo attaching claims by claimnum and setting the Hx835_Claim.IsSupplemental flag (saves queries).</summary>
		public X835(Etrans etransSource,string messageText,string tranSetId,List<Etrans835Attach> listAttached=null,bool isSimple=false):base(messageText) {//Parsing happens in the base class.
			EtransSource=etransSource;
			_tranSetId=tranSetId;
			ProcessMessage();
			DiscoverSplitClaims();
			SetAttached(listAttached);
			if(isSimple) {
				ClearSegments();
				return;
			}
			SetClaimNumsForUnattached();
		}
		
		public X835 Copy(){
			X835 x835=(X835)this.MemberwiseClone();
			x835.ListProvAdjustments=ListProvAdjustments.Select(x => x.Copy()).ToList();
			x835.ListClaimsPaid=ListClaimsPaid.Select(x => x.Copy()).ToList();
			return x835;
		}

		///<summary>Some carriers split every claim into one claim for each procedure (ex Commonwealth of Massachussetts/EOHHS/Office of Medicaid).</summary>
		private void DiscoverSplitClaims() {
			foreach(Hx835_Claim claim in ListClaimsPaid) {
				claim.ListOtherSplitClaims.Clear();
				claim.IsSplitClaim=false;
				//Do not treat reversals like a split claim.
				if(claim.IsReversal) {
					continue;
				}
				List<Hx835_Claim> listOtherSplitClaims=ListClaimsPaid.Where(
					x => x!=claim
					&& !ListTools.In(x.ClaimTrackingNumber,"0","")
					&& x.ClaimTrackingNumber==claim.ClaimTrackingNumber
					&& x.PatientName.Fname==claim.PatientName.Fname
					&& x.PatientName.Lname==claim.PatientName.Lname
				).ToList();
				bool hasBatchReversal=listOtherSplitClaims.Any(x => x.IsReversal);
				listOtherSplitClaims.RemoveAll(x => x.IsReversal && !IsClp02Equivalent(x,claim));
				//Multiple claims can be present with the same ClaimTrackingNumber especially when reading in 835 reports (multiple EOBs in one 835).
				//Compare the Payer Control Number for each claim being considered if provided. When this number differs, the claims are not actually split.
				//A common scenario is when a claim has been reversed and then corrected.
				//The ClaimTrackingNumber for the original and corrected claims will match but should not be 'split' due to different Payer Control Numbers.
				if(hasBatchReversal && !string.IsNullOrWhiteSpace(claim.PayerControlNumber)) {
					listOtherSplitClaims.RemoveAll(x => !string.IsNullOrWhiteSpace(x.PayerControlNumber) && x.PayerControlNumber!=claim.PayerControlNumber);
				}
				if(listOtherSplitClaims.Count==0) {
					continue;//Not a split claim
				}
				claim.IsSplitClaim=true;
				claim.ListOtherSplitClaims=listOtherSplitClaims;
			}
		}

		///<summary>Returns true if Clp02's are equivelent(see code) for both Hx835_Claims.</summary>
		private bool IsClp02Equivalent(Hx835_Claim claim1,Hx835_Claim claim2) {
			switch(claim1.CodeClp02) {
				case "1"://Processed as Primary
				case "19"://Processed as Primary, Forwarded to Additional Payer(s)
					return ListTools.In(claim2.CodeClp02,"1","19");
				case "2"://Processed as Secondary
				case "20"://Processed as Secondary, Forwarded to Additional Payer(s)
					return ListTools.In(claim2.CodeClp02,"2","20");
				case "3"://Processed as Tertiary
				case "21"://Processed as Tertiary, Forwarded to Additional Payer(s)
					return ListTools.In(claim2.CodeClp02,"3","21");
				case "4"://Denied
					return ListTools.In(claim2.CodeClp02,"4");
				case "22"://Reversal of Previous Payment
					return ListTools.In(claim2.CodeClp02,"22");
				case "23"://Not Our Claim, Forwarded to Additional Payer(s)
					return ListTools.In(claim2.CodeClp02,"23");
				case "25"://Predetermination Pricing Only - No Payment
					return ListTools.In(claim2.CodeClp02,"25");
			}
			return false;//Just in case.
		}

		private void SetAttached(List <Etrans835Attach> listAttached) {
			if(listAttached==null || listAttached.Count==0) {
				return;
			}
			//listAttached can contain items created from split claims.
			//These items have the same; etransNum, claimNum and ClpSegmentIndex but different DateTimeEntry.
			//The item with the earliest DateTimeEntry is the original claim associated to the Hx835_Claim and next is the split claim
			//Given listAttached is ordered so that each Hx835_Claim is not associated to the split claim incorrectly.
			foreach(Hx835_Claim claim in ListClaimsPaid) {
				Etrans835Attach attach=listAttached.FirstOrDefault(x => x.ClpSegmentIndex==claim.ClpSegmentIndex && x.EtransNum==EtransSource.EtransNum);
				if(attach!=null) {
					claim.ClaimNum=attach.ClaimNum;
					claim.IsAttachedToClaim=true;
				}
			}
		}

		///<summary>When listClaimNums is null this will run a query.  The listClaimNums count must match listUnattached count and be in the same order.</summary>
		public void SetClaimNumsForUnattached(List<long> listClaimNums=null,List<Hx835_Claim> listUnattached=null) {
			if(listUnattached==null) {
				listUnattached=GetUnattached();
			}
			if(listClaimNums==null) {
				listClaimNums=Claims.GetClaimFromX12(GetClaimMatches(listUnattached));
			}
			if(listClaimNums!=null) {
				for(int i=0;i<listUnattached.Count;i++) {
					listUnattached[i].ClaimNum=listClaimNums[i];
					listUnattached[i].IsAttachedToClaim=false;
				}
			}
		}

		public void RefreshAttachesAndClaimProcsFromDb(out List<Etrans835Attach> listAttaches,out List<Hx835_ShortClaimProc> listClaimProcs) {
			List<long> listClaimNums=ListClaimsPaid.Select(x => x.ClaimNum).Where(x => x!=0).ToList();
			listAttaches=Etrans835Attaches.GetForEtransNumOrClaimNums(false,EtransSource.EtransNum,listClaimNums.ToArray());//Includes manually detached and split attaches.
			//We have to add additional claimNums from listAttaches to account for claims split from their original ERA.
			listClaimNums.AddRange(listAttaches.Where(x => x.ClaimNum!=0).Select(x => x.ClaimNum).Distinct());
			listClaimProcs=Hx835_ShortClaimProc.RefreshForClaims(listClaimNums);
		}

		///<summary>Returns a list of X12ClaimMatchs for unattached Hx835_Claims.</summary>
		public List<X12ClaimMatch> GetClaimMatches(List<Hx835_Claim> listUnattached=null) {
			List<X12ClaimMatch> listClaimMatches=new List<X12ClaimMatch>();
			if(listUnattached==null) {
				listUnattached=GetUnattached();
			}
			foreach(Hx835_Claim claim in listUnattached) {
				X12ClaimMatch claimMatch=new X12ClaimMatch();
				claimMatch.ClaimIdentifier=claim.ClaimTrackingNumber;
				claimMatch.ClaimFee=(double)claim.ClaimFee;
				claimMatch.DateServiceStart=claim.DateServiceStart;
				claimMatch.DateServiceEnd=claim.DateServiceEnd;
				claimMatch.PatFname=claim.PatientName.Fname;
				claimMatch.PatLname=claim.PatientName.Lname;
				claimMatch.SubscriberId=claim.SubscriberName.SubscriberId;
				claimMatch.EtransNum=EtransSource.EtransNum;
				claimMatch.Is835Reversal=(claim.IsReversal);
				claimMatch.List835Procs=claim.ListProcs.Where(x => x.ProcNum!=0).ToList();
				claimMatch.CodeClp02=claim.CodeClp02;
				listClaimMatches.Add(claimMatch);
			}
			return listClaimMatches;
		}

		private List <Hx835_Claim> GetUnattached() {
			List <Hx835_Claim> listUnattached=new List<Hx835_Claim>();
			foreach(Hx835_Claim claim in ListClaimsPaid) {
				if(!claim.IsAttachedToClaim) {
					listUnattached.Add(claim);
				}
			}
			return listUnattached;
		}
		
		///<summary>Clears all segments from both the X835 and X12Object.
		///Called when constructing a isSimple X835, used to save memory.</summary>
		public void ClearSegments() {
			_listSegments=new List<X12Segment>();
			Segments=new List<X12Segment>();
			FunctGroups=new List<X12FunctionalGroup>();
		}

		private void ProcessMessage() {
			//Table 1 - Header
			//ST: Transaction Set Header.  Required.  Repeat 1.  Guide page 68.  The GS segment contains exactly one ST segment below it.
			_controlId=FunctGroups[0].Transactions[0].Header.Get(2);//ST02 (page 68) - Should always exist
			_listSegments=FunctGroups[0].Transactions[0].Segments;
			for(int i=1;i<FunctGroups[0].Transactions.Count;i++) {
				if(_tranSetId==FunctGroups[0].Transactions[i].Header.Get(2)) {
					_controlId=_tranSetId;
					_listSegments=FunctGroups[0].Transactions[i].Segments;
					break;
				}
			}
			ProcessBPR(0);
			ProcessTRN(1);
			int segNum=2;
			//CUR: Foreign Currency Information.  Situational.  Repeat 1.  Guide page 79.  We do not use.
			if(segNum<_listSegments.Count && _listSegments[segNum].SegmentID=="CUR") {
				segNum++;
			}
			//REF*EV: Receiver Identification.  Situational.  Repeat 1.  Guide page 82.  We do not use.
			if(segNum<_listSegments.Count && _listSegments[segNum].SegmentID=="REF" && _listSegments[segNum].Get(1)=="EV") {
				segNum++;
			}
			//REF*F2: Version Identification.  Situational.  Repeat 1.  Guide page 84.  We do not use.
			if(segNum<_listSegments.Count && _listSegments[segNum].SegmentID=="REF" && _listSegments[segNum].Get(1)=="F2") {
				segNum++;
			}
			//DTM: Production Date.  Situational.  Repeat 1.  Guide page 85.  We do not use.
			if(segNum<_listSegments.Count && _listSegments[segNum].SegmentID=="DTM") {
				segNum++;
			}
			ProcessN1_PR(segNum);
			segNum++;
			ProcessN3_PR(segNum);
			segNum++;
			ProcessN4_PR(segNum);
			segNum++;
			//1000A REF: Additional Payer Identification.  Situational.  Repeat 4.  Guide page 92.  We do not use.
			while(segNum<_listSegments.Count && _listSegments[segNum].SegmentID=="REF") {
				segNum++;
			}
			_payerContactInfo="";
			while(segNum<_listSegments.Count && _listSegments[segNum].SegmentID=="PER") {
				//The order of each of these PER segments are as defined in the guide page 62.
				//Some clearinghouses have sent them back in the wrong order.
				//1000A PER*CX: Payer Business Contact Information.  Situational.  Repeat 1.  Guide page 94.  We do not use.
				if(_listSegments[segNum].Get(1)=="CX") {					
				}
				//1000A PER*BL: Payer Technical Contact Information.  Required (but is not included in any of the examples, so we assume situational).  Repeat >1.  Guide page 97.  We only care about the first non-empty occurrence.				
				else if(_listSegments[segNum].Get(1)=="BL") {
					if(_payerContactInfo=="") {
						_payerContactInfo=ProcessPER_BL(segNum);
					}
				}
				//1000A PER*IC: Payer WEB Site.  Situational.  Repeat 1.  Guide page 100.  We do not use.
				else if(_listSegments[segNum].Get(1)=="IC") {
				}
				segNum++;
			}
			//1000B N1*PE: Payee Identification.  Required.  Repeat 1.  Guide page 102.
			if(segNum<_listSegments.Count && _listSegments[segNum].SegmentID=="N1" && _listSegments[segNum].Get(1)=="PE") {
				ProcessN1_PE(segNum);
				segNum++;
			}
			//1000B N3: Payee Address.  Situational.  Repeat 1.  Guide page 104.  We do not use because the payee already knows their own address, and because it is not required.
			if(segNum<_listSegments.Count && _listSegments[segNum].SegmentID=="N3") {
				segNum++;
			}
			//1000B N4: Payee City, State, ZIP Code.  Situational.  Repeat 1.  Guide page 105.  We do not use because the payee already knows their own address, and because it is not required.
			if(segNum<_listSegments.Count && _listSegments[segNum].SegmentID=="N4") {
				segNum++;
			}
			//1000B REF: Payee Additional Identification.  Situational.  Repeat >1.  Guide page 107.  We do not use.
			while(segNum<_listSegments.Count && _listSegments[segNum].SegmentID=="REF") {
				segNum++;
			}
			//1000B RDM: Remittance Delivery Method.  Situational.  Repeat 1.  Guide page 109.  We do not use.
			if(segNum<_listSegments.Count && _listSegments[segNum].SegmentID=="RDM") {
				segNum++;
			}
			//Table 2 - Detail
			//Loop 2000 Header Number.  Repeat >1.  We do not need the information in this loop, because claim payments include the unique claim identifiers that we need to match to the claims one-by-one.
			_listClaimsPaid=new List<Hx835_Claim>();
			bool isLoop2000=true;
			while(isLoop2000) {
				isLoop2000=false;
				//2000 LX: Header Number.  Situational.  Repeat 1.  Guide page 111.  We do not use.
				if(segNum<_listSegments.Count && _listSegments[segNum].SegmentID=="LX") {
					isLoop2000=true;
					segNum++;
				}
				//2000 TS3: Provider Summary Information.  Repeat 1.  Guide page 112.
				string npi="";
				if(segNum<_listSegments.Count && _listSegments[segNum].SegmentID=="TS3") {
					isLoop2000=true;
					npi=_listSegments[segNum].Get(1);
					segNum++;
				}
				//2000 TS2: Provider Supplemental Summary Infromation.  Repeat 1.  Guide page 117.  We do not use.
				if(segNum<_listSegments.Count && _listSegments[segNum].SegmentID=="TS2") {
					isLoop2000=true;
					segNum++;
				}
				//Loop 2100 Claim Payment Information.  Repeat 1.  Guide page 123.
				if(segNum<_listSegments.Count && _listSegments[segNum].SegmentID=="CLP") {
					isLoop2000=true;
					Hx835_Claim claimPaid=ProcessCLP(segNum,npi);
					claimPaid.Era=this;
					_listClaimsPaid.Add(claimPaid);
					segNum+=claimPaid.SegmentCount;
				}
			}
			//Table 3 - Summary
			//PLB: Provider Admustment.  Situational.  Repeat >1.  Guide page 217.
			_listProvAdjustments=new List<Hx835_ProvAdj>();
			while(segNum<_listSegments.Count && _listSegments[segNum].SegmentID=="PLB") {
				_listProvAdjustments.AddRange(ProcessPLB(segNum));
				segNum++;
			}
		}
		
		///<summary>Returns a dictionary such that Key => claimNum, Value => List of matched era claims.
		///ERA claims that were manually detached will have a key of 0.
		///ERA claims that can not be matched to given listClaim options will have key of -1.
		///All other matched ERAs claims in ListClaimsPaid will have a key in returned dictionary.</summary>
		public Dictionary<long,List<Hx835_Claim>> GetClaimsPaidDict() {
			Dictionary<long,List<Hx835_Claim>> dictMatchedEraClaims=new Dictionary<long, List<Hx835_Claim>>();
			foreach(Hx835_Claim eraClaim in this.ListClaimsPaid) {
				long claimNum;//Either -1, 0 or valid ClaimNum
				switch(eraClaim.ClaimStatus) {
					#region set claimNum
					default://Just in case.
					case EraClaimStatus.NotMatched:
						claimNum=-1;
					break;
					case EraClaimStatus.ManuallyDetached:
						claimNum=0;
					break;
					case EraClaimStatus.Attached:
						claimNum=eraClaim.ClaimNum;
					break;
						#endregion
				}
				if(!dictMatchedEraClaims.ContainsKey(claimNum)) {
					dictMatchedEraClaims[claimNum]=new List<Hx835_Claim>();
				}
				dictMatchedEraClaims[claimNum].Add(eraClaim);
			}
			return dictMatchedEraClaims;
		}

		///<summary>Given listClaims should be a list of claims that have been filtered down for this ERA using ListPaidClaims ClaimNums.</summary>
		public List<X835ClaimData> GetClaimDataList(List<Hx835_ShortClaim> listClaims,Dictionary<long,bool> dictClaimPaymentsExist=null) {
			List<long> listClaimNums=this.ListClaimsPaid.Select(x => x.ClaimNum).Where(x => x!=0).ToList();
			//Every claim num is associated to a bool. True when there is an existing claimPayment.
			if(dictClaimPaymentsExist==null) {
				dictClaimPaymentsExist=ClaimPayments.HasClaimPayment(listClaimNums);
				dictClaimPaymentsExist.Add(-1,false);//-1 for unmatched claims, value doesn't effect anything currently.  Simply used for reference.
				dictClaimPaymentsExist.Add(0,false);//0 for manually detached claims, value doesn't effect anything currently.  Simply used for reference.
			}
			Dictionary<long,List<Hx835_Claim>> dictMatchedEraClaims=GetClaimsPaidDict();//Will contain keys -1 and 0 like above if needed.
			List<X835ClaimData> listRetVals=new List<X835ClaimData>();
			foreach(Hx835_Claim eraClaim in this.ListClaimsPaid) {
				Hx835_ShortClaim matchedClaim;//Either null, manually detached or valid claim.
				switch(eraClaim.ClaimStatus) {
					default://Just in case.
					case EraClaimStatus.NotMatched:
						matchedClaim=null;
					break;
					case EraClaimStatus.ManuallyDetached:
						matchedClaim=new Hx835_ShortClaim();
					break;
					case EraClaimStatus.Attached:
						//This could still be null but listClaims should be a filtered list of claims based on ListClaimsPaid ClaimNums.
						matchedClaim=listClaims.FirstOrDefault(x => x.ClaimNum==eraClaim.ClaimNum);
					break;
				}
				long claimNum=-1;
				if(matchedClaim!=null) {
					claimNum=matchedClaim.ClaimNum;
				}
				List<Hx835_Claim> listHx835Claims=new List<Hx835_Claim>();
				if(dictMatchedEraClaims.ContainsKey(claimNum)) {
					listHx835Claims=dictMatchedEraClaims[claimNum];
				}
				listRetVals.Add(new X835ClaimData(matchedClaim,dictClaimPaymentsExist[claimNum],listHx835Claims));
			}
			return listRetVals;
		}

		public X835Status GetStatus(List<Hx835_ShortClaim> listClaims,List<Hx835_ShortClaimProc> listAllClaimProcs,List<Etrans835Attach> listAllAttaches,
			Dictionary<long,bool> dictClaimPaymentsExist=null)
		{
			return GetStatus(this.GetClaimDataList(listClaims,dictClaimPaymentsExist),listAllClaimProcs,listAllAttaches);
		}

		public X835Status GetStatus(List<X835ClaimData> listClaimDatas,List<Hx835_ShortClaimProc> listAllClaimProcs,List<Etrans835Attach> listAllAttaches) { 
			int countProcessed=0;
			int countRevievedWithPay=0;
			int countOther=0;
			int countDetached=0;//Count of manually detached claims. These claims are not considered in status.
			foreach(X835ClaimData claimData in listClaimDatas) {
				if(claimData.IsManuallyDetached) {//Manually detached claim created for claim that was unattached by user.
					countDetached++;
					continue;
				}
				else if(claimData.IsClaimReceived) {
					long etransNum=this.EtransSource.EtransNum;
					Hx835_Claim hx835Claim;//Should always match at least one item.
					if(claimData.TryGetEraClaim(etransNum,out hx835Claim) && hx835Claim.IsProcessed(listAllClaimProcs,listAllAttaches)) {
						countProcessed++;
						if(claimData.HasPayment || hx835Claim.IsPreauth) {
							//Either has finalized claimPayment or is attached to an internal preauth. Either way treat it like it has payment.
							countRevievedWithPay++;
						}
						continue;
					}
				}
				countOther++;//Noramlly this means a claim could not be matched (claimData.AttachedClaim==null) or attached claim is not recieved.
			}
			if(countProcessed==this.ListClaimsPaid.Count-countDetached) {//All claims found and marked recieved.
				if(countRevievedWithPay==this.ListClaimsPaid.Count-countDetached) {
					if(countDetached > 0) {
						if(countDetached==this.ListClaimsPaid.Count) {//All ERA claims have been manually detached.
							return X835Status.FinalizedAllDetached;
						}
						else {
							return X835Status.FinalizedSomeDetached;//All recieved and have existing ins check, but some detached.
						}
					}
					return X835Status.Finalized;//All recieved and have existing ins check.
				}
				return X835Status.NotFinalized;//All recieved, but at least 1 missing an ins check.
			}
			else if(countOther==this.ListClaimsPaid.Count-countDetached) {//Not recieved.
				return X835Status.Unprocessed;
			}
			else if(countProcessed>0) {//Some recieved but not all.
				return X835Status.Partial;
			}
			return X835Status.None;//Just in case
		}

		///<summary>Can derive the status from data held within this class but uses several queries.</summary>
		public X835Status GetStatus() {
			List<Hx835_ShortClaim> listAttachedClaims=RefreshClaims().Select(x => new Hx835_ShortClaim(x)).ToList();
			RefreshAttachesAndClaimProcsFromDb(out List<Etrans835Attach> listAttaches,out List<Hx835_ShortClaimProc> listClaimProcs);
			return GetStatus(listAttachedClaims,listClaimProcs,listAttaches);
		}

		///<summary>Returns a list of claims from the DB that are attached to the current era claims.
		///Going to the DB ensures that we are working with the most current claim information for validation.</summary>
		public List<Claim> RefreshClaims() {
			List<long> listClaimNums=this.ListClaimsPaid.Select(x => x.ClaimNum).Where(x => x!=0).ToList();
			return Claims.GetClaimsFromClaimNums(listClaimNums);
		}

		///<summary>Returns a list of claims from the Db that are attached to the ERA. Will filter an existing list if one is passed in.
		///Detaches preauths from the ERA that aren't attached to a claim. Does not include entries for Preauths or Detatched claims.
		///Replaces any nulls in the list with a new claim that has ClaimNum 0.</summary>
		public List<Claim> GetClaimsForFinalization(List<Claim> listClaimsFor835=null) {
			if(listClaimsFor835==null) {
				listClaimsFor835=this.RefreshClaims();
			}
			List<Claim> listClaims=new List<Claim>();
			List<Hx835_Claim> listSkippedPreauths=this.ListClaimsPaid.FindAll(x => x.IsPreauth && !x.IsAttachedToClaim);
			for(int i=0;i<listSkippedPreauths.Count;i++) {
				Etrans835Attaches.DetachEraClaim(listSkippedPreauths[i]);
			}
			for(int i=0;i<this.ListClaimsPaid.Count;i++) {
				if((this.ListClaimsPaid[i].IsAttachedToClaim && this.ListClaimsPaid[i].ClaimNum==0) //User manually detached claim.
					|| this.ListClaimsPaid[i].IsPreauth)
				{
					continue;
				}
				listClaims.Add(listClaimsFor835.FirstOrDefault(x => x.ClaimNum==this.ListClaimsPaid[i].ClaimNum));//Can add nulls
				int index=listClaims.Count-1;
				Claim claimCur=listClaims[index];
				if(claimCur==null) {//Claim wasn't found in DB.
					claimCur=new Claim();//ClaimNum will be 0, indicating that this is not a real claim.
					listClaims[index]=claimCur;
				}
				claimCur.TagOD=this.ListClaimsPaid[i];
			}
			return listClaims;
		}

		#region Segment Processing

		///<summary>AMT segments are found both at the claim and procedure levels.</summary>
		private Hx835_Info ProcessAMT(int segNum) {
			X12Segment segAMT=_listSegments[segNum];
			Hx835_Info info=new Hx835_Info();
			info.FieldName=GetDescriptForAmountQualifierCode(segAMT.Get(1));
			info.FieldValue=PIn.Decimal(segAMT.Get(2)).ToString("f2");
			return info;
		}

		///<summary>BPR: Financial Information.  Required.  Repeat 1.  Guide page 69.</summary>
		private void ProcessBPR(int segNum) {
			X12Segment segBPR=_listSegments[segNum];
			//BPR01 Transaction Handling Code.  Required.
			_transactionHandlingDescript=this.GetDescriptForTransactionHandlingCode(segBPR.Get(1));
			//BPR02 Total Actual Provider Payment Amount.  Required.
			_insPaid=PIn.Decimal(segBPR.Get(2));
			//BPR03 Credit/Debit Flag Code.  Required.
			_isCredit=false;
			if(segBPR.Get(3)=="C") {
				_isCredit=true;
			}
			//BPR04 Payment Method Code.  Required.
			_paymentMethodCode=segBPR.Get(4);
			_payMethodDescript=GetDescriptForPaymentMethodCode(_paymentMethodCode);
			//BPR05 Payment Format Code.  Situational.  We do not use.
			//BPR06 (DFI) ID Number Qualifier.  Situational.  We do not use.
			//BPR07 (DFI) Identification Number.  Situational.  We do not use.
			//BPR08 Account Number Qualifier.  Situational, but will always be the same value if present.  DA=Demand Deposit.  We do not use.
			//BPR09 Sender Bank Account Number.  Situational.  We do not use.
			//BPR10 Originating Company Identifier.  Situational.  We do not use.
			//BPR11 Originating Company Supplemental Code.  Situational.  We do not use.
			//BPR12 (DFI) ID Number Qualifier.  Situational.  We do not use.
			//BPR13 (DFI) Identification Number.  Situational.  We do not use.
			//BPR14 Account Number Qualifier.  Situational.  Only two values allowed, DA=Demand Deposit, SG=Savings.  We do not use.
			//BPR15 Receiver or Provider Account Number.  Situational.
			_accountNumReceiving=segBPR.Get(15);
			if(_accountNumReceiving.Length>4) {
				//Do not show more than 4 trailing digits for security reasons.
				//For the examples I have seen, this account number should already be a short 4 or 5 digit version of the actual account number, so this replacement is just a safeguard.
				_accountNumReceiving=_accountNumReceiving.Substring(_accountNumReceiving.Length-4);
			}
			//BPR16 Date EFT Effective or Date Check Issued.  Situational.
			_dateEffective=X12Parse.ToDate(segBPR.Get(16));
			//BPR17 through BPR21 at not used in the format.
		}

		///<summary>Converts a CAS segment into a list of up to 6 adjustments.</summary>
		private List<Hx835_Adj> ProcessCAS(int segNum) {
			X12Segment segCAS=_listSegments[segNum];
			List<Hx835_Adj> listAdjustments=new List<Hx835_Adj>();
			string adjCode=segCAS.Get(1);
			string adjDescript=AdjCodeToAdjDescript(adjCode);
			//Each CAS segment can contain up to 6 adjustments of the same type.
			for(int k=2;k<=17;k+=3) {
				Hx835_Adj adj=new Hx835_Adj();
				adj.AdjCode=adjCode;
				adj.AdjustRemarks=adjDescript;
				string strAdjReasonCode=segCAS.Get(k);
				string strAmt=segCAS.Get(k+1);
				if(strAdjReasonCode=="" && strAmt=="") {
					continue;
				}
				adj.AdjAmt=PIn.Decimal(strAmt);
				if(adj.AdjAmt==0) {
					continue;
				}
				adj.ReasonDescript="";
				if(strAdjReasonCode!="") {
					adj.ReasonDescript=GetDescriptFrom139(strAdjReasonCode);
					adj.ListClaimAdjReasonCodes.Add(strAdjReasonCode);
				}
				adj.IsDeductible=false;
				if(adjCode=="PR" && strAdjReasonCode=="1") {
					adj.IsDeductible=true;
				}
				else if(adjCode!="PR" && strAdjReasonCode=="45") {//"Charge exceeds fee schedule/maximum allowable or contracted/legislated fee arrangement."
					//See task #1830229.  Delta Dental of MA was sending Payor Initiated adjustments with reason code 45.  EX: CAS*PI*45*108.07~
					//By the definition of reason 45, this should be a Contractual Obligation.
					adj.AdjCode="CO";//Treat as Contractual Obligation (CO) regardless of given adjustment code (except for patient responsibility PR).
					adj.AdjustRemarks=AdjCodeToAdjDescript(adj.AdjCode);
				}
				listAdjustments.Add(adj);
			}
			return listAdjustments;
		}

		///<summary>See pages 131 and 198 in the guide for claim level and procedure level descriptions.</summary>
		private string AdjCodeToAdjDescript(string adjCode) {
			if(adjCode=="CO") {
				return "Contractual Obligations";
			}
			else if(adjCode=="PI") {
				return "Payor Initiated Reductions";
			}
			else if(adjCode=="PR") {
				return "Patient Responsibility";
			}
			return "Other Adjustments";//adjCode=="OA"
		}

		///<summary>2100 CLP: Claim Payment Information.  Required.  Repeat 1.  Guide page 123.</summary>
		private Hx835_Claim ProcessCLP(int segNum,string npi) {
			int segNumCLP=segNum;
			Hx835_Claim retVal=new Hx835_Claim();
			X12Segment segCLP=_listSegments[segNum];
			retVal.ClpSegmentIndex=segCLP.SegmentIndex;
			retVal.Npi=npi;
			retVal.ClaimTrackingNumber=segCLP.Get(1);//CLP01
			retVal.PayerControlNumber=segCLP.Get(7);//CLP07 Payer Claim Control Number
			string clp02=segCLP.Get(2);
			retVal.CodeClp02=clp02;
			retVal.IsPreauth=(clp02=="25");
			retVal.IsReversal=(clp02=="22");
			retVal.StatusCodeDescript=GetDescriptForClaimStatusCode(clp02);//CLP02 Claim Status Code Description
			retVal.ClaimFee=PIn.Decimal(segCLP.Get(3));//CLP03 Total Claim Charge Amount
			retVal.InsPaid=PIn.Decimal(segCLP.Get(4));//CLP04 Claim Payment Amount
			retVal.PatientRespAmt=PIn.Decimal(segCLP.Get(5));//CLP05 Patient Responsibility Amount
			retVal.DateReceived=DateReceived;
			segNum++;
			retVal.ListClaimAdjustments=new List<Hx835_Adj>();
			//2100 CAS: Claim Adjustment.  Situational.  Repeat 99.  Guide page 129.
			while(segNum<_listSegments.Count && _listSegments[segNum].SegmentID=="CAS") {
				retVal.ListClaimAdjustments.AddRange(ProcessCAS(segNum));
				segNum++;
			}
			retVal.ClaimAdjustmentTotal=0;
			//2100 NM1: Patient Name.  Required.  Repeat 1.  Guide page 137.
			retVal.PatientName=new Hx835_Name();//Unfortunately, ClaimConnect sometimes does not include the required patient name in the NM1*QC segment.
			retVal.PatientName.Fname="UNKNOWN";
			retVal.PatientName.Mname="";
			retVal.PatientName.Lname="UNKNOWN";
			retVal.PatientName.Suffix="";
			retVal.PatientName.SubscriberId="";
			retVal.PatientName.SubscriberIdTypeDesc="";
			if(segNum<_listSegments.Count && _listSegments[segNum].SegmentID=="NM1" && _listSegments[segNum].Get(1)=="QC") {
				retVal.PatientName=ProcessNM1_Person(segNum);
				segNum++;
			}
			//2100 NM1: Insured Name.  Situational.  Required when different from the patient.  Repeat 1.  Guide page 140.
			retVal.SubscriberName=retVal.PatientName;
			if(segNum<_listSegments.Count && _listSegments[segNum].SegmentID=="NM1" && _listSegments[segNum].Get(1)=="IL") {
				retVal.SubscriberName=ProcessNM1_Person(segNum);
				segNum++;
			}
			//2100 NM1: Corrected Patient/Insured Name.  Situational.  Repeat 1.  Guide page 143.  We do not use.
			if(segNum<_listSegments.Count && _listSegments[segNum].SegmentID=="NM1" && _listSegments[segNum].Get(1)=="74") {
				segNum++;
			}
			//2100 NM1: Service Provider Name.  Situational.  Repeat 1.  Guide page 146.  We do not use.
			if(segNum<_listSegments.Count && _listSegments[segNum].SegmentID=="NM1" && _listSegments[segNum].Get(1)=="82") {
				segNum++;
			}
			//2100 NM1: Crossover Carrier Name.  Situational.  Repeat 1.  Guide page 150.  We do not use.
			if(segNum<_listSegments.Count && _listSegments[segNum].SegmentID=="NM1" && _listSegments[segNum].Get(1)=="TT") {
				segNum++;
			}
			//2100 NM1: Corrected Priority Payer Name.  Situational.  Repeat 1.  Guide page 153.  We do not use.
			if(segNum<_listSegments.Count && _listSegments[segNum].SegmentID=="NM1" && _listSegments[segNum].Get(1)=="PR") {
				segNum++;
			}
			//2100 NM1: Other Subscriber Name.  Situational.  Repeat 1.  Guide page 156.  We do not use.
			if(segNum<_listSegments.Count && _listSegments[segNum].SegmentID=="NM1" && _listSegments[segNum].Get(1)=="GB") {
				segNum++;
			}
			//2100 MIA: Inpatient Adjudication Information.  Situational.  Repeat 1.  Guide page 159. 
			retVal.ListAdjudicationInfo=new List<Hx835_Info>();
			if(segNum<_listSegments.Count && _listSegments[segNum].SegmentID=="MIA") {
				retVal.ListAdjudicationInfo.AddRange(ProcessMIA(segNum));
				segNum++;
			}
			//2100 MOA: Outpatient Adjudication Information.  Situational.  Repeat 1.  Guide page 166.
			if(segNum<_listSegments.Count && _listSegments[segNum].SegmentID=="MOA") {
				retVal.ListAdjudicationInfo.AddRange(ProcessMOA(segNum));
				segNum++;
			}
			retVal.IsSplitClaim=false;
			for(int i=0;i<retVal.ListAdjudicationInfo.Count;i++) {
				Hx835_Info info=retVal.ListAdjudicationInfo[i];
				if(info.IsRemarkCode && info.FieldValueRaw=="MA15") {
					retVal.IsSplitClaim=true;
					break;
				}
			}
			//2100 REF: Other Claim Releated Identification.  Situational.  Repeat 5.  Guide page 169.  We do not use.
			//2100 REF: Rendering Provider Identification.  Situational.  Repeat 10.  Guide page 171.  We do not use.
			while(segNum<_listSegments.Count && _listSegments[segNum].SegmentID=="REF") {//We clump 2 segments into a single loop, because neither segment is used, and there are multiple REF01 choices for each.
				segNum++;
			}
			//2100 DTM: Statement From or To Date.  Situational.  Required if at least one service line is missing a service date.  Service line dates override this date.  Repeat 2.  Guide page 173.
			retVal.DateServiceStart=DateTime.MinValue;
			retVal.DateServiceEnd=DateTime.MinValue;
			while(segNum<_listSegments.Count && _listSegments[segNum].SegmentID=="DTM" &&
				(_listSegments[segNum].Get(1)=="050" || _listSegments[segNum].Get(1)=="232" || _listSegments[segNum].Get(1)=="233"))
			{
				if(_listSegments[segNum].Get(2)=="00000000") {
					//Sometimes clearinghouses send us empty dates (bugs).  This is also common for preauths.
				}
				else if(_listSegments[segNum].Get(1)=="232") {
					retVal.DateServiceStart=X12Parse.ToDate(_listSegments[segNum].Get(2));
				}
				else if(_listSegments[segNum].Get(1)=="233") {
					retVal.DateServiceEnd=X12Parse.ToDate(_listSegments[segNum].Get(2));
				}
				else {//050
					//EDS has sent us DTM*050 segments in the past, but they are not part of the standard and we do not have a use for them.
					//We specifically skip DTM*050 segments so that we can ensure to import the valid DTM*232 and DTM*233 segments.
				}
				segNum++;
			}
			if(retVal.DateServiceStart!=DateTime.MinValue && retVal.DateServiceEnd==DateTime.MinValue) {//Start date was provided, but end date was not.
				retVal.DateServiceEnd=retVal.DateServiceStart;//The end date is the same as the start date.
			}
			else if(retVal.DateServiceStart==DateTime.MinValue && retVal.DateServiceEnd!=DateTime.MinValue) {//Start date not provided, but end date was.  Should not happen.  Just in case.
				retVal.DateServiceStart=retVal.DateServiceEnd;
			}
			//2100 DTM: Coverage Expiration Date.  Situational.  Repeat 1.  Guide page 175.  We do not use.
			while(segNum<_listSegments.Count && _listSegments[segNum].SegmentID=="DTM" && _listSegments[segNum].Get(1)=="036") {
				segNum++;
			}
			//2100 DTM: Claim Received Date.  Situational.  Repeat 1.  Guide page 177.
			retVal.DatePayerReceived=DateTime.MinValue;
			while(segNum<_listSegments.Count && _listSegments[segNum].SegmentID=="DTM" && _listSegments[segNum].Get(1)=="050") {
				retVal.DatePayerReceived=X12Parse.ToDate(_listSegments[segNum].Get(2));
				segNum++;
			}
			//2100 PER: Claim Contact Information.  Situational.  Repeat 2.  Guide page 179.  We do not use.
			while(segNum<_listSegments.Count && _listSegments[segNum].SegmentID=="PER") {
				segNum++;
			}
			//2100 AMT: Claim Supplemental Information.  Situational.  Repeat 13.  Guide page 182.
			retVal.ListSupplementalInfo=new List<Hx835_Info>();
			while(segNum<_listSegments.Count && _listSegments[segNum].SegmentID=="AMT") {
				retVal.ListSupplementalInfo.Add(ProcessAMT(segNum));
				segNum++;
			}
			//2100 QTY: Claim Supplemental Information Quantity.  Situational.  Repeat 14.  Guide page 184.  We do not use.
			while(segNum<_listSegments.Count && _listSegments[segNum].SegmentID=="QTY") {
				segNum++;
			}
			//===============================  PROCEDURES  =============================
			//2110 SVC Service Payment Information.  Situational.  Repeat 999.  Guide page 186.
			retVal.ListProcs=new List<Hx835_Proc>();
			while(segNum<_listSegments.Count && _listSegments[segNum].SegmentID=="SVC") {
				Hx835_Proc proc=ProcessSVC(segNum,retVal.DateServiceStart,retVal.DateServiceEnd);
				proc.ClaimPaid=retVal;
				retVal.ListProcs.Add(proc);
				segNum+=proc.SegmentCount;
			}
			retVal.PatientDeductAmt=0;
			retVal.PatientPortionAmt=0;
			retVal.WriteoffAmt=0;
			//"Amounts in CLP05 must have supporting adjustments reflected in CAS segments at the 2100 (CLP) or 2110 (SVC) loop level with a
			//Claim Adjustment Group (CAS01) code or PR (Patient Responsibility)"
			foreach(Hx835_Adj adj in retVal.ListClaimAdjustments) {//Sum claim level adjustments.
				if(adj.AdjCode=="CO") {//Contractual Obligations (writeoffs).  Guide page 198.
					//"Use thie code when a joint payer/payee agreement or a regulatory requirement has resulted in an adjustment."
					retVal.WriteoffAmt+=adj.AdjAmt;
				}
				else if(adj.AdjCode=="OA") {//Other Adjustments.  Guide page 198.
					//Going to display amount to the user, they must decide if they want to use it.
					//"Avoid using the Other Adjustment Group COde (OA) except for business situations described in sections 1.10.2.6, 1.10.2.7 and 1.10.2.13."
				}
				else if(adj.AdjCode=="PI") {//Payor Initiated Reductions  Guide page 198.
					//Going to display amount to the user, they must decide if they want to use it.
					//"Use this code when, in the opinion of the payer, the adjustment is not the responsibility of the patient, 
					//but there is no supporting contract between the provider and the payer (i.e., medical review or professional 
					//review organization adjustments)."
				}
				else if(adj.AdjCode=="PR") {//Patient Responsibility.  Guide page 198.
					if(adj.IsDeductible) {
						retVal.PatientDeductAmt+=adj.AdjAmt;
					}
					else if(!adj.IsDeductible) {
						retVal.PatientPortionAmt+=adj.AdjAmt;
					}
				}
			}
			foreach(Hx835_Proc proc in retVal.ListProcs) {//Add sum of procedure level adjustments to claim level adjustments.
				retVal.PatientDeductAmt+=proc.DeductibleAmt;
				retVal.PatientPortionAmt+=proc.PatientPortionAmt;
				retVal.WriteoffAmt+=proc.WriteoffAmt;
			}
			//Now modify the claim dates to encompass the procedure dates.  This step causes procedure dates to bubble up to the claim level when only service line dates are provided.
			for(int i=0;i<retVal.ListProcs.Count;i++) {
				Hx835_Proc proc=retVal.ListProcs[i];
				if(retVal.DateServiceStart.Year<1880) {
					retVal.DateServiceStart=proc.DateServiceStart;
				}
				else if(proc.DateServiceStart.Year>1880 && proc.DateServiceStart<retVal.DateServiceStart) {
					retVal.DateServiceStart=proc.DateServiceStart;
				}
				if(retVal.DateServiceEnd.Year<1880) {
					retVal.DateServiceEnd=proc.DateServiceEnd;
				}
				else if(proc.DateServiceEnd.Year>1880 && proc.DateServiceEnd>retVal.DateServiceEnd) {
					retVal.DateServiceEnd=proc.DateServiceEnd;
				}
			}
			retVal.SegmentCount=segNum-segNumCLP;
			retVal.AllowedAmt=retVal.InsPaid+retVal.PatientRespAmt;
			return retVal;
		}

		///<summary>The LQ segment contains remark codes that must be converted into human readable text for it to be usable.</summary>
		private string ProcessLQ(int segNum) {
			X12Segment segLQ=_listSegments[segNum];
			string code=segLQ.Get(2);
			if(segLQ.Get(1)=="HE") {//Claim Payment Remark Codes
				return GetDescriptFrom411(code);
			}
			else if(segLQ.Get(1)=="RX") {//National Council for Prescription Drug Programs Reject/Payment Codes.
				//We do not send prescriptions with X12, so we should never get responses with these codes.
				return "Rx Rejection Reason Code: "+code;//just in case, output a generic message so the user can look it up.
			}
			else {//Should not be possible, but here for future versions of X12 just in case.
				return "Code List Qualifier: "+segLQ.Get(1)+" Code: "+code;
			}
		}

		///<summary>The MIA segment is for Medicare inpatient adjudication information.</summary>
		private List<Hx835_Info> ProcessMIA(int segNum) {
			X12Segment segMIA=_listSegments[segNum];
			List<Hx835_Info> listAdjudicationInfo=new List<Hx835_Info>();
			for(int i=1;i<=24;i++) {
				if(segMIA.Get(i)=="") {
					continue;
				}
				Hx835_Info info=new Hx835_Info();
				info.FieldValueRaw=segMIA.Get(i);
				info.IsRemarkCode=false;
				if(i==1) {
					info.FieldName="Covered Days or Visits Count";
					info.FieldValue=segMIA.Get(1);
				}
				else if(i==2) {
					info.FieldName="PPS Operating Outlier Amount";
					info.FieldValue=PIn.Decimal(segMIA.Get(2)).ToString("f2");
				}
				else if(i==3) {
					info.FieldName="Lifetime Psychiatric Days Count";
					info.FieldValue=segMIA.Get(3);
				}
				else if(i==4) {
					info.FieldName="Claim Diagnosis Related Group Amount";
					info.FieldValue=PIn.Decimal(segMIA.Get(4)).ToString("f2");
				}
				else if(i==5) {
					info.FieldName="Claim Payment Remark";
					info.FieldValue=GetDescriptFrom411(segMIA.Get(5));
					info.IsRemarkCode=true;
				}
				else if(i==6) {
					info.FieldName="Disproportionate Share Amount";
					info.FieldValue=PIn.Decimal(segMIA.Get(6)).ToString("f2");
				}
				else if(i==7) {
					info.FieldName="Medicare Secondary Payer (MSP) Pass-Through Amount";
					info.FieldValue=PIn.Decimal(segMIA.Get(7)).ToString("f2");
				}
				else if(i==8) {
					info.FieldName="Prospective Payment System (PPS) Capital Amount";
					info.FieldValue=PIn.Decimal(segMIA.Get(8)).ToString("f2");
				}
				else if(i==9) {
					info.FieldName="Prospectice Payment System (PPS) Capital, Federal Specific Portion, Diagnosis Related Group (DRG) Amount";
					info.FieldValue=PIn.Decimal(segMIA.Get(9)).ToString("f2");
				}
				else if(i==10) {
					info.FieldName="Prospective Payment System (PPS) Capital, Hospital Specific Portion, Diagnosis Related Group (DRG) Amount";
					info.FieldValue=PIn.Decimal(segMIA.Get(10)).ToString("f2");
				}
				else if(i==11) {
					info.FieldName="Prospective Payment System (PPS) Capital, Disproportionate Share, Hospital Diagnosis Related Group (DRG) Amount";
					info.FieldValue=PIn.Decimal(segMIA.Get(11)).ToString("f2");
				}
				else if(i==12) {
					info.FieldName="Old Capital Amount";
					info.FieldValue=PIn.Decimal(segMIA.Get(12)).ToString("f2");
				}
				else if(i==13) {
					info.FieldName="Prospective Payment System (PPS) Capital Indirect Medical Education Claim Amount";
					info.FieldValue=PIn.Decimal(segMIA.Get(13)).ToString("f2");
				}
				else if(i==14) {
					info.FieldName="Hospital Specific Diagnosis Related Group (DRG) Amount";
					info.FieldValue=PIn.Decimal(segMIA.Get(14)).ToString("f2");
				}
				else if(i==15) {
					info.FieldName="Cost Report Day Count";
					info.FieldValue=segMIA.Get(15);
				}
				else if(i==16) {
					info.FieldName="Federal Specific Diagnosis Related Group (DRG) Amount";
					info.FieldValue=PIn.Decimal(segMIA.Get(16)).ToString("f2");
				}
				else if(i==17) {
					info.FieldName="Prospective Payment System (PPS) Capital Outlier Amount";
					info.FieldValue=PIn.Decimal(segMIA.Get(17)).ToString("f2");
				}
				else if(i==18) {
					info.FieldName="Indirect Teaching Amount";
					info.FieldValue=PIn.Decimal(segMIA.Get(18)).ToString("f2");
				}
				else if(i==19) {
					info.FieldName="Professional Component Amount Billed But Not Payable";
					info.FieldValue=PIn.Decimal(segMIA.Get(19)).ToString("f2");
				}
				else if(i==20) {
					info.FieldName="Claim Payment Remark";
					info.FieldValue=GetDescriptFrom411(segMIA.Get(20));
					info.IsRemarkCode=true;
				}
				else if(i==21) {
					info.FieldName="Claim Payment Remark";
					info.FieldValue=GetDescriptFrom411(segMIA.Get(21));
					info.IsRemarkCode=true;
				}
				else if(i==22) {
					info.FieldName="Claim Payment Remark";
					info.FieldValue=GetDescriptFrom411(segMIA.Get(22));
					info.IsRemarkCode=true;
				}
				else if(i==23) {
					info.FieldName="Claim Payment Remark";
					info.FieldValue=GetDescriptFrom411(segMIA.Get(23));
					info.IsRemarkCode=true;
				}
				else if(i==24) {
					info.FieldName="Prospective Payment System (PPS) Capital Exception Amount";
					info.FieldValue=PIn.Decimal(segMIA.Get(24)).ToString("f2");
				}
				listAdjudicationInfo.Add(info);
			}
			return listAdjudicationInfo;
		}

		///<summary>The MOA segment is for Medicare outpatient adjudication information.</summary>
		private List<Hx835_Info> ProcessMOA(int segNum) {
			X12Segment segMOA=_listSegments[segNum];
			List<Hx835_Info> listAdjudicationInfo=new List<Hx835_Info>();
			for(int i=1;i<=9;i++) {
				if(segMOA.Get(i)=="") {
					continue;
				}
				Hx835_Info info=new Hx835_Info();
				info.FieldValueRaw=segMOA.Get(i);
				info.IsRemarkCode=false;
				if(i==1) {
					info.FieldName="Reimbursement Rate";
					info.FieldValue=((PIn.Decimal(segMOA.Get(1))*100).ToString()+"%");
				}
				else if(i==2) {
					info.FieldName="Claim Health Care Financing Administration Common Procedural Coding System (HCPCS) Payable Amount";
					info.FieldValue=PIn.Decimal(segMOA.Get(2)).ToString("f2");
				}
				else if(i==3) {
					info.FieldName="Claim Payment Remark";
					info.FieldValue=GetDescriptFrom411(segMOA.Get(3));
					info.IsRemarkCode=true;
				}
				else if(i==4) {
					info.FieldName="Claim Payment Remark";
					info.FieldValue=GetDescriptFrom411(segMOA.Get(4));
					info.IsRemarkCode=true;
				}
				else if(i==5) {
					info.FieldName="Claim Payment Remark";
					info.FieldValue=GetDescriptFrom411(segMOA.Get(5));
					info.IsRemarkCode=true;
				}
				else if(i==6) {
					info.FieldName="Claim Payment Remark";
					info.FieldValue=GetDescriptFrom411(segMOA.Get(6));
					info.IsRemarkCode=true;
				}
				else if(i==7) {
					info.FieldName="Claim Payment Remark";
					info.FieldValue=GetDescriptFrom411(segMOA.Get(7));
					info.IsRemarkCode=true;
				}
				else if(i==8) {
					info.FieldName="End Stage Renal Disease (ESRD) Payment Amount";
					info.FieldValue=PIn.Decimal(segMOA.Get(8)).ToString("f2");
				}
				else if(i==9) {
					info.FieldName="Professional Component Amount Billed But Not Payable";
					info.FieldValue=PIn.Decimal(segMOA.Get(9)).ToString("f2");
				}
				listAdjudicationInfo.Add(info);
			}
			return listAdjudicationInfo;
		}

		///<summary>1000A N1*PR: Payer Identification.  Required.  Repeat 1.  Guide page 87.</summary>
		private void ProcessN1_PR(int segNum) {
			X12Segment segN1_PR=_listSegments[segNum];
			//N101 Entity Identifier Code.  Required.  Always PR=Payer.
			//N102 Payer Name.  Required.
			_payerName=segN1_PR.Get(2);
			//N103 Identification Code Qualifier.  Situational.
			//N104 Payer Identifier.  Situational.
			_payerId=segN1_PR.Get(4);
			//N105 Entity Relationship Code. Not used.
			//N106 Entity Identifier Code.  Not used.
		}

		///<summary>1000B N1*PE: Payee identification.  Required.  Repeat 1.  Guide page 102.  We include this information because it could be helpful for those customers who are using clinics.</summary>
		private void ProcessN1_PE(int segNum) {
			X12Segment segN1_PE=_listSegments[segNum];
			//N101 Entity Identifier Code.  Required.  Always PE.
			//N102 Payee Name.  Required.
			_payeeName=segN1_PE.Get(2);
			//N103 Identification Code Qualifier.  Required.
			_payeeIdType="";
			string qualifier=segN1_PE.Get(3);
			if(qualifier=="FI") {
				_payeeIdType="TIN";
			}
			else if(qualifier=="XV") {
				_payeeIdType="Medicaid ID";
			}
			else if(qualifier=="XX") {
				_payeeIdType="NPI";
			}
			//N104 Payee Identification Code.  Required.
			_payeeId=segN1_PE.Get(4);
			//N105 Not used.
			//N106 Not used.
		}

		///<summary>1000A N3: Payer Address.  Required.  Repeat 1.  Guide page 89.</summary>
		private void ProcessN3_PR(int segNum) {
			X12Segment segN3=_listSegments[segNum];
			//N301 Payer Address Line 1.  Required
			string address1=segN3.Get(1);
			//N301 Payer Address Line 2.  Situational.
			string address2=segN3.Get(2);
			if(address2=="") {
				_payerAddress=address1;
			}
			else {
				_payerAddress=address1+" "+address2;
			}
		}

		///<summary>1000A N4: Payer City, State, ZIP Code.  Required.  Repeat 1.  Guide page 90.</summary>
		private void ProcessN4_PR(int segNum) {
			X12Segment segN4=_listSegments[segNum];
			//N401 City Name.  Required
			_payerCity=segN4.Get(1);
			//N402 State or Province Code.  Situational.  Required for United States addresses.
			_payerState=segN4.Get(2);
			//N403 Postal Code.  Situational.  Required for United States addresses.
			_payerZip=segN4.Get(3);
			//N404 County Code.  Situational.  Required for addresses outside the United States.
			//N405 Not used.
			//N406 Not used.
			//N407 Country Subdivision Code.  Required when the address is not in the United States.
		}

		///<summary>Converts an NM1 segment for a person into a name object including the full name and identifier.
		///All fields are optional, thus this function returns what is available.</summary>
		private Hx835_Name ProcessNM1_Person(int segNum) {
			Hx835_Name name=new Hx835_Name();
			name.Fname=_listSegments[segNum].Get(4);
			name.Mname=_listSegments[segNum].Get(5);
			name.Lname=_listSegments[segNum].Get(3);
			name.Suffix=_listSegments[segNum].Get(7);
			name.SubscriberId=_listSegments[segNum].Get(9);
			name.SubscriberIdTypeDesc=_listSegments[segNum].Get(8);
			return name;
		}

		///<summary>Gets the contact information from segment PER*BL appended into a single string.
		///Phone/email in PER04 or the contact phone/email in PER06 or both.
		///If neither PER04 nor PER06 are present, then returns empty string.</summary>
		private string ProcessPER_BL(int segNum) {
			X12Segment segPER_BL=_listSegments[segNum];
			string contact_info=segPER_BL.Get(4);//Contact number 1.
			if(segPER_BL.Get(6)!="") {//Contact number 2.
				if(contact_info!="") {
					contact_info+=" or ";
				}
				contact_info+=segPER_BL.Get(6);
			}
			if(segPER_BL.Get(8)!="") {//Telephone extension for contact number 2.
				if(contact_info!="") {
					contact_info+=" x";
				}
				contact_info+=segPER_BL.Get(8);
			}
			return contact_info;
		}

		///<summary>PLB: Provider Admustment.  Situational.  Repeat >1.  Guide page 217.  Each PLB segment can return up to 6 adjustments.</summary>
		private List<Hx835_ProvAdj> ProcessPLB(int segNum) {
			List<Hx835_ProvAdj> retVal=new List<Hx835_ProvAdj>();
			X12Segment segPLB=_listSegments[segNum];
			string npi=segPLB.Get(1);//PLB01 is required.
			string dateFiscalPeriodStr=segPLB.Get(2);//PLB02 is required.
			DateTime dateFiscalPeriod=DateTime.MinValue;
			try {
				int dateEffectiveYear=int.Parse(dateFiscalPeriodStr.Substring(0,4));
				int dateEffectiveMonth=int.Parse(dateFiscalPeriodStr.Substring(4,2));
				int dateEffectiveDay=int.Parse(dateFiscalPeriodStr.Substring(6,2));
				dateFiscalPeriod=new DateTime(dateEffectiveYear,dateEffectiveMonth,dateEffectiveDay);
			}
			catch {
				//Oh well, not very important infomration anyway.
			}
			//After PLB02, the segments are in pairs, with a minimum of one pair, and a maximum of six pairs.  Starting with PLB03 and PLB04 (both are required), the remaining pairs are optional.
			//Each pair represents a single provider adjustment and reason for adjustment.  The provider is identified in PLB01 by NPI.
			//There can be more than one PLB segment, therefore it is possible to create more than six adjustments for a single provider by creating more than one PLB segment.
			//The loop below is intended to capture all adjustments within the current PLB segment.
			int segNumAdjCode=3;//PLB03 and PLB04 are required.  We start at segment 3 and increment by 2 with each iteration of the loop.
			while(segNumAdjCode<segPLB.Elements.Length) {
				Hx835_ProvAdj provAdj=new Hx835_ProvAdj();
				provAdj.Npi=npi;
				provAdj.DateFiscalPeriod=dateFiscalPeriod;
				provAdj.ReasonCode=segPLB.Get(segNumAdjCode,1);
				provAdj.ReasonCodeDescript=GetDescriptForProvAdjCode(provAdj.ReasonCode);
				//For each adjustment reason code, the reference identification is situational.
				provAdj.RefIdentification="";
				if(segPLB.Get(3).Length>provAdj.ReasonCode.Length) {
					provAdj.RefIdentification=segPLB.Get(3,2);
				}
				//For each adjustment reason code, an amount is required.
				provAdj.AdjAmt=PIn.Decimal(segPLB.Get(segNumAdjCode+1));
				retVal.Add(provAdj);
				segNumAdjCode+=2;
			}
			return retVal;
		}

		private Hx835_Proc ProcessSVC(int segNum,DateTime dateClaimServiceStart,DateTime dateClaimServiceEnd) {
			int segNumSVC=segNum;
			X12Segment segSVC=_listSegments[segNum];
			Hx835_Proc proc=new Hx835_Proc();
			proc.ProcCodeAdjudicated=segSVC.Get(1).Split(new string[] { Separators.Subelement },StringSplitOptions.None)[1];//SVC1-2
			proc.ProcFee=PIn.Decimal(segSVC.Get(2));//SVC2
			proc.InsPaid=PIn.Decimal(segSVC.Get(3));//SVC3
			if(segSVC.Get(6)=="") {
				proc.ProcCodeBilled=proc.ProcCodeAdjudicated;
			}
			else {
				proc.ProcCodeBilled=segSVC.Get(6).Split(new string[] { Separators.Subelement },StringSplitOptions.None)[1];//SVC6-2
			}
			segNum++;
			//2110 DTM: Service Date.  Situational.  Repeat 2.  Guide page 194.
			proc.DateServiceStart=dateClaimServiceStart;
			proc.DateServiceEnd=dateClaimServiceEnd;
			while(segNum<_listSegments.Count && _listSegments[segNum].SegmentID=="DTM") {
				string dateStr=_listSegments[segNum].Get(2);
				//Denti-cal and EDS have sent us invalid dates in the past.  Translate invalid dates 0/0/0 to 1/1/1.
				if(dateStr=="00000000") {
					dateStr="00010101";//Date expressed as CCYYMMDD where CC represents the first two digits of the calendar year.
				}
				if(_listSegments[segNum].Get(1)=="151") {//Service period end.
					proc.DateServiceEnd=X12Parse.ToDate(dateStr);
				}
				else {//_listSegments[segNum].Get(1)=="150" || _listSegments[segNum].Get(1)=="472"//Service period start
					proc.DateServiceStart=X12Parse.ToDate(dateStr);
				}
				segNum++;
			}
			if(proc.DateServiceStart!=DateTime.MinValue && proc.DateServiceEnd==DateTime.MinValue) {//Start date provided, but end date not provided.
				proc.DateServiceEnd=proc.DateServiceStart;
			}
			else if(proc.DateServiceStart==DateTime.MinValue && proc.DateServiceEnd!=DateTime.MinValue) {//Start date not provided, but end date provided.  Should not happen.  Just in case.
				proc.DateServiceStart=proc.DateServiceEnd;
			}
			proc.ListProcAdjustments=new List<Hx835_Adj>();
			//2110 CAS: Service Adjustment.  Situational.  Repeat 99.  Guide page 196.
			while(segNum<_listSegments.Count && _listSegments[segNum].SegmentID=="CAS") {
				proc.ListProcAdjustments.AddRange(ProcessCAS(segNum));
				segNum++;
			}
			proc.PatRespTotal=0;
			for(int i=0;i<proc.ListProcAdjustments.Count;i++) {
				Hx835_Adj adj=proc.ListProcAdjustments[i];
				if(adj.AdjCode=="CO"){//Contractual Obligations (writeoffs).  Guide page 198.
					//"Use thie code when a joint payer/payee agreement or a regulatory requirement has resulted in an adjustment."
					proc.WriteoffAmt+=adj.AdjAmt;
				}
				else if(adj.AdjCode=="OA") {//Other Adjustments.  Guide page 198.
					//Going to display amount to the user, they must decide if they want to use it.
					//"Avoid using the Other Adjustment Group COde (OA) except for business situations described in sections 1.10.2.6, 1.10.2.7 and 1.10.2.13."
				}
				else if(adj.AdjCode=="PI") {//Payor Initiated Reductions.  Guide page 198.
					//Going to display amount to the user, they must decide if they want to use it.
					//"Use this code when, in the opinion of the payer, the adjustment is not the responsibility of the patient, 
					//but there is no supporting contract between the provider and the payer (i.e., medical review or professional 
					//review organization adjustments)."
				}
				else if(adj.AdjCode=="PR") {//Patient Responsibility.  Guide page 198.
					proc.PatRespTotal+=adj.AdjAmt;
					if(adj.IsDeductible) {
						proc.DeductibleAmt+=adj.AdjAmt;
					}
					else {
						proc.PatientPortionAmt+=adj.AdjAmt;
					}
				}
			}
			//2110 REF: Service Identification.  Situational.  Repeat 8.  Guide page 204.  We do not use.
			//2110 REF: Line Item Control Number.  Situational.  Repeat 1.  Guide page 206.
			//2110 REF: Rendering Provider Information.  Situational.  Repeat 10.  Guide page 207.  We do not use.
			//2110 REF: HealthCare Policy Identification.  Situational.  Repeat 5.  Guide page 209.  We do not use.
			while(segNum<_listSegments.Count && _listSegments[segNum].SegmentID=="REF") {//4 segment types clumped together, but we only care about REF*6R.
				if(_listSegments[segNum].Get(1)=="6R") {
					string strRef02=_listSegments[segNum].Get(2).ToLower();//Our outgoing values are always lowercase, but some clearinghouses change to uppercase.
					if(strRef02.StartsWith("y")) {
						string[] arrayIdFields=strRef02.Split('/');
						if(arrayIdFields.Length==3) {
							proc.ProcNum=PIn.Long(arrayIdFields[0].Substring(1));//Ignores leading 'y'
							proc.PlanOrdinal=PIn.Long(arrayIdFields[1]);
							proc.PartialPlanNum=PIn.Long(arrayIdFields[2]);
						}
						else {
							//Can not trust format, all fields will default to 0.
						}
					}
					if(strRef02.StartsWith("x")) {
						string[] arrayIdFields=strRef02.Split('/');
						if(arrayIdFields.Length==3) {
							proc.ProcNum=PIn.Long(arrayIdFields[0].Substring(1));//Ignores leading 'x'
							proc.PlanOrdinal=PIn.Long(arrayIdFields[1]);
							proc.PlanNum=PIn.Long(arrayIdFields[2]);
						}
						else {
							//Can not trust format, all fields will default to 0.
						}
					}
					else if(strRef02.StartsWith("p")) {
						//If the control number is prefixed with a "p", then it is a ProcNum.
						//Otherwise, for older versions, it will be the Line Counter from LX01 in the 837, which is basically an index.  We will ignore these older index based values.
						proc.ProcNum=PIn.Long(strRef02.Substring(1));//The entire value excluding the leading "p".
					}	
				}
				segNum++;
			}
			proc.ListSupplementalInfo=new List<Hx835_Info>();
			//2110 AMT: Service Supplemental Amount.  Situational.  Repeat 9.  Guide page 211.
			while(segNum<_listSegments.Count && _listSegments[segNum].SegmentID=="AMT") {
				proc.ListSupplementalInfo.Add(ProcessAMT(segNum));
				segNum++;
			}
			//2110 QTY: Service Supplemental Quantity.  Situational.  Repeat 6.  Guide page 213.  We do not use.
			while(segNum<_listSegments.Count && _listSegments[segNum].SegmentID=="QTY") {
				segNum++;
			}
			//2110 LQ: Health Care Remark Codes.  Repeat 99.  Guide page 215.
			proc.ListRemarks=new List<Hx835_Remark>();
			while(segNum<_listSegments.Count && _listSegments[segNum].SegmentID=="LQ") {
				X12Segment segLQ=_listSegments[segNum];
				string code=segLQ.Get(2);
				string remark=ProcessLQ(segNum);
				proc.ListRemarks.Add(new Hx835_Remark(code,remark));
				segNum++;
			}
			proc.SegmentCount=segNum-segNumSVC;
			proc.AllowedAmt=proc.InsPaid+proc.PatRespTotal;
			return proc;
		}

		///<summary>TRN: Reassociation Trace Number.  Required.  Repeat 1.  Guide page 77.</summary>
		private void ProcessTRN(int segNum) {
			X12Segment segTRN=_listSegments[segNum];
			//TRN01 Trace Type Code.  Required.  Always set to 1.  Not useful.  We do not use.
			//TRN02 Check or EFT Trace Number.  Required.
			_transRefNum=segTRN.Get(2);
			//TRN03 Payer Identifier.  Required.  Must be a 1 followed by the payer's EIN (or TIN).  We do not use.
			//TRN04 Originating Company Supplemental Code.  Situational.  We do not use.
		}

		#endregion Segment Processing
		#region Helpers

		public string GetHumanReadable() {
			StringBuilder retVal=new StringBuilder();
			retVal.AppendLine("Claim Status Reponse From "+PayerName);
			retVal.AppendLine("Effective Pay Date: "+DateEffective.ToShortDateString());
			retVal.AppendLine("Amount: "+InsPaid);
			retVal.AppendLine("Individual Claim Status List: ");
			retVal.AppendLine("Status	ClaimFee	InsPaid	PatientResp		PayerControlNum");
			List<Hx835_Claim> listClaimsPaid=_listClaimsPaid;
			for(int i=0;i<listClaimsPaid.Count;i++) {
				Hx835_Claim claimPaid=listClaimsPaid[i];
				retVal.Append(claimPaid.StatusCodeDescript+"\t");
				retVal.Append(claimPaid.ClaimFee.ToString("f2")+"\t");
				retVal.Append(claimPaid.InsPaid.ToString("f2")+"\t");
				retVal.Append(claimPaid.PatientRespAmt.ToString("f2")+"\t");
				retVal.AppendLine(claimPaid.PayerControlNumber);
			}
			return retVal.ToString();
		}

		#endregion Helpers
		#region Code To Description

		private string GetDescriptForAmountQualifierCode(string code) {
			if(code=="AU") {
				return "Coverage Amount";
			}
			else if(code=="B6") {
				return "Allowed - Actual";
			}
			else if(code=="D8") {
				return "Discount Amount";
			}
			else if(code=="DY") {
				return "Per Day Limit";
			}
			else if(code=="F5") {
				return "Patient Amount Paid";
			}
			else if(code=="I") {
				return "Interest";
			}
			else if(code=="KH") {
				return "Late Filing Reduction";
			}
			else if(code=="NL") {
				return "Negative Ledger Balance";
			}
			else if(code=="T") {
				return "Tax";
			}
			else if(code=="T2") {
				return "Total Claim Before Taxes";
			}
			else if(code=="ZK") {
				return "Federal Medicare or Medicaid Payment Mandate - Category 1";
			}
			else if(code=="ZL") {
				return "Federal Medicare or Medicaid Payment Mandate - Category 2";
			}
			else if(code=="ZM") {
				return "Federal Medicare or Medicaid Payment Mandate - Category 3";
			}
			else if(code=="ZN") {
				return "Federal Medicare or Medicaid Payment Mandate - Category 4";
			}
			else if(code=="ZO") {
				return "Federal Medicare or Medicaid Payment Mandate - Category 5";
			}
			return "Qualifier Code: "+code;
		}

		private string GetDescriptForClaimStatusCode(string code) {
			string claimStatusCodeDescript="";
			if(code=="1") {
				claimStatusCodeDescript="Processed as Primary";
			}
			else if(code=="2") {
				claimStatusCodeDescript="Processed as Secondary";
			}
			else if(code=="3") {
				claimStatusCodeDescript="Processed as Tertiary";
			}
			else if(code=="4") {
				claimStatusCodeDescript="Denied";
			}
			else if(code=="19") {
				claimStatusCodeDescript="Processed as Primary, Forwarded to Additional Payer(s)";
			}
			else if(code=="20") {
				claimStatusCodeDescript="Processed as Secondary, Forwarded to Additional Payer(s)";
			}
			else if(code=="21") {
				claimStatusCodeDescript="Processed as Tertiary, Forwarded to Additional Payer(s)";
			}
			else if(code=="22") {
				claimStatusCodeDescript="Reversal of Previous Payment";
			}
			else if(code=="23") {
				claimStatusCodeDescript="Not Our Claim, Forwarded to Additional Payer(s)";
			}
			else if(code=="25") {
				claimStatusCodeDescript="Predetermination Pricing Only - No Payment";
			}
			return claimStatusCodeDescript;
		}

		public string GetDescriptForPaymentMethodCode(string code) {
			if(code=="ACH") {
				return "Automated Clearing House (ACH)";
			}
			else if(code=="BOP") {
				return "Financial Institution Option";
			}
			else if(code=="CHK") {
				return "Check";
			}
			else if(code=="FWT") {
				return "Federal Reserve Funds/Wire Transfer - Nonrepetitive";
			}
			else if(code=="NON") {
				return "Non-payment Data";
			}
			return "Pay Method Code: "+code;
		}

		///<summary>Used for the reason codes in the PLB segment.</summary>
		private string GetDescriptForProvAdjCode(string code) {
			if(code=="50") {
				return "Late Charge";
			}
			if(code=="51") {
				return "Interest Penalty Charge";
			}
			if(code=="72") {
				return "Authorized Return";
			}
			if(code=="90") {
				return "Early Payment Allowance";
			}
			if(code=="AH") {
				return "Origination Fee";
			}
			if(code=="AM") {
				return "Applied to Borrower's Account";
			}
			if(code=="AP") {
				return "Acceleration of Benefits";
			}
			if(code=="B2") {
				return "Rebate";
			}
			if(code=="B3") {
				return "Recovery Allowance";
			}
			if(code=="BD") {
				return "Bad Debt Adjustment";
			}
			if(code=="BN") {
				return "Bonus";
			}
			if(code=="C5") {
				return "Temporary Allowance";
			}
			if(code=="CR") {
				return "Capitation Interest";
			}
			if(code=="CS") {
				return "Adjustment";
			}
			if(code=="CT") {
				return "Capitation Payment";
			}
			if(code=="CV") {
				return "Capital Passthru";
			}
			if(code=="CW") {
				return "Certified Registered Nurse Anesthetist Passthru";
			}
			if(code=="DM") {
				return "Direct Medical Education Passthru";
			}
			if(code=="E3") {
				return "Withholding";
			}
			if(code=="FB") {
				return "Forwarding Balance";
			}
			if(code=="FC") {
				return "Fund Allocation";
			}
			if(code=="GO") {
				return "Graduate Medical Education Passthru";
			}
			if(code=="HM") {
				return "Hemophilia Clotting Factor Supplement";
			}
			if(code=="IP") {
				return "Incentive Premium Payment";
			}
			if(code=="IR") {
				return "Internal Revenue Service Withholding";
			}
			if(code=="IS") {
				return "Interim Settlement";
			}
			if(code=="J1") {
				return "Nonreimbursable";
			}
			if(code=="L3") {
				return "Penalty";
			}
			if(code=="L6") {
				return "Interest Owed";
			}
			if(code=="LE") {
				return "Levy";
			}
			if(code=="LS") {
				return "Lump Sum";
			}
			if(code=="OA") {
				return "Organ Acquisition Passthru";
			}
			if(code=="OB") {
				return "Offset for Affiliated Providers";
			}
			if(code=="PI") {
				return "Periodic Interim Payment";
			}
			if(code=="PL") {
				return "Payment Final";
			}
			if(code=="RA") {
				return "Retro-activity Adjustment";
			}
			if(code=="RE") {
				return "Return on Equity";
			}
			if(code=="SL") {
				return "Student Loan Repayment";
			}
			if(code=="TL") {
				return "Third Party Liability";
			}
			if(code=="WO") {
				return "Overpayment Recovery";
			}
			if(code=="WU") {
				return "Unspecified Recovery";
			}
			return "Reason "+code;
		}

		///<summary>Gets the description for the transaction handling code in Table 1 (Header) BPR01. Required.</summary>
		public string GetDescriptForTransactionHandlingCode(string code) {
			if(code=="C") {
				return "Payment Accompanies Remittance Advice";
			}
			else if(code=="D") {
				return "Make Payment Only";
			}
			else if(code=="H") {
				return "Notification Only";
			}
			else if(code=="I") {
				return "Remittance Information Only";
			}
			else if(code=="P") {
				return "Prenotification of Future Transfers";
			}
			else if(code=="U") {
				return "Split Payment and Remittance";
			}
			else if(code=="X") {
				return "Handling Party's Option to Split Payment and Remittance";
			}
			return "Transaction Code: "+code;
		}

		///<summary>Code Source 139. Claim Adjustment Reason Codes.  http://www.wpc-edi.com/reference/codelists/healthcare/claim-adjustment-reason-codes/ .
		///Used for claim and procedure reason codes.</summary>
		private string GetDescriptFrom139(string code) {
			if(code=="1") {
				return "Deductible Amount";
			}
			if(code=="2") {
				return "Coinsurance Amount";
			}
			if(code=="3") {
				return "Co-payment Amount";
			}
			if(code=="4") {
				return "The procedure code is inconsistent with the modifier used or a required modifier is missing.";
			}
			if(code=="5") {
				return "The procedure code/bill type is inconsistent with the place of service.";
			}
			if(code=="6") {
				return "	The procedure/revenue code is inconsistent with the patient's age.";
			}
			if(code=="7") {
				return "The procedure/revenue code is inconsistent with the patient's gender.";
			}
			if(code=="8") {
				return "The procedure code is inconsistent with the provider type/specialty (taxonomy).";
			}
			if(code=="9") {
				return "The diagnosis is inconsistent with the patient's age.";
			}
			if(code=="10") {
				return "The diagnosis is inconsistent with the patient's gender.";
			}
			if(code=="11") {
				return "The diagnosis is inconsistent with the procedure.";
			}
			if(code=="12") {
				return "The diagnosis is inconsistent with the provider type.";
			}
			if(code=="13") {
				return "The date of death precedes the date of service.";
			}
			if(code=="14") {
				return "The date of birth follows the date of service.";
			}
			if(code=="15") {
				return "The authorization number is missing, invalid, or does not apply to the billed services or provider.";
			}
			if(code=="16") {
				return "Claim/service lacks information which is needed for adjudication.";
			}
			if(code=="18") {
				return "Exact duplicate claim/service";
			}
			if(code=="19") {
				return "This is a work-related injury/illness and thus the liability of the Worker's Compensation Carrier.";
			}
			if(code=="20") {
				return "This injury/illness is covered by the liability carrier.";
			}
			if(code=="21") {
				return "This injury/illness is the liability of the no-fault carrier.";
			}
			if(code=="22") {
				return "This care may be covered by another payer per coordination of benefits.";
			}
			if(code=="23") {
				return "The impact of prior payer(s) adjudication including payments and/or adjustments.";
			}
			if(code=="24") {
				return "Charges are covered under a capitation agreement/managed care plan.";
			}
			if(code=="26") {
				return "Expenses incurred prior to coverage.";
			}
			if(code=="27") {
				return "Expenses incurred after coverage terminated.";
			}
			if(code=="29") {
				return "The time limit for filing has expired.";
			}	
			if(code=="Patient cannot be identified as our insured.") {
				return "";
			}	
			if(code=="32") {
				return "Our records indicate that this dependent is not an eligible dependent as defined.";
			}	
			if(code=="33") {
				return "Insured has no dependent coverage.";
			}
			if(code=="34") {
				return "Insured has no coverage for newborns.";
			}
			if(code=="35") {
				return "Lifetime benefit maximum has been reached.";
			}
			if(code=="39") {
				return "Services denied at the time authorization/pre-certification was requested.";
			}
			if(code=="40") {
				return "Charges do not meet qualifications for emergent/urgent care. Note: Refer to the 835 Healthcare Policy Identification Segment (loop 2110 Service Payment Information REF), if present.";
			}
			if(code=="44") {
				return "Prompt-pay discount.";
			}
			if(code=="45") {
				return "Charge exceeds fee schedule/maximum allowable or contracted/legislated fee arrangement.";
			}
			if(code=="49") {
				return "These are non-covered services because this is a routine exam or screening procedure done in conjunction with a routine exam.";
			}
			if(code=="50") {
				return "These are non-covered services because this is not deemed a 'medical necessity' by the payer.";
			}
			if(code=="51") {
				return "These are non-covered services because this is a pre-existing condition.";
			}
			if(code=="53") {
				return "Services by an immediate relative or a member of the same household are not covered.";
			}
			if(code=="54") {
				return "Multiple physicians/assistants are not covered in this case.";
			}
			if(code=="55") {
				return "Procedure/treatment is deemed experimental/investigational by the payer.";
			}
			if(code=="56") {
				return "Procedure/treatment has not been deemed 'proven to be effective' by the payer.";
			}
			if(code=="58") {
				return "Treatment was deemed by the payer to have been rendered in an inappropriate or invalid place of service.";
			}
			if(code=="59") {
				return "Processed based on multiple or concurrent procedure rules.";
			}
			if(code=="60") {
				return "Charges for outpatient services are not covered when performed within a period of time prior to or after inpatient services.";
			}
			if(code=="61") {
				return "Penalty for failure to obtain second surgical opinion.";
			}
			if(code=="66") {
				return "Blood Deductible.";
			}
			if(code=="69") {
				return "Day outlier amount.";
			}
			if(code=="70") {
				return "Cost outlier - Adjustment to compensate for additional costs.";
			}
			if(code=="74") {
				return "Indirect Medical Education Adjustment.";
			}
			if(code=="75") {
				return "Direct Medical Education Adjustment.";
			}
			if(code=="76") {
				return "Disproportionate Share Adjustment.";
			}
			if(code=="78") {
				return "Non-Covered days/Room charge adjustment.";
			}
			if(code=="85") {//Use only Group Code PR
				return "Patient Interest Adjustment";
			}
			if(code=="89") {
				return "Professional fees removed from charges.";
			}
			if(code=="90") {
				return "Ingredient cost adjustment.";
			}
			if(code=="91") {
				return "Dispensing fee adjustment.";
			}
			if(code=="94") {
				return "Processed in Excess of charges.";
			}
			if(code=="95") {
				return "Plan procedures not followed.";
			}
			if(code=="96") {
				return "Non-covered charge(s).";
			}
			if(code=="97") {
				return "The benefit for this service is included in the payment/allowance for another service/procedure that has already been adjudicated.";
			}
			if(code=="100") {
				return "Payment made to patient/insured/responsible party/employer.";
			}
			if(code=="101") {
				return "Predetermination: anticipated payment upon completion of services or claim adjudication.";
			}
			if(code=="102") {
				return "Major Medical Adjustment.";
			}
			if(code=="103") {
				return "Provider promotional discount";
			}
			if(code=="104") {
				return "Managed care withholding.";
			}
			if(code=="105") {
				return "Tax withholding.";
			}
			if(code=="106") {
				return "Patient payment option/election not in effect.";
			}
			if(code=="107") {
				return "The related or qualifying claim/service was not identified on this claim.";
			}
			if(code=="108") {
				return "Rent/purchase guidelines were not met.";
			}
			if(code=="109") {
				return "Claim/service not covered by this payer/contractor.";
			}
			if(code=="110") {
				return "Billing date predates service date.";
			}
			if(code=="111") {
				return "Not covered unless the provider accepts assignment.";
			}
			if(code=="112") {
				return "Service not furnished directly to the patient and/or not documented.";
			}
			if(code=="114") {
				return "Procedure/product not approved by the Food and Drug Administration.";
			}
			if(code=="115") {
				return "Procedure postponed, canceled, or delayed.";
			}
			if(code=="116") {
				return "The advance indemnification notice signed by the patient did not comply with requirements.";
			}
			if(code=="117") {
				return "Transportation is only covered to the closest facility that can provide the necessary care.";
			}
			if(code=="118") {
				return "ESRD network support adjustment.";
			}
			if(code=="119") {
				return "Benefit maximum for this time period or occurrence has been reached.";
			}
			if(code=="121") {
				return "Indemnification adjustment - compensation for outstanding member responsibility.";
			}
			if(code=="122") {
				return "Psychiatric reduction.";
			}
			if(code=="125") {
				return "Submission/billing error(s).";
			}
			if(code=="128") {
				return "Newborn's services are covered in the mother's Allowance.";
			}
			if(code=="129") {
				return "Prior processing information appears incorrect.";
			}
			if(code=="130") {
				return "Claim submission fee.";
			}
			if(code=="131") {
				return "Claim specific negotiated discount.";
			}
			if(code=="132") {
				return "Prearranged demonstration project adjustment.";
			}
			if(code=="133") { //Use only with Group Code OA
				return "The disposition of the claim/service is pending further review.";
			}
			if(code=="134") {
				return "Technical fees removed from charges.";
			}
			if(code=="135") {
				return "Interim bills cannot be processed.";
			}
			if(code=="136") { //Use Group Code OA
				return "Failure to follow prior payer's coverage rules.";
			}
			if(code=="137") {
				return "Regulatory Surcharges, Assessments, Allowances or Health Related Taxes.";
			}
			if(code=="138") {
				return "Appeal procedures not followed or time limits not met.";
			}
			if(code=="139") {
				return "Contracted funding agreement - Subscriber is employed by the provider of services.";
			}
			if(code=="140") {
				return "Patient/Insured health identification number and name do not match.";
			}
			if(code=="142") {
				return "Monthly Medicaid patient liability amount.";
			}
			if(code=="143") {
				return "Portion of payment deferred.";
			}
			if(code=="144") {
				return "Incentive adjustment, e.g. preferred product/service.";
			}
			if(code=="146") {
				return "Diagnosis was invalid for the date(s) of service reported.";
			}
			if(code=="147") {
				return "Provider contracted/negotiated rate expired or not on file.";
			}
			if(code=="148") {
				return "Information from another provider was not provided or was insufficient/incomplete.";
			}
			if(code=="149") {
				return "Lifetime benefit maximum has been reached for this service/benefit category.";
			}
			if(code=="150") {
				return "Payer deems the information submitted does not support this level of service.";
			}
			if(code=="151") {
				return "Payment adjusted because the payer deems the information submitted does not support this many/frequency of services.";
			}
			if(code=="152") {
				return "Payer deems the information submitted does not support this length of service.";
			}
			if(code=="153") {
				return "Payer deems the information submitted does not support this dosage.";
			}
			if(code=="154") {
				return "Payer deems the information submitted does not support this day's supply.";
			}
			if(code=="155") {
				return "Patient refused the service/procedure.";
			}
			if(code=="157") {
				return "Service/procedure was provided as a result of an act of war.";
			}
			if(code=="158") {
				return "Service/procedure was provided outside of the United States.";
			}
			if(code=="159") {
				return "Service/procedure was provided as a result of terrorism.";
			}
			if(code=="160") {
				return "Injury/illness was the result of an activity that is a benefit exclusion.";
			}
			if(code=="161") {
				return "Provider performance bonus";
			}
			if(code=="162") {
				return "State-mandated Requirement for Property and Casualty, see Claim Payment Remarks Code for specific explanation.";
			}
			if(code=="163") {
				return "Attachment referenced on the claim was not received.";
			}
			if(code=="164") {
				return "Attachment referenced on the claim was not received in a timely fashion.";
			}
			if(code=="165") {
				return "Referral absent or exceeded.";
			}
			if(code=="166") {
				return "These services were submitted after this payers responsibility for processing claims under this plan ended.";
			}
			if(code=="167") {
				return "This (these) diagnosis(es) is (are) not covered.";
			}
			if(code=="168") {
				return "Service(s) have been considered under the patient's medical plan. Benefits are not available under this dental plan.";
			}
			if(code=="169") {
				return "Alternate benefit has been provided.";
			}
			if(code=="170") {
				return "Payment is denied when performed/billed by this type of provider.";
			}
			if(code=="171") {
				return "Payment is denied when performed/billed by this type of provider in this type of facility.";
			}
			if(code=="172") {
				return "Payment is adjusted when performed/billed by a provider of this specialty.";
			}
			if(code=="173") {
				return "Service was not prescribed by a physician.";
			}
			if(code=="174") {
				return "Service was not prescribed prior to delivery.";
			}
			if(code=="175") {
				return "Prescription is incomplete.";
			}
			if(code=="176") {
				return "Prescription is not current.";
			}
			if(code=="177") {
				return "Patient has not met the required eligibility requirements.";
			}
			if(code=="178") {
				return "Patient has not met the required spend down requirements.";
			}
			if(code=="179") {
				return "Patient has not met the required waiting requirements.";
			}
			if(code=="180") {
				return "Patient has not met the required residency requirements.";
			}
			if(code=="181") {
				return "Procedure code was invalid on the date of service.";
			}
			if(code=="182") {
				return "Procedure modifier was invalid on the date of service.";
			}
			if(code=="183") {
				return "The referring provider is not eligible to refer the service billed.";
			}
			if(code=="184") {
				return "The prescribing/ordering provider is not eligible to prescribe/order the service billed.";
			}
			if(code=="185") {
				return "The rendering provider is not eligible to perform the service billed.";
			}
			if(code=="186") {
				return "Level of care change adjustment.";
			}
			if(code=="187") {
				return "Consumer Spending Account payments.";
			}
			if(code=="188") {
				return "This product/procedure is only covered when used according to FDA recommendations.";
			}
			if(code=="189") {
				return "'Not otherwise classified' or 'unlisted' procedure code (CPT/HCPCS) was billed when there is a specific procedure code for this procedure/service.";
			}
			if(code=="190") {
				return "Payment is included in the allowance for a Skilled Nursing Facility (SNF) qualified stay.";
			}
			if(code=="191") {
				return "Not a work related injury/illness and thus not the liability of the workers' compensation carrier";
			}
			if(code=="192") {
				return "Non standard adjustment code from paper remittance.";
			}
			if(code=="193") {
				return "Original payment decision is being maintained. Upon review, it was determined that this claim was processed properly.";
			}
			if(code=="194") {
				return "Anesthesia performed by the operating physician, the assistant surgeon or the attending physician.";
			}
			if(code=="195") {
				return "Refund issued to an erroneous priority payer for this claim/service.";
			}
			if(code=="197") {
				return "Precertification/authorization/notification absent.";
			}
			if(code=="198") {
				return "Precertification/authorization exceeded.";
			}
			if(code=="199") {
				return "Revenue code and Procedure code do not match.";
			}
			if(code=="200") {
				return "Expenses incurred during lapse in coverage";
			}
			if(code=="201") { //Use group code PR
				return "Workers' Compensation case settled. Patient is responsible for amount of this claim/service through WC 'Medicare set aside arrangement' or other agreement.";
			}
			if(code=="202") {
				return "Non-covered personal comfort or convenience services.";
			}
			if(code=="203") {
				return "Discontinued or reduced service.";
			}
			if(code=="204") {
				return "This service/equipment/drug is not covered under the patient’s current benefit plan.";
			}
			if(code=="205") {
				return "Pharmacy discount card processing fee";
			}
			if(code=="206") {
				return "National Provider Identifier - missing.";
			}
			if(code=="207") {
				return "National Provider identifier - Invalid format";
			}
			if(code=="208") {
				return "National Provider Identifier - Not matched.";
			}
			if(code=="209") { //Use Group code OA
				return "Per regulatory or other agreement. The provider cannot collect this amount from the patient. However, this amount may be billed to subsequent payer. Refund to patient if collected.";
			}
			if(code=="210") {
				return "Payment adjusted because pre-certification/authorization not received in a timely fashion";
			}
			if(code=="211") {
				return "National Drug Codes (NDC) not eligible for rebate, are not covered.";
			}
			if(code=="212") {
				return "Administrative surcharges are not covered";
			}
			if(code=="213") {
				return "Non-compliance with the physician self referral prohibition legislation or payer policy.";
			}
			if(code=="214") {
				return "Workers' Compensation claim adjudicated as non-compensable. This Payer not liable for claim or service/treatment.";
			}
			if(code=="215") {
				return "Based on subrogation of a third party settlement";
			}
			if(code=="216") {
				return "Based on the findings of a review organization";
			}
			if(code=="217") {
				return "Based on payer reasonable and customary fees. No maximum allowable defined by legislated fee arrangement.";
			}
			if(code=="218") {
				return "Based on entitlement to benefits.";
			}
			if(code=="219") {
				return "Based on extent of injury.";
			}
			if(code=="220") {
				return "The applicable fee schedule/fee database does not contain the billed code. Please resubmit a bill with the appropriate fee schedule/fee database code(s) that best describe the service(s) provided and supporting documentation if required.";
			}
			if(code=="221") {
				return "Workers' Compensation claim is under investigation.";
			}
			if(code=="222") {
				return "Exceeds the contracted maximum number of hours/days/units by this provider for this period. This is not patient specific.";
			}
			if(code=="223") {
				return "Adjustment code for mandated federal, state or local law/regulation that is not already covered by another code and is mandated before a new code can be created.";
			}
			if(code=="224") {
				return "Patient identification compromised by identity theft. Identity verification required for processing this and future claims.";
			}
			if(code=="225") {
				return "Penalty or Interest Payment by Payer";
			}
			if(code=="226") {
				return "Information requested from the Billing/Rendering Provider was not provided or was insufficient/incomplete.";
			}
			if(code=="227") {
				return "Information requested from the patient/insured/responsible party was not provided or was insufficient/incomplete.";
			}
			if(code=="228") {
				return "Denied for failure of this provider, another provider or the subscriber to supply requested information to a previous payer for their adjudication.";
			}
			if(code=="229") { //Use only with Group Code PR
				return "Partial charge amount not considered by Medicare due to the initial claim Type of Bill being 12X.";
			}
			if(code=="230") {
				return "No available or correlating CPT/HCPCS code to describe this service.";
			}
			if(code=="231") {
				return "Mutually exclusive procedures cannot be done in the same day/setting.";
			}
			if(code=="232") {
				return "Institutional Transfer Amount.";
			}
			if(code=="233") {
				return "Services/charges related to the treatment of a hospital-acquired condition or preventable medical error.";
			}
			if(code=="234") {
				return "This procedure is not paid separately.";
			}
			if(code=="235") {
				return "Sales Tax";
			}
			if(code=="236") {
				return "This procedure or procedure/modifier combination is not compatible with another procedure or procedure/modifier combination provided on the same day according to the National Correct Coding Initiative.";
			}
			if(code=="237") {
				return "Legislated/Regulatory Penalty.";
			}
			if(code=="238") { //Use Group Code PR
				return "Claim spans eligible and ineligible periods of coverage, this is the reduction for the ineligible period.";
			}
			if(code=="239") {
				return "Claim spans eligible and ineligible periods of coverage. Rebill separate claims.";
			}
			if(code=="240") {
				return "The diagnosis is inconsistent with the patient's birth weight.";
			}
			if(code=="241") {
				return "Low Income Subsidy (LIS) Co-payment Amount";
			}
			if(code=="242") {
				return "Services not provided by network/primary care providers.";
			}
			if(code=="243") {
				return "Services not authorized by network/primary care providers.";
			}
			if(code=="244") {
				return "Payment reduced to zero due to litigation. Additional information will be sent following the conclusion of litigation.";
			}
			if(code=="245") {
				return "Provider performance program withhold.";
			}
			if(code=="246") {
				return "This non-payable code is for required reporting only.";
			}
			if(code=="247") {
				return "Deductible for Professional service rendered in an Institutional setting and billed on an Institutional claim.";
			}
			if(code=="248") {
				return "Coinsurance for Professional service rendered in an Institutional setting and billed on an Institutional claim.";
			}
			if(code=="249") { //Use only with Group Code CO
				return "This claim has been identified as a readmission.";
			}
			if(code=="250") {
				return "The attachment content received is inconsistent with the expected content.";
			}
			if(code=="251") {
				return "The attachment content received did not contain the content required to process this claim or service.";
			}
			if(code=="252") {
				return "An attachment is required to adjudicate this claim/service.";
			}
			if(code=="A0") {
				return "Patient refund amount.";
			}
			if(code=="A1") {
				return "Claim/Service denied.";
			}
			if(code=="A5") {
				return "Medicare Claim PPS Capital Cost Outlier Amount.";
			}
			if(code=="A6") {
				return "Prior hospitalization or 30 day transfer requirement not met.";
			}
			if(code=="A7") {
				return "Presumptive Payment Adjustment";
			}
			if(code=="A8") {
				return "Ungroupable DRG.";
			}
			if(code=="B1") {
				return "Non-covered visits.";
			}
			if(code=="B4") {
				return "Late filing penalty.";
			}
			if(code=="B5") {
				return "Coverage/program guidelines were not met or were exceeded.";
			}
			if(code=="B7") {
				return "This provider was not certified/eligible to be paid for this procedure/service on this date of service.";
			}
			if(code=="B8") {
				return "Alternative services were available, and should have been utilized.";
			}
			if(code=="B9") {
				return "Patient is enrolled in a Hospice.";
			}
			if(code=="B10") {
				return "Allowed amount has been reduced because a component of the basic procedure/test was paid. The beneficiary is not liable for more than the charge limit for the basic procedure/test.";
			}
			if(code=="B11") {
				return "The claim/service has been transferred to the proper payer/processor for processing. Claim/service not covered by this payer/processor.";
			}
			if(code=="B12") {
				return "Services not documented in patients' medical records.";
			}
			if(code=="B13") {
				return "Previously paid. Payment for this claim/service may have been provided in a previous payment.";
			}
			if(code=="B14") {
				return "Only one visit or consultation per physician per day is covered.";
			}
			if(code=="B15") {
				return "This service/procedure requires that a qualifying service/procedure be received and covered. The qualifying other service/procedure has not been received/adjudicated.";
			}
			if(code=="B16") {
				return "'New Patient' qualifications were not met.";
			}
			if(code=="B20") {
				return "Procedure/service was partially or fully furnished by another provider.";
			}
			if(code=="B22") {
				return "This payment is adjusted based on the diagnosis.";
			}
			if(code=="B23") {
				return "Procedure billed is not authorized per your Clinical Laboratory Improvement Amendment (CLIA) proficiency test.";
			}
			if(code=="W1") {
				return "Workers' compensation jurisdictional fee schedule adjustment.";
			}
			if(code=="W2") {
				return "Payment reduced or denied based on workers' compensation jurisdictional regulations or payment policies, use only if no other code is applicable.";
			}
			if(code=="W3") {
				return "The Benefit for this Service is included in the payment/allowance for another service/procedure that has been performed on the same day.";
			}
			if(code=="W4") {
				return "Workers' Compensation Medical Treatment Guideline Adjustment.";
			}
			if(code=="Y1") {
				return "Payment denied based on Medical Payments Coverage (MPC) or Personal Injury Protection (PIP) Benefits jurisdictional regulations or payment policies, use only if no other code is applicable.";
			}
			if(code=="Y2") {
				return "Payment adjusted based on Medical Payments Coverage (MPC) or Personal Injury Protection (PIP) Benefits jurisdictional regulations or payment policies, use only if no other code is applicable.";
			}
			if(code=="Y3") {
				return "Medical Payments Coverage (MPC) or Personal Injury Protection (PIP) Benefits jurisdictional fee schedule adjustment.";
			}
			return "Reason code "+code;//Worst case, if we do not recognize the code, display it verbatim so the user can look it up.
		}

		///<summary>Code Source 411.  Remittance Advice Remark Codes.  https://www.wpc-edi.com/reference/codelists/healthcare/remittance-advice-remark-codes/ </summary>
		private string GetDescriptFrom411(string code) {
			if(code=="M1") { return "X-ray not taken within the past 12 months or near enough to the start of treatment."; }
			else if(code=="M2") { return "Not paid separately when the patient is an inpatient."; }
			else if(code=="M3") { return "Equipment is the same or similar to equipment already being used."; }
			else if(code=="M4") { return "Alert: This is the last monthly installment payment for this durable medical equipment."; }
			else if(code=="M5") { return "Monthly rental payments can continue until the earlier of the 15th month from the first rental month, or the month when the equipment is no longer needed."; }
			else if(code=="M6") { return "Alert: You must furnish and service this item for any period of medical need for the remainder of the reasonable useful lifetime of the equipment."; }
			else if(code=="M7") { return "No rental payments after the item is purchased, or after the total of issued rental payments equals the purchase price."; }
			else if(code=="M8") { return "We do not accept blood gas tests results when the test was conducted by a medical supplier or taken while the patient is on oxygen."; }
			else if(code=="M9") { return "Alert: This is the tenth rental month. You must offer the patient the choice of changing the rental to a purchase agreement."; }
			else if(code=="M10") { return "Equipment purchases are limited to the first or the tenth month of medical necessity."; }
			else if(code=="M11") { return "DME, orthotics and prosthetics must be billed to the DME carrier who services the patient's zip code."; }
			else if(code=="M12") { return "Diagnostic tests performed by a physician must indicate whether purchased services are included on the claim."; }
			else if(code=="M13") { return "Only one initial visit is covered per specialty per medical group."; }
			else if(code=="M14") { return "No separate payment for an injection administered during an office visit, and no payment for a full office visit if the patient only received an injection."; }
			else if(code=="M15") { return "Separately billed services/tests have been bundled as they are considered components of the same procedure. Separate payment is not allowed."; }
			else if(code=="M16") { return "Alert: Please see our web site, mailings, or bulletins for more details concerning this policy/procedure/decision."; }
			else if(code=="M17") { return "Alert: Payment approved as you did not know, and could not reasonably have been expected to know, that this would not normally have been covered for this patient. In the future, you will be liable for charges for the same service(s) under the same or similar conditions."; }
			else if(code=="M18") { return "Certain services may be approved for home use. Neither a hospital nor a Skilled Nursing Facility (SNF) is considered to be a patient's home."; }
			else if(code=="M19") { return "Missing oxygen certification/re-certification."; }
			else if(code=="M20") { return "Missing/incomplete/invalid HCPCS."; }
			else if(code=="M21") { return "Missing/incomplete/invalid place of residence for this service/item provided in a home."; }
			else if(code=="M22") { return "Missing/incomplete/invalid number of miles traveled."; }
			else if(code=="M23") { return "Missing invoice."; }
			else if(code=="M24") { return "Missing/incomplete/invalid number of doses per vial."; }
			else if(code=="M25") { return "The information furnished does not substantiate the need for this level of service. If you believe the service should have been fully covered as billed, or if you did not know and could not reasonably have been expected to know that we would not pay for this level of service, or if you notified the patient in writing in advance that we would not pay for this level of service and he/she agreed in writing to pay, ask us to review your claim within 120 days of the date of this notice. If you do not request an appeal, we will, upon application from the patient, reimburse him/her for the amount you have collected from him/her in excess of any deductible and coinsurance amounts. We will recover the reimbursement from you as an overpayment."; }
			else if(code=="M26") { return "The information furnished does not substantiate the need for this level of service. If you have collected any amount from the patient for this level of service /any amount that exceeds the limiting charge for the less extensive service, the law requires you to refund that amount to the patient within 30 days of receiving this notice. The requirements for refund are in 1824(I) of the Social Security Act and 42CFR411.408. The section specifies that physicians who knowingly and willfully fail to make appropriate refunds may be subject to civil monetary penalties and/or exclusion from the program. If you have any questions about this notice, please contact this office."; }
			else if(code=="M27") { return "Alert: The patient has been relieved of liability of payment of these items and services under the limitation of liability provision of the law. The provider is ultimately liable for the patient's waived charges, including any charges for coinsurance, since the items or services were not reasonable and necessary or constituted custodial care, and you knew or could reasonably have been expected to know, that they were not covered. You may appeal this determination. You may ask for an appeal regarding both the coverage determination and the issue of whether you exercised due care. The appeal request must be filed within 120 days of the date you receive this notice. You must make the request through this office."; }
			else if(code=="M28") { return "This does not qualify for payment under Part B when Part A coverage is exhausted or not otherwise available."; }
			else if(code=="M29") { return "Missing operative note/report."; }
			else if(code=="M30") { return "Missing pathology report."; }
			else if(code=="M31") { return "Missing radiology report."; }
			else if(code=="M32") { return "Alert: This is a conditional payment made pending a decision on this service by the patient's primary payer. This payment may be subject to refund upon your receipt of any additional payment for this service from another payer. You must contact this office immediately upon receipt of an additional payment for this service."; }
			else if(code=="M33") { return "Missing/incomplete/invalid UPIN for the ordering/referring/performing provider."; }
			else if(code=="M34") { return "Claim lacks the CLIA certification number."; }
			else if(code=="M35") { return "Missing/incomplete/invalid pre-operative photos or visual field results."; }
			else if(code=="M36") { return "This is the 11th rental month. We cannot pay for this until you indicate that the patient has been given the option of changing the rental to a purchase."; }
			else if(code=="M37") { return "Not covered when the patient is under age 35."; }
			else if(code=="M38") { return "The patient is liable for the charges for this service as you informed the patient in writing before the service was furnished that we would not pay for it, and the patient agreed to pay."; }
			else if(code=="M39") { return "The patient is not liable for payment for this service as the advance notice of non-coverage you provided the patient did not comply with program requirements."; }
			else if(code=="M40") { return "Claim must be assigned and must be filed by the practitioner's employer."; }
			else if(code=="M41") { return "We do not pay for this as the patient has no legal obligation to pay for this."; }
			else if(code=="M42") { return "The medical necessity form must be personally signed by the attending physician."; }
			else if(code=="M43") { return "Payment for this service previously issued to you or another provider by another carrier/intermediary."; }
			else if(code=="M44") { return "Missing/incomplete/invalid condition code."; }
			else if(code=="M45") { return "Missing/incomplete/invalid occurrence code(s)."; }
			else if(code=="M46") { return "Missing/incomplete/invalid occurrence span code(s)."; }
			else if(code=="M47") { return "Missing/incomplete/invalid internal or document control number."; }
			else if(code=="M48") { return "Payment for services furnished to hospital inpatients (other than professional services of physicians) can only be made to the hospital. You must request payment from the hospital rather than the patient for this service."; }
			else if(code=="M49") { return "Missing/incomplete/invalid value code(s) or amount(s)."; }
			else if(code=="M50") { return "Missing/incomplete/invalid revenue code(s)."; }
			else if(code=="M51") { return "Missing/incomplete/invalid procedure code(s)."; }
			else if(code=="M52") { return "Missing/incomplete/invalid “from” date(s) of service."; }
			else if(code=="M53") { return "Missing/incomplete/invalid days or units of service."; }
			else if(code=="M54") { return "Missing/incomplete/invalid total charges."; }
			else if(code=="M55") { return "We do not pay for self-administered anti-emetic drugs that are not administered with a covered oral anti-cancer drug."; }
			else if(code=="M56") { return "Missing/incomplete/invalid payer identifier."; }
			else if(code=="M57") { return "Missing/incomplete/invalid provider identifier."; }
			else if(code=="M58") { return "Missing/incomplete/invalid claim information. Resubmit claim after corrections."; }
			else if(code=="M59") { return "Missing/incomplete/invalid “to” date(s) of service."; }
			else if(code=="M60") { return "Missing Certificate of Medical Necessity."; }
			else if(code=="M61") { return "We cannot pay for this as the approval period for the FDA clinical trial has expired."; }
			else if(code=="M62") { return "Missing/incomplete/invalid treatment authorization code."; }
			else if(code=="M63") { return "We do not pay for more than one of these on the same day."; }
			else if(code=="M64") { return "Missing/incomplete/invalid other diagnosis."; }
			else if(code=="M65") { return "One interpreting physician charge can be submitted per claim when a purchased diagnostic test is indicated. Please submit a separate claim for each interpreting physician."; }
			else if(code=="M66") { return "Our records indicate that you billed diagnostic tests subject to price limitations and the procedure code submitted includes a professional component. Only the technical component is subject to price limitations. Please submit the technical and professional components of this service as separate line items."; }
			else if(code=="M67") { return "Missing/incomplete/invalid other procedure code(s)."; }
			else if(code=="M68") { return "Missing/incomplete/invalid attending, ordering, rendering, supervising or referring physician identification."; }
			else if(code=="M69") { return "Paid at the regular rate as you did not submit documentation to justify the modified procedure code."; }
			else if(code=="M70") { return "Alert: The NDC code submitted for this service was translated to a HCPCS code for processing, but please continue to submit the NDC on future claims for this item."; }
			else if(code=="M71") { return "Total payment reduced due to overlap of tests billed."; }
			else if(code=="M72") { return "Did not enter full 8-digit date (MM/DD/CCYY)."; }
			else if(code=="M73") { return "The HPSA/Physician Scarcity bonus can only be paid on the professional component of this service. Rebill as separate professional and technical components."; }
			else if(code=="M74") { return "This service does not qualify for a HPSA/Physician Scarcity bonus payment."; }
			else if(code=="M75") { return "Multiple automated multichannel tests performed on the same day combined for payment."; }
			else if(code=="M76") { return "Missing/incomplete/invalid diagnosis or condition."; }
			else if(code=="M77") { return "Missing/incomplete/invalid place of service."; }
			else if(code=="M78") { return "Missing/incomplete/invalid HCPCS modifier."; }
			else if(code=="M79") { return "Missing/incomplete/invalid charge."; }
			else if(code=="M80") { return "Not covered when performed during the same session/date as a previously processed service for the patient."; }
			else if(code=="M81") { return "You are required to code to the highest level of specificity."; }
			else if(code=="M82") { return "Service is not covered when patient is under age 50."; }
			else if(code=="M83") { return "Service is not covered unless the patient is classified as at high risk."; }
			else if(code=="M84") { return "Medical code sets used must be the codes in effect at the time of service"; }
			else if(code=="M85") { return "Subjected to review of physician evaluation and management services."; }
			else if(code=="M86") { return "Service denied because payment already made for same/similar procedure within set time frame."; }
			else if(code=="M87") { return "Claim/service(s) subjected to CFO-CAP prepayment review."; }
			else if(code=="M88") { return "We cannot pay for laboratory tests unless billed by the laboratory that did the work."; }
			else if(code=="M89") { return "Not covered more than once under age 40."; }
			else if(code=="M90") { return "Not covered more than once in a 12 month period."; }
			else if(code=="M91") { return "Lab procedures with different CLIA certification numbers must be billed on separate claims."; }
			else if(code=="M92") { return "Services subjected to review under the Home Health Medical Review Initiative."; }
			else if(code=="M93") { return "Information supplied supports a break in therapy. A new capped rental period began with delivery of this equipment."; }
			else if(code=="M94") { return "Information supplied does not support a break in therapy. A new capped rental period will not begin."; }
			else if(code=="M95") { return "Services subjected to Home Health Initiative medical review/cost report audit."; }
			else if(code=="M96") { return "The technical component of a service furnished to an inpatient may only be billed by that inpatient facility. You must contact the inpatient facility for technical component reimbursement. If not already billed, you should bill us for the professional component only."; }
			else if(code=="M97") { return "Not paid to practitioner when provided to patient in this place of service. Payment included in the reimbursement issued the facility."; }
			else if(code=="M98") { return "Begin to report the Universal Product Number on claims for items of this type. We will soon begin to deny payment for items of this type if billed without the correct UPN."; }
			else if(code=="M99") { return "Missing/incomplete/invalid Universal Product Number/Serial Number."; }
			else if(code=="M100") { return "We do not pay for an oral anti-emetic drug that is not administered for use immediately before, at, or within 48 hours of administration of a covered chemotherapy drug."; }
			else if(code=="M101") { return "Begin to report a G1-G5 modifier with this HCPCS. We will soon begin to deny payment for this service if billed without a G1-G5 modifier."; }
			else if(code=="M102") { return "Service not performed on equipment approved by the FDA for this purpose."; }
			else if(code=="M103") { return "Information supplied supports a break in therapy. However, the medical information we have for this patient does not support the need for this item as billed. We have approved payment for this item at a reduced level, and a new capped rental period will begin with the delivery of this equipment."; }
			else if(code=="M104") { return "Information supplied supports a break in therapy. A new capped rental period will begin with delivery of the equipment. This is the maximum approved under the fee schedule for this item or service."; }
			else if(code=="M105") { return "Information supplied does not support a break in therapy. The medical information we have for this patient does not support the need for this item as billed. We have approved payment for this item at a reduced level, and a new capped rental period will not begin."; }
			else if(code=="M106") { return "Information supplied does not support a break in therapy. A new capped rental period will not begin. This is the maximum approved under the fee schedule for this item or service."; }
			else if(code=="M107") { return "Payment reduced as 90-day rolling average hematocrit for ESRD patient exceeded 36.5%."; }
			else if(code=="M108") { return "Missing/incomplete/invalid provider identifier for the provider who interpreted the diagnostic test."; }
			else if(code=="M109") { return "We have provided you with a bundled payment for a teleconsultation. You must send 25 percent of the teleconsultation payment to the referring practitioner."; }
			else if(code=="M110") { return "Missing/incomplete/invalid provider identifier for the provider from whom you purchased interpretation services."; }
			else if(code=="M111") { return "We do not pay for chiropractic manipulative treatment when the patient refuses to have an x-ray taken."; }
			else if(code=="M112") { return "Reimbursement for this item is based on the single payment amount required under the DMEPOS Competitive Bidding Program for the area where the patient resides."; }
			else if(code=="M113") { return "Our records indicate that this patient began using this item/service prior to the current contract period for the DMEPOS Competitive Bidding Program."; }
			else if(code=="M114") { return "This service was processed in accordance with rules and guidelines under the DMEPOS Competitive Bidding Program or a Demonstration Project. For more information regarding these projects, contact your local contractor."; }
			else if(code=="M115") { return "This item is denied when provided to this patient by a non-contract or non-demonstration supplier."; }
			else if(code=="M116") { return "Processed under a demonstration project or program. Project or program is ending and additional services may not be paid under this project or program."; }
			else if(code=="M117") { return "Not covered unless submitted via electronic claim."; }
			else if(code=="M118") { return "Letter to follow containing further information."; }
			else if(code=="M119") { return "Missing/incomplete/invalid/ deactivated/withdrawn National Drug Code (NDC)."; }
			else if(code=="M120") { return "Missing/incomplete/invalid provider identifier for the substituting physician who furnished the service(s) under a reciprocal billing or locum tenens arrangement."; }
			else if(code=="M121") { return "We pay for this service only when performed with a covered cryosurgical ablation."; }
			else if(code=="M122") { return "Missing/incomplete/invalid level of subluxation."; }
			else if(code=="M123") { return "Missing/incomplete/invalid name, strength, or dosage of the drug furnished."; }
			else if(code=="M124") { return "Missing indication of whether the patient owns the equipment that requires the part or supply."; }
			else if(code=="M125") { return "Missing/incomplete/invalid information on the period of time for which the service/supply/equipment will be needed."; }
			else if(code=="M126") { return "Missing/incomplete/invalid individual lab codes included in the test."; }
			else if(code=="M127") { return "Missing patient medical record for this service."; }
			else if(code=="M128") { return "Missing/incomplete/invalid date of the patient’s last physician visit."; }
			else if(code=="M129") { return "Missing/incomplete/invalid indicator of x-ray availability for review."; }
			else if(code=="M130") { return "Missing invoice or statement certifying the actual cost of the lens, less discounts, and/or the type of intraocular lens used."; }
			else if(code=="M131") { return "Missing physician financial relationship form."; }
			else if(code=="M132") { return "Missing pacemaker registration form."; }
			else if(code=="M133") { return "Claim did not identify who performed the purchased diagnostic test or the amount you were charged for the test."; }
			else if(code=="M134") { return "Performed by a facility/supplier in which the provider has a financial interest."; }
			else if(code=="M135") { return "Missing/incomplete/invalid plan of treatment."; }
			else if(code=="M136") { return "Missing/incomplete/invalid indication that the service was supervised or evaluated by a physician."; }
			else if(code=="M137") { return "Part B coinsurance under a demonstration project or pilot program."; }
			else if(code=="M138") { return "Patient identified as a demonstration participant but the patient was not enrolled in the demonstration at the time services were rendered. Coverage is limited to demonstration participants."; }
			else if(code=="M139") { return "Denied services exceed the coverage limit for the demonstration."; }
			else if(code=="M140") { return "Service not covered until after the patient’s 50th birthday, i.e., no coverage prior to the day after the 50th birthday"; }
			else if(code=="M141") { return "Missing physician certified plan of care."; }
			else if(code=="M142") { return "Missing American Diabetes Association Certificate of Recognition."; }
			else if(code=="M143") { return "The provider must update license information with the payer."; }
			else if(code=="M144") { return "Pre-/post-operative care payment is included in the allowance for the surgery/procedure."; }
			else if(code=="MA01") { return "Alert: If you do not agree with what we approved for these services, you may appeal our decision. To make sure that we are fair to you, we require another individual that did not process your initial claim to conduct the appeal. However, in order to be eligible for an appeal, you must write to us within 120 days of the date you received this notice, unless you have a good reason for being late."; }
			else if(code=="MA02") { return "Alert: If you do not agree with this determination, you have the right to appeal. You must file a written request for an appeal within 180 days of the date you receive this notice."; }
			else if(code=="MA03") { return "If you do not agree with the approved amounts and $100 or more is in dispute (less deductible and coinsurance), you may ask for a hearing within six months of the date of this notice. To meet the $100, you may combine amounts on other claims that have been denied, including reopened appeals if you received a revised decision. You must appeal each claim on time."; }
			else if(code=="MA04") { return "Secondary payment cannot be considered without the identity of or payment information from the primary payer. The information was either not reported or was illegible."; }
			else if(code=="MA05") { return "Incorrect admission date patient status or type of bill entry on claim."; }
			else if(code=="MA06") { return "Missing/incomplete/invalid beginning and/or ending date(s)."; }
			else if(code=="MA07") { return "Alert: The claim information has also been forwarded to Medicaid for review."; }
			else if(code=="MA08") { return "Alert: Claim information was not forwarded because the supplemental coverage is not with a Medigap plan, or you do not participate in Medicare."; }
			else if(code=="MA09") { return "Claim submitted as unassigned but processed as assigned. You agreed to accept assignment for all claims."; }
			else if(code=="MA10") { return "Alert: The patient's payment was in excess of the amount owed. You must refund the overpayment to the patient."; }
			else if(code=="MA11") { return "Payment is being issued on a conditional basis. If no-fault insurance, liability insurance, Workers' Compensation, Department of Veterans Affairs, or a group health plan for employees and dependents also covers this claim, a refund may be due us. Please contact us if the patient is covered by any of these sources."; }
			else if(code=="MA12") { return "You have not established that you have the right under the law to bill for services furnished by the person(s) that furnished this (these) service(s)."; }
			else if(code=="MA13") { return "Alert: You may be subject to penalties if you bill the patient for amounts not reported with the PR (patient portion) group code."; }
			else if(code=="MA14") { return "Alert: The patient is a member of an employer-sponsored prepaid health plan. Services from outside that health plan are not covered. However, as you were not previously notified of this, we are paying this time. In the future, we will not pay you for non-plan services."; }
			else if(code=="MA15") { return "Alert: Your claim has been separated to expedite handling. You will receive a separate notice for the other services reported."; }
			else if(code=="MA16") { return "The patient is covered by the Black Lung Program. Send this claim to the Department of Labor, Federal Black Lung Program, P.O. Box 828, Lanham-Seabrook MD 20703."; }
			else if(code=="MA17") { return "We are the primary payer and have paid at the primary rate. You must contact the patient's other insurer to refund any excess it may have paid due to its erroneous primary payment."; }
			else if(code=="MA18") { return "Alert: The claim information is also being forwarded to the patient's supplemental insurer. Send any questions regarding supplemental benefits to them."; }
			else if(code=="MA19") { return "Alert: Information was not sent to the Medigap insurer due to incorrect/invalid information you submitted concerning that insurer. Please verify your information and submit your secondary claim directly to that insurer."; }
			else if(code=="MA20") { return "Skilled Nursing Facility (SNF) stay not covered when care is primarily related to the use of an urethral catheter for convenience or the control of incontinence."; }
			else if(code=="MA21") { return "SSA records indicate mismatch with name and sex."; }
			else if(code=="MA22") { return "Payment of less than $1.00 suppressed."; }
			else if(code=="MA23") { return "Demand bill approved as result of medical review."; }
			else if(code=="MA24") { return "Christian Science Sanitarium/ Skilled Nursing Facility (SNF) bill in the same benefit period."; }
			else if(code=="MA25") { return "A patient may not elect to change a hospice provider more than once in a benefit period."; }
			else if(code=="MA26") { return "Alert: Our records indicate that you were previously informed of this rule."; }
			else if(code=="MA27") { return "Missing/incomplete/invalid entitlement number or name shown on the claim."; }
			else if(code=="MA28") { return "Alert: Receipt of this notice by a physician or supplier who did not accept assignment is for information only and does not make the physician or supplier a party to the determination. No additional rights to appeal this decision, above those rights already provided for by regulation/instruction, are conferred by receipt of this notice."; }
			else if(code=="MA29") { return "Missing/incomplete/invalid provider name, city, state, or zip code."; }
			else if(code=="MA30") { return "Missing/incomplete/invalid type of bill."; }
			else if(code=="MA31") { return "Missing/incomplete/invalid beginning and ending dates of the period billed."; }
			else if(code=="MA32") { return "Missing/incomplete/invalid number of covered days during the billing period."; }
			else if(code=="MA33") { return "Missing/incomplete/invalid noncovered days during the billing period."; }
			else if(code=="MA34") { return "Missing/incomplete/invalid number of coinsurance days during the billing period."; }
			else if(code=="MA35") { return "Missing/incomplete/invalid number of lifetime reserve days."; }
			else if(code=="MA36") { return "Missing/incomplete/invalid patient name."; }
			else if(code=="MA37") { return "Missing/incomplete/invalid patient's address."; }
			else if(code=="MA38") { return "Missing/incomplete/invalid birth date."; }
			else if(code=="MA39") { return "Missing/incomplete/invalid gender."; }
			else if(code=="MA40") { return "Missing/incomplete/invalid admission date."; }
			else if(code=="MA41") { return "Missing/incomplete/invalid admission type."; }
			else if(code=="MA42") { return "Missing/incomplete/invalid admission source."; }
			else if(code=="MA43") { return "Missing/incomplete/invalid patient status."; }
			else if(code=="MA44") { return "Alert: No appeal rights. Adjudicative decision based on law."; }
			else if(code=="MA45") { return "Alert: As previously advised, a portion or all of your payment is being held in a special account."; }
			else if(code=="MA46") { return "The new information was considered but additional payment will not be issued."; }
			else if(code=="MA47") { return "Our records show you have opted out of Medicare, agreeing with the patient not to bill Medicare for services/tests/supplies furnished. As result, we cannot pay this claim. The patient is responsible for payment."; }
			else if(code=="MA48") { return "Missing/incomplete/invalid name or address of responsible party or primary payer."; }
			else if(code=="MA49") { return "Missing/incomplete/invalid six-digit provider identifier for home health agency or hospice for physician(s) performing care plan oversight services."; }
			else if(code=="MA50") { return "Missing/incomplete/invalid Investigational Device Exemption number for FDA-approved clinical trial services."; }
			else if(code=="MA51") { return "Missing/incomplete/invalid CLIA certification number for laboratory services billed by physician office laboratory."; }
			else if(code=="MA52") { return "Missing/incomplete/invalid date."; }
			else if(code=="MA53") { return "Missing/incomplete/invalid Competitive Bidding Demonstration Project identification."; }
			else if(code=="MA54") { return "Physician certification or election consent for hospice care not received timely."; }
			else if(code=="MA55") { return "Not covered as patient received medical health care services, automatically revoking his/her election to receive religious non-medical health care services."; }
			else if(code=="MA56") { return "Our records show you have opted out of Medicare, agreeing with the patient not to bill Medicare for services/tests/supplies furnished. As result, we cannot pay this claim. The patient is responsible for payment, but under Federal law, you cannot charge the patient more than the limiting charge amount."; }
			else if(code=="MA57") { return "Patient submitted written request to revoke his/her election for religious non-medical health care services."; }
			else if(code=="MA58") { return "Missing/incomplete/invalid release of information indicator."; }
			else if(code=="MA59") { return "Alert: The patient overpaid you for these services. You must issue the patient a refund within 30 days for the difference between his/her payment and the total amount shown as patient portion on this notice."; }
			else if(code=="MA60") { return "Missing/incomplete/invalid patient relationship to insured."; }
			else if(code=="MA61") { return "Missing/incomplete/invalid social security number or health insurance claim number."; }
			else if(code=="MA62") { return "Alert: This is a telephone review decision."; }
			else if(code=="MA63") { return "Missing/incomplete/invalid principal diagnosis."; }
			else if(code=="MA64") { return "Our records indicate that we should be the third payer for this claim. We cannot process this claim until we have received payment information from the primary and secondary payers."; }
			else if(code=="MA65") { return "Missing/incomplete/invalid admitting diagnosis."; }
			else if(code=="MA66") { return "Missing/incomplete/invalid principal procedure code."; }
			else if(code=="MA67") { return "Correction to a prior claim."; }
			else if(code=="MA68") { return "Alert: We did not crossover this claim because the secondary insurance information on the claim was incomplete. Please supply complete information or use the PLANID of the insurer to assure correct and timely routing of the claim."; }
			else if(code=="MA69") { return "Missing/incomplete/invalid remarks."; }
			else if(code=="MA70") { return "Missing/incomplete/invalid provider representative signature."; }
			else if(code=="MA71") { return "Missing/incomplete/invalid provider representative signature date."; }
			else if(code=="MA72") { return "Alert: The patient overpaid you for these assigned services. You must issue the patient a refund within 30 days for the difference between his/her payment to you and the total of the amount shown as patient portion and as paid to the patient on this notice."; }
			else if(code=="MA73") { return "Informational remittance associated with a Medicare demonstration. No payment issued under fee-for-service Medicare as patient has elected managed care."; }
			else if(code=="MA74") { return "This payment replaces an earlier payment for this claim that was either lost, damaged or returned."; }
			else if(code=="MA75") { return "Missing/incomplete/invalid patient or authorized representative signature."; }
			else if(code=="MA76") { return "Missing/incomplete/invalid provider identifier for home health agency or hospice when physician is performing care plan oversight services."; }
			else if(code=="MA77") { return "Alert: The patient overpaid you. You must issue the patient a refund within 30 days for the difference between the patient’s payment less the total of our and other payer payments and the amount shown as patient portion on this notice."; }
			else if(code=="MA78") { return "The patient overpaid you. You must issue the patient a refund within 30 days for the difference between our allowed amount total and the amount paid by the patient."; }
			else if(code=="MA79") { return "Billed in excess of interim rate."; }
			else if(code=="MA80") { return "Informational notice. No payment issued for this claim with this notice. Payment issued to the hospital by its intermediary for all services for this encounter under a demonstration project."; }
			else if(code=="MA81") { return "Missing/incomplete/invalid provider/supplier signature."; }
			else if(code=="MA82") { return "Missing/incomplete/invalid provider/supplier billing number/identifier or billing name, address, city, state, zip code, or phone number."; }
			else if(code=="MA83") { return "Did not indicate whether we are the primary or secondary payer."; }
			else if(code=="MA84") { return "Patient identified as participating in the National Emphysema Treatment Trial but our records indicate that this patient is either not a participant, or has not yet been approved for this phase of the study. Contact Johns Hopkins University, the study coordinator, to resolve if there was a discrepancy."; }
			else if(code=="MA85") { return "Our records indicate that a primary payer exists (other than ourselves); however, you did not complete or enter accurately the insurance plan/group/program name or identification number. Enter the PlanID when effective."; }
			else if(code=="MA86") { return "Missing/incomplete/invalid group or policy number of the insured for the primary coverage."; }
			else if(code=="MA87") { return "Missing/incomplete/invalid insured's name for the primary payer."; }
			else if(code=="MA88") { return "Missing/incomplete/invalid insured's address and/or telephone number for the primary payer."; }
			else if(code=="MA89") { return "Missing/incomplete/invalid patient's relationship to the insured for the primary payer."; }
			else if(code=="MA90") { return "Missing/incomplete/invalid employment status code for the primary insured."; }
			else if(code=="MA91") { return "This determination is the result of the appeal you filed."; }
			else if(code=="MA92") { return "Missing plan information for other insurance."; }
			else if(code=="MA93") { return "Non-PIP (Periodic Interim Payment) claim."; }
			else if(code=="MA94") { return "Did not enter the statement “Attending physician not hospice employee” on the claim form to certify that the rendering physician is not an employee of the hospice."; }
			else if(code=="MA95") { return "A not otherwise classified or unlisted procedure code(s) was billed but a narrative description of the procedure was not entered on the claim. Refer to item 19 on the HCFA-1500."; }
			else if(code=="MA96") { return "Claim rejected. Coded as a Medicare Managed Care Demonstration but patient is not enrolled in a Medicare managed care plan."; }
			else if(code=="MA97") { return "Missing/incomplete/invalid Medicare Managed Care Demonstration contract number or clinical trial registry number."; }
			else if(code=="MA98") { return "Claim Rejected. Does not contain the correct Medicare Managed Care Demonstration contract number for this beneficiary."; }
			else if(code=="MA99") { return "Missing/incomplete/invalid Medigap information."; }
			else if(code=="MA100") { return "Missing/incomplete/invalid date of current illness or symptoms"; }
			else if(code=="MA101") { return "A Skilled Nursing Facility (SNF) is responsible for payment of outside providers who furnish these services/supplies to residents."; }
			else if(code=="MA102") { return "Missing/incomplete/invalid name or provider identifier for the rendering/referring/ ordering/ supervising provider."; }
			else if(code=="MA103") { return "Hemophilia Add On."; }
			else if(code=="MA104") { return "Missing/incomplete/invalid date the patient was last seen or the provider identifier of the attending physician."; }
			else if(code=="MA105") { return "Missing/incomplete/invalid provider number for this place of service."; }
			else if(code=="MA106") { return "PIP (Periodic Interim Payment) claim."; }
			else if(code=="MA107") { return "Paper claim contains more than three separate data items in field 19."; }
			else if(code=="MA108") { return "Paper claim contains more than one data item in field 23."; }
			else if(code=="MA109") { return "Claim processed in accordance with ambulatory surgical guidelines."; }
			else if(code=="MA110") { return "Missing/incomplete/invalid information on whether the diagnostic test(s) were performed by an outside entity or if no purchased tests are included on the claim."; }
			else if(code=="MA111") { return "Missing/incomplete/invalid purchase price of the test(s) and/or the performing laboratory's name and address."; }
			else if(code=="MA112") { return "Missing/incomplete/invalid group practice information."; }
			else if(code=="MA113") { return "Incomplete/invalid taxpayer identification number (TIN) submitted by you per the Internal Revenue Service. Your claims cannot be processed without your correct TIN, and you may not bill the patient pending correction of your TIN. There are no appeal rights for unprocessable claims, but you may resubmit this claim after you have notified this office of your correct TIN."; }
			else if(code=="MA114") { return "Missing/incomplete/invalid information on where the services were furnished."; }
			else if(code=="MA115") { return "Missing/incomplete/invalid physical location (name and address, or PIN) where the service(s) were rendered in a Health Professional Shortage Area (HPSA)."; }
			else if(code=="MA116") { return "Did not complete the statement 'Homebound' on the claim to validate whether laboratory services were performed at home or in an institution."; }
			else if(code=="MA117") { return "This claim has been assessed a $1.00 user fee."; }
			else if(code=="MA118") { return "Coinsurance and/or deductible amounts apply to a claim for services or supplies furnished to a Medicare-eligible veteran through a facility of the Department of Veterans Affairs. No Medicare payment issued."; }
			else if(code=="MA119") { return "Provider level adjustment for late claim filing applies to this claim."; }
			else if(code=="MA120") { return "Missing/incomplete/invalid CLIA certification number."; }
			else if(code=="MA121") { return "Missing/incomplete/invalid x-ray date."; }
			else if(code=="MA122") { return "Missing/incomplete/invalid initial treatment date."; }
			else if(code=="MA123") { return "Your center was not selected to participate in this study, therefore, we cannot pay for these services."; }
			else if(code=="MA124") { return "Processed for IME only."; }
			else if(code=="MA125") { return "Per legislation governing this program, payment constitutes payment in full."; }
			else if(code=="MA126") { return "Pancreas transplant not covered unless kidney transplant performed."; }
			else if(code=="MA127") { return "Reserved for future use."; }
			else if(code=="MA128") { return "Missing/incomplete/invalid FDA approval number."; }
			else if(code=="MA129") { return "This provider was not certified for this procedure on this date of service."; }
			else if(code=="MA130") { return "Your claim contains incomplete and/or invalid information, and no appeal rights are afforded because the claim is unprocessable. Please submit a new claim with the complete/correct information."; }
			else if(code=="MA131") { return "Physician already paid for services in conjunction with this demonstration claim. You must have the physician withdraw that claim and refund the payment before we can process your claim."; }
			else if(code=="MA132") { return "Adjustment to the pre-demonstration rate."; }
			else if(code=="MA133") { return "Claim overlaps inpatient stay. Rebill only those services rendered outside the inpatient stay."; }
			else if(code=="MA134") { return "Missing/incomplete/invalid provider number of the facility where the patient resides."; }
			else if(code=="N1") { return "Alert: You may appeal this decision in writing within the required time limits following receipt of this notice by following the instructions included in your contract, plan benefit documents or jurisdiction statutes."; }
			else if(code=="N2") { return "This allowance has been made in accordance with the most appropriate course of treatment provision of the plan."; }
			else if(code=="N3") { return "Missing consent form."; }
			else if(code=="N4") { return "Missing/Incomplete/Invalid prior Insurance Carrier(s) EOB."; }
			else if(code=="N5") { return "EOB received from previous payer. Claim not on file."; }
			else if(code=="N6") { return "Under FEHB law (U.S.C. 8904(b)), we cannot pay more for covered care than the amount Medicare would have allowed if the patient were enrolled in Medicare Part A and/or Medicare Part B."; }
			else if(code=="N7") { return "Alert: Processing of this claim/service has included consideration under Major Medical provisions."; }
			else if(code=="N8") { return "Crossover claim denied by previous payer and complete claim data not forwarded. Resubmit this claim to this payer to provide adequate data for adjudication."; }
			else if(code=="N9") { return "Adjustment represents the estimated amount a previous payer may pay."; }
			else if(code=="N10") { return "Payment based on the findings of a review organization/professional consult/manual adjudication/medical advisor/dental advisor/peer review."; }
			else if(code=="N11") { return "Denial reversed because of medical review."; }
			else if(code=="N12") { return "Policy provides coverage supplemental to Medicare. As the member does not appear to be enrolled in the applicable part of Medicare, the member is responsible for payment of the portion of the charge that would have been covered by Medicare."; }
			else if(code=="N13") { return "Payment based on professional/technical component modifier(s)."; }
			else if(code=="N14") { return "Payment based on a contractual amount or agreement, fee schedule, or maximum allowable amount."; }
			else if(code=="N15") { return "Services for a newborn must be billed separately."; }
			else if(code=="N16") { return "Family/member Out-of-Pocket maximum has been met. Payment based on a higher percentage."; }
			else if(code=="N17") { return "Per admission deductible."; }
			else if(code=="N18") { return "Payment based on the Medicare allowed amount."; }
			else if(code=="N19") { return "Procedure code incidental to primary procedure."; }
			else if(code=="N20") { return "Service not payable with other service rendered on the same date."; }
			else if(code=="N21") { return "Alert: Your line item has been separated into multiple lines to expedite handling."; }
			else if(code=="N22") { return "This procedure code was added/changed because it more accurately describes the services rendered."; }
			else if(code=="N23") { return "Alert: Patient liability may be affected due to coordination of benefits with other carriers and/or maximum benefit provisions."; }
			else if(code=="N24") { return "Missing/incomplete/invalid Electronic Funds Transfer (EFT) banking information."; }
			else if(code=="N25") { return "This company has been contracted by your benefit plan to provide administrative claims payment services only. This company does not assume financial risk or obligation with respect to claims processed on behalf of your benefit plan."; }
			else if(code=="N26") { return "Missing itemized bill/statement."; }
			else if(code=="N27") { return "Missing/incomplete/invalid treatment number."; }
			else if(code=="N28") { return "Consent form requirements not fulfilled."; }
			else if(code=="N29") { return "Missing documentation/orders/notes/summary/report/chart."; }
			else if(code=="N30") { return "Patient ineligible for this service."; }
			else if(code=="N31") { return "Missing/incomplete/invalid prescribing provider identifier."; }
			else if(code=="N32") { return "Claim must be submitted by the provider who rendered the service."; }
			else if(code=="N33") { return "No record of health check prior to initiation of treatment."; }
			else if(code=="N34") { return "Incorrect claim form/format for this service."; }
			else if(code=="N35") { return "Program integrity/utilization review decision."; }
			else if(code=="N36") { return "Claim must meet primary payer’s processing requirements before we can consider payment."; }
			else if(code=="N37") { return "Missing/incomplete/invalid tooth number/letter."; }
			else if(code=="N38") { return "Missing/incomplete/invalid place of service."; }
			else if(code=="N39") { return "Procedure code is not compatible with tooth number/letter."; }
			else if(code=="N40") { return "Missing radiology film(s)/image(s)."; }
			else if(code=="N41") { return "Authorization request denied."; }
			else if(code=="N42") { return "No record of mental health assessment."; }
			else if(code=="N43") { return "Bed hold or leave days exceeded."; }
			else if(code=="N44") { return "Payer’s share of regulatory surcharges, assessments, allowances or health care-related taxes paid directly to the regulatory authority."; }
			else if(code=="N45") { return "Payment based on authorized amount."; }
			else if(code=="N46") { return "Missing/incomplete/invalid admission hour."; }
			else if(code=="N47") { return "Claim conflicts with another inpatient stay."; }
			else if(code=="N48") { return "Claim information does not agree with information received from other insurance carrier."; }
			else if(code=="N49") { return "Court ordered coverage information needs validation."; }
			else if(code=="N50") { return "Missing/incomplete/invalid discharge information."; }
			else if(code=="N51") { return "Electronic interchange agreement not on file for provider/submitter."; }
			else if(code=="N52") { return "Patient not enrolled in the billing provider's managed care plan on the date of service."; }
			else if(code=="N53") { return "Missing/incomplete/invalid point of pick-up address."; }
			else if(code=="N54") { return "Claim information is inconsistent with pre-certified/authorized services."; }
			else if(code=="N55") { return "Procedures for billing with group/referring/performing providers were not followed."; }
			else if(code=="N56") { return "Procedure code billed is not correct/valid for the services billed or the date of service billed."; }
			else if(code=="N57") { return "Missing/incomplete/invalid prescribing date."; }
			else if(code=="N58") { return "Missing/incomplete/invalid patient liability amount."; }
			else if(code=="N59") { return "Please refer to your provider manual for additional program and provider information."; }
			else if(code=="N60") { return "A valid NDC is required for payment of drug claims effective October 02."; }
			else if(code=="N61") { return "Rebill services on separate claims."; }
			else if(code=="N62") { return "Dates of service span multiple rate periods. Resubmit separate claims."; }
			else if(code=="N63") { return "Rebill services on separate claim lines."; }
			else if(code=="N64") { return "The “from” and “to” dates must be different."; }
			else if(code=="N65") { return "Procedure code or procedure rate count cannot be determined, or was not on file, for the date of service/provider."; }
			else if(code=="N66") { return "Missing/incomplete/invalid documentation."; }
			else if(code=="N67") { return "Professional provider services not paid separately. Included in facility payment under a demonstration project. Apply to that facility for payment, or resubmit your claim if: the facility notifies you the patient was excluded from this demonstration; or if you furnished these services in another location on the date of the patient’s admission or discharge from a demonstration hospital. If services were furnished in a facility not involved in the demonstration on the same date the patient was discharged from or admitted to a demonstration facility, you must report the provider ID number for the non-demonstration facility on the new claim."; }
			else if(code=="N68") { return "Prior payment being cancelled as we were subsequently notified this patient was covered by a demonstration project in this site of service. Professional services were included in the payment made to the facility. You must contact the facility for your payment. Prior payment made to you by the patient or another insurer for this claim must be refunded to the payer within 30 days."; }
			else if(code=="N69") { return "PPS (Prospective Payment System) code changed by claims processing system."; }
			else if(code=="N70") { return "Consolidated billing and payment applies."; }
			else if(code=="N71") { return "Your unassigned claim for a drug or biological, clinical diagnostic laboratory services or ambulance service was processed as an assigned claim. You are required by law to accept assignment for these types of claims."; }
			else if(code=="N72") { return "PPS (Prospective Payment System) code changed by medical reviewers. Not supported by clinical records."; }
			else if(code=="N73") { return "A Skilled Nursing Facility is responsible for payment of outside providers who furnish these services/supplies under arrangement to its residents."; }
			else if(code=="N74") { return "Resubmit with multiple claims, each claim covering services provided in only one calendar month."; }
			else if(code=="N75") { return "Missing/incomplete/invalid tooth surface information."; }
			else if(code=="N76") { return "Missing/incomplete/invalid number of riders."; }
			else if(code=="N77") { return "Missing/incomplete/invalid designated provider number."; }
			else if(code=="N78") { return "The necessary components of the child and teen checkup (EPSDT) were not completed."; }
			else if(code=="N79") { return "Service billed is not compatible with patient location information."; }
			else if(code=="N80") { return "Missing/incomplete/invalid prenatal screening information."; }
			else if(code=="N81") { return "Procedure billed is not compatible with tooth surface code."; }
			else if(code=="N82") { return "Provider must accept insurance payment as payment in full when a third party payer contract specifies full reimbursement."; }
			else if(code=="N83") { return "No appeal rights. Adjudicative decision based on the provisions of a demonstration project."; }
			else if(code=="N84") { return "Alert: Further installment payments are forthcoming."; }
			else if(code=="N85") { return "Alert: This is the final installment payment."; }
			else if(code=="N86") { return "A failed trial of pelvic muscle exercise training is required in order for biofeedback training for the treatment of urinary incontinence to be covered."; }
			else if(code=="N87") { return "Home use of biofeedback therapy is not covered."; }
			else if(code=="N88") { return "Alert: This payment is being made conditionally. An HHA episode of care notice has been filed for this patient. When a patient is treated under a HHA episode of care, consolidated billing requires that certain therapy services and supplies, such as this, be included in the HHA's payment. This payment will need to be recouped from you if we establish that the patient is concurrently receiving treatment under a HHA episode of care."; }
			else if(code=="N89") { return "Alert: Payment information for this claim has been forwarded to more than one other payer, but format limitations permit only one of the secondary payers to be identified in this remittance advice."; }
			else if(code=="N90") { return "Covered only when performed by the attending physician."; }
			else if(code=="N91") { return "Services not included in the appeal review."; }
			else if(code=="N92") { return "This facility is not certified for digital mammography."; }
			else if(code=="N93") { return "A separate claim must be submitted for each place of service. Services furnished at multiple sites may not be billed in the same claim."; }
			else if(code=="N94") { return "Claim/Service denied because a more specific taxonomy code is required for adjudication."; }
			else if(code=="N95") { return "This provider type/provider specialty may not bill this service."; }
			else if(code=="N96") { return "Patient must be refractory to conventional therapy (documented behavioral, pharmacologic and/or surgical corrective therapy) and be an appropriate surgical candidate such that implantation with anesthesia can occur."; }
			else if(code=="N97") { return "Patients with stress incontinence, urinary obstruction, and specific neurologic diseases (e.g., diabetes with peripheral nerve involvement) which are associated with secondary manifestations of the above three indications are excluded."; }
			else if(code=="N98") { return "Patient must have had a successful test stimulation in order to support subsequent implantation. Before a patient is eligible for permanent implantation, he/she must demonstrate a 50 percent or greater improvement through test stimulation. Improvement is measured through voiding diaries."; }
			else if(code=="N99") { return "Patient must be able to demonstrate adequate ability to record voiding diary data such that clinical results of the implant procedure can be properly evaluated."; }
			else if(code=="N100") { return "PPS (Prospect Payment System) code corrected during adjudication."; }
			else if(code=="N101") { return "Additional information is needed in order to process this claim. Please resubmit the claim with the identification number of the provider where this service took place. The Medicare number of the site of service provider should be preceded with the letters 'HSP' and entered into item #32 on the claim form. You may bill only one site of service provider number per claim."; }
			else if(code=="N102") { return "This claim has been denied without reviewing the medical/dental record because the requested records were not received or were not received timely."; }
			else if(code=="N103") { return "Records indicate this patient was a prisoner or in custody of a Federal, State, or local authority when the service was rendered. This payer does not cover items and services furnished to an individual while he or she is in custody under a penal statute or rule, unless under State or local law, the individual is personally liable for the cost of his or her health care while in custody and the State or local government pursues the collection of such debt in the same way and with the same vigor as the collection of its other debts. The provider can collect from the Federal/State/ Local Authority as appropriate."; }
			else if(code=="N104") { return "This claim/service is not payable under our claims jurisdiction area. You can identify the correct Medicare contractor to process this claim/service through the CMS website at www.cms.gov."; }
			else if(code=="N105") { return "This is a misdirected claim/service for an RRB beneficiary. Submit paper claims to the RRB carrier: Palmetto GBA, P.O. Box 10066, Augusta, GA 30999. Call 866-749-4301 for RRB EDI information for electronic claims processing."; }
			else if(code=="N106") { return "Payment for services furnished to Skilled Nursing Facility (SNF) inpatients (except for excluded services) can only be made to the SNF. You must request payment from the SNF rather than the patient for this service."; }
			else if(code=="N107") { return "Services furnished to Skilled Nursing Facility (SNF) inpatients must be billed on the inpatient claim. They cannot be billed separately as outpatient services."; }
			else if(code=="N108") { return "Missing/incomplete/invalid upgrade information."; }
			else if(code=="N109") { return "This claim/service was chosen for complex review and was denied after reviewing the medical records."; }
			else if(code=="N110") { return "This facility is not certified for film mammography."; }
			else if(code=="N111") { return "No appeal right except duplicate claim/service issue. This service was included in a claim that has been previously billed and adjudicated."; }
			else if(code=="N112") { return "This claim is excluded from your electronic remittance advice."; }
			else if(code=="N113") { return "Only one initial visit is covered per physician, group practice or provider."; }
			else if(code=="N114") { return "During the transition to the Ambulance Fee Schedule, payment is based on the lesser of a blended amount calculated using a percentage of the reasonable charge/cost and fee schedule amounts, or the submitted charge for the service. You will be notified yearly what the percentages for the blended payment calculation will be."; }
			else if(code=="N115") { return "This decision was based on a Local Coverage Determination (LCD). An LCD provides a guide to assist in determining whether a particular item or service is covered. A copy of this policy is available at www.cms.gov/mcd, or if you do not have web access, you may contact the contractor to request a copy of the LCD."; }
			else if(code=="N116") { return "This payment is being made conditionally because the service was provided in the home, and it is possible that the patient is under a home health episode of care. When a patient is treated under a home health episode of care, consolidated billing requires that certain therapy services and supplies, such as this, be included in the home health agency’s (HHA’s) payment. This payment will need to be recouped from you if we establish that the patient is concurrently receiving treatment under an HHA episode of care."; }
			else if(code=="N117") { return "This service is paid only once in a patient’s lifetime."; }
			else if(code=="N118") { return "This service is not paid if billed more than once every 28 days."; }
			else if(code=="N119") { return "This service is not paid if billed once every 28 days, and the patient has spent 5 or more consecutive days in any inpatient or Skilled /nursing Facility (SNF) within those 28 days."; }
			else if(code=="N120") { return "Payment is subject to home health prospective payment system partial episode payment adjustment. Patient was transferred/discharged/readmitted during payment episode."; }
			else if(code=="N121") { return "Medicare Part B does not pay for items or services provided by this type of practitioner for beneficiaries in a Medicare Part A covered Skilled Nursing Facility (SNF) stay."; }
			else if(code=="N122") { return "Add-on code cannot be billed by itself."; }
			else if(code=="N123") { return "This is a split service and represents a portion of the units from the originally submitted service."; }
			else if(code=="N124") { return "Payment has been denied for the/made only for a less extensive service/item because the information furnished does not substantiate the need for the (more extensive) service/item. The patient is liable for the charges for this service/item as you informed the patient in writing before the service/item was furnished that we would not pay for it, and the patient agreed to pay."; }
			else if(code=="N125") { return "Payment has been (denied for the/made only for a less extensive) service/item because the information furnished does not substantiate the need for the (more extensive) service/item. If you have collected any amount from the patient, you must refund that amount to the patient within 30 days of receiving this notice. The requirements for a refund are in §1834(a)(18) of the Social Security Act (and in §§1834(j)(4) and 1879(h) by cross-reference to §1834(a)(18)). Section 1834(a)(18)(B) specifies that suppliers which knowingly and willfully fail to make appropriate refunds may be subject to civil money penalties and/or exclusion from the Medicare program. If you have any questions about this notice, please contact this office."; }
			else if(code=="N126") { return "Social Security Records indicate that this individual has been deported. This payer does not cover items and services furnished to individuals who have been deported."; }
			else if(code=="N127") { return "This is a misdirected claim/service for a United Mine Workers of America (UMWA) beneficiary. Please submit claims to them."; }
			else if(code=="N128") { return "This amount represents the prior to coverage portion of the allowance."; }
			else if(code=="N129") { return "Not eligible due to the patient's age."; }
			else if(code=="N130") { return "Consult plan benefit documents/guidelines for information about restrictions for this service."; }
			else if(code=="N131") { return "Total payments under multiple contracts cannot exceed the allowance for this service."; }
			else if(code=="N132") { return "Alert: Payments will cease for services rendered by this US Government debarred or excluded provider after the 30 day grace period as previously notified."; }
			else if(code=="N133") { return "Alert: Services for predetermination and services requesting payment are being processed separately."; }
			else if(code=="N134") { return "Alert: This represents your scheduled payment for this service. If treatment has been discontinued, please contact Customer Service."; }
			else if(code=="N135") { return "Record fees are the patient's portion and limited to the specified co-payment."; }
			else if(code=="N136") { return "Alert: To obtain information on the process to file an appeal in Arizona, call the Department's Consumer Assistance Office at (602) 912-8444 or (800) 325-2548."; }
			else if(code=="N137") { return "Alert: The provider acting on the Member's behalf, may file an appeal with the Payer. The provider, acting on the Member's behalf, may file a complaint with the State Insurance Regulatory Authority without first filing an appeal, if the coverage decision involves an urgent condition for which care has not been rendered. The address may be obtained from the State Insurance Regulatory Authority."; }
			else if(code=="N138") { return "Alert: In the event you disagree with the Dental Advisor's opinion and have additional information relative to the case, you may submit radiographs to the Dental Advisor Unit at the subscriber's dental insurance carrier for a second Independent Dental Advisor Review."; }
			else if(code=="N139") { return "Alert: Under the Code of Federal Regulations, Chapter 32, Section 199.13 a non-participating provider is not an appropriate appealing party. Therefore, if you disagree with the Dental Advisor's opinion, you may appeal the determination if appointed in writing, by the beneficiary, to act as his/her representative. Should you be appointed as a representative, submit a copy of this letter, a signed statement explaining the matter in which you disagree, and any radiographs and relevant information to the subscriber's Dental insurance carrier within 90 days from the date of this letter."; }
			else if(code=="N140") { return "Alert: You have not been designated as an authorized OCONUS provider therefore are not considered an appropriate appealing party. If the beneficiary has appointed you, in writing, to act as his/her representative and you disagree with the Dental Advisor's opinion, you may appeal by submitting a copy of this letter, a signed statement explaining the matter in which you disagree, and any relevant information to the subscriber's Dental insurance carrier within 90 days from the date of this letter."; }
			else if(code=="N141") { return "The patient was not residing in a long-term care facility during all or part of the service dates billed."; }
			else if(code=="N142") { return "The original claim was denied. Resubmit a new claim, not a replacement claim."; }
			else if(code=="N143") { return "The patient was not in a hospice program during all or part of the service dates billed."; }
			else if(code=="N144") { return "The rate changed during the dates of service billed."; }
			else if(code=="N145") { return "Missing/incomplete/invalid provider identifier for this place of service."; }
			else if(code=="N146") { return "Missing screening document."; }
			else if(code=="N147") { return "Long term care case mix or per diem rate cannot be determined because the patient ID number is missing, incomplete, or invalid on the assignment request."; }
			else if(code=="N148") { return "Missing/incomplete/invalid date of last menstrual period."; }
			else if(code=="N149") { return "Rebill all applicable services on a single claim."; }
			else if(code=="N150") { return "Missing/incomplete/invalid model number."; }
			else if(code=="N151") { return "Telephone contact services will not be paid until the face-to-face contact requirement has been met."; }
			else if(code=="N152") { return "Missing/incomplete/invalid replacement claim information."; }
			else if(code=="N153") { return "Missing/incomplete/invalid room and board rate."; }
			else if(code=="N154") { return "Alert: This payment was delayed for correction of provider's mailing address."; }
			else if(code=="N155") { return "Alert: Our records do not indicate that other insurance is on file. Please submit other insurance information for our records."; }
			else if(code=="N156") { return "Alert: The patient is responsible for the difference between the approved treatment and the elective treatment."; }
			else if(code=="N157") { return "Transportation to/from this destination is not covered."; }
			else if(code=="N158") { return "Transportation in a vehicle other than an ambulance is not covered."; }
			else if(code=="N159") { return "Payment denied/reduced because mileage is not covered when the patient is not in the ambulance."; }
			else if(code=="N160") { return "The patient must choose an option before a payment can be made for this procedure/ equipment/ supply/ service."; }
			else if(code=="N161") { return "This drug/service/supply is covered only when the associated service is covered."; }
			else if(code=="N162") { return "Alert: Although your claim was paid, you have billed for a test/specialty not included in your Laboratory Certification. Your failure to correct the laboratory certification information will result in a denial of payment in the near future."; }
			else if(code=="N163") { return "Medical record does not support code billed per the code definition."; }
			else if(code=="N164") { return "Transportation to/from this destination is not covered."; }
			else if(code=="N165") { return "Transportation in a vehicle other than an ambulance is not covered."; }
			else if(code=="N166") { return "Payment denied/reduced because mileage is not covered when the patient is not in the ambulance."; }
			else if(code=="N167") { return "Charges exceed the post-transplant coverage limit."; }
			else if(code=="N168") { return "The patient must choose an option before a payment can be made for this procedure/ equipment/ supply/ service."; }
			else if(code=="N169") { return "This drug/service/supply is covered only when the associated service is covered."; }
			else if(code=="N170") { return "A new/revised/renewed certificate of medical necessity is needed."; }
			else if(code=="N171") { return "Payment for repair or replacement is not covered or has exceeded the purchase price."; }
			else if(code=="N172") { return "The patient is not liable for the denied/adjusted charge(s) for receiving any updated service/item."; }
			else if(code=="N173") { return "No qualifying hospital stay dates were provided for this episode of care."; }
			else if(code=="N174") { return "This is not a covered service/procedure/ equipment/bed, however patient liability is limited to amounts shown in the adjustments under group 'PR'."; }
			else if(code=="N175") { return "Missing review organization approval."; }
			else if(code=="N176") { return "Services provided aboard a ship are covered only when the ship is of United States registry and is in United States waters. In addition, a doctor licensed to practice in the United States must provide the service."; }
			else if(code=="N177") { return "Alert: We did not send this claim to patient’s other insurer. They have indicated no additional payment can be made."; }
			else if(code=="N178") { return "Missing pre-operative images/visual field results."; }
			else if(code=="N179") { return "Additional information has been requested from the member. The charges will be reconsidered upon receipt of that information."; }
			else if(code=="N180") { return "This item or service does not meet the criteria for the category under which it was billed."; }
			else if(code=="N181") { return "Additional information is required from another provider involved in this service."; }
			else if(code=="N182") { return "This claim/service must be billed according to the schedule for this plan."; }
			else if(code=="N183") { return "Alert: This is a predetermination advisory message, when this service is submitted for payment additional documentation as specified in plan documents will be required to process benefits."; }
			else if(code=="N184") { return "Rebill technical and professional components separately."; }
			else if(code=="N185") { return "Alert: Do not resubmit this claim/service."; }
			else if(code=="N186") { return "Non-Availability Statement (NAS) required for this service. Contact the nearest Military Treatment Facility (MTF) for assistance."; }
			else if(code=="N187") { return "Alert: You may request a review in writing within the required time limits following receipt of this notice by following the instructions included in your contract or plan benefit documents."; }
			else if(code=="N188") { return "The approved level of care does not match the procedure code submitted."; }
			else if(code=="N189") { return "Alert: This service has been paid as a one-time exception to the plan's benefit restrictions."; }
			else if(code=="N190") { return "Missing contract indicator."; }
			else if(code=="N191") { return "The provider must update insurance information directly with payer."; }
			else if(code=="N192") { return "Patient is a Medicaid/Qualified Medicare Beneficiary."; }
			else if(code=="N193") { return "Specific federal/state/local program may cover this service through another payer."; }
			else if(code=="N194") { return "Technical component not paid if provider does not own the equipment used."; }
			else if(code=="N195") { return "The technical component must be billed separately."; }
			else if(code=="N196") { return "Alert: Patient eligible to apply for other coverage which may be primary."; }
			else if(code=="N197") { return "The subscriber must update insurance information directly with payer."; }
			else if(code=="N198") { return "Rendering provider must be affiliated with the pay-to provider."; }
			else if(code=="N199") { return "Additional payment/recoupment approved based on payer-initiated review/audit."; }
			else if(code=="N200") { return "The professional component must be billed separately."; }
			else if(code=="N201") { return "A mental health facility is responsible for payment of outside providers who furnish these services/supplies to residents."; }
			else if(code=="N202") { return "Additional information/explanation will be sent separately"; }
			else if(code=="N203") { return "Missing/incomplete/invalid anesthesia time/units"; }
			else if(code=="N204") { return "Services under review for possible pre-existing condition. Send medical records for prior 12 months"; }
			else if(code=="N205") { return "Information provided was illegible"; }
			else if(code=="N206") { return "The supporting documentation does not match the information sent on the claim."; }
			else if(code=="N207") { return "Missing/incomplete/invalid weight."; }
			else if(code=="N208") { return "Missing/incomplete/invalid DRG code"; }
			else if(code=="N209") { return "Missing/incomplete/invalid taxpayer identification number (TIN)."; }
			else if(code=="N210") { return "Alert: You may appeal this decision"; }
			else if(code=="N211") { return "Alert: You may not appeal this decision"; }
			else if(code=="N212") { return "Charges processed under a Point of Service benefit"; }
			else if(code=="N213") { return "Missing/incomplete/invalid facility/discrete unit DRG/DRG exempt status information"; }
			else if(code=="N214") { return "Missing/incomplete/invalid history of the related initial surgical procedure(s)"; }
			else if(code=="N215") { return "Alert: A payer providing supplemental or secondary coverage shall not require a claims determination for this service from a primary payer as a condition of making its own claims determination."; }
			else if(code=="N216") { return "We do not offer coverage for this type of service or the patient is not enrolled in this portion of our benefit package"; }
			else if(code=="N217") { return "We pay only one site of service per provider per claim"; }
			else if(code=="N218") { return "You must furnish and service this item for as long as the patient continues to need it. We can pay for maintenance and/or servicing for the time period specified in the contract or coverage manual."; }
			else if(code=="N219") { return "Payment based on previous payer's allowed amount."; }
			else if(code=="N220") { return "Alert: See the payer's web site or contact the payer's Customer Service department to obtain forms and instructions for filing a provider dispute."; }
			else if(code=="N221") { return "Missing Admitting History and Physical report."; }
			else if(code=="N222") { return "Incomplete/invalid Admitting History and Physical report."; }
			else if(code=="N223") { return "Missing documentation of benefit to the patient during initial treatment period."; }
			else if(code=="N224") { return "Incomplete/invalid documentation of benefit to the patient during initial treatment period."; }
			else if(code=="N225") { return "Incomplete/invalid documentation/orders/notes/summary/report/chart."; }
			else if(code=="N226") { return "Incomplete/invalid American Diabetes Association Certificate of Recognition."; }
			else if(code=="N227") { return "Incomplete/invalid Certificate of Medical Necessity."; }
			else if(code=="N228") { return "Incomplete/invalid consent form."; }
			else if(code=="N229") { return "Incomplete/invalid contract indicator."; }
			else if(code=="N230") { return "Incomplete/invalid indication of whether the patient owns the equipment that requires the part or supply."; }
			else if(code=="N231") { return "Incomplete/invalid invoice or statement certifying the actual cost of the lens, less discounts, and/or the type of intraocular lens used."; }
			else if(code=="N232") { return "Incomplete/invalid itemized bill/statement."; }
			else if(code=="N233") { return "Incomplete/invalid operative note/report."; }
			else if(code=="N234") { return "Incomplete/invalid oxygen certification/re-certification."; }
			else if(code=="N235") { return "Incomplete/invalid pacemaker registration form."; }
			else if(code=="N236") { return "Incomplete/invalid pathology report."; }
			else if(code=="N237") { return "Incomplete/invalid patient medical record for this service."; }
			else if(code=="N238") { return "Incomplete/invalid physician certified plan of care"; }
			else if(code=="N239") { return "Incomplete/invalid physician financial relationship form."; }
			else if(code=="N240") { return "Incomplete/invalid radiology report."; }
			else if(code=="N241") { return "Incomplete/invalid review organization approval."; }
			else if(code=="N242") { return "Incomplete/invalid radiology film(s)/image(s)."; }
			else if(code=="N243") { return "Incomplete/invalid/not approved screening document."; }
			else if(code=="N244") { return "Incomplete/Invalid pre-operative images/visual field results."; }
			else if(code=="N245") { return "Incomplete/invalid plan information for other insurance"; }
			else if(code=="N246") { return "State regulated patient payment limitations apply to this service."; }
			else if(code=="N247") { return "Missing/incomplete/invalid assistant surgeon taxonomy."; }
			else if(code=="N248") { return "Missing/incomplete/invalid assistant surgeon name."; }
			else if(code=="N249") { return "Missing/incomplete/invalid assistant surgeon primary identifier."; }
			else if(code=="N250") { return "Missing/incomplete/invalid assistant surgeon secondary identifier."; }
			else if(code=="N251") { return "Missing/incomplete/invalid attending provider taxonomy."; }
			else if(code=="N252") { return "Missing/incomplete/invalid attending provider name."; }
			else if(code=="N253") { return "Missing/incomplete/invalid attending provider primary identifier."; }
			else if(code=="N254") { return "Missing/incomplete/invalid attending provider secondary identifier."; }
			else if(code=="N255") { return "Missing/incomplete/invalid billing provider taxonomy."; }
			else if(code=="N256") { return "Missing/incomplete/invalid billing provider/supplier name."; }
			else if(code=="N257") { return "Missing/incomplete/invalid billing provider/supplier primary identifier."; }
			else if(code=="N258") { return "Missing/incomplete/invalid billing provider/supplier address."; }
			else if(code=="N259") { return "Missing/incomplete/invalid billing provider/supplier secondary identifier."; }
			else if(code=="N260") { return "Missing/incomplete/invalid billing provider/supplier contact information."; }
			else if(code=="N261") { return "Missing/incomplete/invalid operating provider name."; }
			else if(code=="N262") { return "Missing/incomplete/invalid operating provider primary identifier."; }
			else if(code=="N263") { return "Missing/incomplete/invalid operating provider secondary identifier."; }
			else if(code=="N264") { return "Missing/incomplete/invalid ordering provider name."; }
			else if(code=="N265") { return "Missing/incomplete/invalid ordering provider primary identifier."; }
			else if(code=="N266") { return "Missing/incomplete/invalid ordering provider address."; }
			else if(code=="N267") { return "Missing/incomplete/invalid ordering provider secondary identifier."; }
			else if(code=="N268") { return "Missing/incomplete/invalid ordering provider contact information."; }
			else if(code=="N269") { return "Missing/incomplete/invalid other provider name."; }
			else if(code=="N270") { return "Missing/incomplete/invalid other provider primary identifier."; }
			else if(code=="N271") { return "Missing/incomplete/invalid other provider secondary identifier."; }
			else if(code=="N272") { return "Missing/incomplete/invalid other payer attending provider identifier."; }
			else if(code=="N273") { return "Missing/incomplete/invalid other payer operating provider identifier."; }
			else if(code=="N274") { return "Missing/incomplete/invalid other payer other provider identifier."; }
			else if(code=="N275") { return "Missing/incomplete/invalid other payer purchased service provider identifier."; }
			else if(code=="N276") { return "Missing/incomplete/invalid other payer referring provider identifier."; }
			else if(code=="N277") { return "Missing/incomplete/invalid other payer rendering provider identifier."; }
			else if(code=="N278") { return "Missing/incomplete/invalid other payer service facility provider identifier."; }
			else if(code=="N279") { return "Missing/incomplete/invalid pay-to provider name."; }
			else if(code=="N280") { return "Missing/incomplete/invalid pay-to provider primary identifier."; }
			else if(code=="N281") { return "Missing/incomplete/invalid pay-to provider address."; }
			else if(code=="N282") { return "Missing/incomplete/invalid pay-to provider secondary identifier."; }
			else if(code=="N283") { return "Missing/incomplete/invalid purchased service provider identifier."; }
			else if(code=="N284") { return "Missing/incomplete/invalid referring provider taxonomy."; }
			else if(code=="N285") { return "Missing/incomplete/invalid referring provider name."; }
			else if(code=="N286") { return "Missing/incomplete/invalid referring provider primary identifier."; }
			else if(code=="N287") { return "Missing/incomplete/invalid referring provider secondary identifier."; }
			else if(code=="N288") { return "Missing/incomplete/invalid rendering provider taxonomy."; }
			else if(code=="N289") { return "Missing/incomplete/invalid rendering provider name."; }
			else if(code=="N290") { return "Missing/incomplete/invalid rendering provider primary identifier."; }
			else if(code=="N291") { return "Missing/incomplete/invalid rendering provider secondary identifier."; }
			else if(code=="N292") { return "Missing/incomplete/invalid service facility name."; }
			else if(code=="N293") { return "Missing/incomplete/invalid service facility primary identifier."; }
			else if(code=="N294") { return "Missing/incomplete/invalid service facility primary address."; }
			else if(code=="N295") { return "Missing/incomplete/invalid service facility secondary identifier."; }
			else if(code=="N296") { return "Missing/incomplete/invalid supervising provider name."; }
			else if(code=="N297") { return "Missing/incomplete/invalid supervising provider primary identifier."; }
			else if(code=="N298") { return "Missing/incomplete/invalid supervising provider secondary identifier."; }
			else if(code=="N299") { return "Missing/incomplete/invalid occurrence date(s)."; }
			else if(code=="N300") { return "Missing/incomplete/invalid occurrence span date(s)."; }
			else if(code=="N301") { return "Missing/incomplete/invalid procedure date(s)."; }
			else if(code=="N302") { return "Missing/incomplete/invalid other procedure date(s)."; }
			else if(code=="N303") { return "Missing/incomplete/invalid principal procedure date."; }
			else if(code=="N304") { return "Missing/incomplete/invalid dispensed date."; }
			else if(code=="N305") { return "Missing/incomplete/invalid accident date."; }
			else if(code=="N306") { return "Missing/incomplete/invalid acute manifestation date."; }
			else if(code=="N307") { return "Missing/incomplete/invalid adjudication or payment date."; }
			else if(code=="N308") { return "Missing/incomplete/invalid appliance placement date."; }
			else if(code=="N309") { return "Missing/incomplete/invalid assessment date."; }
			else if(code=="N310") { return "Missing/incomplete/invalid assumed or relinquished care date."; }
			else if(code=="N311") { return "Missing/incomplete/invalid authorized to return to work date."; }
			else if(code=="N312") { return "Missing/incomplete/invalid begin therapy date."; }
			else if(code=="N313") { return "Missing/incomplete/invalid certification revision date."; }
			else if(code=="N314") { return "Missing/incomplete/invalid diagnosis date."; }
			else if(code=="N315") { return "Missing/incomplete/invalid disability from date."; }
			else if(code=="N316") { return "Missing/incomplete/invalid disability to date."; }
			else if(code=="N317") { return "Missing/incomplete/invalid discharge hour."; }
			else if(code=="N318") { return "Missing/incomplete/invalid discharge or end of care date."; }
			else if(code=="N319") { return "Missing/incomplete/invalid hearing or vision prescription date."; }
			else if(code=="N320") { return "Missing/incomplete/invalid Home Health Certification Period."; }
			else if(code=="N321") { return "Missing/incomplete/invalid last admission period."; }
			else if(code=="N322") { return "Missing/incomplete/invalid last certification date."; }
			else if(code=="N323") { return "Missing/incomplete/invalid last contact date."; }
			else if(code=="N324") { return "Missing/incomplete/invalid last seen/visit date."; }
			else if(code=="N325") { return "Missing/incomplete/invalid last worked date."; }
			else if(code=="N326") { return "Missing/incomplete/invalid last x-ray date."; }
			else if(code=="N327") { return "Missing/incomplete/invalid other insured birth date."; }
			else if(code=="N328") { return "Missing/incomplete/invalid Oxygen Saturation Test date."; }
			else if(code=="N329") { return "Missing/incomplete/invalid patient birth date."; }
			else if(code=="N330") { return "Missing/incomplete/invalid patient death date."; }
			else if(code=="N331") { return "Missing/incomplete/invalid physician order date."; }
			else if(code=="N332") { return "Missing/incomplete/invalid prior hospital discharge date."; }
			else if(code=="N333") { return "Missing/incomplete/invalid prior placement date."; }
			else if(code=="N334") { return "Missing/incomplete/invalid re-evaluation date"; }
			else if(code=="N335") { return "Missing/incomplete/invalid referral date."; }
			else if(code=="N336") { return "Missing/incomplete/invalid replacement date."; }
			else if(code=="N337") { return "Missing/incomplete/invalid secondary diagnosis date."; }
			else if(code=="N338") { return "Missing/incomplete/invalid shipped date."; }
			else if(code=="N339") { return "Missing/incomplete/invalid similar illness or symptom date."; }
			else if(code=="N340") { return "Missing/incomplete/invalid subscriber birth date."; }
			else if(code=="N341") { return "Missing/incomplete/invalid surgery date."; }
			else if(code=="N342") { return "Missing/incomplete/invalid test performed date."; }
			else if(code=="N343") { return "Missing/incomplete/invalid Transcutaneous Electrical Nerve Stimulator (TENS) trial start date."; }
			else if(code=="N344") { return "Missing/incomplete/invalid Transcutaneous Electrical Nerve Stimulator (TENS) trial end date."; }
			else if(code=="N345") { return "Date range not valid with units submitted."; }
			else if(code=="N346") { return "Missing/incomplete/invalid oral cavity designation code."; }
			else if(code=="N347") { return "Your claim for a referred or purchased service cannot be paid because payment has already been made for this same service to another provider by a payment contractor representing the payer."; }
			else if(code=="N348") { return "You chose that this service/supply/drug would be rendered/supplied and billed by a different practitioner/supplier."; }
			else if(code=="N349") { return "The administration method and drug must be reported to adjudicate this service."; }
			else if(code=="N350") { return "Missing/incomplete/invalid description of service for a Not Otherwise Classified (NOC) code or for an Unlisted/By Report procedure."; }
			else if(code=="N351") { return "Service date outside of the approved treatment plan service dates."; }
			else if(code=="N352") { return "Alert: There are no scheduled payments for this service. Submit a claim for each patient visit."; }
			else if(code=="N353") { return "Alert: Benefits have been estimated, when the actual services have been rendered, additional payment will be considered based on the submitted claim."; }
			else if(code=="N354") { return "Incomplete/invalid invoice"; }
			else if(code=="N355") { return "Alert: The law permits exceptions to the refund requirement in two cases: - If you did not know, and could not have reasonably been expected to know, that we would not pay for this service; or - If you notified the patient in writing before providing the service that you believed that we were likely to deny the service, and the patient signed a statement agreeing to pay for the service. If you come within either exception, or if you believe the carrier was wrong in its determination that we do not pay for this service, you should request appeal of this determination within 30 days of the date of this notice. Your request for review should include any additional information necessary to support your position. If you request an appeal within 30 days of receiving this notice, you may delay refunding the amount to the patient until you receive the results of the review. If the review decision is favorable to you, you do not need to make any refund. If, however, the review is unfavorable, the law specifies that you must make the refund within 15 days of receiving the unfavorable review decision. The law also permits you to request an appeal at any time within 120 days of the date you receive this notice. However, an appeal request that is received more than 30 days after the date of this notice, does not permit you to delay making the refund. Regardless of when a review is requested, the patient will be notified that you have requested one, and will receive a copy of the determination. The patient has received a separate notice of this denial decision. The notice advises that he/she may be entitled to a refund of any amounts paid, if you should have known that we would not pay and did not tell him/her. It also instructs the patient to contact our office if he/she does not hear anything about a refund within 30 days"; }
			else if(code=="N356") { return "Not covered when performed with, or subsequent to, a non-covered service."; }
			else if(code=="N357") { return "Time frame requirements between this service/procedure/supply and a related service/procedure/supply have not been met."; }
			else if(code=="N358") { return "Alert: This decision may be reviewed if additional documentation as described in the contract or plan benefit documents is submitted."; }
			else if(code=="N359") { return "Missing/incomplete/invalid height."; }
			else if(code=="N360") { return "Alert: Coordination of benefits has not been calculated when estimating benefits for this pre-determination. Submit payment information from the primary payer with the secondary claim."; }
			else if(code=="N361") { return "Payment adjusted based on multiple diagnostic imaging procedure rules"; }
			else if(code=="N362") { return "The number of Days or Units of Service exceeds our acceptable maximum."; }
			else if(code=="N363") { return "Alert: in the near future we are implementing new policies/procedures that would affect this determination."; }
			else if(code=="N364") { return "Alert: According to our agreement, you must waive the deductible and/or coinsurance amounts."; }
			else if(code=="N365") { return "This procedure code is not payable. It is for reporting/information purposes only."; }
			else if(code=="N366") { return "Requested information not provided. The claim will be reopened if the information previously requested is submitted within one year after the date of this denial notice."; }
			else if(code=="N367") { return "Alert: The claim information has been forwarded to a Consumer Spending Account processor for review; for example, flexible spending account or health savings account."; }
			else if(code=="N368") { return "You must appeal the determination of the previously adjudicated claim."; }
			else if(code=="N369") { return "Alert: Although this claim has been processed, it is deficient according to state legislation/regulation."; }
			else if(code=="N370") { return "Billing exceeds the rental months covered/approved by the payer."; }
			else if(code=="N371") { return "Alert: title of this equipment must be transferred to the patient."; }
			else if(code=="N372") { return "Only reasonable and necessary maintenance/service charges are covered."; }
			else if(code=="N373") { return "It has been determined that another payer paid the services as primary when they were not the primary payer. Therefore, we are refunding to the payer that paid as primary on your behalf."; }
			else if(code=="N374") { return "Primary Medicare Part A insurance has been exhausted and a Part B Remittance Advice is required."; }
			else if(code=="N375") { return "Missing/incomplete/invalid questionnaire/information required to determine dependent eligibility."; }
			else if(code=="N376") { return "Subscriber/patient is assigned to active military duty, therefore primary coverage may be TRICARE."; }
			else if(code=="N377") { return "Payment based on a processed replacement claim."; }
			else if(code=="N378") { return "Missing/incomplete/invalid prescription quantity."; }
			else if(code=="N379") { return "Claim level information does not match line level information."; }
			else if(code=="N380") { return "The original claim has been processed, submit a corrected claim."; }
			else if(code=="N381") { return "Consult our contractual agreement for restrictions/billing/payment information related to these charges."; }
			else if(code=="N382") { return "Missing/incomplete/invalid patient identifier."; }
			else if(code=="N383") { return "Not covered when deemed cosmetic."; }
			else if(code=="N384") { return "Records indicate that the referenced body part/tooth has been removed in a previous procedure."; }
			else if(code=="N385") { return "Notification of admission was not timely according to published plan procedures."; }
			else if(code=="N386") { return "This decision was based on a National Coverage Determination (NCD). An NCD provides a coverage determination as to whether a particular item or service is covered. A copy of this policy is available at www.cms.gov/mcd/search.asp. If you do not have web access, you may contact the contractor to request a copy of the NCD."; }
			else if(code=="N387") { return "Alert: Submit this claim to the patient's other insurer for potential payment of supplemental benefits. We did not forward the claim information."; }
			else if(code=="N388") { return "Missing/incomplete/invalid prescription number"; }
			else if(code=="N389") { return "Duplicate prescription number submitted."; }
			else if(code=="N390") { return "This service/report cannot be billed separately."; }
			else if(code=="N391") { return "Missing emergency department records."; }
			else if(code=="N392") { return "Incomplete/invalid emergency department records."; }
			else if(code=="N393") { return "Missing progress notes/report."; }
			else if(code=="N394") { return "Incomplete/invalid progress notes/report."; }
			else if(code=="N395") { return "Missing laboratory report."; }
			else if(code=="N396") { return "Incomplete/invalid laboratory report."; }
			else if(code=="N397") { return "Benefits are not available for incomplete service(s)/undelivered item(s)."; }
			else if(code=="N398") { return "Missing elective consent form."; }
			else if(code=="N399") { return "Incomplete/invalid elective consent form."; }
			else if(code=="N400") { return "Alert: Electronically enabled providers should submit claims electronically."; }
			else if(code=="N401") { return "Missing periodontal charting."; }
			else if(code=="N402") { return "Incomplete/invalid periodontal charting."; }
			else if(code=="N403") { return "Missing facility certification."; }
			else if(code=="N404") { return "Incomplete/invalid facility certification."; }
			else if(code=="N405") { return "This service is only covered when the donor's insurer(s) do not provide coverage for the service."; }
			else if(code=="N406") { return "This service is only covered when the recipient's insurer(s) do not provide coverage for the service."; }
			else if(code=="N407") { return "You are not an approved submitter for this transmission format."; }
			else if(code=="N408") { return "This payer does not cover deductibles assessed by a previous payer."; }
			else if(code=="N409") { return "This service is related to an accidental injury and is not covered unless provided within a specific time frame from the date of the accident."; }
			else if(code=="N410") { return "Not covered unless the prescription changes."; }
			else if(code=="N411") { return "This service is allowed one time in a 6-month period. (This temporary code will be deactivated on 2/1/09. Must be used with Reason Code 119.)"; }
			else if(code=="N412") { return "This service is allowed 2 times in a 12-month period. (This temporary code will be deactivated on 2/1/09. Must be used with Reason Code 119.)"; }
			else if(code=="N413") { return "This service is allowed 2 times in a benefit year. (This temporary code will be deactivated on 2/1/09. Must be used with Reason Code 119.)"; }
			else if(code=="N414") { return "This service is allowed 4 times in a 12-month period. (This temporary code will be deactivated on 2/1/09. Must be used with Reason Code 119.)"; }
			else if(code=="N415") { return "This service is allowed 1 time in an 18-month period. (This temporary code will be deactivated on 2/1/09. Must be used with Reason Code 119.)"; }
			else if(code=="N416") { return "This service is allowed 1 time in a 3-year period. (This temporary code will be deactivated on 2/1/09. Must be used with Reason Code 119.)"; }
			else if(code=="N417") { return "This service is allowed 1 time in a 5-year period. (This temporary code will be deactivated on 2/1/09. Must be used with Reason Code 119.)"; }
			else if(code=="N418") { return "Misrouted claim. See the payer's claim submission instructions."; }
			else if(code=="N419") { return "Claim payment was the result of a payer's retroactive adjustment due to a retroactive rate change."; }
			else if(code=="N420") { return "Claim payment was the result of a payer's retroactive adjustment due to a Coordination of Benefits or Third Party Liability Recovery."; }
			else if(code=="N421") { return "Claim payment was the result of a payer's retroactive adjustment due to a review organization decision."; }
			else if(code=="N422") { return "Claim payment was the result of a payer's retroactive adjustment due to a payer's contract incentive program."; }
			else if(code=="N423") { return "Claim payment was the result of a payer's retroactive adjustment due to a non standard program."; }
			else if(code=="N424") { return "Patient does not reside in the geographic area required for this type of payment."; }
			else if(code=="N425") { return "Statutorily excluded service(s)."; }
			else if(code=="N426") { return "No coverage when self-administered."; }
			else if(code=="N427") { return "Payment for eyeglasses or contact lenses can be made only after cataract surgery."; }
			else if(code=="N428") { return "Not covered when performed in this place of service."; }
			else if(code=="N429") { return "Not covered when considered routine."; }
			else if(code=="N430") { return "Procedure code is inconsistent with the units billed."; }
			else if(code=="N431") { return "Not covered with this procedure."; }
			else if(code=="N432") { return "Adjustment based on a Recovery Audit."; }
			else if(code=="N433") { return "Resubmit this claim using only your National Provider Identifier (NPI)"; }
			else if(code=="N434") { return "Missing/Incomplete/Invalid Present on Admission indicator."; }
			else if(code=="N435") { return "Exceeds number/frequency approved /allowed within time period without support documentation."; }
			else if(code=="N436") { return "The injury claim has not been accepted and a mandatory medical reimbursement has been made."; }
			else if(code=="N437") { return "Alert: If the injury claim is accepted, these charges will be reconsidered."; }
			else if(code=="N438") { return "This jurisdiction only accepts paper claims"; }
			else if(code=="N439") { return "Missing anesthesia physical status report/indicators."; }
			else if(code=="N440") { return "Incomplete/invalid anesthesia physical status report/indicators."; }
			else if(code=="N441") { return "This missed/cancelled appointment is not covered."; }
			else if(code=="N442") { return "Payment based on an alternate fee schedule."; }
			else if(code=="N443") { return "Missing/incomplete/invalid total time or begin/end time."; }
			else if(code=="N444") { return "Alert: This facility has not filed the Election for High Cost Outlier form with the Division of Workers' Compensation."; }
			else if(code=="N445") { return "Missing document for actual cost or paid amount."; }
			else if(code=="N446") { return "Incomplete/invalid document for actual cost or paid amount."; }
			else if(code=="N447") { return "Payment is based on a generic equivalent as required documentation was not provided."; }
			else if(code=="N448") { return "This drug/service/supply is not included in the fee schedule or contracted/legislated fee arrangement"; }
			else if(code=="N449") { return "Payment based on a comparable drug/service/supply."; }
			else if(code=="N450") { return "Covered only when performed by the primary treating physician or the designee."; }
			else if(code=="N451") { return "Missing Admission Summary Report."; }
			else if(code=="N452") { return "Incomplete/invalid Admission Summary Report."; }
			else if(code=="N453") { return "Missing Consultation Report."; }
			else if(code=="N454") { return "Incomplete/invalid Consultation Report."; }
			else if(code=="N455") { return "Missing Physician Order."; }
			else if(code=="N456") { return "Incomplete/invalid Physician Order."; }
			else if(code=="N457") { return "Missing Diagnostic Report."; }
			else if(code=="N458") { return "Incomplete/invalid Diagnostic Report."; }
			else if(code=="N459") { return "Missing Discharge Summary."; }
			else if(code=="N460") { return "Incomplete/invalid Discharge Summary."; }
			else if(code=="N461") { return "Missing Nursing Notes."; }
			else if(code=="N462") { return "Incomplete/invalid Nursing Notes."; }
			else if(code=="N463") { return "Missing support data for claim."; }
			else if(code=="N464") { return "Incomplete/invalid support data for claim."; }
			else if(code=="N465") { return "Missing Physical Therapy Notes/Report."; }
			else if(code=="N466") { return "Incomplete/invalid Physical Therapy Notes/Report."; }
			else if(code=="N467") { return "Missing Report of Tests and Analysis Report."; }
			else if(code=="N468") { return "Incomplete/invalid Report of Tests and Analysis Report."; }
			else if(code=="N469") { return "Alert: Claim/Service(s) subject to appeal process, see section 935 of Medicare Prescription Drug, Improvement, and Modernization Act of 2003 (MMA)."; }
			else if(code=="N470") { return "This payment will complete the mandatory medical reimbursement limit."; }
			else if(code=="N471") { return "Missing/incomplete/invalid HIPPS Rate Code."; }
			else if(code=="N472") { return "Payment for this service has been issued to another provider."; }
			else if(code=="N473") { return "Missing certification."; }
			else if(code=="N474") { return "Incomplete/invalid certification"; }
			else if(code=="N475") { return "Missing completed referral form."; }
			else if(code=="N476") { return "Incomplete/invalid completed referral form"; }
			else if(code=="N477") { return "Missing Dental Models."; }
			else if(code=="N478") { return "Incomplete/invalid Dental Models"; }
			else if(code=="N479") { return "Missing Explanation of Benefits (Coordination of Benefits or Medicare Secondary Payer)."; }
			else if(code=="N480") { return "Incomplete/invalid Explanation of Benefits (Coordination of Benefits or Medicare Secondary Payer)."; }
			else if(code=="N481") { return "Missing Models."; }
			else if(code=="N482") { return "Incomplete/invalid Models"; }
			else if(code=="N483") { return "Missing Periodontal Charts."; }
			else if(code=="N484") { return "Incomplete/invalid Periodontal Charts"; }
			else if(code=="N485") { return "Missing Physical Therapy Certification."; }
			else if(code=="N486") { return "Incomplete/invalid Physical Therapy Certification."; }
			else if(code=="N487") { return "Missing Prosthetics or Orthotics Certification."; }
			else if(code=="N488") { return "Incomplete/invalid Prosthetics or Orthotics Certification"; }
			else if(code=="N489") { return "Missing referral form."; }
			else if(code=="N490") { return "Incomplete/invalid referral form"; }
			else if(code=="N491") { return "Missing/Incomplete/Invalid Exclusionary Rider Condition."; }
			else if(code=="N492") { return "Alert: A network provider may bill the member for this service if the member requested the service and agreed in writing, prior to receiving the service, to be financially responsible for the billed charge."; }
			else if(code=="N493") { return "Missing Doctor First Report of Injury."; }
			else if(code=="N494") { return "Incomplete/invalid Doctor First Report of Injury."; }
			else if(code=="N495") { return "Missing Supplemental Medical Report."; }
			else if(code=="N496") { return "Incomplete/invalid Supplemental Medical Report."; }
			else if(code=="N497") { return "Missing Medical Permanent Impairment or Disability Report."; }
			else if(code=="N498") { return "Incomplete/invalid Medical Permanent Impairment or Disability Report."; }
			else if(code=="N499") { return "Missing Medical Legal Report."; }
			else if(code=="N500") { return "Incomplete/invalid Medical Legal Report."; }
			else if(code=="N501") { return "Missing Vocational Report."; }
			else if(code=="N502") { return "Incomplete/invalid Vocational Report."; }
			else if(code=="N503") { return "Missing Work Status Report."; }
			else if(code=="N504") { return "Incomplete/invalid Work Status Report."; }
			else if(code=="N505") { return "Alert: This response includes only services that could be estimated in real time. No estimate will be provided for the services that could not be estimated in real time."; }
			else if(code=="N506") { return "Alert: This is an estimate of the member’s liability based on the information available at the time the estimate was processed. Actual coverage and member liability amounts will be determined when the claim is processed. This is not a pre-authorization or a guarantee of payment."; }
			else if(code=="N507") { return "Plan distance requirements have not been met."; }
			else if(code=="N508") { return "Alert: This real time claim adjudication response represents the member responsibility to the provider for services reported. The member will receive an Explanation of Benefits electronically or in the mail. Contact the insurer if there are any questions."; }
			else if(code=="N509") { return "Alert: A current inquiry shows the member’s Consumer Spending Account contains sufficient funds to cover the member liability for this claim/service. Actual payment from the Consumer Spending Account will depend on the availability of funds and determination of eligible services at the time of payment processing."; }
			else if(code=="N510") { return "Alert: A current inquiry shows the member’s Consumer Spending Account does not contain sufficient funds to cover the member's liability for this claim/service. Actual payment from the Consumer Spending Account will depend on the availability of funds and determination of eligible services at the time of payment processing."; }
			else if(code=="N511") { return "Alert: Information on the availability of Consumer Spending Account funds to cover the member liability on this claim/service is not available at this time."; }
			else if(code=="N512") { return "Alert: This is the initial remit of a non-NCPDP claim originally submitted real-time without change to the adjudication."; }
			else if(code=="N513") { return "Alert: This is the initial remit of a non-NCPDP claim originally submitted real-time with a change to the adjudication."; }
			else if(code=="N514") { return "Consult plan benefit documents/guidelines for information about restrictions for this service."; }
			else if(code=="N515") { return "Alert: Submit this claim to the patient's other insurer for potential payment of supplemental benefits. We did not forward the claim information. (use N387 instead)"; }
			else if(code=="N516") { return "Records indicate a mismatch between the submitted NPI and EIN."; }
			else if(code=="N517") { return "Resubmit a new claim with the requested information."; }
			else if(code=="N518") { return "No separate payment for accessories when furnished for use with oxygen equipment."; }
			else if(code=="N519") { return "Invalid combination of HCPCS modifiers."; }
			else if(code=="N520") { return "Alert: Payment made from a Consumer Spending Account."; }
			else if(code=="N521") { return "Mismatch between the submitted provider information and the provider information stored in our system."; }
			else if(code=="N522") { return "Duplicate of a claim processed, or to be processed, as a crossover claim."; }
			else if(code=="N523") { return "The limitation on outlier payments defined by this payer for this service period has been met. The outlier payment otherwise applicable to this claim has not been paid."; }
			else if(code=="N524") { return "Based on policy this payment constitutes payment in full."; }
			else if(code=="N525") { return "These services are not covered when performed within the global period of another service."; }
			else if(code=="N526") { return "Not qualified for recovery based on employer size."; }
			else if(code=="N527") { return "We processed this claim as the primary payer prior to receiving the recovery demand."; }
			else if(code=="N528") { return "Patient is entitled to benefits for Institutional Services only."; }
			else if(code=="N529") { return "Patient is entitled to benefits for Professional Services only."; }
			else if(code=="N530") { return "Not Qualified for Recovery based on enrollment information."; }
			else if(code=="N531") { return "Not qualified for recovery based on direct payment of premium."; }
			else if(code=="N532") { return "Not qualified for recovery based on disability and working status."; }
			else if(code=="N533") { return "Services performed in an Indian Health Services facility under a self-insured tribal Group Health Plan."; }
			else if(code=="N534") { return "This is an individual policy, the employer does not participate in plan sponsorship."; }
			else if(code=="N535") { return "Payment is adjusted when procedure is performed in this place of service based on the submitted procedure code and place of service."; }
			else if(code=="N536") { return "We are not changing the prior payer's determination of patient portion, which you may collect, as this service is not covered by us."; }
			else if(code=="N537") { return "We have examined claims history and no records of the services have been found."; }
			else if(code=="N538") { return "A facility is responsible for payment to outside providers who furnish these services/supplies/drugs to its patients/residents."; }
			else if(code=="N539") { return "Alert: We processed appeals/waiver requests on your behalf and that request has been denied."; }
			else if(code=="N540") { return "Payment adjusted based on the interrupted stay policy."; }
			else if(code=="N541") { return "Mismatch between the submitted insurance type code and the information stored in our system."; }
			else if(code=="N542") { return "Missing income verification."; }
			else if(code=="N543") { return "Incomplete/invalid income verification"; }
			else if(code=="N544") { return "Alert: Although this was paid, you have billed with a referring/ordering provider that does not match our system record. Unless, corrected, this will not be paid in the future."; }
			else if(code=="N545") { return "Payment reduced based on status as an unsuccessful eprescriber per the Electronic Prescribing (eRx) Incentive Program."; }
			else if(code=="N546") { return "Payment represents a previous reduction based on the Electronic Prescribing (eRx) Incentive Program."; }
			else if(code=="N547") { return "A refund request (Frequency Type Code 8) was processed previously."; }
			else if(code=="N548") { return "Alert: Patient's calendar year deductible has been met."; }
			else if(code=="N549") { return "Alert: Patient's calendar year out-of-pocket maximum has been met."; }
			else if(code=="N550") { return "Alert: You have not responded to requests to revalidate your provider/supplier enrollment information. Your failure to revalidate your enrollment information will result in a payment hold in the near future."; }
			else if(code=="N551") { return "Payment adjusted based on the Ambulatory Surgical Center (ASC) Quality Reporting Program."; }
			else if(code=="N552") { return "Payment adjusted to reverse a previous withhold/bonus amount."; }
			else if(code=="N553") { return "Payment adjusted based on a Low Income Subsidy (LIS) retroactive coverage or status change."; }
			else if(code=="N554") { return "Missing/Incomplete/Invalid Family Planning Indicator"; }
			else if(code=="N555") { return "Missing medication list."; }
			else if(code=="N556") { return "Incomplete/invalid medication list."; }
			else if(code=="N557") { return "This claim/service is not payable under our service area. The claim must be filed to the Payer/Plan in whose service area the specimen was collected."; }
			else if(code=="N558") { return "This claim/service is not payable under our service area. The claim must be filed to the Payer/Plan in whose service area the equipment was received."; }
			else if(code=="N559") { return "This claim/service is not payable under our service area. The claim must be filed to the Payer/Plan in whose service area the Ordering Physician is located."; }
			else if(code=="N560") { return "The pilot program requires an interim or final claim within 60 days of the Notice of Admission. A claim was not received."; }
			else if(code=="N561") { return "The bundled claim originally submitted for this episode of care includes related readmissions. You may resubmit the original claim to receive a corrected payment based on this readmission."; }
			else if(code=="N562") { return "The provider number of your incoming claim does not match the provider number on the processed Notice of Admission (NOA) for this bundled payment."; }
			else if(code=="N563") { return "Missing required provider/supplier issuance of advance patient notice of non-coverage. The patient is not liable for payment for this service."; }
			else if(code=="N564") { return "Patient did not meet the inclusion criteria for the demonstration project or pilot program."; }
			else if(code=="N565") { return "Alert: This non-payable reporting code requires a modifier. Future claims containing this non-payable reporting code must include an appropriate modifier for the claim to be processed."; }
			else if(code=="N566") { return "Alert: This procedure code requires functional reporting. Future claims containing this procedure code must include an applicable non-payable code and appropriate modifiers for the claim to be processed."; }
			else if(code=="N567") { return "Not covered when considered preventative."; }
			else if(code=="N568") { return "Alert: Initial payment based on the Notice of Admission (NOA) under the Bundled Payment Model IV initiative."; }
			else if(code=="N569") { return "Not covered when performed for the reported diagnosis."; }
			else if(code=="N570") { return "Missing/incomplete/invalid credentialing data"; }
			else if(code=="N571") { return "Alert: Payment will be issued quarterly by another payer/contractor."; }
			else if(code=="N572") { return "This procedure is not payable unless non-payable reporting codes and appropriate modifiers are submitted."; }
			else if(code=="N573") { return "Alert: You have been overpaid and must refund the overpayment. The refund will be requested separately by another payer/contractor."; }
			else if(code=="N574") { return "Our records indicate the ordering/referring provider is of a type/specialty that cannot order or refer. Please verify that the claim ordering/referring provider information is accurate or contact the ordering/referring provider."; }
			else if(code=="N575") { return "Mismatch between the submitted ordering/referring provider name and the ordering/referring provider name stored in our records."; }
			else if(code=="N576") { return "Services not related to the specific incident/claim/accident/loss being reported."; }
			else if(code=="N577") { return "Personal Injury Protection (PIP) Coverage."; }
			else if(code=="N578") { return "Coverages do not apply to this loss."; }
			else if(code=="N579") { return "Medical Payments Coverage (MPC)."; }
			else if(code=="N580") { return "Determination based on the provisions of the insurance policy."; }
			else if(code=="N581") { return "Investigation of coverage eligibility is pending."; }
			else if(code=="N582") { return "Benefits suspended pending the patient's cooperation."; }
			else if(code=="N583") { return "Patient was not an occupant of our insured vehicle and therefore, is not an eligible injured person."; }
			else if(code=="N584") { return "Not covered based on the insured's noncompliance with policy or statutory conditions."; }
			else if(code=="N585") { return "Benefits are no longer available based on a final injury settlement."; }
			else if(code=="N586") { return "The injured party does not qualify for benefits."; }
			else if(code=="N587") { return "Policy benefits have been exhausted."; }
			else if(code=="N588") { return "The patient has instructed that medical claims/bills are not to be paid."; }
			else if(code=="N589") { return "Coverage is excluded to any person injured as a result of operating a motor vehicle while in an intoxicated condition or while the ability to operate such a vehicle is impaired by the use of a drug."; }
			else if(code=="N590") { return "Missing independent medical exam detailing the cause of injuries sustained and medical necessity of services rendered."; }
			else if(code=="N591") { return "Payment based on an Independent Medical Examination (IME) or Utilization Review (UR)."; }
			else if(code=="N592") { return "Adjusted because this is not the initial prescription or exceeds the amount allowed for the initial prescription."; }
			else if(code=="N593") { return "Not covered based on failure to attend a scheduled Independent Medical Exam (IME)."; }
			else if(code=="N594") { return "Records reflect the injured party did not complete an Application for Benefits for this loss."; }
			else if(code=="N595") { return "Records reflect the injured party did not complete an Assignment of Benefits for this loss."; }
			else if(code=="N596") { return "Records reflect the injured party did not complete a Medical Authorization for this loss."; }
			else if(code=="N597") { return "Adjusted based on a medical/dental provider's apportionment of care between related injuries and other unrelated medical/dental conditions/injuries."; }
			else if(code=="N598") { return "Health care policy coverage is primary."; }
			else if(code=="N599") { return "Our payment for this service is based upon a reasonable amount pursuant to both the terms and conditions of the policy of insurance under which the subject claim is being made as well as the Florida No-Fault Statute, which permits, when determining a reasonable charge for a service, an insurer to consider usual and customary charges and payments accepted by the provider, reimbursement levels in the community and various federal and state fee schedules applicable to automobile and other insurance coverages, and other information relevant to the reasonableness of the reimbursement for the service. The payment for this service is based upon 200% of the Participating Level of Medicare Part B fee schedule for the locale in which the services were rendered."; }
			else if(code=="N600") { return "Adjusted based on the applicable fee schedule for the region in which the service was rendered."; }
			else if(code=="N601") { return "In accordance with Hawaii Administrative Rules, Title 16, Chapter 23 Motor Vehicle Insurance Law payment is recommended based on Medicare Resource Based Relative Value Scale System applicable to Hawaii."; }
			else if(code=="N602") { return "Adjusted based on the Redbook maximum allowance."; }
			else if(code=="N603") { return "This fee is calculated according to the New Jersey medical fee schedules for Automobile Personal Injury Protection and Motor Bus Medical Expense Insurance Coverage."; }
			else if(code=="N604") { return "In accordance with New York No-Fault Law, Regulation 68, this base fee was calculated according to the New York Workers' Compensation Board Schedule of Medical Fees, pursuant to Regulation 83 and / or Appendix 17-C of 11 NYCRR."; }
			else if(code=="N605") { return "This fee was calculated based upon New York All Patients Refined Diagnosis Related Groups (APR-DRG), pursuant to Regulation 68."; }
			else if(code=="N606") { return "The Oregon allowed amount for this procedure is based upon the Workers Compensation Fee Schedule (OAR 436-009). The allowed amount has been calculated in accordance with Section 4 of ORS 742.524."; }
			else if(code=="N607") { return "Service provided for non-compensable condition(s)."; }
			else if(code=="N608") { return "The fee schedule amount allowed is calculated at 110% of the Medicare Fee Schedule for this region, specialty and type of service. This fee is calculated in compliance with Act 6."; }
			else if(code=="N609") { return "80% of the providers billed amount is being recommended for payment according to Act 6."; }
			else if(code=="N610") { return "Alert: Payment based on an appropriate level of care."; }
			else if(code=="N611") { return "Claim in litigation. Contact insurer for more information."; }
			else if(code=="N612") { return "Medical provider not authorized/certified to provide treatment to injured workers in this jurisdiction."; }
			else if(code=="N613") { return "Alert: Although this was paid, you have billed with an ordering provider that needs to update their enrollment record. Please verify that the ordering provider information you submitted on the claim is accurate and if it is, contact the ordering provider instructing them to update their enrollment record. Unless corrected, a claim with this ordering provider will not be paid in the future."; }
			else if(code=="N614") { return "Alert: Additional information is included in the 835 Healthcare Policy Identification Segment (loop 2110 Service Payment Information)."; }
			else if(code=="N615") { return "Alert: This enrollee receiving advance payments of the premium tax credit is in the grace period of three consecutive months for non-payment of premium. Under the Code of Federal Regulations, Title 45, Part 156.270, a Qualified Health Plan issuer must pay all appropriate claims for services rendered to the enrollee during the first month of the grace period and may pend claims for services rendered to the enrollee in the second and third months of the grace period."; }
			else if(code=="N616") { return "Alert: This enrollee is in the first month of the advance premium tax credit grace period."; }
			else if(code=="N617") { return "This enrollee is in the second or third month of the advance premium tax credit grace period."; }
			else if(code=="N618") { return "Alert: This claim will automatically be reprocessed if the enrollee pays their premiums."; }
			else if(code=="N619") { return "Coverage terminated for non-payment of premium."; }
			else if(code=="N620") { return "Alert: This procedure code is for quality reporting/informational purposes only."; }
			else if(code=="N621") { return "Charges for Jurisdiction required forms, reports, or chart notes are not payable."; }
			else if(code=="N622") { return "Not covered based on the date of injury/accident."; }
			else if(code=="N623") { return "Not covered when deemed unscientific/unproven/outmoded/experimental/excessive/inappropriate."; }
			else if(code=="N624") { return "The associated Workers' Compensation claim has been withdrawn."; }
			else if(code=="N625") { return "Missing/Incomplete/Invalid Workers' Compensation Claim Number."; }
			else if(code=="N626") { return "New or established patient E/M codes are not payable with chiropractic care codes."; }
			else if(code=="N627") { return "Service not payable per managed care contract."; }
			else if(code=="N628") { return "Out-patient follow up visits on the same date of service as a scheduled test or treatment is disallowed."; }
			else if(code=="N629") { return "Reviews/documentation/notes/summaries/reports/charts not requested."; }
			else if(code=="N630") { return "Referral not authorized by attending physician."; }
			else if(code=="N631") { return "Medical Fee Schedule does not list this code. An allowance was made for a comparable service."; }
			else if(code=="N632") { return "According to the Official Medical Fee Schedule this service has a relative value of zero and therefore no payment is due."; }
			else if(code=="N633") { return "Additional anesthesia time units are not allowed."; }
			else if(code=="N634") { return "The allowance is calculated based on anesthesia time units."; }
			else if(code=="N635") { return "The Allowance is calculated based on the anesthesia base units plus time."; }
			else if(code=="N636") { return "Adjusted because this is reimbursable only once per injury."; }
			else if(code=="N637") { return "Consultations are not allowed once treatment has been rendered by the same provider."; }
			else if(code=="N638") { return "Reimbursement has been made according to the home health fee schedule."; }
			else if(code=="N639") { return "Reimbursement has been made according to the inpatient rehabilitation facilities fee schedule."; }
			else if(code=="N640") { return "Exceeds number/frequency approved/allowed within time period."; }
			else if(code=="N641") { return "Reimbursement has been based on the number of body areas rated."; }
			else if(code=="N642") { return "Adjusted when billed as individual tests instead of as a panel."; }
			else if(code=="N643") { return "The services billed are considered Not Covered or Non-Covered (NC) in the applicable state fee schedule."; }
			else if(code=="N644") { return "Reimbursement has been made according to the bilateral procedure rule."; }
			else if(code=="N645") { return "Mark-up allowance"; }
			else if(code=="N646") { return "Reimbursement has been adjusted based on the guidelines for an assistant."; }
			else if(code=="N647") { return "Adjusted based on diagnosis-related group (DRG)."; }
			else if(code=="N648") { return "Adjusted based on Stop Loss."; }
			else if(code=="N649") { return "Payment based on invoice."; }
			else if(code=="N650") { return "This policy was not in effect for this date of loss. No coverage is available."; }
			else if(code=="N651") { return "No Personal Injury Protection/Medical Payments Coverage on the policy at the time of the loss."; }
			else if(code=="N652") { return "The date of service is before the date of loss."; }
			else if(code=="N653") { return "The date of injury does not match the reported date of loss."; }
			else if(code=="N654") { return "Adjusted based on achievement of maximum medical improvement (MMI)."; }
			else if(code=="N655") { return "Payment based on provider's geographic region."; }
			else if(code=="N656") { return "An interest payment is being made because benefits are being paid outside the statutory requirement."; }
			else if(code=="N657") { return "This should be billed with the appropriate code for these services."; }
			else if(code=="N658") { return "The billed service(s) are not considered medical expenses."; }
			else if(code=="N659") { return "This item is exempt from sales tax."; }
			else if(code=="N660") { return "Sales tax has been included in the reimbursement."; }
			else if(code=="N661") { return "Documentation does not support that the services rendered were medically necessary."; }
			else if(code=="N662") { return "Alert: Consideration of payment will be made upon receipt of a final bill."; }
			else if(code=="N663") { return "Adjusted based on an agreed amount."; }
			else if(code=="N664") { return "Adjusted based on a legal settlement."; }
			else if(code=="N665") { return "Services by an unlicensed provider are not reimbursable."; }
			else if(code=="N666") { return "Only one evaluation and management code at this service level is covered during the course of care."; }
			else if(code=="N667") { return "Missing prescription"; }
			else if(code=="N668") { return "Incomplete/invalid prescription"; }
			else if(code=="N669") { return "Adjusted based on the Medicare fee schedule."; }
			else if(code=="N670") { return "This service code has been identified as the primary procedure code subject to the Medicare Multiple Procedure Payment Reduction (MPPR) rule."; }
			else if(code=="N671") { return "Payment based on a jurisdiction cost-charge ratio."; }
			else if(code=="N672") { return "Alert: Amount applied to Health Insurance Offset."; }
			else if(code=="N673") { return "Reimbursement has been calculated based on an outpatient per diem or an outpatient factor and/or fee schedule amount."; }
			else if(code=="N674") { return "Not covered unless a pre-requisite procedure/service has been provided."; }
			else if(code=="N675") { return "Additional information is required from the injured party."; }
			else if(code=="N676") { return "Service does not qualify for payment under the Outpatient Facility Fee Schedule."; }
			else if(code=="N677") { return "Alert: Films/Images will not be returned."; }
			else if(code=="N678") { return "Missing post-operative images/visual field results."; }
			else if(code=="N679") { return "Incomplete/Invalid post-operative images/visual field results."; }
			else if(code=="N680") { return "Missing/Incomplete/Invalid date of previous dental extractions."; }
			else if(code=="N681") { return "Missing/Incomplete/Invalid full arch series."; }
			else if(code=="N682") { return "Missing/Incomplete/Invalid history of prior periodontal therapy/maintenance."; }
			else if(code=="N683") { return "Missing/Incomplete/Invalid prior treatment documentation."; }
			else if(code=="N684") { return "Payment denied as this is a specialty claim submitted as a general claim."; }
			else if(code=="N685") { return "Missing/Incomplete/Invalid Prosthesis, Crown or Inlay Code."; }
			else if(code=="N686") { return "Missing/incomplete/Invalid questionnaire needed to complete payment determination."; }
			else if(code=="N687") { return "Alert: This reversal is due to a retroactive disenrollment. (Note: To be used with claim/service reversal)"; }
			else if(code=="N688") { return "Alert: This reversal is due to a medical or utilization review decision. (Note: To be used with claim/service reversal)"; }
			else if(code=="N689") { return "Alert: This reversal is due to a retroactive rate change. (Note: To be used with claim/service reversal)"; }
			else if(code=="N690") { return "Alert: This reversal is due to a provider submitted appeal. (Note: To be used with claim/service reversal)"; }
			else if(code=="N691") { return "Alert: This reversal is due to a patient submitted appeal. (Note: To be used with claim/service reversal)"; }
			else if(code=="N692") { return "Alert: This reversal is due to an incorrect rate on the initial adjudication. (Note: To be used with claim/service reversal)"; }
			else if(code=="N693") { return "Alert: This reversal is due to a cancelation of the claim by the provider."; }
			else if(code=="N694") { return "Alert: This reversal is due to a resubmission/change to the claim by the provider."; }
			else if(code=="N695") { return "Alert: This reversal is due to incorrect patient financial portion information on the initial adjudication."; }
			else if(code=="N696") { return "Alert: This reversal is due to a Coordination of Benefits or Third Party Liability Recovery retroactive adjustment. (Note: To be used with claim/service reversal)"; }
			else if(code=="N697") { return "Alert: This reversal is due to a payer's retroactive contract incentive program adjustment. (Note: To be used with claim/service reversal)"; }
			else if(code=="N698") { return "Alert: This reversal is due to non-payment of the Health Insurance Exchange premiums by the end of the premium payment grace period, resulting in loss of coverage. (Note: To be used with claim/service reversal)"; }
			return "Remark code: "+code;//catch all.  The user can look up the code manually in this case.
		}

		#endregion Code To Description

	}

	public enum X835Status {
		///<summary>Just a place holder if there is an issue.  Should never show in UI.</summary>
		None,
		///<summary>There are no received claims attached to the ERA.  There can be one or more detached claims on the ERA.</summary>
		Unprocessed,
		///<summary>Some claims for this ERA have had financial information entered, no finalaized claim payment.</summary>
		Partial,
		///<summary>Ignores manually detached.  All claims for this ERA have had financial information entered, no finalaized claim payment.</summary>
		NotFinalized,
		///<summary>Some claims have been manually detached but all other claims have had financial information entered and finalaized claim payment created.</summary>
		[Description("Finalized*")]
		FinalizedSomeDetached,
		///<summary>All claims have been manually detached.</summary>
		[Description("Finalized*")]
		FinalizedAllDetached,
		///<summary>All claims for this ERA have had financial information entered and a finalaized claim payment was created.</summary>
		Finalized,
	}

	public class X835ClaimData {
		///<summary>Typically this is a valid internal db claim or a manually detached claim with a claimNum of 0.  Can be null.
		///When null then Era claim could not be matched.  When claimNum is 0 then this claim was manually detached.</summary>
		public Hx835_ShortClaim AttachedClaim;
		///<summary>True if AttachedClaim has a finalized insurance payment, otherwise false.</summary>
		public bool HasPayment;
		///<summary>List of ERA claims that the AttachedClaim was matched to if any.</summary>
		public List<Hx835_Claim> ListMatched835Claims=new List<Hx835_Claim>();
		
		///<summary>Returns true if AttachedClaim.ClaimNum is 0, otherwise false.</summary>
		public bool IsManuallyDetached {
			get { return (AttachedClaim!=null && AttachedClaim.ClaimNum==0); }
		}
		///<summary>Returns true if AttachedClaim.ClaimNum is not 0 and AttachedClaim.ClaimStatus is 'R', otherwise false.</summary>
		public bool IsClaimReceived {
			get { return (AttachedClaim!=null && AttachedClaim.ClaimNum!=0 && AttachedClaim.ClaimStatus=="R"); }
		}

		public X835ClaimData(Hx835_ShortClaim attachedClaim,bool hasPayment,List<Hx835_Claim> listMatched835Claims) {
			AttachedClaim=attachedClaim;
			HasPayment=hasPayment;
			ListMatched835Claims=listMatched835Claims;
		}
		
		///<summary>Returns true if there is a Hx835_Claim that can be matched to AttachedClaim for the given etransNum, otherwise false.
		///Sets matchEraClaim to first matched value when true.</summary>
		public bool TryGetEraClaim(long etransNum,out Hx835_Claim matchEraClaim) {
			matchEraClaim=ListMatched835Claims.FirstOrDefault(x => x.Era.EtransSource.EtransNum==etransNum);
			return(matchEraClaim!=null);
		}

	}

	#region Helper Classes

	//Each class in this region is preceeded with an H to help identify the class as a helper class.
	//Immediately following the H, all classes also include the string x835 in their name, so it is easy to identify what purpose they are helpers for.

	///<summary>Provider level adjustment corresponding to a PLB segment.</summary>
	public class Hx835_ProvAdj {
		///<summary>PLB01</summary>
		public string Npi;
		///<summary>PLB02</summary>
		public DateTime DateFiscalPeriod;
		///<summary>Description of the ReasonCode in human readable terms</summary>
		public string ReasonCodeDescript;
		///<summary>PLB03-1 or PLB05-1 or PLB07-1 or PLB09-1 or PLB11-1 or PLB13-1</summary>
		public string ReasonCode;
		///<summary>PLB03-2 or PLB05-2 or PLB07-2 or PLB09-2 or PLB11-2 or PLB13-2</summary>
		public string RefIdentification;
		///<summary>PLB04</summary>
		public decimal AdjAmt;
		
		public Hx835_ProvAdj Copy(){
			return (Hx835_ProvAdj)this.MemberwiseClone();
		}
	}

	///<summary>Information about a single EOB.  There can be many EOBs within a single 835.</summary>
	public class Hx835_Claim {
		///<summary>The ERA object that this ERA claim belongs to.</summary>
		public X835 Era;
		///<summary>The segment index of the first line where the CLP segment is defined within the X12 document.
		///This index is unique, even if there are multiple 835 transactions within the X12 document.</summary>
		public int ClpSegmentIndex;
		///<summary>The number of X12 segments that this claim and all data within it span.</summary>
		public int SegmentCount;
		///<summary>TS301.  Situational.</summary>
		public string Npi;
		///<summary>CLP01 in loop 2100.  Referred to in this format as a Patient Control Number.
		///The claim tracking numbers correspond to CLM01 exactly as submitted in the 837.
		///We refer to CLM01 as the claim identifier on our end. We allow alphanumeric in our claim identifiers, so we must store as a string.</summary>
		public string ClaimTrackingNumber;
		///<summary>CLP07 The claim control number used to identify the claim in the insurance company's database.</summary>
		public string PayerControlNumber;
		///<summary>Indicates Primary, Secondary or Preauth.  The reported CLP02 code used to set StatusCodeDescript below.</summary>
		public string CodeClp02;
		///<summary>CLP02 A human readable copy of the claim status code.</summary>
		public string StatusCodeDescript;
		///<summary>CLP03 The total amount charged by the dentist for the claim.</summary>
		public decimal ClaimFee;
		///<summary>CLP04 The total amount insurance paid.</summary>
		public decimal InsPaid;
		///<summary>CLP05 A portion of the ChargeAmtTotal which the patient is responsible for.</summary>
		public decimal PatientRespAmt;
		///<summary>Patient portion for this claim.
		///This is an amount calculated as the sum of the procedure PatientPortionAmts and claim level patient portion adjustments.</summary>
		public decimal PatientPortionAmt;
		///<summary>Patient deductible total for this claim.
		///This is an amount calculated as the sum of the procedure DeductibleAmts and claim level deductible adjustments.</summary>
		public decimal PatientDeductAmt;
		///<summary>Patient writeoff total for this claim.
		///This is an amount calculated as the sum of the procedure Writeoffs and claim level writeoff adjustments.</summary>
		public decimal WriteoffAmt;
		///<summary>NM1*QC of loop 2100.  Required for Dental and Medical.  Optional for Pharmacy.  For our purposes (Dental and Medical), this data will always be provided.</summary>
		public Hx835_Name PatientName;
		///<summary>NM1*IL of loop 2100.  Required for Dental and Medical.  Optional for Pharmacy.  For our purposes (Dental and Medical), this data will always be provided.</summary>
		public Hx835_Name SubscriberName;
		///<summary>DTM*232 of loop 2100.  Situational, but if not present, then service lines will include service dates.  Service line dates override this date when both are present.</summary>
		public DateTime DateServiceStart;
		///<summary>DTM*233 of loop 2100.  Situational, but if not present, then service lines will include service dates.  Service line dates override this date when both are present.</summary>
		public DateTime DateServiceEnd;
		///<summary>DTM*050 of loop 2100.  Situational.  Will be set to 0001/01/01 if not present for this claim.</summary>
		public DateTime DatePayerReceived;
		///<summary>CAS Adjustments made by the insurance company at the claim level.  These adjustments help explain part of the amount difference between the claim fee and the amount paid.
		///There are also adjustments at the procedure level and the patient portion to account for when balancing.</summary>
		public List<Hx835_Adj> ListClaimAdjustments;
		///<summary>MIA and MOA segments.</summary>
		public List<Hx835_Info> ListAdjudicationInfo;
		///<summary>AMT segments.</summary>
		public List<Hx835_Info> ListSupplementalInfo;
		///<summary>SVC segments.</summary>
		public List<Hx835_Proc> ListProcs;
		///<summary>The sum of all adjustment amounts in ListClaimAdjustments.</summary>
		public decimal ClaimAdjustmentTotal;
		///<summary>AllowedAmt = (Claim InsPaid)+(Claim PatientRespAmt)</summary>
		public decimal AllowedAmt;
		///<summary>True if remark code MA15 is used in either segment MIA or MOA (if present).  Also true if there are multiple CLP segments on
		///the same 835 containing the same ClaimTrackingNumber.  We have seen carriers represent split claims this way(ex Commonwealth of Massachussetts/EOHHS/Office of Medicaid).
		///Claim splits are intended to expedite payment when one or two procedures on the original claim are pending for an extended period of time.</summary>
		public bool IsSplitClaim;
		///<summary>The orignal ClaimNum corresponding to the claim from the 835, or 0 if not found.</summary>
		public long ClaimNum;
		///<summary>True if user has manually attached this 835 claim to a specific claim in OD,
		///or if the user has specified that this 835 claim has no claim in OD.</summary>
		public bool IsAttachedToClaim;
		///<summary>Does not correspond to a particular segment in the 835, internally created based on 835 payment method and date effective.</summary>
		public DateTime DateReceived;
		///<summary>True for preauth claims only.  Preauth claims do not have any dates of service at the claim or procedure level.</summary>
		public bool IsPreauth=false;
		///<summary>True for reversal claims only.  Reversal claims negate all monetary values (excluding patient portion) from original EOB.</summary>
		public bool IsReversal=false;
		///<summary>When IsSplitClaim is true, this list contains all other Hx835_Claims that are associated to this split claim.</summary>
		internal List<Hx835_Claim> ListOtherSplitClaims=new List<Hx835_Claim>();

		public EraClaimStatus ClaimStatus{
			get {
				if(this.IsAttachedToClaim && this.ClaimNum==0) {
					return EraClaimStatus.ManuallyDetached;
				}
				if(this.ClaimNum!=0) {
					return EraClaimStatus.Attached;
				}
				return EraClaimStatus.NotMatched;
			}
		}

		///<summary>Returns true if every Hx835_Proc can be mattched to an internal claim and all financial amounts have been entered.
		///We identify if a payment is supplemental based on if listAttaches contains an entry from a previous etrans/ERA for this.ClaimNum.
		///Also attempts to match a by totals payment if a Hx835_Proc could not be matched.</summary>
		public bool IsProcessed(List<Hx835_ShortClaimProc> listEraClaimProcs,List<Etrans835Attach> listAttaches) {
			Etrans835Attach attach=listAttaches.FirstOrDefault(x => x.EtransNum==Era.EtransSource.EtransNum && x.ClaimNum==ClaimNum && x.ClpSegmentIndex==ClpSegmentIndex);
			if(ClaimNum==0 || attach==null) {
				//Attaches are made after double clicking into the claim EOB on an ERA to process it or by auto-processing an ERA.
				return false;
			}
			//List of claimNums for this ERA which were split from this ERA to a new claim.
			List<long> listSplitClaimNums=listAttaches.Where(x => 
				x.EtransNum==this.Era.EtransSource.EtransNum//Same ERA
				&& x.ClpSegmentIndex==this.ClpSegmentIndex //Same claim
				&& x.ClaimNum!=this.ClaimNum//Different claim, this was split from the original ERA payment window
			).Select(x => x.ClaimNum).ToList();
			List<Hx835_ShortClaimProc> listClaimProcsForEraProcs=GetClaimProcsForEraProcs(listEraClaimProcs,listSplitClaimNums);
			if(listClaimProcsForEraProcs.Count==0) {
				return false;
			}
			bool isEraClaimSupplemental=GetIsSupplemental(listAttaches,listClaimProcsForEraProcs);
			List<Hx835_Proc> listProcsNotMatched=new List<Hx835_Proc>();
			foreach(Hx835_Proc proc in ListProcs) {
				Hx835_ShortClaimProc matchedClaimProc=null;
				if(!proc.TryGetMatchedClaimProc(out matchedClaimProc,listClaimProcsForEraProcs,isEraClaimSupplemental)) {
					listProcsNotMatched.Add(proc);
					continue;
				}
				listClaimProcsForEraProcs.Remove(matchedClaimProc);//So it cannot be matched twice.
			}
			if(listProcsNotMatched.Count>0) {
				double dedNotAppliedTotal=(double)listProcsNotMatched.Sum(x => x.DeductibleAmt);
				double allowedNotAppliedTotal=(double)listProcsNotMatched.Sum(x => x.AllowedAmt);
				double insPayAmtNotAppliedTotal=(double)listProcsNotMatched.Sum(x => x.InsPaid);
				double writeOffNotAppliedTotal=(double)listProcsNotMatched.Sum(x => x.WriteoffAmt);
				bool hasValidByTotal=listClaimProcsForEraProcs
					.Where(x => x.ProcNum==0)
					.Any(x => x.DedApplied==dedNotAppliedTotal && x.AllowedOverride==allowedNotAppliedTotal && 
						x.InsPayAmt==insPayAmtNotAppliedTotal && x.WriteOff==writeOffNotAppliedTotal);
				if(!hasValidByTotal) {
					return false;
				}
			}
			return true;
		}
		
		private List<Hx835_ShortClaimProc> GetClaimProcsForEraProcs(List<Hx835_ShortClaimProc> listAllClaimProcs,List<long> listSplitClaimNums) {
			List<Hx835_ShortClaimProc> listClaimProcs=new List<Hx835_ShortClaimProc>();
			foreach(Hx835_ShortClaimProc claimProc in listAllClaimProcs) {
				if(claimProc.ClaimNum==ClaimNum && ListTools.In(claimProc.Status,ClaimProcStatus.Received,ClaimProcStatus.Supplemental,ClaimProcStatus.Preauth)) {
					listClaimProcs.Add(claimProc);
				}
				else if(listSplitClaimNums.Contains(claimProc.ClaimNum) && ListTools.In(claimProc.Status,ClaimProcStatus.Received,ClaimProcStatus.Supplemental,ClaimProcStatus.NotReceived)) {
					//split claims can never be preauths and may or may not be supplemental (we are not sure but does not hurt to include).
					listClaimProcs.Add(claimProc);
				}
			}
			return listClaimProcs;
		}

		///<summary>Returns true if there is an Etrans835Attach associated to this Hx835_Claim.ClaimNum from a previously imported Etrans/ERA.
		///listAttaches should contain all attaches for a given claim num from multiple etrans.</summary>
		public bool GetIsSupplemental(List<Etrans835Attach> listAttaches,List<Hx835_ShortClaimProc> listEraClaimProcs) {
			if(this.ClaimNum==0) {
				return false;//Claim was not manually attached or automatically matched, we can not determine if it is supplemental.
			}
			else if(this.IsReversal) {
				return true;//Reversals by definitaion must be supplemental.
			}
			if(HasPreviousSupplemental(listEraClaimProcs)) {
				return true;
			}
			//If there is another etrans/ERA in the past which has the same claim attached, then we assume supplemental.
			List<Etrans835Attach> listOtherEtransAttaches=listAttaches.FindAll(x => x.EtransNum!=Era.EtransSource.EtransNum).ToList();
			if(listOtherEtransAttaches.Exists(x => x.ClaimNum==this.ClaimNum && x.DateTimeTrans<this.Era.EtransSource.DateTimeTrans)) {
				return true;
			}
			return false;
		}

		///<summary>Returns true if listEraClaimProcs contains a supplemental for this claim that was entered
		///prior to the date that this Etrans was imported/created.</summary>
		private bool HasPreviousSupplemental(List<Hx835_ShortClaimProc> listEraClaimProcs) {
			return listEraClaimProcs.Any(x => x.ClaimNum==this.ClaimNum//Same claim
				&& x.SecDateEntry<=this.Era.EtransSource.DateTimeTrans.Date//Only consider claimProcs that were created prior to this ERA.
				&& x.Status==ClaimProcStatus.Supplemental//Already has supplemental, all new claimProcs must be supplemental then.
			);
		}

		public List<Hx835_Claim> GetOtherNotDetachedSplitClaims() {
			return this.ListOtherSplitClaims.Where(x =>
				x.ClaimNum!=0 || !x.IsAttachedToClaim//Not manually detached.
			).ToList();
		}

		///<summary>Attempts to get the original claim from the database.  Returns null if not found.</summary>
		public Claim GetClaimFromDb() {
			if(ClaimNum==0) {
				return null;
			}
			return Claims.GetClaim(ClaimNum);
		}

		///<summary>There could be multiple matches for a claimproc if the procedure was split or unbundled.
		///For each claimproc in listClaimProcs, a list of Hx835_Procs will be returned.
		///If there are no matches for a claimProc, then the list corresponding to that claimProc will be an empty list (not null).</summary>
		public static List<List<Hx835_Proc>> GetPaymentsForClaimProcs(List<ClaimProc> listClaimProcs,List<Hx835_Proc> listProcsUnassigned) {
			//This logic is mimiced for split claims in FormEtrans835Edit.EnterPayment(...)
			List<List<Hx835_Proc>> retVal=new List<List<Hx835_Proc>>();
			//First locate matches by unique identifier.  There may be no match for older procedures, because we did not always send the procedure identifiers in the 837s.
			//This loop also ensures that every item of retVal contains an initialized list, although some lists may be empty.
			for(int i=0;i<listClaimProcs.Count;i++) {
				ClaimProc claimProc=listClaimProcs[i];
				List<Hx835_Proc> listProcMatches=new List<Hx835_Proc>();
				for(int j=listProcsUnassigned.Count-1;j>=0;j--) {//We go backward, so we can remove the current item without modifying j.
					if(listProcsUnassigned[j].ProcNum!=claimProc.ProcNum) {
						continue;
					}
					listProcMatches.Add(listProcsUnassigned[j]);
					//Remove the current item.  This will cause [j] to contain an item we have seen already, thus j-- at the top of the loop will get us to the next item.
					listProcsUnassigned.RemoveAt(j);
				}
				retVal.Add(listProcMatches);
			}
			//For those claimprocs for which no match was found using the unique ID, 
			//try to locate by procedure code and procedure fee.
			//Unfortunately, this would not match split procedures which have no specified ProcNum.
			for(int i=0;i<retVal.Count;i++) {
				if(retVal[i].Count>0) {
					continue;//Already matched this claimProc by ProcNum.
				}
				ClaimProc claimProc=listClaimProcs[i];
				for(int j=0;j<listProcsUnassigned.Count;j++) {
					Hx835_Proc procPaid=listProcsUnassigned[j];
					if(procPaid.ProcFee!=(decimal)claimProc.FeeBilled) {
						continue;
					}
					if(procPaid.ProcCodeBilled!=claimProc.CodeSent) {
						continue;
					}
					retVal[i].Add(procPaid);
					listProcsUnassigned.RemoveAt(j);
					break;//Only one unassigned procedure may be asssigned to a claimproc by fee and code.
				}
			}
			return retVal;
		}

		///<summary>Concats all adjustment descriptions from ListClaimAdjustments into a single string, separated by newlines.</summary>
		public string GetRemarks() {
			StringBuilder sb=new StringBuilder();
			for(int i=0;i<ListClaimAdjustments.Count;i++) {
				if(i>0) {
					sb.Append("\r\n");
				}
				Hx835_Adj adj=ListClaimAdjustments[i];
				sb.Append(adj.AdjustRemarks+" - "+adj.ReasonDescript);
			}
			return sb.ToString();
		}

		///<summary>Returns true if the first AND preferred name OR last name of the passed in patient don't match the name on this 835 claim.
		///All names are converted to lower case and spaces are removed to improve matching.</summary>
		public bool IsPatientNameMisMatched(Patient pat) {
			return (this.PatientName.Fname.ToLower().Replace(" ","")!=pat.FName.ToLower().Replace(" ","")
				&& this.PatientName.Fname.ToLower().Replace(" ","")!=pat.Preferred.ToLower().Replace(" ",""))
				|| this.PatientName.Lname.ToLower().Replace(" ","")!=pat.LName.ToLower().Replace(" ","");
		}

		public Hx835_Claim Copy(){
			Hx835_Claim claim=(Hx835_Claim)this.MemberwiseClone();
			claim.ListClaimAdjustments=this.ListClaimAdjustments.Select(x => x.Copy()).ToList();
			claim.ListAdjudicationInfo=this.ListAdjudicationInfo.Select(x => x.Copy()).ToList();
			claim.ListSupplementalInfo=this.ListSupplementalInfo.Select(x => x.Copy()).ToList();
			claim.ListProcs=this.ListProcs.Select(x => x.Copy()).ToList();
			return claim;
		}		
	}

	public enum EraClaimStatus {
		///<summary>A match to a database claim was not found.</summary>
		NotMatched,
		///<summary>User manually clicked the Detach button to ignore this ERA claim.</summary>
		ManuallyDetached,
		///<summary>This ERA claim is attached to a database claim.</summary>
		Attached
	}

	///<summary>Information about a single procedure on an EOB.  There can be many of these for each Hx835_Claim.</summary>
	public class Hx835_Proc {
		///<summary>The 835 claim which owns this 835 procedure.</summary>
		[XmlIgnore,JsonIgnore]
		public Hx835_Claim ClaimPaid;
		///<summary>The number of X12 segments that this claim and all data within it span.</summary>
		public int SegmentCount;
		///<summary>SVC1-2.  The adjudicated procedure code.  Can be different than the submitted procedure code in the case of bundling/unbundling and procedure splits.</summary>
		public string ProcCodeAdjudicated;
		///<summary>SVC2.</summary>
		public decimal ProcFee;
		///<summary>SVC3.</summary>
		public decimal InsPaid;
		///<summary>SVC6-2.  The procedure code submitted with the claim.  Helps identify the procedure the adjudication is regarding in case of bundling/unbundling and procedure splits.</summary>
		public string ProcCodeBilled;
		///<summary>DTM*150 or DTM*472 of loop 2110.  Situational.  If not present, then the procedure service start date is the same as the claim service start date.</summary>
		public DateTime DateServiceStart;
		///<summary>DTM*151 of loop 2110.  Situational.  If not present, then the procedure service end date is the same as the claim service end date.</summary>
		public DateTime DateServiceEnd;
		///<summary>REF*6R from the 837.  Latest format starts with an 'x'.  Older format starts with a 'p'.  Zero for claims older than 2015.</summary>
		public long ProcNum;
		public List<Hx835_Adj> ListProcAdjustments;
		public List<Hx835_Info> ListSupplementalInfo;
		public List<Hx835_Remark> ListRemarks;
		///<summary>The sum of all adjustment amounts in ListProcAdjustments where CAS01=PR, including but not limited to deductibles.
		///PatRespTotal=PatientPortionAmt+DeductibleAmt</summary>
		public decimal PatRespTotal;
		///<summary>The sum of all adjustment amounts in ListProcAdjustments where CAS01=PR and CAS02!=1.
		///PatientPortionAmt=PatRespTotal-DeductibleAmt</summary>
		public decimal PatientPortionAmt;
		///<summary>The sum of all adjustment amounts in ListProcAdjustments where CAS01=PR and CAS02=1.
		///DeductibleAmt=PatRespTotal-PatientPortionAmt</summary>
		public decimal DeductibleAmt;
		///<summary>The sum of all adjustment amounts in ListProcAdjustments which are not patient responsibility (where CAS01!=PR).</summary>
		public decimal WriteoffAmt;
		///<summary>AllowedAmt = (InsPay)+(PatRespTotal)</summary>
		public decimal AllowedAmt;
		///<summary>SVC2. The original ordinal of the claims insurance when the outoing 837 was sent.</summary>
		public long PlanOrdinal;
		///<summary>SVC2. The original InsPlan.PlanNum when the outgoing 837 was sent.</summary>
		public long PlanNum;
		///<summary>SVC2. Like PlanNum but only the right most characters of the InsPlan.PlanNum.
		///Number of characters depends on REF*6R outgoing length prior to getting the right most characters.</summary>
		public long PartialPlanNum;

		///<summary>Identifies the 837 REF*6R pattern that was used when sent.</summary>
		public EraProcMatchingFormat MatchingVersion {
			get {
				if(ProcNum!=0 && PlanOrdinal!=0 && PlanNum!=0) {//Like 'xProc.ProcNum/Ordinal/InsPlan.PlanNum'.  Version 3.
					return EraProcMatchingFormat.X;
				}
				else if(ProcNum!=0 && PlanOrdinal!=0 && PartialPlanNum!=0) {//Like 'yProc.ProcNum/Ordinal/partial InsPlan.PlanNum'.  Version 4.
					return EraProcMatchingFormat.Y;
				}
				else if(ProcNum!=0) {//Like 'pProc.ProcNum'.  Version 2.
					return EraProcMatchingFormat.P;
				}
				return EraProcMatchingFormat.LineNumber;
			}
		}

		///<summary>Concats all remarks in ListRemarks into a single string.</summary>
		public string GetRemarks() {
			StringBuilder sb=new StringBuilder();
			for(int i=0;i<ListProcAdjustments.Count;i++) {
				if(i>0) {
					sb.Append("\r\n");
				}
				Hx835_Adj adj=ListProcAdjustments[i];
				sb.Append(adj.AdjustRemarks+" - "+adj.ReasonDescript);
			}
			for(int i=0;i<ListRemarks.Count;i++) {
				if(sb.Length > 0) {
					sb.Append("\r\n");
				}
				sb.Append(ListRemarks[i].Value);
			}
			return sb.ToString();
		}
		
		public Hx835_Proc Copy() {
			Hx835_Proc proc=(Hx835_Proc)this.MemberwiseClone();
			proc.ListProcAdjustments=this.ListProcAdjustments.Select(x => x.Copy()).ToList();
			proc.ListSupplementalInfo=this.ListSupplementalInfo.Select(x => x.Copy()).ToList();
			proc.ListRemarks=this.ListRemarks.Select(x => x.Copy()).ToList();
			return proc;
		}
		
		///<summary>Returns true and sets out matchedClaimProc if a claimProc from listClaimProcs can be matched to this Hx835_Proc.
		///The listClaimProcs must only contain claim procs associated to this ERAs claim and claim procs for a claim split from this ERA if any.
		///IsSupplemental should only be set by Hx835_Claim.GetIsSupplemental(...).</summary>
		public bool TryGetMatchedClaimProc(out Hx835_ShortClaimProc matchedClaimProc,List<Hx835_ShortClaimProc> listClaimProcs,bool isSupplemental) {
			//Mimics proc matching in claimPaid.GetPaymentsForClaimProcs(...)
			matchedClaimProc=null;
			foreach(Hx835_ShortClaimProc claimProc in listClaimProcs) {
				if(!IsBasicMatch(claimProc,isSupplemental)) {
					continue;
				}
				//Since we passed basic matching we now need to consider if we matched to a claimProc on the claim for this ERA or a potential split claim.
				if(claimProc.ClaimNum==ClaimPaid.ClaimNum && !HasPaymentBeenEntered(claimProc)) {//Proc was not split from original ERA and payment not entered.
					continue;
				}
				//Otherwise claimProc is a split claim claimProc or is a claimProc for this ERAs claim with payment information entered.
				matchedClaimProc=claimProc;
				break;
			}
			return (matchedClaimProc!=null);
		}

		///<summary>Returns true if given claimProc can be matched to this 835 proc.
		///Either directly matches based on ProcNums or through the ProcCodeBilled, ClaimProcStatus and sometimes ProcFee.</summary>
		private bool IsBasicMatch(Hx835_ShortClaimProc claimProc,bool isSupplemental) {
			if(claimProc.ProcNum==0) {//Procedures only, no by total claimProcs
				return false;
			}
			if(this.ProcNum==claimProc.ProcNum) {//Direct match.  When PlanNum and Ordinal are set, then they have already been verified earlier.
				return true;
			}
			if(ProcCodeBilled!=claimProc.CodeSent) {//Failed direct match and code is wrong.
				return false;
			}
			//Code matches from here on.
			if(isSupplemental) {//FeeBilled is not set for supplemental claimProcs.  So only care about status matching.
				return (claimProc.Status==ClaimProcStatus.Supplemental);//Is known to be supplemental and we observe past supplemental.
			}
			//ClaimProcStatus.NotReceived is included because a claimProc could have been split from the ERA an is now on another claim.
			return ((decimal)claimProc.FeeBilled==ProcFee 
				&& ListTools.In(claimProc.Status,ClaimProcStatus.Received,ClaimProcStatus.NotReceived,ClaimProcStatus.Preauth));
		}

		private bool HasPaymentBeenEntered(Hx835_ShortClaimProc claimProc) { 
			return ((claimProc.DedApplied!=0) || (claimProc.AllowedOverride!=-1) || (claimProc.WriteOff!=0) || IsPayAmountMatch(claimProc));
		}

		///<summary>Returns true if given claimProc has a matching InsPayAmt or InsPayEst if claimProc is preauth.</summary>
		private bool IsPayAmountMatch(Hx835_ShortClaimProc claimProc) {
			double payAmt=claimProc.InsPayAmt;
			if(claimProc.Status==ClaimProcStatus.Preauth) {
				payAmt=claimProc.InsPayEst;
			}
			return ((decimal)payAmt==InsPaid);
		}

	}

	public enum EraProcMatchingFormat {
		///<summary>Default REF*6R segement when the claim sent did not include a REF*6R outgoing (value will be from outgoing claim LX01).</summary>
		LineNumber,
		///<summary>pProcNum.</summary>
		P,
		///<summary>xProcNum/Ordinal/InsPlan.PlanNum.</summary>
		X,
		///<summary>yProcNum/Ordinal/(partial InsPlan.PlanNum).  Most common when using random primary keys.
		///Like X above but with leading y and PartialPlanNum is set to right most digits of InsPlan.PlanNum.</summary>
		Y
	}

	///<summary>Corresponds to a CAS segment.  Both the claim level and procedure level include CAS segments.</summary>
	public class Hx835_Adj {
		public string AdjustRemarks;
		public string ReasonDescript;
		public decimal AdjAmt;
		///<summary>Will be one of these 4 values: CO=Contractual Obligations, PI=Payer Initiated Reduction, PR=Patient Responsibility, OA=Other Adjustment.</summary>
		public string AdjCode;
		///<summary>True when CAS01 = PR and (CAS02 or CAS05 or CAS08 or CAS11 or CAS14 or CAS17) is 1.
		///See code source 139 at http://www.wpc-edi.com/reference/codelists/healthcare/claim-adjustment-reason-codes/. 
		///The most useful values in code source 139 are: 1=Deductible Amount, 2=Coinsurance Amount, 3=Co-payment Amount.</summary>
		public bool IsDeductible;
		public List<string> ListClaimAdjReasonCodes=new List<string>();

		public Hx835_Adj Copy() {
			return (Hx835_Adj)this.MemberwiseClone();
		}
	}

	///<summary>The values loaded into this class come from multiple segments, including the MIA, MOA, and AMT segments.</summary>
	public class Hx835_Info {
		public string FieldName;
		///<summary>The user display value of FieldValueRaw.</summary>
		public string FieldValue;
		///<summary>For logic, not for display.</summary>
		public string FieldValueRaw;
		///<summary>True if is a claim payment remark code.</summary>
		public bool IsRemarkCode;

		public Hx835_Info Copy() {
			return (Hx835_Info)this.MemberwiseClone();
		}
	}

	public class Hx835_Remark {
		public string Code;
		public string Value;

		///<summary>This override is never explicitly used.  For serialization.</summary>
		public Hx835_Remark() {
			
		}

		public Hx835_Remark(string code,string value) {
			Code=code;
			Value=value;
		}

		public Hx835_Remark Copy() {
			return (Hx835_Remark)this.MemberwiseClone();
		}
	}

	///<summary>Corresponds to various NM1 segments.</summary>
	public class Hx835_Name {
		public string Fname;
		public string Mname;
		public string Lname;
		public string Suffix;
		public string SubscriberId;
		public string SubscriberIdTypeDesc;

		public override string ToString() {
			return ToString(true);
		}

		public string ToString(bool isIdIncluded) {
			string name=Fname;
			if(Mname!="") {
				if(name!="") {
					name+=" ";
				}
				name+=Mname;
			}
			if(Lname!="") {
				if(name!="") {
					name+=" ";
				}
				name+=Lname;
			}
			if(Suffix!="") {
				if(name!="") {
					name+=" ";
				}
				name+=Suffix;
			}
			if(isIdIncluded && SubscriberId!="") {
				if(name!="") {
					name+=" - ";
				}
				name+=SubscriberIdTypeDesc+": "+SubscriberId;
			}
			return name;
		}

	}

	///<summary>The same as a database Claim object but with only the fields needed for ERA filtering and status calculation.
	///The purpose of using this object instead of a Claim object is to save memory by using less than 10% as much as the Claim object.</summary>
	public class Hx835_ShortClaim {
		///<summary>A copy of the data from claim.ClaimNum</summary>
		public long ClaimNum;
		///<summary>A copy of the data from claim.ClinicNum</summary>
		public long ClinicNum;
		///<summary>A copy of the data from claim.ClaimStatus</summary>
		public string ClaimStatus;
		///<summary>A copy of the data from claim.PlanNum</summary>
		public long PlanNum;

		public Hx835_ShortClaim() {	}

		public Hx835_ShortClaim(Claim claim) {
			ClaimNum=claim.ClaimNum;
			ClinicNum=claim.ClinicNum;
			ClaimStatus=claim.ClaimStatus;
			PlanNum=claim.PlanNum;
		}

		///<summary>Mimics Claims.GetClaimsForClaimNums().</summary>
		public static List<Hx835_ShortClaim> GetClaimsFromClaimNums(List<long> listClaimNums) {
			if(listClaimNums.IsNullOrEmpty()) {
				return new List<Hx835_ShortClaim>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Hx835_ShortClaim>>(MethodBase.GetCurrentMethod(),listClaimNums);
			}
			string command=$"SELECT ClaimNum,ClinicNum,ClaimStatus,PlanNum FROM claim WHERE ClaimNum IN ({string.Join(",",listClaimNums)})";
			return SelectMany(command);
		}

		///<summary>Mimics ClaimCrud.SelectMany().  Gets a list of Hx835_ShortClaim objects from the database using a query.</summary>
		public static List<Hx835_ShortClaim> SelectMany(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<Hx835_ShortClaim> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Mimics ClaimCrud.TableToList().  Converts a DataTable to a list of objects.</summary>
		public static List<Hx835_ShortClaim> TableToList(DataTable table) {
			List<Hx835_ShortClaim> retVal=new List<Hx835_ShortClaim>();
			Hx835_ShortClaim claim;
			foreach(DataRow row in table.Rows) {
				claim=new Hx835_ShortClaim();
				claim.ClaimNum                      = PIn.Long  (row["ClaimNum"].ToString());
				claim.ClaimStatus                   = PIn.String(row["ClaimStatus"].ToString());
				claim.ClinicNum                     = PIn.Long  (row["ClinicNum"].ToString());
				claim.PlanNum                       = PIn.Long(row["PlanNum"].ToString());
				retVal.Add(claim);
			}
			return retVal;
		}
	}

	///<summary>The same as a database ClaimProc object but with only the fields needed for ERAs.
	///The purpose of using this object instead of a ClaimProc object is to save memory by using less than 10% as much as the ClaimProc object.</summary>
	public class Hx835_ShortClaimProc {
		///<summary>A copy of the data from claimproc.ClaimProcNum</summary>
		public long ClaimProcNum;
		///<summary>A copy of the data from claimproc.ProcNum</summary>
		public long ProcNum;
		///<summary>A copy of the data from claimproc.ClaimNum</summary>
		public long ClaimNum;
		///<summary>A copy of the data from claimproc.FeeBilled</summary>
		public double FeeBilled;
		///<summary>A copy of the data from claimproc.InsPayEst</summary>
		public double InsPayEst;
		///<summary>A copy of the data from claimproc.DedApplied</summary>
		public double DedApplied;
		///<summary>A copy of the data from claimproc.Status</summary>
		public ClaimProcStatus Status;
		///<summary>A copy of the data from claimproc.InsPayAmt</summary>
		public double InsPayAmt;
		///<summary>A copy of the data from claimproc.Writeoff</summary>
		public double WriteOff;
		///<summary>A copy of the data from claimproc.CodeSent</summary>
		public string CodeSent;
		///<summary>A copy of the data from claimproc.AllowedOverride</summary>
		public double AllowedOverride;
		///<summary>A copy of the data from claimproc.SecDateEntry</summary>
		public DateTime SecDateEntry;

		public Hx835_ShortClaimProc() { }

		public Hx835_ShortClaimProc(ClaimProc claimProc) {
			ClaimProcNum=claimProc.ClaimProcNum;
			ProcNum=claimProc.ProcNum;
			ClaimNum=claimProc.ClaimNum;
			FeeBilled=claimProc.FeeBilled;
			InsPayEst=claimProc.InsPayEst;
			DedApplied=claimProc.DedApplied;
			Status=claimProc.Status;
			InsPayAmt=claimProc.InsPayAmt;
			WriteOff=claimProc.WriteOff;
			CodeSent=claimProc.CodeSent;
			AllowedOverride=claimProc.AllowedOverride;
			SecDateEntry=claimProc.SecDateEntry;
		}

		///<summary>Mimics ClaimProcs.RefreshForClaims().  Gets a list of ClaimProcs for one claim.</summary>
		public static List<Hx835_ShortClaimProc> RefreshForClaims(List <long> listClaimNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Hx835_ShortClaimProc>>(MethodBase.GetCurrentMethod(),listClaimNums);
			}
			if(listClaimNums.Count==0) {
				return new List<Hx835_ShortClaimProc>();
			}
			List <string> listClaimNumStrs=listClaimNums.Select(x => POut.Long(x)).ToList();
			string command=
				"SELECT ClaimProcNum,ProcNum,ClaimNum,FeeBilled,InsPayEst,DedApplied,Status,InsPayAmt,WriteOff,CodeSent,AllowedOverride,SecDateEntry FROM claimproc "
				+"WHERE ClaimNum IN("+String.Join(",",listClaimNumStrs)+")";
			return SelectMany(command);
		}

		///<summary>Mimics ClaimProcCrud.SelectMany().  Gets a list of ClaimProc objects from the database using a query.</summary>
		public static List<Hx835_ShortClaimProc> SelectMany(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<Hx835_ShortClaimProc> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Mimics ClaimProcCrud.TableToList().  Converts a DataTable to a list of objects.</summary>
		public static List<Hx835_ShortClaimProc> TableToList(DataTable table) {
			List<Hx835_ShortClaimProc> retVal=new List<Hx835_ShortClaimProc>();
			Hx835_ShortClaimProc claimProc;
			foreach(DataRow row in table.Rows) {
				claimProc=new Hx835_ShortClaimProc();
				claimProc.ClaimProcNum        = PIn.Long  (row["ClaimProcNum"].ToString());
				claimProc.ProcNum             = PIn.Long  (row["ProcNum"].ToString());
				claimProc.ClaimNum            = PIn.Long  (row["ClaimNum"].ToString());
				claimProc.FeeBilled           = PIn.Double(row["FeeBilled"].ToString());
				claimProc.InsPayEst           = PIn.Double(row["InsPayEst"].ToString());
				claimProc.DedApplied          = PIn.Double(row["DedApplied"].ToString());
				claimProc.Status              = (OpenDentBusiness.ClaimProcStatus)PIn.Int(row["Status"].ToString());
				claimProc.InsPayAmt           = PIn.Double(row["InsPayAmt"].ToString());
				claimProc.WriteOff            = PIn.Double(row["WriteOff"].ToString());
				claimProc.CodeSent            = PIn.String(row["CodeSent"].ToString());
				claimProc.AllowedOverride     = PIn.Double(row["AllowedOverride"].ToString());
				claimProc.SecDateEntry        = PIn.Date  (row["SecDateEntry"].ToString());
				retVal.Add(claimProc);
			}
			return retVal;
		}
	}

	///<summary>Holds result data for automatic processing of an ERA.</summary>
	public class EraAutomationResult {
		///<summary>Used to display the Carrier name, date, and amount of transaction.</summary>
		public X835 X835Cur;
		///<summary>Used to indicate which transaction from the Etrans our data is for. In version 14.2 and older, a single Etrans could represent multiple transactions from an 835.</summary>
		public int TransactionNumber;
		///<summary>Used to display the total number of transactions from the Etrans. In version 14.2 and older, a single Etrans could represent multiple transactions from an 835.</summary>
		public int TransactionCount;
		///<summary>Number of claims that were fully and successfully processed.</summary>
		public int CountClaimsProcessed=0;
		///<summary>Number of claims that already had a status of Processed before this automation attempt started.</summary>
		public int CountClaimsAlreadyProcessed=0;
		///<summary>List of patient names taken from the ERA that had claim payments that we could not match to a claim in the DB.</summary>
		public List<string> ListPatNamesWithoutClaimMatch=new List<string>();
		///<summary>List of Errors reported for claims that were matched to an 835 claim that we could not process.</summary>
		public List<string> ListClaimErrors=new List<string>();
		///<summary>True if the insurance payment from the EOB was finalized (ClaimPayment was made).</summary>
		public bool IsPaymentFinalized=false;
		///<summary>The reason why we could not finalize the insurance payment.</summary>
		public string PaymentFinalizationError="";
		///<summary>Will be set true if the ERA was not in a Partial, Unprocessed, or NotFinalized state when automatic processing starts.</summary>
		public bool DidEraStartAsFinalized=false;

		public X835Status Status {
			get {
				if(CountClaimsProcessed+CountClaimsAlreadyProcessed > 0 && !AreAllClaimsReceived()) {
					return X835Status.Partial;
				}
				else if(AreAllClaimsReceived() && !IsPaymentFinalized) {
					return X835Status.NotFinalized;
				}
				else if(AreAllClaimsReceived() && IsPaymentFinalized) {
					return X835Status.Finalized;
				}
				return X835Status.Unprocessed;
			}
		}

		///<summary>Return true if we have no unmatched claims and no unprocessed claims.</summary>
		public bool AreAllClaimsReceived() {
			return ListPatNamesWithoutClaimMatch.Count==0 && ListClaimErrors.Count==0;
		}

		///<summary>Returns false if the user must choose an insurance payment plan, the name on the ERA claim does not match the name on the claim from DB, 
		///the carrier does not allow ERA automation, the claim is processed already, the claimprocs are recieved but the ERA should be providing initial payment,
		///or the claimprocs aren't received and the ERA should be providing a supplemental payment or reversal. 
		///If any of these errors are present, an error message will be added to the list for this EraAutomationResult.</summary>
		public bool CanClaimBeAutoProcessed(bool isFullyAutomatic,Patient patient,InsPlan insPlan,Hx835_Claim claimPaid,List<PayPlan> listPayPlans,
			List<Hx835_ShortClaimProc> listClaimProcsAll,List<Hx835_ShortClaimProc> listClaimProcsForClaim,List<Etrans835Attach> listAttaches)
		{
			StringBuilder stringBuilderErrorMessage=new StringBuilder();
			//Check if user must choose an insurance payment plan to apply the payment to.
			if(listPayPlans.Count>1) {
				stringBuilderErrorMessage.AppendLine(Lans.g("X835","There are multiple insurance payment plans that this payment could be associated to. " +
					"The claim must be processed manually so that an insurance payment plan can be chosen."));
			}
			//Check if a name mismatch exists
			bool isPatientNameMisMatched=claimPaid.IsPatientNameMisMatched(patient);
			if(isPatientNameMisMatched) {
				stringBuilderErrorMessage.AppendLine(Lans.g("X835","The patient name on the ERA does not match the patient on this claim."));
			}
			Carrier carrier=Carriers.GetCarrier(insPlan.CarrierNum);
			//Check if Carrier allows autoprocessing
			if(isFullyAutomatic && carrier.GetEraAutomationMode()!=EraAutomationMode.FullyAutomatic) {
				stringBuilderErrorMessage.AppendLine(Lans.g("X835","The carrier is not set to allow fully automated ERA processing."));
			}
			else if(carrier.GetEraAutomationMode()==EraAutomationMode.ReviewAll) {
				stringBuilderErrorMessage.AppendLine(Lans.g("X835","The carrier is not set to allow automated ERA processing."));
			}
			//Check if 835 claim is processed already
			bool is835ClaimProcessed=claimPaid.IsProcessed(listClaimProcsAll,listAttaches);
			if(is835ClaimProcessed) {
				stringBuilderErrorMessage.AppendLine(Lans.g("X835","The claim is already processed."));
			}
			bool is835ClaimSupplemental=claimPaid.GetIsSupplemental(listAttaches,listClaimProcsAll);
			bool areAnyClaimProcsReceived=listClaimProcsForClaim.Any(x => ListTools.In(x.Status,ClaimProcs.GetInsPaidStatuses()));
			//Autoprocessing can only happen if payment is supplemental and claim is received OR payment is not supplemental and claim is not received.
			if(!is835ClaimSupplemental && areAnyClaimProcsReceived) {
				stringBuilderErrorMessage.AppendLine(Lans.g("X835","The ERA should be providing the initial payment, "
					+"but some claim procedures are already marked received."));
			}
			bool areAnyClaimProcsUnreceived=listClaimProcsForClaim.Any(x => ListTools.In(x.Status,ClaimProcs.GetEstimatedStatuses()));
			if(is835ClaimSupplemental && areAnyClaimProcsUnreceived) {
				if(claimPaid.IsReversal) {
					stringBuilderErrorMessage.AppendLine(Lans.g("X835","The ERA should be providing a reversal, but some claim procedures have not recieved an initial payment yet."));
				}
				else {
					stringBuilderErrorMessage.AppendLine(
						Lans.g("X835","The ERA should be providing a supplemental payment, but some claim procedures have not recieved an initial payment yet."));
				}
			}
			string errorMessage=stringBuilderErrorMessage.ToString();
			if(errorMessage.IsNullOrEmpty()) {
				return true;//We can attempt to autoprocess the claim. Further criteria will be checked in EtransL.TryImportEraClaimData.
			}
			AddClaimError(patient,errorMessage);
			return false;
		}

		///<summary>Add the passed in error message to the ListClaimErrors. The patient name and a label are added above each error.</summary>
		public void AddClaimError(Patient patient,string errorMessage) {
			string errorsForClaim=Lans.g("X835","Errors For Claim:");
			errorMessage=patient.GetNameFL()+"\r\n"+errorsForClaim+"\r\n"+errorMessage;
			ListClaimErrors.Add(errorMessage);
		}

		///<summary>Get messages for claim errors as a single string.</summary>
		private string GetMessageForClaimErrors(bool isForSingleEra) {
			StringBuilder stringBuilderClaimErrors=new StringBuilder();
			if(isForSingleEra) {//If any claims are unprocessed and we are dealing with a single ERA, show claim error details.
				if(ListPatNamesWithoutClaimMatch.Count>0) {
					string claimsCouldNotBeMatched=Lans.g("X835","Claims could not be matched to payments for these patients:");
					stringBuilderClaimErrors.AppendLine(claimsCouldNotBeMatched);
					for(int i=0;i<ListPatNamesWithoutClaimMatch.Count;i++) {
						stringBuilderClaimErrors.AppendLine(ListPatNamesWithoutClaimMatch[i]);
					}
					stringBuilderClaimErrors.AppendLine();
				}
				if(ListClaimErrors.Count>0) {
					string claimProcessingErrors=Lans.g("X835","Claims were matched to payments for these patients but could not be processed");
					stringBuilderClaimErrors.AppendLine(claimProcessingErrors+":");
					for(int i=0;i<ListClaimErrors.Count;i++) {
						stringBuilderClaimErrors.AppendLine(ListClaimErrors[i]);
					}
				}
			}
			else {//If we are processing a message for multiple ERAs, show claim error counts.
				if(ListClaimErrors.Count>0) {
					string unprocessedClaims=Lans.g("X835","Claims that could not be processed:");
					stringBuilderClaimErrors.AppendLine(unprocessedClaims+$" {ListClaimErrors.Count}");
				}
				if(ListPatNamesWithoutClaimMatch.Count>0) {
					string unmatchedClaims=Lans.g("X835","Payments that we could not match to a claim:");
					stringBuilderClaimErrors.AppendLine(unmatchedClaims+$" {ListPatNamesWithoutClaimMatch.Count}");
				}
			}
			return stringBuilderClaimErrors.ToString();
		}

		///<summary>Return the results message for display to the user.</summary>
		public static string CreateMessage(List<EraAutomationResult> listAutomationResults,bool isForSingleEra) {
			StringBuilder stringBuilderAutomationMessage=new StringBuilder();
			//Show count of processed claims.
			int countClaimsProcessed=listAutomationResults.Sum(x => x.CountClaimsProcessed);
			string claimsProcessedSuccessfully=Lans.g("X835","Claims Processed Successfully:");
			stringBuilderAutomationMessage.AppendLine(claimsProcessedSuccessfully+$" {countClaimsProcessed}");
			//Show count of ERAs finalized or a message indicating that a single ERA was finalized.
			int countPaymentsFinalized=listAutomationResults.Count(x => x.IsPaymentFinalized);
			if(isForSingleEra && countPaymentsFinalized==1) {//Don't show count if we are only dealing with one ERA and payment was finalized.
				string paymentFinalized=Lans.g("X835","Payment Finalized Successfully.");
				stringBuilderAutomationMessage.AppendLine(paymentFinalized);
			}
			else if(!isForSingleEra) {//For multiple ERAs, show the count finalized.
				string paymentsFinalized=Lans.g("X835","Payments Finalized:");
				stringBuilderAutomationMessage.AppendLine(paymentsFinalized+$" {countPaymentsFinalized}");
			}
			for(int i=0;i<listAutomationResults.Count;i++) {
				if(listAutomationResults[i].Status==X835Status.Finalized) {
					continue;//No error data to show for ERA.
				}
				if(!isForSingleEra) {//Add info for each ERA if dealing with multiple ERAs.
					stringBuilderAutomationMessage.AppendLine();//Line added to separate each ERA in the error message.
					X835 x835Cur=listAutomationResults[i].X835Cur;
					string carrierName=Lans.g("X835","ERA Carrier Name:");
					string date=Lans.g("X835","ERA Date:");
					string amount=Lans.g("X835","ERA Amount:");
					string transaction=Lans.g("X835","ERA Transaction");
					string of=Lans.g("X835","of");
					stringBuilderAutomationMessage.AppendLine(carrierName+$" {x835Cur.PayerName}");
					stringBuilderAutomationMessage.AppendLine(date+$" {x835Cur.EtransSource.DateTimeTrans}");
					stringBuilderAutomationMessage.AppendLine(amount+$" {x835Cur.InsPaid}");
					if(listAutomationResults[i].TransactionCount>1) {//Show the transaction number if Etrans has multiple transactions
						stringBuilderAutomationMessage
							.AppendLine(transaction+$" {listAutomationResults[i].TransactionNumber} "+of+$" {listAutomationResults[i].TransactionCount}");
					}
				}
				if(listAutomationResults[i].DidEraStartAsFinalized) {
					stringBuilderAutomationMessage.AppendLine(Lans.g("X835","The ERA does not have an Unprocessed, Partial, or NotFinalized Status so it could not be processed."));
					continue;
				}
				//If we have no claim errors, all claims were matched, but payment could not be finalized, show the payment finalization error.
				if(listAutomationResults[i].ListClaimErrors.Count==0
					&& listAutomationResults[i].ListPatNamesWithoutClaimMatch.Count==0
					&& !listAutomationResults[i].IsPaymentFinalized) {
					stringBuilderAutomationMessage.AppendLine(listAutomationResults[i].PaymentFinalizationError);
				}
				else {//if we get here, we must have at least one unmatched claim or claim error, otherwise we would have hit continue above.
					string claimErrors=listAutomationResults[i].GetMessageForClaimErrors(isForSingleEra);
					stringBuilderAutomationMessage.Append(claimErrors);
				}
			}
			return stringBuilderAutomationMessage.ToString();
		}

		///<summary>Adds to the existing note passed in for an etrans. Indicates that automatic processing was completed or attempted on today's date.</summary>
		public static string CreateEtransNote(X835Status status,string note) {
			if(note.Trim().IsNullOrEmpty()) {
				note="";//Clear the existing note if it is only white space.
			}
			else {
				note="\r\n"+note;//Prepend a new line to the note if it already has text.
			}
			string automationNote;
			if(status==X835Status.Finalized) {
				automationNote=Lans.g("X835","Automatically received on");
			}
			else {
				automationNote=Lans.g("X835","Automatic processing attempted on");
			}
			return automationNote+" "+DateTime.Today.ToShortDateString()+note;
		}
	}
	#endregion Helper Classes

}

#region Examples - Import by creating a fake clearinghouse and dumping each example into its own txt file within the reports folder.

#region Example 1
//Example 1 From 835 Specification (modified to include an NM1*IL insured name segment).
//The user would enter the claims in this EOB by total, since neither claim includes procedure detail:
//ISA*00*          *00*          *ZZ*810624427      *ZZ*113504607      *140217*1450*^*00501*000000001*0*P*:~
//GS*HP*810624427*113504607*20140217*1450*1*X*005010X224A2~
//ST*835*1234~
//BPR*C*150000*C*ACH*CTX*01*999999992*DA*123456*1512345678**01*999988880*DA*98765*20020913~
//TRN*1*12345*1512345678~
//DTM*405*20020916~
//N1*PR*INSURANCE COMPANY OF TIMBUCKTU~
//N3*1 MAIN STREET~
//N4*TIMBUCKTU*AK*89111~
//REF*2U*999~
//N1*PE*REGIONAL HOPE HOSPITAL*XX*6543210903~
//LX*110212~
//TS3*6543210903*11*20021231*1*211366.97****138018.4**73348.57~
//TS2*2178.45*1919.71**56.82*197.69*4.23~
//CLP*666123*1*211366.97*138018.4**MA*1999999444444*11*1~
//CAS*CO*45*73348.57~
//NM1*QC*1*JONES*SAM*O***HN*666666666A~
//NM1*IL*1*JONES*FATHER*A***MI*ABC987654321~
//MIA*0***138018.4~
//DTM*232*20020816~
//DTM*233*20020824~
//QTY*CA*8~
//LX*130212~
//TS3*6543210909*13*19961231*1*15000****11980.33**3019.67~
//CLP*777777*1*15000*11980.33**MB*1999999444445*13*1~
//CAS*CO*45*3019.67~
//NM1*QC*1*BORDER*LIZ*E***HN*996669999B~
//MOA***MA02~
//DTM*232*20020512~
//PLB*6543210903*20021231*CV:CP*-1.27~
//SE*28*1234~
//GE*1*1~
//IEA*1*000000001~
#endregion Example 1

#region Example 2
//Example 2 From 835 Specification (modified to include: claim supplemental info in AMT, procedure line item control number in REF*6R, procedure supplemental info in AMT, and procedure remarks in LQ):
//ISA*00*          *00*          *ZZ*810624427      *ZZ*113504607      *140217*1450*^*00501*000000001*0*P*:~
//GS*HP*810624427*113504607*20140217*1450*1*X*005010X224A2~
//ST*835*12233~
//BPR*I*945*C*ACH*CCP*01*888999777*DA*24681012*1935665544**01*111333555*DA*144444*20020316~
//TRN*1*71700666555*1935665544~
//DTM*405*20020314~
//N1*PR*RUSHMORE LIFE~
//N3*10 SOUTH AVENUE~
//N4*RAPID CITY*SD*55111~
//N1*PE*ACME MEDICAL CENTER*XX*5544667733~
//REF*TJ*777667755~
//LX*1~
//CLP*55545554444*1*800*450*300*12*94060555410000~
//CAS*CO*A2*50~
//NM1*QC*1*BUDD*WILLIAM****MI*33344555510~
//AMT*D8*0.99~
//SVC*HC:99211*800*500~
//DTM*150*20020301~
//DTM*151*20020304~
//CAS*PR*1*300~
//AMT*T*10~
//LQ*HE*M16~
//CLP*8765432112*1*1200*495*600*12*9407779923000~
//CAS*CO*A2*55~
//NM1*QC*1*SETTLE*SUSAN****MI*44455666610~
//SVC*HC:93555*1200*550~
//DTM*150*20020310~
//DTM*151*20020312~
//CAS*PR*1*600~
//CAS*CO*45*50~
//REF*6R*p1000~
//SE*25*112233~
//GE*1*1~
//IEA*1*000000001~
#endregion Example 2

#region Example 3
//Example 3 From 835 Specification (modified to include a procedure line item control number in REF*6R):
//ISA*00*          *00*          *ZZ*810624427      *ZZ*113504607      *140217*1450*^*00501*000000001*0*P*:~
//GS*HP*810624427*113504607*20140217*1450*1*X*005010X224A2~
//ST*835*0001~
//BPR*I*1222*C*CHK************20050412~
//TRN*1*0012524965*1559123456~
//REF*EV*030240928~
//DTM*405*20050412~
//N1*PR*YOUR TAX DOLLARS AT WORK~
//N3*481A00 DEER RUN ROAD~
//N4*WEST PALM BCH*FL*11114~
//N1*PE*ACME MEDICAL CENTER*FI*5999944521~
//N3*PO BOX 863382~
//N4*ORLANDO*FL*55115~
//REF*PQ*10488~
//LX*1~
//CLP*L0004828311*2*10323.64*912**12*05090256390*11*1~
//CAS*OA*23*9411.64~
//NM1*QC*1*TOWNSEND*WILLIAM*P***MI*XXX123456789~
//NM1*82*2*ACME MEDICAL CENTER*****BD*987~
//DTM*232*20050303~
//DTM*233*20050304~
//AMT*AU*912~
//LX*2~
//CLP*0001000053*2*751.50*310*220*12*50630626430~
//NM1*QC*1*BAKI*ANGI****MI*456789123~
//NM1*82*2*SMITH JONES PA*****BS*34426~
//DTM*232*20050106~
//DTM*233*20050106~
//SVC*HC:12345:26*166.5*30**1~
//DTM*472*20050106~
//CAS*OA*23*136.50~
//REF*6R*p1001~
//REF*1B*43285~
//AMT*AU*150~
//SVC*HC:66543:26*585*280*220*1~
//DTM*472*20050106~
//CAS*PR*1*150**2*70~
//CAS*CO*42*85~
//REF*6R*123456~
//REF*1B*43285~
//AMT*AU*500~
//SE*38*0001~
//GE*1*1~
//IEA*1*000000001~
#endregion Example 3

#region Example 4
//Example 4 From 835 Specification (Modified such that submitted procedure code different than adjudicated procedure code):
//ISA*00*          *00*          *ZZ*810624427      *ZZ*113504607      *140217*1450*^*00501*000000001*0*P*:~
//GS*HP*810624427*113504607*20140217*1450*1*X*005010X224A2~
//ST*835*0001~
//BPR*I*187.50*C*CHK************20050412~
//TRN*1*0012524879*1559123456~
//REF*EV*030240928~
//DTM*405*20050412~
//N1*PR*YOUR TAX DOLLARS AT WORK~
//N3*481A00 DEER RUN ROAD~
//N4*WEST PALM BCH*FL*11114~
//N1*PE*ACME MEDICAL CENTER*FI*599944521~
//N3*PO BOX 863382~
//N4*ORLANDO*FL*55115~
//REF*PQ*10488~
//LX*1~
//CLP*0001000054*3*3345.5*187.50**12*50580155533~
//NM1*QC*1*ISLAND*ELLIS*E****MI*789123456~
//NM1*82*2*JONES JONES ASSOCIATES*****BS*AB34U~
//DTM*232*20050120~
//SVC*HC:12345*3345.5*1766.5*187.50*1*HC:67890~
//DTM*472*20050120~
//CAS*OA*23*1579~
//REF*1B*44280~
//AMT*AU*1700~
//SE*38*0001~
//GE*1*1~
//IEA*1*000000001~
#endregion Example 4

#region Example 5
//Example 5 From 835 Specification:
//ISA*00*          *00*          *ZZ*810624427      *ZZ*113504607      *140217*1450*^*00501*000000001*0*P*:~
//GS*HP*810624427*113504607*20140217*1450*1*X*005010X224A2~
//ST*835*0001~
//BPR*I*34.00*C*CHK************20050318~
//TRN*1*0063158ABC*1566339911~
//REF*EV*030240928~
//DTM*405*20050318~
//N1*PR*YOUR TAX DOLLARS AT WORK~
//N3*481A00 DEER RUN ROAD~
//N4*WEST PALM BCH*FL*11114~
//N1*PE*ATONEWITHHEALTH*FI*3UR334563~
//N3*3501 JOHNSON STREET~
//N4*SUNSHINE*FL*12345~
//REF*PQ*11861~
//LX*1~
//CLP*0001000055*2*541*34**12*50650619501~
//NM1*QC*1*BRUCK*RAYMOND*W***MI*987654321~
//NM1*82*2*PROFESSIONAL TEST 1*****BS*34426~
//DTM*232*20050202~
//DTM*233*20050202~
//SVC*HC:55669*541*34**1~
//DTM*472*20050202~
//CAS*OA*23*516~
//CAS*OA*94*-9~
//REF*1B*44280~
//AMT*AU*550~
//SE*38*0001~
//GE*1*1~
//IEA*1*000000001~
#endregion Example 5

#region Example 6
//Example 6, copied from example 5 and modified: The claims are duplicated several times (with differing identifiers), as a way to test ERA printing on multiple pages.
//Numbers may be off, the only purpose of this example is for multi-page printing and the content has not been sanity checked very deeply.
//ISA*00*          *00*          *ZZ*810624427      *ZZ*113504607      *140217*1450*^*00501*000000001*0*P*:~
//GS*HP*810624427*113504607*20140217*1450*1*X*005010X224A2~
//ST*835*0001~
//BPR*I*510.00*C*CHK************20050318~
//TRN*1*0063158ABC*1566339911~
//REF*EV*030240928~
//DTM*405*20050318~
//N1*PR*YOUR TAX DOLLARS AT WORK~
//N3*481A00 DEER RUN ROAD~
//N4*WEST PALM BCH*FL*11114~
//N1*PE*ATONEWITHHEALTH*FI*3UR334563~
//N3*3501 JOHNSON STREET~
//N4*SUNSHINE*FL*12345~
//REF*PQ*11861~
//LX*1~
//CLP*1/1*2*541*34**12*50650619501~
//NM1*QC*1*BRUCK*RAYMOND*W***MI*987654321~
//NM1*82*2*PROFESSIONAL TEST 1*****BS*34426~
//DTM*232*20050202~
//DTM*233*20050202~
//SVC*HC:55669*541*34**1~
//DTM*472*20050202~
//CAS*OA*23*516~
//CAS*OA*94*-9~
//REF*1B*44280~
//AMT*AU*550~
//CLP*2/2*2*541*34**12*50650619501~
//NM1*QC*1*BRUCK*RAYMOND*W***MI*987654321~
//NM1*82*2*PROFESSIONAL TEST 1*****BS*34426~
//DTM*232*20050202~
//DTM*233*20050202~
//SVC*HC:55669*541*34**1~
//DTM*472*20050202~
//CAS*OA*23*516~
//CAS*OA*94*-9~
//REF*1B*44280~
//AMT*AU*550~
//CLP*3/3*2*541*34**12*50650619501~
//NM1*QC*1*BRUCK*RAYMOND*W***MI*987654321~
//NM1*82*2*PROFESSIONAL TEST 1*****BS*34426~
//DTM*232*20050202~
//DTM*233*20050202~
//SVC*HC:55669*541*34**1~
//DTM*472*20050202~
//CAS*OA*23*516~
//CAS*OA*94*-9~
//REF*1B*44280~
//AMT*AU*550~
//CLP*4/4*2*541*34**12*50650619501~
//NM1*QC*1*BRUCK*RAYMOND*W***MI*987654321~
//NM1*82*2*PROFESSIONAL TEST 1*****BS*34426~
//DTM*232*20050202~
//DTM*233*20050202~
//SVC*HC:55669*541*34**1~
//DTM*472*20050202~
//CAS*OA*23*516~
//CAS*OA*94*-9~
//REF*1B*44280~
//AMT*AU*550~
//CLP*5/5*2*541*34**12*50650619501~
//NM1*QC*1*BRUCK*RAYMOND*W***MI*987654321~
//NM1*82*2*PROFESSIONAL TEST 1*****BS*34426~
//DTM*232*20050202~
//DTM*233*20050202~
//SVC*HC:55669*541*34**1~
//DTM*472*20050202~
//CAS*OA*23*516~
//CAS*OA*94*-9~
//REF*1B*44280~
//AMT*AU*550~
//CLP*6/6*2*541*34**12*50650619501~
//NM1*QC*1*BRUCK*RAYMOND*W***MI*987654321~
//NM1*82*2*PROFESSIONAL TEST 1*****BS*34426~
//DTM*232*20050202~
//DTM*233*20050202~
//SVC*HC:55669*541*34**1~
//DTM*472*20050202~
//CAS*OA*23*516~
//CAS*OA*94*-9~
//REF*1B*44280~
//AMT*AU*550~
//CLP*7/7*2*541*34**12*50650619501~
//NM1*QC*1*BRUCK*RAYMOND*W***MI*987654321~
//NM1*82*2*PROFESSIONAL TEST 1*****BS*34426~
//DTM*232*20050202~
//DTM*233*20050202~
//SVC*HC:55669*541*34**1~
//DTM*472*20050202~
//CAS*OA*23*516~
//CAS*OA*94*-9~
//REF*1B*44280~
//AMT*AU*550~
//CLP*8/8*2*541*34**12*50650619501~
//NM1*QC*1*BRUCK*RAYMOND*W***MI*987654321~
//NM1*82*2*PROFESSIONAL TEST 1*****BS*34426~
//DTM*232*20050202~
//DTM*233*20050202~
//SVC*HC:55669*541*34**1~
//DTM*472*20050202~
//CAS*OA*23*516~
//CAS*OA*94*-9~
//REF*1B*44280~
//AMT*AU*550~
//CLP*9/9*2*541*34**12*50650619501~
//NM1*QC*1*BRUCK*RAYMOND*W***MI*987654321~
//NM1*82*2*PROFESSIONAL TEST 1*****BS*34426~
//DTM*232*20050202~
//DTM*233*20050202~
//SVC*HC:55669*541*34**1~
//DTM*472*20050202~
//CAS*OA*23*516~
//CAS*OA*94*-9~
//REF*1B*44280~
//AMT*AU*550~
//CLP*10/10*2*541*34**12*50650619501~
//NM1*QC*1*BRUCK*RAYMOND*W***MI*987654321~
//NM1*82*2*PROFESSIONAL TEST 1*****BS*34426~
//DTM*232*20050202~
//DTM*233*20050202~
//SVC*HC:55669*541*34**1~
//DTM*472*20050202~
//CAS*OA*23*516~
//CAS*OA*94*-9~
//REF*1B*44280~
//AMT*AU*550~
//CLP*11/11*2*541*34**12*50650619501~
//NM1*QC*1*BRUCK*RAYMOND*W***MI*987654321~
//NM1*82*2*PROFESSIONAL TEST 1*****BS*34426~
//DTM*232*20050202~
//DTM*233*20050202~
//SVC*HC:55669*541*34**1~
//DTM*472*20050202~
//CAS*OA*23*516~
//CAS*OA*94*-9~
//REF*1B*44280~
//AMT*AU*550~
//CLP*12/12*2*541*34**12*50650619501~
//NM1*QC*1*BRUCK*RAYMOND*W***MI*987654321~
//NM1*82*2*PROFESSIONAL TEST 1*****BS*34426~
//DTM*232*20050202~
//DTM*233*20050202~
//SVC*HC:55669*541*34**1~
//DTM*472*20050202~
//CAS*OA*23*516~
//CAS*OA*94*-9~
//REF*1B*44280~
//AMT*AU*550~
//CLP*13/13*2*541*34**12*50650619501~
//NM1*QC*1*BRUCK*RAYMOND*W***MI*987654321~
//NM1*82*2*PROFESSIONAL TEST 1*****BS*34426~
//DTM*232*20050202~
//DTM*233*20050202~
//SVC*HC:55669*541*34**1~
//DTM*472*20050202~
//CAS*OA*23*516~
//CAS*OA*94*-9~
//REF*1B*44280~
//AMT*AU*550~
//CLP*14/14*2*541*34**12*50650619501~
//NM1*QC*1*BRUCK*RAYMOND*W***MI*987654321~
//NM1*82*2*PROFESSIONAL TEST 1*****BS*34426~
//DTM*232*20050202~
//DTM*233*20050202~
//SVC*HC:55669*541*34**1~
//DTM*472*20050202~
//CAS*OA*23*516~
//CAS*OA*94*-9~
//REF*1B*44280~
//AMT*AU*550~
//CLP*15/15*2*541*34**12*50650619501~
//NM1*QC*1*BRUCK*RAYMOND*W***MI*987654321~
//NM1*82*2*PROFESSIONAL TEST 1*****BS*34426~
//DTM*232*20050202~
//DTM*233*20050202~
//SVC*HC:55669*541*34**1~
//DTM*472*20050202~
//CAS*OA*23*516~
//CAS*OA*94*-9~
//REF*1B*44280~
//AMT*AU*550~
//SE*38*0001~
//GE*1*1~
//IEA*1*000000001~
#endregion Example 6

#region Example 7
//Example 7: Provided and engineered by ClaimConnect.  The data is fake.  This example contains multiple transactions (ST segments) even though the X12 guide says there can only be 1.
//See the example content in the file named TEST_DentalX.2030339.835.  Notice that the file extension is "835".
#endregion Example 7

#region Example 8
//This example is from page 45 in the X12 implementation guide. CLP through LQ segments are copied from the guide, but the rest of the example (the filler) is from example 3 to make the message complete.
//ISA*00*          *00*          *ZZ*810624427      *ZZ*113504607      *140217*1450*^*00501*000000001*0*P*:~
//GS*HP*810624427*113504607*20140217*1450*1*X*005010X224A2~
//ST*835*0001~
//BPR*I*1222*C*CHK************20020928~
//TRN*1*0012524965*1559123456~
//REF*EV*030240928~
//DTM*405*20050412~
//N1*PR*YOUR TAX DOLLARS AT WORK~
//N3*481A00 DEER RUN ROAD~
//N4*WEST PALM BCH*FL*11114~
//N1*PE*ACME MEDICAL CENTER*FI*5999944521~
//N3*PO BOX 863382~
//N4*ORLANDO*FL*55115~
//REF*PQ*10488~
//LX*1~
//CLP*9/26*1*740*740*MC~
//NM1*QC*1*CAMPBELL*MAYA*R***MI*854216985~
//NM1*82*2*PROFESSIONAL TEST 1*****BS*34426~
//MOA***MA15~
//SVC*NU:A*300*300**2~
//REF*6R*1~
//SVC*NU:B*400*400**4~
//REF*6R*2~
//SVC*NU:E*40*40**2~
//DTM*150*20020928~
//DTM*151*20020930~
//REF*6R*5~
//LQ*HE*N123~
//SE*38*0001~
//GE*1*1~
//IEA*1*000000001~
#endregion Example 8

#region Example 9
//Example 9 (Modified version of example 4, with a different patient, and more procedures of the same code and fee to test procedure matching):
//ISA*00*          *00*          *ZZ*810624427      *ZZ*113504607      *140217*1450*^*00501*000000001*0*P*:~
//GS*HP*810624427*113504607*20140217*1450*1*X*005010X224A2~
//ST*835*0001~
//BPR*I*40.00*C*CHK************20150116~
//TRN*1*0012524879*1559123456~
//REF*EV*030240928~
//DTM*405*20150116~
//N1*PR*YOUR TAX DOLLARS AT WORK~
//N3*481A00 DEER RUN ROAD~
//N4*WEST PALM BCH*FL*11114~
//N1*PE*ACME MEDICAL CENTER*FI*599944521~
//N3*PO BOX 863382~
//N4*ORLANDO*FL*55115~
//REF*PQ*10488~
//LX*1~
//CLP*0001000054*1*40.00*40.00**12*50580155533~
//NM1*QC*1*NORRIS*CHUCK****MI*8652413659~
//NM1*82*1*NORRIS*CHUCK****MI*8652413659~
//DTM*232*20150116~
//SVC*HC:12345*10.00*10.00**1~
//DTM*472*20150116~
//SVC*HC:12345*10.00*10.00**1~
//DTM*472*20150116~
//REF*6R*p9~
//SVC*HC:56789*10.00*6.00**1*HC:12345~
//DTM*472*20150116~
//REF*6R*p10~
//SVC*HC:98765*0.00*4.00**1*HC:12345~
//DTM*472*20150116~
//REF*6R*p10~
//SVC*HC:12345*10.00*10.00**1~
//DTM*472*20150116~
//REF*6R*p5~
//SE*38*0001~
//GE*1*1~
//IEA*1*000000001~
#endregion Example 9

#region Example 10
//Sometimes the insurance company will report an ERA to the provider for printed claims.
//When this happens, the Claim Identifier on the printed claim will be "0" within the ERA.
//Use this example to test receiving an ERA for a printed claim and attaching the claim in the ERA to the OD claim object.
//This example is copied from Example 4 and is modified to use a different patient/sub name and to use a Claim Identifier of "0".
//ISA*00*          *00*          *ZZ*810624427      *ZZ*113504607      *140217*1450*^*00501*000000001*0*P*:~
//GS*HP*810624427*113504607*20140217*1450*1*X*005010X224A2~
//ST*835*0001~
//BPR*I*187.50*C*CHK************20050412~
//TRN*1*0012524879*1559123456~
//REF*EV*030240928~
//DTM*405*20050412~
//N1*PR*YOUR TAX DOLLARS AT WORK~
//N3*481A00 DEER RUN ROAD~
//N4*WEST PALM BCH*FL*11114~
//N1*PE*ACME MEDICAL CENTER*FI*599944521~
//N3*PO BOX 863382~
//N4*ORLANDO*FL*55115~
//REF*PQ*10488~
//LX*1~
//CLP*0*3*3345.5*187.50**12*50580155533~
//NM1*QC*1*JACKSON*JIMMY*L****MI*789123456~
//NM1*82*2*JONES JONES ASSOCIATES*****BS*AB34U~
//DTM*232*20050120~
//SVC*HC:12345*3345.5*1766.5*187.50*1*HC:67890~
//DTM*472*20050120~
//CAS*OA*23*1579~
//REF*1B*44280~
//AMT*AU*1700~
//SE*38*0001~
//GE*1*1~
//IEA*1*000000001~
#endregion Example 10

#region Example 11
//Example of multiple transactional groups.  Should import this one file into two separate etrans entries each with TranSetId non-blank.
//Copied from Example 1 and modified.
//ISA*00*          *00*          *ZZ*810624427      *ZZ*113504607      *140217*1450*^*00501*000000001*0*P*:~
//GS*HP*810624427*113504607*20140217*1450*1*X*005010X224A2~
//ST*835*1234~
//BPR*C*150000*C*ACH*CTX*01*999999992*DA*123456*1512345678**01*999988880*DA*98765*20020913~
//TRN*1*12345*1512345678~
//DTM*405*20020916~
//N1*PR*INSURANCE COMPANY OF TIMBUCKTU~
//N3*1 MAIN STREET~
//N4*TIMBUCKTU*AK*89111~
//REF*2U*999~
//N1*PE*REGIONAL HOPE HOSPITAL*XX*6543210903~
//LX*110212~
//TS3*6543210903*11*20021231*1*211366.97****138018.4**73348.57~
//TS2*2178.45*1919.71**56.82*197.69*4.23~
//CLP*666123*1*211366.97*138018.4**MA*1999999444444*11*1~
//CAS*CO*45*73348.57~
//NM1*QC*1*HOFFA*JAMES*D***HN*666666666A~
//NM1*IL*1*HOFFA*FATHER*I***MI*ABC987654321~
//MIA*0***138018.4~
//DTM*232*20020816~
//DTM*233*20020824~
//QTY*CA*8~
//LX*130212~
//TS3*6543210909*13*19961231*1*15000****11980.33**3019.67~
//CLP*777777*1*15000*11980.33**MB*1999999444445*13*1~
//CAS*CO*45*3019.67~
//NM1*QC*1*BROWN*ELIZABETH*A***HN*996669999B~
//MOA***MA02~
//DTM*232*20020512~
//PLB*6543210903*20021231*CV:CP*-1.27~
//SE*28*1234~
//ST*835*1234~
//BPR*C*150000*C*ACH*CTX*01*999999992*DA*123456*1512345678**01*999988880*DA*98765*20020913~
//TRN*1*12345*1512345678~
//DTM*405*20020916~
//N1*PR*INSURANCE COMPANY OF TIMBUCKTU~
//N3*1 MAIN STREET~
//N4*TIMBUCKTU*AK*89111~
//REF*2U*999~
//N1*PE*REGIONAL HOPE HOSPITAL*XX*6543210903~
//LX*110212~
//TS3*6543210903*11*20021231*1*211366.97****138018.4**73348.57~
//TS2*2178.45*1919.71**56.82*197.69*4.23~
//CLP*666123*1*211366.97*138018.4**MA*1999999444444*11*1~
//CAS*CO*45*73348.57~
//NM1*QC*1*GARCIA*JUAN*K***HN*666666666A~
//NM1*IL*1*GARCIA*PAPI*A***MI*ABC987654321~
//MIA*0***138018.4~
//DTM*232*20020816~
//DTM*233*20020824~
//QTY*CA*8~
//LX*130212~
//TS3*6543210909*13*19961231*1*15000****11980.33**3019.67~
//CLP*777777*1*15000*11980.33**MB*1999999444445*13*1~
//CAS*CO*45*3019.67~
//NM1*QC*1*ELF*TELF*N***HN*996669999B~
//MOA***MA02~
//DTM*232*20020512~
//PLB*6543210903*20021231*CV:CP*-1.27~
//SE*28*1234~
//GE*1*1~
//IEA*1*000000001~
#endregion Example 11

#endregion Examples