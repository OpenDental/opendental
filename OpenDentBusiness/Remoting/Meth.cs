using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using CodeBase;

namespace OpenDentBusiness {
	///<summary>Short for Method.  Calls a method remotely.  ONLY used if ClientWeb.  This must be tested at top of the method in question.  These will be used extensively since ALL data interface classes will need this.  This completely avoids sending queries directly to the server.  Must pass all the parameters from the original method.</summary>
	public class Meth {
		///<summary></summary>
		public static DataTable GetTable(MethodBase methodBase,params object[] parameters) {
			if(RemotingClient.RemotingRole!=RemotingRole.ClientWeb) {
				throw new ApplicationException("Meth.GetTable may only be used when RemotingRole is ClientWeb.");
			}
			if(ODBuild.IsDebug()) {
				//Verify that it returns a DataTable
				MethodInfo methodInfo=null;
				try {
					methodInfo=methodBase.ReflectedType.GetMethod(methodBase.Name);
				}
				catch(AmbiguousMatchException) {
					//Ambiguous match exceptions do not matter for the middle tier and are just annoying when they get thrown here.  Ignore them.
				}
				if(methodInfo!=null && methodInfo.ReturnType != typeof(DataTable)) {
					throw new ApplicationException("Meth.GetTable calling class must return DataTable.");
				}
			}
			DtoGetTable dto=new DtoGetTable();
			dto.MethodName=methodBase.DeclaringType.Namespace+"."
				+methodBase.DeclaringType.Name+"."+methodBase.Name;
			dto.Params=DtoObject.ConstructArray(methodBase,parameters);
			dto.Credentials=new Credentials();
			dto.Credentials.Username=Security.CurUser.UserName;
			dto.Credentials.Password=Security.PasswordTyped;//.CurUser.Password;
			dto.ComputerName=Security.CurComputerName;
			DataTable retval=new DataTable();
			try {
				retval=RemotingClient.ProcessGetTable(dto);
			}
			catch(ODException ex) {
				if(ex.ErrorCode==(int)ODException.ErrorCodes.CheckUserAndPasswordFailed) {
					CredentialsFailed();//The application pauses here in the main thread to wait for user input.
					retval=GetTable(methodBase,parameters);
				}
				else {
					throw;
				}
			}
			return retval;
		}

		///<summary>Uses lower sql permissions, making it safe to pass a query.</summary>
		public static DataTable GetTableLow(string command) {
			if(RemotingClient.RemotingRole!=RemotingRole.ClientWeb) {
				throw new ApplicationException("Meth.GetTableLow may only be used when RemotingRole is ClientWeb.");
			}
			DtoGetTableLow dto=new DtoGetTableLow();
			MethodInfo methodInfo=typeof(Reports).GetMethod("GetTable");
			dto.MethodName=methodInfo.DeclaringType.Namespace+"."+methodInfo.DeclaringType.Name+"."+methodInfo.Name;
			DtoObject dtoObj=new DtoObject(command,typeof(string));
			dto.Params=new DtoObject[] { dtoObj };
			dto.Credentials=new Credentials();
			dto.Credentials.Username=Security.CurUser.UserName;
			dto.Credentials.Password=Security.PasswordTyped;//.CurUser.Password;
			dto.ComputerName=Security.CurComputerName;
			DataTable retval=new DataTable();
			try {
				retval=RemotingClient.ProcessGetTableLow(dto);
			}
			catch(ODException ex) {
				if(ex.ErrorCode==(int)ODException.ErrorCodes.CheckUserAndPasswordFailed) {
					CredentialsFailed();//The application pauses here in the main thread to wait for user input.
					retval=GetTableLow(command);
				}
				else {
					throw;
				}
			}
			return retval;
		}

