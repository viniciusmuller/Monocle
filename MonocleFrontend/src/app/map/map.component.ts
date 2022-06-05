import { AfterViewInit, Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import * as L from 'leaflet';
import { interval, tap } from 'rxjs';
import { VehicleType } from '../types/enums';
import { Base, BaseType, Item, Player, PlayerId, Position, ServerInfo, Vehicle } from '../types/models';


@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.scss']
})
export class MapComponent implements AfterViewInit, OnChanges {
  @Input() players?: Player[];
  @Input() vehicles?: Vehicle[];
  @Input() bases?: Base[];
  @Input() serverInfo?: ServerInfo;
  @Output() onPlayerSelected = new EventEmitter<PlayerId>();
  @Output() onVehicleSelected = new EventEmitter<string>();
  @Output() onBaseSelected = new EventEmitter<string>();

  private map!: L.Map;
  private playerMarkers: L.Marker[];
  private vehicleMarkers: L.Marker[];
  private baseMarkers: L.Marker[];
  private meter?: number;

  constructor() { 
    this.playerMarkers = [];
    this.vehicleMarkers = [];
    this.baseMarkers = [];

    const subscription = interval(1000)
      .pipe(tap(() => {
        if (this.serverInfo) {
          this.addMap(this.serverInfo);
          let info = this.serverInfo;
          this.meter = ((info.worldSize - info.borderSize) / info.worldSize) / 0.908;
          subscription.unsubscribe();
        }
      }))
      .subscribe();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['players'] && this.map) {
      this.refreshPlayerMarkers();
    }

    if (changes['vehicles'] && this.map) {
      this.refreshVehicleMarkers();
    }

