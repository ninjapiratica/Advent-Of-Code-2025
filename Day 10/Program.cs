var lines = await File.ReadAllLinesAsync("input.txt");
var machines = lines.Select(l => new Machine(l)).ToList();

var total = 0;
foreach (var machine in machines)
{
    for (int i = 1; i <= machine.Buttons.Count; i++)
    {
        if (machine.TestLights(i))
        {
            total += i;
            break;
        }
    }
}

Console.WriteLine(total);

var totalJoltagePresses = 0;
foreach (var machine in machines)
{
    var i = machine.GetJoltagePresses();
    Console.WriteLine("Machine: " + i);
    totalJoltagePresses += i;
}

Console.WriteLine(totalJoltagePresses);

// 547 < i

class Machine
{
    public bool[] LightsTarget { get; set; }
    public List<Button> Buttons { get; set; }
    public int[] JoltageTarget { get; set; }

    public Machine(string line)
    {
        var parts = line.Split(' ');
        LightsTarget = ParseLightsTarget(parts[0]);
        Buttons = ParseButtons(parts[1..^1]);
        JoltageTarget = ParseJoltage(parts.Last());
    }

    bool[] ParseLightsTarget(string l)
    {
        return l[1..^1].Select(x => x == '.' ? false : true).ToArray();
    }

    List<Button> ParseButtons(string[] b)
    {
        return b.Select(x => new Button(x)).ToList();
    }

    int[] ParseJoltage(string l)
    {
        return l[1..^1].Split(',').Select(int.Parse).ToArray(); ;
    }

    public bool TestLights(int buttons)
    {
        foreach (var grouping in GetGroupings(Buttons.ToArray(), buttons))
        {
            var Lights = new bool[LightsTarget.Length];
            foreach (var b in grouping)
            {
                foreach (var l in b.Lights)
                {
                    Lights[l] = !Lights[l];
                }
            }

            if (Lights.SequenceEqual(LightsTarget))
            {
                return true;
            }
        }

        return false;
    }

    private IEnumerable<Button[]> GetGroupings(Button[] buttons, int count)
    {
        var markers = Enumerable.Range(0, count).ToArray();
        do
        {
            yield return buttons.Where((b, ix) => markers.Contains(ix)).ToArray();
        } while (MoveMarkers(buttons, markers));
    }

    private bool MoveMarkers(Button[] buttons, int[] markers)
    {
        if (buttons.Length == 0 || markers.Length == 0)
        {
            return false;
        }

        var markerIndex = markers.Length - 1;
        while (markerIndex >= 0)
        {
            var btnIndex = markers[markerIndex];
            var curButton = buttons[btnIndex];

            btnIndex++;
            var maxButtonIndex = buttons.Length - (markers.Length - markerIndex - 1);
            while (btnIndex < maxButtonIndex
                && buttons[btnIndex] == curButton)
            {
                btnIndex++;
            }

            if (btnIndex >= maxButtonIndex)
            {
                markerIndex--;
                continue;
            }

            markers[markerIndex] = btnIndex;
            markerIndex++;
            btnIndex++;
            while (markerIndex < markers.Length)
            {
                markers[markerIndex] = btnIndex;
                btnIndex++;
                markerIndex++;
            }
            return true;
        }
        return false;
    }

    public int GetJoltagePresses()
    {
        var orderedJoltageTargets = JoltageTarget.Select((t, ix) => (t, ix)).OrderByDescending(x => x.t).ToArray();

        var buttonsPerJoltage = JoltageTarget
            .Select((x, ix) =>
                Buttons
                    .Where(b => b.Lights.Contains(ix))
                    .ToList())
            .ToArray();

        var orderedButtons = buttonsPerJoltage.Select((x, ix) => (ix, Buttons: x)).OrderBy(b => b.Buttons.Count).ToList();

        for (var i = 0; i < orderedButtons.Count; i++)
        {
            var ob1 = orderedButtons[i];
            for (var j = i + 1; j < orderedButtons.Count; j++)
            {
                orderedButtons[j].Buttons.RemoveAll(x => ob1.Buttons.Contains(x));
            }
        }

        var joltages = new int[JoltageTarget.Length];

        TestJoltages(joltages, orderedButtons, out var count);

        return count;
    }

    private bool TestJoltages(int[] currentJoltages, List<(int Index, List<Button> Buttons)> orderedButtons, out int count)
    {
        if (orderedButtons.Count == 0)
        {
            count = 0;
            return true;
        }

        var whatWeCareAbout = orderedButtons.First();
        var hereCount = JoltageTarget[whatWeCareAbout.Index] - currentJoltages[whatWeCareAbout.Index];

        if (hereCount < 0)
        {
            count = 0;
            return false;
        }

        if (hereCount == 0)
        {
            if (TestJoltages([.. currentJoltages], orderedButtons.Skip(1).ToList(), out var subCount))
            {
                count = subCount;
                return true;
            }
            count = 0;
            return false;
        }

        var hereButtons = whatWeCareAbout.Buttons.SelectMany(b => Enumerable.Repeat(b, hereCount)).ToArray();

        var minPresses = int.MaxValue;

        foreach (var grouping in GetGroupings(hereButtons, hereCount))
        {
            int[] test = [.. currentJoltages];

            foreach (var button in grouping)
            {
                foreach (var light in button.Lights)
                {
                    test[light]++;
                }
            }

            if (test.Zip(JoltageTarget).Any(x => x.First > x.Second))
            {
                continue;
            }

            if (TestJoltages(test, orderedButtons.Skip(1).ToList(), out var subCount))
            {
                minPresses = Math.Min(minPresses, hereCount + subCount);
            }
        }

        if (minPresses == int.MaxValue)
        {
            count = 0;
            return false;
        }

        count = minPresses;
        return true;
    }
}

class Button
{
    public List<int> Lights { get; set; }
    public Button(string l)
    {
        Lights = l.Replace("(", "").Replace(")", "").Split(',').Select(int.Parse).ToList();
    }
}