import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import { EventReactorConfiguration, EventReactorConfigurationService } from 'src/app/services/event-reactor-configurations.service';
import { EventService } from 'src/app/services/event.service';
import { IFilter, IFilterBase, IFilterExtension, IFilterGroup, List, LogicalOperator, PipelinesService } from 'src/app/services/pipelines.service';
import { PluginService } from 'src/app/services/plugin.service';
import { OptionsDescription, PropertyDescription } from 'src/app/services/register.service';
import { DropdownMenuEntry } from 'src/stories/components/dropdown-menu/dropdown-menu.component';
import { PipelineBlock, PipelineBlockIcon } from 'src/stories/components/pipeline-element/pipeline-element.component';
import { TableOfOptionsToFill, TableOfOptionsToFillColumns } from '../register-page/register-page.component';

@Component({
  selector: 'app-pipeline-page',
  templateUrl: './pipeline-page.component.html',
  styleUrls: ['./pipeline-page.component.css']
})
export class PipelinePageComponent implements OnInit {
  public showFilterDetails = false;
  public allowEdit = true;
  public displayNewFilterBlock = false;

  public allBlockIcons: any = {
    filter: PipelineBlockIcon.Filter,
    action: PipelineBlockIcon.Action,
    event: PipelineBlockIcon.Event
  };

  public addNewActionDropdownEntries: DropdownMenuEntry[] = [];
  public addNewFilterParts: DropdownMenuEntry[] = [
    {
      label: 'Event Filter',
      selector: 'eventFilter'
    },
    {
      label: 'Filter Group',
      selector: 'filterGroup'
    },
  ];

  public filterGroupOperatorDropdownEntries: DropdownMenuEntry[] = [
    {
      label: 'And',
      selector: 'and'
    },
    {
      label: 'Or',
      selector: 'or'
    },
  ];

  public pipelineName: string | null = null;
  public pipelineDescription: string | null = null;
  public filter: IFilterBase | null = null;

  public event: PipelineBlock | null = null;
  public filterBlock: PipelineBlock | null = null;

  public tableOfPropertiesOfReactorConfig: TableOfOptionsToFill | null = null;

  public eventReactorConfigurations: PipelineBlock[] = [];

  private eventBases: string[] | null = null;
  private pipelineId: string | null = null;
  private validEventConfigurations: EventReactorConfiguration[] | null = null;
  private lastChosenEventReactorConfigToAdd: EventReactorConfiguration | undefined;
  public allEventsDetails: List<OptionsDescription> | undefined;

  constructor(
    private readonly _pipelinesService: PipelinesService,
    private readonly _pluginsService: PluginService,
    private readonly _activatedRoute: ActivatedRoute,
    private readonly _eventsService: EventService,
    private readonly _eventReactorConfigurationService: EventReactorConfigurationService,
    private readonly _router: Router) { }

  ngOnInit(): void {
    this.pipelineId = this._activatedRoute.snapshot.paramMap.get('guid');

    const pipelineLoader = this._pipelinesService.GetRegisteredPipelineById(this.pipelineId as string);
    const eventDetailsLoader = this._pluginsService.GetAllEventsDetailed();
    const eventReactorConfigurationsLoader = this._eventReactorConfigurationService.GetAllEventReactorConfigurations();
    const registeredPipelineByIdAllowedEventBasesLoader = this._eventReactorConfigurationService.GetRegisteredPipelineByIdAllowedEventBases(this.pipelineId as string);
    forkJoin([pipelineLoader, eventReactorConfigurationsLoader, registeredPipelineByIdAllowedEventBasesLoader, eventDetailsLoader]).subscribe(async res => {
      if (res.some(r => !r.successful)) {
        await this._router.navigateByUrl('/login');
      }
      this.filter = res[0].data.eventFilter;

      this.allEventsDetails = res[3].data;
      const eventFullName = res[0].data.eventName;
      const eventFullNameSplits = eventFullName.split('.');

      this.pipelineName = res[0].data.name;
      this.pipelineDescription = res[0].data.description;

      this.eventBases = res[2].data.$values;
      const allEventConfigurations = res[1].data.$values;
      this.validEventConfigurations = allEventConfigurations.filter(c => this.eventBases?.some(eb => eb === c.eventTypeName));

      this.addNewActionDropdownEntries = this.validEventConfigurations.map(c => {
        const deserializedConfig = JSON.parse(c.seralizedEventReactorConfiguration);

        const label = (deserializedConfig.$type as string).split(', ')[0].split('.').pop() + ' (' + (deserializedConfig.$type as string).split(', ')[1] + ')';
        return {
          label: label,
          selector: deserializedConfig.$type,
        };
      });

      const eventDetails = this.allEventsDetails.$values.filter(v => v.optionFullName === eventFullName)[0];

      this.event = {
        title: eventFullNameSplits[eventFullNameSplits.length - 1],
        properties: eventDetails.properties.$values.filter(p => !p.isSecret).map(p => { return { label: p.propertyName, value: p.propertyDescription } }),
        description: eventDetails.optionFullName,
      };

      this.filterBlock = res[0].data.eventFilter && ((res[0].data.eventFilter as any).filters?.$values?.length > 0 || (res[0].data.eventFilter as IFilterExtension).customFilterConfiguration)
        ? {
          title: 'Filter',
          description: 'Filter allow you to specify, which events should trigger this pipeline.',
          properties: [] // we use a custom template to render the filter.
        }
        : null;

      this.eventReactorConfigurations = res[0].data.serializedEventReactorConfiguration.$values.map(v => {
        const config = this.stringifyEventReactorConfiguration(v);

        const configurationWithDetails = this.validEventConfigurations?.filter(c => c.seralizedEventReactorConfiguration.indexOf(config.uncleanedTypeName) !== -1)[0];
        return {
          title: config.typeName,
          properties: config.properties,
          description: configurationWithDetails?.description ?? '',
        };
      });
    });
  }

