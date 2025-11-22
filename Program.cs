using System;
using System.Collections.Generic;
using System.Linq;

public class Telemetry
{
    public double Altitude;
    public double Latitude;
    public double Longitude;
    public double Pitch;
    public double Roll;
    public double Yaw;
    public double Speed;
}

public class DroneState
{
    public double AltitudeEstimate;
    public double Cov = 1;
}

public class AnomalyEventArgs : EventArgs
{
    public string Type;
    public double Score;
    public string Description;
}

public class UAVAnomalyEngine
{
    public event EventHandler<AnomalyEventArgs> OnAnomaly;

    private readonly List<double> altitudeWindow = new();
    private readonly int windowSize = 20;

    public void ProcessTelemetry(Telemetry t, DroneState state)
    {
        // KALMAN FILTER → daha temiz altitude üretir
        double pred = state.AltitudeEstimate;
        double predCov = state.Cov + 0.5;

        double kalmanGain = predCov / (predCov + 1);
        double corrected = pred + kalmanGain * (t.Altitude - pred);

        state.AltitudeEstimate = corrected;
        state.Cov = (1 - kalmanGain) * predCov;

        altitudeWindow.Add(corrected);
        if (altitudeWindow.Count > windowSize)
            altitudeWindow.RemoveAt(0);

        if (altitudeWindow.Count < windowSize) return;

        double trend = corrected - altitudeWindow.First();
        double variance = altitudeWindow.Select(x => Math.Pow(x - altitudeWindow.Average(), 2)).Average();

        double riskScore = 0;

        // Ani düşüş tespiti
        if (trend < -8)
        {
            riskScore += Math.Min(100, Math.Abs(trend) * 8);
            Raise("Rapid Descent", riskScore, "Drone altitude rapidly decreasing!");
        }

        // Stabilite bozulması (IMU)
        if (Math.Abs(t.Pitch) > 35 || Math.Abs(t.Roll) > 35)
        {
            double imuScore = Math.Abs(t.Pitch) + Math.Abs(t.Roll);
            riskScore += imuScore;
            Raise("IMU Instability", imuScore, "Pitch/Roll out of safe bounds");
        }

        // GPS Sapma
        if (Math.Abs(t.Latitude) > 0.001 || Math.Abs(t.Longitude) > 0.001)
        {
            riskScore += 45;
            Raise("GPS Drift", riskScore, "Drone is drifting from expected route");
        }
    }

    private void Raise(string type, double score, string desc)
    {
        OnAnomaly?.Invoke(this, new AnomalyEventArgs
        {
            Type = type,
            Score = Math.Min(score, 100),
            Description = desc
        });
    }
}

public class MissionEngine
{
    private int currentWaypointIndex = 0;
    private List<(double lat, double lon, double alt)> waypoints;

    public MissionEngine()
    {
        waypoints = new()
        {
            (41.0, 29.0, 120),
            (41.01, 29.01, 140),
            (41.015, 29.015, 150)
        };
    }

    public void Process(Telemetry t)
    {
        var wp = waypoints[currentWaypointIndex];

        double dist = Math.Sqrt(
            Math.Pow(t.Latitude - wp.lat, 2) +
            Math.Pow(t.Longitude - wp.lon, 2)
        );

        if (dist < 0.0005)
        {
            Console.WriteLine($"📍 Waypoint {currentWaypointIndex + 1} reached.");
            currentWaypointIndex++;

            if (currentWaypointIndex >= waypoints.Count)
            {
                Console.WriteLine("🎯 Mission Completed!");
                currentWaypointIndex = waypoints.Count - 1;
            }
        }
    }
}

public class Program
{
    public static void Main()
    {
        var anomaly = new UAVAnomalyEngine();
        var mission = new MissionEngine();
        var drone = new DroneState();

        anomaly.OnAnomaly += (s, e) =>
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"🚨 ANOMALY [{e.Type}] | Score={e.Score:F1} → {e.Description}");
            Console.ResetColor();
        };

        Random rnd = new Random();

        // TELEMETRY LOOP
        for (int i = 0; i < 200; i++)
        {
            var tel = new Telemetry
            {
                Altitude = 100 + Math.Sin(i / 10.0) * 3,
                Latitude = 41 + rnd.NextDouble() / 5000,
                Longitude = 29 + rnd.NextDouble() / 5000,
                Pitch = rnd.NextDouble() * 10,
                Roll = rnd.NextDouble() * 10,
                Yaw = rnd.NextDouble() * 180,
                Speed = 10 + rnd.NextDouble() * 2
            };

            // senaryo: 100. adımda ani düşüş
            if (i == 100) tel.Altitude = 20;

            // senaryo: 150. adımda GPS kayması
            if (i == 150) { tel.Latitude += 0.01; tel.Longitude += 0.01; }

            anomaly.ProcessTelemetry(tel, drone);
            mission.Process(tel);

            Console.WriteLine($"Telemetri: Alt={tel.Altitude:F2} Pitch={tel.Pitch:F1} Roll={tel.Roll:F1}");
        }
    }

    private void SimulateFlight(UAVAnomalyEngine uAVAnomalyEngine, object anomaly, MissionEngine missionEngine, object mission, DroneState droneState, object drone)
    {
        throw new NotImplementedException();
    }
}



    Random rnd = new Random();

    for (int i = 0; i < 200; i++)
    {
        var tel = new Telemetry
        {
            Altitude = 100 + Math.Sin(i / 10.0) * 3,
            Latitude = 41 + rnd.NextDouble() / 5000,
            Longitude = 29 + rnd.NextDouble() / 5000,
            Pitch = rnd.NextDouble() * 10,
            Roll = rnd.NextDouble() * 10,
            Yaw = rnd.NextDouble() * 180,
            Speed = 10 + rnd.NextDouble() * 2
        };

        // Senaryolar
        if (i == 50) tel.Altitude = 20;       // Ani düşüş
        if (i == 100) { tel.Latitude += 0.01; tel.Longitude += 0.01; } // GPS sapması
        if (i == 150) { tel.Pitch = 40; tel.Roll = 38; }                // IMU bozulması

        anomaly.ProcessTelemetry(tel, drone);
        mission.Process(tel);

        Console.WriteLine($"[Step {i}] Alt={tel.Altitude:F2} Lat={tel.Latitude:F4} Lon={tel.Longitude:F4} Pitch={tel.Pitch:F1} Roll={tel.Roll:F1}");
    }
}



