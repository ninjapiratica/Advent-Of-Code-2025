using var fs = File.Open("input.txt", FileMode.Open);
using var sr = new StreamReader(fs);

Dictionary<string, HashSet<long>> invalidIds = [];
while (!sr.EndOfStream)
{
    var line = await sr.ReadLineAsync();
    if (line!.Split('-') is [var start, var end])
    {
        var startVal = long.Parse(start);
        var endVal = long.Parse(end);
        for (var length = start.Length; length <= end.Length; length++)
        {
            for (var numLength = 1; numLength <= length / 2; numLength++)
            {
                if (length % numLength > 0)
                    continue;

                var repeats = length / numLength;

                var s1 = length == start.Length
                    ? start[..numLength]
                    : "1" + new string('0', numLength - 1);
                var s1Val = long.Parse(s1);

                do
                {
                    var test = long.Parse(string.Concat(Enumerable.Repeat(s1, repeats)));
                    if (test >= startVal && test <= endVal)
                    {
                        if(invalidIds.TryGetValue(line, out var hash))
                        {
                            hash.Add(test);
                        }
                        else
                        {
                            invalidIds.Add(line, [test]);
                        }
                        Console.WriteLine($"{start} - {test} - {end} - {numLength} - {repeats}");
                    }
                    else if (test > endVal)
                    {
                        break;
                    }
                    s1Val++;
                    s1 = s1Val.ToString();
                }
                while (s1.Length * repeats == length);
            }
        }
    }
}

Console.WriteLine($"Sum: {invalidIds.Values.Sum(x => x.Sum())}");