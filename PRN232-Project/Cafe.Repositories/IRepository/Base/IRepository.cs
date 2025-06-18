using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.Repositories.IRepository.Base
{
    public interface IRepository<T> where T : class
    {
        // Lấy tất cả records
        List<T> GetAll();

        // Lấy records với filter, order và include
        List<T> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "");

        // Lấy record theo ID
        T GetById(object id);

        // Tìm record đầu tiên thỏa mãn điều kiện
        T Find(Expression<Func<T, bool>> predicate);

        // Lấy tất cả records thỏa mãn điều kiện
        List<T> FindAll(Expression<Func<T, bool>> predicate);

        // Thêm record mới
        void Add(T entity);

        // Thêm nhiều records
        void AddRange(IEnumerable<T> entities);

        // Cập nhật record
        void Update(T entity);

        // Xóa record
        void Remove(T entity);

        // Xóa nhiều records
        void RemoveRange(IEnumerable<T> entities);

        // Kiểm tra tồn tại
        bool Exists(Expression<Func<T, bool>> predicate);

        // Đếm số lượng
        int Count(Expression<Func<T, bool>> predicate = null);

        // Lưu thay đổi
        void SaveChanges();
    }
}
