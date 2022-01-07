using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using CodeBase;
using DataConnectionBase;

namespace OpenDentBusiness {
	public class DtoProcessor {
		
		///<summary>Used to indicate whether the middle tier process has initialized.</summary>
		private static bool _isMiddleTierInitialized=false;

		///<summary>Pass in a serialized dto.  It returns a dto which must be deserialized by the client.
		///Set serverMapPath to the root directory of the OpenDentalServerConfig.xml.  Typically Server.MapPath(".") from a web service.
		///Optional parameter because it is not necessary for Unit Tests (mock server).</summary>
		public static string ProcessDto(string dtoString,string serverMapPath="") {
			try {
				#region Normalize DateTime
				if(!_isMiddleTierInitialized) {
					//If this fails, the exception will throw and be serialized and sent to the client.
					ODInitialize.Initialize();
					//Because Security._curUserT is a thread static field, we need to make sure that any new threads that are spawned have that field set.
					ODThread.AddInitializeHandler<Userod>(() => Security.CurUser.Copy(),user => Security.CurUser=user);
					//Same thing for PasswordTyped.
					ODThread.AddInitializeHandler<string>(() => Security.PasswordTyped,password => Security.PasswordTyped=password);
					//Ditto for CurComputerName
					ODThread.AddInitializeHandler<string>(() => Security.CurComputerName,computerName => Security.CurComputerName=computerName);
					//Calling CDT.Class1.Decrypt will cause CDT to verify the assembly and then save the encryption key. This needs to be done here because
					//if we later call CDT.Class1.Decrypt inside a thread, we won't we able to find OpenDentalServer.dll in the call stack.
					ODException.SwallowAnyException(() => CDT.Class1.Decrypt("odv2e$fakeciphertext",out _));
					_isMiddleTierInitialized=true;
				}
				#endregion
				#region Initialize Database Connection
				//Always attempt to set the database connection settings from the config file if they haven't been set yet.
				//We use to ONLY load in database settings when Security.LogInWeb was called but that is not good enough now that we have more services.
				//E.g. We do not want to manually call "Security.LogInWeb" from the CEMT when all we want is a single preference value.
				if(string.IsNullOrEmpty(DataConnection.GetServerName()) && string.IsNullOrEmpty(DataConnection.GetConnectionString())) {
					RemotingClient.RemotingRole=RemotingRole.ServerWeb;
					//the application virtual path is usually /OpenDentalServer, but may be different if hosting multiple databases on one IIS server
					string configFilePath="";
					if(!string.IsNullOrWhiteSpace(HostingEnvironment.ApplicationVirtualPath) && HostingEnvironment.ApplicationVirtualPath.Length>1) {
						//There can be multiple config files within a physical path that is shared by multiple IIS ASP.NET applications.
						//In order for the same physical path to host multiple applications, they each need a unique config file for db connection settings.
						//Each application will have a unique ApplicationVirtualPath which we will use to identify the corresponding config.xml.
						configFilePath=ODFileUtils.CombinePaths(serverMapPath,HostingEnvironment.ApplicationVirtualPath.Trim('/')+"Config.xml");
					}
					if(string.IsNullOrWhiteSpace(configFilePath)
						|| !File.Exists(configFilePath))//returns false if the file doesn't exist, user doesn't have permission for file, path is blank or null
					{
						//either configFilePath not set or file doesn't exist, default to OpenDentalServerConfig.xml
						configFilePath=ODFileUtils.CombinePaths(serverMapPath,"OpenDentalServerConfig.xml");
					}
					Userods.LoadDatabaseInfoFromFile(configFilePath);
				}
				#endregion
				DataTransferObject dto=DataTransferObject.Deserialize(dtoString);
				DtoInformation dtoInformation=new DtoInformation(dto);
				if(dtoInformation.FullNameComponents.Length==3 && dtoInformation.FullNameComponents[2].ToLower()=="hashpassword") {
					return dtoInformation.GetHashPassword();
				}
				//Set Security.CurUser so that queries can be run against the db as if it were this user.
				Security.CurUser=Userods.CheckUserAndPassword(dto.Credentials.Username,dto.Credentials.Password
					,Programs.UsingEcwTightOrFullMode());
				Security.PasswordTyped=dto.Credentials.Password;
				//Set the computer name so securitylog entries use the client's computer name instead of the middle tier server name
				//Older clients might not include ComputerName in the dto, so we need to make sure it's not null.
				Security.CurComputerName=dto.ComputerName??"";
				Type type=dto.GetType();
				#region DtoGetTable
				if(type == typeof(DtoGetTable)) {
					DataTable dt=(DataTable)dtoInformation.MethodInfo.Invoke(null,dtoInformation.ParamObjs);
					return XmlConverter.TableToXml(dt);
				}
				#endregion
				#region DtoGetTableLow
				else if(type == typeof(DtoGetTableLow)) {
					DataTable dt=Reports.GetTable((string)dtoInformation.ParamObjs[0]);
					return XmlConverter.TableToXml(dt);
				}
				#endregion
				#region DtoGetDS
				else if(type == typeof(DtoGetDS)) {
					DataSet ds=(DataSet)dtoInformation.MethodInfo.Invoke(null,dtoInformation.ParamObjs);
					return XmlConverter.DsToXml(ds);
				}
				#endregion
				#region DtoGetSerializableDictionary
				else if(type == typeof(DtoGetSerializableDictionary)) {
					Object objResult=dtoInformation.MethodInfo.Invoke(null,dtoInformation.ParamObjs);
					Type returnType=dtoInformation.MethodInfo.ReturnType;
					return XmlConverterSerializer.Serialize(returnType,objResult);
				}
				#endregion
				#region DtoGetLong
				else if(type == typeof(DtoGetLong)) {
					long longResult=(long)dtoInformation.MethodInfo.Invoke(null,dtoInformation.ParamObjs);
					return longResult.ToString();
				}
				#endregion
				#region DtoGetInt
				else if(type == typeof(DtoGetInt)) {
					int intResult=(int)dtoInformation.MethodInfo.Invoke(null,dtoInformation.ParamObjs);
					return intResult.ToString();
				}
				#endregion
				#region DtoGetDouble
				else if(type == typeof(DtoGetDouble)) {
					double doubleResult=(double)dtoInformation.MethodInfo.Invoke(null,dtoInformation.ParamObjs);
					return doubleResult.ToString();
				}
				#endregion
				#region DtoGetVoid
				else if(type == typeof(DtoGetVoid)) {
					dtoInformation.MethodInfo.Invoke(null,dtoInformation.ParamObjs);
					return "0";
				}
				#endregion
				#region DtoGetObject
				else if(type == typeof(DtoGetObject)) {
					if(dtoInformation.ClassName=="Security" && dtoInformation.MethodName=="LogInWeb") {
						dtoInformation.Parameters[2]=new DtoObject(serverMapPath,typeof(string));//because we can't access this variable from within OpenDentBusiness.
						RemotingClient.RemotingRole=RemotingRole.ServerWeb;
						//We just changed the number of parameters so we need to regenerate ParamObjs.
						dtoInformation.ParamObjs=DtoObject.GenerateObjects(dtoInformation.Parameters);
					}
					Object objResult=dtoInformation.MethodInfo.Invoke(null,dtoInformation.ParamObjs);
					Type returnType=dtoInformation.MethodInfo.ReturnType;
					if(returnType.IsInterface) {
						objResult=new DtoObject(objResult,objResult?.GetType()??returnType);
						returnType=typeof(DtoObject);
					}
					return XmlConverterSerializer.Serialize(returnType,objResult);
				}
				#endregion
				#region DtoGetString
				else if(type == typeof(DtoGetString)) {
					string strResult=(string)dtoInformation.MethodInfo.Invoke(null,dtoInformation.ParamObjs);
					return XmlConverter.XmlEscape(strResult);
				}
				#endregion
				#region DtoGetBool
				else if(type == typeof(DtoGetBool)) {
					bool boolResult=(bool)dtoInformation.MethodInfo.Invoke(null,dtoInformation.ParamObjs);
					return boolResult.ToString();
				}
				#endregion
				else {
					throw new NotSupportedException("Dto type not supported: "+type.FullName);
				}
			}
			catch(Exception e) {
				DtoException exception=new DtoException();
				if(e.InnerException==null) {
					exception=GetDtoException(e);
				}
				else {
					exception=GetDtoException(e.InnerException);
				}
				return exception.Serialize();
			}
		}

