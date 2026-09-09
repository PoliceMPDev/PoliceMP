import { useContext, useEffect } from "react";
import { OverlayIdContext } from "../components/core/OverlaysContainer/Overlay";
import { emitPoliceMpEvent, usePoliceMpEvent } from "./usePoliceMpEvent";

interface OverlayMessage<T = any> {
    Overlay: string,
    EventName: string,
    Message: T
}

export function useOverlayEvent<TResponse = any>(overlayId: string, event: string, callback: (message: TResponse) => void)
{
    usePoliceMpEvent<OverlayMessage<TResponse>>("OverlayEvent", e => {
        if(e.Overlay.toLowerCase() === overlayId.toLowerCase() && e.EventName.toLowerCase() === event.toLowerCase()){
            // console.log("OverlayEvent", e.Overlay, e.EventName, e.Message);
            callback(e.Message)
        }
    })
}

export function emitOverlayEvent<TResponse>(overlayId: string, event: string, message: any = {}) : Promise<TResponse>
{
    return emitPoliceMpEvent(`${overlayId}/${event}`, message);
}