using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cell : MonoBehaviour, IPointerClickHandler
{
    public bool isAvailable;
	public int x;
	public int y;
	public bool turnOff;

	private Color normalColor;
	public Color availableColor;

	private Image image;
	private void Start()
	{
		image = GetComponent<Image>();
		normalColor = image.color;
	}
	public void SetCanMove()
	{
		image.color = availableColor;
		isAvailable = true;
	}

	public void SetAvailable()
	{
		image.color = availableColor;
		isAvailable = true;
	}

	public void ResetCanMove()
	{
		image.color = normalColor;
		isAvailable = false;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if(!turnOff)
		Battle.instance.OnClickCell(this);
	}

	IEnumerator TintColor(Color color)
	{
		var startColor = image.color;
		var time = 0.5f;
		var k = 1 / time;
		while (time > 0)
		{
			yield return new WaitForFixedUpdate();
			time -= Time.fixedDeltaTime;
			image.color = Color.Lerp(color, startColor, time * k);
		}
		time = 0.5f;
		while (time > 0)
		{
			yield return new WaitForFixedUpdate();
			time -= Time.fixedDeltaTime;
			image.color = Color.Lerp(startColor, color, time * k);
		}
		image.color = startColor;
		if (!isAvailable)
			image.color = normalColor;
	}

	public void TakeDamage()
	{
		StartCoroutine(TintColor(Color.red));
	}
}
