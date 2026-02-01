using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject InFightObject;
    public GameObject RoundObject;
    public TMP_Text Round;
    public GameObject Fight;
    public GameObject UnmaskHim;
    public GameObject Maskality;
    public TMP_Text Timer;
    private float TimerTicker;

    //Players
    public GameObject Player1HPObject;
    public GameObject Player1HPObjectHP;
    public GameObject Player2HPObject;
    public GameObject Player2HPObjectHP;
    public List<int> PlayerHPWidths;
    public GameObject Player1Wins;
    public GameObject Player2Wins;
    public List<GameObject> Player1WinObjects;
    public List<GameObject> Player2WinObjects;

    public GameObject Restart;

    //Rounds
    public int CurrentRound;
    public int MaxRound;
    public float NextRoundWaitSeconds;
    public float FightWaitSeconds;

    public bool GameStarted;
    public bool IsInFight;
    public bool IsInMaskality;
    public bool IsFinished;

    //player HP
    public int MaxPlayerHP;
    public int Player1HP;
    public int Player1RoundsWon;
    public int Player2HP;
    public int Player2RoundsWon;
    public Sprite Character1Sprite;
    public Sprite Character2Sprite;
    public Image Player1Character;
    public Image Player2Character;

    //Singleton pattern
    public static GameManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }
        Destroy(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        InFightObject.SetActive(false);
        RoundObject.SetActive(false);
        Fight.SetActive(false);
        UnmaskHim.SetActive(false);
        Maskality.SetActive(false);
        Timer.gameObject.SetActive(true);
        Player1HPObject.SetActive(true);
        Player2HPObject.SetActive(true);
        Restart.SetActive(false);
        Player1Wins.SetActive(false);
        Player2Wins.SetActive(false);
        foreach(var go in Player1WinObjects)
        {
            go.SetActive(false);
        }
        foreach(var go in Player2WinObjects)
        {
            go.SetActive(false);
        }

        CurrentRound = 0;
        GameStarted = false;
        IsInFight = false;
        IsInMaskality = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsInFight)
        {
            UpdatePlayerHPs();
        }

        //test
        if (IsInMaskality && InputController.GetInput(InputPurpose.START_GAME))
        {
            EndGame();
        }

        if (IsFinished && InputController.GetInput(InputPurpose.START_GAME))
        {
            SceneManager.LoadScene(0);
        }
    }

    public void UpdatePlayerHPs()
    {
        //TEST
        if (InputController.GetInput(InputPurpose.P1_PUNCH_1))
        {
            Player2HP -= 10;
        }
        if (InputController.GetInput(InputPurpose.P2_PUNCH_1))
        {
            Player1HP -= 10;
        }

        //set HP width
        var player1HPwidth = PlayerHPWidths[Player1HP/10];
        Player1HPObjectHP.GetComponent<RectTransform>().sizeDelta = new Vector2(player1HPwidth, 15);
        var player2HPwidth = PlayerHPWidths[Player2HP/10];
        Player2HPObjectHP.GetComponent<RectTransform>().sizeDelta = new Vector2(player2HPwidth, 15);

        //Round ends by timer
        TimerTicker -= Time.deltaTime;
        Timer.text = Mathf.RoundToInt(TimerTicker).ToString();
        if(TimerTicker <= 0)
        {
            EndRound();
        }

        //Round ends by no HP
        if(Player1HP == 0 || Player2HP == 0)
        {
            EndRound();
        }
    }

    public void StartGame(bool Player1Character1, bool Player2Character1)
    {
        InFightObject.SetActive(true);
        GameStarted = true;

        Player1Character.sprite = Character2Sprite;
        Player2Character.sprite = Character2Sprite;
        if (Player1Character1)
        {
            Player1Character.sprite = Character1Sprite;
        }
        if (Player2Character1)
        {
            Player2Character.sprite = Character1Sprite;
        }

        NextRound();
    }

    public void NextRound()
    {
        RoundObject.SetActive(true);
        CurrentRound++;
        Round.text = $"ROUND {CurrentRound}";
        TimerTicker = 99;
        Player1HP = MaxPlayerHP;
        Player2HP = MaxPlayerHP;
        AudioManager.Instance.PlaySFXClip(AudioLabel.BellSFX);
        StartCoroutine(NextRoundWait());
    }

    IEnumerator NextRoundWait()
    {
        yield return new WaitForSeconds(NextRoundWaitSeconds);
        RoundObject.SetActive(false);
        Fight.SetActive(true);
        AudioManager.Instance.PlaySFXClip(AudioLabel.FightSFX);
        StartCoroutine(FightWait());
    }

    IEnumerator FightWait()
    {
        yield return new WaitForSeconds(FightWaitSeconds);
        Fight.SetActive(false);
        IsInFight = true;
    }

    public void EndRound()
    {
        if(Player1HP > Player2HP)
        {
            Player1WinObjects[Player1RoundsWon].SetActive(true);
            Player1RoundsWon++;
        }
        else
        {
            Player2WinObjects[Player2RoundsWon].SetActive(true);
            Player2RoundsWon++;
        }

        IsInFight = false;
        AudioManager.Instance.PlaySFXClip(AudioLabel.BellSFX);

        //A player has won
        if(Player1RoundsWon == MaxRound/2 + 1)
        {
            EndGameMaskality();
            return;
        }
        else if(Player2RoundsWon == MaxRound/2 + 1)
        {
            EndGameMaskality();
            return;
        }
        else if(CurrentRound == MaxRound)
        {
            EndGameMaskality();
            return;
        }

        NextRound();
    }

    public void EndGameMaskality()
    {
        IsInMaskality = true;
        UnmaskHim.SetActive(true);
        AudioManager.Instance.PlaySFXClip(AudioLabel.UnmaskHimSFX);
        //opponent freezes
        //winner punches/unmasks opponent one more time -> EndGame
    }

    public void EndGame()
    {
        UnmaskHim.SetActive(false);
        Maskality.SetActive(true);
        AudioManager.Instance.PlaySFXClip(AudioLabel.MaskalitySFX);
        StartCoroutine(EndGameWait());
    }

    IEnumerator EndGameWait()
    {
        yield return new WaitForSeconds(NextRoundWaitSeconds);
        if(Player1RoundsWon > Player2RoundsWon)
        {
            Player1Wins.gameObject.SetActive(true);
        }
        else
        {
            Player2Wins.gameObject.SetActive(true);
        }

        AudioManager.Instance.PlaySFXClip(AudioLabel.VictoryVoice);
        AudioManager.Instance.PlaySFXClip(AudioLabel.ApplauseSFX);
        AudioManager.Instance.PlaySFXClip(AudioLabel.ApplauseSFX);
        Maskality.SetActive(false);
        Restart.SetActive(true);
        IsInMaskality = false;
        IsFinished = true;
    }
}
