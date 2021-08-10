import { Component, OnInit } from '@angular/core';
import { forkJoin } from 'rxjs';
import { FilterOperator, IFilter, IFilterBase, IFilterGroup, LogicalOperator, PipelinesService } from 'src/app/services/pipelines.service';
import { TableColumn, TableRow } from 'src/stories/components/table/table.component';

@Component({
  selector: 'app-dashboard-page',
  templateUrl: './dashboard-page.component.html',
  styleUrls: ['./dashboard-page.component.css']
})
export class DashboardPageComponent implements OnInit {

  public pipelineTableConfiguration: { columns: TableColumn[]; dataItems: TableRow[]; } = {
    columns: [
      {
        title: 'Event',
        selector: 'event',
        widthInPixels: 200,
        sort: 'asc',
        singleLineRow: true
      },
      {
        title: 'Filter',
        selector: 'filter',
        widthInPixels: 400,
        sort: null,
        singleLineRow: true
      },
      {
        title: 'EventReactorConfigurations',
        selector: 'eventReactorConfigurations',
        widthInPixels: 800,
        sort: null,
        singleLineRow: false
      }
    ],
    dataItems: []
  };

  constructor(private readonly _pipelinesService: PipelinesService) { }

  ngOnInit(): void {
    const botStatusLoader = this._pipelinesService.GetRegisteredPipelines();
    forkJoin([botStatusLoader]).subscribe(res => {
      console.log(res);
      this.pipelineTableConfiguration.dataItems = res[0].data.$values.map(
        p => {
          return {
            'event': p.eventName.split(".")[p.eventName.split(".").length - 1],
            'filter': this.stringifyEventFilters(p.eventFilter),
            'eventReactorConfigurations': p.serializedEventReactorConfiguration.$values.map(v => this.stringifyEventReactorConfiguration(v)).join(',\r\n')
          } as TableRow;
        }
      );
    });
  }

  private stringifyEventReactorConfiguration(configuration: string) {
    const parsedConfig = JSON.parse(configuration);
    const type = parsedConfig["$type"].split(", ")[0].split(".")[parsedConfig["$type"].split(", ")[0].split(".").length - 1];
    return type + ": {" + Object.entries(parsedConfig).filter(t => t[0] !== "$type").map(t => "\"" + t[0] + "\": \"" + t[1] + "\"").join(", ") + "}";
  }

  private stringifyEventFilters(eventFilter: IFilterBase): string {
    if (eventFilter.$type.indexOf('YAB.Core.Pipelines.Filter.Filter, ') !== -1) {
      const filter = eventFilter as IFilter;
      const filterOperation = Object.entries(FilterOperator).filter(o => o[1] === filter.operator)[0][0];
      return filter.propertyName + " " + filterOperation + " \"" + filter.filterValue + "\" (ignoreCasing: " + filter.ignoreValueCasing + ")";
    } else {
      const filterGroup = eventFilter as IFilterGroup;
      const filterDescriptions = filterGroup.filters.$values.map(v => "(" + this.stringifyEventFilters(v) + ")");
      const logicOperator = Object.entries(LogicalOperator).filter(o => o[1] === filterGroup.operator)[0][0];
      return filterDescriptions.join(" " + logicOperator + " ");
    }
  }
}

