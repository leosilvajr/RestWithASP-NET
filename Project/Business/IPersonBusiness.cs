using RestWithASPNET.Hypermedia.Utils;
using RestWithASPNETUdemy.Data.VO;

namespace RestWithASPNETUdemy.Business
{
    public interface IPersonBusiness
    {
        #region CREATE
        PersonVO Create(PersonVO person);
        #endregion

        #region READ
        PersonVO FindByID(long id);
        List<PersonVO> FindByName(string firstName, string lastName);
        List<PersonVO> FindAll();
        PagedSearchVO<PersonVO> FindWithPagedSearch(string name, string sortDirection, int pageSize, int page);
        #endregion

        #region UPDATE
        PersonVO Update(PersonVO person);
        PersonVO Disable(long id);
        #endregion

        #region DELETE
        void Delete(long id);
        #endregion
    }

}
