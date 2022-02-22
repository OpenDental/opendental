using CodeBase;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace OpenDentBusiness {
	///<summary>This table is not part of the general release.  User would have to add it manually.
	///Most schema changes are done directly on our live database as needed.  Keeps track of sprints for versions.</summary>
	[Serializable]
	[CrudTable(IsMissingInGeneral=true)]
	public class JobSprint:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey = true)]
		public long JobSprintNum;
		///<summary>Main description of the sprint. Usually a version.</summary>
		public string Title;
		///<summary>Optional note.</summary>
		public string Note;
		///<summary>Start date for the sprint.</summary>
		public DateTime DateStart;
		///<summary>Target end date for the sprint. Used to calculate the allocatable hours.</summary>
		public DateTime DateEndTarget;
		///<summary>Actual date the sprint was finished. Mainly used for historical purposes. Usually a release date of a version.</summary>
		public DateTime DateEndActual;
		///<summary>Percent of engineering time spent on Jobs. Used to calculate allocated hours</summary>
		public double JobPercent;
		///<summary>Average hours spent on jobs. Used when a job has no estimates</summary>
		public double HoursAverageDevelopment;
		///<summary>Actual date the sprint was finished. Mainly used for historical purposes. Usually a release date of a version.</summary>
		public double HoursAverageBreak;
	}
}


