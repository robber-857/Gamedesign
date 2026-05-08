using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour
{
    private const string LegacySceneKey = "Checkpoint.Scene";
    private const string LegacyIndexKey = "Checkpoint.Index";
    private const string LegacyXKey = "Checkpoint.X";
    private const string LegacyYKey = "Checkpoint.Y";
    private const string LegacyZKey = "Checkpoint.Z";

    private static CheckpointManager instance;
    private static bool hasCheckpoint;
    private static string checkpointSceneName;
    private static int checkpointIndex;
    private static Vector3 checkpointPosition;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void EnsureInstance()
    {
        if (instance != null)
        {
            return;
        }

        GameObject managerObject = new("CheckpointManager");
        instance = managerObject.AddComponent<CheckpointManager>();
        DontDestroyOnLoad(managerObject);
        ClearLegacySavedCheckpoint();
    }

    private static void ClearLegacySavedCheckpoint()
    {
        PlayerPrefs.DeleteKey(LegacySceneKey);
        PlayerPrefs.DeleteKey(LegacyIndexKey);
        PlayerPrefs.DeleteKey(LegacyXKey);
        PlayerPrefs.DeleteKey(LegacyYKey);
        PlayerPrefs.DeleteKey(LegacyZKey);
        PlayerPrefs.Save();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    public static void SaveCheckpoint(int checkpointIndex, Vector3 position)
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (hasCheckpoint && checkpointSceneName == sceneName && checkpointIndex < CheckpointManager.checkpointIndex)
        {
            return;
        }

        hasCheckpoint = true;
        checkpointSceneName = sceneName;
        CheckpointManager.checkpointIndex = checkpointIndex;
        checkpointPosition = position;
    }

    public static bool HasCheckpointForCurrentScene()
    {
        return hasCheckpoint && checkpointSceneName == SceneManager.GetActiveScene().name;
    }

    public static void ClearCurrentSceneCheckpoint()
    {
        if (checkpointSceneName != SceneManager.GetActiveScene().name)
        {
            return;
        }

        hasCheckpoint = false;
        checkpointSceneName = null;
        checkpointIndex = 0;
        checkpointPosition = Vector3.zero;
    }

    public static void RespawnAtCheckpointNow()
    {
        if (instance == null)
        {
            EnsureInstance();
        }

        instance.StartCoroutine(instance.ApplyCheckpointAfterFrame());
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(ApplyCheckpointAfterFrame());
    }

    private IEnumerator ApplyCheckpointAfterFrame()
    {
        yield return null;

        if (!HasCheckpointForCurrentScene())
        {
            yield break;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            yield break;
        }

        Rigidbody2D playerBody = player.GetComponent<Rigidbody2D>();

        if (playerBody != null)
        {
            playerBody.linearVelocity = Vector2.zero;
            playerBody.angularVelocity = 0f;
            playerBody.position = checkpointPosition;
        }

        player.transform.position = checkpointPosition;
    }
}
