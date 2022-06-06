import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { interval, lastValueFrom, Observable, of, timer } from 'rxjs';
import { Barricade, Base, BaseType, Item, MonocleEvent, Player, PlayerId, PlayerMessage, SelectedEntity, SelectedEntityType, ServerInfo, Structure, Vehicle } from '../types/models';
import { AuthenticationRequest } from '../types/requests';
import { WebsocketService } from '../services/websocket.service';
import { AuthorizedUserType } from '../types/enums';
import { SuccesfulAuthenticationResponse } from '../types/responses';
import { ScreenshotDialogComponent } from '../screenshot-dialog/screenshot-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import * as uuid from 'uuid';
import { ConfirmDialogComponent } from '../confirm-dialog/confirm-dialog.component';
import { MatSnackBar, _SnackBarContainer } from '@angular/material/snack-bar';

@Component({
  selector: 'app-server-dashboard',
  templateUrl: './server-dashboard.component.html',
  styleUrls: ['./server-dashboard.component.scss'],
  providers: [WebsocketService]
})
export class ServerDashboardComponent implements OnInit {
  loggedIn: boolean = false;
  players?: Player[];
  barricades?: Barricade[];
  structures?: Structure[];
  vehicles?: Vehicle[];
  bases: Base[];
  authenticationData?: SuccesfulAuthenticationResponse;
  serverInfo?: ServerInfo;
  chatLog: PlayerMessage[];
  eventLog: MonocleEvent[];
  selectedEntity?: SelectedEntity<PlayerId | string>;
  selectedEntityType = SelectedEntityType; // used in ngSwitch

  noGroupId: string = '0';
  gameMapUrl?: string;

  connectAndLogin(request: AuthenticationRequest) {
    this.websocketService.connect(request.host, request.port, request.ssl);
    setTimeout(() => {
      this.websocketService.login(request.username, request.password);
    }, 500);
  }

  constructor(private websocketService: WebsocketService, 
              private screenshotDialog: MatDialog,
              private confirmDialog: MatDialog,
              private snackBar: MatSnackBar) {
    this.chatLog = [];
    this.eventLog = [];
    this.bases = [];
  }

  ngOnInit(): void {
    // TODO: These should probably be inside the constructor

    this.websocketService.onLoginSuccessful.subscribe(authData => {
      this.loggedIn = true;
      this.authenticationData = authData;
      this.bindRequests();
    })

    this.websocketService.onGetPlayers.subscribe(players => {
      this.players = players;
    })

    this.websocketService.onGetBarricades.subscribe(barricades => {
      this.barricades = barricades;
    })

    this.websocketService.onGetStructures.subscribe(structures => {
      this.structures = structures;
    })

    this.websocketService.onGetVehicles.subscribe(vehicles => {
      this.vehicles = vehicles;
    })

    this.websocketService.onGetServerInfo.subscribe(serverDetails => {
      this.serverInfo = serverDetails;
    })

    this.websocketService.onGetGameMap.subscribe(gameMap => {
      this.gameMapUrl = `data:image/png;base64,${gameMap}`;
    })

    this.websocketService.onPlayerMessage.subscribe(playerMessage => {
      this.chatLog = [playerMessage, ...this.chatLog];
    })

    this.websocketService.internalServerError.subscribe(error => {
      this.snackBar.open(`Server error: ${error.message}`, 'Close', {
        horizontalPosition: 'right',
        verticalPosition: 'top',
      });
      console.error(error.message, error.code, error.stackTrace);
    })

    this.websocketService.userNotFound.subscribe(error => {
      console.error(`User with id ${error.userId} was not found`);
    })

    this.websocketService.onGetPlayerScreenshot.subscribe(screenshotResponse => {
      const imagePath = 'data:image/jpg;base64,' + screenshotResponse.screenEncoded;
      let dialogRef = this.screenshotDialog.open(ScreenshotDialogComponent, {
        data: { imagePath }
      });
    })

    this.websocketService.onPlayerLeft.subscribe(leftEvent => {
      const message = `${leftEvent.player.name} left`;
      const event = this.buildEvent(leftEvent.time, message);
      this.eventLog = [event, ...this.eventLog];
    })

    this.websocketService.onPlayerJoin.subscribe(joinEvent => {
      const message = `${joinEvent.player.name} joined`;
      const event = this.buildEvent(joinEvent.time, message);
      this.eventLog = [event, ...this.eventLog];
    })

    this.websocketService.onPlayerDeath.subscribe(deathEvent => {
      let message;
      if (!deathEvent.killer) {
        message = `${deathEvent.dead.name} died! [${deathEvent.cause}]`
      } else {
        message = `${deathEvent.killer.name} killed ${deathEvent.dead.name} - [${deathEvent.cause}]`
      }

      const event = this.buildEvent(deathEvent.time, message);
      this.eventLog = [event, ...this.eventLog];
    })
  }

