using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenDentBusiness;

namespace TestCanada {
	public class CarrierTC {
		public static string SetInitialCarriers() {
			//We are starting with zero carriers
			CanadianNetwork network=new CanadianNetwork();
			network.Abbrev="CDANET14";
			network.Descript="CDANET14";
			network.CanadianTransactionPrefix="CDANET14";
			CanadianNetworks.Insert(network);
			Carrier carrier=new Carrier();
			carrier.IsCDA=true;
			carrier.CarrierName="Carrier 1";
			carrier.CanadianNetworkNum=network.CanadianNetworkNum;
			carrier.CDAnetVersion="04";
			carrier.ElectID="666666";
			carrier.CanadianEncryptionMethod=2;
			carrier.CanadianSupportedTypes
				//claim_01 is implied
				= CanSupTransTypes.CobClaimTransaction_07
				//claimAck_11 is implied
				//claimEob_21 is implied
				| CanSupTransTypes.ClaimReversal_02
				| CanSupTransTypes.ClaimReversalResponse_12
				| CanSupTransTypes.PredeterminationSinglePage_03
				| CanSupTransTypes.PredeterminationMultiPage_03
				| CanSupTransTypes.RequestForOutstandingTrans_04
				| CanSupTransTypes.EmailTransaction_24
				| CanSupTransTypes.RequestForSummaryReconciliation_05
				| CanSupTransTypes.RequestForPaymentReconciliation_06;
			Carriers.Insert(carrier);
			//Carrier2---------------------------------------------------
			network=new CanadianNetwork();
			network.Abbrev="A";
			network.Descript="A";
			network.CanadianTransactionPrefix="A";
			CanadianNetworks.Insert(network);
			carrier=new Carrier();
			carrier.IsCDA=true;
			carrier.CarrierName="Carrier 2";
			carrier.CanadianNetworkNum=network.CanadianNetworkNum;
			carrier.CDAnetVersion="04";
			carrier.ElectID="777777";
			carrier.CanadianEncryptionMethod=1;
			carrier.CanadianSupportedTypes
				= CanSupTransTypes.EligibilityTransaction_08
				//claim_01 is implied
				//claimAck_11 is implied
				//claimEob_21 is implied
				| CanSupTransTypes.ClaimReversal_02
				| CanSupTransTypes.PredeterminationSinglePage_03
				| CanSupTransTypes.RequestForOutstandingTrans_04
				| CanSupTransTypes.EmailTransaction_24
				| CanSupTransTypes.RequestForPaymentReconciliation_06;
			Carriers.Insert(carrier);
			//Carrier3---------------------------------------------------
			network=new CanadianNetwork();
			network.Abbrev="AB";
			network.Descript="AB";
			network.CanadianTransactionPrefix="AB";
			CanadianNetworks.Insert(network);
			carrier=new Carrier();
			carrier.IsCDA=true;
			carrier.CarrierName="Carrier 3";
			carrier.CanadianNetworkNum=network.CanadianNetworkNum;
			carrier.CDAnetVersion="04";
			carrier.ElectID="888888";
			carrier.CanadianEncryptionMethod=2;
			carrier.CanadianSupportedTypes
				= CanSupTransTypes.EligibilityTransaction_08
				//claim_01 is implied
				| CanSupTransTypes.CobClaimTransaction_07
				//claimAck_11 is implied
				//claimEob_21 is implied
				| CanSupTransTypes.ClaimReversal_02
				| CanSupTransTypes.PredeterminationSinglePage_03
				| CanSupTransTypes.RequestForOutstandingTrans_04
				| CanSupTransTypes.EmailTransaction_24
				| CanSupTransTypes.RequestForPaymentReconciliation_06;
			Carriers.Insert(carrier);
			//Carrier4---------------------------------------------------
			network=new CanadianNetwork();
			network.Abbrev="ABC";
			network.Descript="ABC";
			network.CanadianTransactionPrefix="ABC";
			CanadianNetworks.Insert(network);
			carrier=new Carrier();
			carrier.IsCDA=true;
			carrier.CarrierName="Carrier 4";
			carrier.CanadianNetworkNum=network.CanadianNetworkNum;
			carrier.CDAnetVersion="04";
			carrier.ElectID="999111";
			carrier.CanadianEncryptionMethod=2;
			carrier.CanadianSupportedTypes
				= CanSupTransTypes.EligibilityTransaction_08
				//claim_01 is implied
				| CanSupTransTypes.CobClaimTransaction_07
				//claimAck_11 is implied
				//claimEob_21 is implied
				| CanSupTransTypes.ClaimReversal_02
				| CanSupTransTypes.PredeterminationSinglePage_03
				| CanSupTransTypes.RequestForOutstandingTrans_04
				| CanSupTransTypes.EmailTransaction_24
				| CanSupTransTypes.RequestForPaymentReconciliation_06;
			Carriers.Insert(carrier);
			//Carrier5---------------------------------------------------
			network=new CanadianNetwork();
			network.Abbrev="V2CAR";
			network.Descript="V2CAR";
			network.CanadianTransactionPrefix="V2CAR";
			CanadianNetworks.Insert(network);
			carrier=new Carrier();
			carrier.IsCDA=true;
			carrier.CarrierName="Carrier 5";
			carrier.CanadianNetworkNum=network.CanadianNetworkNum;
			carrier.CDAnetVersion="02";
			carrier.ElectID="555555";
			carrier.CanadianEncryptionMethod=0;//not applicable
			carrier.CanadianSupportedTypes
				= CanSupTransTypes.EligibilityTransaction_08
				| CanSupTransTypes.EligibilityResponse_18
				//claim_01 is implied
				//claimAck_11 is implied
				//claimEob_21 is implied
				| CanSupTransTypes.ClaimReversal_02
				| CanSupTransTypes.PredeterminationSinglePage_03
				| CanSupTransTypes.PredeterminationMultiPage_03;
			Carriers.Insert(carrier);
			//---------------------------------------------------------
			//Used for Payment Reconciliation test #3 and Summary Reconciliation test #3
			network=new CanadianNetwork();
			network.Abbrev="ABC";
			network.Descript="ABC";
			network.CanadianTransactionPrefix="ABC";
			CanadianNetworks.Insert(network);
			carrier=new Carrier();
			carrier.IsCDA=true;
			carrier.CarrierName="111555";
			carrier.CanadianNetworkNum=network.CanadianNetworkNum;
			carrier.CDAnetVersion="04";
			carrier.ElectID="111555";
			carrier.CanadianEncryptionMethod=0;//not applicable
			carrier.CanadianSupportedTypes
				= CanSupTransTypes.PaymentReconciliation_16;
			Carriers.Insert(carrier);
			return "Carrier objects set.\r\n";
		}

