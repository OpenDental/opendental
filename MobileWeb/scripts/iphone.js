/*
$(document).ready(function () {
    loadPage();
});
function loadPage(url) {
    if (url==undefined) {
        $('#container').load('index.html #header ul', hijackLinks);
    } else {
        $('#container').load(url+' #content', hijackLinks);
    }
}
function hijackLinks() {
    $('#container a').click(function (e) {
        e.preventDefault();
        loadPage(e.target.href);
    });
}

*/
var MessageLoad='<div id="progress"><p>&nbsp;</p><p>Loading...</p><p>&nbsp;</p></div>';
var MessageLoadLogout='<div><div id="progresslogout"><p>&nbsp;</p><p>Logging out...</p><p>&nbsp;</p></div></div>';
var MessageError='<div class="styleError">There has been an error while processing your page. Please try again.<br />If the error persists, please refresh this page using the browser address bar and try again.</div>';
var AppType='';
$(document).ready(function () {
    TraversePage();
});

function TraversePage(){

    //console.log('in TraversePage');
   // window.scrollTo(0, 0); resizeTo(320, 480);
    //Password is retained on some browsers- so it's got to be erased
    $('#password').focus(function () {
        //alert('Handler for password.focus() called.');
        $('#password').val(''); //because password field tends to retain the keyed in password.
    });
    $('#password').click(function (e) {
        //alert('Handler for password.click() called.');
        $('#password').val(''); //because password field tends to retain the keyed in password.
    });

    $('#password').tap(function (e) {
        //alert('Handler for password.tap() called.'); 
        $('#password').val(''); //because password field tends to retain the keyed in password.
    });

    //Process Login
    $('#loginbutton').tap(function (e) { ProcessLogin(); });

	// Process Logout
	$('.button.logout').click(function (e) {ProcessLogout(e);}); 
	// this syntax is incorrect for a callback: $('.button.logout').click(ProcessLogout(e));
	
	// a click is used instead of tap because it gives an error with jQT.goTo(MoveToURL, 'slide') 'Not able to tap element' error.
	$('a[href="#AppointmentList"]').click(function (e) {
		//e.preventDefault();
		//console.log('AppointmentList clicked');
		AppType='AppList';
		var UrlForFetchingData=this.attributes["linkattib"].value;
		var SectionToFill='#AppointmentListContents';
		var MoveToURL='#AppointmentList';
		ProcessArrowlessPageLink(UrlForFetchingData, MoveToURL, SectionToFill);
	});

	$('a[href="#AppointmentImage"]').click(function (e) {
		AppType='AppImage';
    	var UrlForFetchingData=this.attributes["linkattib"].value;
    	var SectionToFill='#AppointmentImageContents';
    	var MoveToURL='#AppointmentImage';
    	ProcessArrowlessPageLink(UrlForFetchingData, MoveToURL, SectionToFill);
    });


    $('a[href="#PharmacyList"]').click(function (e) {
        //e.preventDefault();
        //console.log('PharmacyList clicked');
        var UrlForFetchingData=this.attributes["linkattib"].value;
        var SectionToFill='#PharmacyListContents';
        var MoveToURL='#PharmacyList';
        ProcessArrowlessPageLink(UrlForFetchingData, MoveToURL, SectionToFill);
    });

    $('a[href="#PatientList"]').tap(function (e) {
        //e.preventDefault();
        //console.log('PatientList clicked');
        var UrlForFetchingData=this.attributes["linkattib"].value;
        var SectionToFill='#PatientListContents';
        var MoveToURL='#PatientList';
        ProcessArrowlessPageLink(UrlForFetchingData, MoveToURL, SectionToFill);
        $('#searchpatientbox').val('');
    });
	
	$('#searchbutton').tap(function(e) {
		var searchterm=$('#searchpatientbox').val();
		//console.log('searchterm is dd'+searchterm);
		var UrlForFetchingData='PatientList.aspx?searchterm='+searchterm; 
		var SectionToFill='#PatientListContents';
		ProcessPreviousNextButton(e, UrlForFetchingData, SectionToFill);
	});

	// a tap function is used instead of .live() for elements loaded by AJAX
	// here the tap does not give an error with jQT.goTo(MoveToURL, 'slide')
	$('a[href="#AppointmentDetails"]').tap(function(e) {
		//console.log('AppointmentDetails tapped');
		var UrlForFetchingData=this.attributes["linkattib"].value; 
		var SectionToFill='#AppointmentDetailsContents';
		var MoveToURL='#AppointmentDetails';
		ProcessNormalPageLink(e,UrlForFetchingData, MoveToURL, SectionToFill);
	});

	$('a[href="#PatientDetails"]').tap(function(e) {
		//console.log('PatientDetails tapped');
		var UrlForFetchingData=this.attributes["linkattib"].value; 
		var SectionToFill='#PatientDetailsContents';
		var MoveToURL='#PatientDetails';
		ProcessNormalPageLink(e,UrlForFetchingData, MoveToURL, SectionToFill);
    });

    $('a[href="#PharmacyDetails"]').tap(function (e) {
        //console.log('PharmacyPatientDetails tapped');
        var UrlForFetchingData=this.attributes["linkattib"].value;
        var SectionToFill='#PharmacyDetailsContents';
        var MoveToURL='#PharmacyDetails';
        ProcessNormalPageLink(e, UrlForFetchingData, MoveToURL, SectionToFill);
    });
	
	/*previous, today and next buttons*/
	$('#previous').tap(function(e) {
	    //console.log('Previous button tapped');
	    var UrlForFetchingData=this.attributes["linkattib"].value;
	    var SectionToFill='#AppointmentListContents';
	    ProcessPreviousNextButton(e, UrlForFetchingData, SectionToFill);
	});

	$('#previousApImage').tap(function (e) {
	    //console.log('Previous button tapped');
	    var UrlForFetchingData=this.attributes["linkattib"].value;
	    var SectionToFill='#AppointmentImageContents';
	    ProcessPreviousNextButton(e, UrlForFetchingData, SectionToFill);
	});

	$('#datepickerbutton').tap(function (e) {
	    var UrlForFetchingData="AppointmentFilter.aspx";
	    var SectionToFill='#FilterPickerContents';
	    var MoveToURL='#FilterPicker';
	    ProcessNormalPageLink(e, UrlForFetchingData, MoveToURL, SectionToFill);
	    var DemoDateCookieY=parseInt(getCookie("DemoDateCookieY"));
	    var DemoDateCookieM=parseInt(getCookie("DemoDateCookieM"));
	    var DemoDateCookieD=parseInt(getCookie("DemoDateCookieD"));
	    if (DemoDateCookieY != null && DemoDateCookieY != "" && !isNaN(DemoDateCookieY)) {
	        //console.log('in this if statement '+DemoDateCookieY+" "+(DemoDateCookieM - 1)+" "+DemoDateCookieD);
	        $('#datepicker').datepicker("setDate", new Date(DemoDateCookieY, DemoDateCookieM - 1, DemoDateCookieD));
	    }

    });

	$('#next').tap(function (e) {
	    //console.log('Next button tapped');
	    var UrlForFetchingData=this.attributes["linkattib"].value;
	    var SectionToFill='#AppointmentListContents';
	    ProcessPreviousNextButton(e, UrlForFetchingData, SectionToFill);
	});

	$('#nextApImage').tap(function (e) {
		//console.log('Next button tapped');
		var UrlForFetchingData=this.attributes["linkattib"].value;
		var SectionToFill='#AppointmentImageContents';
		ProcessPreviousNextButton(e, UrlForFetchingData, SectionToFill);
	});
	
	/*home, appt, patient, pharmacies buttons*/
	$('.appts').tap(function (e) {
		//console.log('AppType='+AppType);
		var UrlForFetchingData='AppointmentList.aspx';
		var SectionToFill='#AppointmentListContents';
		var MoveToURL='#AppointmentList';
		var provnum=$("#provlist option:selected").val();
		if (AppType=='AppImage') {
			UrlForFetchingData='AppointmentImage.aspx?ProvNum='+provnum;
			SectionToFill='#AppointmentImageContents';
			MoveToURL='#AppointmentImage';
		} else {
			UrlForFetchingData='AppointmentList.aspx?ProvNum='+provnum;
		}
		ProcessReversePageLink(UrlForFetchingData, MoveToURL, SectionToFill);
		
	});
	
	$('.patients').tap(function(e) {
		//console.log('patients button tapped');
		var searchterm=$('#searchpatientbox').val();
		//console.log('searchterm is '+searchterm);
		var UrlForFetchingData='PatientList.aspx?searchterm='+searchterm; 
		var SectionToFill='#PatientListContents';
		var MoveToURL='#PatientList'; 
		ProcessReversePageLink(UrlForFetchingData, MoveToURL, SectionToFill);
		//var today=new Date(); //console.log("in patients"+today);
    });

$('.pharmacies').tap(function (e) {
    //console.log('pharmacies button tapped');
    var UrlForFetchingData=this.attributes["linkattib"].value;
    var SectionToFill='#PharmacyListContents';
    var MoveToURL='#PharmacyList';
    ProcessReversePageLink(UrlForFetchingData, MoveToURL, SectionToFill);
});

    /*Dennis: this overrides the default behavior of clicking the today button in jqueryui*/
    var _gotoToday=jQuery.datepicker._gotoToday;
    // datepicker is directly inside the jQuery object, so override that
    jQuery.datepicker._gotoToday=function (a) {
        var target=jQuery(a);
        var inst=this._getInst(target[0]);
        var DemoDateCookieY=parseInt(getCookie("DemoDateCookieY"));
        var DemoDateCookieM=parseInt(getCookie("DemoDateCookieM"));
        var DemoDateCookieD=parseInt(getCookie("DemoDateCookieD"));
        if (DemoDateCookieY != null && DemoDateCookieY != "" && !isNaN(DemoDateCookieY)) {//for demo
            inst.selectedYear=DemoDateCookieY;
            inst.selectedMonth=DemoDateCookieM - 1;
            inst.selectedDay=DemoDateCookieD;
            //console.log("In here "+inst.selectedDay+" "+inst.selectedMonth+" "+inst.selectedYear);
        }
        else {//for all other cases
            var today=new Date();
            inst.selectedYear=today.getFullYear();
            inst.selectedMonth=today.getMonth();
            inst.selectedDay=today.getDate();
        }
        //console.log('today button tapped'+inst.selectedDay+" "+inst.selectedMonth+" "+inst.selectedYear);
        // Dennis: if the default behaviour of the "Today" button is needed, uncomment next line
        //_gotoToday.call(this, a);
        // now do an additional call to _selectDate which will set the date and close
        // close the datepicker (if it is not inline)
        jQuery.datepicker._selectDate(a,
        jQuery.datepicker._formatDate(inst, inst.selectedDay, inst.selectedMonth, inst.selectedYear));
    }

    function getCookie(c_name) {
        var i, x, y, ARRcookies=document.cookie.split(";");
        for (i=0; i < ARRcookies.length; i++) {
            x=ARRcookies[i].substr(0, ARRcookies[i].indexOf("="));
            y=ARRcookies[i].substr(ARRcookies[i].indexOf("=")+1);
            x=x.replace(/^\s+|\s+$/g, "");
            if (x==c_name) {
                return unescape(y);
            }
        }
    }
    /* this function is not used but is  kept for completeness sake*/
    function setCookie(c_name, value, exdays) {
        var exdate=new Date();
        exdate.setDate(exdate.getDate()+exdays);
        var c_value=escape(value)+((exdays==null) ? "" : "; expires="+exdate.toUTCString());
        document.cookie=c_name+"="+c_value;
    }

    $("#datepicker").datepicker({
    	onSelect: function (dateText, datePickerInstance) {
    		var SelectedDate=$.datepicker.parseDate('mm/dd/yy', dateText);
    		var d=SelectedDate.getDate();
    		var m=SelectedDate.getMonth()+1; //getMonth() return 0 to 11
    		var y=SelectedDate.getFullYear();
    		var UrlForFetchingData='AppointmentList.aspx';
    		var SectionToFill='#AppointmentListContents';
    		var MoveToURL='#AppointmentList';
    		var provnum=$("#provlist option:selected").val();
    		var param='year=' +y+'&month='+m+'&day='+d+'&ProvNum='+provnum
    		if (AppType=='AppImage') {
    			UrlForFetchingData='AppointmentImage.aspx?'+param;
    			SectionToFill='#AppointmentImageContents';
    			MoveToURL='#AppointmentImage';
    		}
			else {
				UrlForFetchingData='AppointmentList.aspx?'+param;
    		}
    		ProcessArrowlessPageLink(UrlForFetchingData, MoveToURL, SectionToFill);
    	},
    	showButtonPanel: true
    });

    $('.home').click(function (e) { // tap event logs out the user on ipod.
        jQT.goToReverse('#home', 'slide');
    });



} //end of TraversePage function


