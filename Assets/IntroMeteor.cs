using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroMeteor : MonoBehaviour
{
    public AudioClip meteorSound;
    public AudioSource audioSource;
    public AudioClip kaboomSound;

    public Transform meteorStartingPosition;
    public Transform meteorTargetPosition;

    public GameObject meteorTemplate;

    private Meteor _meteor;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            TriggerMeteor();
        }
    }

    private void TriggerMeteor()
    {
        Interact(meteorTargetPosition.position, StartGame);
    }

    private void StartGame()
    {
            SceneManager.LoadScene("Main", LoadSceneMode.Single);
        // StartCoroutine(DoSoon());
        //
        // IEnumerator DoSoon()
        // {
        //     yield return new WaitForSeconds(3);
        //     
        // }
    }

    public void Interact(Vector3 target, Action afterMeteorHasHit)
    {
        var meteorGameObject = Instantiate(meteorTemplate);
        var meteor = meteorGameObject.GetComponent<Meteor>();
        meteor.SetDuration(8);
        meteor.SetToIntroMode();
        _meteor = meteor;

        _meteor.SetTarget(target);
        _meteor.Shoot(meteorStartingPosition.position);

        PlaySound(meteorSound);
        _meteor.BeforeDestroy += afterMeteorHasHit;
        _meteor.Hit += PlayKaboom;

        var introTexts = FindObjectsOfType<IntroText>();
        foreach (var introText in introTexts)
        {
            Destroy(introText.gameObject);
        }
        
        FindObjectOfType<IntroSun>().StartSunSet();
    }

    public void PlayKaboom()
    {
        PlaySound(kaboomSound);
    }

    public void PlaySound(AudioClip sound)
    {
        audioSource.PlayOneShot(sound, .02f * GameManager.MasterVolume);
    }
}