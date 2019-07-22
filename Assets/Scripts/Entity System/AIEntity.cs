using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIEntity : Entity
{

    private static List<AIEntity> aiEntities = new List<AIEntity>();
    public static List<AIEntity> AIEntities { get => aiEntities; }

    protected override void Start()
    {
        base.Start();

        aiEntities.Add(this);
    }

    protected override void Update()
    {
        base.Update();
    }

}
