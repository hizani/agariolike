using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerHelper : NetworkBehaviour
{
    [SyncVar]
    public string PlayerName;
    [SyncVar]
    public float Speed = 5;
    [SyncVar]
    public float Size = 0.2f;
    [SyncVar]
    public int Score = 0;
    public Text PlayerScoreText;
    public int PlayerKills;
    public Text PlayerKillsText;
    [SyncVar]
    public Color Color = Color.blue;
    [SyncVar]
    int spriteNumber;
    public List<Sprite> sprites = new List<Sprite>();

    public List<PlayerHelper> PlayerList = new List<PlayerHelper>();
    public List<Text> ScoreBoard = new List<Text>();
    GameHelper _gameHelper;

    void Start()
    {
        _gameHelper = GameObject.FindObjectOfType<GameHelper>();
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("BoardPlaces"))
        {

            ScoreBoard.Add(player.GetComponent<Text>());
        }
        if (!isLocalPlayer)
            return;
        PlayerName = "Player " + Random.Range(1, 51);
        _gameHelper.CurrentPlayer = this;
        spriteNumber = Random.Range(0, sprites.Count);
        PlayerScoreText = GameObject.FindGameObjectWithTag("PlayerScore").GetComponent<Text>();
        PlayerKillsText = GameObject.FindGameObjectWithTag("PlayerKills").GetComponent<Text>();
        GameObject.FindGameObjectWithTag("PlayerName").GetComponent<Text>().text = PlayerName;
        CalcScore();
    }

    void Update()
    {
        GetComponent<SpriteRenderer>().color = Color;
        GetComponent<SpriteRenderer>().sprite = sprites[spriteNumber];
        if (GameObject.FindGameObjectsWithTag("Player").Length > PlayerList.Count) 
        {
            PlayerList.Clear();
            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            {

                PlayerList.Add(player.GetComponent<PlayerHelper>());
            }
            
        }
        transform.localScale = new Vector2(Size, Size);
        CheckPlace();
        if (!isLocalPlayer)
            return;
        CalcScore();
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        transform.position = Vector2.MoveTowards(transform.position, mousePos,
            Time.deltaTime * Speed / Size);

        CheckBounds();
    }

    private void CheckBounds()
    {
        if (transform.position.x >= _gameHelper.MapSize.x)
            transform.position = new Vector2(_gameHelper.MapSize.x - 0.01f, transform.position.y);
        if (transform.position.x <= -_gameHelper.MapSize.x)
            transform.position = new Vector2(-_gameHelper.MapSize.x + 0.01f, transform.position.y);


        if (transform.position.y >= _gameHelper.MapSize.y)
            transform.position = new Vector2(transform.position.x, _gameHelper.MapSize.y - 0.01f);
        if (transform.position.y <= -_gameHelper.MapSize.y)
            transform.position = new Vector2(transform.position.x, -_gameHelper.MapSize.y + 0.01f);
    }

    [Server]
    public void ChangeSize(float newSize)
    {
        if (newSize < 20)
        {
            Size = newSize;
            transform.localScale = new Vector2(Size, Size);
            var spriteMask = FindObjectOfType<SpriteMask>().transform.localScale;
            spriteMask = new Vector2(spriteMask.x + newSize - Size, spriteMask.y + newSize - Size);

        }
    }
    public void CalcScore()
    {
        Score = (int)(100 * transform.localScale.x);
        PlayerScoreText.text = "Score: " + Score;
        Debug.Log(Score);

    }

    public void SetKills()
    {
        PlayerKills++;
        PlayerKillsText.text = "Kills: " + PlayerKills;
    }

    void CheckPlace()
    {
        int place = PlayerList.Count-1;
        for(int i = 0; i < PlayerList.Count; i++)
        {
            if (this.Score > PlayerList[i].Score)
                place--;
        }
        if (ScoreBoard.Count >= place)
            ScoreBoard[place].text = this.PlayerName + " " + this.Score + "s"; 
    }

    [ServerCallback]
    private void OnTriggerStay2D(Collider2D collision)
    {

        switch (collision.gameObject.tag)
        {
            case "Player":
                Debug.Log("Enemy Enter");
                Bounds enemy = collision.bounds;
                Bounds currentForEnemy = GetComponent<Collider2D>().bounds;

                Vector2 centerEnemy = enemy.center;
                Vector2 centerCurrentForEnemy = currentForEnemy.center;

                if (currentForEnemy.size.x > enemy.size.x + 0.1 &&
                    Vector2.Distance(centerCurrentForEnemy, centerEnemy) < currentForEnemy.size.x)
                {
                    ChangeSize(Size + collision.gameObject.transform.localScale.x);
                    NetworkServer.Destroy(collision.gameObject);
                    if (!isLocalPlayer)
                        return;
                    SetKills();
                }
                break;
            case "Food":
                Debug.Log("Food Enter");
                NetworkServer.Destroy(collision.gameObject);
                ChangeSize(Size + 0.01f);
                _gameHelper.foodCount--;
                Debug.Log(Size);
                break;
            case "GreenThing":
                Debug.Log("GreenThing Enter");
                Bounds greenThing = collision.bounds;
                Bounds currentForGT = GetComponent<Collider2D>().bounds;

                Vector2 centerGreenThing = greenThing.center;
                Vector2 centerCurrentForGT = currentForGT.center;

                if (currentForGT.size.x > greenThing.size.x &&
                    Vector2.Distance(centerCurrentForGT, centerGreenThing) < currentForGT.size.x)
                {
                    ChangeSize(Size - (Size/2) );
                    NetworkServer.Destroy(collision.gameObject);
                }
                break;
        }

    }

}
