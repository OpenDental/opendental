using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using CodeBase;
using Ionic.Zip;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormCodeSystemsImport:FormODBase {
		///<summary>All code systems available.</summary>
		private List<CodeSystem> _listCodeSystems;
		///<summary>If true then SNOMED CT codes will show in the list of available code systems for download.</summary>
		private bool _isMemberNation;
		///<summary>Indicates if user has downloaded codes while in the window.</summary>
		private bool _hasDownloaded;
		///<summary>Track current status of each code system.</summary>
		private Dictionary<string,string> _mapCodeSystemStatus=new Dictionary<string /*code system name*/,string /*status to be printed to grid*/>();
		private CodeSystemName[] _arrayAutoImportCodeSystemNames;
		
		public FormCodeSystemsImport(params CodeSystemName[] arrayAutoImportCodeSystemNames) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_arrayAutoImportCodeSystemNames=arrayAutoImportCodeSystemNames;
		}

		private void FormCodeSystemsImport_Load(object sender,EventArgs e) {
			_isMemberNation=true;//Assume we show the SNOMED row. Handles validating on import instead of on load.
			checkKeepDescriptions.Checked=true;//If checked, preserves old behavior of not updating existing descriptions. Users probably want it unchecked.
			UpdateCodeSystemThread.Finished+=new EventHandler(UpdateCodeSystemThread_FinishedSafe);
			if(_arrayAutoImportCodeSystemNames.Length > 0) {
				CheckUpdates();
				foreach(CodeSystemName codeSystemName in _arrayAutoImportCodeSystemNames) {
					for(int i=0;i<gridMain.ListGridRows.Count;i++) {
						if(gridMain.ListGridRows[i].Cells[0].Text.ToLower()==codeSystemName.ToString().ToLower()) {
							gridMain.SetSelected(i,true);
						}
					}
				}
				Download();
			}
		}
		
		private void FillGrid() {
			_listCodeSystems=CodeSystems.GetForCurrentVersion(_isMemberNation);
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn("Code System",200);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Current Version",100);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Available Version",100);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Download Status",100);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listCodeSystems.Count;i++){
				row=new GridRow();
				row.Cells.Add(_listCodeSystems[i].CodeSystemName);
				row.Cells.Add(_listCodeSystems[i].VersionCur);
				row.Cells.Add(_listCodeSystems[i].VersionAvail);
				//Initialize with the status which may have been set during pre-download in butDownload_Click. This cell will be updated on download progress updates.
				string status="";
				_mapCodeSystemStatus.TryGetValue(_listCodeSystems[i].CodeSystemName,out status);
				row.Cells.Add(status);
				row.Tag=_listCodeSystems[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}
				
		private void butCheckUpdates_Click(object sender,EventArgs e) {
			CheckUpdates();
		}

		private void CheckUpdates() {
			Cursor=Cursors.WaitCursor;
			butDownload.Enabled=false;
			checkKeepDescriptions.Enabled=false;
			try {
				string result="";
				result=RequestCodeSystemsXml();
				XmlDocument doc=new XmlDocument();
				doc.LoadXml(result);
				List<CodeSystem> listCodeSystemsAvailable=CodeSystems.GetForCurrentVersion(_isMemberNation);
				for(int i=0;i<listCodeSystemsAvailable.Count;i++) {
					string codeSystemName=listCodeSystemsAvailable[i].CodeSystemName;
					try {
						XmlNode node=doc.SelectSingleNode("//"+codeSystemName);
						if(node!=null) {
							listCodeSystemsAvailable[i].VersionAvail=node.Attributes["VersionAvailable"].InnerText;
						}
						else {
							listCodeSystemsAvailable[i].VersionAvail=@"N\A";
						}
						CodeSystems.Update(listCodeSystemsAvailable[i]);						
					}
					catch {
						//Might happen if they are running this tool without the right rows in the CodeSystem table? Maybe.
						//Don't prevent the rest of the code systems from being downloaded just because 1 failed.
						continue;
					}
				}
				FillGrid();
				//It is now safe to allow downloading.
				butDownload.Enabled=true;
				checkKeepDescriptions.Enabled=true;
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g("CodeSystemImporter","Error"+": "+ex.Message));
			}
			Cursor=Cursors.Default;
		}

		private void butDownload_Click(object sender,EventArgs e) {
			Download();
		}

		private void Download() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {//Do not let users download code systems when using the middle tier.
				MsgBox.Show("CodeSystemImporter","Cannot download code systems when using the middle tier.");
				return;
			}
			if(gridMain.GetSelectedIndex()==-1) {
				MsgBox.Show("CodeSystemImporter","No code systems selected.");
				return;
			}
			_mapCodeSystemStatus.Clear();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {		
				CodeSystem codeSystem=_listCodeSystems[gridMain.SelectedIndices[i]];
				try {
					//Show warnings and prompts
					if(!PreDownloadHelper(codeSystem.CodeSystemName)) {
						_mapCodeSystemStatus[codeSystem.CodeSystemName]=Lan.g("CodeSystemImporter","Import cancelled");
						continue;
					}
					//CPT codes require user to choose a local file so we will not do this on a thread.
					//We will handle the CPT import right here on the main thread before we start all other imports in parallel below.
					if(codeSystem.CodeSystemName=="CPT") {
						#region Import CPT codes
						//Default status for CPT codes. We will clear this below if the file is selected and unzipped succesfully.
						_mapCodeSystemStatus[codeSystem.CodeSystemName]=Lan.g("CodeSystemImporter","To purchase CPT codes go to https://commerce.ama-assn.org/store/");
						if(!MsgBox.Show("CodeSystemImporter",MsgBoxButtons.OKCancel,"CPT codes must be purchased from the American Medical Association separately in the data file format. "
							+"Please consult the online manual to help determine if you should purchase these codes and how to purchase them. Most offices are not required to purchase these codes. "
							+"If you have already purchased the code file click OK to browse to the downloaded file.")) {
							continue;
						}
						OpenFileDialog fdlg=new OpenFileDialog();
						fdlg.Title=Lan.g("CodeSystemImporter","Choose CPT .zip file");
						fdlg.InitialDirectory=Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
						fdlg.Filter="zip|*.zip";
						fdlg.RestoreDirectory=true;
						fdlg.Multiselect=false;
						if(fdlg.ShowDialog()!=DialogResult.OK) {
							continue;
						}
						if(!fdlg.FileName.ToLower().EndsWith(".zip")) {
							_mapCodeSystemStatus[codeSystem.CodeSystemName]=Lan.g("CodeSystemImporter","Could not locate .zip file in specified folder.");
							continue;
						}
						string versionID="";
						if(fdlg.FileName.ToLower().Contains("cpt-2014-data-files-download.zip")) {
							versionID="2014";
						}
						else if(fdlg.FileName.ToLower().Contains("cpt-2015-data-files.zip")) {
							versionID="2015";
						}
						else if(fdlg.FileName.ToLower().Contains("dl513216_2016.zip")) {
							versionID="2016";
						}
						else if(fdlg.FileName.ToLower().Contains("dl513217.zip")) {
							versionID="2017";
						}
						//Unzip the compressed file-----------------------------------------------------------------------------------------------------
						bool foundFile=false;
						string meduFileName="MEDU.txt";//MEDU stands for MEDium desciption Upper case.
						MemoryStream ms=new MemoryStream();
						using(ZipFile unzipped=ZipFile.Read(fdlg.FileName)) {
							for(int unzipIndex=0;unzipIndex<unzipped.Count;unzipIndex++) {//unzip/write all files to the temp directory
								ZipEntry ze=unzipped[unzipIndex];
								if(!ze.FileName.ToLower().Contains("medu.txt")) {  //This file used to be called "medu.txt.txt" and is now called "medu.txt".  Uses .Contains() to catch both cases.
									continue;
								}
								meduFileName=ze.FileName;
								ze.Extract(PrefC.GetTempFolderPath(),ExtractExistingFileAction.OverwriteSilently);
								foundFile=true;
							}
						}
						if(!foundFile) {
							_mapCodeSystemStatus[codeSystem.CodeSystemName]=Lan.g("CodeSystemImporter","MEDU.txt file not found in zip archive.");  //Used to be MEDU.txt.txt, For error purposes we'll just show .txt
							continue;
						}
						if(versionID=="") {
							//This prompt has the chance of allowing users to "corrupt" their data and import codes for one year and identify them as codes from a 
							//different year. To fix this would require a manual query. If this is a common occurance, consider automating this step based on the codes
							//detected inside the MEDU file.
							using InputBox input=new InputBox("What year are these CPT codes for?");
							if(input.ShowDialog()==DialogResult.Cancel || !Regex.IsMatch(input.textResult.Text,@"^\d{4}$")) {//A four digit value was not entered.
								_mapCodeSystemStatus[codeSystem.CodeSystemName]=Lan.g("CodeSystemImporter","CPT code year must be specified.");
								continue;
							}
							versionID=input.textResult.Text;
						}
						//Add a new thread. We will run these all in parallel once we have them all queued.
						UpdateCodeSystemThread.Add(ODFileUtils.CombinePaths(PrefC.GetTempFolderPath(),meduFileName),
							_listCodeSystems[gridMain.SelectedIndices[i]],new UpdateCodeSystemThread.UpdateCodeSystemArgs(UpdateCodeSystemThread_UpdateSafe),
							versionID,!checkKeepDescriptions.Checked);
						//We got this far so the local file was retreived successfully. No initial status to report.
						_mapCodeSystemStatus[codeSystem.CodeSystemName]="";
						#endregion
					}
					else {
						#region Import all other codes
						//Add a new thread. We will run these all in parallel once we have them all queued.
						//This code system file does not exist on the system so it will be downloaded before being imported.
						if(codeSystem.CodeSystemName=="SNOMEDCT") {//SNOMEDCT codes cannot be given out to non-member nations.  We treat non-USA reg keys as non-member nations.
							//Ensure customer has a valid USA registration key
							#if DEBUG
								OpenDental.localhost.Service1 regService=new OpenDental.localhost.Service1();
							#else
								OpenDental.customerUpdates.Service1 regService=new OpenDental.customerUpdates.Service1();
								regService.Url=PrefC.GetString(PrefName.UpdateServerAddress);
							#endif
							if(PrefC.GetString(PrefName.UpdateWebProxyAddress) !="") {
								IWebProxy proxy = new WebProxy(PrefC.GetString(PrefName.UpdateWebProxyAddress));
								ICredentials cred=new NetworkCredential(PrefC.GetString(PrefName.UpdateWebProxyUserName),PrefC.GetString(PrefName.UpdateWebProxyPassword));
								proxy.Credentials=cred;
								regService.Proxy=proxy;
							}
							XmlWriterSettings settings = new XmlWriterSettings();
							settings.Indent = true;
							settings.IndentChars = ("    ");
							StringBuilder strbuild=new StringBuilder();
							using(XmlWriter writer=XmlWriter.Create(strbuild,settings)) {
								writer.WriteStartElement("IsForeignRegKeyRequest");
								writer.WriteStartElement("RegistrationKey");
								writer.WriteString(PrefC.GetString(PrefName.RegistrationKey));
								writer.WriteEndElement();
								writer.WriteEndElement();
							}
							string result=regService.IsForeignRegKey(strbuild.ToString());
							XmlDocument doc=new XmlDocument();
							doc.LoadXml(result);
							XmlNode node=doc.SelectSingleNode("//IsForeign");
							bool isForeignKey=true;
							if(node!=null) {
								if(node.InnerText=="false") {
									isForeignKey=false;
								}
							}
							if(isForeignKey) {
								string errorMessage=Lan.g(this,"SNOMEDCT has been skipped")+":\r\n";
								node=doc.SelectSingleNode("//ErrorMessage");
								if(node!=null) {
									errorMessage+=node.InnerText;
								}
								//The user will have to click OK on this message in order to continue downloading any additional code systems.
								//In the future we might turn this into calling a delegate in order to update the affected SNOMED row's text instead of stopping the main thread.
								MessageBox.Show(errorMessage);
								continue;
							}
						}
						UpdateCodeSystemThread.Add(_listCodeSystems[gridMain.SelectedIndices[i]],
							new UpdateCodeSystemThread.UpdateCodeSystemArgs(UpdateCodeSystemThread_UpdateSafe),!checkKeepDescriptions.Checked);
						#endregion
					}
				}
				catch(Exception ex) {
					//Set status for this code system.
					_mapCodeSystemStatus[codeSystem.CodeSystemName]=Lan.g("CodeSystemImporter",ex.Message);
				}
			}
			//Threads are all ready to go start them all in parallel. We will re-enable these buttons when we handle the UpdateCodeSystemThread.Finished event.
			if(UpdateCodeSystemThread.StartAll()) {
				butDownload.Enabled=false;
				butCheckUpdates.Enabled=false;
			}
			_hasDownloaded=true;
			FillGrid();
		}

		///<summary>Returns a list of available code systems.  Throws exceptions, put in try catch block.</summary>
		private static string RequestCodeSystemsXml() {
			OpenDental.customerUpdates.Service1 updateService=new OpenDental.customerUpdates.Service1();
			updateService.Url=PrefC.GetString(PrefName.UpdateServerAddress);
			if(PrefC.GetString(PrefName.UpdateWebProxyAddress) !="") {
				IWebProxy proxy=new WebProxy(PrefC.GetString(PrefName.UpdateWebProxyAddress));
				ICredentials cred=new NetworkCredential(PrefC.GetString(PrefName.UpdateWebProxyUserName),PrefC.GetString(PrefName.UpdateWebProxyPassword));
				proxy.Credentials=cred;
				updateService.Proxy=proxy;
			}
			return updateService.RequestCodeSystems("");//may throw error.  No security on this webmethod.
		}

		///<summary>Used to show EULA or other pre-download actions.  Displays message boxes. Returns false if pre-download checks not satisfied.</summary>
		private bool PreDownloadHelper(string codeSystemName) {
			string programVersion=PrefC.GetString(PrefName.ProgramVersion);
			switch(codeSystemName) {
				//Code system specific pre-download actions.
				case "SNOMEDCT":
					#region SNOMEDCT EULA
					string EULA=@"Open Dental "+programVersion+@" includes SNOMED Clinical Terms® (SNOMED CT®) which is used by permission of the International Health Terminology Standards Development Organization (IHTSDO). All rights reserved. SNOMED CT® was originally created by the College of American Pathologists. “SNOMED”, “SNOMED CT” and “SNOMED Clinical Terms” are registered trademarks of the IHTSDO (www.ihtsdo.org).
Use of SNOMED CT in Open Dental "+programVersion+@" is governed by the conditions of the following SNOMED CT Sub-license issued by Open Dental Software Inc.
1. The meaning of the terms “Affiliate”, or “Data Analysis System”, “Data Creation System”, “Derivative”, “End User”, “Extension”, “Member”, “Non-Member Territory”, “SNOMED CT” and “SNOMED CT Content” are as defined in the IHTSDO Affiliate License Agreement (see www.ihtsdo.org/license.pdf).
2. Information about Affiliate Licensing is available at www.ihtsdo.org/license. Individuals or organizations wishing to register as IHTSDO Affiliates can register at www.ihtsdo.org/salsa, subject to acceptance of the Affiliate License Agreement (see www.ihtsdo.org/license.pdf).
3. The current list of IHTSDO Member Territories can be viewed at www.ihtsdo.org/members. Countries not included in that list are “Non-Member Territories”.
4. End Users, that do not hold an IHTSDO Affiliate License, may access SNOMED CT® using Open Dental subject to acceptance of and adherence to the following sub-license limitations:
  a) The sub-licensee is only permitted to access SNOMED CT® using this software (or service) for the purpose of exploring and evaluating the terminology.
  b) The sub-licensee is not permitted the use of this software as part of a system that constitutes a SNOMED CT “Data Creation System” or “Data Analysis System”, as defined in the IHTSDO Affiliate License. This means that the sub-licensee must not use Open Dental "+programVersion+@" to add or copy SNOMED CT identifiers into any type of record system, database or document.
  c) The sub-licensee is not permitted to translate or modify SNOMED CT Content or Derivatives.
  d) The sub-licensee is not permitted to distribute or share SNOMED CT Content or Derivatives.
