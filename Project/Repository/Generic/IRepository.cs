using RestWithASPNET.Model.Base;

namespace RestWithASPNETUdemy.Repository
{
    public interface IRepository<T> where T : BaseEntity
    {
        #region CREATE
        T Create(T item);
        #endregion

        #region READ
        T FindByID(long id);
        List<T> FindAll();
        bool Exists(long id);
        List<T> FindWithPagedSearch(string query);
        int GetCount(string query);
        #endregion

        #region UPDATE
        T Update(T item);
        #endregion

        #region DELETE
        void Delete(long id);
        #endregion
    }

}
