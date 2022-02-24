using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using CodeBase;
using System.Windows.Forms;

namespace OpenDentBusiness.Shared {
	public class Sirona {
		///<summary>Func that abstracts Lans.g.</summary>
		public static Func<string,string,string> Lans_g=(sender,text) => text;

		[DllImport("kernel32")]//this is the windows function for writing to ini files.
		private static extern long WritePrivateProfileString(string section,string key,string val,string filePath);

		[DllImport("kernel32")]//this is the windows function for reading from ini files.
		private static extern int GetPrivateProfileString(string section,string key,string def,StringBuilder retVal,int size,string filePath);

		public static void SendData(string pathExe,List<string> listIniLines) {
			WriteToSendBoxFile(pathExe,listIniLines);
			Process.Start(pathExe);
		}

		public static void WriteToSendBoxFile(string pathExe,List<string> listIniLines) {
			#region Read / Write .ini
			//read file C:\sidexis\sifiledb.ini
			string iniFile=Path.GetDirectoryName(pathExe)+"\\sifiledb.ini";
			if(!File.Exists(iniFile)) {
				throw new ODException(iniFile+" "+Lans_g("Sirona","could not be found. Is Sidexis installed properly?"));
			}
			//read FromStation0 | File to determine location of comm file (sendBox) (siomin.sdx)
			//example:
			//[FromStation0]
			//File=F:\PDATA\siomin.sdx  //only one sendBox on entire network.
			StringBuilder retVal=new StringBuilder(255);
			GetPrivateProfileString("FromStation0","File","",retVal,255,iniFile);
			string sendBox=retVal.ToString();
			//read Multistations | GetRequest (=1) to determine if station can take xrays.
			//but we don't care at this point, so ignore
			//set OfficeManagement | OffManConnected = 1 to make sidexis ready to accept a message.
			WritePrivateProfileString("OfficeManagement","OffManConnected","1",iniFile);
			if(ODBuild.IsDebug() && sendBox.IsNullOrEmpty()) {
				sendBox=@"C:\Bridges\Sirona\iniFile.ini";
			}
			#endregion
			using FileStream fs=new FileStream(sendBox,FileMode.Append);
			using BinaryWriter bw=new BinaryWriter(fs);
			for(int i=0;i<listIniLines.Count;i++) {
				string line=line=listIniLines[i];
				if(ODBuild.IsWeb()) {
					line=line.Replace("{{SystemInformation.ComputerName}}",SystemInformation.ComputerName);
				}
				//Only write the first two bytes to preserve old behavior.
				byte[] arrayBytes=BitConverter.GetBytes(line.Length+2);//the 2 accounts for these two chars.
				bw.Write(arrayBytes[0]);
				bw.Write(arrayBytes[1]);
				bw.Write(StrToBytes(line));
			}
		}

		private static byte[] StrToBytes(string str) {
			byte[] retVal=new byte[str.Length];
			for(int i=0;i<retVal.Length;i++) {
				switch(str[i]) {
					default:
						retVal[i]=(byte)str[i];
						break;
					case '\u00C7'://C,
						retVal[i]=128;
						break;
					case '\u00FC'://u"
						retVal[i]=129;
						break;
					case '\u00E9'://e'
						retVal[i]=130;
						break;
					case '\u00E2'://a^
						retVal[i]=131;
						break;
					case '\u00E4'://a"
						retVal[i]=132;
						break;
					case '\u00E0'://a`
						retVal[i]=133;
						break;
					case '\u00E5'://ao
						retVal[i]=134;
						break;
					case '\u00E7'://c,
						retVal[i]=135;
						break;
					case '\u00EA'://e^
						retVal[i]=136;
						break;
					case '\u00EB'://e"
						retVal[i]=137;
						break;
					case '\u00E8'://e`
						retVal[i]=138;
						break;
					case '\u00EF'://i"
						retVal[i]=139;
						break;
					case '\u00EE'://i^
						retVal[i]=140;
						break;
					case '\u00EC'://i`
						retVal[i]=141;
						break;
					case '\u00C4'://A"
						retVal[i]=142;
						break;
					case '\u00C5'://Ao
						retVal[i]=143;
						break;
					case '\u00C9'://E'
						retVal[i]=144;
						break;
					case '\u00E6'://ae
						retVal[i]=145;
						break;
					case '\u00C6'://AE
						retVal[i]=146;
						break;
					case '\u00F4'://o^
						retVal[i]=147;
						break;
					case '\u00F6'://o"
						retVal[i]=148;
						break;
					case '\u00F2'://o`
						retVal[i]=149;
						break;
					case '\u00FB'://u^
						retVal[i]=150;
						break;
					case '\u00F9'://u`
						retVal[i]=151;
						break;
					case '\u00FF'://y"
						retVal[i]=152;
						break;
					case '\u00D6'://O"
						retVal[i]=153;
						break;
					case '\u00DC'://U"
						retVal[i]=154;
						break;
					//skipped 155 through 159
					case '\u00E1'://a'
						retVal[i]=160;
						break;
					case '\u00ED'://i'
						retVal[i]=161;
						break;
					case '\u00F3'://o'
						retVal[i]=162;
						break;
					case '\u00FA'://u'
						retVal[i]=163;
						break;
					case '\u00F1'://n~
						retVal[i]=164;
						break;
					case '\u00D1'://N~
						retVal[i]=165;
						break;
				}
			}
			return retVal;
		}

	}
}
