/* 
  Homework#4

  Add your name here: ----

  You are free to create as many classes within the Hw4.cs file or across 
  multiple files as you need. However, ensure that the Hw4.cs file is the 
  only one that contains a Main method. This method should be within a 
  class named hw4. This specific HashSetup is crucial because your instructor 
  will use the hw4 class to execute and evaluate your work.
  */
  // BONUS POINT:
  // => Used Pointers from lines 10 to 15 <=
  // => Used Pointers from lines 40 to 63 <=
  

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using AreaID = System.Int32;

namespace Main {
public class Area {
    public AreaID RecordNumber { get; set; }
    public string Zipcode { get; set; }
    public string ZipCodeType { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string LocationType { get; set; }
    public double? Lat { get; set; }
    public double? Long { get; set; }
    public double Xaxis { get; set; }
    public double Yaxis { get; set; }
    public double Zaxis { get; set; }
    public string WorldRegion { get; set; }
    public string Country { get; set; }
    public string LocationText { get; set; }
    public string Location { get; set; }
    public bool Decommisioned { get; set; }
    public int? TaxReturnsFiled { get; set; }
    public int? EstimatedPopulation { get; set; }
    public int? TotalWages { get; set; }

    // ~area = area's record number
    public static int operator ~(Area area) {
        return area.RecordNumber;
    }

    // ToString
    public override string ToString()
    {
        return $"Area(RecordNumber: {RecordNumber}, Zipcode: {Zipcode}, ZipCodeType: {ZipCodeType}, City: {City}, State: {State}, LocationType: {LocationType}, Lat: {Lat}, Long: {Long}, Xaxis: {Xaxis}, Yaxis: {Yaxis}, Zaxis: {Zaxis}, WorldRegion: {WorldRegion}, Country: {Country}, LocationText: {LocationText}, Location: {Location}, Decommisioned: {Decommisioned}, TaxReturnsFiled: {TaxReturnsFiled}, EstimatedPopulation: {EstimatedPopulation}, TotalWages: {TotalWages})";
    }
}
public class Hw4 {
    public static void Main(string[] args)
    {
        // Capture the start time
        // Must be the first line of this method
        DateTime startTime = DateTime.Now; // Do not change
        // ============================
        // Do not add or change anything above, inside the 
        // Main method
        // ============================

        List<Area> areas = GetAreas("zipcodes.txt");
        WriteCommonCityNames(areas);
        WriteLatLong(areas);
        WriteCityStates(areas);

        const AreaID testID = ~areas[20];
        Console.WriteLine($"Testing ID: {testID}");
        

        // ============================
        // Do not add or change anything below, inside the 
        // Main method
        // ============================

        // Capture the end time
        DateTime endTime = DateTime.Now;  // Do not change
        
        // Calculate the elapsed time
        TimeSpan elapsedTime = endTime - startTime; // Do not change
        
        // Display the elapsed time in milliseconds
        Console.WriteLine($"Elapsed Time: {elapsedTime.TotalMilliseconds} ms");
    }

    private static List<string> ReadFile(string file) {
        List<string> lines = new List<string>();
        using (StreamReader reader = new StreamReader(file))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
                lines.Add(line);
        }
        
        return lines;
    }

    public static List<Area> GetAreas(string file) {
        List<Area> areas = new List<Area>();
        List<string> lines = ReadFile(file);
        
        // Skip header
        for (int i = 1; i < lines.Count; i++)
        {
            string line = lines[i];
            
            char[] split = new char[] { '\t' };
            string[] parts = line.Split(split, StringSplitOptions.None);

            Area area = new Area
            {
                RecordNumber = int.Parse(parts[0]),
                Zipcode = parts[1],
                ZipCodeType = parts[2],
                City = parts[3],
                State = parts[4],
                LocationType = parts[5],
                Lat = parts[6] == "" ? null : (double?)double.Parse(parts[6]),
                Long = parts[7] == "" ? null : (double?)double.Parse(parts[7]),
                Xaxis = double.Parse(parts[8]),
                Yaxis = double.Parse(parts[9]),
                Zaxis = double.Parse(parts[10]),
                WorldRegion = parts[11],
                Country = parts[12],
                LocationText = parts[13],
                Location = parts[14],
                Decommisioned = parts[15] == "TRUE",
                TaxReturnsFiled = parts[16] == "" ? null : (int?)int.Parse(parts[16]),
                EstimatedPopulation = parts[17] == "" ? null : (int?)int.Parse(parts[17]),
                TotalWages = parts[18] == "" ? null : (int?)int.Parse(parts[18])
            };
            
            areas.Add(area);
        }

        return areas;
    }

    public static void WriteLatLong(List<Area> areas) {
        List<string> zips = ReadFile("zips.txt");
        Dictionary<string, string> zipsFound = new Dictionary<string, string>();

        foreach (Area area in areas)
        {
            // If the zip is in the list and not already found, add it.
            if (zips.Contains(area.Zipcode)
                && !zipsFound.ContainsKey(area.Zipcode))
                zipsFound[area.Zipcode] = $"{area.Lat} {area.Long}";
        }

        using (StreamWriter writer = new StreamWriter("LatLon.txt"))
        {
            foreach (KeyValuePair<string, string> pair in zipsFound)
                writer.WriteLine($"{pair.Value}");
        }
    }

    public static void WriteCityStates(List<Area> areas) {
        List<string> cities = ReadFile("cities.txt").Select(s => s.ToLower()).ToList();
        // Dictionary of cities mapped to every state they're in.
        Dictionary<string, HashSet<string>> cityStates = new Dictionary<string, HashSet<string>>();

        foreach (Area area in areas)
        {
            // Is this a city we want to write states for?
            if (cities.Contains(area.City.ToLower()))
            {
                // Create new HashSet for this city if we haven't seen it yet.
                if (!cityStates.ContainsKey(area.City))
                    cityStates[area.City] = new HashSet<string>();

                cityStates[area.City].Add(area.State);
            }
        }

        using (StreamWriter writer = new StreamWriter("CityStates.txt"))
        {
            foreach (KeyValuePair<string, HashSet<string>> pair in cityStates) {
                foreach (string state in pair.Value)
                    writer.Write($"{state} ");
                
                writer.WriteLine();
            }
        }
    }

    public static List<string> GetCommonCityNames(List<Area> areas, List<string> states = null) {
        HashSet<string> commonCityNames = new HashSet<string>();
        Dictionary<string, string> cities = new Dictionary<string, string>();

        foreach (Area area in areas)
        {
            if (states != null && !states.Contains(area.State))
                continue;

            else if (!cities.ContainsKey(area.City))
                cities[area.City] = area.State;
            
            else if (cities[area.City] != area.State)
                commonCityNames.Add(area.City);
        }

        List<string> sortedCommonCities = new List<string>(commonCityNames);
        sortedCommonCities.Sort();

        return sortedCommonCities;
    }

    public static void WriteCommonCityNames(List<Area> areas) {
        List<string> states = ReadFile("states.txt");

        List<string> commonCityNames = GetCommonCityNames(areas, states);

        // Write city names to file
        using (StreamWriter writer = new StreamWriter("CommonCityNames.txt"))
        {
            foreach (string city in commonCityNames)
                writer.WriteLine(city);
        }
    }
}
}