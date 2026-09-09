import * as React from "react";
import { Meta, StoryObj } from "@storybook/react";
import Centered from "@storybook/addon-centered/react";
import InteractionAction from "./InteractionAction";
import Control from "../../../models/enums/Control";

const meta: Meta<typeof InteractionAction> = {
  title: "Interaction/InteractionAction",
  component: InteractionAction,
  decorators: [Centered],
};

export default meta;
type Story = StoryObj<typeof InteractionAction>;

export const Primary: Story = {
  args: {
    gamepadControl: Control.FrontendAccept,
    keyboardAndMouseControl: Control.FrontendAccept,
    isHold: false,
    text: "Hello World",
  },
  render: (args) => (
    <div style={{ backgroundColor: "#333" }}>
      <InteractionAction {...args} />{" "}
    </div>
  ),
};
