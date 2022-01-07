using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class UserControlJobVersionStatistics:UserControl {
		public UserControlJobVersionStatistics() {
			InitializeComponent();
		}

		public void SetValues(double percentJobTime,double averageDifference,double averageEstimate
			,double averageActual,int numberJobsCompleted,int totalHoursMinusBreaks) 
		{
			textPercentEngineeringTimeHead.Text=percentJobTime.ToString();
			textAverageDifferenceHead.Text=averageDifference.ToString();
			textAverageEstimatesHead.Text=averageEstimate.ToString();
			textAverageActualHead.Text=averageActual.ToString();
			textNumberOfJobsHead.Text=numberJobsCompleted.ToString();
			textEngineeringHoursMinusBreaksHead.Text=totalHoursMinusBreaks.ToString();
		}
	}
}
