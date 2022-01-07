using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using CodeBase;

namespace OpenDentBusiness{

	///<summary>Import functions in this class should typically be called from a worker thread.</summary>
	public class CodeSystems{

		public delegate void ProgressArgs(int numTotal,int numDone);

		///<summary>Returns a list of code systems in the code system table.  This query will change from version to version depending on what code systems we have available.</summary>
		public static List<CodeSystem> GetForCurrentVersion(bool IsMemberNation) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<CodeSystem>>(MethodBase.GetCurrentMethod(),IsMemberNation);
			}
			string command="";
			if(ODBuild.IsDebug()) {
				command="SELECT * FROM codesystem";// WHERE CodeSystemName IN ('ICD9CM','RXNORM','SNOMEDCT','CPT')";
			}
			else {
				command="SELECT * FROM codesystem WHERE CodeSystemName NOT IN ('AdministrativeSex','CDT')";
				if(!PrefC.GetBool(PrefName.ShowFeatureEhr)) {//When EHR is disabled, only show code systems which are not EHR specific. 
					command+=" AND CodeSystemName IN ('CPT','ICD10CM','ICD9CM','RXNORM','SNOMEDCT','CDCREC')";//Snomed used for drug/problem interactions
				}
			}
			return Crud.CodeSystemCrud.SelectMany(command);
		}

