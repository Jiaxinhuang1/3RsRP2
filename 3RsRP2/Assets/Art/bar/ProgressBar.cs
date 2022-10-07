using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class ProgressBar : MonoBehaviour
{
    public int maximum;
    public int current;
    public Image mask;

    // Start is called before the first frame update
    void Start()
    {
        maximum = GameManager.instance.maxExperience;
    }

    // Update is called once per frame
    void Update()
    {
        if (current > maximum)
        {
            current = maximum;
        }
        else if (current < 0)
        {
            current = 0;
        }
        GetCurrentFill();
    }
    void GetCurrentFill()
    {
        float fillAmount = (float)current / (float)maximum;
        mask.fillAmount = fillAmount; 
    }
}
