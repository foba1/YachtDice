using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class MainManager : MonoBehaviourPunCallbacks
{

    public static MainManager Instance
    {
        get
        {
            if (Instance == null) instance = FindObjectOfType<MainManager>();

            return Instance;
        }
    }

    private static MainManager instance;

    int state = 0, round = 0, turn = 0, roll = 0, choice = 0, state2 = 0, time; // state - 0 : lobby, 1 : game, 2 : finish / state2 - 0 : 스코어, 1 : 시간초과, 2 : 탈주
    int[] total, temp;
    int[,] score;
    int[] keep;
    int[] dice;
    int seed = 0;
    float counttime = 0f, rolltime = 0f, emoticon1 = 0f, emoticon2 = 0f;

    Sprite[] spr_0;
    Sprite[] spr_1;
    Sprite[] spr_2;
    Sprite[] spr_3;
    Sprite[] spr_4;
    Sprite spr;
    int[] diceskin = { 0, 0 };

    public PhotonView PV;
    public GameObject Wait;
    public Text Waiting;
    public Text Roomname;
    public GameObject Game;
    public GameObject[] Emoticon;
    public Text Score_1;
    public Text Score_2;
    public Text Player_1_name;
    public GameObject Player_1;
    public Text Player_2_name;
    public GameObject Player_2;
    public GameObject[] Dice_selected;
    public GameObject[] Dice;
    public GameObject Play_1;
    public GameObject Play_2;
    public GameObject[] Choice;
    public GameObject Exit;
    public GameObject Finish;
    public Text Rollleft;
    public Text CountTime;
    public System.Random rand;

    public AudioManager audiomanager;

    public void UnlockAchievement_1() => Social.ReportProgress(GPGSIds.achievement, 100, (bool success) => { });

    public void UnlockAchievement_2() => Social.ReportProgress(GPGSIds.achievement_2, 100, (bool success) => { });

    public void UnlockAchievement_3() => Social.ReportProgress(GPGSIds.achievement_3, 100, (bool success) => { });

    public void UnlockAchievement_4() => Social.ReportProgress(GPGSIds.achievement_4, 100, (bool success) => { });

    public void AddLeaderboard_1() => Social.ReportScore(int.Parse(PlayerPrefs.GetInt("win").ToString()), GPGSIds.leaderboard, (bool success) => { });

    void EmoticonInactive(int player)
    {
        int i;
        for (i = 0; i < 5; i++)
        {
            if (Emoticon[player].transform.GetChild(i).gameObject.activeInHierarchy)
            {
                Emoticon[player].transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        Emoticon[player].SetActive(false);
    }

    [PunRPC]
    void RPCUpdateEmoticon(int num, int player)
    {
        if (player == 0 && emoticon1 == 0)
        {
            emoticon1 = 2f;
        }
        else if (player == 1 && emoticon2 == 0)
        {
            emoticon2 = 2f;
        }
        else return;
        Emoticon[player].SetActive(true);
        Emoticon[player].transform.GetChild(num).gameObject.SetActive(true);
    }

    public void SendEmoticon(int num)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PV.RPC("RPCUpdateEmoticon", RpcTarget.AllBuffered, num, 0);
        }
        else
        {
            PV.RPC("RPCUpdateEmoticon", RpcTarget.AllBuffered, num, 1);
        }
    }

    [PunRPC]
    void RPCPlaySound(int num)
    {
        audiomanager.playsound(num);
    }

    [PunRPC]
    void RPCReset()
    {
        int i;
        for (i = 0; i < 5; i++)
        {
            Dice_selected[i].SetActive(false);
            Dice[i].GetComponent<Image>().sprite = spr;
        }
        choice = -1;
        for ( i = 0; i < 12; i++)
        {
            Choice[i].SetActive(false);
        }
        if (!CountTime.gameObject.activeInHierarchy)
        {
            CountTime.gameObject.SetActive(true);
        }
        RPCUpdateTimeColor(0);
    }

    [PunRPC]
    void RPCResetColor()
    {
        for (int i = 0; i < 13; i++)
        {
            Player_1.transform.GetChild(i).gameObject.GetComponent<Text>().color = new Color(204f / 255f, 204f / 255f, 204f / 255f);
            Player_2.transform.GetChild(i).gameObject.GetComponent<Text>().color = new Color(204f / 255f, 204f / 255f, 204f / 255f);
        }
    }

    void Reset()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        
        round = 0;
        turn = 0;
        roll = 3;
        choice = 0;
        counttime = time;
        rolltime = 0f;
        total = new[] { 0, 0 };
        temp = new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        score = new[,] { { -1, -1 }, { -1, -1 }, { -1, -1 }, { -1, -1 }, { -1, -1 }, { -1, -1 }, { -1, -1 }, { -1, -1 }, { -1, -1 }, { -1, -1 }, { -1, -1 }, { -1, -1 }, { -1, -1 }, { -1, -1 } };
        keep = new[] { 0, 0, 0, 0, 0 };
        dice = new[] { 0, 0, 0, 0, 0 };

        PV.RPC("RPCUpdateTotal", RpcTarget.AllBuffered, 0, 0);
        PV.RPC("RPCUpdateTotal", RpcTarget.AllBuffered, 0, 1);
        PV.RPC("RPCReset", RpcTarget.AllBuffered);
        PV.RPC("RPCResetColor", RpcTarget.AllBuffered);
        PV.RPC("RPCUpdateCheck", RpcTarget.AllBuffered, turn);

        seed = System.DateTime.Now.Millisecond;
        rand = new System.Random(seed);
    }

    void GameStart()
    {
        PV.RPC("RPCUpdateTimeColor", RpcTarget.AllBuffered, 0);
        if (!PhotonNetwork.IsMasterClient)
        {
            PV.RPC("RPCUpdateNickname", RpcTarget.AllBuffered, PhotonNetwork.MasterClient.NickName, PhotonNetwork.LocalPlayer.NickName);
            Play_1.GetComponent<Image>().color = new Color(255f / 255f, 131f / 255f, 129f / 255f);
            Score_1.GetComponent<Text>().color = new Color(255f / 255f, 131f / 255f, 129f / 255f);
            Player_1.transform.GetChild(13).gameObject.GetComponent<Text>().color = new Color(255f / 255f, 131f / 255f, 129f / 255f);
            PV.RPC("RPCUpdateDiceSkin", RpcTarget.AllBuffered, 1, PlayerPrefs.GetInt("dice"));
            return;
        }

        Play_2.GetComponent<Image>().color = new Color(255f / 255f, 131f / 255f, 129f / 255f);
        Score_2.GetComponent<Text>().color = new Color(255f / 255f, 131f / 255f, 129f / 255f);
        Player_2.transform.GetChild(13).gameObject.GetComponent<Text>().color = new Color(255f / 255f, 131f / 255f, 129f / 255f);
        PV.RPC("RPCUpdateDiceSkin", RpcTarget.AllBuffered, 0, PlayerPrefs.GetInt("dice"));
        Reset();
    }

    void GameFinish()
    {
        if (state2 < 1)
        {
            int score1 = score[13, 0];
            int score2 = score[13, 1];
            if (score1 > score2)
            {
                PV.RPC("RPCGameFinish", RpcTarget.AllBuffered, 0);
            }
            else if (score1 == score2)
            {
                PV.RPC("RPCGameFinish", RpcTarget.AllBuffered, 2);
            }
            else
            {
                PV.RPC("RPCGameFinish", RpcTarget.AllBuffered, 1);
            }
        }
        else if (state2 == 1)
        {
            if (turn == 0)
            {
                PV.RPC("RPCGameFinish", RpcTarget.AllBuffered, 0);
            }
            else
            {
                PV.RPC("RPCGameFinish", RpcTarget.AllBuffered, 1);
            }
        }
        else
        {
            if (turn == 0)
            {
                PV.RPC("RPCGameFinish", RpcTarget.AllBuffered, 1);
            }
            else
            {
                PV.RPC("RPCGameFinish", RpcTarget.AllBuffered, 0);
            }
        }
    }

    [PunRPC]
    void RPCGameFinish(int player)
    {
        state = 2;
        state2 = 1;
        Finish.SetActive(true);
        if (player == 0)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Finish.transform.GetChild(2).gameObject.GetComponent<Text>().text = "You Win!";
                PlayerPrefs.SetInt("win", PlayerPrefs.GetInt("win") + 1);
                PlayerPrefs.Save();
                audiomanager.playsound(4);
                if (PlayerPrefs.GetInt("win") >= 100)
                {
                    UnlockAchievement_1();
                }
                if (PlayerPrefs.GetInt("win") >= 1)
                {
                    UnlockAchievement_2();
                }
                AddLeaderboard_1();
            }
            else
            {
                Finish.transform.GetChild(2).gameObject.GetComponent<Text>().text = "You Lose..";
                PlayerPrefs.SetInt("lose", PlayerPrefs.GetInt("lose") + 1);
                PlayerPrefs.Save();
                audiomanager.playsound(5);
                if (PlayerPrefs.GetInt("lose") >= 1)
                {
                    UnlockAchievement_3();
                }
            }
        }
        else if (player == 1)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Finish.transform.GetChild(2).gameObject.GetComponent<Text>().text = "You Lose..";
                PlayerPrefs.SetInt("lose", PlayerPrefs.GetInt("lose") + 1);
                PlayerPrefs.Save();
                audiomanager.playsound(5);
                if (PlayerPrefs.GetInt("lose") >= 1)
                {
                    UnlockAchievement_3();
                }
            }
            else
            {
                Finish.transform.GetChild(2).gameObject.GetComponent<Text>().text = "You Win!";
                PlayerPrefs.SetInt("win", PlayerPrefs.GetInt("win") + 1);
                PlayerPrefs.Save();
                audiomanager.playsound(4);
                if (PlayerPrefs.GetInt("win") >= 100)
                {
                    UnlockAchievement_1();
                }
                if (PlayerPrefs.GetInt("win") >= 1)
                {
                    UnlockAchievement_2();
                }
                AddLeaderboard_1();
            }
        }
        else
        {
            Finish.transform.GetChild(2).gameObject.GetComponent<Text>().text = "-Draw-";
            PlayerPrefs.SetInt("draw", PlayerPrefs.GetInt("draw") + 1);
            audiomanager.playsound(4);
            if (PlayerPrefs.GetInt("draw") >= 1)
            {
                UnlockAchievement_4();
            }
        }
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    [PunRPC]
    void RPCUpdateTimeColor(int num)
    {
        if (num == 0)
        {
            CountTime.color = new Color(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
        }
        else if (num == 1)
        {
            CountTime.color = new Color(255f / 255f, 170f / 255f, 169f / 255f, 255f / 255f);
        }
        else
        {
            CountTime.color = new Color(255f / 255f, 105f / 255f, 103f / 255f, 255f / 255f);
        }
    }

    [PunRPC]
    void RPCUpdateScoreColor(int num, int player)
    {
        if (player == 0)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Player_1.transform.GetChild(num).gameObject.GetComponent<Text>().color = new Color(151f / 255f, 250f / 255f, 138f / 255f);
            }
            else
            {
                Player_1.transform.GetChild(num).gameObject.GetComponent<Text>().color = new Color(255f / 255f, 131f / 255f, 129f / 255f);
            }
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Player_2.transform.GetChild(num).gameObject.GetComponent<Text>().color = new Color(255f / 255f, 131f / 255f, 129f / 255f);
            }
            else
            {
                Player_2.transform.GetChild(num).gameObject.GetComponent<Text>().color = new Color(151f / 255f, 250f / 255f, 138f / 255f);
            }
        }
    }

    [PunRPC]
    void RPCUpdateTurn(int num)
    {
        turn = num;
    }

    [PunRPC]
    void RPCUpdateChoice2(int num,bool x)
    {
        Choice[num].SetActive(x);
    }

    [PunRPC]
    void RPCUpdateChoice(int num)
    {
        if (choice != -1)
        {
            PV.RPC("RPCUpdateChoice2", RpcTarget.AllBuffered, choice, false);
        }
        choice = num;
        PV.RPC("RPCUpdateChoice2", RpcTarget.AllBuffered, choice, true);
    }

    public void UpdateChoice(int num)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            if (turn == 0) return;
        }
        else
        {
            if (turn == 1) return;
        }

        PV.RPC("RPCUpdateChoice", RpcTarget.AllBuffered, num);
    }

    [PunRPC]
    void RPCUpdateTime(string time)
    {
        CountTime.text = time;
    }

    [PunRPC]
    void RPCUpdateCheck(int num)
    {
        if (num == 0)
        {
            Play_1.SetActive(true);
            Play_2.SetActive(false);
        }
        else
        {
            Play_1.SetActive(false);
            Play_2.SetActive(true);
        }
    }

    [PunRPC]
    void RPCUpdateNickname(string A, string B)
    {
        Player_1_name.text = A;
        Player_2_name.text = B;
    }

    [PunRPC]
    void RPCUpdateTotal(int totals,int player)
    {
        if (player == 0)
        {
            Score_1.text = totals.ToString();
            Player_1.transform.GetChild(13).gameObject.GetComponent<Text>().text = totals.ToString();
            if (PhotonNetwork.IsMasterClient)
            {
                if (totals > PlayerPrefs.GetInt("m_score"))
                {
                    PlayerPrefs.SetInt("m_score", totals);
                    PlayerPrefs.Save();
                }
            }
        }
        else
        {
            Score_2.text = totals.ToString();
            Player_2.transform.GetChild(13).gameObject.GetComponent<Text>().text = totals.ToString();
            if (!PhotonNetwork.IsMasterClient)
            {
                if (totals > PlayerPrefs.GetInt("m_score"))
                {
                    PlayerPrefs.SetInt("m_score", totals);
                    PlayerPrefs.Save();
                }
            }
        }
    }

    void UpdateTotal(int player)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        int sum = 0;
        for (int i = 0; i < 13; i++)
        {
            if (score[i, player] != -1 && score[i, player] != -2 && i != 6) sum += score[i, player];
            if (score[i, player] == -2) sum += 35;
        }
        score[13, player] = sum;
        PV.RPC("RPCUpdateTotal", RpcTarget.AllBuffered, sum, player);
    }

    [PunRPC]
    void RPCUpdateScore(int num,int scores,int player)
    {
        if (scores >= 0)
        {
            if (player == 0)
            {
                Player_1.transform.GetChild(num).gameObject.GetComponent<Text>().text = scores.ToString();
            }
            else
            {
                Player_2.transform.GetChild(num).gameObject.GetComponent<Text>().text = scores.ToString();
            }
        }
        else if (scores == -1)
        {
            if (player == 0)
            {
                Player_1.transform.GetChild(num).gameObject.GetComponent<Text>().text = "";
            }
            else
            {
                Player_2.transform.GetChild(num).gameObject.GetComponent<Text>().text = "";
            }
        }
        else
        {
            if (player == 0)
            {
                Player_1.transform.GetChild(num).gameObject.GetComponent<Text>().text = "35";
                if (PhotonNetwork.IsMasterClient)
                {
                    Player_1.transform.GetChild(num).gameObject.GetComponent<Text>().color = new Color(151f / 255f, 250f / 255f, 138f / 255f);
                }
                else
                {
                    Player_1.transform.GetChild(num).gameObject.GetComponent<Text>().color = new Color(255f / 255f, 131f / 255f, 129f / 255f);
                }
            }
            else
            {
                Player_2.transform.GetChild(num).gameObject.GetComponent<Text>().text = "35";
                if (PhotonNetwork.IsMasterClient)
                {
                    Player_2.transform.GetChild(num).gameObject.GetComponent<Text>().color = new Color(255f / 255f, 131f / 255f, 129f / 255f);
                }
                else
                {
                    Player_2.transform.GetChild(num).gameObject.GetComponent<Text>().color = new Color(151f / 255f, 250f / 255f, 138f / 255f);
                }
            }
        }
    }

    void UpdateScore(int num,int scores,int player)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        
        score[num, player] = scores;

        PV.RPC("RPCUpdateScore", RpcTarget.AllBuffered, num, scores, player);
    }

    [PunRPC]
    void RPCUpdateRollleft(int num)
    {
        Rollleft.text = "Roll : " + num;
    }

    [PunRPC]
    void RPCUpdateDiceSkin(int num1,int num2)
    {
        diceskin[num1] = num2;
    }

    [PunRPC]
    void RPCUpdateDice(int num1,int num2, int turn)
    {
        int dice = diceskin[turn];
        if (dice == 0)
        {
            Dice[num1].GetComponent<Image>().sprite = spr_0[num2];
        }
        else if (dice == 1)
        {
            Dice[num1].GetComponent<Image>().sprite = spr_1[num2];
        }
        else if (dice == 2)
        {
            Dice[num1].GetComponent<Image>().sprite = spr_2[num2];
        }
        else if (dice == 3)
        {
            Dice[num1].GetComponent<Image>().sprite = spr_3[num2];
        }
        else if (dice == 4)
        {
            Dice[num1].GetComponent<Image>().sprite = spr_4[num2];
        }
        else
        {
            Dice[num1].GetComponent<Image>().sprite = spr_0[num2];
        }
        
    }

    [PunRPC]
    void RPCDiceSelect2(int num,bool state)
    {
        Dice_selected[num].SetActive(state);
    }

    [PunRPC]
    void RPCDiceSelect(int num)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        if (roll == 3) return;

        if (keep[num] == 0)
        {
            keep[num] = 1;
            PV.RPC("RPCDiceSelect2", RpcTarget.AllBuffered, num, true);
        }
        else
        {
            keep[num] = 0;
            PV.RPC("RPCDiceSelect2", RpcTarget.AllBuffered, num, false);
        }
    }

    public void DiceSelect(int num)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            if (turn == 0)
            {
                return;
            }
        }
        else
        {
            if (turn == 1)
            {
                return;
            }
        }

        PV.RPC("RPCDiceSelect", RpcTarget.MasterClient, num);
    }

    void Calculate()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

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
            if (count[i] >= 3 && score[i, turn] == -1) c_index = i;
        }

        temp[6] = count[0] + count[1] * 2 + count[2] * 3 + count[3] * 4 + count[4] * 5 + count[5] * 6;

        if (c_index == -1 && score[7, turn] == -1) c_index = 6;

        if (max >= 4)
        {
            temp[7] = temp[6];
            if (score[8, turn] == -1)
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
            if (score[9, turn] == -1)
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
            if (score[10, turn] == -1)
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
            if (score[11, turn] == -1)
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
            if (score[12, turn] == -1)
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
                    if (score[i + 1, turn] == -1)
                    {
                        c_index = i;
                        break;
                    }
                }
                else
                {
                    if (score[i, turn] == -1)
                    {
                        c_index = i;
                        break;
                    }
                }
            }
        }

        PV.RPC("RPCUpdateChoice", RpcTarget.AllBuffered, c_index);
    }

    [PunRPC]
    void RPCRoll(int num)
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
            PV.RPC("RPCUpdateDice", RpcTarget.AllBuffered, i, dice[i] - 1, turn);
        }

        Calculate();

        for (i = 0; i < 14; i++)
        {
            if (i != 6 && i != 13)
            {
                if (i > 6) j = i - 1;
                else j = i;
                if (score[i, turn] == -1)
                {
                    PV.RPC("RPCUpdateScore", RpcTarget.AllBuffered, i, temp[j], turn);
                }
            }
        }
    }

    [PunRPC]
    void RPCRollMotion(int num)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if (roll > 0)
        {
            roll--;
            PV.RPC("RPCUpdateRollleft", RpcTarget.AllBuffered, roll);
            PV.RPC("RPCPlaySound", RpcTarget.AllBuffered, 3);
            rolltime = counttime;
        }
    }

    [PunRPC]
    public void Roll()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            if (turn == 0) return;
        }
        else
        {
            if (turn == 1) return;
        }
        
        PV.RPC("RPCRollMotion", RpcTarget.MasterClient, turn);
    }

    [PunRPC]
    void RPCRecord()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if (choice == -1 || roll == 3 || round == 12)
        {
            return;
        }

        int i, j, sum = 0;
        if (choice > 5) j = choice + 1;
        else j = choice;
        if (turn == 0)
        {
            if (score[j, 0] == -1)
            {
                score[j, 0] = temp[choice];
                PV.RPC("RPCUpdateScoreColor", RpcTarget.AllBuffered, j, 0);
                for (i = 0; i < 13; i++)
                {
                    if (i < 12) temp[i] = 0;
                    if (i < 6 && score[i, 0] != -1) sum += score[i, 0];
                    if (i == 6 && sum >= 63 && score[6, 0] != -2) score[6, 0] = -2;
                    if (i == 6 && sum < 63 && sum != 0) score[6, 0] = sum;
                    PV.RPC("RPCUpdateScore", RpcTarget.AllBuffered, i, score[i, 0], 0);
                }
                UpdateTotal(0);
                turn = 1;
                keep = new[] { 0, 0, 0, 0, 0 };
                roll = 3;
                counttime = time;
                PV.RPC("RPCUpdateTurn", RpcTarget.AllBuffered, turn);
                PV.RPC("RPCUpdateRollleft", RpcTarget.AllBuffered, roll);
                PV.RPC("RPCUpdateCheck", RpcTarget.AllBuffered, turn);
                PV.RPC("RPCReset", RpcTarget.AllBuffered);
                PV.RPC("RPCPlaySound", RpcTarget.AllBuffered, 2);
            }
            else return;
        }
        else
        {
            if (score[j, 1] == -1)
            {
                score[j, 1] = temp[choice];
                PV.RPC("RPCUpdateScoreColor", RpcTarget.AllBuffered, j, 1);
                for (i = 0; i < 13; i++)
                {
                    if (i < 12) temp[i] = 0;
                    if (i < 6 && score[i, 1] != -1) sum += score[i, 1];
                    if (i == 6 && sum >= 63 && score[6, 1] != -2) score[6, 1] = -2;
                    if (i == 6 && sum < 63 && sum != 0) score[6, 1] = sum;
                    PV.RPC("RPCUpdateScore", RpcTarget.AllBuffered, i, score[i, 1], 1);
                }
                UpdateTotal(1);
                turn = 0;
                keep = new[] { 0, 0, 0, 0, 0 };
                roll = 3;
                round++;
                counttime = time;
                PV.RPC("RPCUpdateTurn", RpcTarget.AllBuffered, turn);
                PV.RPC("RPCUpdateRollleft", RpcTarget.AllBuffered, roll);
                PV.RPC("RPCUpdateCheck", RpcTarget.AllBuffered, turn);
                PV.RPC("RPCReset", RpcTarget.AllBuffered);
                PV.RPC("RPCPlaySound", RpcTarget.AllBuffered, 2);
            }
            else return;
        }
    }

    public void Record()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            if (turn == 0) return;
        }
        else
        {
            if (turn == 1) return;
        }

        PV.RPC("RPCRecord", RpcTarget.MasterClient);
    }

    public void Backspace(int num)
    {
        if (state == 0)
        {
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
            SceneManager.LoadScene("Login");
        }
        else
        {
            if (num == 0)
            {
                if (state != 2)
                {
                    PlayerPrefs.SetInt("lose", PlayerPrefs.GetInt("lose") + 1);
                }
                if (PlayerPrefs.GetInt("lose") >= 1)
                {
                    UnlockAchievement_3();
                }
                state = 0;
                Exit.SetActive(false);
                if (PhotonNetwork.InRoom)
                {
                    PhotonNetwork.LeaveRoom();
                }
                SceneManager.LoadScene("Login");
            }
            else if (num == 1)
            {
                Exit.SetActive(true);
            }
            else
            {
                Exit.SetActive(false);
            }
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

        Roomname.text = PhotonNetwork.CurrentRoom.Name;
        if (PhotonNetwork.IsMasterClient)
        {
            if (PlayerPrefs.HasKey("time"))
            {
                time = PlayerPrefs.GetInt("time");
            }
            else
            {
                PlayerPrefs.SetInt("time", 45);
                PlayerPrefs.Save();
                time = PlayerPrefs.GetInt("time");
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected)
        {
            if (state == 0)
            {
                Backspace(0);
            }
            else
            {
                if (Exit.activeInHierarchy == true)
                {
                    Backspace(2);
                }
                else
                {
                    Backspace(1);
                }
            }
        }

        if (emoticon1 > 0) emoticon1 -= Time.deltaTime;
        if (emoticon1 < 0)
        {
            emoticon1 = 0;
            EmoticonInactive(0);
        }
        if (emoticon2 > 0) emoticon2 -= Time.deltaTime;
        if (emoticon2 < 0)
        {
            emoticon2 = 0;
            EmoticonInactive(1);
        }

        if (state == 0)
        {
            if (PhotonNetwork.PlayerList.Length < 2)
            {
                if (counttime <= 1.5f) counttime += Time.deltaTime;
                if (counttime <= 0.5f)
                {
                    Waiting.text = "Waiting.";
                }
                else if (counttime <= 1f)
                {
                    Waiting.text = "Waiting..";
                }
                else if (counttime <= 1.5f)
                {
                    Waiting.text = "Waiting...";
                }
                else
                {
                    counttime = 0f;
                }
            }
            else
            {
                state = 1;
                state2 = 0;
                Wait.SetActive(false);
                Game.SetActive(true);
                GameStart();
            }
        }

        if (state == 1 && PhotonNetwork.PlayerList.Length < 2)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                state = 2;
                state2 = 1;
                turn = 0;
                GameFinish();
            }
            else
            {
                state = 2;
                state2 = 1;
                turn = 1;
                GameFinish();
            }
        }

        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if (round == 12 && state == 1)
        {
            GameFinish();
        }

        if (state == 1)
        {
            if (PhotonNetwork.PlayerList.Length >= 2)
            {
                if (counttime >= 0)
                {
                    counttime -= Time.deltaTime;
                    if (counttime <= 10 && counttime >= 9)
                    {
                        PV.RPC("RPCUpdateTimeColor", RpcTarget.AllBuffered, 2);
                    }
                    if (counttime <= 20 && counttime >= 19)
                    {
                        PV.RPC("RPCUpdateTimeColor", RpcTarget.AllBuffered, 1);
                    }
                }
                else
                {
                    if (roll == 3)
                    {
                        state = 2;
                        state2 = 2;
                        GameFinish();
                    }
                    else
                    {
                        int i, j;
                        for (i = 0; i < 12; i++)
                        {
                            if (i > 5) j = i + 1;
                            else j = i;
                            if (score[j, turn] == -1)
                            {
                                choice = i;
                                break;
                            }
                        }
                        PV.RPC("RPCRecord", RpcTarget.MasterClient);
                    }
                }
                PV.RPC("RPCUpdateTime", RpcTarget.AllBuffered, ((int)counttime).ToString());
                if (counttime >= 0 && rolltime != 0f)
                {
                    if (counttime <= rolltime - 0.5f)
                    {
                        PV.RPC("RPCRoll", RpcTarget.MasterClient, turn);
                        rolltime = 0f;
                    }
                    else
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            if (keep[i] != 1)
                            {
                                PV.RPC("RPCUpdateDice", RpcTarget.AllBuffered, i, rand.Next(0, 6), turn);
                            }
                        }
                    }
                }
            }
        }
    }
    
    public override void OnLeftRoom()
    {
        state = 0;
        state2 = 0;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.LoadScene("Login");
    }
}
