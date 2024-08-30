using UnityEngine;
using TMPro;

public class Tutorial : MonoBehaviour
{
    [SerializeField] GameObject _openingSplash, _clickOverlay;
    [SerializeField] GameObject _speechWindow, _mentorSpeech, _speechWindow2, _scalesParent;
    [SerializeField] GameObject _playerStockBox, _playerCoinBox, _playerTradeBox;
    [SerializeField] GameObject _compStockBox, _compCoinBox, _compTradeBox;
    [SerializeField] GameObject _buttonsParent, _nextButton, _fakeHaggleButton, _haggleButton, _cancelButton, _closeSpeechButton, _goodbyeButton, _openButton, _bankButton, _resetButton;
    [SerializeField] TextMeshProUGUI _text, _mentorText, _button1Text, _button2Text, _button3Text, _speech2Text;
    [SerializeField] TextMeshProUGUI _greetingButtonText;
    [SerializeField] TradingSystem _tradingSystem;
    [SerializeField] TradingUI _tradingUI;
    [SerializeField] Customer[] _mentors;
    [SerializeField] GameObject _musicPlayer, _optionsButton;
    [SerializeField] Town _homeTown;
    [SerializeField] GameObject _tutorialUI, _skiptutorialButton, _acceptMarker, _rejectMarker, _ignoreWarningButton;
    Player _player;
    [SerializeField] GameObject[] _mentorSprites;
    [SerializeField] GameOver _gameOver;

