import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-wizard-step-indicator',
  templateUrl: './wizard-step-indicator.component.html',
  styleUrls: ['./wizard-step-indicator.component.css'],
})
export class WizardStepIndicatorComponent {
  @Input() title: string = "";
  @Input() active: boolean = false;
  @Input() primary: boolean = false;
  @Input() completed: boolean = false;

  constructor() { }
}
