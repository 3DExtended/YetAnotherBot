import { CommonModule } from '@angular/common';
import { Meta, Story } from '@storybook/angular';
import { TableComponent } from './table.component';

export default {
  title: "Basics / Essentials / Table",
  component: TableComponent
} as Meta;


export const Empty: Story = () => ({
  props: {},
  moduleMetadata: {
    imports: [
      CommonModule
    ]
  }
});


export const Sample: Story = () => ({
  props: {
    data: {
      columns: [
        { title: "Firstname", selector: "firstname", widthInPixels: 150 },
        { title: "Lastname", selector: "lastname", widthInPixels: 100 },
        { title: "Address", selector: "address", widthInPixels: 120 },
        { title: "Birthday", selector: "birthday", widthInPixels: 150 },
      ], dataItems: [
        {
          "firstname": "Tim",
          "lastname": "Sample",
          "address": "New York",
          "birthday": "2021.04.19",
        },
        {
          "firstname": "Max",
          "lastname": "Example",
          "address": "Los Angeles",
          "birthday": "2020.05.01",
        },
        {
          "firstname": "Susanne",
          "lastname": "Master",
          "address": "Berlin",
          "birthday": "2019.03.05",
        },
      ]
    }
  },
  moduleMetadata: {
    imports: [
      CommonModule
    ]
  }
});
