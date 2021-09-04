import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

export interface TableColumn {
  title: string;
  singleLineRow: boolean;
  selector: string;
  widthInPixels: number;
  sort: 'asc' | 'desc' | null;
  columnType: TableColumnType;
  customizableInputColumnDefinition?: (dataItem: TableRow) => InputColumnValueType | undefined;
}

export enum TableColumnType {
  // use this for displaying the content as string without edit option
  normal = 'normal',

  // this column will render as toggle switches which can be modified by the user.
  booleanToogleColumn = 'booleanToogleColumn',

  // For each row, there is an lamda expression on the column called, which defines, which input type is expected.
  customizableInputColumn = 'customizableInputColumn'
}

export enum InputColumnValueType {
  string = "string",
  password = "password",
  int = "int",
  floatingPoint = "floatingPoint",
  boolean = "boolean",
}

export interface TableRow {
  [key: string]: string | number | boolean;
}

@Component({
  selector: 'app-table',
  templateUrl: './table.component.html',
  styleUrls: ['./table.component.css'],
  // encapsulation: ViewEncapsulation.None
})
export class TableComponent implements OnInit {
  private _data: { columns: TableColumn[]; dataItems: TableRow[]; } = { columns: [], dataItems: [] };

  public get data(): { columns: TableColumn[]; dataItems: TableRow[]; } {
    return this._data;
  }

  @Input()
  public title: string = "";

  // returns the updated dataItem and the selector of the changed property
  @Output() valueChanged: EventEmitter<{ selector: string; dataItem: TableRow }> = new EventEmitter();

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

  public booleanToogleColumnValueChanged(dataItem: TableRow, column: TableColumn, newValue: boolean) {
    dataItem[column.selector] = newValue;

    this.valueChanged.next({ selector: column.selector, dataItem: dataItem });
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
