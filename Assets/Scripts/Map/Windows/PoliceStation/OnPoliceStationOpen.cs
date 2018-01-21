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

    private List<GameObject> commonCharacters;
    private List<GameObject> specialCharacters;


    private void Start()
    {
        UpdatePoliceStationCharacters();
    }

    private void OnEnable()
    {
        transform.SetAsLastSibling();
        policeKnowledge.value = DataScript.eData.policeKnowledge;
        policeKnowledgeValueText.text = DataScript.eData.policeKnowledge + " / 100";
    }

    public void UpdatePoliceStationCharacters()
    {
        int spCount = DataScript.chData.panelSpCharacters.Count;
        int comCount = DataScript.chData.panelComCharacters.Count;

        if (commonCharacters != null)
            foreach (GameObject commonCharacter in commonCharacters)
                if (commonCharacter.gameObject) Destroy(commonCharacter.gameObject);

        if (specialCharacters != null)
            foreach (GameObject specialCharacter in specialCharacters)
                if (specialCharacter.gameObject) Destroy(specialCharacter.gameObject);

        commonCharacters = new List<GameObject>();
        specialCharacters = new List<GameObject>();

        for (int i = 0; i < spCount; i++)
        {
            if (DataScript.chData.panelSpCharacters[i].Status == CharacterStatus.arrested)
            {
                GameObject spChar = Instantiate(characterPrefab, charactersLocation);
                spChar.GetComponent<CharacterCustomization>().CustomizeSpecialCharacter(i);                    
                specialCharacters.Add(spChar);
            }
        }

        for (int i = 0; i < comCount; i++)
        {
            if (DataScript.chData.panelComCharacters[i].Status == CharacterStatus.arrested)
            {
                GameObject comChar = Instantiate(characterPrefab, charactersLocation);
                comChar.GetComponent<CharacterCustomization>().CustomizeCommonCharacter(i);
                commonCharacters.Add(comChar);
            }
        }
    }
}
