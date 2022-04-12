using System;
using System.Collections.Generic;
using System.Text;
using SnailaxDOTNET;

namespace SnailaxLevelGenerator
{
	public class GiantBox : Generator
	{
		public override string Name
		{
			get
			{
				return "GiantBox";
			}
		}

		public override bool GenerateTiles(SnailaxLevel level, Random rng)
		{
			int max_room_x = (int)Math.Ceiling(level.RoomSizeX / 60f);
			int max_room_y = (int)Math.Ceiling(level.RoomSizeY / 60f);
			PlaceLine(level, "obj_wall", 0, 0, 1, 0, max_room_x);
			PlaceLine(level, "obj_wall", 0, max_room_y - 1, 1, 0, max_room_x);
			PlaceLine(level, "obj_wall", 0, 0, 0, 1, max_room_y);
			PlaceLine(level, "obj_wall", max_room_x - 1, 0, 0, 1, max_room_y);
			level.PlaceTileAtGridPosition(2, max_room_y - 3, "obj_playerspawn");
			level.Tiles.Remove(level.GetTileAtGridPosition(max_room_x - 1, max_room_y - 2));
			level.Tiles.Remove(level.GetTileAtGridPosition(max_room_x - 1, max_room_y - 3));
			return true;
		}
	}

	public class BlockedTerrain : GiantBox
	{
		public override bool GenerateTiles(SnailaxLevel level, Random rng)
		{
			base.GenerateTiles(level,rng);

			int max_room_x = (int)Math.Ceiling(level.RoomSizeX / 60f);
			int max_room_y = (int)Math.Ceiling(level.RoomSizeY / 60f) - 1;
			int base_val = rng.Next(7, 11);
			for (int i = 0; i < (base_val * level.RoomMultiplierX); i++)
			{
				int height = rng.Next(1, 7);
				CreateFilledBox(level, "obj_wall", rng.Next(5, max_room_x - 4), max_room_y - (height), rng.Next(1, 4), height);
			}

			base_val = rng.Next(9, 12);
			for (int i = 0; i < (base_val * level.RoomMultiplierX); i++)
			{
				int height = rng.Next(1, 7);
				CreateFilledBox(level, "obj_wall", rng.Next(5, max_room_x - 4), 1, rng.Next(2, 5), height);
			}

			return true;
		}
	}

	public class TrialDecorator : Generator
	{
		public override string Name
		{
			get
			{
				return "TrialDecorator";
			}
		}

		public override GeneratorTypes Type
		{
			get
			{
				return GeneratorTypes.Decoration;
			}
		}

		public void PlaceRowOfTilesOntoFloor(SnailaxLevel level, int amount, string type, string filter, int[] position_chosen, int ydir = 1)
		{


			int[] original_pos = position_chosen;


			for (int j = 0; j < amount; j++)
			{
				position_chosen = MoveUntilCollision(level, new int[] { original_pos[0] + j, original_pos[1] }, 0, ydir, filter);

				if (level.GetTileAtGridPosition(position_chosen[0], position_chosen[1]) == null)
				{
					level.PlaceTileAtGridPosition(position_chosen[0], position_chosen[1], type);
				}
			}

		}

