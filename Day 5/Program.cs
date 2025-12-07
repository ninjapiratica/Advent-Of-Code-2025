var lines = await File.ReadAllLinesAsync("input.txt");

var tempValues = ParseRanges(lines, out var availableIndex);
var ranges = MergeRanges(tempValues);

var count = 0;
for (int i = availableIndex; i < lines.Length; i++)
{
    var available = long.Parse(lines[i]);
    var range = ranges.FirstOrDefault(r => r.MaxValue > available);
    if (available > range.MinValue && available < range.MaxValue)
    {
        count++;
    }
}

Console.WriteLine("Available: " + count);

var totalFresh = ranges.Select(r => r.MaxValue - r.MinValue + 1).Sum();

Console.WriteLine("Fresh: " + totalFresh);

List<(long MinValue, long MaxValue)> ParseRanges(string[] lines, out int availableIndex)
{
    List<(long MinValue, long MaxValue)> tempValues = new();

    availableIndex = 0;
    for (int i = 0; i < lines.Length; i++)
    {
        var line = lines[i];
        if (line.Length == 0)
        {
            availableIndex = i + 1;
            break;
        }

        if (line.Split('-') is [var minRValue, var maxRValue])
        {
            tempValues.Add((long.Parse(minRValue), long.Parse(maxRValue)));
        }
    }

    return tempValues;
}

List<(long MinValue, long MaxValue)> MergeRanges(List<(long MinValue, long MaxValue)> values)
{
    List<(long MinValue, long MaxValue)> ranges = new();

    var last = (MinValue: 0L, MaxValue: 0L);
    foreach (var tempValue in tempValues.OrderBy(x => x.MinValue))
    {
        if (tempValue.MinValue <= last.MaxValue)
        {
            last.MaxValue = Math.Max(last.MaxValue, tempValue.MaxValue);
            ranges[^1] = last;
        }
        else
        {
            ranges.Add(tempValue);
            last = tempValue;
        }
    }

    return ranges;
}