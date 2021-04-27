import { Component, EventEmitter, Input, OnInit, Output, ViewEncapsulation } from '@angular/core';

@Component({
  selector: 'app-bubble-with-icon',
  templateUrl: './bubble-with-icon.component.html',
  styleUrls: ['./bubble-with-icon.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class BubbleWithIconComponent implements OnInit {

  @Input() title: string = "";
  @Input() message: string = "";

  @Output() onClick: EventEmitter<MouseEvent> = new EventEmitter<MouseEvent>();

  constructor() { }

  ngOnInit(): void {
  }

  public onClickHandler(event: any) {
    this.onClick.next(event);
  }
}
