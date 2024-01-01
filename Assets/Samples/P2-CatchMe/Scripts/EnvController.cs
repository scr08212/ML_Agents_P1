using CatchMe;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class EnvController : MonoBehaviour
{
    [System.Serializable]
    class AgentInfo
    {
        public CatchMeAgent Agent;
        [HideInInspector]
        public Rigidbody Rb;
        [HideInInspector]
        public Vector3 StartPos;
        [HideInInspector]
        public Quaternion StartRot;

        public void Set(Rigidbody rb, Vector3 startPos, Quaternion startRot)
        {
            Rb = rb;
            StartPos = startPos;
            StartRot = startRot;
        }
    }

    [SerializeField]
    int _maxStep = 0;

    [SerializeField]
    List<AgentInfo> _agents = new List<AgentInfo>();

    [SerializeField]
    [ReadOnly(true)]
    int _episode = 1;

    AgentGroup<CatchMeAgent> _polices = new AgentGroup<CatchMeAgent>();
    AgentGroup<CatchMeAgent> _criminals = new AgentGroup<CatchMeAgent>();

    public AgentGroup<CatchMeAgent> Polices =>_polices;
    public AgentGroup<CatchMeAgent> Criminals =>_criminals;

    private void Awake()
    {
        for(int i = 0; i < _agents.Count; i++) 
        {
            _agents[i].Set(
                _agents[i].Agent.GetComponent<Rigidbody>(),
                _agents[i].Agent.gameObject.transform.position,
                _agents[i].Agent.gameObject.transform.rotation);

            _agents[i].Agent.MaxStep = _maxStep;

            switch (_agents[i].Agent.Team)
            {
                case Team.Police: _polices.Register(_agents[i].Agent); break;
                case Team.Criminal: _criminals.Register(_agents[i].Agent); break;
            }
        }

        ResetScene();
    }

    public void EndEpisode()
    {
        foreach(var item in _agents)
        {
            item.Agent.EndEpisode();
        }
    }

    public void ResetScene()
    {
        foreach (var item in _agents)
        {
            item.Agent.transform.SetPositionAndRotation(item.StartPos, item.StartRot);
            item.Rb.velocity = Vector3.zero;
            item.Rb.angularDrag = 0f;
        }
    }
}