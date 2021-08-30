using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
	public string Type;
	public int MaxHP;
	public int HP
	{
		get => hp;
		set
		{
			hp = value;
			HPCount.text = $"{hp}/{MaxHP}";
		}
	}
	private int hp;
	public int Paws
	{
		get => paws;
		set {
			paws = value;
			PawsCount.text = $"{paws}/{MaxPaws}";
		}
	}
	private int paws;
	public int MaxPaws;
	public bool Owner;
	public int MeleeDamage;
	public int RangedDamage;

	public bool isActive=true;

	public bool IsCanMove { get => isActive && Paws > 0; }

	public Text HPCount;
	public Text PawsCount;
	public Text MeleeText;
	public Text RangedText;
	public Color redColor;
	public Color blueColor;

	private Image image;
	private Animator animator;
	private AudioSource audioS;

	private void Start()
	{
		image = GetComponent<Image>();
		//Init(Type, MaxHP, MaxPaws);
		MeleeText.text = MeleeDamage.ToString();
		if (RangedDamage > 0)
		{
			RangedText.text = RangedDamage.ToString();
		}
		animator = GetComponent<Animator>();
		audioS = GetComponent<AudioSource>();
	}

	public void Init()
	{
		HP = MaxHP;
		Paws = MaxPaws;
		image = GetComponent<Image>();
		image.color = Owner ? blueColor : redColor;
	}

	public void Move(Cell cell) {
		transform.SetParent(cell.transform, false);
		Paws--;
	}

	public void Disable()
	{
		isActive = false;
		image.color = new Color(image.color.r, image.color.g, image.color.b, 0.7f);
	}
	
	public void Enable()
	{
		isActive = true;
		Paws = MaxPaws;
		image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
	}

	public void MeleeAtack(Unit target, Cell cell, Cell targetCell)
	{
		Attack(target, cell, targetCell, MeleeDamage);
	}

	public void RangedAttack(Unit target, Cell cell, Cell targetCell)
	{
		Attack(target, cell, targetCell, RangedDamage);
	}

	private void Attack(Unit target, Cell cell, Cell targetCell, int damage) 
	{
		if (!isActive) return;
		animator.SetTrigger("hit");
		audioS.Play();
		target.HP -= damage;
		targetCell.TakeDamage();
		if (target.HP <= 0)
			Battle.instance.OnUnitDie(target, targetCell);
		Disable();
	}
}
