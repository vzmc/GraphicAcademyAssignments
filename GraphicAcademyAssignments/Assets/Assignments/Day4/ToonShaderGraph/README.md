# ToonShaderGraph
![image](https://user-images.githubusercontent.com/6869650/159939012-de43a07a-1550-4fb3-bfc5-9234673ef4d6.png)![image](https://user-images.githubusercontent.com/6869650/160114317-4dfb7712-9138-4150-a5e3-d1f33d7cd689.png)

## SubGraph機能について
プログラムの関数のように、InputとOutputを決めて、機能分けて処理手続きをラップしてSubGraphを作っておくと、MainGraphが簡潔に保てる

## ShaderGraph一覧
### MainGraph
ToonShaderを作るために、MasterStackをUnlitにして、最終Colorのみ受け取るFragment Nodeに設定する。
![image](https://user-images.githubusercontent.com/6869650/159939329-00bcf561-946a-457f-96bc-3431bc14190c.png)

### SubGraph: ToonAlbedo
![image](https://user-images.githubusercontent.com/6869650/159939458-caee3a70-d846-4753-8e4d-2a8a6632994e.png)

### SubGraph: ToonDiffuse
光の取得はCustom Functionを使う

Custom Functonは任意コードを実行することができて、そこからUnityが提供するライブラリから光情報を取得できる
```
GetMainLight();
```
![image](https://user-images.githubusercontent.com/6869650/160115625-e94a704f-0223-4310-bca2-79c9e8a69762.png)
![image](https://user-images.githubusercontent.com/6869650/160115648-8f67fd5c-ea83-43b4-9b02-db804753ceb6.png)
![image](https://user-images.githubusercontent.com/6869650/159939632-f8d0c038-43ed-45ce-9fe5-4a76dba9d8fb.png)

ライト方向と法線方向との内積を計算して、結果の`-1~1`を`0~1`にRemapして、得た値をUV座標として、Rampテクスチャの色をサンプリングして拡散反射光色にする
![image](https://user-images.githubusercontent.com/6869650/160118944-3ee73558-bdec-4954-a1c9-dae5601f5003.png)

### SubGraph: ToonSpecular
![image](https://user-images.githubusercontent.com/6869650/159939819-26a179ff-99ec-4c41-be8a-082deadeb116.png)

Specular値計算するGraph一見複雑だが、コードで表すとこうなる
https://github.com/vzmc/GraphicAcademyAssignments/blob/81d43e8e55075f5441a72df7b2d9ba4a9ff92a3b/GraphicAcademyAssignments_BuildInPipeline/Assets/Assignments/Day3/Toon/ToonShader.shader#L86-L89

こちらのspec値計算はBlinn-Phoneモデルを使ている

次にsmoothstepとfwidthを使ってるのはSpecularライト部分のAnti-Aliasingを付けるため

使わないと縁側にジグザグになるが
![image](https://user-images.githubusercontent.com/6869650/160120868-8cd3a9b6-409d-4f19-8056-292bfed210a2.png)

smoothstepとfwidth合わせて使うと縁側は円滑になる
![image](https://user-images.githubusercontent.com/6869650/160121011-159cdc4f-2c21-4712-a047-17252358fef2.png)



