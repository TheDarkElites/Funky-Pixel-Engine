using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

public partial class BasePixelPhysicsSystem : GodotObject
{
	private double last_physics_update;
	public Dictionary<Vector2, Pixel> pixelDict;	
    private Node2D world;

    private PixelLib pixelLib;

    private Vector2 lastMousePos;

    public BasePixelPhysicsSystem(Node2D world)
    {
        this.world = world;
        last_physics_update = 0;
        pixelDict = new Dictionary<Vector2, Pixel>();
        pixelLib = new PixelLib();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public void RunSim(double delta)
	{
		//Begin Pixel simulation
		
		last_physics_update += delta;
		if(last_physics_update < GLOB.PHYSSPEED)
		{
			return;
		}
		last_physics_update = 0;
		
		bool pixelsUpdated = false;

        Random rand = new Random(); 

        List<Pixel> pixelsToUpdate = new List<Pixel>();

		foreach (Pixel currentPixel in world.GetChildren().OfType<Pixel>()) 
		{
            bool pixelUpdated = false;
            
            //Meta Calulations
			if(currentPixel.Position.Y > 600 || currentPixel.Position.Y <= 0)
			{
				if(pixelsToUpdate.Contains(currentPixel))
                {
                    pixelsToUpdate.Remove(currentPixel);
                }
                pixelLib.DeletePixel(currentPixel, ref pixelDict);
				pixelsUpdated = true;
                pixelUpdated = true;
			}

            if(currentPixel.isStatic) //Dont need to update static pixels or pixels not scheduled to update
            {
                currentPixel.doUpdate = false; //We shouldent be updating static pixels.
				continue;
            }

            if(!currentPixel.doUpdate)
            {
                continue;
            }
			
            //Gravity Calculations
            //Amount of required connections to pixels to avoid us from falling
			int reqConnections = (int)Math.Ceiling(8 - Math.Clamp(currentPixel.ductility * 16, 0, 4));
            if(pixelLib.GetPixelsInRange(currentPixel.Position, currentPixel.GetType(), ref pixelDict, includeCorners: true).Length < reqConnections)
            {
                if(currentPixel is Solid || currentPixel is Liquid)
				{
					if(pixelLib.PixelCanMoveTo(new Vector2(currentPixel.Position.X, currentPixel.Position.Y + GLOB.PIXSIZE), ref pixelDict, currentPixel))
					{
						pixelLib.MovePixel(new Vector2(currentPixel.Position.X, currentPixel.Position.Y + GLOB.PIXSIZE), currentPixel, ref pixelDict);
						pixelsUpdated = true;
						pixelUpdated = true;
					} 
					else if(pixelLib.PixelCanMoveTo(new Vector2(currentPixel.Position.X + GLOB.PIXSIZE, currentPixel.Position.Y + GLOB.PIXSIZE), ref pixelDict, currentPixel) && (rand.Next(1,101) <= (100 * (1 - currentPixel.ductility))))
					{
						pixelLib.MovePixel(new Vector2(currentPixel.Position.X + GLOB.PIXSIZE, currentPixel.Position.Y + GLOB.PIXSIZE), currentPixel, ref pixelDict);
						pixelsUpdated = true;
						pixelUpdated = true;
					}
					else if(pixelLib.PixelCanMoveTo(new Vector2(currentPixel.Position.X - GLOB.PIXSIZE, currentPixel.Position.Y + GLOB.PIXSIZE), ref pixelDict, currentPixel) && (rand.Next(1,101) <= (100 * (1 - currentPixel.ductility))))
					{
						pixelLib.MovePixel(new Vector2(currentPixel.Position.X - GLOB.PIXSIZE, currentPixel.Position.Y + GLOB.PIXSIZE), currentPixel, ref pixelDict);
						pixelsUpdated = true;
						pixelUpdated = true;
					}
				}

				if(currentPixel is Liquid && !pixelUpdated)
				{
					if(pixelLib.PixelCanMoveTo(new Vector2(currentPixel.Position.X + GLOB.PIXSIZE, currentPixel.Position.Y), ref pixelDict, currentPixel))
					{
						pixelLib.MovePixel(new Vector2(currentPixel.Position.X + GLOB.PIXSIZE, currentPixel.Position.Y), currentPixel, ref pixelDict);
						pixelsUpdated = true;
						pixelUpdated = true;
					}
					else if(pixelLib.PixelCanMoveTo(new Vector2(currentPixel.Position.X - GLOB.PIXSIZE, currentPixel.Position.Y), ref pixelDict, currentPixel))
					{
						pixelLib.MovePixel(new Vector2(currentPixel.Position.X - GLOB.PIXSIZE, currentPixel.Position.Y), currentPixel, ref pixelDict);
						pixelsUpdated = true;
						pixelUpdated = true;
					}
				}

				if(currentPixel is Gas)
				{
					if(pixelLib.PixelCanMoveTo(new Vector2(currentPixel.Position.X, currentPixel.Position.Y - GLOB.PIXSIZE), ref pixelDict, currentPixel))
					{
						pixelLib.MovePixel(new Vector2(currentPixel.Position.X, currentPixel.Position.Y - GLOB.PIXSIZE), currentPixel, ref pixelDict);
						pixelsUpdated = true;
						pixelUpdated = true;
					} 
					else if(pixelLib.PixelCanMoveTo(new Vector2(currentPixel.Position.X + GLOB.PIXSIZE, currentPixel.Position.Y - GLOB.PIXSIZE), ref pixelDict, currentPixel) && (rand.Next(1,101) <= (100 * (1 - currentPixel.ductility))))
					{
						pixelLib.MovePixel(new Vector2(currentPixel.Position.X + GLOB.PIXSIZE, currentPixel.Position.Y - GLOB.PIXSIZE), currentPixel, ref pixelDict);
						pixelsUpdated = true;
						pixelUpdated = true;
					}
					else if(pixelLib.PixelCanMoveTo(new Vector2(currentPixel.Position.X - GLOB.PIXSIZE, currentPixel.Position.Y - GLOB.PIXSIZE), ref pixelDict, currentPixel) && (rand.Next(1,101) <= (100 * (1 - currentPixel.ductility))))
					{
						pixelLib.MovePixel(new Vector2(currentPixel.Position.X - GLOB.PIXSIZE, currentPixel.Position.Y - GLOB.PIXSIZE), currentPixel, ref pixelDict);
						pixelsUpdated = true;
						pixelUpdated = true;
					}
					else if(pixelLib.PixelCanMoveTo(new Vector2(currentPixel.Position.X + GLOB.PIXSIZE, currentPixel.Position.Y), ref pixelDict, currentPixel))
					{
						pixelLib.MovePixel(new Vector2(currentPixel.Position.X + GLOB.PIXSIZE, currentPixel.Position.Y), currentPixel, ref pixelDict);
						pixelsUpdated = true;
						pixelUpdated = true;
					}
					else if(pixelLib.PixelCanMoveTo(new Vector2(currentPixel.Position.X - GLOB.PIXSIZE, currentPixel.Position.Y), ref pixelDict, currentPixel))
					{
						pixelLib.MovePixel(new Vector2(currentPixel.Position.X - GLOB.PIXSIZE, currentPixel.Position.Y), currentPixel, ref pixelDict);
						pixelsUpdated = true;
						pixelUpdated = true;
					}
				}

            }

            //Handling for only updating neccesary pixels
            if(pixelUpdated)
            {
                pixelsToUpdate.Add(currentPixel);
            }
            else
            {
                currentPixel.doUpdate = false;
            }
		}

		SanatizePixelDict();

        foreach(Pixel updateTarget in pixelsToUpdate)
        {   
            Pixel[] pixNeighbors = pixelLib.GetPixelsInRange(updateTarget.Position, typeof(Pixel), ref pixelDict, radius: GLOB.PIXELUPDATERADIUS, includeCorners: true, includeSelf: true);
            foreach(Pixel neighbor in pixNeighbors)
		    {   
                neighbor.doUpdate = true;
		    }
        }

		if(pixelsUpdated || (world.GetViewport().GetMousePosition() != lastMousePos))
		{
			HandlePixelTools();
            lastMousePos = world.GetViewport().GetMousePosition();
		}
		
	}

	//Sanatizes our Pixel dict for invalid values
	private void SanatizePixelDict()
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

	/// <summary>
	///	The remainder of this code is for handeling our debug interface of spawning pixels on screen, etc.
	///	To be moved to its own file.
	/// </summary>

	public void HandlePixelTools()
	{
		//The following handles placing pixels
		if((Input.IsMouseButtonPressed(MouseButton.Left) || Input.IsMouseButtonPressed(MouseButton.Right)) && !Input.IsKeyPressed(Key.Ctrl))
		{	
			string pixelID;
			switch(world.GetNode<OptionButton>("Control/PixelSelector").Selected)
			{
				case 0:
					pixelID = "res://Pixels/dirt.tscn";
					break;
				case 1:
					pixelID = "res://Pixels/s_pixel.tscn";
					break;
                case 2:
                    pixelID = "res://Pixels/sand.tscn";
					break;
				case 3:
					pixelID = "res://Pixels/water.tscn";
					break;
				case 4:
					pixelID = "res://Pixels/steam.tscn";
					break;
				default:
					pixelID = "res://Pixels/s_pixel.tscn";
					break;
			}
			
			Vector2 mousePosition = world.GetViewport().GetMousePosition();
			int blobSize = Int32.Parse(world.GetNode<LineEdit>("Control/SizeSetter").Text);
			
			switch(blobSize)
			{
				case 0:
					break;
				case 1:
					if(Input.IsMouseButtonPressed(MouseButton.Left))
					{
						Pixel newPix = pixelLib.CreatePixel(pixelID, mousePosition, ref pixelDict);
						if(newPix == null)
						{
							GD.PushError(String.Format("Newly created Pixel was null!"));
							break;
						}
						world.AddChild(newPix);
					}
					else if(Input.IsMouseButtonPressed(MouseButton.Right))
					{
                        Vector2 delPos = pixelLib.GetPixelCoordinates(mousePosition);
                        if(pixelDict.ContainsKey(delPos))
                            {
                                pixelLib.DeletePixel(pixelDict[delPos], ref pixelDict);
                            }
					}
					break;
				default:
					for(int x = -blobSize/2; x < blobSize/2; x++)
					{
						for(int y = -blobSize/2; y < blobSize/2; y++)
						{
							if(Input.IsMouseButtonPressed(MouseButton.Left))
							{
								Pixel tempPix = pixelLib.CreatePixel(pixelID, new Vector2(mousePosition.X + (x * GLOB.PIXSIZE), mousePosition.Y + (y * GLOB.PIXSIZE)), ref pixelDict);
								if(tempPix == null)
								{
									//GD.PushError(String.Format("Newly created Pixel was null!"));
									continue;
								}
								world.AddChild(tempPix);
							}
							else if(Input.IsMouseButtonPressed(MouseButton.Right))
							{
								Vector2 delPos = pixelLib.GetPixelCoordinates(new Vector2(mousePosition.X + (x * GLOB.PIXSIZE), mousePosition.Y + (y * GLOB.PIXSIZE)));
								if(pixelDict.ContainsKey(delPos))
									{
										pixelLib.DeletePixel(pixelDict[delPos], ref pixelDict);
									}
							}
						}
					}
					break;
			}
		}
	}
}


