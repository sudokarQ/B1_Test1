using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B1_Test1
{
    public class OperationClass
    {
        public static int numberOfStrings = 100000;
        public static int numberOfFiles = 100;
        public static string? bdName = "master";


        public static void CreateFilesFunc()
        {
            CreateLines txt = new CreateLines();
            StringBuilder sb = new StringBuilder();
            string folderName = Directory.GetCurrentDirectory() + @"\" + "Generated Files";
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(folderName);
                directoryInfo.Create();
                int i = 0;
                while (i < numberOfFiles) // не забыть поменять
                {
                    sb.Clear();
                    int j = 0;
                    while (j < numberOfStrings)
                    {
                        sb.Append(txt.ToString());
                        sb.Append("\r\n");
                        j++;
                    }
                    File.WriteAllText(folderName + @"\" + i + ".txt", sb.ToString());
                    i++;
                }
                Console.WriteLine("Файлы успешно созданы");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        public static void MergeFiles()
        {
            string finalFileName = Directory.GetCurrentDirectory() + @"\" + "Generated Files" + @"\" + "fileOutput.txt";
            Console.WriteLine("Введите текст, строку с которым надо удалить");
            string? text = Console.ReadLine();
            if(String.IsNullOrEmpty(text))
            {
                text = "//";
            }
            if (File.Exists(finalFileName))
            {
                File.Delete(finalFileName);
            }

            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + @"\" + "Generated Files");
            
            for (int i = 0; i < files.Length; i++)
            {
                File.AppendAllLines(finalFileName,
    File.ReadLines(files[i]).Where(l => !l.Contains(text)).ToList());
            }
            int count = File.ReadAllLines(finalFileName).Length;
            Console.WriteLine($"Удалено строк: {numberOfFiles * numberOfStrings - count}");
            Console.WriteLine("Файл создан");
        }

        public static void ImportToServer()
        {
            Console.WriteLine("Введите название базы данных. БД по умолчанию - master");
            bdName = Console.ReadLine();
            if (string.IsNullOrEmpty(bdName))
            {
                bdName = "master";
            }
            string createQuery;
            SqlConnection myConn = new SqlConnection();
            myConn.ConnectionString = $"Server=localhost\\SQLExpress;Database={bdName};Trusted_Connection=True;";
            //myConn.ConnectionString = "Server=localhost\\SQLExpress;Database=master;Trusted_Connection=True;";
            Console.WriteLine("Если есть подходящая таблица, нажмите Enter, если нет - введите 'n', она будет создана под именем TableResult");
            if (Console.ReadLine() == "n" || Console.ReadLine() == "N")
            {
                createQuery = $"CREATE TABLE TableResult" +
                "(dateF DATE, lat NVARCHAR(10), rus NVARCHAR(10), number BIGINT, numb NVARCHAR(12))";
                //str = $"CREATE TABLE TableResult" +
                //"(id UniqueIdentifier CONSTRAINT PKeyid PRIMARY KEY," +
                //"dateF DATE, lat NVARCHAR(10), rus NVARCHAR(10), number INT, numb FLOAT)";
                SqlCommand myCommand = new SqlCommand(createQuery, myConn);
                try
                {
                    myConn.Open();
                    myCommand.ExecuteNonQuery();
                    Console.WriteLine("Таблица успешно создана");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    if (myConn.State == ConnectionState.Open)
                    {
                        myConn.Close();
                    }
                }
            }
            Console.WriteLine("Начинается импорт данных");

            myConn.Open();
            string finalFileName = Directory.GetCurrentDirectory() + @"\" + "Generated Files" + @"\" + "fileOutput.txt";
            StreamReader SourceFile = new StreamReader(finalFileName);

            string? line = "";

            int counter = 0;
            int stringCount = File.ReadAllLines(finalFileName).Length;
            SqlCommand myCommand1;
            try
            {
                while ((line = SourceFile.ReadLine()) != null)
                {
                    line = line.Substring(0, line.Length - 2);
                    string query = "Insert into TableResult" +
                           $" Values ('" + line.Replace("||", "','") + "')";

                    myCommand1 = new SqlCommand(query, myConn);
                    myCommand1.ExecuteNonQuery();
                    counter++;
                    // Console.WriteLine($"Имортировано строк - {counter}. Осталось строк - {stringCount - counter}");

                }
            }
            
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                SourceFile.Close();
                myConn.Close();
            }
            
            Console.WriteLine($"Общее число импортированных строк - {counter}");

        }

        public static void ShowSumAndMed()
        {
            SqlConnection myConn = new SqlConnection();
            myConn.ConnectionString = $"Server=localhost\\SQLExpress;Database={bdName};Trusted_Connection=True;";
            myConn.Open();
            SqlCommand medianCommand = myConn.CreateCommand();
            medianCommand.CommandText = "SELECT TOP (1) Percentile_Disc (0.5) WITHIN GROUP(ORDER BY Cast (replace(numb,',','.') as Float )) OVER() AS 'Mediana' FROM TableResult; ";
            //medianCommand.CommandText = "SELECT ((SELECT MAX(Cast(replace(numb, ',', '.') as Float)) AS 'Mediana' FROM (SELECT TOP 50 PERCENT numb FROM TableResult ORDER BY numb) AS BottomHalf) + (SELECT MIN(Cast(replace(numb, ',', '.') as Float)) FROM (SELECT TOP 50 PERCENT numb FROM TableResult ORDER BY numb DESC) AS TopHalf) ) / 2 AS Median;";
            medianCommand.CommandTimeout = 600;
            SqlCommand sumCommand = myConn.CreateCommand();
            //sumCommand.CommandText = "SELECT TOP (1) SUM(number) OVER(order by rus) AS SumResult FROM TableResult ORDER BY rus DESC; ";
            sumCommand.CommandText = "SELECT SUM(number) AS SumResult From TableResult; ";
            try
            {
                using (SqlDataReader fstReader = sumCommand.ExecuteReader())
                {
                    while (fstReader.Read())
                    {
                        Console.WriteLine("Сумма всех целых чисел равна " + fstReader["SumResult"]);
                    }
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            try
            {
                using (SqlDataReader secondReader = medianCommand.ExecuteReader())
                {
                    while (secondReader.Read())
                    {
                        Console.WriteLine("Медиана всех дробных чисел равна " + secondReader["Mediana"]);
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Таблицы по указанному адресу не существует, воспользуйтесь командой импорта, в процессе будет предложено создать таблицу");
            }

            myConn.Close();
        }
    }
    
}