		///<summary></summary>
		public static DataSet GetDS(MethodBase methodBase,params object[] parameters) {
			if(RemotingClient.RemotingRole!=RemotingRole.ClientWeb) {
				throw new ApplicationException("Meth.GetDS may only be used when RemotingRole is ClientWeb.");
			}
			if(ODBuild.IsDebug()) {
				//Verify that it returns a DataSet
				MethodInfo methodInfo=null;
				try {
					methodInfo=methodBase.ReflectedType.GetMethod(methodBase.Name);
				}
				catch(AmbiguousMatchException) {
					//Ambiguous match exceptions do not matter for the middle tier and are just annoying when they get thrown here.  Ignore them.
				}
				if(methodInfo!=null && methodInfo.ReturnType != typeof(DataSet)) {
					throw new ApplicationException("Meth.GetDS calling class must return DataSet.");
				}
			}
			DtoGetDS dto=new DtoGetDS();
			dto.MethodName=methodBase.DeclaringType.Namespace+"."
				+methodBase.DeclaringType.Name+"."+methodBase.Name;
			dto.Params=DtoObject.ConstructArray(methodBase,parameters);
			dto.Credentials=new Credentials();
			dto.Credentials.Username=Security.CurUser.UserName;
			dto.Credentials.Password=Security.PasswordTyped;//.CurUser.Password;
			dto.ComputerName=Security.CurComputerName;
			DataSet retval=new DataSet();
			try {
				retval=RemotingClient.ProcessGetDS(dto);
			}
			catch(ODException ex) {
				if(ex.ErrorCode==(int)ODException.ErrorCodes.CheckUserAndPasswordFailed) {
					CredentialsFailed();//The application pauses here in the main thread to wait for user input.
					retval=GetDS(methodBase,parameters);
				}
				else {
					throw;
				}
			}
			return retval;
		}

		///<summary></summary>
		public static SerializableDictionary<K,V> GetSerializableDictionary<K,V>(MethodBase methodBase,params object[] parameters) {
			if(RemotingClient.RemotingRole!=RemotingRole.ClientWeb) {
				throw new ApplicationException("Meth.GetSerializableDictionary may only be used when RemotingRole is ClientWeb.");
			}
			if(ODBuild.IsDebug()) {
				//Verify that it returns a DataTable
				MethodInfo methodInfo=null;
				try {
					methodInfo=methodBase.ReflectedType.GetMethod(methodBase.Name);
				}
				catch(AmbiguousMatchException) {
					//Ambiguous match exceptions do not matter for the middle tier and are just annoying when they get thrown here.  Ignore them.
				}
				if(methodInfo!=null && methodInfo.ReturnType!=typeof(SerializableDictionary<K,V>)) {
					throw new ApplicationException("Meth.GetSerializableDictionary calling class must return SerializableDictionary.");
				}
			}
			DtoGetSerializableDictionary dto=new DtoGetSerializableDictionary();
			dto.MethodName=methodBase.DeclaringType.Namespace+"."
				+methodBase.DeclaringType.Name+"."+methodBase.Name;
			dto.Params=DtoObject.ConstructArray(methodBase,parameters);
			dto.Credentials=new Credentials();
			dto.Credentials.Username=Security.CurUser.UserName;
			dto.Credentials.Password=Security.PasswordTyped;//.CurUser.Password;
			dto.ComputerName=Security.CurComputerName;
			SerializableDictionary<K,V> retval=new SerializableDictionary<K,V>();
			try {
				retval=RemotingClient.ProcessGetSerializableDictionary<K,V>(dto);
			}
			catch(ODException ex) {
				if(ex.ErrorCode==(int)ODException.ErrorCodes.CheckUserAndPasswordFailed) {
					CredentialsFailed();//The application pauses here in the main thread to wait for user input.
					retval=GetSerializableDictionary<K,V>(methodBase,parameters);
				}
				else {
					throw;
				}
			}
			return retval;
		}

		///<summary></summary>
		public static long GetLong(MethodBase methodBase,params object[] parameters) {
			if(RemotingClient.RemotingRole!=RemotingRole.ClientWeb) {
				throw new ApplicationException("Meth.GetLong may only be used when RemotingRole is ClientWeb.");
			}
			if(ODBuild.IsDebug()) {
				//Verify that it returns an int
				MethodInfo methodInfo=null;
				try {
					methodInfo=methodBase.ReflectedType.GetMethod(methodBase.Name);
				}
				catch(AmbiguousMatchException) {
					//Ambiguous match exceptions do not matter for the middle tier and are just annoying when they get thrown here.  Ignore them.
				}
				if(methodInfo!=null && (methodInfo.ReturnType != typeof(long))) {
					throw new ApplicationException("Meth.GetLong calling class must return long.");
				}
			}
			DtoGetLong dto=new DtoGetLong();
			dto.MethodName=methodBase.DeclaringType.Namespace+"."
				+methodBase.DeclaringType.Name+"."+methodBase.Name;
			dto.Params=DtoObject.ConstructArray(methodBase,parameters);
			dto.Credentials=new Credentials();
			dto.Credentials.Username=Security.CurUser.UserName;
			dto.Credentials.Password=Security.PasswordTyped;//.CurUser.Password;
			dto.ComputerName=Security.CurComputerName;
			long retval=0;
			try {
				retval=RemotingClient.ProcessGetLong(dto);
			}
			catch(ODException ex) {
				if(ex.ErrorCode==(int)ODException.ErrorCodes.CheckUserAndPasswordFailed) {
					CredentialsFailed();//The application pauses here in the main thread to wait for user input.
					retval=GetLong(methodBase,parameters);
				}
				else {
					throw;
				}
			}
			return retval;
		}

