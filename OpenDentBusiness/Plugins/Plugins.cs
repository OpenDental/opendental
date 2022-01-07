using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness.FileIO;
using System.Linq;
using System.Threading;

namespace OpenDentBusiness {
	public class Plugins {
		///<summary>Lock object specifically for _listPlugins.</summary>
		private static ReaderWriterLockSlim _lock=new ReaderWriterLockSlim();
		///<summary>Do not directly reference this variable, use ListPlugins instead.</summary>
		private static List<PluginContainer> _listPlugins;

		///<summary>A list of all plug-ins available.  Dynamically loads in plug-ins if accessing from the middle tier.
		///Also, the getter and setter safely lock access to _listPlugins but do not worry about returning deep copies of the contents.</summary>
		private static List<PluginContainer> ListPlugins {
			get {
				bool isListPluginsNull;
				_lock.EnterReadLock();
				try {
					isListPluginsNull=(_listPlugins==null);
				}
				finally {
					_lock.ExitReadLock();
				}
				if(isListPluginsNull && RemotingClient.RemotingRole==RemotingRole.ServerWeb) {
					LoadAllPlugins(null);
				}
				List<PluginContainer> listPlugins=null;
				_lock.EnterReadLock();
				try {
					if(_listPlugins!=null) {
						//The actual contents of each plug-in is not important to be preserved (all variables should NEVER change).
						//The important part is to make sure that the list itself is locked when accessing it (for looping purposes).
						//Therefore, make a deep copy of the actual list of items but keep shallow copies of the items themselves (saves time).
						listPlugins=new List<PluginContainer>(_listPlugins);
					}
				}
				finally {
					_lock.ExitReadLock();
				}
				return listPlugins;
			}
			set {
				_lock.EnterWriteLock();
				try {
					_listPlugins=value;
				}
				finally {
					_lock.ExitWriteLock();
				}
			}
		}

		public static bool PluginsAreLoaded {
			get {
				return (ListPlugins!=null);
			}
		}

		///<summary>If this is middle tier, pass in null.</summary>
		public static void LoadAllPlugins(Form host) {
			//No need to check RemotingRole; no call to db.
			if(ODBuild.IsWeb()) {
				return;//plugins not allowed in cloud mode
			}
			List<PluginContainer> listPlugins=new List<PluginContainer>();
			//Loop through all programs that are enabled with a plug-in dll name set.
			foreach(Program program in Programs.GetWhere(x => x.Enabled && !string.IsNullOrEmpty(x.PluginDllName))) {
				string dllPath=ODFileUtils.CombinePaths(Application.StartupPath,program.PluginDllName);
				if(RemotingClient.RemotingRole==RemotingRole.ServerWeb) {
					dllPath=ODFileUtils.CombinePaths(System.Web.HttpContext.Current.Server.MapPath(null),program.PluginDllName);
				}
				//Check for the versioning trigger.
				//For example, the plug-in might be entered as MyPlugin[VersionMajMin].dll. The bracketed section will be removed when loading the dll.
				//So it will look for MyPlugin.dll as the dll to load. However, before it loads, it will look for a similar dll with a version number.
				//For example, if using version 14.3.23, it would look for MyPlugin14.3.dll. 
				//If that file is found, it would replace MyPlugin.dll with the contents of MyPlugin14.3.dll, and then it would load MyPlugin.dll as normal.
				if(dllPath.Contains("[VersionMajMin]")) {
					Version vers=Assembly.GetAssembly(typeof(Db)).GetName().Version;
					string dllPathWithVersion=dllPath.Replace("[VersionMajMin]",vers.Major.ToString()+"."+vers.Minor.ToString());
					dllPath=dllPath.Replace("[VersionMajMin]","");//now stripped clean
					if(File.Exists(dllPathWithVersion)) {
						File.Copy(dllPathWithVersion,dllPath,true);
					}
					else{
						//try the Plugins folder
						if(PrefC.AtoZfolderUsed!=DataStorageType.InDatabase) {//must have an AtoZ folder to check
							string dllPathVersionCentral=FileAtoZ.CombinePaths(ImageStore.GetPreferredAtoZpath(),"Plugins",
								program.PluginDllName.Replace("[VersionMajMin]",vers.Major.ToString()+"."+vers.Minor.ToString()));
							if(FileAtoZ.Exists(dllPathVersionCentral)) {
								FileAtoZ.Copy(dllPathVersionCentral,dllPath,FileAtoZSourceDestination.AtoZToLocal,doOverwrite:true);
							}
						}
					}
				}
				//We now know the exact name of the dll for the plug-in.  Check to see if it is present.
				if(!File.Exists(dllPath)) {
					continue;//Nothing to do.
				}
				//The dll was found, try and load it in.
				PluginBase plugin=null;
				Assembly ass=null;
				string assName="";
				try {
					ass=Assembly.LoadFile(dllPath);
					assName=Path.GetFileNameWithoutExtension(dllPath);
					string typeName=assName+".Plugin";
					Type type=ass.GetType(typeName);
					plugin=(PluginBase)Activator.CreateInstance(type);
					plugin.Host=host;
				}
				catch(Exception ex) {
					//Never try and show message boxes when on the middle tier, there is no UI.  We should instead log to a file or the event viewer.
					if(RemotingClient.RemotingRole!=RemotingRole.ServerWeb) {
						//Notify the user that their plug-in is not loaded.
						MessageBox.Show("Error loading Plugin:"+program.PluginDllName+"\r\n"+ex.Message);
					}
					continue;//Don't add it to plugin list.
				}
				//The plug-in was successfully loaded and will start getting hook notifications.  Add it to the list of loaded plug-ins.
				PluginContainer container=new PluginContainer();
				container.Plugin=plugin;
				container.ProgramNum=program.ProgramNum;
				container.Assemb=ass;
				container.Name=assName;
				listPlugins.Add(container);
			}
			ListPlugins=listPlugins;
		}

