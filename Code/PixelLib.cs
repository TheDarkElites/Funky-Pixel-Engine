using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class PixelLib
{	
	//Converts normal vec2 position to Pixel grid
	public Vector2 GetPixelCoordinates(Vector2 cords)
	{
		return new Vector2(GLOB.PIXSIZE*((int)(cords.X+GLOB.PIXSIZE-1)/GLOB.PIXSIZE), GLOB.PIXSIZE*((int)(cords.Y+GLOB.PIXSIZE-1)/GLOB.PIXSIZE));
	}
	//Moves a Pixel
	public void MovePixel(Vector2 location, Pixel targPix, ref Dictionary<Vector2, Pixel> pixelDict, bool exchange = true)
	{	
		if(!PixelCanMoveTo(location, ref pixelDict, targPix))
		{
			GD.PushError(String.Format("{0} failed to be moved to {1}!",targPix, location));
			return;
		}
		location = GetPixelCoordinates(location);
		
		Pixel exchangePixel = null;
		
		if(exchange && pixelDict.ContainsKey(location))
		{
			exchangePixel = pixelDict[location];
			pixelDict.Remove(location);
			exchangePixel.Position = targPix.Position;
		}
		
		pixelDict.Remove(targPix.Position);
		targPix.Position = location;
		pixelDict.Add(targPix.Position, targPix);

		if(exchange && exchangePixel != null)
		{
			pixelDict.Add(exchangePixel.Position, exchangePixel);
		}

		foreach(Pixel neighbor in GetPixelsInRange(targPix.Position, typeof(Pixel), ref pixelDict, radius: GLOB.PIXELUPDATERADIUS, includeSelf: true, includeCorners: true))
		{
			neighbor.doUpdate = true;
		}
	}

	//Deletes a Pixel
	public void DeletePixel(Pixel targPix, ref Dictionary<Vector2, Pixel> pixelDict)
	{
		targPix.Position = GetPixelCoordinates(targPix.Position);

		foreach(Pixel neighbor in GetPixelsInRange(targPix.Position, typeof(Pixel), ref pixelDict, radius: GLOB.PIXELUPDATERADIUS, includeSelf: true, includeCorners: true))
		{
			neighbor.doUpdate = true;
		}

		pixelDict.Remove(targPix.Position);
		targPix.QueueFree();
	}

	//Checks if a Pixel can move to a specific position
	public bool PixelCanMoveTo(Vector2 location, ref Dictionary<Vector2, Pixel> pixelDict)
	{
		location = GetPixelCoordinates(location);
		
		if(pixelDict.ContainsKey(location))
		{
			return false;
		}
		return true;
	}

	public bool PixelCanMoveTo(Vector2 location, ref Dictionary<Vector2, Pixel> pixelDict, Pixel targPix)
	{
		location = GetPixelCoordinates(location);
		
		if(pixelDict.ContainsKey(location))
		{
			Pixel blockingPixel = pixelDict[location];
			
			if(blockingPixel.isStatic)
			{
				return false;
			}
			if(targPix is Solid && blockingPixel is Solid)
			{
				return false;
			}
			if(targPix is Liquid && blockingPixel is Solid)
			{
				return false;
			}
			if(targPix is Liquid && blockingPixel is Liquid)
			{
				Liquid targPixLiq = (Liquid)targPix;
				Liquid blockingPixelLiq = (Liquid)blockingPixel;
				
				if(targPixLiq.density <= blockingPixelLiq.density)
				{
					return false;
				}
			}
		}
		return true;
	}

	public Pixel CreatePixel(string pixIDPath, ref Dictionary<Vector2, Pixel> pixelDict)
	{
		if(!PixelCanMoveTo(Vector2.Zero, ref pixelDict))
		{
			//GD.PushError(String.Format("Pixel attempted to be created in an invalid position, {0}.",Vector2.Zero));
			return null;
		}
		Pixel newPix = (Pixel)GD.Load<PackedScene>(pixIDPath).Instantiate();
		pixelDict[newPix.Position] = newPix;

		foreach(Pixel neighbor in GetPixelsInRange(newPix.Position, typeof(Pixel), ref pixelDict, radius: GLOB.PIXELUPDATERADIUS, includeSelf: true, includeCorners: true))
		{
			neighbor.doUpdate = true;
		}

		return newPix;
	}

	public Pixel CreatePixel(string pixIDPath, Vector2 location, ref Dictionary<Vector2, Pixel> pixelDict)
	{
		if(!PixelCanMoveTo(location, ref pixelDict))
		{
			//GD.PushError(String.Format("Pixel attempted to be created in an invalid position, {0}.",location));
			return null;
		}
		location = GetPixelCoordinates(location);

		Pixel newPix = (Pixel)GD.Load<PackedScene>(pixIDPath).Instantiate();
		MovePixel(location, newPix, ref pixelDict);
		return newPix;
	}

	//Returns all Pixel neighbors of type T
    public Pixel[] GetPixelsInRange(Vector2 location, Type pixType, ref Dictionary<Vector2, Pixel> pixelDict, int radius = 1, bool includeCorners = false, bool includeSelf = false)
    {   
        List<Pixel> neighbors = new List<Pixel>();
        for(int x = -radius; x <= radius; x++)
        {
            for(int y = -radius; y <= radius; y++)
            {
                if(!includeCorners && x != 0 && y != 0)
                {
                    continue;
                }

				if(!includeSelf && x == 0 && y == 0)
				{
					continue;
				}

                Vector2 tempLoc = new Vector2(location.X + (x * GLOB.PIXSIZE),location.Y + (y * GLOB.PIXSIZE));
                if(pixelDict.ContainsKey(tempLoc))
                {
                    if(!GodotObject.IsInstanceValid(pixelDict[tempLoc]))
					{
						pixelDict.Remove(tempLoc);
						continue;
					}
					
					if(!((pixelDict[tempLoc].GetType() == pixType) || pixelDict[tempLoc].GetType().IsSubclassOf(pixType)))
                    {
                        continue;
                    }
                    neighbors.Add(pixelDict[tempLoc]);
                }
            }
        }
        return neighbors.ToArray();
    }
}
