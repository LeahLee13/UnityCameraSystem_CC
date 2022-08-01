using Cinemachine;
using UnityEngine;

public class CMVcam_Free : MonoBehaviour
{
    #region 字段
    private CMVcamManager cmvCamManager;

    [Header("Cinemachine")]
    private CinemachineVirtualCamera virtualCamera;
    private Cinemachine3rdPersonFollow camera3rdPersonFollow;

    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    private const float _threshold = 0.01f;

    [Header("Cinemachine_3rd")]
    [Tooltip("摄像机向上移动最大角度")]
    public float topClamp_Free = 89.0f;
    [Tooltip("摄像机向下移动最大角度")]
    public float bottomClamp_Free = -30.0f;
    [Tooltip("旋转速度")]
    public float rotationSpeed = 1.0f;
    [Tooltip("额外的度数来覆盖摄像头，锁定时用于微调相机位置")]
    public float cameraAngleOverride = 0.0f;
    [Tooltip("摄像机初始距离")]
    public float cameraDistance = 4.0f;

    #region 鼠标
    private float mouseX;
    private float mouseY;
    #endregion

    #region 旋转
    [Header("Controller Input")]
    public KeyCode resetCamera = KeyCode.P;

    public KeyCode lockCameraPositionY = KeyCode.Alpha9;
    public bool lockCameraPositionY_ = false;

    public KeyCode lockCameraPositionX = KeyCode.Alpha8;
    public bool lockCameraPositionX_ = false;

    public bool isMoveCamera_ = false;

    public KeyCode isMouseBtnControlCamera = KeyCode.RightControl;
    public bool isMouseBtnControlCamera_ = true;

    public bool IsMouseBtnControlCamera
    {
        get
        {
            if (isMouseBtnControlCamera_)
            {
                if (Input.GetMouseButton(0))
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

    #region 缩放
    public KeyCode isMouseScrollZoom = KeyCode.RightAlt;
    public bool isMouseScrollZoom_ = true;

    [Header("Zoom")]
    public float scrollSpeed = 10.0f;
    [Range(-50.0f, 5.0f)]
    public float minZoom = -50.0f;
    [Range(5.0f, 50.0f)]
    public float maxZoom = 50.0f;
    private float zoomCameraDistance;
    #endregion
    #endregion

    #region 生命周期函数
    void Awake()
    {
        cmvCamManager = CMVcamManager.Instance;
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        camera3rdPersonFollow = virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        ResetCamera();
    }

    void LateUpdate()
    {
        if (cmvCamManager.followTarget_Free == null)
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
            this.virtualCamera.Follow = cmvCamManager.followTarget_Free;
        }

        if (this.virtualCamera.LookAt == null)
        {
            this.virtualCamera.LookAt = cmvCamManager.followTarget_Free;
        }

        if (_cinemachineTargetYaw == 0)
        {
            _cinemachineTargetYaw = cmvCamManager.followTarget_Free.transform.rotation.eulerAngles.y;
        }

        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        SetInput();
        RotationCamera();
        ZoomCamera();
        MoveCamera();

        if (Input.GetKeyDown(resetCamera))
        {
            ResetCamera();
        }
    }

    void OnValidate()
    {
        maxZoom = Mathf.Max(minZoom, maxZoom);
    }
    #endregion

    #region 方法
    #region 激活
    /// <summary>
    /// 是否激活
    /// </summary>
    /// <param name="isEnabled">是否激活</param>
    public void SetEnable(bool isEnabled = true)
    {
        virtualCamera.enabled = isEnabled;
        enabled = isEnabled;
    }
    #endregion

    #region 输入切换
    private void SetInput()
    {
        //按下右侧Alt，切换滚轮/鼠标Y轴缩放
        if (Input.GetKeyDown(isMouseScrollZoom))
        {
            isMouseScrollZoom_ = !isMouseScrollZoom_;
        }

        //按下右侧Ctrl，切换按下鼠标左键/鼠标移动旋转
        if (Input.GetKeyDown(isMouseBtnControlCamera))
        {
            isMouseBtnControlCamera_ = !isMouseBtnControlCamera_;
        }

        //按下9，切换禁止Y轴旋转（仅水平旋转）
        if (Input.GetKeyDown(lockCameraPositionY))
        {
            lockCameraPositionY_ = !lockCameraPositionY_;
        }

        //按下8，切换禁止X轴旋转（仅垂直旋转）
        if (Input.GetKeyDown(lockCameraPositionX))
        {
            lockCameraPositionX_ = !lockCameraPositionX_;
        }

        //按下鼠标中键/右键，自由移动摄像机与跟随目标相对位置
        if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            isMoveCamera_ = true;
        }
        else if (Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2))
        {
            isMoveCamera_ = false;
        }
    }
    #endregion

    #region 旋转
    /// <summary>
    /// 旋转摄像机
    /// </summary>
    private void RotationCamera()
    {
        Vector2 look = new Vector2(mouseX * rotationSpeed, -mouseY * rotationSpeed);

        if (look.sqrMagnitude >= _threshold)
        {
            if (IsMouseBtnControlCamera)
            {
                if (!lockCameraPositionX_)
                {
                    _cinemachineTargetYaw += look.x;
                }

                if (!lockCameraPositionY_)
                {
                    _cinemachineTargetPitch += look.y;
                }
            }
        }

        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, bottomClamp_Free, topClamp_Free);
        cmvCamManager.followTarget_Free.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + cameraAngleOverride, _cinemachineTargetYaw, 0.0f);
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f) { angle += 360f; }
        if (angle > 360f) { angle -= 360f; }
        return Mathf.Clamp(angle, min, max);
    }
    #endregion

    #region 缩放
    /// <summary>
    /// 缩放摄像机与跟随目标距离
    /// </summary>
    private void ZoomCamera()
    {
        if (isMouseScrollZoom_)
        {
            zoomCameraDistance = camera3rdPersonFollow.CameraDistance - Input.mouseScrollDelta.y * scrollSpeed * Time.deltaTime;
        }
        else
        {
            zoomCameraDistance = camera3rdPersonFollow.CameraDistance - mouseY * scrollSpeed * Time.deltaTime;
        }

        camera3rdPersonFollow.CameraDistance = Mathf.Clamp(zoomCameraDistance, minZoom, maxZoom);
    }
    #endregion

    #region 移动
    private void MoveCamera()
    {
        if (isMoveCamera_)
        {
            Vector3 offset = new Vector3(-mouseX, mouseY, 0);
            camera3rdPersonFollow.ShoulderOffset += offset * 0.5f;
        }
    }
    #endregion

    #region 重置
    /// <summary>
    /// 重置摄像机
    /// </summary>
    private void ResetCamera()
    {
        camera3rdPersonFollow.CameraDistance = cameraDistance;
        camera3rdPersonFollow.ShoulderOffset = Vector3.zero;
    }
    #endregion
    #endregion
}
