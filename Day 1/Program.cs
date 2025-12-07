using var fs = File.Open("input.txt", FileMode.Open);
using var sr = new StreamReader(fs);

var position = 50;
var exactCount = 0;
var crossCount = 0;

while (!sr.EndOfStream)
{
    var line = await sr.ReadLineAsync();
    var oldPosition = position;
    position += (line![0] == 'L' ? -1 : 1) * int.Parse(line[1..]);

    crossCount += TimesCrossing0(oldPosition, position);

    position %= 100;

    if (position < 0)
    {
        position += 100;
    }

    if (position == 0)
    {
        exactCount++;
    }
}

int TimesCrossing0(int oldPosition, int newPosition)
{
    var crossCount = 0;
    if ((oldPosition != 0 && int.IsNegative(newPosition)) || newPosition == 0)
    {
        crossCount++;
    }
    crossCount += Math.Abs(newPosition / 100);
    return crossCount;
}

Console.WriteLine($"ExactCount: {exactCount} - CrossCount: {crossCount}");