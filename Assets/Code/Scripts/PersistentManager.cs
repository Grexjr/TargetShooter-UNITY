using UnityEngine;
using UnityEngine.UI;

public class PersistentManager : MonoBehaviour
{

    public static float Sensitivity = 1.0f;

    public Slider sensitivitySlider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Sensitivity = PlayerPrefs.GetFloat("MouseSensitivity",1.0f);

        if(sensitivitySlider != null)
        {
            sensitivitySlider.value = Sensitivity;
            sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
        }
    }

    void SetSensitivity(float value)
    {
        Sensitivity = value;
        PlayerPrefs.SetFloat("MouseSensitivity",value);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