		private static DtoException GetDtoException(Exception e) {
			DtoException dtoException=new DtoException();
			//The typical outter exception will be a TargetInvocationException due to how we process known DTO payloads.
			//Therefore, we need to get the InnerException which will be the actual exception that the method that was invoked threw.
			//E.g. we need to preserve the fact that an S class method threw an ODException and pass that along to the client workstation.
			dtoException.ExceptionType=e.GetType().Name;
			if(e.GetType()==typeof(ODException)) {
				dtoException.ErrorCode=((ODException)e).ErrorCode;
			}
			dtoException.Message=e.Message;
			return dtoException;
		}

		///<summary>Contains all information about a recieved DTO.</summary>
		private class DtoInformation {
			///<summary>The full name of the method include namespace, class, and method.</summary>
			public string[] FullNameComponents;
			///<summary>The name of the assembly/namespace. Usually OpenDentBusiness, but may be a plugin.</summary>
			public string AssemblyName;
			///<summary>The name of the class that stores the method that will be invoked. If the namespace has a sub-namespace, such as 
			///OpenDentBusiness.Eclaims, the ClassName will include both the sub-namespace and the class such as Eclaims.Eclaims.</summary>
			public string ClassName;
			///<summary>The name of the method that will be invoked.</summary>
			public string MethodName;
			///<summary>The return type of the class that will be invoked.</summary>
			public Type ClassType;
			///<summary>The assembly object for the given AssemblyName.</summary>
			public Assembly Ass;
			///<summary>The parameters that are passed into the method.</summary>
			public DtoObject[] Parameters;
			///<summary>The types for the given parameters.</summary>
			public Type[] ParamTypes;
			///<summary>The information about the given method. Includes fields such as attributes and return types.</summary>
			public MethodInfo MethodInfo;
			///<summary>The objects for the passed in paramaters.</summary>
			public object[] ParamObjs;

