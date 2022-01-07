using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDental;
using OpenDentBusiness;
using CodeBase;

namespace UnitTests{
	public partial class FormComboTests : FormODBase{
		private List<Employee> _listEmps=new List<Employee>();
		private List<string> _listStrings=new List<string>();
		List<RemotingRole> _listRemotingRole=new List<RemotingRole>();
		bool _useStringsForListBox=false;

		public FormComboTests(){
			InitializeComponent();
			InitializeLayoutManager();
			cClinic.IsTestModeNoDb=true;
			cClinicMulti.IsTestModeNoDb=true;
			cClinicL.IsTestModeNoDb=true;
			cClinicMultiL.IsTestModeNoDb=true;
			//new
			cClinic2.IsTestModeNoDb=true;
			cClinicMulti2.IsTestModeNoDb=true;
			cClinicL2.IsTestModeNoDb=true;
			cClinicMultiL2.IsTestModeNoDb=true;
			//in panel
			comboBoxClinicPicker1.IsTestModeNoDb=true;
			comboBoxClinicPicker2.IsTestModeNoDb=true;
			comboBoxClinicPicker3.IsTestModeNoDb=true;
			comboBoxClinicPicker4.IsTestModeNoDb=true;
		}

		private void FormComboTests_Load(object sender, EventArgs e){
			//this.Location=new Point(Location.X,Location.Y-200);
			for(int i=0;i<15;i++){
				comboBoxMS.Items.Add("Item"+i.ToString());
				//can't add clinics from outside
				if(i%2==0) {
					cPlus.Items.Add("Different"+i.ToString());
					cPlusMulti.Items.Add("Different"+i.ToString());
					cPlus2.Items.Add("Different"+i.ToString());
					cPlusMulti2.Items.Add("Different"+i.ToString());
				}
				else {
					cPlus.Items.Add("Item"+i.ToString());
					cPlusMulti.Items.Add("Item"+i.ToString());
					cPlus2.Items.Add("Item"+i.ToString());
					cPlusMulti2.Items.Add("Item"+i.ToString());
				}
			}
			List<string> listStrings=new List<string>(); 
			Random random=new Random();
			for(int i=0;i<=310;i++) {
				listStrings.Add(RandomString(random));
			}
			listStrings.Sort();
			for(int i=0;i<listStrings.Count;i++) {
				if(i<=30) {
					comboBoxOD1.Items.Add(listStrings[i]);
					comboBoxOD2.Items.Add(listStrings[i]);
				}
				comboBoxOD3.Items.Add(listStrings[i]);
				comboBoxOD4.Items.Add(listStrings[i]);
				//listBoxOD.Items.Add(listStrings[i]);
				//listBoxODMulti.Items.Add(listStrings[i]);
			}
			cPlus.SetSelected(2);
			cPlusMulti.SetSelected(2);
			cPlusMulti2.SetSelected(2);
			cPlus2.SetSelected(2);
			comboBoxMS.SelectedIndex=2;
			cClinic.SelectedClinicNum=2;
			cClinicMulti.SelectedClinicNum=2;
			cClinic2.SelectedClinicNum=2;
			cClinicMulti2.SelectedClinicNum=2;
			//listboxes------------------------------------------
			if(_useStringsForListBox) {
				for(int i=0;i<20;i++){
					_listStrings.Add("Item"+i.ToString());
					listBoxMS.Items.Add(_listStrings[i]);
					listBoxMSMulti.Items.Add(_listStrings[i]);
					listBoxOD.Items.Add(_listStrings[i]);
					listBoxODMulti.Items.Add(_listStrings[i]);
					listBoxODNone.Items.Add(_listStrings[i]);
				}
				//listBoxOD.Items.AddEnums<Types>();
				//listBoxODMulti.Items.AddEnums<Types>();
				listBoxMS.SelectedIndex=0;
				listBoxMSMulti.SetSelected(1,true);
				listBoxMSMulti.SetSelected(3,true);
				listBoxOD.SelectedIndex=0;
				listBoxODMulti.SetSelected(1);
				listBoxODMulti.SetSelected(3);
			}
			else {
				FillCustomItems();
			}
			FillGrid();
		}

