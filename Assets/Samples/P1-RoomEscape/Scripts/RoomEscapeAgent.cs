using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEngine;

public class RoomEscapeAgent : Agent
{
    Rigidbody _rb;
    FieldObject _key;
    FieldObject _goal;
    TMPro.TextMeshPro _episodeCountText;

    public float _speed = 0.5f;
    public bool _hasKey = false;

    int _episodeCount = 0;
    Vector3 _initPos = Vector3.zero;
    Quaternion _initRot = Quaternion.identity;

    protected override void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _goal = GameObject.FindGameObjectWithTag("Goal").GetComponent<FieldObject>();
        _key = GameObject.FindGameObjectWithTag("Key").GetComponent<FieldObject>();
        _episodeCountText = GameObject.FindGameObjectWithTag("EpisodeText").GetComponent<TMPro.TextMeshPro>();
        _initPos = transform.position;
        _initRot = transform.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Goal") && _hasKey)
        {
            AddReward(1.0f);
            EndEpisode();
        }
        else if(other.gameObject.CompareTag("Key"))
        {
            _hasKey = true;
            _key.gameObject.SetActive(false);
            Debug.Log("Picked up key");
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        AddReward(-1f/MaxStep);
        MoveAgent(actions.DiscreteActions);
    }

    void MoveAgent(ActionSegment<int> act)
    {
        Vector3 dir = Vector3.zero;
        Vector3 rotateDir = Vector3.zero;

        switch (act[0])
        {
            case 1: // forward
                dir = transform.forward;
                break;
            case 2: // backward
                dir = transform.forward * -1f;
                break;
        }

        switch (act[1])
        {
            case 1: // left
                rotateDir = transform.up * -1f;
                break;
            case 2: // right
                rotateDir = transform.up;
                break;
        }

        transform.Rotate(rotateDir, Time.fixedDeltaTime * 200f);
        _rb.AddForce(dir * _speed, ForceMode.VelocityChange);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(_hasKey);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discrete = actionsOut.DiscreteActions;

        if (Input.GetKey(KeyCode.W)) discrete[0] = 1;
        if (Input.GetKey(KeyCode.S)) discrete[0] = 2;
        if (Input.GetKey(KeyCode.A)) discrete[1] = 1;
        if (Input.GetKey(KeyCode.D)) discrete[1] = 2;
    }

    public override void OnEpisodeBegin()
    {
        // Reset Params
        _hasKey = false;

        // Reset Agent
        transform.position = _initPos;
        transform.rotation = _initRot;
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;

        // Reset Field Objects
        _key.gameObject.SetActive(true);
        _key.ResetPos();
        _goal.ResetPos();

        // Update Episode Count
        _episodeCount++;
        _episodeCountText.text = _episodeCount.ToString();
    }
}