    if (changes['bases'] && this.map) {
      this.refreshBaseMarkers();
    }
  }

  ngAfterViewInit(): void {
    this.initMap();
  }

  private initMap(): void {
    this.map = L.map('map', {
      center: [ 0, 0 ],
      zoom: 1,
      minZoom: -1,
      maxZoom: 4,
      crs: L.CRS.Simple
    });
  }

  calculateGearScore(items: Item[]) {
    if (items.length < 12) {
      return 0;
    }

    let sum = items.reduce((acc, item) => {
      return item.rarity + acc
    }, 0);

    return sum / items.length;
  }

  refreshPlayerMarkers() {
    if (this.players && this.serverInfo) {
      for (let mark of this.playerMarkers) {
        mark.remove();
      }
      this.playerMarkers = [];

      for (let player of this.players) {
        let playerMarker = this.createPlayerMarker(player);
        playerMarker.addTo(this.map);
        this.playerMarkers.push(playerMarker)
      }
    }
  }

  refreshBaseMarkers() {
    if (this.bases && this.serverInfo) {
      for (let mark of this.baseMarkers) {
        mark.remove();
      }
      this.baseMarkers = [];

      for (let base of this.bases) {
        let baseMarker = this.createBaseMarker(base);
        baseMarker.addTo(this.map);
        this.baseMarkers?.push(baseMarker);
      }
    }
  }

  refreshVehicleMarkers() {
    if (this.vehicles && this.serverInfo) {
      if (this.vehicleMarkers) {
        for (let mark of this.vehicleMarkers) {
          mark.remove();
        }
        this.vehicleMarkers = [];
      }

      for (let vehicle of this.vehicles) {
        let vehicleMarker = this.createVehicleMarker(vehicle);
        vehicleMarker.addTo(this.map);
        this.vehicleMarkers?.push(vehicleMarker)
      }
    }
  }

  addMap(serverInfo: ServerInfo) {
    const half = serverInfo.worldSize / 2;
    const bounds = L.latLngBounds([-half, -half], [half, half]); // 2048x2048 -> ([-1024, -1024], [1024, 1024])
    const image = L.imageOverlay('https://i.imgur.com/gqoRnQd.jpg', bounds);
    image.addTo(this.map);
    this.map.fitBounds(bounds);
  }

  createPlayerMarker(player: Player): L.Marker {
    let iconAsset = '';
    let gearScore = this.calculateGearScore(player.items);

    if (gearScore > 1.0) {
      iconAsset = 'raider.png';
    } else if (gearScore > 0.6) {
      iconAsset = 'geared.png';
    } else {
      iconAsset = 'neutral.png';
    }

    if (player.health == 0) {
      iconAsset = 'player-dead.png';
    }

    const playerIcon = L.icon({
      // TODO: Create helper function to create icons
      iconUrl: `assets/img/${iconAsset}`,
      iconSize:     [24, 24], // size of the icon
      shadowSize:   [0, 0], // size of the shadow
      iconAnchor:   [12, 12], // point of the icon which will correspond to marker's location
      shadowAnchor: [0, 0],  // the same for the shadow
      popupAnchor:  [-3, -76] // point from which the popup should open relative to the iconAnchor
    });

    let marker = this.createMarker(player.position, playerIcon);
    let onClick = () => this.onPlayerSelected.emit(player.id);
    return marker.on('click', onClick.bind(this));
  }

  createBaseMarker(base: Base): L.Marker {
    const baseImage = this.getBaseImage(base.type);

    const baseIcon = L.icon({
      iconUrl: `assets/img/${baseImage}`,
      iconSize:     [24, 24], // size of the icon
      shadowSize:   [0, 0], // size of the shadow
      iconAnchor:   [12, 12], // point of the icon which will correspond to marker's location
      shadowAnchor: [0, 0],  // the same for the shadow
      popupAnchor:  [-3, -76] // point from which the popup should open relative to the iconAnchor
    });

    let marker = this.createMarker(base.position, baseIcon);
    let onClick = () => this.onBaseSelected.emit(base.trackId);
    return marker.on('click', onClick);
  }

  getBaseImage(type?: BaseType) {
    if (type) {
      switch (type) {
        case BaseType.Small:
          return 'simple-base.png'
        case BaseType.Large:
          return 'large-base.png'
        case BaseType.Raided:
          return 'raided-base.png'
      }
    } else {
      return 'simple-base.png';
    }
  }

  createVehicleMarker(vehicle: Vehicle): L.Marker {
    const data = this.getVehicleIconData(vehicle.type);

    const vehicleIcon = L.icon({
      // TODO: Handle different vehicles
      iconUrl: `assets/img/${data.sprite}`,
      iconSize:     [data.iconSize, data.iconSize], // size of the icon
      shadowSize:   [0, 0], // size of the shadow
      iconAnchor:   [data.iconSize / 2, data.iconSize / 2], // point of the icon which will correspond to marker's location
      shadowAnchor: [0, 0],  // the same for the shadow
      popupAnchor:  [-3, -76] // point from which the popup should open relative to the iconAnchor
    });

    let marker = this.createMarker(vehicle.position, vehicleIcon);
    let onClick = () => this.onVehicleSelected.emit(vehicle.instanceId);
    return marker.on('click', onClick);
  }

  getVehicleIconData(type: VehicleType): MapEntityData {
    switch (type) {
      case VehicleType.Car:
        return {iconSize: 24, sprite: 'car.png'};
      case VehicleType.Boat:
        return {iconSize: 24, sprite: 'boat.png'};
      case VehicleType.Helicopter:
        return {iconSize: 24, sprite: 'helicopter.png'};
      case VehicleType.Blimp:
        return {iconSize: 24, sprite: 'blimp.png'};
      case VehicleType.Plane:
        return {iconSize: 24, sprite: 'plane.png'};
      case VehicleType.Train:
        return {iconSize: 24, sprite: 'train.png'};
    }
  }

  createMarker(position: Position, icon: L.Icon): L.Marker {
    let [x, y] = this.positionToMapPosition(position);
    return L.marker([x, y], {icon: icon}); // Coordinates are inverted in leaflet's simple CRS 
  }
  
  positionToMapPosition(position: Position): [number, number] {
    let x: number, y: number;

    if (this.meter) {
      x = position.z * this.meter;
      y = position.x * this.meter;
    } else {
      x = position.z;
      y = position.x;
    }

    return [x, y]
  }
}

interface MapEntityData {
  iconSize: number;
  sprite: string;
}