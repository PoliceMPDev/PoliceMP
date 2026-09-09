$(function() {
	$("body").hide();
	
	window.addEventListener('message', function(event) {
			if (event.data.anpr == "on") {
				resetInterface();
				$("body").show();
				
				$("#marquee").html("<marquee>POLICEMP | AUTOMATIC NUMBER PLATE RECOGNITION</marquee>");
			} else if (event.data.anpr == "off") {
				resetInterface();
				$("body").hide();
			}
			
			if (event.data.lock_anpr == "on") {
				$("#lock").show();
			} else if (event.data.lock_anpr == "off") {
				$("#lock").hide();
			}
			
			if (event.data.receive_anpr_data == "on") {
				$("#plate").text(event.data.plate);
				$("#speed").text(event.data.speed + " MPH");
				$("#model").text(event.data.model);
				$("#colour").text(event.data.colour);
				
				setBoxSuccess("#mot", event.data.mot);
				setBoxSuccess("#insurance", event.data.insurance);
				setBoxSuccess("#tax", event.data.tax);
				
				setBoxSuccess("#drugs-intel", event.data.drugsintel);
				setBoxSuccess("#weapons-intel", event.data.weaponsintel);
				setBoxSuccess("#fail-to-stop", event.data.failtostop);
				
				
				setBoxSuccess("#stolen", event.data.stolen);
				
				setBoxSuccess("#wanted", event.data.outstandingcrime);
				
				if (!isTrue(event.data.scrapped) ||
					!isTrue(event.data.writtenoff) ||
					!isTrue(event.data.exported)) {
					setBoxSuccess("#other", "false");
				} else {
					setBoxSuccess("#other", "true");
				}
			} else if (event.data.receive_anpr_data == "off") {
				resetInterface();
			}
	});
	
	function isTrue(value) {
		return (value == "true" || value == "True" || value == "TRUE");
	}
	
	function setBoxSuccess(box, value) {
		if (isTrue(value)) {
			setBoxNone(box);
		} else {
			$(box).attr("style", "background: #B22222; color: #fff; border-color: #000;");
		}
	}
	
	function setBoxNone(box) {
		$(box).attr("style", "background: #C0C0C0; color: #000;");
	}
	
	function resetInterface() {
		$("#plate").text("XX99 XXX");
		$("#speed").text("0 MPH");
		$("#model").text("NO VEHICLE");
		$("#colour").text("N/A");
		
		setBoxNone("#mot");
		setBoxNone("#insurance");
		setBoxNone("#tax");
		
		setBoxNone("#drugs-intel");
		setBoxNone("#weapons-intel");
		setBoxNone("#fail-to-stop");
		setBoxNone("#outstanding-crime");
		
		setBoxNone("#stolen");
		setBoxNone("#written-off");
		setBoxNone("#scrapped");
		setBoxNone("#exported");
		
		setBoxNone("#wanted");
		setBoxNone("#other");
		
		$("#lock").hide();
	}
});