using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class TangJiangHazardZone : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string deathTitle = "Take a breath";
    [SerializeField] private string deathMessage = "You fell into the syrup.";
    [SerializeField] private bool reloadSceneWhenNoUi = true;

    private bool triggered;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag))
        {
            return;
        }

        SetJumpBlocked(other, true);
        ShowDeath();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            SetJumpBlocked(other, true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            SetJumpBlocked(other, false);
        }
    }

    private void ShowDeath()
    {
        if (triggered)
        {
            return;
        }

        triggered = true;

        LevelUIController ui = LevelUIController.FindOrCreate();

        if (ui != null)
        {
            ui.ShowFailure(deathTitle, deathMessage);
            return;
        }

        if (reloadSceneWhenNoUi)
        {
            Scene activeScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(activeScene.name);
        }
    }

    private static void SetJumpBlocked(Collider2D playerCollider, bool blocked)
    {
        PlayerController2D controller = playerCollider.GetComponentInParent<PlayerController2D>();

        if (controller != null)
        {
            controller.SetJumpBlocked(blocked);
        }
    }
}
