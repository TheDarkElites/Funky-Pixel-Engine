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
	public void MovePixel(Vector2 location, Pixel targPix, bool exchange = true)
	{	
		if(!PixelCanMoveTo(location, targPix))
		{
			GD.PushError(String.Format("{0} failed to be moved to {1}!",targPix, location));
			return;
		}
		location = GetPixelCoordinates(location);
		
		Pixel exchangePixel = null;
		
		if(exchange && GMS.GM.pixelDict.ContainsKey(location))
		{
			exchangePixel = GMS.GM.pixelDict[location];
			GMS.GM.pixelDict.Remove(location);
			exchangePixel.Position = targPix.Position;
		}
		
		GMS.GM.pixelDict.Remove(targPix.Position);
		targPix.Position = location;
		GMS.GM.pixelDict.Add(targPix.Position, targPix);

		if(exchange && exchangePixel != null)
		{
			GMS.GM.pixelDict.Add(exchangePixel.Position, exchangePixel);
		}

		foreach(Pixel neighbor in GetPixelsInRange(targPix.Position, typeof(Pixel), radius: GLOB.PIXELUPDATERADIUS, includeSelf: true, includeCorners: true))
		{
			GMS.GM.queuePixelUpdate(neighbor);
		}
	}

	//Deletes a Pixel
	public void DeletePixel(Pixel targPix)
	{
		targPix.Position = GetPixelCoordinates(targPix.Position);

		foreach(Pixel neighbor in GetPixelsInRange(targPix.Position, typeof(Pixel), radius: GLOB.PIXELUPDATERADIUS, includeSelf: true, includeCorners: true))
		{
			GMS.GM.queuePixelUpdate(neighbor);
		}

		GMS.GM.pixelDict.Remove(targPix.Position);
		targPix.QueueFree();
	}

	//Checks if a Pixel can move to a specific position
	public bool PixelCanMoveTo(Vector2 location)
	{
		location = GetPixelCoordinates(location);
		
		if(GMS.GM.pixelDict.ContainsKey(location))
		{
			return false;
		}
		return true;
	}

	public bool PixelCanMoveTo(Vector2 location, Pixel targPix)
	{
		location = GetPixelCoordinates(location);
		
		if(GMS.GM.pixelDict.ContainsKey(location))
		{
			Pixel blockingPixel = GMS.GM.pixelDict[location];
			
			if(blockingPixel.isStatic)
			{
				return false;
			}
			if(targPix is Solid && blockingPixel is Solid)
			{
				return false;
			}
			if(targPix is not Solid && blockingPixel is Solid)
			{
				return false;
			}
			if(targPix is Liquid && blockingPixel is Liquid)
			{			
				if(targPix.density <= blockingPixel.density)
				{
					return false;
				}
			}
			if(targPix is Gas && blockingPixel is Gas)
			{
				if(targPix.density <= blockingPixel.density)
				{
					return false;
				}
			}
		}
		return true;
	}

	public Pixel CreatePixel(string pixIDPath)
	{
		if(!PixelCanMoveTo(Vector2.Zero))
		{
			//GD.PushError(String.Format("Pixel attempted to be created in an invalid position, {0}.",Vector2.Zero));
			return null;
		}
		Pixel newPix = (Pixel)GD.Load<PackedScene>(pixIDPath).Instantiate();
		GMS.GM.pixelDict[newPix.Position] = newPix;

		foreach(Pixel neighbor in GetPixelsInRange(newPix.Position, typeof(Pixel), radius: GLOB.PIXELUPDATERADIUS, includeSelf: true, includeCorners: true))
		{
			GMS.GM.queuePixelUpdate(neighbor);
		}

		return newPix;
	}

	public Pixel CreatePixel(string pixIDPath, Vector2 location)
	{
		if(!PixelCanMoveTo(location))
		{
			//GD.PushError(String.Format("Pixel attempted to be created in an invalid position, {0}.",location));
			return null;
		}
		location = GetPixelCoordinates(location);

		Pixel newPix = (Pixel)GD.Load<PackedScene>(pixIDPath).Instantiate();
		MovePixel(location, newPix);
		return newPix;
	}

	//Returns all Pixel neighbors of type T
    public Pixel[] GetPixelsInRange(Vector2 location, Type pixType, int radius = 1, bool includeCorners = false, bool includeSelf = false)
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
                if(GMS.GM.pixelDict.ContainsKey(tempLoc))
                {
                    if(!GodotObject.IsInstanceValid(GMS.GM.pixelDict[tempLoc]))
					{
						GMS.GM.pixelDict.Remove(tempLoc);
						continue;
					}
					
					if(!((GMS.GM.pixelDict[tempLoc].GetType() == pixType) || GMS.GM.pixelDict[tempLoc].GetType().IsSubclassOf(pixType)))
                    {
                        continue;
                    }
                    neighbors.Add(GMS.GM.pixelDict[tempLoc]);
                }
            }
        }
        return neighbors.ToArray();
    }
}
