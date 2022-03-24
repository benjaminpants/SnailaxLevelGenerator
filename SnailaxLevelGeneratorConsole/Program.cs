using System;
using System.IO;
using System.Collections.Generic;
using SnailaxDOTNET;
using SnailaxLevelGenerator;

namespace SnailaxLevelGeneratorConsole
{
	class Program
	{
		public static List<Generator> GeneralGenerators = new List<Generator>() { new BlockedTerrain(), new GiantBox() };

		static void Main(string[] args)
		{
			SnailaxLevel sn = new SnailaxLevel("testlevel");

			int seed = new Random().Next();

			Console.WriteLine("This is very, very WIP. Type in how long you want your map to be in screens:");
			sn.RoomMultiplierX = float.Parse(Console.ReadLine());
			Console.WriteLine("Type in how tall you want your map to be in screens(NOTE THAT NONE OF THE CURRENT GENERATORS PROPERLY HANDLE HEIGHT):");
			sn.RoomMultiplierY = float.Parse(Console.ReadLine());
			Console.WriteLine("Do you wish to enter a seed? A random one will be used by default(y/n)");
			if (Console.ReadKey().Key == ConsoleKey.Y)
			{
				Console.WriteLine("Please enter seed:");
				seed = int.Parse(Console.ReadLine());
			}
			Console.WriteLine("Choose a terrain generator:\n0 = Blocked Terrain(Recommended)\n1 = Superflat");
			Generator gen = GeneralGenerators[int.Parse(Console.ReadLine())];
			Random rng = new Random();
			sn.Theme = (Themes)rng.Next(0, 5);
			if (sn.Theme == Themes.Brain)
			{
				sn.Song = Songs.BrainAmbience;
			}
			else
			{
				sn.Song = (Songs)rng.Next(0, 21);
			}
			Console.WriteLine("Generating Terrain!");
			gen.GenerateTiles(sn, rng);
			Console.WriteLine("Choose a decorator:\n-1 = NONE\n0 = YOLO(Recommended)");
			int option = int.Parse(Console.ReadLine());
			if (option != -1)
			{
				Console.WriteLine("Decorating...!");
				new TrialDecorator().GenerateTiles(sn, rng);
			}
			Console.WriteLine("Attempting to perform sanity check... (this is broken right now)");
			new SanityChecker().GenerateTiles(sn, rng);
			sn.SquidProperties.GroundSpikeProbability = rng.Next(0, 8);
			sn.SquidProperties.WallSpikeProbablity = rng.Next(0, 8);
			sn.SquidProperties.CeilingSpikeProbability = rng.Next(0, 8);
			sn.SquidProperties.ConveyerBeltChangeProbability = rng.Next(1, 4);
			sn.SquidProperties.IceSpikeFallProbability = rng.Next(1, 12);
			if (rng.Next(0, 100) <= 22)
			{
				sn.SquidProperties.AirSpikeProbability = rng.Next(0, 6);
			}
			Console.WriteLine("Done!\n File outputed to:\n" + Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output.wysld"));
			File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output.wysld"), sn.Serialize());
		}
	}
}
