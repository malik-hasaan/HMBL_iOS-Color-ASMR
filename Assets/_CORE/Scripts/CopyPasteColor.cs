using UnityEngine;

public class CopyPasteColor : MonoBehaviour
{
    public SpriteRenderer source;
    
    void OnEnable()
    {
        this.GetComponent<SpriteRenderer>().color = source.color;
    }
}
