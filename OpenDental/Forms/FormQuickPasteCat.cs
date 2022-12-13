using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using System.Collections.Generic;

namespace OpenDental{
	/// <summary> </summary>
	public partial class FormQuickPasteCat : FormODBase{
		public QuickPasteCat QuickPasteCatCur;

		///<summary>The QuickPasteCat passed into this constructor is directly manipulated.</summary>
		public FormQuickPasteCat(QuickPasteCat quickPasteCat){
			QuickPasteCatCur=quickPasteCat.Copy();
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormQuickPasteCat_Load(object sender, System.EventArgs e) {
			listType.Items.Clear();
			string[] stringArrayTypes;
			if(QuickPasteCatCur.DefaultForTypes==null
				|| QuickPasteCatCur.DefaultForTypes==""){
				stringArrayTypes=new string[0];
			}
			else{
				stringArrayTypes=QuickPasteCatCur.DefaultForTypes.Split(',');
			}
			for(int i=0;i<Enum.GetNames(typeof(QuickPasteType)).Length;i++){
				if((i==(int)QuickPasteType.WebChat || 
					i==(int)QuickPasteType.Office ||
					i==(int)QuickPasteType.JobManager ||
					i==(int)QuickPasteType.EmployeeStatus||
					i==(int)QuickPasteType.FAQ)
					&& !PrefC.IsODHQ) 
				{
					continue;
				}
				listType.Items.Add(Enum.GetNames(typeof(QuickPasteType))[i],(QuickPasteType)i);
			}
			for(int i=0;i<stringArrayTypes.Length;i++) {
				if(listType.Items.Contains((QuickPasteType)PIn.Int(stringArrayTypes[i]))) {
					listType.SetSelectedEnum((QuickPasteType)PIn.Int(stringArrayTypes[i]));
				}
			}
			textDescription.Text=QuickPasteCatCur.Description;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			QuickPasteCatCur.Description=textDescription.Text;
			QuickPasteCatCur.DefaultForTypes="";
			QuickPasteCatCur.DefaultForTypes=string.Join(",",listType.GetListSelected<int>());
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		


	}
}





















