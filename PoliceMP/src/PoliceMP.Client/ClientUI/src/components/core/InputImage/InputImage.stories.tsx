import * as React from "react";
import { Meta, StoryObj } from "@storybook/react";
import Centered from "@storybook/addon-centered/react";
import InputImage, { InputImageProps } from "./InputImage";
import imagePreview from "../../../static/images/controls/Xbox/A.png";
import Control from "../../../models/enums/Control";

const meta: Meta<typeof InputImage> = {
  title: "Core/InputImage",
  component: InputImage,
  decorators: [Centered],
};

export default meta;
type Story = StoryObj<typeof InputImage>;

export const Primary: Story = {
  args: {
    control: Control.Sprint,
    size: 32,
  },
  render: (args) => <InputImage {...args} />,
};
