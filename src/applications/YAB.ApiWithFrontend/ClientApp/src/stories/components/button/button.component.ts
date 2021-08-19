import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-button',
  templateUrl: './button.component.html',
  styleUrls: ['./button.component.css'],
  // encapsulation: ViewEncapsulation.None
})
export class ButtonComponent implements OnInit {

  @Input() label: string = "";

  @Input() primary = true;

  @Input() disabled = false;

  @Input() bigButton = false;

  @Output() clicked: EventEmitter<void> = new EventEmitter();

  constructor() { }

  ngOnInit(): void {
  }
}
