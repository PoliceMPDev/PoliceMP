import React, { useState } from 'react';
import './App.css'
import { debugData } from "../utils/debugData";
import { fetchNui } from "../utils/fetchNui";
import { BrowserRouter, Switch, Route, useHistory } from 'react-router-dom';
import { useNuiEvent } from "../hooks/useNuiEvent";

import './views/Regles.scss'

import Home from './views/Home'

// This will set the NUI to visible if we are
// developing in browser
debugData([
    {
        action: 'setVisible',
        data: true,
    }
])

const App: React.FC = () => {
    const history = useHistory()
    useNuiEvent<string>('setPage', (page) => {
        history.push("/" + page)
    })


    return (
        <Switch>
            <Route exact path="/" component={Home} />
            <Route exact path="/web/build/index.html" component={Home} />
        </Switch>
    );
}

export default App;