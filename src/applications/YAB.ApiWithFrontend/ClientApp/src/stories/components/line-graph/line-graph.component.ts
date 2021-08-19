import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { ChartDataSets, ChartOptions, ChartType } from "chart.js";
import { LineGraphDataset } from './line-graph-dataset';

@Component({
  selector: 'app-line-graph',
  templateUrl: './line-graph.component.html',
  styleUrls: ['./line-graph.component.css'],
  encapsulation: ViewEncapsulation.None,
})
export class LineGrpahComponent implements OnInit {

  private _graphValues: LineGraphDataset | null = null;
  public get graphValues(): LineGraphDataset | null {
    return this._graphValues;
  }

  @Input()
  height: number = 400;

  @Input()
  lineChartType: ChartType = "line";

  @Input()
  public set graphValues(value: LineGraphDataset | null) {
    this._graphValues = value;
    this.updateLineChartColors();
    this.updateLineChartData();
    this.updateLineChartLabels();
    this.updateLineChartLegend();
    this.updateLineChartOptions();
    const temp = this._graphValues;
    this._graphValues = null;
    this._graphValues = temp;
  }

  constructor() { }

  ngOnInit(): void {
  }

  public lineChartData: ChartDataSets[] = [];
  public updateLineChartData(): void {
    if (!this.graphValues) {
      this.lineChartData = [];
    }

    const result = this.graphValues?.lines.map(l => ({ data: l.values, label: l.label }));
    this.lineChartData = result ?? [];
  }

  public lineChartLabels: string[] = [];
  public updateLineChartLabels(): void {
    if (!this.graphValues) {
      this.lineChartLabels = [];
    }

    this.lineChartLabels = this.graphValues?.xAxisLabels ?? [];
  }

  public lineChartOptions: ChartOptions = {};
  public updateLineChartOptions(): void {
    this.lineChartOptions = {
      responsive: true,
      maintainAspectRatio: false,
      scales: { xAxes: [{ stacked: true }], yAxes: [{ stacked: true }] }
    };
  }

  public lineChartColors: any[] = [];
  public updateLineChartColors(): void {
    if (!this.graphValues) {
      this.lineChartColors = [];
    }

    this.lineChartColors = this.graphValues?.lines.map(l => l.lineColorSettings) ?? [];
  }

  public lineChartLegend = true;
  public updateLineChartLegend() {
    this.lineChartLegend = this.graphValues ? this.graphValues.showChartLegend : false;
  }



  public lineChartPlugins: any[] = [];

}
