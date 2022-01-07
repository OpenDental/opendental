using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using OpenDentBusiness.DataAccess;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormAnestheticMedsInventory:Form {
		
		private List<AnesthMedsInventory> listAnestheticMeds;
		public HorizontalAlignment textAlign;
		
		public FormAnestheticMedsInventory() {
			InitializeComponent();
			Lan.F(this);    
		}

		private void FormAnestheticMedsInventory_Load(object sender, System.EventArgs e){
			FillGrid();
		}

		private void FillGrid(){
			listAnestheticMeds = AnesthMeds.CreateObjects();
			gridAnesthMedsInventory.BeginUpdate();
			gridAnesthMedsInventory.Columns.Clear();
			textAlign=HorizontalAlignment.Center;
			ODGridColumn col = new ODGridColumn(Lan.g(this, "Anesthetic Medication"),200,textAlign);
			gridAnesthMedsInventory.Columns.Add(col);
			col = new ODGridColumn(Lan.g(this, "How Supplied"),200,textAlign);
			gridAnesthMedsInventory.Columns.Add(col);
			col = new ODGridColumn(Lan.g(this, "Quantity on Hand (mL)"),180,textAlign);
			gridAnesthMedsInventory.Columns.Add(col);
			gridAnesthMedsInventory.Rows.Clear();
			ODGridRow row;
			for (int i = 0; i < listAnestheticMeds.Count; i++)
			{
				row = new ODGridRow();
				row.Cells.Add(listAnestheticMeds[i].AnesthMedName);
				row.Cells.Add(listAnestheticMeds[i].AnesthHowSupplied);
				row.Cells.Add(listAnestheticMeds[i].QtyOnHand);
				gridAnesthMedsInventory.Rows.Add(row);
			}
			gridAnesthMedsInventory.EndUpdate();

		}

		private void butAddAnesthMeds_Click(object sender, EventArgs e){
				AnesthMedsInventory med = new AnesthMedsInventory();
				med.IsNew = true;
				FormAnesthMedsEdit FormM = new FormAnesthMedsEdit();
				FormM.Med = med;
				FormM.ShowDialog();
			if (FormM.DialogResult == DialogResult.OK)
			{
				FillGrid();
			} 

		}

		private void gridAnesthMedsInventory_CellDoubleClick(object sender, ODGridClickEventArgs e){
			Userod curUser = Security.CurUser;
			if (GroupPermissions.HasPermission(curUser.UserGroupNum, Permissions.AnesthesiaControlMeds))
			{
				FormAnesthMedsEdit FormME = new FormAnesthMedsEdit();
				FormME.Med = listAnestheticMeds[e.Row];
				FormME.ShowDialog();
				if (FormME.DialogResult == DialogResult.OK)
				{
					FillGrid();
				}
				return;
			}
			else
			{
				MessageBox.Show(Lan.g(this, "You must be an administrator with rights to control anesthetic medication inventory levels to unlock this action"));
				return;
			} 

		}

		private void butAnesthMedIntake_Click(object sender, EventArgs e){

			if (!Security.IsAuthorized(Permissions.AnesthesiaIntakeMeds))
			{
				butAnesthMedIntake.Enabled = false;
				return;
			} 
			else
			{
				FormAnestheticMedsIntake FormI = new FormAnestheticMedsIntake();
				FormI.ShowDialog();
				if (FormI.DialogResult == DialogResult.OK)
				{
				FillGrid();
				}
				
			}

		}

		private void butClose_Click(object sender, EventArgs e){
			Close();
		}

		private void butOK_Click(object sender, EventArgs e){

		}

		private void butCancel_Click(object sender, EventArgs e){
            DialogResult = DialogResult.Cancel;
		}

		private void butAdjustQtys_Click(object sender, EventArgs e){
			Userod curUser = Security.CurUser;
			if (GroupPermissions.HasPermission(curUser.UserGroupNum, Permissions.AnesthesiaControlMeds))
			{
				FormAnesthMedsEdit FormA = new FormAnesthMedsEdit();
				AnesthMedsInventory med = new AnesthMedsInventory();
				med.IsNew = true;
				FormA.ShowDialog();
				return;
			}
			else
			{
				MessageBox.Show(Lan.g(this, "You must be an administrator to unlock this action"));
				return;
			} 

		}
	}
}