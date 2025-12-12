var lines = await File.ReadAllLinesAsync("input.txt");

var devices = lines.Select(l => new Device(l)).ToDictionary(d => d.Name, d => d);

foreach (var device in devices)
{
    device.Value.OutputDevices = device.Value.Outputs
        .Select(o => devices[o])
        .ToArray();

    device.Value.InputDevices = devices.Where(d => d.Value.Outputs.Contains(device.Value.Name)).Select(d => d.Value).ToArray();
}

var svr = devices["svr"];
var fft = devices["fft"];
var fftFirst = false;

CheckNodes(fft, x => x.Name == "dac", x => fftFirst = true, x => { });

var lastNode = fft;
var firstNode = fft;
if (fftFirst)
{
    lastNode = devices["dac"];
}
else
{
    firstNode = devices["dac"];
}

CheckNodes(lastNode, x => false, x => { }, x => x.Position = Position.End);
CheckNodes(firstNode, x => false, x => { }, x => x.Position = x.Position == Position.Unknown ? Position.Middle : x.Position);
CheckNodes(svr, x => false, x => { }, x => x.Position = x.Position == Position.Unknown ? Position.Start : x.Position);


var test = devices.GroupBy(d => d.Value.Position).Select(x => new { x.Key, Count = x.Count() }).ToList();

Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(test));

var endPaths = CountNodes(lastNode, x => x.Position == Position.End, x => x.Name == "out");
Console.WriteLine($"endPaths: {endPaths.Count}");

var midPaths = CountNodes(lastNode, x => x.Position == Position.Middle || x == lastNode, x => x == firstNode, x => x.InputDevices);
Console.WriteLine($"midPaths: {midPaths.Count}");

var startPaths = CountNodes(svr, x => x.Position == Position.Start, x => x == firstNode);
Console.WriteLine($"startPaths: {startPaths.Count}");

Console.WriteLine($"mult: {endPaths.Count * (long)midPaths.Count * startPaths.Count}");

void CheckNodes(Device start, Func<Device, bool> isFinalNode, Action<Device> finalNode, Action<Device> notFinalNode)
{
    notFinalNode(start);
    var checkedNodes = new HashSet<Device>();
    var nodes = start.OutputDevices.ToHashSet();
    while (nodes.Count > 0)
    {
        var firstNode = nodes.First();
        if (isFinalNode(firstNode))
        {
            finalNode(firstNode);
            return;
        }
        else
        {
            notFinalNode(firstNode);
        }

        foreach (var node in firstNode.OutputDevices)
        {
            if (!checkedNodes.Contains(node))
            {
                nodes.Add(node);
            }
        }
        checkedNodes.Add(firstNode);
        nodes.Remove(firstNode);
    }
}

List<Path> CountNodes(Device startNode, Func<Device, bool> isGoodPath, Func<Device, bool> isFinalNode, Func<Device, IEnumerable<Device>>? getNextNodes = null)
{
    var paths = new Queue<Path>([new Path { Devices = [startNode] }]);
    var goodPaths = new List<Path>();
    while (paths.TryDequeue(out var path))
    {
        var lastNode = path.Devices.Last();

        getNextNodes ??= x => x.OutputDevices;

        foreach (var node in getNextNodes(lastNode))
        {
            if (isFinalNode(node))
            {
                goodPaths.Add(new Path { Devices = [.. path.Devices, node] });
            }
            else if (isGoodPath(node))
            {
                paths.Enqueue(new Path { Devices = [.. path.Devices, node] });
            }
        }
    }

    return goodPaths;
}


class Device
{
    public Device(string l)
    {
        var x = l.Split(':');

        Name = x[0];
        Outputs = x[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).ToArray();
    }

    public Position Position { get; set; }
    public string Name { get; }
    public string[] Outputs { get; }
    public Device[] OutputDevices { get; set; } = [];
    public Device[] InputDevices { get; set; } = [];

    private int _pathsToOut = -1;
    public int PathsToOut
    {
        get
        {
            if (_pathsToOut >= 0)
            {
                return _pathsToOut;
            }

            if (Outputs.Contains("out"))
            {
                _pathsToOut = 1;
            }
            else
            {
                _pathsToOut = OutputDevices.Sum(d => d.PathsToOut);
            }

            return _pathsToOut;
        }
    }
}

class Path
{
    public List<Device> Devices { get; set; } = [];
}

enum Position
{
    Unknown,
    Start,
    Middle,

    End
}