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
using OpenDental.UI;
using CodeBase;

namespace OpenDental {
	public partial class DashPatPicture:ODPictureBox,IDashWidgetField {
		private Bitmap _patPicture;
		private Document _docPatPicture;
		public LayoutManagerForms LayoutManager;

		public DashPatPicture() {
			InitializeComponent();
		}

		public void PassLayoutManager(LayoutManagerForms layoutManager){
			LayoutManager=layoutManager;
		}

		public void SetData(PatientDashboardDataEventArgs data,SheetField sheetField) {
			if(!IsNecessaryDataAvailable(data)) {
				return;
			}
			if(_docPatPicture==null) {
				return;
			}
			Document docForPat=data.ListDocuments.FirstOrDefault(x => x.DocNum==_docPatPicture.DocNum);
			if(sheetField!=null && docForPat!=null) {
				_docPatPicture=docForPat;
				using(Bitmap bitmapCopy=new Bitmap(data.BitmapImagesModule)) {
					SwapPatPicture(() => {
						using(Bitmap bitmap=ImageHelper.ApplyDocumentSettingsToImage(_docPatPicture,bitmapCopy,ImageSettingFlags.ALL)) {
							return ImageHelper.GetBitmapResized(bitmap,Math.Min(sheetField.Width,sheetField.Height));
						}
					});
				}
			}
		}

		private bool IsNecessaryDataAvailable(PatientDashboardDataEventArgs data) {
			if(data.Pat==null || data.ListDocuments==null || data.BitmapImagesModule==null) { 
				return false;//Necessary data not found in PatientDashboardDataEventArgs.  Make no change.
			}
			return true;
		}

		public void RefreshData(Patient pat,SheetField sheetField) {
			if(pat==null || 
				PrefC.AtoZfolderUsed==DataStorageType.InDatabase)//Do not use patient image when A to Z folders are disabled.
			{
				return;
			}
			void clear() {							
				_patPicture?.Dispose();
				_patPicture=null;
			}
			try{
				_docPatPicture=Documents.GetByNum(PIn.Long(sheetField.FieldValue),true);
				if(_docPatPicture is null) {
					clear();
					return;
				}
				//GetFullImage applies cropping/transposing/etc...
				Bitmap fullImage=ImageHelper.GetFullImage(_docPatPicture,ImageStore.GetPatientFolder(pat,ImageStore.GetPreferredAtoZpath()));
				SwapPatPicture(() => ImageHelper.GetBitmapResized(fullImage,Math.Min(sheetField.Width,sheetField.Height)));
				fullImage.Dispose();
			}
			catch(Exception e){
				e.DoNothing();//Something went wrong retrieving the image.  Default to "Patient Picture Unavailable".
				clear();
			}
		}

		public void RefreshView() {
			Image=_patPicture;
			HasBorder=true;
			TextNullImage="Patient Picture Unavailable";
		}

		private void SwapPatPicture(Func<Bitmap> funcGet) {
			Image imgOld=_patPicture;
			_patPicture=funcGet();
			imgOld?.Dispose();
			imgOld=null;
		}
	}
}
