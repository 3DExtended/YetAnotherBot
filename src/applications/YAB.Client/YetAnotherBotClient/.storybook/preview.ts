import { setCompodocJson } from '@storybook/addon-docs/angular';
// @ts-ignore
// eslint-disable-next-line import/extensions, import/no-unresolved
import docJson from '../documentation.json';
import "./storybookjs.css";

// remove ButtonComponent to test #12009
const filtered = !docJson?.components
  ? docJson
  : {
    ...docJson,
    components: docJson.components.filter((c) => c.name !== 'ButtonComponent'),
  };
setCompodocJson(filtered);

export const parameters = {
  docs: {
    inlineStories: true,
  },
  options: {
    storySort: {
      order: ['Welcome', 'Core ', 'Addons ', 'Basics '],
    },
  },
  themes: {
    default: 'light',
    clearable: false,
    list: [
      { name: 'light', class: '', color: '#FFFFFF' },
      { name: 'dark', class: 'dark', color: '#000000' }
    ],
  },
};

export const globalTypes = {
  locale: {
    name: 'Locale',
    description: 'Internationalization locale',
    defaultValue: 'en',
    toolbar: {
      icon: 'globe',
      items: [
        { value: 'de', right: '🇩🇪', title: 'Deutsch' },
        { value: 'en', right: '🇺🇸', title: 'English' },
        { value: 'es', right: '🇪🇸', title: 'Español' },
        { value: 'zh', right: '🇨🇳', title: '中文' },
        { value: 'kr', right: '🇰🇷', title: '한국어' },
      ],
    },
  },
};
