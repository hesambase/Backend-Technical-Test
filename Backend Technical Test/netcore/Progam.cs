using System;
using System.Collections.Generic;
using System.Data.SQLite;
public class Program
{
    static void Main()
    {
        var connectionString = "Data Source=congestion_tax.db;";
        var calculator = new CongestionTaxCalculator(connectionString);

        var car = new Car();
        var motorbike = new Motorbike();

        var dates = new DateTime[]
        {
            new DateTime(2013, 1, 14, 21, 0, 0),
            new DateTime(2013, 1, 15, 21, 0, 0),
            new DateTime(2013, 2, 7, 6, 23, 27),
            // More dates...
        };

        Console.WriteLine($"Car tax: {calculator.GetTax(car, dates)}");
        Console.WriteLine($"Motorbike tax: {calculator.GetTax(motorbike, dates)}");
    }