    bool _acceptThePremise, _setGreeting, _buyingFinished, _strikesExplained, _sellingFinished, _barteredOnce, _tutorialComplete, _skipTutorial;
    bool[] _next = new bool[25];

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
        DropBox.OnTradeBoxProcessed += DropBox_OnTradeBoxProcessed;
        TradingSystem.OnFinishWithCustomer += TradingSystem_OnFinishWithCustomer;
    }

    void OnDisable()
    {
        DropBox.OnItemPicked -= DropBox_OnItemPicked;
        TradingSystem.OnOfferRejected -= TradingSystem_OnOfferRejected;
        TradingSystem.OnOfferAccepted -= TradingSystem_OnOfferAccepted;
        TradingSystem.OnIncorrectChange -= TradingSystem_OnIncorrectChange;
        DropBox.OnTradeBoxProcessed -= DropBox_OnTradeBoxProcessed;
        TradingSystem.OnFinishWithCustomer -= TradingSystem_OnFinishWithCustomer;
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
        _cancelButton.SetActive(false); // (Finish With Customer button)
        _scalesParent.SetActive(false);
        _haggleButton.SetActive(false);
        _openButton.SetActive(false);
        _bankButton.SetActive(false);
        _resetButton.SetActive(false);
        _ignoreWarningButton.SetActive(false);
        _skiptutorialButton.SetActive(true); // TODO (probably no time but IF there was a save system we could save whether the tutorial was done or not)
    }

    public void Button1()
    {
        if(_skipTutorial)
        {
            _greetingButtonText.text = _button1Text.text;
            SkipGreeting();
            return;
        }

        if(!_acceptThePremise)
        {
            _mentorSprites[0].SetActive(true);
            AcceptThePremise();
            _gameOver.SetMentor(0);
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
        if(_skipTutorial)
        {
            _greetingButtonText.text = _button2Text.text;
            SkipGreeting();
            return;
        }

        if(!_acceptThePremise)
        {
            _mentorSprites[1].SetActive(true);
            AcceptThePremise();
            _gameOver.SetMentor(1);
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
        if(_skipTutorial)
        {
            _greetingButtonText.text = _button3Text.text;
            SkipGreeting();
            return;
        }

        if(!_acceptThePremise)
        {
            _mentorSprites[2].SetActive(true);
            AcceptThePremise();
            _gameOver.SetMentor(2);
            return;
        }

        if(!_setGreeting)
        {
            _greetingButtonText.text = _button3Text.text;
            SetGreeting();
            return;
        }
    }

    public void SkipTutorial()
    {
        _skiptutorialButton.SetActive(false);
        _mentorSprites[0].SetActive(true);
        _gameOver.SetMentor(0);
        _skipTutorial = true;
        SkipThePremise();
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
        if(!_next[24])
        {
            _next[24] = true;
            Next24();
            return;
        }
    }

    void AcceptThePremise()
    {
        _skiptutorialButton.SetActive(false);
        _acceptThePremise = true;
        _buttonsParent.SetActive(false);
        _text.text = "Whatever your personal circumstances might be,\nyour first step should be to consult an expert.";
        _nextButton.SetActive(true);
    }

    void SkipThePremise()
    {
        _acceptThePremise = true;
        _setGreeting = true;
        _buttonsParent.SetActive(false);
        _text.text = "How will you greet your customers?";
        _button1Text.text = "\"What can I do for you?\"";
        _button2Text.text = "\"I have wares, if you have coin.\"";
        _button3Text.text = "\"Buy Something!\"";
        _buttonsParent.SetActive(true);
    }

    void SkipGreeting()
    {
        _speechWindow.SetActive(false);
        _buttonsParent.SetActive(false);
        _speechWindow.SetActive(false);
        _playerStockBox.SetActive(true);
        _playerCoinBox.SetActive(true);
        _player.EnableInventory();
        _compStockBox.SetActive(true);
        _compCoinBox.SetActive(true);
        _playerTradeBox.SetActive(true);
        _compTradeBox.SetActive(true);
        _scalesParent.SetActive(true);
        _ignoreWarningButton.SetActive(true);
        Invoke(nameof(DelayedSkip1), 0.1f);
        _strikesExplained = true;
        _buyingFinished = true;
        _sellingFinished = true;
        _barteredOnce = true;
    }

    void DelayedSkip1()
    {
        _tradingSystem.SetTutorialCustomer(_mentors[1]);
        _resetButton.SetActive(true);
        _mentors[1].ClearInventory();
        _mentors[1].GenerateCoinType(100, Currency.Copper, false); // 100 + 200 + 1000 + 3000
        _mentors[1].GenerateCoinType(20, Currency.Silver, false);
        _mentors[1].GenerateCoinType(10, Currency.Gold, false);
        _mentors[1].GenerateCoinType(4, Currency.Platinum, false);
        _mentors[1].CustomerInventory.CreateDebt();
        _player.SetDebt(8000); // TODO Hello magic number! This seems like a good goal (and it was!)

        Invoke(nameof(DelayedSkip2), 0.1f);
    }

    void DelayedSkip2()
    {
        _player.SetNetWorthText();
        _tradingSystem.SetTutorialCustomer(_mentors[2]);
        _openingSplash.SetActive(false);
        _cancelButton.SetActive(true);
    }

    void Next0()
    {
        _text.text = "There just so happens to be a veteran merchant passing through town...";
        if(_mentorSprites[1].activeSelf)
        {
            foreach(Customer mentor in _mentors)
            {
                mentor.SetName("Corthan");
            }
            _text.text = "You know about a semi-retired trader living in town...";
        }
        if(_mentorSprites[2].activeSelf)
        {
            foreach(Customer mentor in _mentors)
            {
                mentor.SetName("Tigey");
            }
            _tradingUI.SetCustomerName(_mentors[0].Name);
            _text.text = "You've heard tales of a white tiger who loves capitalism...";
        }
        _tradingUI.SetCustomerName(_mentors[0].Name);
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
        _mentorText.text = "Buy low.\nSell high.\nAnd...\nChange is necessary.";
    }

    void Next3()
    {
        _nextButton.SetActive(false);
        _mentorText.text = "But 'fore all that, how are ya gonna greet yer customers?";
        _button1Text.text = "\"What can I do for you?\"";
        _button2Text.text = "\"I have wares, if you have coin.\"";
        _button3Text.text = "\"Buy Something!\"";
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
        _mentorText.text = "This's where yer stock is displayed.";
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
        _player.SetNetWorthText();
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
        _mentorText.text = "Jus' to be real clear, I run an honest trade. These<i> <color=#00ECFF>spell scales</color> </i>show any item's true worth. Nothin's kept secret from customers.";
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
        if(_skipTutorial) { return; }

        if(playerProperty)
        {
            Invoke(nameof(BuyTutorial), 1.25f);
        }
        else
        {
            SellTutorial();
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
        _optionsButton.SetActive(false);
        _mentorSpeech.SetActive(false);
        _speechWindow2.SetActive(true);
        _speech2Text.text = "Click the arrows to increase/decrease your offer.\nYou can also use your scroll wheel.";
        _fakeHaggleButton.SetActive(false);
        _haggleButton.SetActive(true);
    }

    void TradingSystem_OnOfferRejected()
    {
        if(_skipTutorial) { return; }

        CloseSpeech();
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
        if(_skipTutorial) { return; }

        if(buying)
        {
            _speechWindow2.SetActive(false);
            _optionsButton.SetActive(true);
            _mentorText.text = "Not so hard is it? Folks know ya need to make a profit. They'll pay a fee to have <i>what</i> they want, <i>when</i> they want it.";
            _mentorSpeech.SetActive(true);
            _nextButton.SetActive(true);
            _mentors[0].SetTolerance(0);
        }
        else
        {
            _mentors[1].SetTolerance(0);
            _mentorText.text = "There's hope for ya yet. Pay the price the same way ya gave change.";
            _mentorSpeech.SetActive(true);
            _nextButton.SetActive(true);
        }
    }

    void Next11()
    {
        _mentorText.text = "Now the important part is rule three: 'Change is necessary.' It means different things to different types. For traders it goes like this...";
    }

    void Next12()
    {
        _mentorText.text = "'Ya can sell a twenty copper plean for three gold, long as it's agreed to. But shortchanging by a single coin makes ya a thief.'";
    }

    void Next13()
    {
        _mentorText.text = "Even if they don't notice in the moment, by the end of the day, when coins've been counted, the word'll spread.";
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
        if(_skipTutorial) { return; }

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
            _nextButton.SetActive(true); // Next17
            return;
        }
        if(!_sellingFinished)
        {
            _sellingFinished = true;
            SellingFinished();
            return;
        }
        if(!_barteredOnce)
        {
            _barteredOnce = true;
            BarteredOnce();
            return;
        }
    }

    public void CloseSpeech()
    {
        _mentorSpeech.SetActive(false);
        _speechWindow.SetActive(false);
        _speechWindow2.SetActive(false);
        _closeSpeechButton.SetActive(false);
        _optionsButton.SetActive(true);
    }

    void Next17()
    {
        _tradingSystem.FinishWithCustomer();
        _mentorSpeech.SetActive(false);
        _nextButton.SetActive(false);
    }

    void TradingSystem_OnFinishWithCustomer(Customer customer)
    {
        if(!_sellingFinished)
        {
            _mentors[0].ClearInventory();
            _tradingSystem.SetTutorialCustomer(_mentors[1]);
            return;
        }
        if(!_barteredOnce)
        {
            _mentors[1].ClearInventory();
            _tradingSystem.SetTutorialCustomer(_mentors[2]);
            return;
        }
        if(!_tutorialComplete)
        {
            _mentors[2].ClearInventory();
            _tutorialComplete = true;
            TutorialComplete();
        }
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
        //_mentorText.text = "Maybe yer cut out for this after all. Last thing to know is dealin' with other traders.";
        _mentorText.text = "Alright, ya convinced me. To get started yer gonna need more coin, it takes cash to make cash.";
        _mentorSpeech.SetActive(true);
        _nextButton.SetActive(true);
        _resetButton.SetActive(true);
        _mentors[1].ClearInventory();
        _mentors[1].GenerateCoinType(100, Currency.Copper, false); // 100 + 200 + 1000 + 3000
        _mentors[1].GenerateCoinType(20, Currency.Silver, false);
        _mentors[1].GenerateCoinType(10, Currency.Gold, false);
        _mentors[1].GenerateCoinType(4, Currency.Platinum, false);
    }

    void Next20()
    {
        // _mentorText.text = "Don't expect to make money off another merchant, but ya can restock by barterin' with 'em.";
        _mentorText.text = "This here is an investment that yer gonna pay back... With interest!";
        
        // int debt = _mentors[1].CustomerInventory.CoinBox.GetTrueValue();
        // int debtInterest = Mathf.CeilToInt(debt / 30f);
        // debt += debtInterest;
        _mentors[1].CustomerInventory.CreateDebt();
        // _player.SetDebt(debt);
        _player.SetDebt(8000); // TODO Hello magic number! This seems like a good goal for a player to reach (maybe too hard? unclear! no time to test!)
        // TODO Play cash sound!?
    }

    void Next21()
    {
        // _mentorText.text = "Yer free to pick as much of their stuff as ya want in a single trade, or offload things ya can't move.";
        _mentorText.text = "Barter for some of my stock first. I'll accept payment in cash <i>and</i> goods when I come collectin'.";
    }

    void Next22()
    {
        _tradingSystem.FinishWithCustomer();
        _nextButton.SetActive(false);
        _mentorSpeech.SetActive(false);
    }

    void BarteredOnce()
    {
        _ignoreWarningButton.SetActive(true);
        _cancelButton.SetActive(true);
        _mentorText.text = "Most folk'll let ya trade three times. Ya can always stop early, like if ya end up with too little cash.\n\nIf yer low on copper coins, head to the bank to trade for 'em.";
        _mentorSpeech.SetActive(true);
        _closeSpeechButton.SetActive(true);
    }

    void TutorialComplete()
    {
        _player.FinishTutorial();
        if(!_skipTutorial)
        {
            _mentorText.text = "That's everything ya need. The rest is in yer hands.";
            _mentorSpeech.SetActive(true);
            _nextButton.SetActive(true);
        }
        else
        {
            WrapUpTutorial();
        }
    }

    void Next23()
    {
        // _mentorText.text = "Get out there and make yer fortune.";
        _mentorText.text = "If ya really got what it takes, ya can pay off yer debt an' still make a profit.";
        _mentorSpeech.SetActive(true);
        _nextButton.SetActive(true);
    }

    void Next24()
    {
        WrapUpTutorial();
    }

    public void WrapUpTutorial()
    {
        _skiptutorialButton.SetActive(false);
        CloseSpeech();

        _ignoreWarningButton.SetActive(true);
        _acceptMarker.SetActive(false);
        _rejectMarker.SetActive(false);
        _tutorialUI.SetActive(false);
        _clickOverlay.SetActive(false);
        _musicPlayer.SetActive(false);
        _buttonsParent.SetActive(true);
        _compStockBox.SetActive(true);
        _compTradeBox.SetActive(true);
        _compCoinBox.SetActive(true);
        _playerStockBox.SetActive(true);
        _playerCoinBox.SetActive(true);
        _playerTradeBox.SetActive(true);
        _scalesParent.SetActive(true);
        _haggleButton.SetActive(false);
        _openButton.SetActive(true);
        _bankButton.SetActive(true);
        _resetButton.SetActive(true);
        _openingSplash.SetActive(false);
        _cancelButton.SetActive(true);
        _tradingSystem.SetIsBuyTutorial(false);
        _tradingUI.SetShowTradeNumbers(false);
        _player.EnableInventory();
        _homeTown.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
