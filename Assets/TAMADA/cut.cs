using UnityEngine;

public class CloneSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab; // �v���C���[�I�u�W�F�N�g�̃v���n�u
    [SerializeField] private Transform spawnPoint;    // �N���[���̐����ʒu
    [SerializeField] private float jumpForce = 10f;   // �W�����v��

    void Start()
    {
        // �N���[����4����
        CreateClone(KeyCode.A);
        CreateClone(KeyCode.F);
        CreateClone(KeyCode.J);
        CreateClone(KeyCode.L);
    }

    void CreateClone(KeyCode jumpKey)
    {
        // �N���[���𐶐�
        GameObject clone = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
        PlayerCloneController playerController = clone.GetComponent<PlayerCloneController>();

        // �N���[�����ƂɃW�����v�L�[��ݒ�
        if (playerController != null)
        {
            playerController.SetJumpKey(jumpKey, jumpForce);
        }
    }
}

public class PlayerCloneController : MonoBehaviour
{
    [SerializeField] private KeyCode jumpKey;  // �W�����v����L�[
    [SerializeField] private float jumpForce;  // �W�����v��
    private Rigidbody2D rb;   // Rigidbody2D�R���|�[�l���g

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // �W�����v����
        if (Input.GetKeyDown(jumpKey) && Mathf.Abs(rb.velocity.y) < 0.1f)  // �n�ʂɂ��鎞�̂݃W�����v
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // �ړ��������폜�i����Ŗ{�͓̂����Ȃ��j
    }

    // �W�����v�L�[�ƃW�����v�͂�ݒ肷�郁�\�b�h
    public void SetJumpKey(KeyCode key, float force)
    {
        jumpKey = key;
        jumpForce = force;
    }
}
