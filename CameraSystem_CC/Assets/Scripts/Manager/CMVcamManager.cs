using UnityEngine;
using StarterAssets;

public class CMVcamManager : MonoSingleton<CMVcamManager>
{
    #region 字段
    #region 角色
    [HideInInspector]
    public PlayerController_CC playerController;
    [HideInInspector]
    public PlayerInputController_CC inputController;
    #endregion

    #region 摄像机
    [Header("玩家身上的跟随目标")]
    [HideInInspector]
    public Transform followTarget_1st;
    [HideInInspector]
    public Transform followTarget_3rd;
    [HideInInspector]
    public Transform followTarget_Free;

    [Header("虚拟相机")]
    public CMVcam_1st cmvCam_1st;
    public CMVcam_3rd cmvCam_3rd;
    public CMVcam_Free cmvCam_Free;
    public CMVcam_Quarter CMVcam_Quarter;
    public CMVcam_OverLook CMVcam_OverLook;

    [Header("切换视角")]
    public KeyCode is1st = KeyCode.V;
    public bool is1st_ = false;

    public KeyCode is3rd = KeyCode.B;
    public bool is3rd_ = false;

    public KeyCode isQuarter = KeyCode.N;
    public bool isQuarter_ = false;

    public KeyCode isOverLook = KeyCode.M;
    public bool isOverLook_ = false;
    #endregion

    #region 鼠标
    [Header("鼠标显示/隐藏")]
    public KeyCode isCursorLocked = KeyCode.Alpha0;
    public bool isCursorLocked_ = false;
    #endregion
    #endregion

    #region 生命周期函数
    void Start()
    {
        is1st_ = false;
        is3rd_ = false;
        isQuarter_ = false;
        isOverLook_ = false;

        cmvCam_Free.SetEnable(true);
        cmvCam_1st.SetEnable(false);
        cmvCam_3rd.SetEnable(false);
        CMVcam_Quarter.SetEnable(false);
        CMVcam_OverLook.SetEnable(false);
    }

    void Update()
    {
        if (playerController == null)
        {
            GameObject playerObj = GameManager.Instance.playerObj;

            if (playerObj == null)
            {
                return;
            }
            else
            {
                playerController = playerObj.GetComponent<PlayerController_CC>();
                inputController = playerObj.GetComponent<PlayerInputController_CC>();
            }
        }

        if (playerController != null)
        {
            if (followTarget_1st == null)
            {
                followTarget_1st = playerController.followTarget_1st;
            }

            if (followTarget_3rd == null)
            {
                followTarget_3rd = playerController.followTarget_3rd;
            }

            if (followTarget_Free == null)
            {
                followTarget_Free = playerController.followTarget_Free;
            }
        }

        Input();
    }
    #endregion

    #region 方法
    private void Input()
    {
        //按下0，切换是否显示鼠标
        if (UnityEngine.Input.GetKeyDown(isCursorLocked))
        {
            isCursorLocked_ = !isCursorLocked_;
            SetCursorState(isCursorLocked_);
        }

        #region 切换视角
        //第一人称视角/自由视角切换
        if (UnityEngine.Input.GetKeyDown(is1st))
        {
            is1st_ = !is1st_;
            is3rd_ = false;
            isQuarter_ = false;
            isOverLook_ = false;

            cmvCam_Free.SetEnable(!is1st_);
            cmvCam_1st.SetEnable(is1st_);
            cmvCam_3rd.SetEnable(is3rd_);
            CMVcam_Quarter.SetEnable(isQuarter_);
            CMVcam_OverLook.SetEnable(isOverLook_);

            playerController.mesh.SetActive(!is1st_);
            playerController.mesh_CutOut.SetActive(is1st_);
        }

        //第三人称过肩视角/自由视角切换
        if (UnityEngine.Input.GetKeyDown(is3rd))
        {
            is3rd_ = !is3rd_;
            is1st_ = false;
            isQuarter_ = false;
            isOverLook_ = false;

            cmvCam_Free.SetEnable(!is3rd_);
            cmvCam_1st.SetEnable(is1st_);
            cmvCam_3rd.SetEnable(is3rd_);
            CMVcam_Quarter.SetEnable(isQuarter_);
            CMVcam_OverLook.SetEnable(isOverLook_);

            playerController.mesh.SetActive(true);
            playerController.mesh_CutOut.SetActive(false);
        }

        //斜45度视角/自由视角切换
        if (UnityEngine.Input.GetKeyDown(isQuarter))
        {
            isQuarter_ = !isQuarter_;
            is1st_ = false;
            is3rd_ = false;
            isOverLook_ = false;

            cmvCam_Free.SetEnable(!isQuarter_);
            cmvCam_1st.SetEnable(is1st_);
            cmvCam_3rd.SetEnable(is3rd_);
            CMVcam_Quarter.SetEnable(isQuarter_);
            CMVcam_OverLook.SetEnable(isOverLook_);

            playerController.mesh.SetActive(true);
            playerController.mesh_CutOut.SetActive(false);
        }

        //俯视角/自由视角切换
        if (UnityEngine.Input.GetKeyDown(isOverLook))
        {
            isOverLook_ = !isOverLook_;
            is1st_ = false;
            is3rd_ = false;
            isQuarter_ = false;

            cmvCam_Free.SetEnable(!isOverLook_);
            cmvCam_1st.SetEnable(is1st_);
            cmvCam_3rd.SetEnable(is3rd_);
            CMVcam_Quarter.SetEnable(isQuarter_);
            CMVcam_OverLook.SetEnable(isOverLook_);

            playerController.mesh.SetActive(true);
            playerController.mesh_CutOut.SetActive(false);
        }
        #endregion
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
    #endregion
}
