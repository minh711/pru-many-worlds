using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static SceneConstants.SceneIndexs;

/// <summary>
/// Gắn vào enemy để lấy dữ liệu của Battle
/// </summary>
public class EnemyController : MonoBehaviour
{
    public BattleData battleData;

    private bool IsInTrigger = false;
    private Transform Player;

    private AudioSource AudioSource;

    private void Awake()
    {
        AudioSource = GetComponent<AudioSource>();
    }

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
            IsInTrigger = false;
            Player = null;
        }
    }

    public void OnActivate()
    {
        TriggerBattle();
    }

    public void OnMouseDown()
    {
        TriggerBattle();
    }

    private IEnumerator TriggerBattleCoroutine()
    {
        AudioSource.Play();

        yield return new WaitForSeconds(AudioSource.clip.length - 0.5f); // Wait for 1 second

        // Load battle data to game data
        SetBattleData();

        // Save Player position
        GameData.Instance.IsSetPosition = true;
        GameData.Instance.PlayerX = Player.position.x;
        GameData.Instance.PlayerY = Player.position.y;

        //Player entered, so move level
        SceneManager.LoadScene((int)BATTLE_SCENE, LoadSceneMode.Single);
    }

    private void TriggerBattle()
    {
        if (IsInTrigger && PlayerPrefs.GetInt(PlayerPrefsKeys.IS_UI_OPEN) == 0)
        {
            StartCoroutine(TriggerBattleCoroutine());
        }
    }
    
    private void SetBattleData()
    {
        GameData.Instance.SetBattleData(battleData);
    }
}
