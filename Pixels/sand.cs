using Godot;
using System;

public partial class sand : Solid
{
    // Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		ductility = 0.1;

		Name = "Sand";
	}
}
