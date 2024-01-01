using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using Unity.Sentis.Layers;
using Unity.VisualScripting;
using UnityEngine;

namespace CatchMe
{
    public class CatchMeAgent : Agent
    {
        [SerializeField]
        Team _team;

        [SerializeField]
        float _speed;

        [SerializeField]
        int _episode = 0;

        Rigidbody _rb;
        EnvController _env;

        public Team Team => _team;

        protected override void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _env = GetComponentInParent<EnvController>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_team == Team.Police && collision.gameObject.CompareTag("Goal"))
            {
                _env.Polices.AddGroupReward(1f);
                _env.Criminals.AddGroupReward(-1f);
                _env.EndEpisode();
                _env.ResetScene();
            }
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            _env.Criminals.AddGroupReward(1f / MaxStep);
            _env.Polices.AddGroupReward(-1f / MaxStep);
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

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var discrete = actionsOut.DiscreteActions;

            if (Input.GetKey(KeyCode.W)) discrete[0] = 1;
            if (Input.GetKey(KeyCode.S)) discrete[0] = 2;
            if (Input.GetKey(KeyCode.A)) discrete[1] = 1;
            if (Input.GetKey(KeyCode.D)) discrete[1] = 2;
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(_rb.velocity); 
            sensor.AddObservation(_rb.angularVelocity);

            if (_team == Team.Police)
            {
                foreach(var item in _env.Polices.Group)
                {
                    if (item.Equals(this))
                        continue;

                    sensor.AddObservation(item.transform.position.x);
                    sensor.AddObservation(item.transform.position.z);
                    sensor.AddObservation(item.transform.rotation.y);
                }
            }
            
        }

        public override void OnEpisodeBegin()
        {
            _episode++;
        }
    }
}
