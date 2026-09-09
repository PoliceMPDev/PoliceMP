var gamesWindow;
var snakeWindow;
var pongWindow;
var tetrisWindow;
var solitaireWindow;

function showGamesWindow() {
    if (gamesWindow != null) {
        return;
    }
    gamesWindow = Desktop.createWindow({
        resizeable: false,
        draggable: true,
        width: 350,
        height: 380,
        icon: "<span class='fa fa-gamepad'></span>",
        title: "Games",
        content: `
            <div class="container">
                <div class="row">
                    <div class="cell-lg-4">
                        <div class="p-2 clear">
                            <div class="tiles-grid tiles-group size-2">
                                <div onclick="showSnakeWindow()" data-role="tile" class="bg-cyan tile-medium" data-role-tile="true">
                                    <span class="fa fa-square icon"></span>
                                    <span class="branding-bar">Snake</i>
                                    </span>
                                </div>
                                <div onclick="showPongWindow()" data-role="tile" class="bg-red tile-medium" data-role-tile="true">
                                    <span class="fa fa-arrows-h icon"></span>
                                    <span class="branding-bar">Pong</span>
                                </div>
                                <div onclick="showTetrisWindow()" data-role="tile" class="bg-orange tile-medium" data-role-tile="true">
                                    <span class="fa fa-th icon"></span>
                                    <span class="branding-bar">Tetris</span>
                                </div>
                                <div onclick="showSolitaireWindow()" data-role="tile" class="bg-green tile-medium" data-role-tile="true">
                                    <span class="fa fa-files-o icon"></span>
                                    <span class="branding-bar">Solitaire</span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        `,
        onClose: function(win) {
            gamesWindow = null;
        }
    });
}

function showSnakeWindow() {
    if (snakeWindow != null) {
        return;
    }
    snakeWindow = Desktop.createWindow({
        resizeable: true,
        draggable: true,
        icon: "<span class='fa fa-square'></span>",
        title: "Snake",
        clsWindow: 'bg-white',
        content: `<br /><div class="container">
        <iframe src="https://demos.9lessons.info/game.php" frameborder="0" scrolling="no" width="750" height="500" allowfullscreen></iframe>
        </div>
        `,
        onClose: function(win) {
            snakeWindow = null;
        }
    });
}

function showPongWindow() {
    if (pongWindow != null) {
        return;
    }
    pongWindow = Desktop.createWindow({
        resizeable: true,
        draggable: true,
        icon: "<span class='fa fa-arrows-h'></span>",
        title: "Pong",
        clsWindow: 'bg-white',
        content: `<br /><div class="container">
            <iframe src="https://parloti.github.io/PongGame/" frameborder="0" scrolling="no" width="750" height="500" allowfullscreen></iframe>
            <p>Credit: https://parloti.github.io/PongGame/</p>
            </div>
            `,
        onClose: function(win) {
            pongWindow = null;
        }
    });
}

function showTetrisWindow() {
    if (tetrisWindow != null) {
        return;
    }
    tetrisWindow = Desktop.createWindow({
        resizeable: true,
        draggable: true,
        icon: "<span class='fa fa-th'></span>",
        title: "Tetris",
        clsWindow: 'bg-green',
        content: `
            <iframe src="https://chvin.github.io/react-tetris/?lan=en" frameborder="0" scrolling="no" width="1200" height="720" allowfullscreen></iframe>
            
            `,
        onClose: function(win) {
            tetrisWindow = null;
        }
    });
}

function showSolitaireWindow() {
    if (solitaireWindow != null) {
        return;
    }
    solitaireWindow = Desktop.createWindow({
        resizeable: true,
        draggable: true,
        icon: "<span class='fa fa-files-o'></span>",
        title: "Solitaire",
        clsWindow: 'bg-green',
        content: `
            <iframe src="https://www.solitaireforfree.com/" frameborder="0" scrolling="no" width="1200" height="720" allowfullscreen></iframe>            
            `,
        onClose: function(win) {
            solitaireWindow = null;
        }
    });
}