5. IHTSDO Affiliates may use Open Dental "+programVersion+@" as part of a “Data Creation System” or “Data Analysis System” subject to the following conditions:
  a) The IHTSDO Affiliate, using Open Dental "+programVersion+@" must accept full responsibility for any reporting and fees due for use or deployment of such a system in a Non-Member Territory.
  b) The IHTSDO Affiliate must not use Open Dental "+programVersion+@" to access or interact with SNOMED CT in any way that is not permitted by the Affiliate License Agreement.
  c) In the event of termination of the Affiliate License Agreement, the use of Open Dental "+programVersion+@" will be subject to the End User limitations noted in 4.";
					#endregion
					using(MsgBoxCopyPaste FormMBCP=new MsgBoxCopyPaste(EULA)) {
						FormMBCP.ShowDialog();
						if(FormMBCP.DialogResult!=DialogResult.OK) {
							MsgBox.Show("CodeSystemImporter","SNOMED CT codes will not be imported.");
							return false;//next selected index
						}
					}
					break;
				case "LOINC"://Main + third party
					#region Loinc EULA
					string LoincEULA=@"LOINC and RELMA version 2.44
Released June 29, 2013

This product includes all or a portion of the LOINC® table, LOINC panels and forms file, LOINC document ontology file, and/or LOINC hierarchies file, or is derived from one or more of the foregoing, subject to a license from Regenstrief Institute, Inc. Your use of the LOINC table, LOINC codes, LOINC panels and forms file, LOINC document ontology file, and LOINC hierarchies file also is subject to this license, a copy of which is available at http://loinc.org/terms-of-use. The current complete LOINC table, LOINC Users' Guide, LOINC panels and forms file, LOINC document ontology file, and LOINC hierarchies file are available for download at http://loinc.org. The LOINC table and LOINC codes are copyright © 1995-2013, Regenstrief Institute, Inc. and the Logical Observation Identifiers Names and Codes (LOINC) Committee. The LOINC panels and forms file, LOINC document ontology file, and LOINC hierarchies file are copyright © 1995-2013, Regenstrief Institute, Inc. All rights reserved. THE LOINC TABLE (IN ALL FORMATS), LOINC PANELS AND FORMS FILE, LOINC DOCUMENT ONTOLOGY FILE, AND LOINC HIERARCHIES ARE PROVIDED ""AS IS.""  ANY EXPRESS OR IMPLIED WARRANTIES ARE DISCLAIMED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. LOINC® is a registered United States trademark of Regenstrief Institute, Inc. A small portion of the LOINC table may include content (e.g., survey instruments) that is subject to copyrights owned by third parties. Such content has been mapped to LOINC terms under applicable copyright and terms of use. Notice of such third party copyright and license terms would need to be included if such content is included.

