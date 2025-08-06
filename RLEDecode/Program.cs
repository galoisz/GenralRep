// See https://aka.ms/new-console-template for more information


using System.Collections.Concurrent;
using System.Text;

Console.WriteLine(Decode("a2b3c1"));

static string Decode(string input) {
    if(string.IsNullOrWhiteSpace(input)) return input;

    StringBuilder result = new StringBuilder("");
    int i = 0;
    while(i < input.Length)
    {
        char c = input[i];
        i++;
        int appearances = int.Parse( input[i].ToString());
        i++;
        for (int  j = 0; j < appearances; j++)
        {
            result.Append(c);
        }
    }

    return result.ToString();   
}


