using System;
using Microsoft.Data.Sqlite;

namespace TallerAdoNetSqlite
{
    class Program
    {
        static void Main(string[] args)
        {
            // Cadena de conexión: crea automáticamente el archivo inventario_juegos.db si no existe
            string cadenaConexion = "Data Source=inventario_juegos.db";

            Console.WriteLine("=== GESTIÓN DE VIDEOJUEGOS CON ADO.NET Y SQLITE ===\n");

            // 1. Establecer conexión usando 'using' para cerrar y liberar recursos automáticamente
            using (SqliteConnection conexion = new SqliteConnection(cadenaConexion))
            {
                conexion.Open();
                Console.WriteLine("[OK] Conexión establecida con SQLite.\n");

                // 2. Crear la tabla con 4 columnas (Id, Titulo, Genero, Precio)
                string sqlCrearTabla = @"
                    CREATE TABLE IF NOT EXISTS Videojuegos (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Titulo TEXT NOT NULL,
                        Genero TEXT NOT NULL,
                        Precio REAL NOT NULL
                    );";

                using (SqliteCommand cmdTabla = new SqliteCommand(sqlCrearTabla, conexion))
                {
                    cmdTabla.ExecuteNonQuery();
                    Console.WriteLine("[OK] Tabla 'Videojuegos' verificada/creada.");
                }

                // Limpiamos registros previos para que cada ejecución de prueba muestre exactamente los 3 registros
                using (SqliteCommand cmdLimpiar = new SqliteCommand("DELETE FROM Videojuegos;", conexion))
                {
                    cmdLimpiar.ExecuteNonQuery();
                }

                // 3. Insertar al menos 3 registros usando SqliteCommand y Parameters
                InsertarVideojuego(conexion, "The Legend of Zelda: Tears of the Kingdom", "Aventura", 249000);
                InsertarVideojuego(conexion, "EA Sports FC 25", "Deportes", 279000);
                InsertarVideojuego(conexion, "Hollow Knight", "Metroidvania", 45000);

                Console.WriteLine("[OK] Se insertaron 3 videojuegos correctamente.\n");

                // 4. Consultar y mostrar los registros almacenados mediante SqliteDataReader
                Console.WriteLine("--- LISTADO DE VIDEOJUEGOS EN BASE DE DATOS ---");
                string sqlConsulta = "SELECT Id, Titulo, Genero, Precio FROM Videojuegos;";

                using (SqliteCommand cmdConsulta = new SqliteCommand(sqlConsulta, conexion))
                using (SqliteDataReader lector = cmdConsulta.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        int id = lector.GetInt32(0);
                        string titulo = lector.GetString(1);
                        string genero = lector.GetString(2);
                        double precio = lector.GetDouble(3);

                        Console.WriteLine($"ID: {id} | Título: {titulo} | Género: {genero} | Precio: ${precio:N0} COP");
                    }
                }
            } // Aquí 'using' cierra automáticamente la conexión con SQLite

            Console.WriteLine("\n[OK] Conexión cerrada y recursos liberados.");
        }

        static void InsertarVideojuego(SqliteConnection conexion, string titulo, string genero, double precio)
        {
            string sqlInsertar = @"
                INSERT INTO Videojuegos (Titulo, Genero, Precio) 
                VALUES (@Titulo, @Genero, @Precio);";

            using (SqliteCommand cmdInsertar = new SqliteCommand(sqlInsertar, conexion))
            {
                // Uso de Parameters para seguridad y limpieza de tipos de datos
                cmdInsertar.Parameters.AddWithValue("@Titulo", titulo);
                cmdInsertar.Parameters.AddWithValue("@Genero", genero);
                cmdInsertar.Parameters.AddWithValue("@Precio", precio);

                cmdInsertar.ExecuteNonQuery();
            }
        }
    }
}