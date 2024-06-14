using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

///<summary>The namespace for this class must match the dll filename, including capitalization.
///All other classes will typically belong to the same namespace too, but that's not a requirement.</summary>
namespace PluginExample {
	///<summary>Required class.  Don't change the name.</summary>
	public class Plugin : PluginBase {
		private Form _formHost;

		///<summary>When Host is set we will invoke ConvertPluginDatabase.Begin() which is our plug-in startup code.
		///This is to ensure that the database has been correctly upgraded for this specific version (set in AssemblyInfo.cs).</summary>
		public override Form Host {
			get { 
				return _formHost; 
			}
			set {
				_formHost=value;
				ConvertPluginDatabase.Begin();//if this crashes, it will bubble up and result in the plugin not loading.
				//If the plugin is only for personal use, then data tables do not need to be managed in the code.
				//They could instead be managed manually using a tool.
			}
		}

		public override bool HookMethod(object sender,string hookName,params object[] parameters) {//required method
			switch(hookName){
				case "ContrFamily.gridPat_CellDoubleClick":
					ContrFamilyP.gridPat_CellDoubleClick((OpenDental.ControlFamily)sender,(Patient)parameters[0]);
					return true;
				case "ContrChart.FillPtInfo":
					ContrChartP.FillPtInfo((OpenDental.ControlChart)sender,(Patient)parameters[0]);
					return true;
				default:
					return false;//this plugin does not implement the particular hook passed in.
			}
		}

		public override bool HookAddCode(object sender,string hookName,params object[] parameters) {//required method
			switch(hookName){
				case "ContrAccount.InitializeOnStartup_end":
					ContrAccountP.InitializeOnStartup_end((OpenDental.ControlAccount)sender);
					return true;
				case "ContrAccount.RefreshModuleData_end":
					ContrAccountP.RefreshModuleData_end((OpenDental.ControlAccount)sender,(Family)parameters[0],(Patient)parameters[1]);
					return true;
				case "ContrAccount.RefreshModuleData_null":
					ContrAccountP.RefreshModuleData_end((OpenDental.ControlAccount)sender,null,null);
					return true;
			

				default:
					return false;
			}
		}

		public override void LaunchToolbarButton(long patNum) {
			FormFromToolbar formFromToolBar=new FormFromToolbar();
			formFromToolBar.PatNum=patNum;
			formFromToolBar.ShowDialog();
		}

		public override void HookException(Exception e) {
			//This is a generic exception handler for any methods called through the plugin. This could happen if a method gets called that no longer 
			//exists.
			MessageBox.Show("There was an error in PluginExample. Please update the plugin.");
		}


	}
}
