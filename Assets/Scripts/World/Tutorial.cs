using UnityEngine;
using TMPro;

public class Tutorial : MonoBehaviour
{
    [SerializeField] GameObject _openingSplash, _clickOverlay;
    [SerializeField] GameObject _speechWindow, _mentorSpeech, _speechWindow2, _scalesParent;
    [SerializeField] GameObject _playerStockBox, _playerCoinBox, _playerTradeBox;
    [SerializeField] GameObject _compStockBox, _compCoinBox, _compTradeBox;
    [SerializeField] GameObject _buttonsParent, _nextButton, _fakeHaggleButton, _haggleButton, _cancelButton, _closeSpeechButton, _goodbyeButton, _openButton, _bankButton;
    [SerializeField] TextMeshProUGUI _text, _mentorText, _button1Text, _button2Text, _button3Text, _speech2Text;
    [SerializeField] TextMeshProUGUI _greetingButtonText;
    [SerializeField] TradingSystem _tradingSystem;
    [SerializeField] Customer[] _mentors;
    [SerializeField] GameObject _musicPlayer;
    
    Player _player;

    bool _acceptThePremise, _setGreeting, _buyingFinished, _strikesExplained, _sellingFinished;
    bool[] _next = new bool[39];

    void Awake()
    {
        _player = FindAnyObjectByType<Player>();
    }

    void OnEnable()
    {
        DropBox.OnItemPicked += DropBox_OnItemPicked;
        TradingSystem.OnOfferRejected += TradingSystem_OnOfferRejected;
        TradingSystem.OnOfferAccepted += TradingSystem_OnOfferAccepted;
        TradingSystem.OnIncorrectChange += TradingSystem_OnIncorrectChange;
        // TradingSystem.OnTradeCompleted += TradingSystem_OnTradeCompleted;
        DropBox.OnTradeBoxProcessed += DropBox_OnTradeBoxProcessed;
    }

    void OnDisable()
    {
        DropBox.OnItemPicked -= DropBox_OnItemPicked;
        TradingSystem.OnOfferRejected -= TradingSystem_OnOfferRejected;
        TradingSystem.OnOfferAccepted -= TradingSystem_OnOfferAccepted;
        TradingSystem.OnIncorrectChange -= TradingSystem_OnIncorrectChange;
        // TradingSystem.OnTradeCompleted -= TradingSystem_OnTradeCompleted;
        DropBox.OnTradeBoxProcessed -= DropBox_OnTradeBoxProcessed;
    }

    public void ClickToBegin() // UI Button
    {
        _clickOverlay.SetActive(false);
        _musicPlayer.SetActive(true);
        _buttonsParent.SetActive(true);
        _compStockBox.SetActive(false);
        _compTradeBox.SetActive(false);
        _compCoinBox.SetActive(false);
        _playerStockBox.SetActive(false);
        _playerCoinBox.SetActive(false);
        _playerTradeBox.SetActive(false);
        _cancelButton.SetActive(false);
        _scalesParent.SetActive(false);
        _haggleButton.SetActive(false);
        _openButton.SetActive(false);
        _bankButton.SetActive(false);
    }

    public void Button1()
    {
        if(!_acceptThePremise)
        {
            AcceptThePremise();
            return;
        }

        if(!_setGreeting)
        {
            _greetingButtonText.text = _button1Text.text;
            SetGreeting();
            return;
        }
    }

    public void Button2()
    {
        if(!_acceptThePremise)
        {
            AcceptThePremise();
            return;
        }

        if(!_setGreeting)
        {
            _greetingButtonText.text = _button2Text.text;
            SetGreeting();
            return;
        }
    }

    public void Button3()
    {
        if(!_acceptThePremise)
        {
            AcceptThePremise();
            return;
        }

        if(!_setGreeting)
        {
            _greetingButtonText.text = _button3Text.text;
            SetGreeting();
            return;
        }
    }