  public changeFilterGroupOperatorTo(filterBase: IFilterBase, event: string) {
    if (event === 'and') {
      (filterBase as IFilterGroup).operator = LogicalOperator.and as number;
    } else if (event === 'or') {
      (filterBase as IFilterGroup).operator = LogicalOperator.or as number;
    } else {
      throw new Error('Unknown operator');
    }
  }

  public addNewFilterPartToBase(event: string) {
    if (!this.filter) {
      if (event === 'eventFilter') {
        this.filter = {
          $type: 'YAB.Core.Pipelines.Filter.FilterExtension YAB.Core.Pipelines',
          customFilterConfiguration: {
            "$type": "YAB.Core.Filters.EventPropertyFilterConfiguration, YAB.Core",
            "FilterValue": "15",
            "IgnoreValueCasing": true,
            "Operator": 2,
            "PropertyName": "MinuteOfHour"
          }
        } as IFilterBase;
      } else if (event === 'filterGroup') {
        this.filter = {
          $type: 'YAB.Core.Pipelines.Filter.FilterGroup, YAB.Core.Pipelines',

          filters: {
            $type: 'System.Collections.ObjectModel.ReadOnlyCollection`1[[YAB.Core.Pipelines.Filter.FilterBase, YAB.Core.Pipelines]], System.Private.CoreLib',
            $values: [],
          },

          // instance of LogicalOperator
          operator: 0
        } as IFilterBase;
      }
    }
  }

  public addFilterPartToFilterBase(filterBase: IFilterBase, event: string) {
    const arrayOfFilterPartsToAddPartTo = (filterBase as IFilterGroup).filters.$values;

    if (event === 'eventFilter') {
      arrayOfFilterPartsToAddPartTo.push({
        $type: 'YAB.Core.Pipelines.Filter.FilterExtension YAB.Core.Pipelines',
        filterValue: '29',
        ignoreValueCasing: false,
        propertyName: 'MinuteOfHour',

        // instance of FilterOperator
        operator: 0,
      } as IFilterBase);
    } else if (event === 'filterGroup') {
      arrayOfFilterPartsToAddPartTo.push({
        $type: 'YAB.Core.Pipelines.Filter.FilterGroup, YAB.Core.Pipelines',

        filters: {
          $type: 'System.Collections.ObjectModel.ReadOnlyCollection`1[[YAB.Core.Pipelines.Filter.FilterBase, YAB.Core.Pipelines]], System.Private.CoreLib',
          $values: [],
        },

        // instance of LogicalOperator
        operator: 0
      } as IFilterBase);
    }
  }

