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
        messenger.Send("Кларк", @"Привет, Жора!
Бросай эту @#$%! Карла уволили. Ты теперь за него!");
        Battle.instance.OnWin = Start3Level;
        Battle.instance.SetAutomatic();
        Battle.instance.SetSampleCode();
        Battle.instance.ResetBattle(battleDatas[1]);
    }

    void Start3Level()
    {
        messenger.Send("Клара", @"Привет, Жора!
Я добавила лучников. Они могут атаковать на расстоянии -_-
Доработай бота.");
        Battle.instance.OnWin = EndGame;
        Battle.instance.SetAutomatic();
        Battle.instance.ResetBattle(battleDatas[2]);
    }

    void EndGame()
	{
        messenger.Send("Карен", @"Привет, Жора!
У нас уборщица приболела. Инвентарь знаешь где.");
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
        messenger.Send("Карл", @"Привет, Жора!
Попробуй обыграть моего бота. Он не умеет атаковать по диагонали. С этим-то надеюсь ты справишься.");
    }
}
