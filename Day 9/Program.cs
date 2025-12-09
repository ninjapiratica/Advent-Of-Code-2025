using System.Data;

var lines = await File.ReadAllLinesAsync("input.txt");

var tiles = lines.Select(l => new Tile(l.Split(',').Select(long.Parse))).ToList();
List<Connection> vConnections = new();
List<Connection> hConnections = new();
var lastTile = tiles.Last();
for (int i = 0; i < tiles.Count; i++)
{
    var connection = new Connection(tiles[i], lastTile);
    lastTile = tiles[i];

    if (connection.A.X == connection.B.X)
        vConnections.Add(connection);
    else
        hConnections.Add(connection);
}

var maxArea = 0L;
for (var i = 0; i < tiles.Count; i++)
{
    var tileA = tiles[i];
    for (var j = i + 1; j < tiles.Count; j++)
    {
        var tileB = tiles[j];

        var area = (Math.Abs(tileA.X - tileB.X) + 1) * (Math.Abs(tileA.Y - tileB.Y) + 1);
        if (area > maxArea)
        {
            var minX = Math.Min(tileA.X, tileB.X);
            var maxX = Math.Max(tileA.X, tileB.X);
            var minY = Math.Min(tileA.Y, tileB.Y);
            var maxY = Math.Max(tileA.Y, tileB.Y);

            if (tiles.Any(t => t.X > minX && t.X < maxX && t.Y > minY && t.Y < maxY))
            {
                continue;
            }

            if (vConnections.Any(c => c.A.X > minX && c.A.X < maxX && c.MinY < maxY && c.MaxY > minY))
                continue;

            if (hConnections.Any(c => c.A.Y > minY && c.A.Y < maxY && c.MinX < maxX && c.MaxX > minX))
                continue;

            maxArea = area;
        }
    }
}

Console.WriteLine(maxArea);

class Connection
{
    public Tile A { get; set; }
    public Tile B { get; set; }

    public long MinX { get; set; }
    public long MinY { get; set; }
    public long MaxX { get; set; }
    public long MaxY { get; set; }

    public Connection(Tile a, Tile b)
    {
        A = a;
        B = b;

        MinX = Math.Min(a.X, b.X);
        MinY = Math.Min(a.Y, b.Y);
        MaxX = Math.Max(a.X, b.X);
        MaxY = Math.Max(a.Y, b.Y);
    }
}

class Tile
{
    public long X { get; set; }
    public long Y { get; set; }
    public Tile(IEnumerable<long> points)
    {
        X = points.First();
        Y = points.Last();
    }
}