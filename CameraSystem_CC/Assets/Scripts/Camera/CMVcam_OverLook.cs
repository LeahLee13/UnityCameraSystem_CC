using Cinemachine;
using UnityEngine;

public class CMVcam_OverLook : MonoBehaviour
{
    #region 字段
    private CMVcamManager cmvCamManager;

    [Header("Cinemachine")]
    private CinemachineVirtualCamera virtualCamera;
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
    }
    #endregion

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
}
