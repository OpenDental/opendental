<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:decimal-format NaN=" " />
	<xsl:template match="members">
		<html>
			<script type="text/javascript">
			<![CDATA[
			window.onload=function() {
				colorDeprecatedRows();
				createAnchors();
			}

			function colorDeprecatedRows() {
				let rows=document.getElementsByTagName("tr");
				for(let i=1;i<rows.length;i++) {
					let tdSummary=rows[i].getElementsByTagName("td")[1];
					if(tdSummary.textContent!==null && tdSummary.textContent.toLowerCase().includes("deprecated")) {
						rows[i].style.color="#999999";
					}
				}
			}

			function createAnchors() {
				const replacements=[
				["&amp;lt;","<"],
				["&amp;gt;",'>'],
				];
				let rows=document.getElementsByTagName("tr");
				for(let row of rows) {
				let tdDetails=row.getElementsByTagName("td")[2];
				if(!tdDetails) { // falsey, checks for table header row which will be undefined
					continue;
				}
				let stringData=tdDetails.innerHTML;
				replacements.forEach((replacement) => {
					let regex=new RegExp(replacement[0],'g');
					stringData=stringData.replace(regex,replacement[1]);
				});
				tdDetails.innerHTML=stringData;
			  }
			}
			]]>
			</script>
			<head>
				<style type="text/css">
					body {
						font-size: 1rem;
					}

					.mainTable td {
						text-align:left;
						padding: 1px;
						word-wrap: break-word;
					}

					h2 {
						margin-left: 20px;
						margin-top: 0px;
						margin-bottom: 10px;
						font-size: 1.66rem;
						color:#00648A;
					}

					p {
						margin-left: 20px;
						margin-top: 0px;
						margin-bottom: 10px;
						width: 1200px;
						font-size: 0.875rem;
					}

					table, th, td {
						border: 1px solid black;
						border-collapse: collapse;
						font-family: Arial, Helvetica, sans-serif;
					}

					table {
						font-size: 0.875rem;
						table-layout:fixed;
						text-align:Center;
						padding: 1px;
						margin-left: 20px;
					}

					.header {
						font-family: Arial, Helvetica, sans-serif;
					}

					span {
						font-weight: normal;
					}

					a:hover {
						color: #CC0000;
					}

				</style>
				<title>Preference Resources</title>
			</head>
			<body>
				<div class="main">
					<div class="header">
						<h2>Preferences</h2>
						<p>This table provides extra information regarding the preferences found in Open Dental. There are over 1,000 entries in this table, so it is recommended to use CTRL-F to quickly find the preference you are looking for.</p>
					</div>
					<table class="mainTable" width="1200px">
						<theader>
							<tr bgcolor="#D0D1D2">
								<th width="296">Name</th>
								<th width="450">
									Summary<br></br><span>(This information comes from our internal C# documentation. Some of it may be confusing or outdated.)</span>
								</th>
								<th width="450">Details</th>
							</tr>
						</theader>
						<tbody>
							<xsl:for-each select="Preference">
								<tr bgcolor="#F1F4F8">
									<td>
										<xsl:value-of select="PrefName"/>
									</td>
									<td>
										<xsl:value-of select="Summary"/>
									</td>
									<td>
										<xsl:value-of select="Details"/>
									</td>
								</tr>
							</xsl:for-each>
						</tbody>
					</table>
				</div>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>