using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Text;
using System;

public interface IExchangeRateData
{
    string CurrencyPair { get; set; }
    decimal Rate { get; set; }
    DateTime Date { get; set; }
}

public interface IExchangeRateAnalyzer
{
    decimal FilterExchangeRates(List<ExchangeRateData> exchangeRates, string currencyPair, DateTime startDate, DateTime endDate);
}


public class ExchangeRateData : IExchangeRateData
{
    public string CurrencyPair { get; set; }
    public decimal Rate { get; set; }
    public DateTime Date { get; set; }
    public ExchangeRateData(string currencyPair, decimal rate, DateTime date)
    {
        CurrencyPair = currencyPair;
        Rate = rate;
        Date = date;
    }

}

public class ExchangeRateAnalyzer : IExchangeRateAnalyzer
{
    public decimal FilterExchangeRates(List<ExchangeRateData> exchangeRates, string currencyPair, DateTime startDate, DateTime endDate)
    {
        // Implement the method logic
        if (exchangeRates == null || exchangeRates.Count == 0) throw new Exception("empty data");
        List<ExchangeRateData> rates = exchangeRates.Where(x => x.CurrencyPair == currencyPair && x.Date >= startDate && x.Date <= endDate).ToList();
        if (!rates.Any()) throw new Exception($"data not found for currency {currencyPair} in the time interval {startDate.ToString("yyyy-MM-dd")}-{startDate.ToString("yyyy-MM-dd")}");

        // exchangeRates.ForEach(x=>{
        //    Console.WriteLine($"{x.Rate}, {x.CurrencyPair}, {x.Date.ToString("yyyy-MM-dd")}") ;
        // });
        decimal result = exchangeRates.Average(x => x.Rate);
        //Console.WriteLine(result);
        return result;
    }
}

public class Solution
{
    static void Main()
    {
        List<ExchangeRateData> exchangeRates = ReadExchangeRatesFromConsole();
        string currencyPair = Console.ReadLine().Trim();
        DateTime startDate = DateTime.ParseExact(Console.ReadLine().Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
        DateTime endDate = DateTime.ParseExact(Console.ReadLine().Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture);

        ExchangeRateAnalyzer analyzer = new ExchangeRateAnalyzer();
        try
        {
            decimal averageRate = analyzer.FilterExchangeRates(exchangeRates, currencyPair, startDate, endDate);
            Console.WriteLine($"{averageRate:F2}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static List<ExchangeRateData> ReadExchangeRatesFromConsole()
    {
        int numEntries = int.Parse(Console.ReadLine().Trim());
        var exchangeRates = new List<ExchangeRateData>();

        for (int i = 0; i < numEntries; i++)
        {
            string[] entry = Console.ReadLine().Trim().Split(' ');
            string currencyPair = entry[0];
            decimal rate = decimal.Parse(entry[1], CultureInfo.InvariantCulture);
            DateTime date = DateTime.ParseExact(entry[2], "yyyy-MM-dd", CultureInfo.InvariantCulture);
            exchangeRates.Add(new ExchangeRateData(currencyPair, rate, date));
        }

        return exchangeRates;
    }
}