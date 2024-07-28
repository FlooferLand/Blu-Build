using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class TTS : MonoBehaviour {
    public enum TTSName {
        Agit,
        Player
    }

    public TTSLetter[] ttsAlphabet;
    public string inputText;
    public string outputText;
    public TTSName ttsName;
    private string internalText;
    private int letterindex;
    private Player player;
    private AudioSource speech;
    private float timer;

    private void Start() {
        player = GameObject.Find("Player").GetComponent<Player>();
        speech = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    private void Update() {
        if (inputText != internalText) {
            internalText = inputText;
            outputText = "";
            letterindex = 0;
            switch (ttsName) {
                case TTSName.Agit:
                    player.agitText.text = outputText;
                    break;
                case TTSName.Player:
                    player.playerText.text = outputText;
                    break;
            }
        }

        timer = Mathf.Max(0, timer - Time.deltaTime);

        if (internalText != outputText && timer == 0) {
            bool isthereLetter = false;
            outputText += internalText[letterindex];
            switch (ttsName) {
                case TTSName.Agit:
                    player.agitText.text = outputText;
                    break;
                case TTSName.Player:
                    player.playerText.text = outputText;
                    break;
            }

            for (int i = 0; i < ttsAlphabet.Length; i++)
                if (char.ToLower(ttsAlphabet[i].letter) == char.ToLower(internalText[letterindex])) {
                    isthereLetter = true;
                    PlayVoice(i, internalText[letterindex]);
                    letterindex++;
                    break;
                }

            if (!isthereLetter) {
                PlayVoice(Random.Range(0, ttsAlphabet.Length), internalText[letterindex]);
                letterindex++;
            }
        }
    }

    private void PlayVoice(int theletter, char theactualletter) {
        if (ttsAlphabet[theletter].letter != ','
            && ttsAlphabet[theletter].letter != '.'
            && ttsAlphabet[theletter].letter != '!'
            && ttsAlphabet[theletter].letter != '?'
            && ttsAlphabet[theletter].letter != ' '
            && ttsAlphabet[theletter].letter != '\"'
            && ttsAlphabet[theletter].letter != '\''
            && ttsAlphabet[theletter].letter != ':'
            && ttsAlphabet[theletter].letter != ';'
            && ttsAlphabet[theletter].letter != '('
            && ttsAlphabet[theletter].letter != ')'
            && ttsAlphabet[theletter].letter != '-'
            && ttsAlphabet[theletter].letter != '_'
            && ttsAlphabet[theletter].letter != '+'
            && ttsAlphabet[theletter].letter != '='
            && ttsAlphabet[theletter].letter != '*'
            && ttsAlphabet[theletter].letter != '\'') {
            speech.volume = 0.9f;
            speech.PlayOneShot(ttsAlphabet[theletter].sound);
        }

        if (char.ToLower(theactualletter) == 'a'
            && char.ToLower(theactualletter) == 'e'
            && char.ToLower(theactualletter) == 'i'
            && char.ToLower(theactualletter) == 'o'
            && char.ToLower(theactualletter) == 'u'
            && char.ToLower(theactualletter) == 'y')
            timer += 0.06f;
        else if (theactualletter == ' ')
            timer += 0.1f;
        else if (theactualletter == ','
                 || theactualletter == '.'
                 || theactualletter == '!'
                 || theactualletter == '?'
                 || theactualletter == ':'
                 || theactualletter == ';')
            timer += 0.4f;
        else
            timer += 0.05f;
    }
}


[Serializable]
public class TTSLetter {
    public char letter;
    public AudioClip sound;
}