import { AfterViewInit, Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';

@Component({
  selector: 'app-toggle-switch',
  templateUrl: './toggle-switch.component.html',
  styleUrls: ['./toggle-switch.component.css'],
  // encapsulation: ViewEncapsulation.None
})
export class ToggleSwitchComponent implements OnInit, AfterViewInit {

  @Input() label: string = "";

  @Output() valueChanged: EventEmitter<boolean> = new EventEmitter();

  @ViewChild('inputCheckbox') inputCheckbox: ElementRef | null = null;

  private _value: boolean | null = null;

  public get value(): boolean {
    return this._value === null ? false : this._value;
  }

  @Input()
  public set value(newValue: boolean) {
    const oldValue = this._value;
    this._value = newValue;
    if (oldValue !== null && oldValue !== newValue) {
      this.valueChanged.next(newValue);
    }

    if (this.inputCheckbox) {
      this.inputCheckbox.nativeElement.checked = newValue;
    }
  }

  constructor() { }

  ngAfterViewInit(): void {
    if (this.inputCheckbox) {
      this.inputCheckbox.nativeElement.checked = this.value;
    }
  }

  ngOnInit(): void {
  }

}
