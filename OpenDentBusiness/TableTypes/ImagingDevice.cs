using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Xml.Serialization;

namespace OpenDentBusiness {
	/// <summary>Xray sensor, camera, etc. Depending on the hardware, this can either be one physical device or a set of similar devices.</summary>
	[Serializable()]
	public class ImagingDevice: TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ImagingDeviceNum;
		/// <summary>Any description of the device.</summary>
		public string Description;
		/// <summary>Name of the computer where this device is available.  Optional.  If blank, then this device will be available to all computers.</summary>
		public string ComputerName;
		///<summary>Enum:EnumImgDeviceType </summary>
		public EnumImgDeviceType DeviceType;
		///<summary>The name of the twain device as in Windows.</summary>
		public string TwainName;
		///<summary></summary>
		public int ItemOrder;
		///<summary></summary>
		public bool ShowTwainUI;
		
		///<summary></summary>
		public ImagingDevice Copy() {
			return (ImagingDevice)this.MemberwiseClone();
		}

		public bool IsTwain(){
			if(DeviceType==EnumImgDeviceType.TwainRadiograph){//might add more twain types
				return true;
			}
			return false;
		}
		


	}

	///<summary>Order cannot change, since we store in db as enum number.  But we show the list in the UI in a different order or our choosing.</summary>
	public enum EnumImgDeviceType{
		///<summary></summary>
		TwainRadiograph,
		///<summary></summary>
		[Description("XDR (not functional)")]
		XDR
	}
}
