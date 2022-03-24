# MultiTextureShaderGraph
## ShaderGraphで簡単にLitShaderと似ている複数テクスチャ対応のShaderが作れる
![image](https://user-images.githubusercontent.com/6869650/159930137-ae3031b9-b6bd-4066-b6b0-421177b0c09c.png)
![image](https://user-images.githubusercontent.com/6869650/159930210-ad51cb24-cb2a-42ca-8656-e780ec9a0ca5.png)

## ShaderGraph一覧
![image](https://user-images.githubusercontent.com/6869650/159931370-c6e3c66a-586a-473f-891f-3a8bdb0b2156.png)

### BaseMap 自身の色を定義するテクスチャ
BlackboardにTexture2DタイプのBaseMap変数を作る

![image](https://user-images.githubusercontent.com/6869650/159932422-685d7feb-d590-4567-8fe9-90484802b320.png)

SampleTexture2Dの使ってBaseMapをサンプリングし、結果のRGBをFragmentのBaseColorに接続して、AをFragmentのAlphaに接続する

![image](https://user-images.githubusercontent.com/6869650/159932892-1f7403af-29e1-40a8-8412-cb57f5830d47.png)

### NormalMap 法線方向を定義するテクスチャ
BlackboardにTexture2DタイプのNormalMap変数を作る
![image](https://user-images.githubusercontent.com/6869650/159933367-6c948a9f-0903-42d7-a474-2143419bb20a.png)

SampleTexture2Dを使ってNormalMapをサンプリングし、結果のRGBをFragmentのNormalに接続する

ここに注意するのは、SampleTexture2DのTypeを`Normal`に設定し、Spaceを`Tangent`にする必要がある

![image](https://user-images.githubusercontent.com/6869650/159933700-f3b4079f-9307-4da7-9ef2-92c7abb80b56.png)

### MetallicMapとSmooth 金属の部分とその部分の金属具合を定義するテクスチャと変数
BlackboardにTexture2DタイプのMetallicMapとFloatタイプのSmoothを作る

![image](https://user-images.githubusercontent.com/6869650/159935223-f4bcb8d9-31b2-4846-b502-e0d56b085c07.png)

Smoothを0~1の範囲に収めるためSliderに設定する

![image](https://user-images.githubusercontent.com/6869650/159935654-f5c22f27-c433-433a-8de6-ddaa34accda2.png)

まずMetallicMapをSampleTexture2Dでサンプリングして、RをFragmentのMetallicに接続する。次にRとSmoothと乗算してFragmentのSmoothnessい接続する。
そうすることで、Smoothでコントロールする場所は金属部分に限定される

![image](https://user-images.githubusercontent.com/6869650/159936616-8bfd00ed-988e-4b76-b988-c3b7adc569c5.png)

### AOMap 環境光遮蔽を定義するテクスチャ
BlackboardにTexture2DタイプのAOMapを作る

![image](https://user-images.githubusercontent.com/6869650/159936956-20b27fc8-652c-42a0-9860-cc5d7a4814f8.png)

前と同じようにSampleTexture2Dでサンプリングして、RをFragmentのAmbientOcclusionに接続する

![image](https://user-images.githubusercontent.com/6869650/159937404-d57b568f-fbaa-4870-9d7f-302477ea0d68.png)

## マテリアル作って、テクスチャを設定して、URPのLitShaderとほぼ同じ見栄えになる
![image](https://user-images.githubusercontent.com/6869650/159938032-c3b7b235-5c26-44d6-8da0-1476833215ed.png)
