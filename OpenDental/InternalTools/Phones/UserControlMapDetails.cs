using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental.InternalTools.Phones {
	public partial class UserControlMapDetails:UserControl {
		///<summary>Allows for checking the employee currently in the details page when updating elapsed time.</summary>
		public long EmployeeNumCur {get; private set;}

		public UserControlMapDetails() {
			InitializeComponent();
		}

		///<summary>This method will set the detail panel values </summary>
		public void SetEmployee(MapAreaRoomControl mapAreaRoomControl,Image image) {
		string employeeName;
		string extension;
		string status;
		string timer;
		EmployeeNumCur=mapAreaRoomControl.EmployeeNum;
			//If the clicked cube doesn't have an employee associated to it, use generic values.
			if(EmployeeNumCur<1) {
				employeeName="";
				extension="x0000";
				status="None";
				timer="0:00:00";
				odPictureBoxEmployee.Image=null;
			}
			else {
				employeeName=mapAreaRoomControl.PhoneCur.EmployeeName;
				extension="x"+mapAreaRoomControl.Extension.ToString();
				status=mapAreaRoomControl.Status;
				timer=mapAreaRoomControl.Elapsed.ToStringHmmss();
				odPictureBoxEmployee.Image=image;
			}
			labelUserName.Text=employeeName;
			labelExtensionDesc.Text=extension+"   "+mapAreaRoomControl.MapAreaItem.Description;
			labelStatusTime.Text=status+"   "+timer;
			labelCustomer.Text=mapAreaRoomControl.PhoneCur.CustomerNumber;
		}

		///<summary>Should be called from FormMapHQ.SetPhoneList to refresh the currently displayed employeee on signal.</summary>
		public void UpdateControl(MapAreaRoomControl clickedPhone) {
			//Only need to worry about timer and status changing on signal.
			labelCustomer.Text=clickedPhone.PhoneCur.CustomerNumber;
			labelStatusTime.Text=clickedPhone.Status+"   "+clickedPhone.Elapsed.ToStringHmmss();
		}
	}
}
