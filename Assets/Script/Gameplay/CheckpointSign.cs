using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CheckpointSign : MonoBehaviour
{
    public int checkpointIndex;
    public bool readIndexFromName = true;
    public string playerTag = "Player";
    public Transform respawnPoint;
    public Vector3 respawnOffset = new(0f, 0.5f, 0f);

    private void Awake()
    {
        if (readIndexFromName)
        {
            checkpointIndex = GetCheckpointIndexFromName(gameObject.name, checkpointIndex);
        }

        Collider2D signCollider = GetComponent<Collider2D>();

        if (signCollider == null)
        {
            signCollider = gameObject.AddComponent<BoxCollider2D>();
        }

        signCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TrySave(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TrySave(other);
    }

    private void TrySave(Collider2D other)
    {
        if (other == null || !other.CompareTag(playerTag))
        {
            return;
        }

        if (checkpointIndex < 0)
        {
            Debug.LogWarning("CheckpointSign needs a checkpoint index of 0 or higher, or a name like Sign_0/sign1.", this);
            return;
        }

        Vector3 position = respawnPoint != null ? respawnPoint.position : transform.position + respawnOffset;
        CheckpointManager.SaveCheckpoint(checkpointIndex, position);
    }

    private int GetCheckpointIndexFromName(string objectName, int fallbackIndex)
    {
        int value = 0;
        bool hasDigit = false;

        foreach (char character in objectName)
        {
            if (!char.IsDigit(character))
            {
                continue;
            }

            hasDigit = true;
            value = value * 10 + character - '0';
        }

        return hasDigit ? value : fallbackIndex;
    }
}
