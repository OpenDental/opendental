using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Xml;

namespace OpenDentBusiness {
	
	public class ODDataSet {
		public ODDataTableCollection Tables;

		public ODDataSet(string xmlData){
			Tables=new ODDataTableCollection();
			XmlDocument doc=new XmlDocument();
			doc.LoadXml(xmlData);
			ODDataTable currentTable=new ODDataTable();
			ODDataRow currentRow;
			XmlNodeList nodesRows=doc.DocumentElement.ChildNodes;
			for(int i=0;i<nodesRows.Count;i++) {
				currentRow=new ODDataRow();
				if(currentTable.Name=="") {
					currentTable.Name=nodesRows[i].Name;
				}
				else if(currentTable.Name!=nodesRows[i].Name) {
					this.Tables.Add(currentTable);
					currentTable=new ODDataTable();
					currentTable.Name=nodesRows[i].Name;
				}
				foreach(XmlNode nodeCell in nodesRows[i].ChildNodes) {
					currentRow.Add(nodeCell.Name,nodeCell.InnerXml);
				}
				currentTable.Rows.Add(currentRow);
			}
			this.Tables.Add(currentTable);
		}
	}

	///<summary></summary>
	public class ODDataTableCollection:System.Collections.ObjectModel.Collection<ODDataTable>{
		///<summary></summary>
		public ODDataTable this[string name]{
      get{
				foreach(ODDataTable table in this){
					if(table.Name==name){
						return table;
					}
				}
				ODDataTable tbl=new ODDataTable();
				tbl.Name=name;
				return tbl;
      }
		}
	}
	
	

	


}
