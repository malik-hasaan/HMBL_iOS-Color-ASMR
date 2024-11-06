using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ColorInfo : MonoBehaviour
{ 
    public Color thisColor;

    Button btn;

    void Start()
    {
        btn = GetComponent<Button>();
    }

    public void SetColor(Color colorTemp)
    {
        btn.image.color = colorTemp;
        thisColor = colorTemp;
    }

}

