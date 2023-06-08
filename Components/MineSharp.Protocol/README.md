# MineSharp.Protocol
The currently implemented Protocol is Java Version 1.19.3. \
The Idea is that each packet has logic to also read older versions of the packet. \
Packet id's are converted via minecraft-data names, if a packet was removed or renamed, a PacketMapping is used to map the old name to the new name.


## Packets
Currently, not all packets are implemented yet. Here's a list what packets are implemented.

### Clientbound

#### Login
| Id   | Packet name                        | Versions      | Write              | Read |
|------|------------------------------------|---------------|--------------------|------|
| 0x00 | DisconnectPacket                   | 1.18.x-1.19.x | Yes                | Yes  |
| 0x01 | EncryptionRequestPacket            | 1.18.x-1.19.x | Yes                | Yes  |
| 0x02 | LoginSuccessPacket                 | 1.18.x-1.19.x | Yes                | Yes  |
| 0x03 | SetCompressionPacket               | 1.18.x-1.19.x | Yes                | Yes  |
| 0x04 | LoginPluginResponse                | 1.18.x-1.19.x | Yes                | Yes  |

#### Status
| Id   | Packet name                        | Versions      | Write              | Read |
|------|------------------------------------|---------------|--------------------|------|
| 0x00 | StatusResponsePacket               | 1.18.x-1.19.x | Yes                | Yes  |
| 0x01 | PongResponsePacket                 | 1.18.x-1.19.x | Yes                | Yes  |

#### Play
| Id   | Packet name                           | Versions      | Write           | Read |
|------|---------------------------------------|---------------|-----------------|------|
| 0x01 | SpawnEntityPacket                     | 1.18.x-1.19.x | **No**          | Yes  |
| 0x04 | SpawnPlayerPacket                     | 1.18.x-1.19.x | Yes             | Yes  |
| 0x0A | BlockUpdatePacket                     | 1.18.x-1.19.x | Yes             | Yes  |
| 0x1E | UnloadChunkPacket                     | 1.18.x-1.19.x | Yes             | Yes  |
| 0x23 | KeepAlivePacket                       | 1.18.x-1.19.x | Yes             | Yes  |
| 0x24 | ChunkDataAndLightUpdatePacket         | 1.18.x-1.19.x | Yes             | Yes  |
| 0x28 | LoginPacket                           | 1.18.x-1.19.x | **Only 1.19.x** | Yes  |
| 0x2B | UpdateEntityPositionPacket            | 1.18.x-1.19.x | Yes             | Yes  |
| 0x2C | UpdateEntityPositionAndRotationPacket | 1.18.x-1.19.x | Yes             | Yes  |
| 0x2D | UpdateEntityRotationPacket            | 1.18.x-1.19.x | Yes             | Yes  |
| 0x38 | CombatDeathPacket                     | 1.18.x-1.19.x | Yes             | Yes  |
| 0x3C | SynchronizePlayerPositionPacket       | 1.18.x-1.19.x | Yes             | Yes  |
| 0x3E | RemoveEntitiesPacket                  | 1.18.x-1.19.x | Yes             | Yes  |
| 0x41 | RespawnPacket                         | 1.18.x-1.19.x | **Only 1.19.x** | Yes  |
| 0x43 | MultiBlockUpdatePacket                | 1.18.x-1.19.x | Yes             | Yes  |
| 0x54 | SetEntityVelocityPacket               | 1.18.x-1.19.x | Yes             | Yes  |
| 0x57 | SetHealthPacket                       | 1.18.x-1.19.x | Yes             | Yes  |
| 0x68 | TeleportEntityPacket                  | 1.18.x-1.19.x | Yes             | Yes  |
| 0x6A | UpdateAttributesPacket                | 1.18.x-1.19.x | Yes             | Yes  |


### Serverbound

#### Handshake
| Id   | Packet name                        | Versions      | Write              | Read |
|------|------------------------------------|---------------|--------------------|------|
| 0x00 | HandshakePacket                    | 1.18.x-1.19.x | Yes                | Yes  |

#### Login
| Id   | Packet name                        | Versions      | Write              | Read |
|------|------------------------------------|---------------|--------------------|------|
| 0x00 | LoginStartPacket                   | 1.18.x-1.19.x | Yes                | Yes  |
| 0x01 | EncryptionResponsePacket           | 1.18.x-1.19.x | Yes                | Yes  |
| 0x02 | LoginPluginResponsePacket          | 1.18.x-1.19.x | Yes                | Yes  |

#### Status
| Id   | Packet name                        | Versions      | Write              | Read |
|------|------------------------------------|---------------|--------------------|------|
| 0x00 | StatusRequestPacket                | 1.18.x-1.19.x | Yes                | Yes  |
| 0x01 | PingRequestPacket                  | 1.18.x-1.19.x | Yes                | Yes  |

#### Play
| Id   | Packet name                        | Versions      | Write              | Read   |
|------|------------------------------------|---------------|--------------------|--------|
| 0x04 | ChatCommandPacket                  | 1.19.x        | Yes                | **No** |
| 0x05 | ChatMessagePacket                  | 1.18.x-1.19.x | Yes                | Yes    |
| 0x06 | PlayerSessionPacket                | \>= 1.19.3    | Yes                | Yes    |
| 0x07 | ClientCommandPacket                | 1.18.x-1.19.x | Yes                | Yes    |
| 0x12 | KeepAlivePacket                    | 1.18.x-1.19.x | Yes                | Yes    |
| 0x14 | SetPlayerPositionPacket            | 1.18.x-1.19.x | Yes                | Yes    |
| 0x15 | SetPlayerPositionAndRotationPacket | 1.18.x-1.19.x | Yes                | Yes    |

