using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{

    private static List<Player> players = new List<Player>();
    public static List<Player> Players { get => players; }

    protected override void Start()
    {
        base.Start();

        players.Add(this);
    }

    protected override void Update()
    {
        base.Update();


    }
}
