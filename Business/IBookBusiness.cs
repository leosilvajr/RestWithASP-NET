using RestWithASPNET.Model;
using RestWithASPNETUdemy.Model;

namespace RestWithASPNET.Business
{
    public interface IBookBusiness
    {
        Book Create(Book book);
        Book FindByID(long id);
        List<Book> FindAll();
        Book Update(Book book);
        void Delete(int id);
    }
}
