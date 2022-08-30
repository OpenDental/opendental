using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormAnesthMedSuppliersEdit : Form
	{
        public AnesthMedSupplier SupplCur;

		public FormAnesthMedSuppliersEdit()
		{
			InitializeComponent();
			Lan.F(this);
		}

        private void FormAnesthMedSuppliersEdit_Load(object sender, EventArgs e)
        {
            textSupplierName.Text = SupplCur.SupplierName;
            textPhone.Text = SupplCur.Phone;
            textPhoneExt.Text = SupplCur.PhoneExt;
            textFax.Text = SupplCur.Fax;
            textAddr1.Text = SupplCur.Addr1;
            textAddr2.Text = SupplCur.Addr2;
            textCity.Text = SupplCur.City;
            textState.Text = SupplCur.State;
            textZip.Text = SupplCur.Zip;
            textContact.Text = SupplCur.Contact;
            textWebSite.Text = SupplCur.WebSite;
            richTextNotes.Text = SupplCur.Notes;
        }
		private void butOK_Click(object sender,EventArgs e) {
            /* (textSupplierName.Text == "")
            {
                MessageBox.Show(Lan.g(this, "Supplier name cannot be blank."));
                return;
            }
            if (CultureInfo.CurrentCulture.Name == "en-US")
            {
                if (textPhone.Text != "" && TelephoneNumbers.FormatNumbersExactTen(textPhone.Text) == "")
                {
                    MessageBox.Show(Lan.g(this, "Phone number must be in a 10-digit format."));
                    return;
                }
                if (textFax.Text != "" && TelephoneNumbers.FormatNumbersExactTen(textFax.Text) == "")
                {
                    MessageBox.Show(Lan.g(this, "Fax number must be in a 10-digit format."));
                    return;
                }
            }*/
            SupplCur.SupplierName = textSupplierName.Text;
            SupplCur.Phone = textPhone.Text;
            SupplCur.PhoneExt = textPhoneExt.Text;
            SupplCur.Fax = textFax.Text;
            SupplCur.Addr1 = textAddr1.Text;
            SupplCur.Addr2 = textAddr2.Text;
            SupplCur.City = textCity.Text;
            SupplCur.State = textState.Text;
            SupplCur.Zip = textZip.Text;
            SupplCur.Contact = textContact.Text;
            SupplCur.WebSite = textWebSite.Text;
            SupplCur.Notes = richTextNotes.Text;
            AnesthMedSuppliers.WriteObject(SupplCur);

            /*try
            {
                AnesthMedSuppliers.WriteObject(SupplCur);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }*/
            DialogResult = DialogResult.OK;
			
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void textPhone_TextChanged(object sender, EventArgs e)
		{
			/*int cursor = textPhone.SelectionStart;
			int length = textPhone.Text.Length;
			textPhone.Text = TelephoneNumbers.AutoFormat(textPhone.Text);
			if (textPhone.Text.Length > length)
				cursor++;
			textPhone.SelectionStart = cursor;*/
		}

        private void textFax_TextChanged(object sender, EventArgs e)
        {
            /*int cursor = textFax.SelectionStart;
            int length = textFax.Text.Length;
            textFax.Text = TelephoneNumbers.AutoFormat(textFax.Text);
            if (textFax.Text.Length > length)
                cursor++;
            textFax.SelectionStart = cursor;*/
        }

		private void textSupplierName_TextChanged(object sender, EventArgs e)
		{
			/*int cursor = textSupplierName.SelectionStart;
			int length = textSupplierName.Text.Length;
			textSupplierName.Text = textSupplierName.Text;
			textSupplierName.SelectionStart = cursor;*/
		}

		private void textCity_TextChanged(object sender, EventArgs e)
		{

		}


	}
}