/////<summary>Returns a list of code systems in the code system table.  This query will change from version to version depending on what code systems we have available.</summary>
		//public static List<CodeSystem> GetForCurrentVersionNoSnomed() {
		//	if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
		//		return Meth.GetObject<List<CodeSystem>>(MethodBase.GetCurrentMethod());
		//	}
		//	//string command="SELECT * FROM codesystem WHERE CodeSystemName!='AdministrativeSex' AND CodeSystemName!='CDT'";
		//	string command="SELECT * FROM codesystem WHERE CodeSystemName IN ('ICD9CM','RXNORM','CPT')";
		//	return Crud.CodeSystemCrud.SelectMany(command);
		//}
		///<summary></summary>
		public static void Update(CodeSystem codeSystem){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),codeSystem);
				return;
			}
			Crud.CodeSystemCrud.Update(codeSystem);
		}

		///<summary>Updates VersionCurrent to the VersionAvail of the codeSystem object passed in. Used by code system importer after successful import.</summary>
		public static void UpdateCurrentVersion(CodeSystem codeSystem) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),codeSystem);
				return;
			}
			codeSystem.VersionCur=codeSystem.VersionAvail;
			Crud.CodeSystemCrud.Update(codeSystem);
		}

		///<summary>Updates VersionCurrent to the versionID passed in. Used by code system importer after successful import.  Currently only used for CPT.</summary>
		public static void UpdateCurrentVersion(CodeSystem codeSystem, string versionID) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),codeSystem,versionID);
				return;
			}
			if(string.Compare(codeSystem.VersionCur,versionID)>0) {  //If versionCur is newer than the version you just imported, don't update it.
				return;
			}
			codeSystem.VersionCur=versionID;
			Crud.CodeSystemCrud.Update(codeSystem);
		}

		/////<summary>Called after file is downloaded.  Throws exceptions.</summary>
	//public static void ImportAdministrativeSex(string tempFileName) ... not necessary.

		///<summary>Called after file is downloaded.  Throws exceptions.  It is assumed that this is called from a worker thread.  Progress delegate will be called every 100th iteration to inform thread of current progress. Quit flag can be set at any time in order to quit importing prematurely.</summary>
		public static void ImportCdcrec(string tempFileName,ProgressArgs progress,ref bool quit,ref int numCodesImported,ref int numCodesUpdated,
			bool updateExisting) 
		{
			if(tempFileName==null) {
				return;
			}
			Dictionary<string,Cdcrec> dictCdcrecs=Cdcrecs.GetAll().ToDictionary(x => x.CdcrecCode,x => x);
			string[] lines=File.ReadAllLines(tempFileName);
			string[] arrayCDCREC;
			Cdcrec cdcrec=new Cdcrec();
			for(int i=0;i<lines.Length;i++) {//each loop should read exactly one line of code. and each line of code should be a unique code
				if(quit) {
					return;
				}
				if(i%100==0) {
					progress(i+1,lines.Length);
				}
				arrayCDCREC=lines[i].Split('\t');
				if(dictCdcrecs.ContainsKey(arrayCDCREC[0])) {//code already exists
					cdcrec=dictCdcrecs[arrayCDCREC[0]];
					if(updateExisting &&
						(cdcrec.HeirarchicalCode!=arrayCDCREC[1] 
						|| cdcrec.Description!=arrayCDCREC[2])) 
					{
						cdcrec.HeirarchicalCode=arrayCDCREC[1];
						cdcrec.Description=arrayCDCREC[2];
						Cdcrecs.Update(cdcrec);
						numCodesUpdated++;
					}
					continue;
				}
				cdcrec.CdcrecCode				=arrayCDCREC[0];
				cdcrec.HeirarchicalCode	=arrayCDCREC[1];
				cdcrec.Description			=arrayCDCREC[2];
				Cdcrecs.Insert(cdcrec);
				numCodesImported++;
			}
		}

		/////<summary>Called after file is downloaded.  Throws exceptions.</summary>
	//public static void ImportCDT(string tempFileName) ... not necessary.

		///<summary>Called after user provides resource file.  Throws exceptions.  It is assumed that this is called from a worker thread.  Progress delegate will be called every 100th iteration to inform thread of current progress. Quit flag can be set at any time in order to quit importing prematurely.
		///No UpdateExisting parameter because we force users to accept new descriptions.</summary>
		public static void ImportCpt(string tempFileName,ProgressArgs progress,ref bool quit,ref int numCodesImported,ref int numCodesUpdated,
			string versionID)
		{
			if(tempFileName==null) {
				return;
			}
			Dictionary<string,string> dictCodes=Cpts.GetAll().ToDictionary(x=> x.CptCode,x => x.Description);
			Regex regx=new Regex(@"^([\d]{4}[\d\w])\s+(.+?)$");//Regex = "At the beginning of the string, find five numbers, followed by a white space (tab or space) followed by one or more characters (but as few as possible) to the end of the line."
			string[] lines=File.ReadAllLines(tempFileName);
			string[] arrayCpt;
			bool isHeader=true;
			Cpt cpt=new Cpt();
			for(int i=0;i<lines.Length;i++) {//each loop should read exactly one line of code. and each line of code should be a unique code
				if(quit) {
					return;
				}
				if(i%100==0) {
					progress(i+1,lines.Length);
				}
				if(isHeader) {
					if(!regx.IsMatch(lines[i])) {  					//if(!lines[i].Contains("\t")) {	
						continue;//Copyright info is present at the head of the file.
					}
					isHeader=false;
				}
				arrayCpt=new string[2];
				arrayCpt[0]=regx.Match(lines[i]).Groups[1].Value;//First five alphanumeric characters
				arrayCpt[1]=regx.Match(lines[i]).Groups[2].Value;//Everything after the 6th character
				if(dictCodes.Keys.Contains(arrayCpt[0])) {//code already exists
					Cpts.UpdateDescription(arrayCpt[0],arrayCpt[1],versionID);
					if(dictCodes[arrayCpt[0]]!=arrayCpt[1]) {//The description is different
						numCodesUpdated++;
					}
				}
				else {
					cpt.CptCode			=arrayCpt[0];
					cpt.Description	=arrayCpt[1];
					cpt.VersionIDs	=versionID;
					Cpts.Insert(cpt);
					numCodesImported++;
				}
			}
		}

		///<summary>Called after file is downloaded.  Throws exceptions.  It is assumed that this is called from a worker thread.  Progress delegate will be called every 100th iteration to inform thread of current progress. Quit flag can be set at any time in order to quit importing prematurely.</summary>
		public static void ImportCvx(string tempFileName,ProgressArgs progress,ref bool quit,ref int numCodesImported,ref int numCodesUpdated,
			bool updateExisting) 
		{
			if(tempFileName==null) {
				return;
			}
			Dictionary<string,Cvx> dictCodes=Cvxs.GetAll().ToDictionary(x => x.CvxCode,x => x);
			string[] lines=File.ReadAllLines(tempFileName);
			string[] arrayCvx;
			Cvx cvx=new Cvx();
			for(int i=0;i<lines.Length;i++) {//each loop should read exactly one line of code. and each line of code should be a unique code
				if(quit) {
					return;
				}
				if(i%100==0) {
					progress(i+1,lines.Length);
				}
				arrayCvx=lines[i].Split('\t');
				if(dictCodes.ContainsKey(arrayCvx[0])) {//code already exists
					cvx=dictCodes[arrayCvx[0]];
					if(updateExisting && cvx.Description!=arrayCvx[1]) {//We do want to update and description is different.
						cvx.Description=arrayCvx[1];
						Cvxs.Update(cvx);
						numCodesUpdated++;
					}
					continue;
				}
				cvx.CvxCode			=arrayCvx[0];
				cvx.Description	=arrayCvx[1];
				Cvxs.Insert(cvx);
				numCodesImported++;
			}
		}

		///<summary>Called after file is downloaded.  Throws exceptions.  It is assumed that this is called from a worker thread.  Progress delegate will be called every 100th iteration to inform thread of current progress. Quit flag can be set at any time in order to quit importing prematurely.</summary>
		public static void ImportHcpcs(string tempFileName,ProgressArgs progress,ref bool quit,ref int numCodesImported,ref int numCodesUpdated,
			bool updateExisting) 
		{
			if(tempFileName==null) {
				return;
			}
			Dictionary<string,Hcpcs> dictHcpcs=Hcpcses.GetAll().ToDictionary(x => x.HcpcsCode,x => x);
			string[] lines=File.ReadAllLines(tempFileName);
			string[] arrayHCPCS;
			Hcpcs hcpcs=new Hcpcs();
			for(int i=0;i<lines.Length;i++) {//each loop should read exactly one line of code. and each line of code should be a unique code
				if(quit) {
					return;
				}
				if(i%100==0) {
					progress(i+1,lines.Length);
				}
				arrayHCPCS=lines[i].Split('\t');
				if(dictHcpcs.ContainsKey(arrayHCPCS[0])) {//code already exists
					hcpcs=dictHcpcs[arrayHCPCS[0]];
					if(updateExisting && hcpcs.DescriptionShort!=arrayHCPCS[1]) {
						hcpcs.DescriptionShort=arrayHCPCS[1];
						Hcpcses.Update(hcpcs);
						numCodesUpdated++;
					}
					continue;
				}
				hcpcs.HcpcsCode					=arrayHCPCS[0];
				hcpcs.DescriptionShort	=arrayHCPCS[1];
				Hcpcses.Insert(hcpcs);
				numCodesImported++;
			}
		}

		///<summary>Called after file is downloaded.  Throws exceptions.  It is assumed that this is called from a worker thread.  Progress delegate will be called every 100th iteration to inform thread of current progress. Quit flag can be set at any time in order to quit importing prematurely.</summary>
		public static void ImportIcd10(string tempFileName,ProgressArgs progress,ref bool quit,ref int numCodesImported,ref int numCodesUpdated,
			bool updateExisting) 
		{
			if(tempFileName==null) {
				return;
			}
			Dictionary<string,Icd10> dictIcd10s=Icd10s.GetAll().ToDictionary(x => x.Icd10Code,x => x);
			string[] lines=File.ReadAllLines(tempFileName);
			string[] arrayICD10;
			Icd10 icd10=new Icd10();
			for(int i=0;i<lines.Length;i++) {//each loop should read exactly one line of code. and each line of code should be a unique code
				if(quit) {
					return;
				}
				if(i%100==0) {
					progress(i+1,lines.Length);
				}
				arrayICD10=lines[i].Split('\t');
				if(dictIcd10s.ContainsKey(arrayICD10[0])) {//code already exists
					icd10=dictIcd10s[arrayICD10[0]];
					if(updateExisting && 
						(icd10.Description!=arrayICD10[1] || icd10.IsCode!=arrayICD10[2])) //Code informatin is different
					{
						icd10.Description=arrayICD10[1];
						icd10.IsCode=arrayICD10[2];
						Icd10s.Update(icd10);
						numCodesUpdated++;
					}
					continue;
				}
				icd10.Icd10Code		=arrayICD10[0];
				icd10.Description	=arrayICD10[1];
				icd10.IsCode			=arrayICD10[2];
				Icd10s.Insert(icd10);
				numCodesImported++;
			}
		}

		///<summary>Called after file is downloaded.  Throws exceptions.  It is assumed that this is called from a worker thread.  Progress delegate will be called every 100th iteration to inform thread of current progress. Quit flag can be set at any time in order to quit importing prematurely.</summary>
		public static void ImportIcd9(string tempFileName,ProgressArgs progress,ref bool quit,ref int numCodesImported,ref int numCodesUpdated,
			bool updateExisting) 
		{
			if(tempFileName==null) {
				return;
			}
			//Customers may have an old codeset that has a truncated uppercase description, if so we want to update with new descriptions.
			bool IsOldDescriptions=ICD9s.IsOldDescriptions();
			Dictionary<string,ICD9> dictCodes=ICD9s.GetAll().ToDictionary(x => x.ICD9Code,x => x);
			string[] lines=File.ReadAllLines(tempFileName);
			string[] arrayICD9;
			ICD9 icd9=new ICD9();
			for(int i=0;i<lines.Length;i++) {//each loop should read exactly one line of code. and each line of code should be a unique code
				if(quit) {
					return;
				}
				if(i%100==0) {
					progress(i+1,lines.Length);
				}
				arrayICD9=lines[i].Split('\t');
				if(dictCodes.ContainsKey(arrayICD9[0])) {//code already exists
					icd9=dictCodes[arrayICD9[0]];
					if((IsOldDescriptions || updateExisting) && icd9.Description!=arrayICD9[1]) {//The new description does not match the description in the database.
						icd9.Description=arrayICD9[1];
						ICD9s.Update(icd9);
						numCodesUpdated++;
					}
					continue;
				}
				icd9.ICD9Code		=arrayICD9[0];
				icd9.Description=arrayICD9[1];
				ICD9s.Insert(icd9);
				numCodesImported++;
			}
		}

		///<summary>Called after file is downloaded.  Throws exceptions.  It is assumed that this is called from a worker thread.  Progress delegate will be called every 100th iteration to inform thread of current progress. Quit flag can be set at any time in order to quit importing prematurely.</summary>
		public static void ImportLoinc(string tempFileName,ProgressArgs progress,ref bool quit,ref int numCodesImported,ref int numCodesUpdated,
			bool updateExisting) 
		{
			if(tempFileName==null) {
				return;
			}
			Dictionary<string,Loinc> dictLoincs=Loincs.GetAll().ToDictionary(x => x.LoincCode,x => x);
			string[] lines=File.ReadAllLines(tempFileName);
			string[] arrayLoinc;
			Loinc oldLoinc=new Loinc();
			Loinc newLoinc=new Loinc();
			for(int i=0;i<lines.Length;i++) {//each loop should read exactly one line of code. and each line of code should be a unique code
				if(quit) {
					return;
				}
				if(i%100==0) {
					progress(i+1,lines.Length);
				}
				arrayLoinc=lines[i].Split('\t');
				newLoinc.LoincCode               =arrayLoinc[0];
				newLoinc.Component               =arrayLoinc[1];
				newLoinc.PropertyObserved        =arrayLoinc[2];
				newLoinc.TimeAspct               =arrayLoinc[3];
				newLoinc.SystemMeasured          =arrayLoinc[4];
				newLoinc.ScaleType               =arrayLoinc[5];
				newLoinc.MethodType              =arrayLoinc[6];
				newLoinc.StatusOfCode            =arrayLoinc[7];
				newLoinc.NameShort               =arrayLoinc[8];
				newLoinc.ClassType               =arrayLoinc[9];
				newLoinc.UnitsRequired           =arrayLoinc[10]=="Y";
				newLoinc.OrderObs                =arrayLoinc[11];
				newLoinc.HL7FieldSubfieldID      =arrayLoinc[12];
				newLoinc.ExternalCopyrightNotice =arrayLoinc[13];
				newLoinc.NameLongCommon          =arrayLoinc[14];
				newLoinc.UnitsUCUM               =arrayLoinc[15];
				newLoinc.RankCommonTests         =PIn.Int(arrayLoinc[16]);
				newLoinc.RankCommonOrders        =PIn.Int(arrayLoinc[17]);
				if(dictLoincs.ContainsKey(arrayLoinc[0])) {//code already exists; arrayLoinc[0]==Loinc Code
					oldLoinc=dictLoincs[arrayLoinc[0]];
					if(updateExisting &&
						(oldLoinc.LoincCode                  !=arrayLoinc[0]
						 || oldLoinc.Component               !=arrayLoinc[1]
						 || oldLoinc.PropertyObserved        !=arrayLoinc[2]
						 || oldLoinc.TimeAspct               !=arrayLoinc[3]
						 || oldLoinc.SystemMeasured          !=arrayLoinc[4]
						 || oldLoinc.ScaleType               !=arrayLoinc[5]
						 || oldLoinc.MethodType              !=arrayLoinc[6]
						 || oldLoinc.StatusOfCode            !=arrayLoinc[7]
						 || oldLoinc.NameShort               !=arrayLoinc[8]
						 || oldLoinc.ClassType               !=arrayLoinc[9]
						 || oldLoinc.UnitsRequired           !=(arrayLoinc[10]=="Y")
						 || oldLoinc.OrderObs                !=arrayLoinc[11]
						 || oldLoinc.HL7FieldSubfieldID      !=arrayLoinc[12]
						 || oldLoinc.ExternalCopyrightNotice !=arrayLoinc[13]
						 || oldLoinc.NameLongCommon          !=arrayLoinc[14]
						 || oldLoinc.UnitsUCUM               !=arrayLoinc[15]
						 || oldLoinc.RankCommonTests         !=PIn.Int(arrayLoinc[16])
						 || oldLoinc.RankCommonOrders        !=PIn.Int(arrayLoinc[17]))) 
					{						
						Loincs.Update(newLoinc);
						numCodesUpdated++;
					}
					continue;
				}
				Loincs.Insert(newLoinc);
				numCodesImported++;
			}
		}

		///<summary>Called after file is downloaded.  Throws exceptions.  It is assumed that this is called from a worker thread.  Progress delegate will be called every 100th iteration to inform thread of current progress. Quit flag can be set at any time in order to quit importing prematurely.</summary>
		public static void ImportRxNorm(string tempFileName,ProgressArgs progress,ref bool quit,ref int numCodesImported,ref int numCodesUpdated,
			bool updateExisting) {
			if(tempFileName==null) {
				return;
			}
			//RxNorms can have two codes for each RxCui. One RxNorm will have a value in the MmslCode and a blank description and the other will have a
			//value in the Description and a blank MmslCode. 
			List<RxNorm> listRxNorms=RxNorms.GetAll();
			Dictionary<string,RxNorm> dictRxNormsMmslCodes=listRxNorms.Where(x => x.MmslCode!="").ToDictionary(x => x.RxCui,x =>x);
			Dictionary<string,RxNorm> dictRxNormsDefinitions=listRxNorms.Where(x => x.Description!="").ToDictionary(x => x.RxCui,x =>x);
			string[] lines=File.ReadAllLines(tempFileName);
			string[] arrayRxNorm;
			RxNorm rxNorm=new RxNorm();
			for(int i=0;i<lines.Length;i++) {//Each loop should read exactly one line of code. Each line will NOT be a unique code.
				if(quit) {
					return;
				}
				if(i%100==0) {
					progress(i+1,lines.Length);
				}
				arrayRxNorm=lines[i].Split('\t');
				if(dictRxNormsMmslCodes.ContainsKey(arrayRxNorm[0])) {//code with an MmslCode already exists
					rxNorm=dictRxNormsMmslCodes[arrayRxNorm[0]];
					if(updateExisting) {						
						if(arrayRxNorm[1]!="" && arrayRxNorm[1]!=rxNorm.MmslCode) {
							rxNorm.MmslCode=arrayRxNorm[1];
							rxNorm.Description="";//Should be blank for all MMSL code entries. See below for non-MMSL entries with descriptions.
							RxNorms.Update(rxNorm);
							numCodesUpdated++;
						}
					}
					continue;
				}
				if(dictRxNormsDefinitions.ContainsKey(arrayRxNorm[0])) {//code with a Description already exists
					rxNorm=dictRxNormsDefinitions[arrayRxNorm[0]];
					if(updateExisting) {
						string newDescript=arrayRxNorm[2];
						//if(newDescript.Length>255) {
						//	newDescript=newDescript.Substring(0,255);//Description column is only varchar(255) so some descriptions will get truncated.
						//}
						//if(arrayRxNorm[2]!="" && newDescript!=rxNorm.Description) {
						if(arrayRxNorm[2]!="" && arrayRxNorm[2]!=rxNorm.Description) {
							rxNorm.MmslCode="";//should be blank for all entries that have a description.
							rxNorm.Description=arrayRxNorm[2];
							RxNorms.Update(rxNorm);
							numCodesUpdated++;
						}
					}
					continue;
				}
				rxNorm.RxCui				=arrayRxNorm[0];
				rxNorm.MmslCode			=arrayRxNorm[1];
				rxNorm.Description	=arrayRxNorm[2];
				RxNorms.Insert(rxNorm);
				numCodesImported++;
			}
		}

		///<summary>Called after file is downloaded.  Throws exceptions.  It is assumed that this is called from a worker thread.  Progress delegate will be called every 100th iteration to inform thread of current progress. Quit flag can be set at any time in order to quit importing prematurely.</summary>
		public static void ImportSnomed(string tempFileName,ProgressArgs progress,ref bool quit,ref int numCodesImported,ref int numCodesUpdated,
			bool updateExisting) 
		{
			if(tempFileName==null) {
				return;
			}
			Dictionary<string,Snomed> dictSnomeds=Snomeds.GetAll().ToDictionary(x => x.SnomedCode,x => x);
			string[] lines=File.ReadAllLines(tempFileName);
			string[] arraySnomed;
			Snomed snomed=new Snomed();
			for(int i=0;i<lines.Length;i++) {//each loop should read exactly one line of code. and each line of code should be a unique code
				if(quit) {
					return;
				}
				if(i%100==0) {
					progress(i+1,lines.Length);
				}
				arraySnomed=lines[i].Split('\t');
				if(dictSnomeds.ContainsKey(arraySnomed[0])) {//code already exists
					snomed=dictSnomeds[arraySnomed[0]];
					if(updateExisting && snomed.Description!=arraySnomed[1]) {
						snomed.Description=arraySnomed[1];
						Snomeds.Update(snomed);
						numCodesUpdated++;
					}
					continue;
				}
				snomed.SnomedCode		=arraySnomed[0];
				snomed.Description	=arraySnomed[1];
				Snomeds.Insert(snomed);
				numCodesImported++;
			}
		}

		///<summary>Called after file is downloaded.  Throws exceptions.  It is assumed that this is called from a worker thread.  Progress delegate will be called every 100th iteration to inform thread of current progress. Quit flag can be set at any time in order to quit importing prematurely.</summary>
		public static void ImportSop(string tempFileName,ProgressArgs progress,ref bool quit,ref int numCodesImported,ref int numcodesUpdated,
			bool updateExisting) 
		{
			if(tempFileName==null) {
				return;
			}
			Dictionary<string,Sop> dictSops=Sops.GetDeepCopy().ToDictionary(x => x.SopCode,x => x);
			string[] lines=File.ReadAllLines(tempFileName);
			string[] arraySop;
			Sop sop=new Sop();
			for(int i=0;i<lines.Length;i++) {//each loop should read exactly one line of code. and each line of code should be a unique code
				if(quit) {
					return;
				}
				if(i%100==0) {
					progress(i+1,lines.Length);
				}
				arraySop=lines[i].Split('\t');
				if(dictSops.ContainsKey(arraySop[0])) {//code already exists
					sop=dictSops[arraySop[0]];
					if(updateExisting && sop.Description!=arraySop[1]) {
						sop.Description=arraySop[1];
						Sops.Update(sop);
						numcodesUpdated++;
					}
					continue;
				}
				sop.SopCode			=arraySop[0];
				sop.Description	=arraySop[1];
				Sops.Insert(sop);
				numCodesImported++;
			}
		}

		///<summary>Called after file is downloaded.  Throws exceptions.  It is assumed that this is called from a worker thread.  Progress delegate will be called every 100th iteration to inform thread of current progress. Quit flag can be set at any time in order to quit importing prematurely.</summary>
		public static void ImportUcum(string tempFileName,ProgressArgs progress,ref bool quit,ref int numCodesImported,ref int numCodesUpdated,
			bool updateExisting) 
		{
			if(tempFileName==null) {
				return;
			}
			Dictionary<string,Ucum> dictUcums=Ucums.GetAll().ToDictionary(x => x.UcumCode,x => x);
			string[] lines=File.ReadAllLines(tempFileName);
			string[] arrayUcum;
			Ucum ucum=new Ucum();
			for(int i=0;i<lines.Length;i++) {//each loop should read exactly one line of code. and each line of code should be a unique code
				if(quit) {
					return;
				}
				if(i%100==0) {
					progress(i+1,lines.Length);
				}
				arrayUcum=lines[i].Split('\t');
				if(dictUcums.ContainsKey(arrayUcum[0])) {//code already exists
					ucum=dictUcums[arrayUcum[0]];
					if(updateExisting && ucum.Description!=arrayUcum[1]) {
						ucum.Description=arrayUcum[1];
						Ucums.Update(ucum);
						numCodesUpdated++;
					}
					continue;
				}
				ucum.UcumCode			=arrayUcum[0];
				ucum.Description	=arrayUcum[1];
				ucum.IsInUse			=false;
				Ucums.Insert(ucum);
				numCodesImported++;
			}
		}

		/////<summary>Returns number of codes imported.</summary>
		///// <param name="tempFile"></param>
		///// <param name="codeCount">Returns number of new codes inserted.</param>
		///// <param name="totalCodes">Returns number of total codes found.</param>
		///// <returns></returns>
