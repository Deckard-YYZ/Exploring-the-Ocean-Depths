using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
    public TMP_Text noduleCount;
    public TMP_Text funFact;
    public Image funFactBanner;
    public TMP_Text timer;
    public int numMinutes = 5;
    public GameObject gameStartPanel;
    public GameObject gameEndPanel;
    public TMP_Text endNoduleCount;
    public TMP_Text endNotFunFact;

    private float timeLeft;
    private float numCollected;
    private string[] funFacts;

    private void Start() {
        funFact.enabled = false;
        funFactBanner.enabled = false;
        gameEndPanel.SetActive(false);
        timeLeft = numMinutes * 60;

        funFacts = System.IO.File.ReadAllLines("Assets/Resources/NoduleFunFacts.txt");
    }

    private void Update() {
        timeLeft -= Time.deltaTime;
        TimeSpan time = TimeSpan.FromSeconds(timeLeft);
        timer.text = time.ToString("mm':'ss");

        if (timeLeft < 10) {
            timer.color = Color.red;
        }

        if (timeLeft <= 0) {
            EndGame();
        }
    }

    public void UpdateCount(int num) {
        noduleCount.text = "Collected: " + num.ToString();
        numCollected = num;
    }

    public void ShowFunFact(int num) {
        funFact.enabled = true;
        funFactBanner.enabled = true;
        funFact.text = funFacts[num-1];
        StartCoroutine(HideFunFact());
    }

    IEnumerator HideFunFact() {
        yield return new WaitForSeconds(4);
        funFact.enabled = false;
        funFactBanner.enabled = false;
    }

    private void EndGame() {
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        gameEndPanel.SetActive(true);
        endNoduleCount.text = noduleCount.text;

        if (numCollected > 100) {
            endNotFunFact.text = "By mining over 90% of the nodules in this area, you have destroyed this ecosystem.";
        }
        else if (numCollected > 80) {
            endNotFunFact.text = "By mining over 70% of the nodules in this area, you have wreaked havoc on this ecosystem.";
        }
        else if (numCollected > 60) {
            endNotFunFact.text = "By mining over 50% of the nodules in this area, you have caused potentially irreparable damage.";
        }
        else if (numCollected > 40) {
            endNotFunFact.text = "By mining over 30% of the nodules in this area, you have displaced and killed countless plants and animals.";
        }
        else if (numCollected > 20) {
            endNotFunFact.text = "By mining over 15% of the nodules in this area, you have severely damaged the habitats of many creatures.";
        }
        else if (numCollected >= 0) {
            endNotFunFact.text = "Luckily, the impact you have had on the area was minimal. Keep it that way!";
        }
    }


    public void ReturnToMainArea() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene("Mariana Trench");
    }
}
