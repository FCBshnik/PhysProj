namespace Phys.Shared.Utils
{
    public static class CliArgsUtils
    {
        public static Dictionary<string, string?> Parse(params string[] args)
        {
            var result = new Dictionary<string, string?>();
            string? key = null;

            foreach (var arg in args.Where(a => a?.Length > 0).Select(a => a.Trim(' ')))
            {
                if (arg.StartsWith('-'))
                {
                    key = arg.TrimStart('-');
                    if (key.Length == 0)
                        throw new ArgumentException($"failed parse cli args");

                    result.Add(key, null);
                }
                else if (key != null)
                {
                    result[key] = arg;
                    key = null;
                }
                else
                    throw new ArgumentException($"failed parse cli args");
            }

            return result;
        }
    }
}
