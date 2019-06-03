using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System;
using System.Net;
using System.Web;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;



namespace Rextester
{
   
     class Program
    {
        
        String url = "https://www.googleapis.com/books/v1/volumes?key=YOURKEY&q=";
        // q=Kings+inauthor:Jayaraja

        String author, title;

       public SQLiteConnection m_dbConnection;
        public SQLiteCommand command;
        public String sql;
        System.IO.StreamWriter writetextr;


        //accesing the API(google books with author name and book name)
        private void createjson(string a , string b)
        {
            Program n = new Program();
            url = url + a + "+inauthor:" + b;

            Console.WriteLine(url);

            Console.WriteLine("kindly wait for 30 seconds for the output");
            



            using (WebClient wc = new WebClient())
            {
                var json = wc.DownloadString(url);

                var details = JObject.Parse(json);

                //  string count = string.Concat(details["totalItems"]);

               // Console.WriteLine(details);
               //search operation with key and array (passed parameters)
                int t = Convert.ToInt32(details["totalItems"]);
                for (int i = 0; i < t; i++)
                {
                   
                     string title = Convert.ToString(details["items"][i]["volumeInfo"]["title"]);
                     string publisher = Convert.ToString(details["items"][i]["volumeInfo"]["publisher"]);
                     string authorname = Convert.ToString(details["items"][i]["volumeInfo"]["authors"][0]);
                     string isbn = Convert.ToString(details["items"][i]["volumeInfo"]["industryIdentifiers"][0]["type"]); ;

                    Console.WriteLine("Author : " + authorname);
                    Console.WriteLine("Title : "+ title);
                   
                    Console.WriteLine("Publisher : " + publisher);

                    Console.WriteLine("isbn : " + isbn);

                    n.inserttable('"'+authorname+ '"', '"'+title+ '"', '"'+publisher+ '"', '"'+isbn+ '"');
                    n.writetext(authorname, title, publisher, isbn);
                }

                //   Int32.TryParse(details["totalItems"], out int count);

                //  Console.WriteLine(details["items"][1]["volumeInfo"]["publisher"]);
               



            }

        }
        // creates Database 
         public void createdb()
        {

            SQLiteConnection.CreateFile("MyDatabase.sqlite");

            SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
            m_dbConnection.Open();
            string sql = "create table Book (Author varchar(300), Title varchar(300),Publisher varchar(300),ISBN varchar(300))";

            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();

            
        }
        //inserts the value searched and returnrd in to the table
        public void inserttable(String a,String b,String c,String d)
        {
          //  SQLiteConnection.CreateFile("MyDatabase.sqlite");
            m_dbConnection = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
            m_dbConnection.Open();
            sql = "insert into Book (Author, Title,Publisher,ISBN) values (" + a+","+b+","+c+","+d+")";

            command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
            Console.WriteLine("Inserted into DB ");

        }
      
        //writes the searched return values into the file 
        private void writetext(String a, String b, String c, String d)
        {
            //  sql = "select * from temp order by score desc";
            //  command = new SQLiteCommand(sql, m_dbConnection);
            //  SQLiteDataReader reader = command.ExecuteReader();
            
                //  while (reader.Read())
                //   {
                using (FileStream aFile = new FileStream("mybook.txt", FileMode.Append, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(aFile))
                {


                    
                    sw.WriteLine("Author: " + a + "\tTitle: " + b + "\tPublisher: " + c + "\tISBN: " + d);
                    //     }
                }
            Console.WriteLine("writetext to file completed");
            
        }

        public void closemydb()
        {
            m_dbConnection = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
            m_dbConnection.Open();
            m_dbConnection.Close();
        }

        public static void Main(string[] args)
        {
            
            Program n = new Program();


              Console.WriteLine("Enter the author");
            // gets the value of the author to be searched
              n.author = Console.ReadLine();

              Console.WriteLine("Enter the title");
            //gets the value of the book_name to be searched
            n.title = Console.ReadLine();
            n.createdb();
           
            Console.WriteLine();
            n.createjson(n.title,n.author);
           
            n.closemydb();
            




        }
    }
}