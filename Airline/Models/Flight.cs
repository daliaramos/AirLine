using System.Collections.Generic;
using System;
using Airline;
using MySql.Data.MySqlClient;

namespace Airline.Models
{
  public class Flight
  {
    private int _id;
    private string _departureTime;
    private string _arrivalTime;
    private string _status;

    public Flight(string departureTime, string arrivalTime, string status, int id = 0)
    {
      _departureTime = departureTime;
      _arrivalTime = arrivalTime;
      _status = status;
      _id = id;
    }

    public override bool Equals(System.Object otherFlight)
    {
      if (!(otherFlight is Flight))
      {
        return false;
      }
      else
      {
        Flight newFlight = (Flight) otherFlight;
        bool idEquality = (this.GetId() == newFlight.GetId());
        bool departureTimeEquality = (this.GetDepartureTime() == newFlight.GetDepartureTime());
        bool arrivalTimeEquality = (this.GetArrivalTime() == newFlight.GetArrivalTime());
        bool statusEquality = (this.GetStatus() == newFlight.GetStatus());
        return (idEquality && departureTimeEquality && arrivalTimeEquality && statusEquality);
      }
    }

    public override int GetHashCode()
    {
      return this.GetDepartureTime().GetHashCode();
    }

    public int GetId()
    {
      return _id;
    }

    public string GetDepartureTime()
    {
      return _departureTime;
    }

    public void SetDepartureTime(string departureTime)
    {
      _departureTime = departureTime;
    }

    public string GetArrivalTime()
    {
      return _arrivalTime;
    }

    public void SetArrivalTime(string arrivalTime)
    {
      _arrivalTime = arrivalTime;
    }

    public string GetStatus()
    {
      return _status;
    }

    public void SetStatus(string status)
    {
      _status = status;
    }

    public static List<Flight> GetAll()
    {
      List<Flight> allFlights = new List<Flight> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM flights;";
      MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int flightId = rdr.GetInt32(0);
        string flightDepartureTime = rdr.GetString(1);
        string flightArrivalTime = rdr.GetString(2);
        string flightStatus = rdr.GetString(3);
        Flight newFlight = new Flight(flightDepartureTime, flightArrivalTime, flightStatus, flightId);
        allFlights.Add(newFlight);
      }
      conn.Close();
      if (conn != null)
      {
          conn.Dispose();
      }
      return allFlights;
    }

    public static void DeleteAll()
    {
    MySqlConnection conn = DB.Connection();
     conn.Open();

     var cmd = conn.CreateCommand() as MySqlCommand;
     cmd.CommandText = @"DELETE FROM flights;";

     cmd.ExecuteNonQuery();

     conn.Close();
     if (conn != null)
     {
      conn.Dispose();
     }
    }

    public static Flight Find(int id)
    {
     MySqlConnection conn = DB.Connection();
     conn.Open();

     var cmd = conn.CreateCommand() as MySqlCommand;
     cmd.CommandText = @"SELECT * FROM `flights` WHERE id = @thisId;";

     MySqlParameter thisId = new MySqlParameter();
     thisId.ParameterName = "@thisId";
     thisId.Value = id;
     cmd.Parameters.Add(thisId);

     var rdr = cmd.ExecuteReader() as MySqlDataReader;

     int flightId = 0;
     string flightDeaprtureTime = "";
     string flightArrivalTime = "";
     string flightStatus = "";


     while (rdr.Read())
     {
       flightId = rdr.GetInt32(0);
       flightDeaprtureTime = rdr.GetString(1);
       flightArrivalTime = rdr.GetString(2);
       flightStatus = rdr.GetString(3);
     }

     Flight foundFlight= new Flight(flightDeaprtureTime, flightArrivalTime, flightStatus, flightId);

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }

