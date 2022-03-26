using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using SnailaxDOTNET;
using System.Collections.Concurrent;

namespace SnailaxLevelGenerator
{
    public class Multithreading
    {
        public static List<int[]> findEmptySpacesMultiTreaded(SnailaxLevel lvl) {
			int max_room_y = (int)Math.Ceiling(lvl.RoomSizeY / 60f) - 1;
            int max_room_x = (int)Math.Ceiling(lvl.RoomSizeX / 60f) - 1;
            int maxNumber = max_room_x * max_room_y;
            int threadCount = Environment.ProcessorCount;
            int chunkSize = max_room_y / threadCount;
            List<int[]> emptySpaces = new List<int[]>(maxNumber);
            Thread[] threads = new Thread[threadCount];
            for (int i = 0; i < threadCount; i++) {
                int start = i * chunkSize;
                int end = (i + 1) * chunkSize;
                if (i == threadCount - 1) {
                    end = max_room_y;
                }
                threads[i] = new Thread(() => { emptySpaces.AddRange(findEmptySpaces(lvl, start, end));});
                threads[i].Start();
            }
            for (int i = 0; i < threadCount; i++) {
                threads[i].Join();
            }

            return emptySpaces;
        }

        public static List<int[]> findEmptySpaces(SnailaxLevel lvl, int start, int end) {
            int max_room_x = (int)Math.Ceiling(lvl.RoomSizeX / 60f) - 1;
            List<int[]> emptySpaces = new List<int[]>();
            for (int i = start; i < end; i++) {
                for (int j = 0; j < max_room_x; j++) {
                    if (lvl.GetTileAtGridPosition(i, j) == null) {
                        emptySpaces.Add(new int[] { i, j });
                    }
                }
            }
            
            return emptySpaces;
        }
    }
}