Copyright Notice and License: http://loinc.org/terms-of-use 
The LOINC® codes, LOINC® table (regardless of format), LOINC® Release Notes, LOINC® Changes File, and LOINC® Users' Guide are copyright © 1995-2013, Regenstrief Institute, Inc. and the Logical Observation Identifiers Names and Codes (LOINC) Committee. All rights reserved. 
The RELMA® program, RELMA® database and associated search index files (subject to the copyright above with respect to the LOINC® codes and LOINC® table included therein), RELMA® Community Mapping Feature Database, RELMA® Release Notes, and RELMA® Users' Manual are copyright © 1995-2013, Regenstrief Institute, Inc. All rights reserved.
The LOINC® panels and forms file, LOINC® document ontology file, and the LOINC® hierarchies file (subject to the copyright above with respect to the LOINC® codes and LOINC® table to the extent included therein), are copyright © 1995-2013, Regenstrief Institute, Inc. All rights reserved.
LOINC® and RELMA® are registered United States trademarks of Regenstrief Institute, Inc.  
Permission is hereby granted in perpetuity, without payment of license fees or royalties, to use, copy, or distribute the RELMA® program, RELMA® Users' Manual, RELMA® Release Notes, RELMA® database and associated search index files, LOINC® codes, LOINC® Users' Guide, LOINC® table (in all formats in which it is distributed by Regenstrief Institute, Inc. and the LOINC Committee), LOINC® Release Notes, LOINC® Changes File, LOINC® panels and forms file, LOINC® document ontology file, and LOINC® hierarchies file (collectively, the ""Licensed Materials"") for any commercial or non-commercial purpose, subject to the following terms and conditions:
1. To prevent the dilution of the purpose of the LOINC codes and LOINC table of providing a definitive standard for identifying clinical information in electronic reports, users shall not use any of the Licensed Materials for the purpose of developing or promulgating a different standard for identifying patient observations, such as laboratory test results; other diagnostic service test results; clinical observations and measurements; reports produced by clinicians and diagnostic services about patients; panels, forms, and collections that define aggregations of these observations; and orders for these entities in electronic reports and messages.
2. If the user elects to use the RELMA program, users receive the full RELMA database and associated search index files with the RELMA program, including the LOINC table and other database tables comprising the RELMA database. In addition to its use with the RELMA program, users may use the LOINC table by itself and may modify the LOINC table as permitted herein. Users may not use or modify the other database tables from the RELMA database or the associated search index files except in conjunction with their authorized use of the RELMA program, unless prior written permission is granted by the Regenstrief Institute, Inc. To request written permission, please contact loinc@regenstrief.org. The RELMA program also provides access to certain internet-based content copyrighted by Regenstrief Institute. No additional permission to modify or distribute this internet-based content is granted through the user’s use of the RELMA program.
3. The RELMA program also includes the RELMA Community Mappings feature and access to the RELMA Community Mappings feature database. The accuracy and completeness of the information in the RELMA Community Mappings feature is not verified by Regenstrief or the LOINC Committee. Through the RELMA Community Mappings feature, users will have the option of submitting information, including user’s local mappings, back to the RELMA Community Mappings feature database.
a. By using the RELMA Community Mappings feature, users agree as follows:
i. Users may not copy, distribute, or share access to the information provided by the RELMA Community Mappings feature.
ii. Users accept the risk of using the information provided by the RELMA Community Mappings feature, recognize that such information is submitted by other users, and understand that neither Regenstrief Institute, Inc. nor the LOINC Committee are liable for the information provided by the RELMA Community Mappings feature.
iii. Regenstrief may contact users regarding:
1. Use of the RELMA Community Mappings feature;
2. Submission requests for additional information; and
3. Any mapping submissions that the user makes to the RELMA Community Mappings feature database;
iv. Others may contact user about submissions made to the RELMA Community Mappings feature database;
v. Regenstrief may collect information about use of these services including, but not limited to:
1. Device specific information such as hardware model, operating system, and version;
2. Internet Protocol address;
3. How user used the service (such as search queries run and about which LOINC code terms accessory information was reviewed);
4. User’s contact name, email, and organization; and
5. Regenstrief may associate this information with a user’s account on loinc.org;
vi. User will make reasonable efforts to submit user’s mappings back to the RELMA Community Mappings feature database, which may contain the following information (as applicable):
1. Local battery/panel/test code
2. Local battery/panel/test name/description
3. Units of Measure
4. LOINC code to which it is mapped
5. Date of mapping
6. Language of test names
7. Version of LOINC used to do the mapping
8. Contact information;
vii. If a user submits mappings on behalf of an organization, the user represents that the user has the authority to agree to these terms on behalf of user’s organization.
viii. If a user submits mappings back to the RELMA Community Mappings feature database, then the user hereby grants, on behalf of themselves and user’s organization, Regenstrief a non-exclusive license without payment or fees to submitted mappings in perpetuity for purposes related to LOINC, RELMA, and Regenstrief’s mission, including, but not limited to:
1. Making information publicly available;
2. Performing aggregate analysis;
3. Conducting and publishing research that does not identify user or user’s organization by name;
4. Developing and enhancing LOINC and associated software tools. 
4. Users shall not change the meaning of any of the LOINC codes. Users shall not change the name of, or any contents of, any fields in the LOINC table. Users may add new fields to the LOINC table to attach additional information to existing LOINC records. Users shall not change the content or structure of the LOINC document ontology file or the LOINC panels and forms from the LOINC panels and forms file, but may notify the Regenstrief Institute of any potential inconsistencies or corrections needed by contacting loinc@regenstrief.org.
5. A user may delete records from the LOINC table to deal with the user's local requirements. A user also may add new records to the LOINC table to deal with the users' local requirements, provided that if new records are added, any new entry in the LOINC_NUM field of such new records must contain a leading alphabetic ""X"" so that the new codes and records cannot be confused with existing LOINC codes or new LOINC codes as they are defined in later releases of the LOINC table. Records deleted or added by users to deal with local requirements are not reflected in the official LOINC table maintained by the Regenstrief Institute and the LOINC Committee. Users must also make reasonable efforts to submit requests to LOINC for new records to cover observations that are not found in the LOINC table in order to minimize the need for X-codes.
6. LOINC codes and other information from the LOINC table may be used in electronic messages for laboratory test results and clinical observations such as HL7 ORU messages, without the need to include this Copyright Notice and License or a reference thereto in the message (and without the need to include all fields required by Section 8 hereof). When the LOINC code (from the LOINC_NUM field) is included in the message, users are encouraged, but not required, to include the corresponding LOINC short name (from the SHORTNAME field) or the LOINC long common name (from the LONG_COMMON_NAME field) in the message if the message provides a place for a text name representation of the code. 
7. Users may make and distribute an unlimited number of copies of the Licensed Materials. Each copy thereof must include this Copyright Notice and License, and must include the appropriate version number of the Licensed Materials if the Licensed Materials have a version number, or the release date if the Licensed Materials do not have a version number. This Copyright Notice and License must appear on every printed copy of the LOINC table. Where the Licensed Materials are distributed on a fixed storage medium (such as CD-ROM), a printed copy of this Copyright Notice and License must be included on or with the storage medium, and a text file containing this information also must be stored on the storage medium in a file called ""license.txt"". Where the Licensed Materials are distributed via the Internet, this Copyright Notice and License must be accessible on the same Internet page from which the Licensed Materials are available for download. This Copyright Notice and License must appear verbatim on every electronic or printed copy of the RELMA Users' Manual and the LOINC Users' Guide. The RELMA Users' Manual and the LOINC Users' Guide may not be modified, nor may derivative works of the RELMA Users' Manual or LOINC Users' Guide be created, without the prior written permission of the Regenstrief Institute, Inc. To request written permission, please contact loinc@regenstrief.org. The Regenstrief Institute retains the right to approve any modification to, or derivative work of, the RELMA Users' Manual or the LOINC Users' Guide.
8.  Subject to Section 1 and the other restrictions hereof, users may incorporate portions of the LOINC table, LOINC panels and forms file, LOINC document ontology file, and LOINC hierarchies file into another master term dictionary (e.g. laboratory test definition database), or software program for distribution outside of the user's corporation or organization, provided that any such master term dictionary or software program includes the following fields reproduced in their entirety from the LOINC table: LOINC_NUM, COMPONENT, PROPERTY, TIME_ASPCT, SYSTEM, SCALE_TYP, METHOD_TYP, STATUS, and SHORTNAME. Users are also required to either: (1) include the EXTERNAL_COPYRIGHT_NOTICE or (2) delete the rows that include third party copyrighted content (e.g., third party survey instruments and answers). If third party content is included, users are required to comply with any such third party copyright license terms. Users are encouraged, but not required, to also include the RelatedNames2 and the LONG_COMMON_NAME in any such database. Further description of these fields is provided in Appendix A of the LOINC Users' Guide. Every copy of the LOINC table, LOINC panels and forms file, LOINC document ontology file, and/or LOINC hierarchies file incorporated into or distributed in conjunction with another database or software program must include the following notice:

""This product includes all or a portion of the LOINC® table, LOINC panels and forms file, LOINC document ontology file, and/or LOINC hierarchies file, or is derived from one or more of the foregoing, subject to a license from Regenstrief Institute, Inc. Your use of the LOINC table, LOINC codes, LOINC panels and forms file, LOINC document ontology file, and LOINC hierarchies file also is subject to this license, a copy of which is available at http://loinc.org/terms-of-use. The current complete LOINC table, LOINC Users' Guide, LOINC panels and forms file, LOINC document ontology file, and LOINC hierarchies file are available for download athttp://loinc.org. The LOINC table and LOINC codes are copyright © 1995-2013, Regenstrief Institute, Inc. and the Logical Observation Identifiers Names and Codes (LOINC) Committee. The LOINC panels and forms file, LOINC document ontology file, and LOINC hierarchies file are copyright © 1995-2013, Regenstrief Institute, Inc. All rights reserved. THE LOINC TABLE (IN ALL FORMATS), LOINC PANELS AND FORMS FILE, LOINC DOCUMENT ONTOLOGY FILE, AND LOINC HIERARCHIES ARE PROVIDED ""AS IS.""  ANY EXPRESS OR IMPLIED WARRANTIES ARE DISCLAIMED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. LOINC® is a registered United States trademark of Regenstrief Institute, Inc. A small portion of the LOINC table may include content (e.g., survey instruments) that is subject to copyrights owned by third parties. Such content has been mapped to LOINC terms under applicable copyright and terms of use. Notice of such third party copyright and license terms would need to be included if such content is included.""

