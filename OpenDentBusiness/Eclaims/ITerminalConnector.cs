using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness.Eclaims {
	public interface ITerminalConnector {
		void ShowForm();
		void OpenConnection(int modemPort);
		void Dial(string v);
		string WaitFor(string v1,double v2);
		void Pause(double v);
		void ClearRxBuff();
		string Sout(string loginID,int v1,int v2);
		void Send(string submitterLogin);
		byte GetOneByte(double v);
		string Receive(double v);
		void DownloadXmodem(string v);
		void CloseConnection();
		string WaitFor(string v1,string v2,double v3);
		void UploadXmodem(string v);
	}
}
