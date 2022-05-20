using System;
using System.Text.RegularExpressions;

namespace TupiLibraryConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine(TupiComparison.IsInInterval(-150, "[-165.89, -78.789["));

            var val = 12.98;

            try
            {
                Console.WriteLine(val.IsInInterval("[-5, 15]"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            var cpf = "123.132.789-89";

            Console.WriteLine($"{nameof(cpf)}: {cpf} - Valid: {TupiComparison.ValidateCpfSyntax(cpf)}");
        }
    }

    public enum CpfSyntaxOption { Punctuation, NoPunctuation, Both }

    public static class TupiComparison
    {
        private static bool ValidateIntervalSyntax(string interval) =>
            Regex.IsMatch(interval, @"^[\[\]]-?[0-9]+(\.[0-9]+)?, ?-?[0-9]+(\.[0-9]+)?[\[\]]$", RegexOptions.Singleline);

        public static bool IsInInterval(this double value, string interval)
        {
            if (!ValidateIntervalSyntax(interval))
                throw new Exception($"Error: Invalid interval pattern!");
            var parts = interval.Split(',');
            var min = double.Parse(parts[0].Substring(1));
            var max = double.Parse(parts[1].Substring(0, parts[1].Length - 1), System.Globalization.CultureInfo.InvariantCulture);

            if (min > max)
                throw new Exception($"Error: min value cannot be greater than max value! min was {min} and max was {max}.");

            var type = $"{parts[0][0]}{parts[1][^1]}";
            return type switch
            {
                "[[" => value >= min && value < max,
                "]]" => value > min && value <= max,
                "[]" => value >= min && value <= max,
                "][" => value > min && value < max,
                _ => throw new Exception($"Unexpected error!"),
            };
        }

        public static bool ValidateCpfSyntax(string cpfValue) => ValidateCpfSyntax(cpfValue, CpfSyntaxOption.Both);
        public static bool ValidateCpfSyntax(string cpfValue, CpfSyntaxOption option)
        {
            var patterns = new string[] { @"^[0-9]{3}(\.[0-9]{3}){2}-[0-9]{2}$", @"^[0-9]{11}$"};

            if (option != CpfSyntaxOption.Both)
                return Regex.Match(cpfValue, patterns[(int)option], RegexOptions.Singleline).Success;

            foreach(var pattern in patterns)
            {
                if (Regex.Match(cpfValue, pattern, RegexOptions.Singleline).Success)
                    return true;
            }

            return false;
        }
    }
}
