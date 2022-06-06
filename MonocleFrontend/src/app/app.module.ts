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
import { ChatComponent } from './chat/chat.component';
import { EventLogComponent } from './event-log/event-log.component'; 
import {MatExpansionModule} from '@angular/material/expansion';
import { PlayerInventoryComponent } from './player-inventory/player-inventory.component';
import { ItemComponent } from './item/item.component';
import { PlayerEquipmentComponent } from './player-equipment/player-equipment.component';
import { MapComponent } from './map/map.component'; 
import {MatListModule} from '@angular/material/list'; 
import {MatCardModule} from '@angular/material/card'; 
import {MatDialogModule} from '@angular/material/dialog'; 
import {MatCheckboxModule} from '@angular/material/checkbox';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { VehicleComponent } from './vehicle/vehicle.component';
import { BaseComponent } from './base/base.component';
import { BarricadeComponent } from './barricade/barricade.component';
import { StructureComponent } from './structure/structure.component';
import { ScreenshotDialogComponent } from './screenshot-dialog/screenshot-dialog.component';
import { ConfirmDialogComponent } from './confirm-dialog/confirm-dialog.component';

@NgModule({
  declarations: [
    AppComponent,
    ServerDashboardComponent,
    LoginComponentComponent,
    PlayersDisplayComponent,
    ServerDetailsComponent,
    PlayerComponent,
    ChatComponent,
    EventLogComponent,
    PlayerInventoryComponent,
    ItemComponent,
    PlayerEquipmentComponent,
    MapComponent,
    VehicleComponent,
    BaseComponent,
    BarricadeComponent,
    StructureComponent,
    ScreenshotDialogComponent,
    ConfirmDialogComponent,
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
    MatExpansionModule,
    MatListModule,
    MatCardModule,
    MatCheckboxModule,
    MatDialogModule,
    MatSnackBarModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
