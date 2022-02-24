using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSpaceCheck : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    [ContextMenu("CalcPosition")]
    private void CalcPosition()
    {
        Debug.Log($"カメラの世界座標：{_camera.transform.position}");

        var localPosition = (Vector4)transform.localPosition;
        localPosition.w = 1;
        var modelMatrix = transform.localToWorldMatrix;
        var worldPosition = modelMatrix * localPosition;
        Debug.Log($"自身の世界座標：{worldPosition}");

        var viewMatrix = _camera.worldToCameraMatrix;
        var vmMatrix = viewMatrix * modelMatrix;
        var cameraSpacePosition = vmMatrix * localPosition;
        // カメラ空間は右手座標系なので、z軸の方向を逆にする必要がある
        cameraSpacePosition.z *= -1;
        Debug.Log($"自身のカメラ空間内の座標：{cameraSpacePosition}");
    }
}