		private void FillCustomItems() {
			for(int i = 0;i<20;i++) {
				Employee employee = new Employee() { FName="Item",LName=i.ToString(),PayrollID=i.ToString(),EmployeeNum=i };
				_listEmps.Add(employee);
				listBoxMS.Items.Add(employee);
				listBoxMSMulti.Items.Add(employee);
				listBoxOD.Items.Add(employee.FName+employee.LName,employee);
				listBoxODMulti.Items.Add(employee.FName+employee.LName,employee);
				listBoxODNone.Items.Add(employee.FName+employee.LName,employee);
			}
			List<Employee> test1=listBoxODNone.GetListSelected<Employee>();
			Employee emp2=listBoxODNone.GetSelected<Employee>();
			long test2=listBoxODNone.GetSelectedKey<Employee>(x=>x.EmployeeNum);
			string test3=listBoxODNone.GetStringSelectedItems();
			//for(int i = 0;i<Enum.GetValues(typeof(RemotingRole)).Length;i++) {
			//	_listRemotingRole.Add((RemotingRole)i);
			//	listBoxMS.Items.Add(_listRemotingRole[i]);
			//	listBoxMSMulti.Items.Add(_listRemotingRole[i]);
			//	listBoxOD.Items.Add(Enum.GetName(typeof(RemotingRole),_listRemotingRole[i]),_listRemotingRole[i]);
			//	listBoxODMulti.Items.Add(Enum.GetName(typeof(RemotingRole),_listRemotingRole[i]),_listRemotingRole[i]);
			//}
			//listBoxOD.Items.AddEnums<RemotingRole>();
			//listBoxODMulti.Items.AddEnums<RemotingRole>();
		}

		private void FillGrid(){
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Items",100);
			gridMain.ListGridColumns.Add(col);
			 
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<20;i++){
				row=new GridRow();
				row.Cells.Add("Item"+i.ToString());		  
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			gridMain.SetSelected(2,true);
		}

		private void ButTest_Click(object sender, EventArgs e){
			List<long> listLong=cClinicMultiL.ListSelectedClinicNums;
		}

		// Generate a random string with a given size  
		public string RandomString(Random random) {
			StringBuilder builder=new StringBuilder();
			string chars="ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			string result=new string(
        Enumerable.Repeat(chars,9).Select(s => s[random.Next(s.Length)]).ToArray());
			return result.ToLower();
		}

		private void CPlus2_SelectionChangeCommitted(object sender,EventArgs e) {
			MsgBox.Show(sender,MsgBoxButtons.OKCancel,"This is a test");
		}

		private void CClinic2_SelectionChangeCommitted(object sender,EventArgs e) {
			MsgBox.Show(sender,MsgBoxButtons.OKCancel,"This is a test");
		}

		private void FormComboTests_DpiChanged(object sender, DpiChangedEventArgs e)
		{
			//Size=new Size(LogicalToDeviceUnits(Width),LogicalToDeviceUnits(Height));
		}

		private void FormComboTests_DpiChangedAfterParent(object sender, EventArgs e)
		{

		}

		private void FormComboTests_DpiChangedBeforeParent(object sender, EventArgs e)
		{

		}
		
		enum Types {
			Char,
			String,
			Int,
			Long
			//Float,
			//Double,
			//Bool,
			//Object
		}

		private void ButListBoxOD_Click(object sender,EventArgs e) {
			//testing these methods to make sure they work correctly
			int selectedIndex=listBoxOD.SelectedIndex;
			List<int> listSelectedIndices=listBoxOD.SelectedIndices;
			List<int> listSelectedIndicesMulti=listBoxODMulti.SelectedIndices;
			//Types enumType=listBoxODMulti.GetSelected<Types>();//throws an excption correctly
			//Types enumType=listBoxOD.GetSelected<Types>();
			Employee employee=listBoxOD.GetSelected<Employee>();
			long selectedKey=listBoxOD.GetSelectedKey<Employee>(x=>x.EmployeeNum);
			selectedKey=((Employee)listBoxOD.SelectedItem).EmployeeNum;
			string stringSelectedItems=listBoxODMulti.GetStringSelectedItems();
			//Display the reults
			MsgBox.Show("Selected index from single select: "+selectedIndex
				//+"\nGetSelected from single: "+enumType
				+"\nGetSelectedKey from single: "+selectedKey.ToString());

				//+"\nString selected items from multi: "+stringSelectedItems);
		}

		private void ButSelectedSingle_Click(object sender,EventArgs e) {
			if(_useStringsForListBox) {
				listBoxOD.SelectedItem=null;
				listBoxMS.SelectedItem=_listStrings[1];
			}
			else {
				Employee employee=_listEmps[1];
				listBoxOD.SelectedItem=employee;
			}
			object selected=listBoxOD.SelectedItem;
			bool test=listBoxOD.Items.Contains("Item1");
			object item=listBoxMS.SelectedItem;
		}

		private void ButSelectedMulti_Click(object sender,EventArgs e) {
			if(_useStringsForListBox) {
				listBoxODMulti.SelectedItem=_listStrings[2];
				listBoxMSMulti.SelectedItem=_listStrings[2];
			}
			else {
				Employee employee=_listEmps[2];
				listBoxODMulti.SelectedItem=employee;
			}
			object item=listBoxMSMulti.SelectedItem;
		}
	}
}
