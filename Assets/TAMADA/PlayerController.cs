using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float normalJumpForce = 10f;
    [SerializeField] private float highJumpForce = 18f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Camera mainCamera;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private BoxCollider2D boxCollider;

    [Header("非表示にしておくオブジェクト4つ")]
    [SerializeField] private GameObject[] hiddenObjects = new GameObject[4];

    private bool isGrounded;
    private bool isCrouching;
    private float crouchStartTime;
    private bool isGameOver = false;

    private Vector3 originalScale;
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;

    private KeyCode moveLeftKey = KeyCode.A;
    private KeyCode moveRightKey = KeyCode.F;
    private KeyCode jumpKey = KeyCode.J;
    private KeyCode crouchKey = KeyCode.L;

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (boxCollider == null) boxCollider = GetComponent<BoxCollider2D>();
        if (mainCamera == null) mainCamera = Camera.main;

        originalScale = transform.localScale;

        if (boxCollider != null)
        {
            originalColliderSize = boxCollider.size;
            originalColliderOffset = boxCollider.offset;
        }

        // 最初はすべて非表示
        SetObjectsActive(false);
    }

    void Update()
    {
        isGrounded = CheckGroundedByCollider();

        float move = 0f;
        if (Input.GetKey(moveLeftKey)) move = -1f;
        if (Input.GetKey(moveRightKey)) move = 1f;

        if (!isCrouching)
        {
            rb.velocity = new Vector2(move * moveSpeed, rb.velocity.y);
        }

        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            float jumpForce = normalJumpForce;

            if (isCrouching)
            {
                float crouchDuration = Time.time - crouchStartTime;
                if (crouchDuration >= 0.7f)
                {
                    jumpForce = highJumpForce;
                }
                isCrouching = false;
                ResetCrouch();
            }

            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        if (Input.GetKeyDown(crouchKey))
        {
            isCrouching = true;
            crouchStartTime = Time.time;
            StartCrouch();
        }

        if (Input.GetKeyUp(crouchKey))
        {
            isCrouching = false;
            ResetCrouch();
        }

         //Pキーが押されたときにCutメソッドを呼び出す
      if (Input.GetKeyDown(KeyCode.P))
       {
            Cut();
      }

        ClampPositionAndCheckFall();
    }

    private bool CheckGroundedByCollider()
    {
        Bounds bounds = boxCollider.bounds;
        Vector2 bottomCenter = new Vector2(bounds.center.x, bounds.min.y - 0.05f);
        Vector2 checkSize = new Vector2(bounds.size.x * 0.9f, 0.02f);
        return Physics2D.OverlapBox(bottomCenter, checkSize, 0f, groundLayer);
    }

    private void ClampPositionAndCheckFall()
    {
        Vector3 pos = transform.position;

        Vector3 min = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector3 max = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));

        float halfWidth = boxCollider.bounds.extents.x;
        float halfHeight = boxCollider.bounds.extents.y;

        pos.x = Mathf.Clamp(pos.x, min.x + halfWidth, max.x - halfWidth);
        pos.y = Mathf.Clamp(pos.y, min.y - 100f, max.y - halfHeight);

        transform.position = pos;

        if (!isGameOver && transform.position.y < min.y - 1f)
        {
            isGameOver = true;
            if (StageManager.Instance != null)
            {
                StageManager.Instance.SetGameOver();
            }
            else
            {
                Debug.LogWarning("StageManager.Instance が存在しません！");
            }
        }
    }

    void StartCrouch()
    {
        transform.localScale = new Vector3(originalScale.x, originalScale.y * 0.5f, originalScale.z);

        if (boxCollider != null)
        {
            boxCollider.size = new Vector2(originalColliderSize.x, originalColliderSize.y * 0.5f);
            boxCollider.offset = new Vector2(originalColliderOffset.x, originalColliderOffset.y - originalColliderSize.y * 0.25f);
        }
    }

    void ResetCrouch()
    {
        transform.localScale = originalScale;

        if (boxCollider != null)
        {
            boxCollider.size = originalColliderSize;
            boxCollider.offset = originalColliderOffset;
        }
    }

    // Cutメソッド（非表示のオブジェクトを表示した後にPlayerControllerのオブジェクトを非表示）
    public void Cut()
    {
        if (hiddenObjects == null || hiddenObjects.Length == 0)
        {
            Debug.LogWarning("表示させるオブジェクトが設定されていません！");
            return;
        }

        // 非表示のオブジェクトを再表示
        SetObjectsActive(true);

        // 表示されるオブジェクトの位置を現在のオブジェクトの位置に設定
        foreach (GameObject obj in hiddenObjects)
        {
            if (obj != null)
            {
                obj.transform.position = transform.position; // 現在のオブジェクトの位置に設定
            }
        }

        // PlayerControllerが付いているオブジェクトを非表示
        gameObject.SetActive(false); // このオブジェクト（PlayerControllerが付いているもの）を非表示
    }

    // オブジェクトの表示/非表示を設定
    private void SetObjectsActive(bool isActive)
    {
        foreach (GameObject obj in hiddenObjects)
        {
            if (obj != null)
            {
                obj.SetActive(isActive);
            }
        }
    }
}
