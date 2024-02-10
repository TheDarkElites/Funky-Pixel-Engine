using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GameManager : Node2D
{
    //System that handles basic pixel physics (when they arent rigid bodies)
    private BasePixelPhysicsSystem BPPS;
    private List<Sprite2D> updateMaskList;

    public override void _Ready()
    {
        BPPS = new BasePixelPhysicsSystem(this);
        updateMaskList = new List<Sprite2D>();

        CheckBox UpdateMask = GetNode<CheckBox>("Control/IsPhysicsOn");
        UpdateMask.Toggled += (tog) => UpdateMaskClear(tog);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if(GetNode<CheckBox>("Control/IsPhysicsOn").ButtonPressed)
        {
            BPPS.RunSim(delta);
        }

        if(GetNode<CheckBox>("Control/ShowUpdateMask").ButtonPressed)
        {
            UpdateMaskClear();
            
            foreach(pixel currentPixel in GetChildren().OfType<pixel>())
            {
                Texture2D debugTexture = GD.Load<Texture2D>("res://Textures/DebugRed.png");
                if(currentPixel.doUpdate)
                {
                    Sprite2D newMask = new Sprite2D() {Texture = debugTexture, Scale = new Vector2(2, 2), Position = currentPixel.Position};
                    updateMaskList.Add(newMask);
                    AddChild(newMask);
                }
            }
        }
    }

    public override void _Input(InputEvent @event)
    {
        // Mouse in viewport coordinates.
        if (@event is InputEventMouseButton eventMouseButton)
        {
            BPPS.HandlePixelTools();
        }
    }

    private void UpdateMaskClear(bool tog = false)
    {
        if(tog)
        {
            return;
        }
        
        foreach(Sprite2D mask in updateMaskList)
        {
            mask.QueueFree();
        }

        updateMaskList.Clear();
    }
}