using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental.Bridges {
	/// <summary></summary>
	public class Guru {

		[DllImport("MedVisorInterface.dll")]
		private static extern int MVStart();

		//[DllImport("MedVisorInterface.dll")]
		//private static extern bool MVGetRunningState();

		[DllImport("MedVisorInterface.dll")]
		private static extern int MVSendPatient(IntPtr mvPatient);
		private static int MVSendPatient(MVPatient mvPatient) {
			IntPtr patStruct=ToCPlusPlusStructure(mvPatient);
			int retVal=MVSendPatient(patStruct);
			DestroyCPlusPlusStruct(patStruct);
			return retVal;
		}

		///<summary>This class is defined with a structural layout, so that it can be properly marshaled to a C-code structure.</summary>
		[StructLayout(LayoutKind.Sequential,Pack=8)]
		public class MVPatient {
			public char[] LastName=new char[65];
			public char[] MiddleName=new char[65];
			public char[] FirstName=new char[65];
			public char[] Prefix=new char[65];
			public char[] Suffix=new char[65];
			public char[] Sex=new char[2];
			public char[] BirthDate=new char[9];
			public char[] ID=new char[65];
			public char[] Directory=new char[260];
		}

		/// <summary></summary>
		public Guru() {

		}

		///<summary>CRITICAL FUNCTION: Creates a converted copy of this object into an unmanaged c++ structure (including the virutal function table) which is compatible with MedVisorInterface.dll module and returns the address to the newly created structure. The newly created structure will point to the same image data as the managed/local structure does. Finally, the structure which is returned (pointed to) must be freed using DesctroyCPlusPlusStructure(), or else there will be a memory leak. This function was designed to work on 32-bit and 64-bit operating systems.</summary>
		private static IntPtr ToCPlusPlusStructure(MVPatient mvPatient) {
			int size=sizeof(char)*(65+65+65+65+65+2+9+65+260);//The size of the unmanaged structure.
			IntPtr retVal=Marshal.AllocHGlobal(size);//The location where the unmanaged structure will rest.
			unsafe {//unmanaged code section.
				byte* copyLoc=(byte*)retVal.ToPointer();

				////__vfptr (virtual function table pointer).
				////The layout of the virtual function table as expected by MedVisorInterface.dll was discovered by running the
				////sen4 example project (provided by Suni) in debug mode and viewing one of the CImageData object at runtime.
				//IntPtr vfptr=Marshal.AllocHGlobal(3*Marshal.SizeOf(typeof(IntPtr)));
				//byte* vtp=(byte*)vfptr.ToPointer();
				////The pointer to ~CImageData() (will not be called by any native function calls to SuniUSB.dll, 
				////so no need to define it). Always set to zero just to fill the space.
				//Marshal.Copy(new IntPtr[] { IntPtr.Zero },0,(IntPtr)vtp,1);
				//vtp+=Marshal.SizeOf(typeof(IntPtr));
				////PutPixel() pointer
				//IntPtr putPixelPtr=Marshal.GetFunctionPointerForDelegate(ppd);
				//Marshal.Copy(new IntPtr[] { putPixelPtr },0,(IntPtr)vtp,1);
				//vtp+=Marshal.SizeOf(putPixelPtr);
				////GetPixel() pointer
				//IntPtr getPixelPtr=Marshal.GetFunctionPointerForDelegate(gpd);
				//Marshal.Copy(new IntPtr[] { getPixelPtr },0,(IntPtr)vtp,1);
				//vtp+=Marshal.SizeOf(getPixelPtr);
				////End of virtual function table definition.
				////Virtual table
				//Marshal.Copy(new IntPtr[] { vfptr },0,(IntPtr)copyLoc,1);
				//copyLoc+=Marshal.SizeOf(vfptr);

				//LastName
				size=65;
				Marshal.Copy(mvPatient.LastName,0,(IntPtr)copyLoc,size);
				copyLoc+=size;
				//MiddleName
				size=65;
				Marshal.Copy(mvPatient.MiddleName,0,(IntPtr)copyLoc,size);
				copyLoc+=size;
				//FirstName
				size=65;
				Marshal.Copy(mvPatient.FirstName,0,(IntPtr)copyLoc,size);
				copyLoc+=size;
				//Prefix
				size=65;
				Marshal.Copy(mvPatient.Prefix,0,(IntPtr)copyLoc,size);
				copyLoc+=size;
				//Suffix
				size=65;
				Marshal.Copy(mvPatient.Suffix,0,(IntPtr)copyLoc,size);
				copyLoc+=size;
				//Sex
				size=2;
				Marshal.Copy(mvPatient.Sex,0,(IntPtr)copyLoc,size);
				copyLoc+=size;
				//BirthDate
				size=9;
				Marshal.Copy(mvPatient.BirthDate,0,(IntPtr)copyLoc,size);
				copyLoc+=size;
				//ID
				size=65;
				Marshal.Copy(mvPatient.ID,0,(IntPtr)copyLoc,size);
				copyLoc+=size;
				//Directory
				size=260;
				Marshal.Copy(mvPatient.Directory,0,(IntPtr)copyLoc,size);
				copyLoc+=size;
			}
			return retVal;
		}

		///<summary>Destroys a c++ structure created by ToCPlusPlusStructure().</summary>
		public static void DestroyCPlusPlusStruct(IntPtr cPlusPlusStruct) {
			Marshal.FreeHGlobal(cPlusPlusStruct);//Free the structure itself.
		}

		/// <summary></summary>
		public static void SendData(Program ProgramCur,Patient pat) {
			if(pat==null) {
				MsgBox.Show("Guru","Please select a patient first.");
				return;
			}
			try {
				int errorNum=MVStart();
				if(errorNum!=0) {
					/* Error codes:
					MV_SUCCESS if successful
					MV_WRONG_PARAM if hPMS is not null and pContextString is null
					MV_WRONG_PARAM if pContextString is length 0 or is not null terminated within strSize characters
					MV_ERROR if an unknown error occurred

					Enumeration Name - E_MV_ERROR
					This enumeration defines the different return values of exported functions.  When successful, functions return MV_SUCCESS, an other value otherwise:  see the full list below.
					MV_SUCCESS - The function has succeeded
					MV_NOT_RUNNING - MV is not running
					MV_NOT_CREATED - MedVisor has not been created
					MV_NO_PATIENT - No patient has been set in MedVisor
					MV_REQUIRED_DATA - Required data has not been sent
					MV_WRONG_VALUE - Send data are not valid
					MV_FILE_NOT_FOUND - Sent file cannot be found
					MV_ERROR - General Error
					*/
					throw new ODException(Lan.g("Guru","MedVisorInterface.MVStart() returned with an error code of:")+$" {errorNum}");
				}
			}
			catch(DllNotFoundException e) {
				e.DoNothing();
				MessageBox.Show(Lans.g("Guru","Could not find MedVisorInterface.dll. Verify that Guru is installed."));
				return;
			}
			catch(Exception ex) {
				FriendlyException.Show(Lans.g("Guru","An error occurred when launching Guru."),ex);
				return;
			}
			MVPatient mvPatient = new MVPatient();
			mvPatient.LastName = Tidy(pat.LName,64);
			mvPatient.FirstName = Tidy(pat.FName,64);
			if(pat.Gender == PatientGender.Male) {
				mvPatient.Sex = Tidy("M",1);
			}
			else if(pat.Gender == PatientGender.Female) {
				mvPatient.Sex = Tidy("F",1);
			}
			else if(pat.Gender == PatientGender.Unknown) {
				mvPatient.Sex = Tidy("0",1);
			}
			mvPatient.BirthDate = Tidy(pat.Birthdate.ToString("MMddyyyy"),8);
			if(ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"Enter 0 to use PatientNum, or 1 to use ChartNum")=="0") {
				mvPatient.ID=Tidy(pat.PatNum.ToString(),64);
			}
			else {
				mvPatient.ID=Tidy(pat.ChartNumber.ToString(),64);
			}
			if(pat.ImageFolder=="") {//Could happen if the images module has not been visited for a new patient.
				Patient patOld=pat.Copy();
				pat.ImageFolder=ImageStore.GetImageFolderName(pat);
				Patients.Update(pat,patOld);
			}
			string imagePath=CodeBase.ODFileUtils.CombinePaths(ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"Guru image path"),pat.ImageFolder);
			mvPatient.Directory = Tidy(imagePath,259);
			if(MVSendPatient(mvPatient) != 0) {
				MsgBox.Show("Guru","An error has occurred.");
			}
		}
			//end


			//**********************************************************
			/*
			//It's common to build a string
			string str="";
			//ProgramProperties.GetPropVal() is the way to get program properties.
			if(ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"Enter 0 to use PatientNum, or 1 to use ChartNum")=="0") {
				str+="Id="+pat.PatNum.ToString()+" ";
			}
			else {
				str+="Id="+Tidy(pat.ChartNumber)+" ";
			}
			//Nearly always tidy the names in one way or another
			str+=Tidy(pat.LName)+" ";
			//If birthdates are optional, only send them if they are valid.
			if(pat.Birthdate.Year>1880) {
				str+=pat.Birthdate.ToString("MM/dd/yyyy");
			}
			//This patterns shows a way to handle gender unknown when gender is optional.
			if(pat.Gender==PatientGender.Female) {
				str+="F ";
			}
			else if(pat.Gender==PatientGender.Male) {
				str+="M ";
			}
			try {
				Process.Start(path,str);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}	 
		}*/

		///<summary>Takes the input and modifies the length to the specified length. Appends a null, and returns the result as a character array.</summary>
		private static char[] Tidy(string input,int length) {
			string retVal=input;
			if(input.Length > length) {
				retVal=input.Substring(0,length);
			}
			if(input.Length < length) {
				retVal=input.PadRight(length,'\0');
			}
			retVal+='\0';
			return retVal.ToCharArray();
		}

		/////<summary>Defines the different return values of exported functions.</summary>
		//public enum E_MV_ERROR {
		//  ///<summary>The function has succeeded.</summary>
		//  MV_SUCCESS,
		//  ///<summary>MV is not running.</summary>
		//  MV_NOT_RUNNING,
		//  ///<summary>MedVisor has not been created.</summary>
		//  MV_NOT_CREATED,
		//  ///<summary>No patient has been set in MedVisor.</summary>
		//  MV_NO_PATIENT,
		//  ///<summary>Required data has not been sent.</summary>
		//  MV_REQUIRED_DATA,
		//  ///<summary>Send data are not valid.</summary>
		//  MV_WRONG_VALUE,
		//  ///<summary>Sent file cannot be found.</summary>
		//  MV_FILE_NOT_FOUND,
		//  ///<summary>General Error.</summary>
		//  MV_ERROR,
		//}
	}
}

