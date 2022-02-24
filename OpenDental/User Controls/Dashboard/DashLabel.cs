using System.Drawing;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class DashLabel:Label,IDashWidgetField {
		private SheetField _sheetField;
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();

		public DashLabel() {
			InitializeComponent();
		}

		public void PassLayoutManager(LayoutManagerForms layoutManager){
			LayoutManager=layoutManager;
		}

		public void SetData(PatientDashboardDataEventArgs data,SheetField sheetField) {
			if(sheetField==null) {
				return;
			}
			_sheetField=sheetField;//Only make a change if valid sheetField given.
		}

		public void RefreshData(Patient pat,SheetField sheetField) {
			_sheetField=sheetField;
		}

		public void RefreshView() {
			Text=_sheetField.FieldValue;
			TextAlign=ConvertToContentAlignment(_sheetField.TextAlign);
			string fontName="Microsoft Sans Serif";
			if(!string.IsNullOrWhiteSpace(_sheetField.FontName)){
				fontName=_sheetField.FontName;
			}
			FontStyle fontStyle=FontStyle.Regular;
			if(_sheetField.FontIsBold){
				fontStyle=FontStyle.Bold;
			}
			float fontSize=LayoutManager.ScaleF(8);
			if(_sheetField.FontSize>0){
				fontSize=LayoutManager.ScaleF(_sheetField.FontSize);
			}
			Font=new Font(fontName,fontSize,fontStyle);
		}

		private ContentAlignment ConvertToContentAlignment(HorizontalAlignment align) {
			switch(align) {
				case HorizontalAlignment.Right:
					return ContentAlignment.TopRight;
				case HorizontalAlignment.Center:
					return ContentAlignment.TopCenter;
				case HorizontalAlignment.Left:
				default:
					return ContentAlignment.TopLeft;
			}
		}
	}
}
