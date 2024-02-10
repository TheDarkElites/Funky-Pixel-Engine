using Godot;
using System;

public abstract partial class Pixel : Node2D
{
	//Update handler
	public bool doUpdate;
	
	//Properties
	public double ductility;
	public bool conductive;
	public double toughness;
	public bool isStatic;
	
	//Physical Attrributes
	public double temperature;

	public override void _Ready()
	{
		ductility = 1.00;
		conductive = false;
		toughness = 1.00;
		isStatic = false;
		doUpdate = true;

		Name = "PlaceholderPixel";
	}

}
