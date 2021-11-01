import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { EventService } from 'src/app/services/event.service';
import { FilterOperator, IFilter, IFilterBase, IFilterGroup, LogicalOperator, PipelinesService } from 'src/app/services/pipelines.service';

export interface PipelineBlock {
  showDetails: boolean;
  title: string;
  properties: string[];
  description: string | null;
};

@Component({
  selector: 'app-pipeline-page',
  templateUrl: './pipeline-page.component.html',
  styleUrls: ['./pipeline-page.component.css']
})
export class PipelinePageComponent implements OnInit {

  public showFilterDetails = true;

  public filter: IFilterBase = {
    $type: "YAB.Core.Pipelines.Filter.FilterGroup, YAB.Core.Pipelines",
    filters: {
      $type: "System.Collections.ObjectModel.ReadOnlyCollection`1[[YAB.Core.Pipelines.Filter.FilterBase, YAB.Core.Pipelines]], System.Private.CoreLib",
      $values: [{
        $type: "YAB.Core.Pipelines.Filter.Filter, YAB.Core.Pipelines",
        filterValue: "0",
        ignoreValueCasing: true,
        operator: 2,
        propertyName: "MinuteOfHour"
      }, {
        $type: "YAB.Core.Pipelines.Filter.Filter, YAB.Core.Pipelines",
        filterValue: "29",
        ignoreValueCasing: true,
        operator: 2,
        propertyName: "MinuteOfHour"
      }]
    },
    operator: 1
  } as IFilterBase;

  public event: PipelineBlock = {
    showDetails: false,
    title: "TwitchMessage",
    properties: ["Message", "UserName"],
    description: "This event is disposed for you when someone sends a twitch message into chat.",
  };

  public eventReactorConfigurations: PipelineBlock[] = [
    {
      showDetails: false,
      title: "TwitchSendMessage",
      properties: ["Message"],
      description: "This will send a message from your twitch bot into chat.",
    },
    {
      showDetails: false,
      title: "BlinkLight",
      properties: ["Duration"],
      description: "This will blink your lights for a specific duration.",
    },
  ];

  constructor(private readonly _pipelinesService: PipelinesService,
    private readonly _eventsService: EventService,
    private readonly _router: Router) { }

  ngOnInit(): void {
  }

  public isFilterBaseFilter(filterBase: IFilterBase) {
    return (filterBase.$type.indexOf('YAB.Core.Pipelines.Filter.Filter,') !== -1);
  }

  public filterBaseAsFilter(filterBase: IFilterBase): IFilter {
    return filterBase as IFilter;
  }

  public isFilterBaseFilterGroup(filterBase: IFilterBase) {
    return (filterBase.$type.indexOf('YAB.Core.Pipelines.Filter.FilterGroup,') !== -1);
  }

  public filterBaseAsFilterGroup(filterBase: IFilterBase): IFilterGroup {
    return filterBase as IFilterGroup;
  }

  public getBorderImageForEventConfigurationBlockAndIndex(index: number) {
    if (this.eventReactorConfigurations.length === 0) {
      return 'linear-gradient(to right, rgb(0, 0, 0) 50%, rgb(0, 0, 0) 50%) 100% 1';
    }
    else if (index === 0) {
      return 'linear-gradient(to right, rgba(255, 255, 255, 0) 50%, rgb(0, 0, 0) 50%) 100% 1';
    }
    else if (index === this.eventReactorConfigurations.length - 1) {
      return 'linear-gradient(to right, rgb(0, 0, 0) 50%, rgba(255, 255, 255, 0) 50%) 100% 1';
    } else {
      return 'linear-gradient(to right, rgb(0, 0, 0) 50%, rgb(0, 0, 0) 50%) 100% 1';
    }
  }


  public stringifyEventFilters(eventFilter: IFilterBase): string {
    if (!eventFilter) {
      return "";
    }
    if (eventFilter.$type.indexOf('YAB.Core.Pipelines.Filter.Filter, ') !== -1) {
      const filter = eventFilter as IFilter;
      const filterOperation = Object.entries(FilterOperator).filter(o => o[1] === filter.operator)[0][0];
      return "event." + filter.propertyName + " " + filterOperation + " \"" + filter.filterValue + "\" (ignoreCasing: " + filter.ignoreValueCasing + ")";
    } else {
      debugger;
      const filterGroup = eventFilter as IFilterGroup;
      const opEntries = Object.entries(LogicalOperator);
      const logicOperator = opEntries.filter(o => o[1] === filterGroup.operator)[0][0];
      return logicOperator;
    }
  }
}

