// *****************************************************************************************
// CONTROLLER
// *****************************************************************************************
import DpadUp from "../../../static/images/controls/Xbox/Dpad_Up.png";
import DpadDown from "../../../static/images/controls/Xbox/Dpad_Down.png";
import DpadLeft from "../../../static/images/controls/Xbox/Dpad_Left.png";
import DpadRight from "../../../static/images/controls/Xbox/Dpad_Right.png";
import LeftStickUp from "../../../static/images/controls/Xbox/Left_Stick_Up.png";
import LeftStickDown from "../../../static/images/controls/Xbox/Left_Stick_Down.png";
import LeftStickLeft from "../../../static/images/controls/Xbox/Left_Stick_Left.png";
import LeftStickRight from "../../../static/images/controls/Xbox/Left_Stick_Right.png";
import LeftStickClick from "../../../static/images/controls/Xbox/Left_Stick_Click.png";
import LeftStickUpDown from "../../../static/images/controls/Xbox/Left_Stick_UpDown.png";
import LeftStickLeftRight from "../../../static/images/controls/Xbox/Left_Stick_LeftRight.png";
import RightStickUp from "../../../static/images/controls/Xbox/Right_Stick_Up.png";
import RightStickDown from "../../../static/images/controls/Xbox/Right_Stick_Down.png";
import RightStickLeft from "../../../static/images/controls/Xbox/Right_Stick_Left.png";
import RightStickRight from "../../../static/images/controls/Xbox/Right_Stick_Right.png";
import RightStickClick from "../../../static/images/controls/Xbox/Right_Stick_Click.png";
import RightStickUpDown from "../../../static/images/controls/Xbox/Right_Stick_UpDown.png";
import RightStickLeftRight from "../../../static/images/controls/Xbox/Right_Stick_LeftRight.png";
import A from "../../../static/images/controls/Xbox/A.png";
import B from "../../../static/images/controls/Xbox/B.png";
import X from "../../../static/images/controls/Xbox/X.png";
import Y from "../../../static/images/controls/Xbox/Y.png";
import LB from "../../../static/images/controls/Xbox/LB.png";
import LT from "../../../static/images/controls/Xbox/LT.png";
import RB from "../../../static/images/controls/Xbox/RB.png";
import RT from "../../../static/images/controls/Xbox/RT.png";
import Menu from "../../../static/images/controls/Xbox/Menu.png";
import Windows from "../../../static/images/controls/Xbox/Windows.png";

