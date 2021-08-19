import { button } from '@storybook/addon-knobs';
import { Meta } from '@storybook/angular';
import { ChartsModule } from 'ng2-charts';
import { LineGraphDataset } from './line-graph-dataset';
import { LineGrpahComponent } from './line-graph.component';

const expandableExampleGraphData: LineGraphDataset = {
  lines: [
    {
      label: "Test Graph",
      lineColorSettings: {
        backgroundColor: "rgba(255,0,0,0.3)",
        borderColor: "black"
      },
      values: [
        5.0,
        10.0,
        2.0
      ]
    }
  ],
  showChartLegend: true,
  xAxisLabels: ["January", "February", "March"]
};

function deepClone<T>(obj: T): T {
  return JSON.parse(JSON.stringify(obj)) as T;
}

const randomBetween = (min: number, max: number) => min + Math.floor(Math.random() * (max - min + 1));

export const withKnobs = () => ({
  component: LineGrpahComponent,
  props: {
    graphValues: expandableExampleGraphData,
    ExpandFirstLine: button('Add random entries to first line', () => {
      expandableExampleGraphData.lines[0].values.push(5.0 * Math.random());
      expandableExampleGraphData.xAxisLabels.push((5.0 * Math.random()).toFixed(2));
    }, ''),
    AddLinesButton: button('Add lines', () => {
      let newLine = deepClone(expandableExampleGraphData.lines[0]);

      const r = randomBetween(0, 255);
      const g = randomBetween(0, 255);
      const b = randomBetween(0, 255);
      const rgba = `rgba(${r},${g},${b},${Math.random()})`;

      newLine.lineColorSettings.backgroundColor = rgba;

      newLine.values = newLine.values.map(v => v * Math.random() * 2.0);

      expandableExampleGraphData.lines.push(newLine)
    }, '')
  },
  moduleMetadata: {
    imports: [
      ChartsModule
    ]
  }
});

export default {
  title: "Basics / Graphs / LineGraph",
  decorators: [withKnobs]
} as Meta;
