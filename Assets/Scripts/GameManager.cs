using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject InFightObject;
    public GameObject RoundObject;
    public TMP_Text Round;
    public GameObject Fight;
    public GameObject UnmaskHim;
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
    public bool IsFinished;

    //player HP
    public int MaxPlayerHP;
    public int Player1HP;
    public int Player1RoundsWon;
    public int Player2HP;
    public int Player2RoundsWon;

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
        IsInFight = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsInFight)
        {
            //TEST
            if (InputController.GetInput(InputPurpose.P1_PUNCH_1))
            {
                Player2HP--;
            }
            if (InputController.GetInput(InputPurpose.P2_PUNCH_1))
            {
                Player1HP--;
            }

            //set HP width
            var player1HPwidth = PlayerHPWidths[Player1HP];
            Player1HPObjectHP.GetComponent<RectTransform>().sizeDelta = new Vector2(player1HPwidth, 15);
            var player2HPwidth = PlayerHPWidths[Player2HP];
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

        if (IsFinished)
        {
            if (InputController.GetInput(InputPurpose.START_GAME))
            {
                SceneManager.LoadScene(0);
            }
        }
    }

    public void StartGame()
    {
        InFightObject.SetActive(true);
        GameStarted = true;
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

        if(Player1RoundsWon == MaxRound/2 + 1)
        {
            EndGame();
            return;
        }
        else if(Player2RoundsWon == MaxRound/2 + 1)
        {
            EndGame();
            return;
        }
        else if(CurrentRound == MaxRound)
        {
            EndGame();
            return;
        }

        NextRound();
    }

    public void EndGame()
    {
        StartCoroutine(EndGameWait());
    }

    IEnumerator EndGameWait()
    {
        yield return new WaitForSeconds(FightWaitSeconds);
        if(Player1RoundsWon > Player2RoundsWon)
        {
            Player1Wins.gameObject.SetActive(true);
        }
        else
        {
            Player2Wins.gameObject.SetActive(true);
        }

        AudioManager.Instance.PlaySFXClip(AudioLabel.VictoryVoice);
        Restart.SetActive(true);
        IsFinished = true;
    }
}