		///<summary></summary>
		public static int GetInt(MethodBase methodBase,params object[] parameters) {
			if(RemotingClient.RemotingRole!=RemotingRole.ClientWeb) {
				throw new ApplicationException("Meth.GetInt may only be used when RemotingRole is ClientWeb.");
			}
			if(ODBuild.IsDebug()) {
				//Verify that it returns an int
				MethodInfo methodInfo=null;
				try {
					methodInfo=methodBase.ReflectedType.GetMethod(methodBase.Name);
				}
				catch(AmbiguousMatchException) {
					//Ambiguous match exceptions do not matter for the middle tier and are just annoying when they get thrown here.  Ignore them.
				}
				if(methodInfo!=null && methodInfo.ReturnType != typeof(int)) {
					throw new ApplicationException("Meth.GetInt calling class must return int.");
				}
			}
			DtoGetInt dto=new DtoGetInt();
			dto.MethodName=methodBase.DeclaringType.Namespace+"."
				+methodBase.DeclaringType.Name+"."+methodBase.Name;
			dto.Params=DtoObject.ConstructArray(methodBase,parameters);
			dto.Credentials=new Credentials();
			dto.Credentials.Username=Security.CurUser.UserName;
			dto.Credentials.Password=Security.PasswordTyped;//.CurUser.Password;
			dto.ComputerName=Security.CurComputerName;
			int retval=0;
			try {
				retval=RemotingClient.ProcessGetInt(dto);
			}
			catch(ODException ex) {
				if(ex.ErrorCode==(int)ODException.ErrorCodes.CheckUserAndPasswordFailed) {
					CredentialsFailed();//The application pauses here in the main thread to wait for user input.
					retval=GetInt(methodBase,parameters);
				}
				else {
					throw;
				}
			}
			return retval;
		}

		///<summary></summary>
		public static double GetDouble(MethodBase methodBase,params object[] parameters) {
			if(RemotingClient.RemotingRole!=RemotingRole.ClientWeb) {
				throw new ApplicationException("Meth.GetDouble may only be used when RemotingRole is ClientWeb.");
			}
			if(ODBuild.IsDebug()) {
				//Verify that it returns a double
				MethodInfo methodInfo=null;
				try {
					methodInfo=methodBase.ReflectedType.GetMethod(methodBase.Name);
				}
				catch(AmbiguousMatchException) {
					//Ambiguous match exceptions do not matter for the middle tier and are just annoying when they get thrown here.  Ignore them.
				}
				if(methodInfo!=null && methodInfo.ReturnType != typeof(double)) {
					throw new ApplicationException("Meth.GetDouble calling class must return double.");
				}
			}
			DtoGetDouble dto=new DtoGetDouble();
			dto.MethodName=methodBase.DeclaringType.Namespace+"."
				+methodBase.DeclaringType.Name+"."+methodBase.Name;
			dto.Params=DtoObject.ConstructArray(methodBase,parameters);
			dto.Credentials=new Credentials();
			dto.Credentials.Username=Security.CurUser.UserName;
			dto.Credentials.Password=Security.PasswordTyped;//.CurUser.Password;
			dto.ComputerName=Security.CurComputerName;
			double retval=0;
			try {
				retval=RemotingClient.ProcessGetDouble(dto);
			}
			catch(ODException ex) {
				if(ex.ErrorCode==(int)ODException.ErrorCodes.CheckUserAndPasswordFailed) {
					CredentialsFailed();//The application pauses here in the main thread to wait for user input.
					retval=GetDouble(methodBase,parameters);
				}
				else {
					throw;
				}
			}
			return retval;
		}