// *****************************************************************************************
// KEYBOARD & MOUSE
// *****************************************************************************************
import Mouse1 from "../../../static/images/controls/KeyboardMouse/Dark/Mouse_Left_Key_Dark.png";
import Mouse2 from "../../../static/images/controls/KeyboardMouse/Dark/Mouse_Right_Key_Dark.png";
import Mouse3 from "../../../static/images/controls/KeyboardMouse/Dark/Mouse_Simple_Key_Dark.png";
import Mouse4 from "../../../static/images/controls/KeyboardMouse/Dark/Mouse_Simple_Key_Dark.png";
import Mouse5 from "../../../static/images/controls/KeyboardMouse/Dark/Mouse_Simple_Key_Dark.png";
import Mouse6 from "../../../static/images/controls/KeyboardMouse/Dark/Mouse_Simple_Key_Dark.png";
import Mouse7 from "../../../static/images/controls/KeyboardMouse/Dark/Mouse_Simple_Key_Dark.png";
import Mouse8 from "../../../static/images/controls/KeyboardMouse/Dark/Mouse_Simple_Key_Dark.png";
import MouseLeft from "../../../static/images/controls/KeyboardMouse/Dark/Mouse_Simple_Key_Dark_Left.png";
import MouseRight from "../../../static/images/controls/KeyboardMouse/Dark/Mouse_Simple_Key_Dark_Right.png";
import MouseUp from "../../../static/images/controls/KeyboardMouse/Dark/Mouse_Simple_Key_Dark_Up.png";
import MouseDown from "../../../static/images/controls/KeyboardMouse/Dark/Mouse_Simple_Key_Dark_Down.png";
import MouseUpDown from "../../../static/images/controls/KeyboardMouse/Dark/Mouse_Simple_Key_Dark_UpDown.png";
import MouseLeftRight from "../../../static/images/controls/KeyboardMouse/Dark/Mouse_Simple_Key_Dark_LeftRight.png";
import Mouse from "../../../static/images/controls/KeyboardMouse/Dark/Mouse_Simple_Key_Dark.png";
import ScrollUp from "../../../static/images/controls/KeyboardMouse/Dark/Mouse_Middle_Key_Dark_Up.png";
import ScrollDown from "../../../static/images/controls/KeyboardMouse/Dark/Mouse_Middle_Key_Dark_Down.png";
import Scroll from "../../../static/images/controls/KeyboardMouse/Dark/Mouse_Middle_Key_Dark.png";
import Key_Minus from "../../../static/images/controls/KeyboardMouse/Dark/Minus_Key_Dark.png";
import Key_Plus from "../../../static/images/controls/KeyboardMouse/Dark/Plus_Key_Dark.png";
import Key_Period from "../../../static/images/controls/KeyboardMouse/Dark/Period_Key_Dark.png";
import Key_Slash_Back from "../../../static/images/controls/KeyboardMouse/Dark/Slash_Back_Key_Dark.png";
import Key_Slash_Forward from "../../../static/images/controls/KeyboardMouse/Dark/Slash_Forward_Key_Dark.png";
import Key_Asterisk from "../../../static/images/controls/KeyboardMouse/Dark/Asterisk_Key_Dark.png";
import Key_Enter_Tall from "../../../static/images/controls/KeyboardMouse/Dark/Enter_Tall_Key_Dark.png";
import Key_Tab from "../../../static/images/controls/KeyboardMouse/Dark/Tab_Key_Dark.png";
import Key_0 from "../../../static/images/controls/KeyboardMouse/Dark/0_Key_Dark.png";
import Key_1 from "../../../static/images/controls/KeyboardMouse/Dark/1_Key_Dark.png";
import Key_2 from "../../../static/images/controls/KeyboardMouse/Dark/2_Key_Dark.png";
import Key_3 from "../../../static/images/controls/KeyboardMouse/Dark/3_Key_Dark.png";
import Key_4 from "../../../static/images/controls/KeyboardMouse/Dark/4_Key_Dark.png";
import Key_5 from "../../../static/images/controls/KeyboardMouse/Dark/5_Key_Dark.png";
import Key_6 from "../../../static/images/controls/KeyboardMouse/Dark/6_Key_Dark.png";
import Key_7 from "../../../static/images/controls/KeyboardMouse/Dark/7_Key_Dark.png";
import Key_8 from "../../../static/images/controls/KeyboardMouse/Dark/8_Key_Dark.png";
import Key_9 from "../../../static/images/controls/KeyboardMouse/Dark/9_Key_Dark.png";
import Space from "../../../static/images/controls/KeyboardMouse/Dark/Space_Key_Dark.png";

export const InputImageMapping: Record<string, string> = {
  // *****************************************************************************************
  // CONTROLLER
  // *****************************************************************************************
  b_4: DpadUp,
  b_5: DpadDown,
  b_6: DpadLeft,
  b_7: DpadRight,
  b_12: LeftStickUp,
  b_13: LeftStickDown,
  b_14: LeftStickLeft,
  b_15: LeftStickRight,
  b_16: LeftStickClick,
  b_18: LeftStickUpDown,
  b_19: LeftStickLeftRight,
  b_21: RightStickUp,
  b_22: RightStickDown,
  b_23: RightStickLeft,
  b_24: RightStickRight,
  b_25: RightStickClick,
  b_27: RightStickUpDown,
  b_28: RightStickLeftRight,
  b_30: A,
  b_31: B,
  b_32: X,
  b_33: Y,
  b_34: LB,
  b_35: LT,
  b_36: RB,
  b_37: RT,
  b_38: Menu,
  b_39: Windows,

  // *****************************************************************************************
  // KEYBOARD & MOUSE
  // *****************************************************************************************
  b_100: Mouse1,
  b_101: Mouse2,
  b_102: Mouse3,
  b_103: Mouse4,
  b_104: Mouse5,
  b_105: Mouse6,
  b_106: Mouse7,
  b_107: Mouse8,
  b_108: MouseLeft,
  b_109: MouseRight,
  b_110: MouseUp,
  b_111: MouseDown,
  b_112: MouseLeftRight,
  b_113: MouseUpDown,
  b_114: Mouse,
  b_115: ScrollUp,
  b_116: ScrollDown,
  b_117: Scroll,
  b_130: Key_Minus,
  b_131: Key_Plus,
  b_132: Key_Period,
  b_133: Key_Slash_Forward,
  b_134: Key_Asterisk,
  b_135: Key_Enter_Tall,
  b_136: Key_0,
  b_137: Key_1,
  b_138: Key_2,
  b_139: Key_3,
  b_140: Key_4,
  b_141: Key_5,
  b_142: Key_6,
  b_143: Key_7,
  b_144: Key_8,
  b_145: Key_9,
  b_1002: Key_Tab,
  b_2000: Space,
};
