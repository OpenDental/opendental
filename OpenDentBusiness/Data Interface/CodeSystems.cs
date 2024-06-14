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
		public static List<CodeSystem> GetForCurrentVersion(bool isMemberNation) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<CodeSystem>>(MethodBase.GetCurrentMethod(),isMemberNation);
			}
			string command="";
			if(ODBuild.IsDebug()) {
				command="SELECT * FROM codesystem";// WHERE CodeSystemName IN ('ICD9CM','RXNORM','SNOMEDCT','CPT')";
			}
			else {
				command="SELECT * FROM codesystem WHERE CodeSystemName NOT IN ('AdministrativeSex','CDT')";
			}
			return Crud.CodeSystemCrud.SelectMany(command);
		}

		/////<summary>Returns a list of code systems in the code system table.  This query will change from version to version depending on what code systems we have available.</summary>
		//public static List<CodeSystem> GetForCurrentVersionNoSnomed() {
		//	if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
		//		return Meth.GetObject<List<CodeSystem>>(MethodBase.GetCurrentMethod());
		//	}
		//	//string command="SELECT * FROM codesystem WHERE CodeSystemName!='AdministrativeSex' AND CodeSystemName!='CDT'";
		//	string command="SELECT * FROM codesystem WHERE CodeSystemName IN ('ICD9CM','RXNORM','CPT')";
		//	return Crud.CodeSystemCrud.SelectMany(command);
		//}

		///<summary></summary>
		public static void Update(CodeSystem codeSystem){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),codeSystem);
				return;
			}
			Crud.CodeSystemCrud.Update(codeSystem);
		}

		///<summary>Updates VersionCurrent to the VersionAvail of the codeSystem object passed in. Used by code system importer after successful import.</summary>
		public static void UpdateCurrentVersion(CodeSystem codeSystem) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),codeSystem);
				return;
			}
			codeSystem.VersionCur=codeSystem.VersionAvail;
			Crud.CodeSystemCrud.Update(codeSystem);
		}

		///<summary>Updates VersionCurrent to the versionID passed in. Used by code system importer after successful import.  Currently only used for CPT.</summary>
		public static void UpdateCurrentVersion(CodeSystem codeSystem, string versionID) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
		public static void ImportCdcrec(string tempFileName,ProgressArgs progressArgs,ref bool quit,ref int numCodesImported,ref int numCodesUpdated,
			bool updateExisting) 
		{
			if(tempFileName==null) {
				return;
			}
			Dictionary<string,Cdcrec> dictionaryCdcrecs=Cdcrecs.GetAll().ToDictionary(x => x.CdcrecCode,x => x);
			string[] stringArrayLines=File.ReadAllLines(tempFileName);
			string[] stringArrayCdcrecs;
			Cdcrec cdcrec=new Cdcrec();
			for(int i=0;i<stringArrayLines.Length;i++) {//each loop should read exactly one line of code. and each line of code should be a unique code
				if(quit) {
					return;
				}
				if(i%100==0) {
					progressArgs(i+1,stringArrayLines.Length);
				}
				stringArrayCdcrecs=stringArrayLines[i].Split('\t');
				if(dictionaryCdcrecs.ContainsKey(stringArrayCdcrecs[0])) {//code already exists
					cdcrec=dictionaryCdcrecs[stringArrayCdcrecs[0]];
					if(updateExisting &&
						(cdcrec.HeirarchicalCode!=stringArrayCdcrecs[1] 
						|| cdcrec.Description!=stringArrayCdcrecs[2])) 
					{
						cdcrec.HeirarchicalCode=stringArrayCdcrecs[1];
						cdcrec.Description=stringArrayCdcrecs[2];
						Cdcrecs.Update(cdcrec);
						numCodesUpdated++;
					}
					continue;
				}
				cdcrec.CdcrecCode				=stringArrayCdcrecs[0];
				cdcrec.HeirarchicalCode	=stringArrayCdcrecs[1];
				cdcrec.Description			=stringArrayCdcrecs[2];
				Cdcrecs.Insert(cdcrec);
				numCodesImported++;
			}
		}

		/////<summary>Called after file is downloaded.  Throws exceptions.</summary>
	//public static void ImportCDT(string tempFileName) ... not necessary.

		///<summary>Called after user provides resource file.  Throws exceptions.  It is assumed that this is called from a worker thread.  Progress delegate will be called every 100th iteration to inform thread of current progress. Quit flag can be set at any time in order to quit importing prematurely.
		///No UpdateExisting parameter because we force users to accept new descriptions.</summary>
		public static void ImportCpt(string tempFileName,ProgressArgs progressArgs,ref bool quit,ref int numCodesImported,ref int numCodesUpdated,
			string versionID)
		{
			if(tempFileName==null) {
				return;
			}
			Dictionary<string,string> dictionaryCodes=Cpts.GetAll().ToDictionary(x=> x.CptCode,x => x.Description);
			Regex regex=new Regex(@"^([\d]{4}[\d\w])\s+(.+?)$");//Regex = "At the beginning of the string, find five numbers, followed by a white space (tab or space) followed by one or more characters (but as few as possible) to the end of the line."
			string[] stringArrayLines=File.ReadAllLines(tempFileName);
			string[] stringArrayCpts;
			bool isHeader=true;
			Cpt cpt=new Cpt();
			for(int i=0;i<stringArrayLines.Length;i++) {//each loop should read exactly one line of code. and each line of code should be a unique code
				if(quit) {
					return;
				}
				if(i%100==0) {
					progressArgs(i+1,stringArrayLines.Length);
				}
				if(isHeader) {
					if(!regex.IsMatch(stringArrayLines[i])) {  					//if(!lines[i].Contains("\t")) {	
						continue;//Copyright info is present at the head of the file.
					}
					isHeader=false;
				}
				stringArrayCpts=new string[2];
				stringArrayCpts[0]=regex.Match(stringArrayLines[i]).Groups[1].Value;//First five alphanumeric characters
				stringArrayCpts[1]=regex.Match(stringArrayLines[i]).Groups[2].Value;//Everything after the 6th character
				if(dictionaryCodes.Keys.Contains(stringArrayCpts[0])) {//code already exists
					Cpts.UpdateDescription(stringArrayCpts[0],stringArrayCpts[1],versionID);
					if(dictionaryCodes[stringArrayCpts[0]]!=stringArrayCpts[1]) {//The description is different
						numCodesUpdated++;
					}
				}
				else {
					cpt.CptCode			=stringArrayCpts[0];
					cpt.Description	=stringArrayCpts[1];
					cpt.VersionIDs	=versionID;
					Cpts.Insert(cpt);
					numCodesImported++;
				}
			}
		}

		///<summary>Called after file is downloaded.  Throws exceptions.  It is assumed that this is called from a worker thread.  Progress delegate will be called every 100th iteration to inform thread of current progress. Quit flag can be set at any time in order to quit importing prematurely.</summary>
		public static void ImportCvx(string tempFileName,ProgressArgs progressArgs,ref bool quit,ref int numCodesImported,ref int numCodesUpdated,
			bool updateExisting) 
		{
			if(tempFileName==null) {
				return;
			}
			Dictionary<string,Cvx> dictionaryCodes=Cvxs.GetAll().ToDictionary(x => x.CvxCode,x => x);
			string[] stringArrayLines=File.ReadAllLines(tempFileName);
			string[] stringArrayCvxs;
			Cvx cvx=new Cvx();
			for(int i=0;i<stringArrayLines.Length;i++) {//each loop should read exactly one line of code. and each line of code should be a unique code
				if(quit) {
					return;
				}
				if(i%100==0) {
					progressArgs(i+1,stringArrayLines.Length);
				}
				stringArrayCvxs=stringArrayLines[i].Split('\t');
				if(dictionaryCodes.ContainsKey(stringArrayCvxs[0])) {//code already exists
					cvx=dictionaryCodes[stringArrayCvxs[0]];
					if(updateExisting && cvx.Description!=stringArrayCvxs[1]) {//We do want to update and description is different.
						cvx.Description=stringArrayCvxs[1];
						Cvxs.Update(cvx);
						numCodesUpdated++;
					}
					continue;
				}
				cvx.CvxCode			=stringArrayCvxs[0];
				cvx.Description	=stringArrayCvxs[1];
				Cvxs.Insert(cvx);
				numCodesImported++;
			}
		}

		///<summary>Called after file is downloaded.  Throws exceptions.  It is assumed that this is called from a worker thread.  Progress delegate will be called every 100th iteration to inform thread of current progress. Quit flag can be set at any time in order to quit importing prematurely.</summary>
		public static void ImportHcpcs(string tempFileName,ProgressArgs progressArgs,ref bool quit,ref int numCodesImported,ref int numCodesUpdated,
			bool updateExisting) 
		{
			if(tempFileName==null) {
				return;
			}
			Dictionary<string,Hcpcs> dictionaryHcpcs=Hcpcses.GetAll().ToDictionary(x => x.HcpcsCode,x => x);
			string[] stringArrayLines=File.ReadAllLines(tempFileName);
			string[] stringArrayHcpcs;
			Hcpcs hcpcs=new Hcpcs();
			for(int i=0;i<stringArrayLines.Length;i++) {//each loop should read exactly one line of code. and each line of code should be a unique code
				if(quit) {
					return;
				}
				if(i%100==0) {
					progressArgs(i+1,stringArrayLines.Length);
				}
				stringArrayHcpcs=stringArrayLines[i].Split('\t');
				if(dictionaryHcpcs.ContainsKey(stringArrayHcpcs[0])) {//code already exists
					hcpcs=dictionaryHcpcs[stringArrayHcpcs[0]];
					if(updateExisting && hcpcs.DescriptionShort!=stringArrayHcpcs[1]) {
						hcpcs.DescriptionShort=stringArrayHcpcs[1];
						Hcpcses.Update(hcpcs);
						numCodesUpdated++;
					}
					continue;
				}
				hcpcs.HcpcsCode					=stringArrayHcpcs[0];
				hcpcs.DescriptionShort	=stringArrayHcpcs[1];
				Hcpcses.Insert(hcpcs);
				numCodesImported++;
			}
		}

		///<summary>Called after file is downloaded.  Throws exceptions.  It is assumed that this is called from a worker thread.  Progress delegate will be called every 100th iteration to inform thread of current progress. Quit flag can be set at any time in order to quit importing prematurely.</summary>
		public static void ImportIcd10(string tempFileName,ProgressArgs progressArgs,ref bool quit,ref int numCodesImported,ref int numCodesUpdated,
			bool updateExisting) 
		{
			if(tempFileName==null) {
				return;
			}
			Dictionary<string,Icd10> dictionaryIcd10s=Icd10s.GetAll().ToDictionary(x => x.Icd10Code,x => x);
			string[] stringArrayLines=File.ReadAllLines(tempFileName);
			string[] stringArrayIcd10s;
			Icd10 icd10=new Icd10();
			for(int i=0;i<stringArrayLines.Length;i++) {//each loop should read exactly one line of code. and each line of code should be a unique code
				if(quit) {
					return;
				}
				if(i%100==0) {
					progressArgs(i+1,stringArrayLines.Length);
				}
				stringArrayIcd10s=stringArrayLines[i].Split('\t');
				if(dictionaryIcd10s.ContainsKey(stringArrayIcd10s[0])) {//code already exists
					icd10=dictionaryIcd10s[stringArrayIcd10s[0]];
					if(updateExisting && 
						(icd10.Description!=stringArrayIcd10s[1] || icd10.IsCode!=stringArrayIcd10s[2])) //Code informatin is different
					{
						icd10.Description=stringArrayIcd10s[1];
						icd10.IsCode=stringArrayIcd10s[2];
						Icd10s.Update(icd10);
						numCodesUpdated++;
					}
					continue;
				}
				icd10.Icd10Code		=stringArrayIcd10s[0];
				icd10.Description	=stringArrayIcd10s[1];
				icd10.IsCode			=stringArrayIcd10s[2];
				Icd10s.Insert(icd10);
				numCodesImported++;
			}
		}

		///<summary>Called after file is downloaded.  Throws exceptions.  It is assumed that this is called from a worker thread.  Progress delegate will be called every 100th iteration to inform thread of current progress. Quit flag can be set at any time in order to quit importing prematurely.</summary>
		public static void ImportIcd9(string tempFileName,ProgressArgs progressArgs,ref bool quit,ref int numCodesImported,ref int numCodesUpdated,
			bool updateExisting) 
		{
			if(tempFileName==null) {
				return;
			}
			//Customers may have an old codeset that has a truncated uppercase description, if so we want to update with new descriptions.
			bool isDescriptionsOld=ICD9s.IsOldDescriptions();
			Dictionary<string,ICD9> dictionaryCodes=ICD9s.GetAll().ToDictionary(x => x.ICD9Code,x => x);
			string[] stringArrayLines=File.ReadAllLines(tempFileName);
			string[] stringArrayICD9s;
			ICD9 icd9=new ICD9();
			for(int i=0;i<stringArrayLines.Length;i++) {//each loop should read exactly one line of code. and each line of code should be a unique code
				if(quit) {
					return;
				}
				if(i%100==0) {
					progressArgs(i+1,stringArrayLines.Length);
				}
				stringArrayICD9s=stringArrayLines[i].Split('\t');
				if(dictionaryCodes.ContainsKey(stringArrayICD9s[0])) {//code already exists
					icd9=dictionaryCodes[stringArrayICD9s[0]];
					if((isDescriptionsOld || updateExisting) && icd9.Description!=stringArrayICD9s[1]) {//The new description does not match the description in the database.
						icd9.Description=stringArrayICD9s[1];
						ICD9s.Update(icd9);
						numCodesUpdated++;
					}
					continue;
				}
				icd9.ICD9Code		=stringArrayICD9s[0];
				icd9.Description=stringArrayICD9s[1];
				ICD9s.Insert(icd9);
				numCodesImported++;
			}
		}

		///<summary>Called after file is downloaded.  Throws exceptions.  It is assumed that this is called from a worker thread.  Progress delegate will be called every 100th iteration to inform thread of current progress. Quit flag can be set at any time in order to quit importing prematurely.</summary>
		public static void ImportLoinc(string tempFileName,ProgressArgs progressArgs,ref bool quit,ref int numCodesImported,ref int numCodesUpdated,
			bool updateExisting) 
		{
			if(tempFileName==null) {
				return;
			}
			Dictionary<string,Loinc> dictionaryLoincs=Loincs.GetAll().ToDictionary(x => x.LoincCode,x => x);
			string[] stringArrayLines=File.ReadAllLines(tempFileName);
			string[] stringArrayLoincs;
			Loinc loincOld=new Loinc();
			Loinc loincNew=new Loinc();
			for(int i=0;i<stringArrayLines.Length;i++) {//each loop should read exactly one line of code. and each line of code should be a unique code
				if(quit) {
					return;
				}
				if(i%100==0) {
					progressArgs(i+1,stringArrayLines.Length);
				}
				stringArrayLoincs=stringArrayLines[i].Split('\t');
				loincNew.LoincCode               =stringArrayLoincs[0];
				loincNew.Component               =stringArrayLoincs[1];
				loincNew.PropertyObserved        =stringArrayLoincs[2];
				loincNew.TimeAspct               =stringArrayLoincs[3];
				loincNew.SystemMeasured          =stringArrayLoincs[4];
				loincNew.ScaleType               =stringArrayLoincs[5];
				loincNew.MethodType              =stringArrayLoincs[6];
				loincNew.StatusOfCode            =stringArrayLoincs[7];
				loincNew.NameShort               =stringArrayLoincs[8];
				loincNew.ClassType               =stringArrayLoincs[9];
				loincNew.UnitsRequired           =stringArrayLoincs[10]=="Y";
				loincNew.OrderObs                =stringArrayLoincs[11];
				loincNew.HL7FieldSubfieldID      =stringArrayLoincs[12];
				loincNew.ExternalCopyrightNotice =stringArrayLoincs[13];
				loincNew.NameLongCommon          =stringArrayLoincs[14];
				loincNew.UnitsUCUM               =stringArrayLoincs[15];
				loincNew.RankCommonTests         =PIn.Int(stringArrayLoincs[16]);
				loincNew.RankCommonOrders        =PIn.Int(stringArrayLoincs[17]);
				if(dictionaryLoincs.ContainsKey(stringArrayLoincs[0])) {//code already exists; arrayLoinc[0]==Loinc Code
					loincOld=dictionaryLoincs[stringArrayLoincs[0]];
					if(updateExisting &&
						(loincOld.LoincCode                  !=stringArrayLoincs[0]
						 || loincOld.Component               !=stringArrayLoincs[1]
						 || loincOld.PropertyObserved        !=stringArrayLoincs[2]
						 || loincOld.TimeAspct               !=stringArrayLoincs[3]
						 || loincOld.SystemMeasured          !=stringArrayLoincs[4]
						 || loincOld.ScaleType               !=stringArrayLoincs[5]
						 || loincOld.MethodType              !=stringArrayLoincs[6]
						 || loincOld.StatusOfCode            !=stringArrayLoincs[7]
						 || loincOld.NameShort               !=stringArrayLoincs[8]
						 || loincOld.ClassType               !=stringArrayLoincs[9]
						 || loincOld.UnitsRequired           !=(stringArrayLoincs[10]=="Y")
						 || loincOld.OrderObs                !=stringArrayLoincs[11]
						 || loincOld.HL7FieldSubfieldID      !=stringArrayLoincs[12]
						 || loincOld.ExternalCopyrightNotice !=stringArrayLoincs[13]
						 || loincOld.NameLongCommon          !=stringArrayLoincs[14]
						 || loincOld.UnitsUCUM               !=stringArrayLoincs[15]
						 || loincOld.RankCommonTests         !=PIn.Int(stringArrayLoincs[16])
						 || loincOld.RankCommonOrders        !=PIn.Int(stringArrayLoincs[17]))) 
					{						
						Loincs.Update(loincNew);
						numCodesUpdated++;
					}
					continue;
				}
				Loincs.Insert(loincNew);
				numCodesImported++;
			}
		}

		///<summary>Called after file is downloaded.  Throws exceptions.  It is assumed that this is called from a worker thread.  Progress delegate will be called every 100th iteration to inform thread of current progress. Quit flag can be set at any time in order to quit importing prematurely.</summary>
		public static void ImportRxNorm(string tempFileName,ProgressArgs progressArgs,ref bool quit,ref int numCodesImported,ref int numCodesUpdated,
			bool updateExisting) {
			if(tempFileName==null) {
				return;
			}
			//RxNorms can have two codes for each RxCui. One RxNorm will have a value in the MmslCode and a blank description and the other will have a
			//value in the Description and a blank MmslCode. 
			List<RxNorm> listRxNorms=RxNorms.GetAll();
			Dictionary<string,RxNorm> dictionaryRxNormsMmslCodes=listRxNorms.Where(x => x.MmslCode!="").ToDictionary(x => x.RxCui,x =>x);
			Dictionary<string,RxNorm> dictionaryRxNormsDefinitions=listRxNorms.Where(x => x.Description!="").ToDictionary(x => x.RxCui,x =>x);
			string[] stringArrayLines=File.ReadAllLines(tempFileName);
			string[] stringArrayRxNorms;
			RxNorm rxNorm=new RxNorm();
			for(int i=0;i<stringArrayLines.Length;i++) {//Each loop should read exactly one line of code. Each line will NOT be a unique code.
				if(quit) {
					return;
				}
				if(i%100==0) {
					progressArgs(i+1,stringArrayLines.Length);
				}
				stringArrayRxNorms=stringArrayLines[i].Split('\t');
				if(dictionaryRxNormsMmslCodes.ContainsKey(stringArrayRxNorms[0])) {//code with an MmslCode already exists
					rxNorm=dictionaryRxNormsMmslCodes[stringArrayRxNorms[0]];
					if(updateExisting) {						
						if(stringArrayRxNorms[1]!="" && stringArrayRxNorms[1]!=rxNorm.MmslCode) {
							rxNorm.MmslCode=stringArrayRxNorms[1];
							rxNorm.Description="";//Should be blank for all MMSL code entries. See below for non-MMSL entries with descriptions.
							RxNorms.Update(rxNorm);
							numCodesUpdated++;
						}
					}
					continue;
				}
				if(dictionaryRxNormsDefinitions.ContainsKey(stringArrayRxNorms[0])) {//code with a Description already exists
					rxNorm=dictionaryRxNormsDefinitions[stringArrayRxNorms[0]];
					if(updateExisting) {
						string newDescript=stringArrayRxNorms[2];
						//if(newDescript.Length>255) {
						//	newDescript=newDescript.Substring(0,255);//Description column is only varchar(255) so some descriptions will get truncated.
						//}
						//if(arrayRxNorm[2]!="" && newDescript!=rxNorm.Description) {
						if(stringArrayRxNorms[2]!="" && stringArrayRxNorms[2]!=rxNorm.Description) {
							rxNorm.MmslCode="";//should be blank for all entries that have a description.
							rxNorm.Description=stringArrayRxNorms[2];
							RxNorms.Update(rxNorm);
							numCodesUpdated++;
						}
					}
					continue;
				}
				rxNorm.RxCui				=stringArrayRxNorms[0];
				rxNorm.MmslCode			=stringArrayRxNorms[1];
				rxNorm.Description	=stringArrayRxNorms[2];
				RxNorms.Insert(rxNorm);
				numCodesImported++;
			}
		}

		///<summary>Called after file is downloaded.  Throws exceptions.  It is assumed that this is called from a worker thread.  Progress delegate will be called every 100th iteration to inform thread of current progress. Quit flag can be set at any time in order to quit importing prematurely.</summary>
		public static void ImportSnomed(string tempFileName,ProgressArgs progressArgs,ref bool quit,ref int numCodesImported,ref int numCodesUpdated,
			bool updateExisting) 
		{
			if(tempFileName==null) {
				return;
			}
			List<Snomed> listSnomeds=Snomeds.GetAll();
			string[] stringArrayLines=File.ReadAllLines(tempFileName);
			string[] stringArraySnomeds;
			Snomed snomed=new Snomed();
			for(int i=0;i<stringArrayLines.Length;i++) {//each loop should read exactly one line of code. and each line of code should be a unique code
				if(quit) {
					return;
				}
				if(i%100==0) {
					progressArgs(i+1,stringArrayLines.Length);
				}
				stringArraySnomeds=stringArrayLines[i].Split('\t');
				if(stringArraySnomeds.Length<2) {
					continue;//Line is not formatted properly. Do not import this code.
				}
				snomed=listSnomeds.Find(x => x.SnomedCode==stringArraySnomeds[0]);
				if(snomed==null) {//Adding a new SNOMED
					snomed=new Snomed();
					snomed.SnomedCode		=stringArraySnomeds[0];
					snomed.Description	=stringArraySnomeds[1];
					Snomeds.Insert(snomed);
					listSnomeds.Add(snomed);//To prevent inserting duplicates
					numCodesImported++;
					continue;
				}
				if(updateExisting && snomed.Description!=stringArraySnomeds[1]) {
					snomed.Description=stringArraySnomeds[1];
					Snomeds.Update(snomed);
					numCodesUpdated++;
				}
			}
		}

		///<summary>Called after file is downloaded.  Throws exceptions.  It is assumed that this is called from a worker thread.  Progress delegate will be called every 100th iteration to inform thread of current progress. Quit flag can be set at any time in order to quit importing prematurely.</summary>
		public static void ImportSop(string tempFileName,ProgressArgs progressArgs,ref bool quit,ref int numCodesImported,ref int numcodesUpdated,
			bool updateExisting) 
		{
			if(tempFileName==null) {
				return;
			}
			Dictionary<string,Sop> dictionarySops=Sops.GetDeepCopy().ToDictionary(x => x.SopCode,x => x);
			string[] stringArrayLines=File.ReadAllLines(tempFileName);
			string[] stringArraySops;
			Sop sop=new Sop();
			for(int i=0;i<stringArrayLines.Length;i++) {//each loop should read exactly one line of code. and each line of code should be a unique code
				if(quit) {
					return;
				}
				if(i%100==0) {
					progressArgs(i+1,stringArrayLines.Length);
				}
				stringArraySops=stringArrayLines[i].Split('\t');
				if(dictionarySops.ContainsKey(stringArraySops[0])) {//code already exists
					sop=dictionarySops[stringArraySops[0]];
					if(updateExisting && sop.Description!=stringArraySops[1]) {
						sop.Description=stringArraySops[1];
						Sops.Update(sop);
						numcodesUpdated++;
					}
					continue;
				}
				sop.SopCode			=stringArraySops[0];
				sop.Description	=stringArraySops[1];
				Sops.Insert(sop);
				numCodesImported++;
			}
		}

		///<summary>Called after file is downloaded.  Throws exceptions.  It is assumed that this is called from a worker thread.  Progress delegate will be called every 100th iteration to inform thread of current progress. Quit flag can be set at any time in order to quit importing prematurely.</summary>
		public static void ImportUcum(string tempFileName,ProgressArgs progressArgs,ref bool quit,ref int numCodesImported,ref int numCodesUpdated,
			bool updateExisting) 
		{
			if(tempFileName==null) {
				return;
			}
			Dictionary<string,Ucum> dictionaryUcums=Ucums.GetAll().ToDictionary(x => x.UcumCode,x => x);
			string[] stringArrayLines=File.ReadAllLines(tempFileName);
			string[] stringArrayUcums;
			Ucum ucum=new Ucum();
			for(int i=0;i<stringArrayLines.Length;i++) {//each loop should read exactly one line of code. and each line of code should be a unique code
				if(quit) {
					return;
				}
				if(i%100==0) {
					progressArgs(i+1,stringArrayLines.Length);
				}
				stringArrayUcums=stringArrayLines[i].Split('\t');
				if(dictionaryUcums.ContainsKey(stringArrayUcums[0])) {//code already exists
					ucum=dictionaryUcums[stringArrayUcums[0]];
					if(updateExisting && ucum.Description!=stringArrayUcums[1]) {
						ucum.Description=stringArrayUcums[1];
						Ucums.Update(ucum);
						numCodesUpdated++;
					}
					continue;
				}
				ucum.UcumCode			=stringArrayUcums[0];
				ucum.Description	=stringArrayUcums[1];
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
//			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<CodeSystem>(MethodBase.GetCurrentMethod(),codeSystemNum);
			}
			return Crud.CodeSystemCrud.SelectOne(codeSystemNum);
		}

		///<summary></summary>
		public static long Insert(CodeSystem codeSystem){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				codeSystem.CodeSystemNum=Meth.GetLong(MethodBase.GetCurrentMethod(),codeSystem);
				return codeSystem.CodeSystemNum;
			}
			return Crud.CodeSystemCrud.Insert(codeSystem);
		}

		///<summary></summary>
		public static void Delete(long codeSystemNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),codeSystemNum);
				return;
			}
			string command= "DELETE FROM codesystem WHERE CodeSystemNum = "+POut.Long(codeSystemNum);
			Db.NonQ(command);
		}
		*/



	}
}