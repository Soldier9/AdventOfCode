using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solvers.Year2020
{
    class Day20Solver : AbstractSolver
    {
        int GridSize = 0;
        int TileSize = 0;
        Dictionary<int, List<List<string>>> Tiles = new Dictionary<int, List<List<string>>>();
        Dictionary<(int, int), (int, int)> Grid;

        public override string Part1()
        {
            using (var input = File.OpenText(InputFile))
            {
                int num = 0;
                List<string> tile = new List<string>();

                while (!input.EndOfStream)
                {
                    string line = input.ReadLine();
                    if (line.StartsWith("Tile"))
                    {
                        num = int.Parse(line.Substring(5, 4));
                        tile = new List<string>();
                    }
                    else if (line.Length == 0)
                    {
                        Tiles.Add(num, GetPermutations(tile));
                    }
                    else
                    {
                        TileSize = line.Length;
                        tile.Add(line);
                    }
                }
                Tiles.Add(num, GetPermutations(tile));
            }

            GridSize = (int)Math.Sqrt(Tiles.Count);
            ulong result = 0;

            foreach (int num in Tiles.Keys)
            {
                for (int permutation = 0; permutation < Tiles[num].Count; permutation++)
                {
                    Grid = new Dictionary<(int, int), (int, int)>();
                    HashSet<int> usedTiles = new HashSet<int>();

                    Grid[(0, 0)] = (num, permutation);
                    usedTiles.Add(num);

                    Grid = FillGrid(Grid, usedTiles);
                    if (Grid != null)
                    {
                        result = (ulong)Grid[(0, 0)].Item1 * (ulong)Grid[(0, GridSize - 1)].Item1 * (ulong)Grid[(GridSize - 1, 0)].Item1 * (ulong)Grid[(GridSize - 1, GridSize - 1)].Item1;
                        break;
                    }
                }
                if (result != 0) break;
            }

            return result.ToString();
        }

        private Dictionary<(int, int), (int, int)> FillGrid(Dictionary<(int, int), (int, int)> grid, HashSet<int> usedTiles)
        {
            if (grid.Count == Tiles.Count) return grid;

            (int, int) nextPosition = (0, 0);
            for (int x = 0; x < GridSize; x++)
            {
                for (int y = 0; y < GridSize; y++)
                {
                    if (!grid.ContainsKey((x, y)))
                    {
                        nextPosition = (x, y);
                        break;
                    }
                }
                if (nextPosition != (0, 0)) break;
            }

            foreach (int num in Tiles.Keys.Where(k => !usedTiles.Contains(k)))
            {
                for (int permutation = 0; permutation < Tiles[num].Count; permutation++)
                {
                    if (TileFits(grid, nextPosition, num, permutation))
                    {
                        grid.Add(nextPosition, (num, permutation));
                        usedTiles.Add(num);
                        return FillGrid(grid, usedTiles);
                    }
                }
            }

            return null;
        }

        private bool TileFits(Dictionary<(int, int), (int, int)> grid, (int, int) pos, int tileNum, int permutation)
        {
            List<string> thisTile = Tiles[tileNum][permutation];
            (int, int) neighborPos;
            List<string> neighbor;

            // Left
            neighborPos = (pos.Item1 - 1, pos.Item2);
            if (grid.ContainsKey(neighborPos))
            {
                neighbor = Tiles[grid[neighborPos].Item1][grid[neighborPos].Item2];
                for (int y = 0; y < TileSize; y++)
                {
                    if (thisTile[y][0] != neighbor[y][TileSize - 1]) return false;
                }
            }

            // Above
            neighborPos = (pos.Item1, pos.Item2 - 1);
            if (grid.ContainsKey(neighborPos))
            {
                neighbor = Tiles[grid[neighborPos].Item1][grid[neighborPos].Item2];
                for (int x = 0; x < TileSize; x++)
                {
                    if (thisTile[0][x] != neighbor[TileSize - 1][x]) return false;
                }
            }

            return true;
        }

        private List<List<string>> GetPermutations(List<string> tile)
        {
            List<List<string>> tiles = new List<List<string>>();
            tiles.Add(tile);
            for (int i = 0; i < 3; i++)
            {
                tiles.Add(RotateTile(tiles[i]));
            }
            tiles.Add(FlipTile(tiles[0]));
            for (int i = 4; i < 7; i++)
            {
                tiles.Add(RotateTile(tiles[i]));
            }
            return tiles;
        }

        private List<string> RotateTile(List<string> tile)
        {
            List<char[]> rotated = new List<char[]>();
            for (int n = 0; n < TileSize; n++) rotated.Add(new char[TileSize]);

            for (int y = 0; y < TileSize; y++)
            {
                for (int x = 0; x < TileSize; x++)
                {
                    rotated[y][x] = tile[-x + TileSize - 1][y];
                }
            }

            return rotated.Select(l => new string(l)).ToList();
        }

        private List<string> FlipTile(List<string> tile)
        {
            List<string> flipped = new List<string>();
            for (int i = TileSize - 1; i > -1; i--) flipped.Add(tile[i]);
            return flipped;
        }


        public override string Part2()
        {
            int imageSize = GridSize * (TileSize - 2);
            List<List<string>> finalImages = new List<List<string>>();

            List<char[]> tmp = new List<char[]>();
            for (int n = 0; n < imageSize; n++) tmp.Add(new char[imageSize]);

            for (int x = 0; x < GridSize; x++)
            {
                for (int y = 0; y < GridSize; y++)
                {
                    List<string> tile = Tiles[Grid[(x, y)].Item1][Grid[(x, y)].Item2];

                    for (int x1 = 1; x1 < TileSize - 1; x1++)
                    {
                        int tmpX = x * (TileSize - 2) + x1 - 1;
                        for (int y1 = 1; y1 < TileSize - 1; y1++)
                        {
                            int tmpY = y * (TileSize - 2) + y1 - 1;
                            tmp[tmpY][tmpX] = tile[y1][x1];
                        }
                    }
                }
            }

            TileSize = imageSize; // Treat the image as a tile to get permutations
            finalImages = GetPermutations(tmp.Select(l => new string(l)).ToList());

            Regex[] seaMonster = new Regex[3];
            seaMonster[0] = new Regex("^..................#.");
            seaMonster[1] = new Regex("#....##....##....###");
            seaMonster[2] = new Regex("^.#..#..#..#..#..#...");

            foreach (List<string> image in finalImages)
            {
                int monstersFound = 0;
                for (int l = 1; l < image.Count - 1; l++)
                {
                    MatchCollection matches = seaMonster[1].Matches(image[l]);
                    foreach (Match match in matches)
                    {
                        if (seaMonster[2].IsMatch(image[l + 1].Substring(match.Index)) &&
                            seaMonster[0].IsMatch(image[l - 1].Substring(match.Index)))
                        {
                            monstersFound++;
                        }
                    }
                }
                if (monstersFound == 0) continue;

                int result = -monstersFound * 15;
                foreach (string line in image)
                {
                    result += line.Where(c => c == '#').Count();
                }
                return result.ToString();
            }

            return "No monsters found!";
        }
    }
}