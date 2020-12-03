using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSMainPlayerModel : CSAvaterModel3D
{
    public CSMainPlayerModel(CSAvater Avater) : base(Avater)
    {
    }

    protected override void Analyze()
    {
        base.BeginAnalyze();

        Load(10, EModelStructure.Body, CSMotion.Stand, CSDirection.None, EResourceType.Body, EResourceAssistType.Player);

        base.EndAnalyze();
    }
}