		///<summary></summary>
		public static void GetVoid(MethodBase methodBase,params object[] parameters) {
			if(RemotingClient.RemotingRole!=RemotingRole.ClientWeb) {
				throw new ApplicationException("Meth.GetVoid may only be used when RemotingRole is ClientWeb.");
			}
			if(ODBuild.IsDebug()) {
				//Verify that it returns void
				MethodInfo methodInfo=null;
				try {
					methodInfo=methodBase.ReflectedType.GetMethod(methodBase.Name);
				}
				catch(AmbiguousMatchException) {
					//Ambiguous match exceptions do not matter for the middle tier and are just annoying when they get thrown here.  Ignore them.
				}
				if(methodInfo!=null && methodInfo.ReturnType != typeof(void)) {
					throw new ApplicationException("Meth.GetVoid calling class must return void.");
				}
			}
			DtoGetVoid dto=new DtoGetVoid();
			dto.MethodName=methodBase.DeclaringType.Namespace+"."
				+methodBase.DeclaringType.Name+"."+methodBase.Name;
			dto.Params=DtoObject.ConstructArray(methodBase,parameters);
			dto.Credentials=new Credentials();
			dto.Credentials.Username=Security.CurUser.UserName;
			dto.Credentials.Password=Security.PasswordTyped;//.CurUser.Password;
			dto.ComputerName=Security.CurComputerName;
			try {
				RemotingClient.ProcessGetVoid(dto);
			}
			catch(ODException ex) {
				if(ex.ErrorCode==(int)ODException.ErrorCodes.CheckUserAndPasswordFailed) {
					CredentialsFailed();//The application pauses here in the main thread to wait for user input.
					GetVoid(methodBase,parameters);
				}
				else {
					throw;
				}
			}
		}

		///<summary></summary>
		public static T GetObject<T>(MethodBase methodBase,params object[] parameters) {
			if(RemotingClient.RemotingRole!=RemotingRole.ClientWeb) {
				throw new ApplicationException("Meth.GetObject may only be used when RemotingRole is ClientWeb.");
			}
			//can't verify return type
			DtoGetObject dto=new DtoGetObject();
			if(typeof(T).IsGenericType) {
				Type listType=typeof(T).GetGenericArguments()[0];
				dto.ObjectType="List<"+listType.FullName+">";
			}
			else {
				dto.ObjectType=typeof(T).FullName;
			}
			dto.MethodName=methodBase.DeclaringType.Namespace+"."
				+methodBase.DeclaringType.Name+"."+methodBase.Name;
			dto.Params=DtoObject.ConstructArray(methodBase,parameters);
			dto.Credentials=new Credentials();
			dto.Credentials.Username=Security.CurUser.UserName;
			dto.Credentials.Password=Security.PasswordTyped;//.CurUser.Password;
			dto.ComputerName=Security.CurComputerName;
			T retval=default(T);
			try {
				retval=RemotingClient.ProcessGetObject<T>(dto);
			}
			catch(ODException ex) {
				if(ex.ErrorCode==(int)ODException.ErrorCodes.CheckUserAndPasswordFailed) {
					if(RemotingClient.HasLoginFailed) {//login has already failed and we got another CheckUserAndPasswordFailed error, just throw
						throw;
					}
					CredentialsFailed();
					retval=GetObject<T>(methodBase,parameters);
				}
				else {
					throw;
				}
			}
			return retval;
		}

		///<summary></summary>
		public static string GetString(MethodBase methodBase,params object[] parameters) {
			if(RemotingClient.RemotingRole!=RemotingRole.ClientWeb) {
				throw new ApplicationException("Meth.GetString may only be used when RemotingRole is ClientWeb.");
			}
			if(ODBuild.IsDebug()) {
				//Verify that it returns string
				MethodInfo methodInfo=null;
				try {
					methodInfo=methodBase.ReflectedType.GetMethod(methodBase.Name);
				}
				catch(AmbiguousMatchException) {
					//Ambiguous match exceptions do not matter for the middle tier and are just annoying when they get thrown here.  Ignore them.
				}
				if(methodInfo!=null && methodInfo.ReturnType != typeof(string)) {
					throw new ApplicationException("Meth.GetString calling class must return string.");
				}
			}
			DtoGetString dto=new DtoGetString();
			dto.MethodName=methodBase.DeclaringType.Namespace+"."
				+methodBase.DeclaringType.Name+"."+methodBase.Name;
			dto.Params=DtoObject.ConstructArray(methodBase,parameters);
			dto.Credentials=new Credentials();
			dto.Credentials.Username=Security.CurUser.UserName;
			dto.Credentials.Password=Security.PasswordTyped;//.CurUser.Password;
			dto.ComputerName=Security.CurComputerName;
			string retval=null;
			try {
				retval=RemotingClient.ProcessGetString(dto);
			}
			catch(ODException ex) {
				if(ex.ErrorCode==(int)ODException.ErrorCodes.CheckUserAndPasswordFailed) {
					CredentialsFailed();//The application pauses here in the main thread to wait for user input.
					retval=GetString(methodBase,parameters);
				}
				else {
					throw;
				}
			}
			return retval;
		}

