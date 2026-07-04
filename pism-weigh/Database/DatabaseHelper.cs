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
                            UpdateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
                            Category INTEGER DEFAULT 0,
                            ModifyHistory TEXT,
                            ReviewerId TEXT,
                            ReviewerName TEXT,
                            ReviewTime DATETIME
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

                    // 创建基础数据表（通用 key-value 存储发货单位/司机等）
                    string createBasicDataTable = @"
                        CREATE TABLE IF NOT EXISTS BasicData (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Category TEXT NOT NULL,
                            Name TEXT NOT NULL,
                            CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
                            UNIQUE(Category, Name)
                        )";
                    ExecuteNonQuery(createBasicDataTable);

                    // 创建车辆档案表
                    string createVehicleTable = @"
                        CREATE TABLE IF NOT EXISTS Vehicles (
                            Id TEXT PRIMARY KEY,
                            PlateNumber TEXT NOT NULL UNIQUE,
                            Province TEXT,
                            PlateCode TEXT,
                            VehicleType TEXT,
                            BrandModel TEXT,
                            RatedLoad DECIMAL(18,2),
                            CurbWeight DECIMAL(18,2),
                            OwnerName TEXT,
                            OwnerPhone TEXT,
                            OwnerUnit TEXT,
                            FuelType TEXT,
                            EmissionStandard TEXT,
                            RegisteredDate DATETIME,
                            Status TEXT DEFAULT 'Active',
                            PhotoPath TEXT,
                            Remark TEXT,
                            CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
                            UpdateTime DATETIME DEFAULT CURRENT_TIMESTAMP
                        )";
                    ExecuteNonQuery(createVehicleTable);

                    string createVehicleIdx = "CREATE INDEX IF NOT EXISTS idx_vehicle_plate ON Vehicles(PlateNumber)";
                    ExecuteNonQuery(createVehicleIdx);

                    // 创建车辆皮重记录表
                    string createTareTable = @"
                        CREATE TABLE IF NOT EXISTS VehicleTareRecords (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            PlateNumber TEXT NOT NULL,
                            TareWeight DECIMAL(18,2) NOT NULL,
                            WeighDate DATETIME,
                            Source TEXT DEFAULT 'Manual',
                            OperatorName TEXT,
                            Remark TEXT,
                            CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP
                        )";
                    ExecuteNonQuery(createTareTable);

                    string createTareIdx = "CREATE INDEX IF NOT EXISTS idx_tare_plate ON VehicleTareRecords(PlateNumber)";
                    ExecuteNonQuery(createTareIdx);

                    string createRawWeightTable = @"
                        CREATE TABLE IF NOT EXISTS RawWeightLogs (
                            Id TEXT PRIMARY KEY,
                            Frame TEXT,
                            ParsedWeightTon REAL,
                            SourceUnit TEXT,
                            CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                        )";
                    ExecuteNonQuery(createRawWeightTable);

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

        public static bool SaveRawWeightLog(string frame, double parsedWeightTon, string sourceUnit)
        {
            try
            {
                string sql = @"
                    INSERT INTO RawWeightLogs (Id, Frame, ParsedWeightTon, SourceUnit, CreatedAt)
                    VALUES (@Id, @Frame, @ParsedWeightTon, @SourceUnit, @CreatedAt)";

                return ExecuteNonQuery(sql,
                    new SQLiteParameter("@Id", Guid.NewGuid().ToString("N")),
                    new SQLiteParameter("@Frame", frame ?? (object)DBNull.Value),
                    new SQLiteParameter("@ParsedWeightTon", parsedWeightTon),
                    new SQLiteParameter("@SourceUnit", sourceUnit ?? "t"),
                    new SQLiteParameter("@CreatedAt", DateTime.Now)) > 0;
            }
            catch
            {
                return false;
            }
        }

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
                     Remark, PrintCount, IsUploaded, CreateTime, UpdateTime, Category, 
                     ModifyHistory, ReviewerId, ReviewerName, ReviewTime)
                    VALUES 
                    (@Id, @PlateNumber, @Province, @PlateCode, @GrossWeight, @TareWeight, @NetWeight,
                     @CargoType, @Sender, @Receiver, @DriverName, @DriverPhone, @BusinessType, @Status,
                     @FirstWeighTime, @SecondWeighTime, @CompleteTime, @OperatorId, @OperatorName,
                     @Remark, @PrintCount, @IsUploaded, @CreateTime, @UpdateTime, @Category, 
                     @ModifyHistory, @ReviewerId, @ReviewerName, @ReviewTime)";

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
                    new SQLiteParameter("@UpdateTime", DateTime.Now),
                    new SQLiteParameter("@Category", (int)record.Category),
                    new SQLiteParameter("@ModifyHistory", record.ModifyHistory ?? (string)null),
                    new SQLiteParameter("@ReviewerId", record.ReviewerId ?? (string)null),
                    new SQLiteParameter("@ReviewerName", record.ReviewerName ?? (string)null),
                    new SQLiteParameter("@ReviewTime", record.ReviewTime ?? (object)DBNull.Value)
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
                    UpdateTime = Convert.ToDateTime(row["UpdateTime"]),
                    Category = row["Category"] == DBNull.Value ? RecordCategory.Valid : (RecordCategory)Convert.ToInt32(row["Category"]),
                    ModifyHistory = row["ModifyHistory"] == DBNull.Value ? null : row["ModifyHistory"].ToString(),
                    ReviewerId = row["ReviewerId"] == DBNull.Value ? null : row["ReviewerId"].ToString(),
                    ReviewerName = row["ReviewerName"] == DBNull.Value ? null : row["ReviewerName"].ToString(),
                    ReviewTime = row["ReviewTime"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["ReviewTime"])
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
                    UpdateTime = Convert.ToDateTime(row["UpdateTime"]),
                    Category = row["Category"] == DBNull.Value ? RecordCategory.Valid : (RecordCategory)Convert.ToInt32(row["Category"]),
                    ModifyHistory = row["ModifyHistory"] == DBNull.Value ? null : row["ModifyHistory"].ToString(),
                    ReviewerId = row["ReviewerId"] == DBNull.Value ? null : row["ReviewerId"].ToString(),
                    ReviewerName = row["ReviewerName"] == DBNull.Value ? null : row["ReviewerName"].ToString(),
                    ReviewTime = row["ReviewTime"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["ReviewTime"])
                });
            }
            return list;
        }

        /// <summary>
        /// 根据车牌号获取最新未完成称重记录
        /// 条件：状态为 FirstWeigh / SecondWeigh，且未完成
        /// </summary>
        public static WeighRecord GetLatestOpenRecordByPlate(string plateNumber)
        {
            var sql = @"
                SELECT * FROM WeighRecords
                WHERE PlateNumber = @PlateNumber
                  AND Status IN (@FirstStatus, @SecondStatus)
                  AND (CompleteTime IS NULL OR CompleteTime = '' OR CompleteTime = @MinDate)
                ORDER BY CreateTime DESC
                LIMIT 1";
            var dt = ExecuteQuery(sql,
                new SQLiteParameter("@PlateNumber", plateNumber),
                new SQLiteParameter("@FirstStatus", (int)WeighStatus.FirstWeigh),
                new SQLiteParameter("@SecondStatus", (int)WeighStatus.SecondWeigh),
                new SQLiteParameter("@MinDate", DateTime.MinValue));

            if (dt.Rows.Count == 0)
            {
                return null;
            }

            var row = dt.Rows[0];
            return new WeighRecord
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
                CompleteTime = row["CompleteTime"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["CompleteTime"]),
                OperatorId = row["OperatorId"].ToString(),
                OperatorName = row["OperatorName"].ToString(),
                Remark = row["Remark"] == DBNull.Value ? null : row["Remark"].ToString(),
                PrintCount = Convert.ToInt32(row["PrintCount"]),
                IsUploaded = Convert.ToInt32(row["IsUploaded"]) == 1,
                CreateTime = Convert.ToDateTime(row["CreateTime"]),
                UpdateTime = Convert.ToDateTime(row["UpdateTime"]),
                Category = row["Category"] == DBNull.Value ? RecordCategory.Valid : (RecordCategory)Convert.ToInt32(row["Category"]),
                ModifyHistory = row["ModifyHistory"] == DBNull.Value ? null : row["ModifyHistory"].ToString(),
                ReviewerId = row["ReviewerId"] == DBNull.Value ? null : row["ReviewerId"].ToString(),
                ReviewerName = row["ReviewerName"] == DBNull.Value ? null : row["ReviewerName"].ToString(),
                ReviewTime = row["ReviewTime"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["ReviewTime"])
            };
        }

        /// <summary>
        /// 根据类别查询称重记录
        /// </summary>
        public static List<WeighRecord> GetWeighRecordsByCategory(RecordCategory category)
        {
            var list = new List<WeighRecord>();
            var sql = "SELECT * FROM WeighRecords WHERE Category = @Category ORDER BY CreateTime DESC";
            var dt = ExecuteQuery(sql, new SQLiteParameter("@Category", (int)category));
            
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
                    UpdateTime = Convert.ToDateTime(row["UpdateTime"]),
                    Category = row["Category"] == DBNull.Value ? RecordCategory.Valid : (RecordCategory)Convert.ToInt32(row["Category"]),
                    ModifyHistory = row["ModifyHistory"] == DBNull.Value ? null : row["ModifyHistory"].ToString(),
                    ReviewerId = row["ReviewerId"] == DBNull.Value ? null : row["ReviewerId"].ToString(),
                    ReviewerName = row["ReviewerName"] == DBNull.Value ? null : row["ReviewerName"].ToString(),
                    ReviewTime = row["ReviewTime"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["ReviewTime"])
                });
            }
            return list;
        }

        /// <summary>
        /// 根据时间范围查询称重记录
        /// </summary>
        public static List<WeighRecord> GetWeighRecordsByDateRange(DateTime startDate, DateTime endDate)
        {
            var list = new List<WeighRecord>();
            var sql = "SELECT * FROM WeighRecords WHERE CreateTime BETWEEN @StartDate AND @EndDate ORDER BY CreateTime DESC";
            var dt = ExecuteQuery(sql, 
                new SQLiteParameter("@StartDate", startDate),
                new SQLiteParameter("@EndDate", endDate));
            
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
                    UpdateTime = Convert.ToDateTime(row["UpdateTime"]),
                    Category = row["Category"] == DBNull.Value ? RecordCategory.Valid : (RecordCategory)Convert.ToInt32(row["Category"]),
                    ModifyHistory = row["ModifyHistory"] == DBNull.Value ? null : row["ModifyHistory"].ToString(),
                    ReviewerId = row["ReviewerId"] == DBNull.Value ? null : row["ReviewerId"].ToString(),
                    ReviewerName = row["ReviewerName"] == DBNull.Value ? null : row["ReviewerName"].ToString(),
                    ReviewTime = row["ReviewTime"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["ReviewTime"])
                });
            }
            return list;
        }

        /// <summary>
        /// 获取待审核的称重记录
        /// </summary>
        public static List<WeighRecord> GetPendingReviewRecords()
        {
            return GetWeighRecordsByCategory(RecordCategory.PendingReview);
        }

        /// <summary>
        /// 组合条件查询称重记录
        /// </summary>
        public static List<WeighRecord> SearchRecords(
            string plateNumber = null,
            string driverName = null,
            string cargoType = null,
            string sender = null,
            string receiver = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            WeighStatus? status = null)
        {
            var sql = "SELECT * FROM WeighRecords WHERE 1=1";
            var parameters = new List<SQLiteParameter>();

            if (!string.IsNullOrWhiteSpace(plateNumber))
            {
                sql += " AND PlateNumber LIKE @PlateNumber";
                parameters.Add(new SQLiteParameter("@PlateNumber", "%" + plateNumber.Trim() + "%"));
            }
            if (!string.IsNullOrWhiteSpace(driverName))
            {
                sql += " AND DriverName LIKE @DriverName";
                parameters.Add(new SQLiteParameter("@DriverName", "%" + driverName.Trim() + "%"));
            }
            if (!string.IsNullOrWhiteSpace(cargoType))
            {
                sql += " AND CargoType LIKE @CargoType";
                parameters.Add(new SQLiteParameter("@CargoType", "%" + cargoType.Trim() + "%"));
            }
            if (!string.IsNullOrWhiteSpace(sender))
            {
                sql += " AND Sender LIKE @Sender";
                parameters.Add(new SQLiteParameter("@Sender", "%" + sender.Trim() + "%"));
            }
            if (!string.IsNullOrWhiteSpace(receiver))
            {
                sql += " AND Receiver LIKE @Receiver";
                parameters.Add(new SQLiteParameter("@Receiver", "%" + receiver.Trim() + "%"));
            }
            if (startDate.HasValue)
            {
                sql += " AND CreateTime >= @StartDate";
                parameters.Add(new SQLiteParameter("@StartDate", startDate.Value));
            }
            if (endDate.HasValue)
            {
                sql += " AND CreateTime <= @EndDate";
                parameters.Add(new SQLiteParameter("@EndDate", endDate.Value.AddDays(1)));
            }
            if (status.HasValue)
            {
                sql += " AND Status = @Status";
                parameters.Add(new SQLiteParameter("@Status", (int)status.Value));
            }

            sql += " ORDER BY CreateTime DESC";

            var list = new List<WeighRecord>();
            var dt = ExecuteQuery(sql, parameters.ToArray());

            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToRecord(row));
            }
            return list;
        }


        /// <summary>
        /// 审核称重记录
        /// </summary>
        public static bool ReviewWeighRecord(string id, string reviewerId, string reviewerName, bool approved)
        {
            try
            {
                if (approved)
                {
                    // 审核通过，设置为有效记录
                    var sql = "UPDATE WeighRecords SET Category = @Category, ReviewerId = @ReviewerId, ReviewerName = @ReviewerName, ReviewTime = @ReviewTime WHERE Id = @Id";
                    var parameters = new SQLiteParameter[]
                    {
                        new SQLiteParameter("@Category", (int)RecordCategory.Valid),
                        new SQLiteParameter("@ReviewerId", reviewerId),
                        new SQLiteParameter("@ReviewerName", reviewerName),
                        new SQLiteParameter("@ReviewTime", DateTime.Now),
                        new SQLiteParameter("@Id", id)
                    };
                    return ExecuteNonQuery(sql, parameters) > 0;
                }
                else
                {
                    // 审核拒绝，设置为废弃记录
                    var sql = "UPDATE WeighRecords SET Category = @Category, ReviewerId = @ReviewerId, ReviewerName = @ReviewerName, ReviewTime = @ReviewTime WHERE Id = @Id";
                    var parameters = new SQLiteParameter[]
                    {
                        new SQLiteParameter("@Category", (int)RecordCategory.Invalid),
                        new SQLiteParameter("@ReviewerId", reviewerId),
                        new SQLiteParameter("@ReviewerName", reviewerName),
                        new SQLiteParameter("@ReviewTime", DateTime.Now),
                        new SQLiteParameter("@Id", id)
                    };
                    return ExecuteNonQuery(sql, parameters) > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 标记称重记录为临时修改
        /// </summary>
        public static bool MarkAsTemporary(string id, string modifyHistory)
        {
            try
            {
                var sql = "UPDATE WeighRecords SET Category = @Category, ModifyHistory = @ModifyHistory, UpdateTime = @UpdateTime WHERE Id = @Id";
                var parameters = new SQLiteParameter[]
                {
                    new SQLiteParameter("@Category", (int)RecordCategory.Temporary),
                    new SQLiteParameter("@ModifyHistory", modifyHistory),
                    new SQLiteParameter("@UpdateTime", DateTime.Now),
                    new SQLiteParameter("@Id", id)
                };
                return ExecuteNonQuery(sql, parameters) > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 标记称重记录为废弃
        /// </summary>
        public static bool MarkAsInvalid(string id)
        {
            try
            {
                var sql = "UPDATE WeighRecords SET Category = @Category, UpdateTime = @UpdateTime WHERE Id = @Id";
                var parameters = new SQLiteParameter[]
                {
                    new SQLiteParameter("@Category", (int)RecordCategory.Invalid),
                    new SQLiteParameter("@UpdateTime", DateTime.Now),
                    new SQLiteParameter("@Id", id)
                };
                return ExecuteNonQuery(sql, parameters) > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 删除称重记录
        /// </summary>
        public static bool DeleteWeighRecord(string id)
        {
            var sql = "DELETE FROM WeighRecords WHERE Id = @Id";
            return ExecuteNonQuery(sql, new SQLiteParameter("@Id", id)) > 0;
        }

        private static WeighRecord MapRowToRecord(DataRow row)
        {
            return new WeighRecord
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
                UpdateTime = Convert.ToDateTime(row["UpdateTime"]),
                Category = row["Category"] == DBNull.Value ? RecordCategory.Valid : (RecordCategory)Convert.ToInt32(row["Category"]),
                ModifyHistory = row["ModifyHistory"] == DBNull.Value ? null : row["ModifyHistory"].ToString(),
                ReviewerId = row["ReviewerId"] == DBNull.Value ? null : row["ReviewerId"].ToString(),
                ReviewerName = row["ReviewerName"] == DBNull.Value ? null : row["ReviewerName"].ToString(),
                ReviewTime = row["ReviewTime"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["ReviewTime"])
            };
        }

        // ===== 基础数据（BasicData 表）管理 =====

        /// <summary>
        /// 保存基础数据项（运输内容/发货单位/收货单位/司机/司磅员）
        /// </summary>
        public static bool SaveBasicData(string category, string name)
        {
            try
            {
                var sql = "INSERT OR IGNORE INTO BasicData (Category, Name) VALUES (@Category, @Name)";
                return ExecuteNonQuery(sql,
                    new SQLiteParameter("@Category", category),
                    new SQLiteParameter("@Name", name)) >= 0;
            }
            catch { return false; }
        }

        /// <summary>
        /// 删除基础数据项
        /// </summary>
        public static bool DeleteBasicData(string category, string name)
        {
            try
            {
                var sql = "DELETE FROM BasicData WHERE Category = @Category AND Name = @Name";
                return ExecuteNonQuery(sql,
                    new SQLiteParameter("@Category", category),
                    new SQLiteParameter("@Name", name)) > 0;
            }
            catch { return false; }
        }

        /// <summary>
        /// 获取指定类别的基础数据列表
        /// </summary>
        public static List<string> GetBasicData(string category)
        {
            var list = new List<string>();
            try
            {
                var sql = "SELECT Name FROM BasicData WHERE Category = @Category ORDER BY Name";
                var dt = ExecuteQuery(sql, new SQLiteParameter("@Category", category));
                foreach (DataRow row in dt.Rows)
                    list.Add(row["Name"].ToString());
            }
            catch { }
            return list;
        }

        /// <summary>
        /// 批量保存基础数据（删除旧数据+插入新数据）
        /// </summary>
        public static void SaveBasicDataBatch(string category, List<string> items)
        {
            try
            {
                // 删除旧数据
                var deleteSql = "DELETE FROM BasicData WHERE Category = @Category";
                ExecuteNonQuery(deleteSql, new SQLiteParameter("@Category", category));
                // 插入新数据
                foreach (var item in items)
                {
                    if (!string.IsNullOrWhiteSpace(item))
                        SaveBasicData(category, item.Trim());
                }
            }
            catch { }
        }

        // ===== 车辆档案管理 =====

        public static bool SaveVehicle(Vehicle vehicle)
        {
            try
            {
                var sql = @"
                    INSERT OR REPLACE INTO Vehicles 
                    (Id, PlateNumber, Province, PlateCode, VehicleType, BrandModel,
                     RatedLoad, CurbWeight, OwnerName, OwnerPhone, OwnerUnit,
                     FuelType, EmissionStandard, RegisteredDate, Status, PhotoPath,
                     Remark, CreateTime, UpdateTime)
                    VALUES
                    (@Id, @PlateNumber, @Province, @PlateCode, @VehicleType, @BrandModel,
                     @RatedLoad, @CurbWeight, @OwnerName, @OwnerPhone, @OwnerUnit,
                     @FuelType, @EmissionStandard, @RegisteredDate, @Status, @PhotoPath,
                     @Remark, @CreateTime, @UpdateTime)";
                return ExecuteNonQuery(sql,
                    new SQLiteParameter("@Id", vehicle.Id),
                    new SQLiteParameter("@PlateNumber", vehicle.PlateNumber),
                    new SQLiteParameter("@Province", (object)vehicle.Province ?? DBNull.Value),
                    new SQLiteParameter("@PlateCode", (object)vehicle.PlateCode ?? DBNull.Value),
                    new SQLiteParameter("@VehicleType", (object)vehicle.VehicleType ?? DBNull.Value),
                    new SQLiteParameter("@BrandModel", (object)vehicle.BrandModel ?? DBNull.Value),
                    new SQLiteParameter("@RatedLoad", vehicle.RatedLoad),
                    new SQLiteParameter("@CurbWeight", vehicle.CurbWeight),
                    new SQLiteParameter("@OwnerName", (object)vehicle.OwnerName ?? DBNull.Value),
                    new SQLiteParameter("@OwnerPhone", (object)vehicle.OwnerPhone ?? DBNull.Value),
                    new SQLiteParameter("@OwnerUnit", (object)vehicle.OwnerUnit ?? DBNull.Value),
                    new SQLiteParameter("@FuelType", (object)vehicle.FuelType ?? DBNull.Value),
                    new SQLiteParameter("@EmissionStandard", (object)vehicle.EmissionStandard ?? DBNull.Value),
                    new SQLiteParameter("@RegisteredDate", (object)vehicle.RegisteredDate ?? DBNull.Value),
                    new SQLiteParameter("@Status", vehicle.Status ?? "Active"),
                    new SQLiteParameter("@PhotoPath", (object)vehicle.PhotoPath ?? DBNull.Value),
                    new SQLiteParameter("@Remark", (object)vehicle.Remark ?? DBNull.Value),
                    new SQLiteParameter("@CreateTime", vehicle.CreateTime),
                    new SQLiteParameter("@UpdateTime", DateTime.Now)
                ) > 0;
            }
            catch { return false; }
        }

        public static List<Vehicle> GetAllVehicles()
        {
            var list = new List<Vehicle>();
            try
            {
                var dt = ExecuteQuery("SELECT * FROM Vehicles ORDER BY PlateNumber");
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new Vehicle
                    {
                        Id = row["Id"].ToString(),
                        PlateNumber = row["PlateNumber"].ToString(),
                        Province = row["Province"] == DBNull.Value ? null : row["Province"].ToString(),
                        PlateCode = row["PlateCode"] == DBNull.Value ? null : row["PlateCode"].ToString(),
                        VehicleType = row["VehicleType"] == DBNull.Value ? null : row["VehicleType"].ToString(),
                        BrandModel = row["BrandModel"] == DBNull.Value ? null : row["BrandModel"].ToString(),
                        RatedLoad = row["RatedLoad"] == DBNull.Value ? 0 : Convert.ToDecimal(row["RatedLoad"]),
                        CurbWeight = row["CurbWeight"] == DBNull.Value ? 0 : Convert.ToDecimal(row["CurbWeight"]),
                        OwnerName = row["OwnerName"] == DBNull.Value ? null : row["OwnerName"].ToString(),
                        OwnerPhone = row["OwnerPhone"] == DBNull.Value ? null : row["OwnerPhone"].ToString(),
                        OwnerUnit = row["OwnerUnit"] == DBNull.Value ? null : row["OwnerUnit"].ToString(),
                        FuelType = row["FuelType"] == DBNull.Value ? null : row["FuelType"].ToString(),
                        EmissionStandard = row["EmissionStandard"] == DBNull.Value ? null : row["EmissionStandard"].ToString(),
                        RegisteredDate = row["RegisteredDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["RegisteredDate"]),
                        Status = row["Status"] == DBNull.Value ? "Active" : row["Status"].ToString(),
                        PhotoPath = row["PhotoPath"] == DBNull.Value ? null : row["PhotoPath"].ToString(),
                        Remark = row["Remark"] == DBNull.Value ? null : row["Remark"].ToString(),
                        CreateTime = Convert.ToDateTime(row["CreateTime"]),
                        UpdateTime = Convert.ToDateTime(row["UpdateTime"])
                    });
                }
            }
            catch { }
            return list;
        }

        public static List<Vehicle> SearchVehicles(string keyword)
        {
            var list = new List<Vehicle>();
            try
            {
                var sql = "SELECT * FROM Vehicles WHERE PlateNumber LIKE @Keyword OR OwnerName LIKE @Keyword OR OwnerUnit LIKE @Keyword ORDER BY PlateNumber";
                var dt = ExecuteQuery(sql, new SQLiteParameter("@Keyword", "%" + (keyword ?? "") + "%"));
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new Vehicle
                    {
                        Id = row["Id"].ToString(),
                        PlateNumber = row["PlateNumber"].ToString(),
                        Province = row["Province"] == DBNull.Value ? null : row["Province"].ToString(),
                        PlateCode = row["PlateCode"] == DBNull.Value ? null : row["PlateCode"].ToString(),
                        VehicleType = row["VehicleType"] == DBNull.Value ? null : row["VehicleType"].ToString(),
                        BrandModel = row["BrandModel"] == DBNull.Value ? null : row["BrandModel"].ToString(),
                        RatedLoad = row["RatedLoad"] == DBNull.Value ? 0 : Convert.ToDecimal(row["RatedLoad"]),
                        CurbWeight = row["CurbWeight"] == DBNull.Value ? 0 : Convert.ToDecimal(row["CurbWeight"]),
                        OwnerName = row["OwnerName"] == DBNull.Value ? null : row["OwnerName"].ToString(),
                        OwnerPhone = row["OwnerPhone"] == DBNull.Value ? null : row["OwnerPhone"].ToString(),
                        OwnerUnit = row["OwnerUnit"] == DBNull.Value ? null : row["OwnerUnit"].ToString(),
                        Status = row["Status"] == DBNull.Value ? "Active" : row["Status"].ToString(),
                        Remark = row["Remark"] == DBNull.Value ? null : row["Remark"].ToString(),
                        CreateTime = Convert.ToDateTime(row["CreateTime"]),
                        UpdateTime = Convert.ToDateTime(row["UpdateTime"])
                    });
                }
            }
            catch { }
            return list;
        }

        public static Vehicle GetVehicleByPlate(string plateNumber)
        {
            try
            {
                var sql = "SELECT * FROM Vehicles WHERE PlateNumber = @PlateNumber";
                var dt = ExecuteQuery(sql, new SQLiteParameter("@PlateNumber", plateNumber));
                if (dt.Rows.Count == 0) return null;
                var row = dt.Rows[0];
                return new Vehicle
                {
                    Id = row["Id"].ToString(),
                    PlateNumber = row["PlateNumber"].ToString(),
                    Province = row["Province"] == DBNull.Value ? null : row["Province"].ToString(),
                    PlateCode = row["PlateCode"] == DBNull.Value ? null : row["PlateCode"].ToString(),
                    VehicleType = row["VehicleType"] == DBNull.Value ? null : row["VehicleType"].ToString(),
                    BrandModel = row["BrandModel"] == DBNull.Value ? null : row["BrandModel"].ToString(),
                    RatedLoad = row["RatedLoad"] == DBNull.Value ? 0 : Convert.ToDecimal(row["RatedLoad"]),
                    CurbWeight = row["CurbWeight"] == DBNull.Value ? 0 : Convert.ToDecimal(row["CurbWeight"]),
                    OwnerName = row["OwnerName"] == DBNull.Value ? null : row["OwnerName"].ToString(),
                    OwnerPhone = row["OwnerPhone"] == DBNull.Value ? null : row["OwnerPhone"].ToString(),
                    OwnerUnit = row["OwnerUnit"] == DBNull.Value ? null : row["OwnerUnit"].ToString(),
                    Status = row["Status"] == DBNull.Value ? "Active" : row["Status"].ToString(),
                    Remark = row["Remark"] == DBNull.Value ? null : row["Remark"].ToString(),
                    CreateTime = Convert.ToDateTime(row["CreateTime"]),
                    UpdateTime = Convert.ToDateTime(row["UpdateTime"])
                };
            }
            catch { return null; }
        }

        public static bool DeleteVehicle(string id)
        {
            try
            {
                var sql = "DELETE FROM Vehicles WHERE Id = @Id";
                return ExecuteNonQuery(sql, new SQLiteParameter("@Id", id)) > 0;
            }
            catch { return false; }
        }

        // ===== 车辆皮重管理 =====

        public static bool SaveTareRecord(string plateNumber, decimal tareWeight, string source, string operatorName, string remark)
        {
            try
            {
                var sql = "INSERT INTO VehicleTareRecords (PlateNumber, TareWeight, WeighDate, Source, OperatorName, Remark) VALUES (@Plate, @Weight, @Date, @Source, @Op, @Remark)";
                return ExecuteNonQuery(sql,
                    new SQLiteParameter("@Plate", plateNumber),
                    new SQLiteParameter("@Weight", tareWeight),
                    new SQLiteParameter("@Date", DateTime.Now),
                    new SQLiteParameter("@Source", source ?? "Manual"),
                    new SQLiteParameter("@Op", (object)operatorName ?? DBNull.Value),
                    new SQLiteParameter("@Remark", (object)remark ?? DBNull.Value)
                ) > 0;
            }
            catch { return false; }
        }

        public static List<TareRecord> GetTareRecords(string plateNumber)
        {
            var list = new List<TareRecord>();
            try
            {
                var sql = "SELECT * FROM VehicleTareRecords WHERE PlateNumber = @Plate ORDER BY CreateTime DESC LIMIT 20";
                var dt = ExecuteQuery(sql, new SQLiteParameter("@Plate", plateNumber));
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new TareRecord
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        PlateNumber = row["PlateNumber"].ToString(),
                        TareWeight = Convert.ToDecimal(row["TareWeight"]),
                        WeighDate = row["WeighDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["WeighDate"]),
                        Source = row["Source"] == DBNull.Value ? "Manual" : row["Source"].ToString(),
                        OperatorName = row["OperatorName"] == DBNull.Value ? null : row["OperatorName"].ToString(),
                        Remark = row["Remark"] == DBNull.Value ? null : row["Remark"].ToString(),
                        CreateTime = Convert.ToDateTime(row["CreateTime"])
                    });
                }
            }
            catch { }
            return list;
        }

        public static TareRecord GetLatestTare(string plateNumber)
        {
            try
            {
                var sql = "SELECT * FROM VehicleTareRecords WHERE PlateNumber = @Plate ORDER BY CreateTime DESC LIMIT 1";
                var dt = ExecuteQuery(sql, new SQLiteParameter("@Plate", plateNumber));
                if (dt.Rows.Count == 0) return null;
                var row = dt.Rows[0];
                return new TareRecord
                {
                    Id = Convert.ToInt32(row["Id"]),
                    PlateNumber = row["PlateNumber"].ToString(),
                    TareWeight = Convert.ToDecimal(row["TareWeight"]),
                    WeighDate = row["WeighDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["WeighDate"]),
                    Source = row["Source"] == DBNull.Value ? "Manual" : row["Source"].ToString(),
                    OperatorName = row["OperatorName"] == DBNull.Value ? null : row["OperatorName"].ToString(),
                    Remark = row["Remark"] == DBNull.Value ? null : row["Remark"].ToString(),
                    CreateTime = Convert.ToDateTime(row["CreateTime"])
                };
            }
            catch { return null; }
        }

        public static bool DeleteTareRecord(int id)
        {
            try
            {
                var sql = "DELETE FROM VehicleTareRecords WHERE Id = @Id";
                return ExecuteNonQuery(sql, new SQLiteParameter("@Id", id)) > 0;
            }
            catch { return false; }
        }

        #endregion
    }
}
