using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Messenger : MonoBehaviour
{
    public GameObject PhoneBack;
    public RectTransform Phone;
    public Text nameLabel;
	public Text message;

	private float startPosX;

	private float endPosX;
	private AudioSource audioS;

	private void Awake()
	{
		startPosX = Phone.anchoredPosition.x;
		endPosX = startPosX - 280;
		audioS = GetComponent<AudioSource>();
	}
	private void Start()
	{
		
	}

	public void Send(string _name,string text)
	{
		audioS.Play();
        PhoneBack.SetActive(true);
		nameLabel.text = _name;
		message.text = text;
		Phone.anchoredPosition = new Vector3(startPosX, Phone.anchoredPosition.y);

		StartCoroutine(ShowPhone());
	}

	IEnumerator ShowPhone()
	{
		var time = 0.5f;
		float x;
		while (time > 0) 
		{
			x = Mathf.Lerp(endPosX, startPosX, time*2);
			Phone.anchoredPosition = new Vector3(x, Phone.anchoredPosition.y);//, Phone.position.z);
			yield return new WaitForEndOfFrame();
			time -= Time.deltaTime;
		}

	}

	IEnumerator HidePhone()
	{
		var time = 0.5f;
		float x;
		while (time > 0)
		{
			x = Mathf.Lerp(startPosX, endPosX, time*2);
			Phone.anchoredPosition = new Vector3(x, Phone.anchoredPosition.y);
			yield return new WaitForEndOfFrame();
			time -= Time.deltaTime;
		}
		PhoneBack.SetActive(false);
	}

	public void Hide()
	{
		//PhoneBack.SetActive(false);
		StartCoroutine(HidePhone());
	}
	public void TestSend()
	{
		Send("",@"Привет, Жора!
<Hjdfd dfdfd ;ljfkj.");
	}
}
