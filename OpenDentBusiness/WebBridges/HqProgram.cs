using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using CodeBase;
using OpenDentBusiness.UI;
using WebServiceSerializer;

namespace OpenDentBusiness {
	[Serializable]
	public class HqProgram {
		#region private properties
		private static List<HqProgram> _listHqPrograms;
		private static object _lock=new object();
		private ProgramName _programName;
		private string _programNameAsString;
		#endregion
		#region public properties
		public string ProgramNameAsString {
			get {
				return _programNameAsString;
			}
			set {
				_programNameAsString=value;
				Enum.TryParse(value,out _programName);
			}
		}
		
		public bool IsEnabled;
		public string CustErr;
		public List<HqProgramProperty> ListProperties;
		#endregion
		#region Methods
		public static ReadOnlyCollection<HqProgram> GetAll() {
			if(IsInitialized()) {
				lock(_lock) {
					return _listHqPrograms.AsReadOnly();
				}	
			}
			return new List<HqProgram>().AsReadOnly();//empty readonly list
		}
		
		public static bool IsInitialized() {
			lock(_lock) {
				return (_listHqPrograms!=null);
			}
		}

		///<summary>Fills in memory list of HqProgram.  Returns milliseconds for how frequently HQ wants this method called.  Throws exceptions.</summary>
		public static int Download() {
			string result="";
			result=WebServiceMainHQProxy.GetWebServiceMainHQInstance()
				.EnableAdditionalFeatures(PayloadHelper.CreatePayload("",eServiceCode.SoftwareUpdate));
			PayloadHelper.CheckForError(result);//Throws if download failed.
			List<HqProgram> listHqProgs=WebSerializer.DeserializeTag<List<HqProgram>>(result,"ListHqPrograms");
			listHqProgs=listHqProgs.Where(x => x._programName!=ProgramName.None).ToList();//Ignore any programs that could not be deserialized
			lock(_lock) {
				_listHqPrograms=listHqProgs;
			}
			long intervalHours=WebSerializer.DeserializeTag<long>(result,"IntervalHours");			
			UpdateCaches();
			return (int)TimeSpan.FromHours(intervalHours).TotalMilliseconds;
		}

		///<summary>Unit test methods need to clear out the HqProgram cache between tests.</summary>
		public static void ClearCaches() {
			if(!ODInitialize.IsRunningInUnitTest) {
				return;
			}
			lock(_lock) {
				_listHqPrograms=null;
			}
		}

		private static void UpdateCaches() {			
			bool isCacheRefresh=false;
			List<Program> listProgs=Programs.GetListDeep().Where(x=>ListTools.In(x.ProgName,_listHqPrograms.Select(x=>x.ProgramNameAsString))).ToList();
			foreach(HqProgram hqProg in _listHqPrograms) {
				Program prog=listProgs.FirstOrDefault(x => x.ProgName==hqProg.ProgramNameAsString);
				if(prog is null) {
					continue;//Very important to skip for backward compatibility, otherwise we might insert ProgramProperties before a future convert script.
				}				
				Program old=prog.Copy();
				prog.IsDisabledByHq=!Programs.IsEnabledByHq(prog,out string _);
				if(prog.IsDisabledByHq) {
					prog.CustErr=hqProg.CustErr;
				}
				else {
					prog.CustErr="";
				}
				isCacheRefresh|=Programs.Update(prog,old);
				foreach(HqProgramProperty hqProp in hqProg.ListProperties) {
					ProgramProperty prop=ProgramProperties.GetFirstOrDefault(x => x.ProgramNum==prog.ProgramNum && x.PropertyDesc==hqProp.PropertyDesc);
					if(prop is null) {
						continue;
					}
					prop.PropertyValue=hqProp.PropertyValue;
					ProgramProperties.Update(prop);
					isCacheRefresh=true;
				}
			}
			if(isCacheRefresh) {
				Programs.RefreshCache();
				ProgramProperties.RefreshCache();
				Signalods.SetInvalid(InvalidType.Programs);
			}
		}
	}

	#endregion
	[Serializable]
	public class HqProgramProperty {
		public string PropertyDesc;
		public string PropertyValue;
	}
} 