    public void NextButton()
    {
        if(!_next[0])
        {
            _next[0] = true;
            Next0();
            return;
        }
        if(!_next[1])
        {
            _next[1] = true;
            Next1();
            return;
        }
        if(!_next[2])
        {
            _next[2] = true;
            Next2();
            return;
        }
        if(!_next[3])
        {
            _next[3] = true;
            Next3();
            return;
        }
        if(!_next[4])
        {
            _next[4] = true;
            Next4();
            return;
        }
        if(!_next[5])
        {
            _next[5] = true;
            Next5();
            return;
        }
        if(!_next[6])
        {
            _next[6] = true;
            Next6();
            return;
        }
        if(!_next[7])
        {
            _next[7] = true;
            Next7();
            return;
        }
        if(!_next[8])
        {
            _next[8] = true;
            Next8();
            return;
        }
        if(!_next[9])
        {
            _next[9] = true;
            Next9();
            return;
        }
        if(!_next[10])
        {
            _next[10] = true;
            Next10();
            return;
        }
        if(!_next[11])
        {
            _next[11] = true;
            Next11();
            return;
        }
        if(!_next[12])
        {
            _next[12] = true;
            Next12();
            return;
        }
        if(!_next[13])
        {
            _next[13] = true;
            Next13();
            return;
        }
        if(!_next[14])
        {
            _next[14] = true;
            Next14();
            return;
        }
        if(!_next[15])
        {
            _next[15] = true;
            Next15();
            return;
        }
        if(!_next[16])
        {
            _next[16] = true;
            Next16();
            return;
        }
        if(!_next[17])
        {
            _next[17] = true;
            Next17();
            return;
        }
        if(!_next[18])
        {
            _next[18] = true;
            Next18();
            return;
        }
        if(!_next[19])
        {
            _next[19] = true;
            Next19();
            return;
        }
        if(!_next[20])
        {
            _next[20] = true;
            Next20();
            return;
        }
        if(!_next[21])
        {
            _next[21] = true;
            Next21();
            return;
        }
        if(!_next[22])
        {
            _next[22] = true;
            Next22();
            return;
        }
        if(!_next[23])
        {
            _next[23] = true;
            Next23();
            return;
        }
    }

    void AcceptThePremise()
    {
        _acceptThePremise = true;
        _buttonsParent.SetActive(false);
        _text.text = "Whatever your personal circumstances might be,\nyour first step should be to consult an expert.";
        _nextButton.SetActive(true);
    }

    void Next0()
    {
        _text.text = "There just so happens to be a semi-retired merchant in town...";
    }

    void Next1()
    {
        _openingSplash.SetActive(false);
        _speechWindow.SetActive(false);
        _nextButton.SetActive(false);
        Invoke(nameof(IntroduceMentor), 1.5f);
    }

    void IntroduceMentor()
    {
        _mentorText.text = "Hmph, I guess I can teach ya the basics.\nThere's three rules e'ry trader needs to follow:";
        _mentorSpeech.SetActive(true);
        _nextButton.SetActive(true);
    }

    void Next2()
    {
        _mentorText.text = "One, buy low.\nTwo, sell high.\nAnd three...\nChange is necessary.";
    }

    void Next3()
    {
        _nextButton.SetActive(false);
        _mentorText.text = "But 'fore all that, how are ya gonna greet yer customers?";
        _button1Text.text = "What can I do for you?";
        _button2Text.text = "I have wares, if you have coin.";
        _button3Text.text = "Buy something!";
        _buttonsParent.SetActive(true);
    }

    void SetGreeting()
    {
        _buttonsParent.SetActive(false);
        _setGreeting = true;
        _mentorText.text = "Sure, if ya think that'll work.\nLet's start with the most important thing, sellin' junk\nfor more than it's worth.";
        _nextButton.SetActive(true);
    }

    void Next4()
    {
        _playerStockBox.SetActive(true);
        _playerCoinBox.SetActive(true);
        _mentorText.text = "This here's where all yer stock is\nput out on display.";
        _player.EnableInventory();
    }

    void Next5()
    {
        _compStockBox.SetActive(true);
        _compCoinBox.SetActive(true);
        _mentorText.text = "Customer's goods go here,\nif they've got any.\n\nTheir cash ain't seen.";
    }

    void Next6()
    {
        _playerTradeBox.SetActive(true);
        _compTradeBox.SetActive(true);
        _mentorSpeech.SetActive(false);
        _mentorText.text = "When yer makin' deals the stuff up for grabs goes here.\nWhoever's payin' will put their copper on the opposite side.";
        _nextButton.SetActive(false);
        Invoke(nameof(Delayed6), 1f);
    }

    void Delayed6()
    {
        _mentorSpeech.SetActive(true);
        _nextButton.SetActive(true);
    }

    void Next7()
    {
        _scalesParent.SetActive(true);
        _mentorText.text = "Jus' to be real clear, I run an honest trade. These spell scales show any item's true worth. Nothin's kept secret from customers.";
    }

    void Next8()
    {
        _mentorText.text = "Let's practice with ya sellin' me that sword. Say yer greetin' and we'll go from there.";
    }

    void Next9()
    {
        _mentorSpeech.SetActive(false);
        _nextButton.SetActive(false);
        _tradingSystem.SetTutorialCustomer(_mentors[0]);
    }

    void DropBox_OnItemPicked(Item item, bool playerProperty)
    {
        if(playerProperty)
        {
            Invoke(nameof(BuyTutorial), 1.25f);
        }
        else
        {
            Invoke(nameof(SellTutorial), 1.25f);
        }
    }

    void BuyTutorial()
    {
        _mentorText.text = "Since yer learnin' we'll show the numbers. Normally that only happens when two traders barter.";
        _mentorSpeech.SetActive(true);
        _nextButton.SetActive(true);
    }

    void Next10()
    {
        _mentorText.text = "After a customer picks what they want ya need to set a price they'll agree to. Remember they can see the scales, so be reasonable.";
        _nextButton.SetActive(false);
        _fakeHaggleButton.SetActive(true);
    }

