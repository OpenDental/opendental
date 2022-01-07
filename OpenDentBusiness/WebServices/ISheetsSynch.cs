using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	///<summary>This interface needs to have all web methods from SheetSynch that you want to use. This allows for testing code without having to
	///have an instance of the web service running.</summary>
	public interface ISheetsSynch {
		string SetPreferences(string officeData);
		string GetPreferences(string officeData);
		string GetWebFormSheets(string officeData);
		string DeleteSheetData(string officeData);
		string GetDentalOfficeID(string officeData);
		string GetSheetDefAddress(string officeData);
		string DownloadSheetDefs(string officeData);
		string DeleteSheetDef(string officeData);
		string UpLoadSheetDef(string officeData);
		string UpdateSheetDef(string officeData);
		string UploadSheetDefChunk(string officeData);
		string UploadSheetDefFromFile(string officeData);
		string UpdateSheetDefFromFile(string officeData);
		string CheckWebFormsDb();
	}
}
