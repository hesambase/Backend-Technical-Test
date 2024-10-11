using System;
using congestion.calculator;
public class CongestionTaxCalculator
{
    private readonly string _connectionString;

    public CongestionTaxCalculator(string connectionString)
    {
        _connectionString = connectionString;
    }

    public int GetTax(Vehicle vehicle, DateTime[] dates)
    {
        if (IsTollFreeVehicle(vehicle)) return 0;

        int totalFee = 0;
        DateTime? intervalStart = null;

        foreach (var date in dates)
        {
            if (IsTollFreeDate(date)) continue;

            if (intervalStart == null || (date - intervalStart.Value).TotalMinutes > 60)
            {
                intervalStart = date;
                totalFee += GetTollFee(date);
            }
            else
            {
                totalFee = Math.Max(totalFee, GetTollFee(date));
            }

            if (totalFee >= 60) return 60;
        }

        return totalFee;
    }

    private bool IsTollFreeVehicle(Vehicle vehicle)
    {
        if (vehicle == null) return false;
        string vehicleType = vehicle.GetVehicleType();
        return Enum.TryParse(vehicleType, out TollFreeVehicles result);
    }

    private int GetTollFee(DateTime date)
    {
        using (var connection = new SQLiteConnection(_connectionString))
        {
            connection.Open();
            using (var command = new SQLiteCommand(connection))
            {
                command.CommandText = "SELECT Amount FROM TaxRates WHERE StartTime <= @time AND EndTime >= @time";
                command.Parameters.AddWithValue("@time", date.ToString("HH:mm"));
                var result = command.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }
    }

    private bool IsTollFreeDate(DateTime date)
    {
        using (var connection = new SQLiteConnection(_connectionString))
        {
            connection.Open();
            using (var command = new SQLiteCommand(connection))
            {
                command.CommandText = "SELECT COUNT(*) FROM Holidays WHERE Date = @date";
                command.Parameters.AddWithValue("@date", date.ToString("yyyy-MM-dd"));
                var count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0 || date.Month == 7 || date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
            }
        }
    }

    private enum TollFreeVehicles
    {
        Motorcycle,
        Tractor,
        Emergency,
        Diplomat,
        Foreign,
        Military
    }
}
