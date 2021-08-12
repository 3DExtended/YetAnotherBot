import {
  FormsModule
} from '@angular/forms';
import { Meta, Story } from '@storybook/angular';
import { ToggleSwitchComponent } from './toggle-switch.component';

export default {
  title: "Basics / Essentials / Toggle Switch",
  component: ToggleSwitchComponent
} as Meta;


export const TrueState: Story = () => ({
  props: {
    value: true,
  },
  moduleMetadata: {
    imports: [
      FormsModule
    ]
  }
});

export const FalseState: Story = () => ({
  props: {
    value: false,
  },
  moduleMetadata: {
    imports: [
      FormsModule
    ]
  }
});
