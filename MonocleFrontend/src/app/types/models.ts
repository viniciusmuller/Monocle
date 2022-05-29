import { ChatMode } from "./enums";

export type PlayerId = number;

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
    items: Item[];
}

interface Building {
    name: string;
    health: number;
    position: Position;
}

export interface Structure extends Building { }
export interface Barricade extends Building {
    items?: Item[];
}

export interface Vehicle {
    isLocked: boolean;
    name: string;
    position: Position;
    id: number;
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