using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnPoliceStationOpen : MonoBehaviour
{
    public Transform charactersLocation;
    public GameObject characterPrefab;
    public Slider policeKnowledge;
    public Text policeKnowledgeValueText;

    private Dictionary<Character, GameObject> charactersDict;

    private void OnEnable()
    {
        transform.SetAsLastSibling();
        policeKnowledge.value = DataScript.eData.policeKnowledge;
        policeKnowledgeValueText.text = DataScript.eData.policeKnowledge + " / 100";
    }

    public void UpdatePoliceStationCharacters()
    {
        if (charactersDict != null)
            foreach (GameObject character in charactersDict.Values)
                if (character.gameObject) Destroy(character.gameObject);

        charactersDict = new Dictionary<Character, GameObject>();

        foreach (Character character in DataScript.chData.panelCharacters)
        {
            if (character.Status == CharacterStatus.arrested)
            {
                charactersDict.Add(character, Instantiate(characterPrefab, charactersLocation));
                charactersDict[character].GetComponent<CharacterCustomization>().CustomizeCharacter(character);
            }
        }
    }
}
