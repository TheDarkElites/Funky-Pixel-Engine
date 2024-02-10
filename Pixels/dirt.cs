using Godot;
using System;

public partial class dirt : solid
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		ductility = 0.19;

		Name = "Dirt";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
}