function ProcessNormalPageLink(e,UrlForFetchingData, MoveToURL, SectionToFill){
	e.preventDefault();
	//console.log(' UrlForFetchingData ='+UrlForFetchingData );
	$(SectionToFill).append(MessageLoad);
	// for newly loaded links this is null
	if(e.currentTarget.attributes==null){
	//console.log('in this if statement');
	jQT.goTo(MoveToURL,'slide'); //do not use this line with tap event, it gives a 'Not able to tap element' error.
	}
	FetchPage(UrlForFetchingData, SectionToFill)
}

function ProcessArrowlessPageLink(UrlForFetchingData, MoveToURL, SectionToFill){
	//e.preventDefault();
	//console.log(' UrlForFetchingData ='+UrlForFetchingData );
    $(SectionToFill).append(MessageLoad);
	//no slide effect
	jQT.goTo(MoveToURL,''); //do not use this line with tap event, it gives a 'Not able to tap element' error.
	FetchPage(UrlForFetchingData, SectionToFill)
}

function ProcessReversePageLink(UrlForFetchingData, MoveToURL, SectionToFill){
    $(SectionToFill).append(MessageLoad);
    jQT.goToReverse(MoveToURL, 'slide'); //var today=new Date(); //console.log("in ProcessReversePageLink 1a"+today);
    FetchPage(UrlForFetchingData, SectionToFill); //console.log("in ProcessReversePageLink 1b"+today);
}

