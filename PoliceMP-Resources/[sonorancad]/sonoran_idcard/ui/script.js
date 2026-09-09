window.addEventListener("message", (e) => {
  if (e.data.type === "SonoranCAD::civint:id") {
    if (e.data.show) {
      const fullName = e.data.fullName;
      const nameArray = fullName.split(" ");
      const firstName = nameArray[0];
      const lastName = nameArray[1];
      const playerID = e.data.playerID;
      const dob = e.data.dob;

      const html = `
        <div id="id-card">
          <div id="top-info">
            <h2 id="state-name">UNITED KINGDOM</h2>
            <h2 id="top-id">DRIVING LICENCE</h2>
          </div>
          <div id="info-wrapper">
            <div id="id-photo">
              <img
                src="${e.data.img}"
                alt="PHOTO"
                id="id-pic"
              />
              <h2 id="signature">${fullName}</h2>
            </div>
            <div id="personal-info">
              <h2>
                <span class="info-text">1.</span>
                <span id="last-name">${lastName}</span>
              </h2>
              <h2>
                <span class="info-text">2.</span>
                <span id="first-name">${firstName}</span>
              </h2>
              <h2>
                <span class="info-text">3.</span
                ><span class="number" id="dob"> ${dob}</span>
              </h2>
              <h2>
                <span class="info-text">4.</span>
                <span class="number" id="player-id"> ${playerID}</span>
              </h2>
            </div>
          </div>
        </div>
`;

      document.querySelector("body").innerHTML = html;

      setTimeout(() => {
        document.querySelector("body").innerHTML = "";
      }, 6000);
    }
  }
});