using TMPro;
using UnityEngine;

public class NPCDialog : MonoBehaviour
{
    public GameObject dialogBox;
    public TMP_Text dialogText;

    public string beforeQuestText = "Portaldan gec ve canavarlari oldur!";
    public string afterQuestText = "Basardin! Tum canavarlari oldurmusun.";
    public string hintText = "Konusmak icin E'ye bas";

    private bool playerNear;

    void Update()
    {
        if (!playerNear) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            dialogBox.SetActive(true);

            if (GameManager.monstersKilled)
            {
                dialogText.text = afterQuestText;
            }
            else
            {
                dialogText.text = beforeQuestText;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = true;
            dialogBox.SetActive(true);
            dialogText.text = hintText;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = false;
            dialogBox.SetActive(false);
        }
    }
}