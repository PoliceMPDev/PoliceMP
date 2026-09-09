import * as React from 'react'
import { useState, useEffect, useContext } from 'react'
import styled from 'styled-components'
import { emitOverlayEvent, useOverlayEvent } from '../../../hooks/useOverlayEvent'
import { useOverlayId } from '../../../hooks/useOverlayId'
import { InteractionActionMessage, InteractionTargetMessage } from '../../../models/messages/InteractionHudMessages'
import InteractionAction from './InteractionAction'
import InteractionActionTarget from './InteractionActionTarget'

const StyledHud = styled.div`
    display: block;
    margin: 0;
    padding: 5px;
`

const InteractionContainer = styled.div`
    position: absolute;
    right: 16px;
    bottom: 26px;
`

export interface InteractionHudProps {
}

const InteractionHud = (props: InteractionHudProps) => {
    const [interactionName, setInteractionName] = useState("");
    const [interactionActions, setInteractionActions] = useState<InteractionActionMessage[]>([]);
    const overlayId = useOverlayId();

    useEffect(() => {
        emitOverlayEvent<InteractionTargetMessage>(overlayId, "GetInteractionContext")
            .then(context => {
                setInteractionName(context.Name || "");
                setInteractionActions(context.Actions || []);
            });
    }, []);

    useOverlayEvent<InteractionTargetMessage>(overlayId, "SetInteractionContext", target => {
        setInteractionName(target.Name || "");
        setInteractionActions(target.Actions || [])
    });

    useOverlayEvent(overlayId, "ClearInteractionTarget", () => {
        setInteractionName("");
        setInteractionActions([]);
    });

    return <StyledHud>
        <InteractionContainer>
            {interactionActions.map(actionProps => {
                return <InteractionAction 
                    key={actionProps.Id} 
                    text={actionProps.Text} 
                    isHold={actionProps.IsHold}
                    keyboardAndMouseControl={actionProps.MouseAndKeyboardControl}
                    gamepadControl={actionProps.GamepadControl} 
                />
            })}
            <InteractionActionTarget targetName={interactionName} />
        </InteractionContainer>
    </StyledHud>
}

export default InteractionHud
