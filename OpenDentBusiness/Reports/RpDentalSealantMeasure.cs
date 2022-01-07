using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace OpenDentBusiness {
	public class RpDentalSealantMeasure {
		public static DataTable GetDentalSealantMeasureTable(string year) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),year);
			}
			string command=@"SET @ReportingDateStart = CONCAT("+year+@",'-','01','-','01'), @ReportingDateEnd = CONCAT("+year+@",'-','12','-','31');
				SET @PatientDOBStart = @ReportingDateStart - INTERVAL 9 YEAR, @PatientDOBEnd = @ReportingDateEnd - INTERVAL 6 YEAR;
				SELECT provider.LName AS 'Provider', 
				COUNT(ColC.NumeratorPat) AS 'Numerator',
				COUNT(ColA.DenominatorPat) AS 'Denominator',
				FORMAT(ROUND((COALESCE(COUNT(ColC.NumeratorPat)/COUNT(ColA.DenominatorPat),0))*100,2),2) AS 'Percentage Met'
				FROM (	#exclusions/exceptions: any patient that should be excluded from the denominator.
					/*EXCLUSIONS/EXCEPTIONS*/
					SELECT COALESCE(denom.PatNum,0) AS DenominatorPat,denom.ProvNum
					FROM (	#Denominator: count initial population who have at least one of a list of specific Procedure/SnoMed codes.
						/*DENOMINATOR*/
						SELECT DISTINCT initial.PatNum, initial.ProvNum
						FROM (	#initial population: any patients with birthdate between 2006-01-01 and 2009-12-31 with one of a list of specific procedure/medical codes.
							#note: patients that fulfill multiple criterion will be duplicated in this list, but this will not affect the denominator results.
							/*INITIAL POPULATION*/
							SELECT procedurelog.ProvNum,
							patient.PatNum,
							procedurelog.MedicalCode, 
							procedurecode.ProcCode
							FROM patient
							INNER JOIN procedurelog ON patient.PatNum = procedurelog.PatNum
								AND procedurelog.ProcDate BETWEEN @ReportingDateStart AND @ReportingDateEnd
								AND procedurelog.ProcStatus = 2 #complete
							LEFT JOIN procedurecode ON procedurecode.CodeNum = procedurelog.CodeNum
							WHERE patient.Birthdate BETWEEN @PatientDOBStart AND @PatientDOBEnd
							HAVING procedurelog.MedicalCode IN ('99201','99202','99203','99204','99205','99212','99213','99214','99215','99391','99392','99393','99394','99381','99382','99383','99384','99395','99385')
								OR procedurecode.ProcCode IN ('D0120','D0145','D0150','D0180','D0191','D0140','D0160','D0170')
							/*END INITIAL POPULATION*/
						)initial
						INNER JOIN procedurelog ON initial.PatNum = procedurelog.PatNum
							AND procedurelog.ProcDate BETWEEN @ReportingDateStart AND @ReportingDateEnd
							AND procedurelog.ProcStatus = 2
						INNER JOIN procedurecode ON procedurelog.CodeNum = procedurecode.CodeNum
						WHERE initial.ProcCode IN ('D0120','D0145','D0150','D0180','D0191')
							AND (procedurecode.ProcCode IN ('D0602','D0603') OR procedurelog.SnomedBodySite IN ('609399004','609401005','609402003','609403008'))
						/*END DENOMINATOR*/
					)denom
					WHERE denom.PatNum NOT IN 
					(/*NOT IN*/
						SELECT A.PatNum FROM (
							SELECT procedurelog.PatNum, COUNT(DISTINCT procedurelog.ToothNum) AS ToothNums
							FROM procedurelog
							INNER JOIN procedurecode ON procedurelog.CodeNum = procedurecode.CodeNum
							WHERE procedurelog.ProcStatus = 2
								AND procedurelog.ProcDate BETWEEN 
								(CASE WHEN (procedurelog.DiagnosticCode IN ('520.6','520.8','520.9')
									OR procedurelog.DiagnosticCode2 IN ('520.6','520.8','520.9')
									OR procedurelog.DiagnosticCode3 IN ('520.6','520.8','520.9')
									OR procedurelog.DiagnosticCode4 IN ('520.6','520.8','520.9')
									OR procedurelog.DiagnosticCode IN ('520.6','520.8','520.9')
									OR procedurelog.SnomedBodySite IN ('109467004','109468009','109542004',
								'109543009','109544003','234949000','234972003','266413002','278658009')) THEN @ReportingDateStart ELSE '0001-01-01' END)
								AND (CASE WHEN (procedurelog.SnomedBodySite = '234713009' 
									OR procedurecode.ProcCode = 'D1351') THEN @ReportingDateStart ELSE @ReportingDateEnd END)
								AND procedurelog.ToothNum IN (14,3,19,30)
								AND (procedurelog.DiagnosticCode IN ('520.6', '520.8', '520.9', 'K00.6',
								'520.3','521.00','521.09','521.0','521.02','521.03','522.0','522.5',
								'522.7','525.13','520.0','525.19','K00.3','K02','K02.3','K02.9',
								'K02.52','K02.53','K02.63','K04.0','K04.6','K04.7','K08.13','K08.43',
								'K08.131','K08.132','K08.133','K08.134','K08.139','K08.431',
								'K08.432','K08.433','K08.434','K08.439','K00.0','K08.1','K08.4'
								)
								OR procedurelog.DiagnosticCode2 IN ('520.6', '520.8', '520.9', 'K00.6',
								'520.3','521.00','521.09','521.0','521.02','521.03','522.0','522.5',
								'522.7','525.13','520.0','525.19','K00.3','K02','K02.3','K02.9',
								'K02.52','K02.53','K02.63','K04.0','K04.6','K04.7','K08.13','K08.43',
								'K08.131','K08.132','K08.133','K08.134','K08.139','K08.431',
								'K08.432','K08.433','K08.434','K08.439','K00.0','K08.1','K08.4'
								)
								OR procedurelog.DiagnosticCode3 IN ('520.6', '520.8', '520.9', 'K00.6',
								'520.3','521.00','521.09','521.0','521.02','521.03','522.0','522.5',
								'522.7','525.13','520.0','525.19','K00.3','K02','K02.3','K02.9',
								'K02.52','K02.53','K02.63','K04.0','K04.6','K04.7','K08.13','K08.43',
								'K08.131','K08.132','K08.133','K08.134','K08.139','K08.431',
								'K08.432','K08.433','K08.434','K08.439','K00.0','K08.1','K08.4'
								)
								OR procedurelog.DiagnosticCode4 IN ('520.6', '520.8', '520.9', 'K00.6',
								'520.3','521.00','521.09','521.0','521.02','521.03','522.0','522.5',
								'522.7','525.13','520.0','525.19','K00.3','K02','K02.3','K02.9',
								'K02.52','K02.53','K02.63','K04.0','K04.6','K04.7','K08.13','K08.43',
								'K08.131','K08.132','K08.133','K08.134','K08.139','K08.431',
								'K08.432','K08.433','K08.434','K08.439','K00.0','K08.1','K08.4'
								)
								OR procedurelog.SnomedBodySite IN ('109467004','109468009','109542004',
								'109543009','109544003','234949000','234972003','266413002','278658009',
								'234713009','2955000','4237001','5494004','25840002','32620007',
								'42711005','44385007','44758003','65413006','75037000','76719002',
								'80753001','80967001','84752003','95247003','95249000','95252008',
								'95253003','95254009','109568006','109569003','109571003','109574006',
								'109575007','109577004','109591005','109599007','109600005','109752002',
								'109753007','163152009','196305005','370484007','387665005','442231009',
								'442551007','16958000','26624006','37320007','64969001','109442002',
								'109674000','234948008','109727004','366241004'
								)
								OR procedurecode.ProcCode IN ('D1352','D2140','D2150','D2160','D2161',
								'D2335','D2391','D2392','D2393','D2394','D2410','D2420','D2430',
								'D2510','D2520','D2530','D2542','D2543','D2544','D2610','D2620',
								'D2630','D2642','D2643','D2644','D2650','D2651','D2652','D2662',
								'D2663','D2664','D2710','D2712','D2720','D2721','D2722','D2740',
								'D2750','D2751','D2752','D2780','D2781','D2782','D2783','D2790',
								'D2791','D2792','D2794','D2799','D2910','D2931','D2932','D2933',
								'D2940','D2970','D2980','D1351'
								)
							)
							GROUP BY procedurelog.PatNum
						)A
						WHERE A.ToothNums = 4
					)/*END NOT IN*/
					/*END EXCLUSIONS/EXCEPTIONS*/
				)ColA
				LEFT JOIN (
					SELECT Numer.PatNum AS NumeratorPat FROM (
						SELECT procedurelog.PatNum, COUNT(DISTINCT procedurelog.ToothNum) AS ToothNums
						FROM procedurelog
						INNER JOIN procedurecode ON procedurelog.CodeNum = procedurecode.CodeNum
						WHERE procedurelog.ProcStatus = 2
						AND procedurelog.ProcDate BETWEEN @ReportingDateStart  AND @ReportingDateEnd
						AND procedurelog.ToothNum IN (14,3,19,30)
						AND (procedurelog.SnomedBodySite = '234713009' OR procedurecode.ProcCode = 'D1351')
						GROUP BY procedurelog.PatNum
					)Numer
				)ColC ON ColC.NumeratorPat = ColA.DenominatorPat
				INNER JOIN provider ON provider.ProvNum = ColA.ProvNum
				GROUP BY LName";
			return ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(command));

		}	
	}

}
