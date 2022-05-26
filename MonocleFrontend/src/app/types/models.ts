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
    id: number;
    name: string;
    isAdmin: boolean;
    ping: number;
    position: Position;
    health: number;
    rotation: number;
    equipment: Equipment;
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