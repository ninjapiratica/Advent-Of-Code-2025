var lines = await File.ReadAllLinesAsync("input.txt");
var columns = lines[0].Length;

var accessible = new int[lines.Length, lines[0].Length];

for (var i = 0; i < lines.Length; i++)
{
    var line = lines[i];
    for (var j = 0; j < line.Length; j++)
    {
        if (line[j] == '@')
        {
            ImpactNeighbors(i, j, 1);
        }
    }
}

var count = 0;
while (RemoveRolls(out var removedRollsCount))
{
    count += removedRollsCount;
}

Console.WriteLine(count);

void ImpactNeighbors(int i, int j, int count)
{
    // previous row
    if (i > 0)
    {
        accessible[i - 1, j] += count;

        if (j > 0)
        {
            accessible[i - 1, j - 1] += count;
        }

        if (j < columns - 1)
        {
            accessible[i - 1, j + 1] += count;
        }
    }

    // current row
    if (j > 0)
    {
        accessible[i, j - 1] += count;
    }

    if (j < columns - 1)
    {
        accessible[i, j + 1] += count;
    }

    // nex row
    if (i < lines.Length - 1)
    {
        accessible[i + 1, j] += count;

        if (j > 0)
        {
            accessible[i + 1, j - 1] += count;
        }

        if (j < columns - 1)
        {
            accessible[i + 1, j + 1] += count;
        }
    }
}

bool RemoveRolls(out int removedRolls)
{
    removedRolls = 0;
    var didRemoveRoll = false;

    for (var i = 0; i < accessible.GetLength(0); i++)
    {
        for (var j = 0; j < accessible.GetLength(1); j++)
        {
            if (accessible[i, j] < 4 && lines[i][j] == '@')
            {
                removedRolls++;
                didRemoveRoll = true;
                ImpactNeighbors(i, j, -1);
                lines[i] = lines[i][0..j] + 'X' + lines[i][(j + 1)..];
            }
        }
    }

    return didRemoveRoll;
}

// 2115