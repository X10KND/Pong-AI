using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject ai_win;
    public GameObject human_win;

    public Text score_left;
    public Text score_right;

    public Text hitText;

    public int scoreVal_l = 0;
    public int scoreVal_r = 0;

    public int hitBack = 0;

    public GameObject left;
    public GameObject right;

    PongAgent s_left;
    PongAgent s_right;

    //public GameObject pong;
    public Rigidbody2D rigidPong;
    public Vector3 startPos;

    public int speed;

    public bool isCoroutine = false;
    public bool start = false;

    public bool render = false;

    void Start()
    {
        startPos = transform.position;

        rigidPong = GetComponent<Rigidbody2D>();

        s_left = left.GetComponent<PongAgent>();
        s_right = right.GetComponent<PongAgent>();

        StartCoroutine(SpawnPong());
        isCoroutine = true;
    }

    IEnumerator SpawnPong()
    {
        start = false;

        transform.position = new Vector3(startPos.x, startPos.y, startPos.z);
        rigidPong.velocity = new Vector2(0, 0);

        yield return new WaitForSeconds(3f);

        transform.position = new Vector3(startPos.x, startPos.y + Random.Range(-6f, 6f), startPos.z);

        float angle = Random.Range(10f, 80f);

        float x = speed * Mathf.Cos(angle * Mathf.Deg2Rad) * -1f; //Throw to AI first
        float y = speed * Mathf.Sin(angle * Mathf.Deg2Rad) * (2 * Random.Range(0, 2) - 1);

        rigidPong.velocity = new Vector2(x, y) * speed;

        isCoroutine = false;
        start = true;
    }
    
    void Update()
    {
        if (Input.GetAxis("Cancel") == 1)
        {
            Application.Quit();
        }
        if (render)
        {
            score_left.text = scoreVal_l + "   ";
            score_right.text = "   " + scoreVal_r;
            hitText.text = "Defense = " + hitBack;
        }

        if (transform.localPosition.x < -16 || transform.localPosition.x > 16 ||
            transform.localPosition.y < -8 || transform.localPosition.y > 8)
        {
            if (!isCoroutine)
            {
                Reset();
            }
        }
        if(Mathf.Abs(rigidPong.velocity.x) < 2f && start)
        {
            rigidPong.velocity = new Vector2((2 * Random.Range(0, 2) - 1) * 2f, rigidPong.velocity.y);
        }
        if (Mathf.Abs(rigidPong.velocity.y) < 0.5f && start)
        {
            rigidPong.velocity = new Vector2(rigidPong.velocity.x, (2 * Random.Range(0, 2) - 1));
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Left")
        {
            s_left.AddReward(Mathf.Abs(transform.localPosition.y - left.transform.localPosition.y) * -1f - 1f);
            //s_left.AddReward(-1f);
            s_right.AddReward(1f);

            if (render)
            { 
                scoreVal_r += 1;

                if (scoreVal_r >= 10)
                {
                    isCoroutine = true;
                    human_win.SetActive(true);
                    StartCoroutine(EndGame());
                }
            }
            if (!isCoroutine)
            {
                Reset();
            }
        }
        else if (other.tag == "Right")
        {
            s_left.AddReward(1f);
            //s_right.AddReward(-1f);
            s_right.AddReward(Mathf.Abs(transform.localPosition.y - right.transform.localPosition.y) * -1f - 1f);

            if (render)
            {
                scoreVal_l += 1;

                if (scoreVal_l >= 10)
                {
                    isCoroutine = true;
                    ai_win.SetActive(true);
                    StartCoroutine(EndGame());
                }
            }

            if (!isCoroutine)
            {
                Reset();
            }
        }
    }

    public void Reset()
    {
        s_left.EndEpisode();
        s_right.EndEpisode();

        StartCoroutine(SpawnPong());
        isCoroutine = true;
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(7f);
        Reset();

        scoreVal_l = 0;
        scoreVal_r = 0;
        hitBack = 0;

        start = false;

        human_win.SetActive(false);
        ai_win.SetActive(false);
    }
}
