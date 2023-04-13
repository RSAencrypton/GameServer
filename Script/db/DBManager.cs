using System;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using GameServer.Script.logic;
namespace GameServer.Script.db
{
	public class DBManager
	{

		public static MySqlConnection mySQL;

		//连接数据库
		public static bool ConnectSQL(string server,string db, string userID, string password) {
            mySQL = new MySqlConnection();
			string str = string.Format("SERVER={0}; DATABASE={1}; UID={2}; PASSWORD={3};",
				server, db, userID, password);
			mySQL.ConnectionString = str;

			try
			{
				mySQL.Open();
				Console.WriteLine("数据库连接成功");
				return true;
			}
			catch (Exception e) {
				Console.WriteLine("数据库连接失败：" + e.ToString());
				return false;
			}
		}

		//检查安全字符
		private static bool IsSafeString(string str) {
			return !Regex.IsMatch(str, @"[-|; |, |\/|\(|\)|\[|\]|\}|\{|%|@|\*|! |\']");
		}

		public static bool IsHasAccount(string ID) {
			//防止SQL注入
			if (!IsSafeString(ID)) {
				return false;
			}
			//创建查找对应ID的SQL语句
			string str = string.Format("select * from account where ID = '{0}';", ID);
			//查询是否存在对应数据，并且检查是否存在异常
			try
			{
				MySqlCommand cmd = new MySqlCommand(str, mySQL);
				MySqlDataReader dataReader = cmd.ExecuteReader();
				bool hasRow = dataReader.HasRows;
				dataReader.Close();
				return !hasRow;
			}
			catch (Exception e) {
				Console.WriteLine("数据库检测相同账号出错：" + e.ToString());
				return false;
			}
		}

		public static bool RegisterAccount(string ID, string password) {
			//防止SQL注入
			if (!IsSafeString(ID)) {
				Console.WriteLine("ID存在风险");
				return false;
			}
            if (!IsSafeString(password))
            {
                Console.WriteLine("密码存在风险");
                return false;
            }
			//是否存在相同账号
			if (!IsHasAccount(ID)) {
				Console.WriteLine("已经存在相同用户");
				return false;
			}
			//写入数据库
			string str = string.Format("insert into account set id='{0}', password='{1}';",
				ID, password);

			try
			{
				MySqlCommand sqlCommand = new MySqlCommand(str, mySQL);
				sqlCommand.ExecuteNonQuery();
				return true;
			}
			catch (Exception e) {
				Console.WriteLine("注册程序意外出错：" + e.ToString());
				return false;
			}
        }

		public static bool CreatePlayer(string id) {
			if (!DBManager.IsSafeString(id)) {
				Console.WriteLine("该ID有风险");
				return false;
			}

			PlayerData playerData = new PlayerData();
			string str = JsonConvert.SerializeObject(playerData);
			string sqlCMD = string.Format("insert into player set ID='{0}', data='{1}'",
				id, str);
			try
			{
				MySqlCommand cmd = new MySqlCommand(sqlCMD, mySQL);
				cmd.ExecuteNonQuery();
				return true;
			}
			catch (Exception e) {
				Console.WriteLine("玩家数据写入错误：" + e.ToString());
				return false;
			}
		}

		public static bool CheckPassword(string id, string pw) {
            //防止SQL注入
            if (!IsSafeString(id))
            {
                Console.WriteLine("ID存在风险");
                return false;
            }
            if (!IsSafeString(pw))
            {
                Console.WriteLine("密码存在风险");
                return false;
            }

			string str = string.Format("select * from account where ID='{0}' and password='{1}'",
				id, pw);
			try
			{
				MySqlCommand sqlCommand = new MySqlCommand(str, mySQL);
				MySqlDataReader dataReader = sqlCommand.ExecuteReader();
				bool hasRow = dataReader.HasRows;
				dataReader.Close();
				return hasRow;
			}
			catch (Exception e) {
				Console.WriteLine("验证密码错误：" + e.ToString());
				return false;
			}
        }

		public static PlayerData GetPlayerDataByID(string id) {
			if (!DBManager.IsSafeString(id)) {
				Console.WriteLine("ID存在风险！！！");
				return null;
			}

			string str = string.Format("select * from player where ID='{0}'",
				id);

			try
			{
				MySqlCommand cmd = new MySqlCommand(str, mySQL);
				MySqlDataReader dataReader = cmd.ExecuteReader();
				bool hasData = dataReader.HasRows;

				if (!hasData) {
					Console.WriteLine("玩家数据没有存入player表中");
					dataReader.Close();
					return null;
				}

				dataReader.Read();
				string data = dataReader.GetString("data");
				Type type = Type.GetType("PlayerData");
				PlayerData playerData = JsonConvert.DeserializeObject<PlayerData>(data);
				dataReader.Close();
				return playerData;
			}
			catch (Exception e) {
				Console.WriteLine("玩家数据获取失败：" + e.ToString());
				return null;
			}
		}

		public static bool UpdatePlayerData(string id, PlayerData playerData) {
			if (!DBManager.IsSafeString(id)) {
				Console.WriteLine("该ID存在风险");
				return false;
			}

			string str = JsonConvert.SerializeObject(playerData);

			string str1 = string.Format("update player set data='{0}' where ID='{1}'",
				str, id);

			try
			{
				MySqlCommand cmd = new MySqlCommand(str1, mySQL);
				cmd.ExecuteNonQuery();
				return true;
			}
			catch (Exception e) {
				Console.WriteLine("更新数据错误：" + e.ToString());
				return false;
			}
		}
	}
}

