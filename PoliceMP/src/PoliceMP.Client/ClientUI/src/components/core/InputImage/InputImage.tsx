import * as React from 'react'
import styled from 'styled-components';
import { InputImageMapping } from './InputImageMapping';
import blankKey from '../../../static/images/controls/KeyboardMouse/Blanks/Blank_Black_Normal.png'
import { emitPoliceMpEvent } from '../../../hooks/usePoliceMpEvent';
import { GetInstructionalButtonMessage } from '../../../models/messages/GetInstructionalButtonMessage';
import Control from '../../../models/enums/Control';
import InputMethodContext from '../../../context/InputMethodContext';

export interface InputImageProps {
    control: Control;
    size: number;
}

const StyledDiv = styled.div`
    display: inline-block;
    align-items: center;
    position: relative;
`

const StyledImage = styled.img<InputImageProps>`
    vertical-align: middle;
    width: ${props => props.size}px;
    height: ${props => props.size}px;
`

const StyledText = styled.span<InputImageProps>`
    position: absolute;
    left: 50%;
    top: 50%;
    transform: translate(-50%, -52%);
    font-size: ${props => props.size / 3}px;
    font-family: 'Paytone One', sans-serif;
    color: #cdcdcd;
`

const InputImage = (props: InputImageProps) => {    
    let [image, setImage] = React.useState<string>(blankKey);
    let [text, setText] = React.useState('');
    let inputMode = React.useContext(InputMethodContext);
    
    React.useEffect(() => {
        let message = new GetInstructionalButtonMessage(inputMode, props.control);

        emitPoliceMpEvent<string>('GetInstructionalButton', message)
        .then(button => {
            if(button.startsWith('b_'))
            {
                let newImage = InputImageMapping[button];

                if(newImage === undefined)
                {
                    setImage(blankKey);
                    setText(button);
                }
                else {
                    setImage(newImage);
                    setText('');
                }
            }
            // Key mapping
            else if(button.startsWith('t_'))
            {
                let text = button.replace(new RegExp('^t_'), '').toUpperCase();
                console.log(text)
                setImage(_ => blankKey);
                setText(text);
            }
            else {
                setImage(blankKey);
                setText(`${button}???`)
            }
        });
    }, [inputMode, props.control])

    return <StyledDiv>
        <StyledImage src={image} {... props} />
        <StyledText {...props}>{text}</StyledText>
    </StyledDiv> 
}

export default InputImage
