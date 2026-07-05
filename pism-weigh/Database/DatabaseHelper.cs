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

                    // 2.3: WeighRecords 新增 VehicleId 列（幂等）
                    try { ExecuteNonQuery("ALTER TABLE WeighRecords ADD COLUMN VehicleId TEXT"); } catch { }

                    // 创建车辆进出场记录表
                    string createVehicleLogTable = @"
                        CREATE TABLE IF NOT EXISTS VehicleLogs (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            PlateNumber TEXT NOT NULL,
                            Direction TEXT NOT NULL,
                            LogTime DATETIME NOT NULL,
                            RelatedWeighId TEXT,
                            GrossWeight DECIMAL(18,2),
                            TareWeight DECIMAL(18,2),
                            OperatorName TEXT,
                            Remark TEXT,
                            CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP
                        )";
                    ExecuteNonQuery(createVehicleLogTable);

                    string createVLogIdx = "CREATE INDEX IF NOT EXISTS idx_vlog_time ON VehicleLogs(LogTime)";
                    ExecuteNonQuery(createVLogIdx);
                    string createVLogIdx2 = "CREATE INDEX IF NOT EXISTS idx_vlog_plate ON VehicleLogs(PlateNumber)";
                    ExecuteNonQuery(createVLogIdx2);

                    // 创建车辆-司机绑定表
                    string createVDriverTable = @"
                        CREATE TABLE IF NOT EXISTS VehicleDrivers (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            VehicleId TEXT NOT NULL,
                            DriverName TEXT NOT NULL,
                            DriverPhone TEXT,
                            IsDefault INTEGER DEFAULT 0,
                            FOREIGN KEY (VehicleId) REFERENCES Vehicles(Id)
                        )";
                    ExecuteNonQuery(createVDriverTable);

                    // 创建摄像头配置表
                    string createCameraTable = @"
                        CREATE TABLE IF NOT EXISTS Cameras (
                            Id TEXT PRIMARY KEY,
                            Name TEXT NOT NULL,
                            CameraType TEXT NOT NULL,
                            IPAddress TEXT,
                            Port INTEGER DEFAULT 8000,
                            Username TEXT,
                            Password TEXT,
                            ChannelNo INTEGER DEFAULT 1,
                            RTSPUrl TEXT,
                            Resolution TEXT DEFAULT '1920x1080',
                            IsEnabled INTEGER DEFAULT 1,
                            IsDefault INTEGER DEFAULT 0,
                            CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
                            UpdateTime DATETIME DEFAULT CURRENT_TIMESTAMP
                        )";
                    ExecuteNonQuery(createCameraTable);

                    // 创建车牌识别记录表
                    string createLprTable = @"
                        CREATE TABLE IF NOT EXISTS PlateRecognitionRecords (
                            Id TEXT PRIMARY KEY,
                            PlateNumber TEXT,
                            Confidence REAL DEFAULT 0,
                            CameraName TEXT,
                            CameraType TEXT,
                            ImagePath TEXT,
                            VehicleId TEXT,
                            RecognizeTime DATETIME,
                            Source TEXT DEFAULT 'Auto',
                            Remark TEXT,
                            CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP
                        )";
                    ExecuteNonQuery(createLprTable);
                    ExecuteNonQuery("CREATE INDEX IF NOT EXISTS idx_lpr_time ON PlateRecognitionRecords(RecognizeTime)");
                    ExecuteNonQuery("CREATE INDEX IF NOT EXISTS idx_lpr_plate ON PlateRecognitionRecords(PlateNumber)");

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
                     ModifyHistory, ReviewerId, ReviewerName, ReviewTime, VehicleId)
                    VALUES 
                    (@Id, @PlateNumber, @Province, @PlateCode, @GrossWeight, @TareWeight, @NetWeight,
                     @CargoType, @Sender, @Receiver, @DriverName, @DriverPhone, @BusinessType, @Status,
                     @FirstWeighTime, @SecondWeighTime, @CompleteTime, @OperatorId, @OperatorName,
                     @Remark, @PrintCount, @IsUploaded, @CreateTime, @UpdateTime, @Category, 
                     @ModifyHistory, @ReviewerId, @ReviewerName, @ReviewTime, @VehicleId)";

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
                    new SQLiteParameter("@ReviewTime", record.ReviewTime ?? (object)DBNull.Value),
                    new SQLiteParameter("@VehicleId", (object)record.VehicleId ?? DBNull.Value)
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

        // ===== 车辆进出场日志 =====

        public static bool SaveVehicleLog(VehicleLog log)
        {
            try
            {
                var sql = @"INSERT INTO VehicleLogs (PlateNumber, Direction, LogTime, RelatedWeighId,
                    GrossWeight, TareWeight, OperatorName, Remark)
                    VALUES (@Plate, @Dir, @Time, @WeighId, @Gross, @Tare, @Op, @Remark)";
                return ExecuteNonQuery(sql,
                    new SQLiteParameter("@Plate", log.PlateNumber),
                    new SQLiteParameter("@Dir", log.Direction),
                    new SQLiteParameter("@Time", log.LogTime),
                    new SQLiteParameter("@WeighId", (object)log.RelatedWeighId ?? DBNull.Value),
                    new SQLiteParameter("@Gross", log.GrossWeight),
                    new SQLiteParameter("@Tare", log.TareWeight),
                    new SQLiteParameter("@Op", (object)log.OperatorName ?? DBNull.Value),
                    new SQLiteParameter("@Remark", (object)log.Remark ?? DBNull.Value)
                ) > 0;
            }
            catch { return false; }
        }

        public static List<VehicleLog> GetVehicleLogs(DateTime? start, DateTime? end, string plateNumber)
        {
            var list = new List<VehicleLog>();
            try
            {
                var sql = "SELECT * FROM VehicleLogs WHERE 1=1";
                var ps = new List<SQLiteParameter>();
                if (start.HasValue) { sql += " AND LogTime >= @Start"; ps.Add(new SQLiteParameter("@Start", start.Value)); }
                if (end.HasValue) { sql += " AND LogTime <= @End"; ps.Add(new SQLiteParameter("@End", end.Value.AddDays(1))); }
                if (!string.IsNullOrWhiteSpace(plateNumber)) { sql += " AND PlateNumber LIKE @Plate"; ps.Add(new SQLiteParameter("@Plate", "%" + plateNumber + "%")); }
                sql += " ORDER BY LogTime DESC LIMIT 500";

                var dt = ExecuteQuery(sql, ps.ToArray());
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new VehicleLog
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        PlateNumber = row["PlateNumber"].ToString(),
                        Direction = row["Direction"].ToString(),
                        LogTime = Convert.ToDateTime(row["LogTime"]),
                        RelatedWeighId = row["RelatedWeighId"] == DBNull.Value ? null : row["RelatedWeighId"].ToString(),
                        GrossWeight = row["GrossWeight"] == DBNull.Value ? 0 : Convert.ToDecimal(row["GrossWeight"]),
                        TareWeight = row["TareWeight"] == DBNull.Value ? 0 : Convert.ToDecimal(row["TareWeight"]),
                        OperatorName = row["OperatorName"] == DBNull.Value ? null : row["OperatorName"].ToString(),
                        Remark = row["Remark"] == DBNull.Value ? null : row["Remark"].ToString(),
                        CreateTime = Convert.ToDateTime(row["CreateTime"])
                    });
                }
            }
            catch { }
            return list;
        }

        public static int GetActiveVehicleCount()
        {
            try
            {
                var sql = @"SELECT COUNT(DISTINCT PlateNumber) FROM VehicleLogs
                    WHERE PlateNumber NOT IN (
                        SELECT PlateNumber FROM VehicleLogs v2
                        WHERE v2.Direction = 'Out' AND v2.LogTime = (
                            SELECT MAX(LogTime) FROM VehicleLogs v3 WHERE v3.PlateNumber = v2.PlateNumber
                        )
                    )
                    AND PlateNumber IN (
                        SELECT PlateNumber FROM VehicleLogs WHERE Direction = 'In'
                    )";
                var dt = ExecuteQuery(sql);
                if (dt.Rows.Count > 0) return Convert.ToInt32(dt.Rows[0][0]);
            }
            catch { }
            return 0;
        }

        // ===== 车辆统计分析 =====

        public static List<VehicleStatItem> GetVehicleStats(DateTime start, DateTime end)
        {
            var list = new List<VehicleStatItem>();
            try
            {
                var sql = @"
                    SELECT w.PlateNumber,
                        v.VehicleType, v.OwnerName,
                        COUNT(*) as WeighCount,
                        SUM(w.GrossWeight) as TotalGross, SUM(w.TareWeight) as TotalTare, SUM(w.NetWeight) as TotalNet,
                        ROUND(AVG(w.NetWeight), 2) as AvgNet, MAX(w.NetWeight) as MaxNet,
                        MIN(w.FirstWeighTime) as FirstWeigh, MAX(w.CompleteTime) as LastWeigh,
                        SUM(w.PrintCount) as TotalPrints
                    FROM WeighRecords w
                    LEFT JOIN Vehicles v ON v.PlateNumber = w.PlateNumber
                    WHERE w.Status = 2 AND w.CompleteTime >= @Start AND w.CompleteTime <= @End
                    GROUP BY w.PlateNumber
                    ORDER BY TotalNet DESC";
                var dt = ExecuteQuery(sql,
                    new SQLiteParameter("@Start", start),
                    new SQLiteParameter("@End", end.AddDays(1)));
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new VehicleStatItem
                    {
                        PlateNumber = row["PlateNumber"].ToString(),
                        VehicleType = row["VehicleType"] == DBNull.Value ? "" : row["VehicleType"].ToString(),
                        OwnerName = row["OwnerName"] == DBNull.Value ? "" : row["OwnerName"].ToString(),
                        WeighCount = Convert.ToInt32(row["WeighCount"]),
                        TotalGross = Convert.ToDecimal(row["TotalGross"]),
                        TotalTare = Convert.ToDecimal(row["TotalTare"]),
                        TotalNet = Convert.ToDecimal(row["TotalNet"]),
                        AvgNet = Convert.ToDecimal(row["AvgNet"]),
                        MaxNet = Convert.ToDecimal(row["MaxNet"]),
                        FirstWeigh = row["FirstWeigh"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["FirstWeigh"]),
                        LastWeigh = row["LastWeigh"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["LastWeigh"]),
                        TotalPrints = Convert.ToInt32(row["TotalPrints"])
                    });
                }
            }
            catch { }
            return list;
        }

        // ===== 车辆-司机绑定 =====

        public static bool SaveVehicleDriver(string vehicleId, string driverName, string driverPhone, bool isDefault)
        {
            try
            {
                if (isDefault)
                    ExecuteNonQuery("UPDATE VehicleDrivers SET IsDefault = 0 WHERE VehicleId = @VId",
                        new SQLiteParameter("@VId", vehicleId));
                var sql = "INSERT OR REPLACE INTO VehicleDrivers (VehicleId, DriverName, DriverPhone, IsDefault) VALUES (@VId, @Name, @Phone, @Def)";
                return ExecuteNonQuery(sql,
                    new SQLiteParameter("@VId", vehicleId),
                    new SQLiteParameter("@Name", driverName),
                    new SQLiteParameter("@Phone", (object)driverPhone ?? DBNull.Value),
                    new SQLiteParameter("@Def", isDefault ? 1 : 0)) > 0;
            }
            catch { return false; }
        }

        public static string GetDefaultDriver(string plateNumber)
        {
            try
            {
                var sql = @"SELECT vd.DriverName FROM VehicleDrivers vd
                    JOIN Vehicles v ON v.Id = vd.VehicleId
                    WHERE v.PlateNumber = @Plate AND vd.IsDefault = 1 LIMIT 1";
                var dt = ExecuteQuery(sql, new SQLiteParameter("@Plate", plateNumber));
                if (dt.Rows.Count > 0) return dt.Rows[0][0].ToString();
            }
            catch { }
            return null;
        }

        // ===== 摄像头配置管理 =====

        public static bool SaveCamera(CameraConfig camera)
        {
            try
            {
                var sql = @"INSERT OR REPLACE INTO Cameras
                    (Id, Name, CameraType, IPAddress, Port, Username, Password, ChannelNo, RTSPUrl, Resolution, IsEnabled, IsDefault, CreateTime, UpdateTime)
                    VALUES (@Id, @Name, @Type, @IP, @Port, @User, @Pwd, @Ch, @RTSP, @Res, @Enabled, @Default, @CT, @UT)";
                camera.UpdateTime = DateTime.Now;
                return ExecuteNonQuery(sql,
                    new SQLiteParameter("@Id", camera.Id),
                    new SQLiteParameter("@Name", camera.Name),
                    new SQLiteParameter("@Type", camera.CameraType),
                    new SQLiteParameter("@IP", (object)camera.IPAddress ?? DBNull.Value),
                    new SQLiteParameter("@Port", camera.Port),
                    new SQLiteParameter("@User", (object)camera.Username ?? DBNull.Value),
                    new SQLiteParameter("@Pwd", (object)camera.Password ?? DBNull.Value),
                    new SQLiteParameter("@Ch", camera.ChannelNo),
                    new SQLiteParameter("@RTSP", (object)camera.RTSPUrl ?? DBNull.Value),
                    new SQLiteParameter("@Res", (object)camera.Resolution ?? DBNull.Value),
                    new SQLiteParameter("@Enabled", camera.IsEnabled ? 1 : 0),
                    new SQLiteParameter("@Default", camera.IsDefault ? 1 : 0),
                    new SQLiteParameter("@CT", camera.CreateTime),
                    new SQLiteParameter("@UT", camera.UpdateTime)
                ) > 0;
            }
            catch { return false; }
        }

        public static List<CameraConfig> GetAllCameras()
        {
            var list = new List<CameraConfig>();
            try
            {
                var dt = ExecuteQuery("SELECT * FROM Cameras ORDER BY IsDefault DESC, Name");
                foreach (DataRow row in dt.Rows)
                    list.Add(MapCamera(row));
            }
            catch { }
            return list;
        }

        public static CameraConfig GetDefaultCamera()
        {
            try
            {
                var dt = ExecuteQuery("SELECT * FROM Cameras WHERE IsEnabled = 1 AND IsDefault = 1 LIMIT 1");
                if (dt.Rows.Count > 0) return MapCamera(dt.Rows[0]);
                dt = ExecuteQuery("SELECT * FROM Cameras WHERE IsEnabled = 1 LIMIT 1");
                if (dt.Rows.Count > 0) return MapCamera(dt.Rows[0]);
            }
            catch { }
            return null;
        }

        public static bool DeleteCamera(string id)
        {
            var sql = "DELETE FROM Cameras WHERE Id = @Id";
            return ExecuteNonQuery(sql, new SQLiteParameter("@Id", id)) > 0;
        }

        private static CameraConfig MapCamera(DataRow row)
        {
            return new CameraConfig
            {
                Id = row["Id"].ToString(),
                Name = row["Name"].ToString(),
                CameraType = row["CameraType"].ToString(),
                IPAddress = row["IPAddress"] == DBNull.Value ? null : row["IPAddress"].ToString(),
                Port = Convert.ToInt32(row["Port"]),
                Username = row["Username"] == DBNull.Value ? null : row["Username"].ToString(),
                Password = row["Password"] == DBNull.Value ? null : row["Password"].ToString(),
                ChannelNo = Convert.ToInt32(row["ChannelNo"]),
                RTSPUrl = row["RTSPUrl"] == DBNull.Value ? null : row["RTSPUrl"].ToString(),
                Resolution = row["Resolution"] == DBNull.Value ? "1920x1080" : row["Resolution"].ToString(),
                IsEnabled = Convert.ToInt32(row["IsEnabled"]) == 1,
                IsDefault = Convert.ToInt32(row["IsDefault"]) == 1,
                CreateTime = Convert.ToDateTime(row["CreateTime"]),
                UpdateTime = Convert.ToDateTime(row["UpdateTime"])
            };
        }

        // ===== 车牌识别记录管理 =====

        public static bool SavePlateRecognitionRecord(PlateRecognitionRecord record)
        {
            try
            {
                var sql = @"INSERT INTO PlateRecognitionRecords
                    (Id, PlateNumber, Confidence, CameraName, CameraType, ImagePath, VehicleId, RecognizeTime, Source, Remark, CreateTime)
                    VALUES (@Id, @Plate, @Conf, @CamName, @CamType, @ImgPath, @VId, @Time, @Src, @Remark, @CT)";
                return ExecuteNonQuery(sql,
                    new SQLiteParameter("@Id", record.Id),
                    new SQLiteParameter("@Plate", (object)record.PlateNumber ?? DBNull.Value),
                    new SQLiteParameter("@Conf", record.Confidence),
                    new SQLiteParameter("@CamName", (object)record.CameraName ?? DBNull.Value),
                    new SQLiteParameter("@CamType", (object)record.CameraType ?? DBNull.Value),
                    new SQLiteParameter("@ImgPath", (object)record.ImagePath ?? DBNull.Value),
                    new SQLiteParameter("@VId", (object)record.VehicleId ?? DBNull.Value),
                    new SQLiteParameter("@Time", record.RecognizeTime),
                    new SQLiteParameter("@Src", record.Source ?? "Auto"),
                    new SQLiteParameter("@Remark", (object)record.Remark ?? DBNull.Value),
                    new SQLiteParameter("@CT", record.CreateTime)
                ) > 0;
            }
            catch { return false; }
        }

        public static List<PlateRecognitionRecord> GetRecognitionRecords(DateTime? start, DateTime? end, string plateFilter)
        {
            var list = new List<PlateRecognitionRecord>();
            try
            {
                var sql = "SELECT * FROM PlateRecognitionRecords WHERE 1=1";
                var ps = new List<SQLiteParameter>();
                if (start.HasValue) { sql += " AND RecognizeTime >= @Start"; ps.Add(new SQLiteParameter("@Start", start.Value)); }
                if (end.HasValue) { sql += " AND RecognizeTime <= @End"; ps.Add(new SQLiteParameter("@End", end.Value.AddDays(1))); }
                if (!string.IsNullOrWhiteSpace(plateFilter)) { sql += " AND PlateNumber LIKE @Plate"; ps.Add(new SQLiteParameter("@Plate", "%" + plateFilter.Trim() + "%")); }
                sql += " ORDER BY RecognizeTime DESC LIMIT 500";

                var dt = ExecuteQuery(sql, ps.ToArray());
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new PlateRecognitionRecord
                    {
                        Id = row["Id"].ToString(),
                        PlateNumber = row["PlateNumber"] == DBNull.Value ? null : row["PlateNumber"].ToString(),
                        Confidence = row["Confidence"] == DBNull.Value ? 0 : Convert.ToDouble(row["Confidence"]),
                        CameraName = row["CameraName"] == DBNull.Value ? null : row["CameraName"].ToString(),
                        CameraType = row["CameraType"] == DBNull.Value ? null : row["CameraType"].ToString(),
                        ImagePath = row["ImagePath"] == DBNull.Value ? null : row["ImagePath"].ToString(),
                        VehicleId = row["VehicleId"] == DBNull.Value ? null : row["VehicleId"].ToString(),
                        RecognizeTime = Convert.ToDateTime(row["RecognizeTime"]),
                        Source = row["Source"] == DBNull.Value ? "Auto" : row["Source"].ToString(),
                        Remark = row["Remark"] == DBNull.Value ? null : row["Remark"].ToString(),
                        CreateTime = Convert.ToDateTime(row["CreateTime"])
                    });
                }
            }
            catch { }
            return list;
        }

        #endregion
    }
}