  watchPlayer(id: PlayerId) {
    this.getPlayerScreenshot(id);
  }

  async banPlayer(player: Player) {
    let confirmation = this.dialogConfirm('Ban Player', `Are you sure you want to ban ${player.name}?`, true);
    if (await confirmation) {
      this.websocketService.banPlayer(player);
    }
  }

  async kickPlayer(player: Player) {
    let confirmation = this.dialogConfirm('Kick Player', `Are you sure you want to kick ${player.name}?`, true);
    if (await confirmation) {
      this.websocketService.kickPlayer(player);
    }
  }

  async destroyVehicle(vehicle: Vehicle) {
    let confirmation = this.dialogConfirm('Destroy Vehicle', `Are you sure you want to destroy ${vehicle.name}?`, true);
    if (await confirmation) {
      this.websocketService.destroyVehicle(vehicle);
    }
  }

  async destroyItem(item: Item) {
    let confirmation = this.dialogConfirm('Destroy Item', `Are you sure you want to destroy ${item.name}?`, true);
    if (await confirmation) {
      this.websocketService.destroyItem(item);
    }
  }

  async dialogConfirm(title: string, message: string, isDangerous: boolean): Promise<boolean> {
      let dialogRef = this.confirmDialog.open(ConfirmDialogComponent, {
        data: { 
          title,
          message,
          isDangerous
        }
      });

      return await lastValueFrom<boolean>(dialogRef.afterClosed());
  }

  bindRequests() {
    // TODO: Find more elegant approach to these
    this.getPlayers();
    const playerFetchInterval = interval(1000);
    playerFetchInterval.subscribe(() => this.getPlayers())

    this.getVehicles()
    const vehicleFetchInterval = interval(5_000);
    vehicleFetchInterval.subscribe(() => this.getVehicles())

    this.getBarricades();
    const barricadesFetchInterval = interval(60_000);
    barricadesFetchInterval.subscribe(() => this.getBarricades())

    this.getStructures();
    const structuresFetchInterval = interval(60_000);
    structuresFetchInterval.subscribe(() => this.getStructures())

    this.getGameMap();

    setTimeout(() => {
      this.findBases()
    }, 1000)
    const findBasesInterval = interval(60_000);
    findBasesInterval.subscribe(() => this.findBases())

    // TODO: Find more elegant approach to these
    this.getServerDetails();
    const serverDetailsFetchInterval = interval(60_000);
    serverDetailsFetchInterval.subscribe(() => this.getServerDetails())
  }

  // Move this to its own context
  findBases() {
    const minDistanceBetweenBases = 35;
    this.bases = [];

    if (this.barricades && this.structures) {
      const buildingsGroupedByGroup = this.groupBuildingsByGroup(this.barricades, this.structures);
      const basesPerGroup = this.groupGroupsBuildings(buildingsGroupedByGroup, minDistanceBetweenBases);

      for (const [_groupId, groupBases] of basesPerGroup) {
        for (const base of groupBases) {
          const baseType = this.getBaseType(base);
          if (baseType != BaseType.Invalid) {
            base.type = baseType;
            this.bases.push(base);
          }
        }
      };
    }
  }

  getBaseType(base: Base): BaseType {
    if (base.barricades.length <= 2 || base.structures.length <= 8) {
      return BaseType.Invalid;
    } else if (base.barricades.length <= 10 && base.structures.length <= 25) {
      return BaseType.Small;
    } else {
      return BaseType.Large;
    }
    // TODO: Check if the base is raided
  }

  groupBuildingsByGroup(barricades: Barricade[], structures: Structure[]): Base[] {
    // TODO: Refactor
    let bases: any = {}; // [key: string]: Base[]// TODO: Type

    for (const barricade of barricades) {
      let base = bases[barricade.groupId];

      if (!base) {
        base = {};
        bases[barricade.groupId] = base;
        base.barricades = [];
        base.structures = [];
        base.groupId = barricade.groupId;
      } 

      base.barricades.push(barricade);
    }

    for (const structure of structures) {
      let base = bases[structure.groupId];

      if (!base) {
        base = {};
        bases[structure.groupId] = base;
        base.structures = [];
        base.barricades = [];
        base.groupId = structure.groupId;
      } 

      base.structures.push(structure);
    }

    return Object.values(bases);
  }

