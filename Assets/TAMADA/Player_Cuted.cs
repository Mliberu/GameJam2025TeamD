using UnityEngine;

public class Player_Cuted : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;   // �i�ޑ���
    [SerializeField] private float jumpForce = 10f;  // �W�����v�̋���
    [SerializeField] private KeyCode jumpKey = KeyCode.Space; // �W�����v����L�[���C���X�y�N�^�[�őI��

    private Rigidbody2D rb;  // Rigidbody2D�R���|�[�l���g

    void Start()
    {
        // Rigidbody2D�R���|�[�l���g���擾
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // �L�����N�^�[���E�ɐi�߂�
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);

        // �W�����v����
        if (Input.GetKeyDown(jumpKey) && Mathf.Abs(rb.velocity.y) < 0.01f)  // �n�ʂɂ��鎞�̂݃W�����v
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
}
