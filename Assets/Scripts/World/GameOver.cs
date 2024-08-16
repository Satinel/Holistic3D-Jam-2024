using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] GameObject[] _mentors;
    [SerializeField] Player _player;
    [SerializeField] GameObject _finalSpeech, _continueButton, _gameOverSplash, _happyEndingSplash, _backgroundImage;
    [SerializeField] GameObject _goodEndButton, _badRepButton, _badDebtButton, _megaBadButton;
    [SerializeField] GameObject _compItems, _playerTradeBox, _scalesParent, _activeButtonsParent;
    [SerializeField] TextMeshProUGUI _finalSpeechText;

    int _mentorIndex = 0;
    int _lineIndex = 0;

    string[] _megaBadLines = new string[]
    {
        "Lemme be real honest with ya. This ain't the life for you. Some folk'll be too nice ta turn a profit. Some'll step on anyone for a copper...",
        "Somehow yer the worst o' both worlds. No profits and no reputation... It takes a certain skill, I'll grant ya.",
        "But it ain't one that'll get ya far in the trading business. Better find a new line o' work, 'cause I'll be back for my coin another day.",
    };

    string[] _badRepLines = new string[]
    {
        "Look, squirt. Ya got to have an eye on the big picture. Sure, yer rolling in cash <i>now</i>...",
        "But come tomorrow, there ain't a soul in town who'll have a thing to do with ya.",
        "Best ya find some work with money that don't involve dealin' with folk directly."
    };

    string[] _badDebtLines = new string[]
    {
        "Sorry, friend. Yer heart may be in the right place, ya just don't got the knack.",
        "Gettin' along with the customer is vital, sure. So's turnin' a profit.",
        "If ya can't do both, yer not fit for the life. Better luck with something else.",
    };

    string[] _goodEndLines = new string[]
    {
        "Hmmm. Yah. I figured I was right on the money about ya.",
        "This here'd be where I let ya keep all the profits and send ya out into the world.",
        "But we're in a bit o' a time <i>jam</i>, so it'll have to wait. Good show.",
    };

    public void StartJudgement() // UI Button
    {
        _compItems.SetActive(false);
        _playerTradeBox.SetActive(false);
        _scalesParent.SetActive(false);
        _activeButtonsParent.SetActive(false);
        
        _mentors[_mentorIndex].SetActive(true);
        _backgroundImage.SetActive(true);
        _continueButton.SetActive(true);
    }

    public void SetMentor(int mentorIndex)
    {
        _mentorIndex = mentorIndex;
    }

    public void GetVerdict() // UI Button
    {
        _finalSpeech.SetActive(true);
        _continueButton.SetActive(false);
        
        if(_player.Reputation < 0)
        {
            if(_player.Debt > _player.NetWorth)
            {
                MegaBadEnd();
                return;
            }
            BadReputationEnd();
            return;
        }
        if(_player.Debt > _player.NetWorth)
        {
            BadDebtEnd();
            return;
        }

        GoodEnd();
    }

    public void GoodEnd() // Also UI Button
    {
        if(_lineIndex >= 3)
        {
            _goodEndButton.SetActive(false);
            _happyEndingSplash.SetActive(true);
            return;
        }
        _goodEndButton.SetActive(true);
        _finalSpeechText.text = _goodEndLines[_lineIndex];
        _lineIndex++;
    }

    public void BadDebtEnd() // Also UI Button
    {
        if(_lineIndex >= 3)
        {
            _badDebtButton.SetActive(false);
            _gameOverSplash.SetActive(true);
            return;
        }
        _badDebtButton.SetActive(true);
        _finalSpeechText.text = _badDebtLines[_lineIndex];
        _lineIndex++;
    }

    public void BadReputationEnd() // Also UI Button
    {
        if(_lineIndex >= 3)
        {
            _badRepButton.SetActive(false);
            _gameOverSplash.SetActive(true);
            return;
        }
        _badRepButton.SetActive(true);
        _finalSpeechText.text = _badRepLines[_lineIndex];
        _lineIndex++;
    }

    public void MegaBadEnd() // Also UI Button
    {
        if(_lineIndex >= 3)
        {
            _megaBadButton.SetActive(false);
            _gameOverSplash.SetActive(true);
            return;
        }
        _megaBadButton.SetActive(true);
        _finalSpeechText.text = _megaBadLines[_lineIndex];
        _lineIndex++;
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
}
