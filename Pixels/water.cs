using Godot;
using System;

public partial class water : Liquid
{
    // Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		density = 0.3;

		Name = "Water";
	}
}
