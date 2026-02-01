using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    //todo:
    //insert coin "animation" -> any key
    //PlayerPickCharacter
    //Space to start -> Rounds, Fight (RoundsManager)

    public GameObject MenuObject;
    public bool IsInInsertCoin;
    public GameObject InsertCoin;
    public bool IsInCharacterPicker;
    public GameObject CharacterPicker;
    public GameObject Player1Picker;
    public GameObject Player2Picker;
    public bool IsInGame;
    public GameObject SpaceToStart;

    public float CharacterPickerWaitSeconds;
    public Vector3 Character1Position;
    public Vector3 Character2Position;
    public bool Player1Character1;
    public bool Player2Character1;

    //Singleton pattern
    public static MenuManager Instance;
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
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);

        //cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        InputController.InputEnabled = true;

        MenuObject.SetActive(true);
        InsertCoin.SetActive(true);
        CharacterPicker.SetActive(false);
        Player1Picker.SetActive(false);
        Player2Picker.SetActive(false);
        SpaceToStart.SetActive(false);

        IsInInsertCoin = true;
        IsInCharacterPicker = false;
        IsInGame = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (InputController.GetInput(InputPurpose.QUIT))
        {
            Quit();
        }

        if (InputController.GetInput(InputPurpose.ANY) && IsInInsertCoin)
        {
            IsInInsertCoin = false;
            InsertCoin.SetActive(false);

            CharacterPicker.SetActive(true);
            AudioManager.Instance.PlaySFXClip(AudioLabel.PickCharacterVoice);
            StartCoroutine(CharacterPickerWait());
            return;
        }

        if (IsInCharacterPicker)
        {
            if (InputController.GetInput(InputPurpose.P1_MOVE_RIGHT) && Player1Character1)
            {
                Player1Character1 = false;
                Player1Picker.transform.localPosition = Character2Position;
                AudioManager.Instance.PlaySFXClip(AudioLabel.SwapMenuSFX);
            }
            else if (InputController.GetInput(InputPurpose.P1_MOVE_LEFT) && !Player1Character1)
            {
                Player1Character1 = true;
                Player1Picker.transform.localPosition = Character1Position;
                AudioManager.Instance.PlaySFXClip(AudioLabel.SwapMenuSFX);
            }

            if (InputController.GetInput(InputPurpose.P2_MOVE_RIGHT) && Player2Character1)
            {
                Player2Character1 = false;
                Player2Picker.transform.localPosition = Character2Position;
                AudioManager.Instance.PlaySFXClip(AudioLabel.SwapMenuSFX);
            }
            else if (InputController.GetInput(InputPurpose.P2_MOVE_LEFT) && !Player2Character1)
            {
                Player2Character1 = true;
                Player2Picker.transform.localPosition = Character1Position;
                AudioManager.Instance.PlaySFXClip(AudioLabel.SwapMenuSFX);
            }

            if (InputController.GetInput(InputPurpose.START_GAME))
            {
                IsInCharacterPicker = false;
                CharacterPicker.SetActive(false);
                SpaceToStart.SetActive(false);
                MenuObject.SetActive(false);

                IsInGame = true;
                GameManager.Instance.StartGame();
            }
        }
    }

    IEnumerator CharacterPickerWait()
    {
        yield return new WaitForSeconds(CharacterPickerWaitSeconds);
        IsInCharacterPicker = true;
        Player1Character1 = true;
        Player1Picker.transform.localPosition = Character1Position;
        Player2Character1 = false;
        Player2Picker.transform.localPosition = Character2Position;
        SpaceToStart.SetActive(true);
        Player1Picker.SetActive(true);
        Player2Picker.SetActive(true);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
