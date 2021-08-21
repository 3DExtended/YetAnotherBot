import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { forkJoin, Subject, timer } from 'rxjs';
import { map, takeUntil } from 'rxjs/operators';
import { EventLoggingEntryDto, EventService } from 'src/app/services/event.service';
import { FilterOperator, IFilter, IFilterBase, IFilterGroup, List, LogicalOperator, PipelinesService } from 'src/app/services/pipelines.service';
import { LineGraphDataset } from 'src/stories/components/line-graph/line-graph-dataset';
import { TableColumn, TableColumnType, TableRow } from 'src/stories/components/table/table.component';

@Component({
  selector: 'app-dashboard-page',
  templateUrl: './dashboard-page.component.html',
  styleUrls: ['./dashboard-page.component.css']
})
export class DashboardPageComponent implements OnInit {

  private refreshRateOfEventsInMs = 5000;

  public eventGraphValues: LineGraphDataset = {
    lines: [],
    xAxisLabels: [
      "-23h", "-22h", "-21h", "-20h",
      "-19h", "-18h", "-17h", "-16h",
      "-15h", "-14h", "-13h", "-12h",
      "-11h", "-10h", "-9h", "-8h",
      "-7h", "-6h", "-5h", "-4h",
      "-3h", "-2h", "-1h", "-0h",
    ],
    showChartLegend: false,
  };

  private errorRefreshingBotEvents: Subject<boolean> = new Subject<boolean>();

  public pipelineTableConfiguration: { columns: TableColumn[]; dataItems: TableRow[]; } = {
    columns: [
      {
        title: 'Event',
        selector: 'event',
        widthInPixels: 200,
        sort: 'asc',
        singleLineRow: true,
        columnType: TableColumnType.normal
      },
      {
        title: 'Filter',
        selector: 'filter',
        widthInPixels: 400,
        sort: null,
        singleLineRow: true,
        columnType: TableColumnType.normal
      },
      {
        title: 'EventReactorConfigurations',
        selector: 'eventReactorConfigurations',
        widthInPixels: 800,
        sort: null,
        singleLineRow: false,
        columnType: TableColumnType.normal
      }
    ],
    dataItems: []
  };

  public past24HourEvents: List<EventLoggingEntryDto> | null = null;

  constructor(private readonly _pipelinesService: PipelinesService,
    private readonly _eventsService: EventService,
    private readonly _router: Router) { }

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

    timer(0, this.refreshRateOfEventsInMs).pipe(
      takeUntil(this.errorRefreshingBotEvents),
      map(_ => {
        return this._eventsService.EventsOfPast24Hours();
      })
    )
      .subscribe({
        next: status => {
          status.subscribe(async (res) => {
            if (!res.successful) {
              this.errorRefreshingBotEvents.next(false);

              await this._router.navigateByUrl("/login");
              return;
            }
            this.past24HourEvents = res.data;
            var now = new Date();
            var utc_timestamp = Date.UTC(now.getUTCFullYear(), now.getUTCMonth(), now.getUTCDate(),
              now.getUTCHours(), now.getUTCMinutes(), now.getUTCSeconds(), now.getUTCMilliseconds());

            // group events into hours
            let didEventChartChange = false;

            const groupsByEventType: { [key: string]: EventLoggingEntryDto[] } = {};

            this.past24HourEvents.$values.forEach(event => {
              if (event.eventName in groupsByEventType) {
                groupsByEventType[event.eventName].push(event);
              } else {
                groupsByEventType[event.eventName] = [event];
              }
            });

            Object.entries(groupsByEventType).forEach((groupEntry: [string, EventLoggingEntryDto[]], index: number, _) => {
              const groupsByHours: { [key: string]: any[] } = {};


              groupEntry[1].forEach(event => {
                const differenceInHoursToNow = Math.abs((new Date(event.timeOfEvent)).getTime() - utc_timestamp) / 36e5;
                const hourString = "-" + parseInt(differenceInHoursToNow.toString(), 10) + "h";
                if (hourString in groupsByHours) {
                  groupsByHours[hourString].push(event);
                } else {
                  groupsByHours[hourString] = [event];
                }
              });

              if (this.eventGraphValues.lines.length <= index) {
                let colorEnergy = 511;
                const red = parseInt((Math.random() * 255).toString(), 10);
                colorEnergy -= red;
                const green = parseInt((Math.random() * 255).toString(), 10);
                colorEnergy -= green;
                const blue = colorEnergy;

                this.eventGraphValues.lines.push({
                  label: groupEntry[0],
                  lineColorSettings: {
                    backgroundColor: "rgba(" + red + "," + green + "," + blue + ",1)",
                    borderColor: "black"
                  },
                  values: [
                    0, 0, 0, 0,
                    0, 0, 0, 0,
                    0, 0, 0, 0,
                    0, 0, 0, 0,
                    0, 0, 0, 0,
                    0, 0, 0, 0,
                  ]
                });
              }

              this.eventGraphValues.lines[index].values = this.eventGraphValues.xAxisLabels.map((label: string, indexOfLabel: number, array: string[]) => {
                let numberOfEventsForLabel = 0;
                if (label in groupsByHours) {
                  numberOfEventsForLabel = groupsByHours[label].length;
                }

                if (this.eventGraphValues.lines[index].values[indexOfLabel] !== numberOfEventsForLabel) {
                  didEventChartChange = true;
                }

                return numberOfEventsForLabel;
              });
            });

            if (didEventChartChange) {
              this.eventGraphValues = { ...this.eventGraphValues };
            }
          });
        }
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

