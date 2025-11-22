
public static void SimulateFlight(UAVAnomalyEngine anomaly, MissionEngine mission, DroneState drone)
{
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
