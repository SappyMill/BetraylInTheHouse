using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameInput : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_InputField nameInputField = null;
    [SerializeField] private Button continueButton = null;

    public static string DisplayName { get; private set; }

    private const string PlayerPrefsNamesKey = "PlayerName";

    private void SetUpInputField()
    {
        if(PlayerPrefs.HasKey(PlayerPrefsNamesKey))
        {
            string defaultName = PlayerPrefs.GetString(PlayerPrefsNamesKey);

            nameInputField.text = defaultName;

            SetPlayerName(defaultName);
        }
    }

    public void SetPlayerName(string name)
    {
        continueButton.interactable = string.IsNullOrEmpty(name);
    }

    public void SavePlayerName()
    {
        DisplayName = nameInputField.text;
        PlayerPrefs.SetString(PlayerPrefsNamesKey, DisplayName);
    }
}
