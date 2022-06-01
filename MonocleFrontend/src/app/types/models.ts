import { ChatMode, ItemRarity, VehicleType } from "./enums";

// TODO: Try to use ids as numbers
export type PlayerId = string;

export interface Position {
    x: number;
    y: number;
    z: number;
}

export interface Item {
    name: string;
    amount: number;
    id: number;
    durability: number;
    rarity: ItemRarity;
}

export interface Equipment {
    hat?: Item;
    vest?: Item;
    backpack?: Item;
    pants?: Item;
    shirt?: Item;
    mask?: Item;
    glasses?: Item;
    primary?: Item;
    secondary?: Item;
}

export enum SelectedEntityType {
    Base,
    Vehicle,
    Player
}

export interface SelectedEntity<T> {
    type: SelectedEntityType,
    id: T;
}

export interface Player {
    id: PlayerId;
    name: string;
    isAdmin: boolean;
    ping: number;
    position: Position;
    health: number;
    rotation: number;
    reputation: number;
    equipment: Equipment;
    joined?: number;
    groupId: string;
    items: Item[];
}

interface Building {
    id: number;
    name: string;
    health: number;
    position: Position;
    groupId: string;
    ownerId: PlayerId;
    instanceId: string;
}

export interface Structure extends Building { }
export interface Barricade extends Building {
    items?: Item[];
}

export interface Base {
    groupId: string;
    structures: Structure[];
    barricades: Barricade[];
    position: Position;
}

export interface Vehicle {
    isLocked: boolean;
    name: string;
    position: Position;
    id: number;
    instanceId: string;
    ownerId: PlayerId,
    battery: number;
    fuel: number;
    health: number;
    type: VehicleType;
}

export interface ServerInfo {
    serverName: string;
    monocleVersion: string;
    unturnedVersion: string;
    mapName: string;
    maxPlayers: number;
    currentPlayers: number;
    queueSize: number;
    playersInQueue: number;
    worldSize: number;
    borderSize: number;
    mapImageEncoded: string;
}

export interface PlayerMessage {
    author: Player;
    content: string;
    chatMode: ChatMode;
    colorHex: string;
    time: Date;
}

export interface PlayerJoinOrLeave {
    player: Player;
    time: Date;
}

export interface PlayerDeath {
    time: Date;
    dead: Player;
    killer: Player;
    cause: string;
}

export interface MonocleEvent {
    time: Date;
    message: string;
}