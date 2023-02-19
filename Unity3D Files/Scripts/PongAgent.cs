using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class PongAgent : Agent
{
    //public GameObject self;
    public GameObject enemy;

    public GameObject pong;
    public Rigidbody2D rigidPong;
    GameController s_pong;

    public int isRight;
    public int speed;
    public float input;

    public bool isPlay = false;

    float yPos;

    Vector3 startPos;

    public void FixedUpdate()
    {
        //RequestDecision();
        
        if ((isRight == 0 && rigidPong.velocity.x < 0) || isPlay)
        {
            RequestDecision();
        }
        else if ((isRight == 1 && rigidPong.velocity.x > 0) || isPlay)
        {
            RequestDecision();
        }
        else
        {
            input = 0;
        }
    }

    public void Update()
    {
        yPos = transform.localPosition.y + (input * Time.deltaTime * speed);
        
        if (yPos > 7.25f)
        {
            yPos = 7.25f;
        }
        else if (yPos < -7.25f)
        {
            yPos = -7.25f;
        }

        transform.localPosition = new Vector3(transform.localPosition.x, yPos, transform.localPosition.z);
    }

    public override void Initialize()
    {
        startPos = transform.localPosition;
        rigidPong = pong.GetComponent<Rigidbody2D>();
        s_pong = pong.GetComponent<GameController>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(pong.transform.localPosition.x / 20f);
        sensor.AddObservation(pong.transform.localPosition.y / 20f);

        sensor.AddObservation(rigidPong.velocity.x / 20f);
        sensor.AddObservation(rigidPong.velocity.y / 20f);

        sensor.AddObservation(transform.localPosition.y / 8f);
        sensor.AddObservation(enemy.transform.localPosition.y / 8f);

        sensor.AddObservation(isRight);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        input = Mathf.Clamp(vectorAction[0], -1f, 1f);
        //input = Mathf.RoundToInt(vectorAction[0]) - 1;
        AddReward(-0.005f * Mathf.Abs(input));
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = startPos;
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Pong Move");
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Pong")
        {
            if (s_pong.render) { 
                s_pong.hitBack += isRight;
            }
            AddReward(0.25f);
            //s_pong.Reset();
            GetComponent<AudioSource>().Play();
        }
    }
}
