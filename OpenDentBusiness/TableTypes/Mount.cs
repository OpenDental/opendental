using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Xml.Serialization;

namespace OpenDentBusiness {
	/// <summary>A mount shows in the images module just like other images in the tree.  But it is just a container for images within it rather than an actual image itself.  A mount layout cannot be edited once created for a patient (simply because we didn't add that functionality), but the individual images on it can be edited.</summary>
	[Serializable()]
	public class Mount : TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long MountNum;
		///<summary>FK to patient.PatNum</summary>
		public long PatNum;
		///<summary>FK to definition.DefNum. Categories for documents.</summary>
		public long DocCategory;
		/// <summary>The date/time at which the mount itself was created. Usually, all the images on the mount are the same date, but not always.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateCreated;
		/// <summary>Used to provide a document description in the image module tree-view.</summary>
		public string Description;
		/// <summary>To allow the user to enter specific information regarding the exam and tooth numbers, as well as points of interest in the xray images.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Note;
		/// <summary>The width of the mount, in pixels.</summary>
		public int Width;
		/// <summary>The height of the mount, in pixels.</summary>
		public int Height;
		///<summary>Color of the mount background.  Typically white for photos and black for radiographs.</summary>
		[XmlIgnore]
		public Color ColorBack;
		///<summary>FK to provider.ProvNum. Optional. Used for radiographs.</summary>
		public long ProvNum;

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("ColorBack",typeof(int))]
		public int ColorBackXml {
			get {
				return ColorBack.ToArgb();
			}
			set {
				ColorBack = Color.FromArgb(value);
			}
		}

		///<summary></summary>
		public Mount Copy() {
			Mount mount=(Mount)this.MemberwiseClone();
			if(ListMountItems!=null){
				mount.ListMountItems=new List<MountItem>(ListMountItems);
			}
			return mount;
		}

		[CrudColumn(IsNotDbColumn =true)]
		public List<MountItem> ListMountItems;

	}
}