function ProcessPreviousNextButton(e,UrlForFetchingData, SectionToFill){
	e.preventDefault();
	//console.log(' UrlForFetchingData ='+UrlForFetchingData );
	$(SectionToFill).append(MessageLoad);
	FetchPage(UrlForFetchingData, SectionToFill);
}



function FetchPage(UrlForFetchingData, SectionToFill){
    $.ajax({
		type: "GET",
		url: UrlForFetchingData,
		success: function (msg) {
			var $response=$(msg);
			var IsLoggedIn=$response.filter('#loggedin').text();
			//console.log('IsLoggedIn='+IsLoggedIn);
			var Content=$response.filter('#content').html();
			if(IsLoggedIn=='LoggedIn'){
				//console.log('still in session');
			    $(SectionToFill).html(Content);
			}else{
			   	//console.log('session ended,about to flip');
			   //alert('msg='+msg);
             jQT.goTo('#login', 'flip');
			}
		},
        error: function (jqXHR, textStatus, errorThrown) {
        	$(SectionToFill).html(MessageError+textStatus+errorThrown); // this takes care of a page not found or page not responding situation.
        }
	});

}	

function ProcessLogin() {
    var username=$('#username').val();
    var password=$('#password').val();
	var rememberusername=$('#rememberusername').attr('checked');
    var datatosent="username="+username+"&password="+password+ "&rememberusername="+rememberusername;
    //console.log(datatosent);
	//console.log('login clicked');
    $('#login').append(MessageLoad);
    $.ajax({
        type: "POST",
        url: "ProcessLogin.aspx",
        data: datatosent,
        success: function (msg) {
            $('#progress').replaceWith(''); //$('#login').remove('#progress') will not work
            if (msg=="CorrectLogin") {
                //console.log("here");
				$('#LabelMessage').text('');
                jQT.goTo('#home');
            } else {
                $('#LabelMessage').text(msg);
            }
        }
    });
    //jQT.goBack();
    return false;
}

