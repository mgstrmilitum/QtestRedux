using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuActive;
    [SerializeField] AudioSource audioSource;
    [SerializeField] float songTimer;
    [SerializeField] float songLength;
    
    [SerializeField] TMP_Text goalCountText;
    public Image playerHealthBar;
    public Image playerShieldBar;

    public GameObject damagePanel;

    public GameObject player;
    public playerController playerScript;

    public bool isPaused = false;
    int goalCount;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();

        Time.timeScale = 1f;
        
    }

    // Update is called once per frame
    void Update()
    {
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
}
