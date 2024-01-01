using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.MLAgents;
using UnityEngine;

public class AgentGroup<T> where T : Agent
{
    List<T> _group = new List<T>();

    public List<T> Group=>_group;

    public void Register(T agent)
    {
        _group.Add(agent);
    }

    public void Unregister(T agent)
    {
        _group.Remove(agent);
    }

    public void EndGroupEpisode()
    {
        foreach(T agent in _group)
        {
            agent.EndEpisode();
        }
    }

    public void AddGroupReward(float value)
    {
        foreach(T agent in _group)
        {
            agent.AddReward(value);
        }
    }

    public void SetGroupReward(float value) 
    {
        foreach(T agent in _group)
        {
            agent.SetReward(value);
        }
    }

    public void GroupEpisodeInterrupted()
    {
        foreach (T agent in _group)
        {
            agent.EpisodeInterrupted();
        }
    }
}