		///<summary></summary>
		public static bool GetBool(MethodBase methodBase,params object[] parameters) {
			if(RemotingClient.RemotingRole!=RemotingRole.ClientWeb) {
				throw new ApplicationException("Meth.GetBool may only be used when RemotingRole is ClientWeb.");
			}
			DtoGetBool dto=new DtoGetBool();
			dto.MethodName=methodBase.DeclaringType.Namespace+"."
				+methodBase.DeclaringType.Name+"."+methodBase.Name;
			dto.Params=DtoObject.ConstructArray(methodBase,parameters);
			dto.Credentials=new Credentials();
			dto.Credentials.Username=Security.CurUser.UserName;
			dto.Credentials.Password=Security.PasswordTyped;//.CurUser.Password;
			dto.ComputerName=Security.CurComputerName;
			bool retval;
			try {
				retval=RemotingClient.ProcessGetBool(dto);
			}
			catch(ODException ex) {
				if(ex.ErrorCode==(int)ODException.ErrorCodes.CheckUserAndPasswordFailed) {
					CredentialsFailed();//The application pauses here in the main thread to wait for user input.
					retval=GetBool(methodBase,parameters);
				}
				else {
					throw;
				}
			}
			return retval;
		}

		public static bool GetIsMiddleTierAvailable(MethodBase methodBase,params object[] parameters) {
			if(RemotingClient.RemotingRole!=RemotingRole.ClientWeb) {
				throw new ApplicationException("Meth.GetIsMiddleTierAvailable may only be used when RemotingRole is ClientWeb.");
			}
			try {
				DtoGetBool dto=new DtoGetBool();
				dto.MethodName=methodBase.DeclaringType.Namespace+"."
					+methodBase.DeclaringType.Name+"."+methodBase.Name;
				dto.Params=DtoObject.ConstructArray(methodBase,parameters);
				dto.Credentials=new Credentials();
				dto.Credentials.Username=Security.CurUser.UserName;
				dto.Credentials.Password=Security.PasswordTyped;//.CurUser.Password;
				//Passing hasLostConnection=false allows this method to throw an exception, such as if the Middle Tier connection has been lost.
				return RemotingClient.ProcessGetBool(dto,false);
			}
			catch(WebException wex) {
				//If no connection monitoring desired or this is a WebException that we aren't explicitly looking for then bubble up the exception.
				//WebException class: https://docs.microsoft.com/en-us/dotnet/api/system.net.webexception?view=netframework-4.7.2
				//WebException.Status property: https://docs.microsoft.com/en-us/dotnet/api/system.net.webexception.status?view=netframework-4.7.2
				//Handling WebExceptions: https://docs.microsoft.com/en-us/dotnet/framework/network-programming/handling-errors?view=netframework-4.7.2
				if(wex.Status!=WebExceptionStatus.ConnectFailure) {
					throw;
				}
				return false;//Indicates to DtoProcessor.IsMiddleTierAvailable() that the Middle Tier connection has not been restored.
			}
		}

		///<summary>Fires a CredentialsFailedAfterLoginEvent to notify the main thread that the user needs to log in again.
		///This method will then force any threads that called it to wait here indefinitely or until the user has logged in successfully.</summary>
		private static void CredentialsFailed() {
			CredentialsFailedAfterLoginEvent.Fire(ODEventType.ServiceCredentials,
				"Invalid username or password.  You must login again or exit the program.");
			while(RemotingClient.HasLoginFailed) {
				Thread.Sleep(100);//wait for flag to be cleared before returning and allowing the method to continue
			}
		}


	}
}
