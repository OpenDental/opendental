﻿using System;
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
		///<summary>This tells us what kind of image is present (webcam vs stock). If it's a stock image, then we won't try repeatedly to get that same image because it's larger and slower. But this field does not prevent frequent attempts to get webcam image.</summary>
		public EnumMapImageDisplayStatus MapImageDisplayStatus;
		///<summary>The employeeNum of the image showing. The image refresh lags a bit behind the other details, so we need to track it separately.</summary>
		public long EmployeeNumImage;


		public UserControlMapDetails() {
			InitializeComponent();
			this.DoubleBuffered=true;
		}

		///<summary>This method will set the detail panel values </summary>
		public void SetEmployee(MapCubicle mapCubicle) {
			string employeeName;
			string extension;
			string status;
			string strTimer;
			EmployeeNumCur=mapCubicle.EmployeeNum;
			//If the clicked cubicle doesn't have an employee associated with it, use generic values.
			if(EmployeeNumCur<1) {
				employeeName="";
				extension="x0000";
				status="None";
				strTimer="0:00:00";
				//odPictureBoxEmployee.Image=null;
			}
			else {
				employeeName=mapCubicle.PhoneCur.EmployeeName;
				extension="x"+mapCubicle.Extension.ToString();
				status=mapCubicle.Status;
				strTimer=mapCubicle.TimeSpanElapsed.ToStringHmmss();
				//odPictureBoxEmployee.Image=bitmap;
			}
			labelUserName.Text=employeeName;
			labelExtensionDesc.Text=extension+"   "+mapCubicle.MapAreaCur.Description;
			labelStatusTime.Text=status+"   "+strTimer;
			labelCustomer.Text=mapCubicle.PhoneCur.CustomerNumber;
		}

		///<summary>This is for the new FormMap that replaced old FormMapHQ.</summary>
		public void SetEmployee2(CubicleClickedDetail cubicleClickedDetail) {
			string employeeName;
			string extension;
			string status;
			string strTimer;
			EmployeeNumCur=cubicleClickedDetail.EmployeeNum;
			//If the clicked cubicle doesn't have an employee associated with it, use generic values.
			if(EmployeeNumCur<1) {
				employeeName="";
				extension="x0000";
				status="None";
				strTimer="0:00:00";
				//odPictureBoxEmployee.Image=null;
			}
			else {
				employeeName=cubicleClickedDetail.EmployeeName;
				extension="x"+cubicleClickedDetail.Extension.ToString();
				status=cubicleClickedDetail.Status;
				strTimer=cubicleClickedDetail.TimeSpanElapsed.ToStringHmmss();
				//odPictureBoxEmployee.Image=bitmap;
			}
			labelUserName.Text=employeeName;
			labelExtensionDesc.Text=extension+"   "+cubicleClickedDetail.Description;
			labelStatusTime.Text=status+"   "+strTimer;
			labelCustomer.Text=cubicleClickedDetail.CustomerNumber;
		}

		///<summary>Works for null</summary>
		public void SetBitmap(Bitmap bitmap,EnumMapImageDisplayStatus mapImageDisplayStatus,long employeeNum){
			odPictureBoxEmployee.Image?.Dispose();
			odPictureBoxEmployee.Image=bitmap;
			MapImageDisplayStatus=mapImageDisplayStatus;
			EmployeeNumImage=employeeNum;
		}

		///<summary>Should be called from FormMapHQ.SetPhoneList to refresh the currently displayed employeee on signal.</summary>
		public void UpdateControl(MapCubicle clickedPhone) {
			//Only need to worry about timer and status changing on signal.
			labelCustomer.Text=clickedPhone.PhoneCur.CustomerNumber;
			labelStatusTime.Text=clickedPhone.Status+"   "+clickedPhone.TimeSpanElapsed.ToStringHmmss();
		}
	}

	public enum EnumMapImageDisplayStatus{
		///<summary>No image. Null.</summary>
		Empty,
		///<summary></summary>
		Stock,
		///<summary></summary>
		WebCam
	}
}
