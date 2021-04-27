import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BubbleWithIconComponent } from 'src/stories/components/bubble-with-icon/bubble-with-icon.component';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';


@NgModule({
  declarations: [
    AppComponent,
    BubbleWithIconComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
