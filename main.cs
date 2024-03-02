using Godot;
using System;

public partial class main : Node2D
{
    public override void _Ready()
    {
        AddChild(GMS.GM);
    }
}
