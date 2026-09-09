document.addEventListener("DOMContentLoaded", function () {
    document.getElementById("elevatorMenu").style.display = "none"; // Ensure menu is hidden on load
});

window.addEventListener('message', function(event) {
    if (event.data.action === "openMenu") {
        let menu = document.getElementById("elevatorMenu");
        let floorButtons = document.getElementById("floorButtons");
        floorButtons.innerHTML = ""; // Clear previous buttons

        event.data.floors.forEach(floor => {
            let button = document.createElement("button");
            button.innerText = floor.name;
            button.onclick = function() {
                if (floor.coords) {
                    fetch(`https://${GetParentResourceName()}/teleport`, {
                        method: "POST",
                        headers: { "Content-Type": "application/json" },
                        body: JSON.stringify({ location: floor.coords })
                    }).catch(err => console.log("[ERROR] Failed to fetch teleport:", err));
                } else {
                    console.log("[ERROR] Floor coordinates missing:", floor);
                }
                closeMenu();
            };
            
            floorButtons.appendChild(button);
        });

        menu.style.display = "block";
    }
});

function closeMenu() {
    document.getElementById("elevatorMenu").style.display = "none"; // Hide the menu
    fetch(`https://${GetParentResourceName()}/closeMenu`, { method: "POST" });
}
