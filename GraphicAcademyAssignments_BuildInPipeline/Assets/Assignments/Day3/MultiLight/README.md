# マルチライト対応 (README更新中)
## 重要ライト、非重要ライトともに対応したShader
![image](https://user-images.githubusercontent.com/6869650/159719201-bd65c56f-bde2-469e-99af-b870f06139ce.png)

## マルチライト対応するには2つのPassを作る
### 平行光、非重要ライトを処理するためForwardBase Passを作る
https://github.com/vzmc/GraphicAcademyAssignments/blob/102f3c30195ce455c16cccc51aababd195304b30/GraphicAcademyAssignments_BuildInPipeline/Assets/Assignments/Day3/MultiLight/MultiLightShader.shader#L109
PassのTagsに`"LightMode"="ForwardBase"`を入れれば、そのPassはForwardBaseになる

https://github.com/vzmc/GraphicAcademyAssignments/blob/102f3c30195ce455c16cccc51aababd195304b30/GraphicAcademyAssignments_BuildInPipeline/Assets/Assignments/Day3/MultiLight/MultiLightShader.shader#L114
次にCGPROGRAMブロックの先頭に`#pragma multi_compile_fwdbase`を追加する必要がある。そうすることで、ForwardBase処理するための変数やマクロが定義される。

### ForwardBase Passの頂点シェーダに環境光と非重要ライトの処理をする
https://github.com/vzmc/GraphicAcademyAssignments/blob/102f3c30195ce455c16cccc51aababd195304b30/GraphicAcademyAssignments_BuildInPipeline/Assets/Assignments/Day3/MultiLight/MultiLightShader.shader#L47-L61

#### ForwardBase Passでのみ動かせるためのマクロ定義
`#if UNITY_SHOULD_SAMPLE_SH` のマクロ定義はUnityCG.cgincにあり、以下の形である
```
#define UNITY_SHOULD_SAMPLE_SH (defined(LIGHTPROBE_SH) && !defined(UNITY_PASS_FORWARDADD) && !defined(UNITY_PASS_PREPASSBASE) && !defined(UNITY_PASS_SHADOWCASTER) && !defined(UNITY_PASS_META))
```
一見複雑な条件があるが、実質`#if UNITY_SHOULD_SAMPLE_SH`に囲まれるブロックはForwardBase Passでのみ動く。

https://github.com/vzmc/GraphicAcademyAssignments/blob/102f3c30195ce455c16cccc51aababd195304b30/GraphicAcademyAssignments_BuildInPipeline/Assets/Assignments/Day3/MultiLight/MultiLightShader.shader#L51
`#ifdef VERTEXLIGHT_ON`のマクロの囲まれるブロックは、シーンに非重要ライトが存在時のみ動く。それで囲まないと、非重要ライトが存在しない時も、最後に存在した非重要ライトの色情報が入ってしまう。

`#ifdef VERTEXLIGHT_ON`は上述の`#pragma multi_compile_fwdbase`によって機能する。

#### 4つまでの非重要ライト処理
https://github.com/vzmc/GraphicAcademyAssignments/blob/102f3c30195ce455c16cccc51aababd195304b30/GraphicAcademyAssignments_BuildInPipeline/Assets/Assignments/Day3/MultiLight/MultiLightShader.shader#L53-L57
`Shade4PointLights`の関数を使って、4つまでのの非重要ライト情報を処理してくれて、最後の色を返してくれる。
引数に必要な非重要ライトの情報変数は`#include "UnityCG.cginc"`によって一緒にインクルードされている

#### それ以上の非重要ライトはShadeSH9で球面調和ライトとして処理される
https://github.com/vzmc/GraphicAcademyAssignments/blob/102f3c30195ce455c16cccc51aababd195304b30/GraphicAcademyAssignments_BuildInPipeline/Assets/Assignments/Day3/MultiLight/MultiLightShader.shader#L60
`ShadeSH9`の関数を使えば残りの非重要ライトが球面調和ライトとして処理される
`ShadeSH9`の関数も同じく`UnityCG.cginc`に定義されている

### ForwardBase PassのフラグメントシェーダにはSingleLightの時と同様に自身とライトの色を処理する
https://github.com/vzmc/GraphicAcademyAssignments/blob/102f3c30195ce455c16cccc51aababd195304b30/GraphicAcademyAssignments_BuildInPipeline/Assets/Assignments/Day3/MultiLight/MultiLightShader.shader#L68-L81

### 平行光以外の重要ライトを処理するためForwardAdd Passを作る
https://github.com/vzmc/GraphicAcademyAssignments/blob/102f3c30195ce455c16cccc51aababd195304b30/GraphicAcademyAssignments_BuildInPipeline/Assets/Assignments/Day3/MultiLight/MultiLightShader.shader#L109
PassのTagsに`"LightMode"="ForwardAdd"`を入れれば、そのPassはForwardAddになる

### ForwardAdd Passのフラグメントシェーダに光の減衰も考慮する
https://github.com/vzmc/GraphicAcademyAssignments/blob/102f3c30195ce455c16cccc51aababd195304b30/GraphicAcademyAssignments_BuildInPipeline/Assets/Assignments/Day3/MultiLight/MultiLightShader.shader#L83-L93
Unityでのライト減衰値はライト減衰テクスチャの保存されてあり、ライトとフラグメントの距離をUV値とし、減衰テクスチャをサンプリングした結果の中にある`UNITY_ATTEN_CHANNEL`がその減衰値となる。またPointライトとSpotライトの減衰テクスチャのを取得する方法は異なる。

## 全Passに共通で使われる部分重複定義しないために
`CGINCLUDE ~ ENDCG`で共通に使われる部分を囲むと、全Passに自動インクルードするため重複定義がなくなって、コードが簡潔になる。
また、`CGINCLUDE ~ ENDCG`の定義はShader直下に定義すると全SubShader共通になり、SubShader直下に定義するとそのSubShader内の全Pass共通になる
