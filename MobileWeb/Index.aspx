<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="MobileWeb.Index" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
	<head  runat="server">
		<title>Open Dental Mobile</title>
		<% if (HttpContext.Current.IsDebuggingEnabled) { %>
		<link type="text/css" rel="stylesheet" media="screen" href="css/themes/apple/theme.css?v=<%Response.Write(""+random.Next());%>" />
		<link type="text/css" rel="stylesheet" media="screen" href="css/jqtouch.css?v=<%Response.Write(""+random.Next());%>" />
		<link type="text/css" rel="stylesheet" media="screen" href="css/jqueryui/jquery.ui.all.css?v=<%Response.Write(""+random.Next());%>" />
		<link type="text/css" rel="stylesheet" media="screen" href="css/iphone.css?v=<%Response.Write(""+random.Next());%>" />
		<script type="text/javascript" src="scripts/jquery.js?v=<%Response.Write(""+random.Next());%>"></script>
		<script type="text/javascript" src="scripts/jqtouch.js?v=<%Response.Write(""+random.Next());%>"></script>
		<script type="text/javascript" src="scripts/jqueryui/jquery.ui.core.js?v=<%Response.Write(""+random.Next());%>"></script>
		<script type="text/javascript" src="scripts/jqueryui/jquery.ui.datepicker.js?v=<%Response.Write(""+random.Next());%>"></script>
		<script type="text/javascript" src="scripts/iphone.js?v=<%Response.Write(""+random.Next());%>"></script>
		<% } else { %>
		<link type="text/css" rel="stylesheet" media="screen" href="css/themes/apple/theme.min.css?v=<%Response.Write(""+random.Next());%>" />
		<link type="text/css" rel="stylesheet" media="screen" href="css/jqtouch.css?v=<%Response.Write(""+random.Next());%>" /><%--no minified version for this file--%>
		<link type="text/css" rel="stylesheet" media="screen" href="css/iphone.min.css?v=<%Response.Write(""+random.Next());%>" />
		<link type="text/css" rel="stylesheet" media="screen" href="css/jqueryui/jquery.ui.all.min.css?v=<%Response.Write(""+random.Next());%>" />
		<script type="text/javascript" src="scripts/jquery.min.js?v=<%Response.Write(""+random.Next());%>"></script>
		<script type="text/javascript" src="scripts/jqtouch.min.js?v=<%Response.Write(""+random.Next());%>"></script>
		<script type="text/javascript" src="scripts/jqueryui/jquery.ui.core.min.js?v=<%Response.Write(""+random.Next());%>"></script>
		<script type="text/javascript" src="scripts/jqueryui/jquery.ui.datepicker.min.js?v=<%Response.Write(""+random.Next());%>"></script>
		<script type="text/javascript" src="scripts/iphone.min.js?v=<%Response.Write(""+random.Next());%>"></script>
		<% } %>
		<script type="text/javascript">
			/*Dennis: the default slide animation is disabled on anchor tags with arrowless style and id searchbutton*/
			var jQT = $.jQTouch({
				icon: 'Mob.png',
				statusBar: 'black',
				slideSelector: 'body > * > ul li a:not(.arrowless, #searchbutton)'
			});
		</script>
	</head>
	<body>

	<div id="login">
	<div class="toolbar">
				<h1>Open Dental</h1>
			</div>
			<br />
			<span style="margin-left:15px;text-align:center;">For a demo, use the User name:<span style="color:Blue">demo</span></span>
			<br /><br />
			<form id="form1" method="post" runat="server">
			<span class="style1" style="font-weight:bold;position:relative;left:15px;">User name</span><br />
				<ul style="margin-top:4px">
					<li class="normalheight"><asp:TextBox placeholder="" ID="username" runat="server"></asp:TextBox></li>
				</ul>
				<span class="style1" style="font-weight:bold;position:relative;left:15px;">Password</span>
				<ul style="margin-top:4px">
					<li class="normalheight"><input type="password" placeholder="" name="password" id="password" autocapitalize="off" autocorrect="off" autocomplete="off" /></li>
				</ul>
				<div style="margin-left:15px;margin-bottom:10px">  
					<asp:CheckBox ID="rememberusername" title="Remember username" runat="server" /><span class="style1" style="margin-left:15px;font-weight:bold;position:relative;top:0px;left:0px;">Remember username</span>
				</div>	
				<ul class="rounded narrowul">
					<li><a id="loginbutton" class="arrowless" href="#">Login</a></li>
				</ul>
				 <div class="styleError" style="margin-left:15px;">  
				 <asp:Label ID="LabelMessage" runat="server" Text="" ForeColor="Red"></asp:Label>
				 </div>	
			</form>
		</div>

		<div id="logout">
			<div class="toolbar">
			<h1>Open Dental</h1>
			</div>
			<div style="height: 100px">
			</div>
			<div id="logoutmessage">
			<div class="style1" style="font-weight:bold;text-align:center;">You have been logged out.</div>
			</div>
				<ul class="rounded narrowul">
					<li><a href="#login">Login</a></li>
				</ul>
			
		</div>

		<div id="home">
			<div class="toolbar">
				<h1>Home</h1>
				<a class="button logout" href="#">Logout</a>
			</div>
			<div style="height: 70px">
			</div>
			
			<ul class="rounded narrowul">
				<li><a class="arrowless" linkattib="AppointmentList.aspx" href="#AppointmentList">Appointments</a></li>
			</ul>
