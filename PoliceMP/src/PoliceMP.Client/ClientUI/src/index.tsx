import 'react-devtools'; // Do not move!
import * as React from 'react';
import * as ReactDOM from "react-dom";
import "./static/styles/fonts.css";
import 'bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import App from './App';

var mountNode = document.getElementById("app");
ReactDOM.render(<App />, mountNode);
