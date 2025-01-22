

using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class Settings : MonoBehaviour
{


   [SerializeField] public float mouseSens = 30;
    public float musicVolume;
    public bool invertAxis;
 
    public QMove player; 

    public UnityEngine.UI.Slider sensSlider;
    public UnityEngine.UI.Slider volumeSlider;
    public Image invertButton;

    private void Awake()
    {
        LoadSettings();
    }
    public void AdjustSens(float amount)
    {
        mouseSens = amount;
    }
    public void SaveSettings()
    {
        mouseSens = player.xMouseSensitivity;
        PlayerPrefs.SetFloat("mouseSens", mouseSens);

        //saving the invert bool
        invertAxis = player.invertLook;
        int axisInt;
        if(invertAxis  == true) { axisInt = 1; }
        else { axisInt = 0; }
        PlayerPrefs.SetFloat("invertAxis", axisInt);


    }
    public void LoadSettings()
    {
        mouseSens = PlayerPrefs.GetFloat("mouseSens", 2);
        invertAxis = (PlayerPrefs.GetInt("invertAxis", 0) != 0);
        
if (invertAxis == true) { invertButton.SetActive(true); }
       else if (invertAxis == false) { invertButton.SetActive(false); }


    }

}
