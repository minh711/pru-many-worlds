using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static SceneConstants;

public class MoveScene : MonoBehaviour
{
    public int SceneIndex;
    private bool IsInTrigger = false;
    private Transform Player;
    //Level move zoned enter, if collider is a player
    //Move game to another scene
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player = other.transform;
            IsInTrigger = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player = other.transform;
            IsInTrigger = false;
        }
    }
    public void OnActivate()
    {
        TriggerTele();
    }

    public void OnMouseDown()
    {
        TriggerTele();
    }
    private void TriggerTele()
    {
        if (IsInTrigger)
        {
            GameData.Instance.IsSetPosition = false;
            GameData.Instance.CurrentSceneIndex = SceneIndex;
            SceneManager.LoadScene(SceneIndex, LoadSceneMode.Single);
        }
    }
}
