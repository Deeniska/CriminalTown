using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestAuthority : MonoBehaviour
{
    public InputField authorityInputField;

    // Use this for initialization
    void Start ()
    {
        authorityInputField.text = DataScript.pData.authority.ToString();
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void OnEndEditAuthority()
    {
        int.TryParse(authorityInputField.text, out DataScript.pData.authority);
        DataScript.SaveProgressData();
    }
}
