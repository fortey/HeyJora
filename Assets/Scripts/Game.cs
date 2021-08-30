using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public Messenger messenger;
    public BattleData[] battleDatas;
    public GameObject menu;

    private bool isStart;

    void Start()
    {
        
    }

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape)&&isStart)
		{
            menu.SetActive(!menu.activeSelf);
		}
	}
	void Start2Level()
	{
        messenger.Send("�����", @"������, ����!
������ ��� @#$%! ����� �������. �� ������ �� ����!");
        Battle.instance.OnWin = Start3Level;
        Battle.instance.SetAutomatic();
        Battle.instance.SetSampleCode();
        Battle.instance.ResetBattle(battleDatas[1]);
    }

    void Start3Level()
    {
        messenger.Send("�����", @"������, ����!
� �������� ��������. ��� ����� ��������� �� ���������� -_-
��������� ����.");
        Battle.instance.OnWin = EndGame;
        Battle.instance.SetAutomatic();
        Battle.instance.ResetBattle(battleDatas[2]);
    }

    void EndGame()
	{
        messenger.Send("�����", @"������, ����!
� ��� �������� ���������. ��������� ������ ���.");
    }

	public void Exit()
	{
        Application.Quit();
    }

    public void StartGame()
	{
        if (isStart)
            Battle.instance.ResetBattle(battleDatas[0]);
        else
            Battle.instance.Init(battleDatas[0]);
        isStart = true;

        
        Battle.instance.OnWin = Start2Level;
        //Battle.instance.SetAutomatic();
        messenger.Send("����", @"������, ����!
�������� �������� ����� ����. �� �� ����� ��������� �� ���������. � ����-�� ������� �� ����������.");
    }
}
