var lines = await File.ReadAllLinesAsync("input.txt");

var parts = lines.Select(l => l.Split(' ', StringSplitOptions.RemoveEmptyEntries)).ToList();

var problemsCount = parts[0].Length;

var all = 0L;
for (var i = 0; i < problemsCount; i++)
{
    var isAdd = false;
    var values = new List<long>();
    foreach (var part in parts)
    {
        if (part[i] == "*")
        {
            isAdd = false;
        }
        else if (part[i] == "+")
        {
            isAdd = true;
        }
        else
        {
            values.Add(long.Parse(part[i]));
        }
    }

    var total = isAdd
        ? values.Aggregate(0L, (acc, val) => acc + val)
        : values.Aggregate(1L, (acc, val) => acc * val);
    Console.WriteLine($"{string.Join(isAdd ? " + " : " * ", values)} = {total}");
    all += total;
}

Console.WriteLine("Bad Math: " + all);

var spaceIndices = lines.Select(l => l.Select((c, i) => c == ' ' ? i : -1).ToList()).ToList();
var length = lines[0].Length;
var columnIndices = new List<int>() { -1 };

for (var i = 0; i < length; i++)
{
    if (spaceIndices.All(l => l[i] == i))
    {
        columnIndices.Add(i);
    }
}

var previousIndex = length;
var goodTotal = 0L;
for (var i = columnIndices.Count - 1; i >= 0; i--)
{
    var columnIndex = columnIndices[i];
    var isAdd = false;
    var values = new List<long>();

    for (var j = previousIndex - 1; j > columnIndex; j--)
    {
        var val = "";
        foreach (var line in lines)
        {
            if (line[j] == '*')
            {
                isAdd = false;
            }
            else if (line[j] == '+')
            {
                isAdd = true;
            }
            else
            {
                val += line[j];
            }
        }

        values.Add(long.Parse(val));
        previousIndex = columnIndex;
    }

    var total = isAdd
        ? values.Aggregate(0L, (acc, val) => acc + val)
        : values.Aggregate(1L, (acc, val) => acc * val);
    Console.WriteLine($"{string.Join(isAdd ? " + " : " * ", values)} = {total}");
    goodTotal += total;
}

Console.WriteLine($"Good Total: {goodTotal}");