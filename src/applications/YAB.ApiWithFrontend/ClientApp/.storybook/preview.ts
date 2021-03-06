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
        { value: 'de', right: 'ð©ðª', title: 'Deutsch' },
        { value: 'en', right: 'ðºð¸', title: 'English' },
        { value: 'es', right: 'ðªð¸', title: 'EspaÃ±ol' },
        { value: 'zh', right: 'ð¨ð³', title: 'ä¸­æ' },
        { value: 'kr', right: 'ð°ð·', title: 'íêµ­ì´' },
      ],
    },
  },
};
