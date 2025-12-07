using var fs = File.Open("input.txt", FileMode.Open);
using var sr = new StreamReader(fs);

var totalBatteries = 12;
var totalJoltage = 0L;
while (!sr.EndOfStream)
{
    var line = await sr.ReadLineAsync();
    var chars = new char[totalBatteries];
    var index = -1;
    for (var i = 0; i < totalBatteries; i++)
    {
        GetMaxValue(line!, index + 1, line!.Length - (totalBatteries - i), out var val, out index);
        chars[i] = val;
    }

    Console.WriteLine($"{line} {string.Concat(chars)}");

    totalJoltage += long.Parse(new string(chars));
}

Console.WriteLine(totalJoltage);

void GetMaxValue(string line, int startIndex, int endIndex, out char val, out int index)
{
    val = (char)('0' - 1);
    index = -1;
    for (int i = startIndex; i <= endIndex; i++)
    {
        var c = line![i];
        if (c > val)
        {
            val = c;
            index = i;

            if (c == '9')
                break;
        }
    }
}