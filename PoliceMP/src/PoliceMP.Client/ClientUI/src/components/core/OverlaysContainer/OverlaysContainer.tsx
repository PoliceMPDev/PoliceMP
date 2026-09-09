import * as React from 'react';
import InputMethodContext from '../../../context/InputMethodContext';
import { emitPoliceMpEvent, usePoliceMpEvent } from '../../../hooks/usePoliceMpEvent';
import InputMode from '../../../models/enums/InputMode';
import { OverlayProps } from './Overlay';

export class OverlaysContainerProps
{
    active?: string[];
    children?: React.ReactElement<OverlayProps> | React.ReactElement<OverlayProps>[];
    exclusive?: boolean;
}

export let OverlaysContainer = (props: OverlaysContainerProps) => {

    let [activeChildren, setActiveChildren] = React.useState<string[]>(props.active || []);
    let [inputMode, setInputMode] = React.useState<InputMode>(InputMode.MouseAndKeyboard);

    function getUnique<T>(array: T[])
    {
        return array.filter((v, i, a) => a.indexOf(v) === i);
    }

    usePoliceMpEvent<InputMode>("SetInputMode", mode => {
        console.log("SET INPUT MODE", mode)
        setInputMode(mode);
    })

    usePoliceMpEvent("DisableOverlay", (overlay: string) => {
        console.log("Disabling overlay", overlay)
        setActiveChildren(a => getUnique(a.filter(i => i != overlay)));

        emitPoliceMpEvent("OverlayHidden", overlay);
    });

    usePoliceMpEvent("EnableOverlay", (overlay: string) => {
        console.log("Enabling overlay", overlay)
        if(props.exclusive === true)
        {
            setActiveChildren([ overlay ]);
        }
        else
        {
            setActiveChildren(previousChildren => getUnique([...previousChildren, overlay]));
        }

        emitPoliceMpEvent("OverlayShown", overlay);
    });

    return <>
        <InputMethodContext.Provider value={inputMode}>
            {React.Children.map(props.children, child => {
                if(activeChildren != null && child != null &&
                    activeChildren.includes(child.props.id)){
                        return child;
                }

                return null;
            })}
        </InputMethodContext.Provider>
    </>
}

