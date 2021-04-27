import { Meta, Story } from '@storybook/angular';
import { NavbarComponent } from './navbar.component';

export default {
  title: "Basics / Essentials / Navbar",
  component: NavbarComponent
} as Meta;


export const LoggedIn: Story = () => ({
  props: {
    loggedIn: true
  },
});

export const LoggedOut: Story = () => ({
  props: {},
});
