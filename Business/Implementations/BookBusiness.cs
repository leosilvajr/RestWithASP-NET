using RestWithASPNET.Data.Converter.Implementations;
using RestWithASPNET.Data.VO;
using RestWithASPNET.Model;
using RestWithASPNETUdemy.Repository;

namespace RestWithASPNET.Business.Implementations
{
    public class BookBusiness : IBookBusiness
    {
        private readonly IRepository<Book> _repository;
        private readonly BookConverter converter;
        public BookBusiness(IRepository<Book> repository)
        {
            _repository = repository;
            converter = new BookConverter();
        }
        public List<BookVO> FindAll()
        {
            return converter.Parse(_repository.FindAll());
        }
        public BookVO FindByID(long id)
        {
            return converter.Parse(_repository.FindByID(id));
        }

        public BookVO Create(BookVO book)
        {
            var bookEntity = converter.Parse(book);
            bookEntity = _repository.Create(bookEntity);
            return converter.Parse(bookEntity);
        }
        public BookVO Update(BookVO book)
        {
            var bookEntity = converter.Parse(book);
            bookEntity = _repository.Update(bookEntity);
            return converter.Parse(bookEntity);
        }
        public void Delete(int id)
        {
             _repository.Delete(id);
        }

    }
}
