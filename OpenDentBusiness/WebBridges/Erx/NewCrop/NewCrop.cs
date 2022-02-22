using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CodeBase;

namespace OpenDentBusiness {

	public class NewCrop {

		public static string NewCropPartnerName {
			get {
				string newCropName=PrefC.GetString(PrefName.NewCropName);
				if(newCropName!="") { //Distributors use this field to send different credentials. Thus, if blank, then send OD credentials.
					return PrefC.GetString(PrefName.NewCropPartnerName);//Distributor
				}
				if(PrefC.GetBool(PrefName.NewCropIsLexiData)) {
					return "OpenDentalLexi";
				}
				else {//First Data Bank (FDB) customers.
					return "OpenDental";
				}
			}
		}

		public static string NewCropAccountName {
			get {
				string newCropName=PrefC.GetString(PrefName.NewCropName);
				if(newCropName!="") { //Distributors use this field to send different credentials. Thus, if blank, then send OD credentials.
					return newCropName;//Distributor
				}
				if(ODBuild.IsDebug()) {
					return CodeBase.MiscUtils.Decrypt("Xv40GArhEXYjEZxAE3Fw9g==");//Assigned by NewCrop. Used globally for all customers.
				}
				else {
					//Assigned by NewCrop.  Used globally for all customers.
					if(PrefC.GetBool(PrefName.NewCropIsLexiData)) {
						return CodeBase.MiscUtils.Decrypt("22kFdicmXHDom20yKqaLIJAo0iq9fGvQnjjyV7Pb3Pn51zo0gjs0/h8eWNyCPO68");
					}
					else {//First Data Bank (FDB) customers.
						return CodeBase.MiscUtils.Decrypt("HumacKlUtM1MLCHsZY/PH86W10AY5u2bukFp15lEKhT6zD/aa9nG//zYzbYgpH8+");
					}
				}
			}
		}

		public static string NewCropAccountPasssword {
			get {
				string newCropName=PrefC.GetString(PrefName.NewCropName);
				if(newCropName!="") { //Distributors use this field to send different credentials. Thus, if blank, then send OD credentials.
					return PrefC.GetString(PrefName.NewCropPassword);//Distributor
				}
				if(ODBuild.IsDebug()) {
					return CodeBase.MiscUtils.Decrypt("Xv40GArhEXYjEZxAE3Fw9g==");//Assigned by NewCrop. Used globally for all customers.
				}
				else {
					//Assigned by NewCrop. Used globally for all customers.
					if(PrefC.GetBool(PrefName.NewCropIsLexiData)) {
						return CodeBase.MiscUtils.Decrypt("tv9uB38IYv1dRddpVgJjDYD9JlEpPhWd3VpmXd9KtpS7DkOxUdYt8ggS+tFZeYsv");
					}
					else {//First Data Bank (FDB) customers.
						return CodeBase.MiscUtils.Decrypt("I0Itlo5F3ZOYUSwMKpgbY5X6++XpUetMvrqj0vVB7bKzYwJlWtsLiFFgpMBplLaH");
					}
				}
			}
		}

		public static string NewCropProductName {
			get { return "OpenDental"; }
		}

		public static string NewCropProductVersion {
			get { return Assembly.GetAssembly(typeof(Db)).GetName().Version.ToString(); }
		}

