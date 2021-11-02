import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import { EventService } from 'src/app/services/event.service';
import { FilterOperator, IFilter, IFilterBase, IFilterGroup, LogicalOperator, PipelinesService } from 'src/app/services/pipelines.service';
import { PipelineBlock } from 'src/stories/components/pipeline-element/pipeline-element.component';

@Component({
  selector: 'app-pipeline-page',
  templateUrl: './pipeline-page.component.html',
  styleUrls: ['./pipeline-page.component.css']
})
export class PipelinePageComponent implements OnInit {

  public showFilterDetails = false;

  public allowEdit = false;

  public pipelineName: string | null = null;
  public pipelineDescription: string | null = null;
  public filter: IFilterBase | null = null;

  public event: PipelineBlock | null = null;
  public filterBlock: PipelineBlock | null = null;

  public eventReactorConfigurations: PipelineBlock[] = [];

  pipelineId: string | null = null;

  constructor(private readonly _pipelinesService: PipelinesService,
    private readonly _activatedRoute: ActivatedRoute,
    private readonly _eventsService: EventService,
    private readonly _router: Router) { }

  ngOnInit(): void {
    this.pipelineId = this._activatedRoute.snapshot.paramMap.get("guid");

    const pipelineLoader = this._pipelinesService.GetRegisteredPipelineById(this.pipelineId as string);
    forkJoin([pipelineLoader]).subscribe(async res => {
      if (res.some(r => !r.successful)) {
        await this._router.navigateByUrl("/login");
      }
      this.filter = res[0].data.eventFilter;

      const eventFullNameSplits = res[0].data.eventName.split(".");

      this.pipelineName = res[0].data.name;
      this.pipelineDescription = res[0].data.description;

      this.event = {
        title: eventFullNameSplits[eventFullNameSplits.length - 1],
        properties: ["TODO GET ME"],
        description: "TODO GET ME",
      };

      this.filterBlock = {
        title: "Filter",
        description: "Filter allow you to specify, which events should trigger this pipeline.",
        properties: [] // we use a custom template to render the filter.
      };

      this.eventReactorConfigurations = res[0].data.serializedEventReactorConfiguration.$values.map(v => {
        const config = this.stringifyEventReactorConfiguration(v);
        return {
          title: config.typeName,
          properties: config.properties,
          description: "TODO GET ME",
        };
      });
    });
  }

  private stringifyEventReactorConfiguration(configuration: string): { typeName: string, properties: string[] } {
    const parsedConfig = JSON.parse(configuration);
    let type = parsedConfig["$type"].split(", ")[0].split(".")[parsedConfig["$type"].split(", ")[0].split(".").length - 1] as string;
    type = type.substr(0, type.lastIndexOf("ReactorConfiguration"));
    const properties = Object.entries(parsedConfig).filter(t => t[0] !== "$type").map(t => t[0] + ": \"" + t[1] + "\"");

    return { typeName: type, properties: properties };
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
    else if (this.eventReactorConfigurations.length === 1) {
      return 'linear-gradient(to right, rgba(255, 255, 255, 0) 50%, rgba(255, 255, 255, 0) 50%) 100% 1';
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
      const filterGroup = eventFilter as IFilterGroup;
      const opEntries = Object.entries(LogicalOperator);
      const logicOperator = opEntries.filter(o => o[1] === filterGroup.operator)[0][0];
      return logicOperator;
    }
  }
}

