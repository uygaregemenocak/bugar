using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public string targetSceneName = "DungeonScene";
    public string message = "Portala girmek icin E'ye bas";

    public GameObject dialogBox;
    public TMP_Text dialogText;

    private bool playerNear;

    void Update()
    {
        if (playerNear && Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene(targetSceneName);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = true;

            if (dialogBox != null && dialogText != null)
            {
                dialogBox.SetActive(true);
                dialogText.text = message;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = false;

            if (dialogBox != null)
            {
                dialogBox.SetActive(false);
            }
        }
    }
}
