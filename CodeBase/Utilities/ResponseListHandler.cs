using System.Collections.Generic;

namespace CodeBase.Utilities.DcvExtension {
	public class ResponseListHandler {
		private readonly List<string> _listResponses=new List<string>();
		private readonly object _listLock=new object();

		public void AddResponse(string response) {
			lock (_listLock) {
				_listResponses.Add(response);
			}
		}

		public string GetResponse(string requestId) {
			lock(_listLock) {
				string response=null;
				if (_listResponses.Count > 0) {
					int removeIdx=-1;
					for (int i=0;i<_listResponses.Count;i++) {
						if (_listResponses[i].Contains(requestId)) {
							int delimiterIndex=_listResponses[i].IndexOf("<FileIdentifier:");
							response=_listResponses[i].Substring(0, delimiterIndex);
							removeIdx=i;
							break;
						}
					}
					if(removeIdx>=0) {
						_listResponses.RemoveAt(removeIdx);
					}
				}
				return response;
			}
		}
	}
}