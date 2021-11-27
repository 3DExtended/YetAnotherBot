import { Component, EventEmitter, Input, Output } from '@angular/core';

export interface DropdownMenuEntry {
  label: string;
  selector: string;
};

@Component({
  selector: 'app-dropdown-menu',
  templateUrl: './dropdown-menu.component.html',
  styleUrls: ['./dropdown-menu.component.css'],
  // encapsulation: ViewEncapsulation.None
})
export class DropdownMenuComponent {
  @Input() entries: DropdownMenuEntry[] = [];
  @Output() valueChanged: EventEmitter<string> = new EventEmitter();

  @Input() leftOfButton = false;
  @Input() topOfButton = false;

  private mouseEnteredContent = false;

  public showOptions: boolean = false;

  public showContent() {
    this.showOptions = true;
    this.mouseEnteredContent = false;
  }

  public hideContent() {
    if (this.mouseEnteredContent) {
      this.showOptions = false;
    }
  }

  public allowMouseLeaveToCloseContent() {
    this.mouseEnteredContent = true;
  }

  public selected(entry: DropdownMenuEntry) {
    this.showOptions = false;
    this.valueChanged.next(entry.selector);
  }

  constructor() { }
}
