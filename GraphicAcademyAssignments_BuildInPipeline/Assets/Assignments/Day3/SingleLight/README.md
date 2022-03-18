# シングルライト対応 (README更新中)

## 最終の色はいくつの部分で合成される
### 自身の色 Albedo (テクスチャと修正Colorの乗算)
https://github.com/vzmc/GraphicAcademyAssignments/blob/2234d2360566b98f74b04ced345ad71bbd65e0b9/GraphicAcademyAssignments_BuildInPipeline/Assets/Assignments/Day3/SingleLight/FragmentLightShader.shader#L54-L55
![image](https://user-images.githubusercontent.com/6869650/158923826-72f98790-b7ed-4793-ab81-64732b4e80e0.png) ![image](https://user-images.githubusercontent.com/6869650/158923885-126bfe34-7de9-4117-bd4d-b6ef4d956cfe.png)

### 環境光 Ambient
`UnityCG.cginc`をインクルードすることで一緒に入った`UnityShaderVariables.cginc`の中に定義されてるこの変数から環境設定に設定されている環境光色を取得できる
https://github.com/vzmc/GraphicAcademyAssignments/blob/2234d2360566b98f74b04ced345ad71bbd65e0b9/GraphicAcademyAssignments_BuildInPipeline/Assets/Assignments/Day3/SingleLight/FragmentLightShader.shader#L57-L58
![image](https://user-images.githubusercontent.com/6869650/158927498-bfc80591-6124-4b77-8be7-c49c6a5b6b84.png)

取得した環境光と自身の色と乗算して環境光に影響される結果になる
```
fixed3 finalColor = ambient * albedo;
```
![image](https://user-images.githubusercontent.com/6869650/158924277-1a4d651e-be6d-4eae-823d-717667a27f14.png)

### 拡散反射光 Diffuse
まず法線方向を世界空間に変換し、世界空間のライト方向と内積を取って、saturateで(0,1)範囲に収めて、
その計算結果をライトの色と乗算すれば拡散反射光の色になる
https://github.com/vzmc/GraphicAcademyAssignments/blob/2234d2360566b98f74b04ced345ad71bbd65e0b9/GraphicAcademyAssignments_BuildInPipeline/Assets/Assignments/Day3/SingleLight/FragmentLightShader.shader#L60-L64

ここの`UnityWorldSpaceLightDir(i.posWS)`が世界空間のライト方向を取得できるUnityの便利関数で、
実際その中に、ライトの種類によって異なる処理をやってくれてる
```
// Computes world space light direction, from world space position
inline float3 UnityWorldSpaceLightDir( in float3 worldPos )
{
    #ifndef USING_LIGHT_MULTI_COMPILE
        return _WorldSpaceLightPos0.xyz - worldPos * _WorldSpaceLightPos0.w;
    #else
        #ifndef USING_DIRECTIONAL_LIGHT
        return _WorldSpaceLightPos0.xyz - worldPos;
        #else
        return _WorldSpaceLightPos0.xyz;
        #endif
    #endif
}
```

環境光の上に拡散反射光も加わるとこうなる
```
fixed3 finalColor = (ambient + diffuse) * albedo;
```
![image](https://user-images.githubusercontent.com/6869650/158928719-c8b25447-b4c5-47e4-8dd3-4c615a7479b1.png)

### 鏡面反射光 Specular
1. reflect関数を使ってライトの反射ベクトルを算出する
2. 視線方向算出し、1の結果と内積して(0,1)に収める
3. 2の結果をGloss回自乗して鏡面反射光の強度になる(Glossが大きければ大きいほど鏡面反射光の範囲が縮まる)
4. 3の結果をライトの色と乗算して最後の鏡面反射光色になる

https://github.com/vzmc/GraphicAcademyAssignments/blob/2234d2360566b98f74b04ced345ad71bbd65e0b9/GraphicAcademyAssignments_BuildInPipeline/Assets/Assignments/Day3/SingleLight/FragmentLightShader.shader#L66-L71

![image](https://user-images.githubusercontent.com/6869650/158931086-98ffbbd7-1e5d-44ff-b2ca-bb06118cb463.png)
![image](https://user-images.githubusercontent.com/6869650/158930981-586c2d86-5f23-4648-8fe7-7b36ec295fa1.png)

前述のSpecularは自身の色に影響されるもので、もしSpecularを自身の色に影響したくないなら、
最終色を合成計算の時、Specularを切り離しAlbedoと乗算しなければよい。
そうすることで結構明るい鏡面反射光が得られて、より滑らかな質感を表すことができる
```
fixed3 finalColor = (ambient + diffuse) * albedo + specular; 
```
![image](https://user-images.githubusercontent.com/6869650/158931865-998db034-0eab-4a82-a1f1-3f37d0a928f9.png)

### 完成したシェーダにテクスチャも付けて、モデルに適用するとこうなる

![image](https://user-images.githubusercontent.com/6869650/158932235-9ed4f6f6-51d1-4a86-9684-e6661ca08418.png)


## FragmentLight と VertexLight 
上述のライト処理はFragmentShader内で処理するもので、画素ごとにライト処理をして、滑らか結果になるが、処理コストもかかる。

あまり重要ではないライト処理をVertexShader内にさせて、頂点ごとにライト色を計算して、頂点間の色を自動補間させて画素ごとの色を出すことによって、
雑な色になるが、処理コストが結構軽くなる

![image](https://user-images.githubusercontent.com/6869650/158935953-28078f9a-70ba-46bb-b45a-bac441394d1b.png)

## 影の投射
物体から影を投射するには、ShadowCasterのPassを実装する必要はあるが、
最後のところにUnityビルドインの影を投射するShaderにFallBackしておくと、Shadow CasterのPassを書かなくてもFallBack先のShaderのShadowCasterが適用される
https://github.com/vzmc/GraphicAcademyAssignments/blob/2234d2360566b98f74b04ced345ad71bbd65e0b9/GraphicAcademyAssignments_BuildInPipeline/Assets/Assignments/Day3/SingleLight/FragmentLightShader.shader#L82
