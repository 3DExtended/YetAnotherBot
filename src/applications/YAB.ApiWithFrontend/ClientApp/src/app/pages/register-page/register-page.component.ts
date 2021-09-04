import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { cloneDeep } from 'lodash';
import { forkJoin } from 'rxjs';
import { List } from 'src/app/services/pipelines.service';
import { InstalledPluginTupleDto, PluginService } from 'src/app/services/plugin.service';
import { OptionsDescription, PropertyValueType, RegisterService } from 'src/app/services/register.service';
import { InputColumnValueType, TableColumn, TableColumnType, TableRow } from 'src/stories/components/table/table.component';

export interface TableOfOptionsToFill {
  columns: TableColumn[];
  dataItems: TableRow[];
  title: string;
};

const TableOfOptionsToFillColumns: TableColumn[] = [
  {
    title: 'Setting',
    selector: 'propertyName',
    widthInPixels: 200,
    sort: 'asc',
    singleLineRow: true,
    columnType: TableColumnType.normal
  },
  {
    title: 'Description',
    selector: 'propertyDescription',
    widthInPixels: 500,
    sort: null,
    singleLineRow: false,
    columnType: TableColumnType.normal
  },
  {
    title: 'Value',
    selector: 'value',
    widthInPixels: 300,
    sort: null,
    singleLineRow: true,
    columnType: TableColumnType.customizableInputColumn,
    customizableInputColumnDefinition: (dataItem: TableRow): InputColumnValueType => {
      if (!dataItem) {
        return InputColumnValueType.string;
      }

      if (dataItem.isSecret) {
        return InputColumnValueType.password;
      }

      if (dataItem.valueType === PropertyValueType["string"]) {
        return InputColumnValueType.string;
      }
      if (dataItem.valueType === PropertyValueType["int"]) {
        return InputColumnValueType.int;
      }
      if (dataItem.valueType === PropertyValueType["floatingPoint"]) {
        return InputColumnValueType.floatingPoint;
      }

      // TODO add other datatypes as well (like boolean...)
      return InputColumnValueType.string;
    },
  }
];

@Component({
  selector: 'app-register-page',
  templateUrl: './register-page.component.html',
  styleUrls: ['./register-page.component.css']
})
export class RegisterPageComponent implements OnInit {
  public allPlugins: List<InstalledPluginTupleDto> | null = null;

  public pluginsTableConfig: { columns: TableColumn[]; dataItems: TableRow[]; } = {
    columns: [
      {
        title: 'Installed',
        selector: 'installed',
        widthInPixels: 100,
        sort: null,
        singleLineRow: true,
        columnType: TableColumnType.booleanToogleColumn
      },
      {
        title: 'Plugin-Name',
        selector: 'pluginName',
        widthInPixels: 300,
        sort: 'asc',
        singleLineRow: true,
        columnType: TableColumnType.normal
      },
      {
        title: 'Repository',
        selector: 'repository',
        widthInPixels: 800,
        sort: null,
        singleLineRow: true,
        columnType: TableColumnType.normal
      }
    ],
    dataItems: []
  };

  public tableOfOptionsToFill: TableOfOptionsToFill[] = [];

  public optionsToFill: List<OptionsDescription> | null = null;

  constructor(private readonly _registerService: RegisterService,
    private readonly _pluginService: PluginService,
    private readonly _activatedRoute: ActivatedRoute,
    private readonly _router: Router) { }

  ngOnInit(): void {
    const installedPluginsLoader = this._pluginService.InstalledStatusOfSupportedPlugins();
    const optionsToFillLoader = this._registerService.GetOptionsToFill();
    forkJoin([installedPluginsLoader, optionsToFillLoader]).subscribe((res) => {
      if (res.some(r => !r.successful)) {
        return;
      }
      this.allPlugins = res[0].data;
      this.optionsToFill = res[1].data;

      this.tableOfOptionsToFill = this.optionsToFill.$values.map(v => {
        return {
          columns: cloneDeep(TableOfOptionsToFillColumns),
          dataItems: v.properties.$values.map(pv => {
            return {
              "propertyName": pv.propertyName,
              "propertyDescription": pv.propertyDescription,
              "value": "",
              "isSecret": pv.isSecret,
              "valueType": pv.valueType
            };
          }),
          title: v.optionFullName.split(".")[v.optionFullName.split(".").length - 1],
        };
      });

      this.pluginsTableConfig.dataItems = this.allPlugins.$values.map(p => {
        return {
          "installed": p.item2,
          "pluginName": p.item1.pluginName,
          "repository": p.item1.repositoryUrl,
        };
      })
    });
  }

  public async installedPluginsValueChanged(event: { selector: string, dataItem: TableRow }) {
    if (event.dataItem[event.selector] === true) {
      console.log("install extension");
      await this._registerService.InstallPlugin(event.dataItem['pluginName'].toString()).toPromise();
    } else {
      console.log("uninstall extension");
      // TODO how to uninstall extensions?...
    }
  }
}
