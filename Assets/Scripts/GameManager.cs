using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("-----Timer-----")]
    [SerializeField] bool timerOn;
    [SerializeField] float timeLeft;
    [SerializeField] TMP_Text timerText;

    public static GameManager Instance;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuActive;
    [SerializeField] AudioSource audioSource;
    [SerializeField] float songTimer;
    [SerializeField] float songLength;
    [SerializeField] public bool devMode = false;
    
    [SerializeField] TMP_Text goalCountText;
    public Image playerHealthBar;
    public Image playerHealthBarBack;
    public Image playerShieldBar;
    public Image playerShieldBarBack;
    public float lossSpeed;
    public float lerpTimer;

    public GameObject damagePanel;
    public GameObject interactButton;

    public GameObject player;
    public QMove playerScript;

    public bool isPaused = false;
    public int goalCount;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;

        timerOn = true;

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<QMove>();

        Time.timeScale = 1f;
        
    }

    // Update is called once per frame
    void Update()
    {
        TimerLogic();

        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                StatePause();
                menuActive = menuPause;
                menuActive.SetActive(isPaused);
            }
            else if(menuActive == menuPause)
            {
                StateUnpause();
            }
        }

        songTimer += Time.deltaTime;
        if (songTimer > songLength)
        {
            audioSource.Play();
            songTimer = 0;
        }
    }

    public void StatePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void StateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    public void UpdateGameGoal(int _amount)
    {
        goalCount += _amount;
        goalCountText.text = goalCount.ToString("F0");

        //YOU WIN!!!
        if (goalCount <= 0)
        {
            StatePause();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }
    }

    public void YouLose()
    {
        StatePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
        //THINK ABOUT CONDENSING THESE FUNCTIONS
    }

    public void TimerLogic()
    {
        if (timerOn)
        {
            if(timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
                UpdateTimer(timeLeft);
            }
            else
            {
                timeLeft = 0;
                timerOn = false;
                timerText.text = string.Format("{0:00} : {1:00}", 0f, 0f);
                YouLose();
            }
        }
    }

    public void UpdateTimer(float currentTime)
    {
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);
        float milliseconds = (Mathf.FloorToInt(currentTime * 1000f)) % 1000;

        timerText.text = string.Format("{0:00} : {1:00}",seconds, milliseconds);
    }
}

