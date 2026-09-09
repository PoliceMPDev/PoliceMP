import * as React from "react";
import styled from "styled-components";

export const OverlayIdContext = React.createContext<string>("");

const StyledOverlay = styled.div`
    position: absolute;
    left: 0;
    top: 0;
    width: 100%;
    height: 100%;
`

export interface OverlayProps
{
    id: string;
    children?: React.ReactNode;
}

export let Overlay = (props: OverlayProps) => {
    return <StyledOverlay>
        <OverlayIdContext.Provider value={props.id}>
            {props.children}
        </OverlayIdContext.Provider>
    </StyledOverlay>
}