using UnityEngine;
using DG.Tweening;

public class FadeInSpriteRenderer : MonoBehaviour
{
    public SpriteRenderer SR;

    void Start()
    {
        SR.gameObject.SetActive(true);

        SR.DOFade(0, .001f).SetEase(Ease.Linear).OnComplete(() => 
        {
            SR.DOFade(1, 1f).SetEase(Ease.Linear);
        });
    }
}