  groupGroupsBuildings(bases: Base[], minDistanceBetweenBases: number): Map<string, IterableIterator<Base>> {
    const resultingBases: Map<string, IterableIterator<Base>> = new Map();

    for (const base of bases) {
      let basesBounds: Map<Bounds, Base> = new Map();

      for (let structure of base.structures) {
        const point = {x: structure.position.z, y: structure.position.x };

        let isWithinKnownBounds = false;
        let currentBound: Bounds;
        for (let bound of basesBounds.keys()) {
          if (this.withinBounds(point, bound)) {
            isWithinKnownBounds = true;
            currentBound = bound;
          }
        }

        if (isWithinKnownBounds) {
          let base = basesBounds.get(currentBound!);
          base?.structures.push(structure);
        } else {
          let bound = {
            topRightX: point.x + minDistanceBetweenBases,
            topRightY: point.y + minDistanceBetweenBases,
            bottomLeftX: point.x - minDistanceBetweenBases,
            bottomLeftY: point.y - minDistanceBetweenBases,
          };
          let base: Base = {
            position: {
              x: (bound.topRightY + bound.bottomLeftY) / 2,
              z: (bound.topRightX + bound.bottomLeftX) / 2, 
              y: structure.position.y
            },
            structures: [structure],
            barricades: [],
            trackId: uuid.v4()
          }
          basesBounds.set(bound, base)
          if (structure.groupId == this.noGroupId) {
            base.ownerId = structure.ownerId;
          } else {
            base.groupId = structure.groupId;
          }
        }
      }

      for (let barricade of base.barricades) {
        const point = {x: barricade.position.z, y: barricade.position.x };

        let isWithinKnownBounds = false;
        let currentBound: Bounds;
        for (let bound of basesBounds.keys()) {
          if (this.withinBounds(point, bound)) {
            isWithinKnownBounds = true;
            currentBound = bound;
          }
        }

        if (isWithinKnownBounds) {
          let base = basesBounds.get(currentBound!);
          base?.barricades.push(barricade);
        } 
      }

      resultingBases.set(base.groupId!, basesBounds.values());
    }

    return resultingBases;
  }

  withinBounds(point: Point, bounds: Bounds) {
    return (point.x > bounds.bottomLeftX && point.x < bounds.topRightX && 
            point.y > bounds.bottomLeftY && point.y < bounds.topRightY);
  }

  buildEvent(time: Date, message: string): MonocleEvent {
    return { time, message }
  }

  vehicleSelected(id: string) {
    this.selectedEntity = { type: SelectedEntityType.Vehicle, id }
  }

  baseSelected(id: string) {
    this.selectedEntity = { type: SelectedEntityType.Base, id }
  }

  playerSelected(id: PlayerId) {
    this.selectedEntity = { type: SelectedEntityType.Player, id }
  }

  getSelectedPlayer(): Player | undefined {
    if (this.selectedEntity?.type == SelectedEntityType.Player) {
      const id = this.selectedEntity.id;
      return this.players?.find(p => p.id == id);
    }
    return;
  }

  getSelectedBase(): Base | undefined {
    if (this.selectedEntity?.type == SelectedEntityType.Base) {
      const id = this.selectedEntity.id;
      return this.bases?.find(p => p.trackId == id);
    }
    return;
  }

  getSelectedVehicle(): Vehicle | undefined {
    if (this.selectedEntity?.type == SelectedEntityType.Vehicle) {
      const id = this.selectedEntity.id;
      return this.vehicles?.find(p => p.instanceId == id);
    }
    return;
  }

  userCanModerate(): boolean {
    return this.authenticationData?.userType == AuthorizedUserType.Administrator;
  }

  getPlayers() { this.websocketService.getPlayers(); }
  getPlayerScreenshot(id: PlayerId) { this.websocketService.getPlayerScreenshot(id); }
  getVehicles() { this.websocketService.getVehicles(); }
  getGameMap() { this.websocketService.getGameMap(); }
  getStructures() { this.websocketService.getStructures(); }
  getBarricades() { this.websocketService.getBarricades(); }
  getServerDetails() { this.websocketService.getServerDetails(); }
}

interface Bounds {
  topRightX: number,
  topRightY: number,
  bottomLeftX: number,
  bottomLeftY: number,
}

interface Point {
  x: number,
  y: number,
}
