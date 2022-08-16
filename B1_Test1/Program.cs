using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B1_Test1
{
    public class Program
    {

        public static void Main(string[] args)
        {
            //CreateFiles.CreateFilesFunc();
            //CreateFiles.MergeFiles();
            //CreateFiles.ImportToServer();
            //CreateFiles.ShowSumAndMed();
            Console.WriteLine("Меню опций:");
            Console.WriteLine("Введите 1, чтобы создать новые 100 файлов");
            Console.WriteLine("Введите 2, чтобы объединить файлы в один");
            Console.WriteLine("Введите 3, чтобы перенести данные с общего файла в таблицу SQL");
            Console.WriteLine("Введите 4, чтобы вывести на экран сумму целых чисел и медиану дробных чисел");
            Console.WriteLine("Введите 0, чтобы выйти");
            string s;
            while(true)
            {
                s = Console.ReadLine();
                switch(s) {
                    case "1":
                        OperationClass.CreateFilesFunc();
                        break;
                    case "2":
                        OperationClass.MergeFiles();
                        break;
                    case "3":
                        OperationClass.ImportToServer();
                        break;
                    case "4":
                        OperationClass.ShowSumAndMed();
                        break;
                    case "0":
                        return;
                    default: Console.WriteLine("Введите команду");
                        break;
                }
            }

        }
    }
}