using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;
using CodeBase;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormSheetFieldAdd:FormODBase {
		public Sheet SheetCur;

		public FormSheetFieldAdd() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormSheetFieldAdd_Load(object sender,EventArgs e) {
			List<SheetFieldType> listSheetFieldTypes=SheetDefs.GetVisibleButtons(SheetCur.SheetType);
			butPatImage.Visible=listSheetFieldTypes.Contains(SheetFieldType.PatImage);
		}

		private void butPatImage_Click(object sender,EventArgs e) {
			if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
				MsgBox.Show(this,"Not allowed because not using AtoZ folder");
				return;
			}
			//Font font=new Font(SheetDefCur.FontName,SheetDefCur.FontSize);
			using FormSheetFieldEditPatImage formSheetFieldEditPatImage=new FormSheetFieldEditPatImage();
			formSheetFieldEditPatImage.SheetCur=SheetCur;
			SheetField sheetField=new SheetField();
			sheetField.IsNew=true;
			sheetField.FieldType=SheetFieldType.PatImage;
			sheetField.FieldName="";
			sheetField.FieldValue="";
			sheetField.SheetNum=SheetCur.SheetNum;
			sheetField.XPos=0;
			sheetField.YPos=0;
			sheetField.Width=100;
			sheetField.Height=100;
			formSheetFieldEditPatImage.SheetFieldCur=sheetField;
			formSheetFieldEditPatImage.ShowDialog();
			if(formSheetFieldEditPatImage.DialogResult!=DialogResult.OK  || formSheetFieldEditPatImage.SheetFieldCur==null) {//SheetFieldCur==null if it was Deleted
				return;
			}
			SheetCur.SheetFields.Add(sheetField);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}