		public override bool GenerateTiles(SnailaxLevel level, Random rng, bool mt)
		{

			bool has_guns = false;

			List<int[]> possibletiles = GetEmptyGridPositions(level, mt);

			int floating_platforms = rng.Next(0, (int)(2 * level.RoomMultiplierX));

			for (int i = 0; i < floating_platforms; i++)
			{
				int chosen_index = rng.Next(0, possibletiles.Count - 1);
				int[] position_chosen = possibletiles[chosen_index];

				int maxtop_pos = MoveUntilCollision(level, possibletiles[chosen_index], 0, -1)[1];
				int maxbot_pos = MoveUntilCollision(level, possibletiles[chosen_index], 0, 1)[1];
				int off;

				if (maxtop_pos < maxbot_pos)
					off = rng.Next(maxtop_pos, maxbot_pos);
                		else
					 off = rng.Next(maxbot_pos, maxtop_pos);

				level.PlaceTileAtGridPosition(position_chosen[0], off, "obj_wall");

				level.PlaceTileAtGridPosition(position_chosen[0] + 1, off, "obj_wall");

				level.PlaceTileAtGridPosition(position_chosen[0] - 1, off, "obj_wall");
			}

			possibletiles = GetEmptyGridPositions(level); //refresh empty tiles

			int ceiling_spike_amount = (int)(rng.Next(5, 15) * level.RoomMultiplierX);

			for (int i = 0; i < ceiling_spike_amount; i++)
			{
				int chosen_index = rng.Next(0, possibletiles.Count - 1);
				int[] position_chosen = possibletiles[chosen_index];

				position_chosen = MoveUntilCollision(level,position_chosen,0,-1,"obj_wall");

				level.PlaceTileAtGridPosition(position_chosen[0], position_chosen[1], "obj_spike_permanent");

				possibletiles.RemoveAt(chosen_index);
			}

			int floor_spike_amount = (int)(rng.Next(3, 7) * level.RoomMultiplierX);

			for (int i = 0; i < floor_spike_amount; i++)
			{
				int chosen_index = rng.Next(0, possibletiles.Count - 1);
				int[] position_chosen = possibletiles[chosen_index];

				PlaceRowOfTilesOntoFloor(level, rng.Next(1, 4), "obj_spike_permanent", "obj_wall", position_chosen);

				possibletiles.RemoveAt(chosen_index);
			}

			possibletiles = GetEmptyGridPositions(level); //refresh empty tiles

			int conveyer_amount = (int)(rng.Next(0, 3) * level.RoomMultiplierX);

			for (int i = 0; i < conveyer_amount; i++)
			{
				int chosen_index = rng.Next(0, possibletiles.Count - 1);
				int[] position_chosen = possibletiles[chosen_index];

				int length = rng.Next(3, 7);

				PlaceRowOfTilesOntoFloor(level, length, "obj_conveyor_belt", "obj_wall", position_chosen);

				if (rng.Next(0,100) <= 40)
				{
					int maxtop_pos = MoveUntilCollision(level, possibletiles[chosen_index], 0, -1)[1];
					int maxbot_pos = MoveUntilCollision(level, possibletiles[chosen_index], 0, 1)[1];
					level.PlaceTileAtGridPosition(position_chosen[0] + rng.Next(0, length), (int)(((float)maxtop_pos + (float)maxbot_pos) / 2f) + rng.Next(-1,1), "obj_bomb_spawner");
				}

				possibletiles.RemoveAt(chosen_index);
			}

			possibletiles = GetEmptyGridPositions(level); //refresh empty tiles

			if (has_guns)
			{
				int gun_amount = rng.Next(1, (int)level.RoomMultiplierX);

				int earliest_up_gun = int.MaxValue;

				int earliest_dwn_gun = int.MaxValue;

				string[] potential_guns = { "obj_sh_gun", "obj_sh_gun2", "obj_sh_gun3" };

				for (int i = 0; i < gun_amount; i++)
				{
					int chosen_index = rng.Next(0, possibletiles.Count - 1);
					int[] position_chosen = possibletiles[chosen_index];

					position_chosen = MoveUntilCollision(level, position_chosen, 0, 1, "obj_wall");

					int selgun = rng.Next(0, 2);

					if (selgun == 0)
					{
						if (earliest_up_gun > position_chosen[0])
						{
							earliest_up_gun = position_chosen[0];
						}
					}

					if (selgun == 2)
					{
						earliest_dwn_gun = position_chosen[0];
					}

					level.PlaceTileAtGridPosition(position_chosen[0], position_chosen[1], potential_guns[selgun]);

					possibletiles.RemoveAt(chosen_index);
				}

				if (earliest_up_gun != int.MaxValue)
				{
					int ceiling_tomfoolery = (int)(rng.Next(1, 2) * level.RoomMultiplierX);
					for (int i = 0; i < ceiling_tomfoolery; i++)
					{
						int chosen_index = rng.Next(0, possibletiles.Count - 1);
						int[] position_chosen = possibletiles[chosen_index];

						PlaceRowOfTilesOntoFloor(level, rng.Next(2, 6), "obj_destructable_wall", "obj_wall", position_chosen, -1);

						possibletiles.RemoveAt(chosen_index);
					}
				}

				if (earliest_dwn_gun != int.MaxValue)
				{
					int ceiling_tomfoolery = (int)(rng.Next(1, 2) * level.RoomMultiplierX);
					for (int i = 0; i < ceiling_tomfoolery; i++)
					{
						int chosen_index = rng.Next(0, possibletiles.Count - 1);
						int[] position_chosen = possibletiles[chosen_index];

						PlaceRowOfTilesOntoFloor(level, rng.Next(1, 3), "obj_destructable_wall", "obj_wall", position_chosen);

						possibletiles.RemoveAt(chosen_index);
					}
				}
			}


			int squasher_amount = rng.Next(0, (int)(3 * level.RoomMultiplierX));

			for (int i = 0; i < squasher_amount; i++)
			{
				int chosen_index = rng.Next(0, possibletiles.Count - 1);
				int[] position_chosen = possibletiles[chosen_index];

				int maxtop_pos = MoveUntilCollision(level, possibletiles[chosen_index], 0, -1)[1];
				int maxbot_pos = MoveUntilCollision(level, possibletiles[chosen_index], 0, 1)[1];

				Tile til;

				if (maxtop_pos < maxbot_pos)
					til = level.PlaceTileAtGridPosition(position_chosen[0], rng.Next(maxtop_pos, maxbot_pos), "obj_squasher");
				else
					til = level.PlaceTileAtGridPosition(position_chosen[0], rng.Next(maxbot_pos, maxtop_pos), "obj_squasher");

				til.rotation = rng.Next(0,3) * 90;

				possibletiles.RemoveAt(chosen_index);
			}

			possibletiles = GetEmptyGridPositions(level); //refresh empty tiles

			int ice_ceiling_spike_amount = rng.Next(0, (int)(1 * level.RoomMultiplierX));

			for (int i = 0; i < ice_ceiling_spike_amount; i++)
			{
				int chosen_index = rng.Next(0, possibletiles.Count - 1);
				int[] position_chosen = possibletiles[chosen_index];

				position_chosen = MoveUntilCollision(level, position_chosen, 0, -1, "obj_wall");

				PlaceRowOfTilesOntoFloor(level, rng.Next(3, 8), "obj_ice_spike", "obj_wall", position_chosen, -1);

				possibletiles.RemoveAt(chosen_index);
			}


			return true;
		}

	}


	public class SanityChecker : Generator
	{
		public override string Name
		{
			get
			{
				return "SanityChecker";
			}
		}

		public override GeneratorTypes Type
		{
			get
			{
				return GeneratorTypes.PostDecoration;
			}
		}

		public override bool GenerateTiles(SnailaxLevel level, Random rng)
		{
			for (int i = 0; i < level.Tiles.Count; i++)
			{
				Tile til = level.Tiles[i];
				if (til.Object == "obj_spike_permanent")
				{
					int[] pos = til.GetGridPosition();

					if (level.GetTileAtGridPosition(pos[0] - 1, pos[1] + 1, "obj_wall") == null && level.GetTileAtGridPosition(pos[0] + 1, pos[1] + 1, "obj_wall") != null)
					{
						level.Tiles.RemoveAt(i);
					}
				}
				if (til.Object == "obj_playerspawn")
				{
					int[] pos = { 0, 0 };
					pos = MoveUntilCollision(level, pos, 0, 1, "obj_wall");
					Tile spiketile = level.GetTileAtGridPosition(pos[0], pos[1], "obj_spike_permanent");
					if (spiketile != null)
					{
						level.Tiles.Remove(spiketile);
					}
				}
			}
			return true;
		}
	}

}
