using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Benchmarks.Tables;
using BTDB.KVDBLayer;
using BTDB.ODBLayer;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("BTDB tests");
            var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
//            var l = new Listing();
//            l.GlobalSetup();
//            while (true)
//            {
//                var p1 = l.SmallNumberOfValuesUseKeyListBy();
//                Console.WriteLine($"Count: {p1.Count}");
//            }
        }
    }

    

    
}