function ProcessLogout(e) {
	//console.log('log out clicked');
    e.preventDefault();
    var logoutConfirmation=$('#logoutmessage').html();
    $('#logoutmessage').html('');
    $('#logoutmessage').append(MessageLoadLogout);
		jQT.goTo('#logout');
		$.ajax({
		    type: "GET",
		    url: "ProcessLogout.aspx",
		    data: "",
		    success: function (msg) {
		        if (msg=="LoggedOut") {
		            $('#progresslogout').replaceWith('');
		            $('#logoutmessage').html(logoutConfirmation);
		            //console.log('in LoggedOut');
		            $('#password').val(''); //because password field tend to retain the keyed in password, its made blank on logout.
		            //jQT.goTo('#logout'); // no 'Not able to tap element' error.
		        }
		    }
		});

}
/// <summary>This fn had to be used because this does not work:
///$('area').live('click', function () { 
///<summary>
    function areaClicked(url) {
    	//console.log('areaClicked !' + url);
        UrlForFetchingData=url;
        var SectionToFill='#AppointmentDetailsContents';
        var MoveToURL='#AppointmentDetails';
        //console.log('UrlForFetchingData='+UrlForFetchingData);
        ProcessArrowlessPageLink(UrlForFetchingData, MoveToURL, SectionToFill);
    }


    /* Dennis: programatically adjusting zoom (after a pinch zoom) is not possible reliably hence this feature is put on hold
    if (initialWindowWidth==0) {
    initialWindowWidth=jQuery(window).width(); // This value is read only once because it changes for android. In android jQuery(window).width()=window.innerWidth
    appImageWindowWidth=window.innerWidth;
    }

    function resizeAppointmentImageToolbar() {
    if (initialWindowWidth==0) {
    initialWindowWidth=jQuery(window).width(); // This value is read only once because it changes for android. In android jQuery(window).width()=window.innerWidth
    }
    var zoomL=window.innerWidth/initialWindowWidth;
    // //console.log("window.innerWidth="+window.innerWidth)
    // //console.log("initialWindowWidth="+initialWindowWidth)
    // //console.log("jQuery(window).width()="+jQuery(window).width())
    $('#toolbarAppointmentImage').attr('style', '-webkit-transform: scale('+zoomL+')');
    }
    //setInterval(resizeAppointmentImageToolbar, 2000);

    */

    /* Dennis: programatically adjusting zoom (after a pinch zoom) is not possible reliably hence this feature is put on hold
    var previousOrientation=0;
    var checkOrientation=function () {
    //console.log(window.orientation);
    //displayWidth();
    //console.log('orientation changed window.innerWidth='+window.innerWidth)
    if (window.orientation != previousOrientation) {
    previousOrientation=window.orientation;
    // orientation changed, do stuff here
    //console.log('orientation changed window.innerWidth='+window.innerWidth)

    switch (window.orientation) {
    case 0:
    //displayStr += "Portrait";
    //resetScreenSizeOr();
    break;
    case -90:
    //displayStr += "Landscape (right, screen turned clockwise)";
    break;
    case 90:
    //displayStr += "Landscape (left, screen turned counterclockwise)";
    break;
    case 180:
    //displayStr += "Portrait (upside-down portrait)";
    //resetScreenSizeOr();
    break;

    }


    }



    };

    window.addEventListener("resize", checkOrientation, false);
    window.addEventListener("orientationchange", checkOrientation, false);
    //setInterval(checkOrientation, 2000);

    function displayWidth(){
    //$(window).width();
    //console.log('window width '+$(window).width());
    //$(window).width() changes when the orientation changes, window.innerWidth does not change when the orientation changes
    }

    function resetScreenSizeOr() {
    //console.log("Adjustion for orientatiion change");
    scale=appImageWindowWidth / jQuery(window).width();
    $('body').attr('style', '-webkit-transform: scale('+scale+'); -webkit-transform-origin: 0 0;');

    }
    */
    //end


    /*This function actually scales the elements giving the illusion of a changing screen size*/

    /* Dennis: programatically adjusting zoom (after a pinch zoom) is not possible reliably hence this feature is put on hold
    function resetScreenSize() {
    //displayWidth();
    appImageWindowWidth=window.innerWidth; // Dennis: note that window.innerWidth can be changed by a *physical* pinch and zoom only
    var scale=appImageWindowWidth / initialWindowWidth;
    //console.log("window.innerWidth a="+window.innerWidth);
    //$('body').attr('style', '-webkit-transform: scale('+scale+'); -webkit-transform-origin: 0 0;');

    //$('body').attr('style', 'zoom:25%');
    //screen.width=initialWindowWidth;
    //observation
    $('meta[name=viewport]').attr('content', 'width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=1');
    //console.log("in reset"+ $('meta[name=viewport]').attr("content"));


    var browser=navigator.userAgent.toLowerCase(); cc
    if (browser.indexOf('android')!=-1) {
    $('meta[name=viewport]').attr('content', 'width=device-width, initial-scale=1.0, maximum-scale=10.0'); //works for android
    //console.log("window.innerWidth a="+window.innerWidth);
    }
    else {//iphone
    var zoom=window.innerWidth/jQuery(window).width(); //console.log("window.innerWidth b="+window.innerWidth);
    $('body').attr('style', '-webkit-transform: scale('+zoom+'); -webkit-transform-origin: 0 0;'); //works for iphone
    //window.innerWidth=initialWindowWidth;
    //console.log("window.innerWidth c="+window.innerWidth);
    }
  
    }
    */		

		
		
		