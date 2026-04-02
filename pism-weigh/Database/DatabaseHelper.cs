using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using pism_weigh.Models;

namespace pism_weigh.Database
{
    /// <summary>
    /// SQLite 数据库帮助类
    /// </summary>
    public class DatabaseHelper
    {
        private static readonly string DbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "weigh.db");
        private static string ConnectionString
        {
            get { return "Data Source=" + DbPath + ";Version=3;"; }
        }

        /// <summary>
        /// 初始化数据库
        /// </summary>
        public static void Initialize()
        {
            try
            {
                // 确保数据库目录存在
                string dbDir = Path.GetDirectoryName(DbPath);
                if (!Directory.Exists(dbDir))
                {
                    Directory.CreateDirectory(dbDir);
                }

                // 如果数据库文件不存在则创建
                if (!File.Exists(DbPath))
                {
                    SQLiteConnection.CreateFile(DbPath);
                }

                // 创建表结构
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();
                    
                    // 创建称重记录表
                    string createWeighRecordTable = @"
                        CREATE TABLE IF NOT EXISTS WeighRecords (
                            Id TEXT PRIMARY KEY,
                            PlateNumber TEXT,
                            Province TEXT,
                            PlateCode TEXT,
                            GrossWeight REAL,
                            TareWeight REAL,
                            NetWeight REAL,
                            CargoType TEXT,
                            Sender TEXT,
                            Receiver TEXT,
                            DriverName TEXT,
                            DriverPhone TEXT,
                            BusinessType INTEGER,
                            Status INTEGER,
                            FirstWeighTime DATETIME,
                            SecondWeighTime DATETIME,
                            CompleteTime DATETIME,
                            OperatorId TEXT,
                            OperatorName TEXT,
                            Remark TEXT,
                            PrintCount INTEGER DEFAULT 0,
                            IsUploaded INTEGER DEFAULT 0,
                            CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
                            UpdateTime DATETIME DEFAULT CURRENT_TIMESTAMP
                        )";
                    ExecuteNonQuery(createWeighRecordTable);

                    // 创建客户表
                    string createCustomerTable = @"
                        CREATE TABLE IF NOT EXISTS Customers (
                            Id TEXT PRIMARY KEY,
                            Name TEXT NOT NULL,
                            Type INTEGER,
                            ContactPerson TEXT,
                            Phone TEXT,
                            Address TEXT,
                            Remark TEXT,
                            IsActive INTEGER DEFAULT 1,
                            CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP
                        )";
                    ExecuteNonQuery(createCustomerTable);

                    // 创建货物类型表
                    string createCargoTypeTable = @"
                        CREATE TABLE IF NOT EXISTS CargoTypes (
                            Id TEXT PRIMARY KEY,
                            Name TEXT NOT NULL,
                            Code TEXT,
                            Unit TEXT DEFAULT '吨',
                            Remark TEXT,
                            IsActive INTEGER DEFAULT 1
                        )";
                    ExecuteNonQuery(createCargoTypeTable);

                    // 创建索引
                    string createIndexPlateNumber = "CREATE INDEX IF NOT EXISTS IDX_PlateNumber ON WeighRecords(PlateNumber)";
                    ExecuteNonQuery(createIndexPlateNumber);
                    
                    string createIndexCreateTime = "CREATE INDEX IF NOT EXISTS IDX_CreateTime ON WeighRecords(CreateTime)";
                    ExecuteNonQuery(createIndexCreateTime);
                    
                    string createIndexStatus = "CREATE INDEX IF NOT EXISTS IDX_Status ON WeighRecords(Status)";
                    ExecuteNonQuery(createIndexStatus);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("数据库初始化失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 执行非查询 SQL
        /// </summary>
        public static int ExecuteNonQuery(string sql, params SQLiteParameter[] parameters)
        {
            using (var conn = new SQLiteConnection(ConnectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 执行查询返回 DataTable
        /// </summary>
        public static DataTable ExecuteQuery(string sql, params SQLiteParameter[] parameters)
        {
            using (var conn = new SQLiteConnection(ConnectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    using (var adapter = new SQLiteDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        #region 称重记录操作

        /// <summary>
        /// 保存称重记录
        /// </summary>
        public static bool SaveWeighRecord(WeighRecord record)
        {
            try
            {
                string sql = @"
                    INSERT OR REPLACE INTO WeighRecords 
                    (Id, PlateNumber, Province, PlateCode, GrossWeight, TareWeight, NetWeight,
                     CargoType, Sender, Receiver, DriverName, DriverPhone, BusinessType, Status,
                     FirstWeighTime, SecondWeighTime, CompleteTime, OperatorId, OperatorName,
                     Remark, PrintCount, IsUploaded, CreateTime, UpdateTime)
                    VALUES 
                    (@Id, @PlateNumber, @Province, @PlateCode, @GrossWeight, @TareWeight, @NetWeight,
                     @CargoType, @Sender, @Receiver, @DriverName, @DriverPhone, @BusinessType, @Status,
                     @FirstWeighTime, @SecondWeighTime, @CompleteTime, @OperatorId, @OperatorName,
                     @Remark, @PrintCount, @IsUploaded, @CreateTime, @UpdateTime)";

                var parameters = new SQLiteParameter[]
                {
                    new SQLiteParameter("@Id", record.Id),
                    new SQLiteParameter("@PlateNumber", record.PlateNumber ?? (string)null),
                    new SQLiteParameter("@Province", record.Province ?? (string)null),
                    new SQLiteParameter("@PlateCode", record.PlateCode ?? (string)null),
                    new SQLiteParameter("@GrossWeight", record.GrossWeight),
                    new SQLiteParameter("@TareWeight", record.TareWeight),
                    new SQLiteParameter("@NetWeight", record.NetWeight),
                    new SQLiteParameter("@CargoType", record.CargoType ?? (string)null),
                    new SQLiteParameter("@Sender", record.Sender ?? (string)null),
                    new SQLiteParameter("@Receiver", record.Receiver ?? (string)null),
                    new SQLiteParameter("@DriverName", record.DriverName ?? (string)null),
                    new SQLiteParameter("@DriverPhone", record.DriverPhone ?? (string)null),
                    new SQLiteParameter("@BusinessType", (int)record.BusinessType),
                    new SQLiteParameter("@Status", (int)record.Status),
                    new SQLiteParameter("@FirstWeighTime", record.FirstWeighTime ?? (object)DBNull.Value),
                    new SQLiteParameter("@SecondWeighTime", record.SecondWeighTime ?? (object)DBNull.Value),
                    new SQLiteParameter("@CompleteTime", record.CompleteTime),
                    new SQLiteParameter("@OperatorId", record.OperatorId ?? (string)null),
                    new SQLiteParameter("@OperatorName", record.OperatorName ?? (string)null),
                    new SQLiteParameter("@Remark", record.Remark ?? (string)null),
                    new SQLiteParameter("@PrintCount", record.PrintCount),
                    new SQLiteParameter("@IsUploaded", record.IsUploaded ? 1 : 0),
                    new SQLiteParameter("@CreateTime", record.CreateTime),
                    new SQLiteParameter("@UpdateTime", DateTime.Now)
                };

                return ExecuteNonQuery(sql, parameters) > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取所有称重记录
        /// </summary>
        public static List<WeighRecord> GetAllWeighRecords()
        {
            var list = new List<WeighRecord>();
            var dt = ExecuteQuery("SELECT * FROM WeighRecords ORDER BY CreateTime DESC");
            
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new WeighRecord
                {
                    Id = row["Id"].ToString(),
                    PlateNumber = row["PlateNumber"].ToString(),
                    Province = row["Province"].ToString(),
                    PlateCode = row["PlateCode"].ToString(),
                    GrossWeight = Convert.ToDecimal(row["GrossWeight"]),
                    TareWeight = Convert.ToDecimal(row["TareWeight"]),
                    NetWeight = Convert.ToDecimal(row["NetWeight"]),
                    CargoType = row["CargoType"].ToString(),
                    Sender = row["Sender"].ToString(),
                    Receiver = row["Receiver"].ToString(),
                    DriverName = row["DriverName"].ToString(),
                    DriverPhone = row["DriverPhone"].ToString(),
                    BusinessType = (BusinessType)Convert.ToInt32(row["BusinessType"]),
                    Status = (WeighStatus)Convert.ToInt32(row["Status"]),
                    FirstWeighTime = row["FirstWeighTime"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["FirstWeighTime"]),
                    SecondWeighTime = row["SecondWeighTime"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["SecondWeighTime"]),
                    CompleteTime = Convert.ToDateTime(row["CompleteTime"]),
                    OperatorId = row["OperatorId"].ToString(),
                    OperatorName = row["OperatorName"].ToString(),
                    Remark = row["Remark"].ToString(),
                    PrintCount = Convert.ToInt32(row["PrintCount"]),
                    IsUploaded = Convert.ToInt32(row["IsUploaded"]) == 1,
                    CreateTime = Convert.ToDateTime(row["CreateTime"]),
                    UpdateTime = Convert.ToDateTime(row["UpdateTime"])
                });
            }
            return list;
        }

        /// <summary>
        /// 根据车牌号查询称重记录
        /// </summary>
        public static List<WeighRecord> GetWeighRecordsByPlate(string plateNumber)
        {
            var list = new List<WeighRecord>();
            var sql = "SELECT * FROM WeighRecords WHERE PlateNumber LIKE @PlateNumber ORDER BY CreateTime DESC";
            var dt = ExecuteQuery(sql, new SQLiteParameter("@PlateNumber", "%" + plateNumber + "%"));
            
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new WeighRecord
                {
                    Id = row["Id"].ToString(),
                    PlateNumber = row["PlateNumber"].ToString(),
                    Province = row["Province"].ToString(),
                    PlateCode = row["PlateCode"].ToString(),
                    GrossWeight = Convert.ToDecimal(row["GrossWeight"]),
                    TareWeight = Convert.ToDecimal(row["TareWeight"]),
                    NetWeight = Convert.ToDecimal(row["NetWeight"]),
                    CargoType = row["CargoType"].ToString(),
                    Sender = row["Sender"].ToString(),
                    Receiver = row["Receiver"].ToString(),
                    DriverName = row["DriverName"].ToString(),
                    DriverPhone = row["DriverPhone"].ToString(),
                    BusinessType = (BusinessType)Convert.ToInt32(row["BusinessType"]),
                    Status = (WeighStatus)Convert.ToInt32(row["Status"]),
                    FirstWeighTime = row["FirstWeighTime"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["FirstWeighTime"]),
                    SecondWeighTime = row["SecondWeighTime"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["SecondWeighTime"]),
                    CompleteTime = Convert.ToDateTime(row["CompleteTime"]),
                    OperatorId = row["OperatorId"].ToString(),
                    OperatorName = row["OperatorName"].ToString(),
                    Remark = row["Remark"].ToString(),
                    PrintCount = Convert.ToInt32(row["PrintCount"]),
                    IsUploaded = Convert.ToInt32(row["IsUploaded"]) == 1,
                    CreateTime = Convert.ToDateTime(row["CreateTime"]),
                    UpdateTime = Convert.ToDateTime(row["UpdateTime"])
                });
            }
            return list;
        }

        /// <summary>
        /// 删除称重记录
        /// </summary>
        public static bool DeleteWeighRecord(string id)
        {
            var sql = "DELETE FROM WeighRecords WHERE Id = @Id";
            return ExecuteNonQuery(sql, new SQLiteParameter("@Id", id)) > 0;
        }

        #endregion
    }
}
