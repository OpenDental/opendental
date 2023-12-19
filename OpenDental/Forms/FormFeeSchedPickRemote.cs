using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormFeeSchedPickRemote:FormODBase {

		///<summary>Url must include trailing slash.</summary>
		public string Url="";
		private List<string> _listFeeSchedFilesOntario;
		private List<string> _listFeeSchedFilesBCDA;
		private const string ONTARIO_DENTAL_ASSOCIATION="ODA";
		private const string BRITISH_COLUMBIA_DENTAL_ASSOCIATION="BCDA";

		///<summary>Returns the full URL of the chosen file.</summary>
		public string getFileChosenUrl() {		
			return Url+getFileChosenName();
		}

		///<summary>Returns just the file name of the chosen file.</summary>
		public string getFileChosenName() {
			if(gridFeeSchedFiles.GetSelectedIndex()==-1) {
				return "";
			}
			return gridFeeSchedFiles.ListGridRows[gridFeeSchedFiles.GetSelectedIndex()].Cells[0].Text;
		}

		///<summary>Returns true if the file chosen must be downloaded from the web service. Currently, Ontario fees must be downloaded from the web service.
		///Otherwise, if false is returned, the file can be downloaded by anyone directly from our website.</summary>
		public bool IsFileChosenProtected() {
			string fileNameChosen=getFileChosenName();
			return _listFeeSchedFilesOntario.Contains(fileNameChosen) || _listFeeSchedFilesBCDA.Contains(fileNameChosen);
		}

		public string GetSelectedFeeDA() {
			if(_listFeeSchedFilesBCDA.Contains(getFileChosenName())) {
				return BRITISH_COLUMBIA_DENTAL_ASSOCIATION;
			}
			return ONTARIO_DENTAL_ASSOCIATION;
		}

		protected override string GetHelpOverride() {
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				return "FormFeeSchedPickRemoteCanada";
			}
			return "FormFeeSchedTools";
		}

		public FormFeeSchedPickRemote() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormFeeSchedPickRemote_Load(object sender,EventArgs e) {			
			Cursor=Cursors.WaitCursor;
			FillListFeeSchedFilesAll();
			Cursor=Cursors.Default;
		}

		private void FillListFeeSchedFilesAll() {
			FillListFeeSchedFilesOntario();
			FillListFeeSchedFilesBCDA();
			List<string> listAllFeeSchedFiles=new List<string>();
			listAllFeeSchedFiles.AddRange(_listFeeSchedFilesOntario);
			listAllFeeSchedFiles.AddRange(_listFeeSchedFilesBCDA);
			HttpWebRequest httpWebRequest=(HttpWebRequest)WebRequest.Create(Url);
			using(HttpWebResponse httpWebResponse=(HttpWebResponse)httpWebRequest.GetResponse()) 
			using(StreamReader streamReader=new StreamReader(httpWebResponse.GetResponseStream())) {
				string html=streamReader.ReadToEnd();
				int startIndex=html.IndexOf("<body>")+6;
				int bodyLength=html.Substring(startIndex).IndexOf("</body>");
				string fileListStr=html.Substring(startIndex,bodyLength).Trim();
				string[] stringArrayFiles=fileListStr.Split(new string[] { "\n","\r\n" },StringSplitOptions.RemoveEmptyEntries);
				for(int i=0;i<stringArrayFiles.Length;i++) {
					if(stringArrayFiles[i].ToLower().StartsWith("procedurecodes")) {
						continue;//Skip any files which contain procedure codes, because we only want to display fee files.
					}
					if(stringArrayFiles[i].ToUpper().StartsWith("QC_ACDQ_")) {
						try {
							long year=PIn.Long(stringArrayFiles[i].Substring(8,4));
							if(year<DateTime.Now.Year) {//Current year and next year will show up.
								continue;//Quebec fee schedules for previous years must be unavailable to the user.
							}
						}
						catch {
							continue;//Improperty formatted file name.
						}
					}
					listAllFeeSchedFiles.Add(stringArrayFiles[i]);
				}
			}
			listAllFeeSchedFiles.Sort();
			gridFeeSchedFiles.BeginUpdate();
			gridFeeSchedFiles.Columns.Clear();
			GridColumn col=new GridColumn("",35);
			gridFeeSchedFiles.Columns.Add(col);
			gridFeeSchedFiles.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<listAllFeeSchedFiles.Count;i++) {
				row=new GridRow();
				row.Cells.Add(listAllFeeSchedFiles[i]);
				gridFeeSchedFiles.ListGridRows.Add(row);
			}
			gridFeeSchedFiles.EndUpdate();
		}

		private void FillListFeeSchedFilesOntario() {
			_listFeeSchedFilesOntario=new List<string>();
			_listFeeSchedFilesOntario.Add("ON_ODA_2023_AN.txt");//Ontario Dental Association 2023 fee schedule for Anaesthesiologists.
			_listFeeSchedFilesOntario.Add("ON_ODA_2023_EN.txt");//Ontario Dental Association 2023 fee schedule for Endodontists.
			_listFeeSchedFilesOntario.Add("ON_ODA_2023_GP.txt");//Ontario Dental Association 2023 fee schedule for General Practitioners.
			_listFeeSchedFilesOntario.Add("ON_ODA_2023_OS.txt");//Ontario Dental Association 2023 fee schedule for Oral & Maxillofacial Surgeons.
			_listFeeSchedFilesOntario.Add("ON_ODA_2023_PA.txt");//Ontario Dental Association 2023 fee schedule for Paediatric Dentists.
			_listFeeSchedFilesOntario.Add("ON_ODA_2023_PE.txt");//Ontario Dental Association 2023 fee schedule for Periodontists.
			_listFeeSchedFilesOntario.Add("ON_ODA_2023_PR.txt");//Ontario Dental Association 2023 fee schedule for Prosthodontists.
			_listFeeSchedFilesOntario.Add("ON_ODA_2024_AN.txt");//Ontario Dental Association 2024 fee schedule for Anaesthesiologists.
			_listFeeSchedFilesOntario.Add("ON_ODA_2024_EN.txt");//Ontario Dental Association 2024 fee schedule for Endodontists.
			_listFeeSchedFilesOntario.Add("ON_ODA_2024_GP.txt");//Ontario Dental Association 2024 fee schedule for General Practitioners.
			_listFeeSchedFilesOntario.Add("ON_ODA_2024_OS.txt");//Ontario Dental Association 2024 fee schedule for Oral & Maxillofacial Surgeons.
			_listFeeSchedFilesOntario.Add("ON_ODA_2024_PA.txt");//Ontario Dental Association 2024 fee schedule for Paediatric Dentists.
			_listFeeSchedFilesOntario.Add("ON_ODA_2024_PE.txt");//Ontario Dental Association 2024 fee schedule for Periodontists.
			_listFeeSchedFilesOntario.Add("ON_ODA_2024_PR.txt");//Ontario Dental Association 2024 fee schedule for Prosthodontists.
		}

		private void FillListFeeSchedFilesBCDA() {
			_listFeeSchedFilesBCDA=new List<string>();
			_listFeeSchedFilesBCDA.Add("BC_BCDA_2022_GP.txt");//British Columbia Dental Association 2022 fee schedule for General Practitioners.
			_listFeeSchedFilesBCDA.Add("BC_BCDA_2022_LTC.txt");//British Columbia Dental Association 2022 fee schedule.
			_listFeeSchedFilesBCDA.Add("BC_BCDA_2022_PA.txt");//British Columbia Dental Association 2022 fee schedule for Paediatric Dentists.
			_listFeeSchedFilesBCDA.Add("BC_BCDA_2022_PE.txt");//British Columbia Dental Association 2022 fee schedule for Periodontists.
			_listFeeSchedFilesBCDA.Add("BC_BCDA_2023_GP.txt");//British Columbia Dental Association 2023 fee schedule for General Practitioners.
			_listFeeSchedFilesBCDA.Add("BC_BCDA_2023_LTC.txt");//British Columbia Dental Association 2023 fee schedule.
			_listFeeSchedFilesBCDA.Add("BC_BCDA_2023_PA.txt");//British Columbia Dental Association 2023 fee schedule for Paediatric Dentists.
			_listFeeSchedFilesBCDA.Add("BC_BCDA_2023_PE.txt");//British Columbia Dental Association 2023 fee schedule for Periodontists.
		}

		private void gridFeeSchedFiles_CellClick(object sender,ODGridClickEventArgs e) {
			butOK.Enabled=(gridFeeSchedFiles.SelectedIndices.Length>0);
		}

		private void gridFeeSchedFiles_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}

	}
}