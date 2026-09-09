import * as React from 'react'
import styled from 'styled-components'
import Control from '../../../models/enums/Control'
import { UiText } from '../../core/UiText'
import InputImage from '../../core/InputImage'
import { UiFont } from '../../../models/enums/Fonts'

export interface InteractionActionTargetProps {
    targetName: string
}

const TitleLine = styled.hr`
    height: 3px;
    background: white;
    margin-bottom: 0;
`

const TitleRight = styled.div`
    display: block;
    width: 100%;
    text-align: right;
`

const InteractionActionTarget = (props: InteractionActionTargetProps) => {
    return <div>
        <TitleLine/>
        <TitleRight>
            <UiText font={UiFont.Chalk} fontSize='32px' color="#FFF">{props.targetName}</UiText>
            <InputImage control={Control.Aim} size={64} />
        </TitleRight>
        
    </div>
}

export default InteractionActionTarget
