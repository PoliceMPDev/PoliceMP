import React, { useState } from "react";
import "./App.css";
import { debugData } from "../utils/debugData";
import { fetchNui } from "../utils/fetchNui";
import { BrowserRouter, Switch, Route, useHistory } from "react-router-dom";
import { useNuiEvent } from "../hooks/useNuiEvent";

import Notif from "./views/Notif";



// This will set the NUI to visible if we are
// developing in browser
debugData([
    {
        action: "setVisible",
        data: true,
    },
]);

const App: React.FC = () => {
    const history = useHistory();
    useNuiEvent<string>("setPage", (page) => {
        history.push("/" + page);
    });

    return (

        <Switch>
            <Route exact path="/" component={Notif} />
            <Route exact path="/web/build/index.html" component={Notif} />

        </Switch>
    );
};

export default App;
