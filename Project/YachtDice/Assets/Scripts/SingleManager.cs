using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class SingleManager : MonoBehaviourPunCallbacks
{
    int state = 0, round = 1, roll = 0, choice = 0, total = 0;
    int[] temp, score, keep, dice;
    int seed = 0;
    float rolltime = 0f;

    Sprite[] spr_0;
    Sprite[] spr_1;
    Sprite[] spr_2;
    Sprite[] spr_3;
    Sprite[] spr_4;
    Sprite spr;

    public GameObject Game;
    public Text Round;
    public Text Score_1;
    public Text Player_1_name;
    public GameObject Player_1;
    public GameObject[] Dice_selected;
    public GameObject[] Dice;
    public GameObject[] Choice;
    public GameObject Exit;
    public GameObject Finish;
    public Text Rollleft;
    public System.Random rand;

    public AudioManager audiomanager;

    void PlaySound(int num)
    {
        audiomanager.playsound(num);
    }

    public void DiceSelect(int num)
    {
        if (roll == 3) return;

        if (keep[num] == 0)
        {
            keep[num] = 1;
            Dice_selected[num].SetActive(true);
        }
        else
        {
            keep[num] = 0;
            Dice_selected[num].SetActive(false);
        }
    }

    void UpdateRound()
    {
        Round.text = "Round : " + round.ToString();
    }

    void UpdateScoreText(int num, int scores)
    {
        if (scores >= 0)
        {
            Player_1.transform.GetChild(num).gameObject.GetComponent<Text>().text = scores.ToString();
        }
        else if (scores == -1)
        {
            Player_1.transform.GetChild(num).gameObject.GetComponent<Text>().text = "";
        }
        else
        {
            Player_1.transform.GetChild(num).gameObject.GetComponent<Text>().text = "35";
            Player_1.transform.GetChild(num).gameObject.GetComponent<Text>().color = new Color(151f / 255f, 250f / 255f, 138f / 255f, 255f / 255f);
        }
    }

    void UpdateTotal()
    {
        int sum = 0;
        for (int i = 0; i < 13; i++)
        {
            if (score[i] != -1 && score[i] != -2 && i != 6) sum += score[i];
            if (score[i] == -2) sum += 35;
        }
        score[13] = sum;
        total = sum;
        Score_1.text = total.ToString();
        Player_1.transform.GetChild(13).gameObject.GetComponent<Text>().text = total.ToString();
    }

    void UpdateRollleft()
    {
        Rollleft.text = "Roll : " + roll;
    }

    void UpdateDice(int num, int dice)
    {
        int skin = PlayerPrefs.GetInt("dice");
        if (skin == 0)
        {
            Dice[num].GetComponent<Image>().sprite = spr_0[dice];
        }
        else if (skin == 1)
        {
            Dice[num].GetComponent<Image>().sprite = spr_1[dice];
        }
        else if (skin == 2)
        {
            Dice[num].GetComponent<Image>().sprite = spr_2[dice];
        }
        else if (skin == 3)
        {
            Dice[num].GetComponent<Image>().sprite = spr_3[dice];
        }
        else if (skin == 4)
        {
            Dice[num].GetComponent<Image>().sprite = spr_4[dice];
        }
        else
        {
            Dice[num].GetComponent<Image>().sprite = spr_0[dice];
        }
    }

    public void UpdateChoice(int num)
    {
        if (choice != -1)
        {
            Choice[choice].SetActive(false);
        }
        choice = num;
        Choice[choice].SetActive(true);
    }

    void Calculate()
    {
        int i, max = 0, min = 5, str = 0, c_index = -1;
        int[] count;
        count = new[] { 0, 0, 0, 0, 0, 0 };

        for (i = 0; i < 5; i++)
        {
            count[dice[i] - 1]++;
        }

        for (i = 0; i < 6; i++)
        {
            if (count[i] > max) max = count[i];
            if (count[i] < min && count[i] != 0) min = count[i];
            if (count[i] != 0)
            {
                if (str <= 4)
                {
                    str++;
                }

            }
            if (count[i] == 0)
            {
                if (str == 4) str = 6;
                else if (str >= 5) continue;
                else str = 0;
            }
        }

        temp[0] = count[0];
        temp[1] = count[1] * 2;
        temp[2] = count[2] * 3;
        temp[3] = count[3] * 4;
        temp[4] = count[4] * 5;
        temp[5] = count[5] * 6;

        for (i = 0; i < 6; i++)
        {
            if (count[i] >= 3 && score[i] == -1) c_index = i;
        }

        temp[6] = count[0] + count[1] * 2 + count[2] * 3 + count[3] * 4 + count[4] * 5 + count[5] * 6;

        if (c_index == -1 && score[7] == -1) c_index = 6;

        if (max >= 4)
        {
            temp[7] = temp[6];
            if (score[8] == -1)
            {
                c_index = 7;
            }
        }
        else
        {
            temp[7] = 0;
        }

        if ((max == 3 && min == 2) || max == 5)
        {
            temp[8] = temp[6];
            if (score[9] == -1)
            {
                c_index = 8;
            }
        }
        else
        {
            temp[8] = 0;
        }

        if (str >= 4)
        {
            temp[9] = 15;
            if (score[10] == -1)
            {
                c_index = 9;
            }
        }
        else
        {
            temp[9] = 0;
        }

        if (str == 5)
        {
            temp[10] = 30;
            if (score[11] == -1)
            {
                c_index = 10;
            }
        }
        else
        {
            temp[10] = 0;
        }

        if (max == 5)
        {
            temp[11] = 50;
            if (score[12] == -1)
            {
                c_index = 11;
            }
        }
        else
        {
            temp[11] = 0;
        }

        if (c_index == -1)
        {
            for (i = 0; i < 12; i++)
            {
                if (i > 5)
                {
                    if (score[i + 1] == -1)
                    {
                        c_index = i;
                        break;
                    }
                }
                else
                {
                    if (score[i] == -1)
                    {
                        c_index = i;
                        break;
                    }
                }
            }
        }

        if (c_index > -1)
        {
            UpdateChoice(c_index);
        }
    }

    void Roll()
    {
        int i, j;

        for (i = 0; i < 5; i++)
        {
            if (keep[i] == 0)
            {
                dice[i] = rand.Next(1, 7);
            }
        }
        for (i = 0; i < 5; i++)
        {
            UpdateDice(i, dice[i] - 1);
        }

        Calculate();

        for (i = 0; i < 14; i++)
        {
            if (i != 6 && i != 13)
            {
                if (i > 6) j = i - 1;
                else j = i;
                if (score[i] == -1)
                {
                    UpdateScoreText(i, temp[j]);
                }
            }
        }
    }

    public void Rolling()
    {
        if (roll > 0 && round <= 12)
        {
            state = 1;
            roll--;
            UpdateRollleft();
            PlaySound(3);
            rolltime = 0.5f;
        }
    }

    public void Record()
    {
        if (choice == -1 || round >= 13 || roll == 3)
        {
            return;
        }

        int i, j, sum = 0;
        if (choice > 5) j = choice + 1;
        else j = choice;
        if (score[j] == -1)
        {
            score[j] = temp[choice];
            Player_1.transform.GetChild(j).gameObject.GetComponent<Text>().color = new Color(151f / 255f, 250f / 255f, 138f / 255f);
            for (i = 0; i < 13; i++)
            {
                if (i < 12) temp[i] = 0;
                if (i < 6 && score[i] != -1) sum += score[i];
                if (i == 6 && sum >= 63 && score[6] != -2) score[6] = -2;
                if (i == 6 && sum < 63 && sum != 0) score[6] = sum;
                UpdateScoreText(i, score[i]);
            }
            UpdateTotal();
            keep = new[] { 0, 0, 0, 0, 0 };
            roll = 3;
            round++;
            UpdateRound();
            UpdateRollleft();
            Reset();
            PlaySound(2);
        }
        else return;
    }

    void GameFinish()
    {
        round = 13;
        Finish.SetActive(true);
        Finish.transform.GetChild(2).GetComponent<Text>().text = "Score : " + total.ToString();
    }
    
    void Reset()
    {
        int i;
        for (i = 0; i < 5; i++)
        {
            Dice_selected[i].SetActive(false);
            Dice[i].GetComponent<Image>().sprite = spr;
        }
        choice = -1;
        for (i = 0; i < 12; i++)
        {
            Choice[i].SetActive(false);
        }
    }

    void ResetColor()
    {
        for (int i = 0; i < 14; i++)
        {
            if (i != 13)
            {
                Player_1.transform.GetChild(i).gameObject.GetComponent<Text>().color = new Color(204f / 255f, 204f / 255f, 204f / 255f);
            }
            else if (i == 13)
            {
                Player_1.transform.GetChild(i).gameObject.GetComponent<Text>().color = new Color(151f / 255f, 250f / 255f, 138f / 255f);
            }
        }
    }

    public void Backspace(int num)
    {
        if (num == 0)
        {
            Exit.SetActive(true);
        }
        else if (num == 1)
        {
            Exit.SetActive(false);
        }
        else
        {
            if (Exit.activeInHierarchy)
            {
                Exit.SetActive(false);
            }
            if (Finish.activeInHierarchy)
            {
                Finish.SetActive(false);
            }
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("Login");
        }
    }

    void Awake()
    {
        rand = new System.Random(seed);
        spr_0 = Resources.LoadAll<Sprite>("Dice_0");
        spr_1 = Resources.LoadAll<Sprite>("Dice_1");
        spr_2 = Resources.LoadAll<Sprite>("Dice_2");
        spr_3 = Resources.LoadAll<Sprite>("Dice_3");
        spr_4 = Resources.LoadAll<Sprite>("Dice_4");
        spr = Resources.Load<Sprite>("Dice_none_0");

        Game.SetActive(true);
        round = 1;
        roll = 3;
        choice = 0;
        rolltime = 0f;
        total = 0;
        temp = new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        score = new[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
        keep = new[] { 0, 0, 0, 0, 0 };
        dice = new[] { 0, 0, 0, 0, 0 };

        Player_1_name.text = PhotonNetwork.LocalPlayer.NickName;
        UpdateRound();
        UpdateTotal();
        Reset();
        ResetColor();

        seed = System.DateTime.Now.Millisecond;
        rand = new System.Random(seed);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected)
        {
            if (round <= 12)
            {
                if (Exit.activeInHierarchy)
                {
                    Backspace(1);
                }
                else
                {
                    Backspace(0);
                }
            }
            else if (Finish.activeInHierarchy)
            {
                Backspace(2);
            }
        }

        if (round == 13)
        {
            GameFinish();
        }

        if (state == 1 && rolltime >= 0f)
        {
            rolltime -= Time.deltaTime;
            for (int i = 0; i < 5; i++)
            {
                if (keep[i] != 1)
                {
                    UpdateDice(i, rand.Next(0, 6));
                }
            }
        }

        if (state == 1 && rolltime < 0f)
        {
            Roll();
            rolltime = 0f;
            state = 0;
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.LoadScene("Login");
    }
}