		///<summary>Returns null if no plugin assembly loaded with the given name.
		///So OpenDentBusiness can be passed through here quickly to return null.</summary>
		public static Assembly GetAssembly(string name) {
			if(ListPlugins==null) {
				return null;//Fail silently if plugins could not be loaded.
			}
			PluginContainer pluginContainer=ListPlugins.FirstOrDefault(x => x.Name==name);
			return (pluginContainer==null ? null : pluginContainer.Assemb);
		}

		///<summary>Will return true if a plugin implements this method, replacing the default behavior.</summary>
		public static bool HookMethod(object sender,string hookName,params object[] parameters) {
			if(ListPlugins==null) {
				return false;//Fail silently if plugins could not be loaded.
			}
			foreach(PluginContainer pluginContainer in ListPlugins) {
				try {
					//Invoke the first implementation that we come across even if there are multiple plug-ins that implement this HookMethod.
					if(pluginContainer.Plugin.HookMethod(sender,hookName,parameters)) {
						return true;
					}
				}
				catch(Exception e) {
					pluginContainer.Plugin.HookException(e);
					//Continue the for loop looking for another potential hook that implements this method since this one failed.
				}
			}
			return false;//Indicates that no implementation was found for this method and that the default behavior is desired.
		}

		///<summary>Adds code without disrupting existing code.</summary>
		public static void HookAddCode(object sender,string hookName,params object[] parameters) {
			if(ListPlugins==null) {
				return;//Fail silently if plugins could not be loaded.
			}
			foreach(PluginContainer pluginContainer in ListPlugins) {
				//if there are multiple plugins, we run them all
				try {
					pluginContainer.Plugin.HookAddCode(sender,hookName,parameters);
				}
				catch(Exception e) {
					pluginContainer.Plugin.HookException(e);
					//Continue the loop looking for another potential hook that implements this method since this one failed.
				}
			}
		}

		public static void LaunchToolbarButton(long programNum,long patNum) {
			if(ListPlugins==null) {
				return;//Fail silently if plugins could not be loaded.
			}
			PluginContainer pluginContainer=ListPlugins.FirstOrDefault(x => x.ProgramNum==programNum && x.Plugin!=null);
			if(pluginContainer!=null) {
				try {
					pluginContainer.Plugin.LaunchToolbarButton(patNum);
				}
				catch(Exception e) {
					pluginContainer.Plugin.HookException(e);
				}
			}
		}


	}
}