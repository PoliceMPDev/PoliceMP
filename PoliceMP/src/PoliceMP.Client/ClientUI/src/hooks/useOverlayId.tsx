import { useContext } from "react";
import { OverlayIdContext } from "../components/core/OverlaysContainer/Overlay";
import InputMode from "../models/enums/InputMode";

export function useOverlayId() : string
{
    return useContext(OverlayIdContext);
}