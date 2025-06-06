using UnityEngine;
using Photon.Pun;

public class PlayerMoveController : MonoBehaviourPun
{
    private CharacterController characterController;
    private Transform cameraTransform;
    private PhotonView view;

    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float mouseSensitivity = 1.8f;
    [SerializeField] float gravity = 9.8f;

    private float verticalVelocity = 0f;
    private float cameraPitch = 0f;
    [SerializeField] Camera playerCamera;
    [SerializeField] private Animator animatoController;

    void Start()
    {
        // 自分のオブジェクトでなければ，カメラとこのスクリプトを無効化
        if (!photonView.IsMine)
        {
            cameraTransform = transform.Find("Camera");
            if (cameraTransform != null)
                cameraTransform.gameObject.SetActive(false);

            this.enabled = false; // このスクリプトを止める
            return;
        }

        characterController = GetComponent<CharacterController>();

        mouseSensitivity = GameManager.Instance.MouseSensitivity;
        GameManager.Instance.OnMouseSensitivityChanged += SetMouseSensitivity;

        Transform camTransform = transform.Find("Camera");
        if (camTransform != null)
        {
            cameraTransform = camTransform;
        }
        else
        {
            Debug.LogError("子オブジェクトに 'Camera' が見つかりません");
        }

        cameraPitch = cameraTransform.localEulerAngles.x;

        Cursor.lockState = CursorLockMode.Locked;
        // カーソルを完全に自由にする
        // Cursor.lockState = CursorLockMode.None;
        // Cursor.visible = true;
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            if (!GameManager.Instance.CheckUIActive())
            {
                RotateView();
                MovePlayer();
            }
        }
    }

    void RotateView()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -89f, 89f);
        cameraTransform.localEulerAngles = new Vector3(cameraPitch, 0, 0);
    }

    void MovePlayer()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        move *= moveSpeed;

        // キー入力があればアニメーションを再生
        bool isWalk = moveX != 0 || moveZ != 0;
        animatoController.SetBool("isWalk", isWalk);

        if (characterController.isGrounded)
        {
            verticalVelocity = -1f;
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        move.y = verticalVelocity;
        characterController.Move(move * Time.deltaTime);
    }

    public void SetMouseSensitivity(float value)
    {
        mouseSensitivity = value;
    }
    
    void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnMouseSensitivityChanged -= SetMouseSensitivity;
        }
    }
}
