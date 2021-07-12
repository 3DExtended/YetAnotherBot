import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { forkJoin } from 'rxjs';
import { BotStatusService } from 'src/app/services/bot-status.service';

@Component({
  selector: 'app-bot-status-indicator',
  templateUrl: './bot-status-indicator.component.html',
  styleUrls: ['./bot-status-indicator.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class BotStatusIndicatorComponent implements OnInit {
  public botIsRunning = false;
  constructor(private readonly _botStatusService: BotStatusService) { }

  ngOnInit(): void {
    this.reloadBotStatusForEver(1000);
  }

  public toggleTheBotState() {
    if (this.botIsRunning) {
      this._botStatusService.StopBot()
        .subscribe(() => {
          this.botIsRunning = false;
        });
    } else {
      this._botStatusService.StartBot()
        .subscribe(() => {
          this.botIsRunning = true;
        });
    }
  }

  private reloadBotStatusForEver(refreshRate: number) {
    setTimeout(() => {
      this.updateBotStatus();
      this.reloadBotStatusForEver(refreshRate);
    }, refreshRate);
  }

  private updateBotStatus() {
    const botStatusLoader = this._botStatusService.IsBotRunning();
    forkJoin([botStatusLoader]).subscribe(res => {
      this.botIsRunning = res[0];
    });
  }
}