/*
			//Insert Guru Bridge
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				    +") VALUES("
				    +"'Guru', "
				    +"'Guru from guru.waziweb.com', "
				    +"'0', "
				    +"'"+POut.String(@"C:\Program Files (x86)\AaTemplate\AaTemplate.exe")+"',"
				    +"'', "
				    +"'')";
					long programNum=Db.NonQ(command,true);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				    +") VALUES("
				    +"'"+POut.Long(programNum)+"', "
				    +"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
				    +"'0')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				    +") VALUES("
				    +"'"+POut.Long(programNum)+"', "
				    +"'Guru image path', "
				    +"'C:\')";
					Db.NonQ(command);
					command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
				    +"VALUES ("
				    +"'"+POut.Long(programNum)+"', "
				    +"'"+POut.Int(((int)ToolBarsAvail.ChartModule))+"', "
				    +"'Guru')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				    +") VALUES("
				    +"(SELECT MAX(ProgramNum)+1 FROM program),"
				    +"'Guru', "
				    +"'Guru from guru.waziweb.com/', "
				    +"'0', "
				    +"'"+POut.String(@"C:\Program Files (x86)\AaTemplate\AaTemplate.exe")+"',"
				    +"'', "
				    +"'')";
					long programNum=Db.NonQ(command,true);
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
				    +") VALUES("
				    +"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
				    +"'"+POut.Long(programNum)+"', "
				    +"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
				    +"'0')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
				    +") VALUES("
				    +"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
				    +"'"+POut.Long(programNum)+"', "
				    +"'Guru image path', "
				    +"'C:\')";
					Db.NonQ(command);
					command="INSERT INTO toolbutitem (ToolButItemNum,ProgramNum,ToolBar,ButtonText) "
				    +"VALUES ("
				    +"(SELECT MAX(ToolButItemNum)+1 FROM toolbutitem),"
				    +"'"+POut.Long(programNum)+"', "
				    +"'"+POut.Int(((int)ToolBarsAvail.ChartModule))+"', "
				    +"'Guru')";
					Db.NonQ(command);
				}//end Guru bridge
*/