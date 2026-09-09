import * as React from 'react'
import { useState, useEffect } from 'react';
import styled from 'styled-components';
import InputImage from "../../core/InputImage";
import Control from '../../../models/enums/Control';
import { useInputMode } from '../../../hooks/useInputMode';
import InputMode from '../../../models/enums/InputMode';
import { UiFont } from '../../../models/enums/Fonts';
import { UiText } from '../../core/UiText';

export interface InteractionActionProps {
    keyboardAndMouseControl: Control;
    gamepadControl: Control,
    text: string;
    isHold: boolean;
}

const StyledDiv = styled.div`
    display: flex;
    width: fit-content;
    margin-left: auto;
    margin-right: 10px;
    align-items: center;
`


const InteractionAction = (props: InteractionActionProps) => {
    const inputMode = useInputMode();
    const [control, setControl] = useState<Control>(props.keyboardAndMouseControl);

    useEffect(() => {
        const newControl = inputMode == InputMode.MouseAndKeyboard ? props.keyboardAndMouseControl : props.gamepadControl;
        setControl(newControl);
    }, [inputMode])

    return <StyledDiv>
        <UiText color="#FFF" font={UiFont.Chalk} fontSize="24px">
            {props.text}
        </UiText>

        {props.isHold && <UiText font={UiFont.Chalk} color="#FFF" fontSize="24px">
            (hold)
        </UiText>}
        
        <InputImage control={control} size={56}/>
    </StyledDiv>
}

export default InteractionAction
