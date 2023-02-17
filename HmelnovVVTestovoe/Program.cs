// See https://aka.ms/new-console-template for more information
using Microsoft.VisualBasic;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using MySqlX.XDevAPI.Relational;
using Org.BouncyCastle.Asn1.X500;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Xml.Linq;

string ConnectionString = "Server=localhost;Port=3306;Database=ptmk;User=root;Password=root";

MySqlConnection conn = new MySqlConnection(ConnectionString);
conn.Open();

Console.WriteLine("Введите команду или 0 для завершения работы программы");

string param = "0";
param = Console.ReadLine();
string tableName = "zapusk";
while (param != "0")
{
    string param1 = param.Substring(0,7);
    switch (param1)
    {
        case "myApp 1":
            tableName += Convert.ToString(DateTime.UtcNow.Minute);
            MyApp1(conn,tableName);
        break;
        case "myApp 2":
            string param2 = param.Substring(7);
            MyApp2(conn,param2,tableName);
        break;

        case "myApp 3":
            MyApp3(conn, tableName);
            break;

        case "myApp 4":
            MyApp4(conn, tableName);
            Console.WriteLine("всё");
            break;

        case "myApp 5":
            MyApp5(conn, tableName);
            break;

        case "myApp 6":
            MyApp6(conn, tableName);
            break;
        

        default:
            Console.WriteLine("Вы ввели неправильную команду");
            param = "1";
        break;
    }
    param = Console.ReadLine();
}
Console.WriteLine("Программа завершена");

void MyApp1(MySqlConnection conn,string tableName)
{
    string pomoch = "CREATE TABLE " + tableName +
        " (" +
    "ID int NOT NULL AUTO_INCREMENT," +
    "FIO varchar(255)," +
    "BirthDay DATE," +
    "Pol varchar(255)," +
    "PRIMARY KEY(ID));";
    MySqlCommand command = new MySqlCommand(pomoch, conn);

    command.ExecuteNonQuery();
}

void MyApp2(MySqlConnection conn, string parametrs,string tableName)
{
    string pomoch = "INSERT INTO " + tableName + "(FIO, BirthDay,Pol) VALUES (@fio, @birthday, @pol);"; 
    MySqlCommand command = new MySqlCommand(pomoch, conn);
    string fio = "";
    string birthday = "";
    string pol = "";

    String[] words = parametrs.Split(new Char[] { ' ','\t', '\r', '\n' },
StringSplitOptions.RemoveEmptyEntries);

    fio = words[0]+' ' + words[1]+' '+ words[2];
    birthday = words[3];
    pol = words[4];

    MySqlParameter fioParam = new MySqlParameter("@fio", fio);
    MySqlParameter birthdayParam = new MySqlParameter("@birthday", birthday);
    MySqlParameter polParam = new MySqlParameter("@pol", pol);
    command.Parameters.Add(fioParam);
    command.Parameters.Add(birthdayParam);
    command.Parameters.Add(polParam);

    command.ExecuteNonQuery();
}

void MyApp3(MySqlConnection conn, string tableName)
{
    string pomoch = "SELECT DISTINCT FIO, BirthDay, Pol, " +
        "DATEDIFF('2023-02-17', `BirthDay`) div 365 as year " +
        " FROM " + tableName + " ORDER BY FIO;";
    MySqlCommand command = new MySqlCommand(pomoch, conn);

    MySqlDataReader reader = command.ExecuteReader();
    if (reader.HasRows)
    {
        Console.WriteLine("{0}\t{1}\t{2}\t{3}", reader.GetName(0), reader.GetName(1), reader.GetName(2), reader.GetName(3));



        while (reader.Read())
        {
            object fio = reader.GetValue(0);
            object birthdate = reader.GetValue(1);
            object pol = reader.GetValue(2);
            object year = reader.GetValue(3);

            Console.WriteLine($"{reader[0]} {reader[1]} {reader[2]} {reader[3]}");
        }
    }

    reader.Close();
    reader.Dispose();
}

