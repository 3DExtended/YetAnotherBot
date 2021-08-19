import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  // encapsulation: ViewEncapsulation.None
})
export class AppComponent {
  public graphData = {
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

  title = 'YetAnotherBotClient';
}
