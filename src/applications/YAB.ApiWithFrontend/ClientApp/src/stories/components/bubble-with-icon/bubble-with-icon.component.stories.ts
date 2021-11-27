import { action } from '@storybook/addon-actions';
import { Meta, Story } from '@storybook/angular';
import { BubbleWithIconComponent } from './bubble-with-icon.component';

export default {
  title: "Basics / Popups / Bubbles / Bubble with Icon",
  component: BubbleWithIconComponent
} as Meta;

export const Basic: Story = () => ({
  props: {
    title: "You got a new message",
    message: "Click here to read it!",
    onClick: action('On click'),
  },
});

export const LongUse: Story = () => ({
  props: {
    title: "You got a new message and the programmer forgot that this title should not be too long.",
    message: "Click here to read it! However, you might consider beating the developer for not using proper text here.",
    onClick: action('On click'),
  },
});

export const Test: Story = (args) => ({
  props: args,
});
Test.argTypes = { onClick: { action: 'On click' } };
