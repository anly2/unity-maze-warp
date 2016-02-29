using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LabelledValue : MonoBehaviour {
    public string label;

    private Text text;

    void Awake()
    {
        text = GetComponent<Text>();

        if (label == null || label.Length == 0)
            label = text.text;
    }


    public void Set(object value)
    {
        Set(value.ToString());
    }

    public void Set(string value)
    {
        text.text = label + value;
    }
}
