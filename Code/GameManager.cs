using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GameManager : Node2D
{   
    //Pixel Handeling Utilities
    
    //Pixel Stacks
    private Stack<Pixel> updateStack;
    private Stack<Pixel> updateStackBack;
    public Dictionary<Vector2, Pixel> pixelDict;
    
    //Times our physics updates
    private double last_physics_update;

    public override void _Ready()
    {
        pixelDict = new Dictionary<Vector2, Pixel>();
        updateStack = new Stack<Pixel>();
        updateStackBack = new Stack<Pixel>();
        last_physics_update = 0;

        GetNode<CheckBox>("/root/Main/Control/ShowUpdateMask").Pressed += () => GMS.DBS.UpdateMaskClear();
    }
    public override void _Process(double delta)
    {
        base._Process(delta);
        last_physics_update += delta;
		if(last_physics_update > GLOB.PHYSSPEED && GetNode<CheckBox>("/root/Main/Control/IsPhysicsOn").ButtonPressed)
		{
			last_physics_update = 0;
            GMS.BPPS.Fire(new Stack<Pixel>(updateStack));
            SanatizePixelDict();
            GMS.AFS.Fire(new Stack<Pixel>(updateStack));
            GMS.DBS.Fire(new Stack<Pixel>(updateStack));

            Dictionary<int,Pixel> updatedPixels = new Dictionary<int,Pixel>();
            updateStack.Clear();
            while(updateStackBack.Count > 0)
            {
                Pixel currentPixel = updateStackBack.Pop();
                if(updatedPixels.ContainsKey(currentPixel.GetHashCode()) || !IsInstanceValid(currentPixel))
                {
                    continue;
                }
                else
                {
                    updatedPixels[currentPixel.GetHashCode()] = currentPixel;
                    updateStack.Push(currentPixel);
                }
            }
            updateStackBack.Clear();
		}
    }

    public override void _Input(InputEvent @event)
    {
        // Mouse in viewport coordinates.
        if (@event is InputEventMouseButton eventMouseButton)
        {
            GMS.DBS.HandlePixelTools();
        }
    }

    //Lets us queue pixels to update
    public void queuePixelUpdate(Pixel pixelToUpdate)
    {
        updateStackBack.Push(pixelToUpdate);
    }
    public void queuePixelUpdate(List<Pixel> pixelsToUpdate)
    {
        foreach(Pixel pixelToUpdate in pixelsToUpdate)
        {
            updateStackBack.Push(pixelToUpdate);
        }
    }

    //Sanatizes our Pixel dict for invalid values
	public void SanatizePixelDict()
	{
		foreach(KeyValuePair<Vector2, Pixel> pixelPair in pixelDict)
		{
			if(IsInstanceValid(pixelPair.Value))
			{
				continue;
			}
			pixelDict.Remove(pixelPair.Key);
		}
	}
}