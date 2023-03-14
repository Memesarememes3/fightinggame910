using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [Header("Lists")]
    public Color[] player_colors;
    public Sprite[] player_sprites;
    public List<PlayerController> player_list = new List<PlayerController>();
    public Transform[] spawn_points;


    [Header("prefab refs")]

    [Header("power-ups")]
    public float lastPowerUp;
    public float powerUpTimer;
    public GameObject[] powerups;
    public Transform[] PowerupSpawns;
    
    public GameObject playerContPrefab;

    [Header("Components")]
    private AudioSource audio;
    public AudioClip[] game_fx;
    public Transform containerGroup;
    public TextMeshProUGUI timeText;

    [Header("level vars")]
    public float startTime;
    public float curTime;
    List<PlayerController> winningplayers;
    public bool canJoin;


    //singleton
    public static GameManager instance;



    private void Awake()
    {
        canJoin = true;
        instance = this;
        audio = GetComponent<AudioSource>();
        containerGroup = GameObject.FindGameObjectWithTag("UIContainer").GetComponent<Transform>();
        startTime = PlayerPrefs.GetFloat("roundTimer", 100);
        winningplayers = new List<PlayerController>();
    }
    // Start is called before the first frame update
    void Start()
    {
        curTime = startTime;
        timeText.text = ((int)curTime).ToString();

    }

    // Update is called once per frame
    void Update()
    {

        /*if (Time.time - lastPowerUp > powerUpTimer)
        {
            Instantiate(powerups[Random.Range(0, powerups.Length)], PowerupSpawns[Random.Range(0, PowerupSpawns.Length)]);
            lastPowerUp = Time.time;
        }*/

        timeText.text = ((int)curTime).ToString();

        if (curTime <= 0 )
        {
            //find winner
            int highscore = 0;
            int index = 0;
            
            foreach( PlayerController player in player_list)
            {

                
                if (player.score > highscore)
                {
                    winningplayers.Clear();
                    highscore = player.score;
                    index = player_list.IndexOf(player);
                    winningplayers.Add(player);
                }
                else if (player.score == highscore)
                {
                    winningplayers.Add(player);
                }
                



            }

            if(winningplayers.Count > 1)
            {
                canJoin = false;
                //this is a tie

                // play sound to indicate over time

                foreach(PlayerController player in player_list)
                {
                    if( !winningplayers.Contains(player))
                        
                        {
                        player.dropOut();
                        }
                }
                curTime = 30;
            }

            else
            {
                PlayerPrefs.SetInt("colorIndex", index);
                SceneManager.LoadScene("winScreen");
            }
            
            
        }
    }

    public void FixedUpdate()
    {
        curTime -= Time.deltaTime;
    }


    public void onPlayerJoined(PlayerInput player)
    {
        if(canJoin)
        {
            audio.PlayOneShot(game_fx[0]);
            //player.GetComponentInChildren<SpriteRenderer>().color = player_colors[player_list.Count];
            player.GetComponentInChildren<SpriteRenderer>().sprite = player_sprites[player_list.Count];

            //create a ui container
            PlayerContainerScript cont = Instantiate(playerContPrefab, containerGroup).GetComponent<PlayerContainerScript>();
            //assign cont to a player
            player.GetComponent<PlayerController>().setUI(cont);
            cont.initialize(player_colors[player_list.Count]);


            player_list.Add(player.GetComponent<PlayerController>());

            player.transform.position = spawn_points[Random.Range(0, spawn_points.Length)].position;
        }
        
    }
}
