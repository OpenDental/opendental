using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental {
	public class PluginLoader {
		///<summary>There is a similar method for Middle Tier at OpenDentBusiness.Plugins.LoadAllPlugins.</summary>
		public static void LoadAllPlugins(Form host) {
			if(Environment.MachineName.ToLower()=="jordanhome" || Environment.MachineName.ToLower()=="jordancryo"){
				bool isAllowed=DatabaseIntegrities.IsPluginAllowed("someplugin.dll");//to simulate loading a dll
			}
			//No need to check MiddleTierRole; no call to db.
			if(ODBuild.IsWeb()) {
				return;//plugins not allowed in cloud mode
			}
			List<PluginContainer> listPluginContainers=new List<PluginContainer>();
			//Loop through all programs that are enabled with a plug-in dll name set.
			List<Program> listPrograms=Programs.GetWhere(x => x.Enabled && !string.IsNullOrEmpty(x.PluginDllName));
			for(int i=0;i<listPrograms.Count;i++) {
				string dllPath=ODFileUtils.CombinePaths(Application.StartupPath,listPrograms[i].PluginDllName);
				if(RemotingClient.MiddleTierRole==MiddleTierRole.ServerMT) {
					dllPath=ODFileUtils.CombinePaths(System.Web.HttpContext.Current.Server.MapPath(null),listPrograms[i].PluginDllName);
				}
				if(!DatabaseIntegrities.IsPluginAllowed(listPrograms[i].PluginDllName)){
					DatabaseIntegrity databaseIntegrity=DatabaseIntegrities.GetOnePlugin(listPrograms[i].PluginDllName);
					if(databaseIntegrity is null){
						//keep going, I guess. Shouldn't happen
					}
					else{
						using FormDatabaseIntegrity formDatabaseIntegrity=new FormDatabaseIntegrity();
						formDatabaseIntegrity.IsPlugin=true;
						formDatabaseIntegrity.MessageToShow=databaseIntegrity.Message.Replace("[Plugin]",listPrograms[i].PluginDllName);
						formDatabaseIntegrity.ShowDialog();
						if(databaseIntegrity.Behavior==EnumIntegrityBehavior.PluginBlock){
							continue;
						}
						if(databaseIntegrity.Behavior==EnumIntegrityBehavior.PluginWarning){
							//nothing
						}
					}
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
								listPrograms[i].PluginDllName.Replace("[VersionMajMin]",vers.Major.ToString()+"."+vers.Minor.ToString()));
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
					if(RemotingClient.MiddleTierRole!=MiddleTierRole.ServerMT) {
						//Notify the user that their plug-in is not loaded.
						MessageBox.Show("Error loading Plugin:"+listPrograms[i].PluginDllName+"\r\n"+ex.Message);
					}
					continue;//Don't add it to plugin list.
				}
				//The plug-in was successfully loaded and will start getting hook notifications.  Add it to the list of loaded plug-ins.
				PluginContainer pluginContainer=new PluginContainer();
				pluginContainer.Plugin=plugin;
				pluginContainer.ProgramNum=listPrograms[i].ProgramNum;
				pluginContainer.Assemb=ass;
				pluginContainer.Name=assName;
				listPluginContainers.Add(pluginContainer);
			}
			Plugins.ListPlugins=listPluginContainers;
		}
	}
}
