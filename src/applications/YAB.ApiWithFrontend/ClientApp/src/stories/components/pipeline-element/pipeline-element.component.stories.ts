import { CommonModule } from '@angular/common';
import {
  FormsModule
} from '@angular/forms';
import { Meta, Story } from '@storybook/angular';
import { PipelineElementComponent } from './pipeline-element.component';

export default {
  title: "Basics / Essentials / Button",
  component: PipelineElementComponent
} as Meta;

export const PrimaryBasic: Story = () => ({
  props: {
    label: "Username",
    primary: true
  },
  moduleMetadata: {
    imports: [
      FormsModule,
      CommonModule
    ]
  }
});

export const PrimaryLongUse: Story = () => ({
  props: {
    label: "UsernameUsernameUsernameUsernameUsernameUsername",
    primary: true
  },
  moduleMetadata: {
    imports: [
      FormsModule,
      CommonModule
    ]
  }
});

export const SecondaryBasic: Story = () => ({
  props: {
    label: "Username",
    primary: false
  },
  moduleMetadata: {
    imports: [
      FormsModule,
      CommonModule
    ]
  }
});

export const SecondaryLongUse: Story = () => ({
  props: {
    label: "UsernameUsernameUsernameUsernameUsernameUsername",
    primary: false
  },
  moduleMetadata: {
    imports: [
      FormsModule,
      CommonModule
    ]
  }
});

// export const Test: Story = (args) => ({
//   props: args,
// });
// Test.argTypes = { onClick: { action: 'On click' } };
