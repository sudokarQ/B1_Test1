using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B1_Test1
{
    public class CreateLines
    {
        static DateTime startDate = new DateTime(2017, 08, 09);
        int range = (DateTime.Today - startDate).Days;
        double minDouble = 1;
        double maxDouble = 20;
        const string engChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        const string ruChars = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя";
        public static Random rnd = new Random();
        

        DateTime GetRandomDate() => startDate.AddDays(rnd.Next(range));

        int GetRandomInt() => 2 * rnd.Next(1, 50000001);

        double GetRandomDouble() {
            var next = rnd.NextDouble();

            return Math.Round(minDouble + (next * (maxDouble - minDouble)), 8);
        }

        string GetRandomString(string charList)
        {
            string s = "";
            char[] chars = charList.ToCharArray();
            for (int i = 0; i < 10; i++)
            {
                s += chars[rnd.Next(chars.Length)];
            }
            return s;
        }


        

        public override string? ToString()
        {
            return GetRandomDate().ToString("d") + "||" + GetRandomString(engChars) + "||" + GetRandomString(ruChars) + "||" + GetRandomInt().ToString() + "||" + GetRandomDouble().ToString() + "||";
        }
    }
}
