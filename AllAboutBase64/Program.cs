// See https://aka.ms/new-console-template for more information
using System.Text;
using System.Text.RegularExpressions;

while (true)
{
    Console.WriteLine("Enter a string to decode:");
    string input = Console.ReadLine();

    bool isstringbase64 = isValidBase64(input);
    Console.WriteLine($"is Base64 : {isstringbase64}");
    bool isstringprobablybase64 = IsProbablyBase64(input);
    Console.WriteLine($"is probably Base64 : {isstringprobablybase64}");
}
 static bool isValidBase64(string s)
{
    if (string.IsNullOrWhiteSpace(s))
        return false;

    s = s.Trim();

    return s.Length % 4 == 0 &&
           Regex.IsMatch(s, @"^[A-Za-z0-9+/]+={0,2}$") &&
           !s.Contains("===", StringComparison.Ordinal);
}

// Optional: entropy check to reduce false positives
static bool IsProbablyBase64(string input)
{
    if (input.Length % 4 != 0 || input.Length < 8)// 8 is a reasonable minimum to expect the content actually came from meaningful binary/ text
        return false;

    // Very naive heuristic: mostly printable after decode
    try
    {
        byte[] data = Convert.FromBase64String(input);
        // Optional: skip very short results (likely random junk)
        if (data.Length < 4) return false;

        string decoded = Encoding.UTF8.GetString(data);

        if (decoded.Length < 6) return false;

        int printableCount = 0;

        foreach (char c in decoded)
        {
            if (!(char.IsLetterOrDigit(c) || c == '+' || c == '/' || c == '='))
                return false;
            if (!char.IsControl(c)) printableCount++;
        }

        return (printableCount * 100 / decoded.Length) > 80; // 80% printable
    }
    catch
    {
        return false;
    }
}
