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
	///<summary>This is the box at the left of the map that shows details for one employee. Still used in the new map</summary>
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

		///<summary>This is for the new FormMap that replaced old FormMapHQ.</summary>
		public void SetEmployee(CubicleClickedDetail cubicleClickedDetail) {
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
			pictureBox.Image?.Dispose();
			pictureBox.Image=bitmap;
			MapImageDisplayStatus=mapImageDisplayStatus;
			EmployeeNumImage=employeeNum;
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
