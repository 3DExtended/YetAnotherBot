import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { Meta, Story } from '@storybook/angular';
import { NgxGraphModule } from '@swimlane/ngx-graph';
import { BubbleWithIconComponent } from '../bubble-with-icon/bubble-with-icon.component';
import { PipelineGraphComponent } from './pipeline-graph.component';

export default {
  title: "Basics / Graphs / Pipeline Graph",
  component: PipelineGraphComponent
} as Meta;


export const Basic: Story = () => ({
  props: {
  },
  styles: [
    "height: 100%",
    "width: 100%"
  ],
  moduleMetadata: {
    declarations: [
      BubbleWithIconComponent
    ],
    imports: [
      NgxGraphModule,
      BrowserModule,
      BrowserAnimationsModule,
    ]
  }
});
