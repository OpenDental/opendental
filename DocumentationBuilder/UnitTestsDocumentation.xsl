<?xml version='1.0'?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:decimal-format NaN=" "/>
	<xsl:template match="members">
		<!--STYLES FROM EXISTING MANUAL PAGE STRIPPED DOWN AND MODIFIED-->
		<html>
			<script type="text/javascript" >
				<![CDATA[
		function refreshList() {
			var input, filter, table, tr, td, i;
			input=document.getElementById("filter");
			filter=input.value.toUpperCase();
			table=document.getElementById("TestTable");
			tr=table.getElementsByTagName("tr");
			//Start at 1 because of the table header
			for(i=1; i<tr.length; i++){
				tdName=tr[i].getElementsByTagName("td")[1];
				tdNum=tr[i].getElementsByTagName("td")[0];
				if(tdName){
					if(tdName.innerHTML.toUpperCase().indexOf(filter) > -1 || tdNum.innerHTML.toUpperCase().indexOf(filter) > -1){
						tr[i].style.display="";
					}
					else {
						tr[i].style.display = "none";
					}
				}
			}
		}
		]]>
			</script>
			<head>
				<style type="text/css">
					
					.TempCursor {/*Used when editing pages to show position*/
					background-color: rgb(255,255,150);
					}

					a:visited {
					color: rgb(85, 26, 139);
					}

					a:hover {
					color: #CC0000;
					}

					.splitLeft {
					width: 720px;
					z-index: 1;
					top: 0;
					left: 0;
					display: inline-block;
					}

					.splitRight {
					width: 800px;
					z-index: 1;
					top: 0;
					display: inline-block;
					background-color: #FFFFFF;
					background-repeat: repeat-x;
					min-width: min-content;
					}

					.splitRight td {
					text-align:center;
					padding: 2px;
					word-wrap: break-word;
					}

					.splitRight h1 {
					margin-left: 20px;
					margin-top: 0px;
					}

					.splitLeft table, th, td {
					border: 1px solid black;
					border-collapse: collapse;
					margin-left:5px;
					margin-bottom:5px;
					margin-top:5px;
					font-family: Arial, Helvetica, sans-serif;
					font-size: 11pt;
					}

					.splitRight table, th, td {
					border: 1px solid black;
					border-collapse: collapse;
					font-family: Arial, Helvetica, sans-serif;
					font-size: 11pt;
					}

					.splitRight table {
					margin-top: 5px;
					margin-bottom: 5px;
					}

					.testDataDiv {
					padding-bottom: 25px;
					margin-left: 20px;
					}

					.splitLeft img {
					border:0;
					margin-bottom:8px;
					background-color: #FFFFFF;
					}

					.summary {
					margin-left:20;
					margin-right:20;
					margin-top:20;
					margin-bottom:20;
					}

					.splitLeft table{
					width:676;
					}

					a {
					color: #343C83;
					/*text-decoration: underline;*/
					}

					body {
					font-family: Arial, Helvetica, sans-serif;
					font-size: 11pt;
					}

					nav {
					display: inline-block;
					position: fixed;
					top:0;
					bottom:0;
					Left:0;
					Overflow-x:scroll;
					overflow-y:scroll;
					background-color: #FeFeFe;
					width:700;
					}

					p {
					margin-bottom: 0px;
					}

					ul, ol {
					margin-top: 0px;
					}

					a {
					margin-left: auto;
					margin-right: auto;
					}

					h1 {
					color:#00648A;
					}

					h2 {
					color:#00648A;
					font-size: 20;
					margin:0;
					}

					input{
					margin-left:20;
					}
				</style>
				<title>Open Dental Software Manual: Unit Test Resources</title>
			</head>
			<body style="min-width: 1775px">
				<div class="splitLeft">
					<nav>
						<a href="/">
							<img src="../images/logos/logo.png" alt="Home"></img>
						</a>
						<br></br>
						<input type="text" id="filter" placeholder="Filter Tests..." onkeyup="refreshList()"></input>
						<table id="TestTable">
							<th bgcolor="#A5B7C9" style="width:19;"> #</th>
							<th bgcolor="#A5B7C9">Unit Tests</th>
							<xsl:for-each select="UnitTest">
								<xsl:sort select="@Name"/>
								<tr style="background-color: #FFFFFF">
									<td style="text-align:center">
										<xsl:value-of select="TestNum"/>
									</td>
									<td>
										<a>
											<xsl:attribute name="href">
												#<xsl:value-of select="@Name"/>
											</xsl:attribute>
											<xsl:value-of select="@Name"/>
										</a>
									</td>
								</tr>
							</xsl:for-each>
						</table>
					</nav>
				</div>
				<div class="splitRight">
					<br></br>
					<h1>
						Unit Testing
					</h1>
					<div class="summary">
						Unit testing, aka regression testing, is a programming technique to ensure that the behavior of a complex program does not change between versions. All unit tests are run automatically before each version release. The number of unit tests grows with each version. Every time we encounter a bug in the math, we first add a unit test (that fails), and then fix the program to make the unit test pass. Existing unit tests are never changed. Instead, new tests are added and obsolete ones deprecated. This list is not exhaustive.  There are many unit tests that have not been published here for one reason or another.
					</div>
					<xsl:for-each select="UnitTest" >
						<div class="testDataDiv">
							<xsl:attribute name="ID">
								<xsl:value-of select="@Name"/>
							</xsl:attribute>
							<h2>
								Test number <xsl:value-of select="TestNum"/>:  <xsl:value-of select="@Name"/>
							</h2>
							<xsl:if test="VersionAdded!=''">
								<div>
									Added in: v. <xsl:value-of select="VersionAdded"/>
								</div>
							</xsl:if>
							<xsl:if test="Description!=''">
								<div>
									<xsl:copy-of select="Description"/>
								</div>
							</xsl:if>
						</div>
					</xsl:for-each>
				</div>
				<br>
					<br>
						<br></br>
					</br>
				</br>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>
