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

class Solution
{



    static IEnumerable<byte> Encode(IEnumerable<byte> original)
    {
        List<byte> result = new List<byte>();

        List<byte> originalList = original.ToList();

        int i = 0;
        while (i < originalList.Count)
        {
            byte currentByte = originalList[i];
            int count = 1;
            while (i + 1 < originalList.Count && originalList[i + 1] == currentByte)
            {
                count++;
                i++;
            }

            result.Add(currentByte);
            result.Add((byte)count);

            i++;
        }

        return result;
    }


    static void Main(string[] args)
    {
        //TextWriter textWriter = new StreamWriter(@System.Environment.GetEnvironmentVariable("OUTPUT_PATH"), true);

        //int originalCount = Convert.ToInt32(Console.ReadLine());

        //byte[] original = new byte[originalCount];

        //for (int i = 0; i < originalCount; i++)
        //{
        //    byte originalItem = Convert.ToByte(Console.ReadLine());
        //    original[i] = originalItem;
        //}


        //IEnumerable<byte> res = Encode(original);

        //textWriter.WriteLine(string.Join("\n", res));

        //textWriter.Flush();
        //textWriter.Close();


        List<int> originalInts = new() { 1, 1, 1, 2, 2, 2, 3 };
        byte[] original = originalInts.Select(x => Convert.ToByte(x)).ToArray();
        IEnumerable<byte> res = Encode(original);
        Console.WriteLine(string.Join("\n", res));
    }
}
