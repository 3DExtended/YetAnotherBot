import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import { List } from 'src/app/services/pipelines.service';
import { InstalledPluginTupleDto, PluginService } from 'src/app/services/plugin.service';
import { RegisterService } from 'src/app/services/register.service';
import { TableColumn, TableColumnType, TableRow } from 'src/stories/components/table/table.component';

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

  constructor(private readonly _registerService: RegisterService,
    private readonly _pluginService: PluginService,
    private readonly _activatedRoute: ActivatedRoute,
    private readonly _router: Router) { }

  ngOnInit(): void {
    const installedPluginsLoader = this._pluginService.InstalledStatusOfSupportedPlugins();

    forkJoin([installedPluginsLoader]).subscribe((res) => {
      if (res.some(r => !r.successful)) {
        return;
      }
      this.allPlugins = res[0].data;

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
