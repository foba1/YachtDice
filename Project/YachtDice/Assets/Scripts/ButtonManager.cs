using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public GameObject Login;
    public GameObject Option;
    public GameObject Userinfo;
    public GameObject Dicepurchase;
    public Button[] Purchase;
    public Button[] Apply;
    public GameObject Rule;
    public GameObject Genealogy;
    public GameObject Finish;

    public System.Random rand;

    Sprite[] spr_0;
    Sprite[] spr_1;
    Sprite[] spr_2;
    Sprite[] spr_3;
    Sprite[] spr_4;

    int state = 0; // 0 : main, 1 : option, 2 : rule, 3 : genealogy, 4 : finish, 5 : userinfo, 6 : dicepurchase

    void Awake()
    {
        rand = new System.Random();

        spr_0 = Resources.LoadAll<Sprite>("Dice_0");
        spr_1 = Resources.LoadAll<Sprite>("Dice_1");
        spr_2 = Resources.LoadAll<Sprite>("Dice_2");
        spr_3 = Resources.LoadAll<Sprite>("Dice_3");
        spr_4 = Resources.LoadAll<Sprite>("Dice_4");
    }

    void Updatediceskin()
    {
        int skin = PlayerPrefs.GetInt("dice");
        if (skin == 0)
        {
            Userinfo.transform.GetChild(4).GetComponent<Image>().sprite = spr_0[rand.Next(0,6)];
        }
        else if (skin == 1)
        {
            Userinfo.transform.GetChild(4).GetComponent<Image>().sprite = spr_1[rand.Next(0, 6)];
        }
        else if (skin == 2)
        {
            Userinfo.transform.GetChild(4).GetComponent<Image>().sprite = spr_2[rand.Next(0, 6)];
        }
        else if (skin == 3)
        {
            Userinfo.transform.GetChild(4).GetComponent<Image>().sprite = spr_3[rand.Next(0, 6)];
        }
        else
        {
            Userinfo.transform.GetChild(4).GetComponent<Image>().sprite = spr_4[rand.Next(0, 6)];
        }
    }

    void Updatedicepurchase()
    {
        for (int i = 0; i < 5; i++)
        {
            if (i < 4)
            {
                if (PlayerPrefs.HasKey((i + 1).ToString()))
                {
                    if (Purchase[i].gameObject.activeInHierarchy)
                    {
                        Purchase[i].gameObject.SetActive(false);
                    }
                    if (!Apply[i + 1].gameObject.activeInHierarchy)
                    {
                        Apply[i + 1].gameObject.SetActive(true);
                    }
                }
                else
                {
                    if (!Purchase[i].gameObject.activeInHierarchy)
                    {
                        Purchase[i].gameObject.SetActive(true);
                    }
                    if (Apply[i + 1].gameObject.activeInHierarchy)
                    {
                        Apply[i + 1].gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                if (!Apply[0].gameObject.activeInHierarchy)
                {
                    Apply[0].gameObject.SetActive(true);
                }
            }
        }

        int skin = PlayerPrefs.GetInt("dice");
        for (int i = 0; i < 5; i++)
        {
            if (i == skin)
            {
                Apply[i].transform.GetChild(0).GetComponent<Text>().text = "적용중";
            }
            else
            {
                Apply[i].transform.GetChild(0).GetComponent<Text>().text = "적용";
            }
        }
    }

    public void purchase(int num)
    {
        if (num == 0)
        {
            if (!PlayerPrefs.HasKey("1"))
            {
                PlayerPrefs.SetInt("1", 1);
                PlayerPrefs.SetInt("dice", 1);
                PlayerPrefs.Save();
            }
        }
        else if (num == 1)
        {
            if (!PlayerPrefs.HasKey("2"))
            {
                PlayerPrefs.SetInt("2", 1);
                PlayerPrefs.SetInt("dice", 2);
                PlayerPrefs.Save();
            }
        }
        else if (num == 2)
        {
            if (!PlayerPrefs.HasKey("3"))
            {
                PlayerPrefs.SetInt("3", 1);
                PlayerPrefs.SetInt("dice", 3);
                PlayerPrefs.Save();
            }
        }
        else
        {
            if (!PlayerPrefs.HasKey("4"))
            {
                PlayerPrefs.SetInt("4", 1);
                PlayerPrefs.SetInt("dice", 4);
                PlayerPrefs.Save();
            }
        }
        Updatedicepurchase();
        Updatediceskin();
    }

    public void apply(int num)
    {
        if (num == 0)
        {
            if (PlayerPrefs.GetInt("dice") != 0)
            {
                PlayerPrefs.SetInt("dice", 0);
                PlayerPrefs.Save();
            }
        }
        else if (num == 1)
        {
            if (PlayerPrefs.GetInt("dice") != 1 && PlayerPrefs.HasKey("1"))
            {
                PlayerPrefs.SetInt("dice", 1);
                PlayerPrefs.Save();
            }
        }
        else if (num == 2)
        {
            if (PlayerPrefs.GetInt("dice") != 2 && PlayerPrefs.HasKey("2"))
            {
                PlayerPrefs.SetInt("dice", 2);
                PlayerPrefs.Save();
            }
        }
        else if (num == 3)
        {
            if (PlayerPrefs.GetInt("dice") != 3 && PlayerPrefs.HasKey("3"))
            {
                PlayerPrefs.SetInt("dice", 3);
                PlayerPrefs.Save();
            }
        }
        else
        {
            if (PlayerPrefs.GetInt("dice") != 4 && PlayerPrefs.HasKey("4"))
            {
                PlayerPrefs.SetInt("dice", 4);
                PlayerPrefs.Save();
            }
        }
        Updatedicepurchase();
        Updatediceskin();
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void option()
    {
        if (state == 0)
        {
            state = 1;
            Option.SetActive(true);
        }
    }

    public void userinfo()
    {
        if (state == 0)
        {
            state = 5;
            Userinfo.SetActive(true);
            Updatediceskin();
        }
    }

    public void dicepurchase()
    {
        if (state == 5)
        {
            state = 6;
            Dicepurchase.SetActive(true);
            Updatedicepurchase();
        }
    }

    public void rule()
    {
        if (state == 0)
        {
            state = 2;
            Rule.SetActive(true);
        }
    }

    public void genealogy()
    {
        if (state == 2)
        {
            state = 3;
            Genealogy.SetActive(true);
        }
    }

    public void backspace()
    {
        if (Login.activeInHierarchy)
        {
            if (state == 1)
            {
                state = 0;
                Option.SetActive(false);
            }
            else if (state == 2)
            {
                state = 0;
                Rule.SetActive(false);
            }
            else if (state == 3)
            {
                state = 2;
                Genealogy.SetActive(false);
            }
            else if (state == 4)
            {
                state = 0;
                Finish.SetActive(false);
            }
            else if (state == 5)
            {
                state = 0;
                Userinfo.SetActive(false);
            }
            else
            {
                state = 5;
                Dicepurchase.SetActive(false);
            }
        }
    }
    
    void Update()
    {
        if (Login.activeInHierarchy)
        {
            if (state == 0 && Input.GetKeyDown(KeyCode.Escape))
            {
                Finish.SetActive(true);
                state = 4;
            }
            else if (state != 0 && Input.GetKeyDown(KeyCode.Escape))
            {
                if (state == 1)
                {
                    Option.SetActive(false);
                    state = 0;
                }
                else if (state == 2)
                {
                    Rule.SetActive(false);
                    state = 0;
                }
                else if (state == 3)
                {
                    Genealogy.SetActive(false);
                    state = 2;
                }
                else if (state == 4)
                {
                    Finish.SetActive(false);
                    state = 0;
                }
                else if (state == 5)
                {
                    Userinfo.SetActive(false);
                    state = 0;
                }
                else
                {
                    Dicepurchase.SetActive(false);
                    state = 5;
                }
            }
        }
    }
}
