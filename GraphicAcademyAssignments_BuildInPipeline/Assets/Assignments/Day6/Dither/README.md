# Dither
## ディザで不透明オブジェクトに透明感を出す。
通常のAlpha Blend Transparentによる透明処理よりコストが低い

![5](https://user-images.githubusercontent.com/6869650/162434607-a67c53f4-0298-487d-80e5-890c28d5bae6.gif)

## ディザするため、まずフラグメントのスクリーン座標を取得する。以下の2つの方法がある
### 1. ClipPosから算出する
https://github.com/vzmc/GraphicAcademyAssignments/blob/02f2b85853f3c802e8fb64751e0b86f5d762b4eb/GraphicAcademyAssignments_BuildInPipeline/Assets/Assignments/Day6/Dither/DitherShader_2.shader#L52-L57

注意: vert shaderから出力する`posCS`も`UnityObjectToClipPos(v.posOS)`で変換される値だが、`SV_POSITION`である故、それをfrag shaderでClipPosとして使うところで正しくスクリーン座標を取得できない。スクリーン座標を計算するため、`SV_POSITION`の`posCS`のほかにClipPosが必要
https://github.com/vzmc/GraphicAcademyAssignments/blob/02f2b85853f3c802e8fb64751e0b86f5d762b4eb/GraphicAcademyAssignments_BuildInPipeline/Assets/Assignments/Day6/Dither/DitherShader_2.shader#L40-L48

### 2. VPOSの入力を受けて、直接スクリーン座標を取得する
https://github.com/vzmc/GraphicAcademyAssignments/blob/02f2b85853f3c802e8fb64751e0b86f5d762b4eb/GraphicAcademyAssignments_BuildInPipeline/Assets/Assignments/Day6/Dither/DitherShader.shader#L48
frag shaderの入力に`UNITY_VPOS_TYPE screenPos : VPOS`を定義すると、そのままスクリーン座標を取得できる

ただし、frag shaderの入力に`SV_POSITION`と`VPOS`が同時に定義してはいけない、さもなければこのエラーになる
![image](https://user-images.githubusercontent.com/6869650/162442467-ccb487b5-552a-47d8-9712-f6a70ab57bef.png)

`SV_POSITION`はfrag shaderで使わないので、それを解決するためにfrag shaderの入力`v2f構造体`から、`SV_POSITION`を除いて、
vert shaderで`out引数`で`v2f`に入れずに直接`SV_POSITION`を出力する。
https://github.com/vzmc/GraphicAcademyAssignments/blob/02f2b85853f3c802e8fb64751e0b86f5d762b4eb/GraphicAcademyAssignments_BuildInPipeline/Assets/Assignments/Day6/Dither/DitherShader.shader#L27-L30
https://github.com/vzmc/GraphicAcademyAssignments/blob/02f2b85853f3c802e8fb64751e0b86f5d762b4eb/GraphicAcademyAssignments_BuildInPipeline/Assets/Assignments/Day6/Dither/DitherShader.shader#L38-L46

## スクリーン座標を取得できたら、ディザデータが定義されてるテクスチャを取得する
Unityのビルドイン変数に、`_DitherMaskLOD2D`というテクスチャが定義されているので、Sampler2Dで直接取得できる
https://github.com/vzmc/GraphicAcademyAssignments/blob/02f2b85853f3c802e8fb64751e0b86f5d762b4eb/GraphicAcademyAssignments_BuildInPipeline/Assets/Assignments/Day6/Dither/DitherShader.shader#L36

### この_DitherMaskLOD2Dは以下のような形になる
横4px、縦4pxで1コマ、縦方向に16コマがあって、上から下までディザの密度が上がる。
ディザ情報は`_DitherMaskLOD2D`のアルファチャンネルに保存され、黒ゴマは0、白ゴマは1。

![image](https://user-images.githubusercontent.com/6869650/162446006-e61b120e-00d3-4c1a-8001-0e295efb8fc7.png)


## ディザテクスチャサンプリングするためのUV計算
まずスクリーン座標を`4 x 4`のコマをサンプリングするため、スクリーン座標の`x, y`を`0 ~ ScreenWidth`, `0 ~ ScreenHeight`範囲からそれぞれ`0 ~ 1`のUVにRemapする。
https://github.com/vzmc/GraphicAcademyAssignments/blob/02f2b85853f3c802e8fb64751e0b86f5d762b4eb/GraphicAcademyAssignments_BuildInPipeline/Assets/Assignments/Day6/Dither/DitherShader.shader#L50-L51

次、ディザテクスチャの縦方向は16コマがあるので、縦方向の最初の1コマに当てはまるために16で割る
https://github.com/vzmc/GraphicAcademyAssignments/blob/02f2b85853f3c802e8fb64751e0b86f5d762b4eb/GraphicAcademyAssignments_BuildInPipeline/Assets/Assignments/Day6/Dither/DitherShader.shader#L52-L53

最後、0 ~ 1範囲のAlphaでディザの疎密をコントロールするために、Alphaで縦方向のOffsetを計算し、DitherUV.yに足す。
https://github.com/vzmc/GraphicAcademyAssignments/blob/02f2b85853f3c802e8fb64751e0b86f5d762b4eb/GraphicAcademyAssignments_BuildInPipeline/Assets/Assignments/Day6/Dither/DitherShader.shader#L54-L55

### Offset = Alpha x 15/16の理由について
- まずAlpha = 0、つまりOffset = 0の時、最初の1コマがサンプリングされる

![image](https://user-images.githubusercontent.com/6869650/162451136-ed40a251-433e-41f4-922c-37ca5b273905.png)

- Alpha = 1の時に、最後の1コマをサンプリングしたいので、その時のOffset = 15 / 16

![image](https://user-images.githubusercontent.com/6869650/162451961-2101e71d-d3af-4ea0-8cca-105eb2593819.png)

## 最後に、サンプリングしたディザ値でClip処理する
https://github.com/vzmc/GraphicAcademyAssignments/blob/02f2b85853f3c802e8fb64751e0b86f5d762b4eb/GraphicAcademyAssignments_BuildInPipeline/Assets/Assignments/Day6/Dither/DitherShader.shader#L57-L59

## 参考資料
https://light11.hatenadiary.com/entry/2018/07/31/234555
https://light11.hatenadiary.com/entry/2018/07/26/205513
