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
		private List<string> ListFeeSchedFilesOntario;
		private List<string> _listFeeSchedFilesBCDA;
		private const string ONTARIO_DENTAL_ASSOCIATION="ODA";
		private const string BRITISH_COLUMBIA_DENTAL_ASSOCIATION="BCDA";

		///<summary>Returns the full URL of the chosen file.</summary>
		public string FileChosenUrl {
			get {				
				return Url+FileChosenName;
			}
		}

		///<summary>Returns just the file name of the chosen file.</summary>
		public string FileChosenName {
			get {
				if(gridFeeSchedFiles.GetSelectedIndex()==-1) {
					return "";
				}
				return gridFeeSchedFiles.ListGridRows[gridFeeSchedFiles.GetSelectedIndex()].Cells[0].Text;
			}
		}

		public string GetSelectedFeeDA() {
			if(_listFeeSchedFilesBCDA.Contains(FileChosenName)) {
				return BRITISH_COLUMBIA_DENTAL_ASSOCIATION;
			}
			return ONTARIO_DENTAL_ASSOCIATION;
		}

		///<summary>Returns true if the file chosen must be downloaded from the web service. Currently, Ontario fees must be downloaded from the web service.
		///Otherwise, if false is returned, the file can be downloaded by anyone directly from our website.</summary>
		public bool IsFileChosenProtected {
			get {
				return ListFeeSchedFilesOntario.Contains(FileChosenName) || _listFeeSchedFilesBCDA.Contains(FileChosenName);
			}
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
			List<string> ListFeeSchedFilesAll=new List<string>();
			ListFeeSchedFilesAll.AddRange(ListFeeSchedFilesOntario);
			ListFeeSchedFilesAll.AddRange(_listFeeSchedFilesBCDA);
			HttpWebRequest request=(HttpWebRequest)WebRequest.Create(Url);
			using(HttpWebResponse response=(HttpWebResponse)request.GetResponse()) {
				using(StreamReader reader=new StreamReader(response.GetResponseStream())) {
					string html=reader.ReadToEnd();
					int startIndex=html.IndexOf("<body>")+6;
					int bodyLength=html.Substring(startIndex).IndexOf("</body>");
					string fileListStr=html.Substring(startIndex,bodyLength).Trim();
					string[] files=fileListStr.Split(new string[] { "\n","\r\n" },StringSplitOptions.RemoveEmptyEntries);
					for(int i=0;i<files.Length;i++) {
						if(files[i].ToLower().StartsWith("procedurecodes")) {
							continue;//Skip any files which contain procedure codes, because we only want to display fee files.
						}
						if(files[i].ToUpper().StartsWith("QC_ACDQ_")) {
							try {
								long year=PIn.Long(files[i].Substring(8,4));
								if(year<DateTime.Now.Year) {//Current year and next year will show up.
									continue;//Quebec fee schedules for previous years must be unavailable to the user.
								}
							}
							catch {
								continue;//Improperty formatted file name.
							}
						}
						ListFeeSchedFilesAll.Add(files[i]);
					}
				}
			}
			ListFeeSchedFilesAll.Sort();
			gridFeeSchedFiles.BeginUpdate();
			gridFeeSchedFiles.Columns.Clear();
			GridColumn col=new GridColumn("",35);
			gridFeeSchedFiles.Columns.Add(col);
			gridFeeSchedFiles.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<ListFeeSchedFilesAll.Count;i++) {
				row=new GridRow();
				row.Cells.Add(ListFeeSchedFilesAll[i]);
				gridFeeSchedFiles.ListGridRows.Add(row);
			}
			gridFeeSchedFiles.EndUpdate();
		}

		private void FillListFeeSchedFilesOntario() {
			ListFeeSchedFilesOntario=new List<string>();
			ListFeeSchedFilesOntario.Add("ON_ODA_2021_GP.txt");//Ontario Dental Association 2021 fee schedule for General Practitioners.
			ListFeeSchedFilesOntario.Add("ON_ODA_2021_AN.txt");//Ontario Dental Association 2021 fee schedule for Anaesthesiologists.
			ListFeeSchedFilesOntario.Add("ON_ODA_2021_EN.txt");//Ontario Dental Association 2021 fee schedule for Endodontists.
			ListFeeSchedFilesOntario.Add("ON_ODA_2021_OS.txt");//Ontario Dental Association 2021 fee schedule for Oral & Maxillofacial Surgeons.
			ListFeeSchedFilesOntario.Add("ON_ODA_2021_PA.txt");//Ontario Dental Association 2021 fee schedule for Paediatric Dentists.
			ListFeeSchedFilesOntario.Add("ON_ODA_2021_PE.txt");//Ontario Dental Association 2021 fee schedule for Periodontists.
			ListFeeSchedFilesOntario.Add("ON_ODA_2021_PR.txt");//Ontario Dental Association 2021 fee schedule for Prosthodontists.
			ListFeeSchedFilesOntario.Add("ON_ODA_2022_AN.txt");//Ontario Dental Association 2022 fee schedule for Anaesthesiologists.
			ListFeeSchedFilesOntario.Add("ON_ODA_2022_EN.txt");//Ontario Dental Association 2022 fee schedule for Endodontists.
			ListFeeSchedFilesOntario.Add("ON_ODA_2022_GP.txt");//Ontario Dental Association 2022 fee schedule for General Practitioners.
			ListFeeSchedFilesOntario.Add("ON_ODA_2022_OS.txt");//Ontario Dental Association 2022 fee schedule for Oral & Maxillofacial Surgeons.
			ListFeeSchedFilesOntario.Add("ON_ODA_2022_PA.txt");//Ontario Dental Association 2022 fee schedule for Paediatric Dentists.
			ListFeeSchedFilesOntario.Add("ON_ODA_2022_PE.txt");//Ontario Dental Association 2022 fee schedule for Periodontists.
			ListFeeSchedFilesOntario.Add("ON_ODA_2022_PR.txt");//Ontario Dental Association 2022 fee schedule for Prosthodontists.
		}

		private void FillListFeeSchedFilesBCDA() {
			_listFeeSchedFilesBCDA=new List<string>();
			_listFeeSchedFilesBCDA.Add("BC_BCDA_2022_GP.txt");//British Columbia Dental Association 2022 fee schedule for General Practitioners.
			_listFeeSchedFilesBCDA.Add("BC_BCDA_2022_LTC.txt");//British Columbia Dental Association 2022 fee schedule.
			_listFeeSchedFilesBCDA.Add("BC_BCDA_2022_PA.txt");//British Columbia Dental Association 2022 fee schedule for Paediatric Dentists.
			_listFeeSchedFilesBCDA.Add("BC_BCDA_2022_PE.txt");//British Columbia Dental Association 2022 fee schedule for Periodontists.
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

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}