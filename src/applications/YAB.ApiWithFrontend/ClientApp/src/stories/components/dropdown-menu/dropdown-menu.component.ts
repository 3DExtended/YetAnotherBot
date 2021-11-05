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

  public showOptions: boolean = false;

  public showContent() {
    this.showOptions = true;
  }

  public hideContent() {
    this.showOptions = false;
  }

  public selected(entry: DropdownMenuEntry) {
    this.hideContent();
    this.valueChanged.next(entry.selector);
  }

  constructor() { }
}
