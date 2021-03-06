import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';

export interface TableColumn {
  title: string;
  singleLineRow: boolean;
  selector: string;
  widthInPixels: number;
  sort: 'asc' | 'desc' | null
}

export interface TableRow {
  [key: string]: string;
}

@Component({
  selector: 'app-table',
  templateUrl: './table.component.html',
  styleUrls: ['./table.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class TableComponent implements OnInit {
  private _data: { columns: TableColumn[]; dataItems: TableRow[]; } = { columns: [], dataItems: [] };

  public get data(): { columns: TableColumn[]; dataItems: TableRow[]; } {
    return this._data;
  }

  @Input()
  public set data(value: { columns: TableColumn[]; dataItems: TableRow[]; }) {
    this._data = value;
    this._data.columns.forEach(c => {
      const castedColumn = c as any;
      castedColumn.widthBeforeResizing = c.widthInPixels;
    })
  }

  constructor() { }

  ngOnInit(): void {
  }

  public toggleSorting(event: MouseEvent, column: TableColumn) {
    event.preventDefault();

    this._data.columns.forEach(c => {
      if (c.selector === column.selector) {
        if (c.sort === null || c.sort === 'desc') {
          c.sort = 'asc';
        } else {
          c.sort = 'desc'
        }
      } else {
        c.sort = null;
      }
    });

    const sortColumn = this._data.columns.filter(c => c.sort !== null);
    const sortedData = { columns: this._data.columns, dataItems: [...this._data.dataItems] };
    if (sortColumn) {
      sortedData.dataItems = sortedData.dataItems.sort((a, b) => {
        if (a[sortColumn[0].selector] < b[sortColumn[0].selector]) {
          return sortColumn[0].sort === 'asc' ? -1 : 1;
        } else {
          return sortColumn[0].sort === 'asc' ? 1 : -1;
        }
      });
    }

    this.data = sortedData;
  }

  public resizeHandlerClickHandler(event: MouseEvent, mouseDown: boolean, column: TableColumn | any) {
    event.preventDefault();
    column['isResizing'] = mouseDown;
    column['clickedX'] = event.x;
    column['widthBeforeResizing'] = column.widthInPixels;
  }

  public generalResizeHandlerMouseMove(event: MouseEvent) {
    const resizingColumns = this.data.columns.filter(c => (c as any)['isResizing']);
    if (resizingColumns.length > 1 || event.buttons === 0) {
      this.generalResizeHandlerResetHandler();
    }
    if (resizingColumns.length === 1) {
      this.resizeHandlerMouseMove(event, resizingColumns[0]);
    }
  }

  public generalResizeHandlerResetHandler() {
    this.data.columns.forEach(column => {
      (column as any)['isResizing'] = false;
    });
  }

  public resizeHandlerMouseMove(event: MouseEvent, column: TableColumn | any) {
    if (event.buttons === 0) {
      column['isResizing'] = false;
    }

    if (column['isResizing']) {
      if (event.type === "mousemove") {
        column.widthInPixels = event.x - column['clickedX'] + column['widthBeforeResizing'];
      }
    }
  }
}
