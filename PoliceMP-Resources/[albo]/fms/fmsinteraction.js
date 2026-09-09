/* eslint-disable */
const nfetch = require("node-fetch");

// Sets up the socket connection and adds event handlers.
const token = GetResourceMetadata("fms", "websitetoken", 0);
const websiteurl = GetResourceMetadata("fms", "websiteurl", 0);
const version = 1020;
const socket = require('socket.io-client')(websiteurl, {
	path: "/fivem",
	query: { token, version: version }
});

let tabletGlobalSettings = {};
const allowedLiveMapIDs = (GetResourceMetadata("fms", "allowedLiveMapIDs", 0) || "").split(",");

socket.on('connect', () => {
	console.log("[fms] Successfully connected to FMS.");
});

socket.on('disconnect', (reason) => {
	if (reason === "io server disconnect") {
		console.log("[fms] Connection closed by server. This may be because FMS settings were changed. If this persists, please review the setup instructions carefully.");
	}

	setTimeout(() => {
		if (socket.disconnected) {
			socket.connect();
		}
	}, 6000);
});

socket.on("connect_error", (err) => {
	if (err != null && err.message == "xhr poll error") {
		return;
	}

	if (err != null && err.message != null) {
		console.error("[fms] Server connection error: " + err.message);
	} else {
		console.error("[fms] Server connection error. Ensure the token/website URL are correct, that the IP is whitelisted, and that the fms resource is fully updated. Please follow the setup instructions carefully.");
	}

	setTimeout(() => {
		if (socket.disconnected) {
			socket.connect();
		}
	}, 30000);
});	

socket.on("logmessage", (isError, message) => {
	if (message == null) { return; }
	if (!isError) {
		console.log("[fms] Server message: " + message);
	} else {
		console.error("[fms] Server error: " + message);
	}
});

