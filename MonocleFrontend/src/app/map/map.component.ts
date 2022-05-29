import { AfterViewInit, Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import * as L from 'leaflet';
import { Player, PlayerId } from '../types/models';


@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.scss']
})
export class MapComponent implements AfterViewInit, OnChanges {
  @Input() players?: Player[]; 
  @Output() onPlayerSelected = new EventEmitter<PlayerId>();

  private map!: L.Map;
  private markers?: L.Marker[];

  private initMap(): void {
    this.map = L.map('map', {
      center: [ 0, 0 ],
      zoom: 1,
      minZoom: -1,
      maxZoom: 4,
      crs: L.CRS.Simple
    });

    const bounds = L.latLngBounds([-1024, -1024], [1024, 1024]);
    const image = L.imageOverlay('https://i.imgur.com/gqoRnQd.jpg', bounds);
    image.addTo(this.map);
    this.map.fitBounds(bounds);
  }

  refreshMap() {
    if (this.players) {

      if (this.markers) {
        for (let mark of this.markers) {
          mark.remove();
        }
      }

      for (let player of this.players) {
        let playerMarker = this.createPlayerMarker(player);
        playerMarker.addTo(this.map);
        this.markers?.push(playerMarker)
      }
    }
  }

  createPlayerMarker(player: Player): L.Marker {
    const playerIcon = L.icon({
      iconUrl: '/assets/img/unturned-zombie.jpg',
      iconSize:     [24, 24], // size of the icon
      shadowSize:   [0, 0], // size of the shadow
      iconAnchor:   [0, 0], // point of the icon which will correspond to marker's location
      shadowAnchor: [0, 0],  // the same for the shadow
      popupAnchor:  [-3, -76] // point from which the popup should open relative to the iconAnchor
    });

    let marker = L.marker([player.position.z, player.position.x], {icon: playerIcon});
    // Coordinates are inverted in leaflet's simple CRS 
    let onClick = () => this.onPlayerSelected.emit(player.id);
    return marker.on('click', onClick.bind(this));
  }

  constructor() { 
    this.markers = [];
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['players'] && this.map) {
      this.refreshMap();
    }
  }

  ngAfterViewInit(): void {
    this.initMap();
  }
}