void MyApp4(MySqlConnection conn, string tableName)
{
    var startDate = DateTime.Now.AddYears(-100);
    var endDate = DateTime.Now;
    char[] letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
    Random rand = new Random();
    for (int i = 0; i < 1000000; i++)
    {
        int kol = rand.Next(2, 5);
        string F = "";
        string I = "";
        string O = "";
        string pol = "";
        for (int j = 0; j < kol; j++)
        {
            int letter_numF = rand.Next(0, letters.Length - 1);
            F += letters[letter_numF];
            int letter_numI = rand.Next(0, letters.Length - 1);
            I += letters[letter_numI];
            int letter_numO = rand.Next(0, letters.Length - 1);
            O += letters[letter_numO];
        }

        int randomYear = rand.Next(startDate.Year, endDate.Year);
        int randomMonth = rand.Next(1, 12);
        int randomDay = rand.Next(1, DateTime.DaysInMonth(randomYear, randomMonth));

        switch (rand.Next(2))
        {
            case 0:
                pol = "M";
                break;
            case 1:
                pol = "W";
                break;
        }
        
        if ((i+1)%10000 == 0)
        {
            F = "F" + F;
        }
        string birthday = "";
        string fio = F + ' ' + I + ' ' + O;
        birthday = "\'" + Convert.ToString(randomYear) + "." +
            Convert.ToString(randomMonth) + "." + Convert.ToString(randomDay) +"\'";
        
        string pomoch = "INSERT INTO " + tableName + " (FIO, BirthDay,Pol) VALUES (@fio, "+birthday+", @pol);";
        MySqlCommand command = new MySqlCommand(pomoch, conn);

        MySqlParameter fioParam = new MySqlParameter("@fio", fio);
        MySqlParameter birthdayParam = new MySqlParameter("@birthday", birthday);
        MySqlParameter polParam = new MySqlParameter("@pol", pol);
        command.Parameters.Add(fioParam);
        command.Parameters.Add(birthdayParam);
        command.Parameters.Add(polParam);

        command.ExecuteNonQuery();
    }
}

void MyApp5(MySqlConnection conn, string tableName)
{
    Stopwatch stopwatch = new Stopwatch();
    stopwatch.Start();

    string pomoch = "SELECT *  FROM " + tableName + " WHERE (FIO LIKE 'F%' AND Pol LIKE 'M%');";
    MySqlCommand command = new MySqlCommand(pomoch, conn);
    MySqlDataReader reader = command.ExecuteReader();
    if (reader.HasRows)
    {
        Console.WriteLine("{0}\t{1}\t{2}\t{3}", reader.GetName(0), reader.GetName(1), reader.GetName(2), reader.GetName(3));



        while (reader.Read())
        {
            object id = reader.GetValue(0);
            object fio = reader.GetValue(1);
            object birthdate = reader.GetValue(2);
            object pol = reader.GetValue(3);

            Console.WriteLine($"{reader[0]}  {reader[1]} {reader[2]} {reader[3]}");


        }
    }
    stopwatch.Stop();
    reader.Close();
    reader.Dispose();
    Console.WriteLine("Затраченное время: " + stopwatch.ElapsedMilliseconds);
    Console.WriteLine("Для уменьшения времени запроса введите myApp 6");
}
void MyApp6(MySqlConnection conn, string tableName)
{
    string pomoch = "ALTER TABLE " + tableName + " ADD (`F` varchar(10), `I` varchar(10), `O` varchar(10));";
    MySqlCommand command = new MySqlCommand(pomoch, conn);
    command.ExecuteNonQuery();

    pomoch = "UPDATE " + tableName +
        " SET F=SUBSTRING_INDEX(FIO, ' ', 1)," +
        "I = SUBSTRING_INDEX(SUBSTRING_INDEX(FIO, ' ', 2), ' ', -1)," +
        "O = SUBSTRING_INDEX(FIO, ' ', -1);";

    MySqlCommand command1 = new MySqlCommand(pomoch, conn);
    command1.ExecuteNonQuery();

    pomoch = "ALTER TABLE " + tableName +" DROP COLUMN FIO;";
    MySqlCommand command2 = new MySqlCommand(pomoch, conn);
    command2.ExecuteNonQuery();

    Stopwatch stopwatch = new Stopwatch();
    stopwatch.Start();

    pomoch = "SELECT F,I,O,BirthDay,Pol  FROM " + tableName + " WHERE (F LIKE 'F%' AND Pol LIKE 'M%');";
    MySqlCommand command3 = new MySqlCommand(pomoch, conn);
    MySqlDataReader reader = command3.ExecuteReader();
    if (reader.HasRows) 
    {
        Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", reader.GetName(0), reader.GetName(1), reader.GetName(2), 
            reader.GetName(3), reader.GetName(4));



        while (reader.Read())
        {
            object f = reader.GetValue(0);
            object i = reader.GetValue(1);
            object O = reader.GetValue(2);
            object birthdate = reader.GetValue(3);
            object pol = reader.GetValue(4);

            Console.WriteLine($"{reader[0]}  {reader[1]} {reader[2]} {reader[3]} {reader[4]}");

            
        }
    }
    stopwatch.Stop();
    reader.Close();
    reader.Dispose();
    Console.WriteLine("Затраченное время: " + stopwatch.ElapsedMilliseconds);
}

