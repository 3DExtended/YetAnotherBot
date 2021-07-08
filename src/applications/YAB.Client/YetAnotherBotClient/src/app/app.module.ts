import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { NgxGraphModule } from '@swimlane/ngx-graph';
import { ChartsModule } from 'ng2-charts';
import { BubbleWithIconComponent } from 'src/stories/components/bubble-with-icon/bubble-with-icon.component';
import { ButtonComponent } from 'src/stories/components/button/button.component';
import { InputFieldComponent } from 'src/stories/components/input-field/input-field.component';
import { LineGrpahComponent } from 'src/stories/components/line-graph/line-graph.component';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginPageComponent } from './pages/login-page/login-page.component';


@NgModule({
  declarations: [
    AppComponent,
    BubbleWithIconComponent,
    LineGrpahComponent,
    InputFieldComponent,
    LoginPageComponent,
    ButtonComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    ChartsModule,
    NgxGraphModule,
    FormsModule,
    CommonModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
