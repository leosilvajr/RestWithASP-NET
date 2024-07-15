//using RestWithASPNET.Model;
//using RestWithASPNETUdemy.Model.Context;

//namespace RestWithASPNET.Repository.Implementations
//{
//    public class BookRepositoryImplementation : IBookRepository
//    {
//        private MySQLContext _context;
//        public BookRepositoryImplementation(MySQLContext context)
//        {
//            _context = context;
//        }

//        public Book FindByID(long id)
//        {
//            return _context.Books.FirstOrDefault(p => p.Id.Equals(id));
//        }

//        public List<Book> FindAll()
//        {
//            return _context.Books.ToList();
//        }
//        public Book Create(Book book)
//        {
//            try
//            {
//                _context.Add(book);
//                _context.SaveChanges();
//            }
//            catch (Exception)
//            {
//                throw;
//            }
//            return book;
//        }
//        public Book Update(Book book)
//        {
//            if (!Exists(book.Id)) return null;

//            var result = _context.Persons.SingleOrDefault(p => p.Id.Equals(book.Id));

//            if (result != null)
//            {
//                try
//                {
//                    _context.Entry(result).CurrentValues.SetValues(book);
//                    _context.SaveChanges();
//                }
//                catch (Exception)
//                {

//                    throw;
//                }
//            }
//            return book;
//        }
//        public void Delete(long id)
//        {
//            var result = _context.Books.SingleOrDefault(p => p.Id.Equals(id));
//            if (result != null)
//            {
//                try
//                {
//                    _context.Books.Remove(result);
//                    _context.SaveChanges();
//                }
//                catch (Exception)
//                {

//                    throw;
//                }
//            }
//        }

//        public bool Exists(long id)
//        {
//            return _context.Books.Any(p => p.Id.Equals(id));
//        }

//    }
//}


////DAO - Data Acess Objetc - Implementado apenas pensando na base de dados.