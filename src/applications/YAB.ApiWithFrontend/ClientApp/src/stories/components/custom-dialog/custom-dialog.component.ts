import { Component, EventEmitter, Input, Output } from '@angular/core';

export interface CustomDialogButtons {
  label: string,
  selector: string,
  primary: boolean | undefined
};

@Component({
  selector: 'app-custom-dialog',
  templateUrl: './custom-dialog.component.html',
  styleUrls: ['./custom-dialog.component.css'],
  // encapsulation: ViewEncapsulation.None
})
export class CustomDialogComponent {
  @Output() valueChanged: EventEmitter<string> = new EventEmitter();

  @Input() buttons: CustomDialogButtons[] = [
    {
      label: "Ok",
      selector: "confirm",
      primary: true,
    },
    {
      label: "Cancel",
      selector: "cancel",
      primary: false,
    }
  ];

  public buttonClicked(selector: string) {
    this.valueChanged.next(selector);
  }

  constructor() { }
}
