using Cinemachine;
using UnityEngine;

public class CMVcam_3rd : MonoBehaviour
{
    #region 字段
    private CMVcamManager cmvCamManager;

    [Header("Cinemachine")]
    private CinemachineVirtualCamera virtualCamera;

    private float _cinemachineTargetPitch;
    private const float _threshold = 0.01f;

    [Header("Cinemachine_1st")]
    [Tooltip("摄像机向上移动最大角度")]
    public float topClamp_3rd = 80.0f;
    [Tooltip("摄像机向下移动最大角度")]
    public float bottomClamp_3rd = -40.0f;
    [Tooltip("旋转速度")]
    public float rotationSpeed = 1.0f;
    [Tooltip("额外的度数来覆盖摄像头，锁定时用于微调相机位置")]
    public float cameraAngleOverride = 0.0f;

    #region 鼠标
    private float mouseY;
    #endregion

    #region 旋转
    [Header("Controller Input")]
    public bool isMoveCamera_ = false;

    public KeyCode isMouseBtnControlCamera = KeyCode.RightControl;
    public bool isMouseBtnControlCamera_ = true;

    public bool IsMouseBtnControlCamera
    {
        get
        {
            if (isMouseBtnControlCamera_)
            {
                if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
    }
    #endregion
    #endregion

    #region 生命周期函数
    void Awake()
    {
        cmvCamManager = CMVcamManager.Instance;
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    void LateUpdate()
    {
        if (cmvCamManager.followTarget_3rd == null)
        {
            if (this.virtualCamera.Follow == null)
            {
                this.virtualCamera.Follow = null;
            }

            if (this.virtualCamera.LookAt == null)
            {
                this.virtualCamera.LookAt = null;
            }

            return;
        }

        if (this.virtualCamera.Follow == null)
        {
            this.virtualCamera.Follow = cmvCamManager.followTarget_3rd;
        }

        if (this.virtualCamera.LookAt == null)
        {
            this.virtualCamera.LookAt = cmvCamManager.followTarget_3rd;
        }

        mouseY = Input.GetAxis("Mouse Y");

        SetInput();
        RotationCamera();
    }
    #endregion

    #region 方法
    #region 激活
    /// <summary>
    /// 是否激活
    /// </summary>
    /// <param name="isEnabled">是否激活</param>
    public void SetEnable(bool isEnabled = false)
    {
        virtualCamera.enabled = isEnabled;
        enabled = isEnabled;
    }
    #endregion

    #region 输入切换
    private void SetInput()
    {
        //按下右侧Ctrl，切换按下鼠标左键/鼠标移动旋转
        if (Input.GetKeyDown(isMouseBtnControlCamera))
        {
            isMouseBtnControlCamera_ = !isMouseBtnControlCamera_;
        }
    }
    #endregion

    #region 旋转
    /// <summary>
    /// 旋转摄像机
    /// </summary>
    private void RotationCamera()
    {
        Vector2 look = new Vector2(0, -mouseY * rotationSpeed);

        if (IsMouseBtnControlCamera)
        {
            if (look.sqrMagnitude >= _threshold)
            {
                cmvCamManager.playerController.RotationVelocity = look.x;
                _cinemachineTargetPitch += look.y;

                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, bottomClamp_3rd, topClamp_3rd);
                cmvCamManager.followTarget_3rd.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);
                cmvCamManager.playerController.transform.Rotate(Vector3.up * cmvCamManager.playerController.RotationVelocity);
            }
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f) { angle += 360f; }
        if (angle > 360f) { angle -= 360f; }
        return Mathf.Clamp(angle, min, max);
    }
    #endregion
    #endregion
}