If the master term dictionary or software program containing the LOINC table, LOINC panels and forms file, LOINC document ontology file, and/or LOINC hierarchies file is distributed with a printed license, this statement must appear in the printed license. Where the master term dictionary or software program containing the LOINC table, LOINC panels and forms file, LOINC document ontology file, and/or LOINC hierarchies file is distributed on a fixed storage medium, a text file containing this information also must be stored on the storage medium in a file called ""LOINC_short_license.txt"". Where the master term dictionary or software program containing the LOINC table, LOINC panels and forms file, LOINC document ontology file, and/or LOINC hierarchies file is distributed via the Internet, this information must be accessible on the same Internet page from which the product is available for download. 
9. Subject to Section 1 and the other restrictions hereof, users may incorporate portions of the LOINC table and LOINC panels and forms file into another document (e.g., an implementation guide or other technical specification) for distribution outside of the user's corporation or organization, subject to these terms:
a. Every copy of the document that contains portions of the LOINC table or LOINC panels and forms file must include the following notice:

""This material contains content from LOINC® (http://loinc.org). The LOINC table, LOINC codes, and LOINC panels and forms file are copyright © 1995-2013, Regenstrief Institute, Inc. and the Logical Observation Identifiers Names and Codes (LOINC) Committee and available at no cost under the license at http://loinc.org/terms-of-use.”
b. Users are strongly encouraged, but not required, to indicate the appropriate version number of the Licensed Material used.
c. Any information in the document that is extracted from the LOINC table or LOINC panels and forms file must always be associated with the corresponding LOINC code.
d. Along with the LOINC code, users are required to include one of the following LOINC display names:
i. The fully-specified name, which includes the information from the COMPONENT, PROPERTY, TIME_ASPCT, SYSTEM, SCALE_TYP, and METHOD_TYP fields;
ii. The LOINC short name (from the SHORTNAME field); and
iii. The LOINC long common name (from the LONG_COMMON_NAME field).
e. Users are also required to either:
i. Include the EXTERNAL_COPYRIGHT_NOTICE, or
ii. Exclude information from the rows that include third party copyrighted content (e.g., third party survey instruments and answers). If third party content is included, users are required to comply with any such third party copyright license terms.
10. Use and distribution of the Licensed Materials in ways that are not specifically discussed herein shall always be accompanied by the notice provided in Section 8 hereof. The guidelines for providing the notice that are contained in the last paragraph of Section 8 also shall apply. If a user has a question about whether a particular use of any of the Licensed Materials is permissible, the user is invited to contact the Regenstrief Institute by e-mail at loinc@regenstrief.org.
11. If the user desires to translate any of the Licensed Materials into a language other than English, then user shall notify Regenstrief via email at loinc@regenstrief.org. Any such translation is a derivative work, and the user agrees and does hereby assign all right, title and interest in and to such derivative work: (1) to Regenstrief and the LOINC Committee if the translation is a derivative of the LOINC codes, LOINC Users' Guide, or LOINC table, and (2) to Regenstrief if the translation is a derivative work of the RELMA program, LOINC panels and forms file, LOINC document ontology file, LOINC hierarchies file, RELMA Users' Manual, RELMA database or associated search index files. Further, user shall fully cooperate with Regenstrief in the filing and reviewing of any copyright applications or other legal documents, and signing any documents (such as declarations, assignments, affidavits, and the like) that are reasonably necessary to the preparation of any such copyright application. The assignment granted by this paragraph extends to all proprietary rights both in the United States, and in all foreign countries. No other right to create a derivative work of any of the Licensed Materials is hereby granted (except the right to translate into a language other than English granted in this Section), and Regenstrief and the LOINC Committee respectively reserve all other rights not specifically granted herein. All such translations shall be electronically transmitted to Regenstrief, and such translations shall be made available and are subject to the same license rights and restrictions contained herein. Regenstrief will give credit on the LOINC website (and on screens in RELMA) to the user and/or entity that did the translation.
12. The Regenstrief Institute, Inc. and the LOINC Committee welcome requests for new LOINC content (terms, codes, or associated material such as text descriptions and synonyms) and suggestions about revisions to existing content within the Licensed Materials. Any content submitted in conjunction with such a request is subject to the LOINC Submissions Policy, which is available at http://loinc.org/submissions-policy.
13. The names ""Regenstrief,"" ""Regenstrief Foundation,"" ""Regenstrief Institute,"" and ""LOINC Committee"" may not be used in a way which could be interpreted as an endorsement or a promotion of any product or service without prior written permission of the Regenstrief Institute, Inc. Further, no right to use the trademarks of Regenstrief is licensed hereunder. To request written permission, please contact loinc@regenstrief.org.
14. DISCLAIMER:  REGENSTRIEF INSTITUTE, INC. AND THE LOINC COMMITTEE, AS WELL AS ANY CONTRIBUTORS WHO HAVE PROVIDED TRANSLATIONS OF THE LICENSED MATERIALS, DO NOT ACCEPT LIABILITY FOR ANY OMISSIONS OR ERRORS IN THE LICENSED MATERIALS OR ANY OTHER MATERIALS OBTAINED FROM REGENSTRIEF INSTITUTE, INC. AND/OR THE LOINC COMMITTEE. THE LICENSED MATERIALS AND ALL OTHER MATERIALS OBTAINED FROM REGENSTRIEF INSTITUTE, INC. AND/OR THE LOINC COMMITTEE ARE PROVIDED ""AS IS,"" WITHOUT WARRANTY OF ANY KIND. ANY EXPRESSED OR IMPLIED WARRANTIES ARE HEREBY DISCLAIMED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF TITLE, NON-INFRINGEMENT, MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE AND WARRANTIES ARISING FROM A COURSE OF DEALING, TRADE USAGE, OR TRADE PRACTICE. FURTHER, NO WARRANTY OR REPRESENTATION IS MADE CONCERNING THE ACCURACY, COMPLETENESS, SEQUENCE, TIMELINESS OR AVAILABILITY OF THE LICENSED MATERIALS OR ANY OTHER MATERIALS OBTAINED FROM REGENSTRIEF INSTITUTE, INC. AND/OR THE LOINC COMMITTEE, OR ANY TRANSLATIONS OR DERIVATIVE WORKS OF ANY OF THE FOREGOING. IN NO EVENT SHALL REGENSTRIEF INSTITUTE, INC. OR THE LOINC COMMITTEE OR ITS CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, RELIANCE, OR CONSEQUENTIAL DAMAGES OR ATTORNEYS' FEES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; OPPORTUNITY COSTS; LOSS OF USE, DATA, SAVINGS OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THE LICENSED MATERIALS OR ANY OTHER MATERIALS OBTAINED FROM REGENSTRIEF INSTITUTE, INC. AND/OR THE LOINC COMMITTEE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE OR IF SUCH DAMAGES WERE FORESEEABLE. SOME JURISDICTIONS DO NOT ALLOW THE LIMITATION OR EXCLUSION OF CERTAIN WARRANTIES OR CONDITIONS, SO SOME OF THE FOREGOING MAY NOT APPLY TO YOU.
15. This license shall be construed and interpreted in accordance with the laws of the State of Indiana, United States of America, excluding its conflicts of law rules.
 
Notice of Third Party Content and Copyright Terms
A small portion of the content of the LOINC table, LOINC panels and forms file, LOINC document ontology file, LOINC hierarchies file, RELMA database and associated search index files consists of content subject to copyright from third parties. This third party content is either used with permission or under the applicable terms of use. In all such cases, we have included the copyright notice. The copyright of the LOINC codes per se remain owned by Regenstrief Institute, Inc. and the LOINC Committee and subject to the LOINC Copyright Notice and License.
The third party content is identified in the LOINC table by the applicable copyright notice (up to 250 characters) stored in the EXTERNAL_COPYRIGHT_NOTICE field. In RELMA and our web-based search application (http://search.loinc.org), the third party content is highlighted as follows: When such content appears in a search result grid, the programs will display a field with a link to a page containing the copyright notice and terms of use for that content. The programs may also visually highlight the rows of these LOINC codes.
We have included third party content that allows use and distribution at least for clinical, administrative, and research purposes. The third party copyright owners generally ask for attribution of the source, allow the free use of the content for treatment, health care management, and research purposes. They generally forbid alteration of their content (e.g., survey questions and/or answers) and use for commercial purpose, which usually means the direct sale of the survey instruments. They often do allow use of their content in commercial software, medical record and other clinical database systems, and the messaging of patient information collected through the use of these instruments.";
					#endregion
					using(MsgBoxCopyPaste FormLoincEula=new MsgBoxCopyPaste(LoincEULA)) {
						FormLoincEula.ShowDialog();
						if(FormLoincEula.DialogResult!=DialogResult.OK) {
							MsgBox.Show("CodeSystemImporter","LOINC codes will not be imported.");
							return false;//next selected index
						}
					}
					#region External Loinc copyrights
					string externalCopyright=@"External Copyright Notice

Copyright © 2010 FACIT.org. Used with permission

InterRAI holds the copyright to Version 2.0 of the RAI for long term care outside of the US. Content for MDS items in LOINC was derived from Version 2.0 of the RAI/MDS, and should not be reproduced outside of the United States without permission of InterRAI. Within the US, Version 2.0 is in the public domain.

©2010 PROMIS Health Organization or other individuals/entities that have contributed information and materials to Assessment Center, and are being used with the permission of the copyright holders. Use of PROMIS instruments (e.g., item banks, short forms, profile measures) are subject to the PROMIS Terms and Conditions available at: http://www.nihpromis.org/Web%20Pages/Network%20Testing.aspx

©2010 David Cella, PhD. Used with permission. All of these Neuro-QOL instruments can be used at no charge. We do ask, however, that you contact Neuro-QOL project manager, Vitali Ustsinovich, at v-ustsinovich@northwestern.edu and let him know that you intend to use Neuro-QOL.

The Outcome and Assessment Information Set (OASIS)© Copyright © 2002 Center for Health Services Research, UCHSC, Denver, CO. All Rights Reserved.

Copyright © Pfizer Inc. All rights reserved. Developed by Drs. Robert L. Spitzer, Janet B.W. Williams, Kurt Kroenke and colleagues, with an educational grant from Pfizer Inc. No permission required to reproduce, translate, display or distribute.

©2005 American Physical Therapy Association. All rights reserved. Used with permission per LOINC Terms of Use, and in addition: Any further distribution or reproduction must include the following copyright statement: ""Copyright �2005 American Physical Therapy Association. All rights reserved."" Contact permissions@apta.org with questions or for further information.

©2004. All rights reserved. Used with permission per LOINC Terms of Use. The Veterans RAND 12 Item Health Survey (VR-12) was developed from the Veterans RAND 36 Item Health Survey (VR-36) which was developed from the MOS RAND SF-36 Version 1.0. Details involving the VR-36 and VR-12 questionnaires, scoring algorithms for PCS and MCS summary measures, and imputation programs for missing values can be obtained on request by agreeing to the stipulations given by the RAND Corporation website (See http://www.rand.org/health/surveys_tools/mos/mos_core_36item_terms.html) in a letter to Dr. Lewis Kazis (lek@bu.edu) on institutional letter head. Users of the VR-12 or VR-36 in clinical research studies should also notify Dr. Kazis.

©2001 Authors: Suzann K. Campbell, Gay L. Girolami, Thubi, H. A. Kolobe, Elizabeth T. Osten, Maureen C. Lenke. All rights reserved. Reproduced with permission.

Copyright ©PSM ZI Mannheim /Germany

© World Health Organization 2006© World Health Organization 2006. All rights reserved. Used with permission. Publications of the World Health Organization can be obtained from WHO Press, World Health Organization, 20 Avenue Appia, 1211 Geneva 27, Switzerland (tel: +41 22 791 2476; fax: +41 22 791 4857; email: bookorders@who.int). Requests for permission to reproduce or translate WHO publications - whether for sale or for noncommercial distribution - should be addressed to WHO Press, at the above address (fax: +41 22 791 4806; email: permissions@who.int).The designations employed and the presentation of the material in this publication do not imply the expression of any opinion whatsoever on the part of the World Health Organization concerning the legal status of any country, territory, city or area or of its authorities, or concerning the delimitation of its frontiers or boundaries. Dotted lines on maps represent approximate border lines for which there may not yet be full agreement.

Used with permission

© World Health Organization 2006. All rights reserved. Used with permission. Publications of the World Health Organization can be obtained from WHO Press, World Health Organization, 20 Avenue Appia, 1211 Geneva 27, Switzerland (tel: +41 22 791 2476; fax: +41 22 791 4857; email: bookorders@who.int). Requests for permission to reproduce or translate WHO publications - whether for sale or for noncommercial distribution - should be addressed to WHO Press, at the above address (fax: +41 22 791 4806; email: permissions@who.int).The designations employed and the presentation of the material in this publication do not imply the expression of any opinion whatsoever on the part of the World Health Organization concerning the legal status of any country, territory, city or area or of its authorities, or concerning the delimitation of its frontiers or boundaries. Dotted lines on maps represent approximate border lines for which there may not yet be full agreement.The mention of specific companies or of certain manufacturers' products does not imply that they are endorsed or recommended by the World Health Organization in preference to others of a similar nature that are not mentioned. Errors and omissions excepted, the names of proprietary products are distinguished by initial capital letters.All reasonable precautions have been taken by WHO to verify the information contained in this publication. However, the published material is being distributed without warranty of any kind, either express or implied. The responsibility for the interpretation and use of the material lies with the reader. In no event shall the World Health Organization be liable for damages arising from its use.

Copyright © 1996-2005 John Spertus MD MPH, used with permission

Adapted from: Inouye SK, vanDyck CH, Alessi CA, Balkin S, Siegal AP, Horwitz RI. Clarifying confusion: The Confusion Assessment Method. A new method for detection of delirium. Ann Intern Med. 1990; 113: 941-948. Confusion Assessment Method: Training Manual and Coding Guide, Copyright 2003, Sharon K. Inouye, M.D., MPH.

Portions © 1997 Barbara Huffines, used with permission

© Barbara Braden and Nancy Bergstrom 1988. All rights reserved. Used with permission. It is understood that the name of the instrument and the indication that the copyright belongs to Braden and Bergstrom remain on any copies and that you do not make any changes to the wording or scoring of this tool.

© 2002, The Regents of the University of Michigan. Include the following when printing the FLACC on documentation records, etc: Printed with permission © 2002, The Regents of the University of Michigan

Used with permission.

© World Health Organization 2006. All rights reserved. Used with permission. Publications of the World Health Organization can be obtained from WHO Press, World Health Organization, 20 Avenue Appia, 1211 Geneva 27, Switzerland (tel: +41 22 791 2476; fax: +41 22 791 4857; email: bookorders@who.int). Requests for permission to reproduce or translate WHO publications - whether for sale or for noncommercial distribution - should be addressed to WHO Press, at the above address (fax: +41 22 791 4806; email: permissions@who.int).The designations employed and the presentation of the material in this publication do not imply the expression of any opinion whatsoever on the part of the World Health Organization concerning the legal status of any country, territory, city or area or of its authorities, or concerning the delimitation of its frontiers or boundaries. Dotted lines on maps represent approximate border lines for which there may not yet be full agreement.

©1986 Regents of the University of Minnesota, All rights reserved.  Do not copy or reproduce without permission. LIVING WITH HEART FAILURE� is a registered trademark of the Regents of the University of Minnesota.

Copyright © The Hartford Institute for Geriatric Nursing, College of Nursing, New York University, used with permission

MiniCog

Copyright © Pfizer Inc. All rights reserved. Reproduced with permission.Developed by Drs. Robert L. Spitzer, Janet B.W. Williams, Kurt Kroenke and colleagues, with an educational grant from Pfizer Inc. No permission required to reproduce, translate, display or distribute.

Copyright 2011 International Association for the Study of Pain (IASP). Used with permission. Before translating text into a language other than found on the Pediatric Pain Sourcebook (www.painsourcebook.ca), please contact IASP to gain permission.

Copyright © Pfizer Inc. All rights reserved. Reproduced with permission.

Copyright © 2009 by Paul H. Brookes Publishing Co., Inc

Copyright ©NPUAP, used with permission of the National Pressure Ulcer Advisory Panel 2012
";
					#endregion
					using(MsgBoxCopyPaste FormLoincEula2=new MsgBoxCopyPaste(externalCopyright)) {
						FormLoincEula2.ShowDialog();
						if(FormLoincEula2.DialogResult!=DialogResult.OK) {
							MsgBox.Show("CodeSystemImporter","LOINC codes will not be imported.");
							return false;//next selected index
						}
					}
					break;
				case "UCUM":
					#region UCUM EULA
					string UcumEULA=@"Unified Codes for Units of Measures (UCUM) version 1.7
This product includes all or a portion of the UCUM table, UCUM codes, and UCUM definitions or is derived from it, subject to a license from Regenstrief Institute, Inc. and The UCUM Organization. Your use of the UCUM table, UCUM codes, UCUM definitions also is subject to this license, a copy of which is available at http://unitsofmeasure.org. The current complete UCUM table, UCUM Specification are available for download at http://unitsofmeasure.org. The UCUM table and UCUM codes are copyright © 1995-2009, Regenstrief Institute, Inc. and the Unified Codes for Units of Measures (UCUM) Organization. All rights reserved.

THE UCUM TABLE (IN ALL FORMATS), UCUM DEFINITIONS, AND SPECIFICATION ARE PROVIDED ""AS IS."" ANY EXPRESS OR IMPLIED WARRANTIES ARE DISCLAIMED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. 

Copyright Notice and License
The UCUM codes, UCUM table (regardless of format), and UCUM Specification are copyright © 1999-2009, Regenstrief Institute, Inc. and the Unified Codes for Units of Measures (UCUM) Organization. All rights reserved.
Regenstrief Institute, Inc. and the Unified Codes for Units of Measures (UCUM) Organization are hereunder collectively referred to as ""The Organization"".
Permission is hereby granted in perpetuity, without payment of license fees or royalties, to use, copy, or distribute the UCUM codes, UCUM Specification, and UCUM table (in all formats in which it is distributed by The Organization and the UCUM Organization) (collectively, the ""Licensed Materials"") for any commercial or non-commercial purpose, subject to the following terms and conditions:
1) To prevent the dilution of the purpose of the Licensed Materials, i.e., that of providing a definitive standard for identifying units of measures in electronic documents and messages, users shall not use any of the Licensed Materials for the purpose of developing or promulgating a different standard for identifying units of measure, regardless of whether the intended use is in the field of medicine, or any other field of science or trade.
2) Users shall not modify the Licensed Materials and may not distribute modified versions of the UCUM table (regardless of format) or UCUM Specification. Users shall not modify any existing contents, fields, description, or comments of the Licensed Materials, and may not add any new contents to it.
3) Users shall not use any of the UCUM codes in a way that expressly or implicitly changes their meaning.
4) RESERVED
5) UCUM codes and other information from the UCUM table may be used in electronic messages communicating measurements without the need to include this Copyright Notice and License or a reference thereto in the message (and without the need to include all fields required by Section 7 hereof).
6) Users may make and distribute an unlimited number of copies of the Licensed Materials. Each copy thereof must include this Copyright Notice and License, and must include the appropriate version or revision number of the Licensed Materials if the Licensed Materials have a version or revision number, or the release date if the Licensed Materials do not have a version or revision number. This Copyright Notice and License must appear on every printed copy of the Licensed Materials. Where the Licensed Materials are distributed on a fixed storage medium (such as a CD-ROM), a printed copy of this Copyright Notice and License must be included on or with the storage medium, and a text file containing this information also must be stored on the storage medium in a file called ""license.txt"". Where the Licensed Materials are distributed via the Internet, this Copyright Notice and License must be accessible on the same Internet page from which the Licensed Materials are available for download. This Copyright Notice and License must appear verbatim on every electronic or printed copy of the Licensed Materials. The UCUM Specification and related documents may not be modified, nor may derivative works be created from it, without the prior written permission of The Organization. To request written permission, please contact ucum@… (at unitsofmeasure.org). The Organization retains the right to approve any modification to, or derivative work of the Licensed Materials.
7) Subject to Section 1 and the other restrictions hereof, users may incorporate portions of the UCUM table and definitions into another master term dictionary (e.g. laboratory test definition database), or software program for distribution outside of the user's corporation or organization, provided that any such master term dictionary or software program includes the following fields reproduced in their entirety from the UCUM table: UCUM code, definition value and unit. Every copy of the UCUM table incorporated into or distributed in conjunction with another database or software program must include the following notice:
""This product includes all or a portion of the UCUM table, UCUM codes, and UCUM definitions or is derived from it, subject to a license from Regenstrief Institute, Inc. and The UCUM Organization. Your use of the UCUM table, UCUM codes, UCUM definitions also is subject to this license, a copy of which is available at http://unitsofmeasure.org. The current complete UCUM table, UCUM Specification are available for download at http://unitsofmeasure.org. The UCUM table and UCUM codes are copyright © 1995-2009, Regenstrief Institute, Inc. and the Unified Codes for Units of Measures (UCUM) Organization. All rights reserved.
THE UCUM TABLE (IN ALL FORMATS), UCUM DEFINITIONS, AND SPECIFICATION ARE PROVIDED ""AS IS."" ANY EXPRESS OR IMPLIED WARRANTIES ARE DISCLAIMED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE.""
If the master term dictionary or software program containing the UCUM table, UCUM definitions and/or UCUM specification is distributed with a printed license, this statement must appear in the printed license. Where the master term dictionary or software program containing the UCUM table, UCUM definitions, and/or UCUM specification is distributed on a fixed storage medium, a text file containing this information also must be stored on the storage medium in a file called ""UCUM_short_license.txt"". Where the master term dictionary or software program containing the UCUM table, UCUM definitions, and/or UCUM specification is distributed via the Internet, this information must be accessible on the same Internet page from which the product is available for download.
8) Use and distribution of the Licensed Materials in ways that are not specifically discussed herein shall always be accompanied by the notice provided in Section 7 hereof. The guidelines for providing the notice that are contained in the last paragraph of Section 7 also shall apply. If a user has a question about whether a particular use of any of the Licensed Materials is permissible, the user is invited to contact The Organization by e-mail at ucum@… (at unitsofmeasure.org).
9) If the user desires to translate any of the Licensed Materials into a language other than English, then user shall notify The Organization via email at ucum@… (at unitsofmeasure.org). Any such translation is a derivative work, and the user agrees and does hereby assign all right, title and interest in and to such derivative work to The Organization. Further, user shall fully cooperate with The Organization in the filing and reviewing of any copyright applications or other legal documents, and signing any documents (such as declarations, assignments, affidavits, and the like) that are reasonably necessary to the preparation of any such copyright application. The assignment granted by this paragraph extends to all proprietary rights both in the United States, and in all foreign countries. No other right to create a derivative work of any of the Licensed Materials is hereby granted (except the right to translate into a language other than English granted in this Section 9), and The Organization reserves all other rights not specifically granted herein. All such translations shall be electronically transmitted to The Organization, and such translations shall be made available and are subject to the same license rights and restrictions contained herein. The Organization will give credit on its website to the user and/or entity that did the translation.
10) The Organization welcome requests for new UCUM content (terms, codes or associated material such as text descriptions and synonyms) and suggestions about revisions to existing content within the Licensed Materials. Such submissions should be done on The Organization's web site http://unitsofmeasure.org/trac/newticket.
11) The names ""UCUM Organization"" may not be used in a way which could be interpreted as an endorsement or a promotion of any product or service without prior written permission of The Organization. To request written permission, please contact ucum@… (at unitsofmeasure.org).
12) DISCLAIMER: REGENSTRIEF INSTITUTE, INC. AND THE UCUM ORGANIZATION DO NOT ACCEPT LIABILITY FOR ANY OMISSIONS OR ERRORS IN THE LICENSED MATERIALS OR ANY OTHER MATERIALS OBTAINED FROM REGENSTRIEF INSTITUTE, INC. AND/OR THE UCUM ORGANIZATION. THE LICENSED MATERIALS AND ALL OTHER MATERIALS OBTAINED FROM REGENSTRIEF INSTITUTE, INC. AND/OR THE UCUM ORGANIZATION ARE PROVIDED ""AS IS,"" WITHOUT WARRANTY OF ANY KIND. ANY EXPRESSED OR IMPLIED WARRANTIES ARE HEREBY DISCLAIMED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF TITLE, NON-INFRINGEMENT, MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE AND WARRANTIES ARISING FROM A COURSE OF DEALING, TRADE USAGE, OR TRADE PRACTICE. FURTHER, NO WARRANTY OR REPRESENTATION IS MADE CONCERNING THE ACCURACY, COMPLETENESS, SEQUENCE, TIMELINESS OR AVAILABILITY OF THE LICENSED MATERIALS OR ANY OTHER MATERIALS OBTAINED FROM REGENSTRIEF INSTITUTE, INC. AND/OR THE UCUM ORGANIZATION, OR ANY TRANSLATIONS OR DERIVATIVE WORKS OF ANY OF THE FOREGOING. IN NO EVENT SHALL REGENSTRIEF INSTITUTE, INC. OR THE UCUM ORGANIZATION OR ITS CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, RELIANCE, OR CONSEQUENTIAL DAMAGES OR ATTORNEYS' FEES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; OPPORTUNITY COSTS; LOSS OF USE, DATA, SAVINGS OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THE LICENSED MATERIALS OR ANY OTHER MATERIALS OBTAINED FROM REGENSTRIEF INSTITUTE, INC. AND/OR THE UCUM ORGANIZATION, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE OR IF SUCH DAMAGES WERE FORESEEABLE. SOME JURISDICTIONS DO NOT ALLOW THE LIMITATION OR EXCLUSION OF CERTAIN WARRANTIES OR CONDITIONS, SO SOME OF THE FOREGOING MAY NOT APPLY TO YOU.
13) This license shall be construed and interpreted in accordance with the laws of the State of Indiana, United States of America, excluding its conflicts of law rules.";
					#endregion
					using(MsgBoxCopyPaste FormUcumEULA=new MsgBoxCopyPaste(UcumEULA)) {
						FormUcumEULA.ShowDialog();
						if(FormUcumEULA.DialogResult!=DialogResult.OK) {
							MsgBox.Show("CodeSystemImporter","UCUM codes will not be imported.");
							return false;//next selected index
						}
					}
					break;
			}
			return true;
		}

		#region thread handlers
		///<summary>Call this from external thread. Invokes to main thread to avoid cross-thread collision.</summary>
		private void UpdateCodeSystemThread_FinishedSafe(object sender,EventArgs e) {
			try {
				this.BeginInvoke(new EventHandler(UpdateCodeSystemThread_FinishedUnsafe),new object[] { sender,e });				
			}
			//most likely because form is no longer available to invoke to
			catch { }
		}

		///<summary>Do not call this directly from external thread. Use UpdateCodeSystemThread_FinishedSafe.</summary>
		private void UpdateCodeSystemThread_FinishedUnsafe(object sender,EventArgs e) {
				butCheckUpdates.Enabled=true;
				butDownload.Enabled=true;
		}

		///<summary>Call this from external thread. Invokes to main thread to avoid cross-thread collision.</summary>
		private void UpdateCodeSystemThread_UpdateSafe(CodeSystem codeSystem,string status,double percentDone,bool done,bool success,int numUpdated) {
			try {
				this.BeginInvoke(new UpdateCodeSystemThread.UpdateCodeSystemArgs(UpdateCodeSystemThread_UpdateUnsafe),new object[] { codeSystem,status,percentDone,done,success,numUpdated });
			}
			//most likely because form is no longer available to invoke to
			catch { }			
		}

		///<summary>Do not call this directly from external thread. Use UpdateCodeSystemThread_UpdateSafe.</summary>
		private void UpdateCodeSystemThread_UpdateUnsafe(CodeSystem codeSystem,string status,double percentDone,bool done,bool success,int numUpdated) {
			//This is called a lot from the import threads so don't bother with the full FillGrid. Just find our row and column and update the cell's text.
			gridMain.BeginUpdate();
			for(int i=0;i<gridMain.ListGridRows.Count;i++) {
				if(gridMain.ListGridRows[i].Tag==null 
					|| !(gridMain.ListGridRows[i].Tag is CodeSystem)
					|| !(((CodeSystem)gridMain.ListGridRows[i].Tag).CodeSystemName==codeSystem.CodeSystemName)) {
					continue;
				}
				string cellText=((int)percentDone)+"%"+" -- "+status;
				if(done) {
					if(success) {
						//If done==true percentDone is the number of codes that were imported, not the percent done.  
						//This is done so we don't have to change the signatures of exisiting functions but can still alert the user when no codes were actually imported.
						cellText=Lan.g("CodeSystemImporter","Import complete")+"! -- "+Lan.g("CodeSystemImporter","Number of codes imported")+": "
							+Convert.ToInt32(percentDone).ToString()+" "+Lan.g("CodeSystemImporter","Number of codes updated")+": "+numUpdated;
					}
					else {
						cellText=Lan.g("CodeSystemImporter","Import failed")+"! -- "+status;
					}
				}
				gridMain.ListGridRows[i].Cells[3].Text=cellText;
			}
			gridMain.EndUpdate();  //Need to call this instead of gridMain.Invalidate() because we need text wrapping to happen if there was a long error message.
		}
		#endregion

		///<summary>Worker thread class. 1 thread will be spawned for each code sytem being downloaded. All threads will run in parallel.</summary>
		private class UpdateCodeSystemThread {
			///<summary>Number of bytes in a kilobyte.</summary>
			private const int KB_SIZE=1024;
			///<summary>Number of kilobytes to download in each chunk.</summary>
			private const int CHUNK_SIZE=10;
			///<summary>Static lis of threads. All managed internally. Must always be locked by _lock when accessed!!!</summary>
			private static List<UpdateCodeSystemThread> _threads=new List<UpdateCodeSystemThread>();
			///<summary>All access of _threads member MUST BE enclosed with lock statement in order to prevent thread-lock and race conditions.</summary>
			private static object _lock=new object();
			///<summary>The code system being updated.</summary>
			private CodeSystem _codeSystem;			
			///<summary>Download and import functions will check this flag occasionally to see if they should abort prematurely.</summary>
			private bool _quit=false;
			///<summary>Function signature required to send an update.  When done==true, percentDone is the number of codes imported.</summary>			
			public delegate void UpdateCodeSystemArgs(CodeSystem codeSystem,string status,double percentDone,bool done,bool success,int numUpdated);			
			///<summary>Required by ctor. Used to keep main thread aware of update progress.</summary>			
			private UpdateCodeSystemArgs _updateHandler;
			///<summary>Event will be fired when the final thread has finished and all threads have been cleared from the list.</summary>
			public static EventHandler Finished;
			///<summary>If this is a CPT import then the file must exist localally and the file location will be provided by the user. All other code system files are held behind the Customer Update web service and will be downloaded to a temp file location in order to be imported.</summary>
			private string _localFilePath;
			///<summary>The version of the code system being imported.  Currently only used for CPT codes.</summary>
			private string _versionID;
			///<summary>If this code system has been previously imported, this flag will determine whether existing codes are updated.</summary>
			private bool _updateExisting;

			///<summary>Aborts the thread. Only called by StopAll.</summary>
			private void Quit() {
				_quit=true;
			}

			///<summary>Indicates if there are still 1 or more active threads.</summary>
			public static bool IsRunning {
				get {
					lock(_lock){
						return _threads.Count>=1;
					}					
				}
			}

			///<summary>Private ctor. Will only be used internally by Add. If localFilePath is set here then it is assumed that the file exists locally and file download will be skipped before importing data from the file. This will only happen for the CPT code system.</summary>
			private UpdateCodeSystemThread(string localFilePath,CodeSystem codeSystem,UpdateCodeSystemArgs onUpdateHandler, string versionID,
				bool updateExisting) 
			{
				_localFilePath=localFilePath;
				_codeSystem=codeSystem;
				_updateHandler+=onUpdateHandler;
				_versionID=versionID;
				_updateExisting=updateExisting;
			}
			
			///<summary>Provide a nice ledgible identifier.</summary>
			public override string ToString() {
				return _codeSystem.CodeSystemName;
			}

			///<summary>Thread list manager needs this to remove threads. Required for List.Contains.</summary>
			public override bool Equals(object obj) {
				return ((UpdateCodeSystemThread)obj)._codeSystem.CodeSystemNum==_codeSystem.CodeSystemNum;
			}

			///<summary>Just to prevent a VS warning.</summary>
			public override int GetHashCode() {
				return base.GetHashCode();
			}

			///<summary>Add a thread to the queue. These threads will not be started until StartAll is called subsequent to adding all necessary threads. If localFilePath is set here then it is assumed that the file exists locally and file download will be skipped before importing data from the file. This will only happen for the CPT code system.</summary>
			public static void Add(string localFilePath,CodeSystem codeSystem,UpdateCodeSystemArgs onUpdateHandler, string versionID,bool updateExisting) {
				UpdateCodeSystemThread thread=new UpdateCodeSystemThread(localFilePath,codeSystem,onUpdateHandler,versionID,updateExisting);
				lock(_lock) {
					_threads.Add(thread);
				}
			}

			///<summary>Add a thread to the queue. These threads will not be started until StartAll is called subsequent to adding all necessary threads. This version assures that code system file will be downloaded before import. Use for all code system except CPT.</summary>
			public static void Add(CodeSystem codeSystem,UpdateCodeSystemArgs onUpdateHandler,bool updateExisting) {
				Add("",codeSystem,onUpdateHandler,"",updateExisting);
			}

			///<summary>Use this to start the threads once all threads have been added using Add.</summary>
			public static bool StartAll() {				
				bool startedAtLeastOne=false;
				lock(_lock) {
					foreach(UpdateCodeSystemThread thread in _threads) {
						Thread th=new Thread(new ThreadStart(thread.Run));
						th.Name=thread.ToString();
						th.Start();
						startedAtLeastOne=true;
					}
				}
				return startedAtLeastOne;
			}

			///<summary>Sets the Quit flag for all threads. Use this if early abort is desired.</summary>
			public static void StopAll() {
				lock(_lock) {
					foreach(UpdateCodeSystemThread thread in _threads) {
						thread.Quit();
					}
					_threads.Clear();
				}
			}

			///<summary>Called internally each time time a thread has completed. Will trigger the Finished event if this is the last thread to complete.</summary>
			private void Done(string status,bool success, int numCodesImported,int numCodesUpdated) {
				_updateHandler(_codeSystem,status,Convert.ToDouble(numCodesImported),true,success,numCodesUpdated);//Pass in the number of codes imported as percentDone.  This was done so can display the number of codes imported withouth having to add a new perameter to the delegate function.
				bool finished=false;
				lock(_lock) {
					if(_threads.Contains(this)) {
						_threads.Remove(this);
					}
					finished=_threads.Count<=0;
				}
				if(finished && Finished!=null) {
					Finished("UpdateCodeSystemThread",new EventArgs());
				}
			}

			///<summary>Update the current status of this import thread. Thread owner is required to handle this as the delegat is required in the ctor.</summary>
			private void Update(string status,int numDone,int numTotal) {
				double percentDone=0;
				//Guard against illegal division.
				if(numTotal>0) {
					percentDone=100*(numDone/(double)numTotal);
				}
				_updateHandler(_codeSystem,status,percentDone,false,true,0);
			}

			///<summary>Helper used internally.</summary>
			private void ImportProgress(int numDone,int numTotal) {
				Update(Lan.g("CodeSystemImporter","Importing"),numDone,numTotal);
			}

			///<summary>Helper used internally.</summary>
			private void DownloadProgress(int numDone,int numTotal) {
				Update(Lan.g("CodeSystemImporter","Downloading"),numDone,numTotal);
			}

			///<summary>The thread function.</summary>
			private void Run() {
				try {
					string failText="";
					int _numCodesImported=0;
					int numCodesUpdated=0;
					if(!RequestCodeSystemDownloadHelper(ref failText,ref _numCodesImported,ref numCodesUpdated)) {
						throw new Exception(failText);						
					}
					//set current version=available version
					if(_codeSystem.CodeSystemName=="CPT") {
						CodeSystems.UpdateCurrentVersion(_codeSystem, _versionID);
					}
					else {
						CodeSystems.UpdateCurrentVersion(_codeSystem);
					}
					//All good!
					Done(Lan.g("CodeSystemImporter","Import Complete"),true,_numCodesImported,numCodesUpdated);
				}
				catch(Exception ex) {
					//Something failed!
					Done(Lan.g("CodeSystemImporter","Error")+": "+ex.Message,false,0,0);
				}
			}

			///<summary>Will request, download, and import codeSystem from webservice. Returns false if unsuccessful.</summary>
			private bool RequestCodeSystemDownloadHelper(ref string failText,ref int numCodesImported,ref int numCodesUpdated) {
				try {
					//If local file was not provided then try to download it from Customer Update web service. 
					//Local file will only be provided for CPT code system.
					if(string.IsNullOrEmpty(_localFilePath)) { 
						string result=SendAndReceiveDownloadXml(_codeSystem.CodeSystemName);
						XmlDocument doc=new XmlDocument();
						doc.LoadXml(result);
						string strError=WebServiceRequest.CheckForErrors(doc);
						if(!string.IsNullOrEmpty(strError)) {
							throw new Exception(strError);
						}
						XmlNode node=doc.SelectSingleNode("//CodeSystemURL");
						if(node==null) {
							throw new Exception(Lan.g("CodeSystemImporter","Code System URL is empty for ")+": "+_codeSystem.CodeSystemName);
						}
						//Node's inner text contains the URL
						_localFilePath=DownloadFileHelper(node.InnerText);					
					}
					if(!File.Exists(_localFilePath)) {
						throw new Exception(Lan.g("CodeSystemImporter","Local file not found ")+": "+_localFilePath);
					}
					switch(_codeSystem.CodeSystemName) {
						case "CDCREC":
							CodeSystems.ImportCdcrec(_localFilePath,new CodeSystems.ProgressArgs(ImportProgress),ref _quit,ref numCodesImported,ref numCodesUpdated,
								_updateExisting);
							break;
						case "CVX":
							CodeSystems.ImportCvx(_localFilePath,new CodeSystems.ProgressArgs(ImportProgress),ref _quit,ref numCodesImported,ref numCodesUpdated,
								_updateExisting);
							break;
						case "HCPCS":
							CodeSystems.ImportHcpcs(_localFilePath,new CodeSystems.ProgressArgs(ImportProgress),ref _quit,ref numCodesImported,ref numCodesUpdated,
								_updateExisting);
							break;
						case "ICD10CM":
							CodeSystems.ImportIcd10(_localFilePath,new CodeSystems.ProgressArgs(ImportProgress),ref _quit,ref numCodesImported,ref numCodesUpdated,
								_updateExisting);
							break;
						case "ICD9CM":
							CodeSystems.ImportIcd9(_localFilePath,new CodeSystems.ProgressArgs(ImportProgress),ref _quit,ref numCodesImported,ref numCodesUpdated,
								_updateExisting);
							break;
						case "LOINC":
							CodeSystems.ImportLoinc(_localFilePath,new CodeSystems.ProgressArgs(ImportProgress),ref _quit,ref numCodesImported,ref numCodesUpdated,
								_updateExisting);
							break;
						case "RXNORM":
							CodeSystems.ImportRxNorm(_localFilePath,new CodeSystems.ProgressArgs(ImportProgress),ref _quit,ref numCodesImported,ref numCodesUpdated,
								_updateExisting);
							break;
						case "SNOMEDCT":
							CodeSystems.ImportSnomed(_localFilePath,new CodeSystems.ProgressArgs(ImportProgress),ref _quit,ref numCodesImported,ref numCodesUpdated,
								_updateExisting);
							break;
						case "SOP":
							CodeSystems.ImportSop(_localFilePath,new CodeSystems.ProgressArgs(ImportProgress),ref _quit,ref numCodesImported,ref numCodesUpdated,
								_updateExisting);
							break;
						case "UCUM":
							CodeSystems.ImportUcum(_localFilePath,new CodeSystems.ProgressArgs(ImportProgress),ref _quit,ref numCodesImported,ref numCodesUpdated,
								_updateExisting);
							break;
						case "CPT":
							CodeSystems.ImportCpt(_localFilePath,new CodeSystems.ProgressArgs(ImportProgress),ref _quit,ref numCodesImported,ref numCodesUpdated,
								_versionID);
							break;
						case "CDT":  //import not supported
						case "AdministrativeSex":  //import not supported
						default:  //new code system perhaps?
							throw new Exception(Lan.g("CodeSystemImporter","Unsupported Code System")+": "+_codeSystem.CodeSystemName);
					}
					//Import succeded so delete the import file where necessary.
					DeleteImportFileIfNecessary();
					//We got here so everything succeeded.
					return true;
				}
				catch(Exception ex) {
					failText=ex.Message;
				}
				//We got here so something failed.
				return false;
			}

			///<summary>Delete the import file which was created locally. This file was either downloaded or extracted from a zip archive. Either way it is temporary and can be deleted.</summary>
			private void DeleteImportFileIfNecessary() {
				//Don't bother if the file isn't there.
				if(!File.Exists(_localFilePath)) {
					return;
				}				
				//We got this far so assume the file is safe to delete.
				File.Delete(_localFilePath);
			}

			///<summary>Returns temp file name used to download file.  Can throw exception.</summary>
			private string DownloadFileHelper(string codeSystemURL) {
				string zipFileDestination=PrefC.GetRandomTempFile(".tmp");
				//Cleanup existing.
				File.Delete(zipFileDestination);
				try {
					//Perform the download
					DownloadFileWorker(codeSystemURL,zipFileDestination);
					Thread.Sleep(100);//allow file to be released for use by the unzipper.
					//Unzip the compressed file-----------------------------------------------------------------------------------------------------
					using(MemoryStream ms=new MemoryStream())
					using(ZipFile unzipped=ZipFile.Read(zipFileDestination)) {
						ZipEntry ze=unzipped[0];
						ze.Extract(PrefC.GetTempFolderPath(),ExtractExistingFileAction.OverwriteSilently);
						return ODFileUtils.CombinePaths(PrefC.GetTempFolderPath(),unzipped[0].FileName);
					}
				}
				finally{
					//We are done with the zip file.
					File.Delete(zipFileDestination);
				}
			}

			///<summary>Download given URI to given local path. Can throw exception.</summary>
			private void DownloadFileWorker(string codeSystemURL,string destinationPath) {
				byte[] buffer;
				int chunkIndex=0;
				WebRequest wr=WebRequest.Create(codeSystemURL);
				int fileSize=0;
				using(WebResponse webResp=wr.GetResponse()) { //Quickly get the size of the entire package to be downloaded.
					fileSize=(int)webResp.ContentLength;
				}
				using(WebClient myWebClient=new WebClient())
				using(Stream readStream=myWebClient.OpenRead(codeSystemURL))
				using(BinaryReader br=new BinaryReader(readStream))
				using(FileStream writeStream=new FileStream(destinationPath,FileMode.Create))
				using(BinaryWriter bw=new BinaryWriter(writeStream)) {
					while(true) {
						if(_quit) {
							throw new Exception(Lan.g("CodeSystemImporter","Download aborted"));
						}
						//Update the progress.
						DownloadProgress(CHUNK_SIZE*KB_SIZE*chunkIndex,fileSize);
						//Download another chunk.
						buffer=br.ReadBytes(CHUNK_SIZE*KB_SIZE);
						if(buffer.Length==0) { //Nothing left to download so we are done.
							break;
						}
						//Write out to the file.
						bw.Write(buffer);
						chunkIndex++;
					}
				}
			}

			///<summary>Can throw exception.</summary>
			private static string SendAndReceiveDownloadXml(string codeSystemName) {
				//prepare the xml document to send--------------------------------------------------------------------------------------
				XmlWriterSettings settings = new XmlWriterSettings();
				settings.Indent = true;
				settings.IndentChars = ("    ");
				StringBuilder strbuild=new StringBuilder();
				using(XmlWriter writer=XmlWriter.Create(strbuild,settings)) {
					//TODO: include more user information
					writer.WriteStartElement("UpdateRequest");
					writer.WriteStartElement("RegistrationKey");
					writer.WriteString(PrefC.GetString(PrefName.RegistrationKey));
					writer.WriteEndElement();
					writer.WriteStartElement("PracticeTitle");
					writer.WriteString(PrefC.GetString(PrefName.PracticeTitle));
					writer.WriteEndElement();
					writer.WriteStartElement("PracticeAddress");
					writer.WriteString(PrefC.GetString(PrefName.PracticeAddress));
					writer.WriteEndElement();
					writer.WriteStartElement("PracticePhone");
					writer.WriteString(PrefC.GetString(PrefName.PracticePhone));
					writer.WriteEndElement();
					writer.WriteStartElement("ProgramVersion");
					writer.WriteString(PrefC.GetString(PrefName.ProgramVersion));
					writer.WriteEndElement();
					writer.WriteStartElement("CodeSystemRequested");
					writer.WriteString(codeSystemName);
					writer.WriteEndElement();
					writer.WriteEndElement();
				}
#if DEBUG
				OpenDental.localhost.Service1 updateService=new OpenDental.localhost.Service1();
#else
				OpenDental.customerUpdates.Service1 updateService=new OpenDental.customerUpdates.Service1();
				updateService.Url=PrefC.GetString(PrefName.UpdateServerAddress);
#endif
				if(PrefC.GetString(PrefName.UpdateWebProxyAddress) !="") {
					IWebProxy proxy = new WebProxy(PrefC.GetString(PrefName.UpdateWebProxyAddress));
					ICredentials cred=new NetworkCredential(PrefC.GetString(PrefName.UpdateWebProxyUserName),PrefC.GetString(PrefName.UpdateWebProxyPassword));
					proxy.Credentials=cred;
					updateService.Proxy=proxy;
				}
				//may throw error
				return updateService.RequestCodeSystemDownload(strbuild.ToString());
			}
		}
		
		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		///<summary>If there are still import threads running then prompt the user to see if they want to abort the imports prematurely.</summary>
		private void FormCodeSystemsImport_FormClosing(object sender,FormClosingEventArgs e) {
			if(!UpdateCodeSystemThread.IsRunning) { //All done, exit.
				if(_hasDownloaded) {
					DataValid.SetInvalid(InvalidType.EhrCodes);//Update in-memory list of codes for all other workstations
					DataValid.SetInvalid(InvalidType.Diseases);//The purpose of this line is to refresh the ICD9 cache, because the ICD9s do not have an invalid type yet.
					DataValid.SetInvalid(InvalidType.Sops);
				}
				return;
			}
			if(MsgBox.Show("CodeSystemImporter",MsgBoxButtons.YesNo,"Import in progress. Would you like to abort?")) {
				//User wants abort the threads.
				UpdateCodeSystemThread.StopAll();
				_hasDownloaded=false;
				return;
			}
			//User elected to continue waiting so cancel the Close event.
			e.Cancel=true;
		}
	}

	public enum CodeSystemName {
		CDCREC,
		CVX,
		HCPCS,
		ICD10CM,
		ICD9CM,
		LOINC,
		RXNORM,
		SNOMEDCT,
		SOP,
		UCUM,
		CPT,
	}

}