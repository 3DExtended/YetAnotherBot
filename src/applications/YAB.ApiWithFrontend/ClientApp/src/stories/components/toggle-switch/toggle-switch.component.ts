import { Component, EventEmitter, Input, OnInit, Output, ViewEncapsulation } from '@angular/core';

@Component({
  selector: 'app-toggle-switch',
  templateUrl: './toggle-switch.component.html',
  styleUrls: ['./toggle-switch.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class ToggleSwitchComponent implements OnInit {

  @Input() label: string = "";
  @Input() defaultState: boolean = false;

  @Output() valueChanged: EventEmitter<boolean> = new EventEmitter();

  private _value: boolean = false;

  public get value(): boolean {
    return this._value;
  }

  @Input()
  public set value(newValue: boolean) {
    this._value = newValue;
    console.log(newValue);
    this.valueChanged.next(newValue);
  }

  constructor() { }

  ngOnInit(): void {
    this.value = this.defaultState;
  }
}
