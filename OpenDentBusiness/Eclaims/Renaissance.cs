using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;

namespace OpenDentBusiness.Eclaims{
	///<summary>Summary description for Renaissance.</summary>
	public class Renaissance{
		private static ClaimFormItem[] items;
		private static IFormClaimFormItemEdit FormCFI;
		private static string[][] DisplayStrings;
		private static Clearinghouse _clearinghouseClin;

		public delegate string[][] FillRenaissanceDelegate(long claimNum,long patNum,List<ClaimFormItem> listItems);

		///<summary>Summary description for Renaissance.</summary>
		public Renaissance(){
			
		}

		///<summary>Called from Eclaims and includes multiple claims.</summary>
		public static string SendBatch(Clearinghouse clearinghouseClin,List<ClaimSendQueueItem> queueItems,int batchNum,IFormClaimFormItemEdit formCFI,
			FillRenaissanceDelegate fillRenaissance) 
		{
			for(int i=0;i<queueItems.Count;i++) {
				_clearinghouseClin=clearinghouseClin;
				if(!CreateClaim(queueItems[i].PatNum,queueItems[i].ClaimNum,batchNum,formCFI,fillRenaissance)) {
					return "";
				}
			}
			return "Sent";
		}

		///<summary>Called once for each claim to be created.  For claims with a lot of procedures, this may actually create multiple claims.</summary>
		public static bool CreateClaim(long patNum,long claimNum,int batchNum,IFormClaimFormItemEdit formCFI,FillRenaissanceDelegate fillRenaissance) {
			//this can be eliminated later after error checking complete:
			FormCFI=formCFI;
			FormCFI.FillFieldNames();
			items=new ClaimFormItem[241];//0 is not used 
			Fill(1,"IsPreAuth");
			Fill(2,"IsStandardClaim");
			Fill(3,"IsMedicaidClaim");
			Fill(4,"");//EPSTD
			Fill(5,"TreatingProviderSpecialty");
			Fill(6,"PreAuthString");
			Fill(7,"PriInsCarrierName");
			Fill(8,"PriInsAddressComplete");
			Fill(9,"PriInsCity");
			Fill(10,"PriInsST");
			Fill(11,"PriInsZip");
			Fill(12,"PatientLastFirst");
			Fill(13,"PatientAddressComplete");
			Fill(14,"PatientCity");
			Fill(15,"PatientST");
			Fill(16,"PatientDOB","MM/dd/yyyy");
			Fill(17,"PatientID-MedicaidOrSSN");
			Fill(18,"PatientIsMale");
			Fill(19,"PatientIsFemale");
			Fill(20,"PatientPhone");
			Fill(21,"PatientZip");
			Fill(22,"RelatIsSelf");
			Fill(23,"RelatIsSpouse");
			Fill(24,"RelatIsChild");
			Fill(25,"RelatIsOther");
			Fill(26,"");//relat other descript
			Fill(27,"");//pat employer/school
			Fill(28,"");//pat employer/school address
			Fill(29,"SubscrID");
			Fill(30,"EmployerName");
			Fill(31,"GroupNum");
			Fill(32,"OtherInsNotExists");
			Fill(33,"OtherInsExists");
			Fill(34,"OtherInsExists");//dental
			Fill(35,"");//other ins exists medical
			Fill(36,"OtherInsSubscrID");
			Fill(37,"SubscrLastFirst");
			Fill(38,"OtherInsSubscrLastFirst");
			Fill(39,"SubscrAddressComplete");
			Fill(40,"SubscrPhone");
			Fill(41,"OtherInsSubscrDOB","MM/dd/yyyy");
			Fill(42,"OtherInsSubscrIsMale");
			Fill(43,"OtherInsSubscrIsFemale");
			Fill(44,"OtherInsGroupNum");//Other Plan/Program name
			Fill(45,"SubscrCity");
			Fill(46,"SubscrST");
			Fill(47,"SubscrZip");
			Fill(48,"");//other subscr employer/school
			Fill(49,"");//other subscr emp/school address
			Fill(50,"SubscrDOB","MM/dd/yyyy");
			Fill(51,"SubscrIsMarried");
			Fill(52,"SubscrIsSingle");
			Fill(53,"");//subscr marital status other
			Fill(54,"SubscrIsMale");
			Fill(55,"SubscrIsFemale");
			Fill(56,"");//subscr is employed
			Fill(57,"");//subscr is part time
			Fill(58,"SubscrIsFTStudent");
			Fill(59,"SubscrIsPTStudent");
			Fill(60,"");//subsc employer/school
			Fill(61,"");//subsc employer/school address
			Fill(62,"PatientRelease");
			Fill(63,"PatientReleaseDate","MM/dd/yyyy");
			Fill(64,"PatientAssignment");
			Fill(65,"PatientAssignmentDate","MM/dd/yyyy");
			Fill(66,"BillingDentist");
			Fill(67,"BillingDentistPhoneFormatted");
			Fill(68,"BillingDentistMedicaidID");
			Fill(69,"BillingDentistSSNorTIN");
			Fill(70,"PayToDentistAddress");
			Fill(71,"PayToDentistAddress2");
			Fill(72,"BillingDentistLicenseNum");
			Fill(73,"DateService","MM/dd/yyyy");
			Fill(74,"PlaceIsOffice");
			Fill(75,"PlaceIsHospADA2002");
			Fill(76,"PlaceIsExtCareFacilityADA2002");
			Fill(77,"PlaceIsOtherADA2002");
			Fill(78,"PayToDentistCity");
			Fill(79,"PayToDentistST");
			Fill(80,"PayToDentistZip");
			Fill(81,"IsRadiographsAttached");
			Fill(82,"RadiographsNumAttached");
			Fill(83,"RadiographsNotAttached");
			Fill(84,"IsOrtho");
			Fill(85,"IsNotOrtho");
			Fill(86,"IsInitialProsth");
			Fill(87,"IsReplacementProsth");
			Fill(88,"");//reason for replacement
			Fill(89,"DatePriorProsthPlaced","MM/dd/yyyy");
			Fill(90,"DateOrthoPlaced");
			Fill(91,"MonthsOrthoRemaining");
			Fill(92,"IsNotOccupational");
			Fill(93,"IsOccupational");
			Fill(94,"");//description of occupational injury
			Fill(95,"IsAutoAccident");
			Fill(96,"IsOtherAccident");
			Fill(97,"IsNotAccident");
			Fill(98,"");//description of accident
			Fill(99,"TreatingDentistNPI");//Individual NPI
			Fill(100,"");//Attachment ID
			Fill(101,"BillingDentistNPI");//Group NPI
			Fill(102,"");//Specail Tesia Remarks field Import Field gets moved to 232 on outbound
			Fill(103,"");//4th Carrier Address Line
			Fill(104,"");//4th Billing Address Line
			Fill(105,"");//4th Subscriber Address Line
			Fill(106,"");//4th Patient Address Line
			//proc 1
			Fill(107,"P1Date","MM/dd/yyyy");
			Fill(108,"P1ToothNumber");
			Fill(109,"P1Surface");
			Fill(110,"");//diag index
			Fill(111,"P1Code");
			Fill(112,"");//quantity
			Fill(113,"P1Description");
			Fill(114,"P1Fee");
			//proc 2
			Fill(115,"P2Date","MM/dd/yyyy");
			Fill(116,"P2ToothNumber");
			Fill(117,"P2Surface");
			Fill(118,"");
			Fill(119,"P2Code");
			Fill(120,"");
			Fill(121,"P2Description");
			Fill(122,"P2Fee");
			//proc 3
			Fill(123,"P3Date","MM/dd/yyyy");
			Fill(124,"P3ToothNumber");
			Fill(125,"P3Surface");
			Fill(126,"");
			Fill(127,"P3Code");
			Fill(128,"");
			Fill(129,"P3Description");
			Fill(130,"P3Fee");
			//proc 4
			Fill(131,"P4Date","MM/dd/yyyy");
			Fill(132,"P4ToothNumber");
			Fill(133,"P4Surface");
			Fill(134,"");
			Fill(135,"P4Code");
			Fill(136,"");
			Fill(137,"P4Description");
			Fill(138,"P4Fee");
			//proc 5
			Fill(139,"P5Date","MM/dd/yyyy");
			Fill(140,"P5ToothNumber");
			Fill(141,"P5Surface");
			Fill(142,"");
			Fill(143,"P5Code");
			Fill(144,"");
			Fill(145,"P5Description");
			Fill(146,"P5Fee");
			//proc 6
			Fill(147,"P6Date","MM/dd/yyyy");
			Fill(148,"P6ToothNumber");
			Fill(149,"P6Surface");
			Fill(150,"");
			Fill(151,"P6Code");
			Fill(152,"");
			Fill(153,"P6Description");
			Fill(154,"P6Fee");
			//proc 7
			Fill(155,"P7Date","MM/dd/yyyy");
			Fill(156,"P7ToothNumber");
			Fill(157,"P7Surface");
			Fill(158,"");
			Fill(159,"P7Code");
			Fill(160,"");
			Fill(161,"P7Description");
			Fill(162,"P7Fee");
			//proc 8
			Fill(163,"P8Date","MM/dd/yyyy");
			Fill(164,"P8ToothNumber");
			Fill(165,"P8Surface");
			Fill(166,"");
			Fill(167,"P8Code");
			Fill(168,"");
			Fill(169,"P8Description");
			Fill(170,"P8Fee");
			//end of procs
			Fill(171,"TotalFee");
			Fill(172,"");//payment by other plan. Only applicable on secondary insurance.
			Fill(173,"");//max allowable
			Fill(174,"");//deductible
			Fill(175,"");//carrier percent
			Fill(176,"");//carrier pays
			Fill(177,"");//patient pays
			Fill(178,"");//Work injury.  Only x is accepted.
			Fill(179,"P1Area");
			Fill(180,"");//Tooth System 1- Will accept any text string
			Fill(181,"P2Area");
			Fill(182,"");//Tooth System 2- Will accept any text string
			Fill(183,"P3Area");
			Fill(184,"");//Tooth System 3- Will accept any text string
			Fill(185,"P4Area");
			Fill(186,"");//Tooth System 4- Will accept any text string
			Fill(187,"P5Area");
			Fill(188,"");//Tooth System 5- Will accept any text string
			Fill(189,"P6Area");
			Fill(190,"");//Tooth System 6- Will accept any text string
			Fill(191,"P7Area");
			Fill(192,"");//Tooth System 7- Will accept any text string
			Fill(193,"P8Area");
			Fill(194,"");//Tooth System 8- Will accept any text string
			Fill(195,"P9Date","MM/dd/yyyy");
			Fill(196,"P9Area");
			Fill(197,"");//Tooth System 9- Will accept any text string
			Fill(198,"P9ToothNumber");
			Fill(199,"P9Surface");
			Fill(200,"P9Code");
			Fill(201,"P9Description");
			Fill(202,"P9Fee");
			Fill(203,"P10Date","MM/dd/yyyy");
			Fill(204,"P10Area");
			Fill(205,"");//Tooth System 10- Will accept any text string
			Fill(206,"P10ToothNumber");
			Fill(207,"P10Surface");
			Fill(208,"P10Code");
			Fill(209,"P10Description");
			Fill(210,"P10Fee");
			Fill(211,"TreatingDentistProviderID");//Treating/Rending Provider ID- Will accept any text string
			Fill(212,"");//COB Relationship To Subscriber (Self) - Only 'X' will be accepted
			Fill(213,"");//COB Relationship To Subscriber (Spouse) - Only 'X' will be accepted
			Fill(214,"");//COB Relationship To Subscriber (Child) - Only 'X' will be accepted
			Fill(215,"");//COB Relationship To Subscriber (Other) - Only 'X' will be accepted
			Fill(216,"OtherInsCarrierName");//COB insurance company name
			Fill(217,"OtherInsAddress");//COB address
			Fill(218,"OtherInsCity");//COB ins City
			Fill(219,"OtherInsST");//COB ins State
			Fill(220,"OtherInsZip");//COB Zip
			Fill(221,"");//Accident ST - Only 2 character state abbreviation.
			Fill(222,"");
			Fill(223,"");
			Fill(224,"");
			Fill(225,"");
			Fill(226,"");
			Fill(227,"");
			Fill(228,"");
			Fill(229,"");
			Fill(230,"Remarks");//Remarks (Such as NEA numbers etc.) - Will accept any text string
			Fill(231,"");
			Fill(232,"");//Excellus Additional Info Field 
			Fill(233,"");//COB Primary Payment Date- Expecting MM/DD/YYYY
			Fill(234,"TreatingDentistSignature");//Treating Dentist Signature - Will accept any text string
			Fill(235,"TreatingDentistLicense");//Treating License # - Will accept any text string
			Fill(236,"TreatingDentistSigDate");//Date Signed - Expecting MM/DD/YYYY
			Fill(237,"TreatingDentistAddress");//Address Where Procedure Performed - Will accept any text string
			Fill(238,"TreatingDentistCity");//City Where Procedure Performed - Will accept any text string
			Fill(239,"TreatingDentistST");//State Where Procedure Performed - Will accept any text string
			Fill(240,"TreatingDentistZip");//Zip Code Where Procedure Performed - Expecting 5 digit
			DisplayStrings=fillRenaissance(claimNum,patNum,items.ToList());
			SaveFile(batchNum);
			return true;
		}

