using System;
using System.Collections.Generic;
using System.Linq;

public class SensorState
{
    // Kalman filter
    public double Estimate { get; set; }
    public double ErrorCovariance { get; set; } = 1;
    public double ProcessNoise { get; set; } = 0.01;
    public double MeasurementNoise { get; set; } = 0.5;

    // Öğrenme penceresi
    public List<double> Window { get; set; } = new();
}

public class UltraAdvancedAnomalyEngine
{
    private readonly Dictionary<string, SensorState> sensors = new();
    private readonly int windowSize;

    public event Action<string, double, double> OnAnomaly;

    public UltraAdvancedAnomalyEngine(int windowSize = 40)
    {
        this.windowSize = windowSize;
    }

    private double KalmanUpdate(SensorState s, double measurement)
    {
        // Prediction
        double predictedEstimate = s.Estimate;
        double predictedCov = s.ErrorCovariance + s.ProcessNoise;

        // Update
        double kalmanGain = predictedCov / (predictedCov + s.MeasurementNoise);
        double updatedEstimate = predictedEstimate + kalmanGain * (measurement - predictedEstimate);
        double updatedCov = (1 - kalmanGain) * predictedCov;

        s.Estimate = updatedEstimate;
        s.ErrorCovariance = updatedCov;

        return Math.Abs(measurement - updatedEstimate);
    }

    private double TrendDivergence(List<double> w)
    {
        double trend = 0;
        for (int i = 1; i < w.Count; i++)
        {
            trend += (w[i] - w[i - 1]);
        }
        return Math.Abs(trend / w.Count);
    }

    private double BehaviorPatternScore(List<double> w)
    {
        double variance = w.Select(x => Math.Pow(x - w.Average(), 2)).Average();
        return Math.Min(variance * 2, 50);
    }

    public void Process(string sensor, double value)
    {
        if (!sensors.ContainsKey(sensor))
            sensors[sensor] = new SensorState();

        var s = sensors[sensor];

        if (s.Estimate == 0)
            s.Estimate = value;

        double kalmanErr = KalmanUpdate(s, value);

        s.Window.Add(value);
        if (s.Window.Count > windowSize)
            s.Window.RemoveAt(0);

        if (s.Window.Count < windowSize)
            return;

        double trendScore = TrendDivergence(s.Window) * 10;
        double behaviorScore = BehaviorPatternScore(s.Window);

        // Total anomaly score calculation
        double totalScore = Math.Min(kalmanErr * 10 + trendScore + behaviorScore, 100);

        if (totalScore > 70)
            OnAnomaly?.Invoke(sensor, value, totalScore);
    }
}

public class Program
{
    public static void Main()
    {
        var engine = new UltraAdvancedAnomalyEngine();

        engine.OnAnomaly += (sensor, val, score) =>
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"🚨 Ultra Anomali [{sensor}] Değer={val:F2} | Skor={score:F1}");
            Console.ResetColor();
        };

        Random rnd = new Random();

        for (int i = 0; i < 150; i++)
        {
            double s1 = rnd.NextDouble() * 10;
            double s2 = rnd.NextDouble() * 20;

            if (i == 60) s1 = 90;     // büyük sıçrama
            if (i == 110) s2 = -30;   // ters yönde çöküş
            if (i > 120) s2 += 0.3 * (i - 120); // davranış değişikliği (yavaş drift)

            engine.Process("Sicaklik", s1);
            engine.Process("Basinc", s2);

            Console.WriteLine($"📡 Sıcaklık={s1:F2} Basınç={s2:F2}");
        }
    }
}
