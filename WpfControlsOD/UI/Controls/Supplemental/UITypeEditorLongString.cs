using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace WpfControls.UI {
	//This doesn't work, but I'm leaving it here anyway

	public class UITypeEditorLongString:UITypeEditor {
		public override object EditValue(ITypeDescriptorContext context,IServiceProvider provider,object value) {
			IWindowsFormsEditorService editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			if(editorService != null) {
				// use your custom form or control here
				FormLongStringEditor formLongStringEditor = new FormLongStringEditor();
				formLongStringEditor.Value = (string)value;
				editorService.DropDownControl(formLongStringEditor);
				value = formLongStringEditor.Value;
			}
			return value;
		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
	}
}
