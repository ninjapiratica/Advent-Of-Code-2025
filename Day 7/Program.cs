var lines = await File.ReadAllLinesAsync("input.txt");

var beamEnds = new List<BeamEnd>() { new BeamEnd(lines[0].IndexOf('S'), 0) };
var splitCount = 0;

for (var i = 0; i < beamEnds.Count; i++)
{
    var beamEnd = beamEnds[i];
    while (lines.Length > beamEnd.Y + 1)
    {
        var nextLine = lines[beamEnd.Y + 1];
        if (nextLine[beamEnd.X] == '.')
        {
            nextLine = nextLine[..beamEnd.X] + '|' + nextLine[(beamEnd.X + 1)..];
            lines[beamEnd.Y + 1] = nextLine;
            beamEnd.Y++;
        }
        else if (nextLine[beamEnd.X] == '|')
        {
            break;
        }
        else if (nextLine[beamEnd.X] == '^')
        {
            if (beamEnd.X > 0)
            {
                var leftBeamEnd = new BeamEnd(beamEnd.X - 1, beamEnd.Y + 1);
                if (nextLine[leftBeamEnd.X] == '.')
                {
                    beamEnds.Add(leftBeamEnd);
                    nextLine = nextLine[..(beamEnd.X - 1)] + '|' + nextLine[beamEnd.X..];
                }
            }

            if (beamEnd.X < nextLine.Length - 1)
            {
                var rightBeamEnd = new BeamEnd(beamEnd.X + 1, beamEnd.Y + 1);
                if (nextLine[rightBeamEnd.X] == '.')
                {
                    beamEnds.Add(rightBeamEnd);
                    nextLine = nextLine[..(beamEnd.X + 1)] + '|' + nextLine[(beamEnd.X + 2)..];
                }
            }

            lines[beamEnd.Y + 1] = nextLine;
            splitCount++;
            break;
        }
    }
}

foreach (var line in lines)
{
    Console.WriteLine(line);
}

Console.WriteLine(splitCount);

var splitters = new List<Splitter>();
for (var i = lines.Length - 1; i >= 0; i--)
{
    var line = lines[i];
    for (var j = 0; j < line.Length; j++)
    {
        if (line[j] == '^')
        {
            var lookAheadLeft = i + 1;
            Splitter? left = null;
            while (lookAheadLeft < lines.Length)
            {
                if (lines[lookAheadLeft][j - 1] == '^')
                {
                    left = splitters.First(s => s.X == j - 1 && s.Y == lookAheadLeft);
                    break;
                }
                else
                {
                    lookAheadLeft++;
                }
            }

            var lookAheadRight = i + 1;
            Splitter? right = null;
            while (lookAheadRight < lines.Length)
            {
                if (lines[lookAheadRight][j + 1] == '^')
                {
                    right = splitters.First(s => s.X == j + 1 && s.Y == lookAheadRight);
                    break;
                }
                else
                {
                    lookAheadRight++;
                }
            }

            var s = new Splitter(j, i, (left?.Total ?? 1) + (right?.Total ?? 1));
            s.Left = left;
            s.Right = right;
            splitters.Add(s);
        }
    }
}

var indexOfS = lines[0].IndexOf('S');
var dimensions = 1L;
for (int i = 1; i < lines.Length; i++)
{
    if (lines[i][indexOfS] == '^')
    {
        dimensions = splitters.First(s => s.X == indexOfS && s.Y == i).Total;
        break;
    }
}

foreach (var splitter in splitters.OrderBy(s => s.Y))
{
    Console.WriteLine($"{splitter.Y} - {splitter.X} - {splitter.Total} - LEFT: {splitter.Left?.Y},{splitter.Left?.X} - RIGHT: {splitter.Right?.Y},{splitter.Right?.X} ");
}

Console.WriteLine(dimensions);

// > 1870027164

class BeamEnd
{
    public BeamEnd(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }
    public int X { get; set; }
    public int Y { get; set; }
}

class Splitter
{
    public Splitter(int x, int y, long total)
    {
        X = x;
        Y = y;
        Total = total;
    }

    public Splitter? Left { get; set; }
    public Splitter? Right { get; set; }

    public int X { get; set; }
    public int Y { get; set; }
    public long Total { get ; set; }
}