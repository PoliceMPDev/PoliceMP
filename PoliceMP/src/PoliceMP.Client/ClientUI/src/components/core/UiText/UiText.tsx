import * as React from 'react';
import { Color } from 'react-bootstrap/esm/types';
import styled from 'styled-components'
import { UiFont } from '../../../models/enums/Fonts';
import '../../../static/styles/fonts.css'

export class UiTextProps {
    verticalSpacing?: string = '0';
    font?: UiFont = UiFont.Normal;
    fontSize?: string = '24px';
    color?: string = "#FFFFFF";
    children?: React.ReactNode;
}

const StyledText = styled.div<UiTextProps>`
    font-family: ${props => props.font};
    font-size: ${props => props.fontSize ?? '32px'};
    color: ${props => props.color};
    display: inline-block;
    margin: 0;
    margin-right: 10px;
    margin-top: ${props => props.verticalSpacing};
    vertical-align: middle;
`;

export const UiText = (props: UiTextProps) => {
    return <StyledText  {... props}>{props.children}</StyledText>
}