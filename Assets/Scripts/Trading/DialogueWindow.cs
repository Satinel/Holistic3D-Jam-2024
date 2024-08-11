using UnityEngine;
using TMPro;

public class DialogueWindow : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _customerDialogueText;
    [SerializeField] float _messageSpeed = 5f;
    float _timer = 0;

    void Start()
    {
        Dialogue.OnLineSpoken += Dialogue_OnLineSpoken;
        gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        Dialogue.OnLineSpoken -= Dialogue_OnLineSpoken;
    }

    void Update()
    {
        _timer += Time.deltaTime;

        if(_timer > _messageSpeed)
        {
            gameObject.SetActive(false);
            _customerDialogueText.text = string.Empty;
        }
    }

    void Dialogue_OnLineSpoken(string line)
    {
        _customerDialogueText.text = line;
        _timer = 0;
        gameObject.SetActive(true);
    }

    public void SetMessageSpeed(float speed) // UI Button (maybe one day!)
    {
        _messageSpeed = speed;
    }
}
