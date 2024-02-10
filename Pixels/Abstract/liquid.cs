using Godot;
using System;

public abstract partial class Liquid : Pixel
{
    // Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
        base._Ready();
        ductility = 0;
    }
}
