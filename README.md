# TBL_Simulator

Unity Version: 2021.3.1f1

The Black List made with Unity

## Flow

### Scene Flow

```mermaid
flowchart

Menu[Menu]
Room[Room]
Ingame[Ingame]

Menu --> |Create|Room
Menu --> |Join|Room

Room --> Ingame

```

### Game Initialize

```mermaid
sequenceDiagram

box Server
participant M as GameManager
participant S as Server
end
box Client
participant C as Client
end

M -->> S: Wait Initialize

critical Enter Game Scene
S --> S: Create Network Players
C --> +C: Create Network Players
Note over C: Wait ServerReadyPacket
end

M ->> S: [Broadcast] ServerReadyPacket
S ->> C: ServerReadyPacket

C ->> -S: PlayerReadyPacket

S ->> M: PlayerReadyPacket
Note over S,M: Wait all PlayerReadyPacket

M ->> S: [Broadcast] GameStartPacket
S ->> C: GameStartPacket

```
