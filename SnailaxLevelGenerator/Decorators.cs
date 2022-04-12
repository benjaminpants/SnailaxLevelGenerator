using System;
using System.Collections.Generic;
using System.Text;
using SnailaxDOTNET;

namespace SnailaxLevelGenerator
{

	public enum GeneratorTypes
	{
		Terrain,
		Decoration,
		PostDecoration
	}


	public class Generator
	{

		public virtual string Name
		{
			get
			{
				return "Example";
			}
		}

		public virtual GeneratorTypes Type
		{
			get
			{
				return GeneratorTypes.Terrain;
			}
		}

		public static void PlaceLine(SnailaxLevel lvl, string tile, int startx, int starty, int addx, int addy, int repeat)
		{
			for (int i = 0; i < repeat; i++)
			{
				lvl.PlaceTileAtGridPosition(startx + (addx * i), starty + (addy * i), tile);
			}
		}

		public static List<int[]> GetEmptyGridPositions(SnailaxLevel lvl, bool mt = true)
		{
			if (mt == false)
			{
				List<int[]> Positions = new List<int[]>();
				int max_room_x = (int)Math.Ceiling(lvl.RoomSizeX / 60f) - 1;
				int max_room_y = (int)Math.Ceiling(lvl.RoomSizeY / 60f) - 1;
				for (int i = 0; i < max_room_x; i++)
				{
					for (int j = 0; j < max_room_y; j++)
					{
						if (lvl.GetTileAtGridPosition(i, j) == null)
						{
							Positions.Add(new int[] { i, j });
						}
					}
				}
				return Positions;
			} else
            {
				return Multithreading.findEmptySpacesMultiTreaded(lvl);
            }
			
		}

		public static int[] MoveUntilCollision(SnailaxLevel lvl, int[] int_to_change, int movex, int movey, string filter = "")
		{
			bool has_hit_bottom = false;
			int max_room_y = (int)Math.Ceiling(lvl.RoomSizeY / 60f);
			while (has_hit_bottom == false)
			{
				if (lvl.GetTileAtGridPosition(int_to_change[0], int_to_change[1], filter) != null || int_to_change[1] > max_room_y)
				{
					int_to_change[0] -= movex;
					int_to_change[1] -= movey;
					has_hit_bottom = true;
				}
				else
				{
					int_to_change[0] += movex; //go back one because we are now in a wall
					int_to_change[1] += movey;
				}
			}
			return int_to_change;
		}

		public static void CreateFilledBox(SnailaxLevel lvl, string tile, int startx, int starty, int sizex, int sizey)
		{
			for (int i = 0; i < sizey; i++)
			{
				PlaceLine(lvl,tile,startx,starty + i,1,0,sizex);
			}
		}

		public virtual bool GenerateTiles(SnailaxLevel level, Random rng)
		{
			return false;
		}
	}
}
