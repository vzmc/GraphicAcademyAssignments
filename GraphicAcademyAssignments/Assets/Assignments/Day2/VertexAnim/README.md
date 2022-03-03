# VertexAnim

## VertexAnimShader
VertexShaderで頂点位置をy = A * sin(F * x + T) の式で波動状にする
波動はy方向上起伏し、x方向に流れる
https://github.com/vzmc/GraphicAcademyAssignments/blob/0201b19de3c0c3d92ca860fa8b702eea1d586493/GraphicAcademyAssignments/Assets/Assignments/Day2/VertexAnim/VertexAnimShader.shader#L47-L48
![3](https://user-images.githubusercontent.com/6869650/156200761-a5abb87c-ce3a-4ccb-97f2-84fddfbfd90e.gif)

## VertexAnimShader2
波動の起伏方向をy方向に固定せず、頂点の法線方向にする
流れる方向もx方向に固定せず、uvの方向で設定できるようにした。
そうすることで、球体Mesh上ではこんなぷにぷにな動きにすることもできる
https://github.com/vzmc/GraphicAcademyAssignments/blob/0201b19de3c0c3d92ca860fa8b702eea1d586493/GraphicAcademyAssignments/Assets/Assignments/Day2/VertexAnim/VertexAnimShader2.shader#L52-L54
![3](https://user-images.githubusercontent.com/6869650/156397541-c27ca580-5bb1-466b-868e-4d819e5c6aeb.gif)
