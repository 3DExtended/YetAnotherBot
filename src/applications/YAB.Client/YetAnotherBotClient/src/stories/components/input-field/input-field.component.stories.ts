import { FormsModule } from '@angular/forms';
import { Meta, Story } from '@storybook/angular';
import { InputFieldComponent } from './input-field.component';

export default {
  title: "Basics / Essentials / Input field",
  component: InputFieldComponent
} as Meta;


export const Basic: Story = () => ({
  props: {
    value: "3DExtended",
    placeholder: "Username",
  },
  moduleMetadata: {
    imports: [
      FormsModule
    ]
  }
});

export const LongUse: Story = () => ({
  props: {
    value: "3DExtended3DExtended3DExtended3DExtended3DExtended",
    placeholder: "Username",
  },
  moduleMetadata: {
    imports: [
      FormsModule
    ]
  }
});

// export const Test: Story = (args) => ({
//   props: args,
// });
// Test.argTypes = { onClick: { action: 'On click' } };