function escapeHtml(text) {
	if (text == null) {
		return null;
	}

	const map = {
		'&': '&amp;',
		'<': '&lt;',
		'>': '&gt;',
		'"': '&quot;',
		"'": '&#039;'
	};

	return text.toString().replace(/[&<>"']/g, function (m) { return map[m]; });
}

function getGameLicense(player) {
	let gamelicense;

	const numIdentifiers = GetNumPlayerIdentifiers(player);
	for (let id = 0; id < numIdentifiers; id++) {
		const identifier = GetPlayerIdentifier(player, id);
		if (identifier.startsWith("license:")) {
			gamelicense = identifier;
		}
	}
	return gamelicense;
}

// Returns player ID or -1 if no player found with license.
function getPlayerFromLicense(gamelicense) {
	const numIndices = GetNumPlayerIndices();

	for (let pidx = 0; pidx < numIndices; pidx++) {
		const player = GetPlayerFromIndex(pidx);
		const playerLicense = getGameLicense(player);
		if (gamelicense == playerLicense) {
			return player;
		}
	}

	return -1;
}

socket.on("kickplayer", (data) => {
	if (!data || !data.playerid) { return; }
	if (!data.hideconsole) {
		console.log("[fms] Kicking player " + data.playerid + " - " + GetPlayerName(data.playerid));
	}
	DropPlayer(data.playerid.toString(), data.reason || "You have been kicked from the server.");
});

function generateNotificationHtml(bodyHtml, branch, division) {
	// Default
	let headerText = (tabletGlobalSettings.fms || "FMS") + " MDT" + (branch != null ? " - " + branch : "");
	let headerClass = "noty-header-blue";

	// Customise the card-header-colour classes in pNotify/html/themes.css
	// You can customise the headerText and headerClass for notifications based on the branch abbreviation or division (police/civilian/ambulance/fire/control), for example...
	if (division === "fire") {
		headerClass = "noty-header-red";
	} else if (division === "ambulance") {
		headerClass = "noty-header-green";
	}

	return "<div class=\"border-0\"><div class=\"noty-header text-center " + headerClass + "\"><img class=\"header_image\" src=\"logo.png\" width=\"25\" height=\"25\" /><h4 style=\"margin-bottom:0; display:inline;\">" + headerText + "</h4></div><div>" + bodyHtml + "</div></div>";
}

function generateCadHtml(unitcallsign, cad, unitsoncad, branch, division) {
	let cadhtml = "";
	if (tabletGlobalSettings.fivemcadnotificationsetting == 1) {
		cadhtml += "<img style='max-width:100%; float: right;' src='logo.png' width=\"40\" height=\"40\" />"
		cadhtml += "<div class=\"cadfield\"><strong>" + escapeHtml(unitcallsign) + "</strong> attached to CAD <strong>" + escapeHtml(cad.ref) + "</strong>" +
			"<br/><br/>" + escapeHtml(cad.openingcode) + " <span class=\"badge " + (cad.gradingclassnames || "") + "\">" + escapeHtml(cad.grading || "N/A") + "</span>" + 
			"<br/>Attached Units: ";
		if (unitsoncad != null) {
			cadhtml += unitsoncad.map(x => x.callsign).join(", ");
		}
		cadhtml += "<br/><b><u>" + escapeHtml(cad.type) + "</b></u> - " + escapeHtml(cad.location) +
			"<br/>" + escapeHtml(cad.commsgroup) + " - " + escapeHtml(cad.channel) +
			"<br/>" + cad.description + "</div>";
		return cadhtml;
	} else {
		cadhtml += "<div class=\"cadfield\"><strong>" + escapeHtml(unitcallsign) + "</strong> attached to CAD <strong>" + escapeHtml(cad.ref) + "</strong>" +
			"<br/><strong>Code:</strong> " + escapeHtml(cad.openingcode) +
			"<br/><strong>Location:</strong> " + escapeHtml(cad.location) +
			"<br/><strong>Channel:</strong> " + escapeHtml(cad.channel) +
			"<br/><strong>Units:</strong> ";
		if (unitsoncad != null) {
			cadhtml += unitsoncad.map(x => x.callsign).join(", ");
		}

		cadhtml += "<br /><strong>Grading:</strong> <span class=\"badge " + (cad.gradingclassnames || "") + "\">" + escapeHtml(cad.grading || "N/A") + "</span>";
		cadhtml += "<br /><strong>" + escapeHtml(cad.type) + "</strong> - " + escapeHtml(cad.description) + "</div>";
	}
	return generateNotificationHtml(cadhtml, branch, division);
}

socket.on("unitattached", (data) => {
	const licensesdeploymentinfo = data.licensesdeploymentinfo;
	const unitcallsign = data.unitcallsign;
	const cad = data.cad;
	const unitsoncad = data.unitsoncad;
	const licensesupdatetablet = data.licensesupdatetablet;
	const comments = data.comments;
	const unitid = data.unitid;
	if (unitcallsign == null) {
		return;
	}

	emit("fms:unitattachedSv", unitcallsign, cad, unitsoncad);
	for (const l of licensesupdatetablet) {
		const playerId = getPlayerFromLicense(l.gamelicense);
		if (playerId != -1) {
			emitNet("fms:clnuidata", playerId, "updatemdtdata", { updatetype: "attachunit_mdt", unitid: unitid, rcad: cad, rcomments: comments, runitsoncad: unitsoncad });
		}
	}
	for (let i = 0; i < licensesdeploymentinfo.length; i++) {
		const playerId = getPlayerFromLicense(licensesdeploymentinfo[i].gamelicense);
		if (playerId != -1) {
			emitNet("fms:unitattachedCl", playerId, unitcallsign, cad, unitsoncad);
			if (cad == null) {
				continue; // Only proceed if attached to a CAD, not detached
			}

			if (licensesdeploymentinfo[i].deploymenteventname) {
				return emitNet(licensesdeploymentinfo[i].deploymenteventname, playerId, unitcallsign, cad, unitsoncad);
			}

			let soundname = "deployment.ogg";
			if (licensesdeploymentinfo[i].deploymentsoundname) {
				soundname = licensesdeploymentinfo[i].deploymentsoundname;
			}
			emitNet('pNotify:SendNotification', playerId, {
				layout: "centerRight", theme: "fms", text: generateCadHtml(unitcallsign, cad, unitsoncad, licensesdeploymentinfo[i].branchabbr, licensesdeploymentinfo[i].branchdivision), type: "alert",
				timeout: 12000, progressBar: false,
				sounds: {
					sources: [soundname], // For sounds to work, you place your sound in the html folder and then add it to the files array in the fxmanifest.lua file.
					volume: 0.5,
					conditions: ["docVisible"] // This means it will play the sound when the notification becomes visible.
				}
			});
		}
	}
});

socket.on("newopencad", (data) => {
	if (!data || !data.gamelicenses || !data.cad) { return; }
	const licensesdeploymentinfo = data.gamelicenses;

	for (let i = 0; i < licensesdeploymentinfo.length; i++) {
		const playerId = getPlayerFromLicense(licensesdeploymentinfo[i]);
		if (playerId != -1) {
			getPlayerFmsData(playerId, (fmsdata) => {
				const notiBranch = fmsdata != null ? (fmsdata.currentPatrolBranch || fmsdata.branch) : null;
				const notiDivision = fmsdata != null ? (fmsdata.currentPatrolBranchDivision || fmsdata.branchDivision) : null;
				let statehtml = "New open CAD: <strong>" + data.cad.ref + "</strong>";
				
				emitNet('pNotify:SendNotification', playerId, {
					layout: "centerRight", theme: "fms", text: generateNotificationHtml(statehtml, notiBranch, notiDivision), type: "alert",
					timeout: 4000, progressBar: false,
				});
			});
		}
	}
});


socket.on("stateupdate", (data) => {
	const gamelicenses = data.gamelicenses;
	const callsign = data.callsign; 
	const state = data.state;
	const basestate = data.basestate;
	const statename = data.statename;
	const stateorderid = data.stateorderid;
	const detailedstateorderid = data.detailedstateorderid;
	if (callsign == null || state == null || statename == null || stateorderid == null) {
		return;
	}

	emit("fms:stateupdateSv", callsign, state, statename, basestate);
	if (gamelicenses == null) {
		return;
	}

	for (let i = 0; i < gamelicenses.length; i++) {
		const playerId = getPlayerFromLicense(gamelicenses[i].gamelicense);
		if (playerId != -1) {
			getPlayerFmsData(playerId, (fmsdata) => {
				emitNet("fms:stateupdateCl", playerId, callsign, state, statename, basestate);
				emitNet("fms:clnuidata", playerId, "updatemdtdata", { updatetype: "unitstateupdate", state: state, statename: statename, stateorderid: stateorderid, basestate: basestate, detailedstateorderid: detailedstateorderid });
				if (state !== 0) {
					// Only show the notification if this isn't a panic button.
					const notiBranch = fmsdata != null ? (fmsdata.currentPatrolBranch || fmsdata.branch) : null;
					const notiDivision = fmsdata != null ? (fmsdata.currentPatrolBranchDivision || fmsdata.branchDivision) : null;
					let statehtml = "<strong>" + escapeHtml(callsign) + "</strong> is now showing as <strong><span class=\"badge\" state='" + escapeHtml(state) + "' basestate=\"" + escapeHtml(basestate) + "\">" + escapeHtml(statename) + "</span></strong>";
					emitNet('pNotify:SendNotification', playerId, {
						layout: "centerRight", theme: "fms", progressBar: false,
						text: generateNotificationHtml(statehtml, notiBranch, notiDivision),
						type: "alert", timeout: 3500
					});
				}
			});
		}
	}
});

socket.on("unitradioping", (gamelicenses, targetunitcallsign, sourcedisplayname) => {
	if (gamelicenses == null) {
		return;
	}

	for (let i = 0; i < gamelicenses.length; i++) {
		const playerId = getPlayerFromLicense(gamelicenses[i].gamelicense);
		if (playerId != -1) {
			getPlayerFmsData(playerId, (fmsdata) => {
				const notiBranch = fmsdata != null ? (fmsdata.currentPatrolBranch || fmsdata.branch) : null;
				const notiDivision = fmsdata != null ? (fmsdata.currentPatrolBranchDivision || fmsdata.branchDivision) : null;
				let statehtml = "<strong>" + escapeHtml(targetunitcallsign) + "</strong>: Radio callback ping by <strong>" + escapeHtml(sourcedisplayname) + "</strong>";
				emitNet('pNotify:SendNotification', playerId, {
					layout: "centerRight", theme: "fms", progressBar: false,
					text: generateNotificationHtml(statehtml, notiBranch, notiDivision),
					type: "alert", timeout: 8000,
					sounds: {
						sources: ["urgentcallback.ogg"], // For sounds to work, you place your sound in the html folder and then add it to the files array in the fxmanifest.lua file.
						volume: 0.3,
						conditions: ["docVisible"] // This means it will play the sound when the notification becomes visible.
					}
				});
			});
		}
	}
});

socket.on("panicbutton", (displayname, callsign, streetname, branchdivision, branchabbreviation) => {
	if (displayname == null || callsign == null) {
		return;
	}

	let statehtml = "<div class=\"grading-red\"><b>" + escapeHtml(callsign || "") + " - " + escapeHtml(displayname || "") + (branchabbreviation != null ? " - " + branchabbreviation : "") + "</b> has pressed their panic button."
	if (streetname != null && streetname.trim() != "") {
		statehtml += "<br />Last known location: <b>" + streetname + "</b>.";
	}
	statehtml += "</div>";

	getAllPlayersFmsData((allPlayerData) => {
		for (let i = 0; i < GetNumPlayerIndices(); i++) {
			const playerId = GetPlayerFromIndex(i);
			const fmsdata = allPlayerData[playerId.toString()];
			let notiBranch = null;
			let notiDivision = null;
			if (fmsdata != null) {
				notiBranch = fmsdata.currentPatrolBranch || fmsdata.branch;
				notiDivision = fmsdata.currentPatrolBranchDivision || fmsdata.branchDivision;
				if (fmsdata.currentPatrolBranchDivision === "civilian") {
					// Comment the next line to enable panic buttons for civilians during patrols.
					continue;
				} else if (branchdivision != null && fmsdata.currentPatrolBranchDivision !== branchdivision) {
					// Only show panic buttons for police to police units, ambulance to ambulance units etc.
					continue;
				}
			}

			emitNet('pNotify:SendNotification', playerId, {
				layout: "centerRight", theme: "fms", progressBar: false,
				text: generateNotificationHtml(statehtml, notiBranch, notiDivision),
				type: "warning", timeout: 10000,
				sounds: {
					sources: ["panic.wav"], // For sounds to work, you place your sound in the html folder and then add it to the files array in the fxmanifest.lua file.
					volume: 0.5,
					conditions: ["docVisible"] // This means it will play the sound when the notification becomes visible.
				}
			});
		}
	});
});

socket.on("updateobservation", (data) => {
	if (data == null || data.observation == null || !data.gamelicenses) {
		return;
	}
	const gamelicenses = data.gamelicenses;
	const observation = data.observation; 
	const isUpdate = data.isUpdate;
	const observationtext = observation.observation;

	let timeout = 3000;
	if (observationtext.length < 20) {
		timeout = 3000;
	}
	else if (observationtext.length < 40) {
		timeout = 5000;
	}
	else if (observationtext.length < 80) {
		timeout = 8000;
	} else {
		timeout = 10000;
	} 
	for (const l of gamelicenses) {
		const playerId = getPlayerFromLicense(l.gamelicense);
		if (playerId != -1) {
			emitNet("fms:clnuidata", playerId, "updatemdtdata", { updatetype: isUpdate ? "updateobservation" : "newobservation", observation: observation });

			getPlayerFmsData(playerId, (fmsdata) => {
				const notiBranch = fmsdata != null ? (fmsdata.currentPatrolBranch || fmsdata.branch) : null;
				const notiDivision = fmsdata != null ? (fmsdata.currentPatrolBranchDivision || fmsdata.branchDivision) : null;
				let observationhtml = "<strong>" + (isUpdate ? "Observations Update" : "New Observations") + "</strong>: " + escapeHtml(observationtext);
				emitNet('pNotify:SendNotification', playerId, {
					layout: "centerRight", theme: "fms", progressBar: false,
					text: generateNotificationHtml(observationhtml, notiBranch, notiDivision),
					type: "alert", timeout
				});
			});
		}
	}
});

socket.on("deleteobservation", (data) => {
	if (data == null || data.obsid == null) {
		return;
	}
	emitNet("fms:clnuidata", -1, "updatemdtdata", { updatetype: "deleteobservation", observationid: data.obsid });
});

socket.on("updateunitvehicletype", (data) => {
	if (data == null || data.vehicletype == null || !data.gamelicenses) {
		return;
	}
	const gamelicenses = data.gamelicenses;

	for (const l of gamelicenses) {
		const playerId = getPlayerFromLicense(l.gamelicense);
		if (playerId != -1) {
			emitNet("fms:clnuidata", playerId, "updatemdtdata", { updatetype: "updateunitvehicletype", vehicletype: data.vehicletype });
		}
	}
});

function genObsRequest(source, args) {
	const gamelicense = getGameLicense(source);
	const playerId = source;

	if (gamelicense) {
		socket.emit('fivemgetobservations', (success, observations) => {
			if (success) {
				if (!observations || observations.length == 0) {
					return emitNet('chatMessage', playerId, 'FMS', [0, 255, 0], "No outstanding observations.");
				}
				let totalTimeout = 0;
				let obsString = "<strong>Outstanding Observations</strong><br/>--<br/>";
				for (let i = 0; i < observations.length; i++) {
					let timeout = 3000;
					if (observations[i].length < 20) {
						timeout = 3000;
					}
					else if (observations[i].length < 40) {
						timeout = 5000;
					}
					else if (observations[i].length < 80) {
						timeout = 8000;
					} else if (observations[i].length < 120) {
						timeout = 10000;
					} else {
						timeout = 12000;
					}

					totalTimeout += timeout;

					obsString += escapeHtml(observations[i].observation.substring(0, 210)) + (observations[i].observation.length > 210 ? "..." : "") + "<br/>--<br/>";
				}


				if (totalTimeout > 18000) {
					totalTimeout = 18000;
				}
				getPlayerFmsData(playerId, (fmsdata) => {
					const notiBranch = fmsdata != null ? (fmsdata.currentPatrolBranch || fmsdata.branch) : null;
					const notiDivision = fmsdata != null ? (fmsdata.currentPatrolBranchDivision || fmsdata.branchDivision) : null;
					emitNet('pNotify:SendNotification', playerId, {
						layout: "centerRight", theme: "fms", progressBar: false,
						text: generateNotificationHtml(obsString, notiBranch, notiDivision),
						type: "alert", timeout: totalTimeout
					});
				});
			}
		});
	}

}

RegisterCommand('genobs', genObsRequest);

socket.on("bookoffunit", (data) => {
	if (!data || data.unitid == null) { return; }
	emitNet("fms:clnuidata", -1, "updatemdtdata", { updatetype: "bookoffunit", unitid: data.unitid });
});

socket.on("bookedon", (gamelicenses, unitcallsign, creweddisplaynames) => {
	emit("fms:bookedonSv", unitcallsign, creweddisplaynames);
	for (let i = 0; i < gamelicenses.length; i++) {
		const playerId = getPlayerFromLicense(gamelicenses[i].gamelicense);
		if (playerId != -1) {
			emitNet("fms:clnuidata", playerId, "updatemdtdata", { updatetype: "bookonunit" });
			emitNet("fms:bookedonCl", playerId, unitcallsign, creweddisplaynames);
			getPlayerFmsDataOpts({ playerId, forceRefresh: true }, (fmsdata) => {
				const notiBranch = fmsdata != null ? (fmsdata.currentPatrolBranch || fmsdata.branch) : null;
				const notiDivision = fmsdata != null ? (fmsdata.currentPatrolBranchDivision || fmsdata.branchDivision) : null;
				let bookedhtml = "You are now booked on with the following callsign: <strong>" + escapeHtml(unitcallsign) + "</strong><br/>";
				if (creweddisplaynames) {
					bookedhtml += "Crewed users: <strong>" + escapeHtml(creweddisplaynames.join(" - ")) + "</strong>";
				}
				emitNet('pNotify:SendNotification', playerId, {
					layout: "centerRight", theme: "fms", text: generateNotificationHtml(bookedhtml, notiBranch, notiDivision),
					type: "alert",
					timeout: 7000, progressBar: false
				});
			});
		}
	}
});

socket.on("bookoffuserfromunit", (unitgamelicenses, usergamelicense, unitcallsign, bookedoffdisplayname, forpageralert) => {
	const initiatingUserId = getPlayerFromLicense(usergamelicense);
	if (initiatingUserId != -1) {
		emitNet("fms:clnuidata", initiatingUserId, "updatemdtdata", { updatetype: "bookoffuserfromunit" });
	}
	for (let i = 0; i < unitgamelicenses.length; i++) {
		const playerId = getPlayerFromLicense(unitgamelicenses[i].gamelicense);
		if (playerId != -1) {
			emitNet("fms:clnuidata", playerId, "updatemdtdata", { updatetype: "bookoffuserfromunit" });
			getPlayerFmsDataOpts({ playerId, forceRefresh: true }, (fmsdata) => {
				const notiBranch = fmsdata != null ? (fmsdata.currentPatrolBranch || fmsdata.branch) : null;
				const notiDivision = fmsdata != null ? (fmsdata.currentPatrolBranchDivision || fmsdata.branchDivision) : null;
				let bookedhtml = "<strong>" + escapeHtml(bookedoffdisplayname) + "</strong> has booked off from <strong>" + unitcallsign + "</strong>";
				if (forpageralert == true) {
					bookedhtml += " because they accepted a pager alert.";
				}

				emitNet('pNotify:SendNotification', playerId, {
					layout: "centerRight", theme: "fms", text: generateNotificationHtml(bookedhtml, notiBranch, notiDivision),
					type: "alert",
					timeout: 5000, progressBar: false
				});
			});
		}
	}

	if (forpageralert == true) {
		return;
	}

	const playerId = getPlayerFromLicense(usergamelicense);
	if (playerId != -1) {
		getPlayerFmsDataOpts({ playerId, forceRefresh: true }, (fmsdata) => {
			const notiBranch = fmsdata != null ? (fmsdata.currentPatrolBranch || fmsdata.branch) : null;
			const notiDivision = fmsdata != null ? (fmsdata.currentPatrolBranchDivision || fmsdata.branchDivision) : null;
			let bookedhtml = "You have been booked off from <strong>" + unitcallsign + "</strong>";

			emitNet('pNotify:SendNotification', playerId, {
				layout: "centerRight", theme: "fms", text: generateNotificationHtml(bookedhtml, notiBranch, notiDivision),
				type: "alert",
				timeout: 5000, progressBar: false
			});
		});
	}
});

socket.on("unitcommsgroupset", (gamelicenses, unitcallsign, commsgroupname) => {
	emit("fms:unitCommsGroupSetSv", unitcallsign, commsgroupname);
	for (let i = 0; i < gamelicenses.length; i++) {
		const playerId = getPlayerFromLicense(gamelicenses[i].gamelicense);
		if (playerId != -1) {
			getPlayerFmsDataOpts({ playerId }, (fmsdata) => {
				emitNet("fms:unitCommsGroupSetCl", playerId, unitcallsign, commsgroupname);
				const notiBranch = fmsdata != null ? (fmsdata.currentPatrolBranch || fmsdata.branch) : null;
				const notiDivision = fmsdata != null ? (fmsdata.currentPatrolBranchDivision || fmsdata.branchDivision) : null;
				let bookedhtml = "Comms group set to <strong>" + escapeHtml(commsgroupname) + "</strong>";
				emitNet('pNotify:SendNotification', playerId, {
					layout: "centerRight", theme: "fms", text: generateNotificationHtml(bookedhtml, notiBranch, notiDivision),
					type: "alert",
					timeout: 3000, progressBar: false
				});
			});
		}
	}
});

socket.on("usercommsgroupset", (gamelicense, commsgroupname) => {
	const playerId = getPlayerFromLicense(gamelicense);
	if (playerId != -1) {
		emit("fms:userCommsGroupSetSv", playerId, commsgroupname);
		getPlayerFmsDataOpts({ playerId }, (fmsdata) => {
			emitNet("fms:userCommsGroupSetCl", playerId, commsgroupname);
			const notiBranch = fmsdata != null ? (fmsdata.currentPatrolBranch || fmsdata.branch) : null;
			const notiDivision = fmsdata != null ? (fmsdata.currentPatrolBranchDivision || fmsdata.branchDivision) : null;
			let bookedhtml = "Comms group set to <strong>" + escapeHtml(commsgroupname) + "</strong>";
			emitNet('pNotify:SendNotification', playerId, {
				layout: "centerRight", theme: "fms", text: generateNotificationHtml(bookedhtml, notiBranch, notiDivision),
				type: "alert",
				timeout: 3000, progressBar: false
			});
		});
	}
});

socket.on("newpageralert", (usergamelicenses, pageralert) => {
	if (!usergamelicenses || !pageralert) {
		return;
	}

	for (let i = 0; i < usergamelicenses.length; i++) {
		const playerId = getPlayerFromLicense(usergamelicenses[i]);
		if (playerId != -1) {
			getPlayerFmsDataOpts({ playerId, forceRefresh: true }, (fmsdata) => {
				const notiBranch = fmsdata != null ? (fmsdata.currentPatrolBranch || fmsdata.branch) : null;
				const notiDivision = fmsdata != null ? (fmsdata.currentPatrolBranchDivision || fmsdata.branchDivision) : null;
				
				let bodyhtml = "<b>New Pager Alert</b> - <b>" + escapeHtml(pageralert.callsign) + "</b>"
									+ "<br/><strong>/apager " + escapeHtml(pageralert.id) + "</strong> to accept"
									+ "<br/>Max " + escapeHtml(pageralert.maxusers + " user" + (pageralert.maxusers == 1 ? "" : "s")) + ". " 
									+ escapeHtml(pageralert.alertedusers) + " user" + (pageralert.alertedusers == 1 ? "" : "s") 
									+ " subscribed to <b>" + escapeHtml(pageralert.branchabbreviation) + "</b> alerts." 
									+ "<br/>" + escapeHtml(pageralert.description || "");
									

				emitNet('pNotify:SendNotification', playerId, {
					layout: "centerRight", theme: "fms", text: generateNotificationHtml(bodyhtml, notiBranch, notiDivision),
					type: "alert",
					timeout: 10000, progressBar: false,
					sounds: {
						sources: ["pageralert.ogg"],
						volume: 0.3,
						conditions: ["docVisible"]
					}
				});
			});
		}
	}
});

function acceptPagerAlert(playerid, pageralertid) {
	const gamelicense = getGameLicense(playerid);
	if (gamelicense) {
		socket.emit('fivemacceptpageralert', gamelicense, pageralertid, (error) => {
			if (error != null) {
				emitNet('chatMessage', playerid, 'FMS', [0, 255, 0], error);
			}
		});
	}
};

RegisterCommand('apager', (source, args) => {
	if (!args || args.length == 0) { return; }
	acceptPagerAlert(source, args[0]);
});

RegisterCommand('acceptpager', (source, args) => {
	if (!args || args.length == 0) { return; }
	acceptPagerAlert(source, args[0]);
});

function pagerInfoHandler(source) {
	const gamelicense = getGameLicense(source);
	if (gamelicense) {
		socket.emit('fivemgetpagerinfo', gamelicense, (error, pagerinfo) => {
			if (error != null || pagerinfo == null) {
				return emitNet('chatMessage', source, 'FMS', [0, 255, 0], error || "Internal server error");
			}

			getPlayerFmsDataOpts({ playerId: source, forceRefresh: true }, (fmsdata) => {
				const notiBranch = fmsdata != null ? (fmsdata.currentPatrolBranch || fmsdata.branch) : null;
				const notiDivision = fmsdata != null ? (fmsdata.currentPatrolBranchDivision || fmsdata.branchDivision) : null;
				let bodyhtml = "";
				if (pagerinfo.subscribedBranchAbbs != null && pagerinfo.subscribedBranchAbbs.length > 0) { 
					bodyhtml += "You're currently subscribed to pager alerts for ";
					for (let i = 0; i < pagerinfo.subscribedBranchAbbs.length; i++) {
						if (i > 0) {
							bodyhtml += ", ";
						} 

						bodyhtml += "<strong>" + escapeHtml(pagerinfo.subscribedBranchAbbs[i]) + "</strong>";
						
					}
					bodyhtml += ". Unsubscribe using <strong>/unsubpager</strong><br/>";
				}

				if (pagerinfo.canSubscribeBranchAbbs != null && pagerinfo.canSubscribeBranchAbbs.length > 0) { 
					bodyhtml += "You can subscribe to pager alerts for ";
					for (let i = 0; i < pagerinfo.canSubscribeBranchAbbs.length; i++) {
						if (i > 0) {
							bodyhtml += ", ";
						} 

						bodyhtml += "<strong>" + escapeHtml(pagerinfo.canSubscribeBranchAbbs[i]) + "</strong>";
						
					}
					bodyhtml += ". Use <strong>/subpager</strong><br/>";
				}

				const mainUnitCallsign = fmsdata != null ? fmsdata.mainUnitCallsign : null;
				if (mainUnitCallsign != null) {
					bodyhtml += escapeHtml(pagerinfo.bookoffStateName || "Book off") + " to return to your main unit: <strong>" + escapeHtml(mainUnitCallsign) + "</strong><br/>";
				}

				if (bodyhtml == "") {
					bodyhtml += "You're not eligible to subscribe to any pager alerts.<br/>";
				}
 									
				emitNet('pNotify:SendNotification', source, {
					layout: "centerRight", theme: "fms", text: generateNotificationHtml(bodyhtml, notiBranch, notiDivision),
					type: "alert",
					timeout: 8000, progressBar: false
				});
			});
		});
	}
};

RegisterCommand('pagerinfo', pagerInfoHandler);
RegisterCommand('pi', pagerInfoHandler);

function sPagerHandler(source, args) {
	const branchabb = args.join(" ").trim();
	if (branchabb == "") {
		return pagerInfoHandler(source);
	}
	const gamelicense = getGameLicense(source);
	if (gamelicense) {
		socket.emit('fivemsubscribetopageralerts', gamelicense, branchabb, (error, branchabbreviation) => {
			if (error != null) {
				emitNet('chatMessage', source, 'FMS', [0, 255, 0], error);
				return pagerInfoHandler(source);
			}

			getPlayerFmsDataOpts({ playerId: source }, (fmsdata) => {
				const notiBranch = fmsdata != null ? (fmsdata.currentPatrolBranch || fmsdata.branch) : null;
				const notiDivision = fmsdata != null ? (fmsdata.currentPatrolBranchDivision || fmsdata.branchDivision) : null;
				
				let bodyhtml = "You've subscribed to pager alerts for <b>" + escapeHtml(branchabbreviation || branchabb) + "</b>";
									
				emitNet('pNotify:SendNotification', source, {
					layout: "centerRight", theme: "fms", text: generateNotificationHtml(bodyhtml, notiBranch, notiDivision),
					type: "alert",
					timeout: 4000, progressBar: false
				});
			});
		});
	}
};

RegisterCommand('spager', sPagerHandler);
RegisterCommand('subpager', sPagerHandler);

function usPagerHandler(source, args) {
	const branchabb = args.join(" ").trim();
	if (branchabb == "") {
		return pagerInfoHandler(source);
	}
	const gamelicense = getGameLicense(source);
	if (gamelicense) {
		socket.emit('fivemunsubscribefrompageralerts', gamelicense, branchabb, (error, branchabbreviation) => {
			if (error != null) {
				emitNet('chatMessage', source, 'FMS', [0, 255, 0], error);
				return pagerInfoHandler(source);
			}

			getPlayerFmsDataOpts({ playerId: source }, (fmsdata) => {
				const notiBranch = fmsdata != null ? (fmsdata.currentPatrolBranch || fmsdata.branch) : null;
				const notiDivision = fmsdata != null ? (fmsdata.currentPatrolBranchDivision || fmsdata.branchDivision) : null;
				
				let bodyhtml = "You've unsubscribed from pager alerts for <b>" + escapeHtml(branchabbreviation || branchabb) + "</b>";
				emitNet('pNotify:SendNotification', source, {
					layout: "centerRight", theme: "fms", text: generateNotificationHtml(bodyhtml, notiBranch, notiDivision),
					type: "alert",
					timeout: 4000, progressBar: false
				});
			});
		});
	}
};

RegisterCommand('upager', usPagerHandler);
RegisterCommand('unsubpager', usPagerHandler);

function showcad(playerid) {
	const gamelicense = getGameLicense(playerid);

	if (gamelicense) {
		socket.emit('fivemgetcad', gamelicense, (success, unitcallsign, cad, unitsoncad, unitstate) => {
			if (success) {
				getPlayerFmsData(playerid, (fmsdata) => {
					const notiBranch = fmsdata != null ? (fmsdata.currentPatrolBranch || fmsdata.branch) : null;
					const notiDivision = fmsdata != null ? (fmsdata.currentPatrolBranchDivision || fmsdata.branchDivision) : null;

					emitNet('pNotify:SendNotification', playerid, {
						layout: "centerRight", theme: "fms", text: generateCadHtml(unitcallsign, cad, unitsoncad, notiBranch, notiDivision), type: "alert",
						timeout: 15000, progressBar: false
					});
				});
			} else {
				emitNet('chatMessage', playerid, 'FMS', [0, 255, 0], "You're not attached to a CAD.");
			}
		});
	}
}

onNet("fms:showcad", () => {
	showcad(source);
});

RegisterCommand('ci', showcad);

function getPlayerAttachedCad(playerId, callback) {
	if (!playerId || !callback || typeof callback !== "function") {
		return;
	}

	const gamelicense = getGameLicense(playerId);
	if (!gamelicense) {
		console.error("Invalid game license");
		return callback("Invalid game license");
	}

	socket.emit('fivemgetcad', gamelicense, (success, unitcallsign, cad) => {
		if (!success || !cad) {
			return callback(null, null);
		} else {
			return callback(null, {
				ref: cad.ref,
				type: cad.type,
				status: cad.status,
				description: cad.description,
				location: cad.location,
				channel: cad.channel,
				commsgroup: cad.commsgroup,
				grading: cad.grading
			});
		}
	});
}

exports('getPlayerAttachedCad', getPlayerAttachedCad);

function getCadByReference(ref, callback) {
	if (!ref || !callback || typeof callback !== "function") {
		console.error("getCadByReference no reference/callback specified.");
		return;
	} 

	socket.emit('fivemgetcadbyref', ref, (err, cad) => {
		if (err) {
			console.error(err);
			return callback(err, null);
		} else if (!cad) {
			return callback(err, cad);
		}

		return callback(null, {
			ref: cad.ref,
			type: cad.type,
			status: cad.status,
			description: cad.description,
			location: cad.location,
			channel: cad.channel,
			commsgroup: cad.commsgroup,
			grading: cad.grading
		});
	});
}

exports('getCadByReference', getCadByReference);

function getAllUnits(callback) {
	if (!callback || typeof callback !== "function") {
		console.error("getAllUnits no callback specified.");
		return;
	}

	socket.emit('fivemgetallunits', (error, units) => {
		if (error != null) {
			console.error("getAllUnits error: " + error);
		} 

		return callback(error, units);
	});
}

exports('getAllUnits', getAllUnits);

const playerUniformData = {};
function fetchUniforms(playerid) {
	const stringedplayerid = playerid.toString();
	const pdata = playerUniformData[stringedplayerid];
	if (pdata && pdata.timestamp && Date.now() - pdata.timestamp < 60 * 1000 && pdata.uniforms != null) {
		emitNet('fms:pushuniforms', playerid, pdata.uniforms);
		return;
	}

	const gamelicense = getGameLicense(playerid);

	if (gamelicense) {
		socket.emit('fivemgetuniforms', gamelicense, (success, uniforms) => {
			if (success) {
				playerUniformData[stringedplayerid] = { uniforms: uniforms, timestamp: Date.now() };
				emitNet('fms:pushuniforms', playerid, uniforms);
			} else {
				emitNet('chatMessage', playerid, 'FMS', [0, 255, 0], "Couldn't get your uniforms.");
			}
		});
	}
}

onNet("fms:fetchuniforms", () => {
	fetchUniforms(source);
});

/* RegisterCommand('uniforms', (source, args) => {
	fetchUniforms(source);
}); */

function stateChangeCmd(source, statecode) {
	if (statecode == null) {
		return;
	}
	const gamelicense = getGameLicense(source);

	if (gamelicense) {
		socket.emit('fivemstateupdate', gamelicense, statecode, (success) => {
			if (!success) {
				emitNet('chatMessage', source, 'FMS', [0, 255, 0], "Error processing status update. Are you booked on/attached to a CAD?");
			}
		});
	}
}

on("fms:UpdateState", (playerId, statecode) => {
	stateChangeCmd(playerId, statecode);
});

onNet("fms:tabletupdatestate", (data) => {
	const playerId = source;
	if (!data || data.statecode == null) { return; }
	const statecode = data.statecode;
	if (statecode == 1) {
		return bookOnDuty(playerId);
	}

	stateChangeCmd(playerId, statecode);
	if (statecode == 2) {
		carChannelOpt(playerId, true);
	}
});

RegisterCommand('st', (source, args) => {
	const statecode = args[0];
	// tabetglobalsettings.find.basestate
	const thestate = tabletGlobalSettings.states?.find(x => x.state == statecode);
	if (!thestate) {
		return;
	}
	if (thestate.basestate == 1) {
		return bookOnDuty(source);
	}

	stateChangeCmd(source, statecode);
	if (thestate.basestate == 2 && !args[1]) {
		carChannelOpt(source, true);
	}
});

const registeredUnitStateCommands = [];
const registeredCommentTypeCommands = [];
const registeredGeneralCommands = {};

function updateCustomisableCommands() {
	if (socket.connected) {
		socket.emit("fivemgetcustomisablecommands", function(success, unitstatecommands, commenttypecommands, generalcommands) {
			if (success) {
				for (const u of unitstatecommands) {
					if (!registeredUnitStateCommands.includes(u.command)) {
						RegisterCommand(u.command, (source, args) => {
							if (u.basestate == 1) {
								return bookOnDuty(source);
							}

							stateChangeCmd(source, u.state);
							if (u.basestate == 2 && !args[0]) {
								carChannelOpt(source, true);
							}
						});
						registeredUnitStateCommands.push(u.command);
					}
				}

				for (const u of commenttypecommands) {
					if (!registeredCommentTypeCommands.includes(u.command)) {
						RegisterCommand(u.command, (source, args) => {
							if (!args || args.length == 0) { return; }
							const comment = args.join(" ");
							const gamelicense = getGameLicense(source);

							if (gamelicense) {
								addCommentToCad({ playerId: source, comment: comment, commentTypeId: u.id }, (error2) => {
									if (error2) {
										emitNet('chatMessage', source, 'FMS', [0, 255, 0], "Error processing CAD comment, are you attached to a CAD?");
									}
								});
							}
						});
						registeredCommentTypeCommands.push(u.command);
					}
				}

				function processGeneralCommand(commandpropertyname, func) {
					if (!generalcommands || !generalcommands[commandpropertyname] || !Array.isArray(generalcommands[commandpropertyname].commands)) {
						console.error("Could not load general commands: " + commandpropertyname);
						return;
					}
					for (const c of generalcommands[commandpropertyname].commands) {
						if (!Array.isArray(registeredGeneralCommands[commandpropertyname])) {
							registeredGeneralCommands[commandpropertyname] = [];
						}

						if (!registeredGeneralCommands[commandpropertyname].includes(c)) {
							RegisterCommand(c, func);
							registeredGeneralCommands[commandpropertyname].push(c);
						}
					}
				}

				processGeneralCommand("tabletcommands", (source) => {
					emitNet("fms:opentablet", source);
				});

				processGeneralCommand("pnccommands", (source) => {
					emitNet("fms:opentablet", source, "/pnc");
				});

				processGeneralCommand("formscommands", (source) => {
					emitNet("fms:opentablet", source, "/forms");
				});

				processGeneralCommand("custodyformcommands", (source) => {
					emitNet("fms:opentablet", source, "/custodyform");
				});

				processGeneralCommand("mdtcommands", (source) => {
					emitNet("fms:opentablet", source, "/mdt");
				});

				processGeneralCommand("civcommands", (source) => {
					emitNet("fms:opentablet", source, "/callstack");
				});

				processGeneralCommand("opencadscommands", (source) => {
					emitNet("fms:opentablet", source, "/opencads");
				});

				processGeneralCommand("anprhitscommands", (source) => {
					emitNet("fms:opentablet", source, "/anpr");
				});
			} else {
				console.error("Error updating customisable commands");
			}
		});
	}

	setTimeout(() => {
		updateCustomisableCommands();
	}, 120000);
}

function updateTabletGlobalSettings() {
	if (socket.connected) {
		socket.emit("fivemgettabletglobalsettings", function(error, settings) {
			if (error) {
				console.error("Error updating tablet global settings: " + error);
			} else if (settings != null) {
				if (JSON.stringify(tabletGlobalSettings) != JSON.stringify(settings)) {
					emitNet("fms:clnuidata", -1, "updateglobalsettings", settings);
				}
				tabletGlobalSettings = settings;
			} 
		});
	}

	setTimeout(() => {
		updateTabletGlobalSettings();
	}, 120000);
}


onNet("fms:gettabletglobalsettings", () => {
	emitNet("fms:clnuidata", source, "updateglobalsettings", tabletGlobalSettings);
});

function bookOnDuty(playerid) {
	const gamelicense = getGameLicense(playerid);

	if (gamelicense) {
		socket.emit('fivembookonduty', gamelicense, (err, success) => {
			if (err != null) {
				emitNet('chatMessage', playerid, 'FMS', [0, 255, 0], "Error booking you on duty: " + err);
			}
		});
	}
}

function pncPerson(playerid, data) {
	const gamelicense = getGameLicense(playerid);
	if (!gamelicense || !data) { return; }
	socket.emit('fivempncperson', gamelicense, data, (error, response) => {
		if (error != null) {
			emitNet('chatMessage', playerid, 'FMS', [0, 255, 0], error || "Error in person check.");
		} else {
			emitNet("fms:clnuidata", playerid, "personpnc", response);
		}
	});
}

onNet("fms:pncperson", (data) => {
	pncPerson(source, data);
});

function pncVehicle(playerid, data) {
	const gamelicense = getGameLicense(playerid);
	if (!gamelicense || !data) { return; }
	socket.emit('fivempncvehicle', gamelicense, data, (error, response) => {
		if (error != null) {
			emitNet('chatMessage', playerid, 'FMS', [0, 255, 0], error || "Error in vehicle check.");
		} else {
			emitNet("fms:clnuidata", playerid, "vehiclepnc", response);
		}
	});
}

onNet("fms:pncvehicle", (data) => {
	pncVehicle(source, data);
});

function pncAddress(playerid, data) {
	const gamelicense = getGameLicense(playerid);
	if (!gamelicense || !data) { return; }
	socket.emit('fivempncaddress', gamelicense, data, (error, response) => {
		if (error != null) {
			emitNet('chatMessage', playerid, 'FMS', [0, 255, 0], error || "Error in address check.");
		} else {
			emitNet("fms:clnuidata", playerid, "addresspnc", response);
		}
	});
}

onNet("fms:pncaddress", (data) => {
	pncAddress(source, data);
});

function pncPersonComment(playerid, data) {
	const gamelicense = getGameLicense(playerid);
	if (!gamelicense || !data) { return; }
	socket.emit('fivempncpersoncomment', gamelicense, data, (error, response) => {
		if (error != null) {
			emitNet('chatMessage', playerid, 'FMS', [0, 255, 0], error || "Error adding person comment.");
		} else {
			emitNet("fms:clnuidata", playerid, "personcommentpnc", response);
		}
	});
}

onNet("fms:pncpersoncomment", (data) => {
	pncPersonComment(source, data);
});

function pncVehicleComment(playerid, data) {
	const gamelicense = getGameLicense(playerid);
	if (!gamelicense || !data) { return; }
	socket.emit('fivempncvehiclecomment', gamelicense, data, (error, response) => {
		if (error != null) {
			emitNet('chatMessage', playerid, 'FMS', [0, 255, 0], error || "Error adding vehicle comment.");
		} else {
			emitNet("fms:clnuidata", playerid, "vehiclecommentpnc", response);
		}
	});
}

onNet("fms:pncvehiclecomment", (data) => {
	pncVehicleComment(source, data);
});


function pncUpdatePersonMarkers(playerid, data) {
	const gamelicense = getGameLicense(playerid);
	if (!gamelicense || !data) { return; }
	socket.emit('fivempncupdatepersonmarkers', gamelicense, data, (error, response) => {
		if (error != null) {
			emitNet('chatMessage', playerid, 'FMS', [0, 255, 0], error || "Error processing person markers.");
		} else {
			emitNet("fms:clnuidata", playerid, "updatepersonmarkerspnc", response);
		}
	});
}

onNet("fms:pncupdatepersonmarkers", (data) => {
	pncUpdatePersonMarkers(source, data);
});

function pncUpdateVehicleMarkers(playerid, data) {
	const gamelicense = getGameLicense(playerid);
	if (!gamelicense || !data) { return; }
	socket.emit('fivempncupdatevehiclemarkers', gamelicense, data, (error, response) => {
		if (error != null) {
			emitNet('chatMessage', playerid, 'FMS', [0, 255, 0], error || "Error processing vehicle markers.");
		} else {
			emitNet("fms:clnuidata", playerid, "updatevehiclemarkerspnc", response);
		}
	});
}

onNet("fms:pncupdatevehiclemarkers", (data) => {
	pncUpdateVehicleMarkers(source, data);
});

function addCommentToOpenCad(opts, callback) {
	if (callback == null || typeof callback !== "function") { return; }
	if (opts.playerId == null || opts.comment == null || opts.comment.trim() == "" || opts.cadid == null) { return callback(false); }
	if (!opts.commentTypeId) {
		opts.commentTypeId = null;
	}
	if (!opts.commentTemplateId) {
		opts.commentTemplateId = null;
	}

	const gamelicense = getGameLicense(opts.playerId);
	if (!gamelicense) {
		console.error("Could not find gamelicense for playerId");
		return callback(false);
	}

	socket.emit('fivemaddopencadcomment', gamelicense, opts.comment, opts.commentTypeId, opts.commentTemplateId, opts.cadid, (error, commentName) => {
		if (error != null) {
			console.error("Error processing addCommentToOpenCad: " + error);
		} else {
			getPlayerFmsData(opts.playerId, (fmsdata) => {
				const notiBranch = fmsdata != null ? (fmsdata.currentPatrolBranch || fmsdata.branch) : null;
				const notiDivision = fmsdata != null ? (fmsdata.currentPatrolBranchDivision || fmsdata.branchDivision) : null;
				const commenthtml = "Your " + escapeHtml(commentName || "comment") + " was successfully added to the open CAD.";
				emitNet('pNotify:SendNotification', opts.playerId, {
					layout: "centerRight", theme: "fms", progressBar: false,
					text: generateNotificationHtml(commenthtml, notiBranch, notiDivision), type: "alert", timeout: 4000
				});
			});
		} 

		callback(error, commentName);
	});
}

onNet("fms:TabletOpenCadComment", (data) => {
	if (!data || !data.comment || !data.cadid) {
		return;
	}

	addCommentToOpenCad({ playerId: source, comment: data.comment, commentTypeId: data.commentTypeId, commentTemplateId: data.commentTemplateId, cadid: data.cadid }, (error) => {
		if (error) {
			emitNet('chatMessage', source, 'FMS', [0, 255, 0], "Error processing open CAD comment, do you have permission for open CADs?");
		}
	});
});

function addCommentToCad(opts, callback) {
	if (callback == null || typeof callback !== "function") { return; }
	if (opts.playerId == null || opts.comment == null || opts.comment.trim() == "") { return callback(false); }
	if (!opts.commentTypeId) {
		opts.commentTypeId = null;
	}
	if (!opts.commentTemplateId) {
		opts.commentTemplateId = null;
	}

	const gamelicense = getGameLicense(opts.playerId);
	if (!gamelicense) {
		console.error("Could not find gamelicense for playerId");
		return callback(false);
	}

	socket.emit('fivemaddcomment', gamelicense, opts.comment, opts.commentTypeId, opts.commentTemplateId, (error, commentName) => {
		if (error != null) {
			console.error("Error processing addCommentToCad: " + error);
		} else {
			getPlayerFmsData(opts.playerId, (fmsdata) => {
				const notiBranch = fmsdata != null ? (fmsdata.currentPatrolBranch || fmsdata.branch) : null;
				const notiDivision = fmsdata != null ? (fmsdata.currentPatrolBranchDivision || fmsdata.branchDivision) : null;
				const commenthtml = "Your " + escapeHtml(commentName || "comment") + " was successfully added to the CAD.";
				emitNet('pNotify:SendNotification', opts.playerId, {
					layout: "centerRight", theme: "fms", progressBar: false,
					text: generateNotificationHtml(commenthtml, notiBranch, notiDivision), type: "alert", timeout: 4000
				});
			});
		} 

		callback(error, commentName);
	});
}

exports('addCommentToCad', addCommentToCad);

onNet("fms:TabletComment", (data) => {
	if (!data || !data.comment) {
		return;
	}

	addCommentToCad({ playerId: source, comment: data.comment, commentTypeId: data.commentTypeId, commentTemplateId: data.commentTemplateId }, (error) => {
		if (error) {
			emitNet('chatMessage', source, 'FMS', [0, 255, 0], "Error processing CAD comment, are you attached to a CAD?");
		}
	});
});

function addcommentfunc(source, args) {
	if (!args || args.length == 0) { return; }
	const comment = args.join(" ");
	const gamelicense = getGameLicense(source);

	if (gamelicense) {
		addCommentToCad({ playerId: source, comment: comment, commentTypeId: null }, (error) => {
			if (error) {
				emitNet('chatMessage', source, 'FMS', [0, 255, 0], "Error processing CAD comment, are you attached to a CAD?");
			}
		});
	}
};

RegisterCommand('comment', addcommentfunc);
RegisterCommand('addcomment', addcommentfunc);

socket.on("newcomment", (data) => {
	if (data == null || data.comment == null) {
		return;
	}

	const gamelicenses = data.gamelicenses;
	const comment = data.comment;
	const notifyunits = data.notifyunits;
	const sourceusergamelicense = data.sourceusergamelicense;

	for (const l of gamelicenses) {
		const playerId = getPlayerFromLicense(l.gamelicense);
		if (playerId != -1) {
			emitNet("fms:clnuidata", playerId, "updatemdtdata", { updatetype: "newcomment", comment: comment });
		}
	}

	if (notifyunits != 1) {
		return;
	}

	let fivemcomment = "";
	if (comment.commenttypename != null) {
		fivemcomment += "[" + comment.commenttypename + "] ";
	}
	fivemcomment += comment.description;
	let timeout = 4000;
	if (fivemcomment.length < 30) {
		timeout = 4000;
	} else if (fivemcomment.length < 60) {
		timeout = 6000;
	} else if (fivemcomment.length < 90) {
		timeout = 8000;
	} else {
		timeout = 10000;
	} 
	
	for (const l of gamelicenses.filter(x => x.gamelicense != sourceusergamelicense)) {
		const playerId = getPlayerFromLicense(l.gamelicense);
		
		if (playerId != -1) {
			const commenthtml = "<strong>New Comment - CAD " + escapeHtml(comment.cad_ref) + "</strong><br/><div class=\"cadfield\"><strong>" + escapeHtml(comment.callsign || '') + " - " + escapeHtml(comment.creatorfull) + ":</strong> " + escapeHtml(fivemcomment);

			getPlayerFmsData(playerId, (fmsdata) => {
				const notiBranch = fmsdata != null ? (fmsdata.currentPatrolBranch || fmsdata.branch) : null;
				const notiDivision = fmsdata != null ? (fmsdata.currentPatrolBranchDivision || fmsdata.branchDivision) : null;
				emitNet('pNotify:SendNotification', playerId, {
					layout: "centerRight", theme: "fms", progressBar: false,
					text: generateNotificationHtml(commenthtml, notiBranch, notiDivision),
					type: "alert", timeout
				});
			});
		}
	}
});

socket.on("updatecad", (data) => {
	if (data == null || data.cad == null) {
		return;
	}

	const gamelicenses = data.gamelicenses;
	const cad = data.cad;
	for (const l of gamelicenses) {
		const playerId = getPlayerFromLicense(l.gamelicense);
		if (playerId != -1) {
			emitNet("fms:clnuidata", playerId, "updatemdtdata", { updatetype: "updatecad", rcad: cad });
		}
	}
});

function getForms(playerid, data) {
	const gamelicense = getGameLicense(playerid);
	if (!gamelicense || !data) { return; }
	socket.emit('fivemgetforms', gamelicense, (error, response) => {
		if (error != null) {
			emitNet('chatMessage', playerid, 'FMS', [0, 255, 0], error || "Error getting forms.");
		} else {
			emitNet("fms:clnuidata", playerid, "refreshforms", response);
		}
	});
}

onNet("fms:getforms", (data) => {
	getForms(source, data);
});

function getForm(playerid, data) {
	const gamelicense = getGameLicense(playerid);
	if (!gamelicense || !data) { return; }
	socket.emit('fivemgetform', gamelicense, data, (error, response) => {
		if (error != null) {
			emitNet('chatMessage', playerid, 'FMS', [0, 255, 0], error || "Error getting form.");
		} else {
			emitNet("fms:clnuidata", playerid, "loadform", response);
		}
	});
}

onNet("fms:getform", (data) => {
	getForm(source, data);
});

function submitForm(playerid, data) {
	const gamelicense = getGameLicense(playerid);
	if (!gamelicense || !data) { return; }
	socket.emit('fivemsubmitform', gamelicense, data, (error, response) => {
		if (error != null) {
			emitNet("fms:clnuidata", playerid, "submittedform", { error: (error || "Error submitting form.") });
		} else {
			emitNet("fms:clnuidata", playerid, "submittedform", response);
		}
	});
}

onNet("fms:submitform", (data) => {
	submitForm(source, data);
});

function getOpenCads(playerid, data) {
	const gamelicense = getGameLicense(playerid);
	if (!gamelicense || !data) { return; }
	socket.emit('fivemgetopencads', gamelicense, (error, response) => {
		if (error != null) {
			emitNet('chatMessage', playerid, 'FMS', [0, 255, 0], error || "Error getting open CADs (no permission?).");
		} else {
			emitNet("fms:clnuidata", playerid, "refreshopencads", response);
		}
	});
}

onNet("fms:getopencads", (data) => {
	getOpenCads(source, data);
});

function getOpenCad(playerid, data) {
	const gamelicense = getGameLicense(playerid);
	if (!gamelicense || !data) { return; }
	socket.emit('fivemgetopencad', gamelicense, data, (error, response) => {
		if (error != null) {
			emitNet('chatMessage', playerid, 'FMS', [0, 255, 0], error || "Error getting open CAD.");
		} else {
			emitNet("fms:clnuidata", playerid, "loadopencad", response);
		}
	});
}

onNet("fms:getopencad", (data) => {
	getOpenCad(source, data);
});

function selfAttachOpenCad(playerid, data) {
	const gamelicense = getGameLicense(playerid);
	if (!gamelicense || !data) { return; }
	socket.emit('fivemopencadselfattach', gamelicense, data, (error, response) => {
		if (error != null) {
			emitNet('chatMessage', playerid, 'FMS', [0, 255, 0], error || "Error self-attaching to open CAD.");
		}
	});
}

onNet("fms:opencadselfattach", (data) => {
	selfAttachOpenCad(source, data);
});

function getAnprHits(playerid, data) {
	const gamelicense = getGameLicense(playerid);
	if (!gamelicense || !data) { return; }
	socket.emit('fivemgetanprhits', gamelicense, (error, response) => {
		if (error != null) {
			emitNet('chatMessage', playerid, 'FMS', [0, 255, 0], error || "Error getting ANPR hits (no permission?).");
		} else {
			emitNet("fms:clnuidata", playerid, "refreshanprhits", response);
		}
	});
}

onNet("fms:getanprhits", (data) => {
	getAnprHits(source, data);
});

function getAnprMarkers(playerid, data) {
	const gamelicense = getGameLicense(playerid);
	if (!gamelicense || !data) { return; }
	socket.emit('fivemgetanprmarkers', gamelicense, (error, response) => {
		if (error != null) {
			emitNet('chatMessage', playerid, 'FMS', [0, 255, 0], error || "Error getting ANPR markers (no permission?).");
		} else {
			emitNet("fms:clnuidata", playerid, "refreshanprmarkers", response);
		}
	});
}

onNet("fms:getanprmarkers", (data) => {
	getAnprMarkers(source, data);
});

function tabletUpateAnprMarker(playerid, data) {
	const gamelicense = getGameLicense(playerid);
	if (!gamelicense || !data) { return; }
	socket.emit('fivemanprplatemarkerupdate', gamelicense, data, (error, success) => {
		if (error != null) {
			emitNet('chatMessage', playerid, 'FMS', [0, 255, 0], error || "Error updating ANPR marker.");
		} else {
			getAnprMarkers(playerid, data);
		}
	});
}

onNet("fms:TabletUpdateAnprMarker", (data) => {
	tabletUpateAnprMarker(source, data);
});

socket.on("anprhit", (response) => {
	if (!response.anprhit) { return; }
	if (response.notifyallusers == true) {
		return emitNet("fms:clnuidata", -1, "anprhit", { anprhit: response.anprhit });
	}

	const gamelicenses = response.anprgamelicenses;
	for (const l of gamelicenses) {
		const playerId = getPlayerFromLicense(l);
		if (playerId != -1) {
			emitNet("fms:clnuidata", playerId, "anprhit", { anprhit: response.anprhit });
		}
	}
});

function getCustodyForm(playerid, data) {
	const gamelicense = getGameLicense(playerid);
	if (!gamelicense || !data) { return; }
	socket.emit('fivemgetcustodyform', gamelicense, data, (error, response) => {
		if (error != null) {
			emitNet('chatMessage', playerid, 'FMS', [0, 255, 0], error || "Error getting custody form.");
		} else {
			emitNet("fms:clnuidata", playerid, "loadcustodyform", response);
		}
	});
}

onNet("fms:getcustodyform", (data) => {
	getCustodyForm(source, data);
});

function submitCustodyForm(playerid, data) {
	const gamelicense = getGameLicense(playerid);
	if (!gamelicense || !data) { return; }
	socket.emit('fivemsubmitcustodyform', gamelicense, data, (error, response) => {
		if (error != null) {
			emitNet("fms:clnuidata", playerid, "submittedcustodyform", { error: (error || "Error submitting custody form.") });
		} else {
			emitNet("fms:clnuidata", playerid, "submittedcustodyform", response);
		}
	});
}

onNet("fms:submitcustodyform", (data) => {
	submitCustodyForm(source, data);
});

function getMdtInfo(playerid, data) {
	const gamelicense = getGameLicense(playerid);
	if (!gamelicense || !data) { return; }
	socket.emit('fivemgetmdtinfo', gamelicense, (error, response) => {
		if (error != null) {
			emitNet('chatMessage', playerid, 'FMS', [0, 255, 0], error || "Error getting MDT info.");
		} else {
			emitNet("fms:clnuidata", playerid, "loadmdtinfo", response);
		}
	});
}

onNet("fms:getmdtinfo", (data) => {
	getMdtInfo(source, data);
});

function getBookonInfo(playerid, data) {
	const gamelicense = getGameLicense(playerid);
	if (!gamelicense || !data) { return; }
	socket.emit('fivemgetbookoninfo', gamelicense, (error, response1) => {
		if (error != null) {
			emitNet('chatMessage', playerid, 'FMS', [0, 255, 0], error || "Error getting bookon info.");
		} else {
			emitNet("fms:clnuidata", playerid, "loadbookoninfo", response1);
		}
	});
}

onNet("fms:getbookoninfo", (data) => {
	getBookonInfo(source, data);
});

function callsignGroupNextCallsign(playerid, data) {
	const gamelicense = getGameLicense(playerid);
	if (!gamelicense || !data || !data.callsigngroupid) { return; }
	socket.emit('fivemcallsigngroupnextcallsign', gamelicense, data, (error, response) => {
		if (error != null) {
			emitNet('chatMessage', playerid, 'FMS', [0, 255, 0], error || "Error getting callsign group callsign.");
		} 
		emitNet("fms:clnuidata", playerid, "loadcallsigngroupcallsign", response);
	});
}

onNet("fms:callsigngroupnextcallsign", (data) => {
	callsignGroupNextCallsign(source, data);
});

function submitFreeBookonForm(playerid, data) {
	const gamelicense = getGameLicense(playerid);
	if (!gamelicense || !data || !data.data) { return; }
	socket.emit('fivemsubmitfreebookonform', gamelicense, data.data, (error, response) => {
		if (error != null) {
			emitNet('chatMessage', playerid, 'FMS', [0, 255, 0], error || "Error booking on.");
		}
	});
}

onNet("fms:submitfreebookonform", (data) => {
	submitFreeBookonForm(source, data);
});

function getCallstack(playerid, data) {
	const gamelicense = getGameLicense(playerid);
	if (!gamelicense || !data) { return; }
	socket.emit('fivemgetcivcallstack', gamelicense, (error, response) => {
		if (error != null) {
			emitNet('chatMessage', playerid, 'FMS', [0, 255, 0], error || "Error getting callstack.");
		} else {
			emitNet("fms:clnuidata", playerid, "refreshcallstack", response);
		}
	});
}

onNet("fms:getcallstack", (data) => {
	getCallstack(source, data);
});

function getCreatecivcall(playerid, data) {
	const gamelicense = getGameLicense(playerid);
	if (!gamelicense || !data) { return; }
	socket.emit('fivemgetcreatecivcall', gamelicense, (error, response) => {
		if (error != null) {
			emitNet('chatMessage', playerid, 'FMS', [0, 255, 0], error || "Error getting create civ call.");
		} else {
			emitNet("fms:clnuidata", playerid, "refreshcreatecivcall", response);
		}
	});
}

onNet("fms:getcreatecivcall", (data) => {
	getCreatecivcall(source, data);
});

function submitCreatecivcall(playerid, data) {
	const gamelicense = getGameLicense(playerid);
	if (!gamelicense || !data || !data.data) { return; }
	socket.emit('fivemsubmitcreatecivcall', gamelicense, data, (error, response) => {
		emitNet("fms:clnuidata", playerid, "submitcreatecivcallresponse", error);
	});
}

onNet("fms:submitcreatecivcall", (data) => {
	submitCreatecivcall(source, data);
});

onNet("teleportOnJoin", () => {
	const playerid = source;
	const gamelicense = getGameLicense(playerid);

	if (!gamelicense) { return; }
	socket.emit('fivemgetspawnpoint', gamelicense, (success, x, y, z, heading, spawnmodel) => {
		if (success && x && y && z && heading) {
			emitNet('teleportPlayerOnJoin', playerid, x, y, z, heading, spawnmodel);
		}
	});

	if (version >= tabletGlobalSettings.currentFmsInteractionVersion) { return; }
	getPlayerFmsData(playerid, (fmsdata) => {
		if (!fmsdata || !fmsdata.permissions || !fmsdata.permissions.includes(1)) {
			return;
		}

		const latestversionmsg = "Your installed fms resource is older than the latest version and may soon become unsupported. Please update the fms resource. Please follow the setup instructions carefully.";
		emitNet('chatMessage', playerid, 'FMS', [255, 0, 0], latestversionmsg);
		const notiBranch = fmsdata != null ? (fmsdata.currentPatrolBranch || fmsdata.branch) : null;
		const notiDivision = fmsdata != null ? (fmsdata.currentPatrolBranchDivision || fmsdata.branchDivision) : null;
		emitNet('pNotify:SendNotification', playerid, {
			layout: "centerRight", theme: "fms", progressBar: false,
			text: generateNotificationHtml(latestversionmsg, notiBranch, notiDivision),
			type: "alert", timeout: 12000
		});
	});
});

onNet("CameraTech:svFixedANPRHit", (playerId, vrm, vehicle, location, marker, x, y, z) => {
	const gamelicense = getGameLicense(playerId);
	if (!gamelicense) { return; }

	let body = {
		vrm: vrm,
		vehicle: vehicle,
		location: location,
		marker: marker,
		x: x,
		y: y,
		z: z, 
		gamelicense: gamelicense
	};

	if (tabletGlobalSettings.anprscreenshots != 1 || GetResourceState("screenshot-basic-fms") != "started") {
		nfetch(websiteurl + "frameworkapi/newanprhit", {
			method: 'POST',
			body: JSON.stringify(body),
			headers: {
				"api-token": token,
				'Content-Type': 'application/json'
			}
		});//.then(resp => resp.text()).then(resp => { console.log(resp); });
		return;
	}
	
	exports["screenshot-basic-fms"]["requestClientScreenshot"](playerId, { encoding: "jpg", quality: 0.4 }, function(err, data) {
		if (err) {
			console.error("screenshot-basic-fms error in CameraTech:svFixedANPRHit: " + err);
		}
		
		body.screenshotbase64 = data;
		nfetch(websiteurl + "frameworkapi/newanprhit", {
			method: 'POST',
			body: JSON.stringify(body),
			headers: {
				"api-token": token,
				'Content-Type': 'application/json'
			}
		});//.then(resp => resp.text()).then(resp => { console.log(resp); });
	});
});

const mappingdata = {};
onNet("fms:updateposition", (x, y, z, streetname, speed) => {

	const playerid = source;
	const gamelicense = getGameLicense(playerid);
	if (gamelicense) {
		mappingdata[playerid] = { gamelicense, x, y, z, streetname, speed, playername: GetPlayerName(playerid) };
	}
});

function updateWebsiteMapping() {
	setTimeout(() => {
		if (allowedLiveMapIDs != null && allowedLiveMapIDs.length > 0) {
			if (socket.connected) {
				socket.emit('fivemupdatemapping', mappingdata, allowedLiveMapIDs);
			}
			updateWebsiteMapping();
		}
	}, 2500);
}

on("playerConnecting", (name, setKickReason, deferrals) => {
	const playerid = global.source;
	let steamid = null;
	let gameident = null;
	let discord = null;
	let xbl = null;
	let liveid = null;
	let userip = null;

	for (let i = 0; i < GetNumPlayerIdentifiers(playerid); i++) {
		const identifier = GetPlayerIdentifier(playerid, i);

		if (identifier.startsWith('steam:')) {
			steamid = identifier;
		} else if (identifier.startsWith('license:')) {
			gameident = identifier;
		} else if (identifier.startsWith('xbl:')) {
			xbl = identifier;
		} else if (identifier.startsWith('ip:')) {
			userip = identifier;
		} else if (identifier.startsWith('discord:')) {
			discord = identifier;
		} else if (identifier.startsWith('live:')) {
			liveid = identifier;
		}
	}

	deferrals.defer();

	setTimeout(() => {
		deferrals.update("Checking whitelist...");
		console.log("[fms] Checking whitelist for " + steamid + " " + gameident + " " + discord);
		// pretend to be a wait
		setTimeout(() => {

			if (!steamid) {
				const steamApiConvar = GetConvar("steam_webApiKey", "");
				if (steamApiConvar == null || steamApiConvar.trim() == "") {
					console.error("[fms] Your Steam Web API key in your server.cfg is invalid and must be added (please review the customer information - FiveM Integration section).");
					return deferrals.done("FMS: Steam web API key not found in server.cfg. Try again later or contact your community's owner / administrator.");
				} 
				
				console.log("[fms] No valid SteamID - user not running Steam. Or your Steam Web API key in your server.cfg may be invalid and must be refreshed (please review the customer information - FiveM Integration section).");
				return deferrals.done("SteamID not found. Please check that Steam is running before starting FiveM.");
			} else if (!gameident) {
				console.log("[fms] No valid game identifier.");
				return deferrals.done("Game identifier not found. Check FiveM is installed properly.");
			} else if (!discord) {
				console.log("[fms] No valid Discord identifier.");
				return deferrals.done("Discord not found. Check the Discord desktop application is running and linked to FiveM (settings).");
			} else if (!userip) {
				console.log("[fms] No valid IP address.");
				return deferrals.done("IP address not found. Check FiveM is installed properly");
			} else if (!token || token.trim() == "" || !websiteurl || websiteurl.trim() == "") {
				console.error("[fms] Invalid token/website URL, please follow the setup instructions carefully.");
				return deferrals.done("FMS resource has not been setup properly. Try again later.");
			} else if (socket.disconnected) {
				console.error("[fms] Could not connect to the FMS. Ensure the token/website URL are correct and that the fms resource is fully updated. Please follow the setup instructions carefully.");
				return deferrals.done("FMS: Could not connect. Try again later or contact your community's owner / administrator. Possible causes: Resource is outdated or has not been setup properly.");
			} else {
				socket.emit('fivemcheckwhitelisting', name, steamid, gameident, xbl, userip, discord, liveid, (success, msg) => {
					if (success) {
						console.log("[fms] " + name + " whitelist check successful, connecting " + gameident);
						return deferrals.done();
					} else {
						console.log("[fms] Kicking player " + name + " as they are not whitelisted: " + steamid + " " + gameident);
						return deferrals.done(msg || "You are not on the whitelist.");
					}
				});
			}
		}, 0);
	}, 0);
});

on("playerDropped", (reason) => {
	const playerid = source;
	delete mappingdata[playerid];
	delete playerFmsData[playerid];
});

const playerFmsData = {};
function getPlayerFmsData(playerId, callback) {
	getPlayerFmsDataOpts({ playerId }, callback);
}

exports('getPlayerFmsData', getPlayerFmsData);

function getPlayerFmsDataOpts(opts, callback) {
	if (!callback || opts == null || opts.playerId == null) { return; }
	const playerid = opts.playerId;
	const stringedplayerid = playerid.toString();
	const pdata = playerFmsData[stringedplayerid];
	if (pdata && pdata.timestamp && Date.now() - pdata.timestamp < 60 * 1000 && opts.forceRefresh != true) {
		return callback(pdata);
	}

	const gamelicense = getGameLicense(playerid);

	if (gamelicense) {
		socket.emit('fivemgetfmsplayerdata', gamelicense, (success, playerdata) => {
			if (success && playerdata) {
				playerdata.timestamp = Date.now();
				playerFmsData[stringedplayerid] = playerdata;
				return callback(playerdata);
			}
			return callback(null);
		});
	} else {
		console.error("fms:getPlayerFmsDataOpts: invalid playerId.");
		return callback(null);
	}
}

exports('getPlayerFmsDataOpts', getPlayerFmsDataOpts);

function getAllPlayersFmsData(callback) {
	getAllPlayersFmsDataOpts({}, callback);
}

exports('getAllPlayersFmsData', getAllPlayersFmsData);

function getAllPlayersFmsDataOpts(opts, callback) {
	if (!callback) { return; }

	const numPlayers = GetNumPlayerIndices();

	function getPlayerFmsDataPromise(playerId) {
		const finalOpts = Object.assign({ forceRefresh: false }, opts, { playerId });
		return new Promise((resolve, reject) => {
			getPlayerFmsDataOpts(finalOpts, (playerdata) => {
				resolve(playerdata);
			});
		});
	}

	const promises = [];
	for (let i = 0; i < numPlayers; i++) {
		const index = GetPlayerFromIndex(i);
		promises.push(getPlayerFmsDataPromise(index));
	}

	Promise.all(promises).then(() => {
		return callback(playerFmsData);
	});
}

exports('getAllPlayersFmsDataOpts', getAllPlayersFmsDataOpts);

let allTrainingGroupSkillUsers = [];
let skills = {};
let patrolactive = true;

socket.on("fivempushskills", (pskills, pallTrainingGroupSkillUsers, ppatrolactive) => {
	skills = pskills;
	if (patrolactive != ppatrolactive) {
		emit("fivemskillsreset");
		emitNet("fivemskillsreset", -1);
	}
	patrolactive = ppatrolactive;
	allTrainingGroupSkillUsers = pallTrainingGroupSkillUsers;

});

function updateSkillsData() {
	socket.emit('fivemgetskills');
	setTimeout(() => {
		updateSkillsData();
	}, 30 * 1000);
}

emit("fivemskillsreset");
emitNet("fivemskillsreset", -1);

function hasFivemSkillOpts(opts) {
	if (opts == null || opts.playerId == null || isNaN(opts.playerId) || opts.skillName == null || opts.skillName.trim() == "") {
		return false;
	}

	const playerid = opts.playerId;
	const skillname = opts.skillName;
	const alwaysAllowOutsidePatrols = opts.alwaysAllowOutsidePatrols === false ? false : true;
	const gamelicense = getGameLicense(playerid);
	if (allTrainingGroupSkillUsers.includes(gamelicense)) {
		return true;
	} else if (alwaysAllowOutsidePatrols == true && patrolactive == false) {
		return true;
	}

	if (!gamelicense || !skills[skillname]) {
		return false;
	}

	return skills[skillname].includes(gamelicense);
}

exports('hasFivemSkillOpts', (opts) => {
	return hasFivemSkillOpts(opts);
});

exports('hasfivemskill', (playerId, skillName) => {
	return hasFivemSkillOpts({ playerId, skillName });
});

function checkPlayerPermission(opts, callback) {
	if (!callback || typeof callback !== "function" || opts == null || opts.playerId == null || opts.permissionId == null) { return; }
	const playerId = opts.playerId;
	const permissionId = opts.permissionId;
	
	if (opts.alwaysAllowOutsidePatrols == true && !patrolactive) {
		return callback(null, true);
	}

	getPlayerFmsDataOpts({ playerId: playerId }, (fmsdata) => {
		if (!fmsdata || !fmsdata.permissions) {
			return callback(null, false);
		}

		const hasPermission = fmsdata.permissions.some(x => x == permissionId);
		return callback(null, hasPermission);
	});
}

exports('checkPlayerPermission', checkPlayerPermission);


function getPatrolVehicleForPlayer(opts, callback) {
	if (!opts || !callback || typeof callback !== "function") {
		return;
	} else if (opts.playerId == null) {
		return callback("Invalid input");
	}

	const gamelicense = getGameLicense(opts.playerId);
	if (!gamelicense) {
		return callback("Invalid playerId");
	}

	socket.emit('fivemgetpatrolvehicle', gamelicense, opts, (error, patrolvehicle) => {
		if (error) {
			console.error("getPatrolVehicleForPlayer error: " + error);
			return callback(error);
		}

		return callback(null, patrolvehicle);
	});
}

exports('getPatrolVehicleForPlayer', getPatrolVehicleForPlayer);

function patrolvehfunc(source, args) {
	getPatrolVehicleForPlayer({ playerId: source, includeUpcomingEvents: false }, (error, patrolvehicle) => {
		if (error) {
			emitNet('chatMessage', source, 'FMS', [0, 255, 0], "Internal server error.");
		} else if (!patrolvehicle) {
			emitNet('chatMessage', source, 'FMS', [0, 255, 0], "No vehicle found. Are you booked on a started patrol with an assigned vehicle?");
		} else {
			emitNet('fms:spawnvehicle', source, patrolvehicle.spawncode, patrolvehicle.liverynumber, patrolvehicle.plate || "changeme");
			emitNet('chatMessage', source, 'FMS', [0, 255, 0], "Spawning " + patrolvehicle.displayname);
		}
	});
}

RegisterCommand('patrolveh', patrolvehfunc);

function getCadRoute(source) {
	const gamelicense = getGameLicense(source);

	if (gamelicense) {
		socket.emit('fivemgetcadstreetcoords', gamelicense, (success, matchType, streetname, x, y) => {
			if (success) {
				emitNet('fms:setroute', source, x, y);
				if (matchType == 0) {
					emitNet('chatMessage', source, 'FMS', [0, 255, 0], "Destination set to CAD coordinates: (" + x + ", " + y + ")");
				} else if (matchType == 1) {
					emitNet('chatMessage', source, 'FMS', [0, 255, 0], "Destination set to: " + streetname);
				} else {
					emitNet('chatMessage', source, 'FMS', [0, 255, 0], "Destination set to nearest match: " + streetname);
				}
			} else {
				emitNet('chatMessage', source, 'FMS', [0, 255, 0], "Error. Not attached to CAD or unrecognised location.");
			}
		});
	}
}

RegisterCommand('cadroute', getCadRoute);

onNet("fms:cadroute", (data) => {
	getCadRoute(source);
});

function updateCadCoords(playerId, x, y) {
	const gamelicense = getGameLicense(playerId);
	if (x == null || y == null) { return; }

	if (gamelicense) {
		socket.emit('fivemupdatecadcoordinates', gamelicense, x, y, (error) => {
			if (error) {
				emitNet('chatMessage', playerId, 'FMS', [0, 255, 0], "Error: " + error);
			} else {
				emitNet('chatMessage', playerId, 'FMS', [0, 255, 0], "Updated CAD coordinates.");
			}
		});
	}
}

onNet("fms:updatecadcoords", (data) => {
	updateCadCoords(source, data.x, data.y);
});

function speedCameraHit(vrm, cameradetails, speedlimit, speedmeasured, ewsactivated) {

	const playerid = source;
	const gamelicense = getGameLicense(playerid);
	if (gamelicense && patrolactive) {
		socket.emit('fivemspeedcamerahit', gamelicense, vrm, cameradetails, speedlimit, speedmeasured, ewsactivated);
	}
}

onNet("fms:speedcamerahit", speedCameraHit);
onNet("SpeedCameras:SpeedCameraHit", speedCameraHit);

onNet("fms:disccheck", (vrm) => {
	const playerid = source;
	const gamelicense = getGameLicense(playerid);

	const searchstring = vrm.replace(/\s/g, "");
	if (gamelicense) {
		socket.emit('fivemdisccheck', searchstring, (success, vehicledetails) => {
			if (!success) {
				emitNet('chatMessage', playerid, 'FMS', [0, 255, 0], "No disc found.");
			} else {
				let dischtml = "Disc Check on <strong>" + escapeHtml(vehicledetails.vrm) + "</strong><br/>";
				let body = "<p><b>" + escapeHtml(vehicledetails.mot.name) + ":</b> ";
				if (vehicledetails.mot.valid) {
					body += "Valid (Expires " + escapeHtml(vehicledetails.mot.expirydate) + ")";
				} else if (vehicledetails.mot.expirydate) {
					body += "Expired on " + escapeHtml(vehicledetails.mot.expirydate);
				} else {
					body += "No records";
				}

				body += "</p><p><b>" + escapeHtml(vehicledetails.tax.name) + ":</b> ";
				if (vehicledetails.tax.valid) {
					body += "Valid (Expires " + escapeHtml(vehicledetails.tax.expirydate) + ")";
				} else if (vehicledetails.tax.expirydate) {
					body += "Expired on " + escapeHtml(vehicledetails.tax.expirydate);
				} else {
					body += "No records";
				}

				body += "</p><p><b>" + escapeHtml(vehicledetails.insurance.name) + ":</b> ";
				if (vehicledetails.insurance.valid) {
					body += "Valid (Policy from " + escapeHtml(vehicledetails.insurance.startdate) + " to " + escapeHtml(vehicledetails.insurance.enddate) + ")";
				} else if (vehicledetails.insurance.startdate && vehicledetails.insurance.enddate) {
					body += "Invalid (Policy from " + escapeHtml(vehicledetails.insurance.startdate) + " to " + escapeHtml(vehicledetails.insurance.enddate) + ")";
				} else {
					body += "No policy";
				}

				body += "</p>";
				dischtml += body;

				getPlayerFmsData(playerid, (fmsdata) => {
					const notiBranch = fmsdata != null ? (fmsdata.currentPatrolBranch || fmsdata.branch) : null;
					const notiDivision = fmsdata != null ? (fmsdata.currentPatrolBranchDivision || fmsdata.branchDivision) : null;
					emitNet('pNotify:SendNotification', playerid, {
						layout: "centerRight", theme: "fms", progressBar: false,
						text: generateNotificationHtml(dischtml, notiBranch, notiDivision),
						type: "alert", timeout: 10000
					});
				});
			}
		});
	}
});

function toggleRadio(source, args) {
	
	const gamelicense = getGameLicense(source);
	if (gamelicense) {
		socket.emit('fivemtoggleradio', gamelicense, (success, newcomms) => {
			if (!success) {
				emitNet('chatMessage', source, 'FMS', [0, 255, 0], "Error. Are you booked on with a valid callsign? Are the channel group IDs set?");
			}
		});
	}
}

RegisterCommand('radio', toggleRadio);

function setCommsGroup(opts, callback) {
	if (!callback || typeof callback !== "function") {
		return;
	} else if (opts.playerId == null || opts.commsGroup == null) {
		return callback("Invalid input");
	}

	const gamelicense = getGameLicense(opts.playerId);
	if (!gamelicense) {
		return callback("Invalid playerId");
	}

	socket.emit('fivemsetcommsgroup', gamelicense, opts.commsGroup, (error, newCommsGroupName) => {
		if (error || !newCommsGroupName) {
			return callback(error || "Internal server error.");
		}

		return callback(null, newCommsGroupName);
	});
}

exports('setCommsGroup', setCommsGroup);

on("fms:SetCommsGroup", (playerId, commsGroup) => {
	setCommsGroup({ playerId: playerId, commsGroup: commsGroup }, (error) => {
		if (error) {
			console.error("fms:SetCommsGroup error: " + error);
		}
	});
});

onNet("fms:TabletSetCommsGroup", (data) => {
	if (!data || !data.commsGroup) { return; }
	setCommsGroup({ playerId: source, commsGroup: data.commsGroup }, (error) => {
		if (error) {
			console.error("SetCommsGroup error: " + error);
		}
	});
});

function setCommsCmd(source, args) {
	if (!args || !args[0]) {
		emitNet('chatMessage', source, 'FMS', [0, 255, 0], "Error. No Comms Group specified.");
		return;
	}

	const commsGroup = args.join(" ").trim();
	const opts = {
		playerId: source,
		commsGroup: commsGroup
	}

	setCommsGroup(opts, (error, newCommsGroupName) => {
		if (error) {
			emitNet('chatMessage', source, 'FMS', [0, 255, 0], "Error setting comms group: " + error);
		}
	});
}

RegisterCommand('comms', setCommsCmd);

function carChannel(source) {
	carChannelOpt(source, false);
}

function carChannelOpt(source, hideMsg) {
	const gamelicense = getGameLicense(source);

	if (gamelicense) {
		socket.emit('fivemcarchannel', gamelicense, (success) => {
			if (!success && !hideMsg) {
				emitNet('chatMessage', source, 'FMS', [0, 255, 0], "Error. Are you booked on with a valid callsign with a valid channel ID?");
			}
		});
	}
}

on("fms:CarChannel", (playerId) => {
	carChannelOpt(playerId, false);
});

onNet("fms:CarChannel", () => {
	carChannelOpt(source, false);
});

RegisterCommand('carc', carChannel);

function incChannel(source) {
	const gamelicense = getGameLicense(source);

	if (gamelicense) {
		socket.emit('fivemincchannel', gamelicense, (success) => {
			if (!success) {
				emitNet('chatMessage', source, 'FMS', [0, 255, 0], "Error. Are you booked on with a valid callsign and attached to a CAD with a valid channel?");
			}
		});
	}
}

on("fms:IncidentChannel", (playerId) => {
	incChannel(playerId);
});
onNet("fms:IncidentChannel", () => {
	incChannel(source);
});

RegisterCommand('inc', incChannel);



function eventChannel(source, eventchannelnumber) {
	const gamelicense = getGameLicense(source);
	if (gamelicense) {
		socket.emit('fivemeventchannel', gamelicense, eventchannelnumber, (success) => {
			if (!success) {
				emitNet('chatMessage', source, 'FMS', [0, 255, 0], "Error. Are you attached to a CAD with a valid temp channel? Event channel number between 1-5?");
			}
		});
	}
}

function eventChannelCmd(source, args) {
	if (!args || !args[0] || isNaN(args[0])) {
		emitNet('chatMessage', source, 'FMS', [0, 255, 0], "Error. No valid event channel number specified.");
		return;
	}
	
	const eventchannelnumber = args[0];
	eventChannel(source, eventchannelnumber);
}

on("fms:EventChannel", (playerId, eventChannelNumber) => {
	if (!eventChannelNumber) {
		console.error("fms:EventChannel: invalid input");
		return;
	}
	eventChannel(playerId, eventChannelNumber);
});

onNet("fms:TabletEventChannel", (data) => {
	if (!data || !data.eventChannelNumber) {
		console.error("fms:TabletEventChannel: invalid input");
		return;
	}
	eventChannel(source, data.eventChannelNumber);
});

RegisterCommand('event', eventChannelCmd);

function moveToChannel(opts, callback) {
	if (!callback || typeof callback !== "function") {
		return;
	} else if (opts.playerId == null || opts.channelName == null) {
		return callback("Invalid input");
	}

	const gamelicense = getGameLicense(opts.playerId);
	if (!gamelicense) {
		return callback("Invalid playerId");
	}

	socket.emit('fivemmovetochannel', gamelicense, opts.channelName, (error, newChannelName) => {
		if (error || !newChannelName) {
			return callback(error || "Internal server error.");
		}

		return callback(null, newChannelName);
	});
}

exports('moveToChannel', moveToChannel);

on("fms:MoveToChannel", (playerId, channelName) => {
	moveToChannel({ playerId: playerId, channelName: channelName }, (error) => {
		if (error) {
			console.error("fms:MoveToChannel error: " + error);
		}
	});
});

function moveToChannelCmd(source, args) {
	if (!args || !args[0]) {
		emitNet('chatMessage', source, 'FMS', [0, 255, 0], "Error. No channel name specified.");
		return;
	}

	const channelName = args.join(" ").trim();
	const opts = {
		playerId: source,
		channelName: channelName
	}

	moveToChannel(opts, (error, newChannelName) => {
		if (error) {
			emitNet('chatMessage', source, 'FMS', [0, 255, 0], "Error moving to channel: " + error);
		} else {
			emitNet('chatMessage', source, 'FMS', [0, 255, 0], "Moved to channel " + newChannelName + ".");
		}
	});
}

RegisterCommand('channel', moveToChannelCmd);

function urgentCallback(playerId) {
	const gamelicense = getGameLicense(playerId);
	if (gamelicense) {
		socket.emit('fivemurgentcallback', gamelicense, (success) => {
			if (!success) {
				emitNet('chatMessage', playerId, 'FMS', [0, 255, 0], "Error. Are you booked on CAD?");
			} else {
				getPlayerFmsDataOpts({ playerId }, (fmsdata) => {
					const notiBranch = fmsdata != null ? (fmsdata.currentPatrolBranch || fmsdata.branch) : null;
					const notiDivision = fmsdata != null ? (fmsdata.currentPatrolBranchDivision || fmsdata.branchDivision) : null;
					let bookedhtml = "Urgent callback requested.";
					emitNet('pNotify:SendNotification', playerId, {
						layout: "centerRight", theme: "fms", text: generateNotificationHtml(bookedhtml, notiBranch, notiDivision),
						type: "alert",
						timeout: 3000, progressBar: false
					});
				});
			}
		});
	}
}

RegisterCommand('uc', urgentCallback);
RegisterCommand('urgentcallback', urgentCallback);

onNet("fms:urgentcallback", () => {
	urgentCallback(source);
});

function disableUserWhitelist(playerid, note, callback) {
	const gamelicense = getGameLicense(playerid);

	if (gamelicense && note) {
		socket.emit('fivemdisablewhitelist', gamelicense, note, (success) => {
			if (!success) {
				console.error("fivemdisablewhitelist - error");
			}

			if (callback) {
				callback(success);
			}
		});
	} else if (callback) {
		callback(false);
	}
}

exports('disableUserWhitelist', disableUserWhitelist);

function doStreetSearch(source, args) {
	const gamelicense = getGameLicense(source);
	const street = args.join(" ").trim();

	if (gamelicense) {
		if (!street || street == "") {
			emitNet('fms:clearroute', source);
			emitNet('chatMessage', source, 'FMS', [0, 255, 0], "Destination cleared.");
			return;
		}

		socket.emit('fivemstreetsearch', street, (success, exactMatch, streetname, x, y) => {
			if (success) {
				emitNet('fms:setroute', source, x, y);
				if (exactMatch) {
					emitNet('chatMessage', source, 'FMS', [0, 255, 0], "Destination set to: " + streetname);
				} else {
					emitNet('chatMessage', source, 'FMS', [0, 255, 0], "Destination set to nearest match: " + streetname);
				}
			} else {
				emitNet('chatMessage', source, 'FMS', [0, 255, 0], "Unrecognised location. Destination cleared.");
			}
		});
	}
}

RegisterCommand('gps', doStreetSearch);

function doesPlateExistOnPnc(plate, callback) {
	if (!plate || !callback) {
		return;
	}

	const searchstring = plate.replace(/\s/g, "");
	socket.emit('fivempncvehicle', searchstring, (success) => {
		return callback(success);
	});
};

const addEPNBFunc = function (source, args) {
	if (!args || args.length == 0) { return; }
	const description = args.join(" ");
	const gamelicense = getGameLicense(source);

	if (gamelicense) {
		socket.emit('fivemaddepnb', gamelicense, description, (success) => {
			if (success) {
				const epnbhtml = "New ePNB entry successfully added.";
				getPlayerFmsData(source, (fmsdata) => {
					const notiBranch = fmsdata != null ? (fmsdata.currentPatrolBranch || fmsdata.branch) : null;
					const notiDivision = fmsdata != null ? (fmsdata.currentPatrolBranchDivision || fmsdata.branchDivision) : null;
					emitNet('pNotify:SendNotification', source, {
						layout: "centerRight", theme: "fms", progressBar: false,
						text: generateNotificationHtml(epnbhtml, notiBranch, notiDivision), type: "alert", timeout: 4000
					});
				});
			} else {
				emitNet('chatMessage', source, 'FMS', [0, 255, 0], "Error processing ePNB entry.");
			}
		});
	}
};

RegisterCommand('pnb', addEPNBFunc);
RegisterCommand('epnb', addEPNBFunc);

function quickCadFunc(playerId, description) {
	const gamelicense = getGameLicense(playerId);
	if (gamelicense) {
		socket.emit('fivemquickcad', gamelicense, description, (success) => {
			if (!success) {
				emitNet('chatMessage', playerId, 'FMS', [0, 255, 0], "Error processing quick CAD - are you booked on CAD?");
			} 
		});
	}
};

RegisterCommand('quickcad', (source, args) => {
	if (!args || args.length == 0) { return; }
	const description = args.join(" ");
	quickCadFunc(source, description);
});

onNet("fms:QuickCad", (data) => {
	if (!data.description) {
		return;
	}
	quickCadFunc(source, data.description);
});

exports('doesPlateExistOnPnc', doesPlateExistOnPnc);

function createCad(cad, callback) {
	if (!cad || !callback) {
		console.error("createCad no cad or callback specified.");
		return;
	}

	socket.emit('fivemcreatecad', cad, (error, createdCad) => {
		if (error) {
			console.error(error);
		}

		return callback(error, createdCad);
	});
};

exports('createCad', createCad);

function createPncPerson(person, callback) {
	if (!person || !callback) {
		console.error("createPncPerson no person or callback specified.");
		return;
	}

	if (person.creatorPlayerId != null) {
		person.creatorGameLicense = getGameLicense(person.creatorPlayerId);	
	}

	socket.emit('fivemcreatepncperson', person, (error, id) => {
		if (error) {
			console.error(error);
		}

		return callback(error, id);
	});
};

exports('createPncPerson', createPncPerson);

function createPncVehicle(vehicle, callback) {
	if (!vehicle || !callback) {
		console.error("createPncVehicle no vehicle or callback specified.");
		return;
	}

	if (vehicle.creatorPlayerId != null) {
		vehicle.creatorGameLicense = getGameLicense(vehicle.creatorPlayerId);	
	}

	socket.emit('fivemcreatepncvehicle', vehicle, (error, id) => {
		if (error) {
			console.error(error);
		}

		return callback(error, id);
	});
};

exports('createPncVehicle', createPncVehicle);

function p2pfunc(source, forcenumbers, callback) {
	if (!callback || typeof callback !== "function") {
		return;
	} 

	const gamelicense = getGameLicense(source);
	if (gamelicense) {
		socket.emit('fivemp2p', gamelicense, forcenumbers, callback);
	}
}

on("fms:CreateP2P", (playerId, otherPlayerId) => {
	getPlayerFmsDataOpts({ playerId: otherPlayerId }, (otherPlayerData) => {
		if (!otherPlayerData || !otherPlayerData.forcenumber) {
			console.error("fms:CreateP2P internal server error: otherPlayerId forcenumber not found.");
			return;
		}

		p2pfunc(playerId, [otherPlayerData.forcenumber], (error) => {
			if (error) {
				console.error("fms:CreateP2P error: " + error);
			}
		});
	});
});

function p2pcmd(source, args) {
	if (!args || args.length == 0) { return; }
	const forcenumbers = args.join(" ").split(";");
	if (!forcenumbers || forcenumbers.length == 0) {
		return;
	}

	p2pfunc(source, forcenumbers, (error) => {
		if (error != null) {
			emitNet('chatMessage', source, 'FMS', [0, 255, 0], "Error creating P2P: " + error);
		} 
	});
};

RegisterCommand('p2p', p2pcmd);

onNet("fms:TabletP2P", (data) => {
	if (!data || !data.forcenumbers) { return; }
	p2pfunc(source, data.forcenumbers, (error) => {
		if (error != null) {
			emitNet('chatMessage', source, 'FMS', [0, 255, 0], "Error creating P2P: " + error);
		} 
	});
});

function getAllPatrolVehicles(includeUpcomingEvents, callback) {
	if (!callback || typeof callback !== "function") {
		console.error("getAllPatrolVehicles no callback specified.");
		return;
	} 

	socket.emit('fivemgetallpatrolvehicles', { includeUpcomingEvents: includeUpcomingEvents }, (error, vehicles) => {
		if (error) {
			console.error(error);
		}

		return callback(error, vehicles);
	});
}

exports('getAllPatrolVehicles', getAllPatrolVehicles);

function getAllPncPersonsForPlayer(playerId, callback) {
	if (!callback || typeof callback !== "function") {
		console.error("getAllPncPersonsForPlayer no callback specified.");
		return;
	} 
	const gamelicense = getGameLicense(playerId);
	if (!gamelicense) {
		console.error("getAllPncPersonsForPlayer invalid playerId specified.");
		return;
	}

	socket.emit('fivemgetallpncpersonsforplayer', gamelicense, (error, pncpersons) => {
		if (error) {
			console.error(error);
		}

		return callback(error, pncpersons);
	});
}

exports('getAllPncPersonsForPlayer', getAllPncPersonsForPlayer);

function resetfmstablet(source, args) {
	emitNet("fms:clnuidata", source, "loadtabletdimensions", { });
	emitNet("fms:opentablet", source, "/about");
};

RegisterCommand('resetfmstablet', resetfmstablet);

let vehiclesJsonString = null;
let vehiclesJsonStringLastRefreshed = null;

function getVehiclesJsonString(callback) {
	if (vehiclesJsonString != null && vehiclesJsonStringLastRefreshed != null && Date.now() - vehiclesJsonStringLastRefreshed < 60 * 1000) {
		return callback(null, vehiclesJsonString);
	}

	socket.emit('fivemgetvehiclesjson', (error, vehiclesjson) => {
		if (error) {
			console.error("Error getting vehiclesjson:" + (error || ""));
			return callback(error);
		}
		vehiclesJsonStringLastRefreshed = Date.now();
		vehiclesJsonString = vehiclesjson;
		return callback(null, vehiclesJsonString);
	});
}

exports('getVehiclesJsonString', getVehiclesJsonString);

onNet("VehicleSpawnMenu:RequestCategoriesJsonString", () => {
	const playerid = source;
	getVehiclesJsonString((err, jsonString) => {
		if (!err) {
			emitNet("VehicleSpawnMenu:CategoriesJsonString", playerid, jsonString);
		}
	});
});

let anprVehicleModelJsonString = null;
let anprVehicleModelJsonStringLastRefreshed = null;
function refreshAnprVehicleModelJsonString(playerid = -1) {
	if (playerid != -1 && anprVehicleModelJsonString != null && anprVehicleModelJsonStringLastRefreshed != null && (socket.disconnected || Date.now() - anprVehicleModelJsonStringLastRefreshed < 60 * 1000)) {
		emitNet("CameraTech:ANPRModelsJsonString", playerid, anprVehicleModelJsonString, true);
		return;
	}
	if (socket.connected) {
		socket.emit("fivemgetanprvehiclemodeljsonstring", function(success, jsonString) {
			if (success) {
				anprVehicleModelJsonString = jsonString || "[]";
				anprVehicleModelJsonStringLastRefreshed = Date.now();
				if (playerid == -1) {
					emitNet("CameraTech:ANPRModelsJsonString", -1, anprVehicleModelJsonString, false);
				} else {
					emitNet("CameraTech:ANPRModelsJsonString", playerid, anprVehicleModelJsonString, true);
				}
			} else {
				console.error("Error updating anprVehicleModelJsonString");
			}
		});
	}

	if (playerid == -1) {
		setTimeout(() => {
			refreshAnprVehicleModelJsonString();
		}, 60000);
	}
}

onNet("CameraTech:GetANPRModelsJsonString", () => {
	refreshAnprVehicleModelJsonString(source);
});

on("SmartFires:NewAutomaticFire", AutomaticSmartFireCad);

function AutomaticSmartFireCad(description, x, y) {
	if (GetResourceMetadata("fms", "AutomaticSmartFiresIntegration", 0) == "true") {
		createCad({ type: "Automatic Fire", description: description, location: "SmartFires", x: x, y: y }, (err, createdCad) => { });	
	}
}

function SmartSignsInit() {
	function smartSignUpdate(signId, text) {
		if (signId == null || text == null) {
			return;
		}

		socket.emit("fivemupdatesmartsign", {
			id: signId,
			text: text
		}, (success) => {
			if (!success) {
				console.error("Error updating smart sign.");
			}
		});
	}

	on("SmartSigns:UpdateSign", smartSignUpdate);
	on("SmartSigns:updateSignExternal", smartSignUpdate);

	socket.on('fivempushupdatesmartsign', (signId, text) => {
		emit("SmartSigns:apiUpdateSign", signId, text);
	});

	const signs = exports["SmartSigns"]["SmartSigns:GetSigns"]();
	const smartSigns = [];
	for (let i = 0; i < signs.length; i++) {
		smartSigns.push({
			id: signs[i].id,
			x: signs[i].x,
			y: signs[i].y,
			text: signs[i].defaultText
		});
	}

	socket.emit("fivemloadallsmartsigns", smartSigns, (success) => {
		if (!success) {
			console.error("[fms] Error updating smart sign.");
		}
	});

	console.log("[fms] SmartSigns detected, initialized interaction.");
}

function CameraTechInit() {
	function platemarkerUpdate(playerId, plate, marker) {
		const gamelicense = getGameLicense(playerId);
		if (plate == null) {
			return;
		}

		socket.emit("fivemanprplatemarkerupdate", gamelicense, {
			plate: plate,
			marker: marker || null
		}, (error) => {
			if (error != null) {
				console.error("[fms] fivemanprplatemarkerupdate error.");
			}
		});
	}

	socket.emit("fivemgetanprplatemarkers", (error, plateinfo) => {
		if (error) {
			return console.error("[fms] fivemgetanprplatemarkers error: " + error);
		}

		emit("CameraTech:UpdateAllPlateInfo", plateinfo);
	});

	on("CameraTech:PlateMarkerUpdate", platemarkerUpdate);

	socket.on("anprinfo", (plate, markers) => {
		if (plate != null) {
			emit("CameraTech:UpdateVehicleInfo", plate.replace(/\s/g, "").toUpperCase(), markers);
		}
	});

	socket.on("updateallanprinfo", (plateinfo) => {
		emit("CameraTech:UpdateAllPlateInfo", plateinfo);
	});

	console.log("[fms] CameraTech detected, initialized interaction.");
}

socket.once("connect", () => {
	updateWebsiteMapping();
	updateCustomisableCommands();
	updateSkillsData();
	refreshAnprVehicleModelJsonString();
	updateTabletGlobalSettings();

	if (GetResourceMetadata("fms", "SmartSignsIntegration", 0) == "true") {
		if (GetResourceState("SmartSigns") == "started") {
			SmartSignsInit();
		} else {
			on("onResourceStart", (resourceName) => {
				if (resourceName == "SmartSigns") {
					SmartSignsInit();
				}
			});
		}
	}

	if (GetResourceState("CameraTech") == "started") {
		CameraTechInit();
	} else {
		on("onResourceStart", (resourceName) => {
			if (resourceName == "CameraTech") {
				CameraTechInit();
			}
		});
	}

	on("onResourceStart", (resourceName) => {
		if (resourceName == "CameraTech") {
			socket.emit("fivemgetanprplatemarkers", (error, plateinfo) => {
				if (error) {
					return console.error("[fms] fivemgetanprplatemarkers error: " + error);
				}
		
				emit("CameraTech:UpdateAllPlateInfo", plateinfo);
			});
		}
	});
});