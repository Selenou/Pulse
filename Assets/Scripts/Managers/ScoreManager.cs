using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

	public TextMeshProUGUI scoreTxt;
	public TextMeshProUGUI streakTxt;
	public Image accuracyArrow;
	public Image accuracyBg;
	public Image multiplierValueImg;
	public int noteValue;
	public int longNoteScoringPeriod; // ms
	private Dictionary<string, IEnumerator> scoringLongNoteCoroutines; // we need to keep references for all checkers
	private int currentStreak;
	private int scoreMultiplier;
	private int score;
	private Sprite[] multiplierSprites;
	public Sprite accuracyBgHigh;
	public Sprite accuracyBgMedium;
	public Sprite accuracyBgLow;

	// Accuracy stuff
	private float accuracy;
	private int totalNoteNb;
	private int failedNoteNb;

	void Start () {
		this.scoringLongNoteCoroutines = new Dictionary<string, IEnumerator>();
		this.multiplierSprites = Resources.LoadAll<Sprite>("Textures/ui/mult");
		this.accuracyBgHigh = Resources.Load<Sprite>("Textures/ui/rock_hi");
		this.accuracyBgMedium = Resources.Load<Sprite>("Textures/ui/rock_med");
		this.accuracyBgLow = Resources.Load<Sprite>("Textures/ui/rock_low");
		this.currentStreak = 0;
		this.scoreMultiplier = 1;
		this.score = 0;
		this.totalNoteNb = 0;
		this.accuracy = 0.0f;
	}

	public void UpdateScore() {
		this.IncrementScore();
		this.IncrementStreak();
		this.UpdateAccuracy();
		this.CheckForScoreMultiplierUpdate();
		this.UpdateHUD();
	}

	public void ResetScore() {
		this.ResetStreak();
		this.ResetScoreMultiplier();
		this.UpdateAccuracy();
		this.UpdateHUD();
	}

	public void StartLongNoteScoring(string tag) {
		this.scoringLongNoteCoroutines.Add(tag, this.LongNoteScoring());
        StartCoroutine(this.scoringLongNoteCoroutines[tag]);
	}

	public void StopLongNoteScoring(string tag) {
		if(this.scoringLongNoteCoroutines.ContainsKey(tag)) {
			StopCoroutine(this.scoringLongNoteCoroutines[tag]);
			this.scoringLongNoteCoroutines.Remove(tag);
		}
	}

	private void IncrementStreak() {
		this.currentStreak++;
	}

	private void IncrementScore() {
		this.score += this.noteValue * this.scoreMultiplier;
	}

    private IEnumerator LongNoteScoring(){
        while (true){
			yield return new WaitForSeconds(this.longNoteScoringPeriod / 1000.0f);
			this.score += (int)((this.longNoteScoringPeriod / 1000.0f) * this.noteValue * this.scoreMultiplier);
			this.UpdateScoreHUD();
        }
    }

	private void CheckForScoreMultiplierUpdate() {
		if(this.scoreMultiplier == 4) {
			return; // We dont need to go further if the multiplier is already set to 4
		} 

		if(this.currentStreak >= 30) {
			this.scoreMultiplier = 4;
		} else if(this.currentStreak >= 20){
			this.scoreMultiplier = 3;
		} else if(this.currentStreak >= 10){
			this.scoreMultiplier = 2;
		}

		this.multiplierValueImg.sprite = this.multiplierSprites[this.scoreMultiplier - 1];
	}

	private void UpdateAccuracy() {
		this.totalNoteNb += 1;

		if(this.currentStreak == 0) //player just missed a note
			this.failedNoteNb++; 

		this.accuracy = ((this.totalNoteNb - this.failedNoteNb) * 100.0f) / this.totalNoteNb;
		float rotation = this.accuracy * (-0.8f) + 40;

		if(rotation > 13.3f) {
			this.accuracyBg.sprite = this.accuracyBgLow;
		} else if(rotation > -13.3f) {
			this.accuracyBg.sprite = this.accuracyBgMedium;
		} else {
			this.accuracyBg.sprite = this.accuracyBgHigh;
		}

		this.accuracyArrow.rectTransform.rotation = Quaternion.Euler(0, 0, rotation);
	}

	private void ResetScoreMultiplier() {
		this.scoreMultiplier = 1;
		this.multiplierValueImg.sprite = this.multiplierSprites[0];
	}

	private void ResetStreak() {
		this.currentStreak = 0;
	}

	private void UpdateHUD() {
		this.UpdateScoreHUD();
		string test = this.currentStreak.ToString();
		string test2 = test.Substring(0,test.Length - 1);
		string test3 = test.Substring(test.Length - 1);
		this.streakTxt.SetText(test2 + "<#000000>" + test3 + "</color>");
	}

	private void UpdateScoreHUD() {
		this.scoreTxt.SetText(this.score.ToString());
	}
}
