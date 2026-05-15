using UnityEngine;

public class AutoRotator : MonoBehaviour
{
    [Header("旋转设置")]
    [Tooltip("旋转速度（度/秒，正数顺时针，负数逆时针）")]
    public float rotationSpeed = 90f; // 默认为每秒顺时针旋转90度

    [Tooltip("是否启用旋转")]
    public bool isRotating = true;

    private void Update()
    {
        if (isRotating)
        {
            // 计算每帧旋转角度（Time.deltaTime确保不同帧率下速度一致）
            float rotateAngle = rotationSpeed * Time.deltaTime;
            // 围绕Z轴旋转（2D物体的旋转轴）
            transform.Rotate(0, 0, rotateAngle);
        }
    }

    // 可选：通过代码控制旋转开关
    public void ToggleRotation()
    {
        isRotating = !isRotating;
    }

    // 可选：设置旋转方向和速度
    public void SetRotationSpeed(float speed)
    {
        rotationSpeed = speed;
    }
}