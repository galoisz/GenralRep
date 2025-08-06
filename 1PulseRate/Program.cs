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
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;


//public class MedicalRecord
//{
//    public int Id { get; set; }
//    public long Timestamp { get; set; }
//    public Diagnosis Diagnosis { get; set; }
//    public Vitals Vitals { get; set; }
//    public Doctor Doctor { get; set; }
//    public int UserId { get; set; }
//    public string UserName { get; set; }
//    public string UserDob { get; set; }
//    public Meta Meta { get; set; }
//}

//public class Diagnosis
//{
//    public int Id { get; set; }
//    public string Name { get; set; }
//    public int Severity { get; set; }
//}

//public class Vitals
//{
//    public int BloodPressureDiastole { get; set; }
//    public int BloodPressureSystole { get; set; }
//    public int Pulse { get; set; }
//    public int BreathingRate { get; set; }
//    public double BodyTemperature { get; set; }
//}

//public class Doctor
//{
//    public int Id { get; set; }
//    public string Name { get; set; }
//}

//public class Meta
//{
//    public int Height { get; set; }
//    public int Weight { get; set; }
//}

//public class ApiResponse
//{
//    public int Page { get; set; }
//    public int PerPage { get; set; }
//    public int Total { get; set; }
//    public int TotalPages { get; set; }
//    public List<MedicalRecord> Data { get; set; }
//}


class Result
{

    /*
     * Complete the 'pulseRate' function below.
     *
     * The function is expected to return an INTEGER.
     * The function accepts following parameters:
     *  1. STRING diagnosisName
     *  2. INTEGER doctorId
     * API URL: https://jsonmock.hackerrank.com/api/medical_records?page={page_no}
     */

    private static readonly HttpClient _httpClient = new HttpClient();

    public static async Task<int> PulseRateAsync(string diagnosisName, int doctorId)
    {
        string baseUrl = "https://jsonmock.hackerrank.com/api/medical_records?page=";
        int page = 1, totalPages = 1;
        List<int> pulseRates = new List<int>();

        while (page <= totalPages)
        {
            string url = $"{baseUrl}{page}";
            string response = await _httpClient.GetStringAsync(url);
            using JsonDocument doc = JsonDocument.Parse(response);

            if (page == 1)
                totalPages = doc.RootElement.GetProperty("total_pages").GetInt32();

            foreach (var record in doc.RootElement.GetProperty("data").EnumerateArray())
            {
                string recordDiagnosis = record.GetProperty("diagnosis").GetProperty("name").GetString();
                int recordDoctorId = record.GetProperty("doctor").GetProperty("id").GetInt32();
                int pulse = record.GetProperty("vitals").GetProperty("pulse").GetInt32();

                if (recordDiagnosis == diagnosisName && recordDoctorId == doctorId)
                    pulseRates.Add(pulse);
            }

            page++;
        }

        return pulseRates.Any() ? (int)pulseRates.Average() : 0;
    }

}

class Solution
{
    public static void Main(string[] args)
    {
        //TextWriter textWriter = new StreamWriter(@System.Environment.GetEnvironmentVariable("OUTPUT_PATH"), true);

        string diagnosisName = Console.ReadLine();

        int doctorId = Convert.ToInt32(Console.ReadLine().Trim());

        //int result = Result.pulseRate(diagnosisName, doctorId);
        int result = Result.PulseRateAsync(diagnosisName, doctorId).Result;

        //textWriter.WriteLine(result);

        //textWriter.Flush();
        //textWriter.Close();
    }
}
