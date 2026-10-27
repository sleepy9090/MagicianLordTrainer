# MagicianLordTrainer (GoG Version, setup_magician_lord_1.0_(69161).exe)

<img width="786" height="593" alt="image" src="https://github.com/user-attachments/assets/1db7f228-2f8f-4f3d-b236-0fd298c90a94" />

<img width="1347" height="789" alt="image" src="https://github.com/user-attachments/assets/8327b9b0-3a5f-48b6-b34a-28175277d48f" />


## Misc notes dump on reversing and offsets
NeoGeo.exe+2C5C00 - NeoGeo.exe+2C8CFE



NeoGeo.exe+2C8CA0      0      1      2      3       4       5        6
                       Normal Dragon Raijin Shinobi Samurai Poseidon Waterman 
NeoGeo.exe+2C8CA2      10     10     12      12     12      10       10
NeoGeo.exe+2C8CA3      10     08     10      12     11      07       09

NeoGeo.exe+2C8CA4 ORBS 0      5      15      7      11      09       10


Fire + Fire   - The Dragon Warrior
Water + Water - The Waterman
Water + Fire  - Poseidon
Wind + Fire   - Shinobi
Water + Wind  - Samurai
Wind + Wind   - Raijin

NeoGeo.exe+2C8CA4 ORBS 
0   [ ][ ]
1   [ ][FIRE]
2   [ ][WATER]
3   [ ][WIND]
4   [FIRE][ ]
5   [FIRE][FIRE]
6   [FIRE][WATER]
7   [FIRE][WIND]
8   [WATER][ ]
9   [WATER][FIRE]
10  [WATER][WATER]
11  [WATER][WIND]
12  [WIND][ ]
13  [WIND][FIRE]
14  [WIND][WATER]
15  [WIND][WIND]

16  [ ][ ]
17  [ ][FIRE]
18  [ ][WATER]
19  [ ][WIND]
20  [FIRE][ ]
...
43  [WATER][WIND]
125 [WIND][FIRE]
175 [WIND][WIND]





Continue Timer
Base Address             Offset 0
"NeoGeo.exe"+002D8B5C    317

Spawns Death Bringer (255 = spawn)
Base Address             Offset 0
"NeoGeo.exe"+002D8B60    64


Hi Score 110000 0
Base Address             Offset 0
"NeoGeo.exe"+002D8B60    9C

Hi Score Related
Base Address             Offset 0
"NeoGeo.exe"+002D8B60    9D

Hi Score 000011 0
Base Address             Offset 0
"NeoGeo.exe"+002D8B60    9E

Hi Score 001100 0
Base Address             Offset 0
"NeoGeo.exe"+002D8B60    9F

Score 110000 0
Base Address             Offset 0
"NeoGeo.exe"+002D8B60    A0

Score Related
Base Address             Offset 0
"NeoGeo.exe"+002D8B60    A1

Score 000011 0
Base Address             Offset 0
"NeoGeo.exe"+002D8B60    A2

Score 001100 0
Base Address             Offset 0
"NeoGeo.exe"+002D8B60    A3

Flying Monster Spawn (0 for none, 255 for many)
Base Address             Offset 0
"NeoGeo.exe"+002D8B60    BA

Flying Monster Spawn (0 for none, 255 for many)
Base Address             Offset 0
"NeoGeo.exe"+002D8B60    BB

Demo Countdown Timer 1 (Each countdown adds 1 to Timer 2)
Base Address             Offset 0
"NeoGeo.exe"+002D8B60    502

Demo Countdown Timer 2 (End Demo = 3)
Base Address             Offset 0
"NeoGeo.exe"+002D8B60    503

Death Bringer Countdown Timer for when you wait to long (37, 25h)
Base Address             Offset 0
"NeoGeo.exe"+002D8B8C    1FF

Form (0 - 6, Normal Dragon Raijin Shinobi Samurai Poseidon Waterman)
Base Address             Offset 0
"NeoGeo.exe"+002D8B8C    300

Lives
Base Address             Offset 0
"NeoGeo.exe"+002D8B8C    301

Form related 1
Base Address             Offset 0
"NeoGeo.exe"+002D8B8C    302

Form related 2
Base Address             Offset 0
"NeoGeo.exe"+002D8B8C    303

Orbs
Base Address             Offset 0
"NeoGeo.exe"+002D8B8C    304

POW Bar
Base Address             Offset 0
"NeoGeo.exe"+002D8B8C    305

HP Bar
Base Address             Offset 0
"NeoGeo.exe"+002D8B8C    306

Invincibility
Base Address             Offset 0
"NeoGeo.exe"+002D8B8C    308