    public void FakeHaggleButton()
    {
        _mentorSpeech.SetActive(false);
        _speechWindow2.SetActive(true);
        _speech2Text.text = "Click the arrows to increase or decrease your offer. You can also use the scroll wheel.";
        _fakeHaggleButton.SetActive(false);
        _haggleButton.SetActive(true);
        _closeSpeechButton.SetActive(true);
    }

    void TradingSystem_OnOfferRejected()
    {
        if(!_strikesExplained)
        {
            ExplainStrikes();
        }
    }

    void ExplainStrikes()
    {
        _mentorText.text = "Customers don't have a lot of patience round here. Ya get a couple tries to figure their limits before they walk.";
        _mentorSpeech.SetActive(true);
        _closeSpeechButton.SetActive(true);
        _strikesExplained = true;
    }
    
    void TradingSystem_OnOfferAccepted(bool buying, int offer)
    {
        if(buying)
        {
            _mentorText.text = "Not so hard is it? Folks know ya need to make a profit. They'll pay a fee to have what they want, when they want it.";
            _mentorSpeech.SetActive(true);
            _nextButton.SetActive(true);
            _mentors[0].SetTolerance(0);
        }
        else
        {
            _mentorText.text = "There's hope for ya yet. Pay the price the same way ya gave change.";
            _mentorSpeech.SetActive(true);
            _nextButton.SetActive(true);
            _mentors[0].SetTolerance(0);
        }
    }

    void Next11()
    {
        _mentorText.text = "Now the important part is rule three. Change is necessary. Means different things to different types. For traders it goes like this:";
    }

    void Next12()
    {
        _mentorText.text = "Ya can sell a twenty copper plean for three gold, long as it's agreed to. But shortchanging by a single coin makes ya a thief.";
    }

    void Next13()
    {
        _mentorText.text = "Even if they don't notice in the moment, by the end of the day, when coin's been counted, the word'll spread.";
    }

    void Next14()
    {
        _mentorSpeech.SetActive(false);
        _text.text = "Left click on a coin to add it as change. You can also hold the button to drag it.";
        _speechWindow.SetActive(true);
    }

    void Next15()
    {
        _text.text = "Use the scroll wheel to add and remove coins quickly. If that's still not fast enough, right click to add ten at a time.";
    }

    void Next16()
    {
        _speechWindow.SetActive(false);
        _nextButton.SetActive(false);
    }

    void TradingSystem_OnIncorrectChange()
    {
        _mentorText.text = "Bad at math? I know ya can't be doing this on purpose";
        _mentorSpeech.SetActive(true);
        _closeSpeechButton.SetActive(true);
    }

    void DropBox_OnTradeBoxProcessed()
    {
        if(!_buyingFinished)
        {
            _buyingFinished = true;
            _mentorText.text = "Jus' so ya know, givin' too much change ain't great, but loads better than too little. Let's see ya handle buyin' next.";
            _mentorSpeech.SetActive(true);
            _nextButton.SetActive(true);
            return;
        }
        if(!_sellingFinished)
        {
            _sellingFinished = true;
            SellingFinished();
        }
    }

    public void CloseSpeech()
    {
        _mentorSpeech.SetActive(false);
        _speechWindow.SetActive(false);
        _speechWindow2.SetActive(false);
        _closeSpeechButton.SetActive(false);
    }

    void Next17()
    {
        _mentorSpeech.SetActive(false);
        _nextButton.SetActive(false);
        _tradingSystem.SetTutorialCustomer(_mentors[1]);
    }

    void SellTutorial()
    {
        _mentorText.text = "Like ya can figure, buy offers gotta be lower than the real price to make a profit.";
        _mentorSpeech.SetActive(true);
        _nextButton.SetActive(true);
    }

    void Next18()
    {
        _mentorSpeech.SetActive(false);
        _nextButton.SetActive(false);
        _haggleButton.SetActive(true);
    }

    void Next19()
    {
        _mentorSpeech.SetActive(false);
        _nextButton.SetActive(false);
    }

    void SellingFinished()
    {
        _mentorText.text = "Maybe yer cut out for this after all. Last thing to know is dealing with other traders.";
        _mentorSpeech.SetActive(true);
        _nextButton.SetActive(true);
    }

    void Next20()
    {
        _mentorText.text = "Don't expect to make money off another merchant, but ya can restock by barterin' with 'em.";
    }

    void Next21()
    {
        _mentorText.text = "Yer free to pick as much of their stuff as ya want in a single trade, or offload things ya can't move.";
    }

    void Next22()
    {
        _mentorSpeech.SetActive(false);
        _tradingSystem.SetTutorialCustomer(_mentors[2]);
    }

    void Next23()
    {
        _cancelButton.SetActive(true);
        _mentorSpeech.SetActive(true);
        _mentorText.text = "If ya ever find yerself in a pickle ya can always choose to end an offer yerself.";
    }
}
