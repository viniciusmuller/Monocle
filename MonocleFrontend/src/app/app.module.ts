import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { ServerDashboardComponent } from './server-dashboard/server-dashboard.component';
import { ReactiveFormsModule } from '@angular/forms';
import { LoginComponentComponent } from './login-component/login-component.component';
import { PlayersDisplayComponent } from './players-display/players-display.component';
import {MatProgressSpinnerModule} from '@angular/material/progress-spinner';
import { ServerDetailsComponent } from './server-details/server-details.component';
import { PlayerComponent } from './player/player.component'; 

@NgModule({
  declarations: [
    AppComponent,
    ServerDashboardComponent,
    LoginComponentComponent,
    PlayersDisplayComponent,
    ServerDetailsComponent,
    PlayerComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    ReactiveFormsModule,
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
