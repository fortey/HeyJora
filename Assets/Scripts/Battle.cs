using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Battle : MonoBehaviour
{
	public static Battle instance;
	
	[SerializeField]
	public Board board;
	public Unit[][] units = new Unit[][]{
		new Unit[]{null,null,null,null,null},
		new Unit[]{null,null,null,null,null},
		new Unit[]{null,null,null,null,null},
		new Unit[]{null,null,null,null,null}
	};
	private readonly int RowsCount = 4;
	private readonly int ColsCount = 5;

	private Cell currentCell;
	public bool CurrentPlayer { get=>currentPlayer; }
	private bool currentPlayer = true;

	private JSCompiler jsCompiler;

	public GameObject Swordsman;
	public GameObject Archer;
	private BattleData battleData;
	public Button endTurnButton;
	public Button SubmitButton;
	public Button DisAutoButton;
	public InputField playerCode;
	public Bot playerSample;

	private Queue<ActionData> ActionQueue = new Queue<ActionData>();
	public Action OnWin;
	private bool isAutomatic;
	private int turnCount;
	private bool isStop;

	private Coroutine CurrentCoroutine;

	void Awake()
	{
		if (!instance)
			instance = this;
	}
	private void Start()
	{

	}

	public void Init(BattleData data)
	{
		LoadData(data);

		jsCompiler = FindObjectOfType<JSCompiler>();
		jsCompiler.SetValue("Action", new Action<int, int, int, int>((x, y, targetX, targetY) => ActionQueue.Enqueue(new ActionData { x = x, y = y, targetX = targetX, targetY = targetY })));
		jsCompiler.SetValue("GetUnits", new Func<Unit[][]>(() => units));
		jsCompiler.SetValue("GetUnit", new Func<int,int,Unit>((y,x) => GetUnit(y,x)));
		jsCompiler.SetValue("EndTurn", new Action(()=>EndTurn()));
		jsCompiler.SetValue("numberOfRows", RowsCount);
		jsCompiler.SetValue("numberOfColumns", ColsCount);
	}

	public void LoadData(BattleData data)
	{
		if (CurrentCoroutine != null)
		{
			StopCoroutine(CurrentCoroutine);
			CurrentCoroutine = null;
		}

		battleData = data;

		for (int r = 0; r < RowsCount; r++)
		{
			for (int c = 0; c < ColsCount; c++)
			{
				var unitType = data.rows[r].units[c];
				if (unitType == "Swordsman")
				{
					var unit = Instantiate(Swordsman, board[r, c].transform).GetComponent<Unit>();
					units[r][c] = unit;
					unit.Owner = r > 1;
					unit.Init();
				}
				if (unitType == "Archer")
				{
					var unit = Instantiate(Archer, board[r, c].transform).GetComponent<Unit>();
					units[r][c] = unit;
					unit.Owner = r > 1;
					unit.Init();
				}
			}
		}
		currentPlayer = true;
		endTurnButton.interactable = currentPlayer;
		RefreshUnits();
	}

	public void ShowAvailableCells(int x, int y)
	{
		if (units[y][ x])
		{
			if (x > 0) 
			{
				if(units[y][ x - 1])
					board[y, x - 1].SetAvailable();
				else if(units[y][ x].IsCanMove)
					board[y, x - 1].SetCanMove();
			}
			if (x < ColsCount-1)
			{
				if(units[y][x + 1])
					board[y, x + 1].SetAvailable();
				else if (units[y][x].IsCanMove)
					board[y, x + 1].SetCanMove();
			}
			if (y > 0)
			{
				if(units[y - 1][x])
					board[y - 1, x].SetAvailable();
				else if (units[y][x].IsCanMove)
					board[y - 1, x].SetCanMove();

				if (x > 0 && units[y - 1][x - 1])
					board[y - 1, x - 1].SetAvailable();
				if (x < ColsCount - 1 && units[y - 1][x + 1])
					board[y - 1, x + 1].SetAvailable();
			}
			if (y < RowsCount-1)
			{
				if(units[y + 1][x])
					board[y + 1, x].SetAvailable();
				else if (units[y][x].IsCanMove)
					board[y + 1, x].SetCanMove();

				if (x > 0 && units[y + 1][x - 1])
					board[y + 1, x - 1].SetAvailable();
				if (x < ColsCount - 1 && units[y + 1][x + 1])
					board[y + 1, x + 1].SetAvailable();
			}
			if(units[y][x].Type == "Archer"){
				for (int r = 0; r < RowsCount; r++)
				{
					for (int c = 0; c < ColsCount; c++)
					{
						var unit = units[r][c];
						if (unit)
						{
							board[r,c].SetAvailable();							
						}
					}
				}
			}
		}
	}

	public void OnClickCell(Cell cell)
	{
		if (!currentCell && units[cell.y][ cell.x] && units[cell.y][ cell.x].Owner && units[cell.y][cell.x].isActive)
		{
			currentCell = cell;
			ShowAvailableCells(cell.x, cell.y);
		}
		else
		{		
			if (cell.isAvailable)
			{
				if (!units[cell.y][ cell.x])
				{
					MoveUnit(cell);
					ResetAviable();
					currentCell = cell;
					ShowAvailableCells(cell.x, cell.y);
				}
				else
				{
					UnitAction(cell);
					ResetAviable();
					currentCell = null;
				}
			}
			else
			{
				
				ResetAviable();
				currentCell = null;
			}
		}
	}

	public void ResetBattle(BattleData data)
	{
		ActionQueue.Clear();
		isStop = true;
		ClearBoard();
		LoadData(data);
		turnCount = 0;
		SubmitButton.interactable = true;
	}
	public void ClearBoard()
	{
		for (int r = 0; r < RowsCount; r++)
		{
			for (int c = 0; c < ColsCount; c++)
			{
				var unit = units[r][c];
				if (unit)
				{
					units[r][c] = null;
					Destroy(unit.gameObject);
				}
			}
		}

		ResetAviable();
		currentCell = null;
	}
	private void MoveUnit(Cell cell)
	{
		var unit = units[currentCell.y][ currentCell.x];
		units[currentCell.y][ currentCell.x] = null;
		units[cell.y][ cell.x] = unit;
		unit.Move(cell);
	}

	private void MoveUnit(Unit unit, Cell cell, int targetX, int targetY)
	{
		if (targetX >= 0 && targetX < ColsCount && targetY >= 0 && targetY < RowsCount)
		{
			units[cell.y][cell.x] = null;
			units[targetY][targetX] = unit;
			unit.Move(board[targetY,targetX]);
		}
	}

	public void UnitAction(Cell cell)
	{
		if (Mathf.Abs(cell.x - currentCell.x) < 2 && Mathf.Abs(cell.y - currentCell.y) < 2)
			units[currentCell.y][ currentCell.x].MeleeAtack(units[cell.y][ cell.x], currentCell, cell);
		else
			units[currentCell.y][currentCell.x].RangedAttack(units[cell.y][cell.x], currentCell, cell);
	}
	private void ResetAviable()
	{
		for (int r = 0; r < RowsCount; r++)
		{
			for (int c = 0; c < ColsCount; c++)
			{
				board[r, c].ResetCanMove();
			}
		}
	}

	public void BotActions()
	{
		jsCompiler.ExecuteFunc(battleData.bot.code);
		CurrentCoroutine = StartCoroutine(InvokeBotActions());
	}

	IEnumerator InvokeBotActions()
	{
		yield return new WaitForSeconds(0.5f);
		while (ActionQueue.Count > 0)
		{
			var actData = ActionQueue.Dequeue();
			Action(actData.x, actData.y, actData.targetX, actData.targetY);
			yield return new WaitForSeconds(0.4f);
		}
		EndTurn();
	}

	private void PlayerBotActions()
	{
		try
		{
			jsCompiler.ExecuteFunc(playerCode.text);
		}
		catch(Exception e)
		{
			jsCompiler.AddLog(e.ToString());
			ActionQueue.Clear();
			ResetBattle(battleData);
			return;
		}
		CurrentCoroutine =  StartCoroutine(InvokeBotActions());
	}

	public void OnClickSubmit()
	{
		SubmitButton.interactable = false;
		isStop = false;
		PlayerBotActions();
	}

	public void EndTurn()
	{
		currentCell = null;
		if (isAutomatic && isStop) 
			return;
		currentPlayer = !currentPlayer;
		endTurnButton.interactable = currentPlayer;
		
		ResetAviable();
		RefreshUnits();
		
		if (!currentPlayer)
			BotActions();
		else if (isAutomatic)
		{
			turnCount++;
			if (turnCount > 50)
				ResetBattle(battleData);
			else
				PlayerBotActions();
		}
	}
	public void RefreshUnits()
	{
		for (int r = 0; r < RowsCount; r++)
		{
			for (int c = 0; c < ColsCount; c++)
			{
				var unit = units[r][c];
				if (unit)
				{
					if (unit.Owner == currentPlayer)
					{
						unit.Enable();
					}
					else
						unit.Disable();
				}
			}
		}
	}
	public void OnUnitDie(Unit unit, Cell cell)
	{
		units[cell.y][ cell.x] = null;
		Destroy(unit.gameObject);
		CheckEndBattle();
	}

	public void Action(int x, int y, int targetX, int targetY)
	{
		var unit = GetUnit(y, x);
		if(unit && unit.Owner==currentPlayer && unit.isActive)
		{
			var target = GetUnit(targetY, targetX);
			if (target)
			{
				if (Mathf.Abs(x - targetX) < 2 && Mathf.Abs(y - targetY) < 2)
					unit.MeleeAtack(target, board[y, x], board[targetY, targetX]);
				else if (unit.Type == "Archer")
					unit.RangedAttack(target, board[y, x], board[targetY, targetX]);
			}
			else if(unit.IsCanMove && (Mathf.Abs(x - targetX)==1 && y == targetY) || (Mathf.Abs(y - targetY) == 1 && x == targetX))
			{
				MoveUnit(unit, board[y,x], targetX, targetY);
			}

		}
	}

	private void CheckEndBattle()
	{
		var my = false;
		var enemy = false;
		for (int r = 0; r < RowsCount; r++)
		{
			for (int c = 0; c < ColsCount; c++)
			{
				var unit = units[r][c];
				if (unit)
				{
					if (unit.Owner)
					{
						my = true;
					}
					else
						enemy = true;
				}
			}
		}

		if (my && !enemy)
			Win();
		else if (!my)
			Lose();
	}

	public void SetAutomatic()
	{
		isAutomatic = true;
		for (int r = 0; r < RowsCount; r++)
		{
			for (int c = 0; c < ColsCount; c++)
			{
				board[r, c].turnOff = true;
			}
		}

		endTurnButton.gameObject.SetActive(false);
		SubmitButton.gameObject.SetActive(true);
		DisAutoButton.gameObject.SetActive(true);
	}

	public void DisableAutomatic()
	{
		isAutomatic = false;
		for (int r = 0; r < RowsCount; r++)
		{
			for (int c = 0; c < ColsCount; c++)
			{
				board[r, c].turnOff = false;
			}
		}

		endTurnButton.gameObject.SetActive(true);
		endTurnButton.interactable = currentPlayer;
		SubmitButton.gameObject.SetActive(false);
		DisAutoButton.gameObject.SetActive(false);
	}
	public void Win()
	{
		OnWin.Invoke();
	}

	public void Lose()
	{
		ResetBattle(battleData);
	}
	private Unit GetUnit(int y, int x)
	{
		if (x >= 0 && x < ColsCount && y >= 0 && y < RowsCount)
			return units[y][x];
		else return null;
	}

	public void SetSampleCode()
	{
		playerCode.text = playerSample.code;
	}
	struct ActionData 
	{
		public int x;
		public int y;
		public int targetX;
		public int targetY;
	}
}

[Serializable]
public struct Row
{
	public Cell[] cells;

	public Cell this[int i]
	{
		get { return cells[i]; }
		set { cells[i] = value; }
	}
}

[Serializable]
public struct Board
{
	public Row[] rows;

	public Cell this[int x, int y]
	{
		get { return rows[x][y]; }
		set { rows[x][y] = value; }
	}
}