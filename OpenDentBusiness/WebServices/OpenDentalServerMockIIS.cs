using CodeBase;
using DataConnectionBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness.WebServices {
	public class OpenDentalServerMockIIS : IOpenDentalServer {
		private string _server="";
		private string _db="";
		private string _user="";
		private string _password="";
		private string _userLow="";
		private string _passLow="";
		private DatabaseType _dbType=DatabaseType.MySql;

		///<summary>Optionally pass in database connection settings to override using the parent thread database context.</summary>
		public OpenDentalServerMockIIS(string server="",string db="",string user="",string password="",string userLow="",string passLow=""
			,DatabaseType dbType=DatabaseType.MySql)
		{
			_server=server;
			_db=db;
			_user=user;
			_password=password;
			_userLow=userLow;
			_passLow=passLow;
			_dbType=dbType;
		}

		public string ProcessRequest(string dtoString) {
			return RunWebMethod(() => DtoProcessor.ProcessDto(dtoString));
		}

		///<summary></summary>
		private string RunWebMethod(Func<string> funcWebMethod) {
			Exception ex=null;
			RemotingRole remotingRoleCur=RemotingClient.RemotingRole;
			//Create an ODThread so that we can safely change the database connection settings without affecting the calling method's connection.
			ODThread odThread=new ODThread(new ODThread.WorkerDelegate((ODThread o) => {
				//Change the remoting role to ServerWeb (this will get set back to remotingRoleCur later.
				RemotingClient.RemotingRole=RemotingRole.ServerWeb;
				//Set new database connection settings if designated.  E.g. some unit tests need to set and utilize userLow and passLow.
				if(!string.IsNullOrEmpty(_server)
					&& !string.IsNullOrEmpty(_db)
					&& !string.IsNullOrEmpty(_user))
				{
					new DataConnection().SetDbT(_server,_db,_user,_password,_userLow,_passLow,DatabaseType.MySql);
				}
				//Execute the func that was passed in now that our remoting role and database connection are correct.
				o.Tag=funcWebMethod();
			}));
			odThread.AddExceptionHandler(new ODThread.ExceptionDelegate((Exception e) => {
				ex=e;//Cannot throw exception here due to still being in a threaded context.
			}));
			if(ex!=null) {
				throw ex;//Should never happen because this would be a "web exception" as in the ProcessRequest could not even invoke or something like that.
			}
			odThread.Name="threadMiddleTierUnitTestRunWebMethod";
			//Set the remoting role back after exiting the thread.
			odThread.AddExitHandler((e) => RemotingClient.RemotingRole=remotingRoleCur);
			odThread.Start(true);
			odThread.Join(System.Threading.Timeout.Infinite);
			return (string)odThread.Tag;
		}

	}
}
