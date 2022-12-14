using System.Text;

namespace CsvComparer
{
    internal class Program
    {

        static void Main(string[] args)
        {
            string table = "Task.csv";
            string path1 = "C:\\Users\\user\\Desktop\\compare\\Redshift\\"+table;
            string path2 = "C:\\Users\\user\\Desktop\\compare\\Salesforce\\"+table;

            var fileComparer = new FileComparer();

            var result = fileComparer.AreEqualAsync(path1, path2);

            Console.WriteLine(result.Result);
            Console.ReadLine();
        }
    }
}