import { Color } from "ng2-charts";

export interface LineGraphDataset {
  lines: LineGraphLine[];
  xAxisLabels: string[];
  showChartLegend: boolean;
};

export interface LineGraphLine {
  values: number[];
  label: string;
  lineColorSettings: Color;
};
