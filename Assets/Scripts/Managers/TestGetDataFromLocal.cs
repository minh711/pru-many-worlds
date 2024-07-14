using Assets.Scripts.Managers;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TestGetDataFromLocal : MonoBehaviour
{
    public GameSave gameSave;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(() => 
        {
            var data = gameSave.GetPlayerDataFromLocal();
            Debug.Log("Load data from local: " + JsonUtility.ToJson(data));
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
