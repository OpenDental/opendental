using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenDental;
using OpenDental.InternalTools.Phones;

namespace UnitTests {
	public partial class FormEscalationViewTests:FormODBase {
		public FormEscalationViewTests() {
			InitializeComponent();
			InitializeLayoutManager();
			//LayoutManager.ZoomTest=50;
		}

		private void FormEscalationViewTests_Load(object sender,EventArgs e) {
			escalationView.BeginUpdate();
			escalationView.Items.Clear();
			escalationView.DictProximity.Clear();
			escalationView.DictShowExtension.Clear();
			escalationView.DictExtensions.Clear();
			escalationView.DictWebChat.Clear();
			escalationView.DictGTAChat.Clear();
			escalationView.DictRemoteSupport.Clear();
			escalationView.Items.Add("Item1");
			escalationView.Items.Add("Item2");
			escalationView.DictShowExtension.Add("Item2",true);
			escalationView.DictExtensions.Add("Item2",1230);
			escalationView.Items.Add("Item3");
			escalationView.DictProximity.Add("Item3",true);
			escalationView.Items.Add("Item4");
			escalationView.DictWebChat.Add("Item4",true);
			escalationView.Items.Add("Item5");
			escalationView.DictGTAChat.Add("Item5",true);
			escalationView.Items.Add("Item6");
			escalationView.DictRemoteSupport.Add("Item6",true);
			escalationView.Items.Add("Item7");
			escalationView.DictShowExtension.Add("Item7",true);
			escalationView.DictExtensions.Add("Item7",1230);
			escalationView.DictProximity.Add("Item7",true);
			escalationView.DictWebChat.Add("Item7",true);
			escalationView.Items.Add("BenjaminH");
			escalationView.DictWebChat.Add("BenjaminH",true);
			escalationView.EndUpdate();
			escalationView2.FadeAlphaIncrement=20;
			escalationView2.BeginUpdate();
			List<EscalationItem> listEscalationItems=new List<EscalationItem>();
			EscalationItem escalationItem=new EscalationItem();
			escalationItem.EmpName="Item1";
			listEscalationItems.Add(escalationItem);
			escalationItem=new EscalationItem();
			escalationItem.EmpName="Item2";
			escalationItem.ShowExtension=true;
			escalationItem.Extension=1230;
			listEscalationItems.Add(escalationItem);
			escalationItem=new EscalationItem();
			escalationItem.EmpName="Item3";
			escalationItem.IsProximity=true;
			listEscalationItems.Add(escalationItem);
			escalationItem=new EscalationItem();
			escalationItem.EmpName="Item4";
			escalationItem.IsWebChat=true;
			listEscalationItems.Add(escalationItem);
			escalationItem=new EscalationItem();
			escalationItem.EmpName="Item5";
			escalationItem.IsGTAChat=true;
			listEscalationItems.Add(escalationItem);
			escalationItem=new EscalationItem();
			escalationItem.EmpName="Item6";
			escalationItem.IsRemoteSupport=true;
			listEscalationItems.Add(escalationItem);
			escalationItem=new EscalationItem();
			escalationItem.EmpName="Item7";
			escalationItem.ShowExtension=true;
			escalationItem.Extension=1230;
			escalationItem.IsProximity=true;
			escalationItem.IsWebChat=true;
			listEscalationItems.Add(escalationItem);
			escalationItem=new EscalationItem();
			escalationItem.EmpName="BenjaminH";
			escalationItem.IsWebChat=true;
			listEscalationItems.Add(escalationItem);
			escalationView2.ListEscalationItems=listEscalationItems;
			escalationView2.EndUpdate();
		}

	}
}
