using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Settings : MonoBehaviour
{
    [SerializeField] public float MouseSens = 30;
    public UnityEngine.UI.Slider sensSlider;


   
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

   
    void Update()
    {
        
    }
    public void AdjustSens(float amount)
    {
        MouseSens = amount;
    }
    public void UpdateSettings()
    {

    }
}
