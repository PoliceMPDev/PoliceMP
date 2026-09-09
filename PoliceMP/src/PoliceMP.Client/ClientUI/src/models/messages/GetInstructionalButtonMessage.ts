import Control from "../enums/Control";
import InputMode from "../enums/InputMode";

export class GetInstructionalButtonMessage
{
    InputMode: InputMode;
    Control: Control;

    constructor(inputMode: InputMode, control: Control)
    {
        this.InputMode = inputMode;
        this.Control = control;
    }
}