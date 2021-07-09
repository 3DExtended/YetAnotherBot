import { Component, EventEmitter, Input, OnInit, Output, ViewEncapsulation } from '@angular/core';

@Component({
  selector: 'app-input-field',
  templateUrl: './input-field.component.html',
  styleUrls: ['./input-field.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class InputFieldComponent implements OnInit {
  private _value: string = "";

  public get value(): string {
    return this._value;
  }

  public set value(value: string) {
    this._value = value;
    this.valueChanged.next(value);
  }

  @Input() name: string = "";
  @Input() placeholder: string = "";

  @Input() inputType = "text";

  @Output() valueChanged: EventEmitter<string> = new EventEmitter();

  constructor() { }

  ngOnInit(): void {
  }
}
