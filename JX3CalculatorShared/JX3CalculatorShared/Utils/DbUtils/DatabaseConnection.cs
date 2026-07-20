using MingYiTools;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JX3CalculatorShared.Utils.DbUtils
{
    public class DatabaseConnection
    {
        // 保存连接字符串，而不是连接对象
        private readonly string _connectionString;

        public string DbPath { get; }
        public SqlSugarClient Client { get; private set; } = null;

        public DatabaseConnection(string dbPath)
        {
            DbPath = dbPath;
            // 构建 SQLite 连接字符串
            _connectionString = $"Data Source={dbPath};Version=3;";
        }

        public void Connect()
        {
            Client = GetClient();
        }

        // 私有辅助方法：获取一个新的轻量级 SqlSugarClient 实例
        // IsAutoCloseConnection = true 确保了用完自动释放 SQLite 文件句柄，防止“文件被占用”错误
        private SqlSugarClient GetClient()
        {
            return new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = _connectionString,
                DbType = DbType.Sqlite,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute
            });
        }

        // ==========================================
        //  异步方法 (推荐在 WPF 生产环境使用)
        // ==========================================
        public async Task<List<T>> GetAllAsync<T>() where T : class, new()
        {
            // SqlSugar 的最佳实践：使用 using 块，用完即销毁
            using (var db = GetClient())
            {
                return await db.Queryable<T>().ToListAsync();
            }
        }

        // ==========================================
        //  同步方法 (方便调试、单元测试或后台线程使用)
        // ==========================================
        public List<T> GetAll<T>() where T : class, new()
        {
            using (var db = GetClient())
            {
                return db.Queryable<T>().ToList();
            }
        }

        // ==========================================
        //  条件查询 (支持 Lambda 表达式)
        // ==========================================
        // 注意：这里的参数类型我改为了 Expression<Func<T, bool>>
        // 原因：Func 是内存过滤（性能差），Expression 是数据库层面过滤（生成 SQL WHERE 子句，性能好）
        public List<T> Query<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            using (var db = GetClient())
            {
                return db.Queryable<T>().Where(predicate).ToList();
            }
        }

        // 补充：异步的条件查询
        public async Task<List<T>> QueryAsync<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            using (var db = GetClient())
            {
                return await db.Queryable<T>().Where(predicate).ToListAsync();
            }
        }

    }
}