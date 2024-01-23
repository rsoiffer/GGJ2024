using System.Collections;
using System.Collections.Generic;
using Data;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuessingGame : MonoBehaviour
{
    public PokemonDatabase pokemonDatabase;

    public Image image;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI scoreText;
    public Button[] buttons;
    public AudioSource guessAudio;
    public AudioSource cryAudio;

    [CanBeNull] private Button recentChoice;

    public IEnumerator Start()
    {
        foreach (var button in buttons) button.onClick.AddListener(() => recentChoice = button);

        var numCorrect = 0;
        var numTotal = 0;

        yield return null;
        while (true)
        {
            var options = PickRandom(4);
            var choiceId = Random.Range(0, options.Count);
            var choice = options[choiceId];

            image.color = Color.black;
            image.sprite = choice.spriteSet.front;
            descriptionText.text = "Who's that Pokemon?";
            for (var i = 0; i < options.Count; i++)
                buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = options[i].Name;
            guessAudio.Play();

            while (recentChoice == null) yield return null;
            numTotal += 1;
            numCorrect += recentChoice == buttons[choiceId] ? 1 : 0;
            recentChoice = null;

            image.color = Color.white;
            descriptionText.text = choice.Name;
            scoreText.text = $"Score: {numCorrect}/{numTotal}";
            cryAudio.clip = choice.cry;
            cryAudio.Play();

            yield return new WaitForSeconds(5);
        }
    }

    private List<PokemonData> PickRandom(int count)
    {
        var r = new List<PokemonData>();
        while (r.Count < count)
        {
            var pokemon = pokemonDatabase.database[Random.Range(0, pokemonDatabase.database.Length)];
            if (!string.IsNullOrEmpty(pokemon.ParentId) || r.Contains(pokemon)) continue;
            r.Add(pokemon);
        }

        return r;
    }
}