		public static long GetCarrierNumById(string carrierId) {
			for(int i=0;i<Carriers.Listt.Length;i++) {
				if(Carriers.Listt[i].ElectID==carrierId) {
					return Carriers.Listt[i].CarrierNum;
				}
			}
			return 0;
		}

		///<summary>encryptionMethod should be 1 or 2.</summary>
		public static void SetEncryptionMethod(long planNum,byte encryptionMethod) {
			InsPlan plan=InsPlans.RefreshOne(planNum);
			Carrier carrier=Carriers.GetCarrier(plan.CarrierNum);
			if(carrier.CanadianEncryptionMethod!=encryptionMethod) {
				carrier.CanadianEncryptionMethod=encryptionMethod;
				Carriers.Update(carrier);
				Carriers.RefreshCache();
			}
		}

		///<summary>version should be "02" or "04". Returns old version.</summary>
		public static string SetCDAnetVersion(long planNum,string version) {
			InsPlan plan=InsPlans.RefreshOne(planNum);
			Carrier carrier=Carriers.GetCarrier(plan.CarrierNum);
			string oldVersion=carrier.CDAnetVersion;
			if(carrier.CDAnetVersion!=version) {
				carrier.CDAnetVersion=version;
				Carriers.Update(carrier);
				Carriers.RefreshCache();
			}
			return oldVersion;
		}

	}
}
