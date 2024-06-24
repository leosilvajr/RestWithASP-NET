document.addEventListener('DOMContentLoaded', () => {
    fetch('https://localhost:7138/api/person/v1')
        .then(response => response.json())
        .then(data => {
            const personList = document.getElementById('person-list');
            data.forEach(person => {
                const personCard = document.createElement('div');
                personCard.classList.add('person-card');

                const personName = document.createElement('h2');
                personName.textContent = `${person.firstName} ${person.lastName}`;
                
                const personAddress = document.createElement('p');
                personAddress.textContent = `Address: ${person.address}`;
                
                const personGender = document.createElement('p');
                personGender.textContent = `Gender: ${person.gender}`;

                personCard.appendChild(personName);
                personCard.appendChild(personAddress);
                personCard.appendChild(personGender);

                personList.appendChild(personCard);
            });
        })
        .catch(error => console.error('Error fetching data:', error));
});
