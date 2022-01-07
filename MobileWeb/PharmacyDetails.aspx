<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PharmacyDetails.aspx.cs" Inherits="MobileWeb.PharmacyDetails" %>
<%@ Import namespace="OpenDentBusiness.Mobile" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
</head>
<body>
	<div id="loggedin"><asp:Literal runat="server" ID="Message"></asp:Literal></div>
	<div id="content">
	<div class="styleError">  
				 <asp:Label ID="LabelError" runat="server" Text=""></asp:Label>
</div>

<ul class="contact">
<li><span class="style1"><%Response.Write(phar.StoreName);%>
</span></li>
	<li class="style1">
		<div>
			<div class="leftcolumn">
			Address
			</div>
			<div class="coloncolumn">:</div>
			<div class="rightcolumn">
					<div style="word-wrap:break-word;white-space:normal;"><%Response.Write(phar.Address);%></div>
					<%if(!String.IsNullOrEmpty(phar.Address2)) {%>
					<div class="info"><%Response.Write(phar.Address2);%></div>
					<%}%>
					<div class="infocolumn"><%Response.Write(phar.City);%></div>
					<div class="infocolumn"><%Response.Write(phar.State);%></div>
			</div>
			<div style='clear:both'></div>								
		</div>			
	</li>
	<li class="style1">
		<div class="leftcolumn">Phone</div>
		<div class="coloncolumn">:</div>
		<div class="rightcolumn"><%Response.Write(phar.Phone);%> <%Response.Write(DialLinkPhone);%></div>
	</li>
	<li class="style1">
		<div class="leftcolumn">Fax </div>
		<div class="coloncolumn">:</div>
		<div class="rightcolumn"><%Response.Write(phar.Fax);%></div>
	</li>
	<li class="style1">
		<div class="leftcolumn">Note </div>
		<div class="coloncolumn">:</div>
		<div class="rightcolumn">
			<div style="word-wrap:break-word;white-space:normal;"><%Response.Write(phar.Note);%></div>
		</div>
	</li>
</ul>
</div>
</body>
</html>
