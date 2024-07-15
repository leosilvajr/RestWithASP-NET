using RestWithASPNET.Data.Converter.Implementations;
using RestWithASPNET.Hypermedia.Utils;
using RestWithASPNETUdemy.Data.VO;
using RestWithASPNETUdemy.Model;
using RestWithASPNETUdemy.Repository;
using System.Collections.Generic;

namespace RestWithASPNETUdemy.Business.Implementations
{
    //Vamos ajustar a PersonBusiness para lidar com PesonVO, ou seja, vamos encapsular a logica da nossa aplicação.
    public class PersonBusiness : IPersonBusiness
    {
        private readonly IPersonRepository _repository;
        private readonly PersonConverter _converter;

        public PersonBusiness(IPersonRepository repository)
        {
            _repository = repository;
            _converter = new PersonConverter();
        }

        #region CREATE
        //Quando o objeto chega, ele é um VO então não da pra persistir na base de dados.
        public PersonVO Create(PersonVO person)
        {
            //Vamos parsear ele para a entidade Person, com isso podemos persistir
            var personEntity = _converter.Parse(person);
            personEntity = _repository.Create(personEntity);
            return _converter.Parse(personEntity);
        }
        #endregion

        #region READ
        public List<PersonVO> FindAll()
        {
            return _converter.Parse(_repository.FindAll());
        }

        public PersonVO FindByID(long id)
        {
            return _converter.Parse(_repository.FindByID(id));
        }

        public List<PersonVO> FindByName(string firstName, string lastName)
        {
            return _converter.Parse(_repository.FindByName(firstName, lastName));
        }
        public PagedSearchVO<PersonVO> FindWithPagedSearch(string name, string sortDirection, int pageSize, int page)
        {
            var sort = (!string.IsNullOrWhiteSpace(sortDirection)) && !sortDirection.Equals("desc") ? "asc" : "desc";
            var size = (pageSize < 1 ) ? 10 : pageSize;
            var offset = page > 0 ? (page -1) * size : 0;

            string query = @"SELECT * FROM person p WHERE 1 = 1";

            if (!string.IsNullOrWhiteSpace(name)) query = query + $" AND p.first_name like '%{name}%' ";

            query += $" ORDER BY p.first_name {sort} LIMIT {size} offset {offset}";

            var countQuery = @"SELECT COUNT(*) FROM person p WHERE 1 = 1";
            if (!string.IsNullOrWhiteSpace(name)) countQuery = countQuery + $" AND p.first_name like '%{name}%' ";

            var persons = _repository.FindWithPagedSearch(query);
            int totalResuts = _repository.GetCount(countQuery);
            return new PagedSearchVO<PersonVO>
            {
                CurrentPage = page,
                List = _converter.Parse(persons),
                PageSize = size,
                SortDirections = sort,
                TotalResults = totalResuts,
            };
        }
        #endregion

        #region UPDATE
        public PersonVO Update(PersonVO person)
        {
            var personEntity = _converter.Parse(person);
            personEntity = _repository.Update(personEntity);
            return _converter.Parse(_repository.Update(personEntity));
        }

        public PersonVO Disable(long id)
        {
            var personEntity = _repository.Disable(id);
            return _converter.Parse(personEntity);
        }
        #endregion

        #region DELETE
        public void Delete(long id)
        {
            _repository.Delete(id);
        }


        #endregion
    }
}
