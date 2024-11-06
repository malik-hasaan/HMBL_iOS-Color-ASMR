using UnityEngine;
using UnityEngine.UI;

public class PensShopScript : MonoBehaviour
{
    int levelNum;

    public GameObject b1, b2, b3, b4,b5, b6, b7, b8, b9, b10;
    public GameObject ULb1, ULb2, ULb3, ULb4, ULb5, ULb6, ULb7, ULb8, ULb9, ULb10;

    public Image[] BtnImg;
    public Image[] BtnTickImg;
    public Sprite SelectSprite, UnSelectSprite;

    void Start()
    {
        levelNum = SaveSystem.Instance.DataFields.levelNumber;// PlayerPrefs.GetInt("LEVELNUMBER");
        CheckForUnlockPen(levelNum);

        int valNew =  PlayerPrefs.GetInt("PenImage",0);

        SelectPenBtn(valNew);
    }

    void CheckForUnlockPen(int lvl)
    {
        if (lvl > 9)
        {
            b1.SetActive(false);
            ULb1.SetActive(true);
        }

        if (lvl > 12)
        {
            b2.SetActive(false);
            ULb2.SetActive(true);
        }

        if (lvl > 16)
        {
            b3.SetActive(false);
            ULb3.SetActive(true);
        }

        if (lvl > 20)
        {
            b4.SetActive(false);
            ULb4.SetActive(true);
        }

        if (lvl > 25)
        {
            b5.SetActive(false);
            ULb5.SetActive(true);
        }

        if (lvl > 31)
        {
            b6.SetActive(false);
            ULb6.SetActive(true);
        }

        if (lvl > 45)
        {
            b7.SetActive(false);
            ULb7.SetActive(true);
        }

        if (lvl > 50)
        {
            b8.SetActive(false);
            ULb8.SetActive(true);
        }

        if (lvl > 54)
        {
            b9.SetActive(false);
            ULb9.SetActive(true);
        }

        if (lvl > 70)
        {
            b10.SetActive(false);
            ULb10.SetActive(true);
        }
    }

    public void SelectPenBtn(int n)
    {
        AudioManagerButtons._inst.PlayMusicTap();

        for (int i = 0; i < BtnImg.Length; i++)
        {
            BtnImg[i].sprite = UnSelectSprite;
            BtnTickImg[i].gameObject.SetActive(false);
        }

        BtnImg[n].sprite = SelectSprite;
        BtnTickImg[n].gameObject.SetActive(true);

        PlayerPrefs.SetInt("PenImage", n);

        GameManager.Instance.ChangePenFunction();

        GameManager.Instance.VibrateIt();
    }
}
