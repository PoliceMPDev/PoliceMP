import InputMode from "../models/enums/InputMode";
import * as React from 'react';

export default React.createContext<InputMode>(InputMode.MouseAndKeyboard);