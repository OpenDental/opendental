using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfControls.UI {
	public class MenuItem : System.Windows.Controls.MenuItem{
		public MenuItem(){
			VerticalAlignment=VerticalAlignment.Stretch;
		}

		public MenuItem(string text){
			VerticalAlignment=VerticalAlignment.Stretch;
			Header=text;
		}

		public MenuItem(string text,EventHandler click){
			VerticalAlignment=VerticalAlignment.Stretch;
			Header=text;
			Click+=(sender,routedEventArgs)=>click?.Invoke(sender,new EventArgs());
		}

		public void Add(MenuItem menuItem){
			Items.Add(menuItem);
		}

		public void Add(string text,EventHandler click){
			MenuItem menuItem=new MenuItem(text,click);
			Items.Add(menuItem);
		}

		public void AddSeparator(){
			Items.Add(new System.Windows.Controls.Separator());
		}

	}
}