		private static void Fill(int index,string fieldName,string formatString){
			if(fieldName!=""){
				for(int i=0;i<FormCFI.FieldNames.Length;i++){
					if(FormCFI.FieldNames[i]==fieldName){
						break;
					}
					if(i==FormCFI.FieldNames.Length-1){
						MessageBox.Show(fieldName+" is not valid");
						return;
					}
				}
			}
			items[index]=new ClaimFormItem();
			items[index].FieldName=fieldName;
			items[index].FormatString=formatString;
		}

		private static void Fill(int index,string fieldName){
			Fill(index,fieldName,"");
		}

		private static void SaveFile(int batchNum){
			//this actually gets the current batch number since it was already incremented
			//int batchNum=PIn.PInt(((Pref)PrefC.HList["RenaissanceLastBatchNumber"]).ValueString);
			for(int i=0;i<DisplayStrings.GetLength(0);i++){//usually 1, but sometimes 2 or 3
				string uploadPath=_clearinghouseClin.ExportPath;//@"C:\Program Files\Renaissance\dotr\upload\";
				if(!Directory.Exists(uploadPath)){
					MessageBox.Show("Error. Renaissance not installed.  "+uploadPath+" not valid");
					return;
				}
				int fileEnd=1;
				string fileName;
				try {
					do{//loop to find the next available filename
						fileName="C"+batchNum.ToString().PadLeft(3,'0')
							+"C"+fileEnd.ToString().PadLeft(3,'0')+".rss";
						fileEnd++;
					}
					while(File.Exists(uploadPath+fileName));//Since this is a windows program, no need to use ODFileUtils.CombinePaths()
					using (StreamWriter sw = new StreamWriter(uploadPath+fileName)){
						for(int ii=1;ii<DisplayStrings[i].Length;ii++){
							sw.WriteLine(ii.ToString().PadLeft(3,'0')+":"+DisplayStrings[i][ii]);
						}
					}
				}
				catch (Exception ex) {
					MessageBox.Show(Lans.g("Renaissance","An error occurred while saving this document:")+"\n"+ex.Message);
				}
			}
		}

		///<summary>Returns a string describing all missing data on this claim.  Claim will not be allowed to be sent electronically unless this string comes back empty.</summary>
		public static string GetMissingData(ClaimSendQueueItem queueItem){
			//Our support for Renaissance is minimal, because they do not use the X12 format and we do not recommend that our customers use it.
			//Thus, we do not perform validation.
			return "";
		}




	}

	public interface IFormClaimFormItemEdit {
		void FillFieldNames();
		string[] FieldNames { get; set; }
	}
	
}
