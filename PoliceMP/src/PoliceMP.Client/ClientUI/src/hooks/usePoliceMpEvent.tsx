import { DependencyList, useEffect } from "react";

interface NuiMessage<T = any> {
    EventName: string,
    Message: T
}

declare function GetParentResourceName(): string;

export function usePoliceMpEvent<TResponse = any>(event: string, callback: (message: TResponse) => void, deps?: DependencyList | undefined)
{
    useEffect(() => {
        let callbackInternal = (e: MessageEvent<NuiMessage<TResponse>>) => {
            let payload = e.data;
            if(payload.EventName === undefined) return;
            if(payload.EventName.toLowerCase() === `policemp:${event.toLowerCase()}`){
                callback(e.data.Message);
            }
        }
    
        window.addEventListener('message', callbackInternal);

        return (() => {
            window.removeEventListener('message', callbackInternal);
        })
    }, [])
}

export function emitPoliceMpEvent<TResponse>(eventName: string, message: any): Promise<TResponse> 
{
    eventName = eventName.toLowerCase();
    let messageJson = JSON.stringify(message);

    return fetch(`https://${GetParentResourceName()}/${eventName}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json; charset=UTF-8',
        },
        body: messageJson
    })
    .then(resp => {
        // console.log("Received response:", eventName, resp)
        return resp.json().then(respObj => {
            return respObj;
        })
    })
}