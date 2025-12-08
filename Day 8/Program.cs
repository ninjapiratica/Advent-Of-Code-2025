var lines = await File.ReadAllLinesAsync("input.txt");

var junctionBoxes = lines.Select(l => new JunctionBox(l.Split(',').Select(int.Parse).ToArray())).ToList();

var possibleConnections = new List<Connection>();
for (var i = 0; i < lines.Length; i++)
{
    var junctionBox1 = junctionBoxes[i];
    for (var j = i + 1; j < lines.Length; j++)
    {
        var junctionBox2 = junctionBoxes[j];
        possibleConnections.Add(new Connection(junctionBox1, junctionBox2));
    }
}

var connections = possibleConnections
    .OrderBy(x => x.Distance)
    // .Take(1000)
    .ToList();

var circuits = new List<Circuit>();
Connection? finalConnection = null;
foreach (var pc in connections)
{
    List<Circuit?> existingCircuits = [pc.A.Circuit, pc.B.Circuit];
    existingCircuits = existingCircuits.Where(x => x != null).ToList();

    if (existingCircuits.Count > 0)
    {
        var newCircuit = existingCircuits[0]!;
        newCircuit.Add(pc);

        if (existingCircuits.Count > 1 && existingCircuits[0] != existingCircuits[1])
        {
            var oldCircuit = existingCircuits[1]!;
            newCircuit.Add(oldCircuit);
            circuits.Remove(oldCircuit);
        }
    }
    else
    {
        var newCircuit = new Circuit();
        circuits.Add(newCircuit);
        newCircuit.Add(pc);
    }

    if (circuits.Count == 1 && circuits[0].JunctionBoxes.Count == lines.Length)
    {
        Console.WriteLine($"Found at: {pc.A} - {pc.B}");
        finalConnection = pc;
        break;
    }
}

if (finalConnection != null)
    Console.WriteLine(finalConnection.A.X * finalConnection.B.X);

class Circuit
{
    public HashSet<JunctionBox> JunctionBoxes = new();

    public void Add(Connection c)
    {
        JunctionBoxes.Add(c.A);
        JunctionBoxes.Add(c.B);
        c.A.Circuit = this;
        c.B.Circuit = this;
    }

    public void Add(Circuit c)
    {
        foreach (var x in c.JunctionBoxes)
        {
            JunctionBoxes.Add(x);
            x.Circuit = this;
        }
    }
}

class JunctionBox
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
    public Circuit? Circuit { get; set; }
    public JunctionBox(int[] points)
    {
        X = points[0];
        Y = points[1];
        Z = points[2];
    }

    public override string ToString()
    {
        return $"{{ {X}, {Y}, {Z} }}";
    }
}

class Connection
{
    public JunctionBox A { get; set; }
    public JunctionBox B { get; set; }
    public long Distance { get; set; }

    public Connection(JunctionBox a, JunctionBox b)
    {
        A = a;
        B = b;
        Distance =
            (long)Math.Pow(a.X - b.X, 2)
            + (long)Math.Pow(a.Y - b.Y, 2)
            + (long)Math.Pow(a.Z - b.Z, 2);
    }
}