  public async saveNewAction() {
    // add confirm button which stores those table values into the backend, adds a new action to the pipeline

    const config = this.stringifyEventReactorConfiguration(this.lastChosenEventReactorConfigToAdd?.seralizedEventReactorConfiguration as string);

    if (!this.lastChosenEventReactorConfigToAdd || !this.tableOfPropertiesOfReactorConfig) {
      return;
    }

    const propertiesFromTable = this.tableOfPropertiesOfReactorConfig.dataItems.map(d => {
      return {
        value: d.value,
        propertyName: d.propertyName
      } as PropertyDescription;
    });

    // make request to backend
    await this._pipelinesService.AddNewActionToPipeline(this.pipelineId ?? '', this.lastChosenEventReactorConfigToAdd, propertiesFromTable).toPromise();

    this.eventReactorConfigurations.push({
      title: config.typeName,
      properties: propertiesFromTable.map(pr => {
        return { label: pr.propertyName, value: pr.value };
      }),
      description: this.lastChosenEventReactorConfigToAdd?.description ?? '',
    });

    this.tableOfPropertiesOfReactorConfig = null;
    this.lastChosenEventReactorConfigToAdd = undefined;
  }

  public isNewActionCompletlyConfigured(): boolean {
    return !this.tableOfPropertiesOfReactorConfig?.dataItems.some(d => !d.value);
  }

  public addNewActionToPipeline(selector: string) {
    const addedActionType = this.validEventConfigurations?.filter(c => c.seralizedEventReactorConfiguration.indexOf(selector) !== -1)[0];
    const config = this.stringifyEventReactorConfiguration(addedActionType?.seralizedEventReactorConfiguration as string);
    const configurationWithDetails = this.validEventConfigurations?.filter(c => c.seralizedEventReactorConfiguration.indexOf(config.uncleanedTypeName) !== -1)[0];

    this.lastChosenEventReactorConfigToAdd = configurationWithDetails;

    this.tableOfPropertiesOfReactorConfig = {
      columns: TableOfOptionsToFillColumns,
      dataItems: (configurationWithDetails?.properties.$values ?? [] as PropertyDescription[]).map(pv => {
        return {
          'propertyName': pv.propertyName,
          'propertyDescription': pv.propertyDescription,
          'value': pv.currentValue ?? '',
          'isSecret': pv.isSecret,
          'valueType': pv.valueType
        };
      }),
      title: 'Add new "' + config.typeName + '"'
    };
  }

  public isFilterBaseFilter(filterBase: IFilterBase) {
    return (filterBase.$type.indexOf('YAB.Core.Pipelines.Filter.FilterExtension') !== -1);
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
    } else if (this.eventReactorConfigurations.length === 1) {
      return 'linear-gradient(to right, rgba(255, 255, 255, 0) 50%, rgba(255, 255, 255, 0) 50%) 100% 1';
    } else if (index === 0) {
      return 'linear-gradient(to right, rgba(255, 255, 255, 0) 50%, rgb(0, 0, 0) 50%) 100% 1';
    } else if (index === this.eventReactorConfigurations.length - 1) {
      return 'linear-gradient(to right, rgb(0, 0, 0) 50%, rgba(255, 255, 255, 0) 50%) 100% 1';
    } else {
      return 'linear-gradient(to right, rgb(0, 0, 0) 50%, rgb(0, 0, 0) 50%) 100% 1';
    }
  }

  public stringifyEventFilters(eventFilter: IFilterBase): string {
    if (!eventFilter) {
      return '';
    }
    if (eventFilter.$type.indexOf('YAB.Core.Pipelines.Filter.FilterExtension, ') !== -1) {
      const filter = eventFilter as IFilterExtension;
      const configuration = filter.customFilterConfiguration;
      const configurationName = configuration.$type.split(", ")[0].split('.')[configuration.$type.split(", ")[0].split('.').length - 1];

      const configurationEntries = Object.entries(configuration).filter(o => o[0] !== "$type");
      return configurationName + ': {' + configurationEntries.map(t => '"' + t[0] + '": "' + t[1] + '"').join(', ') + '}';
    } else {
      const filterGroup = eventFilter as IFilterGroup;
      const opEntries = Object.entries(LogicalOperator);
      debugger;
      const logicOperator = opEntries.filter(o => o[1] === filterGroup.operator)[0][0];
      return logicOperator;
    }
  }

  private stringifyEventReactorConfiguration(configuration: string): { typeName: string, properties: { label: string, value: string }[], uncleanedTypeName: string } {
    const parsedConfig = JSON.parse(configuration);
    const type = parsedConfig['$type'].split(', ')[0].split('.')[parsedConfig['$type'].split(', ')[0].split('.').length - 1] as string;
    const cleanedType = type.substr(0, type.lastIndexOf('ReactorConfiguration'));
    const properties = Object.entries(parsedConfig).filter(t => t[0] !== '$type').map(t => { return { label: t[0] as string, value: t[1] as string } });

    return { typeName: cleanedType, properties: properties, uncleanedTypeName: type };
  }
}
