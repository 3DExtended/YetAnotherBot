import { Component, ContentChild, Input, OnInit, TemplateRef } from '@angular/core';

export interface PipelineBlock {
  title: string;
  properties: string[];
  description: string | null;
};

export enum PipelineBlockIcon {
  Event = 'EVENT',
  Filter = 'FILTER',
  Action = 'ACTION'
}

export interface PipelineElementBorderParts {
  topLeft: boolean;
  topRight: boolean;
  bottomLeft: boolean;
  bottomRight: boolean;
  leftTop: boolean;
  leftBottom: boolean;
  lineIntoFromTop: boolean;
  lineFromBottomDown: boolean;
  lineFromLeftIntoRight: boolean;
};


@Component({
  selector: 'app-pipeline-element',
  templateUrl: './pipeline-element.component.html',
  styleUrls: ['./pipeline-element.component.css'],
  // encapsulation: ViewEncapsulation.None
})
export class PipelineElementComponent implements OnInit {

  private borderStyle = '4px solid';

  @Input() elementDetails: PipelineBlock | null = null;
  @Input() borderDefinition: PipelineElementBorderParts = {
    topLeft: false,
    topRight: false,
    bottomLeft: false,
    bottomRight: false,
    leftTop: false,
    leftBottom: true,
    lineIntoFromTop: true,
    lineFromBottomDown: true,
    lineFromLeftIntoRight: true,
  };

  @Input() icon: PipelineBlockIcon = PipelineBlockIcon.Event;
  @Input() marginXAuto: boolean = false;

  @ContentChild('content', { static: false }) injectedContentTemplate: TemplateRef<any> | undefined;

  public showDetails: boolean = false;

  constructor() { }

  ngOnInit(): void {
  }

  public getTopBorderStyle(): any {
    if (!this.borderDefinition.topLeft && !this.borderDefinition.topRight) {
      return {}; // no additional borders needed
    } else {

      return {
        'border-top': this.borderStyle,
        'border-image': 'linear-gradient(to right, '
          + (this.borderDefinition.topLeft ? 'rgb(0, 0, 0)' : 'rgba(255, 255, 255, 0)') + ' 50%, '
          + (this.borderDefinition.topRight ? 'rgb(0, 0, 0)' : 'rgba(255, 255, 255, 0)') + ' 50%) 100% 1'
      };
    }
  }

  public getBottomBorderStyle(): any {
    if (!this.borderDefinition.bottomLeft && !this.borderDefinition.bottomRight) {
      return {}; // no additional borders needed
    } else {
      return {
        'border-bottom': this.borderStyle,
        'border-image': 'linear-gradient(to right, '
          + (this.borderDefinition.bottomLeft ? 'rgb(0, 0, 0)' : 'rgba(255, 255, 255, 0)') + ' 50%, '
          + (this.borderDefinition.bottomRight ? 'rgb(0, 0, 0)' : 'rgba(255, 255, 255, 0)') + ' 50%) 100% 1'
      };
    }
  }

  public getLeftBorderStyle(): any {
    if (!this.borderDefinition.leftBottom && !this.borderDefinition.leftTop) {
      return {}; // no additional borders needed
    } else {
      return {
        'border-left': this.borderStyle,
        'border-image': 'linear-gradient(180deg, '
          + (this.borderDefinition.leftTop ? 'rgb(0, 0, 0)' : 'rgba(255, 255, 255, 0)') + ' 50%, '
          + (this.borderDefinition.leftBottom ? 'rgb(0, 0, 0)' : 'rgba(255, 255, 255, 0)') + ' 50%) 100% 1'
      };
    }
  }
}
