import { AfterViewInit, Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import * as L from 'leaflet';
import { interval, tap } from 'rxjs';
import { Player, PlayerId, Position, ServerInfo, Vehicle } from '../types/models';


@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.scss']
})
export class MapComponent implements AfterViewInit, OnChanges {
  @Input() players?: Player[];
  @Input() vehicles?: Vehicle[];
  @Input() serverInfo?: ServerInfo;
  @Output() onPlayerSelected = new EventEmitter<PlayerId>();

  private map!: L.Map;
  private playerMarkers: L.Marker[];
  private vehicleMarkers: L.Marker[];
  private meter?: number;

  constructor() { 
    this.playerMarkers = [];
    this.vehicleMarkers = [];

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

  refreshPlayerMarkers() {
    if (this.players && this.serverInfo) {
      if (this.playerMarkers) {
        for (let mark of this.playerMarkers) {
          mark.remove();
        }
        this.playerMarkers = [];
      }

      for (let player of this.players) {
        let playerMarker = this.createPlayerMarker(player);
        playerMarker.addTo(this.map);
        this.playerMarkers?.push(playerMarker)
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
    const playerIcon = L.icon({
      iconUrl: '/assets/img/unturned-zombie.jpg',
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

  createVehicleMarker(vehicle: Vehicle): L.Marker {
    const playerIcon = L.icon({
      iconUrl: '/assets/img/unturned-zombie.jpg',
      iconSize:     [24, 24], // size of the icon
      shadowSize:   [0, 0], // size of the shadow
      iconAnchor:   [12, 12], // point of the icon which will correspond to marker's location
      shadowAnchor: [0, 0],  // the same for the shadow
      popupAnchor:  [-3, -76] // point from which the popup should open relative to the iconAnchor
    });

    let marker = this.createMarker(vehicle.position, playerIcon);
    let onClick = () => console.log(vehicle);
    return marker.on('click', onClick);
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
