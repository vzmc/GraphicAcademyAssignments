# SpriteAnimShader

## SpriteAnimShader1 (流れる順番は左->右、下->上)
![5](https://user-images.githubusercontent.com/6869650/156617516-f3521617-a77b-4980-9b44-577c7a851fc6.gif)

0-1のUV座標を指定した数を分割し、経過時間で今のコマのIndexを算出し、
そのIndexを横分割数で余算と除算で列と行の番号を計算しだして、UV座標の変換を行う
https://github.com/vzmc/GraphicAcademyAssignments/blob/8655ab994da3a30c6c06717538d41655f7227b7d/GraphicAcademyAssignments/Assets/Assignments/Day2/SpriteAnim/SpriteAnimShader.shader#L42-L56

## SpriteAnimShader2 (流れる順番は左->右、上->下、アルファ値は透過する)
![5](https://user-images.githubusercontent.com/6869650/156765200-9fecbd7f-997c-491c-8fb9-21f722a2127f.gif)

アルファ値で透過したいので、TagsのRenderTypeとQueueをTransparentに設定する
https://github.com/vzmc/GraphicAcademyAssignments/blob/8655ab994da3a30c6c06717538d41655f7227b7d/GraphicAcademyAssignments/Assets/Assignments/Day2/SpriteAnim/SpriteAnimShader2.shader#L12-L17

流れの方向を変えるには、uv座標変換の時のcolとrowの符号を逆にすればいい
例えばここにcolの符号を逆にすれば、縦方向の流れは上->下になる
横方向を変えたければ、rowの符号を逆にすればいい
https://github.com/vzmc/GraphicAcademyAssignments/blob/8655ab994da3a30c6c06717538d41655f7227b7d/GraphicAcademyAssignments/Assets/Assignments/Day2/SpriteAnim/SpriteAnimShader2.shader#L63
