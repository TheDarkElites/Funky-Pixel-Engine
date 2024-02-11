using Godot;
using System;

public partial class steam : Gas
{
    // Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		ductility = 0.0;
        density = 0.005;

		Name = "Steam";
	}
}