<%--			<ul class="rounded narrowul">
				<li><a class="arrowless" linkattib="AppointmentImage.aspx" href="#AppointmentImage">Appointments <br />(graphical)</a></li>
			</ul>--%>
			<ul class="rounded narrowul">
				<li><a class="arrowless" linkattib="PatientList.aspx" href="#PatientList">Patients</a></li>
			</ul>
			<ul class="rounded narrowul">
				<li><a class="arrowless" linkattib="PharmacyList.aspx" href="#PharmacyList">Pharmacies</a></li>
			</ul>
		</div>

		<div id="AppointmentList">
			<div class="toolbar">
				<h1>Appointments</h1>
				<a class="home" href="#">Home</a>
				<a class="button logout" href="#">Logout</a>
			</div>
			<div id="AppointmentListContents">
			</div>
		</div>

		<div id="AppointmentImage">
			<div class="toolbar" id="toolbarAppointmentImage" style="">
				<h1>Appointments</h1>
				<a class="home" href="#">Home</a>
				<a class="button logout" href="#">Logout</a>
			</div>
		
			<div id="AppointmentImageContents">
			</div>
		</div>

		 <div id="FilterPicker">
			<div class="toolbar">
				<h1>View</h1>
				<a class="appts" linkattib="AppointmentList.aspx" href="#">Appts</a>
				<a class="button logout" href="#">Logout</a>
			</div>
				<div id="datepicker"></div>
			<div id="FilterPickerContents">
			</div>
		</div>

		<div id="PatientList">
			<div class="toolbar">
				<h1>Patients</h1>
				<a class="home" href="#">Home</a>
				<a class="button logout" href="#">Logout</a>
			</div>
			 <ul style="width:71%; display:inline-block;">
				<li class="normalheight">
				<input type="text" placeholder="Search Patient" name="searchpatientbox" id="searchpatientbox" autocapitalize="off" autocorrect="off" autocomplete="off" />
				</li>
			</ul>
			<a class="button" id="searchbutton" href="#">Search</a>
				
			<div id="PatientListContents">
			 </div>
		</div>

		<div id="PharmacyList">
		<div class="toolbar">
			<h1>Pharmacies</h1>
			<a class="home" href="#">Home</a>
			<a class="button logout" href="#">Logout</a>
		</div>
		<div id="PharmacyListContents">
		</div>
	</div>

   <div id="PatientDetails">
		<div class="toolbar">
		<h1>Patient</h1>
		<a class="patients" linkattib="PatientList.aspx" href="#">Patients</a>
		<a class="button logout" href="#">Logout</a>
	</div>
   <div id="PatientDetailsContents">
   </div>
   </div>

   
   <div id="AppointmentDetails">
		<div class="toolbar">
		<h1>Appointment</h1>
		<a class="appts" linkattib="AppointmentList.aspx" href="#">Appts</a>
		<a class="button logout" href="#">Logout</a>
		</div>
	   <div id="AppointmentDetailsContents">
	   </div>
   </div>

	  <div id="PharmacyDetails">
		<div class="toolbar">
		<h1>Pharmacy</h1>
		<a class="pharmacies" linkattib="PharmacyList.aspx" href="#">Pharmacies</a>
		<a class="button logout" href="#">Logout</a>
		</div>
	   <div id="PharmacyDetailsContents">
	   </div>
   </div>
	</body>
</html>
