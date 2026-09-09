import Control from "../enums/Control";

export interface InteractionActionMessage
{
    Id: string;
    Text: string;
    GamepadControl: Control;
    MouseAndKeyboardControl: Control;
    IsHold: boolean;
}

export interface InteractionTargetMessage
{
    Name: string;
    Actions: InteractionActionMessage[];
}

