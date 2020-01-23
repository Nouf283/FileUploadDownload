using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace UoloadDownload
{
    class Program
    {
        static void Main(string[] args)
        {

          //  ---------------------------------- Upload--------------------------------
            var path = (@"D:\work\nature.jpg");
            FileInfo fileInfo = new FileInfo(path);
            byte[] data = new byte[fileInfo.Length];
            using (FileStream fs = fileInfo.OpenRead())
            {
                fs.Read(data, 0, data.Length);
            }

            using (SqlConnection sqlconnection = new SqlConnection(@"Data Source=cel89; 
                Initial Catalog=MyDatabase; Integrated Security=SSPI;"))
            {
                sqlconnection.Open();

                // create table if not exists 
                string createTableQuery = @"Create Table [MyTable](ID int, [BinData] varbinary(max))";
                SqlCommand command = new SqlCommand(createTableQuery, sqlconnection);
                command.ExecuteNonQuery();

                string insertXmlQuery = @"Insert Into [MyTable] (ID,[BinData]) Values(1,@BinData)";

                // Insert Byte [] Value into Sql Table by SqlParameter
                SqlCommand insertCommand = new SqlCommand(insertXmlQuery, sqlconnection);
                SqlParameter sqlParam = insertCommand.Parameters.AddWithValue("@BinData", data);
                sqlParam.DbType = DbType.Binary;
                insertCommand.ExecuteNonQuery();
            }

            //  ---------------------------------- Download--------------------------------

            var path = (@"D:\work");
            string con = @"Data Source=cel89;Initial Catalog=MyDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection connection = new SqlConnection(con);
            connection.Open();
            string sql = "select BinData from Mytable where id=1";
            SqlCommand cmd = new SqlCommand(sql, connection);
            SqlDataReader dr = cmd.ExecuteReader();
            byte[] data = null;
            while (dr.Read())
            {
                data = (byte[])dr[0];
            }
            
            using (var fs = new FileStream(Path.Combine("D:\\", "nouf.jpg"), FileMode.Create, FileAccess.Write))
            {
                fs.Write(data, 0, data.Length);
            }


            Console.WriteLine("success");

        }
    }
}