		public static PersonNameType GetPersonNameForProvider(Provider prov) {
			//No need to check RemotingRole; no call to db.
			PersonNameType personName=new PersonNameType();
			personName.last=prov.LName.Trim();//Cannot be blank.
			personName.first=prov.FName.Trim();//Cannot be blank.
			personName.middle=prov.MI;//May be blank.
			if(prov.Suffix!="") {
				personName.suffixSpecified=true;
				personName.suffix=PersonNameSuffix.DDS;
				string[] suffixes=prov.Suffix.ToUpper().Split(' ','.');
				for(int i = 0;i<suffixes.Length;i++) {
					if(suffixes[i]=="ABOC") {
						personName.suffix=PersonNameSuffix.ABOC;
						break;
					}
					else if(suffixes[i]=="ANP") {
						personName.suffix=PersonNameSuffix.ANP;
						break;
					}
					else if(suffixes[i]=="APRN") {
						personName.suffix=PersonNameSuffix.APRN;
						break;
					}
					else if(suffixes[i]=="ARNP") {
						personName.suffix=PersonNameSuffix.ARNP;
						break;
					}
					else if(suffixes[i]=="CNM") {
						personName.suffix=PersonNameSuffix.CNM;
						break;
					}
					else if(suffixes[i]=="CNP") {
						personName.suffix=PersonNameSuffix.CNP;
						break;
					}
					else if(suffixes[i]=="CNS") {
						personName.suffix=PersonNameSuffix.CNS;
						break;
					}
					else if(suffixes[i]=="CRNP") {
						personName.suffix=PersonNameSuffix.CRNP;
						break;
					}
					else if(suffixes[i]=="DDS") {
						personName.suffix=PersonNameSuffix.DDS;
						break;
					}
					else if(suffixes[i]=="DMD") {
						personName.suffix=PersonNameSuffix.DMD;
						break;
					}
					else if(suffixes[i]=="DO") {
						personName.suffix=PersonNameSuffix.DO;
						break;
					}
					else if(suffixes[i]=="DPM") {
						personName.suffix=PersonNameSuffix.DPM;
						break;
					}
					else if(suffixes[i]=="ESQ") {//ESQ or Esq
						personName.suffix=PersonNameSuffix.ESQ;
						break;
					}
					else if(suffixes[i]=="ESQ1") {//ESQ1 or Esq1
						personName.suffix=PersonNameSuffix.ESQ1;
						break;
					}
					else if(suffixes[i]=="FACC") {
						personName.suffix=PersonNameSuffix.FACC;
						break;
					}
					else if(suffixes[i]=="FACP") {
						personName.suffix=PersonNameSuffix.FACP;
						break;
					}
					else if(suffixes[i]=="FNP") {
						personName.suffix=PersonNameSuffix.FNP;
						break;
					}
					else if(suffixes[i]=="GNP") {
						personName.suffix=PersonNameSuffix.GNP;
						break;
					}
					else if(suffixes[i]=="I") {
						personName.suffix=PersonNameSuffix.I;
						break;
					}
					else if(suffixes[i]=="II") {
						personName.suffix=PersonNameSuffix.II;
						break;
					}
					else if(suffixes[i]=="III") {
						personName.suffix=PersonNameSuffix.III;
						break;
					}
					else if(suffixes[i]=="IV") {
						personName.suffix=PersonNameSuffix.IV;
						break;
					}
					else if(suffixes[i]=="JR") {//JR or jr
						personName.suffix=PersonNameSuffix.Jr;
						break;
					}
					else if(suffixes[i]=="JR1") {//JR1 or jr1
						personName.suffix=PersonNameSuffix.Jr;
						break;
					}
					else if(suffixes[i]=="LPN") {
						personName.suffix=PersonNameSuffix.LPN;
						break;
					}
					else if(suffixes[i]=="LVN") {
						personName.suffix=PersonNameSuffix.LVN;
						break;
					}
					else if(suffixes[i]=="MA") {
						personName.suffix=PersonNameSuffix.MA;
						break;
					}
					else if(suffixes[i]=="MD") {
						personName.suffix=PersonNameSuffix.MD;
						break;
					}
					else if(suffixes[i]=="NB") {
						personName.suffix=PersonNameSuffix.NB;
						break;
					}
					else if(suffixes[i]=="ND") {
						personName.suffix=PersonNameSuffix.ND;
						break;
					}
					else if(suffixes[i]=="NP") {
						personName.suffix=PersonNameSuffix.NP;
						break;
					}
					else if(suffixes[i]=="OD") {
						personName.suffix=PersonNameSuffix.OD;
						break;
					}
					else if(suffixes[i]=="PA") {
						personName.suffix=PersonNameSuffix.PA;
						break;
					}
					else if(suffixes[i]=="PAC") {
						personName.suffix=PersonNameSuffix.PAC;
						break;
					}
					else if(suffixes[i]=="PHARMD") {//PARMD or PharmD
						personName.suffix=PersonNameSuffix.PharmD;
						break;
					}
					else if(suffixes[i]=="PHD") {//PHD or PhD
						personName.suffix=PersonNameSuffix.PhD;
						break;
					}
					else if(suffixes[i]=="PNP") {
						personName.suffix=PersonNameSuffix.PNP;
						break;
					}
					else if(suffixes[i]=="RD") {
						personName.suffix=PersonNameSuffix.RD;
						break;
					}
					else if(suffixes[i]=="RN") {
						personName.suffix=PersonNameSuffix.RN;
						break;
					}
					else if(suffixes[i]=="RPAC") {
						personName.suffix=PersonNameSuffix.RPAC;
						break;
					}
					else if(suffixes[i]=="RPH") {//RPH or RPh
						personName.suffix=PersonNameSuffix.RPh;
						break;
					}
					else if(suffixes[i]=="SR") {//SR or Sr
						personName.suffix=PersonNameSuffix.Sr;
						break;
					}
					else if(suffixes[i]=="SR1") {//SR1 or Sr1
						personName.suffix=PersonNameSuffix.Sr1;
						break;
					}
					else if(suffixes[i]=="V") {
						personName.suffix=PersonNameSuffix.V;
						break;
					}
					else if(suffixes[i]=="VI") {
						personName.suffix=PersonNameSuffix.VI;
						break;
					}
				}
			}
			return personName;
		}
	}
}