			public DtoInformation(DataTransferObject dto) {
				FullNameComponents=GetComponentsFromDtoMeth(dto.MethodName);
				AssemblyName=FullNameComponents[0];//OpenDentBusiness or else a plugin name
				ClassName=FullNameComponents[1];
				MethodName=FullNameComponents[2];
				ClassType=null;
				Ass=Plugins.GetAssembly(AssemblyName);
				if(Ass==null) {
					ClassType=Type.GetType(AssemblyName//actually, the namespace which we require to be same as assembly by convention
						+"."+ClassName+","+AssemblyName);
				}
				else {//plugin was found
					ClassType=Ass.GetType(AssemblyName//actually, the namespace which we require to be same as assembly by convention
						+"."+ClassName);
				}
				Parameters=dto.Params;
				ParamTypes=DtoObject.GenerateTypes(Parameters,AssemblyName);
				MethodInfo=ClassType.GetMethod(MethodName,ParamTypes);
				if(MethodInfo==null) {
					throw new ApplicationException("Method not found with "+Parameters.Length.ToString()+" parameters: "+dto.MethodName);
				}
				ParamObjs=DtoObject.GenerateObjects(Parameters);
			}

			///<summary>Only used if the dto is trying to call "Userods.HashPassword".
			///This is so that passwords will be hashed on the server to utilize the server's MD5 hash algorithm instead of the workstation's algorithm.  
			///This is due to the group policy security option "System cryptography: Use FIPS compliant algorithms for encryption,
			///hashing and signing" that is enabled on workstations for some users but not on the server.  This allows those users to utilize the server's
			///algorithm without requiring the workstations to have the algorithm at all.</summary>
			public string GetHashPassword() {
				string strResult=(string)MethodInfo.Invoke(null,ParamObjs);
				strResult=XmlConverter.XmlEscape(strResult);
				return strResult;
			}

			///<summary>Helper function to handle full method name and turn it into 3 components.  The 3 components returned are:
			///1. Assembly name
			///2. Class name (however, this may contain the portion of the namespace after the assembly, 
			///e.g. if "OpenDentBusiness.Eclaims.Eclaims.GetMissingData" is passed in, this component will contain "Eclaims.Eclaims".)
			///3. Method name</summary>
			private static string[] GetComponentsFromDtoMeth(string methodName) {
				if(methodName.Split('.').Length==2) {
					//Versions prior to 14.3 will send 2 components. 14.3 and above will send the assembly name OpenDentBusiness or plugin assembly name.  
					//If only 2 components are received, we will prepend OpenDentBusiness so this will be backward compatible with versions prior to 14.3.
					methodName="OpenDentBusiness."+methodName;
				}
				if(methodName.Split('.').Length<=3) {
					return methodName.Split('.');
				}
				//The method is in a namespace that contains multiple parts.
				int firstIdx=methodName.IndexOf('.');
				int lastIdx=methodName.LastIndexOf('.');
				return new string[] {
					//First part of namespace which should also be the assembly name
					methodName.Substring(0,firstIdx),
					//The rest of the namespace plus the class name
					methodName.Substring(firstIdx+1,lastIdx-firstIdx-1),
					//The method name
					methodName.Substring(lastIdx+1)
				};
			}
		}
	}
}
