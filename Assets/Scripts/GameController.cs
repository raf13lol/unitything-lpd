using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Data;

public class GameController : MonoBehaviour
{
    public AudioSource musicPlayer;
    public RawImage faderexe;

    public string whereWarpForced;
    public int whereWarpNumForced;
    public bool isWarpForced = false;

    public bool deadLol = false;

    public static AudioClip music;
    public static string instrument;

    bool fadingInTheWorld = false;
    bool fadingOutTheWorld = false;

    // FUCK!
    bool doRandMaterial;
    public Material greyMat;
    public Material swapGreyMat;

    // Start is called before the first frame update
    void Start()
    {
        musicInitHandle();
        doRandMaterial = Random.Range(0, 10) == 5;
        DoMaterialChangeShit();
        SceneManager.activeSceneChanged += SceneChangeMFs;
    }

    void musicInitHandle()
    {
        musicPlayer = GameObject.FindGameObjectWithTag("MainCamera").gameObject.GetComponent<AudioSource>();
        string[] instruments = { "normel", "animated" };
        if (instrument == null)
            instrument = instruments[Random.Range(0, instruments.Length)];
        var musicnames = new Dictionary<string, string>() {
            {"lightmoonflat", "HappyCity"},
            {"happycity", "HappyCity" }
        };
        var musicname = musicnames[SceneManager.GetActiveScene().name.ToLower()];
        string[] whichones = { "a", "b",/* "c", "d", "e"*/};
        var whichone = whichones[Random.Range(0, whichones.Length)];
        music = (AudioClip)Resources.Load($"Assets/Sounds/{instrument}/{musicname}/{whichone}", typeof(AudioClip));
        musicPlayer.clip = music;
        musicPlayer.loop = true;
        musicPlayer.Play(0);
    }
    // Update is called once per frame
    void Update()
    {
        transitionHandling();
    }

    public void OhShitWereWarping()
    {
        fadingOutTheWorld = true;
        faderexe.enabled = true;
        Color[] cols = { Color.black, Color.blue, Color.red, Color.green, Color.white, Color.magenta, Color.yellow };
        faderexe.color = cols[Random.Range(0, cols.Length)];
        if (deadLol)
            faderexe.color = Color.black;
        Color blackColor = faderexe.color;
        blackColor.a = 0;
        faderexe.color = blackColor;
    }
    void SceneChangeMFs(Scene current, Scene next)
    {
        musicInitHandle();
        fadingInTheWorld = true;
        faderexe.enabled = true;
        Color blackColor = faderexe.color;
        blackColor.a = 1;
        faderexe.color = blackColor;

        var warps = GameObject.FindGameObjectsWithTag("Warp");
        if (warps.Length == 0)
        {
            isWarpForced = false;
            return;
        }
        var warpIndex = Random.Range(0, warps.Length);
        if (isWarpForced)
        {
            warpIndex = whereWarpNumForced;
        }
        foreach (GameObject warp in warps)
        {
            if (warp.gameObject.GetComponent<Warp>().warpNum == warpIndex)
            {
                GameObject.FindGameObjectWithTag("Player").gameObject.transform.position = warp.gameObject.transform.position;
                isWarpForced = false;
            }
        }
        DoMaterialChangeShit();
    }

    void DoMaterialChangeShit()
    {
        if (!doRandMaterial)
             return;
        var bigguy = GameObject.FindGameObjectWithTag("HasAll");
        GameObject[] childrens = new GameObject[bigguy.transform.childCount];
        for (var i = 0; i < bigguy.transform.childCount; i++)
        {
            childrens[i] = bigguy.transform.GetChild(i).gameObject;
        }
        string[] materialSwaps = { greyMat.name };
        Material[] newMats = { swapGreyMat };
        foreach (GameObject children in childrens)
        {
            var myMatIndex = -1;
            for (var j = 0; j < materialSwaps.Length; j++)
            {
                if (children.GetComponent<Renderer>().material.name.Replace(" (Instance)", "") == materialSwaps[j])
                    myMatIndex = j;
            }
            if (myMatIndex != -1)
            {
                children.GetComponent<Renderer>().material = newMats[myMatIndex];
            }
        }
    }

    void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    private void transitionHandling()
    {
        if (faderexe == null)
            faderexe = GameObject.FindGameObjectWithTag("Respawn").gameObject.GetComponent<RawImage>();
        if (fadingInTheWorld)
        {
            Color blackColor = faderexe.color;
            blackColor.a -= Time.deltaTime;
            faderexe.color = blackColor;
            if (blackColor.a <= 0.01f)
            {
                fadingInTheWorld = false;
                faderexe.enabled = false;
            }
        }
        else if (fadingOutTheWorld)
        {
            Color blackColor = faderexe.color;
            blackColor.a += Time.deltaTime;
            faderexe.color = blackColor;
            if (blackColor.a >= 0.99f)
            {
                blackColor.a = 1;
                faderexe.color = blackColor;
                fadingOutTheWorld = false;
                // faderexe.enabled = false;
                if (deadLol)
                {
                    print("die biatch");
                    Application.Quit(0);
                }
                else if (!isWarpForced)
                {
                    string[] possibleWarpPlaces = { "LightMoonFlat", "HappyCity" };
                    var index = 0;
                    var currentPlaceRN = SceneManager.GetActiveScene().name;
                    foreach (string place in possibleWarpPlaces)
                    {
                        if (place.Contains(currentPlaceRN))
                        {
                            if (currentPlaceRN == "LightMoonFlat")
                                possibleWarpPlaces[index] = "HappyCity";
                            else
                                possibleWarpPlaces[index] = "LightMoonFlat";
                        }
                        index++;
                    }
                    ChangeScene(possibleWarpPlaces[Random.Range(0, possibleWarpPlaces.Length)]);
                }
                else
                {
                    ChangeScene(whereWarpForced);
                }
            }
        }
    }
}
