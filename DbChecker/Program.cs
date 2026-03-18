using System;
using System.IO;
using MySqlConnector;

class Program
{
    static void Main()
    {
        string connectionString = "Server=mysql-1a6d9690-yaramiller35-f668.b.aivencloud.com;Port=13933;Database=mysmartdevicedb;Uid=avnadmin;Pwd=AVNS_O1UP-pYivOgcIZovdbI;SslMode=Required;";

        using var connection = new MySqlConnection(connectionString);
        connection.Open();

        using var writer = new StreamWriter("schema_dump.txt");

        PrintColumns(connection, writer, "soportes");
        PrintColumns(connection, writer, "propiedades");
        PrintColumns(connection, writer, "ubicaciones");
    }

    static void PrintColumns(MySqlConnection connection, StreamWriter writer, string tableName)
    {
        writer.WriteLine($"\n--- Columns for {tableName} ---");
        using var cmd = new MySqlCommand($"DESCRIBE {tableName}", connection);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            writer.WriteLine($"{reader.GetString("Field")} - {reader.GetString("Type")}");
        }
    }
}