     return foundFlight;
    }

    public void Save()
    {
      MySqlConnection conn = DB.Connection();
     conn.Open();

     var cmd = conn.CreateCommand() as MySqlCommand;

    cmd.CommandText = @"INSERT INTO `flights` (departure_time, arrival_time, status) VALUES (@FlightDepartureTime, @FlightArrivalTime, @FlightStatus);";

     MySqlParameter departureTime = new MySqlParameter();
     departureTime.ParameterName = "@FlightDepartureTime";
     departureTime.Value = this._departureTime;
     cmd.Parameters.Add(departureTime);

     MySqlParameter arrivalTime = new MySqlParameter();
     arrivalTime.ParameterName = "@FlightArrivalTime";
     arrivalTime.Value = this._arrivalTime;
     cmd.Parameters.Add(arrivalTime);

     MySqlParameter status = new MySqlParameter();
     status.ParameterName = "@FlightStatus";
     status.Value = this._status;
     cmd.Parameters.Add(status);

     cmd.ExecuteNonQuery();
     _id = (int) cmd.LastInsertedId;

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public void Delete()
    {
    MySqlConnection conn = DB.Connection();
    conn.Open();
    var cmd = conn.CreateCommand() as MySqlCommand;
    cmd.CommandText = @"DELETE FROM flights WHERE id = @FlightId; DELETE FROM categories_flights WHERE flight_id = @FlightId;";

    MySqlParameter flightIdParameter = new MySqlParameter();
    flightIdParameter.ParameterName = "@FlightId";
    flightIdParameter.Value = this.GetId();
    cmd.Parameters.Add(flightIdParameter);

    cmd.ExecuteNonQuery();
    if (conn != null)
    {
      conn.Close();
    }
    }

    public List<City> GetCities()
    {
     MySqlConnection conn = DB.Connection();
     conn.Open();
     var cmd = conn.CreateCommand() as MySqlCommand;
     cmd.CommandText = @"SELECT city_id FROM cities_flights WHERE flight_id = @flightId;";

     MySqlParameter flightIdParameter = new MySqlParameter();
     flightIdParameter.ParameterName = "@flightId";
     flightIdParameter.Value = _id;
     cmd.Parameters.Add(flightIdParameter);

     var rdr = cmd.ExecuteReader() as MySqlDataReader;

     List<int> cityIds = new List<int> {};
     while(rdr.Read())
     {
         int cityId = rdr.GetInt32(0);
         cityIds.Add(cityId);
     }
     rdr.Dispose();

     List<City> cities = new List<City> {};
     foreach (int cityId in cityIds)
     {
         var cityQuery = conn.CreateCommand() as MySqlCommand;
         cityQuery.CommandText = @"SELECT * FROM cities WHERE id = @CityId;";

         MySqlParameter cityIdParameter = new MySqlParameter();
         cityIdParameter.ParameterName = "@CityId";
         cityIdParameter.Value = cityId;
         cityQuery.Parameters.Add(cityIdParameter);

         var cityQueryRdr = cityQuery.ExecuteReader() as MySqlDataReader;
         while(cityQueryRdr.Read())
         {
             int thisCityId = cityQueryRdr.GetInt32(0);
             string cityName = cityQueryRdr.GetString(1);
             City foundCity = new City(cityName, thisCityId);
             cities.Add(foundCity);
         }
         cityQueryRdr.Dispose();
     }
     conn.Close();
     if (conn != null)
     {
         conn.Dispose();
     }
     return cities;
    }

    public void AddCity(City newCity)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO cities_flights (city_id, flight_id) VALUES (@CityId, @FlightId);";

      MySqlParameter city_id = new MySqlParameter();
      city_id.ParameterName = "@CityId";
      city_id.Value = newCity.GetId();
      cmd.Parameters.Add(city_id);

      MySqlParameter flight_id = new MySqlParameter();
      flight_id.ParameterName = "@FlightId";
      flight_id.Value = _id;
      cmd.Parameters.Add(flight_id);

      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
          conn.Dispose();
      }
    }

  }
}
