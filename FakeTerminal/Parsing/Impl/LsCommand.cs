using System.Text;

namespace FakeTerminal.Parsing.Impl;

public class LsCommand : ACommand
{
    public LsCommand() : base("ls")
    { }

    public override string Description => "Display the list of files and folders in the current directory";

    internal override CommandParameter[] AllParameters => [
        new()
        {
            ShortName = 'l',
            LongName = "list",
            Description = "Give more information about the files",
            Mandatory = false
        }
    ];

    private static string FormatFileName(string name)
    {
        if (name.Contains(' ')) return $"\"{name}\"";
        return name;
    }
    public override bool DoAction(Client client, Parameter[] parameters, out string output)
    {
        var res = base.DoAction(client, parameters, out output);
        if (!res) return false;

        if (parameters.Any(x => x.RefParameter.LongName == "list"))
        {
            // Detailed view
            StringBuilder str = new();
            foreach (var d in Directory.GetDirectories(client.CurrentDir.FullName))
            {
                DirectoryInfo di = new(d);
                str.AppendLine($"d---- {di.Name,50}");
            }
            foreach (var f in Directory.GetFiles(client.CurrentDir.FullName))
            {
                FileInfo fi = new(f);
                str.AppendLine($"----- {fi.Name,50} {fi.Length,10}");
            }

            output = str.ToString();
            return true;
        }

        List<string> files = [];

        foreach (var d in Directory.GetDirectories(client.CurrentDir.FullName))
        {
            DirectoryInfo di = new(d);
            files.Add(FormatFileName(di.Name) + "/");
        }
        foreach (var f in Directory.GetFiles(client.CurrentDir.FullName))
        {
            FileInfo fi = new(f);
            files.Add(FormatFileName(fi.Name));
        }

        output = string.Join(" ", files);

        return true;
    }
}