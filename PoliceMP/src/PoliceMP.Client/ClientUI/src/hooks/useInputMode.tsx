import { useContext } from "react";
import InputMethodContext from "../context/InputMethodContext";
import InputMode from "../models/enums/InputMode";

export function useInputMode() : InputMode
{
    return useContext(InputMethodContext);
}