//		public static void ImportEhrCodes(string tempFile,out int newCodeCount,out int totalCodeCount,out int availableCodeCount){
//			newCodeCount=0;
//			totalCodeCount=0;
//			availableCodeCount=0;
//			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
//				Meth.GetVoid(MethodBase.GetCurrentMethod(),tempFile,newCodeCount,totalCodeCount,availableCodeCount);
//				return;
//			}
//			//UNION ALL to speed up query.  Used to determine what codes to add to DB.
//			string command=@"SELECT CdcrecCode FROM cdcrec
//											UNION ALL
//											SELECT ProcCode FROM procedurecode
//											UNION ALL
//											SELECT CptCode FROM cpt
//											UNION ALL
//											SELECT CvxCode FROM cvx
//											UNION ALL
//											SELECT HcpcsCode FROM hcpcs
//											UNION ALL
//											SELECT Icd10Code FROM icd10
//											UNION ALL
//											SELECT ICD9Code FROM icd9
//											UNION ALL
//											SELECT LoincCode FROM loinc
//											UNION ALL
//											SELECT RxCui FROM rxnorm
//											UNION ALL
//											SELECT SnomedCode FROM snomed
//											UNION ALL
//											SELECT SopCode FROM sop";
//			DataTable T = DataCore.GetTable(command);
//			HashSet<string> allCodeHash=new HashSet<string>();
//			for(int i=0;i<T.Rows.Count;i++) {
//				allCodeHash.Add(T.Rows[i][0].ToString());
//			}
//			HashSet<string> ehrCodeHash=EhrCodes.GetAllCodesHashSet();
//			string[] lines=File.ReadAllLines(tempFile);
//			string[] arrayEHRCode;
//			EhrCode ehrc=new EhrCode();
//			for(int i=0;i<lines.Length;i++) {//each loop should read exactly one line of code. and each line of code should be a unique code
//				arrayEHRCode=lines[i].Split('\t');
//				if(!allCodeHash.Contains(arrayEHRCode[0]) && arrayEHRCode[6]!="AdministrativeSex") {//exception for AdministrativeSex because it is not stored in the DB.
//					continue;//code does not exist in the database in one of the standard code system tables.
//				}
//				if(ehrCodeHash.Contains(arrayEHRCode[4]+arrayEHRCode[2])) {
//					continue;//Code already inserted in ehrCodes table
//				}
//				ehrc.MeasureIds		=arrayEHRCode[0];
//				ehrc.ValueSetName	=arrayEHRCode[1];
//				ehrc.ValueSetOID	=arrayEHRCode[2];
//				ehrc.QDMCategory	=arrayEHRCode[3];
//				ehrc.CodeValue		=arrayEHRCode[4];
//				ehrc.Description	=arrayEHRCode[5];
//				ehrc.CodeSystem		=arrayEHRCode[6];
//				ehrc.CodeSystemOID=arrayEHRCode[7];
//				EhrCodes.Insert(ehrc);
//				newCodeCount++;//return value
//			}
//			totalCodeCount=ehrCodeHash.Count+newCodeCount;//return value
//			availableCodeCount=lines.Length;//return value
//		}


		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary>Gets one CodeSystem from the db.</summary>
		public static CodeSystem GetOne(long codeSystemNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<CodeSystem>(MethodBase.GetCurrentMethod(),codeSystemNum);
			}
			return Crud.CodeSystemCrud.SelectOne(codeSystemNum);
		}

		///<summary></summary>
		public static long Insert(CodeSystem codeSystem){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				codeSystem.CodeSystemNum=Meth.GetLong(MethodBase.GetCurrentMethod(),codeSystem);
				return codeSystem.CodeSystemNum;
			}
			return Crud.CodeSystemCrud.Insert(codeSystem);
		}

		///<summary></summary>
		public static void Delete(long codeSystemNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),codeSystemNum);
				return;
			}
			string command= "DELETE FROM codesystem WHERE CodeSystemNum = "+POut.Long(codeSystemNum);
			Db.NonQ(command);
		}
		*/



	}
}